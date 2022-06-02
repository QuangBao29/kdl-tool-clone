#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kawaii.ResourceManager;
using KAP.Config;
using KAP.ToolCreateMap;
namespace KAP.Tools
{
    public class CreateConfigHiveShop : Editor
    {
        const string _configDecoFilePath = "/_KAP/_GameResources/Configs/Deco/ConfigDeco.csv";
        const string _configHiveShopFilePath = "/_KAP/_GameResources/Configs/Hive/ConfigHiveShop.csv";
        const string _configHiveThemeUnlockFilePath = "/_KAP/_GameResources/Configs/Hive/ConfigHiveThemeUnlock.csv";
        const string _configDecoGachaFilePath = "/_KAP/_GameResources/Configs/Gacha/ConfigGachaRate.csv";
        const string _configDecoGachaFindoutFilePath = "/_KAP/_GameResources/Configs/Gacha/ConfigGachaFindout.csv";


        [MenuItem("Tools/KAP/Create ConfigHiveShop.csv", false, 2)]
        public static void Create()
        {
            var txtConfigDeco = FileSaving.Load(Application.dataPath + _configDecoFilePath);
            var configDeco = new ConfigDeco();
            configDeco.LoadFromString(txtConfigDeco);

            var txtConfigHiveShop = FileSaving.Load(Application.dataPath + _configHiveShopFilePath);
            var configHiveShop = new ConfigHiveShop();
            configHiveShop.LoadFromString(txtConfigHiveShop);

            var txtConfigHiveThemeUnlock = FileSaving.Load(Application.dataPath + _configHiveThemeUnlockFilePath);
            var configHiveThemeUnlock = new ConfigHiveThemeUnlock();
            configHiveThemeUnlock.LoadFromString(txtConfigHiveThemeUnlock);

            List<ConfigHiveShopRecord> lstHiveShopRecords = new List<ConfigHiveShopRecord>();

            var txtConfigDecoGacha = FileSaving.Load(Application.dataPath + _configDecoGachaFilePath);
            var configDecoGacha = new ConfigDecoGacha();
            configDecoGacha.LoadFromString(txtConfigDecoGacha);

            // Added for Findout Mode
            var txtConfigDecoGachaFindout = FileSaving.Load(Application.dataPath + _configDecoGachaFindoutFilePath);
            var configDecoGachaFindout = new ConfigDecoGachaFindout();
            configDecoGachaFindout.LoadFromString(txtConfigDecoGachaFindout);

            ConfigHiveShopRecord config = null;
            foreach (var record in configDeco.Records)
            {
                var decoType = (ListDecoFilter.DecoType)(record.Id / 10000);
                var themeConfig = configHiveThemeUnlock.GetThemeById(record.ThemeId);

                if (themeConfig != null)
                {
                    var decoGachaRecord = configDecoGacha.GetById(record.Id);
                    var decoGachaFindoutRecord = configDecoGachaFindout.GetById(record.Id);
                    if (decoGachaRecord != null && decoGachaRecord.Rate > 0)
                    {
                        //Debug.LogError("Dealing with Config Gacha Normal" + record.Id);

                        config = new ConfigHiveShopRecord
                        {
                            Id = record.Id,
                            Coin = 0,
                            Gem = 0,
                            PriceType = HiveShopPriceType.Gacha,
                            Enable = true,
                            Ad = false
                        };
                        lstHiveShopRecords.Add(config);
                        continue;
                    } else if (decoGachaFindoutRecord != null && decoGachaFindoutRecord.Rate > 0)
                    {
                        //Debug.LogError("Dealing with Config Gacha Findout" + record.Id);
                        config = new ConfigHiveShopRecord
                        {
                            Id = record.Id,
                            Coin = 0,
                            Gem = 0,
                            PriceType = HiveShopPriceType.Findout,
                            Enable = true,
                            Ad = false
                            
                        };
                        lstHiveShopRecords.Add(config);
                        continue;
                    }
                }

                switch (decoType)
                {
                    case ListDecoFilter.DecoType.None:
                    case ListDecoFilter.DecoType.Basic:
                    case ListDecoFilter.DecoType.VerHalfTile:
                    case ListDecoFilter.DecoType.HorHalfTile:
                    case ListDecoFilter.DecoType.Wallpaper_5_2:
                    case ListDecoFilter.DecoType.Wallpaper_5_6:
                    case ListDecoFilter.DecoType.Wallpaper_5_9:
                    case ListDecoFilter.DecoType.Wallpaper_5_18:
                    case ListDecoFilter.DecoType.Wallpaper_6_2:
                    case ListDecoFilter.DecoType.CrossWallpaper_2:
                    case ListDecoFilter.DecoType.CrossWallpaper_6:
                    case ListDecoFilter.DecoType.CrossWallpaper_9:
                    case ListDecoFilter.DecoType.CrossWallpaper_18:
                        //disable in shop
                        break;
                    default:
                        if(themeConfig != null)
                        {
                            int coin = CalculateCoinPrice(record, themeConfig.DecoPriceFactor);
                            //Debug.LogError("Factor " + themeConfig.DecoPriceFactor + "Coin: " + coin);

                            if(coin > 0)
                            {
                                int gem = CalculateGemPrice(coin);
                                var oldConfig = configHiveShop.GetDecoById(record.Id);
                                config = new ConfigHiveShopRecord
                                {
                                    Id = record.Id,
                                    Coin = coin,
                                    Gem = gem,
                                    PriceType = oldConfig != null ? oldConfig.PriceType : HiveShopPriceType.Coin,
                                    Enable = oldConfig != null ? oldConfig.Enable : true,
                                    Ad = oldConfig.Ad
                                };
                                lstHiveShopRecords.Add(config);
                            }
                        }
                        break;
                }


            }

            List<string> lstVariables = new List<string> { "Id", "Coin", "Gem", "PriceType", "Enable", "Ad" };
            string txt = "";
            for (var i = 0; i < lstVariables.Count - 1; i++)
            {
                txt += lstVariables[i] + "\t";
            }
            txt += lstVariables[lstVariables.Count - 1] + "\n";

            foreach (var record in lstHiveShopRecords)
            {
                txt += record.Id + "\t" + record.Coin + "\t" + record.Gem + "\t" + record.PriceType + "\t" + record.Enable + "\t" + record.Ad + "\n";
            }


            FileSaving.Save(Application.dataPath + _configHiveShopFilePath, txt);
        }

        static int CalculateCoinPrice(ConfigDecoRecord record, float themeFactor)
        {
            float baseCost = 0;
            if (record.IsWallHang)
            {
                baseCost = 10*2;
                return Mathf.CeilToInt(record.SizeY * record.SizeZ * baseCost * themeFactor);
            }
            if(record.ColliderLayer == DemoColliderLayer.Carpet)
            {
                baseCost = 12 * 2;
                return Mathf.CeilToInt(record.SizeX * record.SizeY * baseCost * themeFactor);
            }
            if(record.ColliderLayer == DemoColliderLayer.Indoor)
            {
                baseCost = 7 * 2;
                return Mathf.CeilToInt(record.SizeX * record.SizeY * record.SizeZ * baseCost * themeFactor);
            }
            if(record.ColliderLayer == DemoColliderLayer.Tile)
            {
                baseCost = 300 * 2;
                return Mathf.CeilToInt(baseCost * themeFactor);
            }
            if(record.ColliderLayer == DemoColliderLayer.Wallpaper)
            {
                if(record.SizeZ >=18) //similar tile
                {
                    baseCost = 300 * 2;
                    return Mathf.CeilToInt(baseCost * themeFactor);
                }
                //similar indoor
                baseCost = 7 * 2;
                return Mathf.CeilToInt(record.SizeX * record.SizeY * record.SizeZ * baseCost * themeFactor);
            }

            return 0;
        }

        static int CalculateGemPrice(int coinPrice)
        {
            if (coinPrice < 1000)
                return Mathf.CeilToInt(coinPrice / 200f);
            if (coinPrice < 2000)
                return Mathf.CeilToInt(coinPrice / 250f);
            if (coinPrice < 3000)
                return Mathf.CeilToInt(coinPrice / 300f);
            if (coinPrice < 5000)
                return Mathf.CeilToInt(coinPrice / 350f);
            if (coinPrice < 7000)
                return Mathf.CeilToInt(coinPrice / 400f);
            if (coinPrice < 9000)
                return Mathf.CeilToInt(coinPrice / 450f);
            return Mathf.CeilToInt(coinPrice / 500f);
        }
    }

}
#endif