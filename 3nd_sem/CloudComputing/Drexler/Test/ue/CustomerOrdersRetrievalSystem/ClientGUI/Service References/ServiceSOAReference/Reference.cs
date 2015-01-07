﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClientGUI.ServiceSOAReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceSOAReference.IService1")]
    public interface IService1 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetCustomers", ReplyAction="http://tempuri.org/IService1/GetCustomersResponse")]
        ServiceData.Customer[] GetCustomers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetCustomers", ReplyAction="http://tempuri.org/IService1/GetCustomersResponse")]
        System.Threading.Tasks.Task<ServiceData.Customer[]> GetCustomersAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetOrders", ReplyAction="http://tempuri.org/IService1/GetOrdersResponse")]
        ServiceData.Order[] GetOrders(string customerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetOrders", ReplyAction="http://tempuri.org/IService1/GetOrdersResponse")]
        System.Threading.Tasks.Task<ServiceData.Order[]> GetOrdersAsync(string customerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/AddOrder", ReplyAction="http://tempuri.org/IService1/AddOrderResponse")]
        bool AddOrder(ServiceData.Order order);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/AddOrder", ReplyAction="http://tempuri.org/IService1/AddOrderResponse")]
        System.Threading.Tasks.Task<bool> AddOrderAsync(ServiceData.Order order);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/AddCustomer", ReplyAction="http://tempuri.org/IService1/AddCustomerResponse")]
        bool AddCustomer(ServiceData.Customer customer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/AddCustomer", ReplyAction="http://tempuri.org/IService1/AddCustomerResponse")]
        System.Threading.Tasks.Task<bool> AddCustomerAsync(ServiceData.Customer customer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/DeleteOrder", ReplyAction="http://tempuri.org/IService1/DeleteOrderResponse")]
        bool DeleteOrder(ServiceData.Order order);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/DeleteOrder", ReplyAction="http://tempuri.org/IService1/DeleteOrderResponse")]
        System.Threading.Tasks.Task<bool> DeleteOrderAsync(ServiceData.Order order);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/DeleteCustomer", ReplyAction="http://tempuri.org/IService1/DeleteCustomerResponse")]
        bool DeleteCustomer(ServiceData.Customer customer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/DeleteCustomer", ReplyAction="http://tempuri.org/IService1/DeleteCustomerResponse")]
        System.Threading.Tasks.Task<bool> DeleteCustomerAsync(ServiceData.Customer customer);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService1Channel : ClientGUI.ServiceSOAReference.IService1, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service1Client : System.ServiceModel.ClientBase<ClientGUI.ServiceSOAReference.IService1>, ClientGUI.ServiceSOAReference.IService1 {
        
        public Service1Client() {
        }
        
        public Service1Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service1Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ServiceData.Customer[] GetCustomers() {
            return base.Channel.GetCustomers();
        }
        
        public System.Threading.Tasks.Task<ServiceData.Customer[]> GetCustomersAsync() {
            return base.Channel.GetCustomersAsync();
        }
        
        public ServiceData.Order[] GetOrders(string customerName) {
            return base.Channel.GetOrders(customerName);
        }
        
        public System.Threading.Tasks.Task<ServiceData.Order[]> GetOrdersAsync(string customerName) {
            return base.Channel.GetOrdersAsync(customerName);
        }
        
        public bool AddOrder(ServiceData.Order order) {
            return base.Channel.AddOrder(order);
        }
        
        public System.Threading.Tasks.Task<bool> AddOrderAsync(ServiceData.Order order) {
            return base.Channel.AddOrderAsync(order);
        }
        
        public bool AddCustomer(ServiceData.Customer customer) {
            return base.Channel.AddCustomer(customer);
        }
        
        public System.Threading.Tasks.Task<bool> AddCustomerAsync(ServiceData.Customer customer) {
            return base.Channel.AddCustomerAsync(customer);
        }
        
        public bool DeleteOrder(ServiceData.Order order) {
            return base.Channel.DeleteOrder(order);
        }
        
        public System.Threading.Tasks.Task<bool> DeleteOrderAsync(ServiceData.Order order) {
            return base.Channel.DeleteOrderAsync(order);
        }
        
        public bool DeleteCustomer(ServiceData.Customer customer) {
            return base.Channel.DeleteCustomer(customer);
        }
        
        public System.Threading.Tasks.Task<bool> DeleteCustomerAsync(ServiceData.Customer customer) {
            return base.Channel.DeleteCustomerAsync(customer);
        }
    }
}
