﻿using System;
using System.Collections.Generic;
using System.Data;

namespace ServerTools
{
    class Lottery
    {
        public static bool IsEnabled = false, OpenLotto = false, ShuttingDown = false;
        public static int Bonus = 0, LottoValue = 0;
        public static List<ClientInfo> LottoEntries = new List<ClientInfo>();

        public static void Response(ClientInfo _cInfo)
        {
            if (OpenLotto)
            {
                string _phrase536;
                if (!Phrases.Dict.TryGetValue(536, out _phrase536))
                {
                    _phrase536 = " a lottery is open for {Value} {CoinName}. Minimum buy in is {BuyIn}. Enter it by typing /lottery enter.";
                }
                int _value = LottoValue * LottoEntries.Count;
                _phrase536 = _phrase536.Replace("{Value}", _value.ToString());
                _phrase536 = _phrase536.Replace("{CoinName}", Wallet.Coin_Name);
                _phrase536 = _phrase536.Replace("{BuyIn}", LottoValue.ToString());
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase536 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                string _phrase535;
                if (!Phrases.Dict.TryGetValue(535, out _phrase535))
                {
                    _phrase535 = " there is no open lottery. Type /lottery # to open a new lottery at that buy in price. You must have enough in your wallet.";
                }
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase535 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void NewLotto(ClientInfo _cInfo, string _message, string _playerName)
        {
            if (!ShuttingDown)
            {
                if (OpenLotto)
                {
                    int _winnings = LottoValue * LottoEntries.Count;
                    string _phrase536;
                    if (!Phrases.Dict.TryGetValue(536, out _phrase536))
                    {
                        _phrase536 = " a lottery is open for {Value} {CoinName}. Minimum buy in is {BuyIn}. Enter it by typing /lottery enter.";
                    }
                    _phrase536 = _phrase536.Replace("{Value}", _winnings.ToString());
                    _phrase536 = _phrase536.Replace("{CoinName}", Wallet.Coin_Name);
                    _phrase536 = _phrase536.Replace("{BuyIn}", LottoValue.ToString());
                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase536 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
                else
                {
                    int _lottoValue;
                    if (int.TryParse(_message, out _lottoValue))
                    {
                        if (_lottoValue > 0)
                        {
                            int _currentCoins = Wallet.GetcurrentCoins(_cInfo);
                            if (_currentCoins >= _lottoValue)
                            {
                                OpenLotto = true;
                                LottoValue = _lottoValue;
                                LottoEntries.Add(_cInfo);
                                Wallet.SubtractCoinsFromWallet(_cInfo.playerId, LottoValue);
                                string _phrase538;
                                if (!Phrases.Dict.TryGetValue(538, out _phrase538))
                                {
                                    _phrase538 = " you have opened a new lottery for {Value} {CoinName}.";
                                }
                                _phrase538 = _phrase538.Replace("{Value}", _lottoValue.ToString());
                                _phrase538 = _phrase538.Replace("{CoinName}", Wallet.Coin_Name);
                                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase538 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                                string _phrase539;
                                if (!Phrases.Dict.TryGetValue(539, out _phrase539))
                                {
                                    _phrase539 = "A lottery has opened for {Value} {CoinName} and will draw soon. Type /lottery enter to join.";
                                }
                                _phrase539 = _phrase539.Replace("{Value}", LottoValue.ToString());
                                _phrase539 = _phrase539.Replace("{CoinName}", Wallet.Coin_Name);
                                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase538 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Global, null);
                            }
                            else
                            {
                                string _phrase540;
                                if (!Phrases.Dict.TryGetValue(540, out _phrase540))
                                {
                                    _phrase540 = " you do not have enough {CoinName}. Earn some more and enter the lottery before it ends.";
                                }
                                _phrase540 = _phrase540.Replace("{CoinName}", Wallet.Coin_Name);
                                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase540 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                            }
                        }
                        else
                        {
                            string _phrase537;
                            if (!Phrases.Dict.TryGetValue(537, out _phrase537))
                            {
                                _phrase537 = " you must type a valid integer above zero for the lottery #.";
                            }
                            ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase537 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                    else
                    {
                        string _phrase537;
                        if (!Phrases.Dict.TryGetValue(537, out _phrase537))
                        {
                            _phrase537 = " you must type a valid integer above zero for the lottery #.";
                        }
                        ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase537 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
            }
        }

        public static void EnterLotto(ClientInfo _cInfo)
        {
            if (OpenLotto)
            {
                int _currentCoins = Wallet.GetcurrentCoins(_cInfo);
                if (_currentCoins >= LottoValue)
                {
                    if (!LottoEntries.Contains(_cInfo))
                    {
                        LottoEntries.Add(_cInfo);
                        Wallet.SubtractCoinsFromWallet(_cInfo.playerId, LottoValue);
                        string _phrase541;
                        if (!Phrases.Dict.TryGetValue(541, out _phrase541))
                        {
                            _phrase541 = " you have entered the lottery, good luck in the draw.";
                        }
                        ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase541 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        if (LottoEntries.Count == 10)
                        {
                            StartLotto();
                        }
                    }
                    else
                    {
                        string _phrase542;
                        if (!Phrases.Dict.TryGetValue(542, out _phrase542))
                        {
                            _phrase542 = " you are already in the lottery, good luck in the draw.";
                        }
                        ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase542 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
                else
                {
                    string _phrase540;
                    if (!Phrases.Dict.TryGetValue(540, out _phrase540))
                    {
                        _phrase540 = " you do not have enough {CoinName}. Earn some more and enter the lottery before it ends.";
                    }
                    _phrase540 = _phrase540.Replace("{CoinName}", Wallet.Coin_Name);
                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase540 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
            else
            {
                string _phrase535;
                if (!Phrases.Dict.TryGetValue(535, out _phrase535))
                {
                    _phrase535 = " there is no open lottery. Type /lottery # to open a new lottery at that buy in price. You must have enough in your wallet.";
                }
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase535 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void StartLotto()
        {
            Random rnd = new Random();
            int _random = rnd.Next(LottoEntries.Count + 1);
            ClientInfo _winner = LottoEntries[_random];
            int _winnings;
            if (LottoEntries.Count == 10)
            {
                _winnings = LottoValue * LottoEntries.Count + Bonus;
            }
            else
            {
                _winnings = LottoValue * LottoEntries.Count;
            }
            OpenLotto = false;
            LottoValue = 0;
            LottoEntries.Clear();
            Wallet.AddCoinsToWallet(_winner.playerId, _winnings);
            string _phrase544;
            if (!Phrases.Dict.TryGetValue(544, out _phrase544))
            {
                _phrase544 = "Winner! {PlayerName} has won the lottery and received {Value} {CoinName}.";
            }
            _phrase544 = _phrase544.Replace("{PlayerName}", _winner.playerName);
            _phrase544 = _phrase544.Replace("{Value}", _winnings.ToString());
            _phrase544 = _phrase544.Replace("{CoinName}", Wallet.Coin_Name);
            ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase544 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
        }

        public static void Alert()
        {
            string _phrase543;
            if (!Phrases.Dict.TryGetValue(543, out _phrase543))
            {
                _phrase543 = "A lottery draw will begin in five minutes. Get your entries in before it starts. Type /lottery enter.";
            }
            ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase543 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
        }
    }
}
