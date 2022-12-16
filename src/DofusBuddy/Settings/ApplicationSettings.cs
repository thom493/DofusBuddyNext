using System.Collections.Generic;

namespace DofusBuddy.Settings
{
    public class ApplicationSettings
    {
        public List<CharacterSettings> Characters { get; set; } = new List<CharacterSettings>();

        public WindowPositionSettings? WindowPosition { get; set; }

        public FeaturesSettings Features { get; set; } = new FeaturesSettings();

        public PositionSettings Positions { get; set; } = new PositionSettings();
    }
}
