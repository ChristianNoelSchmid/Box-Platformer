using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cschmid.BoxPlatformer.Multiplayer
{
    public class OnlinePlayerData
    {
        public Transform playerTransform;
        public PlayerPackage playerPackage;
        public float timeoutTimer;
    }

    public class OnlinePlayerGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _templatePlayerObject;

        private Dictionary<string, OnlinePlayerData> _onlinePlayers = new Dictionary<string, OnlinePlayerData>();

        private void Start()
        {
            _templatePlayerObject.gameObject.SetActive(false);
        }

        private void Update()
        {
            string[] playerKeys = new string[_onlinePlayers.Count];
            _onlinePlayers.Keys.CopyTo(playerKeys, 0);

            foreach (var key in playerKeys)
            {
                if(_onlinePlayers[key].playerTransform == null)
                {
                    _templatePlayerObject.SetActive(true);
                    _onlinePlayers[key].playerTransform = Instantiate(_templatePlayerObject, null).transform;
                    _templatePlayerObject.SetActive(false);
                }

                _onlinePlayers[key].timeoutTimer -= Time.deltaTime;

                if (_onlinePlayers[key].timeoutTimer <= 0.0f)
                {
                    Destroy(_onlinePlayers[key].playerTransform.gameObject);
                    _onlinePlayers.Remove(key);
                }
                else
                {
                    var player = _onlinePlayers[key];

                    var position = Vector3.Lerp(player.playerTransform.position, player.playerPackage.pos, Time.time);
                    player.playerTransform.position = position;

                    var rotation = Mathf.Lerp(player.playerTransform.rotation.eulerAngles.z, player.playerPackage.zRot, Time.time);
                    player.playerTransform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
                }
            }
        }

        public void UpdatePlayerPosition(string clientIp, PlayerPackage package)
        {
            if (!_onlinePlayers.ContainsKey(clientIp))
            {
                _onlinePlayers.Add(clientIp, new OnlinePlayerData
                {
                    playerTransform = null,
                    playerPackage = package,
                    timeoutTimer = 5.0f
                });
            }
            else
            {
                _onlinePlayers[clientIp].playerPackage = package;
                _onlinePlayers[clientIp].timeoutTimer = 10.0f;
            }
        }
    }
}
