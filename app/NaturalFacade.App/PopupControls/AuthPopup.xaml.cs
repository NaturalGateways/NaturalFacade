namespace NaturalFacade.App.PopupControls;

public partial class AuthPopup : ContentView
{
	private MainPage m_mainPage = null;

	public AuthPopup(MainPage mainPage)
	{
		InitializeComponent();

        m_mainPage = mainPage;

        this.WebView.Source = Services.AuthenticationService.CognitoUrl;
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        m_mainPage.OnCancelLogin();
    }

    private void WebView_Navigating(object sender, WebNavigatingEventArgs args)
    {
        Uri uri = new Uri(args.Url);
        if (Services.AuthenticationService.IsCallbackUrl(uri))
        {
            // Get code
            if (uri.Query.StartsWith("?code=") == false)
            {
                throw new Exception("Invalid callback.");
            }
            string code = uri.Query.Substring(6);

            // Login
            Services.AuthenticationService.AuthenticateWithCognitoCode(code);

            // Clear webview
            args.Cancel = true;
            m_mainPage.OnCancelLogin();
        }
    }
}