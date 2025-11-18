using GamerTrade.Services.Interfaces;

namespace GamerTrade.Services.Implementaciones
{
    public class OrdenarPorNombreDesc : IOrdenamientoStrategy
    {
        public IEnumerable<GamerTrade.Models.DTO.JuegoDTO> Ordenar(IEnumerable<GamerTrade.Models.DTO.JuegoDTO> juegos)
            => juegos.OrderByDescending(j => j.Titulo);
    }
}
