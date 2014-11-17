using UnityEngine;
using System.Collections;

public class TestSynonymFinder : MonoBehaviour
{

    public bool runSynonymFinderTest = false;

    // Use this for initialization
    void Start()
    {
        if (!runSynonymFinderTest) { return; }
        Synonym s = new Synonym("brain", DoNothing);
        Synonym s2 = new Synonym("hate", DoNothing);
        Synonym s3 = new Synonym("cry", DoNothing);

        s.PrintWhenReady();
        s2.PrintWhenReady();
        s3.PrintWhenReady();
    }

    Word DoNothing(Word w)
    {
        //do nothing
        return new Word();
    }
}
