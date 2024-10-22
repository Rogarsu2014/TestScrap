using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TestScrap
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class TestScrap : BaseUnityPlugin
    {
        public static TestScrap Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        public static AssetBundle MyAssetBundle;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            MyAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "bombbundle"));
            if (MyAssetBundle == null)
            {
                Logger.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }
            Logger.LogInfo("Test 1");

            int iRarity = 100;
            Item MyCustomItem = MyAssetBundle.LoadAsset<Item>("Assets/TestStuff/TestItemMod.asset");
            LethalLib.Modules.Utilities.FixMixerGroups(MyCustomItem.spawnPrefab);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(MyCustomItem.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(MyCustomItem, iRarity, LethalLib.Modules.Levels.LevelTypes.All);

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
