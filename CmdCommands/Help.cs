using System;
using System.Collections.Generic;
using System.Text;


namespace CmdCommands
{
    class Help
    {

        public static string[] inform = { "Displays the name of or changes the current directory.",
                            "Clears the screen.",
                            "Displays a list of files and subdirectories in a directory.",
                            "quit the shell.",
                            "Copies one or more files to another location.",
                            "Deletes one or more files.",
                            "creates a directory.",
                            "information about the commands.",
                            "removes a directory.",
                            "renames a file.",
                            "displays the contents of text file.",
                            "import text files from your computer.",
                            "export text files to your computer." };

        public static string[] Commands = { "cd", "cls", "dir", "quit", "copy", "del", "md",
                                      "help", "rd", "rename", "type", "import", "export" };

        //method with no prameters
        public static void help()
        {
            Console.WriteLine("For more information on a specific command, type HELP command-name\n");
            for (int i = 0; i < Commands.Length; i++)
            {
                Console.Write(Commands[i] + "     " + inform[i] + "\n");
            }
        }

        //method with prameters
        // bool found = false;
        public static void help(string argu)
        {
            for (int i = 0; i < Commands.Length; i++)
            {
                if (argu == Commands[i])
                {
                    Console.WriteLine(inform[i]);
                }
            }

        }
    }
}
