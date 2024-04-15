// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuLose : IMenuController {
    public void Start() {
        var system = SystemScript.System;
        var menu = system.MenuLose;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        canvas.transform.Find("Floor").GetComponent<TextMeshProUGUI>().text = $"You made it to\nFloor {system.Floor}";
    }

    public void Update() {
        var system = SystemScript.System;
        while (system.ClickEvents.Count > 0) {
            string clickEvent = system.ClickEvents.Dequeue();
            switch (clickEvent) {
                case "Continue":
                    system.Reset();
                    break;
            }
        }
    }

    public void Destroy() {
        SystemScript.System.MenuLose.SetActive(false);
    }
}
