using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    private Animator circleAnimator;
    public Image circleHighlight; 

    public Button goButton;         

    private Room room;
    public GameObject ButtonPar;

    public AudioClip clickSound;
    public AudioSource audioSource;

    void Start()
    {
        circleAnimator = circleHighlight.GetComponent<Animator>();

        room = GetComponentInParent<Room>(); 

        MapUIManager.Instance.RegisterRoomUI(this);

        if (circleHighlight != null)
            circleHighlight.color = Color.clear;

        ButtonPar = GameObject.Find("ButtonPar");
    }

    public void SetAsVisited()
    {
        if (circleHighlight != null)
            circleHighlight.color = Color.green;
        if (goButton != null)
            goButton.gameObject.SetActive(false);
        
    }

    public void OnRoomClicked()
    {
        if (room.incoming.Contains(PlayerMapController.Instance.currentRoom))
        {
            MapUIManager.Instance.SelectRoom(this);

            circleHighlight.color = Color.yellow;
            circleAnimator.SetBool("HightLight", true);
            goButton.gameObject.SetActive(true);
            goButton.transform.SetParent(ButtonPar.transform, false);
        }
    }


    public void OnGoClicked()
    {
        PlayerMapController.Instance.MoveTo(room);

        if (room.type == RoomType.Battle)
        {
            audioSource.PlayOneShot(clickSound);

            MapUIManager.Instance.ShowBattleCanvas();
        }

        if(room.type == RoomType.Shop)
        {
            MapUIManager.Instance.OpenShop();
        }

        if(room.type == RoomType.Event)
        {
            MapUIManager.Instance.ShowEventUI();
        }
    }


    public void SetAsCurrent()
    {
        if (circleHighlight != null)
            circleHighlight.color = Color.green; 
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

    public void ResetUI()
    {
        if (room != null)
        {
            if (room == PlayerMapController.Instance.currentRoom)
            {
                SetAsCurrent();
                return;
            }
            else if (room.visited)
            {
                SetAsVisited();
                circleAnimator.SetBool("HightLight", false);
                return;
            }
        }

        if (circleHighlight != null)
            circleHighlight.color = Color.clear;
        if (goButton != null)
            goButton.gameObject.SetActive(false);
    }

}
