using System;
using System.IO;
using System.Reflection;
using Harmony;
using Newtonsoft.Json.Linq;

namespace RTVersion
{
    public static class RTVersion
    {
        internal static string ModDirectory;
        internal static string Version;

        public static void Init(string directory, string _settingsJson)
        {
            ModDirectory = directory;
            LoadVersion();
            var harmony = HarmonyInstance.Create("de.morphyum.RTVersion");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void LoadVersion()
        {
            if (!string.IsNullOrEmpty(Version)) return;

            using (var r = new StreamReader($"{ModDirectory}/mod.json")) 
            {
                var json = r.ReadToEnd();
                var parsed = JObject.Parse(json);
                Version = parsed.GetValue("Version").ToString();
            }
        }
    }
    
    [HarmonyPatch(typeof(VersionInfo), "GetReleaseVersion")]
    [HarmonyAfter(new string[] {"io.github.mpstark.ModTek"})]
    public static class VersionInfo_GetReleaseVersion_Patch {
        public static void Postfix(ref string __result) {
            var old = __result;
            __result = $"{old} w/ RT v{RTVersion.Version}";
        }
    }

    [HarmonyPatch(typeof(VersionInfo), "GetFormattedInfo", MethodType.Normal)]
    public static class VersionInfo_GetFormattedInfo_Patch
    {
        public static void Postfix(ref string __result)
        {
            var old = __result;
            __result = $"{old}RTVersion: {RTVersion.Version}";
        }
    }
}
