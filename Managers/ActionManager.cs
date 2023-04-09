using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager instance;
        private List<ActionHandler> actionHandlers = new List<ActionHandler>();
        private IEnumerator actionHandlerCoroutine;
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
            actionHandlerCoroutine = ActionHandlerCoroutine();
            StartCoroutine(actionHandlerCoroutine);
        }

        private IEnumerator ActionHandlerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshRate);

                foreach (ActionHandler actionHandler in actionHandlers) { }
            }
        }

        public static void RegisterActionHandler(ActionHandler actionHandler)
        {
            if (instance.actionHandlers.Contains(actionHandler))
                return;
            instance.actionHandlers.Add(actionHandler);
        }

        public static void UnregisterActionHandler(ActionHandler actionHandler)
        {
            instance.actionHandlers.Remove(actionHandler);
        }
    }
}
