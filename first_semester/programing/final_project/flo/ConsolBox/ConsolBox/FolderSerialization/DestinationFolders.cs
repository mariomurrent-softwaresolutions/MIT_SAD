﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConsolBox
{
    public class DestinationFolders
    {
        [XmlElement("DestinationFolder")]
        public List<DestinationFolder> destinationFolder { get; set; }
    }
}
