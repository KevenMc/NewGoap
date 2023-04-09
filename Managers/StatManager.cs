using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class StatManager : MonoBehaviour
    {
        public static StatManager instance;
        public List<StatHandler> statHandlers;
        private IEnumerator statHandlerCoroutine;
        public float refreshRate = 0.1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            statHandlerCoroutine = StatHandlerCoroutine();
            StartCoroutine(statHandlerCoroutine);
        }

        private IEnumerator StatHandlerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshRate);

                foreach (StatHandler handler in statHandlers)
                {
                    foreach (Stat stat in handler.stats)
                    {
                        stat.current += stat.increment * refreshRate;
                    }
                }
            }
        }

        public void RegisterStatHandler(StatHandler handler)
        {
            if (!statHandlers.Contains(handler))
            {
                statHandlers.Add(handler);
            }
        }

        public void UnregisterStatHandler(StatHandler handler)
        {
            if (statHandlers.Contains(handler))
            {
                statHandlers.Remove(handler);
            }
        }
    }
}
