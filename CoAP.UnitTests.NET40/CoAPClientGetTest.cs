using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoAP.EndPoint.Resources;
using IoTLib;

namespace CoAP.UnitTests.NET40
{
    [TestClass]
    public class CoAPClientGetTest
    {
        [TestMethod]
        public void CoAPClientGetUnitTest()
        {
            String method = "GET";
            //Uri uri = new Uri("coap://127.0.0.1");
            Uri uri = new Uri("coap://127.0.0.1");
            String payload = "Test Msg01"; // null;
            
            IoTMessage payload2 = new IoTMessage();
            payload = payload2.ToJsonString();

            Boolean loop = false;
            Boolean byEvent = false; // true;
            
            Request request = new Request(Code.GET); ;  // CreateRequest(method);
            
            request.URI = uri;
            request.SetPayload(payload);
            request.Token = TokenManager.Instance.AcquireToken();
            request.ResponseQueueEnabled = true;
            request.SeparateResponseEnabled = true;

            try
            {
                if (byEvent)
                {
                    throw new Exception("This code should not be executing in this test...");
                    request.Responded += delegate(Object sender, ResponseEventArgs e)
                    {
                        Response response = e.Response;
                        if (response == null)
                        {
                            Console.WriteLine("Request timeout");
                        }
                        else
                        {
                            Console.WriteLine(response);
                            Console.WriteLine("Time (ms): " + response.RTT);
                        }
                        if (!response.IsEmptyACK && !loop)
                            Environment.Exit(0);
                    };
                    request.Execute();
                    while (true)
                    {
                        Console.ReadKey();
                    }
                }
                else
                {
                    request.Execute();

                    do
                    {
                        // Console.WriteLine("Receiving response...");

                        Response response = null;
                        response = request.ReceiveResponse();

                        if (response == null)
                        {
                            //Console.WriteLine("Request timeout");
                            Assert.Fail("Response wait timed out!");
                        }
                        else
                        {
                            //Console.WriteLine(response);
                            //Console.WriteLine("Time (ms): " + response.RTT);

                            if (response.ContentType == MediaType.ApplicationLinkFormat)
                            {
                                Assert.Fail("This code should not be executing in this test!");
                                Resource root = RemoteResource.NewRoot(response.PayloadString);
                                if (root == null)
                                {
                                    Console.WriteLine("Failed parsing link format");
                                    Environment.Exit(1);
                                }
                                else
                                {
                                    Console.WriteLine("Discovered resources:");
                                    Console.WriteLine(root);
                                }
                            }
                        }
                    } while (loop);
                }
            }
            catch (Exception ex)
            {
                throw;
                //Console.WriteLine("Failed executing request: " + ex.Message);
                //Environment.Exit(1);
            }
        }
    }
}
