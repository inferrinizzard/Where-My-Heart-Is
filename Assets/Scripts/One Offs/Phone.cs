﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : InteractableObject
{
    public override string prompt { get => "Press E to Interact with Phone"; }
    public override void Interact()
    {
        StartCoroutine(Player.Instance.mask.PreTransition());
    }
}
