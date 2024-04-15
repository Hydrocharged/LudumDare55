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

public class MenuCombine : IMenuController {
    private int currentPage;
    private int selectedIndex;
    private List<int> displayedIndexes;

    public MenuCombine(int page, int selectedIndex) {
        currentPage = page;
        this.selectedIndex = selectedIndex;

        var system = SystemScript.System;
        Creature selectedCreature;
        if (selectedIndex < system.Party.Length) {
            selectedCreature = system.Party[selectedIndex];
        } else {
            selectedCreature = system.Inventory[selectedIndex - system.Party.Length];
        }
        displayedIndexes = new List<int>();
        for (int i = 0; i < system.Inventory.Count; i++) {
            if (i == selectedIndex - system.Party.Length) {
                continue;
            }
            if (system.Inventory[i].ID == selectedCreature.ID) {
                displayedIndexes.Add(i+system.Party.Length);
            }
        }
        // Handle the case where we've removed the last visible creature on the page
        if (currentPage > 1 && currentPage > ((displayedIndexes.Count - 1) / 5) + 1) {
            currentPage--;
        }
    }

    public void Start() {
        var system = SystemScript.System;
        var menu = system.MenuCombine;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        int max = Math.Min(currentPage * 5, displayedIndexes.Count);
        int partyItemIndex = 1;
        for (int displayIndex = (currentPage - 1) * 5; displayIndex < max; displayIndex++, partyItemIndex++) {
            int index = displayedIndexes[displayIndex];
            Creature creature;
            if (index < system.Party.Length) {
                creature = system.Party[index];
            } else {
                creature = system.Inventory[index - system.Party.Length];
            }
            menu.transform.Find($"PartyItem{partyItemIndex}").gameObject.SetActive(true);
            var piName = canvas.transform.Find($"PartyItem{partyItemIndex}Name");
            piName.gameObject.SetActive(true);
            piName.GetComponent<TextMeshProUGUI>().text = $"{creature.Star}* {creature.Name}";
            var piRank = canvas.transform.Find($"PartyItem{partyItemIndex}Rank");
            piRank.gameObject.SetActive(true);
            piRank.GetComponent<TextMeshProUGUI>().text = $"R{creature.Rank}";
        }
        // Disable all items that aren't being displayed
        for (; partyItemIndex <= 5; partyItemIndex++) {
            menu.transform.Find($"PartyItem{partyItemIndex}").gameObject.SetActive(false);
            canvas.transform.Find($"PartyItem{partyItemIndex}Name").gameObject.SetActive(false);
            canvas.transform.Find($"PartyItem{partyItemIndex}Rank").gameObject.SetActive(false);
        }
        canvas.transform.Find("Page").GetComponent<TextMeshProUGUI>().text = $"Page\n{currentPage}/{((displayedIndexes.Count - 1) / 5) + 1}";

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
    }

    public void Update() {
        var system = SystemScript.System;
        while (system.ClickEvents.Count > 0) {
            string clickEvent = system.ClickEvents.Dequeue();
            switch (clickEvent) {
                case "PartyItem1":
                    Fuse(0);
                    system.SetController(new MenuCombine(currentPage, selectedIndex));
                    break;
                case "PartyItem2":
                    Fuse(1);
                    system.SetController(new MenuCombine(currentPage, selectedIndex));
                    break;
                case "PartyItem3":
                    Fuse(2);
                    system.SetController(new MenuCombine(currentPage, selectedIndex));
                    break;
                case "PartyItem4":
                    Fuse(3);
                    system.SetController(new MenuCombine(currentPage, selectedIndex));
                    break;
                case "PartyItem5":
                    Fuse(4);
                    system.SetController(new MenuCombine(currentPage, selectedIndex));
                    break;
                case "Back":
                    system.SetController(new MenuParty((selectedIndex / 5) + 1, selectedIndex));
                    break;
                case "FuseAll":
                    FuseAll();
                    system.SetController(new MenuCombine(1, selectedIndex));
                    break;
                case "Prev":
                    if (currentPage <= 1) {
                        continue;
                    }
                    system.SetController(new MenuCombine(currentPage-1, selectedIndex));
                    break;
                case "Next":
                    if (currentPage * 5 >= displayedIndexes.Count) {
                        continue;
                    }
                    system.SetController(new MenuCombine(currentPage+1, selectedIndex));
                    break;
            }
        }
    }

    public void Fuse(int partySlot) {
        var system = SystemScript.System;
        int targetIndex = displayedIndexes[((currentPage - 1) * 5) + partySlot];
        if (selectedIndex < system.Party.Length) {
            system.Party[selectedIndex].Rank += system.Inventory[targetIndex - system.Party.Length].Rank;
            system.Inventory.RemoveAt(targetIndex - system.Party.Length);
        } else {
            system.Inventory[selectedIndex - system.Party.Length].Rank += system.Inventory[targetIndex - system.Party.Length].Rank;
            system.Inventory.RemoveAt(targetIndex - system.Party.Length);
            if (selectedIndex > targetIndex) {
                selectedIndex--;
            }
        }
    }

    public void FuseAll() {
        var system = SystemScript.System;
        for (int i = displayedIndexes.Count - 1; i >= 0; i--) {
            int targetIndex = displayedIndexes[i];
            if (selectedIndex < system.Party.Length) {
                system.Party[selectedIndex].Rank += system.Inventory[targetIndex - system.Party.Length].Rank;
                system.Inventory.RemoveAt(targetIndex - system.Party.Length);
            } else {
                system.Inventory[selectedIndex - system.Party.Length].Rank += system.Inventory[targetIndex - system.Party.Length].Rank;
                system.Inventory.RemoveAt(targetIndex - system.Party.Length);
                if (selectedIndex > targetIndex) {
                    selectedIndex--;
                }
            }
        }
        displayedIndexes.Clear();
    }

    public void Destroy() {
        SystemScript.System.MenuCombine.SetActive(false);
    }
}
