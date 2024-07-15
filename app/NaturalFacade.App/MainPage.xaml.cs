namespace NaturalFacade.App
{
    public partial class MainPage : ContentPage, UI.IModelListener
    {
        #region Base

        public MainPage()
        {
            InitializeComponent();

            Services.AuthenticationService.AuthModel.AddListener(this);
        }

        private const int POPUP_FADE_ANIM_MILLIS = 300;
        private const int POPUP_MOVE_ANIM_MILLIS = 600;

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            this.PopupContent.Content = new PopupControls.AuthPopup(this);
            this.PopupContent.TranslationY = this.Height;
            this.PopupBacking.Opacity = 0.0;
            this.PopupContainer.IsVisible = true;

            _ = this.PopupBacking.FadeTo(1.0, POPUP_FADE_ANIM_MILLIS, Easing.Linear);
            _ = this.PopupContent.TranslateTo(0.0, 0.0, POPUP_MOVE_ANIM_MILLIS, Easing.CubicOut);
        }

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            Services.AuthenticationService.Logout();
        }

        public void OnCancelLogin()
        {
            _ = OnCancelLoginAsync();
        }

        private async Task OnCancelLoginAsync()
        {
            _ = this.PopupBacking.FadeTo(0.0, POPUP_FADE_ANIM_MILLIS, Easing.Linear);
            await this.PopupContent.TranslateTo(0.0, this.Height, POPUP_MOVE_ANIM_MILLIS, Easing.CubicOut);

            this.PopupContainer.IsVisible = false;
            this.PopupContent.Content = null;
        }

        private void RefreshOnAuthChangeOnMainThread()
        {
            AuthState authState = Services.AuthenticationService.AuthModel.AuthState;
            bool isLoggedIn = authState?.ApiAccess != null;
            this.LoginButton.IsVisible = isLoggedIn == false;
            this.LogoutButton.IsVisible = isLoggedIn;
            this.AnonContent.IsVisible = isLoggedIn == false;
        }

        #endregion

        #region UI.IModelListener implementation

        /// <summary>Called when model data changes.</summary>
        public void OnDataChanged(UI.Model model, string context)
        {
            MainThread.BeginInvokeOnMainThread(RefreshOnAuthChangeOnMainThread);
        }

        #endregion
    }
}
