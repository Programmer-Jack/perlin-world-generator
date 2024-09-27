using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    SerializedProperty width; 
    SerializedProperty height;
    SerializedProperty xOrigin;
    SerializedProperty yOrigin;
    SerializedProperty xOffsetVariance;
    SerializedProperty yOffsetVariance;
    SerializedProperty scale;

    SerializedProperty threshold;

    SerializedProperty blockContainerPrefab;

    SerializedProperty grass;

    SerializedProperty dirt;
    SerializedProperty lastDirtChance;

    SerializedProperty stone;

    SerializedProperty gold;
    SerializedProperty goldHeightLimit;
    SerializedProperty goldChance;
    SerializedProperty extendedGoldChance;

    SerializedProperty water;
    SerializedProperty seaLevel;

    SerializedProperty sand;
    SerializedProperty beachSize;
    
    void OnEnable()
    {
        width = serializedObject.FindProperty("_width");
        height = serializedObject.FindProperty("_height");
        xOrigin = serializedObject.FindProperty("_xOrigin");
        yOrigin = serializedObject.FindProperty("_yOrigin");
        xOffsetVariance = serializedObject.FindProperty("_xOffsetVariance");
        yOffsetVariance = serializedObject.FindProperty("_yOffsetVariance");
        scale = serializedObject.FindProperty("_scale");
        threshold = serializedObject.FindProperty("_thresholdMultiplier");
        blockContainerPrefab = serializedObject.FindProperty("_blockContainerPrefab");
        grass = serializedObject.FindProperty("_grass");
        dirt = serializedObject.FindProperty("_dirt");
        lastDirtChance = serializedObject.FindProperty("_lastDirtChance");
        stone = serializedObject.FindProperty("_stone");
        gold = serializedObject.FindProperty("_gold");
        goldHeightLimit = serializedObject.FindProperty("_goldHeightLimit");
        goldChance = serializedObject.FindProperty("_goldChance");
        extendedGoldChance = serializedObject.FindProperty("_extendedGoldChance");
        water = serializedObject.FindProperty("_water");
        seaLevel = serializedObject.FindProperty("_seaLevel");
        sand = serializedObject.FindProperty("_sand");
        beachSize = serializedObject.FindProperty("_beachSize");
    }

    public override void OnInspectorGUI()
    {
        WorldGenerator worldGenerator = (WorldGenerator)target;

        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(xOrigin);
        EditorGUILayout.PropertyField(yOrigin);
        EditorGUILayout.PropertyField(xOffsetVariance);
        EditorGUILayout.PropertyField(yOffsetVariance);
        EditorGUILayout.PropertyField(scale);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(threshold);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(blockContainerPrefab);
        EditorGUILayout.PropertyField(grass);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(dirt);
        EditorGUILayout.PropertyField(lastDirtChance);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(stone);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(gold);
        EditorGUILayout.PropertyField(goldHeightLimit);
        EditorGUILayout.PropertyField(goldChance);
        EditorGUILayout.PropertyField(extendedGoldChance);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(water);
        EditorGUILayout.PropertyField(seaLevel);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(sand);
        EditorGUILayout.PropertyField(beachSize);
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate World (yxz)"))
        {
            worldGenerator.GenerateYXZ();
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate World (xzy)"))
        {
            worldGenerator.GenerateWorld();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
