using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PhaseController : MonoBehaviour
{
    protected int levelPhase;
    public int goldEarned;
    public int kills;

    [SerializeField] protected GameObject[] phases;

    [SerializeField] private TextMeshProUGUI killcount;
    [SerializeField] private TextMeshProUGUI goldcount;
    [SerializeField] private TextMeshProUGUI phasecount;
    [SerializeField] private GameObject cScreen;

    [HideInInspector] public bool completed;
    protected DialogueDisplayer displayer;

    public virtual void Awake()
    {
        displayer = FindObjectOfType<DialogueDisplayer>();
        GameObject.Find("Buttons").SetActive(GameMaster.GM.progress.enabledButtons);
    }

    public virtual void DialogueAction(int dId, int sId)
    {

    }
    protected void SetFinishLevel(int ph)
    {
        GameMaster.enabledMovement = false;
        goldcount.text = $"Получено золота: {goldEarned}";
        GameMaster.GM.progress.gold += goldEarned;
        goldEarned = 0;
        killcount.text = $"Побеждено врагов: {kills}";
        phasecount.text = $"Фаза: {GameMaster.GM.progress.levelDatas[ph].phase} из {GameMaster.GM.progress.levelDatas[ph].maxPhase}";
        GameMaster.stagnate = true;
        cScreen.SetActive(true);
    }
}
