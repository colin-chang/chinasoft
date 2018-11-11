using System;

namespace Com.ChinaSoft.DataAcquisition
{
    /// <summary>
    /// Connection Builder class handles connecting and disconnecting from the Historian. Use this library as
    /// an example on how to establish a connection with Historian 11
    /// </summary>
    public class ConnectionBuilder
    {
        #region --Variables--
        private ArchestrA.HistorianAccessError mLastError;
        private ArchestrA.HistorianConnectionStatus mStatus;
        private ArchestrA.HistorianAccess mHistorianAccess;
        private System.Threading.Thread connectionthread;

        /// <summary>
        /// Historian Connection Arguments. You can set this directly prior to calling the Connect function
        /// </summary>
        public ArchestrA.HistorianConnectionArgs HistorianConnection = new ArchestrA.HistorianConnectionArgs();

        /// <summary>
        /// Provides the last error from performing historian operations.
        /// </summary>
        public ArchestrA.HistorianAccessError LastError
        {
            get
            {
                return mLastError;
            }
        }

        /// <summary>
        /// Provides the status of the Historian connection
        /// </summary>
        public ArchestrA.HistorianConnectionStatus Status
        {
            get
            {
                return mStatus;
            }
        }

        /// <summary>
        /// Provides the currect connection object.
        /// </summary>
        public ArchestrA.HistorianAccess HistorianAccess => mHistorianAccess;

        /// <summary>
        /// Connection attempts provides the number of times the connectionbuilder will try to check the status of the connection.
        /// This actaully is not necessary because the Historian will automatically connect after calling the OpenConnection command.
        /// </summary>
        public int ConnectionAttempts = 10;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionBuilder()
        {
            HistorianConnection = new ArchestrA.HistorianConnectionArgs();
            mHistorianAccess = new ArchestrA.HistorianAccess();
        }

        /// <summary>
        /// Connect function will try to establish a connection with the historian using current HistorianConnection arguments
        /// </summary>
        /// <returns>Returns the connection status after having tried the connection attempts.</returns>
        public ArchestrA.HistorianConnectionStatus Connect()
        {
            mStatus = new ArchestrA.HistorianConnectionStatus();
            mLastError = null;

            //It takes several seconds before a connection is established. You can use a for loop and wait
            //or you can just continue the operation of adding tags and storing data in offline mode. Once a connection is
            //established, any tags you have added or stored will automatically be synchronized with the historian.
            if (HistorianAccess.OpenConnection(HistorianConnection, out mLastError))
            {
                for (int i = 0; i < ConnectionAttempts; i++)
                {
                    HistorianAccess.GetConnectionStatus(ref mStatus);

                    if (Status.ConnectedToServer)
                    {
                        return Status;
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }
            else
            {
                return null;
            }

            return Status;
        }

        /// <summary>
        /// Connect function for async thread. Will continue to monitor connection
        /// </summary>        
        public void ConnectAsyncThreadFunc()
        {
            mStatus = new ArchestrA.HistorianConnectionStatus();
            mLastError = null;

            //We will open connection then continue to monitor status
            if (HistorianAccess.OpenConnection(HistorianConnection, out mLastError))
            {
                while (true)
                {
                    HistorianAccess.GetConnectionStatus(ref mStatus);

                    UpdateStatus(mStatus);

                    System.Threading.Thread.Sleep(5000);
                }
            }
            else
            {
                UpdateStatus(null);
            }

        }

        /// <summary>
        /// Connect function will try to establish a connection with the historian. Minimum parameters are passed to the connection arguments.
        /// </summary>
        /// <param name="MinStoreForwardDuration">Minimum time client remains in storeforward after a disconnect</param>
        /// <param name="Password">Windows Authenticated password for the Historian</param>
        /// <param name="ReadOnly">Readonly disables ability to addtags or store. Set to false if you need to store values.</param>
        /// <param name="ServerName">Historian's server name</param>
        /// <param name="StoreForwardFreeDiskSpace">Minimum disk space before storeforward starts to delete the oldest block</param>
        /// <param name="StoreForwardPath">Path of the storeforward directory. This must be unique to each connection if the historian is the same.</param>
        /// <param name="TcpPort">WCF port to connect to. This is the same port that is seen in the parameters section of the SMC on the historian</param>
        /// <param name="UserName">Windows Authenticated username for the Historian</param>
        /// <returns>Returns the connection status after having tried the connection attempts.</returns>
        public ArchestrA.HistorianConnectionStatus Connect(uint MinStoreForwardDuration,
                                                            String Password,
                                                            Boolean ReadOnly,
                                                            String ServerName,
                                                            uint StoreForwardFreeDiskSpace,
                                                            String StoreForwardPath,
                                                            ushort TcpPort,
                                                            String UserName)
        {
            //Set connection arguments prior to connecting.
            HistorianConnection.MinStoreForwardDuration = MinStoreForwardDuration;
            HistorianConnection.Password = Password;
            HistorianConnection.ReadOnly = ReadOnly;
            HistorianConnection.ServerName = ServerName;
            HistorianConnection.StoreForwardFreeDiskSpace = StoreForwardFreeDiskSpace;
            HistorianConnection.StoreForwardPath = StoreForwardPath;
            HistorianConnection.TcpPort = TcpPort;
            HistorianConnection.UserName = UserName;

            return Connect();
        }

        /// <summary>
        /// Connect function will try to establish a connection with the historian asynchronously. Minimum parameters are passed to the connection arguments.
        /// </summary>
        /// <param name="MinStoreForwardDuration">Minimum time client remains in storeforward after a disconnect</param>
        /// <param name="Password">Windows Authenticated password for the Historian</param>
        /// <param name="ReadOnly">Readonly disables ability to addtags or store. Set to false if you need to store values.</param>
        /// <param name="ServerName">Historian's server name</param>
        /// <param name="StoreForwardFreeDiskSpace">Minimum disk space before storeforward starts to delete the oldest block</param>
        /// <param name="StoreForwardPath">Path of the storeforward directory. This must be unique to each connection if the historian is the same.</param>
        /// <param name="TcpPort">WCF port to connect to. This is the same port that is seen in the parameters section of the SMC on the historian</param>
        /// <param name="UserName">Windows Authenticated username for the Historian</param>        
        public void ConnectAsync(uint MinStoreForwardDuration,
                                                            String Password,
                                                            Boolean ReadOnly,
                                                            String ServerName,
                                                            uint StoreForwardFreeDiskSpace,
                                                            String StoreForwardPath,
                                                            ushort TcpPort,
                                                            String UserName)
        {
            //Set connection arguments prior to connecting.
            HistorianConnection.MinStoreForwardDuration = MinStoreForwardDuration;
            HistorianConnection.Password = Password;
            HistorianConnection.ReadOnly = ReadOnly;
            HistorianConnection.ServerName = ServerName;
            HistorianConnection.StoreForwardFreeDiskSpace = StoreForwardFreeDiskSpace;
            HistorianConnection.StoreForwardPath = StoreForwardPath;
            HistorianConnection.TcpPort = TcpPort;
            HistorianConnection.UserName = UserName;

            connectionthread = new System.Threading.Thread(ConnectAsyncThreadFunc) { IsBackground = true };

            connectionthread.Start();
        }

        /// <summary>
        /// UpdateStatus event updates the status of Historian connection
        /// </summary>
        public event ConnectionStatusChanged UpdateStatus;
        public delegate void ConnectionStatusChanged(ArchestrA.HistorianConnectionStatus status);

        /// <summary>
        /// Disconnect from the Historian
        /// </summary>
        public void Disconnect()
        {
            if (connectionthread != null)
                connectionthread.Abort();
            mLastError = null;
            mHistorianAccess.CloseConnection(out mLastError);
        }

    }
}
