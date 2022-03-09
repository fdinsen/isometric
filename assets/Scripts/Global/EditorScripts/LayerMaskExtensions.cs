using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool IsInLayerMasks(this GameObject gameObject, int layerMasks)
    {
        //A layermask is a bitmask, which is just a 4 byte/32 bit integer, with each bit representing one of the 32 layers
        //1 is equal to 00000000 00000000 00000000 00000001

        // << is a bitshift, it shifts the bits to the left by the amount on the right
        // so if gameObject.layer has layer 8, it'll shift 1 to the left 8 times
        // becomes 00000000 00000000 00000001 00000000

        // | is the OR operator, it compares 2 ints bit for bit, and each bit is 1 if one or both of the bits in either int is 1
        // so 00000000 00000000 00000000 10001000
        //  | 00000000 00000000 00000000 00001001
        //  = 00000000 00000000 00000000 10001001

        //What we're doing here is we merge the bitmask of the current layer with the layermask which contains all the selected layers using OR |
        //then we compare the result with the original layermask, if it has changed, the gameObject layer was NOT in the layermask
        return (layerMasks == (layerMasks | (1 << gameObject.layer)));
    }
}
