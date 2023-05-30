using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LightingHelper : EditorWindow
{
    private Material skyboxMaterial;
    private AmbientMode ambientMode;
    
    private Color skyColor;
    private Color equatorColor;
    private Color groundColor;

    //Fog
    private bool isFogEnabled;
    private Color fogColor;
    private FogMode fogMode;
    private float fogDensity;
    private float startDistance;
    private float endDistance;

    private static Vector2 size = new Vector2(300, 500);

    [MenuItem("Tools/Lighting Helper", false, 1)]
    private static void Init()
    {
        var window = (LightingHelper)GetWindow(typeof(LightingHelper));
        
        window.minSize = size;
        window.maxSize = size;
        
        window.Show();
    }

    private void OnGUI()
    {
        SetValues();

        if (GUILayout.Button("Replace lighting settings on all build scenes"))
            if (Warn())
                ReplaceLights();
    }

    private void OnEnable()
    {
        skyboxMaterial = RenderSettings.skybox;
        ambientMode = RenderSettings.ambientMode;
        skyColor = RenderSettings.ambientSkyColor;
        equatorColor = RenderSettings.ambientEquatorColor;
        groundColor = RenderSettings.ambientGroundColor;

        isFogEnabled = RenderSettings.fog;
        fogColor = RenderSettings.fogColor;
        fogMode = RenderSettings.fogMode;
        fogDensity = RenderSettings.fogDensity;
        startDistance = RenderSettings.fogStartDistance;
        endDistance = RenderSettings.fogEndDistance;
    }

    private void SetValues()
    {
        skyboxMaterial = (Material)EditorGUILayout.ObjectField("SkyBox Material", skyboxMaterial, typeof(Material), true);
        ambientMode = (AmbientMode)EditorGUILayout.EnumPopup("Ambient Mode", ambientMode);

        if (ambientMode != AmbientMode.Skybox)
        {
            skyColor = EditorGUILayout.ColorField(new GUIContent("Skybox Color"), skyColor,true,false,true);
            
            if (ambientMode == AmbientMode.Trilight)
            {
                equatorColor = EditorGUILayout.ColorField(new GUIContent("Equator Color"), equatorColor,true,false,true);
                groundColor = EditorGUILayout.ColorField(new GUIContent("Ground Color"), groundColor,true,false,true);
            }
        }

        isFogEnabled = EditorGUILayout.Toggle("Fog", isFogEnabled);

        if (isFogEnabled)
        {
            fogColor = EditorGUILayout.ColorField(new GUIContent("Fog Color"), fogColor,true,true,false);
            fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", fogMode);

            if (fogMode != FogMode.Linear)
            {
                fogDensity = EditorGUILayout.FloatField("Fog Density", fogDensity);
            }
            else
            {
                startDistance = EditorGUILayout.FloatField("Start Density", startDistance);
                endDistance = EditorGUILayout.FloatField("Fog Density", endDistance);
            }
        }
    }

    private void ReplaceLights()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            EditorSceneManager.OpenScene(scene.path);

            RenderSettings.skybox = skyboxMaterial;

            RenderSettings.ambientMode = ambientMode;

            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = equatorColor;
            RenderSettings.ambientGroundColor = groundColor;

            RenderSettings.fog = isFogEnabled;
            RenderSettings.fogMode = fogMode;

            RenderSettings.fogColor = fogColor;

            RenderSettings.fogStartDistance = startDistance;
            RenderSettings.fogEndDistance = endDistance;

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }

    private bool Warn()
    {
        return EditorUtility.DisplayDialog("Warning",
            "This will replace lighting settings of all build scenes. Are you sure?", "Yes", "No");
    }
}