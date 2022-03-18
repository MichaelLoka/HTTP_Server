using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientsocket = this.serverSocket.Accept();
                Console.WriteLine("New Client: {0}", clientsocket.RemoteEndPoint);

                //start thread for each accepted connection
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientsocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket socket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            socket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.

            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] recievedata = new byte[1024];
                    int receivedLength = socket.Receive(recievedata);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", socket.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(recievedata, 0, receivedLength));
                    // TODO: Call HandleRequest Method that returns the response
                    Response Res = this.HandleRequest(request);
                    // TODO: Send Response back to client
                    socket.Send(Encoding.ASCII.GetBytes(Res.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            socket.Close();
        }

        Response HandleRequest(Request request)
        {
            string content_type = "text/html";
            string content;
            StatusCode code = StatusCode.OK;
            Response res = null;
            try
            {
                //TODO: check for bad request
                bool requestSuccess = request.ParseRequest();
                if(!requestSuccess)
                {
                    code = StatusCode.BadRequest;
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(code, content_type, content, string.Empty);
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Configuration.RootPath + '\\' + request.relativeURI;
                Console.WriteLine(request.relativeURI);

                //TODO: check for redirect
                string redirectpath = GetRedirectionPagePathIFExist(request.relativeURI);
                if(redirectpath != string.Empty)
                {
                    code = StatusCode.Redirect;
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    return new Response(code, content_type, content, redirectpath);
                }

                //TODO: check file exists
                bool fileExist = File.Exists(physicalPath);
                if(!fileExist)
                {
                    code = StatusCode.NotFound;
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(code, content_type, content, string.Empty);
                }
                //TODO: read the physical file
                content = LoadDefaultPage(request.relativeURI);

                // Create OK response
                content = LoadDefaultPage(request.relativeURI);
                res = new Response(StatusCode.OK, content_type, content, string.Empty);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                code = StatusCode.InternalServerError;
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(code, content_type, content, string.Empty);

            }
            return res;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirectpath = string.Empty;

            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                Console.WriteLine("IN the function");
                redirectpath = Configuration.RedirectionRules[relativePath];
            }

            return redirectpath;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Configuration.RootPath + '\\' + defaultPageName;
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                if(!(File.Exists(filePath)))
                    throw new FileNotFoundException();

            }
            catch(Exception ex )
            {
                Logger.LogException(ex);
                return string.Empty;
            }
            // else read file and return its content
            string content = File.ReadAllText(filePath);
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] rules = lines[i].Split(',');

                    Configuration.RedirectionRules.Add(rules[0], rules[1]);
                }

                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}