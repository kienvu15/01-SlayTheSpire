using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    public DamagePopup popupPrefab;
    public Canvas worldCanvas; // canvas dạng World Space

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(Vector3 position, int damageAmount, bool isMiss = false)
    {
        DamagePopup popup = Instantiate(popupPrefab, position, Quaternion.identity, worldCanvas.transform);
        popup.Setup(damageAmount, isMiss);
    }
}
