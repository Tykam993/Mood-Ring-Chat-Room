using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawEmotionModel : MonoBehaviour
{
    //Should we run the test???
    public bool runDrawEmotionModel = false;

    //public Vector3 circleScale = new Vector3(1, 1, 1);
    private static List<GameObject> _ideals;

    //private const int defButtonWidth = 100, defButtonHeight = 40, defHeightOffset = 40, defWidthStart=8, defHeightStart=12;
    //private Rect _button1 = new Rect(Screen.width / defWidthStart, Screen.height / defHeightStart, defButtonWidth, defButtonHeight);
    //private Rect _button2 = new Rect(Screen.width / defWidthStart, (Screen.height / defHeightStart) + defHeightOffset, defButtonWidth, defButtonHeight);
    //private Rect _button3 = new Rect(Screen.width / defWidthStart, (Screen.height / defHeightStart) + (defHeightOffset*2), defButtonWidth, defButtonHeight);
    //private Rect _button4 = new Rect(Screen.width / defWidthStart, (Screen.height / defHeightStart) + (defHeightOffset*3), defButtonWidth, defButtonHeight);

    [SerializeField]
    private float _circleScale = 2f;
    public float CircleScale
    {
        get { return Mathf.Abs(_circleScale); }
    }

    [SerializeField]
    private float _idealOffset = .5f;
    public float IdealOffset
    {
        get { return Mathf.Abs(_idealOffset); }
    }

    private GameObject _emoCircle;
    public GameObject EmotionCircle
    {
        get
        {
            if (_emoCircle == null)
            {
                _emoCircle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _emoCircle.renderer.material.shader = Shader.Find("Transparent/Diffuse");
                Color newCol = _emoCircle.renderer.material.color;
                newCol.a = .45f;
                _emoCircle.renderer.material.color = newCol;
            }
            return _emoCircle;
        }
        set
        {
            _emoCircle = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!runDrawEmotionModel) { return; }

        EmotionCircle.transform.localScale = new Vector3(1, 1, 1);
        CreateSurroundingPoints();
    }

    void Update()
    {
        if (!runDrawEmotionModel) { return; }
        Vector3 newScale = new Vector3(CircleScale, CircleScale, CircleScale);
        EmotionCircle.transform.localScale = newScale;

        UpdateSurroundingPoints();

        DrawCurrentState();
    }


    void DrawCurrentState()
    {
        Debug.DrawLine(EmotionCircle.transform.position,
            EmotionModel.CurrentState * (CircleScale/2f), Color.green, 0);
    }


    void CreateSurroundingPoints()
    {
        float radius = CircleScale / 2f;
        float diagonal = (radius + IdealOffset) / Mathf.Sqrt(2);

        if (_ideals == null)
        {
            _ideals = new List<GameObject>();

            //Create 8 surrounding squares
            GameObject square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(-radius - IdealOffset, 0, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(radius + IdealOffset, 0, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(0, radius + IdealOffset, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(0, -radius - IdealOffset, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(diagonal, diagonal, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(-diagonal, -diagonal, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(diagonal, -diagonal, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);

            square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.transform.position = new Vector3(-diagonal, diagonal, 0);
            square.AddComponent<EmotionIdeal>();
            _ideals.Add(square);
        }
    }

    void UpdateSurroundingPoints()
    {
        float radius = CircleScale / 2f;

        foreach (GameObject obj in _ideals)
        {
            obj.transform.localScale = new Vector3(.5f, .5f, .5f);

            Vector3 newPos = Vector3.zero;
            Vector3 objPos = obj.transform.position;

            if (objPos.x < 0)
            {
                newPos = objPos;
                newPos.x = -radius - IdealOffset;
            }
            else if (objPos.x > 0)
            {
                newPos = objPos;
                newPos.x = radius + IdealOffset;
            }

            if (objPos.y < 0)
            {
                newPos = objPos;
                newPos.y = -radius - IdealOffset;
            }
            else if (objPos.y > 0)
            {
                newPos = objPos;
                newPos.y = radius + IdealOffset;
            }

            if (objPos.x < 0 && objPos.y < 0)
            {
                newPos = objPos;
                newPos.x = -(radius + IdealOffset) / Mathf.Sqrt(2);
                newPos.y = -(radius + IdealOffset) / Mathf.Sqrt(2);
            }
            else if (objPos.x > 0 && objPos.y > 0)
            {
                newPos = objPos;
                newPos.x = (radius + IdealOffset) / Mathf.Sqrt(2);
                newPos.y = (radius + IdealOffset) / Mathf.Sqrt(2);
            }
            else if (objPos.x > 0 && objPos.y < 0)
            {
                newPos = objPos;
                newPos.x = (radius + IdealOffset) / Mathf.Sqrt(2);
                newPos.y = -(radius + IdealOffset) / Mathf.Sqrt(2);
            }
            else if (objPos.x < 0 && objPos.y > 0)
            {
                newPos = objPos;
                newPos.x = -(radius + IdealOffset) / Mathf.Sqrt(2);
                newPos.y = (radius + IdealOffset) / Mathf.Sqrt(2);
            }

            obj.transform.position = newPos;
        }
    }


    //void OnGUI()
    //{
    //    if (GUI.Button(_button1, "")) { }
    //}
}
