using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Services
{
    public class FacturasHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public FacturasHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ProcesarFacturas, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private void ProcesarFacturas(Object state)
        {
            //Instanciando el db context
            using (var escope = _serviceProvider.CreateScope())
            {
                var context = escope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SetearMalaPaga(context);
                EmitirFacturas(context);
            }
        }

        private static void SetearMalaPaga(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlRaw("exec SP_Setear_Mala_Paga");
        }

        private static void EmitirFacturas(ApplicationDbContext context)
        {
            var hoy = DateTime.Today;
            var fechaComparacion = hoy.AddMonths(-1);
            var facturasDelMesEmitidas = context.FacturasEmitidas
                .Any(x => x.Año == fechaComparacion.Year && x.Mes == fechaComparacion.Month);

            if (!facturasDelMesEmitidas)
            {
                var fechaInicio = new DateTime(fechaComparacion.Year, fechaComparacion.Month, 1);
                var fechaFin = fechaInicio.AddMonths(1);

                //ejecutando el store procedure de la base de datos
                context.Database
                    .ExecuteSqlInterpolated($"exec SP_Creacion_Facturas {fechaInicio.ToString("yyyy-MM-dd")}, {fechaFin.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
