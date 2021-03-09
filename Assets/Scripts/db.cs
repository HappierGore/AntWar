using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

public class db
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void other()
    {
        //
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
                                                                                             //string conn = "URI=file:" + Application.dataPath + "/Plugins/DeepSpace.db"; //Path to database.
        IDbConnection dbconn;
        //conexion a la base de datos
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT * FROM shipsEquipments"; //Seleccionar todo
        //string sqlQuery = "UPDATE shipsEquipments SET rightEquipment = 'simpleBullet' WHERE id = 1"; //Actualizar valor de una tabla
        string sqlQuery = "INSERT INTO manager (type,positionX,positionY,group,health)";
        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string shipType = reader.GetString(1);
            string rightEquipment = reader.GetString(2);
            string leftEquipment = reader.GetString(3);

            Debug.Log("id = " + id + " shipType = " + shipType + " rightEquipment =" + rightEquipment + "  leftEquipment =" + leftEquipment);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;
    }
    //ANT creator
    public static void NewAnt(string name, string Type, Vector2 position, string group, float health, float speed, float damage, float strenght)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db";
        //string conn = "URI=file:" + Application.dataPath + "/Plugins/DeepSpace.db"; //Path to database.
        IDbConnection dbconn;
        //conexion a la base de datos
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT * FROM shipsEquipments"; //Seleccionar todo
        //string sqlQuery = "UPDATE shipsEquipments SET rightEquipment = 'simpleBullet' WHERE id = 1"; //Actualizar valor de una tabla
        string sqlQuery = "INSERT INTO Manager (name,type,positionX,positionY,groups,health,speed,damage,strenght) VALUES " +
            "('" + name + "','" + Type + "','" + position.x + "','" + position.y + "','" + group + "','" + health + "','" + speed + "','" + damage + "','" + strenght + "')";
        Debug.Log(sqlQuery);
        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;
    }
    public static bool CheckAlreadyExists(string name)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT name from Manager WHERE name = " + "'" + name + "'";

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string nombre = "none";
        while (reader.Read())
        {
            nombre = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        if (nombre != "none")
        {
            return true;
        }
        return false;
    }

    //DB manager
    public static void  CheckIfDBExists()
    {
        string filepath = Application.persistentDataPath + "/antManager.db";
        if (!File.Exists(filepath))
        {
            Debug.Log("create database");
            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "antManager.db");
            while (!loadDB.isDone) { }
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDB.bytes);

        }
    }

    //GETTERS
    // ----------------- ANTS ----------------
    public static int GetIDCounter()
    {
        //
        //Debug.Log("URI = file:" + Application.dataPath + " / StreamingAssets / antManager.db");

        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT id from Manager";

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        int MaxId = 0;
        while (reader.Read())
        {
            MaxId = reader.GetInt16(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return MaxId;
    }
    public static string GetNameFromID(int id)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT name from Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string name = "none";
        while (reader.Read())
        {
            name = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return name;
    }
    public static string GetTypeFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT type from Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string type = "none";
        while (reader.Read())
        {
            type = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return type;
    }
    public static string GetGroupFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT groups FROM Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string group = "none";
        while (reader.Read())
        {
            group = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return group;
    }
    public static Vector2 GetPositionFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT positionX,positionY FROM Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        Vector2 pos = new Vector2(0.0f, 0.0f);
        while (reader.Read())
        {
            pos.x = reader.GetFloat(0);
            pos.y = reader.GetFloat(1);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return pos;
    }
    public static float GetHealthFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT health FROM Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        float health = 0.0f;
        while (reader.Read())
        {
            health = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return health;
    }
    public static float GetSpeedFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT speed FROM Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        float speed = 0.0f;
        while (reader.Read())
        {
            speed = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return speed;
    }
    public static float GetDamageFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT damage FROM Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        float damage = 0.0f;
        while (reader.Read())
        {
            damage = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return damage;
    }
    public static float GetStrenghtFromID(int id)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        //string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT strenght FROM Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        float strenght = 0.0f;
        while (reader.Read())
        {
            strenght = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return strenght;
    }
    public static string GetObjectToGo(int id)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT objectToGo from Manager WHERE id = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string objectName = "none";
        while (reader.Read())
        {
            objectName = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return objectName;
    }


    //------------------- GROUPS ------------------

    //SETTERS
    public static void UpdateAntData(string name, string Type, Vector2 position, string group, float health, float speed, float damage, float strenght, string objectToGo)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db";
        //string conn = "URI=file:" + Application.dataPath + "/Plugins/DeepSpace.db"; //Path to database.
        IDbConnection dbconn;
        //conexion a la base de datos
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT * FROM shipsEquipments"; //Seleccionar todo
        //string sqlQuery = "UPDATE shipsEquipments SET rightEquipment = 'simpleBullet' WHERE id = 1"; //Actualizar valor de una tabla
        string sqlQuery = "UPDATE Manager SET name = '" + name + "',type = '" + Type + "',positionX = '" + position.x + "',positionY = '" + position.y +
            "',groups = '" + group + "',health = '" + health + "',speed = '" + speed + "',damage = '" + damage + "',strenght = '" + strenght +
            "',objectToGo = '" + objectToGo + "' WHERE name = '" + name + "'";
        Debug.Log(sqlQuery);
        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

    }
    public static void UpdateGroupName(int ID, string name)
    {
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db";
        //string conn = "URI=file:" + Application.dataPath + "/Plugins/DeepSpace.db"; //Path to database.
        IDbConnection dbconn;
        //conexion a la base de datos
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT * FROM shipsEquipments"; //Seleccionar todo
        //string sqlQuery = "UPDATE shipsEquipments SET rightEquipment = 'simpleBullet' WHERE id = 1"; //Actualizar valor de una tabla
        string sqlQuery = "UPDATE Groups SET name = '" + name + "' WHERE groups = " + ID;
        Debug.Log(sqlQuery);
        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando

        dbcmd.Dispose();
        dbcmd = null;


        dbconn.Close();
        dbconn = null;

    }
    public static void SaveAntsToGroup(int id, string ants)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db";
        //string conn = "URI=file:" + Application.dataPath + "/Plugins/DeepSpace.db"; //Path to database.
        IDbConnection dbconn;
        //conexion a la base de datos
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
        //string sqlQuery = "SELECT * FROM shipsEquipments"; //Seleccionar todo
        //string sqlQuery = "UPDATE shipsEquipments SET rightEquipment = 'simpleBullet' WHERE id = 1"; //Actualizar valor de una tabla
        string sqlQuery = "UPDATE Groups SET ants = '" + ants + "'" + " WHERE groups = " + id;
        Debug.Log(sqlQuery);
        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;
    }

    //Getters
    public static string GetGroupToUI(int id)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT name from Groups WHERE groups = " + id;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string group = "none";
        while (reader.Read())
        {
            group = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return group;
    }
    public static string GetAntsOfGroup(int group)
    {
        //
        string conn = "URI = file:" + Application.persistentDataPath + "/antManager.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        //COMANDO
        string sqlQuery = "SELECT ants from Groups WHERE groups = " + group;

        dbcmd.CommandText = sqlQuery; //guarda comando
        IDataReader reader = dbcmd.ExecuteReader(); //ejecuta comando
        string ants = "none";
        while (reader.Read())
        {
            ants = reader.GetString(0);
        }
        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;

        dbconn.Close();
        dbconn = null;

        return ants;
    }
}
