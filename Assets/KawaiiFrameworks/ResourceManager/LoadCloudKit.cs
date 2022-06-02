using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using AssetBundles;

namespace Kawaii.ResourceManager
{
    public class LoadCloudKit
    {
        private Dictionary<string, KawaiiAtlas> _allAtlas;
        public LoadCloudKit(Dictionary<string, KawaiiAtlas> allAtlas)
        {
            _allAtlas = allAtlas;
        }

        private static AssetBundleManager _abm = null;
        private static Dictionary<string, bool> _dicCloudBundle = new Dictionary<string, bool>();
        private static Dictionary<string, AssetBundleAsync> _dicCloudBundleDownloading = new Dictionary<string, AssetBundleAsync>();

        public IEnumerator SetupCloud(string server)
        {
            _abm = new AssetBundleManager();
            _abm.SetBaseUri(server);
            var initializeAsync = _abm.InitializeAsync();
            yield return initializeAsync;
            if (initializeAsync.Success)
            {
                if (_abm.Manifest != null)
                {
                    var arr = _abm.Manifest.GetAllAssetBundles();
                    if (arr != null)
                    {
                        foreach (var iter in arr)
                            _dicCloudBundle[iter] = true;
                    }
                }
            }
        }

        public bool IsCloudBundle(string bundleName)
        {
            return _dicCloudBundle.ContainsKey(bundleName);
        }

        public IEnumerator LoadAtlasConroutine(string bundleName)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(bundleName, out atlas))
            {
                AssetBundleAsync assetBundleAsync = null;
                if (!_dicCloudBundleDownloading.TryGetValue(bundleName, out assetBundleAsync))
                {
                    string fullBundleName = bundleName + ".bundle";
                    assetBundleAsync = _abm.GetBundleAsync(fullBundleName);
                    _dicCloudBundleDownloading[bundleName] = assetBundleAsync;
                }

                while (!assetBundleAsync.IsDone)
                    yield return 0;
                var allData = assetBundleAsync.AssetBundle.LoadAllAssets();

                foreach (GameObject obj in allData)
                {
                    atlas = obj.GetComponent<KawaiiAtlas>();
                    atlas.Setup();
                }
                _allAtlas[bundleName] = atlas;
                _dicCloudBundleDownloading.Remove(bundleName);
                if (assetBundleAsync != null)
                    assetBundleAsync.AssetBundle.Unload(false);
            }

            yield return atlas;
        }


        public async Task<KawaiiAtlas> LoadAtlasAsync(string bundleName)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(bundleName, out atlas))
            {
                AssetBundleAsync assetBundleAsync = null;
                if (!_dicCloudBundleDownloading.TryGetValue(bundleName, out assetBundleAsync))
                {
                    string fullBundleName = bundleName + ".bundle";
                    assetBundleAsync = _abm.GetBundleAsync(fullBundleName);
                    _dicCloudBundleDownloading[bundleName] = assetBundleAsync;
                }

                while (!assetBundleAsync.IsDone)
                    await Task.Yield();
                var allData = assetBundleAsync.AssetBundle.LoadAllAssets();

                foreach (GameObject obj in allData)
                {
                    atlas = obj.GetComponent<KawaiiAtlas>();
                    atlas.Setup();
                }
                _allAtlas[bundleName] = atlas;
                _dicCloudBundleDownloading.Remove(bundleName);
                if (assetBundleAsync != null)
                    assetBundleAsync.AssetBundle.Unload(false);
            }

            return atlas;
        }
    }
}
