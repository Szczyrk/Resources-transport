using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace vvcx.BD
{
    public static class SQLite
    {
        static SQLiteConnection connection;
        static SQLiteCommand command;

        public static void CreateBD()
        {
            connection = new SQLiteConnection(string.Format("Data Source={0}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", ""), "BD", "DIYstores.db")));

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", ""), "BD", "DIYstores.db")))
                return;
            connection.Open();
            string SQL = "create table if not exists shop(Id integer primary key autoincrement, Name varchar(100), City varchar(100), Adress varchar(100), " +
            "Latitude real, Longitude real)";
            command = new SQLiteCommand(SQL, connection);
            command.ExecuteNonQuery();
            SQL = "create table if not exists product(Id integer primary key autoincrement, IdShop integer, Name varchar(100), Weight real)";
            command = new SQLiteCommand(SQL, connection);
            command.ExecuteNonQuery();
            connection.Close();
            List<Product> products = new List<Product>();
            products.Add(new Product("Cegła", 5));
            products.Add(new Product("Pustaki", 8));
            products.Add(new Product("Cement", 2));
            products.Add(new Product("Deski", 3));
            Shop example = new Shop("Manex", "Wroclaw", "Bystrzycka 26", 51.1183418, 16.9766105, products);
            List<Product> products2 = new List<Product>();
            products2.Add(new Product("Zaprawa", 1));
            products2.Add(new Product("Elewacja", 1.5));
            products2.Add(new Product("Bloczki betonowe", 15));
            products2.Add(new Product("Gwoździe", 0.1));
            Shop example1 = new Shop("Budinpol", "Wroclaw", "aleja Aleksandra Brucknera 23", 51.1328043, 17.0804143, products2);
            List<Shop> shops = new List<Shop>();
            shops.Add(example1);
            shops.Add(example);
            AddShop(shops);
        }
        public static bool AddShop(List<Shop> shop)
        {
            connection.Open();
            if (connection.State == ConnectionState.Closed)
                return false;

            string SQL = "select Id from shop order by Id desc limit 1";
            command = new SQLiteCommand(SQL, connection);
            int lastIdShop = Convert.ToInt32(command.ExecuteScalar());
            if (lastIdShop == 0)
                lastIdShop = 1;
            for (int i = 0; i < shop.Count; i++)
            {
                command.CommandText = string.Format("insert into shop(Id, Name, City, Adress, Latitude, Longitude) values({0}, @name, @city, @adress, @latitude, @longitude)", lastIdShop + i);
                string[] shopValue = shop[i].ShopToBD().Split(',');
                command.Parameters.Add("@name", DbType.String).Value = shopValue[0];
                command.Parameters.Add("@city", DbType.String).Value = shopValue[1];
                command.Parameters.Add("@adress", DbType.String).Value = shopValue[2];
                command.Parameters.Add("@latitude", DbType.Double).Value = shopValue[3];
                command.Parameters.Add("@longitude", DbType.Double).Value = shopValue[4];
                command.ExecuteNonQuery();
                command.Cancel();
                for (int j = 0; j < shop[i].Products.Count; j++)
                {
                    command.CommandText = string.Format("insert into product(IdShop, Name, Weight) values({0}, @name, @weight)", lastIdShop + i);
                    string[] productValue = shop[i].Products[j].ProductToBD().Split(',');
                    command.Parameters.Add("@name", DbType.String).Value = productValue[0];
                    command.Parameters.Add("@weight", DbType.Double).Value = productValue[1];
                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
            return true;
        }

        public static Shop GetShop(int id, bool closeConnection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            if (connection.State == ConnectionState.Closed)
                return null;

            string SQL = string.Format("select * from shop where Id = {0}", id);
            command = new SQLiteCommand(SQL, connection);
            SQLiteDataReader dataReader = command.ExecuteReader();

            SQL = string.Format("select * from product where IdShop = {0}", id);
            command = new SQLiteCommand(SQL, connection);
            SQLiteDataReader dataReaderProducts = command.ExecuteReader();
            List<Product> products = new List<Product>();
            while (dataReaderProducts.Read())
            {
                products.Add(new Product(dataReaderProducts["Name"].ToString(), dataReaderProducts.GetDouble(3)));
            }
            dataReader.Read();
            Shop shop = new Shop(dataReader.GetInt32(0), dataReader["Name"].ToString(), dataReader["City"].ToString(), dataReader["Adress"].ToString(),
                         dataReader.GetDouble(4), dataReader.GetDouble(5), products);
            if (closeConnection)
                connection.Close();
            return shop;
        }

        public static List<Shop> GetShops()
        {
            List<Shop> shops = new List<Shop>();

            connection.Open();
            if (connection.State == ConnectionState.Closed)
                return null;

            string SQL = "select Id from shop";
            command = new SQLiteCommand(SQL, connection);
            SQLiteDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                shops.Add(GetShop(dataReader.GetInt32(0), false));
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
            }
            connection.Close();
            return shops;
        }

        public static bool RemoveShop(int id)
        {
            connection.Open();
            if (connection.State == ConnectionState.Closed)
                return false;

            string SQL = string.Format("delete from shop where Id = {0}", id);
            command = new SQLiteCommand(SQL, connection);
            command.ExecuteNonQuery();

            connection.Close();
            return true;
        }

        public static bool RemoveProduct(int idShop, string name)
        {
            connection.Open();
            if (connection.State == ConnectionState.Closed)
                return false;

            string SQL = string.Format("delete from product where IdShop = {0} and Name = {1]", idShop, name);
            command = new SQLiteCommand(SQL, connection);
            command.ExecuteNonQuery();

            connection.Close();
            return true;
        }

        public static bool RemoveProducts(int idShop)
        {
            connection.Open();
            if (connection.State == ConnectionState.Closed)
                return false;

            string SQL = string.Format("delete from product where IdShop = {0}", idShop);
            command = new SQLiteCommand(SQL, connection);
            command.ExecuteNonQuery();

            connection.Close();
            return true;
        }
    }
}