using System;
using System.Collections.Generic;
using System.Text;

namespace CmdCommands
{
    public class DirectoryEntry
    {
        public char[] file_name = new char[11];
        public byte[] file_empty = new byte[12]; // group of zeros
        public byte file_attribute; // 0x0 for files, or 0x10 for folders
        public int file_size;
        public int first_cluster;

        public DirectoryEntry(char[] name, byte attr, int cluster)
        {
            // check file name size
            string s = new string(name);
            if (name.Length < 11)
            {
                for (int i = name.Length; i < 11; i++)
                    s += " ";
                name = s.ToCharArray();
            }

            // initialize file name, attribute, first cluster
            this.file_name = name;
            this.file_attribute = attr;
            this.first_cluster = cluster;
        }
        public DirectoryEntry() { }

        // return data in bytes
        public byte[] get_bytes()
        {
            // convert name from char[] to bytes[]
            byte[] b = new byte[32];
            byte[] name = new byte[11];
            name = Encoding.ASCII.GetBytes(file_name);

            // check name size 
            for (int i = 0; i < 11; i++)
            {
                if (i < name.Length)
                    b[i] = name[i];
                else
                    b[i] = (byte)' ';

            }

            // store file attribute
            b[11] = file_attribute;

            // store file empty 
            for (int i = 0; i < 12; i++)
            {
                b[i + 12] = 0;
            }

            // convert first cluster to bytes
            byte[] fc = new byte[4];
            fc = BitConverter.GetBytes(first_cluster);
            for (int i = 0; i < 4; i++)
                b[i + 24] = fc[i];

            // convert file size to bytes
            byte[] fs = new byte[4];
            fs = BitConverter.GetBytes(file_size);
            for (int i = 0; i < 4; i++)
                b[i + 28] = fs[i];

            return b;
        }

        public DirectoryEntry get_directory_entry(byte[] b)
        {
            DirectoryEntry de = new DirectoryEntry();

            // convert file name
            for (int i = 0; i < 11; i++)
                de.file_name[i] = (char)b[i];

            // store file attrib
            de.file_attribute = b[11];

            // store file empty
            for (int i = 0; i < 12; i++)
                de.file_empty[i] = 0;

            // convert first cluster
            byte[] fc = new byte[4];
            for (int i = 24; i < 28; i++)
                fc[i % 24] = b[i];
            de.first_cluster = BitConverter.ToInt32(fc, 0);

            // convert file size
            byte[] fs = new byte[4];
            for (int i = 28; i < 32; i++)
                fs[i % 28] = b[i];
            de.file_size = BitConverter.ToInt32(fs, 0);

            return de;
        }

        public DirectoryEntry get_directory_entry()
        {
            // object to store data and return directory entry
            DirectoryEntry d1 = new DirectoryEntry(this.file_name, this.file_attribute, this.first_cluster);
            d1.file_size = this.file_size;
            d1.file_empty = this.file_empty;

            return d1;
        }
    }
}
