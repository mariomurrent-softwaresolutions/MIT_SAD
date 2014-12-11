﻿using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using RestSharp;
using RestSoapClient.Models;

namespace RestSoapClient
{
    class Program
    {
        private static JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        private const string RestEndPoint = "http://localhost:1337";
        private static RestClient restClient;
        private static readonly Dictionary<ConsoleKey, string> RequestOptions = new Dictionary<ConsoleKey, string>
        {
             {ConsoleKey.S, "S) SOAP"},
            {ConsoleKey.R, "R) REST"}
        };
        private static readonly Dictionary<ConsoleKey, string> Requests = new Dictionary<ConsoleKey, string>
        {
            {ConsoleKey.G, "G) Get Customers"},
            {ConsoleKey.H, "H) Get Orders"},
            {ConsoleKey.J, "J) Add Customer"},
            {ConsoleKey.K, "K) Add Order"},
            {ConsoleKey.D, "D) Delete Customer"},
            {ConsoleKey.E, "F) Delete Order"}
        };
        static void Main(string[] args)
        {
            ChooseMethod();
        }

        private static void ChooseMethod()
        {
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please choose request method");
            Console.WriteLine("---------------------------------");
            WriteDictionaryOptionsToConsole(RequestOptions);
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.S)
            {

            }
            else if (keyInfo.Key == ConsoleKey.R)
            {
                restClient = new RestClient(RestEndPoint);
                ChooseRestRequest();
            }
            else
            {
                ChooseMethod();
            }

        }

        private static void WriteDictionaryOptionsToConsole(Dictionary<ConsoleKey, string> dictionary)
        {
            foreach (var entry in dictionary)
            {
                Console.WriteLine(entry.Value);
            }
        }

        private static void ChooseRestRequest()
        {
            RestRequest restRequest = null;
            string requestParameter = String.Empty;
            Console.Clear();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please Choose Request");
            Console.WriteLine("---------------------------------");
            WriteDictionaryOptionsToConsole(Requests);
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.D:
                    requestParameter = GetParameter("Delete customer: ");
                    restRequest = new RestRequest("customer/delete/{name}", Method.DELETE);
                    restRequest.AddUrlSegment("name", requestParameter);
                    IRestResponse deleteCustomerResponse = restClient.Execute(restRequest);
                    SuccessResponse deleteCustomerSuccessResponse = javaScriptSerializer.Deserialize<SuccessResponse>(deleteCustomerResponse.Content);
                    Console.WriteLine("Success: " + deleteCustomerSuccessResponse.Success);
                    break;

                case ConsoleKey.F:
                    requestParameter = GetParameter("Delete order for customer: ");
                    restRequest = new RestRequest("order/delete/{name}", Method.DELETE);
                    restRequest.AddUrlSegment("name", requestParameter);
                    IRestResponse deleteOrdeResponseResponse = restClient.Execute(restRequest);
                    SuccessResponse deleteOrderSuccessResponse = javaScriptSerializer.Deserialize<SuccessResponse>(deleteOrdeResponseResponse.Content);
                    Console.WriteLine("Success: " + deleteOrderSuccessResponse.Success);
                    break;

                case ConsoleKey.G:
                    restRequest = new RestRequest("customers", Method.GET);
                    IRestResponse customersResponse = restClient.Execute(restRequest);
                    List<Customer> customersContent = javaScriptSerializer.Deserialize<List<Customer>>(customersResponse.Content);
                    Console.Clear();
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("Customers");
                    Console.WriteLine("-------------------------");
                    foreach (Customer customer in customersContent)
                    {
                        Console.WriteLine(customer.Name);
                        foreach (string order in customer.Orders)
                        {
                            Console.WriteLine("\t|--" + order);
                        }
                    }
                    break;

                case ConsoleKey.H:
                    requestParameter = GetParameter("Order for customer: ");
                    restRequest = new RestRequest("order/{name}", Method.GET);
                    restRequest.AddUrlSegment("name", requestParameter);
                    IRestResponse ordersResponse = restClient.Execute(restRequest);
                    List<string> ordersContent = javaScriptSerializer.Deserialize<List<string>>(ordersResponse.Content);
                    Console.Clear();
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("Orders");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine(requestParameter);
                    foreach (string order in ordersContent)
                    {
                        Console.WriteLine("\t|-" + order);
                    }
                    break;

                case ConsoleKey.J:
                    requestParameter = GetParameter("New customer name: ");
                    restRequest = new RestRequest("customer/add/{name}", Method.PUT);
                    restRequest.AddUrlSegment("name", requestParameter);
                    IRestResponse newCustomerResponse = restClient.Execute(restRequest);
                    SuccessResponse customerSuccessResponse = javaScriptSerializer.Deserialize<SuccessResponse>(newCustomerResponse.Content);
                    Console.WriteLine("Success: " + customerSuccessResponse.Success);
                    break;

                case ConsoleKey.K:
                    requestParameter = GetParameter("New order for customer: ");
                    restRequest = new RestRequest("order/add/{name}", Method.PUT);
                    restRequest.AddUrlSegment("name", requestParameter);
                    IRestResponse newOrderResponse = restClient.Execute(restRequest);
                    SuccessResponse orderSuccessResponse = javaScriptSerializer.Deserialize<SuccessResponse>(newOrderResponse.Content);
                    Console.WriteLine("Success: " + orderSuccessResponse.Success);
                    break;

                default:
                    ChooseRestRequest();
                    break;
            }
            StartFromBeginning();
            ChooseRestRequest();
        }

        private static string GetParameter(string text)
        {
            Console.Write(text);
            string parameter = Console.ReadLine();
            if (parameter != String.Empty)
            {
                return parameter;
            }
            return "Emtpy";
        }

        private static void StartFromBeginning()
        {
            Console.WriteLine("Start over? y/n. n will close the application");
            ConsoleKeyInfo info = Console.ReadKey(true);
            if (info.Key == ConsoleKey.Y)
            {
                ChooseRestRequest();
            }
            else if(info.Key == ConsoleKey.N)
            {
                Environment.Exit(0);
            }
        }
    }
}
