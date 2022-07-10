using Commons.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FilesWatcher
{
    internal class Worker
        : BackgroundService
    {
        #region Private Fields

        private readonly IExecutor executorService;
        private readonly IWatcher watcherService;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IWatcher watcherService, IExecutor executorService)
        {
            this.watcherService = watcherService;
            this.executorService = executorService;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            watcherService.Execute(cancellationToken);

            await executorService.ExecuteAsync(cancellationToken);
        }

        #endregion Protected Methods
    }
}