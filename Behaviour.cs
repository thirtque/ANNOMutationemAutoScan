using HighlightingSystem;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using Interactive.Item;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ANNOMutationemAutoScan;

public class Behaviour : MonoBehaviour
{
    private bool _enabled;

    internal static void Initialize()
    {
        ClassInjector.RegisterTypeInIl2Cpp<Behaviour>();

        GameObject gameObject = new("ExplorerBehaviour");
        DontDestroyOnLoad(gameObject);
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        gameObject.AddComponent<Behaviour>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
            _enabled = !_enabled;

        if (_enabled)
            UpdateAutoScan();
    }

    private void UpdateAutoScan()
    {
        var scene = SceneManager.GetActiveScene();
        var gameObjects = scene.GetRootGameObjects();
        foreach (var gameObject in gameObjects)
        {
            var highlighterComponents = gameObject.GetComponentsInChildren<Highlighter>(true);
            foreach (var highlighterComponent in highlighterComponents)
            {
                if (!highlighterComponent.enabled)
                    highlighterComponent.enabled = true;
            }

            var boxComponents = gameObject.GetComponentsInChildren<InteractiveBox>(true);
            foreach (var boxComponent in boxComponents)
            {
                if (boxComponent.highlighters == null)
                    continue;

                if (boxComponent.items.Count > 0)
                    UpdateHighlighters(boxComponent.highlighters, true, Color.green);
                else
                    UpdateHighlighters(boxComponent.highlighters, false);
            }
        }
    }

    private void UpdateHighlighters(List<Highlighter> highlighterComponents, bool enable, Color? color = null)
    {
        foreach (var highlighterComponent in highlighterComponents)
        {
            if (!highlighterComponent)
                continue;

            if (highlighterComponent.enabled != enable)
                highlighterComponent.enabled = enable;

            if (color != null && highlighterComponent.color != color)
            {
                highlighterComponent.color = (Color)color;

                foreach (var highlightableRendererComponent in highlighterComponent.highlightableRenderers)
                {
                    if (!highlightableRendererComponent)
                        continue;

                    highlightableRendererComponent.SetColor((Color)color);
                }
            }
        }
    }
}