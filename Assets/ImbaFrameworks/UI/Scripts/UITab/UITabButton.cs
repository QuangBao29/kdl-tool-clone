using System;
using UnityEngine;
using System.Collections;
using Imba.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Imba.UI
{
	[ExecuteInEditMode]
	public class UITabButton : MonoBehaviour, IPointerClickHandler
	{
		public UITabGroup group;
		
		public bool isDefault;
		public string soundSelected = "Tap";
		
		public UnityEvent onTabSelected;
		public UnityEvent onTabDeselected;
		
		private UITabEffect _effect;


		public bool IsSelected { get; private set; }

		void Awake()
		{
			_effect = GetComponent<UITabEffect>();
			
			if(group == null) Debug.LogException(new Exception("Pls add tab group"));
			
			group.Subcribe(this);
		}

		private void Start()
		{
			if (isDefault)
				group.ChangeTab(this, false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			group.ChangeTab(this);
			if(!string.IsNullOrEmpty(soundSelected)) AudioManager.Instance.PlaySFX(soundSelected);
		}
		
		public void Select(bool triggerEvent)
		{
			if (triggerEvent)
			{
				if (onTabSelected != null)
				{
					onTabSelected.Invoke();
				}
			}

			if (_effect != null)
				_effect.Play(true);
			
			IsSelected = true;
		}

		public void Deselect(bool triggerEvent)
		{
			if (triggerEvent)
			{
				if (onTabDeselected != null)
				{
					onTabDeselected.Invoke();
				}
			}

			if (_effect != null)
				_effect.Play(false);
			
			IsSelected = false;

		}

		public void ActiveMe()
		{
			group.ChangeTab(this, false);
		}
	
	}
}