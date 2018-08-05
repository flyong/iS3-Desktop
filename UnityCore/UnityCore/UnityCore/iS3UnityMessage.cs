using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore.MessageSys
{
    public class iS3UnityMessage : IMessage
    {
        private MessageType _type;
        public virtual MessageType type
        {
            get
            {
                return _type;
            }
        }
        public virtual string SerializeObject()
        {
            return "";
        }
        public virtual void DeSerializeObject(string message)
        {

        }
    }

}
