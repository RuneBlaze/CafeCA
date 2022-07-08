using System;
using System.Collections;
using Cafeo.Messaging;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class DialogUI : MonoBehaviour
    {
        public DialogMessage message;
        private int textPointer;

        [SerializeField] private Text nameText;
        [SerializeField] private Text dialogText;

        private enum State
        {
            Inactive,
            Displaying,
            Finished,
        }

        private void Awake()
        {
            Assert.IsNotNull(nameText);
            Assert.IsNotNull(dialogText);
        }

        private void Start()
        {
            message = new DialogMessage(null, "你好你好你好！");
            StartCoroutine(DisplayDialog());
        }
        
        private IEnumerator DisplayDialog()
        {
            // nameText.text = message.name;
            textPointer = 0;
            dialogText.text = "";
            while (textPointer < message.message.Length)
            {
                dialogText.text += message.message[textPointer];
                textPointer++;
                yield return new WaitForSeconds(0.02f);
            }
        }

        private State state = State.Inactive;

        private void OnEnterState(State newState)
        {
            
        }

        private void ProgressState()
        {
            switch (state)
            {
                case State.Inactive:
                    state = State.Displaying;
                    OnEnterState(state);
                    break;
                case State.Displaying:
                    state = State.Finished;
                    OnEnterState(state);
                    break;
                case State.Finished:
                    break;
            }
        }

        public void WakeUp(DialogMessage newMessage)
        {
            if (state != State.Inactive)
            {
                throw new Exception("DialogUI is already active.");
            }
            message = newMessage;
            ProgressState();
        }
    }
}