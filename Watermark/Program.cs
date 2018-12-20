using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

[assembly: AssemblyVersion("0.3.*")]

namespace Watermark
{
    class Program
    {
        private static List<PhotoInfo> photoList;
        static void Main(string[] args)
        {
            Console.Title = "DXPress Image Watermark Tool";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine($"Image Watermark Tool for DXPress\n© 2018 DXPress\nVersion. {Assembly.GetExecutingAssembly().GetName().Version}\n");
            
            // Choose Image Files
            Console.WriteLine("Choose image files...");
            string[] filelistStrings = GetFileLists();
            Console.WriteLine($"{filelistStrings.Length} file(s) selected.");
            photoList = new List<PhotoInfo>();
            foreach (var str in filelistStrings)
            {
                PhotoInfo photo = new PhotoInfo
                {
                    OriginPath = str,
                    FileName = Path.GetFileNameWithoutExtension(str)
                };
                photoList.Add(photo);
                Console.WriteLine(str);
            }

            // Choose Saving Folder
            Console.WriteLine("\nChoose saving folder...");
            string savepathString = GetSavingFolder();
            Console.WriteLine($"Save to {savepathString}");

            ChooseFormat: Console.Write("\nChoose output format: [jpg]/png/gif >");
            string strFormat = Console.ReadLine();
            Photo.PicFormat outputFormat;
            switch (strFormat.ToLower())
            {
                case "":
                case "jpg":
                    outputFormat = Photo.PicFormat.jpg;
                    break;
                case "png":
                    outputFormat = Photo.PicFormat.png;
                    break;
                case "gif":
                    outputFormat = Photo.PicFormat.gif;
                    break;
                default:
                    Console.WriteLine("Please type valid format."); 
                    goto ChooseFormat;
            }
            Console.WriteLine("Save in format " + outputFormat);
            Console.WriteLine("\n\nPress any key to start....");
            Console.ReadKey();
            Console.WriteLine("Starting...\n");

            Parallel.ForEach(photoList, p =>
            {
#if !DEBUG
                try
                {
#endif
                    Photo photo = new Photo(p.OriginPath) {FileName = p.FileName + "[DXPress]"};
                    if (photo.Width > 2000 || photo.Height > 2000) // No resize for image smaller than 2000*2000
                        photo.Resize();
                    photo.Watermark();
                    photo.AddCopyright($"Copyright, ECNU Daxia Press, {DateTime.Today.Year}. All rights reserved.");
                    photo.SaveImage(savepathString, photo.FrameCount > 1 ? Photo.PicFormat.gif:outputFormat);
                    p.IsSuccess = true;
                    Console.WriteLine(Path.GetFileName(p.OriginPath) + " ... Success");
#if !DEBUG
            }
                catch (Exception e)
                {
                    p.IsSuccess = false;
                    p.ErrorMessage = e.Message;
                    Console.WriteLine(Path.GetFileName(p.OriginPath) + " ... ERROR: " + e.Message);
            }
#endif
            });
            Summary();
        }

        private static void Summary()
        {
            if (photoList == null)
            {
                Environment.Exit(0);
            }
            Console.WriteLine($"\n\nTotal {photoList.Count} , Successed {photoList.Count(i => i.IsSuccess == true)} , Failed {photoList.Count(i => i.IsSuccess == false)}");
            if (photoList.Count(i => i.IsSuccess == false) != 0)
            {
                Console.WriteLine("Faillist:");
                foreach (var p in photoList.Where(i => i.IsSuccess == false).ToList())
                {
                    Console.WriteLine($"File: {p.OriginPath} Error Message: {p.ErrorMessage}");
                }
            }

            //if (photoList.Count(i => i.IsSuccess == null) != 0)
            //{
            //    Console.WriteLine("Passlist:");
            //    foreach (var p in photoList.Where(i => i.IsSuccess == null).ToList())
            //    {
            //        Console.WriteLine($"File: {p.OriginPath}");
            //    }
            //}
            Console.WriteLine("\nPress any key to exit....");
            Console.ReadKey();

        }

        //private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        //{
        //    Summary();
        //}

        private static string[] GetFileLists()
        {
            string fileliststr; 
            do
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    IntPtr filelisyIntPtr;
                    filelisyIntPtr = FileDialog.tinyfd_openFileDialog("Choose Images", "", 4, new string[4] { "*.jpg", "*.png", "*.bmp", "*.gif" }, "Images", 1);
                    fileliststr = StringFromChar(filelisyIntPtr);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    fileliststr = GetStdOut("openfiledialog-linux");
                }
                else
                {
                    fileliststr = GetStdOut("openfiledialog-osx");
                }
            } while (string.IsNullOrEmpty(fileliststr));
            string[] filelistStrings = fileliststr.Split('|');
            return filelistStrings;
        }

        private static string GetSavingFolder()
        {
            string savepathString;
            do
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    IntPtr savepathIntPtr = FileDialog.tinyfd_selectFolderDialog("Select Saveing Folder", "");
                    savepathString = StringFromChar(savepathIntPtr);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    savepathString = GetStdOut("selectfolderdialog-linux");
                }
                else
                {
                    savepathString = GetStdOut("selectfolderdialog-osx");
                }
                
            } while (string.IsNullOrEmpty(savepathString));

            return savepathString;
        }

        private static string GetStdOut(string fileName)
        {
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return output;
        }

        private static string StringFromChar(IntPtr ptr)
        {
            return Marshal.PtrToStringAnsi(ptr);
        }

        class PhotoInfo
        {
            public string OriginPath;
            public string FileName;
            public bool? IsSuccess = null;
            public string ErrorMessage;
        }
    }
}
