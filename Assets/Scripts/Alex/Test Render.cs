using Tymski;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
public class SceneRenderSetup : MonoBehaviour {

    public SceneReference mainMenuScene;
    public SceneReference gameScene;

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        var pipelineAsset = GraphicsSettings.defaultRenderPipeline
            as UniversalRenderPipelineAsset;

        if (pipelineAsset == null) return;

        // �������� RendererData � ����������� �� �����
        if (scene.name == mainMenuScene.SceneName) {
            pipelineAsset.LoadBuiltinRendererData(RendererType.UniversalRenderer);
            SetupFor3D();
        } else if (scene.name == gameScene.SceneName) {
            pipelineAsset.LoadBuiltinRendererData(RendererType._2DRenderer);
            SetupFor2D();
        }
    }

    private void SetupFor3D() {
        // 3D ���������
        RenderSettings.ambientIntensity = 1.0f;
        // �������������� ���������...
    }

    private void SetupFor2D() {
        // 2D ���������
        RenderSettings.ambientIntensity = 0.8f;
        // �������� Pixel Perfect ���� �����
        var pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        if (pixelPerfectCamera != null)
            pixelPerfectCamera.enabled = true;
    }
}
