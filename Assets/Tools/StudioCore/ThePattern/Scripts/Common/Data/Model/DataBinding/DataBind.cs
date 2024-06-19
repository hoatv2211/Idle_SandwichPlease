using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ThePattern
{
    [Serializable]
    public class DataBind<T> : IDataModel, ISerializable
    {
        #region Variables and Properties
        private T _data = default(T);                                       // data value
        [NonSerialized]
        private List<Action<T>> OnChanged = new List<Action<T>>();          // list subriber method
        [NonSerialized]
        private readonly object _lock = new object();                       // object for lock
        public T Value 
        { 
            get => _data; 
            set 
            {
                if(object.Equals((object) _data, (object) value))
                    return;
                SetValueAndForceNotify(value);
            }
        }
        #endregion

        #region Constructors
        public DataBind() : this(default(T))
        {

        }
        public DataBind(T initialValue)
        {
            this.SetValueOnly(initialValue);
        }
        protected DataBind(SerializationInfo info, StreamingContext ctxt)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }
            foreach (PropertyInfo property in GetProperties())
            {
                try
                {
                    property.SetValue((object)this, info.GetValue(property.Name, property.PropertyType));
                }
                catch (SerializationException ex)
                {
                    // Log.Warning((object)("Can't Serialization: " + property.Name), 0);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// For Get All PropertyInfos of this
        /// </summary>
        /// <returns>PropertyInfo Array</returns>
        private PropertyInfo[] GetProperties()
        {
            return this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// For Set Data and Force Notify
        /// </summary>
        /// <param name="value"></param>
        public void SetValueAndForceNotify(T value)
        {
            this.SetValueOnly(value);
            this.ForceNotify();
        }

        /// <summary>
        /// For Set Data Only
        /// </summary>
        /// <param name="value"></param>
        public void SetValueOnly(T value)
        {
            this._data = value;
        }

        /// <summary>
        /// For Force Notify to All Listener
        /// </summary>
        private void ForceNotify()
        {
            lock (_lock)
            {
                for (int index = OnChanged.Count - 1; index >= 0; --index)
                {
                    try
                    {
                        if (!(OnChanged[index].Target == null))
                            OnChanged[index].Invoke(_data);
                        else
                            OnChanged.RemoveAt(index);
                    }
                    catch
                    {
                        if (index >= 0 && index < this.OnChanged.Count)
                            OnChanged.RemoveAt(index);
                    }
                }
            }
        }

        /// <summary>
        /// For Listener Register to Get Notify on Data Changed and Invoke Event onChanged (just Registed) Immediately
        /// </summary>
        /// <param name="onChanged"></param>
        public void RegisterNotify(Action<T> onChanged)
        {
            this.RegisterNotifyOnChanged(onChanged);
            onChanged?.Invoke(Value);
        }

        /// <summary>
        /// For Listener Register to Get Notify on Data Changed
        /// </summary>
        /// <param name="onChanged"></param>
        public void RegisterNotifyOnChanged(Action<T> onChanged)
        {
            if (this.OnChanged.Contains(onChanged))
                return;
            lock (_lock)
                this.OnChanged.Add(onChanged);
        }
        #endregion

        #region Override Methods
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(string.Empty);
            }
            foreach (PropertyInfo property in GetProperties())
            {   
                info.AddValue(property.Name, property.GetValue((object)this), property.PropertyType);
            }
        }

        public override string ToString()
        {
            return _data.ToString();
        }
        #endregion
    }
}

