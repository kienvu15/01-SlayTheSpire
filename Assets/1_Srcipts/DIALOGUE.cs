using UnityEngine;
using TMPro;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class DIALOGUE : MonoBehaviour
{
    public event Action OnItemFlyCompleted;

    [SerializeField] private RelicManager relicManager;
    public TextMeshProUGUI dialogueCop;
    public string[] lines;
    public float textSpeed = 0.05f;
    public GameObject object01;
    public GameObject object02;
    public Transform spawnPoint;
    public ParticleSystem effect;
    public Animator avatar;
    public Transform targetTransform;
    public GameObject mapIcon;

    private int index;
    private bool hasSpawnedItem;
    private bool hasSpawnedSecondItem;
    private bool isDone;

    void Start()
    {
        dialogueCop.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueCop.text == lines[index])
                NextLine();
            else
            {
                StopAllCoroutines();
                dialogueCop.text = lines[index];
            }
        }

        if (dialogueCop.text == lines[1] && !hasSpawnedItem)
        {
            hasSpawnedItem = true;
            SpawnAndAnimateItem(object01);
        }

        if (dialogueCop.text == lines[2] && !isDone)
        {
            effect.Play();
            StartCoroutine(endDioluge());
        }
    }

    IEnumerator endDioluge()
    {
        yield return new WaitForSeconds(1.4f);
        Image img = GetComponent<Image>();
        isDone = true;
        dialogueCop.gameObject.SetActive(false);
        img.enabled = false;
        avatar.Play("Disappear");
        StartCoroutine(afterAnimation());
    }

    IEnumerator afterAnimation()
    {
        yield return new WaitForSeconds(3f);
        avatar.gameObject.SetActive(false);
        effect.Stop();

        if (!hasSpawnedSecondItem)
        {
            SpawnAndAnimateItem(object02);
            hasSpawnedSecondItem = true;
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueCop.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            dialogueCop.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }

    void SpawnAndAnimateItem(GameObject prefab)
    {
        Vector3 startPos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject itemObj = Instantiate(prefab, startPos, Quaternion.identity);

        Vector3 endPos = startPos + new Vector3(UnityEngine.Random.Range(-8f, -6f), -4f - startPos.y, 0f);
        Vector3 controlPoint = startPos + new Vector3((endPos.x - startPos.x) / 2f, 3f, 0f);
        float t = 0f;

        DOTween.To(() => t, x =>
        {
            t = x;
            Vector3 pos =
                Mathf.Pow(1 - t, 2) * startPos +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * endPos;

            itemObj.transform.position = pos;
        }, 1f, 1.2f)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            Item item = itemObj.GetComponent<Item>();
            item.flyTarget = targetTransform;
            item.mapIcon = mapIcon;
            item.OnFlyCompleted += OnItemFlyDone;
            item.relicManager = relicManager;

            if (item != null && ItemUIManager.HasInstance)
                ItemUIManager.Instance.RegisterItem(item);
        });
    }

    void OnItemFlyDone()
    {
        OnItemFlyCompleted?.Invoke();
    }
}
