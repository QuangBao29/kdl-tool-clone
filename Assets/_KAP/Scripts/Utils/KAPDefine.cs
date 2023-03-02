using System.Collections;
using System.Collections.Generic;
using Kawaii.IsoTools.DecoSystem;
using Pathfinding.Serialization.JsonFx;
namespace KAP
{
    public enum AtlasType
    {
        Deco,
        RoomPreview,
    }

    public enum KAPDecoAreaFace
    {
        None = 0,
        Tile = 1,
        Carpet = 2,
        Indoor = 3,
        AboveIndoor = 4,
        WallHang = 5,
        //Floor = 6,
        Wallpaper = 7,
        //ShortWallpaper = 8,
        CrossTile = 9
    }

    public enum KAPDecoSortingLayerName
    {
        None = 0,
        Tile = 1,
        Carpet = 2,
        Indoor = 3
    }

    /// <summary>
    /// MainMenu Item Color
    /// </summary>
    public enum MainColorType
    {
        Grey = 0, // locked
        DarkGreen,
        Yellow,
        LimeGreen,
        Violet,
        Cyan,
        Pink,
    }

    public class KAPDefine
    {
        public static int DefaultStar = 5;
        public enum SceneName
        {
            scene_login = 0,
            scene_room = 1,
            scene_theme = 2,
        }
        public const string FormatMoney = "###,###,##0";

        // ======================================================
        #region ATLAS

        // map json atlas
        public const string roomMapAtlasName = "roommaps";
        public const string roomThemeMapAtlasName = "roomthememaps";

        // sprite atlas
        public const string UiCommonAtlasName = "commonicons";
        //public const string RoomPreviewAtlasName = "room";
        //public const string RoomThemeIconAtlasName = "roomtheme";
        public const string RoomTypeIconAtlasName = "roomtypeicons";

        // atlas url
        public const string DecoAtlasURL = "Atlas/Decos/";

        #endregion
        // ======================================================
        #region ROOM

        public const int DefaultRoomId = 100000;
        public const int DefaultRoomPlayId = 114000;
        public const int DefaultRoomPlayKDLID = 101005;
        public const int DefaultMansionID = 520024;
        public const string RoomMapURL = "Maps/Rooms/";

        #endregion
        // ======================================================
        #region THEME ROOM

        public const int DefaultRoomThemeId = 520010;
        public const string ThemeMapURL = "Maps/Themes/";

        #endregion
        // ======================================================
        #region WONDER

        public const int DefaultWonderId = 600000;
        public const string WonderMapURL = "Maps/Wonders/";

        #endregion

        #region ROOM CHALLENGE
        public const int DefaultRoomChallengeID = 120000;
        public const string RoomChallengeMapURL = "Maps/RoomChallenge/";
        #endregion
        // ======================================================
        // ======================================================
        #region DECO

        public const float DecoMaxIsoDisanceCanFillShadow = 0.5f;
        public const int DecoDefaultThemeId = 99000;
        public const int DecoShadowThemeId = 99900;

        #endregion
        // ======================================================
    }

    #region Deco System Define
    public enum DemoAreaFace
    {
        None = 0,
        Tile = 1,
        Carpet = 2,
        Indoor = 3,
        AboveIndoor = 4,
        WallHang = 5,
        Floor = 6,
        Wallpaper = 7,
        ShortWallpaper = 8,
        CrossTile = 9
    }

    public enum DemoSortingLayerName
    {
        None = 0,
        Tile = 1,
        Carpet = 2,
        Indoor = 3
    }

    public enum DemoColliderLayer
    {
        None = 0,
        //Floor = 8,
        Tile = 9,
        Carpet = 10,
        //Wall = 11,
        Wallpaper = 12,
        Indoor = 13,
    }

    public enum DemoDecoPrefab
    {
        None = 0,
        Indoor = 1,
        Tile = 2,
        Wallpaper = 3,
        ShortWallpaper = 4,
    }

    public class DecoInfo : IDecoInfo
    {
        public int Id;
        public int Color;
        public bool IsStatic;
        public string ShadowColor;
        public bool IsBubble;
        public bool IsUnpacking;

        public string ExportToJson(object param)
        {
            var data = new Dictionary<string, object>();
            if (Id > 0)
                data["Id"] = Id;
            if (Color > 0)
                data["Color"] = Color;
            if (IsStatic)
                data["IsStatic"] = IsStatic;
            if (IsBubble)
                data["IsBubble"] = IsBubble;
            if (IsUnpacking)
                data["IsUnpacking"] = IsUnpacking;

            if (!string.IsNullOrEmpty(ShadowColor))
            {
                data["ShadowColor"] = ShadowColor;
            }

            return data.Count > 0 ? JsonWriter.Serialize(data) : null;
        }
    }
    #endregion
}