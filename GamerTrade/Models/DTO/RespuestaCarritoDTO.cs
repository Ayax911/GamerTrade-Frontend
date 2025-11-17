namespace GamerTrade.Models.DTO
{
    public class RespuestaCarritoDTO
    {
        public string Mensaje { get; set; } = string.Empty;
        public int Exito { get; set; } // 1 = éxito, 0 = error

        // Propiedad de ayuda para verificar si fue exitoso
        public bool FueExitoso => Exito == 1;
    }
}
