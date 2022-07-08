using System;
using System.Collections.Generic;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.Messaging
{
    public class MessageBox : MonoBehaviour
    {
        private Queue<AbstractMessage> messages;

        private void Awake()
        {
            messages = new Queue<AbstractMessage>();
        }
    }
}