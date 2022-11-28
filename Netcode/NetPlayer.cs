using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{
    public enum actions
    {
        PlayCard, SelectBuilding, SelectArmy
    }
    public int port;
    public string IP = "127.0.0.1";
    public Player Owner;
    public Transform Players;
    public StateManager State;
    private UdpClient udp;
    private int[] received = new int[0];

    // Start is called before the first frame update
    void Start()
    {
        port = FreeTcpPort();
        udp = new UdpClient(port);
        //port = udp.Client.LocalEndPoint.
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        udp.BeginReceive(new AsyncCallback(OnUdpData), udp);
        //Connect();
    }

    public void SendPlayCard(GameObject card)
    {
            // Format array Player, Action, Target
        IEnumerable<byte> Player = BitConverter.GetBytes(Owner.playerNum);
        IEnumerable<byte> Action = BitConverter.GetBytes((int)actions.PlayCard);
        IEnumerable<byte> Target = BitConverter.GetBytes(Owner.MyHand.UIint[card]);
        IEnumerable<byte> Bytes = Player.Concat(Action).Concat(Target);
        SendByteArr(Bytes.ToArray());

    }
    private bool TimeClickBuilding(Building what)
    {
        //ToDo, Determine if selecting a building is a valid action before sending data.
        return true;
    }
    private bool TimeClickArmy(Army what)
    {
        //ToDo, Determine if selecting is a valid action before sending data.
        return true;
    }
    public void SendClick(GameObject Clicked)
    {
        // Format array Player, Action, Target, Targets owner
        IEnumerable<byte> Player = BitConverter.GetBytes(Owner.playerNum);
        IEnumerable<byte> Action = null;
        IEnumerable<byte> TOwner = null;
        IEnumerable<byte> Target = null;
        IEnumerable<byte> Bytes;
        Building cBuild;
        Army cArmy;
        string[] buildings;
        bool Send = false;
        if (Clicked.TryGetComponent<Building>(out cBuild))
        {
            if (TimeClickBuilding(cBuild))
            {
                Action = BitConverter.GetBytes((int)actions.SelectBuilding);
                buildings = State.board.AllBuildings[cBuild].Split("/");
                Target = BitConverter.GetBytes(int.Parse(buildings[1]));
                TOwner = BitConverter.GetBytes(int.Parse(buildings[0]));
                Send = true;
            }
        }
        if (Clicked.TryGetComponent<Army>(out cArmy))
        {
            if (TimeClickArmy(cArmy))
            {
                cBuild = cArmy.parent;
                Action = BitConverter.GetBytes((int)actions.SelectArmy);
                buildings = State.board.AllBuildings[cBuild].Split("/");
                Target = BitConverter.GetBytes(int.Parse(buildings[1]));
                TOwner = BitConverter.GetBytes(int.Parse(buildings[0]));
                Send = true;
            }
        }
        if (Send)
        {
            Bytes = Player.Concat(Action).Concat(Target).Concat(TOwner);
            SendByteArr(Bytes.ToArray());
        }
    }
    public void SendByteArr(byte[] Bytes)
    {
        try
        {
            // basic af
            IPEndPoint target = new IPEndPoint(IPAddress.Parse(IP), port);
            udp.Send(Bytes, Bytes.Length, target);
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
    }
     void OnUdpData(IAsyncResult result)
    {
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;

        // points towards whoever had sent the message:
        IPEndPoint source = new IPEndPoint(0, 0);
        // get the actual message and fill out the source:
        byte[] message = socket.EndReceive(result, ref source);
        // do what you'd like with `message` here:
        print("Got " + message.Length + " bytes from " + source);
        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
       parseReceived(message);
    }
    void parseReceived(byte[] data)
    {

        received = new int[data.Length / 4];
        byte[] chunk = new byte[4];
        for (int i = 0; i < data.Length; i++)
        {
            chunk[i % 4] = data[i];
            //when 4 bytes(1 int) is value of chunk, convert to int
            if (i%4 == 3)
            {
                received[i / 4] = BitConverter.ToInt32(chunk);
            }
        }

    }
    void ReceivedToGame()
    {
        if (received[0] > -1)
        {
            Player player = Players.GetChild(received[0]).GetComponent<Player>();
            //a player did something
            if (received[1]==(int)actions.PlayCard)
            {
                // if that something was play a card, received[2] is what card it was.
                player.MyHand.PlayCard(player.MyHand.CardsUI[received[2]]);
            }
            if (received[1]==(int)actions.SelectBuilding)
            {
                State.SelectedBuilding(State.board.buildings[received[3]][received[2]]);
            }
            if (received[1]==(int)actions.SelectArmy)
            {
                State.SelectedArm(State.board.buildings[received[3]][received[2]].army);
            }
        }

        received = new int[0];
    }
    static int FreeTcpPort()
    {
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }
    // Update is called once per frame
    void Update()
    {
        //have to call this from the main thread for unity to play nice with it.
        if (received.Length > 0)
        {
            ReceivedToGame();
        }
    }
}
