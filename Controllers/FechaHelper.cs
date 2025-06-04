using System.Runtime.InteropServices;

namespace MiniMercado.Controllers {
    public static class FechaHelper
    {
        public static DateTime ObtenerHoraArgentina()
        {
            var argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "Argentina Standard Time"
                    : "America/Argentina/Buenos_Aires");

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, argentinaTimeZone);
        }
    }

}
