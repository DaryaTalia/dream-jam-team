using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCheckpointJourneyScript : MonoBehaviour
{
    public List<float> Checkpoints;

    public float nextCheckpoint;

    public float journeyLength;

    public GameObject gameplayUI;
    public Slider journeySlider;

    public Vector3 checkpointStartingRange;
    public Vector3 checkpointEndingRange;

    public GameObject checkpointSprite;
    public List<GameObject> checkPointInstances;

    // Start is called before the first frame update
    void Start()
    {
        Checkpoints.Add(30f);
        Checkpoints.Add(60f);
        Checkpoints.Add(90f);

        nextCheckpoint = Checkpoints[0];

        journeyLength = Checkpoints[Checkpoints.Count - 1];

        journeySlider.minValue = 0;
        journeySlider.maxValue = journeyLength;

        foreach(float point in Checkpoints)
        {
            float xPosition = journeySlider.transform.position.x + point;
            checkPointInstances.Add(Instantiate(checkpointSprite, gameplayUI.transform, false));
        }

        float sliderLength = journeySlider.gameObject.GetComponent<RectTransform>().rect.width;

        int i = 0;
        foreach(GameObject sprite in checkPointInstances)
        {
            sprite.GetComponent<Image>().color = Color.red;
            sprite.transform.position = new Vector3(((Checkpoints[i] * sliderLength) / journeyLength) + 50, journeySlider.transform.position.y, 0);
            Debug.Log("Checkpoint " + i + " position: " + sprite.transform.position.x);
            ++i;
        }
}

    // Update is called once per frame
    void Update()
    {
        if(Checkpoints.Count > 1 && GameManager.Instance.cargoController.DistanceTraveled >= nextCheckpoint)
        {
            nextCheckpoint = Checkpoints[1];
            checkPointInstances[0].GetComponent<Image>().color = Color.green;
            checkPointInstances.RemoveAt(0);
            Checkpoints.RemoveAt(0);
        } else if (Checkpoints.Count == 1 && GameManager.Instance.cargoController.DistanceTraveled >= nextCheckpoint)
        {
            nextCheckpoint = -1;
            checkPointInstances[0].GetComponent<Image>().color = Color.green;
            checkPointInstances.RemoveAt(0);
            Checkpoints.Clear();
        }

        if(nextCheckpoint != -1)
        {
            journeySlider.value = GameManager.Instance.cargoController.DistanceTraveled;
        }
    }
}
