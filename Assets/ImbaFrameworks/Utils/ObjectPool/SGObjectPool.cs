using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Imba.Utils
{
    public class SGObjectPool<T> where T : Component
    {
        private T _prefab;
        private Transform _container;

        private List<T> _allObjs = new List<T>();
        private Stack<T> _inActiveObjs = new Stack<T>();

        public SGObjectPool(T prefab, Transform container, List<T> preloads = null)
        {
            _prefab = prefab;
            _container = container;
            if (preloads != null)
            {
                foreach (var iter in preloads)
                {
                    _allObjs.Add(iter);
                    iter.gameObject.SetActive(false);
                    _inActiveObjs.Push(iter);
                }
            }
        }

        public T Get()
        {
            T result = null;
            if (_inActiveObjs.Count > 0)
            {
                result = _inActiveObjs.Pop();
                result.gameObject.SetActive(true);
            }
            else
            {
                result = SGUtils.InstantiateObject<T>(_prefab, _container);
                if (result != null)
                {
                    _allObjs.Add(result);
                    result.gameObject.SetActive(true);
                }
            }
            return result;
        }

        public void Remove(T obj)
        {
            if (obj == null)
                return;
            obj.transform.SetParent(_container);
            obj.gameObject.SetActive(false);
            _inActiveObjs.Push(obj);
        }

        public void RemoveAll()
        {
            foreach (var iter in _allObjs)
            {
                if (iter.gameObject.activeSelf)
                {
                    iter.gameObject.SetActive(false);
                    _inActiveObjs.Push(iter);
                }
            }
        }

    }

}
