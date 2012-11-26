using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace RedirectTool
{
    /// <summary>
    /// This class extracts 301 and 302 redirects from an .htaccess file and builds static webpages with header redirects </summary>
    /// <remarks>
    /// For use in converting a website from an Apache server to IIS</remarks>
    class RedirectBuilder
    {
        /// <summary>
        /// Get the path our executable resides in</summary>
        private static string GetApplicationPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        }

        /// <summary>
        /// List that will hold the initial values from .htaccess</summary>
        public List<string> redirectsList = new List<string>();

        /// <summary>
        /// The filetype we'll be generating (php, asp, asp.net)</summary>
        public string fileType;

        /// <summary>
        /// Counter to keep track of how many redirects we've processed</summary>
        public int counter;

        public RedirectBuilder()
        {
            // constructor stuff
        }
        
        /// <summary>
        /// Web Writer creates all the folders neccessary, then calls the method to create the redirect page in each</summary>
        /// <remarks>
        /// Starts the write portion of functionality</remarks>
        public string PageWriter()
        {
            string ourResults = null;

            redirectsList.ForEach(delegate(String line)
            {
                // split the array with the delimiters we put in earlier
                string[] lineArray = line.Split('|');

                // first element is the redirect type
                string redirectType = lineArray[0];
                
                // get old url and switch from linux style URLs to Windows
                string oldUrl = lineArray[1].Replace('/', '\\');

                // where we are redirecting to
                string newPage = lineArray[2];

                // actual folder we'll be creating
                string ourFolder = null;
                
                // if a file exists, we're going to recreate it, only with a redirect
                string ourFile = null;

                // prep the local path so DirectoryInfo can write it
                string localPath = new Uri(GetApplicationPath() + oldUrl).LocalPath;

                string ourFileName = null;

                try
                {
                    // if the path ends with an extension we know it's not a folder, so we have to temporarily remove that extension
                    if ((localPath.EndsWith(".php")) || (localPath.EndsWith(".asp")) || (localPath.EndsWith(".aspx")) || (localPath.EndsWith(".htm")) || (localPath.EndsWith(".html")))
                    {
                        // split folders and files
                        string[] tempFilePath = localPath.Split('\\');

                        // our file will be the last element
                        ourFile = tempFilePath.Last();

                        // remove the filename from path
                        ourFolder = localPath.Replace(ourFile, "");

                        // get the file name needed (it will be the old file, or an indexer)
                        ourFileName = localPath;

                    }
                    else
                    {
                        // get the file name needed (it will be the old file, or an indexer)
                        ourFileName = localPath + GetFileName(localPath);
                        ourFolder = localPath;
                    }

                    // if the directory doesn't exist
                    if (!Directory.Exists(ourFolder))
                    {
                        // create it
                        DirectoryInfo di = Directory.CreateDirectory(ourFolder);                        
                    }
                    
                    // write the new file and embed the redirect code in it.
                    FileWriter(ourFileName, GetRedirectFileData(redirectType, newPage));
                    // populate our results
                    ourResults += "Wrote " + ourFileName + " with a " + redirectType + " redirect \n";
                }
                catch (IOException ioex)
                {
                    // populate results with exception output
                    ourResults = ioex.Message.ToString();
                }

            });

            return ourResults;
        }

        /// <summary>
        /// GetFileName checks the end of a string for a file extension. If none exists we assume its a folder and create a default index file.</summary>
        /// <param name="url">url extracted from .htaccess</param>
        /// <remarks>
        /// For use in converting a website from an Apache server to IIS</remarks>
        public string GetFileName(string url)
        {
            // split the url by slashes
            string[] urlArray = url.Split('\\');

            // this is the last element of the array, which we assume to be the filename
            string lastUrlItem = urlArray[urlArray.Length -1];
            
            // create our new file name variable
            string newFileName = null;


            // if the last item is less than 1, it must be a folder
            if (lastUrlItem.Length < 1)
            {
                    // so we need to create an index file, chosen by the user
                    switch (fileType)
                    {
                        case "php":
                            newFileName = "index.php";
                            break;

                        case "asp":
                            newFileName = "Index.asp";
                            break;

                        case "aspnet":
                            newFileName = "Default.aspx";
                            break;
                    }
            }
            else
            {
                // file already exists, we'll use that one
                newFileName = urlArray.Last();
            } 
            // return the new file name
            return newFileName;
        }
       
        /// <summary>
        /// ReadFile opens the .htaccess file and extracts 301 and 302 redirects.</summary>
        /// <param name="fileName">Name of the .htaccess file to open</param>
        /// <remarks>
        /// For use in converting a website from an Apache server to IIS</remarks>
        public void ReadFile(string fileName)
        {
            // I'm trying to keep this method relatively simple and just extract the information from the .htaccess file
            // We'll do some more cleanup of the data in another method. 
            try
            {
                // String created for the lines we'll be reading
                string line;

                // Read the file and display it line by line
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);

                while ((line = file.ReadLine()) != null)
                {
                    // some logic to make sure we're only looking for redirects. So far we only want 301 and 302s and we check for lowercase
                    if ((line.StartsWith("Redirect 301") || line.StartsWith("Redirect 302") || line.StartsWith("redirect 301") || line.StartsWith("redirect 302")))
                    {

                        // Use a Regex to collapse the spaces here, often times there are tabs or lots of spaces in the .htaccess</summary>
                        line = Regex.Replace(line, @"\s+", " ");

                        // split the string by spaces</summary>
                        string[] words = line.Split(' ');

                        // we don't start at words[0] because that's the word "Redirect"
                        string finalLine = words[1] + "|" + words[2] + "|" + words[3];

                        // add the complete string to the list
                        redirectsList.Add(finalLine);

                        // increment counter
                        counter++;
                    }

                }
                // close the file
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// FileWriter creates a text file that will be the .aspx or .php file with a redirect inside</summary>
        /// <param name="fileName">Name of the file to create</param>
        /// <param name="fileData">The string we're going to write to a file</param>
        /// <remarks>
        /// For creating files on the filesystem</remarks>
        public void FileWriter(string fileName, string fileData)
        {
            string localPath = new Uri(fileName).LocalPath;

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(localPath, false))
                {
                    // write the file
                    file.WriteLine(fileData);
                    // close it
                    file.Close();
                }
            }
            //catch (UnauthorizedAccessException ex)
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// GetRedirectFileData method takes the redirect type and location and writes the web file that will redirect</summary>
        /// <param name="redirType">Type of redirect (301 or 302)</param>
        /// <param name="location">The url we're redirecting to</param>
        /// <remarks>
        /// For generating new web files</remarks>
        public string GetRedirectFileData(string redirType, string location)
        {
            string redirectFileData = null;

            switch (fileType)
            {
                // generate php files
                case "php":

                    if (redirType == "301") 
                    {
                        redirectFileData = ("<?php\n\theader(\"HTTP/1.1 301 Moved Permanently\");\n\theader(\"Location: " + location + "\");\n\texit();\n?>");
                    }
                    else if (redirType == "302")
                    {
                        redirectFileData = ("<?php\n\theader(\"HTTP/1.1 302 Moved Temporarily\");\n\theader(\"Location: " + location + "\");\n\texit();\n?>");
                    }
                    else if (redirType == "")
                    {
                        // we should never get here
                        redirectFileData = ("There was an error selecting the redirect type");
                    }

                    break;
                // generate classic ASP files
                case "asp":

                    if (redirType == "301")
                    {
                        redirectFileData = ("<%@ Language=VBScript %>\n<%\n\tResponse.Status=\"301 Moved Permanently\"\n\tResponse.AddHeader \"Location\", \"" + location + "\"\n%>");
                    }
                    else if (redirType == "302")
                    {
                        redirectFileData = ("<%@ Language=VBScript %>\n<%\n\tResponse.Status=\"302 Moved Temporarily\"\n\tResponse.AddHeader \"Location\", \"" + location +"\"\n%>");
                    }
                    else if (redirType == "")
                    {
                        // we should never get here
                        redirectFileData = ("There was an error selecting the redirect type");
                    }

                    break;
                // generate ASP.NET files
                case "aspnet":

                    if (redirType == "301")
                    {
                        redirectFileData = ("<script language=\"c#\" runat=\"server\">\n\tprivate void Page_Load(object sender, System.EventArgs e)\n\t{\n\t\tResponse.Status = \"301 Moved Permanently\";\n\t\tResponse.AddHeader(\"Location\",\"" + location + "\");\n\t}\n</script>");
                    }
                    else if (redirType == "302")
                    {
                        redirectFileData = ("<script language=\"c#\" runat=\"server\">\n\tprivate void Page_Load(object sender, System.EventArgs e)\n\t{\n\t\tResponse.Status = \"302 Moved Temporarily\";\n\t\tResponse.AddHeader(\"Location\",\"" + location + "\");\n\t}\n</script>");
                    }
                    else if (redirType == "")
                    {
                        // we should never get here
                        redirectFileData = ("There was an error selecting the redirect type");
                    }

                    break;
            }
            // return the value
            return redirectFileData;
        }
    }
}
