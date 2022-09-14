using Dapper;
using MasCom.Lib;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharper.Server;
using Log = Serilog.Log;

namespace MasCom.Serverv2.WebsocketServices
{
    public static class WSServicesExtensions
    {
        private static string ConnectionString => new ConfigurationBuilder()
                                                        .AddJsonFile("appsettings.json")
                                                        .Build().GetConnectionString("MysqlDb");

        public static Trivial.CommandLine.StyleConsole cmdy = new Trivial.CommandLine.StyleConsole();

        public static bool IsConnectionSecured(this WebSocketBehavior wssb)
        {
            if (wssb.Context != null)
                if (wssb.Context.IsSecureConnection && wssb.Context.WebSocket.IsSecure)
                    return true;
            return false;
        }

        /// <summary>
        /// Get current user from http context
        /// </summary>
        /// <param name="wssb"></param>
        /// <returns>Current user as [User] or null </returns>
        public static async Task<User> GetCurrentUser(this WebSocketBehavior wssb)
        {
            if (wssb.Context != null)
            {
                if (wssb.Context.IsAuthenticated || wssb.Context.User.Identity.IsAuthenticated)
                {
                    using (IDbConnection db = new MySqlConnection(ConnectionString))
                    {
                        db.Open();
                        var currentUser = (await db.GetListAsync<User>(new { })).Where(u => string.Compare(u.UserName, wssb.Context.User.Identity.Name, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                        db.Close();

                        return currentUser;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Update current user mapping to websocket session id
        /// </summary>
        /// <param name="wssb">---</param>
        /// <returns>true if success, otherwise false</returns>
        public static async Task<bool> UpdateUserMapping(this WebSocketBehavior wssb, UserToSessionsMapping sessionsMapping)
        {
            int affectedRowsCount = 0;
            using (IDbConnection db = new MySqlConnection(ConnectionString))
            {
                db.Open();

                affectedRowsCount = await db.UpdateAsync(sessionsMapping);

                db.Close();
            }

            return affectedRowsCount == 1;
        }

        /// <summary>
        /// Register Update current user mapping to websocket session id
        /// </summary>
        /// <param name="wssb">---</param>
        /// <returns>true if success, otherwise false</returns>
        public static async Task<int?> RegisterUserMapping(this WebSocketBehavior wssb)
        {
            int? id = 0;
            var user = await GetCurrentUser(wssb);
            if (user == null)
                return id;

            using (IDbConnection db = new MySqlConnection(ConnectionString))
            {
                db.Open();

                //-- Get current user mapping
                var mapping = (await db.GetListAsync<UserToSessionsMapping>(new { UserId = user.Id })).FirstOrDefault();

                //User does not have a mapping
                if (mapping == null)
                {
                    Log.Logger.Warning("user [{0}] session Mapping does not exists, defining one...", user.UserName);
                    //define one
                    id = await db.InsertAsync(new UserToSessionsMapping()
                    {
                        UserId = user.Id
                    });
                }

                db.Close();
            }

            return id;
        }

        /// <summary>
        /// Returns current user session mappings
        /// </summary>
        /// <param name="wssb"></param>
        /// <returns></returns>
        public static async Task<UserToSessionsMapping> GetCurrentUserSessionMappings(this WebSocketBehavior wssb)
        {
            var currentUser = await GetCurrentUser(wssb);

            if (currentUser == null)
                return null;

            UserToSessionsMapping mapping = null;
            using (IDbConnection db = new MySqlConnection(ConnectionString))
            {
                db.Open();
                mapping = (await db.GetListAsync<UserToSessionsMapping>(new { UserId = currentUser.Id })).FirstOrDefault();
                db.Close();
            }

            return mapping;
        }

        /// <summary>
        /// Return the session mapping object for the specified user's userid
        /// </summary>
        /// <param name="_"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<UserToSessionsMapping> GetSpecifiedUserSessionMappings(this WebSocketBehavior _, int userId)
        {
            Lib.UserToSessionsMapping mapping = null;
            using (IDbConnection db = new MySqlConnection(ConnectionString))
            {
                db.Open();
                mapping = (await db.GetListAsync<Lib.UserToSessionsMapping>(new { UserId = userId })).FirstOrDefault();
                db.Close();
            }

            return mapping;
        }

        public static async Task<int?> SaveFileMessageHeadersToDatabase(this WebSocketBehavior _, FileMessageHeaders fileMessageHead)
        {
            int? id;
            using (IDbConnection db = new MySqlConnection(ConnectionString))
            {
                db.Open();
                id = await db.InsertAsync(fileMessageHead);
                db.Close();
            }
            return id;
        }

        public static async Task<FileMessageHeaders> GetAssociatedDeliveryHeaders(this WebSocketBehavior _, string deliveryId)
        {
            FileMessageHeaders fmH = null;
            using (IDbConnection db = new MySqlConnection(ConnectionString))
            {
                db.Open();
                fmH = (await db.GetListAsync<FileMessageHeaders>())
                            .Where(fm => fm.DeliveryId == deliveryId)
                                    .FirstOrDefault();
                db.Close();
            }
            return fmH;
        }
    }
}
