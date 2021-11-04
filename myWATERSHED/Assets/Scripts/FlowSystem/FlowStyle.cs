using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This abstract class is only defining the methods that we want to be able to call on different flow styles

public abstract class FlowStyle : MonoBehaviour
{
    public abstract bool CanFlow(GameObject senderTile, GameObject receiverTile);
    public abstract void Flow(GameObject senderTile, GameObject receiverTile);
}
