using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;


namespace Diplom
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif


            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Picker), (handler, view) =>
    {
#if WINDOWS
        var combobox = handler.PlatformView;
        combobox.BorderBrush = null;
        combobox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
    });

            return builder.Build();
        }
    }
}
