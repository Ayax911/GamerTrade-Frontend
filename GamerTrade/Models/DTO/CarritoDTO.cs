using GamerTrade.Models.DTO;

namespace GamerTrade.Models.DTO
{
    public class CarritoDTO
    {
        public Guid UniqueID { get; set; }
        public string Correo_usuario { get; set; } = string.Empty;
        public List<ItemCarritoDTO> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Precio * i.Cantidad);
        
    }


}
