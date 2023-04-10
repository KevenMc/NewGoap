using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager instance;
        private List<ActionHandler> actionHandlers = new List<ActionHandler>();
        private List<ActionHandler> removeActionHandlers = new List<ActionHandler>();
        private List<ActionHandler> addActionHandlers = new List<ActionHandler>();
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
                AddRegisteredActionHandlers();
                ClearUnregisteredActionHandlers();

                foreach (ActionHandler actionHandler in actionHandlers)
                {
                    actionHandler.HasArrivedAtLocation();
                }
            }
        }

        public static void RegisterActionHandler(ActionHandler actionHandler)
        {
            if (instance.actionHandlers.Contains(actionHandler))
                return;
            instance.addActionHandlers.Add(actionHandler);
        }

        public static void UnregisterActionHandler(ActionHandler actionHandler)
        {
            instance.removeActionHandlers.Add(actionHandler);
        }

        private void ClearUnregisteredActionHandlers()
        {
            List<ActionHandler> tempList = new List<ActionHandler>(removeActionHandlers);
            foreach (ActionHandler actionHandler in tempList)
            {
                if (actionHandlers.Contains(actionHandler))
                {
                    actionHandlers.Remove(actionHandler);
                    removeActionHandlers.Remove(actionHandler);
                }
            }
        }

        private void AddRegisteredActionHandlers()
        {
            List<ActionHandler> tempList = new List<ActionHandler>(addActionHandlers);
            foreach (ActionHandler actionHandler in tempList)
            {
                if (!actionHandlers.Contains(actionHandler))
                {
                    actionHandlers.Add(actionHandler);
                    addActionHandlers.Remove(actionHandler);
                }
            }
        }
    }
}
