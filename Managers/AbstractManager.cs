using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public abstract class AbstractManager<T> : MonoBehaviour
    {
        protected List<T> subscribers = new List<T>();
        protected List<T> removeSubscribers = new List<T>();
        protected List<T> addSubscribers = new List<T>();
        protected IEnumerator managerCoroutine;
        public float refreshRate = 0.1f;

        protected virtual void Start()
        {
            managerCoroutine = ManagerCoroutine();
            StartCoroutine(managerCoroutine);
        }

        protected virtual IEnumerator ManagerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshRate);
                AddRegisteredSubscribers();
                ClearUnregisteredSubscribers();

                foreach (T subscriber in subscribers)
                {
                    Debug.Log(":::" + subscriber);
                    PerformTask(subscriber);
                }
            }
        }

        protected virtual void PerformTask(T subscriber) { }

        public void RegisterSubscriber(T subscriber)
        {
            if (subscribers.Contains(subscriber))
                return;
            addSubscribers.Add(subscriber);
        }

        public void UnregisterSubscriber(T subscriber)
        {
            removeSubscribers.Add(subscriber);
        }

        protected virtual void ClearUnregisteredSubscribers()
        {
            List<T> tempList = new List<T>(removeSubscribers);
            foreach (T subscriber in tempList)
            {
                if (subscribers.Contains(subscriber))
                {
                    subscribers.Remove(subscriber);
                    removeSubscribers.Remove(subscriber);
                }
            }
        }

        protected virtual void AddRegisteredSubscribers()
        {
            List<T> tempList = new List<T>(addSubscribers);
            foreach (T subscriber in tempList)
            {
                if (!subscribers.Contains(subscriber))
                {
                    subscribers.Add(subscriber);
                    addSubscribers.Remove(subscriber);
                }
            }
        }
    }
}
