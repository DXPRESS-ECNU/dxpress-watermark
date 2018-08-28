using Avalonia;
using Avalonia.Markup.Xaml;

namespace Watermark
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
