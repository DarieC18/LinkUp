namespace LinkUp.Application.ViewModels.Shared
{
    public sealed class ConfirmActionVm
    {
        public string Title { get; set; } = "Confirmar acción";
        public string Message { get; set; } = "¿Deseas continuar?";
        public string PostAction { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string ReturnTo { get; set; } = "Index";
        public string Id { get; set; } = string.Empty;
    }
}
