using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.SimpleMVVM
{
    #region Abstraction
   
    public interface IReadProperty<T> : IReadProperty
    {
        delegate void ValueChangedDelegate(T oldValue, T newValue);
        
        event ValueChangedDelegate ValueChanged;

        T GetValue();
    }
    
    public interface IReadProperty
    {
        delegate void BoxedChangedDelegate(object oldValue, object newValue);
        
        event BoxedChangedDelegate BoxedValueChanged;

        object GetBoxedValue();
    }
    
    public interface IWriteProperty<in T> : IWriteProperty
    {
        void SetValue(T value);
    }
    
    public interface IWriteProperty
    {
        void SetBoxedValue(object value);
    }
    
    public interface IReadWriteProperty<T> : IReadWriteProperty, IReadProperty<T>, IWriteProperty<T>
    {
        T Value
        {
            get => GetValue();
            set => SetValue(value);
        }   
    }
    
    public interface IReadWriteProperty : IReadProperty, IWriteProperty
    {
        object BoxedValue
        {
            get => GetBoxedValue();
            set => SetBoxedValue(value);
        }  
    }
    
    #endregion

    #region Surrogates
    
    /// <summary>
    /// Just the wrapper around value, upon creation of value will be captured 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct StaticValue<T> : IReadProperty<T>
    {
        private readonly T _value;
        public event IReadProperty.BoxedChangedDelegate BoxedValueChanged { add { } remove { } }

        public event IReadProperty<T>.ValueChangedDelegate ValueChanged { add { } remove { } }
        
        public StaticValue(T value) => _value = value;
        
        public object GetBoxedValue() => GetValue();

        public T GetValue() => _value;

        public static implicit operator StaticValue<T>(T value) => new(value);
    }
    
    /// <summary>
    /// Value getter that is defined elsewhere
    /// </summary>
    public readonly struct DynamicReadValue<T> : IReadProperty<T>
    {
        private readonly Func<T> _getter;
        
        public event IReadProperty.BoxedChangedDelegate BoxedValueChanged { add { } remove { } }
        
        public event IReadProperty<T>.ValueChangedDelegate ValueChanged { add { } remove { } }

        public DynamicReadValue(Func<T> getter) => _getter = getter ?? DefaultGetter;

        public object GetBoxedValue() => GetValue();

        public T GetValue() => _getter();

        private static T DefaultGetter() => throw new Exception("No getter defined!");
    }
    
    /// <summary>
    /// Value setter that is defined elsewhere
    /// </summary>
    public readonly struct DynamicWriteValue<T> : IWriteProperty<T>
    {
        private readonly Action<T> _setter;

        public DynamicWriteValue(Action<T> setter) => _setter = setter ?? DefaultSetter;

        public void SetValue(T value) => _setter(value);

        public void SetBoxedValue(object value) => SetValue((T)value);
        
        private static void DefaultSetter(T value) => throw new Exception("No setter defined!");
    }


    public readonly struct DynamicReadWriteValue<T> : IReadWriteProperty<T>
    {
        private readonly DynamicReadValue<T> _read;
        private readonly DynamicWriteValue<T> _write;
        
        public event IReadProperty.BoxedChangedDelegate BoxedValueChanged
        {
            add => _read.BoxedValueChanged += value;
            remove => _read.BoxedValueChanged -= value;
        }
        public event IReadProperty<T>.ValueChangedDelegate ValueChanged
        {
            add => _read.ValueChanged += value;
            remove => _read.ValueChanged -= value;
        }

        public DynamicReadWriteValue(Func<T> read, Action<T> write)
        {
            _read = new DynamicReadValue<T>(read);
            _write = new DynamicWriteValue<T>(write);
        }

        public object GetBoxedValue() => _read.GetBoxedValue();
        public void SetBoxedValue(object value) => _write.SetBoxedValue(value);

        public T GetValue() => _read.GetValue();
        public void SetValue(T value) => _write.SetValue(value);
        
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public object BoxedValue
        {
            get => GetBoxedValue();
            set => SetBoxedValue(value);
        }
        
        public override string ToString() => Value + " " + GetType().Name;

        public static implicit operator T(DynamicReadWriteValue<T> property) => property.Value;
    }
    
    #endregion
    
    [Serializable, InlineProperty]
    public sealed class Property<T> : Property, IReadWriteProperty<T>, ISerializationCallbackReceiver
    {
        [SerializeField, HideLabel] private T _value;

        public event IReadProperty.BoxedChangedDelegate BoxedValueChanged;
        public event IReadProperty<T>.ValueChangedDelegate ValueChanged;
        
        public Property() : this(default, null) { }

        public Property(IReadProperty<T>.ValueChangedDelegate onValueChanged) : this(default, onValueChanged) { }

        public Property(T value, IReadProperty<T>.ValueChangedDelegate onValueChanged = null)
        {
            _value = value;
            if (onValueChanged != null) ValueChanged += onValueChanged;
        }

        public object GetBoxedValue() => GetValue();
        public void SetBoxedValue(object value) => SetValue((T)value);
        
        public T GetValue() => _value;
        public void SetValue(T value)
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;

            var oldValue = _value;
            _value = value;
            InvokeUpdate(oldValue);
        }
        
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public object BoxedValue
        {
            get => GetBoxedValue();
            set => SetBoxedValue(value);
        }

        private void InvokeUpdate(T oldValue)
        {
            ValueChanged?.Invoke(oldValue, Value);
            BoxedValueChanged?.Invoke(oldValue, Value);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() => InvokeUpdate(_value);
        
        public override string ToString() => Value + " " + GetType().Name;

        public static implicit operator T(Property<T> property) => property.Value;
        public static implicit operator Property<T>(T value) => new(value);
    }

    public abstract class Property
    {
        
    }
}