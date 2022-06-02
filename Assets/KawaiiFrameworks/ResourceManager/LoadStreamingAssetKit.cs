using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Kawaii.ResourceManager
{
    public class LoadStreamingAssetKit
    {
        private Dictionary<string, KawaiiAtlas> _allAtlas;
        private Dictionary<string, AssetBundleCreateRequest> _isBundleCreateRequestLoading = new Dictionary<string, AssetBundleCreateRequest>();
        private Dictionary<string, AssetBundleRequest> _isBundleRequestLoading = new Dictionary<string, AssetBundleRequest>();

        public void ClearAll()
        {
            foreach(var iter in _isBundleCreateRequestLoading)
            {
                iter.Value.assetBundle.Unload(true);
            }
            _isBundleCreateRequestLoading.Clear();
            _isBundleRequestLoading.Clear();
        }

        public void Clear(string atlasName)
        {
            AssetBundleCreateRequest request = null;
            if(_isBundleCreateRequestLoading.TryGetValue(atlasName, out request))
            {
                request.assetBundle.Unload(true);
                _isBundleCreateRequestLoading.Remove(atlasName);
                _isBundleRequestLoading.Remove(atlasName);
            }
        }

        public LoadStreamingAssetKit(Dictionary<string, KawaiiAtlas> allAtlas)
        {
            _allAtlas = allAtlas;
        }

        public async Task<KawaiiAtlas> LoadAtlasAsync(string bundleName)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(bundleName, out atlas))
            {
                AssetBundleCreateRequest assetLoadCreateRequest = null;
                if (!_isBundleCreateRequestLoading.TryGetValue(bundleName, out assetLoadCreateRequest))
                {
                    string fullBundleName = bundleName + ".bundle";
                    string streamingAssetsPath = "";
#if UNITY_ANDROID
                    streamingAssetsPath = Application.streamingAssetsPath + "/Android/" + fullBundleName;
#else
                    streamingAssetsPath = Application.streamingAssetsPath + "/iOS/" + fullBundleName;
#endif
                    assetLoadCreateRequest = AssetBundle.LoadFromFileAsync(streamingAssetsPath);
                    _isBundleCreateRequestLoading[bundleName] = assetLoadCreateRequest;
                    Debug.LogWarning(streamingAssetsPath);
                }

                while (!assetLoadCreateRequest.isDone)
                    await Task.Yield();

                if (assetLoadCreateRequest.assetBundle != null)
                {
                    var myLoadedAssetBundle = assetLoadCreateRequest.assetBundle;
                    AssetBundleRequest assetLoadRequest = null;
                    if (!_isBundleRequestLoading.TryGetValue(bundleName, out assetLoadRequest))
                    {
                        assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<KawaiiAtlas>(bundleName);
                        _isBundleRequestLoading[bundleName] = assetLoadRequest;
                    }
                    while (!assetLoadRequest.isDone)
                        await Task.Yield();
                    var allData = assetLoadRequest.allAssets;
                    if (allData != null)
                    {
                        foreach (KawaiiAtlas obj in allData)
                        {
                            atlas = obj;
                            atlas.Setup();
                        }
                    }
                    //if (myLoadedAssetBundle != null)
                    //myLoadedAssetBundle.Unload(false);
                }
                else
                    Debug.LogError("Error At " + bundleName);
                _allAtlas[bundleName] = atlas;
                //_isBundleCreateRequestLoading.Remove(bundleName);
                //_isBundleRequestLoading.Remove(bundleName);
            }
            return atlas;
        }

        public IEnumerator LoadAtlasConroutine(string bundleName)
        {
            KawaiiAtlas atlas = null;
            if (!_allAtlas.TryGetValue(bundleName, out atlas))
            {
                AssetBundleCreateRequest assetLoadCreateRequest = null;
                if (!_isBundleCreateRequestLoading.TryGetValue(bundleName, out assetLoadCreateRequest))
                {
                    string fullBundleName = bundleName + ".bundle";
                    string streamingAssetsPath = "";
#if UNITY_ANDROID
                    streamingAssetsPath = Application.streamingAssetsPath + "/Android/" + fullBundleName;
#else
                    streamingAssetsPath = Application.streamingAssetsPath + "/iOS/" + fullBundleName;
#endif
                    assetLoadCreateRequest = AssetBundle.LoadFromFileAsync(streamingAssetsPath);
                    _isBundleCreateRequestLoading[bundleName] = assetLoadCreateRequest;
                }

                while (!assetLoadCreateRequest.isDone)
                    yield return 0;

                if (assetLoadCreateRequest.assetBundle != null)
                {
                    var myLoadedAssetBundle = assetLoadCreateRequest.assetBundle;
                    AssetBundleRequest assetLoadRequest = null;
                    if (!_isBundleRequestLoading.TryGetValue(bundleName, out assetLoadRequest))
                    {
                        assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<KawaiiAtlas>(bundleName);
                        _isBundleRequestLoading[bundleName] = assetLoadRequest;
                    }
                    while (!assetLoadRequest.isDone)
                        yield return 0;
                    var allData = assetLoadRequest.allAssets;
                    if (allData != null)
                    {
                        foreach (KawaiiAtlas obj in allData)
                        {
                            atlas = obj;
                            atlas.Setup();
                        }
                    }
                    if (myLoadedAssetBundle != null)
                        myLoadedAssetBundle.Unload(false);
                }
                _allAtlas[bundleName] = atlas;
                _isBundleCreateRequestLoading.Remove(bundleName);
                _isBundleRequestLoading.Remove(bundleName);
            }
            yield return atlas;
        }
    }
}

