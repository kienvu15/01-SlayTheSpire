using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;   // ✅ Thêm dòng này

public class DIALOGUE : MonoBehaviour
{
    public TextMeshProUGUI dialogueCop;
    public string[] lines;
    public float textSpeed = 0.05f;

    private int index;

    [Header("Object Spawn")]
    public GameObject object01;   // Prefab item sẽ spawn
    public GameObject object02;   // (tuỳ nếu bạn cần spawn thêm sau)
    public Transform spawnPoint;  // Điểm spawn (nếu null thì dùng transform.position)

    private bool hasSpawnedItem = false;
    private bool hasSpawnedSecondItem = false;
    [SerializeField] private bool isDone = false;
    [SerializeField] private RelicManager relicManager;

    public ParticleSystem effect;
    public Animator avatar;
    public Transform targetTransform;
    public GameObject mapIcon;

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
            {
                NextLine();
            }
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

        if(dialogueCop.text == lines[2] && !isDone)
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

    //coroutine set active false after animation
    IEnumerator afterAnimation()
    {
        yield return new WaitForSeconds(3f);
        avatar.gameObject.SetActive(false);
        effect.Stop();
        if (!hasSpawnedSecondItem)
        {
            SpawnAndAnimateItem(object02);
            Item item = object02.GetComponent<Item>();
            item.flyTarget = targetTransform;
            item.mapIcon = mapIcon;
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
        else
        {
            //gameObject.SetActive(false);
        }
    }

    void SpawnAndAnimateItem(GameObject p2refab)
    {
        Vector3 startPos = spawnPoint ? spawnPoint.position : transform.position;
        
        // Tạo item
        GameObject itemObj = Instantiate(p2refab, startPos, Quaternion.identity);

        // Xác định điểm đến (ví dụ, lệch sang trái và xuống dưới)
        Vector3 endPos = startPos + new Vector3(Random.Range(-8f, -6f), -4f - startPos.y, 0f);

        // Xác định điểm điều khiển (control point) cao hơn để tạo đường cong
        Vector3 controlPoint = startPos + new Vector3((endPos.x - startPos.x) / 2f, 3f, 0f);

        // t chạy từ 0 → 1
        float t = 0f;

        // Dùng DOTween để tween giá trị t
        DOTween.To(() => t, x => {
            t = x;
            // Nội suy theo công thức Bezier
            Vector3 pos =
                Mathf.Pow(1 - t, 2) * startPos +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * endPos;

            itemObj.transform.position = pos;
        }, 1f, 1.2f) // thời gian 1.2s
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            // Khi tới điểm đến, đăng ký item vào UI manager
            Item item = itemObj.GetComponent<Item>();
            item.relicManager = relicManager;
            if (item != null && ItemUIManager.HasInstance)
            {
                ItemUIManager.Instance.RegisterItem(item);
            }
        });
    }

}
