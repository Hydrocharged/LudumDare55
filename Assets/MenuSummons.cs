// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuSummons : IMenuController {
    private Creature[] summons;
    private int index;

    public MenuSummons(Creature[] allSummonedCreatures, int currentIndex) {
        summons = allSummonedCreatures;
        index = currentIndex;
    }

    public void Start() {
        var system = SystemScript.System;
        var menu = system.MenuSummons;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        canvas.transform.Find("CreatureName").GetComponent<TextMeshProUGUI>().text = summons[index].Name;
        canvas.transform.Find("Summary").GetComponent<TextMeshProUGUI>().text = summons.Length > 1 ? "Summary" : "Continue";
        canvas.transform.Find("Ability").GetComponent<TextMeshProUGUI>().text = summons[index].AbilityDescription;
        menu.transform.Find("Card").Find("CardSprite").GetComponent<SpriteRenderer>().sprite = Utils.SummonTexture(summons[index].ID);

        var card = menu.transform.Find("Card").gameObject;
        var cardParticles = card.GetComponent<ParticleSystem>();
        var cardParticlesMain = cardParticles.main;
        var cardParticlesEmission = cardParticles.emission;
        switch (summons[index].Star) {
            case 1:
                cardParticlesMain.startColor = Color.white;
                cardParticlesEmission.rateOverTime = 50;
                break;
            case 2:
                cardParticlesMain.startColor = Color.white;
                cardParticlesEmission.rateOverTime = 100;
                break;
            case 3:
                cardParticlesMain.startColor = Color.white;
                cardParticlesEmission.rateOverTime = 200;
                break;
            case 4:
                cardParticlesMain.startColor = new Color(197, 0, 255);
                cardParticlesEmission.rateOverTime = 200;
                break;
            case 5:
                cardParticlesMain.startColor = new Color(255, 123, 0);
                cardParticlesEmission.rateOverTime = 200;
                break;
        }
    }

    public void Update() {
        var system = SystemScript.System;
        while (system.ClickEvents.Count > 0) {
            string clickEvent = system.ClickEvents.Dequeue();
            switch (clickEvent) {
                case "Card":
                    if (summons.Length <= 1) {
                        system.SetController(new MainMenu());
                    } else if (index+1 < summons.Length) {
                        system.SetController(new MenuSummons(summons, index+1));
                    } else {
                        system.SetController(new MenuSummonSummary(summons));
                    }
                    break;
                case "Summary":
                    if (summons.Length <= 1) {
                        system.SetController(new MainMenu());
                    } else {
                        system.SetController(new MenuSummonSummary(summons));
                    }
                    break;
            }
        }
    }

    public void Destroy() {
        SystemScript.System.MenuSummons.SetActive(false);
    }
}
