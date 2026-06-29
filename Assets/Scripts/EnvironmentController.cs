using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class EnvironmentController : MonoBehaviour
{
    public Color backgroundColor = Color.black;
    public Color ambientColor = Color.black;
    public float ambientIntensity = 0f;
    public bool disableSceneLights = true;

    void Start()
    {
        ApplyEnvironmentSettings();
    }

    void OnValidate()
    {
        ApplyEnvironmentSettings();
    }

    private void ApplyEnvironmentSettings()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = backgroundColor;
        }

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = ambientIntensity;

        if (disableSceneLights)
        {
            DisableAllSceneLights();
        }
    }

    private void DisableAllSceneLights()
    {
        Light[] lights = FindObjectsOfType<Light>(true);
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
