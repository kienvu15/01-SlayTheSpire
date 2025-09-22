using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTest : MonoBehaviour
{
    public float refillDelay = 3f;

    List<int> deck = new List<int>() { 1, 2, 3 };
    List<int> discard = new List<int>() { 4, 5, 6, 7, 8 };

    void Start()
    {
        StartCoroutine(DrawHand(5));
    }

    IEnumerator DrawHand(int count)
    {
        int needToDraw = count;

        // --- Rút hết trong deck ---
        while (needToDraw > 0 && deck.Count > 0)
        {
            int card = deck[0];
            deck.RemoveAt(0);
            Debug.Log($"Rút lá {card}, deck còn {deck.Count}");
            needToDraw--;
            yield return new WaitForSeconds(0.5f);
        }

        // --- Nếu chưa đủ thì refill ---
        if (needToDraw > 0)
        {
            yield return StartCoroutine(RefillDeck());

            // --- Rút tiếp ---
            while (needToDraw > 0 && deck.Count > 0)
            {
                int card = deck[0];
                deck.RemoveAt(0);
                Debug.Log($"Rút lá {card}, deck còn {deck.Count}");
                needToDraw--;
                yield return new WaitForSeconds(0.5f);
            }
        }

        Debug.Log("Kết thúc rút");
    }

    IEnumerator RefillDeck()
    {
        Debug.Log($"Deck rỗng, CHỜ {refillDelay} giây để refill...");
        yield return new WaitForSeconds(refillDelay);

        if (discard.Count > 0)
        {
            deck.AddRange(discard);
            discard.Clear();
            Debug.Log($"Refill xong! Deck hiện tại: {deck.Count}");
        }
        else
        {
            Debug.Log("Không có discard để refill.");
        }
    }
}
