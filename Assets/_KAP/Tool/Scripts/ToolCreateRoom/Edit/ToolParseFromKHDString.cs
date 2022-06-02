using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kawaii.IsoTools.DecoSystem;
using Kawaii.IsoTools;

namespace KAP.ToolCreateMap
{
    public class ToolParseFromKHDString : MonoBehaviour
    {
        public class KHDDecoInfo
        {
            public int Id;
            public int Color;
            public int Direct;
            public Vector3 Position;
        }

        [SerializeField]
        private AreaManager _areaManager = null;
        [SerializeField]
        private ToolCreateMapConfigController _configController = null;
        [SerializeField]
        private ToolCreateMapImportDeco _importDecoController = null;
     
        [SerializeField]
        private ToolCreateMapListDecos _lstRoomController = null;
        [SerializeField]
        private int _defaultTileId = 2201001;
        [SerializeField]
        private int _defaultWallpaperId = 2301001;
        [SerializeField]
        private int _defaultWallpaper1_5Id = 2401001;

        public void OnButtonParseClick()
        {
            var data = GUIUtility.systemCopyBuffer;
            if(string.IsNullOrEmpty(data))
            {
                Debug.LogError("Data is Empty!");
                return;
            }
            var lstParts = data.Split('|');
            var decoDataPart = lstParts[0];

            int sizeX = int.MinValue;
            int sizeY = int.MinValue;

            List<KHDDecoInfo> lstInfloorInfos = new List<KHDDecoInfo>();
            List<KHDDecoInfo> lstAboveInfos = new List<KHDDecoInfo>();
            List<KHDDecoInfo> lstWallHangInfo = new List<KHDDecoInfo>();

            var lstDecos = decoDataPart.Split(';');
            foreach(var deco in lstDecos)
            {
                if (string.IsNullOrEmpty(deco))
                    continue;
                var lstDecoParams = deco.Split(',');
                if (lstDecoParams.Length < 4)
                    continue;
                int id = 0;
                if (!int.TryParse(lstDecoParams[0], out id))
                    continue;
                id += 100000;
                int color = 0;
                if (!int.TryParse(lstDecoParams[1], out color))
                    continue;
                var lstPosParams = lstDecoParams[2].Split('_');
                if (lstPosParams.Length < 2)
                    continue;
                int x = 0;
                if (!int.TryParse(lstPosParams[0], out x))
                    continue;
                int y = 0;
                if (!int.TryParse(lstPosParams[1], out y))
                    continue;
                int z = 0;
                if (lstPosParams.Length >= 3 && !int.TryParse(lstPosParams[2], out z))
                    continue;
                int direct = 0;
                if (!int.TryParse(lstDecoParams[3], out direct))
                    continue;
                if (id > 1200000 && id < 1210000) //wallpaper
                    continue;
                if(id > 1210000 && id < 1220000) //tile
                {
                    if (x + 6 > sizeX)
                        sizeX = x + 6;
                    if (y + 6 > sizeY)
                        sizeY = y + 6;
                }
                else
                {
                    var configDeco = _configController.ConfigDeco.GetDecoById(id);
                    if(configDeco == null)
                    {
                        Debug.LogError(string.Format("Fail At Id: {0} - Config Null", id));
                        continue;
                    }
                    if (configDeco.IsWallHang)
                        continue;
                    if (z == 0)
                        lstInfloorInfos.Add(new KHDDecoInfo { Id = id, Color = color, Direct = direct, Position = new Vector3(x, y, 1) });
                    else
                        lstAboveInfos.Add(new KHDDecoInfo { Id = id, Color = color, Direct = direct, Position = new Vector3(x, y, z + 1) });
                    if (x + configDeco.SizeX > sizeX)
                        sizeX = x + configDeco.SizeX;
                    if (y + configDeco.SizeY > sizeY)
                        sizeY = y + configDeco.SizeY;
                }
                    //Debug.LogError(string.Format("Id: {0}, Color: {1}, Pos: {2}/{3}/{4}, Direct: {5}", id, color, x, y, z, direct));
            }
            
            if(sizeX == int.MinValue || sizeY == int.MinValue)
            {
                Debug.LogError("Data is Wrong!");
                return;
            }
            _areaManager.ClearAllRooms();
            Debug.LogError(sizeX + "," + sizeY);
            _importDecoController.CreateARoom(0,Vector3.zero, new Vector3(sizeX, sizeY));
            _lstRoomController.Setup();
            //add Tile
            for(int x = 0; x < sizeX; x+=6)
            {
                for(int y = 0; y < sizeY; y+=6)
                {
                    if (!AddDeco(new KHDDecoInfo { Id = _defaultTileId, Position = new Vector3(x, y) }))
                        break;
                }
            }

            //add wall FL
            for (int y = 0; y < sizeY; y+=6)
            {
                if (!AddDeco(new KHDDecoInfo { Id = _defaultWallpaperId, Position = new Vector3(sizeX - 1, y, 1) }))
                    break;
            }

            //add wall FR

            for(int x = 0; x < sizeX - 6; x+=6)
            {
                if (!AddDeco(new KHDDecoInfo { Id = _defaultWallpaperId, Position = new Vector3(x, sizeY - 1, 1), Direct = IsoDirect.FR }))
                    break;
            }
            AddDeco(new KHDDecoInfo { Id = _defaultWallpaper1_5Id, Position = new Vector3(sizeX - 6, sizeY - 1, 1), Direct = IsoDirect.FR });

            foreach(var info in lstInfloorInfos)
            {
                AddDeco(info);
            }

            foreach (var info in lstAboveInfos)
            {
                AddDeco(info);
            }
        }

        bool AddDeco(KHDDecoInfo info)
        {
            var deco = _importDecoController.CreateDeco(info.Id, info.Color);
            if(deco == null)
            {
                Debug.LogError(string.Format("Fail At Id: {0} - Config Null", info.Id));
                return false;
            }
            deco.Info = new DecoInfo { Id = info.Id, Color = info.Color };
            deco.Position = info.Position;
            deco.WorldDirect = info.Direct;
            var decoEdit = deco.GetComponent<DecoEditDemo>();
            decoEdit.StartMove();
            decoEdit.EndMove();
            if (decoEdit.EditStatus != KHHEditStatus.Valid)
            {
                deco.Remove();
                Debug.LogError(string.Format("Fail At Id: {0}_{4}, Direct: {5}, Pos: {1}-{2}-{3}", info.Id, info.Position.x, info.Position.y, info.Position.z, info.Color, info.Direct));
                return false;
            }
            return true;
        }
    }
}

