﻿namespace VirtualMachineClient.Models
{
    using System.Collections.ObjectModel;

    public class SuccessResponse
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public ObservableCollection<VmInfo> Data { get; set; }
    }
}
