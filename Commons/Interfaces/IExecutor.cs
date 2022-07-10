using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IExecutor
    {
        #region Public Methods

        void Add(string path);

        Task ExecuteAsync(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}