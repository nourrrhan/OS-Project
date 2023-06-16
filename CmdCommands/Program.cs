using System;
using System.IO;
using System.Collections.Generic;

namespace CmdCommands
{
    class Program
    {
        static public Directoriy currdirectory;
        static public string currpath;

        static void Main(string[] args)
        {
            // print the current directory
            VirtualDisk.Initialize(@"D:\Virtual disk.txt");

            //string path = Directory.GetCurrentDirectory();

            //store the path of currdirectory.file
            currpath = new string(currdirectory.file_name).Trim();


            Console.Write(currpath + "\\>");

            // read commands from user
            string Command;
            Command = Console.ReadLine();
            Command = Command.ToLower();

            // process commands
            CommandValidation com = new CommandValidation(Command);
            com.PrintValidation();
        }

    }
}
