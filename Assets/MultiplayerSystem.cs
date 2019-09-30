using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace cschmid.BoxPlatformer.Multiplayer
{
    public class MultiplayerSystem : MonoBehaviour
    {

        [SerializeField]
        private OnlinePlayerGenerator _onlinePlayerGenerator;
        // The player that's connected to this application
        // It's info will be the first in _playerPackages, unless the
        // application is a client, rather than host
        [SerializeField]
        private Transform _localPlayerTransform;
        [SerializeField]
        private InputField _connectInputField;
        [SerializeField]
        private Text _couldNotConnectText;
        private float _countDownConnect = 0.0f;
        private bool _waitingForCountdown = false;

        private PlayerPackage _playerPackage;

        private Thread _updateThread; 
        private Socket client;

        private string _uniquePCKey;

        private bool _isHost = false;
        private void Start()
        { 
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _uniquePCKey = (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault().Trim();
        }
        private void Update()
        {
            _playerPackage.pos = new Vector2(
                (float)Math.Round(_localPlayerTransform.position.x, 4),
                (float)Math.Round(_localPlayerTransform.position.y, 4)
            );

            _playerPackage.zRot = (float)Math.Round(_localPlayerTransform.rotation.eulerAngles.z);
            _countDownConnect -= Time.time;
        }

        public void Connect()
        {
            if (client.Connected)
            {
                client.Shutdown(SocketShutdown.Both);
                _updateThread.Abort();
            }
            ConnectToServer();
            _connectInputField.enabled = false;
            _connectInputField.enabled = true;
        }
        private void ConnectToServer()
        {

            var segs = _connectInputField.text.Split(':');
            if (segs.Length == 2)
            {
                string ip = segs[0];

                int port;
                if (int.TryParse(segs[1], out port))
                {
                    try
                    {
                        client.Connect(ip, port);

                        if (client.Connected)
                        {
                            _updateThread = new Thread(UpdateServerThread);
                            _updateThread.Start();

                            return;
                        }
                    }
                    catch(Exception e)
                    {
                        _couldNotConnectText.text = "Could not connect to server!";  
                    }
                }
            }

            _couldNotConnectText.text = "Could not connect to server!";  
        }

        private void UpdateServerThread()
        {
            while (true)
            {
                try
                {
                    client.Send(
                        Encoding.Default.GetBytes($"{_uniquePCKey},{_playerPackage.ToString()}")
                    );

                    byte[] buffer = new byte[256];
                    client.Receive(buffer);

                    var data = Encoding.Default.GetString(buffer).Split('>');
                    foreach (var line in data)
                    {
                        var properties = line.Split(',');
                        if (properties.Length == 4 && properties[0].Trim() != _uniquePCKey.Trim())
                        {
                            float px, py, rz;
                            if (float.TryParse(properties[1].Trim(), out px) && 
                                float.TryParse(properties[2].Trim(), out py) &&
                                float.TryParse(properties[3].Trim(), out rz))
                            {
                                _onlinePlayerGenerator.UpdatePlayerPosition (
                                    properties[0],
                                    new PlayerPackage { pos = new Vector2(px, py), zRot = rz }
                                );
                            }
                        }
                    }
                }
                catch (SocketException se)
                {
                    Debug.Log(se.Message + se.ErrorCode + se.StackTrace);
                }
            }
        }

        private void OnApplicationQuit()
        {
            _updateThread.Abort();
            client.Shutdown(SocketShutdown.Both);
        }
    }
}
