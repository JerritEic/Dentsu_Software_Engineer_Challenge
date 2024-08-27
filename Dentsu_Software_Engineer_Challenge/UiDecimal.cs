using System.ComponentModel;

namespace Dentsu_Software_Engineer_Challenge;


/// <summary>
/// Helper class adding <see cref="PropertyChangedEventHandler"/> to a <see cref="decimal"/> value to allow
/// easier UI updates
/// </summary>
public class UiDecimal(decimal defaultValue) : INotifyPropertyChanged
{
    // Declare property changed event
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private decimal _value = defaultValue;
    public decimal Value
    {
        get => _value;
        set
        {
            if (value == _value)
                return;
            _value = value;
            OnPropertyChanged();
        }
    }
    // Invoke the PropertyChangedEvent
    private void OnPropertyChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
    }
}