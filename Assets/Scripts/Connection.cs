using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using Newtonsoft.Json.Linq;
using PlatformSupport.Collections.ObjectModel;

public class Connection
{
    private readonly SocketManager _manager;
    private readonly string _room;

    public Connection(string url, string room, string type)
    {
        var options = new SocketOptions
        {
            AutoConnect = false,
            AdditionalQueryParams = new ObservableDictionary<string, string>
            {
                ["room"] = room,
                ["type"] = type
            }
        };

        _manager = new SocketManager(new Uri(url), options);
        _room = room;
    }

    public void OnError(Action<string> handler)
    {
        _manager.Socket.On(SocketIOEventTypes.Error,
            (socket, packet, args) =>
            {
                handler(args[0].ToString());
            });
    }

    public void OnConnect(Action handler)
    {
        _manager.Socket.On(SocketIOEventTypes.Connect, (socket, packet, args) => handler(), false);
    }

    public void OnDisconnect(Action handler)
    {
        _manager.Socket.On(SocketIOEventTypes.Disconnect, (socket, packet, args) => handler(), false);
    }

    private struct ConnectedUserInfo
    {
#pragma warning disable 649
        // ReSharper disable once InconsistentNaming
        public string id;
        // ReSharper disable once InconsistentNaming
        public string type;
#pragma warning restore 649
    }

    public void OnOtherConnect(Action<string, string> handler)
    {
        _manager.Socket.On("__client_connected__", (socket, packet, _) =>
        {
            var args = GetArgs<List<ConnectedUserInfo>>(packet);
            var list = args.Item1;
            foreach (var info in list)
                handler(info.id, info.type);
        }, false);
    }

    public void OnOtherDisconnect(Action<string, string> handler)
    {
        On("__client_disconnected__", handler);
    }

    private T ParseObj<T>(JToken o)
    {
        try
        {
            return o.ToObject<T>();
        }
        catch (Exception e)
        {
            throw new Exception($"Could not parse argument of type {typeof(T)}. JSON: {o}. Exception: {e.Message}");
        }
    }

    private Tuple<T> GetArgs<T>(Packet packet)
    {
        var args = ParseArgArray(packet, 1);
        return new Tuple<T>(ParseObj<T>(args[0]));
    }

    private Tuple<T1, T2> GetArgs<T1, T2>(Packet packet)
    {
        var args = ParseArgArray(packet, 2);
        return new Tuple<T1, T2>(ParseObj<T1>(args[0]), ParseObj<T2>(args[1]));
    }

    private Tuple<T1, T2, T3> GetArgs<T1, T2, T3>(Packet packet)
    {
        var args = ParseArgArray(packet, 3);
        return new Tuple<T1, T2, T3>(ParseObj<T1>(args[0]), ParseObj<T2>(args[1]), ParseObj<T3>(args[2]));
    }

    private Tuple<T1, T2, T3, T4> GetArgs<T1, T2, T3, T4>(Packet packet)
    {
        var args = ParseArgArray(packet, 4);
        return new Tuple<T1, T2, T3, T4>(ParseObj<T1>(args[0]), ParseObj<T2>(args[1]), ParseObj<T3>(args[2]), ParseObj<T4>(args[3]));
    }

    private JArray ParseArgArray(Packet packet, int numExpected)
    {
        var argsString = packet.RemoveEventName(false);
        var args = JArray.Parse(argsString);
        if (args.Count != numExpected)
        {
            throw new Exception($"Expected {numExpected} args but got {args.Count}: {packet.Payload}");
        }
        return args;
    }

    public void On(string message, Action<string> handler)
    {
        _manager.Socket.On(message, (socket, packet, _) =>
        {
            // ValidateArgs(message, args, 1);
            var sourceId = GetArgs<string>(packet).Item1;
            handler(sourceId);
        }, false);
    }

    public void On<T>(string message, Action<string, T> handler)
    {
        _manager.Socket.On(message, (socket, packet, _) =>
        {
            var (sourceId, arg) = GetArgs<string, T>(packet);
            handler(sourceId, arg);
        }, false);
    }

    public void On<T1, T2>(string message, Action<string, T1, T2> handler)
    {
        _manager.Socket.On(message, (socket, packet, args) =>
        {
            var (sourceId, arg1, arg2) = GetArgs<string, T1, T2>(packet);
            handler(sourceId, arg1, arg2);
        }, false);
    }

    public void On<T1, T2, T3>(string message, Action<string, T1, T2, T3> handler)
    {
        _manager.Socket.On(message, (socket, packet, args) =>
        {
            var (sourceId, arg1, arg2, arg3) = GetArgs<string, T1, T2, T3>(packet);
            handler(sourceId, arg1, arg2, arg3);
        }, false);
    }

    public void Send(string message)
    {
        _manager.Socket.Emit(message, _room);
    }

    public void Send<T>(string message, T data)
    {
        _manager.Socket.Emit(message, _room, data);
    }

    public void Send<T1, T2>(string message, T1 data1, T2 data2)
    {
        _manager.Socket.Emit(message, _room, data1, data2);
    }

    public void Send<T1, T2, T3>(string message, T1 data1, T2 data2, T3 data3)
    {
        _manager.Socket.Emit(message, _room, data1, data2, data3);
    }

    public void SendTo(string message, string room)
    {
        _manager.Socket.Emit(message, room);
    }

    public void SendTo<T>(string message, string room, T data)
    {
        _manager.Socket.Emit(message, room, data);
    }

    public void SendTo<T1, T2>(string message, string room, T1 data1, T2 data2)
    {
        _manager.Socket.Emit(message, room,  data1, data2);
    }

    public void SendTo<T1, T2, T3>(string message, string room,T1 data1, T2 data2, T3 data3)
    {
        _manager.Socket.Emit(message, room, data1, data2, data3);
    }

    public void Open()
    {
        _manager.Open();
    }

    public void Close()
    {
        _manager.Close();
    }
}