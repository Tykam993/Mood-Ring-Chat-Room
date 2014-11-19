using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
public class MySQLDictionary : MonoBehaviour {

    public const float IN_BETWEEN_WAIT = 1f;

    public enum RequestType { Contains, Get, Add };
    public struct Request
    {
        public RequestType type;
        public string word;
        public WVObject<bool?> boolResult;
        public WVObject<WordAndEmoIdeal> wordEmoIdealResult;
        public EmotionModel.EmotionIdeal emotionIdeal;
    }
    private Queue<Request> _requestQueue;
    private Queue<Request> RequestQueue
    {
        get
        {
            if (_requestQueue == null) { _requestQueue = new Queue<Request>(); }
            return _requestQueue;
        }
    }

    private List<WVObject<WordAndEmoIdeal>> _wvObjectList_WordAndEmoIdeal; //this is a list of WVObjects, which stands for Word and Value Object
    private List<WVObject<WordAndEmoIdeal>> WVObjectList_WordAndEmoIdeal
    {
        get { if (_wvObjectList_WordAndEmoIdeal == null)
        { 
            _wvObjectList_WordAndEmoIdeal = new List<WVObject<WordAndEmoIdeal>>(); } return _wvObjectList_WordAndEmoIdeal;
        }
    }

    /// <summary>
    /// this is a list of contains object requests. Basically, when we want to see if a word is in the database,
    /// we create a new WVObject of type bool which--once it's finished contacting the db--will say whether it
    /// contians the word or not.
    /// </summary>
    private List<WVObject<bool?>> _wvObjectList_bool; //this is a list of WVObjects, which stands for Word and Value Object
    private List<WVObject<bool?>> WVObjectList_Bool
    {
        get { if (WVObjectList_Bool == null) { _wvObjectList_bool = new List<WVObject<bool?>>(); } return _wvObjectList_bool; }
        set { _wvObjectList_bool = value; }
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
            else if (Instance._connection.State == System.Data.ConnectionState.Closed)
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
        //We create the WVObject, and pass it to the coroutine
        WVObject<bool?> result = new WVObject<bool?>(word);

        Request newRequest = new Request();
        newRequest.type = RequestType.Add;
        newRequest.word = word;
        newRequest.emotionIdeal = emotionIdeal;
        newRequest.boolResult = result;

        Instance.RequestQueue.Enqueue(newRequest);
    }

    public static WVObject<bool?> DatabaseContainsEntry(string word)
    {
        //We create the WVObject, and pass it to the coroutine
        WVObject<bool?> result = new WVObject<bool?>(word);

        Request newRequest = new Request();
        newRequest.type = RequestType.Contains;
        newRequest.word = word;
        newRequest.boolResult = result;

        Instance.RequestQueue.Enqueue(newRequest);

        return result;
    }

    public static WVObject<WordAndEmoIdeal> GetWordFromDatabase(string word)
    {
        //We create the WVObject, and pass it to the coroutine
        WVObject<WordAndEmoIdeal> result = new WVObject<WordAndEmoIdeal>(word);

        Request newRequest = new Request();
        newRequest.type = RequestType.Get;
        newRequest.word = word;
        newRequest.wordEmoIdealResult = result;

        Instance.RequestQueue.Enqueue(newRequest);

        return result;
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
            if (Instance._connection != null && Instance._connection.State == System.Data.ConnectionState.Closed)
            {
                Instance._connection.Open();
            }
            else if (Instance._connection == null)
            {
                Instance._connection = new MySqlConnection(_source);
                Instance._connection.Open();
            }

            Debug.Log("Connection State: " + Instance._connection.State.ToString());

            //Now start the request loop
            Instance.StartCoroutine(Instance.ProcessRequestLoop());

            //AddEntry("testString", EmotionModel.EmotionIdeal.Active);
            //DatabaseContainsEntry("testString");
            //GetWordFromDatabase("testString");

        }
        catch (MySqlException e)
        {
            throw e;
        }
    }

    private IEnumerator ProcessRequestLoop()
    {
        bool _foundRequest=false;
        string _rType = "";

        if (RequestQueue.Count > 0)
        {
            _foundRequest = true;
            Request r = RequestQueue.Dequeue();
            Debug.Log("Starting new request: " + r.type.ToString());
            _rType = r.type.ToString();
            switch (r.type)
            {
                case RequestType.Add:
                    yield return StartCoroutine(Instance.AddEntryToDatabase(r.word, r.emotionIdeal, r.boolResult));
                    yield return new WaitForSeconds(IN_BETWEEN_WAIT);
                    break;
                case RequestType.Contains:
                    yield return StartCoroutine(Instance.DatabaseContains(r.word, r.boolResult));
                    yield return new WaitForSeconds(IN_BETWEEN_WAIT);
                    break;
                case RequestType.Get:
                    yield return StartCoroutine(Instance.GetWord(r.word, r.wordEmoIdealResult));
                    yield return new WaitForSeconds(IN_BETWEEN_WAIT);
                    break;
            }
        }

        if (_foundRequest) { Debug.Log("Finished request: " + _rType); }
        yield return null;

        StartCoroutine("ProcessRequestLoop");
    }

    /// <summary>
    /// Raw write-to function for adding a string and EmotionIdeal to the database.
    /// This assumes that word is not too long (> 30 chars).
    /// </summary>
    /// <param name="word">The string to enter as the word</param>
    /// <param name="emotionIdeal">The emotion ideal to associate with word</param>
    private IEnumerator AddEntryToDatabase(string word, EmotionModel.EmotionIdeal emotionIdeal, WVObject<bool?> result)
    {
        Debug.Log("Adding entry " + word + " to db");
        using (MySqlCommand command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO " + TABLE_NAME + "(" + TABLE_WORD + ", " + TABLE_EMO_IDEAL + ")" +
            " VALUES" + "('" + word + "', " + "'" + ((int)emotionIdeal).ToString() + "');";

            System.IAsyncResult asyncResult = command.BeginExecuteNonQuery();

            while (!asyncResult.IsCompleted) { yield return null; }

            command.EndExecuteNonQuery(asyncResult);

            result.value = true;
        }
    }


    private IEnumerator DatabaseContains(string word, WVObject<bool?> result)
    {
        Debug.Log("Checking if db contains word " + word);

        using (MySqlCommand command = _connection.CreateCommand())
        {
            command.CommandText = "SELECT " + TABLE_WORD + " FROM " + TABLE_NAME +
    " WHERE " + TABLE_WORD + " = " + "'" + word + "'";

            System.IAsyncResult asyncResult = command.BeginExecuteReader();

            while (!asyncResult.IsCompleted) { yield return null; }
            //Debug.Log(asyncResult.AsyncState);
            //Debug.Log(asyncResult.IsCompleted);
            MySqlDataReader data;
            using (data = command.EndExecuteReader(asyncResult))
            {
                while (data == null)
                {
                    //Debug.Log(command.CommandText);
                    Debug.LogWarning("warning! DatabaseContains(" + word + ") returned null!");
                    yield return new WaitForSeconds(10f);
                    //Debug.Log(command);
                    //Debug.Log(asyncResult);
                    data = command.EndExecuteReader(asyncResult);
                }
                result.value = data.Read();
                Debug.Log("DatabaseContains word '" + word + "': " + result.value.ToString());
                //if (!data.IsClosed) data.Close();
            }
        }
    }

    /// <summary>
    /// Gets a word and EmotionIdeal associated with it. This assumes that
    /// the database contains the word!
    /// </summary>
    /// <param name="word">Word to search for</param>
    /// <returns>WordAndEmoIdeal -- struct containing string and emotion ideal</returns>
    private IEnumerator GetWord(string word, WVObject<WordAndEmoIdeal> result)
    {

        Debug.Log("Getting word " + word + " from db");
        //WVObject<WordAndEmoIdeal> result = new WVObject<WordAndEmoIdeal>(word);

        WVObjectList_WordAndEmoIdeal.Add(result);

        WordAndEmoIdeal wordEmoIdeal = new WordAndEmoIdeal();

        using (MySqlCommand command = _connection.CreateCommand())
        {
            command.CommandText = "SELECT " + TABLE_WORD + ", " + TABLE_EMO_IDEAL + " FROM " + TABLE_NAME +
            " WHERE " + TABLE_WORD + " = " + "'" + word + "'";

            System.IAsyncResult asyncResult = command.BeginExecuteReader(System.Data.CommandBehavior.CloseConnection);

            Debug.Log("Connection before command: " + Connection.State.ToString());

            while (!asyncResult.IsCompleted) { yield return null; }
            try
            {
                MySqlDataReader data;
                using (data = command.EndExecuteReader(asyncResult))
                {
                    Debug.LogWarning("before data.Read()");
                    while (data.Read())
                    {
                        string w = (string)data[TABLE_WORD];
                        EmotionModel.EmotionIdeal emoIdeal = (EmotionModel.EmotionIdeal)
                            System.Enum.Parse(typeof(EmotionModel.EmotionIdeal),
                            data[TABLE_EMO_IDEAL].ToString());

                        wordEmoIdeal.word = w;
                        wordEmoIdeal.emoEnum = emoIdeal;

                        result.value = wordEmoIdeal;//at this point, since it's not null, we set it!
                        Debug.LogWarning("reading data");
                    }
                    Debug.LogWarning("Data.isClosed = " + data.IsClosed.ToString());
                    //if (!data.IsClosed) data.Close();
                }
                Debug.LogWarning("Data.isClosed AFTER using: " + data.IsClosed.ToString());

                Debug.Log("Connection after everything: " + Connection.State.ToString());
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogError(e.StackTrace);
                Debug.LogError(e.Message);
            }

        }
    }

    public class WVObject<T>
    {

        public T value;

        public bool IsDone
        {
            get { return value != null; }
        }

        private string _wordName;
        public string WordName
        {
            get { return _wordName; }
        }

        public WVObject(string wordName)
        {
            _wordName = wordName;
        }

        // can directly compare a WVObject to a string!
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }

            if (obj is string)
            {
                return (WordName == obj);
            }

            if (obj is WVObject<T>)
            {
                WVObject<T> other = (WVObject<T>)obj;
                return WordName == other.WordName && value.Equals(value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return WordName.GetHashCode() + value.GetHashCode();
        }
    }


    public class WordAndEmoIdeal
    {
        public string word;
        public EmotionModel.EmotionIdeal emoEnum;

        public WordAndEmoIdeal(string w, EmotionModel.EmotionIdeal e)
        {
            word = w;
            emoEnum = e;
        }

        public WordAndEmoIdeal()
        {
            word = "";
            emoEnum = EmotionModel.EmotionIdeal.None;
        }
    }
}
