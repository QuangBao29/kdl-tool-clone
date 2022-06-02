using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;
using System.Threading.Tasks;
using Imba.Utils;

namespace Kawaii
{
    public enum LoadState
    {
        None,
        Reset,//GC, Hide dialog ....
        Unload,//unload unsed resource
        //Preload,//Update text, fade loading bg
        Loading,//update trang thai loading
        Loaded,//loading xong
        //Waiting,//waiting for load data
        Finished,//
    }

    public class KawaiiSceneManager : ManualSingletonMono<KawaiiSceneManager>
    {
        //ui data
        public CanvasGroup bg;
        public TextMeshProUGUI txtTip;
        public Slider sliderLoad;
        public TextMeshProUGUI txtPercent;
        //[Space]
        public List<Sprite> lstBanner;
        [Space]
        public Image imgBanner;

        //private data
        string prevScene;
        object prevParam;

        string currentScene;
        object currentParam;

        bool sceneDataLoadded = false;
        float finishLoadTime = 0;

        [Serializable]
        public class EventCallback : UnityEvent<string, LoadState> { };
        public EventCallback OnChangeSceneEvent = null;

        void Start()
        {
            bg.gameObject.SetActive(false);
        }

        async void Process(string nextScene)
        {
            //Debug.LogError("Next scene " + nextScene);
            //reset
            var currentState = LoadState.Reset;
            OnChangeSceneEvent.Invoke(nextScene, currentState);
            finishLoadTime = 0; 
            sceneDataLoadded = false;
            if (lstBanner != null && lstBanner.Count > 0)
            {
                var ran = new System.Random();
                int ranIndex = ran.Next(0, lstBanner.Count);
                imgBanner.sprite = lstBanner[ranIndex];
                imgBanner.SetNativeSize();
            }
            txtTip.text = GetRandomTip();
            txtPercent.text = "0%";
            sliderLoad.value = 0;
            bg.gameObject.SetActive(true);
            bg.alpha = 0;
            bg.DOFade(1f, 0.2f);
            await Task.Delay(200);
            //unload
            currentState = LoadState.Unload;
            OnChangeSceneEvent.Invoke(nextScene, currentState);
            var unloadResourceTask = Resources.UnloadUnusedAssets();
            while(!unloadResourceTask.isDone)
                await Task.Yield();

            //loading
            currentState = LoadState.Loading;
            OnChangeSceneEvent.Invoke(nextScene, currentState);
            var loadSceneTask = SceneManager.LoadSceneAsync(nextScene);
            while (!loadSceneTask.isDone)
            {
                txtPercent.text = string.Format("{0}%", (int)(loadSceneTask.progress * 100));
                sliderLoad.value = loadSceneTask.progress;
                await Task.Yield();
            }
            currentScene = nextScene;
            txtPercent.text = "100%";
            sliderLoad.value = 1f;
            currentState = LoadState.Loaded;
            OnChangeSceneEvent.Invoke(nextScene, currentState);

            while (!sceneDataLoadded)
            {
                await Task.Yield();
            }
            currentState = LoadState.Finished;
            OnChangeSceneEvent.Invoke(nextScene, currentState);
            bg.DOFade(0f, 0.2f).OnComplete(() => {
                bg.gameObject.SetActive(false);
            });
            finishLoadTime = Time.realtimeSinceStartup;
        }

        #region Public Method

        public void ChangeScene(string scene, object p = null)
        {
            prevScene = currentScene;
            prevParam = currentParam;

            currentParam = p;
            Process(scene);
        }

        public void Back()
        {
            if (!string.IsNullOrEmpty(prevScene))
                ChangeScene(prevScene, prevParam);
        }

        public void Finish()
        {
            sceneDataLoadded = true;
        }

        public void SetLoadingDataProcess(float value)
        {
            sliderLoad.value = value;
            txtPercent.text = Mathf.RoundToInt(value * 100) + "%";
        }

        public object Param
        {
            get { return currentParam; }
            set
            {
                currentParam = value;
            }
        }

        public string CurrentScene
        {
            get { return currentScene; }
        }

        public string PrevScene
        {
            get
            {
                return prevScene;
            }
        }

        public float FinishLoadingSceneTime
        {
            get
            {
                if (finishLoadTime <= 0)
                    return 0; // chưa load xong
                else
                    return Time.realtimeSinceStartup - finishLoadTime;
            }
        }

        #endregion

        string GetRandomTip()
        {
            return "";
        }
    }

}
