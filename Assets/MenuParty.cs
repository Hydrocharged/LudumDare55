// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuParty : IMenuController {
    private int currentPage;
    private int selectedIndex;

    public MenuParty(int page, int selectedIndex) {
        currentPage = page;
        this.selectedIndex = selectedIndex;
    }

    public void Start() {
        var system = SystemScript.System;
        var menu = system.MenuParty;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        // TODO: set all of the party art
        int max = Math.Min(currentPage * 5, system.Inventory.Count + system.Party.Length);
        int partyItemIndex = 1;
        for (int index = (currentPage - 1) * 5; index < max; index++, partyItemIndex++) {
            Creature creature;
            if (index < system.Party.Length) {
                creature = system.Party[index];
            } else {
                creature = system.Inventory[index - system.Party.Length];
            }
            menu.transform.Find($"PartyItem{partyItemIndex}").gameObject.SetActive(true);
            canvas.transform.Find($"PartyItem{partyItemIndex}Slot").gameObject.SetActive(index == partyItemIndex-1 && index < system.Party.Length);
            var piName = canvas.transform.Find($"PartyItem{partyItemIndex}Name");
            piName.gameObject.SetActive(true);
            piName.GetComponent<TextMeshProUGUI>().text = $"{creature.Star}* {creature.Name}";
            var piRank = canvas.transform.Find($"PartyItem{partyItemIndex}Rank");
            piRank.gameObject.SetActive(true);
            piRank.GetComponent<TextMeshProUGUI>().text = $"R{creature.Rank}";
            canvas.transform.Find($"PartyItem{partyItemIndex}Selected").gameObject.SetActive(index == selectedIndex);
        }
        // Disable all items that aren't being displayed
        for (; partyItemIndex <= 5; partyItemIndex++) {
            menu.transform.Find($"PartyItem{partyItemIndex}").gameObject.SetActive(false);
            canvas.transform.Find($"PartyItem{partyItemIndex}Slot").gameObject.SetActive(false);
            canvas.transform.Find($"PartyItem{partyItemIndex}Name").gameObject.SetActive(false);
            canvas.transform.Find($"PartyItem{partyItemIndex}Rank").gameObject.SetActive(false);
            canvas.transform.Find($"PartyItem{partyItemIndex}Selected").gameObject.SetActive(false);
        }
        canvas.transform.Find("Page").GetComponent<TextMeshProUGUI>().text = $"Page\n{currentPage}/{((system.Inventory.Count + (system.Party.Length - 1)) / 5) + 1}";

        Creature selectedCreature;
        if (selectedIndex < system.Party.Length) {
            selectedCreature = system.Party[selectedIndex];
        } else {
            selectedCreature = system.Inventory[selectedIndex - system.Party.Length];
        }
        canvas.transform.Find("SelectedName").GetComponent<TextMeshProUGUI>().text = $"{selectedCreature.Name}";
        canvas.transform.Find("SelectedRank").GetComponent<TextMeshProUGUI>().text = $"Rank {selectedCreature.Rank}";
        canvas.transform.Find("SelectedElement").GetComponent<TextMeshProUGUI>().text = $"{Utils.ToString(selectedCreature.Element)} Element";
        canvas.transform.Find("SelectedAbility").GetComponent<TextMeshProUGUI>().text = selectedCreature.AbilityDescription;
        menu.transform.Find("SelectedImage").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(selectedCreature.ID);
        menu.transform.Find("CreatureSlotA").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[0].ID);
        menu.transform.Find("CreatureSlotB").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[1].ID);
        menu.transform.Find("CreatureSlotC").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[2].ID);
        menu.transform.Find("CreatureSlotD").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(system.Party[3].ID);
    }

    public void Update() {
        var system = SystemScript.System;
        while (system.ClickEvents.Count > 0) {
            string clickEvent = system.ClickEvents.Dequeue();
            switch (clickEvent) {
                case "CreatureSlotA":
                    AddToParty(0);
                    system.SetController(new MenuParty(currentPage, selectedIndex));
                    break;
                case "CreatureSlotB":
                    AddToParty(1);
                    system.SetController(new MenuParty(currentPage, selectedIndex));
                    break;
                case "CreatureSlotC":
                    AddToParty(2);
                    system.SetController(new MenuParty(currentPage, selectedIndex));
                    break;
                case "CreatureSlotD":
                    AddToParty(3);
                    system.SetController(new MenuParty(currentPage, selectedIndex));
                    break;
                case "PartyItem1":
                    system.SetController(new MenuParty(currentPage, (currentPage - 1) * 5));
                    break;
                case "PartyItem2":
                    system.SetController(new MenuParty(currentPage, ((currentPage - 1) * 5) + 1));
                    break;
                case "PartyItem3":
                    system.SetController(new MenuParty(currentPage, ((currentPage - 1) * 5) + 2));
                    break;
                case "PartyItem4":
                    system.SetController(new MenuParty(currentPage, ((currentPage - 1) * 5) + 3));
                    break;
                case "PartyItem5":
                    system.SetController(new MenuParty(currentPage, ((currentPage - 1) * 5) + 4));
                    break;
                case "Back":
                    system.SetController(new MainMenu());
                    break;
                case "Prev":
                    if (currentPage <= 1) {
                        continue;
                    }
                    system.SetController(new MenuParty(currentPage-1, selectedIndex));
                    break;
                case "Next":
                    if (currentPage * 5 >= system.Inventory.Count + system.Party.Length) {
                        continue;
                    }
                    system.SetController(new MenuParty(currentPage+1, selectedIndex));
                    break;
                case "Combine":
                    system.SetController(new MenuCombine(1, selectedIndex));
                    break;
            }
        }
    }

    public void AddToParty(int partySlot) {
        var system = SystemScript.System;
        if (selectedIndex < system.Party.Length) {
            (system.Party[partySlot], system.Party[selectedIndex]) = (system.Party[selectedIndex], system.Party[partySlot]);
        } else {
            var inventoryIndex = selectedIndex - system.Party.Length;
            (system.Party[partySlot], system.Inventory[inventoryIndex]) = (system.Inventory[inventoryIndex], system.Party[partySlot]);
            system.SortInventory();
        }
        selectedIndex = partySlot;
    }

    public void Destroy() {
        SystemScript.System.MenuParty.SetActive(false);
    }
}
