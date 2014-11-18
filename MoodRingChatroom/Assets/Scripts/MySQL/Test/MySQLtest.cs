using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;

public class MySQLtest : MonoBehaviour
{
    public bool runMySQLconnectionTest = false;
    private string source;
    private MySqlConnection connection;


    // Use this for initialization
    void Start() { 
        try
        {
            if (!runMySQLconnectionTest) { return; }
            source = "Server=sql4.freemysqlhosting.net;" +
                     "Database=sql458495;" +
                     "User ID=sql458495;" +
                     "Password=uM2*yY2%";

            //         // Or option localhost, but still got error:
            // source =       "Server=localhost;" +
            //               "Database = 5963_androidbase_1;" +    
            //           "User ID = root;" +
            //           "Pooling = false;" +
            //               "Password= ";

            Connect();
            Listing(connection);
            connection.Close();
        }
        catch (MySqlException e)
        {
            Debug.LogError("Cannot connect to MySQL server. Please recheck source details:\n" +
            source);
        }
 }

    void Connect()
    {
        try
        {
            connection = new MySqlConnection(source);
            connection.Open();
        }
        catch (MySqlException e)
        {
            throw e;
        }
    }

    void Listing(MySqlConnection _connection)
    {
        MySqlCommand command = _connection.CreateCommand();
        command.CommandText = "SELECT VERSION(), CURRENT_DATE;";
        MySqlDataReader data = command.ExecuteReader();

        while (data.Read())
        {
            string name = (string)data["VERSION()"] + ", " + data["CURRENT_DATE"].ToString();


            Debug.Log("Version/Date: " + name);

        }

    }

}