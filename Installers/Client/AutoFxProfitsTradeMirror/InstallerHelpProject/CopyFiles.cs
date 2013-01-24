using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace InstallerHelpProject
{
    public class CopyFiles
    {
        private string AppDataLocation = System.Environment.GetEnvironmentVariable("APPDATA");

        /// <summary>
        /// Specifies the source path and reteives the target path from the string that is passed to it and then calls the Method Copy to 
        /// copy a file from the source to the target
        /// </summary>
        /// <param name="sourcePath"> </param>
        /// <param name="targetPath"> </param>
        public void Run(string sourcePath, string targetPath)
        {
            try
            {
                string sourceDirectory = @sourcePath;
                string targetDirectory = @targetPath + "\\";

                string[] files = Directory.GetFiles(sourceDirectory);
                int numOfFiles = files.Length;

                if (numOfFiles > 0)
                {
                    foreach (string fName in files)
                    {
                        try
                        {
                            string copyFile = Path.GetFileName(fName);
                            string source = @sourceDirectory + copyFile;
                            File.Delete(source);
                        }
                        catch (Exception exception)
                        {
                        }
                    }

                    string[] dirs = Directory.GetDirectories(sourceDirectory);
                    
                    int numOfDirectories = dirs.Length;
                    
                    if (numOfDirectories > 0)
                    {
                        foreach (string dirName in dirs)
                        {
                            if (dirName.Contains("MQL"))
                            {
                                string[] splittedDirName = dirName.Split('\\');

                                string tempDir = splittedDirName[splittedDirName.Length - 1];
                                string sourceDir = @sourceDirectory + "\\" + tempDir;
                                string[] sDirFiles = Directory.GetFiles(sourceDir);
                                
                                if (sDirFiles.Length > 0)
                                {
                                    foreach (string file in sDirFiles)
                                    {
                                        string[] splitFile = file.Split('\\');

                                        string copyFile = Path.GetFileName(file);
                                        string source = sourceDir + "\\" + copyFile;
                                        string target = string.Empty;

                                        if (splitFile[splitFile.Length - 1].Trim() == "AutoFXProfits - Client.mq4")
                                        {
                                            target = @targetDirectory + "experts\\AutoFXProfits - Client.mq4";
                                        }
                                        else if (splitFile[splitFile.Length - 1].Trim() == "Communication Library.dll")
                                        {
                                            target = @targetDirectory + "experts\\libraries\\Communication Library.dll";
                                        }

                                        if(!string.IsNullOrEmpty(target))
                                        {
                                            Copy(source, target);
                                        }
                                    }
                                }
                            }
                            else if (dirName.Contains("Terminal"))
                            {
                                string[] splittedDirName = dirName.Split('\\');

                                string tempDir = splittedDirName[splittedDirName.Length - 1];
                                
                                string sourceDir = @sourceDirectory + "\\" + tempDir;
                                
                                string[] sDirFiles = Directory.GetFiles(sourceDir);
                                
                                if (sDirFiles.Length > 0)
                                {
                                    foreach (string file in sDirFiles)
                                    {
                                        string[] splitFile = file.Split('\\');

                                        string copyFile = Path.GetFileName(file);
                                        string source = sourceDir + "\\" + copyFile;
                                        
                                        string target = AppDataLocation + "\\AutoFX Profits\\AutoFXProfitsClientTerminal\\";
                                        
                                        target = target + splitFile[splitFile.Length - 1].Trim();
                                        if (!string.IsNullOrEmpty(target))
                                        {
                                            Copy(source, target);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                
            }
        }


        /// <summary>
        /// Method Copy copies the file from souce directory to the target directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void Copy(string source, string target)
        {
            try
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }

                File.Copy(source, target);
            }
            catch (Exception exception)
            {
            }
        }
    }

}
