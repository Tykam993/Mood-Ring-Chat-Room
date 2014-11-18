using UnityEngine;
using System.Collections;

/// <summary>
/// The purpose of this class is to cross-reference a string with a list of strings that represent "ideal emotions."
/// Once it finds a reference, it will *add* that emotion ideal to our model. If a word has no emotion ideal then
/// it has no impact on the model!
/// </summary>
public class ProcessWordEmotions : MonoBehaviour {

    public static EmotionModel.EmotionIdeal ProcessWordEmotionsAndAddToModel(string word)
    {
        EmotionModel.EmotionIdeal result = Emotions.GetEmotionIdealAssociated(word);

        return result;
    }
}
