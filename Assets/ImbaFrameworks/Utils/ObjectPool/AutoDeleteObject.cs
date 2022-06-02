using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Imba.Utils
{
    public class AutoDeleteObject : MonoBehaviour
    {
        public SGObjectPool<AutoDeleteObject> ObjectPool;
        [SerializeField]
        protected float _timeLife;

        private void OnEnable()
        {
            if(_timeLife > 0)
                Invoke("Delete", _timeLife);
        }

        protected virtual void Delete()
        {
            if (ObjectPool != null)
                ObjectPool.Remove(this);
            else
                Destroy(gameObject);
        }

        public virtual void ForceDelete()
        {
            CancelInvoke();
            Delete();
        }
    }
}


