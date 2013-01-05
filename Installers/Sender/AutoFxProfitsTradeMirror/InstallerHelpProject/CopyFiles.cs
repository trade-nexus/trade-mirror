using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
//using TraceSourceLogger;

namespace InstallerHelpProject
{
    public class CopyFiles
    {
        private static readonly Type OType = typeof(CopyFiles);

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

                //MessageBox.Show("No of files found in the installation directory to be deleted = ", numOfFiles.ToString());
                //Logger.Debug("No of files found in the installation directory to be deleted = " + numOfFiles, OType.FullName, "Run");

                if (numOfFiles > 0)
                {
                    foreach (string fName in files)
                    {
                        try
                        {
                            string copyFile = Path.GetFileName(fName);
                            string source = @sourceDirectory + copyFile;
                            File.Delete(source);
                            //MessageBox.Show("File at " + source + " deleted");
                            //Logger.Debug("File at " + source + " deleted", OType.FullName, "Run");
                        }
                        catch (Exception exception)
                        {
                            //MessageBox.Show(exception.ToString());
                            //Logger.Error(exception, OType.FullName, "Run");
                        }
                    }

                    string[] dirs = Directory.GetDirectories(sourceDirectory);
                    
                    int numOfDirectories = dirs.Length;
                    //Logger.Info("Number of directories = " + numOfDirectories, OType.FullName, "Run");
                    //MessageBox.Show("Number of directories = " + numOfDirectories);

                    if (numOfDirectories > 0)
                    {
                        foreach (string dirName in dirs)
                        {
                            //MessageBox.Show("Directory Name: " + dirName);
                            //Logger.Info("Directory Name: " + dirName, OType.FullName, "Run");

                            if (dirName.Contains("MQL"))
                            {
                                string[] splittedDirName = dirName.Split('\\');

                                ////MessageBox.Show(splittedDirName[splittedDirName.Length-1]);

                                string tempDir = splittedDirName[splittedDirName.Length - 1];
                                //MessageBox.Show("TEmp Dir = " + tempDir);
                                //Logger.Info("TEmp Dir = " + tempDir, OType.FullName, "Run");

                                string sourceDir = @sourceDirectory + "\\" + tempDir;
                                //MessageBox.Show("Source Dir = " + sourceDir);
                                //Logger.Info("Source Dir = " + sourceDir, OType.FullName, "Run");

                                string[] sDirFiles = Directory.GetFiles(sourceDir);
                                //MessageBox.Show("Found " + sDirFiles.Length + " Files at " + sourceDir);
                                //Logger.Info("Found " + sDirFiles.Length + " Files at " + sourceDir, OType.FullName, "Run");

                                if (sDirFiles.Length > 0)
                                {
                                    foreach (string file in sDirFiles)
                                    {
                                        string[] splitFile = file.Split('\\');

                                        //MessageBox.Show("File name = " + splitFile[splitFile.Length - 1]);
                                        //Logger.Info("File name = " + splitFile[splitFile.Length - 1], OType.FullName, "Run");

                                        string copyFile = Path.GetFileName(file);
                                        string source = sourceDir + "\\" + copyFile;
                                        //MessageBox.Show("Source path for File = " + source);
                                        //Logger.Info("Source path for File = " + source, OType.FullName, "Run");

                                        string target = string.Empty;

                                        if (splitFile[splitFile.Length - 1].Trim() == "AutoFXProfitsSender.mq4")
                                        {
                                            target = @targetDirectory + "experts\\AutoFXProfitsSender.mq4";
                                            //MessageBox.Show("Target location = " + target);
                                            //Logger.Debug("Target location = " + target, OType.FullName, "Run");
                                        }
                                        else if (splitFile[splitFile.Length - 1].Trim() == "Communication Library.dll")
                                        {
                                            target = @targetDirectory + "experts\\libraries\\Communication Library.dll";
                                            //MessageBox.Show("Target location = " + target);
                                            //Logger.Debug("Target location = " + target, OType.FullName, "Run");
                                        }

                                        if(!string.IsNullOrEmpty(target))
                                        {
                                            Copy(source, target);
                                        }
                                        else
                                        {
                                            //Logger.Debug("Invalid target = " + target, OType.FullName, "Run");
                                            //MessageBox.Show("Invalid target = " + target);
                                        }
                                    }
                                }
                            }
                            else if (dirName.Contains("Terminal"))
                            {
                                string[] splittedDirName = dirName.Split('\\');

                                ////MessageBox.Show(splittedDirName[splittedDirName.Length-1]);

                                string tempDir = splittedDirName[splittedDirName.Length - 1];
                                //MessageBox.Show("TEmp Dir = " + tempDir);
                                //Logger.Info("TEmp Dir = " + tempDir, OType.FullName, "Run");

                                string sourceDir = @sourceDirectory + "\\" + tempDir;
                                //MessageBox.Show("Source Dir = " + sourceDir);
                                //Logger.Info("Source Dir = " + sourceDir, OType.FullName, "Run");

                                string[] sDirFiles = Directory.GetFiles(sourceDir);
                                //MessageBox.Show("Found " + sDirFiles.Length + " Files at " + sourceDir);
                                //Logger.Info("Found " + sDirFiles.Length + " Files at " + sourceDir, OType.FullName, "Run");

                                if (sDirFiles.Length > 0)
                                {
                                    foreach (string file in sDirFiles)
                                    {
                                        string[] splitFile = file.Split('\\');

                                        //MessageBox.Show("File name = " + splitFile[splitFile.Length - 1]);
                                        //Logger.Info("File name = " + splitFile[splitFile.Length - 1], OType.FullName, "Run");

                                        string copyFile = Path.GetFileName(file);
                                        string source = sourceDir + "\\" + copyFile;
                                        //MessageBox.Show("Source path for File = " + source);
                                        //Logger.Info("Source path for File = " + source, OType.FullName, "Run");

                                        string target = @targetDirectory + "experts\\files\\" + splitFile[splitFile.Length - 1].Trim();

                                        if (!string.IsNullOrEmpty(target))
                                        {
                                            Copy(source, target);
                                        }
                                        else
                                        {
                                            //Logger.Debug("Invalid target = " + target, OType.FullName, "Run");
                                            //MessageBox.Show("Invalid target = " + target);
                                        }
                                    }
                                }
                            }
                        }
                        //MessageBox.Show("Process Complete");
                        //Logger.Debug("Process Complete", OType.FullName, "Run");
                    }
                }
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.ToString());
                //Logger.Error(exception, OType.FullName,"Run");
            }
        }


        /// <summary>
        /// Method Copy copies the file from souce directory to the target directory
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void Copy(string source, string target)
        {
            try
            {
                //Logger.Debug("Initializing copy file operation", OType.FullName,"Copy");
                //MessageBox.Show("Initializing copy file operation");

                if (File.Exists(target))
                {
                    //MessageBox.Show("File already exists");
                    File.Delete(target);
                }
                File.Copy(source, target);
                //MessageBox.Show("Copied file from " + source + " to " + target);
                //Logger.Debug("Copied " + source + " to " + target, OType.FullName, "Copy");
            }

            catch (Exception exception)
            {
                //MessageBox.Show(exception.ToString());
                //Logger.Error(exception, OType.FullName, "Copy");
            }
        }
    }

}
