using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolKits
{
    public class MissingPrefabChecker
    {
        private const string PATH = "Assets";
        private const string PrefabNameColor = "#00FF00";
        private const string SubPrefabNameColor = "#FF0000";

        [MenuItem("Tools/查找丢失prefab", priority = 1)]
        private static void CheckMissingPrefab()
        {
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] { PATH });
            var length = guids.Length;
            var index = 1;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                EditorUtility.DisplayProgressBar("玩命检查中", "玩命检查中..." + path, (float)index / length);
                FindMissingPrefabRecursive(prefab, prefab.name, true);
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "检查prefab结束", "OK");
        }

        static void FindMissingPrefabRecursive(GameObject gameObject, string prefabName, bool isRoot)
        {
            if (gameObject.name.Contains("Missing Prefab"))
            {
                Debug.LogError(
                    $"<color={PrefabNameColor}>{prefabName}</color> has missing prefab <color={SubPrefabNameColor}>{gameObject.name}</color>");
                return;
            }

            if (PrefabUtility.IsPrefabAssetMissing(gameObject))
            {
                Debug.LogError(
                    $"<color={PrefabNameColor}>{prefabName}</color> has missing prefab <color={SubPrefabNameColor}>{gameObject.name}</color>");
                return;
            }

            if (PrefabUtility.IsDisconnectedFromPrefabAsset(gameObject))
            {
                Debug.LogError(
                    $"<color={PrefabNameColor}>{prefabName}</color> has missing prefab <color={SubPrefabNameColor}>{gameObject.name}</color>");
                return;
            }

            if (!isRoot)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                {
                    return;
                }

                GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
                if (root == gameObject)
                {
                    return;
                }
            }

            foreach (Transform childT in gameObject.transform)
            {
                FindMissingPrefabRecursive(childT.gameObject, prefabName, false);
            }
        }
    }
}