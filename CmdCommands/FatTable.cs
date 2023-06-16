using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CmdCommands
{
    public class FatTable
    {

        static int[] fat_table;
        public static string path = @"D:\Virtual disk.txt";


        public static void initialize_fattable()
        {
            // initialize fat table
            fat_table = new int[1024];

            for (int i = 0; i < 1024; i++)
                if (i < 5)
                    fat_table[i] = -1;
        }

        public static void write_fattable()
        {
            // store fat table data in the file

            // open text file
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write);

            // skip 1024 byte by seek function
            fs.Seek(1024, SeekOrigin.Begin);

            // write fat table in the file
            for (int i = 0; i < 1024; i++)
            {
                // convert from int to byte
                byte[] byte_fat = new byte[4];
                byte_fat = BitConverter.GetBytes(fat_table[i]);

                // write in the file
                fs.Write(byte_fat, 0, byte_fat.Length);
            }

            // close file
            fs.Close();
        }

        public static void get_fattable()
        {
            // get fat table data from the file

            // open file
            FatTable.initialize_fattable();
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            // skip 1024 byte by seek function
            fs.Seek(1024, SeekOrigin.Begin);

            // read in byte array from file 
            for (int i = 0; i < 1024; i++)
            {
                // read bytes from file
                byte[] byte_fat = new byte[4];
                fs.Read(byte_fat, 0, byte_fat.Length);

                // convert bytes to int and store them
                fat_table[i] = BitConverter.ToInt32(byte_fat, 0);
            }

            //close file
            fs.Close();
        }

        public void print_fattable()
        {
            // get fat table
            //int [] fat=get_fattable();

            // print fat table in console
            for (int i = 0; i < 1024; i++)
            {
                Console.Write(i);
                Console.Write(' ');
                Console.WriteLine(fat_table[i]);
            }
        }

        public static int get_available_block()
        {
            // if fat = 0 print index
            for (int i = 0; i < 1024; i++)
            {
                if (fat_table[i] == 0)
                    return i;
            }

            // if there is no empty blocks
            return -1;
        }

        public static int get_next(int index)
        {
            // get next block
            return fat_table[index];
        }

        public static void set_next(int index, int value)
        {
            // set value to index
            fat_table[index] = value;
        }

        public static int get_available_blocks()
        {
            // count number of empty blocks and return counter
            int counter = 0;

            for (int i = 0; i < 1024; i++)
            {
                if (fat_table[i] == 0)
                    counter++;
            }

            return counter;
        }
        public static int Getfreespace()
        {
            return get_available_blocks() * 1024;
        }

    }
}
