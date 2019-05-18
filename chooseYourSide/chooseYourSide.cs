//#define DEBUG
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("chooseYourSide", "Ohm", "1.0.0")]
    [Description("whatever the shit")]
    public class chooseYourSide : RustPlugin
    {

        DynamicConfigFile spawns = Interface.Oxide.DataFileSystem.GetDatafile("chooseYourSide_Spawns");

        private void Init()
        {
            permission.RegisterPermission("chooseYourSide.corn", this);
            permission.RegisterPermission("chooseYourSide.pumpkin", this);

        }

        void OnPluginLoaded(Plugin name)
        {

            if (!permission.GroupExists("corn"))
            {
                Puts("corn permission group doesn't exist, creating!");
                if (permission.CreateGroup("corn", "corn", 0))
                {
                    Puts("Success!");
                    permission.GrantGroupPermission("corn", "chooseYourSide.corn", this);
                }
                else Puts("couldn't create corn permission group :(");
            }

            if (!permission.GroupExists("pumpkin"))
            {
                Puts("pumpkin permission group doesn't exist, creating!");
                if (permission.CreateGroup("pumpkin", "pumpkin", 0))
                {
                    Puts("Success!");
                    permission.GrantGroupPermission("pumpkin", "chooseYourSide.pumpkin", this);
                }
                else Puts("couldn't create pumpkin permission group :(");
            }
        }

        #region Oxide Hooks

        object OnPlayerSpawn(BasePlayer player)
        {
            return OnPlayerRespawn(player);
        }
        object OnPlayerRespawn(BasePlayer player)
        {
            if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.corn") && permission.UserHasPermission(player.UserIDString, "chooseYourSide.pumpkin"))
            {
                permission.RevokeUserPermission(player.UserIDString, "chooseYourSide.corn");
                permission.RevokeUserPermission(player.UserIDString, "chooseYourSide.pumpkin");
                return getDefaultSpawn();
            }
                if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.corn"))
            {
                return getCornSpawn();
            }

            else if(permission.UserHasPermission(player.UserIDString, "chooseYourSide.pumpkin"))
            {
                return getPumpkinSpawn();
            }

            else
            {
                return getDefaultSpawn();
            }
        }

        void OnPlayerSleepEnded(BasePlayer player)
        {
            if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.corn")) return;
            else if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.pumpkin")) return;
            else
            {
                PrintToChat(player, "You haven't chosen your side yet! type either /corn or /pumpkin in chat to chose!");
            }
        }

        #endregion

        #region commands

        [ChatCommand("corn")]
        private void choseCorn(BasePlayer player, string command, string[] args)
        {
            if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.corn"))
            {
                PrintToChat(player, "You are already a member of Corn!");
                return;
            }
            if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.pumpkin"))
            {
                PrintToChat(player, "You are already a member of Pumpkin!");
                return;
            }
            PrintToChat(player, "You have chosen to be a member of Corn!");
            permission.AddUserGroup(player.UserIDString, "corn");
            //permission.GrantUserPermission(player.UserIDString, "chooseYourSide.corn", this);
            DoTeleport(player, getCornSpawn().pos);
        }

        [ChatCommand("pumpkin")]
        private void chosePumpkin(BasePlayer player, string command, string[] args)
        {
            if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.corn"))
            {
                PrintToChat(player, "You are already a member of Corn!");
                return;
            }
            if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.pumpkin"))
            {
                PrintToChat(player, "You are already a member of Pumpkin!");
                return;
            }
            PrintToChat(player, "You have chosen to be a member of Pumpkin!");
            permission.AddUserGroup(player.UserIDString, "pumpkin");
            //permission.GrantUserPermission(player.UserIDString, "chooseYourSide.pumpkin", this);
            DoTeleport(player, getPumpkinSpawn().pos);
        }

        [ChatCommand("leave")]
        private void leavecmd(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin)
            {
                if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.corn"))
                {
                    PrintToChat(player, "leaving corn");
                    permission.RemoveUserGroup(player.UserIDString, "corn");
                    return;
                }
                else if (permission.UserHasPermission(player.UserIDString, "chooseYourSide.pumpkin"))
                {
                    PrintToChat(player, "leaving pumpkin");
                    permission.RemoveUserGroup(player.UserIDString, "pumpkin");
                    return;
                }
                else
                {
                    PrintToChat(player, "you are not in a group!");
                    return;
                }
            }
        }

        [ChatCommand("setCorn")]
        private void setCornSpawn(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin)
            {
                spawns["cornX"] = player.transform.position.x;
                spawns["cornY"] = player.transform.position.y;
                spawns["cornZ"] = player.transform.position.z;
                spawns.Save();
                PrintToChat(player, "Corn Spawnpoint saved!");
            }
        }

        [ChatCommand("setPumpkin")]
        private void setPumpkinSpawn(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin)
            {
                spawns["pumpX"] = player.transform.position.x;
                spawns["pumpY"] = player.transform.position.y;
                spawns["pumpZ"] = player.transform.position.z;
                spawns.Save();
                PrintToChat(player, "Pumpkin Spawnpoint saved!");
            }
        }

        [ChatCommand("setDefault")]
        private void setDefaultSpawn(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin)
            {
                spawns["defX"] = player.transform.position.x;
                spawns["defY"] = player.transform.position.y;
                spawns["defZ"] = player.transform.position.z;
                spawns.Save();
                PrintToChat(player, "Default Spawnpoint saved!");
            }
        }

        #endregion

        #region Helpers

        BasePlayer.SpawnPoint getCornSpawn()
        {
            if (spawns["cornX"] == null) return null;
            return new BasePlayer.SpawnPoint() { pos = new Vector3(float.Parse(spawns["cornX"].ToString()), float.Parse(spawns["cornY"].ToString()), float.Parse(spawns["cornZ"].ToString())), rot = new Quaternion(0, 0, 0, 1) };
        }

        BasePlayer.SpawnPoint getPumpkinSpawn()
        {
            if (spawns["pumpX"] == null) return null;
            return new BasePlayer.SpawnPoint() { pos = new Vector3(float.Parse(spawns["pumpX"].ToString()), float.Parse(spawns["pumpY"].ToString()), float.Parse(spawns["pumpZ"].ToString())), rot = new Quaternion(0, 0, 0, 1) };
        }

        BasePlayer.SpawnPoint getDefaultSpawn()
        {
            if (spawns["defX"] == null) return null;
            return new BasePlayer.SpawnPoint() { pos = new Vector3(float.Parse(spawns["defX"].ToString()), float.Parse(spawns["defY"].ToString()), float.Parse(spawns["defZ"].ToString())), rot = new Quaternion(0, 0, 0, 1) };
        }

        private void DoTeleport(BasePlayer player, Vector3 location)
            //by Tori
        {
            if (player.IsConnected)
            {
                player.ClientRPCPlayer(null, player, "StartLoading");

                player.StartSleeping();
                player.Teleport(location);
                player.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
                player.UpdateNetworkGroup();
                player.SendNetworkUpdateImmediate(false);
                player.SendFullSnapshot();
            }
            else
            {
                try { player.ClearEntityQueue(null); }
                catch { }

                player.Teleport(location);
            }
        }

        #endregion

    }
}