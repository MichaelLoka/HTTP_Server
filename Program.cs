using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            string redirectionMatrixPath = Path.GetFullPath("redirectionRules.txt");
            Server server = new Server(1000, redirectionMatrixPath);
            server.StartServer();
            
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            StreamWriter sr = new StreamWriter("redirectionRules.txt");
            
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            sr.WriteLine("aboutus.html,aboutus2.html");
            sr.Close();
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
