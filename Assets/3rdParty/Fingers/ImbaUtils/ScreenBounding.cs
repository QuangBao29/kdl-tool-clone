using UnityEngine;
namespace Fingers
{
    public class ScreenBounding : MonoBehaviour
    {
#if UNITY_EDITOR
        public Transform tranformMin;
        public Transform tranformMax;
#endif
        public Vector2 roomSize;

        [HideInInspector]
        public Vector3 Min;

        [HideInInspector]
        public Vector3 Max;


        void Start()
        {
            CalcMinMax();
        }

        void OnEnable()
        {
            CalcMinMax();
        }

        void OnValidate()
        {
            CalcMinMax();
        }

        public void SetCenter(Vector2 centerPos)
        {
            transform.position = centerPos;
        }
        public void CalcMinMax()
        {
            Vector3 mi = new Vector3(roomSize.x * -0.5f, roomSize.y * -0.5f + 1, 0);
            Vector3 mx = new Vector3(roomSize.x * 0.5f, roomSize.y * 0.5f + 1, 0);
#if UNITY_EDITOR
            if (!tranformMin)
                tranformMin = transform.Find("Min");
            if (!tranformMax)
                tranformMax = transform.Find("Max");
            tranformMin.localPosition = mi;
            tranformMax.localPosition = mx;
#endif
            Min = transform.TransformPoint(mi);
            Max = transform.TransformPoint(mx);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Min, 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(Max, 0.5f);

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(Min, Max);

            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, roomSize);
        }
#endif
    }

}


