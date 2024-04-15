// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : IMenuController {
    public void Start() {
        var system = SystemScript.System;
        var menu = system.MainMenu;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        canvas.transform.Find("Floor").GetComponent<TextMeshProUGUI>().text = $"Floor {system.Floor}";
        canvas.transform.Find("Gems").GetComponent<TextMeshProUGUI>().text = (system.Gems != 1) ? $"{system.Gems} Gems" : "1 Gem";
        if (system.FreeSummons >= 10) {
            var message = $"{system.FreeSummons} Free Summons";
            canvas.transform.Find("10SummonDesc").GetComponent<TextMeshProUGUI>().text = message;
            canvas.transform.Find("1SummonDesc").GetComponent<TextMeshProUGUI>().text = message;
        } else if (system.FreeSummons > 0) {
            canvas.transform.Find("10SummonDesc").GetComponent<TextMeshProUGUI>().text = $"{(10 - system.FreeSummons) * 10} Gems - Guaranteed 3*+";
            canvas.transform.Find("1SummonDesc").GetComponent<TextMeshProUGUI>().text = $"{system.FreeSummons} Free Summon{(system.FreeSummons != 1 ? "s" : "")}";
        } else {
            canvas.transform.Find("10SummonDesc").GetComponent<TextMeshProUGUI>().text = "100 Gems - Guaranteed 3*+";
            canvas.transform.Find("1SummonDesc").GetComponent<TextMeshProUGUI>().text = "10 Gems";
        }
        if (system.BattleOptions == null || system.BattleOptions.Length == 0) {
            system.BattleOptions = new BattleOptions[] {
                new BattleOptions(
                    new Battle((double)system.Floor * 10.0, 15, Utils.RandomElement()),
                    new Battle((double)system.Floor * 30.0, 50, Utils.RandomElement())
                ),
                new BattleOptions(
                    new Battle((double)system.Floor * 20.0, 30, Utils.RandomElement()),
                    new Battle((double)system.Floor * 60.0, 100, Utils.RandomElement())
                ),
                new BattleOptions(new Battle((double)system.Floor * 40.0, 50, EElement.None)),
            };
            for (int i = 0; i < system.BattleOptions.Length; i++) {
                SetBattleOptionsText(canvas, i);
            }
        }
        menu.transform.Find("CreatureSlot1").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[0].ID);
        menu.transform.Find("CreatureSlot2").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[1].ID);
        menu.transform.Find("CreatureSlot3").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[2].ID);
        menu.transform.Find("CreatureSlot4").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[3].ID);
    }

    public void SetBattleOptionsText(int index) {
        SetBattleOptionsText(SystemScript.System.MainMenu.transform.Find("Canvas").gameObject, index);
    }

    public void SetBattleOptionsText(GameObject canvas, int index) {
        var system = SystemScript.System;
        var battle = system.BattleOptions[index].Battle;
        if (index+1 < system.BattleOptions.Length) {
            canvas.transform.Find($"Battle{index+1}Text").GetComponent<TextMeshProUGUI>().text = $"HP\n{battle.HP}\n\nElement\n{Utils.ToString(battle.Element)}\n\nGems\n{battle.Gems}";
        } else {
            var temporaryAbility = "Defeat to move to the next floor";
            canvas.transform.Find($"Battle{index+1}Text").GetComponent<TextMeshProUGUI>().text = $"HP\n{battle.HP}\n\nGems\n{battle.Gems}\n\n{temporaryAbility}";
        }
    }

    public void Update() {
        var system = SystemScript.System;
        while (system.ClickEvents.Count > 0) {
            string clickEvent = system.ClickEvents.Dequeue();
            switch (clickEvent) {
                case "Battle1":
                    system.BattleOptions[0].Switch();
                    SetBattleOptionsText(0);
                    break;
                case "Battle2":
                    system.BattleOptions[1].Switch();
                    SetBattleOptionsText(1);
                    break;
                case "Battle3":
                    system.SetController(new MenuBattle(0, new BattleStats()));
                    break;
                case "Party":
                    system.SetController(new MenuParty(1, 0));
                    break;
                case "10Summon":
                    if (system.FreeSummons + (system.Gems / 10) < 10) {
                        continue;
                    }
                    if (system.FreeSummons >= 10) {
                        system.FreeSummons -= 10;
                    } else {
                        ulong payment = 10 - system.FreeSummons;
                        system.FreeSummons = 0;
                        system.Gems -= payment * 10;
                    }
                    var creatures = Utils.RandomCreatureMultiple(10);
                    system.Inventory.AddRange(creatures);
                    system.SortInventory();
                    system.SetController(new MenuSummons(creatures, 0));
                    break;
                case "1Summon":
                    if (system.FreeSummons == 0 && system.Gems < 10) {
                        continue;
                    }
                    if (system.FreeSummons >= 1) {
                        system.FreeSummons--;
                    } else {
                        system.Gems -= 10;
                    }
                    var creature = Utils.RandomCreature();
                    system.Inventory.Add(creature);
                    system.SortInventory();
                    system.SetController(new MenuSummons(new Creature[] {creature}, 0));
                    break;
            }
        }
    }

    public void Destroy() {
        SystemScript.System.MainMenu.SetActive(false);
    }
}
