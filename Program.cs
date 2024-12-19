using System;
using System.IO;

//TODO: More testing required, check each part of the code individually and check off here
class Program{
    static void Main(string[] args){
        //string? gets rid of null warnings by telling the program that the variable can hold a null
        string? folder_path = "", sort_type = "", need_help, manual_naming = "";
        bool boolcontinue;
        List<string> new_folders = new List<string>();

        //gives the user the choice between wanting help or not, input is read regardless
        Console.WriteLine("Do you need help with using the program? y/n/enter(skip)");
        need_help = Console.ReadLine();

        if(need_help == "y"){

            Console.WriteLine("What folder do you want to sort?");
            Console.WriteLine("Give the file to folder in this format (C:\\path\\to\\folder) or (/path/to/file) depending on your OS");
            folder_path = Console.ReadLine();

            Console.WriteLine("Here's how you can sort your files:\n1. By extension\n2. By name\n3. By date made\n4. Last edited\n5. Last opened");
            Console.WriteLine("Please enter the number corresponding to the way you'd like to sort your files");
            sort_type = Console.ReadLine();

            Console.WriteLine("Would you like to name the folders manually? y/n");
            manual_naming = Console.ReadLine();

            boolcontinue = true;
        }
        else if(need_help == "n" || need_help == ""){
            folder_path = Console.ReadLine();
            sort_type = Console.ReadLine();
            manual_naming = Console.ReadLine();

            boolcontinue = true;
        }
        else{
            boolcontinue = false;
        }

        //starts the sorting of files as long as the user has correctly assigned values to the variables
        if(boolcontinue && Directory.Exists(folder_path) && (manual_naming == "y" || manual_naming == "n")
         && new List<string> {"1","2","3","4","5"}.Contains(sort_type ?? "6")){
            switch(sort_type){
                /* the functions, apart from sorting files, also returns the names of newly made files, these will then be used later
                    in case the user has opted to manually name the files */
                case "1":
                    new_folders = SortExtension(folder_path);
                    break;
                case "2":
                    new_folders = SortName(folder_path);
                    break;
                case "3":
                    new_folders = SortDate(folder_path, sort_type);
                    break;
                case "4":
                    new_folders = SortDate(folder_path, sort_type);
                    break;
                case "5":
                    new_folders = SortDate(folder_path, sort_type);
                    break;
            }

            //renames the files in case the user has opted for this
            if(manual_naming == "y"){
                        string? new_name;
                        char separator = Path.DirectorySeparatorChar;
                        List<string> invalid = new List<string> {"\\","/",":","*","?","\"","<",">","|"};
                        bool validname = false;

                        //goes through each new folder and asks the user for a name
                        foreach(string folder in new_folders){
                            Console.WriteLine("What would you like to name the folder \"" + folder + "\"\nPlease only provide the name and not the path");
                            new_name = Console.ReadLine();
                            new_name ??= string.Empty;

                            /* If the user enters an invalid name, either by entering nothing, or using characters that aren't allowed
                            in file names, the program will keep asking the user for a valid name until they enter one*/
                            while (!validname){
                                validname = true;

                                foreach (string character in invalid){
                                    if (new_name.Contains(character)){
                                        validname = false;
                                    }
                                }

                                if(string.IsNullOrWhiteSpace(new_name) || new_name == Path.GetFileName(folder)){
                                    validname = false;
                                }

                                if (!validname){
                                    Console.WriteLine("Please give a valid name");
                                    new_name = Console.ReadLine();
                                    new_name ??= string.Empty;
                                }
                            }

                            Directory.Move(folder, folder_path + separator + new_name);
                        }
                    }

            Console.WriteLine("End of program");
            Console.ReadLine();
        }
        else{
            Console.Write("Your input is incorrect, please double-check it");

            //this is so the programm doesn't instantly close, and the user can read the message above
            Console.ReadLine();
        }
        
    }

    //function for sorting based on extensions
    static List<string> SortExtension(string folder_path){
        
        int location;
        string extension, name_folder, name_file, new_folder_path;
        string[] files;
        List<string> new_folders = new List<string>();
        char separator;

        //puts the paths to all files in the specified folder into an array
        files = Directory.GetFiles(folder_path);

        //gets the appropriate separator for the file system
        separator = Path.DirectorySeparatorChar;

        //goes through each file and puts it in the appropriate folder
        foreach (string file in files){
            //finds what extension the file has
            location = file.LastIndexOf(".");
            extension = file.Substring(location+1);

            //gives the folder a (temporary) name and puts it's path in a variable
            name_folder = extension + "folder";
            new_folder_path = folder_path + separator + name_folder;


            //checks if a directory for the extension exists and creates it if not
            if(!Directory.Exists(new_folder_path)){
                Directory.CreateDirectory(new_folder_path);
                new_folders.Add(new_folder_path);
            }

            //find name of the file
            name_file = Path.GetFileName(file);

            //adds the files to the target folder
            try{
                File.Move(file, (new_folder_path + separator + name_file));
            }
            catch(Exception error){
                Console.WriteLine("A problem arose moving the file " + file + ": " + error.ToString());
            }
        }

        //returns all new folder created by the function
        return new_folders;
    }

    //function for sorting based on the first letter of the file
    static List<string> SortName(string folder_path){
        string name_folder, name_file, letter, new_folder_path;
        string[] files;
        List<string> new_folders = new List<string>();
        char separator;

        //puts the paths to all files in the specified folder into an array
        files = Directory.GetFiles(folder_path);

        //gets the appropriate separator for the file system
        separator = Path.DirectorySeparatorChar;

        //goes through each file and puts it in the appropriate folder
        foreach (string file in files){
            //find name of the file
            name_file = Path.GetFileName(file);

            //take first letter from filename
            letter = name_file.Substring(0, 1);

            //give the folder a name and puts it's path in a variable
            name_folder = letter + "folder";
            new_folder_path = folder_path + separator + name_folder;

            //check if a directory for the letter exists and creates it if not
            if(!Directory.Exists(new_folder_path)){
                Directory.CreateDirectory(new_folder_path);
                new_folders.Add(new_folder_path);
            }

            //adds the files to the target folder
            try{
                File.Move(file, (new_folder_path + separator + name_file));
            }
            catch(Exception error){
                Console.WriteLine("A problem arose moving the file " + file + ": " + error.ToString());
            }
        }

        //returns all new folder created by the function
        return new_folders;
    }

    /* functions for sorting based on date (creation date, last accessed, last edited). Originally I had three separate functions for these
    but the code was nearly identical so I combined them into one functions to improve readability and make it easier should the function
    need to be edited later on */
    static List<string> SortDate(string folder_path, string sort_type){
        string name_folder, name_file, date, new_folder_path;
        DateTime time = DateTime.MinValue;
        string[] files;
        List<string> new_folders = new List<string>();
        char seperator;

        //puts the paths to all files in the specified folder into an array
        files = Directory.GetFiles(folder_path);

        //gets the appropeiate separator for the file system
        seperator = Path.DirectorySeparatorChar;

        //goes through each file and puts it in the appropriate folder
        foreach (string file in files){
            //find name of the file
            name_file = Path.GetFileName(file);

            //uses user input to determine what date it should take from the file
            switch (sort_type){
                case "3":
                    time = File.GetCreationTime(file);
                    break;
                case "4":
                    time = File.GetLastWriteTime(file);
                    break;
                case "5":
                    time = File.GetLastAccessTime(file);
                    break;
            }

            //changes the date so it's valid for a filename
            date = time.Day + "-" + time.Month + "-" + time.Year;

            //give folder a name and puts the path in a variable
            name_folder = date + "folder";
            new_folder_path = folder_path + seperator + name_folder;

            //check if a directory for the date exists and creates it if not
            if(!Directory.Exists(new_folder_path)){
                Directory.CreateDirectory(new_folder_path);
                new_folders.Add(new_folder_path);
            }

            //adds the files to the target folder
            try{
                File.Move(file, (new_folder_path + seperator + name_file));
            }
            catch(Exception error){
                Console.WriteLine("A problem arose moving the file " + file + ": " + error.ToString());
            }
        }
        
        //returns all newly made folder by the function
        return new_folders;
    }
}