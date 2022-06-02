using KAP.ToolCreateMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace KAP.Tools
{
    public class ToolScreenShotRemoveBG : MonoBehaviour
    {
        public Action ActionSetupScreenShoot;
        public Action ActionSetupFinishScreenShoot;

        [SerializeField] private string _saveFolderPath = "Screenshots";
        [SerializeField] private string _screenShootName = "";
        [SerializeField] private Camera _mainCam = null;
        [SerializeField] private ToolCreateMapListRooms _toolCreateMap = null;
        
        private GameObject _whiteCamGameObject;
        private Camera _whiteCam;
        private GameObject _blackCamGameObject;
        private Camera _blackCam;
        
        private int _screenWidth;
        private int _screenHeight;
        private Texture2D _textureBlack;
        private Texture2D _textureWhite;
        private Texture2D _textureTransparentBackground;

        void Awake()
        {
            CreateBlackAndWhiteCameras();
            HideBlackAndWhiteCameras();
            CacheAndInitialiseFields();
        }

        public void SetSaveFolderPath(string path)
        {
            _saveFolderPath = path;
        }

        public void SetScreenShotName(string name)
        {
            _screenShootName = name;
        }

        // =============================================================================
        #region ScreenShoot

        public void OnScreenShotClick()
        {
            SetupScreenshot();
            StartCoroutine(CaptureFrame());
        }

        IEnumerator CaptureFrame()
        {
            yield return new WaitForEndOfFrame();
            RenderCamToTexture(_blackCam, _textureBlack);
            RenderCamToTexture(_whiteCam, _textureWhite);
            CalculateOutputTexture();
            SavePng();

            SetupFinishScreenShot();
        }

        IEnumerator CaptureFrame(Stack<ToolCreateMapListRoomItem> toolCreateMapStack)
        {
            yield return new WaitForEndOfFrame();

            if (toolCreateMapStack.Count <= 0)
            {
                foreach (var item in _toolCreateMap.GetLstRoomItem())
                {
                    item.OnButtonChangeToogleClick();
                }
                yield break;

            }

            var poppedItem = toolCreateMapStack.Pop();
            poppedItem.OnButtonChangeToogleClick();

            RenderCamToTexture(_blackCam, _textureBlack);
            RenderCamToTexture(_whiteCam, _textureWhite);
            CalculateOutputTexture();


            var stringurl = poppedItem.GetRoomId().ToString();
            SavePng(stringurl);

            SetupFinishScreenShot();

            Debug.LogError("End capture");

            poppedItem.OnButtonChangeToogleClick();
            yield return StartCoroutine(CaptureFrame(toolCreateMapStack));
        }

        public void OnScreenshotEachRoomClick()
        {
            SetupScreenshot();
            Stack<ToolCreateMapListRoomItem> stack = new Stack<ToolCreateMapListRoomItem>();
            foreach(var item in _toolCreateMap.GetLstRoomItem())
            {
                stack.Push(item);
                if (item.IsOn())
                {
                    item.OnButtonChangeToogleClick();
                }
            }

            StartCoroutine(CaptureFrame(stack));
        }



        #endregion
        // =============================================================================
        #region Setup Screenshot

        private void SetupScreenshot()
        {
            ActionSetupScreenShoot?.Invoke();
            ShowBlackAndWhiteCameras();
            _whiteCam.orthographicSize = _mainCam.orthographicSize;
            _blackCam.orthographicSize = _mainCam.orthographicSize;
        }

        private void SetupFinishScreenShot()
        {
            HideBlackAndWhiteCameras();
            ActionSetupFinishScreenShoot?.Invoke();
        }

        private void HideBlackAndWhiteCameras()
        {
            _whiteCamGameObject.SetActive(false);
            _blackCamGameObject.SetActive(false);
        }

        private void ShowBlackAndWhiteCameras()
        {
            _whiteCamGameObject.SetActive(true);
            _blackCamGameObject.SetActive(true);
        }

        private void RenderCamToTexture(Camera cam, Texture2D tex)
        {
            cam.enabled = true;
            cam.Render();
            WriteScreenImageToTexture(tex);
            cam.enabled = false;
        }

        private void CreateBlackAndWhiteCameras()
        {
            _whiteCamGameObject = (GameObject)new GameObject();
            _whiteCamGameObject.name = "White Background Camera";
            _whiteCam = _whiteCamGameObject.AddComponent<Camera>();
            _whiteCam.CopyFrom(_mainCam);
            _whiteCam.backgroundColor = Color.white;
            _whiteCamGameObject.transform.SetParent(_mainCam.transform, true);

            _blackCamGameObject = (GameObject)new GameObject();
            _blackCamGameObject.name = "Black Background Camera";
            _blackCam = _blackCamGameObject.AddComponent<Camera>();
            _blackCam.CopyFrom(_mainCam);
            _blackCam.backgroundColor = Color.black;
            _blackCamGameObject.transform.SetParent(_mainCam.transform, true);
        }

        private void CacheAndInitialiseFields()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            _textureBlack = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _textureWhite = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _textureTransparentBackground = new Texture2D(_screenWidth, _screenHeight, TextureFormat.ARGB32, false);
        }

        #endregion
        // =============================================================================
        #region Calculate & Export PNG

        private void WriteScreenImageToTexture(Texture2D tex)
        {
            tex.ReadPixels(new Rect(0, 0, _screenWidth, _screenHeight), 0, 0);
            tex.Apply();
        }

        private void CalculateOutputTexture()
        {
            Color color;
            for (int y = 0; y < _textureTransparentBackground.height; ++y)
            {
                for (int x = 0; x < _textureTransparentBackground.width; ++x)
                {
                    // each column
                    float alpha = _textureWhite.GetPixel(x, y).r - _textureBlack.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    if (alpha <= 0.001f)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = _textureBlack.GetPixel(x, y) / alpha;
                    }
                    color.a = alpha;
                    _textureTransparentBackground.SetPixel(x, y, color);
                }
            }
        }

        private string GetScreenShootName()
        {
            string defaultName = DateTime.UtcNow.ToLongTimeString();
            return (string.IsNullOrEmpty(_screenShootName)) ? defaultName : _screenShootName;
        }

        private void SavePng()
        {
            string saveFolder = Path.Combine(Application.dataPath, _saveFolderPath);
            string nameWithURL =  Path.Combine(saveFolder, GetScreenShootName());
            var pngShot = _textureTransparentBackground.EncodeToPNG();

            Debug.LogError(string.Format("Saving screenshot: {0}", nameWithURL));
            File.WriteAllBytes(nameWithURL, pngShot);
        }

        private void SavePng(string inputName)
        {
            var s = GetScreenShootName();
            s= s.Replace(".png", "");
            

            string saveFolder = Path.Combine(Application.dataPath, _saveFolderPath);
            saveFolder = Path.Combine(saveFolder, s);

            string nameWithURL = Path.Combine(saveFolder, string.Format("{0}.png", inputName));
            var pngShot = _textureTransparentBackground.EncodeToPNG();

            File.WriteAllBytes(nameWithURL, pngShot);
        }

        #endregion
        // =============================================================================
    }
}