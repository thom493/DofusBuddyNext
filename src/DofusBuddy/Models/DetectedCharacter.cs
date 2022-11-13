using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Models
{
    public class DetectedCharcter : ObservableObject
    {
        public DetectedCharcter(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
