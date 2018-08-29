using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

[assembly: AssemblyVersion("0.1.*")]

namespace Watermark
{
    class Program
    {
        private static List<PhotoInfo> photoList;
        static void Main(string[] args)
        {
            Console.Title = "DXPress Image Watermark Tool";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            var app = new Application();
            AppBuilder.Configure(app)
                .UsePlatformDetect()
                .SetupWithoutStarting();

            Console.WriteLine($"Image Watermark Tool for DXPress\n© 2018 DXPress\nVersion. {Assembly.GetExecutingAssembly().GetName().Version}\n");

            // Choose Image Files
            Console.WriteLine("Choose image files...");
            string[] filelistStrings;
            do
            {
                var chooseImageTask = Task.Run(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Choose Images",
                        AllowMultiple = true
                    };
                    openFileDialog.Filters.Add(new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png", "bmp", "gif" } });
                    var outPathStrings = openFileDialog.ShowAsync();
                    return outPathStrings;
                });
                filelistStrings = chooseImageTask.GetAwaiter().GetResult();
            } while (filelistStrings == null);
            Console.WriteLine($"{filelistStrings.Length} file(s) selected.");
            photoList = new List<PhotoInfo>();
            foreach (var str in filelistStrings)
            {
                PhotoInfo photo = new PhotoInfo
                {
                    OriginPath = str,
                    FileName = System.IO.Path.GetFileNameWithoutExtension(str)
                };
                photoList.Add(photo);
                Console.WriteLine(str);
            }

            // Choose Saving Folder
            Console.WriteLine("\nChoose saving folder...");
            string savepathString;
            do
            {
                var chooseFolderTask = Task.Run(() =>
                {
                    OpenFolderDialog openFolderDialog = new OpenFolderDialog
                    {
                        Title = "Choose Saving Folder"
                    };
                    var outPathString = openFolderDialog.ShowAsync();
                    return outPathString;
                });
                savepathString = chooseFolderTask.GetAwaiter().GetResult();
            } while (string.IsNullOrEmpty(savepathString));
            Console.WriteLine($"Save to {savepathString}");

            ChooseFormat: Console.Write("\nChoose output format: [png]/jpg/gif >");
            string strFormat = Console.ReadLine();
            Photo.PicFormat outputFormat = Photo.PicFormat.png;
            switch (strFormat.ToLower())
            {
                case "":
                case "png":
                    break;
                case "jpg":
                    outputFormat = Photo.PicFormat.jpg;
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

            Parallel.ForEach(photoList, p =>
            {
                try
                {
                    Photo photo = new Photo(p.OriginPath) {FileName = p.FileName + "[DXPress]"};
                    photo.Resize();
                    photo.Watermark();
                    photo.SaveImage(savepathString, outputFormat);
                    p.IsSuccess = true;
                    Console.WriteLine(System.IO.Path.GetFileName(p.OriginPath) + " ... Success");
                }
                catch (Exception e)
                {
                    p.IsSuccess = false;
                    p.ErrorMessage = e.Message;
                    Console.WriteLine(System.IO.Path.GetFileName(p.OriginPath) + " ... ERROR: " + e.Message);
                }
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

        class PhotoInfo
        {
            public string OriginPath;
            public string FileName;
            public bool? IsSuccess = null;
            public string ErrorMessage;
        }
    }
}
