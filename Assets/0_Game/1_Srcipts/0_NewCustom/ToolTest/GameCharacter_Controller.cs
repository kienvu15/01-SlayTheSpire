using UnityEngine;

public class GameCharacter_Controller : MonoBehaviour
{
    Character_Info_UI character_Info_UI;
    Character character;

    void Update_CharacterStat_UI()
    {

        character_Info_UI.Update_InfoUI();
    }
}
