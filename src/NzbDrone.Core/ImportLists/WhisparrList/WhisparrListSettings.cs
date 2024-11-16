using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.ImportLists.LunarrList
{
    public class LunarrSettingsValidator : AbstractValidator<LunarrListSettings>
    {
        public LunarrSettingsValidator()
        {
            RuleFor(c => c.Url).ValidRootUrl();
        }
    }

    public class LunarrListSettings : IProviderConfig
    {
        private static readonly LunarrSettingsValidator Validator = new LunarrSettingsValidator();

        [FieldDefinition(0, Label = "List URL", HelpText = "The URL for the movie list")]
        public string Url { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
