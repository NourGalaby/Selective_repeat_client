﻿//using System.Net.Sockets.SocketException;
using MyRefrence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace client
{

    class Program
    {
      public static   Button[] buttons;
          public static  Form abc;
          static Panel panel;

        public static bool[] status;
        static int plp = 20 ;
        static int SIZE=5;
        static bool simulateLoss()
        {
            Random rand = new Random();

            int x = rand.Next(1, 101);
            //  Console.WriteLine(x);
            if (x >= plp)
            {
                return true;
            }
            //  Console.WriteLine("LOSS");
            return false;
        }


        static int BUFFER_SIZE = 691-8;
        static object deserialize(byte[] data)
        {


            using (var mystream = new MemoryStream(data))
            {
                var bf = new BinaryFormatter();
                return bf.Deserialize(mystream);
            }

        }



        static int zero_one(int x)
        {
            if (x == 1)
                return 0;
            else return 1;

        }

        static public void bytestofile(List<myMessage> splitted, string fileName)
        {



            int size = splitted.Count * BUFFER_SIZE;

            byte[] file = new byte[size];
            int x = 0;
            foreach (myMessage y in splitted)
            {
                Array.Copy(y.data, 0, file, x, y.data.Length);
                x += y.data.Length;
            }

            using (FileStream
                fileStream = new FileStream(fileName, FileMode.Create))
            {
                // Write the data to the file, byte by byte.
                for (int i = 0; i < file.Length; i++)
                {
                    fileStream.WriteByte(file[i]);
                }
                fileStream.Seek(0, SeekOrigin.Begin);
            }
        }



    static    void init_gui(int SIZE){

        abc = new Form();
                     buttons = new Button[SIZE];
               for(int i=0;i<SIZE;i++){
                   Button b = new Button();

                   b.Location = new System.Drawing.Point(((i * 15) % SIZE), (int) ((i * 15)/ SIZE*15));
                   b.Name = "";
                   b.Size = new System.Drawing.Size(15, 15);
                   b.TabIndex = 0;
                   b.Text = "";
                   b.UseVisualStyleBackColor = true;
                   b.BackColor = System.Drawing.Color.Red;

                   buttons[i] = b;
               }
              
       panel= new Panel();

  
       panel.Location = new System.Drawing.Point(12, 7);
       panel.Name = "panel1";
       panel.Size = new System.Drawing.Size(900, 900);
       panel.TabIndex = 0;


               panel.Controls.AddRange(buttons);
               abc.Controls.Add(panel);

               abc.Size = new System.Drawing.Size(900, 900);


               Application.EnableVisualStyles();
               Application.Run(abc); // or whatever
             
        }
        
        
        static void Main(string[] args)
        {


            byte[] bytes = null;



            myMessage m1 = new myMessage();


            using (var mystream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(mystream, m1);
                bytes = mystream.ToArray();

            }
           
      //  string filename = "SamSmith.mp3";
   string filename = "picture.jpg";
   //  string filename = "Test.txt";
            bytes = Encoding.ASCII.GetBytes(filename);


            byte[] data = new byte[2000];
            int recv;

            IPEndPoint endpoint = new IPEndPoint(
                 IPAddress.Parse("127.0.0.1"), 950);
                    //   IPAddress.Parse("172.16.13.237"), 950); 


            Socket server = new Socket(AddressFamily.InterNetwork,
                           SocketType.Dgram, ProtocolType.Udp);
            //    Console.WriteLine(welcome);
            //   data = Encoding.ASCII.GetBytes(welcome);

            // Console.WriteLine("msg = "+ Encoding.ASCII.GetString( bytes) );
            server.SendTo(bytes, bytes.Length, SocketFlags.None, endpoint);


            // server.SendTo(data, data.Length, SocketFlags.None, endpoint);
            // Console.WriteLine("welcome1");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;

            //  server.SendTo(data, data.Length, SocketFlags.None, endpoint);
            server.SendBufferSize = 0;


        

            int pck_count = 12000;

            try
            {
                recv = server.ReceiveFrom(data, ref Remote);
                pck_count = int.Parse(Encoding.ASCII.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Maybe server is not running");

                //   return;
            }
            // Console.WriteLine("welcome3");
            Console.WriteLine("Message received from {0}:", Remote.ToString());
            //    Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));



            Thread gui;

            gui = new Thread(

                 () => init_gui(pck_count));

      


        gui.Start();

            status = new bool[pck_count];


            Thread.Sleep(20);
            //      byte current_seq = 1;
            List<myMessage> messages = new List<myMessage>();

           // byte[] ack = new byte[1];
            //ack[0] = 1;

            int k = 0;


            myMessage[] msgWindow = new myMessage[SIZE];

           
          //  for (int i = 0; i < Size; i++)
            while(true)
            {
                int n = 0;
                for ( n = 0; n < SIZE; n++) //check if last window arrived, then break
                {
                    try
                    {
                        if (!status[pck_count - n - 1])
                        {
                            n--;
                            break;

                        }
                    }
                    catch { }
                }


                //stop receving if all last windows arrived 
                if (n >= SIZE) { 
                    break; 
                }
                    data = new byte[1024];
                //////if (msg_rec.seq_no == ack[0])
                //////{
                //////    i--;
                //////    Console.WriteLine("DUPLICATE Packet");
                //////    //duplicate packet... dont add to list

                //////    if (simulateLoss())
                //////    {
                //////        // string  ack =zero_one(msg_rec.seq_no).ToString();

                //////        //send same ack again

                //////        //reverse ack , send, then reverse
                //////        ack[0] = (byte)zero_one(ack[0]);

                //receive data
               // Console.WriteLine("data size: " + data.Length); 
                recv = server.ReceiveFrom(data, ref Remote);

              
          //      Console.WriteLine("Received PACKET WITH SIZE:" + recv);    
                //convert from data to Message object
                myMessage msg_rec = (myMessage)deserialize(data);
          
                string seq_s= msg_rec.seq_no.ToString()+ "," ;
                data=Encoding.ASCII.GetBytes(seq_s );
              
                if(simulateLoss()){
                       server.SendTo(data, data.Length, SocketFlags.None, Remote);
                }
                else { 
                    Console.WriteLine("Loss");
                }
              Thread.Sleep(10);
                        Console.WriteLine("Sent ACk  " + msg_rec.seq_no );
                     
                 // put in a list
                if(!status[msg_rec.seq_no]){
                    try
                    {
                        buttons[msg_rec.seq_no].BackColor = System.Drawing.Color.Green;
                    }
                    catch { }
                  //  abc.Update();
                     messages.Add(msg_rec);
                               status[msg_rec.seq_no]=true;
                }else{
                    Console.WriteLine("Duplicate deteceted");
                 
                }
          

                }
     
              
                   
       

                    {
                        ////////Console.WriteLine("Loss ");
                        ////////ack[0] = (byte)zero_one(ack[0]);
                    }

                
                        //here all data is done receving so 
            //we want to covert from data to file
            // so  use the function you made to convert it to files


                    foreach (bool ss in status)
                    {
                        if (!ss)
                            Console.WriteLine("PACKET NOT ARRIVED");
                    }

                //    messages.OrderBy(s => s.seq_no);
          
                    messages.Sort((y, x) => y.seq_no - x.seq_no);
                    foreach (myMessage ss in messages)
                    {
                        Console.Write(ss.seq_no + " - ");
                    }

            bytestofile(messages, filename);

            Console.WriteLine("after sorting Data");

            Console.WriteLine("Stopping client");
            Console.ReadLine();

            }



        }

    }

