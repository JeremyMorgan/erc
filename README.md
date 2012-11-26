ERC - Easy Redirect Converter
===

This is an executable that will convert the 301 and 302 .htaccess redirects into hard webpage files for your server that contain redirects.

Written and maintained by [Jeremy Morgan](https://github.com/JeremyMorgan)

Just want the executable? [ Get it Here ](https://github.com/downloads/JeremyMorgan/erc/easy-redirect-converter.zip)( Microsoft .NET Framework 3.0 required )

<br>
###What Problem Does This Solve?

When webmasters move their websites from a Linux server to a Windows server, they find that .htaccess redirects don't work anymore. Sometimes the host provides IIS redirects, often times they don't. 

Also some Linux servers do not support .htaccess, leaving you with unresolved 404s. 

This tool reads .htaccess files and creates redirect pages to serve as redirect files for servers that do not support redirects. This is a drop in solution for servers without redirect support.

<br>
###How Do I use it?

ERC.exe is a standalone executable that resides on your hard drive, you place it in the base of your website folder (make a backup!) and run it from the command prompt. 

There are the following options:

run erc.exe by itself, and it will automatically read .htaccess and generate Default.aspx files as an index. 

If you would like to use different languages for your index file, 3 options are provided:

- asp - Classic ASP ( index.asp )
- aspnet - ASP.NET pages ( Default.aspx )
- php - PHP pages  ( index.php )

There is also an option to specify a custom input file besides htaccess, you would select one of the previous filetypes and then put the filename to read read from after that. 

>erc aspnet allmyurls.txt

<br>
###How does it work? 

Example: typing **erc php** would do the following:

if you have the following line in your .htaccess

>Redirect 301 /home/oldfolder /home/newfolder/

that means that /home/oldfolder was being redirected to /home/newfolder by .htaccess as the page was requested. 

ERC will create a folder called /home/oldfolder/ and create a file called "index.php" in that folder. In the index.php there will be a header redirect pointing to /home/newfolder/

This way whenever a visitor or search engine crawler goes to the old url at www.yoursite.com/home/oldfolder they are redirected to the new URL, even if it's on another domain. 

<br>
### Can I use it commercially / fork it / or include it in my program? 

Yes, yes and yes. This application is free for use and covered by the GPL License. If you're going to fork it or do something cool with it, let me know! 

The source code can be found here - [ ERC Source ](https://github.com/JeremyMorgan/erc "ERC Source") ( Built with Visual Studio 2010 Express )


This program created and maintained by [ Jeremy Morgan ](http://www.jeremymorgan.com "Title") 