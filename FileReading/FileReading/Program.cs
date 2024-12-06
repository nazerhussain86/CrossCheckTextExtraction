using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string pythonExePath = @"D:\\PythonScript\\newpythondecember\\venv\\Scripts\\python.exe";
        string scriptPath = @"D:\\PythonScript\\newpythondecember\\Original_PDF_to_Txt_Script.py";
        string pdfFilePath = @"C:\Users\SCM041\Downloads\New File.pdf";

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonExePath,
            Arguments = $"\"{scriptPath}\" \"{pdfFilePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false, // Ensure standard input is not required
            CreateNoWindow = true
        };

        try
        {
            using (Process? process = Process.Start(start))
            {
                if (process == null)
                {
                    Console.WriteLine("Failed to start the process.");
                    return;
                }

                // Read output and error streams
                string output = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Check exit code
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Python script exited with code {process.ExitCode}");
                    Console.WriteLine($"Error Output: {errorOutput}");
                }
                else
                {
                    Console.WriteLine("Python script executed successfully.");
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        try
                        {
                            string[]? pageTexts = JsonConvert.DeserializeObject<string[]>(output);
                            Console.WriteLine("Extracted Text:");
                            if (pageTexts != null)
                            {
                                foreach (var line in pageTexts)
                                {
                                    Console.WriteLine(line);
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                            Console.WriteLine($"Raw Output: {output}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No output returned from Python script.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
