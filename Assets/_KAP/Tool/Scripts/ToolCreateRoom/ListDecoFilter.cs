using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KAP.Config;
using UnityEngine.Events;

namespace KAP.ToolCreateMap
{
    public class ListDecoFilter : MonoBehaviour
    {
        [SerializeField]
        private DecoThemeOptions _themeOptions = null;
        [SerializeField]
        private InputField _inputSearch = null;
        [SerializeField]
        private Toggle _toggerIndoor = null;
        [SerializeField]
        private Toggle _toggleWallHang = null;
        [SerializeField]
        private Toggle _toggleTile = null;
        [SerializeField]
        private Toggle _toggleWallpaper = null;
        [SerializeField]
        private Toggle _toggleCarpet = null;

        [SerializeField]
        private Dropdown _dropdownTab = null;
        [SerializeField]
        private Dropdown _dropdownSubTab = null;

        [SerializeField]
        private UnityEvent _onChangeFilter = null;

        private void Start()
        {
            _themeOptions.Value = 0;
            if(_dropdownTab != null)
            {
                _dropdownTab.options.Clear();
                foreach (var obj in Enum.GetValues(typeof(Tab)))
                {
                    var tab = (Tab)obj;
                    _dropdownTab.options.Add(new Dropdown.OptionData(tab.ToString()));
                }
            }
        }

        public void SetupThemeOptions(ReadOnlyCollection<ConfigDecoThemeRecord> lstRecords)
        {
            _themeOptions.Setup(lstRecords);
        }

        public List<ConfigDecoRecord> Filter(ReadOnlyCollection<ConfigDecoRecord> allRecords)
        {
            List<ConfigDecoRecord> lstResults = new List<ConfigDecoRecord>();
            if (allRecords == null)
                return lstResults;

            foreach(var record in allRecords)
            {
                if (FilterARecord(record))
                    lstResults.Add(record);
            }
            return lstResults;
        }

        public List<ConfigDecoRecord> Filter(List<ConfigDecoRecord> allRecords)
        {
            List<ConfigDecoRecord> lstResults = new List<ConfigDecoRecord>();
            if (allRecords == null)
                return lstResults;

            foreach (var record in allRecords)
            {
                if (FilterARecord(record))
                    lstResults.Add(record);
            }
            return lstResults;
        }

        bool FilterARecord(ConfigDecoRecord record)
        {
            if (_themeOptions.Value > 0 && record.ThemeId != _themeOptions.Value)
                return false;
            if (!string.IsNullOrEmpty(_inputSearch.text))
            {
                if (!record.Id.ToString().Contains(_inputSearch.text) && !record.Name.Contains(_inputSearch.text))
                    return false;
            }
          
            if (!_toggerIndoor.isOn)
            {
                if (!record.IsWallHang && record.ColliderLayer == DemoColliderLayer.Indoor)
                    return false;
            }
            if (!_toggleWallHang.isOn)
            {
                if (record.IsWallHang)
                    return false;
            }
            if (!_toggleTile.isOn)
            {
                if (record.ColliderLayer == DemoColliderLayer.Tile)
                    return false;
            }
            if (!_toggleWallpaper.isOn)
            {
                if (record.ColliderLayer == DemoColliderLayer.Wallpaper)
                    return false;
            }
            if (!_toggleCarpet.isOn)
            {
                if (record.ColliderLayer == DemoColliderLayer.Carpet)
                    return false;
            }

            var decoType = (DecoType)(record.Id / 10000);
            if(CurrentTab == Tab.None)
                return true;
            List<DecoType> allValidTypes = new List<DecoType>();
            GetTypesBySubTab(CurrentSubTab, ref allValidTypes);
            if (!allValidTypes.Contains(decoType))
                return false;
            return true;
        }

        public void OnFilterChangeValue()
        {
            _onChangeFilter?.Invoke();
        }

        #region Tab/SubTab
        public enum DecoType
        {
            None = 0,
            Basic = 100,
            TwinBed = 101,
            Table = 102,
            Chair = 103,
            Closet = 104,
            BathFloorAcc = 105,
            BathWallAcc = 106,
            ShelfFloor = 107,
            ShelfWall = 108,
            ApplianceFloor = 109,
            ApplianceWall = 110,
            PlantFloor = 111,
            PlantWall = 112,
            LightFloor = 113,
            LightWall = 114,
            Barrier = 115,
            Carpet = 116,
            ArtFloor = 117,
            ArtWall = 118,
            WindowIn = 119,
            WindowOut = 123,
            Door = 124,
            OuterWall = 125,
            Roof = 126,
            Yard = 127,
            YardDecor = 128,
            BathTub = 129,
            Lavabo = 131,
            Toilet = 133,
            //CrossWall = 134,
            ExteriorTop = 135,
            ExteriorBottom = 136,
            Vanity = 137,
            KitDeviceFloor = 138,
            KitDeviceWall = 139,
            KitFurFloor = 140,
            KitFurWall = 141,
            KitExtraFloor = 142,
            KitExtraWall = 143,
            TableLow = 144,
            Sofa = 145,
            SingleBed = 146,
            Cradle = 147,
            Bedside = 148,
            Fridge = 149,
            Cooker = 150,
            DrinkMaker = 151,
            Worktable = 152,
            KitSink = 153,
            CupboardFloor = 154,
            CupboardWall = 155,
            FoodstandFloor = 156,
            FoodstandWall = 157,
            DrinkstandFloor = 158,
            DrinkstandWall = 159,
            Food = 160,
            Drink = 161,
            Desk = 162,
            AccentTable = 163,
            ConsoleTable = 164,
            Armchair = 165,
            Chaise = 166,
            Footstool = 167,
            Bench = 168,
            Cushion = 169,
            Swing = 170,
            CabinetFloor = 171,
            CabinetWall = 172,
            SideboardFloor = 173,
            SideboardWall = 174,
            BookcaseFloor = 175,
            BookcaseWall = 176,
            TVFloor = 177,
            TVWall = 178,
            PhoneFloor = 179,
            PhoneWall = 180,
            CoolerFloor = 181,
            CoolerWall = 182,
            Computer = 183,
            Radio = 184,
            SpeakerFloor = 185,
            SpeakerWall = 186,
            ClockFloor = 187,
            ClockWall = 188,
            Vacuum = 189,
            WashingMachine = 190,

            GameMachine = 193,
            BeautyMachine = 194,
            KidFloor = 195,
            MusicFloor = 196,
            HeaterFloor = 197,
            MirrorFloor = 198,
            SportFloor = 199,
            Beauty = 200,
            Book = 201,
            Stuff = 202,
            KidWall = 203,
            MusicWall = 204,
            HeaterWall = 205,
            MirrorWall = 206,
            SportWall = 207,
            Shower = 208,
            Soap = 209,
            TowelFloor = 210,
            TowelWall = 211,
            BathFurFloor = 212,
            BathFurWall = 213,
            Slipper = 214,
            Screen = 215,
            Buildin = 216,
            Walkthrough = 217,
            ShowroomText = 218,
            Stair = 219,
            Tile = 220,
            HorHalfTile = 221,
            VerHalfTile = 222,

            Wallpaper_6_18 = 230,
            Wallpaper_6_9 = 231,
            Wallpaper_6_6 = 232,
            Wallpaper_6_2 = 233,
            Wallpaper_5_18 = 240,
            Wallpaper_5_9 = 241,
            Wallpaper_5_6 = 242,
            Wallpaper_5_2 = 243,
            CrossWallpaper_18 = 250,
            CrossWallpaper_9 = 251,
            CrossWallpaper_6 = 252,
            CrossWallpaper_2 = 253
        }
        public enum Tab
        {
            None = 0,
            Bedroom = 1,
            Kitchen = 2,
            InteriorDecor = 3,
            Bathroom = 4,
            Overview = 5
        }

        public enum SubTab
        {
            None = 0,
            AllBedroom = 1,
            Bed = 2,
            Closet = 3,
            Vanity = 4,

            AllKitchen = 11,
            KitDevice = 12,
            KitFurniture = 13,
            KitExtra = 14,

            AllInterior = 21,
            Table = 22,
            Chair = 23,
            Shelf = 24,
            Lighting = 25,
            Appliance = 26,
            FloorDecor = 27,
            WallDecor = 28,
            Plant = 29,

            AllBathroom = 31,
            Bathtub = 32,
            Lavabo = 33,
            Toilet = 34,
            BathExtra = 35,

            AllOverview = 41,
            Partitions = 42,
            Carpet = 43,
            Wallpaper = 44,
            Tile = 45,

            Bedside = 84,
        }

        public void GetTypesBySubTab(SubTab subtab, ref List<DecoType> types)
        {
            if (types == null)
                types = new List<DecoType>();
            switch (subtab)
            {
                case SubTab.AllBedroom:
                    types.Add(DecoType.TwinBed);
                    types.Add(DecoType.SingleBed);
                    types.Add(DecoType.Cradle);
                    types.Add(DecoType.Bedside);
                    types.Add(DecoType.Closet);
                    types.Add(DecoType.Vanity);
                    break;
                case SubTab.Bed:
                    types.Add(DecoType.TwinBed);
                    types.Add(DecoType.SingleBed);
                    types.Add(DecoType.Cradle);
                    break;
                case SubTab.Bedside:
                    types.Add(DecoType.Bedside);
                    break;
                case SubTab.Closet:
                    types.Add(DecoType.Closet);
                    break;
                case SubTab.Vanity:
                    types.Add(DecoType.Vanity);
                    break;
                case SubTab.AllKitchen:
                    types.Add(DecoType.KitDeviceFloor);
                    types.Add(DecoType.KitDeviceWall);
                    types.Add(DecoType.Fridge);
                    types.Add(DecoType.Cooker);
                    types.Add(DecoType.DrinkMaker);

                    types.Add(DecoType.KitFurFloor);
                    types.Add(DecoType.KitFurWall);
                    types.Add(DecoType.Worktable);
                    types.Add(DecoType.KitSink);
                    types.Add(DecoType.CupboardFloor);
                    types.Add(DecoType.CupboardWall);
                    types.Add(DecoType.FoodstandFloor);
                    types.Add(DecoType.FoodstandWall);
                    types.Add(DecoType.DrinkstandFloor);
                    types.Add(DecoType.DrinkstandWall);

                    types.Add(DecoType.KitExtraFloor);
                    types.Add(DecoType.KitExtraWall);
                    types.Add(DecoType.Food);
                    types.Add(DecoType.Drink);
                    break;
                case SubTab.KitDevice:
                    types.Add(DecoType.KitDeviceFloor);
                    types.Add(DecoType.KitDeviceWall);
                    types.Add(DecoType.Fridge);
                    types.Add(DecoType.Cooker);
                    types.Add(DecoType.DrinkMaker);
                    break;
                case SubTab.KitFurniture:
                    types.Add(DecoType.KitFurFloor);
                    types.Add(DecoType.KitFurWall);
                    types.Add(DecoType.Worktable);
                    types.Add(DecoType.KitSink);
                    types.Add(DecoType.CupboardFloor);
                    types.Add(DecoType.CupboardWall);
                    types.Add(DecoType.FoodstandFloor);
                    types.Add(DecoType.FoodstandWall);
                    types.Add(DecoType.DrinkstandFloor);
                    types.Add(DecoType.DrinkstandWall);
                    break;
                case SubTab.KitExtra:
                    types.Add(DecoType.KitExtraFloor);
                    types.Add(DecoType.KitExtraWall);
                    types.Add(DecoType.Food);
                    types.Add(DecoType.Drink);
                    break;
                case SubTab.AllInterior:
                    types.Add(DecoType.Table);
                    types.Add(DecoType.TableLow);
                    types.Add(DecoType.Desk);
                    types.Add(DecoType.AccentTable);
                    types.Add(DecoType.ConsoleTable);

                    types.Add(DecoType.Chair);
                    types.Add(DecoType.Sofa);
                    types.Add(DecoType.Armchair);
                    types.Add(DecoType.Chaise);
                    types.Add(DecoType.Footstool);
                    types.Add(DecoType.Bench);
                    types.Add(DecoType.Cushion);
                    types.Add(DecoType.Swing);

                    types.Add(DecoType.ShelfFloor);
                    types.Add(DecoType.ShelfWall);
                    types.Add(DecoType.CabinetFloor);
                    types.Add(DecoType.CabinetWall);
                    types.Add(DecoType.SideboardFloor);
                    types.Add(DecoType.SideboardWall);
                    types.Add(DecoType.BookcaseFloor);
                    types.Add(DecoType.BookcaseWall);

                    types.Add(DecoType.LightFloor);
                    types.Add(DecoType.LightWall);

                    types.Add(DecoType.ApplianceFloor);
                    types.Add(DecoType.ApplianceWall);
                    types.Add(DecoType.TVFloor);
                    types.Add(DecoType.TVWall);
                    types.Add(DecoType.PhoneFloor);
                    types.Add(DecoType.PhoneWall);
                    types.Add(DecoType.CoolerFloor);
                    types.Add(DecoType.CoolerWall);
                    types.Add(DecoType.Computer);
                    types.Add(DecoType.Radio);
                    types.Add(DecoType.SpeakerFloor);
                    types.Add(DecoType.SpeakerWall);
                    types.Add(DecoType.ClockFloor);
                    types.Add(DecoType.ClockWall);
                    types.Add(DecoType.Vacuum);
                    types.Add(DecoType.WashingMachine);
                    types.Add(DecoType.GameMachine);
                    types.Add(DecoType.BeautyMachine);

                    types.Add(DecoType.ArtFloor);
                    types.Add(DecoType.KidFloor);
                    types.Add(DecoType.MusicFloor);
                    types.Add(DecoType.HeaterFloor);
                    types.Add(DecoType.MirrorFloor);
                    types.Add(DecoType.SportFloor);
                    types.Add(DecoType.Beauty);
                    types.Add(DecoType.Book);
                    types.Add(DecoType.Stuff);

                    types.Add(DecoType.ArtWall);
                    types.Add(DecoType.WindowIn);
                    types.Add(DecoType.KidWall);
                    types.Add(DecoType.MusicWall);
                    types.Add(DecoType.HeaterWall);
                    types.Add(DecoType.MirrorWall);
                    types.Add(DecoType.SportWall);

                    types.Add(DecoType.PlantFloor);
                    types.Add(DecoType.PlantWall);
                    break;
                case SubTab.Table:
                    types.Add(DecoType.Table);
                    types.Add(DecoType.TableLow);
                    types.Add(DecoType.Desk);
                    types.Add(DecoType.AccentTable);
                    types.Add(DecoType.ConsoleTable);
                    break;
                case SubTab.Chair:
                    types.Add(DecoType.Chair);
                    types.Add(DecoType.Sofa);
                    types.Add(DecoType.Armchair);
                    types.Add(DecoType.Chaise);
                    types.Add(DecoType.Footstool);
                    types.Add(DecoType.Bench);
                    types.Add(DecoType.Cushion);
                    types.Add(DecoType.Swing);
                    break;
                case SubTab.Shelf:
                    types.Add(DecoType.ShelfFloor);
                    types.Add(DecoType.ShelfWall);
                    types.Add(DecoType.CabinetFloor);
                    types.Add(DecoType.CabinetWall);
                    types.Add(DecoType.SideboardFloor);
                    types.Add(DecoType.SideboardWall);
                    types.Add(DecoType.BookcaseFloor);
                    types.Add(DecoType.BookcaseWall);
                    break;
                case SubTab.Lighting:
                    types.Add(DecoType.LightFloor);
                    types.Add(DecoType.LightWall);
                    break;
                case SubTab.Appliance:
                    types.Add(DecoType.ApplianceFloor);
                    types.Add(DecoType.ApplianceWall);
                    types.Add(DecoType.TVFloor);
                    types.Add(DecoType.TVWall);
                    types.Add(DecoType.PhoneFloor);
                    types.Add(DecoType.PhoneWall);
                    types.Add(DecoType.CoolerFloor);
                    types.Add(DecoType.CoolerWall);
                    types.Add(DecoType.Computer);
                    types.Add(DecoType.Radio);
                    types.Add(DecoType.SpeakerFloor);
                    types.Add(DecoType.SpeakerWall);
                    types.Add(DecoType.ClockFloor);
                    types.Add(DecoType.ClockWall);
                    types.Add(DecoType.Vacuum);
                    types.Add(DecoType.WashingMachine);
                    types.Add(DecoType.GameMachine);
                    types.Add(DecoType.BeautyMachine);
                    break;
                case SubTab.FloorDecor:
                    types.Add(DecoType.ArtFloor);
                    types.Add(DecoType.KidFloor);
                    types.Add(DecoType.MusicFloor);
                    types.Add(DecoType.HeaterFloor);
                    types.Add(DecoType.MirrorFloor);
                    types.Add(DecoType.SportFloor);
                    types.Add(DecoType.Beauty);
                    types.Add(DecoType.Book);
                    types.Add(DecoType.Stuff);
                    break;
                case SubTab.WallDecor:
                    types.Add(DecoType.ArtWall);
                    types.Add(DecoType.WindowIn);
                    types.Add(DecoType.KidWall);
                    types.Add(DecoType.MusicWall);
                    types.Add(DecoType.HeaterWall);
                    types.Add(DecoType.MirrorWall);
                    types.Add(DecoType.SportWall);
                    break;
                case SubTab.Plant:
                    types.Add(DecoType.PlantFloor);
                    types.Add(DecoType.PlantWall);
                    break;
                case SubTab.AllBathroom:
                    types.Add(DecoType.BathTub);
                    types.Add(DecoType.Shower);
                    types.Add(DecoType.Lavabo);
                    types.Add(DecoType.Toilet);
                    types.Add(DecoType.BathFloorAcc);
                    types.Add(DecoType.BathWallAcc);
                    types.Add(DecoType.Soap);
                    types.Add(DecoType.TowelFloor);
                    types.Add(DecoType.TowelWall);
                    types.Add(DecoType.BathFurFloor);
                    types.Add(DecoType.BathFurWall);
                    types.Add(DecoType.Slipper);
                    break;
                case SubTab.Bathtub:
                    types.Add(DecoType.BathTub);
                    types.Add(DecoType.Shower);
                    break;
                case SubTab.Lavabo:
                    types.Add(DecoType.Lavabo);
                    break;
                case SubTab.Toilet:
                    types.Add(DecoType.Toilet);
                    break;
                case SubTab.BathExtra:
                    types.Add(DecoType.BathFloorAcc);
                    types.Add(DecoType.BathWallAcc);
                    types.Add(DecoType.Soap);
                    types.Add(DecoType.TowelFloor);
                    types.Add(DecoType.TowelWall);
                    types.Add(DecoType.BathFurFloor);
                    types.Add(DecoType.BathFurWall);
                    types.Add(DecoType.Slipper);
                    break;
                case SubTab.AllOverview:
                    types.Add(DecoType.Stair);
                    types.Add(DecoType.Barrier);
                    types.Add(DecoType.Screen);
                    types.Add(DecoType.Buildin);
                    types.Add(DecoType.Walkthrough);
                    types.Add(DecoType.Carpet);
                    types.Add(DecoType.Wallpaper_5_2);
                    types.Add(DecoType.Wallpaper_5_6);
                    types.Add(DecoType.Wallpaper_5_9);
                    types.Add(DecoType.Wallpaper_5_18);
                    types.Add(DecoType.Wallpaper_6_2);
                    types.Add(DecoType.Wallpaper_6_6);
                    types.Add(DecoType.Wallpaper_6_9);
                    types.Add(DecoType.Wallpaper_6_18);
                    types.Add(DecoType.CrossWallpaper_2);
                    types.Add(DecoType.CrossWallpaper_6);
                    types.Add(DecoType.CrossWallpaper_9);
                    types.Add(DecoType.CrossWallpaper_18);
                    types.Add(DecoType.Tile);
                    types.Add(DecoType.HorHalfTile);
                    types.Add(DecoType.VerHalfTile);
                    break;
                case SubTab.Partitions:
                    types.Add(DecoType.Stair);
                    types.Add(DecoType.Barrier);
                    types.Add(DecoType.Screen);
                    types.Add(DecoType.Buildin);
                    types.Add(DecoType.Walkthrough);
                    break;
                case SubTab.Carpet:
                    types.Add(DecoType.Carpet);
                    break;
                case SubTab.Wallpaper:
                    types.Add(DecoType.Wallpaper_5_2);
                    types.Add(DecoType.Wallpaper_5_6);
                    types.Add(DecoType.Wallpaper_5_9);
                    types.Add(DecoType.Wallpaper_5_18);
                    types.Add(DecoType.Wallpaper_6_2);
                    types.Add(DecoType.Wallpaper_6_6);
                    types.Add(DecoType.Wallpaper_6_9);
                    types.Add(DecoType.Wallpaper_6_18);
                    types.Add(DecoType.CrossWallpaper_2);
                    types.Add(DecoType.CrossWallpaper_6);
                    types.Add(DecoType.CrossWallpaper_9);
                    types.Add(DecoType.CrossWallpaper_18);
                    break;
                case SubTab.Tile:
                    types.Add(DecoType.Tile);
                    types.Add(DecoType.HorHalfTile);
                    types.Add(DecoType.VerHalfTile);
                    break;
            }
        }

        public List<SubTab> GetSubTabsByTab(Tab tab)
        {
            List<SubTab> result = new List<SubTab>();
            switch(tab)
            {
                case Tab.None:
                    break;
                case Tab.Bedroom:
                    result.Add(SubTab.AllBedroom);
                    result.Add(SubTab.Bed);
                    result.Add(SubTab.Closet);
                    result.Add(SubTab.Vanity);
                    result.Add(SubTab.Bedside);
                    break;
                case Tab.Kitchen:
                    result.Add(SubTab.AllKitchen);
                    result.Add(SubTab.KitDevice);
                    result.Add(SubTab.KitFurniture);
                    result.Add(SubTab.KitExtra);
                    break;
                case Tab.InteriorDecor:
                    result.Add(SubTab.AllInterior);
                    result.Add(SubTab.Table);
                    result.Add(SubTab.Chair);
                    result.Add(SubTab.Shelf);
                    result.Add(SubTab.Lighting);
                    result.Add(SubTab.Appliance);
                    result.Add(SubTab.FloorDecor);
                    result.Add(SubTab.WallDecor);
                    result.Add(SubTab.Plant);
                    break;
                case Tab.Bathroom:
                    result.Add(SubTab.AllBathroom);
                    result.Add(SubTab.Bathtub);
                    result.Add(SubTab.Lavabo);
                    result.Add(SubTab.Toilet);
                    result.Add(SubTab.BathExtra);
                    break;
                case Tab.Overview:
                    result.Add(SubTab.AllOverview);
                    result.Add(SubTab.Partitions);
                    result.Add(SubTab.Carpet);
                    result.Add(SubTab.Wallpaper);
                    result.Add(SubTab.Tile);
                    break;
            }
            return result;
        }

        public Tab CurrentTab
        {
            get
            {
                var tab = Tab.None;
                if (_dropdownTab == null)
                    return tab;
                var valueStr = _dropdownTab.captionText.text;

                Enum.TryParse<Tab>(valueStr, out tab);
                return tab;
            }
        }

        public SubTab CurrentSubTab
        {
            get
            {
                var subTab = SubTab.None;
                if (_dropdownSubTab == null)
                    return subTab;
                var valueStr = _dropdownSubTab.captionText.text;
                Enum.TryParse<SubTab>(valueStr, out subTab);
                return subTab;
            }
        }

        public void OnDropdownTabChangeValue()
        {
            _dropdownSubTab.options.Clear();
            var lstSubtabs = GetSubTabsByTab(CurrentTab);
            foreach(var subTab in lstSubtabs)
            {
                _dropdownSubTab.options.Add(new Dropdown.OptionData(subTab.ToString()));
            }
            if (lstSubtabs.Count > 0)
            {
                _dropdownSubTab.value = 0;
                _dropdownSubTab.captionText.text = lstSubtabs[0].ToString();
            }
            else
                _dropdownSubTab.captionText.text = "";
            _onChangeFilter?.Invoke();
        }

        public void OnDropdownSubtabChangeValue()
        {
            _onChangeFilter?.Invoke();
        }
        #endregion
    }

}
