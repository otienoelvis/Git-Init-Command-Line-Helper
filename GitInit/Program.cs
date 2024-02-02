// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, There!");
var main = new GitInit();
try
{


    main.Create();
}
catch (Exception e)
{
    main.LogToFile($"\n Error: {e}, Message: {e.Message}");
    Console.WriteLine($"Error: {e}, Message: {e.Message}");
    throw;
}


public class GitInit
{
    public string mainBranch;
    public string firstCommitMessage;
    public string remoteOrigin;
    public string proceedYesNo = "Y";
    public bool proceed = false;
    public string addAllFiles = "Y";
    public string currentDirectory = Environment.CurrentDirectory;
    public List<string> actionPlans = new List<string>();

    public void Create()
    {
        Console.WriteLine("Name of your main/master branch: ");
        mainBranch = Console.ReadLine().Trim();


        Console.WriteLine("Add all files? (Y)");
        addAllFiles = Console.ReadLine().Trim();

        Console.WriteLine("First commit message: (leave blank for later)");
        firstCommitMessage = Console.ReadLine().Trim();

        Console.WriteLine("RemoteOrigin URL: (leave blank for later)");
        remoteOrigin = Console.ReadLine().Trim();



        Regex validBranchNameRegex = new Regex("^[a-zA-Z0-9._-]+$");

        if (!validBranchNameRegex.IsMatch(mainBranch))
        {
            Console.WriteLine("Invalid Git branch name. Branch names can only contain letters, numbers, dots, underscores, and hyphens.");
            Create();
        }
        else
        {
            Console.WriteLine("Valid Git branch name.");

            string currentDirectory = Environment.CurrentDirectory;
            Console.WriteLine($"Current directory: {currentDirectory}");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();

            actionPlans.Add("git init");

            if (!string.IsNullOrEmpty(addAllFiles) && addAllFiles == "Y")
            {
                actionPlans.Add($"git add .");
            }

            if (!string.IsNullOrEmpty(firstCommitMessage))
                actionPlans.Add($"git commit {firstCommitMessage}");
            actionPlans.Add($"git branch -M {mainBranch}");
            actionPlans.Add($"git remote add origin {remoteOrigin}");
            // actionPlans.Add($"git push -u origin {remoteOrigin}");



            Console.WriteLine($"The following actions will be performed on this path {currentDirectory}");
            foreach (var item in actionPlans)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Do you wish to proceed? (Y)");
            proceedYesNo = Console.ReadLine();
            if (string.IsNullOrEmpty(proceedYesNo))
            {
                proceedYesNo = "Y";
            }

            JsonSerializer.Serialize(actionPlans);
            if (proceedYesNo == "Y")
            {
                foreach (var item in actionPlans)
                {
                    Console.WriteLine($"Running ->  {item}");
                    //LogToFile($"Running ->  {item}");
                    process.StandardInput.WriteLine(item);
                }

                Console.WriteLine("Added your git!!");
                //continue;
            }
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(output);
            LogToFile(output);

        }
    }

    public void LogToFile(string message)
    {
        string logFilePath = $"{currentDirectory}/log.txt";

        // Append the log message to the file
        File.AppendAllText(logFilePath, $"{DateTime.Now} - {message}\n");
    }


}