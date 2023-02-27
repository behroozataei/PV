using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Threading;
using UpgradeHelpers.Helpers;

namespace UpgradeHelpers.DB
{
    /// <summary>
    /// DbConnection Helper Class, used to control command timeouts defined in the connection object
    /// </summary>
    public partial class DbConnectionHelper
    {
        private static readonly WeakDictionary<DbConnection, int> _timeouts = new WeakDictionary<DbConnection, int>();

        /// <summary>
        /// Remove the timeout for a given connection.
        /// </summary>
        /// <param name="conn">connection object to clear timeout</param>
        public static void ClearTimeOut(DbConnection conn)
        {
            lock (_timeouts)
            {
                if (_timeouts.ContainsKey(conn))
                {
                    _timeouts.Remove(conn);
                }
            }

        }
        /// <summary>
        /// Set the command timeout associated to a connection
        /// </summary>
        /// <param name="conn">The connection to set the timeout to.</param>
        /// <param name="t">timeout to set</param>
        public static void SetCommandTimeOut(DbConnection conn, int t)
        {
            lock (_timeouts)
            {
                if (_timeouts.ContainsKey(conn))
                {
                    _timeouts[conn] = t;
                }
                else
                {
                    _timeouts.Add(conn, t);
                }
            }
        }

        /// <summary>
        /// Returns the command timeout associated to a connection.
        /// </summary>
        /// <param name="conn">The connection to get the timeout from.</param>
        /// <returns>The timeout associated with the parameter.</returns>
        public static int GetCommandTimeOut(DbConnection conn)
        {
            int t = -1;  //default value
            lock (_timeouts)
            {
                if (_timeouts.ContainsKey(conn))
                {
                    t = _timeouts[conn];
                }
            }
            return t;
        }

        /// <summary>
        /// Returns the current command with the timeout associated.
        /// </summary>
        /// <param name="commandRef">The connection to get the timeout from.</param>
        /// <returns>The timeout command associated.</returns>
        public static void ResetCommandTimeOut(DbCommand commandRef)
        {
            if (commandRef.CommandTimeout == 30)
            {
                int tmpTimeOut = GetCommandTimeOut(commandRef.Connection);
                if (tmpTimeOut != -1)
                {
                    commandRef.CommandTimeout = tmpTimeOut;
                }
            }
        }

        
        
}
}
