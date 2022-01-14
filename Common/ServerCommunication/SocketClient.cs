using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

//// ORIGINAL CODE CAME FROM https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
//// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-an-asynchronous-client-socket

// State object for receiving data from remote device.  
public class StateObject
{
	// Client socket.  
	public Socket workSocket = null;
	// Size of receive buffer.  
	public const int BufferSize = 256;
	// Receive buffer.  
	public byte[] buffer = new byte[BufferSize];
	// Received data string.  
	public StringBuilder sb = new StringBuilder();
}

public class AsynchronousClient
{
	// The port number for the remote device.  
	//private const int port = 11000;

	// ManualResetEvent instances signal completion.  
	private static ManualResetEvent _ConnectDone = new ManualResetEvent(false);
	private static ManualResetEvent _SendDone = new ManualResetEvent(false);
	private static ManualResetEvent _ReceiveDone = new ManualResetEvent(false);

	// The response from the remote device.  
	private static String _Response = String.Empty;

	/// <summary>
	/// Socket connection
	/// </summary>
	private static Socket _Client = null;

	/// <summary>
	/// Is the connection valid - ie has the GUI logged into the client?
	/// </summary>
	private static bool _IsConnectionValid = false;

	public static string StartClient(string data, string address, int port)
	{
		// Connect to a remote device.  
		Serilog.Log.Logger.Debug($"Socket - Start Client sending {data}");
		try
		{
			// Connect to the remote endpoint if needed. 
			if (!_IsConnectionValid)
			{
				IPAddress ipAddress = null;
				if (!IPAddress.TryParse(address, out ipAddress))
				{
					IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
					ipAddress = ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
				}

				_Client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_Client.Connect(ipAddress, port);
				_IsConnectionValid = true;
				Console.WriteLine("START SLEEPING FOR DO COMMANDS");
				System.Threading.Thread.Sleep(30000);
				Console.WriteLine("DONE SLEEPING FOR DO COMMANDS");
			}

			// Send test data to the remote device.  
			Send(_Client, data);
			_SendDone.WaitOne();

			// Receive the response from the remote device.  
			Receive(_Client);
			_ReceiveDone.WaitOne();

			// Write the response to the console.  
			Console.WriteLine("Response received : {0}", _Response);

			// Release the socket.  
			// TODO: When the system is shutting down this needs to be closed
			//client.Shutdown(SocketShutdown.Both);
			//client.Close();

			Serilog.Log.Logger.Debug($"Socket - Start Client received {_Response}");

			return _Response;

		}
		catch (Exception e)
		{
			_IsConnectionValid = false;
			Console.WriteLine(e.ToString());
		}
		return string.Empty;
	}

	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.  
			Socket client = (Socket)ar.AsyncState;

			// Complete the connection.  
			client.EndConnect(ar);

			Console.WriteLine("Socket connected to {0}",
				client.RemoteEndPoint.ToString());

			// Signal that the connection has been made.  
			_ConnectDone.Set();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	private static void Receive(Socket client)
	{
		try
		{
			// Create the state object.  
			StateObject state = new StateObject();
			state.workSocket = client;

			// Begin receiving the data from the remote device.  
			client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
				new AsyncCallback(ReceiveCallback), state);
			System.Diagnostics.Debug.WriteLine($"Recieve Called");
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	private static void ReceiveCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the state object and the client socket
			// from the asynchronous state object.  
			StateObject state = (StateObject)ar.AsyncState;
			Socket client = state.workSocket;

			// Read data from the remote device.  
			int bytesRead = client.EndReceive(ar);

			if (bytesRead > 0)
			{
				// There might be more data, so store the data received so far.  
				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

				// Get the rest of the data.  
				client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
					new AsyncCallback(ReceiveCallback), state);
			}
			//else
			//{
			//    // All the data has arrived; put it in response.  
			if (state.sb.Length >= 1)
			{
				_Response = state.sb.ToString();
			}
			// Signal that all bytes have been received.  
			_ReceiveDone.Set();
			//}
			System.Diagnostics.Debug.WriteLine($"RecieveCallback Finshed {bytesRead}");
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	private static void Send(Socket client, String data)
	{
		// Convert the string data to byte data using ASCII encoding.  
		byte[] byteData = Encoding.ASCII.GetBytes(data);

		// Begin sending the data to the remote device.  
		client.BeginSend(byteData, 0, byteData.Length, 0,
			new AsyncCallback(SendCallback), client);
		System.Diagnostics.Debug.WriteLine($"Send Finshed ${data}");
	}

	private static void SendCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.  
			Socket client = (Socket)ar.AsyncState;

			// Complete sending the data to the remote device.  
			int bytesSent = client.EndSend(ar);
			Console.WriteLine("Sent {0} bytes to server.", bytesSent);

			// Signal that all bytes have been sent.  
			_SendDone.Set();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

}