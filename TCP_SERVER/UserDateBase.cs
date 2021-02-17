using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_SERVER
{
    public class UserDateBase
    {
        public string connectionString = @"Data Source=PRJ-VOLOBUEV;Initial Catalog=ViberSVZK;Integrated Security=True";// строка подключения к бд
        public List<User> GetUsers()
        {
            using (DataContext db = new DataContext(connectionString))
            {
                var users = db.GetTable<User>();
                var list = users.ToList();
                return list;
            }
        }

        //добавление в бд пользователя  +++++++
        public void AddUser(User user)
        {
            string sqlExpression = "sp_InsertUser"; // название процедуры
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = user.Name
                };
                command.Parameters.Add(nameParam);
                SqlParameter passParam = new SqlParameter
                {
                    ParameterName = "@password",
                    Value = user.Password
                };
                command.Parameters.Add(passParam);
                var result = command.ExecuteNonQuery();
            }
        }

        //поиск пользователя в базе
        public string Find(string name, string password)
        {

            using (DataContext db = new DataContext(connectionString))
            {

                User user = db.GetTable<User>().FirstOrDefault(x => x.Name == name && x.Password == password);

                if (user == null)
                {
                    return "ок";
                }
                else
                {
                    return "no";
                }
            }
        }
        
       
       
        //список имён пользователей+++
        public string GetNameUsers()
        {
            using (DataContext db = new DataContext(connectionString))
            {
                var users = db.GetTable<User>().ToList();
                var listNames = new List<string>();
                foreach(var name in users)
                {
                    listNames.Add(name.Name);
                }
                string[] data = listNames.ToArray();
                string json = JsonConvert.SerializeObject(data);
                return json;
            }
        }
       
       
        //список задач пользователя
        public string Get_Zadachi(string userName)
        {
            using (DataContext db = new DataContext(connectionString))
            {
                var zadachi_1 = db.GetTable<Zadacha>().Where(x => x.Avtor== userName).ToList();//список с 1-м критерием поиска 
                var zadach_2 = db.GetTable<Zadacha>().Where(x => x.Ispolniteli==userName).ToList();//список с 2-м критерием поиска
                var zadachi = zadachi_1.Union(zadach_2).ToArray();//объединяем результаты поисков
                string json = JsonConvert.SerializeObject(zadachi);
                return json;
            }
        }
        //задача принята
        public string Prinyato(string id)
        {

            using (DataContext db = new DataContext(connectionString))
            {
                int _id = Convert.ToInt32(id);
                var zadachi_1 = db.GetTable<Zadacha>().FirstOrDefault(x => x.Id == _id);
                zadachi_1.Prinyato = true;
                db.SubmitChanges();
                return "задача принята";
            }
        }
        //задача отклонена
        public string Otklon(string id)
        {

            using (DataContext db = new DataContext(connectionString))
            {
                int _id = Convert.ToInt32(id);
                var zadachi_1 = db.GetTable<Zadacha>().FirstOrDefault(x => x.Id == _id);
                zadachi_1.Prinyato = false;
                db.SubmitChanges();
                return "задача отклонена";
            }
        }
        //задача выполнена
        public string Vypoln(string id)
        {

            using (DataContext db = new DataContext(connectionString))
            {
                int _id = Convert.ToInt32(id);
                var zadachi_1 = db.GetTable<Zadacha>().FirstOrDefault(x => x.Id == _id);
                zadachi_1.Gotovnost = true;
                db.SubmitChanges();
                return "задача выполнена";
            }
        }
        //новая задача
        public string NewZadacha(string [] mass,string name)
        {
            Zadacha zadacha = new Zadacha()
            {
                Avtor = name,
                Otdel = mass[0],
                Stadia = mass[1],
                Shifr = mass[2],
                Obj = mass[3],
                Prioritet = mass[4],
                Srok = DateTime.Parse(mass[5]),
                Ispolniteli = mass[6],
                Comment = mass[7],
                Gotovnost = false,
                Prinyato = false
            };
            using (DataContext db = new DataContext(connectionString))
            {
                // добавляем его в таблицу Users
                db.GetTable<Zadacha>().InsertOnSubmit(zadacha);
                db.SubmitChanges();
            }
            return "задача отправлена!";

        }
       
    }

}
