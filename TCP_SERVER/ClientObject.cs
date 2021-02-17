using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TCP_SERVER
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера
        UserDateBase bs = new UserDateBase();

        public string Login { get; set; }

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = "";
                int i = 1;
                // получаем имя пользователя
                while (i>0)
                {
                    //получаем сообщения от пользователя
                    
                        message = GetMessage();
                        if(message=="reg")
                        {
                            Console.WriteLine("РЕГИСТРАЦИЯ");
                            message = GetMessage();

                           string[] data = JsonConvert.DeserializeObject<string[]>(message);
                           string login = data[0];
                           string password = data[1];
                           string findAcc = bs.Find(login,password);// поиск аккаунта в бд
                           if (findAcc == "ок")
                           {
                               Console.WriteLine(AddUser(login, password));
                               Send("YesReg");


                           }
                           else
                           {
                              Send("NameErr");
                           }
                       }
                       else
                       {
                            Console.WriteLine("Вxод");
                            message = GetMessage();
                            string[] data = JsonConvert.DeserializeObject<string[]>(message);
                            string login = data[0];
                            string password = data[1];
                            string findAcc = bs.Find(login,password);// поиск аккаунта в бд

                            if (findAcc == "no")
                            {
                               Login = login;
                               Console.WriteLine(Login);
                               Send("YesEnter");
                               Send(bs.Get_Zadachi(Login));///отправляем все задачи пользователя
                                i = 0;

                            }
                           else
                           {
                              Send("NoEnter");

                           }
                    
                       }
                   

                }

                     userName = Login;
                     message = userName + " вошел в чат";
                    Console.WriteLine(message);



                //в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {

                        message = GetMessage();

                        if (message == "")
                        {
                            message = String.Format("{0}: покинул чат", userName);
                            Console.WriteLine(message);
                            break;
                        }
                        if (message == "обнов.зад.")
                        {
                            Send(bs.Get_Zadachi(userName));///отправляем все задачи пользователя

                        }

                        else if(message == "_zadachi")//отправка задач пользователя
                        {
                            message = GetMessage();//получаем имя пользователя
                            Send("_zadachi");
                            Send(bs.Get_Zadachi(message));///отправляем все задачи пользователя

                        }
                        else if (message == "_PRIN")// пользователь принял задачу
                        {
                            message = GetMessage();
                            if(message== "ПРИНЯТО")
                            {
                                message = GetMessage();// получаем id задачи
                                Send(bs.Prinyato(message));                               
                                
                            }
                            else if(message== "ОТКЛОНИТЬ")
                            {
                                message = GetMessage();// получаем id задачи
                                Send(bs.Otklon(message));
                            }
                            else
                            {
                                message = GetMessage();// получаем id задачи
                                Send(bs.Vypoln(message));
                            }
                            

                        }
                        else if(message == "_нов.зад")
                        {
                            message = GetMessage();// получаем задачу
                            string[] zadacha = JsonConvert.DeserializeObject<string[]>(message);
                            Send(bs.NewZadacha(zadacha, userName));
                        }
                        else if(message == "полз.")
                        {
                            string json = bs.GetNameUsers();
                            Send(json);
                        }
                            

                    }
                    catch
                    {
                        message = String.Format("{0}: вышел из системы", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            finally
            {

                server.RemoveConnection(this.Id);
                Close();
            }
        }
        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {

            BinaryReader reader = new BinaryReader(Stream);
            // считываем строку из потока
            string message = reader.ReadString();
            return message;
        }

        // закрытие подключения
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        
        //добавление пользователя в бд(регистрация)
        public string AddUser(string name, string password)
        {
            try
            {
                
                User user = new User() { Name = name, Password = password };
                bs.AddUser(user);
                return "Регистрация прошла успешно!";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        //отправка списка пользоваетелей ,которые онлайн
       
        //отправка сообщения клиенту
        public void Send(string text)
        {
            BinaryWriter writer = new BinaryWriter(Stream);
            writer.Write(text);
            writer.Flush();
        }

    }
}
