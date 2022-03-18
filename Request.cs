using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            //TODO: parse the receivedRequest using the \r\n delimeter
            requestLines = requestString.Split(new string[] {"\r\n"},StringSplitOptions.None);
            
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;

            // Parse Request line
            if (!(this.ParseRequestLine())) return false;
            // Validate blank line exists
            if (!(this.ValidateBlankLine())) return false;
            // Load header lines into HeaderLines dictionary
            if (!(this.LoadHeaderLines())) return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            string requestline = requestLines[0];
            string[] tokens = requestline.Split(' ');

            if (tokens.Length != 3)                 return false;
            if (tokens[0] == "GET")                 this.method = RequestMethod.GET;
            else if (tokens[0] == "POST")           this.method = RequestMethod.POST;
            else if (tokens[0] == "HEAD")           this.method = RequestMethod.HEAD;
            else                                    return false;

            if (!(this.ValidateIsURI(tokens[1])))   return false;

            if (tokens[2] == "HTTP/1.1") this.httpVersion = HTTPVersion.HTTP11;
            else if (tokens[2] == "HTTP/1.0") this.httpVersion = HTTPVersion.HTTP10;
            else if (tokens[2] == "HTTP/0.9") this.httpVersion = HTTPVersion.HTTP09;
            else return false;

            this.method = RequestMethod.GET;
            
            this.relativeURI = tokens[1].Replace('/' , '\\').TrimStart('\\');


            return true;

        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            bool HostheaderFound = false;
            for ( int i = 1; i < requestLines.Length -2; i++)
            {
                string[] key_value = requestLines[i].Split(' ');
                headerLines.Add(key_value[0], key_value[1]);

                if (key_value[0] == "Host:")
                    HostheaderFound = true;
            }
            if (!HostheaderFound)
                return false;
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length-1] == "")
                return true;

            return false;
        }

    }
}
