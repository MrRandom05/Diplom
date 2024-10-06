
namespace Diplom
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var win = base.CreateWindow(activationState);
            win.Width=1000;
            return win;
        }
    }
}
