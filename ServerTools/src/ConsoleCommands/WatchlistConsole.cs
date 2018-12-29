﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    public class CommandWatchlistConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools]-Enable, Add, Remove and View steamids on the Watchlist.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. WatchList off\n" +
                   "  2. WatchList on\n" +
                   "  3. Watchlist add <steamID> <reason>\n" +
                   "  4. watchlist remove <steamID>\n" +
                   "  5. watchlist list\n" +
                   "1. Turn off watch list\n" +
                   "2. Turn on watch list\n" +
                   "3. Adds a steamID  and name to the Watchlist\n" +
                   "4. Removes a steamID from the Ping Watchlist\n" +
                   "5. Lists all steamIDs that are on the Watchlist";
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-WatchList", "watchlist", "wl" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 1)
                {
                    SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 1 to 3, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    Watchlist.IsEnabled = false;
                    SdtdConsole.Instance.Output(string.Format("Watch list has been set to off"));
                    return;
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    Watchlist.IsEnabled = true;
                    SdtdConsole.Instance.Output(string.Format("Watch list has been set to on"));
                    return;
                }
                else if (_params[0].ToLower().Equals("add"))
                {
                    if (_params.Count != 3)
                    {
                        SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 3, found {0}.", _params.Count));
                        return;
                    }
                    if (_params[1].Length != 17)
                    {
                        SdtdConsole.Instance.Output(string.Format("Can not add SteamId: Invalid SteamId {0}", _params[1]));
                        return;
                    }
                    if (Watchlist.Dict.ContainsKey(_params[1]))
                    {
                        SdtdConsole.Instance.Output(string.Format("Can not add SteamId. {0} is already in the Watchlist.", _params[1]));
                        return;
                    }
                    if (_params.Count == 3)
                    {
                        Watchlist.Dict.Add(_params[1], _params[2]);
                        SdtdConsole.Instance.Output(string.Format("Added SteamId {0} with the reason {1} the Watchlist.", _params[1], _params[2]));
                    }
                    Watchlist.UpdateXml();
                }
                else if (_params[0].ToLower().Equals("remove"))
                {
                    if (_params.Count != 2)
                    {
                        SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 2, found {0}", _params.Count));
                        return;
                    }
                    if (!Watchlist.Dict.ContainsKey(_params[1]))
                    {
                        SdtdConsole.Instance.Output(string.Format("SteamId {0} was not found.", _params[1]));
                        return;
                    }
                    Watchlist.Dict.Remove(_params[1]);
                    SdtdConsole.Instance.Output(string.Format("Removed SteamId {0} from the Watchlist.", _params[1]));
                    Watchlist.UpdateXml();
                }
                else if (_params[0].ToLower().Equals("list"))
                {
                    if (_params.Count != 1)
                    {
                        SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 1, found {0}.", _params.Count));
                        return;
                    }
                    if (Watchlist.Dict.Count < 1)
                    {
                        SdtdConsole.Instance.Output("There are no steamIds on the Watchlist.");
                        return;
                    }
                    foreach (KeyValuePair<string, string> _key in Watchlist.Dict)
                    {
                        string _output = string.Format("{0} {1}", _key.Key, _key.Value);
                        SdtdConsole.Instance.Output(_output);
                    }
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("Invalid argument {0}.", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in WatchlistCommandConsole.Run: {0}.", e));
            }
        }
    }
}