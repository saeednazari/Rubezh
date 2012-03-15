﻿using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XProperty
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Value { get; set; }
    }
}