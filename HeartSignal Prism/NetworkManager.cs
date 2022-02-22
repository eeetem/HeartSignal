using System;
using System.Collections.Generic;
using System.Threading;
using PrimS.Telnet;
using System.IO;

namespace HeartSignal
{
	
    public static class NetworkManager
    {
        
	    public static Client TelnetClient;

	    public static void ConnectToServer()
	    {
		    if (TelnetClient!=null && TelnetClient.IsConnected)
		    {
			    return;
		    }
		    ServerLoop();
	    }

	    private static void SplitServerMessage(string input)
	    {
		    
		    File.AppendAllText("debuglog.txt", "splitting:"+input+"\n");

		    int idx = 0;
		    //int idx = input.IndexOf(Environment.NewLine, StringComparison.Ordinal);
		    //          if (idx < 1) {


		    //       idx = input.IndexOf('\n');

		    //if (idx < 1)
		    //{


		    idx = input.IndexOf('\r');
		    

		    if (idx > 0)
		    {
			    string MSG = input.Substring(0, idx);
			    File.AppendAllText("debuglog.txt", "parsing:"+MSG+"\n");
			    Thread t = new Thread(() => Program.ParseServerMessage(MSG));
			    t.IsBackground = true;
			    t.Start();

			    if(idx +2 > 0){
					
				    input = input.Remove(0, idx + 2);
			    }
			    else
			    {
				    File.AppendAllText("debuglog.txt", "returned due to index exception\n");
				    return;
			    }

			    if (input.Length > 1)
			    {

				    SplitServerMessage(input);


			    }
			    //File.AppendAllText("debuglog.txt", "returned as expected\n");
			    return;
		    }

		    Thread t2 = new Thread(() => Program.ParseServerMessage(input));
		    t2.IsBackground = true;
		    t2.Start();




	    }


	    static List<string> messageQueue = new List<string>();
	    public static bool lockMessages = false;
	    public static bool SendNetworkMessage(string message, bool bypassLock = false)
	    {

		    if (lockMessages && !bypassLock)
		    {
			    return false;
		    }

		    System.Console.WriteLine(message);
		    messageQueue.Add(message);
		    needToSendMessage = true;

		    return true;

	    }

	    static bool needToSendMessage = false;
	    private static async void ServerLoop()
		{


			using (Client client = new Client("deathcult.today", 6666, new CancellationToken()))
				{
					await client.TryLoginAsync("", "", 1000);


					TelnetClient = client;
					


					while (TelnetClient.IsConnected)
					{
				
						if (needToSendMessage)
						{
							foreach (string message in new List<string>(messageQueue))
							{
								await client.WriteLine(message);
								messageQueue.Remove(message);
							}

							if (messageQueue.Count < 1)
							{
								needToSendMessage = false;
							}
						}

						string response = "";
						while (true)
						{
							string recived = await client.ReadAsync(TimeSpan.FromMilliseconds(100));
							if (recived.Length > 1)
							{
								//if we recived something - try reciving again since the message might have been cut halfway
								response += recived;
								continue;
							}
							//if nothing was recived in last 100ms process it
							break;

						}

						
						if (response.Length > 1)
						{
							SplitServerMessage(response);
#if DEBUG

   
                        System.Console.WriteLine(response);
                    


#endif
						}

						if (!client.IsConnected)
						{
							break;
						}
						//await Task.Delay(50);
					}
				}
			
		
		
			Program.loginConsole.miniDisplay.Cursor.Print("Could not connect to server").NewLine();
			

			Program.MainConsole.ReciveExternalInput("Could not connect to server");

		}

	}
        
        
}
