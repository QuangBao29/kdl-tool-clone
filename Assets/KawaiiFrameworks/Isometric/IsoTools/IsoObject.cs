using UnityEngine;
using System;

namespace Kawaii.IsoTools {
	
	[ExecuteInEditMode]
	public class IsoObject : MonoBehaviour 
    {
        public int SortStep = 1;
		Vector3   _lastPosition  = Vector3.zero;
		Vector3   _lastTransform = Vector3.zero;
		
		[SerializeField]
		protected Vector3 _position = Vector3.zero;
		public Action<Vector3, Vector3> OnPositionChange;
        public Action<int> OnSortingOrderChange;
        public Action<string> OnSortingLayerChange;

		public virtual Vector3 Position {
			get { return _position; }
			set {
				
				Vector3 old = _position;
				_position = value;

                if (Alignment)
                    FixAlignment();
                else
                    FixTransform();
                OnPositionChange?.Invoke(old, _position);
			}
		}

        [SerializeField]
		protected Vector3 _size = Vector3.one;

        public Vector3 Size
        {
            get { return _size; }
            set
            {
                _size = value;
            }
        }

        public Vector3 CenterBottomIsoPosition
        {
            get
            {
                float centerBottomIsoPosX = Position.x + Size.x / 2;
                float centerBottomIsoPosY = Position.y + Size.y / 2;
                return new Vector3(centerBottomIsoPosX, centerBottomIsoPosY, Position.z);
            }
        }

        public Vector3 CenterIsoPosition
        {
            get
            {
                float centerBottomIsoPosX = Position.x + Size.x / 2;
                float centerBottomIsoPosY = Position.y + Size.y / 2;
                float centerBottomIsoPosZ = Position.z + Size.z / 2;
                return new Vector3(centerBottomIsoPosX, centerBottomIsoPosY, centerBottomIsoPosZ);
            }
        }

        [SerializeField]
		bool _alignment = true;

        public bool Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                if (Alignment)
                {
                    FixAlignment();
                }
                else
                {
                    FixTransform();
                }
            }
        }

        public Vector3 TilePosition {
			get {
				return new Vector3(
					Mathf.Round(Position.x),
					Mathf.Round(Position.y),
					Mathf.Round(Position.z));
			}
		}
		
		public void FixAlignment() {
			_position = TilePosition;
			FixTransform();
		}
		
		public void FixTransform() {
            Vector3 trans = IsoWorld.IsoToWorld(Position);
            trans.z = transform.position.z;
            transform.position = trans;
            _lastPosition = Position;
            _lastTransform = trans;
        }
		
		public void FixIsoPosition() {
            Vector2 trans = transform.position;
            Position = IsoWorld.WorldToIso(trans, Position.z);
        }
		
		void Awake() {
			_lastPosition = Position;
			_lastTransform = transform.position;
		}

        public virtual int SortingLayerOrder
        {
            get
            {
                return 0;
            }
            set
            {
                OnSortingOrderChange?.Invoke(value);
            }
        }

        public virtual string SortingLayerName
        {
            get
            {
                return "";
            }
            set
            {
                OnSortingLayerChange?.Invoke(value);
            }
        }

#if UNITY_EDITOR
        void Update() {
            //if(!Application.isPlaying)
            {
                if (_lastPosition != _position)
                    Position = _position;
                if (_lastTransform != transform.position)
                    FixIsoPosition();
                //if (lastSize != Size)
                    //lastSize = Size;
            }
        }
#endif
    }

}
