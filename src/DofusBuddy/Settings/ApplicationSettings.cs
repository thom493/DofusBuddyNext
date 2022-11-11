using System.Collections.Generic;

namespace DofusBuddy.Settings
{
    public class ApplicationSettings
    {
        public List<Character> Characters { get; set; } = new List<Character>();

        public WindowPosition WindowPosition { get; set; }

        public Features Features { get; set; } = new Features();
    }
}
