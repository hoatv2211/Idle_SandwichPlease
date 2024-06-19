using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;


public class CSVImportPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {

            if (str.IndexOf("/map1.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                MapModel gm = AssetDatabase.LoadAssetAtPath<MapModel>(assetfile);
                if (gm == null)
                {
                    gm = new MapModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.mapObjects = CSVSerializer.Deserialize<DataMapProcess>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/map2.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                MapModel gm = AssetDatabase.LoadAssetAtPath<MapModel>(assetfile);
                if (gm == null)
                {
                    gm = new MapModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.mapObjects = CSVSerializer.Deserialize<DataMapProcess>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/PU_Speed.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("PU_Speed", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.PU_Speed = CSVSerializer.Deserialize<UpgradeData>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/PU_Profit.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("PU_Profit", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.PU_Profit = CSVSerializer.Deserialize<UpgradeData>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/PU_Capacity.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("PU_Capacity", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.PU_Capacity = CSVSerializer.Deserialize<UpgradeData>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/Staff_Capacity.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Staff_Capacity", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.Staff_Capacity = CSVSerializer.Deserialize<UpgradeData>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/Staff_Employ.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Staff_Employ", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.Staff_Employ = CSVSerializer.Deserialize<UpgradeData>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/Staff_Speed.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Staff_Speed", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.Staff_Speed = CSVSerializer.Deserialize<UpgradeData>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }


            if (str.IndexOf("/Units.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Units", "UnitModel");
                UnitModel gm = AssetDatabase.LoadAssetAtPath<UnitModel>(assetfile);
                if (gm == null)
                {
                    gm = new UnitModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.unitDataBases = CSVSerializer.Deserialize<UnitDataBase>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/Level_Process.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Level_Process", "UpgradeModel");
                UpgradeModel gm = AssetDatabase.LoadAssetAtPath<UpgradeModel>(assetfile);
                if (gm == null)
                {
                    gm = new UpgradeModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.levelProcesses = CSVSerializer.Deserialize<LevelProcess>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/Skins.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Skins", "SkinModel");
                SkinModel gm = AssetDatabase.LoadAssetAtPath<SkinModel>(assetfile);
                if (gm == null)
                {
                    gm = new SkinModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.infoSkins = CSVSerializer.Deserialize<InfoSkin>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            if (str.IndexOf("/IAP.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("IAP", "IAP");
                IAPModel gm = AssetDatabase.LoadAssetAtPath<IAPModel>(assetfile);
                if (gm == null)
                {
                    gm = new IAPModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.productIAP = CSVSerializer.Deserialize<ProductIAP>(data.text);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }


            if (str.IndexOf("/Level_User.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Level_User", "BattlePassModel");
                BattlePassModel gm = AssetDatabase.LoadAssetAtPath<BattlePassModel>(assetfile);
                if (gm == null)
                {
                    gm = new BattlePassModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.level_Users = CSVSerializer.Deserialize<Level_User>(data.text).ToList();

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();

            }

            if (str.IndexOf("/Battle_Rewards.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                assetfile = assetfile.Replace("Battle_Rewards", "BattlePassModel");
                BattlePassModel gm = AssetDatabase.LoadAssetAtPath<BattlePassModel>(assetfile);
                if (gm == null)
                {
                    gm = new BattlePassModel();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                gm.reward_Battles = CSVSerializer.Deserialize<Reward_Info>(data.text).ToList();

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();

            }


        }
    }
}
#endif