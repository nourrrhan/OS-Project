# File System Project

This project demonstrates the implementation of various file system operations and interaction with the file system. It involves reading and writing data to a file, creating and displaying a FAT table, and validating user input commands.

Tools: Visual Studio 2019

Language: C#

The project comprises 9 classes:

1- Program Class: This class contains the main function that initiates the project. It starts by printing the current directory to the user in the console window. Then, it creates a virtual disk, accepts user commands, validates them using the CommandValidation class, and calls the PrintValidation function.

---------------------------------------

2- CommandValidation Class: This class includes the PrintValidation function, which checks the validity of user commands. It stores the current virtual file path, captures the first command from the user, and saves it. The commands are stored in an array. It checks if a command has any arguments (e.g., help md) and stores the command in one variable and the argument in another.

If the user types "quit," the console is closed.

If the user types "help," the argument is checked. If it is not empty, information specific to that command is printed. Otherwise, information about all commands is printed using the Help class.

If the user types "cls," the screen is cleared.

If the user types "dir," the contents of the current directory are printed.

If the user types "md," a new directory is created in the current directory with a specified name.

If the user types "rd," a specific directory is removed from the current directory.

If the user types "cd," the current directory is changed to the specified directory.

If the user types "import," a file is imported from the actual disk to the virtual disk.

If the user types "export," a file is exported from the virtual disk to the actual disk.

If the user types "type," the content of a file with the specified name is printed.

If the user types "rename," the name of a directory is changed to a new name entered by the user.

If the user types "del," a specified file is removed from the virtual disk.

If the user types "copy," a specified file is copied from the virtual disk to another path in the virtual disk.

If the user enters a valid command, a Boolean value is set to true.

If the user enters an invalid command, a message is displayed indicating that the command was incorrect. The program continues to accept commands and checks for their validity.

---------------------------------------

3- Help Class: This class is dedicated to the "help" command. It stores all commands in an array and their corresponding information in another array. It includes a function to print information about all commands and another function to print information about a specific command (e.g., "help copy").

---------------------------------------

4- Path Class: This class handles the setting and retrieval of the current directory whenever required by other classes.

---------------------------------------

5- VirtualDisk Class: This class focuses on the virtual file.

The "Initialize" function creates the virtual disk and initializes it if it doesn't already exist. It also initializes the FAT table and creates a root directory.

The "ReadBlock" and "WriteBlock" functions are responsible for reading and writing a single block of data in the file.

---------------------------------------

6- FatTable Class: This class manages the FAT table within the mini-FAT system.

It includes a function to declare and initialize the FAT table.

The "WriteFatTable" function writes the FAT table to the virtual disk by converting the data to bytes and writing them after skipping the first 1024 bytes.

The "ReadFatTable" function reads the FAT table from the virtual disk by skipping the first 1024 bytes, converting the data to integers, and returning them.

Other functions are provided to determine the index of the first empty block, set and retrieve the next index of a specific block, calculate the number of empty blocks in the FAT table, and determine the free space in the disk.

---------------------------------------

7- DirectoryEntry Class: This class handles directory entries. It has a constructor to set the data.

The "ConvertToBytes" function converts the directory entry data from any other datatype to bytes, stores them in an array, and returns the array.

The "ConvertFromBytes" function converts the directory entry data from bytes to its original datatypes, stores them in an object, and returns the object.

---------------------------------------

8- Directory Class: This class deals with files and directories. It has a constructor to set the data.

The "WriteDirectory" function writes a directory to the virtual disk by storing its directory table and directory entry. It selects an available block to write in and performs the writing process.

The "ReadDirectory" function reads a directory from the virtual disk by creating variables for the directory table and directory entry. It iterates through the content, reads and stores data in the directory table.

The "SearchDirectory" function searches for a directory by its name in the directory table and returns its index if found.

The "UpdateDirectory" function updates the content of a directory by reading the directory data, searching for its index, removing the old directory, and writing the new one.

The "RemoveDirectory" function removes a directory by deleting its content from the FAT table, removing it from its parent directory, and updating the FAT table.

---------------------------------------

9- FileEntry Class: This class handles file operations. It has a constructor to set the data.

The "ReadContent" function reads the content of the file in bytes and converts it to a string.

The "WriteContent" function writes content to the file by converting it from a string to bytes. It determines the first empty block to write in, calculates the number of full blocks, handles the remaining blocks, and updates the FAT table.

The "RemoveFile" function removes the file from the virtual disk by setting its content to null using the FAT table and removing the file from its parent directory table.











