using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmdCommands
{
    class CommandValidation
    {
        string Path = Directory.GetCurrentDirectory();
        string Command, argument = " ";
        public CommandValidation(string command)
        {
            Command = command;
        }
        public static void dir()
        {
            //method to display content of directory
            int fileco = 0, dirco = 0, sizefile = 0;
            Console.WriteLine("Directory of: " + Program.currpath.Trim());
            for (int i = 0; i < Program.currdirectory.directorytable.Count; i++)
            {
                if (Program.currdirectory.directorytable[i].file_attribute == 0x0)
                {
                    Console.WriteLine(Program.currdirectory.directorytable[i].file_size + " ");
                    string n = new string(Program.currdirectory.directorytable[i].file_name);
                    Console.WriteLine(n + "\n");
                    fileco++;
                    sizefile += Program.currdirectory.directorytable[i].file_size;

                }
                else
                {
                    string name = new string(Program.currdirectory.directorytable[i].file_name);
                    Console.WriteLine("<dir>   " + name + "\n");
                    dirco++;
                }
            }
            Console.WriteLine(fileco + " " + "File(s)" + "   " + sizefile + " " + "bytes");
            Console.WriteLine(dirco + " " + "Dir(s)" + "   " + FatTable.Getfreespace() + " " + "bytes free");
        }

        public static void md(string name)
        {
            if (Program.currdirectory.SearchDirectory(name) != -1)
                Console.WriteLine("This File is Already Exist.");
            else
            {
                DirectoryEntry d = new DirectoryEntry(name.ToCharArray(), 0x10, 0);
                Program.currdirectory.directorytable.Add(d);
                Program.currdirectory.WriteDirectory();

                if (Program.currdirectory.parent != null)
                {
                    Program.currdirectory.parent.UpDatecontent(Program.currdirectory.parent);
                    Program.currdirectory.parent.WriteDirectory();
                }
            }
        }

        public static void rd(string name)
        {
            if (Program.currdirectory.SearchDirectory(name) != -1)
            {
                int index = Program.currdirectory.SearchDirectory(name);
                int fc = Program.currdirectory.directorytable[index].first_cluster;

                Directoriy d = new Directoriy(name.ToCharArray(), 0x10, fc, Program.currdirectory);
                d.DeleteDirectory();
                Program.currpath = new string(Program.currdirectory.file_name).Trim();
            }
            else
                Console.WriteLine("The system cannot find the path specified.");

        }



        public static void cd(string name)
        {
            int i = Program.currdirectory.SearchDirectory(name);
            if (i != -1)
            {
                int fc = Program.currdirectory.directorytable[i].first_cluster;
                Directoriy d = new Directoriy(name.ToCharArray(), 0x10, fc, Program.currdirectory);
                Program.currdirectory = d;
                Program.currpath += '\\';
                //Program.currpath += new string(Program.currdirectory.file_name).Trim();
                Program.currpath += new string(d.file_name).Trim();
                d.ReadDirectory();
            }
            else
                Console.WriteLine("The system cannot find the path specified.");
        }

        public static void import(string path)
        {
            // add file from computer to virtual disk

            // if file exist
            if (File.Exists(path))
            {
                // extract file name
                int slash_idx = path.LastIndexOf('\\');
                string file_name = "";
                for (int i = slash_idx + 1; i < path.Length; i++)
                    file_name += path[i];

                // read file content
                string content = File.ReadAllText(path);
                int size = content.Length;

                // search about file in current directory
                int index = Program.currdirectory.SearchDirectory(file_name);
                if (index == -1)
                {
                    // start adding file to current directory
                    int first_cluster = 0;
                    if (size > 0)
                        first_cluster = FatTable.get_available_block();
                    FileEntry f = new FileEntry(file_name.ToCharArray(), 0x0, first_cluster, size, Program.currdirectory, content);

                    // writ file content
                    f.WriteFile();

                    // add to directory table
                    DirectoryEntry d = new DirectoryEntry(file_name.ToCharArray(), 0x0, first_cluster); // +size
                    Program.currdirectory.directorytable.Add(d);
                    Program.currdirectory.WriteDirectory();

                }
                else
                    Console.WriteLine("Can't add file, this file is already exist.");

            }
            else
                Console.WriteLine("This file is not exist.");

        }

        public static void export(string source, string distenation)
        {
            // export file from virtual disk to actual disk

            // search file in virtual disk
            int index = Program.currdirectory.SearchDirectory(source);
            if (index != -1)
            {
                // check distenation
                if (Directory.Exists(distenation))
                {
                    int first_cluster = Program.currdirectory.directorytable[index].first_cluster;
                    int size = Program.currdirectory.directorytable[index].file_size;
                    string content = null;

                    // read file content
                    FileEntry f = new FileEntry(source.ToCharArray(), 0x0, first_cluster, size, Program.currdirectory, content);
                    f.ReadFile();

                    // create file in actual disk and write content
                    StreamWriter sw = new StreamWriter(distenation + source);
                    sw.Write(f.content);

                    // close file
                    sw.Flush();
                    sw.Close();
                }
                else
                    Console.WriteLine("The system can not find the path specified in computer disk.");
            }
            else
                Console.WriteLine("File is not exist in virtual disk.");
        }

        public static void type(string file_name)
        {
            // print content of file
            // search about file in current directory
            int index = Program.currdirectory.SearchDirectory(file_name);
            if (index != -1)
            {
                int first_cluster = Program.currdirectory.directorytable[index].first_cluster;
                int size = Program.currdirectory.directorytable[index].file_size;
                string content = null;

                // read file content
                FileEntry f = new FileEntry(file_name.ToCharArray(), 0x0, first_cluster, size, Program.currdirectory, content);
                f.ReadFile();
                Console.WriteLine(f.content);

            }
            else
                Console.WriteLine("The system can not find the file specified.");

        }

        public static void rename(string old_name, string new_name)
        {
            // rename file or directory

            // search file with old name
            int index = Program.currdirectory.SearchDirectory(old_name);
            if (index != -1)
            {
                // search file with new name
                int index1 = Program.currdirectory.SearchDirectory(new_name);
                if (index1 == -1)
                {
                    DirectoryEntry d = Program.currdirectory.directorytable[index];
                    d.file_name = new_name.ToCharArray();

                    // remove file and add again
                    Program.currdirectory.directorytable.RemoveAt(index);
                    Program.currdirectory.directorytable.Insert(index, d);

                    Program.currdirectory.WriteDirectory();

                }
                else
                    Console.WriteLine("Dublicate file name exist, or file can not be found.");
            }
            else
                Console.WriteLine("The system can not find the file specified.");
        }

        public static void del(string file_name)
        {
            // delete files

            // search file with name
            int index = Program.currdirectory.SearchDirectory(file_name);
            if (index != -1)
            {
                // ensure file
                if (Program.currdirectory.directorytable[index].file_attribute == 0x0)
                {
                    // get first cluster and size
                    int firstcluster = Program.currdirectory.directorytable[index].first_cluster;
                    int fsize = Program.currdirectory.directorytable[index].file_size;

                    // delete file
                    FileEntry f = new FileEntry(file_name.ToCharArray(), 0x0, firstcluster, fsize, Program.currdirectory, null);
                    f.DeleteFile(file_name);

                }
                else
                    Console.WriteLine("The system can not find the file specified.");
            }
            else
                Console.WriteLine("The system can not find the file specified.");
        }


        // copy file name with size and content
        public static void copy(string source, string distenation)
        {
            // search source 
            int index = Program.currdirectory.SearchDirectory(source);
            if (index != -1)
            {
                // extract file name from distenation
                int slash_idx = distenation.LastIndexOf('\\');
                string dis_file_name = "";
                for (int i = slash_idx + 1; i < distenation.Length; i++)
                    dis_file_name += distenation[i];

                // distenatio = current path
                if (dis_file_name == Program.currdirectory.file_name.ToString())
                    Console.WriteLine("Can not copy in current path.");
                else
                {
                    // search distenation
                    int index1 = Program.currdirectory.SearchDirectory(dis_file_name);
                    if (index1 == -1)
                    {
                        // distenation is not exist, make new directory
                        int first_cluster = FatTable.get_available_block();
                        Directoriy d = new Directoriy(dis_file_name.ToCharArray(), 0x10, first_cluster, Program.currdirectory);
                        Program.currdirectory.directorytable.Add(d);

                        // get file first cluster and size
                        int f_cluster = Program.currdirectory.directorytable[index].first_cluster;
                        int f_size = Program.currdirectory.directorytable[index].file_size;


                        //Program.currdirectory = d;
                        //Program.currpath += '\\';
                        //Program.currpath += new string(d.file_name).Trim();

                        // add file to current directory
                        FileEntry f = new FileEntry(source.ToCharArray(), 0x0, f_cluster, f_size, Program.currdirectory, "");
                        Program.currdirectory.directorytable.Add(f);
                        Program.currdirectory.WriteDirectory();

                    }
                }

            }
            else
                Console.WriteLine("The system can not find the path specified.");
        }


        public void PrintValidation()
        {
            bool found = false;

            //create array of commands
            string[] Commands = { "cd", "cls", "dir", "quit", "copy", "del", "help",
                                  "md", "rd", "rename", "type", "import", "export" };


            // split the string into commands and args
            string[] arguments = Command.Split(' ');
            Command = arguments[0];

            // program start
            while (true)
            {
                // if command = quit close the program
                if (Command == "quit")
                    Environment.Exit(0);

                // check what is the command
                for (int i = 0; i < Commands.Length; i++)
                {
                    if (Command == Commands[i])
                    {
                        //Console.WriteLine("Valid Command");
                        if (Command == "help")
                        {
                            //check if command with args or not
                            if (arguments.Length == 1)
                                Help.help();
                            else if (arguments.Length == 2)
                                Help.help(arguments[1]);
                            else if (arguments.Length > 2)
                                Console.WriteLine("HELP [command] command - displays help information on that command.");
                            else if (argument != " ")
                                Help.help(argument);
                            else
                                Help.help();
                        }
                        else if (Command == "cls")
                            Console.Clear();
                        else if (Command == "dir")
                        {
                            // check number of words typed
                            if (arguments.Length > 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                dir();
                        }
                        else if (Command == "md")
                        {
                            // check number of words typed
                            if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                            {
                                for (int arg = 1; arg < arguments.Length; arg++)
                                    md(arguments[arg]);
                            }
                        }
                        else if (Command == "rd")
                        {
                            if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                            {
                                for (int arg = 1; arg < arguments.Length; arg++)
                                    rd(arguments[arg]);
                            }
                        }
                        else if (Command == "cd")
                        {
                            if (arguments.Length == 1)
                                Console.WriteLine(Program.currpath);
                            else if (arguments.Length > 2)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                cd(arguments[1]);
                        }
                        else if (Command == "import")
                        {
                            // check number of words typed
                            if (arguments.Length > 2)
                                Console.WriteLine("The system cannot find the path specified.");
                            else if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                import(arguments[1]);
                        }
                        else if (Command == "export")
                        {
                            // check number of words typed
                            if (arguments.Length > 3)
                                Console.WriteLine("The system cannot find the path specified.");
                            else if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                export(arguments[1], arguments[2]);
                        }
                        else if (Command == "type")
                        {
                            // check number of words typed
                            if (arguments.Length > 2)
                                Console.WriteLine("The system cannot find the path specified.");
                            else if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                type(arguments[1]);
                        }
                        else if (Command == "rename")
                        {
                            // check number of words typed
                            if (arguments.Length > 3)
                                Console.WriteLine("The system cannot find the path specified.");
                            else if (arguments.Length == 1 || arguments.Length == 2)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                rename(arguments[1], arguments[2]);
                        }
                        else if (Command == "del")
                        {
                            // check number of words typed
                            if (arguments.Length > 2)
                                Console.WriteLine("The system cannot find the path specified.");
                            else if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                del(arguments[1]);
                        }
                        else if (Command == "copy")
                        {
                            // check number of words typed
                            if (arguments.Length > 3)
                                Console.WriteLine("The system cannot find the path specified.");
                            else if (arguments.Length == 1)
                                Console.WriteLine("The syntax of the command is incorrect.");
                            else
                                copy(arguments[1], arguments[2]);
                        }

                        // if the command is valid
                        found = true;
                        break;
                    }

                }


                // if the command is invalid
                if (!found)
                    Console.WriteLine(Command + " is not recognized as an internal or external command.");

                // get more commands
                Console.Write("\n" + Program.currpath + ">");
                Command = Console.ReadLine();
                Command = Command.ToLower();

                //split the string into commands and args
                arguments = Command.Split(' ');
                Command = arguments[0];

                found = false;

                // check if the command has arguments
                //if (arguments.Length > 1)
                //argument = arguments[1];

            }
        }

    }
}
