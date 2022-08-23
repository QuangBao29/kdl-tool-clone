using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Kawaii.IsoTools;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.ResourceManager;

namespace KAP.ToolCreateMap
{
    public class Bubble : Deco
    {
        //[SerializeField]
        //private Deco _prefabBubbleDeco = null;
        //[SerializeField]
        //private AreaManager _areaManager = null;
        [SerializeField]
        protected ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private GameObject _box = null;
        [HideInInspector] 
        public Dictionary<int, List<int>> DctDecoIdColor = new Dictionary<int, List<int>>();
        private int _bubbleIndex;
        private int _roomIdx;
        private string _bubbleId;
        private ToolCreateMapBubbleDecoItems _prefab = null;
        private bool _selected = false;
        public int BubbleIndex
        {
            get => _bubbleIndex;
            set => _bubbleIndex = value;
        }
        public int RoomIndex
        {
            get => _roomIdx;
            set => _roomIdx = value;
        }
        public ToolCreateMapBubbleDecoItems Prefab
        {
            get => _prefab;
            set => _prefab = value;
        }
        public string BubbleId
        {
            get => _bubbleId;
            set => _bubbleId = value;
        }

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        public Bubble() { }

    }
}

