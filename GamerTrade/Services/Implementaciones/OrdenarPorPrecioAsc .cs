using GamerTrade.Services.Interfaces;

namespace GamerTrade.Services.Implementaciones
{
    public class OrdenarPorPrecioAsc : IOrdenamientoStrategy
    {
        public IEnumerable<GamerTrade.Models.DTO.JuegoDTO> Ordenar(IEnumerable<GamerTrade.Models.DTO.JuegoDTO> juegos)
            => juegos.OrderBy(j => j.Precio);
    }
}
