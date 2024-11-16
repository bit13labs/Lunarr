using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Processes;
using NzbDrone.Core.HealthCheck;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Movies;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Notifications.CustomScript
{
    public class CustomScript : NotificationBase<CustomScriptSettings>
    {
        private readonly IDiskProvider _diskProvider;
        private readonly IProcessProvider _processProvider;
        private readonly Logger _logger;

        public CustomScript(IDiskProvider diskProvider, IProcessProvider processProvider, Logger logger)
        {
            _diskProvider = diskProvider;
            _processProvider = processProvider;
            _logger = logger;
        }

        public override string Name => "Custom Script";

        public override string Link => "https://wiki.servarr.com/lunarr/settings#connections";

        public override ProviderMessage Message => new ProviderMessage("Testing will execute the script with the EventType set to Test, ensure your script handles this correctly", ProviderMessageType.Warning);

        public override void OnGrab(GrabMessage message)
        {
            var movie = message.Movie;
            var remoteMovie = message.RemoteMovie;
            var quality = message.Quality;
            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "Grab");
            environmentVariables.Add("Lunarr_Movie_Id", movie.Id.ToString());
            environmentVariables.Add("Lunarr_Movie_Title", movie.Title);
            environmentVariables.Add("Lunarr_Movie_Year", movie.Year.ToString());
            environmentVariables.Add("Lunarr_Movie_ImdbId", movie.ImdbId ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_TmdbId", movie.TmdbId.ToString());
            environmentVariables.Add("Lunarr_Movie_In_Cinemas_Date", movie.MovieMetadata.Value.InCinemas.ToString() ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_Physical_Release_Date", movie.MovieMetadata.Value.PhysicalRelease.ToString() ?? string.Empty);
            environmentVariables.Add("Lunarr_Release_Title", remoteMovie.Release.Title);
            environmentVariables.Add("Lunarr_Release_Indexer", remoteMovie.Release.Indexer ?? string.Empty);
            environmentVariables.Add("Lunarr_Release_Size", remoteMovie.Release.Size.ToString());
            environmentVariables.Add("Lunarr_Release_ReleaseGroup", remoteMovie.ParsedMovieInfo.ReleaseGroup ?? string.Empty);
            environmentVariables.Add("Lunarr_Release_Quality", quality.Quality.Name);
            environmentVariables.Add("Lunarr_Release_QualityVersion", quality.Revision.Version.ToString());
            environmentVariables.Add("Lunarr_IndexerFlags", remoteMovie.Release.IndexerFlags.ToString());
            environmentVariables.Add("Lunarr_Download_Client", message.DownloadClientName ?? string.Empty);
            environmentVariables.Add("Lunarr_Download_Client_Type", message.DownloadClientType ?? string.Empty);
            environmentVariables.Add("Lunarr_Download_Id", message.DownloadId ?? string.Empty);

            ExecuteScript(environmentVariables);
        }

        public override void OnDownload(DownloadMessage message)
        {
            var movie = message.Movie;
            var movieFile = message.MovieFile;
            var sourcePath = message.SourcePath;
            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "Download");
            environmentVariables.Add("Lunarr_IsUpgrade", message.OldMovieFiles.Any().ToString());
            environmentVariables.Add("Lunarr_Movie_Id", movie.Id.ToString());
            environmentVariables.Add("Lunarr_Movie_Title", movie.Title);
            environmentVariables.Add("Lunarr_Movie_Year", movie.Year.ToString());
            environmentVariables.Add("Lunarr_Movie_Path", movie.Path);
            environmentVariables.Add("Lunarr_Movie_ImdbId", movie.ImdbId ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_TmdbId", movie.TmdbId.ToString());
            environmentVariables.Add("Lunarr_Movie_In_Cinemas_Date", movie.MovieMetadata.Value.InCinemas.ToString() ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_Physical_Release_Date", movie.MovieMetadata.Value.PhysicalRelease.ToString() ?? string.Empty);
            environmentVariables.Add("Lunarr_MovieFile_Id", movieFile.Id.ToString());
            environmentVariables.Add("Lunarr_MovieFile_RelativePath", movieFile.RelativePath);
            environmentVariables.Add("Lunarr_MovieFile_Path", Path.Combine(movie.Path, movieFile.RelativePath));
            environmentVariables.Add("Lunarr_MovieFile_Quality", movieFile.Quality.Quality.Name);
            environmentVariables.Add("Lunarr_MovieFile_QualityVersion", movieFile.Quality.Revision.Version.ToString());
            environmentVariables.Add("Lunarr_MovieFile_ReleaseGroup", movieFile.ReleaseGroup ?? string.Empty);
            environmentVariables.Add("Lunarr_MovieFile_SceneName", movieFile.SceneName ?? string.Empty);
            environmentVariables.Add("Lunarr_MovieFile_SourcePath", sourcePath);
            environmentVariables.Add("Lunarr_MovieFile_SourceFolder", Path.GetDirectoryName(sourcePath));
            environmentVariables.Add("Lunarr_Download_Client", message.DownloadClientInfo?.Name ?? string.Empty);
            environmentVariables.Add("Lunarr_Download_Client_Type", message.DownloadClientInfo?.Type ?? string.Empty);
            environmentVariables.Add("Lunarr_Download_Id", message.DownloadId ?? string.Empty);

            if (message.OldMovieFiles.Any())
            {
                environmentVariables.Add("Lunarr_DeletedRelativePaths", string.Join("|", message.OldMovieFiles.Select(e => e.RelativePath)));
                environmentVariables.Add("Lunarr_DeletedPaths", string.Join("|", message.OldMovieFiles.Select(e => Path.Combine(movie.Path, e.RelativePath))));
            }

            ExecuteScript(environmentVariables);
        }

        public override void OnMovieRename(Movie movie, List<RenamedMovieFile> renamedFiles)
        {
            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "Rename");
            environmentVariables.Add("Lunarr_Movie_Id", movie.Id.ToString());
            environmentVariables.Add("Lunarr_Movie_Title", movie.Title);
            environmentVariables.Add("Lunarr_Movie_Year", movie.Year.ToString());
            environmentVariables.Add("Lunarr_Movie_Path", movie.Path);
            environmentVariables.Add("Lunarr_Movie_ImdbId", movie.ImdbId ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_TmdbId", movie.TmdbId.ToString());
            environmentVariables.Add("Lunarr_Movie_In_Cinemas_Date", movie.MovieMetadata.Value.InCinemas.ToString() ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_Physical_Release_Date", movie.MovieMetadata.Value.PhysicalRelease.ToString() ?? string.Empty);
            environmentVariables.Add("Lunarr_MovieFile_Ids", string.Join(",", renamedFiles.Select(e => e.MovieFile.Id)));
            environmentVariables.Add("Lunarr_MovieFile_RelativePaths", string.Join("|", renamedFiles.Select(e => e.MovieFile.RelativePath)));
            environmentVariables.Add("Lunarr_MovieFile_Paths", string.Join("|", renamedFiles.Select(e => e.MovieFile.Path)));
            environmentVariables.Add("Lunarr_MovieFile_PreviousRelativePaths", string.Join("|", renamedFiles.Select(e => e.PreviousRelativePath)));
            environmentVariables.Add("Lunarr_MovieFile_PreviousPaths", string.Join("|", renamedFiles.Select(e => e.PreviousPath)));

            ExecuteScript(environmentVariables);
        }

        public override void OnMovieFileDelete(MovieFileDeleteMessage deleteMessage)
        {
            var movie = deleteMessage.Movie;
            var movieFile = deleteMessage.MovieFile;

            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "MovieFileDelete");
            environmentVariables.Add("Lunarr_MovieFile_DeleteReason", deleteMessage.Reason.ToString());
            environmentVariables.Add("Lunarr_Movie_Id", movie.Id.ToString());
            environmentVariables.Add("Lunarr_Movie_Title", movie.Title);
            environmentVariables.Add("Lunarr_Movie_Year", movie.Year.ToString());
            environmentVariables.Add("Lunarr_Movie_Path", movie.Path);
            environmentVariables.Add("Lunarr_Movie_ImdbId", movie.ImdbId ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_TmdbId", movie.TmdbId.ToString());
            environmentVariables.Add("Lunarr_MovieFile_Id", movieFile.Id.ToString());
            environmentVariables.Add("Lunarr_MovieFile_RelativePath", movieFile.RelativePath);
            environmentVariables.Add("Lunarr_MovieFile_Path", Path.Combine(movie.Path, movieFile.RelativePath));
            environmentVariables.Add("Lunarr_MovieFile_Size", movieFile.Size.ToString());
            environmentVariables.Add("Lunarr_MovieFile_Quality", movieFile.Quality.Quality.Name);
            environmentVariables.Add("Lunarr_MovieFile_QualityVersion", movieFile.Quality.Revision.Version.ToString());
            environmentVariables.Add("Lunarr_MovieFile_ReleaseGroup", movieFile.ReleaseGroup ?? string.Empty);
            environmentVariables.Add("Lunarr_MovieFile_SceneName", movieFile.SceneName ?? string.Empty);

            ExecuteScript(environmentVariables);
        }

        public override void OnMovieDelete(MovieDeleteMessage deleteMessage)
        {
            var movie = deleteMessage.Movie;
            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "MovieDelete");
            environmentVariables.Add("Lunarr_Movie_Id", movie.Id.ToString());
            environmentVariables.Add("Lunarr_Movie_Title", movie.Title);
            environmentVariables.Add("Lunarr_Movie_Year", movie.Year.ToString());
            environmentVariables.Add("Lunarr_Movie_Path", movie.Path);
            environmentVariables.Add("Lunarr_Movie_ImdbId", movie.ImdbId ?? string.Empty);
            environmentVariables.Add("Lunarr_Movie_TmdbId", movie.TmdbId.ToString());
            environmentVariables.Add("Lunarr_Movie_DeletedFiles", deleteMessage.DeletedFiles.ToString());

            if (deleteMessage.DeletedFiles && movie.MovieFile != null)
            {
                environmentVariables.Add("Lunarr_Movie_Folder_Size", movie.MovieFile.Size.ToString());
            }

            ExecuteScript(environmentVariables);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "HealthIssue");
            environmentVariables.Add("Lunarr_Health_Issue_Level", Enum.GetName(typeof(HealthCheckResult), healthCheck.Type));
            environmentVariables.Add("Lunarr_Health_Issue_Message", healthCheck.Message);
            environmentVariables.Add("Lunarr_Health_Issue_Type", healthCheck.Source.Name);
            environmentVariables.Add("Lunarr_Health_Issue_Wiki", healthCheck.WikiUrl.ToString() ?? string.Empty);

            ExecuteScript(environmentVariables);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var environmentVariables = new StringDictionary();

            environmentVariables.Add("Lunarr_EventType", "ApplicationUpdate");
            environmentVariables.Add("Lunarr_Update_Message", updateMessage.Message);
            environmentVariables.Add("Lunarr_Update_NewVersion", updateMessage.NewVersion.ToString());
            environmentVariables.Add("Lunarr_Update_PreviousVersion", updateMessage.PreviousVersion.ToString());

            ExecuteScript(environmentVariables);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            if (!_diskProvider.FileExists(Settings.Path))
            {
                failures.Add(new NzbDroneValidationFailure("Path", "File does not exist"));
            }

            foreach (var systemFolder in SystemFolders.GetSystemFolders())
            {
                if (systemFolder.IsParentPath(Settings.Path))
                {
                    failures.Add(new NzbDroneValidationFailure("Path", $"Must not be a descendant of '{systemFolder}'"));
                }
            }

            if (failures.Empty())
            {
                try
                {
                    var environmentVariables = new StringDictionary();
                    environmentVariables.Add("Lunarr_EventType", "Test");

                    var processOutput = ExecuteScript(environmentVariables);

                    if (processOutput.ExitCode != 0)
                    {
                        failures.Add(new NzbDroneValidationFailure(string.Empty, $"Script exited with code: {processOutput.ExitCode}"));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    failures.Add(new NzbDroneValidationFailure(string.Empty, ex.Message));
                }
            }

            return new ValidationResult(failures);
        }

        private ProcessOutput ExecuteScript(StringDictionary environmentVariables)
        {
            _logger.Debug("Executing external script: {0}", Settings.Path);

            var processOutput = _processProvider.StartAndCapture(Settings.Path, Settings.Arguments, environmentVariables);

            _logger.Debug("Executed external script: {0} - Status: {1}", Settings.Path, processOutput.ExitCode);
            _logger.Debug("Script Output: \r\n{0}", string.Join("\r\n", processOutput.Lines));

            return processOutput;
        }

        private bool ValidatePathParent(string possibleParent, string path)
        {
            return possibleParent.IsParentPath(path);
        }
    }
}
