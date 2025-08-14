using System;
using System.Threading.Tasks;
using UnityEngine;
using SimpleFileBrowser;

namespace Source_Code {
    public class CrossPlatformFilePicker : MonoBehaviour
    {
        public static Task<string> PickFileAsync(string title = "Select a file", string[] extensions = null)
        {
            var tcs = new TaskCompletionSource<string>();

#if UNITY_STANDALONE || UNITY_EDITOR
            try {
                FileBrowser.SetFilters(true, new FileBrowser.Filter("Images",
                    ".jpg", ".png", ".bmp", ".xcf", ".tga"));
                FileBrowser.SetDefaultFilter(".jpg");

                FileBrowser.ShowLoadDialog((paths) => {
                        tcs.TrySetResult(FileBrowser.Result[0]);
                    },
                    () => { Debug.Log("Cancelled"); }, FileBrowser.PickMode.Files, false,
                    null, null, "Select Images", "Load");
            }
            catch (Exception e) {
                Debug.LogError($"Desktop file dialog failed: {e}");
                tcs.SetResult(null);
            }

            return tcs.Task;

#elif UNITY_ANDROID || UNITY_IOS
        // ---- Mobile: NativeFilePicker (yasirkula) ----
        // Installiere das Asset "Native File Picker" und:
        // using NativeFilePickerNamespace;
        try
        {
            // Mappe Endungen auf MimeTypes (optional). Ohne Mapping: "*/*"
            string[] mimeTypes = null;
            if (extensions != null && extensions.Length > 0)
            {
                // sehr grob – passe bei Bedarf an
                mimeTypes = new[] { "*/*" };
            }

            // Der Callback des Plugins ist callback-basiert, also in Task umwandeln
            NativeFilePicker.PickFile(
                (path) =>
                {
                    // path = lokaler Pfad (Plugin kopiert/zugänglich gemacht)
                    tcs.TrySetResult(path);
                },
                (error) =>
                {
                    Debug.LogWarning($"File picking cancelled/failed: {error}");
                    tcs.TrySetResult(null);
                },
                mimeTypes
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"Mobile file picker failed: {e}");
            tcs.SetResult(null);
        }
#else
        Debug.LogWarning("File picking not implemented on this platform.");
        tcs.SetResult(null);
#endif

            return tcs.Task;
        }
    }
}
