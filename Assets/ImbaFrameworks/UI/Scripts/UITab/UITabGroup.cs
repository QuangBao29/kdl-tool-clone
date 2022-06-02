using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Imba.UI
{
	public class UITabGroup : MonoBehaviour
	{
		private List<UITabButton> _listTabs;

		UITabButton _current;

		public UITabButton Current
		{
			get { return _current; }
		}

		public void Subcribe(UITabButton tab)
		{
			if(_listTabs == null) _listTabs = new List<UITabButton>();
			_listTabs.Add(tab);
		}

		public void ChangeTab(UITabButton tabButton, bool triggerEvent = true)
		{
			if (_current != null)
				_current.Deselect(triggerEvent);
			
			_current = tabButton;
			
			if (_current != null)
				_current.Select(triggerEvent);
		}
	}
}