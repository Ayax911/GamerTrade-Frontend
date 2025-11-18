namespace GamerTrade.Services.Interfaces
{
    public interface IOrdenamientoStrategy
    {
        IEnumerable<GamerTrade.Models.DTO.JuegoDTO> Ordenar(IEnumerable<GamerTrade.Models.DTO.JuegoDTO> juegos);
    }
}
