// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuSummonSummary : IMenuController {
    private Creature[] summons;

    public MenuSummonSummary(Creature[] allSummonedCreatures) {
        summons = allSummonedCreatures;
    }

    public void Start() {
        var system = SystemScript.System;
        var menu = system.MenuSummonsSummary;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        menu.transform.Find("Card1").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[0].ID);
        menu.transform.Find("Card2").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[1].ID);
        menu.transform.Find("Card3").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[2].ID);
        menu.transform.Find("Card4").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[3].ID);
        menu.transform.Find("Card5").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[4].ID);
        menu.transform.Find("Card6").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[5].ID);
        menu.transform.Find("Card7").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[6].ID);
        menu.transform.Find("Card8").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[7].ID);
        menu.transform.Find("Card9").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[8].ID);
        menu.transform.Find("Card10").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[9].ID);
    }

    public void Update() {
        var system = SystemScript.System;
        while (system.ClickEvents.Count > 0) {
            string clickEvent = system.ClickEvents.Dequeue();
            switch (clickEvent) {
                case "Continue":
                    system.SetController(new MainMenu());
                    break;
            }
        }
    }

    public void Destroy() {
        SystemScript.System.MenuSummonsSummary.SetActive(false);
    }
}
