using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace unlit_climber
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<Color> body;
        static ConfigEntry<Color> cloth;
        static ConfigEntry<Color> crown;
        static ConfigEntry<bool> is_enabled;
        static Plugin Instance;
        static Material unlit_mat;
        private void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Plugin));
            // Plugin startup logic
            unlit_mat = AssetBundle.LoadFromStream(typeof(Plugin).Assembly.GetManifestResourceStream("unlit_climber.unlit")).LoadAsset<Material>("Assets/CustomShaders/Template/unlit.mat");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            is_enabled = Config.Bind<bool>("Mod", "enabled", true, "if false, the mod doesn't change any materials.");
            body = Config.Bind<Color>("Colors", "Body", new Color(0.984f,0.47f,0.02f));
            cloth = Config.Bind<Color>("Colors", "Cloth", new Color(0.984f,0.514f,0.02f));
            crown = Config.Bind<Color>("Colors", "Crown", new Color(0.984f,0.514f,0.02f));
        }
        [HarmonyPatch(typeof(ClimberMain), nameof(ClimberMain.Start))]
        [HarmonyPostfix]
        public static void onStart(ClimberMain __instance) {
            Instance.Config.Reload();
            if (!is_enabled.Value) return;
            unlit_mat.SetColor("_Color", body.Value);
            var body_rend = __instance.bodyScript.bodyModel.GetComponent<Renderer>();
            var mats = new Material[2];
            mats[0] = new Material(unlit_mat);
            mats[1] = new Material(unlit_mat);
            body_rend.materials = mats;

            var cloth_rend = __instance.loinCloth;
            unlit_mat.SetColor("_Color", cloth.Value);
            var cloth_mat = new Material(unlit_mat);
            cloth_rend.material = cloth_mat;

            var crown_rend = __instance.hat.GetComponentInChildren<Renderer>();
            if (crown_rend == null) return;
            unlit_mat.SetColor("_Color", crown.Value);
            var crown_mat = new Material(unlit_mat);
            crown_rend.material = crown_mat;
        }
    }
}
