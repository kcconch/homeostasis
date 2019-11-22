using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Tls;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Connection connection;

#if UNITY_EDITOR
    private const string SocketUrl = "https://homeo.glitch.me/socket.io/";
    //private const string SocketUrl = "http://localhost:5000/socket.io/";
# else
    private const string SocketUrl = "https://homeo.glitch.me/socket.io/";
# endif

    private readonly Dictionary<string, ClientData> _clients = new Dictionary<string, ClientData>();

    private void Awake()
    {
        // Initialize connection
        InitConnection();
    }

    private void InitConnection()
    {
        // Setup the connection
        Debug.Log("Connecting to " + SocketUrl);
        connection = new Connection(SocketUrl, "Bouncer", "app");

        connection.OnConnect(() => { Debug.Log("Connected to server."); });

        connection.OnDisconnect(() =>
        {
            Debug.Log("Disconnected from server.");
            ClearAllClients();
        });

        connection.OnOtherConnect((id, type) =>
        {
            Debug.Log($"OTHER CONNECTED: {type} ({id})");
            if (type != "user") return;
            AddClient(id);
            PositionClients();
        });

        connection.OnOtherDisconnect((id, type) =>
        {
            Debug.Log($"OTHER DISCONNECTED: {type} ({id})");
            if (type == "user")
            {
                ClearClient(id);
                PositionClients();
            }
        });

        connection.OnError(err => { Debug.Log($"Connection error: {err}"); });

        connection.On("move", (string sourceId, float x, float y) =>
        {
            if (GetDataForClient(sourceId, out var data))
            {
                data.Input = new Vector2(x, y);
            }
        });

        connection.Open();
    }

    private void Func()
    {
        
    }
    
    private void PositionClients()
    {
        // Get all the clients and put them in an array
        var clientArray = _clients.Values.ToArray();
        // sort that array
        Array.Sort(clientArray, (ClientData clientData1, ClientData clientData2) => { 
            return (int)(clientData1.view.transform.position.x - clientData2.view.transform.position.x); 
        });
        var width = 1920 * 4;
        var n = clientArray.Length + 1;
        var offset = width / n;
        for (var i = 0; i < clientArray.Length; i++)
        {
            var c = clientArray[i];
            var p = c.view.transform.position;

            // c.view.transform.position = new Vector3(offset * i, p.y, p.z) * 2;

            if ( (offset * i) == 0) {
                c.view.transform.position = new Vector3(offset * i * 2, p.y, p.z)  + new Vector3(100, 0, 0);
            } else if ((offset * i) == width / 2 ) {
                c.view.transform.position = new Vector3(offset * i * 2, p.y, p.z)  - new Vector3(100, 0, 0);
            } else {
                c.view.transform.position = new Vector3(offset * i * 2, p.y, p.z);
            }

            Debug.Log(offset * i);
            
            connection.SendTo("player_location", c.id, offset * i);
        }
        // assign the new positions
    }
    
    private void OnDestroy()
    {
        connection.Close();
    }

    private void AddClient(string id)
    {
        if (GetDataForClient(id, out var data)) data.Destroy();
        _clients[id] = new ClientData(id);
    }

    private void ClearClient(string id)
    {
        if (GetDataForClient(id, out var data)) data.Destroy();
        _clients.Remove(id);
    }

    private void ClearAllClients()
    {
        foreach (var entry in _clients)
        {
            entry.Value.Destroy();
        }

        _clients.Clear();
    }

    private bool GetDataForClient(string clientId, out ClientData data)
    {
        if (clientId == null || !_clients.ContainsKey(clientId))
        {
            data = null;
            return false;
        }

        data = _clients[clientId];
        return true;
    }
}