namespace GamerTrade.State
{
    public class AppState
    {
        public bool IsAuthenticated { get; private set; } = false;
        public int? UsuarioId { get; private set; }
        public string? Token { get; private set; }
        public event Action? OnChange;

        public void SetAuthenticated(int usuarioId, string token)
        {
            UsuarioId = usuarioId;
            Token = token;
            IsAuthenticated = true;
            NotifyStateChanged();
        }

        public void Logout()
        {
            UsuarioId = null;
            Token = null;
            IsAuthenticated = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
