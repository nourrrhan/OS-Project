using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CmdCommands
{
    public class FileEntry : DirectoryEntry
    {
        public string content;
        public Directoriy parent;
        public int fsize;

        public FileEntry(char[] name, byte attr, int cluster, int size, Directoriy parent, string content) : base(name, attr, cluster)
        {
            this.parent = parent;
            this.content = content;
            file_size = size;
        }


        // method to read content of file
        public void ReadFile()
        {
            List<byte> ls = new List<byte>();
            int fatindex, next;
            // check file
            if (first_cluster != 0 && FatTable.get_next(first_cluster) != 0)
            {
                fatindex = first_cluster;
                next = FatTable.get_next(fatindex);
                do
                {
                    // read block and add to list
                    ls.AddRange(VirtualDisk.ReadBlock(fatindex));

                    // update fatindex 
                    fatindex = next;
                    if (fatindex != -1)
                        next = FatTable.get_next(fatindex);

                } while (fatindex != -1);

                // add 32 byte of array in directory table2
                string s = "";
                for (int i = 0; i < ls.Count; i++)
                {
                    // convert from byte to char and store
                    if ((char)ls[i] != '\0')
                        s += (char)ls[i];
                }
                content = s;
            }
        }


        //method to write content in file
        public void WriteFile()
        {
            // convert content to byte
            byte[] byte_content = Encoding.ASCII.GetBytes(content);

            // total number of required blocks
            int required_blocks = (int)(Math.Ceiling(content.Length / 1024.0)); // 2050 / 1024 = 3
            int full_blocks = content.Length / 1024; // 2050 / 1024 = 2
            int reminder = content.Length % 1024; // 2050 % 1024

            // check if first cluster is reserved
            int fat_index, last_index = -1;
            if (first_cluster != 0)
                fat_index = first_cluster;
            else
            {
                // get available block to start write
                fat_index = FatTable.get_available_block();
                first_cluster = fat_index;
            }


            // check if there is available blocks to write content
            if (required_blocks <= FatTable.get_available_blocks())
            {
                List<byte[]> l = new List<byte[]>();

                // if content has full blocks
                if (full_blocks > 0)
                {
                    int count = 0;
                    for (int j = 0; j < full_blocks; j++)
                    {
                        // store full blocks
                        byte[] list = new byte[1024];
                        for (int i = 0; i < 1024; i++)
                        {
                            list[i] = byte_content[count];
                            count++;
                        }
                        l.Add(list);
                    }
                }

                // if content has reminder block
                if (reminder > 0)
                {
                    // start from end of full blocks
                    int ic = 1024 * full_blocks;
                    byte[] list = new byte[1024];
                    for (int i = 0; i < reminder; i++)
                    {
                        // store reminder blocks
                        list[i] = byte_content[ic];
                        ic++;
                    }
                    l.Add(list);
                }


                // write blocks in virtual disk
                for (int i = 0; i < l.Count; i++)
                {
                    VirtualDisk.WriteBlock(l[i], fat_index);
                    FatTable.set_next(fat_index, -1);
                    if (last_index != -1)
                        FatTable.set_next(fat_index, last_index);

                    // get next available block
                    last_index = fat_index;
                    fat_index = FatTable.get_available_block();
                }
            }
            FatTable.write_fattable();

        }


        // method to delete file
        public void DeleteFile(string file_name)
        {
            // delete the content of the file
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

            // delete the file from its parent
            if (parent != null)
            {
                // get file index in directory table
                parent.ReadDirectory();
                string n = file_name;
                int i = parent.SearchDirectory(n);
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
