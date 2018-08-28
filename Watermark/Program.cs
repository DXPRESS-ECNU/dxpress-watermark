using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;

namespace Watermark
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Application();
            AppBuilder.Configure(app)
                .UsePlatformDetect()
                .SetupWithoutStarting();
            string[] filelistStrings;
            do
            {
                var task = Task.Run(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Choose Images",
                        AllowMultiple = true
                    };
                    openFileDialog.Filters.Add(new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png", "tif" } });
                    var outPathStrings = openFileDialog.ShowAsync();
                    return outPathStrings;
                });
                filelistStrings = task.GetAwaiter().GetResult();
            } while (filelistStrings == null);

            foreach (var str in filelistStrings)
            {
                Console.WriteLine(str);
            }


        }

    }
}
