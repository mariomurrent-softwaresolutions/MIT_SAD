﻿namespace VirtualMachineClient.Models
{
    using System.Collections.Generic;

    using GalaSoft.MvvmLight;

    public class VmInfo : ViewModelBase
    {
        public string Id { get; set; }

        public string ReferencedVirtualMachineId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string ApplicationType { get; set; }

        public string OperatingSystem { get; set; }

        public string OperatingSystemType
        {
            get;

            set;
        }

        public string OperatingSystemVersion { get; set; }

        public string Size { get; set; }

        public string RecommendedCPU { get; set; }

        public string RecommendedRAM { get; set; }

        public string SupportedVirtualizationPlatform { get; set; }

        public List<string> Software { get; set; }

        public List<string> SupportedProgramingLanguages { get; set; }

        public string Status { get; set; }

        public string Rating { get; set; }

        public string RatingDescription { get; set; }
    }
}
