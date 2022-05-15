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
			
			SplitServerMessage("[JSON]{\"root\":{\"1\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<sad¦mask(#321)>[c:r¦f:yellow]}[c:u]\"},\"2\":{\"name\":\"<[c:r¦f:white]left¦eye[c:u](#1793)>\"},\"3\":{\"name\":\"<[c:r¦f:white]right¦eye[c:u](#1794)>\"},\"name\":\"<[c:r¦f:white]head[c:u](#1763)>\"},\"2\":{\"name\":\"<[c:r¦f:white]mouth[c:u](#1795)>\"},\"3\":{\"name\":\"<[c:r¦f:white]neck[c:u](#1762)>\"},\"4\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<elated¦shirt(#1796)>[c:r¦f:yellow]}[c:u]\"},\"name\":\"<[c:r¦f:white]torso[c:u](#1757)>\"},\"5\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<left¦elated¦shirt¦sleeve(#1796)>[c:r¦f:yellow]}[c:u]\"},\"2\":{\"1\":{\"name\":\"[c:glyph¦0,128,0;0,0,0;!:128,128,128;0,0,0;.].[c:u]<wooden¦club(#1798)>[c:glyph¦0,128,0;0,0,0;!:128,128,128;0,0,0;.].\"},\"name\":\"[c:glyph¦255,0,0;0,0,0;/:0,128,0;0,0,0;Ä:0,0,255;0,0,0;\\\\:0,128,0;0,0,0;|].[c:u]<[c:r¦f:white]left¦hand[c:u](#1769)>[c:glyph¦255,0,0;0,0,0;/:0,128,0;0,0,0;Ä:0,0,255;0,0,0;\\\\:0,128,0;0,0,0;|].[c:u]\"},\"name\":\"<[c:r¦f:white]left¦arm[c:u](#1765)>\"},\"6\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<right¦elated¦shirt¦sleeve(#1796)>[c:r¦f:yellow]}[c:u]\"},\"2\":{\"1\":{\"name\":\"[c:r¦f:gray]@[c:u]<dagger¦'Virginslayer'(#1540)>[c:r¦f:gray]@[c:u]\"},\"name\":\"<[c:r¦f:white]right¦hand[c:u](#1771)>\"},\"name\":\"<[c:r¦f:white]right¦arm[c:u](#1767)>\"},\"7\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<elated¦shirt(#1796)>[c:r¦f:yellow]}[c:u]\"},\"name\":\"<[c:r¦f:white]abdomen[c:u](#1759)>\"},\"8\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<pair¦of¦elated¦pants(#1797)>[c:r¦f:yellow]}[c:u]\"},\"name\":\"<[c:r¦f:white]groin[c:u](#1760)>\"},\"9\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<pair¦of¦elated¦pants(#1797)>[c:r¦f:yellow]}[c:u]\"},\"2\":{\"name\":\"<[c:r¦f:white]left¦foot[c:u](#1779)>\"},\"name\":\"<[c:r¦f:white]left¦leg[c:u](#1773)>\"},\"10\":{\"1\":{\"name\":\"[c:r¦f:yellow]{[c:u]<pair¦of¦elated¦pants(#1797)>[c:r¦f:yellow]}[c:u]\"},\"2\":{\"name\":\"<[c:r¦f:white]right¦foot[c:u](#1781)>\"},\"name\":\"<[c:r¦f:white]right¦leg[c:u](#1776)>\"},\"name\":\"i, <Constantine(#1756)>\"},\"type\":\"inventory\"}");

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

						if (!client.IsConnected)
						{
							break;
						}
						//await Task.Delay(50);
					}
				}

			}
			catch (Exception e)
			{
				Program.loginConsole.miniDisplay.Cursor.Print(needToSendMessage.ToString()).NewLine();
			

				Program.MainConsole.ReciveExternalInput(e.ToString());
			}

			Program.loginConsole.miniDisplay.Cursor.Print("Could not connect to server").NewLine();
			

			Program.MainConsole.ReciveExternalInput("Could not connect to server");

		}

	}
        
        
}
