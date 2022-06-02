using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private InputField _inputField = null;
    [SerializeField] private List<Sprite> _lstBg = null;
    [SerializeField] private List<Sprite> _lstLight = null;
    [SerializeField] private Image _imgBackground = null;
    [SerializeField] private Image _imgLight = null;

    public void OnLoadBackGroundClick()
    {
        int id = int.Parse(_inputField.text);
        if(id == 0)
        {
            _imgBackground.sprite = _lstBg[0];
            _imgLight.gameObject.SetActive(false);
            return;
        }
        _imgBackground.sprite = _lstBg[id];
        _imgLight.sprite = _lstLight[id-1];
    }

}
