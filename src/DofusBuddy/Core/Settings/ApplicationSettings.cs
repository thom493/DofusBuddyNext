using System.Collections.Generic;

namespace DofusBuddy.Core.Settings
{
    public class ApplicationSettings
    {
        public List<CharacterSettings> Characters { get; set; } = new List<CharacterSettings>();

        public WindowPositionSettings? WindowPosition { get; set; }

        public FeaturesSettings Features { get; set; } = new FeaturesSettings();
    }
}
