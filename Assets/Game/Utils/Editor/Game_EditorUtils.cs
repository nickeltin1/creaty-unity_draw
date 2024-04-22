using UnityEditor;
using UnityEngine;

namespace Game.Utils
{
    internal static class Game_EditorUtils
    {
        [MenuItem("Utils/Reveal persistentDataPath")]
        private static void OpenDataPath()
        {
            var path = Application.persistentDataPath;
            EditorUtility.RevealInFinder(path);
        }
    }
}