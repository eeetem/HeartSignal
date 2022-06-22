using System;
using System.Collections.Generic;
using System.Threading;
using PrimS.Telnet;
using System.IO;
using Console = SadConsole.Console;

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
		    
		    new Thread(ServerLoop).Start();
	    }

	    private static void SplitServerMessage(string input)
	    {
		    
		   
		    int idx = 0;
		    //int idx = input.IndexOf(Environment.NewLine, StringComparison.Ordinal);
		    //          if (idx < 1) {


		    //       idx = input.IndexOf('\n');

		    //if (idx < 1)
		    //{

		    Program.PromptWindow.toptext = "ayo";
		    Program.PromptWindow.middletext = "changooooooos";
		    Program.PromptWindow.Type = PromptWindow.PopupType.MultiLine;
		    Program.PromptWindow.needsDraw = true;

		    idx = input.IndexOf('\r');
		    

		    if (idx > 0)
		    {
			    string MSG = input.Substring(0, idx);
			    File.AppendAllText("debuglog.txt", "parsing:"+MSG+"\n");
			    Thread t = new Thread(() => MessageParser.ParseServerMessage(MSG));
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

		    Thread t2 = new Thread(() => MessageParser.ParseServerMessage(input));
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
#if DEBUG
		     System.Console.WriteLine(message);
#endif
		   
		    messageQueue.Add(message);
		    needToSendMessage = true;

		    return true;

	    }

	    static bool needToSendMessage = false;
	    private static async void ServerLoop()
		{
			
			try
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
							string recived = await client.ReadAsync(TimeSpan.FromMilliseconds(150));
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

						
					}
				}

			}
			catch (Exception e)
			{
				Program.loginConsole?.miniDisplay?.Cursor.Print(e.Message).NewLine();
			
				//Program.MainConsole.ReciveExternalInput("[clear]");
				Program.MainConsole.ReciveExternalInput(e.Message);
			}

			Program.loginConsole?.miniDisplay?.Cursor.Print("Connection to server ended").NewLine();
			

			Program.MainConsole.ReciveExternalInput("Connection to server ended");
			
			PostPorcessing.SetOverlay("disconnected.jpg");
			PostPorcessing.AddTween("overlayalpha",1f,1f);

		}

	}
        
        
}
