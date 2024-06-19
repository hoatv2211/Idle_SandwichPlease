using UnityEngine;
using System;
using System.Collections.Generic;
using NekoMart.Utils;

using UnityEngine.SceneManagement;

namespace NekoMart.Scene
{
    public class ScenePathList
    {
      
        public enum ShortCuts
        {
            None = 0,
            Home = 1,
            Gacha =2,
            Menu = 3,
            Back = 999,
        }

        private static string[] sortCutNames = new string[]
        {
            string.Empty,
           
        };


        static public string GetScenePath(ShortCuts target)
        {
            if (target == ShortCuts.Back)
            {
                return string.Empty;
            }

            return sortCutNames[(int)target];
        }

        static private Dictionary<string, ScenePathList.ShortCuts> apiSceneMap = new Dictionary<string, ScenePathList.ShortCuts>
        {
            {string.Empty, ShortCuts.Home},
        };

        public struct APISceneInfo
        {
            public string sceneName;
            public string scenePath;
            public string sceneParam;
        }

        public static APISceneInfo GetSceneInfo(string transition_screen_kind)
        {
            var sk = new APISceneInfo();

            if (string.IsNullOrEmpty(transition_screen_kind))
            {
                return sk;
            }

            ShortCuts shortCut;
            string[] param = transition_screen_kind.Split('/');

            if (apiSceneMap.ContainsKey(transition_screen_kind))
            {
                shortCut = apiSceneMap[transition_screen_kind];
            }
            else if (apiSceneMap.ContainsKey(param[0]))
            {
                shortCut = apiSceneMap[param[0]];
                sk.sceneParam = param.Length > 1 ? param[1] : null;
            }
            else
            {
                return sk;
            }

            sk.scenePath = GetScenePath(shortCut);

            return sk;
        }

        public const string Splash = "Splash";
        public const string Header = "Header";
        public const string Footer = "Footer";
        public const string Game = "Gameplay";
        public const string Home = "MainMenuScene";
        public const string MinterScene = "MinterScene";
        public const string FarmHouse = "FarmScene";
       
    }
}