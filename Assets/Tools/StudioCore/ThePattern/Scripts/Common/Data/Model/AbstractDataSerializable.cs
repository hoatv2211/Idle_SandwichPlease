using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

using ThePattern.Utils;
using ThePattern.Common.Injection;

namespace ThePattern.Model
{
    // TODO: Missing Compress Type: Gzip, 7Zip => add it in future
    public class AbstractDataSerializable<T> : CollectionBase, ISerializable where T : AbstractDataSerializable<T>, new()
    {
        protected const string VERSION_NAME_KEY = "PatternSave";

        private static readonly Dictionary<Type, SerializableOption> _mappingOption = new Dictionary<Type, SerializableOption>();

        public static T Instance
        {
            get
            {
                object instance;
                if (!_instances.TryGetValue(typeof(T), out instance)) // If can't get Value in colection instance
                {
                    instance = AbstractDataSerializable<T>.Deserialize(); // Try Deserialize
                    _instances.Add(typeof(T), instance); // Add in collection instance
                    ((T)instance).Init((T)instance); // call Init Method;
                }
                return (T)instance;
            }
        }
        internal static SerializableOption Option
        {
            get
            {
                if (!_mappingOption.ContainsKey(typeof(T)))
                    _mappingOption[typeof(T)] = new SerializableOption();
                return _mappingOption[typeof(T)];
            }
        }
        private FieldInfo[] SerializationFields() => this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        private static string FilePath => Path.Combine(PatternInjection.Configuration.CachePath, AbstractDataSerializable<T>.Option == null || string.IsNullOrWhiteSpace(AbstractDataSerializable<T>.Option.FileName) ? string.Format("{0}.esd", (object)StringUtils.CreateMD5(typeof(T).Name).ToLower()) : AbstractDataSerializable<T>.Option.FileName);


        protected AbstractDataSerializable() => this.AutomaticInitData(this.SerializationFields());
        protected AbstractDataSerializable(SerializationInfo info, StreamingContext context)
        {
            FieldInfo[] fieldInfos = this.SerializationFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                try
                {
                    fieldInfo.SetValue(this, info.GetValue(fieldInfo.Name, fieldInfo.FieldType));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogWarning(string.Format("Deserialize Exception class type:'{0}' field: '{1}'", this.GetType().Name, fieldInfo.Name));
                }
            }
            int oldVersion = 0;
            try
            {
                oldVersion = (int)info.GetValue(VERSION_NAME_KEY, typeof(int));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogWarning($"SerializationInfo '{VERSION_NAME_KEY}' not exits! Set default old version: 0");
            }
            this.OnLoadVersion(oldVersion, info);
            this.AutomaticInitData(fieldInfos);
        }

        public static void SetOption(SerializableOption option) => _mappingOption[typeof(T)] = option;

        private void Init(T instance) => Initialize();
        protected virtual void Initialize() { }
        protected virtual void AutomaticInitData(FieldInfo[] fieldInfos)
        {
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetValue(this) == null)
                    fieldInfo.SetValue(this, Activator.CreateInstance(fieldInfo.FieldType));
            }
        }
        protected virtual void OnLoadVersion(int oldVersion, SerializationInfo oldInfo)
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            FieldInfo[] fieldInfoArray = this.SerializationFields();
            if ((uint)fieldInfoArray.Length > 0U)
            {
                foreach (FieldInfo fieldInfo in fieldInfoArray)
                    info.AddValue(fieldInfo.Name, fieldInfo.GetValue(Instance));
            }
            info.AddValue(VERSION_NAME_KEY, Option.Version);
        }

        public void Serializable()
        {
            Stream serializationStream = (Stream)null;
            try
            {
                using (serializationStream = (Stream)File.Open(AbstractDataSerializable<T>.FilePath, FileMode.OpenOrCreate))
                {
                    new BinaryFormatter().Serialize(serializationStream, this);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                serializationStream?.Close();
            }
        }

        private static T Deserialize()
        {
            string filePath = AbstractDataSerializable<T>.FilePath;
            if (File.Exists(filePath))
            {
                    Stream serializationStream = (Stream)null;
                    try
                    {
                        serializationStream = (Stream)File.Open(filePath, FileMode.Open);
                        return (T)new BinaryFormatter().Deserialize(serializationStream);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                    finally
                    {
                        serializationStream?.Close();
                    }
            }
            return new T();
        }


        public void DeleteCache()
        {
            this.Reset();
            string filePath = AbstractDataSerializable<T>.FilePath;
            if (!File.Exists(filePath))
                return;
            File.Delete(filePath);
        }
        
        public static void DeleteAllSerializable()
        {
            foreach (AbstractDataSerializable<T> dataSerializable in typeof(AbstractDataSerializable<T>).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AbstractDataSerializable<T>)) && !t.IsAbstract).Select(t => (AbstractDataSerializable<T>)Activator.CreateInstance(t)))
                dataSerializable.DeleteCache();
        }
        protected virtual void Reset()
        {
            foreach (FieldInfo serializationField in SerializationFields())
                serializationField.SetValue(this, null);
        }
    }
}

