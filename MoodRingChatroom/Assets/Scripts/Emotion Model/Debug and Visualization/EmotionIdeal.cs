using UnityEngine;
using System.Collections;

public class EmotionIdeal : MonoBehaviour
{

    private static DrawEmotionModel _drawEmotionModel;

    Color defColor = Color.white;

    void Start()
    {
        if (_drawEmotionModel == null)
        {
            _drawEmotionModel = GameObject.Find("DebugObject").GetComponent<DrawEmotionModel>();
        }
    }

    //Add to our current model!
    void OnMouseDown()
    {
        Vector3 vectorToAdd = gameObject.transform.position.normalized;
        vectorToAdd *= (_drawEmotionModel.CircleScale + _drawEmotionModel.IdealOffset);

        EmotionModel.ChangeStateByAddingVector(vectorToAdd);
    }

    void OnMouseOver()
    {
        SetColor(Color.red);
    }

    void OnMouseExit()
    {
        SetColor(defColor);
    }

    void SetColor(Color col)
    {
        gameObject.renderer.material.color = col;
    }
}
