using AudioTools;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioSelector
{
    internal class AudioSelectorViewModel/* : INotifyPropertyChanged*/
    {
        public ObservableCollection<MultiMediaDevice> Devices { get; set; }

        // It is reserved for future use.
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        //    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
