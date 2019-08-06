using BetterBindableBaseSample.Common;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.Connectivity;

namespace BetterBindableBaseSample
{
    public sealed class ViewModel : BetterBindableBase
    {
        #region --- fields ---

        private Random _random = new Random((int)DateTime.Now.Ticks);
        private long _totalSampleCount;
        private long _totalWithinCircleCount;

        #endregion

        #region --- constructor ---

        public ViewModel()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            UpdateIsInternetConnected();
        }

        #endregion

        #region --- network information ---

        private string _connectionName;
        public string ConnectionName
        {
            get { return _connectionName; }
            set { SetProperty(ref _connectionName, value); }
        }

        private string _connectionStatus;
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set { SetProperty(ref _connectionStatus, value); }
        }

        private ConnectionCost _connectionCost;
        public ConnectionCost ConnectionCost
        {
            get { return _connectionCost; }
            set { SetProperty(ref _connectionCost, value); }
        }

        private DataUsage _dataUsage;
        public DataUsage DataUsage
        {
            get { return _dataUsage; }
            set { SetProperty(ref _dataUsage, value); }
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            UpdateIsInternetConnected();
        }

        private void UpdateIsInternetConnected()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile != null)
            {
                ConnectionName = connectionProfile.ProfileName;
                ConnectionCost = connectionProfile.GetConnectionCost();
                DataUsage = connectionProfile.GetLocalUsage(DateTimeOffset.Now - TimeSpan.FromHours(24), DateTimeOffset.Now);

                switch (connectionProfile.GetNetworkConnectivityLevel())
                {
                    case NetworkConnectivityLevel.None:
                        ConnectionStatus = "None";
                        break;
                    case NetworkConnectivityLevel.LocalAccess:
                        ConnectionStatus = "Local access";
                        break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess:
                        ConnectionStatus = "Constrained internet access";
                        break;
                    case NetworkConnectivityLevel.InternetAccess:
                        ConnectionStatus = "Internet access";
                        break;
                }
            }
            else
            {
                ConnectionName = "None";
                ConnectionStatus = "None";
                ConnectionCost = null;
                DataUsage = null;
            }
        }

        #endregion

        #region --- calculation ---

        public ICommand ImproveEstimateCommand
        {
            get
            {
                return new DelegateCommand(delegate
                    {
                        // Do work on a thread-pool thread,
                        // so it doesn't block the UI.
                        Task.Run(new Action(DoWork));
                    });
            }
        }

        private bool _isWorking;
        public bool IsWorking
        {
            get { return _isWorking; }
            set { SetProperty(ref _isWorking, value); }
        }

        private double _estimateOfPi;
        public double EstimateOfPi
        {
            get { return _estimateOfPi; }
            set { SetProperty(ref _estimateOfPi, value); }
        }

        private void DoWork()
        {
            IsWorking = true;

            // Iterate as many times as we can in 3 seconds.
            const double workIntervalInSeconds = 3;
            long withinCircleCount = 0;
            long sampleCount = 0;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < workIntervalInSeconds)
            {
                // Count the number of random points in the unit square
                // that fall within the unit circle.
                double x = _random.NextDouble();
                double y = _random.NextDouble();
                double radiusSquared = x * x + y * y;
                bool isWithinCircle = radiusSquared <= 1;
                if (isWithinCircle)
                {
                    withinCircleCount++;
                }
                sampleCount++;
            }

            // Add our counts to the totals.
            _totalSampleCount += sampleCount;
            _totalWithinCircleCount += withinCircleCount;

            // Update our estimate of pi.
            EstimateOfPi = 4 * (double)_totalWithinCircleCount / (double)_totalSampleCount;

            IsWorking = false;
        }

        #endregion
    }
}
