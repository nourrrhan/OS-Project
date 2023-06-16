# File System Project

This project shows how to interact with the file system and perform various file system operations.
It reads and writes data to a file, creates and prints a FAT table, and validates user input commands.

Tools: Visual Studio 2019.
Languages: C#

This Project contains 9 classes:


###  Class Program
This class contains the main function which starts the project.
First, we print the current directory to the user in the console window,
then we create our virtual disk and start taking commands from the user, 
and then we check weather the command is valid or not by creating an object from
CommandValidation class,
and then calling the PrintValidation function.

---------------------------------------------

### Class CommandValidation
This class contains the PrintValidation function which checks if the command validation.
We stored the current virtual file path and took the first command from the user and stored it.
We created an array called commands to store all commands, checked for the command if it contains any arguments (ex. help md) then we store the
command “help” in variable and the argument “md” in another variable.

We check if the user typed “quit”, then we close the console.

If the user typed “help”, we then check the argument and print its information if it is not empty,
else we print information about all commands by creating an object from Help class and then calling help function.

If the user typed “cls”, then we clear the screen.

If the user typed “dir”, we print the content of the current directory.

If the user typed “md”, we create a new directory in the current directory with specific name.

If the user typed “rd”, we remove the specific directory from current directory.

If the user typed “cd”, we change the current directory to the directory specified.

If the user typed “import”, we import a file from our actual disk to our virtual disk.

If the user typed “export”, we export a file from our virtual disk to our actual disk.

If the user typed “type”, we print the content of the file with specified name.

If the user typed “rename”, we change the name of the directory to a new name entered by the user.

If the user typed “del”, we remove specified file from virtual disk.

If the user typed “copy”, we copy the file specified from virtual disk to another path in the virtual disk.

And finally, if the user entered a valid command we set the Boolean to be true.

Now if the user entered an invalid command, we print a message tells that the command was wrong.
And then we start taking more commands from the user and check for its validation.

---------------------------------------------

###  Class Help
This class specializes in “help” command.
We stored all commands in an array, and their information in another array.
We created a function to print all commands and their information (ex. “help”).
We created another function to print information about specific command (ex. “help copy”).

---------------------------------------------

### Class Path
This class is specialized in setting the current directory, and returning it whenever any class need.

---------------------------------------------

### Class VirtualDisk
This class is specialized in the virtual file.

"Initialize" Function to create the virtual disk and initialize it if it was not exist, and then initialize fat table and create a root directory.

"ReadBlock" and "WriteBlock" Functions to write a single block in the file, and read a single block from the file.

---------------------------------------------

### Class FatTable
This class is specialized in the fat table that in mini-fat system.

Function to declare and initialize the fat table.

Function to write fat table in virtual disk, by skipping first 1024 byte and converting the rest of data to bytes, then writing them.

Function to read fat table from virtual disk by skipping first 1024 bytes and converting the rest of data to integers, and then returning them.

Function to return the index of the first empty block.

Functions to set and return the next of specific index.

Function to return the number of empty blocks in the fat table.

Function to return the free space in the disk.

---------------------------------------------

### Class DirectoryEntry
This class is specialized in the directory entry.
We have a constructor to set data.

Function to convert directory entry data from any other datatype to bytes, store them in array and return the array.

Function to convert directory entry data from byte to its original datatypes, store them in one object and then return the object.

---------------------------------------------

### Class Directory
This class is specialized in files and directories.
We have a constructor to set data.

Function to write a directory in the virtual disk by storing its directory table and directory entry,
choosing an available block to write in, and then start writing.

Function to read a directory from the virtual disk by creating variables for its directory table and directory entry,
looping through its content, and then start reading and storing data in the directory table.

Function to search about the directory by its name in directory table, and return its index if it exists.

Function to update the content of a directory by reading directory data,
and searching about its index, then remove the old directory and write the new one.

Function to remove a directory by deleting its content from fat table, then deleting itself from its parent directory, and finally write fat table.

---------------------------------------------

### Class FileEntry
This class is specialized in dealing with files.
We have a constructor to set data.

Function to read content from the file in bytes and convert it to string.

Function to write content in the file by converting it from string to byte,
and decide the first empty block to write in, and calculate the number of full
blocks to write, number of reminder block, and finally start writing in the file,
and updating fat table.

Last function to remove the file from virtual disk by setting its content to null using fat table,
and then removing the file itself from its parent directory table.







