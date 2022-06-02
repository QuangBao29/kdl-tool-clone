using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Imba.UI
{

	public class UISGTabFullEffect : UISGTabBaseEffect
	{

		public Image img;
		public Color imgNormalColor;
		public Color imgDisableColor;

		public Text lbl;
		public Color lblNormalColor;
		public Color lblDisableColor;

		public Outline outline;
		public Color outNormalColor;
		public Color outDisableColor;

		public RectTransform rectTrans;
		public Vector2 localPosNormal;
		public Vector2 localPosDiable;

		public Transform trans;
		public Vector3 scaleNormal;
		public Vector3 scaleDisable;

		public Image imgSpr;
		public Sprite imgSprNormal;
		public Sprite imgSprDisble;

		public GameObject EffectObj;

		protected override void Enable()
		{
			if (img != null)
			{
				img.color = imgNormalColor;
			}
			if (lbl != null)
			{
				lbl.color = lblNormalColor;
			}
			if (outline != null)
			{
				outline.effectColor = outNormalColor;
			}
			if (rectTrans != null)
			{
				rectTrans.anchoredPosition = localPosNormal;
			}

			if (trans != null)
			{
				trans.localScale = scaleNormal;
			}

			if (imgSpr != null)
				imgSpr.sprite = imgSprNormal;

			if (EffectObj != null)
				EffectObj.SetActive(false);
		}

		protected override void Disable()
		{
			if (img != null)
			{
				img.color = imgDisableColor;
			}
			if (lbl != null)
			{
				lbl.color = lblDisableColor;
			}
			if (outline != null)
			{
				outline.effectColor = outDisableColor;
			}
			if (rectTrans != null)
			{
				rectTrans.anchoredPosition = localPosDiable;
			}
			if (imgSpr != null)
				imgSpr.sprite = imgSprDisble;

			if (trans != null)
			{
				trans.localScale = scaleDisable;
			}

			if (EffectObj != null)
				EffectObj.SetActive(true);
		}
	}
}