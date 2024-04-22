using Game.Scripts.SimpleMVVM;
using UnityEngine;

namespace Game.Saving
{
    /// <summary>
    /// Access point for persistent data.
    /// Instance of data is loaded at application start, all other APIs modifies this instance, afterwards saving.
    /// </summary>
    public static class GameState
    {
        public const string KEY = "Game.SerializedState";

        public static SerializedGameData SerializedData { get; } = new();
        public static RuntimeGameData RuntimeData { get; } = new();
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Load();
        }

        public static void Load()
        {
            var json = PlayerPrefs.GetString(KEY);
            JsonUtility.FromJsonOverwrite(json, SerializedData);
            RuntimeData.Read(SerializedData);
        }
        
        public static void Save()
        {
            RuntimeData.Write(SerializedData);
            var json = JsonUtility.ToJson(SerializedData, false);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }
    }
}