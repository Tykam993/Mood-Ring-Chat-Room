using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;

public class MySQLDictionary : MonoBehaviour {
    public struct WordAndEmoIdeal {
        public string word;
        public EmotionModel.EmotionIdeal emoEnum;

        public WordAndEmoIdeal(string w, EmotionModel.EmotionIdeal e)
        {
            word = w;
            emoEnum = e;
        }
    }

    public const int MAX_WORD_LENGTH = 30;
    private static MySQLDictionary _instance;
    private static MySQLDictionary Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("MySQL Dictionary").GetComponent<MySQLDictionary>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<MySQLDictionary>();
                    _instance.name = "MySQL Dictionary";
                }
            }
            return _instance;
        }
    }

    private const string TABLE_NAME = "WordToEmotionIdeal";
    private const string TABLE_WORD = "Word";
    private const string TABLE_EMO_IDEAL = "EmotionIdealValue";

    private static string _source;
    private MySqlConnection _connection;
    public static MySqlConnection Connection
    {
        get
        {
            if (Instance._connection == null)
            {
                Connect();
            }
            return Instance._connection;
        }
    }


    // Use this for initialization
    void Start()
    {
        try
        {
            Connect();
        }
        catch (MySqlException e)
        {
            Debug.LogError("MySqlException " + e.StackTrace.ToString());
        }
    }

    #region Public Static Functions -- use these!

    public static void AddEntry(string word, EmotionModel.EmotionIdeal emotionIdeal)
    {
        Instance.AddEntryToDatabase(word, emotionIdeal);
    }

    public static bool DatabaseContainsEntry(string word)
    {
        return Instance.DatabaseContains(word);
    }

    public static WordAndEmoIdeal GetWordFromDatabase(string word)
    {
        return Instance.GetWord(word);
    }

    public static void InsertIntoDatabase(string word, EmotionModel.EmotionIdeal emoIdeal)
    {

    }

    #endregion

    static void Connect()
    {
        //         // Or option localhost, but still got error:
        // _source =       "Server=localhost;" +
        //               "Database = 5963_androidbase_1;" +    
        //           "User ID = root;" +
        //           "Pooling = false;" +
        //               "Password= ";

        _source = "Server=sql4.freemysqlhosting.net;" +
                     "Database=sql458495;" +
                     "User ID=sql458495;" +
                     "Password=uM2*yY2%";

        try
        {
            if (Instance._connection != null && Instance._connection.State != System.Data.ConnectionState.Open)
            {
                Instance._connection.Close();
                Connection.Open();
            }
            else
            {
                Instance._connection = new MySqlConnection(_source);
                Instance._connection.Open();
            }
        }
        catch (MySqlException e)
        {
            throw e;
        }
    }

    /// <summary>
    /// Raw write-to function for adding a string and EmotionIdeal to the database.
    /// This assumes that word is not too long (> 30 chars).
    /// </summary>
    /// <param name="word">The string to enter as the word</param>
    /// <param name="emotionIdeal">The emotion ideal to associate with word</param>
    private void AddEntryToDatabase(string word, EmotionModel.EmotionIdeal emotionIdeal)
    {
        MySqlCommand command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO " + TABLE_NAME + "(" + TABLE_WORD + ", " + TABLE_EMO_IDEAL + ")" +
            " VALUES" + "('" + word + "', " + "'" + ((int)emotionIdeal).ToString() + "');";

        command.ExecuteNonQuery();
    }


    private bool DatabaseContains(string word)
    {
        MySqlCommand command = _connection.CreateCommand();
        command.CommandText = "SELECT " + TABLE_WORD + " FROM " + TABLE_NAME +
            " WHERE " + TABLE_WORD + " = " + "'" + word + "'";

        MySqlDataReader data = command.ExecuteReader();

        return data.Read();
    }

    /// <summary>
    /// Gets a word and EmotionIdeal associated with it. This assumes that
    /// the database contains the word!
    /// </summary>
    /// <param name="word">Word to search for</param>
    /// <returns>WordAndEmoIdeal -- struct containing string and emotion ideal</returns>
    private WordAndEmoIdeal GetWord(string word)
    {
        WordAndEmoIdeal result = new WordAndEmoIdeal();

        MySqlCommand command = _connection.CreateCommand();
        command.CommandText = "SELECT " + TABLE_WORD + ", " + TABLE_EMO_IDEAL + " FROM " + TABLE_NAME +
            " WHERE " + TABLE_WORD + " = " + "'" + word + "'";

        MySqlDataReader data = command.ExecuteReader();

        while (data.Read())
        {
            string w = (string)data[TABLE_WORD];
            EmotionModel.EmotionIdeal emoIdeal = (EmotionModel.EmotionIdeal)
                System.Enum.Parse(typeof(EmotionModel.EmotionIdeal), 
                (string)data[TABLE_EMO_IDEAL]);

            result.word = w;
            result.emoEnum = emoIdeal;
        }

        return result;
    }
}
