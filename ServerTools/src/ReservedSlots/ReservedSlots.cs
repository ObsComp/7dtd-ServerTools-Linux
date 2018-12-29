﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ServerTools
{
    public class ReservedSlots
    {
        public static bool IsEnabled = false, IsRunning = false, Donator_Name_Coloring = false,
            Reduced_Delay = false;
        public static int Session_Time = 30, Admin_Level = 0, Admin_Slots = 0;
        public static Dictionary<string, DateTime> Dict = new Dictionary<string, DateTime>();
        public static Dictionary<string, string> Dict1 = new Dictionary<string, string>();
        public static Dictionary<string, DateTime> Kicked = new Dictionary<string, DateTime>();
        private static string file = "ReservedSlots.xml";
        private static string filePath = string.Format("{0}/{1}", API.ConfigPath, file);
        private static FileSystemWatcher fileWatcher = new FileSystemWatcher(API.ConfigPath, file);

        public static void Load()
        {
            if (IsEnabled && !IsRunning)
            {
                LoadXml();
                InitFileWatcher();
            }
        }

        public static void Unload()
        {
            Dict.Clear();
            fileWatcher.Dispose();
            IsRunning = false;
        }

        private static void LoadXml()
        {
            bool _update = false;
            if (!Utils.FileExists(filePath))
            {
                UpdateXml();
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);
            }
            catch (XmlException e)
            {
                Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", file, e.Message));
                return;
            }
            XmlNode _XmlNode = xmlDoc.DocumentElement;
            foreach (XmlNode childNode in _XmlNode.ChildNodes)
            {
                if (childNode.Name == "Players")
                {
                    Dict.Clear();
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'Players' section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("SteamId"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Player entry because of missing 'SteamId' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("Name"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Player entry because of missing 'Name' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("Expires"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Player entry because of missing 'Expires' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        DateTime _dt;
                        if (_line.GetAttribute("Expires") == "")
                        {
                            _dt = DateTime.Parse("10/29/2050 7:30:00 AM");
                            _update = true;
                        }
                        else
                        {
                            if (!DateTime.TryParse(_line.GetAttribute("Expires"), out _dt))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring Player entry because of invalid (date) value for 'Expires' attribute: {0}", subChild.OuterXml));
                                continue;
                            }
                        }
                        if (!Dict.ContainsKey(_line.GetAttribute("SteamId")))
                        {
                            Dict.Add(_line.GetAttribute("SteamId"), _dt);
                        }
                        if (!Dict1.ContainsKey(_line.GetAttribute("SteamId")))
                        {
                            Dict1.Add(_line.GetAttribute("SteamId"), _line.GetAttribute("Name"));
                        }
                    }
                }
            }
            if (_update)
            {
                UpdateXml();
            }
        }

        public static void UpdateXml()
        {
            fileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<ReservedSlots>");
                sw.WriteLine("    <Players>");
                if (Dict.Count > 0)
                {
                    foreach (KeyValuePair<string, DateTime> kvp in Dict)
                    {
                        string _name = "";
                        Dict1.TryGetValue(kvp.Key, out _name);
                        sw.WriteLine(string.Format("        <Player SteamId=\"{0}\" Name=\"{1}\" Expires=\"{2}\" />", kvp.Key, _name, kvp.Value.ToString()));
                    }
                }
                else
                {
                    sw.WriteLine(string.Format("        <Player SteamId=\"76561191234567891\" Name=\"foobar.\" Expires=\"10/29/2050 7:30:00 AM\" />"));
                }
                sw.WriteLine("    </Players>");
                sw.WriteLine("</ReservedSlots>");
                sw.Flush();
                sw.Close();
            }
            fileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.EnableRaisingEvents = true;
            IsRunning = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!Utils.FileExists(filePath))
            {
                UpdateXml();
            }
            LoadXml();
        }

        public static void CheckReservedSlot(ClientInfo _cInfo)
        {
            int _playerCount = ConnectionManager.Instance.ClientCount();
            if (_playerCount >= API.MaxPlayers - Admin_Slots)
            {
                AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                if (Admin.PermissionLevel > Admin_Level)
                {
                    if (Dict.ContainsKey(_cInfo.playerId))
                    {
                        DateTime _dt;
                        Dict.TryGetValue(_cInfo.playerId, out _dt);
                        if (DateTime.Now < _dt)
                        {
                            OpenSlot();
                        }
                        else
                        {
                            string _phrase20;
                            if (!Phrases.Dict.TryGetValue(22, out _phrase20))
                            {
                                _phrase20 = "Sorry {PlayerName} server is at max capacity and this slot is reserved.";
                            }
                            _phrase20 = _phrase20.Replace("{PlayerName}", _cInfo.playerName);
                            SdtdConsole.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.playerId, _phrase20), (ClientInfo)null);
                        }
                    }
                    else
                    {
                        string _phrase20;
                        if (!Phrases.Dict.TryGetValue(22, out _phrase20))
                        {
                            _phrase20 = "Sorry {PlayerName} server is at max capacity and this slot is reserved.";
                        }
                        _phrase20 = _phrase20.Replace("{PlayerName}", _cInfo.playerName);
                        SdtdConsole.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.playerId, _phrase20), (ClientInfo)null);
                    }
                }
            }
        }

        public static void OpenSlot()
        {
            List<string> _sessionList = new List<string>(Players.Session.Keys);
            for (int i = 0; i < _sessionList.Count; i++)
            {
                string _player = _sessionList[i];
                ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForPlayerId(_player);
                if (_cInfo != null)
                {
                    AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                    if (Admin.PermissionLevel > Admin_Level)
                    {
                        if (!Dict.ContainsKey(_cInfo.playerId))
                        {
                            DateTime _dateTime;
                            Players.Session.TryGetValue(_cInfo.playerId, out _dateTime);
                            TimeSpan varTime = DateTime.Now - _dateTime;
                            double fractionalMinutes = varTime.TotalMinutes;
                            int _timepassed = (int)fractionalMinutes;
                            if (_timepassed >= Session_Time)
                            {
                                Kicked.Add(_cInfo.playerId, DateTime.Now);
                                string _phrase20;
                                if (!Phrases.Dict.TryGetValue(22, out _phrase20))
                                {
                                    _phrase20 = "Sorry {PlayerName} server is at max capacity and this slot is reserved.";
                                }
                                _phrase20 = _phrase20.Replace("{PlayerName}", _cInfo.playerName);
                                SdtdConsole.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.playerId, _phrase20), (ClientInfo)null);
                                return;
                            }
                        }
                        else
                        {
                            DateTime _dt;
                            Dict.TryGetValue(_cInfo.playerId, out _dt);
                            if (DateTime.Now > _dt)
                            {
                                DateTime _dateTime;
                                Players.Session.TryGetValue(_cInfo.playerId, out _dateTime);
                                TimeSpan varTime = DateTime.Now - _dateTime;
                                double fractionalMinutes = varTime.TotalMinutes;
                                int _timepassed = (int)fractionalMinutes;
                                if (_timepassed >= Session_Time)
                                {
                                    Kicked.Add(_cInfo.playerId, DateTime.Now);
                                    string _phrase21;
                                    if (!Phrases.Dict.TryGetValue(21, out _phrase21))
                                    {
                                        _phrase21 = "Sorry {PlayerName} server is at max capacity and your reserved status has expired.";
                                    }
                                    _phrase21 = _phrase21.Replace("{PlayerName}", _cInfo.playerName);
                                    SdtdConsole.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.playerId, _phrase21), (ClientInfo)null);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}