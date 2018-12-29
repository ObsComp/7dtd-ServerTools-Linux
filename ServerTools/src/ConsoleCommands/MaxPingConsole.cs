﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    public class MaxPingConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools]-Sets the Max Ping limit for the server.";
        }
        public override string GetHelp()
        {
            return "Usage: maxping <ping>";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-MaxPing", "maxping", string.Empty };
        }
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SdtdConsole.Instance.Output("Wrong number of arguments.");
                    return;
                }
                if (!int.TryParse(_params[0], out HighPingKicker.Max_Ping))
                {
                    SdtdConsole.Instance.Output("Maxping is not an integer.");
                    return;
                }
                SdtdConsole.Instance.Output(string.Format("Max ping limit set to {0}", HighPingKicker.Max_Ping));
                LoadConfig.UpdateXml();
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ReservedSlotConsole.Run: {0}.", e));
            }
        }
    }
}