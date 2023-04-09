using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class MovementManager : MonoBehaviour
    {
        public static MovementManager instance;
        public List<MovementHandler> movementHandlers = new List<MovementHandler>();
        private IEnumerator movementHandlerCoroutine;
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
            movementHandlerCoroutine = MovementHandlerCoroutine();
            StartCoroutine(movementHandlerCoroutine);
        }

        private IEnumerator MovementHandlerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshRate);

                //Handle movement
            }
        }

        public static void RegisterMovementHandler(MovementHandler planner)
        {
            if (instance.movementHandlers.Contains(planner))
                return;
            instance.movementHandlers.Add(planner);
        }

        public static void UnregisterMovementHandler(MovementHandler planner)
        {
            instance.movementHandlers.Remove(planner);
        }
    }
}
