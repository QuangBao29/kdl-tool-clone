using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneP.InfinityScrollView;
using UnityEngine.UI;
using Kawaii.IsoTools;
using KAP.Config;

namespace KAP.ToolCreateMap
{
    public class ToolCreateMapListDecoItem : InfinityBaseItem
    {
        [SerializeField] private EditManager _editManager = null;
        [SerializeField] private ToolCreateMapImportDeco _importDecoController = null;
        [SerializeField] private ToolCreateMapListDecos _uiParent = null;
        [SerializeField] private Image _imgIcon = null;
        [SerializeField] private Text _lblName = null;

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
            var current = _editManager.Current;
            if (current != null && current.EditStatus != KHHEditStatus.Valid)
                return;
            if (record != null)
            {
                var deco = _importDecoController.CreateDeco(record.Id, 0);
                deco.Info = new DecoInfo { Id = record.Id };
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
}