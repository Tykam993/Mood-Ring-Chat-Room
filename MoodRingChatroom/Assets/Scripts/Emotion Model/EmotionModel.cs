using UnityEngine;
using System.Collections;

/// <summary>
/// This is the core model of how we represent the emotional state of the user.
/// Basically, the emotional state is stored as a Vector2 with a current weight.
/// 
///     Vector2 : Based on the orthogonal axes
///         (Unpleasant (-x) <----> Pleasant (+x)) and
///         (Active (+y) <----> Inactive (-y))
///         
///         the current state of the user is some point on or within the unit circle created by these axes.
///         Thus,  0 >= ||currentState|| <= 1
///         
///     Weight:
///         This is essentially a count of how many times the model has been modified.
///         The higher the weight, the less effective each subsequent change becomes.
///         If we do this linearly, the first changes will have a disproportionately large affect on the model,
///         and the later changes will have too little an effect. To counter this, we are using
///         a nonlinear model (TBD).
///         
/// 
/// 
/// When we modify the state of the model, we move the vector in the direction of an ideal.
/// An ideal is a Vector2 that lies OUTSIDE the unit circle. There are 8 ideals, each with an Active/Deactive and Pleasant/Unpleaseant classification:
/// 
///     (0, +y)  -> Active
///     (+x, +y) -> Excited
///     (+x, 0)  -> Pleasant
///     (+x, -y) -> Content
///     (0, -y)  -> Deactive
///     (-x, -y) -> Bored
///     (-x, 0)  -> Unpleasant
///     (-x, +y) -> Panic
///     
/// We have a list of words we ascribe these ideals to.
///     For example, "mistrust" might be labeled as [Unpleasant, Panic]
///         - That means our ideal lies in between Unpleasant and Panic.
///         - In this case, we moving to an in-between an ideal.
///         - Our current state moves in the direction of that ideal, given the current weight
/// </summary>
public class EmotionModel
{

    public enum EmotionIdeal { None=-1, Active=1, Excited=2, Pleasant=3, Content=4,
        Deactive=5, Bored=6, Unpleasant=7, Panic=8 };

    #region Never Touch With Code!!
    private Vector2 _currentStateRaw = Vector2.zero; //do not directly modify!! EVER!
    #endregion

    #region Private Members
    private float _weight = .5f; //between 0 and 1; 0 means there's no effect; 1 meanas it has a total effect (full interpolation from vectorA to vectorB)
    private static EmotionModel _instance;
    private int _modCount = 0; //the number of times we've modified the model
    #endregion

    #region Private Properties
    private Vector2 _currentState
    {
        get
        {
            return _currentStateRaw;
        }
        set
        {
            if (value.magnitude > 1)
            {
                _currentStateRaw = value.normalized;
            }
            else
            {
                _currentStateRaw = value;
            }
        }
    }

    private static EmotionModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EmotionModel();
            }
            return _instance;
        }
    }
    #endregion

    #region Public Properties -- READ ONLY
    public static Vector2 CurrentState
    {
        get
        {
            return Instance._currentState;
        }
    }

    public static float Weight
    {
        get
        {
            return Instance._weight;
        }
    }

    public static int NumberOfModifications
    {
        get
        {
            return Instance._modCount;
        }
    }
    #endregion

    #region Public Static Functions -- USE THESE FUNCTIONS!

    public static void ChangeStateByAddingEmotions(EmotionIdeal emotionIdeal, float idealOffset = .5f)
    {
        ChangeStateByAddingEmotions(new EmotionIdeal[] { emotionIdeal }, idealOffset);
    }
    /// <summary>
    /// Given one or many EmotionIdeals, it modifies the current state of the emotion model
    /// based on the current weight and what emotion ideals were given.
    /// </summary>
    /// <param name="emotionIdeals">The emotion ideals that we are "aiming" for</param>
    /// <param name="idealOffset">How far from the unit circle the ideals lie; 0.5 by default</param>
    public static void ChangeStateByAddingEmotions(EmotionIdeal[] emotionIdeals, float idealOffset = .5f)
    {
        //Get the emo vector associated with the Emotion Ideals
        Vector2 emoIdealVector = Instance.GetEmoVector(emotionIdeals, idealOffset);

        //Move the current state vector
        Instance.MoveCurrentState(emoIdealVector);

        //Update the number of times we've modified the model
        Instance._modCount++;

        //Update the Weight of our model
        Instance.UpdateWeight();
    }


    public static void ChangeStateByAddingVector(Vector2 vec, float idealOffset = .5f)
    {
        //Move the current state vector
        Instance.MoveCurrentState(vec);

        //Update the number of times we've modified the model
        Instance._modCount++;

        //Update the Weight of our model
        Instance.UpdateWeight();
    }

    #endregion


    #region Private Functions

    private void UpdateWeight()
    {
        float maxWeight = .1f, minWeight = .01f;

        float currentProgress = (float)NumberOfModifications / 100f;
        if (currentProgress > 1f) { currentProgress = 1f; }

        _weight = Mathf.Lerp(maxWeight, minWeight, currentProgress);

        //Debug.Log("Current Weight: " + Weight.ToString());
    }

    /// <summary>
    /// Given the current weight of the system, we want to move our current state in the
    /// direction of the given ideal move-to location. Note that the idealMoveTo location
    /// may be OUTSIDE the unit circle, but the currentState will never lie outside the unit circle.
    /// </summary>
    /// <param name="idealMoveTo">The ideal position we will move to!</param>
    private void MoveCurrentState(Vector2 idealMoveTo)
    {
        _currentState = Vector2.Lerp(_currentState, idealMoveTo, Weight);
    }

    private Vector2 GetEmoVector(EmotionIdeal[] emotionIdeals, float idealOffset = .5f)
    {
        Vector2 result;
        int count = emotionIdeals.Length;
        float summedX = 0, summedY = 0;//total sum of the X and Y components for each emotionIdeal vector

        Vector2 temp;
        foreach (EmotionIdeal emoIdeal in emotionIdeals)
        {
            temp = GetEmoVector(emoIdeal, idealOffset);
            summedX += temp.x;
            summedY += temp.y;
        }

        summedX /= count;
        summedY /= count;

        //Set result values to the average X/Y values
        result.x = summedX;
        result.y = summedY;

        return result;
    }

    /// <summary>
    /// Returns the vector representation of a given emotional ideal
    /// </summary>
    /// <param name="emotionIdeal">The emotional ideal to get a vec2 for</param>
    /// <param name="idealOffset">How much further the ideal is away from the unit circle. 0.5 by default</param>
    /// <returns>A vector representing the ideal</returns>
    private Vector2 GetEmoVector(EmotionIdeal emotionIdeal, float idealOffset = .5f)
    {
        float inBetweenScalar = (1f + idealOffset) / Mathf.Sqrt(2);
        switch (emotionIdeal)
        {
            case EmotionIdeal.Active:
                return new Vector2(0, 1f + idealOffset);
            case EmotionIdeal.Bored:
                return new Vector2(-inBetweenScalar, -inBetweenScalar);
            case EmotionIdeal.Content:
                return new Vector2(inBetweenScalar, -inBetweenScalar);
            case EmotionIdeal.Deactive:
                return new Vector2(0, -1 - idealOffset);
            case EmotionIdeal.Excited:
                return new Vector2(inBetweenScalar, inBetweenScalar);
            case EmotionIdeal.Panic:
                return new Vector2(-inBetweenScalar, inBetweenScalar);
            case EmotionIdeal.Pleasant:
                return new Vector2(1 + idealOffset, 0);
            case EmotionIdeal.Unpleasant:
                return new Vector2(-1 - idealOffset, 0);
            default:
                throw new System.Exception("Invalid Emotional Vector given for GetEmoVector(" + emotionIdeal.ToString() + ")");
        }
    }

    #endregion
}
