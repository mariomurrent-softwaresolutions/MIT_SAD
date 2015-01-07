﻿//-----------------------------------------------------------------------
// <copyright file="MetaDataObject.cs" company="MD Development">
//     Copyright (c) MD Development. All rights reserved.
// </copyright>
// <author>Michael Drexler</author>
//-----------------------------------------------------------------------
namespace DataLayer.OpenStack.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class MetaDataObject
    {
        /// <summary>
        /// Gets or sets the is_admin value
        /// </summary>
        [DataMember]
        public int Is_Admin { get; set; }

        /// <summary>
        /// Gets or sets the roles
        /// </summary>
        [DataMember]
        public string[] Roles { get; set; }
    }
}
