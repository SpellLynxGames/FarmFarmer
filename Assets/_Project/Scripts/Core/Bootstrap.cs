using UnityEngine;

namespace FarmFarmer.Core
{
    // No scene-dependent init: this fires before any scene loads, so app flow never depends on
    // a bootstrap object being manually placed in a scene.
    public static class Bootstrap
    {
        public static GameStateMachineRunner Runner { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Run()
        {
            if (Runner != null) return; // guards re-entry (e.g. domain reload edge cases in editor)

            var root = new GameObject("GameRoot");
            Object.DontDestroyOnLoad(root);
            Runner = root.AddComponent<GameStateMachineRunner>();
            Runner.Machine.ChangeState(new BootState());
        }
    }
}
