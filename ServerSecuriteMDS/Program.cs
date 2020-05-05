using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ServerSecuriteMDS
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            var prefixes = new List<string>() { "http://localhost:8080/" };

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();

                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                Console.WriteLine($"Recived request for {request.Url}");
                Console.WriteLine(documentContents);

                System.Security.Principal.IPrincipal user = context.User;
                System.Security.Principal.IIdentity id = user.Identity;
                Console.WriteLine(user);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString = "aaaa";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            listener.Stop();
        }

        public static string ClientInformation(HttpListenerContext context)
        {
            System.Security.Principal.IPrincipal user = context.User;
            System.Security.Principal.IIdentity id = user.Identity;
            if (id == null)
            {
                return "Client authentication is not enabled for this Web server.";
            }

            string display;
            if (id.IsAuthenticated)
            {
                display = String.Format("{0} was authenticated using {1}", id.Name,
                    id.AuthenticationType);
            }
            else
            {
                display = String.Format("{0} was not authenticated", id.Name);
            }
            return display;
        }
    }
    }
