using System.Threading;

namespace Commons.Interfaces
{
    public interface IWatcher
    {
        #region Public Methods

        void Execute(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}