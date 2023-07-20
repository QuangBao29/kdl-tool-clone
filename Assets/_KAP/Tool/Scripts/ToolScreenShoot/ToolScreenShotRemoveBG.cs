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
            //CreateBlackAndWhiteCameras();
            //HideBlackAndWhiteCameras();
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
        public void OnClickScreenShotKDL()
        {
            ActionSetupScreenShoot?.Invoke();
            StartCoroutine(SetupScreenShotKDL());
        }
        public IEnumerator SetupScreenShotKDL()
        {
            yield return new WaitForEndOfFrame(); // it must be a coroutine 

            Vector2 temp = _mainCam.transform.position;
            var startX = temp.x - _screenWidth / 2;
            var startY = temp.y - _screenHeight / 2;

            var tex = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(-1, -1, _screenWidth, _screenHeight), 0, 0);
            tex.Apply();

            // Encode texture into PNG
            var bytes = tex.EncodeToPNG();
            Destroy(tex);

            string saveFolder = Path.Combine(Application.dataPath, _saveFolderPath);
            string nameWithURL = Path.Combine(saveFolder, GetScreenShootName());

            File.WriteAllBytes(nameWithURL, bytes);

            //string imgsrc = System.Convert.ToBase64String(bytes);
            //Texture2D scrnShot = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            //scrnShot.LoadImage(System.Convert.FromBase64String(imgsrc));

            //Sprite sprite = Sprite.Create(scrnShot, new Rect(0, 0, scrnShot.width, scrnShot.height), new Vector2(.5f, .5f));
            //img.sprite = sprite;
        }
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
            WriteScreenImageToTexture(cam, tex);
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

        private void WriteScreenImageToTexture(Camera cam, Texture2D tex)
        {
            tex.ReadPixels(new Rect(0, 0, _screenWidth, _screenHeight), 1, 1);
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