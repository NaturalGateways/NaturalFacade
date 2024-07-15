namespace NaturalFacade.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // TODO: Make better/data-driven
            Services.AuthenticationService = new ServiceImp.AppAuthenticationService();

            this.MainPage = new MainPage();
        }
    }
}
