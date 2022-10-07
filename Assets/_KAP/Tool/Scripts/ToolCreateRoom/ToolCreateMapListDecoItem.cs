using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneP.InfinityScrollView;
using UnityEngine.UI;
using Kawaii.IsoTools;
using KAP.Config;
using DigitalRubyShared;
using KAP.Tools;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListDecoItem : InfinityBaseItem
    {
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField] private ToolCreateMapListDecos _uiParent = null;
        [SerializeField] private Image _imgIcon = null;
        [SerializeField] private Text _lblName = null;
        [SerializeField] private GameObject _panelBubbleDecoSetting = null;
        [Space]
        [SerializeField] private ToolCreateMapBubbleSetting _toolBubbleSetting = null;
        [SerializeField] private ToolCreateMapBubbleDecoSetting _toolBubbleDecoSetting = null;
        private ConfigDecoRecord record;

        public override void Reload(InfinityScrollView infinity, int _index)
        {
            base.Reload(infinity, _index);
            record = _uiParent.GetConfigByIndex(_index);
            if (record == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            _imgIcon.sprite = _importDecoController.GetSprite(record.Id.ToString(), record.ThemeId);
            _lblName.text = record.Id.ToString();
        }

        public void OnItemClick()
        {
            if (ToolEditMode.Instance.CurrentPhaseMode == PhaseMode.Unpacking)
            {
                var current = _editManager.Current;
                if (current != null && current.EditStatus != KHHEditStatus.Valid)
                    return;
                if (record != null)
                {
                    var deco = _importDecoController.CreateDeco(record.Id, 0);
                    deco.Info = new DecoInfo { Id = record.Id, IsBubble = false, IsUnpacking = true };
                    deco.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
                    var decoEdit = deco.GetComponent<DecoEditDemo>();
                    if (_editManager.SetCurrent(decoEdit))
                    {
                        decoEdit.StartMove();
                        decoEdit.EndMove();
                        _editManager.editTool.SetValid(decoEdit.EditStatus);
                    }
                }
            }
            else if (ToolEditMode.Instance.CurrentPhaseMode == PhaseMode.Bubble)
            {
                if (_toolBubbleSetting.CurrentBubble == null)
                {
                    Debug.LogError("Select a Current Bubble!");
                    return;
                }
                var current = _editManager.Current;
                if (current != null && current.EditStatus != KHHEditStatus.Valid)
                    return;
                if (record != null)
                {
                    var bubble = _toolBubbleSetting.CreateDecoBubble(record.Id, 0);
                    bubble.Info = new DecoInfo { Id = record.Id, Color = 0, IsBubble = true };
                    bubble.BubbleIndex = _toolBubbleSetting.CurrentBubble.Index;
                    bubble.RoomIndex = _toolBubbleSetting.CurrentBubble.RoomIndex;
                    bubble.BubbleId = bubble.RoomIndex + "_" + bubble.BubbleIndex;
                    bubble.Prefab = null;
                    bubble.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
                    var decoEdit = bubble.GetComponent<DecoEditDemo>();
                    if (_editManager.SetCurrent(decoEdit))
                    {
                        decoEdit.StartMove();
                        decoEdit.EndMove();
                        _editManager.editTool.SetValid(decoEdit.EditStatus);
                    }
                }
            }
            else
            {
                var current = _editManager.Current;
                if (current != null && current.EditStatus != KHHEditStatus.Valid)
                    return;
                if (record != null)
                {
                    var deco = _importDecoController.CreateDeco(record.Id, 0);
                    deco.Info = new DecoInfo { Id = record.Id, IsBubble = false };
                    deco.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
                    var decoEdit = deco.GetComponent<DecoEditDemo>();
                    if (_editManager.SetCurrent(decoEdit))
                    {
                        decoEdit.StartMove();
                        decoEdit.EndMove();
                        _editManager.editTool.SetValid(decoEdit.EditStatus);
                    }
                }
            }
        }
        public void CreateBubbleDeco()
        {
            var bubble = _toolBubbleSetting.CreateDecoBubble(record.Id, 0);
            bubble.Info = new DecoInfo { Id = record.Id, Color = 0, IsBubble = true };
            //bubble.BubbleIndex = _bubbleIndex;
            //bubble.RoomIndex = _roomIdx;
            //bubble.BubbleId = bubble.RoomIndex + "_" + bubble.BubbleIndex;
            //bubble.Prefab = this;
            bubble.Position = IsoWorld.WorldToIso(Camera.main.transform.position, 0);
            var decoEdit = bubble.GetComponent<DecoEditDemo>();
            if (_editManager.Current != null)
            {
                var current = _editManager.Current;
                _editManager.SetCurrent(null);
                current.deco.Remove();
            }
            if (_editManager.SetCurrent(decoEdit))
            {
                decoEdit.StartMove();
                decoEdit.EndMove();
                _editManager.editTool.SetValid(decoEdit.EditStatus);
            }

            //foreach (var root in _toolBubbleDecoSetting.DctRootDecoItems)
            //{
            //    if (root.Key.BubbleId == _toolBubbleSetting.CurrentBubble.BubbleId)
            //    {
            //        if (root.Key.BubbleDeco != null)
            //        {
            //            var temp = root.Key.BubbleDeco;
            //            temp.Remove();
            //            root.Key.BubbleDeco = null;
            //        }
            //        //root.Key.BubbleDeco = bubble;
            //        foreach (var item in root.Value)
            //        {
            //            item.UnActiveImgCheck();
            //        }
            //        break;
            //    }
            //}
            //SetActiveImgCheck();
        }
    }
}