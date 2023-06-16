using System;
using System.Collections.Generic;
using System.Text;

namespace CmdCommands
{
    public class Directoriy : DirectoryEntry
    {
        public List<DirectoryEntry> directorytable;
        public Directoriy parent;


        public Directoriy(char[] name, byte attr, int cluster, Directoriy parent) : base(name, attr, cluster)
        {
            directorytable = new List<DirectoryEntry>();
            this.parent = parent;
        }



        public void WriteDirectory()
        {  // method to write directory table in virtual disk

            //declare array of bytes to store directory table
            byte[] Directory_table_bytes = new byte[32 * directorytable.Count];

            //declare array of bytes to store directory entry
            byte[] Directory_entry_bytes = new byte[32];
            for (int i = 0; i < directorytable.Count; i++)
            {
                //read all bytes from the specific dir entry
                Directory_entry_bytes = directorytable[i].get_bytes();
                //write bytes of data entry in dirctory table bytes 
                for (int j = i * 32, c = 0; c < 32; j++, c++)
                    Directory_table_bytes[j] = Directory_entry_bytes[c];
            }

            // get numbers of block to store directory table
            double number_of_required_block = Math.Ceiling(Directory_table_bytes.Length / 1024.0);
            //get number of full bolcks
            int number_of_full_size_block = Directory_table_bytes.Length / 1024;
            //get number of reminder blocks
            int reminder = Directory_table_bytes.Length % 1024;

            //create list of array of bytes to store the blocks
            int fatindex, lastindex = -1;
            //check if firstcluster is reserved
            if (first_cluster != 0)
                fatindex = first_cluster;
            else
            {   //get available block and store it in fatindex
                fatindex = FatTable.get_available_block();
                first_cluster = fatindex;
            }

            List<byte[]> ls = new List<byte[]>();
            if (number_of_required_block <= FatTable.get_available_blocks())
            {
                if (Directory_table_bytes.Length > 0)
                {
                    byte[] b = new byte[1024];
                    for (int i = 0; i < number_of_full_size_block; i++)
                    {
                        for (int j = i * 1024; j < ((i + 1) * 1024); j++)
                            b[j % 1024] = Directory_table_bytes[j];
                        ls.Add(b);
                    }
                    if (reminder > 0)
                    {
                        b = new byte[1024];
                        for (int i = number_of_full_size_block * 1024, k = 0; k < reminder; i++, k++)
                            b[k] = Directory_table_bytes[i];
                        ls.Add(b);
                    }

                    //write the full blocks
                    for (int i = 0; i < ls.Count; i++)
                    {
                        VirtualDisk.WriteBlock(ls[i], fatindex);
                        FatTable.set_next(fatindex, -1);
                        if (lastindex != -1)
                            FatTable.set_next(lastindex, fatindex);
                        lastindex = fatindex;
                        fatindex = FatTable.get_available_block();

                    }
                }

                //write the remainder blocks
                if (directorytable.Count == 0)
                {
                    if (first_cluster != 0)
                    {
                        FatTable.set_next(first_cluster, 0);
                        first_cluster = 0;
                    }
                }
                FatTable.write_fattable();
            }
        }


        public void ReadDirectory()
        {
            // List<DirectoryEntry> directorytable2 = new List<DirectoryEntry>();
            directorytable = new List<DirectoryEntry>();
            List<byte> ls = new List<byte>();
            FatTable f1 = new FatTable();
            string FilePath = @"D:\Virtual disk.txt ";
            VirtualDisk v1 = new VirtualDisk(FilePath);
            int fatindex, next;
            if (first_cluster != 0 && FatTable.get_next(first_cluster) != 0)
            {
                fatindex = first_cluster;
                next = FatTable.get_next(fatindex);

                do
                {
                    //read and write the block in list
                    ls.AddRange(VirtualDisk.ReadBlock(fatindex));
                    //update fatindex 
                    fatindex = next;
                    if (fatindex != -1)
                        next = FatTable.get_next(fatindex);
                } while (fatindex != -1);

                //add every 32 byte of array in directory table2
                byte[] data = new byte[32];
                for (int i = 0; i < ls.Count; i++)
                {
                    data[i % 32] = ls[i];

                    //convert bytes to directory entry 
                    if ((i + 1) % 32 == 0)
                    {
                        DirectoryEntry d = get_directory_entry(data);
                        if (d.file_name[0] != '\0')
                            directorytable.Add(d);
                    }
                }

            }


        }



        public int SearchDirectory(string filename)
        {
            ReadDirectory();
            if (filename.Length < 11)
            {
                for (int i = filename.Length; i < 11; i++)
                    filename += " ";
            }

            for (int i = 0; i < directorytable.Count; i++)
            {
                string s = new string(directorytable[i].file_name);
                if (filename.Equals(s))
                {
                    return i;
                }

            }
            return -1;
        }




        //method to update the content of dirtable
        public void UpDatecontent(DirectoryEntry d)
        {
            ReadDirectory();
            //convert char[] to string
            string filename = new string(d.file_name);
            //search if file exist 
            int index = SearchDirectory(filename);
            if (index != -1)
            {
                //delete the old directory entry and write the new
                directorytable.RemoveAt(index);
                directorytable.Insert(index, d);
            }
            WriteDirectory();

        }

        // method to delete directory
        public void DeleteDirectory()
        {
            if (first_cluster != 0)
            {
                int index = first_cluster;

                int next = FatTable.get_next(index);
                do
                {
                    FatTable.set_next(index, 0);
                    index = next;
                    if (index != -1)
                        next = FatTable.get_next(index);
                }
                while (index != -1);
            }
            if (parent != null)
            {
                parent.ReadDirectory();

                string filename = new string(file_name);
                int i = parent.SearchDirectory(filename);
                if (i != -1)
                {
                    parent.directorytable.RemoveAt(i);
                    parent.WriteDirectory();

                }
            }
            FatTable.write_fattable();

        }

    }
}
