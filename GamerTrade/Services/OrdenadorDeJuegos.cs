using GamerTrade.Services.Interfaces;

namespace GamerTrade.Services
{
    public class OrdenadorDeJuegos
    {
        public IOrdenamientoStrategy? Strategy { get; set; }

        public IEnumerable<GamerTrade.Models.DTO.JuegoDTO> Ejecutar(IEnumerable<GamerTrade.Models.DTO.JuegoDTO> juegos)
        {
            if (Strategy == null) return juegos;
            return Strategy.Ordenar(juegos);
        }
    }
}
