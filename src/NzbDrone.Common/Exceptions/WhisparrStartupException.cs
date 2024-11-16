using System;

namespace NzbDrone.Common.Exceptions
{
    public class LunarrStartupException : NzbDroneException
    {
        public LunarrStartupException(string message, params object[] args)
            : base("Lunarr failed to start: " + string.Format(message, args))
        {
        }

        public LunarrStartupException(string message)
            : base("Lunarr failed to start: " + message)
        {
        }

        public LunarrStartupException()
            : base("Lunarr failed to start")
        {
        }

        public LunarrStartupException(Exception innerException, string message, params object[] args)
            : base("Lunarr failed to start: " + string.Format(message, args), innerException)
        {
        }

        public LunarrStartupException(Exception innerException, string message)
            : base("Lunarr failed to start: " + message, innerException)
        {
        }

        public LunarrStartupException(Exception innerException)
            : base("Lunarr failed to start: " + innerException.Message)
        {
        }
    }
}
