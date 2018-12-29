﻿using System.Collections.Generic;
using System.Linq;

namespace ServerTools
{
    class AdminList
    {
        public static bool IsEnabled = false;
        public static int Admin_Level = 0, Mod_Level = 1;
        private static List<string> Admins = new List<string>();
        private static List<string> Mods = new List<string>();

        public static void List(ClientInfo _cInfo, bool _announce, string _playerName)
        {
            Admins.Clear();
            Mods.Clear();
            List<ClientInfo> _cInfoList = ConnectionManager.Instance.Clients.List.ToList();
            for (int i = 0; i < _cInfoList.Count; i++)
            {
                ClientInfo _cInfoAdmins = _cInfoList[i];
                GameManager.Instance.adminTools.IsAdmin(_cInfoAdmins.playerId);
                AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfoAdmins.playerId);
                if (Admin.PermissionLevel <= Admin_Level)
                {
                    Admins.Add(_cInfoAdmins.playerName);
                }
                if (Admin.PermissionLevel > Admin_Level & Admin.PermissionLevel <= Mod_Level)
                {
                    Mods.Add(_cInfoAdmins.playerName);
                }
            }
            Response(_cInfo, _announce, _playerName);
        }

        public static void Response(ClientInfo _cInfo, bool _announce, string _playerName)
        {
            string _adminList = string.Join(", ", Admins.ToArray());
            string _modList = string.Join(", ", Mods.ToArray());
            if (_announce)
            {
                string _phrase725;
                if (!Phrases.Dict.TryGetValue(725, out _phrase725))
                {
                    _phrase725 = "Server admins in game: [FF8000]";
                }
                string _phrase726;
                if (!Phrases.Dict.TryGetValue(726, out _phrase726))
                {
                    _phrase726 = "Server moderators in game: [FF8000]";
                }
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase725 + _adminList + ".[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Global, null);
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase726 + _modList + ".[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Global, null);
            }
            else
            {
                string _phrase725;
                if (!Phrases.Dict.TryGetValue(725, out _phrase725))
                {
                    _phrase725 = "Server admins in game: [FF8000]";
                }
                string _phrase726;
                if (!Phrases.Dict.TryGetValue(726, out _phrase726))
                {
                    _phrase726 = "Server moderators in game: [FF8000]";
                }
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase725 + _adminList + ".[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase726 + _modList + ".[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }
    }
}
