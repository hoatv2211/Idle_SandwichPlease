using UnityEngine;
using UnityEditor;
using ThePattern.Utils;

namespace ThePattern.Editor
{
    public static class ThePatternMenu
    {
        //[MenuItem("ThePattern/Save File/Delete Save")]
        public static void DeleteAllSave() {
            FileUtils.EmptyDirectory(Application.persistentDataPath);
            PlayerPrefs.DeleteAll();

        }
    }
}

