using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cschmid.BoxPlatformer.MonoBehaviors
{
    public class TriggerHandler : MonoBehaviour
    {     
        public bool IsTriggered { get; private set; } = false;

        private void OnTriggerExit2D(Collider2D collision)
        {
            IsTriggered = false;
        }
        private void OnTriggerStay2D(Collider2D collision)
        { 
            IsTriggered = true;
        }
    }
}
