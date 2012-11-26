using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RedirectTool
{
    /// <summary>
    /// Redirect Builder main executable file</summary>
    /// <remarks>
    /// Generates an executable for our Redirect Builder Program</remarks>
    class ERC
    {
        /// <summary>
        /// The entry point for the application.
        /// </summary>
        /// <param name="args"> A list of command line arguments</param>
        public static void Main(String[] args)
        {
                // start our builder object
                RedirectBuilder ourBuilder = new RedirectBuilder();
                Console.WriteLine("ERC v1.0 Copyright (C) 2012 Jeremy Morgan\n\nConverts values in .htaccess to pages with redirects embedded\n\nThis program comes with ABSOLUTELY NO WARRANTY\n\nThis is free software, and you are welcome to redistribute it \nunder certain conditions. See license.txt for details.\n\n");

                // if there are no arguments, we'll use the defaults
                if (args.Length == 0)
                {   
                    // make sure an .htaccess exists
                    if((File.Exists(".htaccess")))
                    {
                        Console.WriteLine("--------\nRunning with Default Options\nNote: You can read from and generate different files.\n\n\tType erc /? for help\n\n");
                        
                        // default is aspx
                        ourBuilder.fileType = "aspnet";

                        // default readfile is .htaccess
                        ourBuilder.ReadFile(@".htaccess");

                        // run the page writer
                        Console.WriteLine(ourBuilder.PageWriter());

                    }else {
                        // show an error saying no .htaccess was found
                        Console.WriteLine("Error: no .htaccess found. If you want to specify a location or different name for this file, you may use an alternate file. Type erc /? for details\n\n");
                    }
                }
                else
                {
                    if ((args[0] == ("/?") || args[0] == ("/help")))
                    {
                        // user opted for help option
                        Console.WriteLine("Usage: erc [Filetype] [alternate .htaccess]\n\nAvailable Filetypes:\n\n\tasp = Classic .asp\n\taspnet = ASP.NET\n\tphp = PHP\n\n");
                    }
                    else
                    {
                        // check to make sure filetype is correct
                        if ((args[0] == "asp") || (args[0] == "aspnet") || (args[0] == "php"))
                        {
                            // get the file type
                            ourBuilder.fileType = args[0];

                            // see if the 2nd argument was entered
                            if (args.Length > 1)
                            {
                                string ourCustomFile = args[1];

                                if ((File.Exists(ourCustomFile)))
                                {
                                    // set the readfile name
                                    ourBuilder.ReadFile(args[1]);
                                    // run the page writer
                                    Console.WriteLine(ourBuilder.PageWriter());
                                }
                                else
                                {
                                    Console.WriteLine("Error: The custom file (" + ourCustomFile + ") does not exist or is not readable. Please double check the filename and permissions. \n\n");
                                }                                
                            }
                            else
                            {
                                // if not we'll just read .htaccess
                                ourBuilder.ReadFile(@".htaccess");
                                // run the page writer
                                Console.WriteLine(ourBuilder.PageWriter());
                            }                            
                        }
                        else
                        {
                            // run the page writer
                            Console.WriteLine("\nERROR: You did not specify a valid filetype.\n\nValid types are:\n\tasp\n\taspnet\n\tphp\n\n");
                        }
                    }
                }
                
            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadLine();

        }
    }
}
