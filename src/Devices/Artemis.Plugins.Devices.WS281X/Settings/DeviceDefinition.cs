using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Artemis.Plugins.Devices.WS281X.Settings
{
    public class DeviceDefinition : AbstractBindable
    {
        private string _name;
        private string _port;
        private string _hostname;
        private int _speed = 115200;
        private DeviceDefinitionType _type;
        
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public DeviceDefinitionType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Hostname
        {
            get => _hostname;
            set => SetProperty(ref _hostname, value);
        }
        public int Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }
    }

    public enum DeviceDefinitionType
    {
        Arduino,
        Bitwizard,
        ESP8266
    }


    /// <inheritdoc />
    /// <summary>
    ///     Represents a basic bindable class which notifies when a property value changes.
    /// </summary>
    public abstract class AbstractBindable : IBindable
    {
        #region Events

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        ///     Checks if the property already matches the desirec value or needs to be updated.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to the backing-filed.</param>
        /// <param name="value">Value to apply.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool RequiresUpdate<T>(ref T storage, T value)
        {
            return !Equals(storage, value);
        }

        /// <summary>
        ///     Checks if the property already matches the desired value and updates it if not.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to the backing-filed.</param>
        /// <param name="value">Value to apply.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This value is optional
        ///     and can be provided automatically when invoked from compilers that support <see cref="CallerMemberNameAttribute" />
        ///     .
        /// </param>
        /// <returns><c>true</c> if the value was changed, <c>false</c> if the existing value matched the desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!RequiresUpdate(ref storage, value)) return false;

            storage = value;
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Triggers the <see cref="PropertyChanged" />-event when a a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This value is optional
        ///     and can be provided automatically when invoked from compilers that support <see cref="CallerMemberNameAttribute" />
        ///     .
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    ///     Represents a basic bindable class which notifies when a property value changes.
    /// </summary>
    public interface IBindable : INotifyPropertyChanged
    {
    }
}