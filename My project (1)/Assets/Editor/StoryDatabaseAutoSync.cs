using System.Linq;
using UnityEditor;
using UnityEngine;

public class StoryDatabaseAutoSync : AssetPostprocessor
{
    private static bool refreshScheduled;

    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths
    )
    {
        bool assetChanged =
            importedAssets.Any(IsAssetFile) ||
            deletedAssets.Any(IsAssetFile) ||
            movedAssets.Any(IsAssetFile) ||
            movedFromAssetPaths.Any(IsAssetFile);

        if (!assetChanged || refreshScheduled)
            return;

        refreshScheduled = true;

        /*
         * 에셋 임포트 처리 중에 바로 데이터베이스를 수정하면
         * 오류가 발생할 수 있으므로 다음 Editor 프레임에 실행한다.
         */
        EditorApplication.delayCall += RefreshAllDatabases;
    }

    private static bool IsAssetFile(string path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
               path.EndsWith(".asset");
    }

    [MenuItem("Tools/Story/StoryDatabase 새로고침")]
    private static void RefreshAllDatabases()
    {
        refreshScheduled = false;

        string[] databaseGuids =
            AssetDatabase.FindAssets("t:StoryDatabase");

        int databaseCount = 0;

        foreach (string guid in databaseGuids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            StoryDatabase database =
                AssetDatabase.LoadAssetAtPath<StoryDatabase>(
                    path
                );

            if (database == null)
                continue;

            database.RefreshNodesFromProject();
            databaseCount++;
        }

        AssetDatabase.SaveAssets();

        Debug.Log(
            $"StoryDatabase {databaseCount}개를 자동 갱신했습니다."
        );
    }
}