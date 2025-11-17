using GamerTrade.Models.DTO;

namespace GamerTrade.Models.DTO
{
    public class CarritoDTO
    {
        public Guid CarritoID { get; set; }
        public string CorreoUsuario { get; set; }
        public List<ItemCarritoDTO> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    
}
