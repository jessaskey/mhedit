﻿using System;

namespace GameEditor.Core.Serialization
{
    public struct DeserializationEvents
    {
        EventHandler onUnknownType;
        //internal object _sender;

        public EventHandler OnUnknownType
        {
            get
            {
                return this.onUnknownType;
            }
            set
            {
                onUnknownType = value;
            }
        }
    }

}
