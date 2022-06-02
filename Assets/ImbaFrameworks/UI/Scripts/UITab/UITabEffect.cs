using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Imba.UI
{

	public class UITabEffect : MonoBehaviour
	{
		public Image img;
		public Color normalImgColor = Color.black;
		public Color selectedImgColor = Color.black;

		public TextMeshProUGUI text;
		public Color normalTextColor = Color.black;
		public Color selectedTextColor = Color.black ;

		public RectTransform moveObj;
		public Vector2 normalPos = Vector2.zero;
		public Vector2 selectedPos = Vector2.zero;

		public Transform scaleObj;
		public Vector3 normalScale = Vector3.one;
		public Vector3 selectedScale = Vector3.one;

		public Image imgSpr;
		public Sprite normalSprite;
		public Sprite selectedSprite;
		public void Play(bool selected)
		{
			if (img != null)
			{
				img.color = selected?  selectedImgColor : normalImgColor;
			}
			if (text != null)
			{
				text.color = selected?  selectedTextColor: normalTextColor;
			}
		
			if (moveObj != null)
			{
				moveObj.anchoredPosition = selected? selectedPos : normalPos;
			}

			if (scaleObj != null)
			{
				scaleObj.localScale = selected ? selectedScale : normalScale;
			}

			if (imgSpr != null)
				imgSpr.sprite = selected? selectedSprite : normalSprite;
		}
	}
}