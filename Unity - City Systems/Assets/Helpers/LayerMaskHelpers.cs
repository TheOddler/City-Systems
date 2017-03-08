﻿using UnityEngine;

public static class LayerMaskHelpers
{
    public static bool Contains(this LayerMask layermask, int layer)
    {
        return layermask == (layermask | (1 << layer));
    }

    public static bool IsEmpty(this LayerMask layermask)
    {
        return layermask.value == 0;
    }
}