using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameBrain : MonoBehaviour
{
    /*Interfaces are not implemented as I haven't found an instance where they could have been useful.
     Interfaces are useful when there is are several interconnected mega classes in a game where functionality is repetitive.
    Since I cannot define implementation, I'll avoid using it. "(technically it is possible since C# 8)"*/

    [Header("Frame Settings")]
    public GameObject prefabFrame;
    public int frameNum;
    public float bounceTime = 0.25f;
    public float bounceHeight = 10;
    [SerializeField]
    public string correctTile;
    [SerializeField]
    int correctTileNum;
    [Space(2)]

    [Header("Frame Items")]
    public List<Sprite> frameIter = new List<Sprite>();
    HashSet<int> randomSet= new HashSet<int>();
    List<int> randomValues = new List<int>();

    [Header("Canvas Misc")]
    public Text findText;
    public GameObject winScreen;
    public Image loadScreen;
    public GameObject mainCanvas;
    [SerializeField]
    int firstCorrect;
    [SerializeField]
    int secodCorrect;
    GameObject[] gb;
    bool winGame;

    void Start()
    {
        winGame = false;
        frameNum = 3;
        StartCoroutine(Fade(findText.gameObject,0.05f,true));
        StartCoroutine(TileManager());
        firstCorrect=-1;
        secodCorrect=-1;
    }
    public void Restart()
    {
        IEnumerator RestartEnum()
        {
            //The white load screen from the task.
            loadScreen.gameObject.SetActive(true);
            Color loadAplha = loadScreen.color;
            StartCoroutine(Fade(loadScreen.gameObject, 0.02f, true));
            winScreen.SetActive(false);
            //Reset the varuables to their initial state.
            frameNum = 3;
            randomValues.Clear();
            randomSet.Clear();
            firstCorrect = -1;
            secodCorrect = -1;
            yield return new WaitForSeconds(2);
            //Fade out the load screen
            StopCoroutine("Fade");
            StartCoroutine(Fade(loadScreen.gameObject, 0.02f, false));
            yield return new WaitForSeconds(2);
            findText.gameObject.SetActive(true);
            winGame = false;
            StopCoroutine(TileManager());
            StartCoroutine(TileManager());
        }
        StartCoroutine(RestartEnum());
    }
    IEnumerator CorrectTile()
    {
        //If the correct tile is selected restart the TileManager with more frames(boxes/blocks) so that each time the number is increased by 3.
        yield return new WaitForSeconds(2f);
        print("Correct");
        randomValues.Clear();
        randomSet.Clear();
        foreach (Object obj in gb)
        {
            if (obj != null)
                Destroy(obj);
        }
        frameNum += 3;
        winGame = frameNum == 9;
        StopCoroutine(TileManager());
        StartCoroutine(TileManager());
    }
    public void TileClick(int setTile)
    {
        gb[setTile].transform.Find("Background/Symbol").DOJump(gb[setTile].transform.position, 5, 2, 2);
        print("Clicked Tile: "+setTile);
        //First condition if the correct tile is selected but it is not yet the final level. 
        if (setTile==correctTileNum&&!winGame)
        {
            gb[setTile].transform.Find("Particles").gameObject.SetActive(true);
            StartCoroutine(CorrectTile());
        }
        //Second condition if the correct tile is selected and it is the final level. 
        else if (setTile == correctTileNum && winGame)
        {
            StartCoroutine(GameWin());
            findText.gameObject.SetActive(false);
            gb[setTile].transform.Find("Particles").gameObject.SetActive(true);
        }
        //In any other case, we've selected the incorrect tile.
        else
        {
            gb[setTile].transform.Find("Background/Symbol").DOShakePosition(2.0f, strength: Vector3.one, vibrato: 10, randomness: 2, snapping: false, fadeOut: true);
            print("Incorrect");
        }
    }
    IEnumerator GameWin()
    {
        //Gamewin with the win screen exact to the task.
        yield return new WaitForSeconds(1.5f);
        DestroyObject();
        winScreen.SetActive(true);
        Transform winAlpha=winScreen.transform.Find("Background");
        StartCoroutine(Fade(winAlpha.gameObject, 0.02f, true,0.7f));
    }
    void DestroyObject()
    {
        //Objects are destroyed one after another. I thought it looked cool, so I haven't implemented a coroutine. 
        for (int i = 0; i < frameNum; i++)
        {
            Destroy(gb[i],i*0.2f);
        }
    }
    IEnumerator Fade(GameObject fadeObject,float stepTime,bool fadeIn,float inLim=1,float outLim=0)
    {
        Color fObjectColor= new Color();
        float fadeAlpha;
        if (fadeObject.GetComponent<Image>() != null)
        {
            fObjectColor = fadeObject.GetComponent<Image>().color;
        }
        else if (fadeObject.GetComponent<Text>() != null)
        {
            fObjectColor = fadeObject.GetComponent<Text>().color;
        }
        fadeAlpha = fObjectColor.a;
        if (fadeIn)
        {
            while (fObjectColor.a < inLim)
            {
                fadeAlpha += 0.03f;
                fObjectColor.a = fadeAlpha;
                if (fadeObject.GetComponent<Image>() != null)
                {
                    fadeObject.GetComponent<Image>().color = fObjectColor;
                }
                else if (fadeObject.GetComponent<Text>() != null)
                {
                    fadeObject.GetComponent<Text>().color = fObjectColor;
                }
                yield return new WaitForSeconds(stepTime);
            }
        }
        else
        {
            while (fObjectColor.a > outLim)
            {
                fadeAlpha -= 0.03f;
                fObjectColor.a = fadeAlpha;
                if (fadeObject.GetComponent<Image>() != null)
                {
                    fadeObject.GetComponent<Image>().color = fObjectColor;
                }
                else if (fadeObject.GetComponent<Text>() != null)
                {
                    fadeObject.GetComponent<Text>().color = fObjectColor;
                }
                yield return new WaitForSeconds(stepTime);
            }
        }
    }
    IEnumerator TileManager()
    {
        //Hide loadscreen if it is not hidden.
        loadScreen.gameObject.SetActive(false);
        //Add random numbers in a range from 0 to 35 to the set.
        //We're using a set to ensure that we'll have unique values.
        for (int i = 0; i < 15; i++)
        {
            randomSet.Add(Random.Range(0, 36));
        }
        //Convert the set to a list so that we may implement through indexation
        randomValues = new List<int>(randomSet);
        gb = new GameObject[frameNum];
        //position of tile generation.
        Vector3 posHolder = mainCanvas.transform.position+Vector3.left*16 + Vector3.up * 10;
        float xPos = posHolder.x;
        //Randomly chosen correct tile. 
        correctTileNum = Random.Range(0, frameNum);
        //Checks if the value is unique, should have done with a list find and remove. 
        //while (firstCorrect == correctTileNum||secodCorrect==correctTileNum)
        //{
        //    if (correctTileNum <= frameNum && correctTileNum>=0)
        //    {
        //        correctTileNum += Random.Range(-1, 1);
        //    }
        //}
        //Debug and implementation of the correct tile.
        correctTile = frameIter[randomValues[correctTileNum]].name;
        print("Correct Tile: " + correctTile);
        findText.text = $"FIND {correctTile.ToUpper()}";
        print("Correct Tile Number: " + correctTileNum + 1);
        //Selections to filter out the numbers that have occured in the previous trials.
        if (firstCorrect == -1)
        {
            firstCorrect = correctTileNum;
        }
        else
        {
            secodCorrect = correctTileNum;
        }
        //Setter for loop
        for (int i = 0; i < frameNum; i++)
        {
            //Instantiate the blocks with numbers and give random colors to frames.
            gb[i] = Instantiate(prefabFrame, posHolder, prefabFrame.transform.rotation, mainCanvas.transform);
            Transform backColor = gb[i].transform.Find("Background");
            Transform symbolHolder = gb[i].transform.Find("Background/Symbol");
            symbolHolder.GetComponent<Image>().sprite = frameIter[randomValues[i]];
            backColor.GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //Shift of position of each new block generated.
            posHolder += Vector3.right * 18;
            //Each third block falls on a new line
            if ((i + 1) % 3 == 0)
            {
                posHolder = new Vector3(xPos, posHolder.y, posHolder.z);
                posHolder += Vector3.down * 18;
            }
            //This one has been a disaster. >:( The scope of the lambda function is not covering the iteration of the loop so that I have to
            //define a separate variable so that the listener takes the values of the iteration and creates uniqe button arguments
            //this step is neccessary do define the functionality of each implemented button. 
            int j = i;
            gb[i].GetComponent<Button>().onClick.AddListener(() => { TileClick(j); });
            if (frameNum == 3)
            {
                gb[i].transform.DOPunchScale(Vector3.one / 2, 1, 1, 0);
            }
            //gb[i].transform.DOJump(gb[i].transform.position + Vector3.up * 10, 5, 1, 1);
            gb[i].GetComponent<Button>().interactable = false;
            //This ensures that the blocks appear simultaneously. It looks really ugly, but it was one of the conditions of the task.  
            if (frameNum == 3)
            {
                yield return new WaitForSeconds(.5f);
            }
            else
            {
                yield return null;
            }
        }
        //After all the buttons appear, they'll become interactable.
        for (int i = 0; i < frameNum; i++)
        {
            gb[i].GetComponent<Button>().interactable = true;
        }
    }
}