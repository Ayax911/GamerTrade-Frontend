namespace GamerTrade.Models.DTO
{
    public class ResumenCarritoDTO
    {
        public Guid CarritoID { get; set; }
        public decimal Total { get; set; }
        public int CantidadItems { get; set; }
        public string Correo_usuario { get; set; } = string.Empty;
        public List<ItemCarritoDTO> Items { get; set; } = new List<ItemCarritoDTO>();
    }
}
