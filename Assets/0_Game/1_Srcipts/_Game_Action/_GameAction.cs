using System.Collections;

public abstract class GameAction
{
    public abstract IEnumerator Execute(ActionQueue queue);
}