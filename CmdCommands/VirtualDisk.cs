using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CmdCommands
{
    public class VirtualDisk
    {
        public static string path = @"D:\Virtual disk.txt";
        public VirtualDisk(string Path)
        {
            path = Path;
        }

        public static void Initialize(string path)
        {
            if (!File.Exists(path))
            {  //initiate variable to write on file
                StreamWriter writer = new StreamWriter(path);
                //first loop to put 1024 byte of zeros "first block"
                for (int i = 0; i < 1024; i++)
                    writer.Write('0');

                //second loop to write 1024 * 4 byte of '*'  "reserve place for FatTable"
                for (int i = 0; i < 1024 * 4; i++)
                    writer.Write('*');

                //third loop to write 1024 * 1019 of '#' "Data File"
                for (int i = 0; i < 1024 * 1019; i++)
                    writer.Write('#');
                writer.Close();
                //FatTable fat = new FatTable();
                FatTable.initialize_fattable();
                Directoriy root = new Directoriy("H:".ToCharArray(), 0x10, 5, null);

                root.WriteDirectory();
                Program.currdirectory = root;
                FatTable.write_fattable();
            }
            else
            {
                // FatTable f1 = new FatTable();
                FatTable.get_fattable();
                //char[] filename = new char[1];
                Directoriy root = new Directoriy("H:".ToCharArray(), 0x10, 5, null);
                root.ReadDirectory();
                Program.currdirectory = root;
            }
        }


        public static void WriteBlock(byte[] data, int index)
        {
            //open file to write
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write);

            //write from specific place
            fs.Seek(1024 * index, SeekOrigin.Begin);

            //write the data in file
            fs.Write(data, 0, data.Length);

            //close file
            fs.Close();
        }
        public static byte[] ReadBlock(int index)
        {
            //open file to read
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            //seek 1024 bytes * index to read specific data
            fs.Seek(1024 * index, SeekOrigin.Begin);

            // read data
            byte[] b = new byte[1024];
            fs.Read(b, 0, b.Length);

            // close file
            fs.Close();

            return b;
        }
    }
}
