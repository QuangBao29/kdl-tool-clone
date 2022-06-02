using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kawaii.IsoTools {
    public class IsoWorld
    {
		public enum TileTypes 
        {
			Isometric,
			UpDown
		}
		public static TileTypes TileType   = TileTypes.Isometric;
		public static float TileSize   = 0.16f;
		public static float TileHeight = 1.0f;

        public static Vector2 IsoToWorld(Vector3 pos) {
			switch ( TileType ) {
			case TileTypes.Isometric:
				return new Vector2(
					(pos.x - pos.y)* TileSize,
					((pos.x + pos.y) * 0.5f + pos.z*TileHeight)* TileSize) ;
			case TileTypes.UpDown:
				return new Vector2(
					pos.x,
					pos.y + pos.z) * TileSize;
			default:
				throw new UnityException("IsoWorld. Type is wrong!");
			}
		}

        public static Vector3 WorldToIso(Vector2 pos, float iso_z)
        {
            switch (TileType)
            {
                case TileTypes.Isometric:
                    {
                        var iso_pos = ScreenToIso(new Vector2(pos.x, pos.y - iso_z * TileSize * TileHeight));
                        iso_pos.z = iso_z;
                        return iso_pos;
                    }
                case TileTypes.UpDown:
                    {
                        var iso_pos = ScreenToIso(new Vector2(pos.x, pos.y - iso_z * TileSize));
                        iso_pos.z = iso_z;
                        return iso_pos;
                    }
                default:
                    throw new UnityException("IsoWorld. Type is wrong!");
            }
        }

        static Vector3 ScreenToIso(Vector2 pos) {
			switch ( TileType ) {
			case TileTypes.Isometric:
				return new Vector3(
					(pos.x * 0.5f + pos.y),
					(pos.y - pos.x * 0.5f),
					0.0f) / TileSize;
			case TileTypes.UpDown:
				return new Vector3(
					pos.x,
					pos.y,
					0.0f) / TileSize;
			default:
				throw new UnityException("IsoWorld. Type is wrong!");
			}
		}
		
		
		
    }
} 