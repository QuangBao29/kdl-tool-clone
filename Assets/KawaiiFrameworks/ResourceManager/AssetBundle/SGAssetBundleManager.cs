//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using Imba.Atlas;

//namespace AssetBundles
//{
//    public class SGAssetBundleManager : MonoBehaviour
//    {
//        AssetBundleManager _abm = null;

//        Dictionary<string, bool> _dicRemoteBundleNames = new Dictionary<string, bool>();
//        Dictionary<string, bool> _dicIsLoadingBundles = new Dictionary<string, bool>();
//        Dictionary<string, ImbaAtlas> _dicAllAtlas = new Dictionary<string, ImbaAtlas>();

//        bool isInit;
//        bool isInitStatic;

//        public bool IsInitSucess
//        {
//            get
//            {
//                return isInit && isInitStatic;
//            }
//        }

//        public IEnumerator Init(string server)
//        {
//            _abm = new AssetBundleManager();
//            //string server = NetworkSetting.AssetServer.Replace(string.Format("/{0}/", NetworkSetting.Platform), "");
//            _abm.SetBaseUri(server);
//            var initializeAsync = _abm.InitializeAsync();
//            yield return initializeAsync;
//            try
//            {
//                if (initializeAsync.Success)
//                {
//                    if (_abm.Manifest != null)
//                    {
//                        var arr = _abm.Manifest.GetAllAssetBundles();
//                        if (arr != null)
//                        {
//                            foreach (var iter in arr)
//                                _dicRemoteBundleNames[iter] = true;
//                        }
//                    }
//                    isInit = true;
//                }
//            }
//            catch
//            {
//                _dicRemoteBundleNames.Clear();
//                isInit = false;
//            }
//            isInitStatic = true;
//        }


//        public IEnumerator GetBundleAsync(string bundlename, Action<ImbaAtlas> cb)
//        {
//            ImbaAtlas atlas = null;
//            if (!_dicAllAtlas.TryGetValue(bundlename, out atlas))
//            {
//                if(!_dicIsLoadingBundles.ContainsKey(bundlename))
//                {
//                    _dicIsLoadingBundles[bundlename] = true;
//                    var fullName = bundlename + ".bundle";
//                    if (IsRemoteBundle(fullName))
//                    {
//                        AssetBundleAsync bundle = _abm.GetBundleAsync(fullName);
//                        yield return bundle;
//                        if (bundle != null)
//                        {
//                            UnityEngine.Object[] allData = bundle.AssetBundle.LoadAllAssets();

//                            foreach (GameObject obj in allData)
//                            {
//                                atlas = obj.GetComponent<ImbaAtlas>();
//                                atlas.Setup();
//                            }
//                        }
//                    }
//                    else
//                    {
//                        string streamingAssetsPath = "";
//#if UNITY_EDITOR
//#if UNITY_ANDROID
//                        streamingAssetsPath = Application.streamingAssetsPath + "/Android/" + fullName;
//#else
//                        streamingAssetsPath = Application.streamingAssetsPath + "/iOS/" + fullName;
//#endif


//#else
//#if UNITY_ANDROID
//                streamingAssetsPath = Application.streamingAssetsPath + "/Android/" + bundlename + ".bundle";
//#else
//                streamingAssetsPath = Application.streamingAssetsPath + "/iOS/" + bundlename + ".bundle";
//#endif
//#endif
//                        var assetLoadCreateRequest = AssetBundle.LoadFromFileAsync(streamingAssetsPath);
//                        yield return assetLoadCreateRequest;
//                        if (assetLoadCreateRequest != null && assetLoadCreateRequest.assetBundle != null)
//                        {
//                            var myLoadedAssetBundle = assetLoadCreateRequest.assetBundle;
//                            var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(bundlename);
//                            yield return assetLoadRequest;
//                            UnityEngine.Object[] allData = assetLoadRequest.allAssets;
//                            if (allData != null)
//                            {
//                                foreach (GameObject obj in allData)
//                                {
//                                    atlas = obj.GetComponent<ImbaAtlas>();
//                                    atlas.Setup();
//                                }
//                            }
//                            myLoadedAssetBundle.Unload(false);
//                        }
//                    }
//                    _dicAllAtlas[bundlename] = atlas;
//                    _dicIsLoadingBundles.Remove(bundlename);
//                }
//            }

//            if (cb != null)
//                cb(atlas);
//            yield return atlas;
//        }

//        public bool IsRemoteBundle(string bundlename)
//        {
//            return _dicRemoteBundleNames.ContainsKey(bundlename);
//        }

//        public bool IsDownloading(string bundlename)
//        {
//            return _dicIsLoadingBundles.ContainsKey(bundlename);
//        }

//        public void Clean()
//        {
//            _dicAllAtlas.Clear();
//            if (_abm != null)
//                _abm.Dispose();
//        }
//    }
//}