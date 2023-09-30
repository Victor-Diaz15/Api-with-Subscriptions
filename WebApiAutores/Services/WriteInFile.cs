namespace WebApiAutores.Services
{
    public class WriteInFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string NombreArchivo = "Archivo 1.txt";
        private Timer timer;

        public WriteInFile(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            WriteFile("Proceso Iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            WriteFile("Proceso Finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            WriteFile("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void WriteFile(string message)
        {
            var route = $@"{env.ContentRootPath}\wwwroot\{NombreArchivo}";
            using (StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(message);
            }
        }

    }
}
