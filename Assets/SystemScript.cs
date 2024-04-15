// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemScript : MonoBehaviour {
    public static SystemScript System;
    public GameObject MainMenu;
    public GameObject MenuSummons;
    public GameObject MenuSummonsSummary;
    public GameObject MenuBattle;
    public GameObject MenuParty;
    public GameObject MenuCombine;
    public GameObject MenuLose;

    public Sprite SummonBee;
    public Sprite SummonCat;
    public Sprite SummonDog;
    public Sprite SummonElephant;
    public Sprite SummonGorilla;
    public Sprite SummonLion;
    public Sprite SummonPig;
    public Sprite SummonQuail;
    public Sprite SummonSpider;
    public Sprite SummonWhale;

    public Sprite MiniBee;
    public Sprite MiniCat;
    public Sprite MiniDog;
    public Sprite MiniElephant;
    public Sprite MiniGorilla;
    public Sprite MiniLion;
    public Sprite MiniPig;
    public Sprite MiniQuail;
    public Sprite MiniSpider;
    public Sprite MiniWhale;

    public Sprite Enemy1;
    public Sprite Enemy2;
    public Sprite Enemy3;

    [NonSerialized] public IMenuController CurrentMenu;

    [NonSerialized] public ulong Floor;
    [NonSerialized] public ulong Gems;
    [NonSerialized] public ulong FreeSummons;
    [NonSerialized] public Creature[] Party;
    [NonSerialized] public List<Creature> Inventory; // TODO: Need to use/find an equivalent to SortedList/SortedDictionary without the KV part
    [NonSerialized] public BattleOptions[] BattleOptions;
    [NonSerialized] public Queue<string> ClickEvents;

    public SystemScript() {
        Initialize();
    }

    private void Initialize() {
        System = this;
        Floor = 1;
        Gems = 0;
        FreeSummons = 20;
        Party = new Creature[] {
            new Creature1A(),
            new Creature1B(),
            new Creature2A(),
            new Creature2B()
        };
        Inventory = new List<Creature>();
        BattleOptions = null;
        ClickEvents = new Queue<string>();
    }

    void Start() {
        Screen.SetResolution(948, 533, false);
        MainMenu.SetActive(false);
        MenuSummons.SetActive(false);
        MenuSummonsSummary.SetActive(false);
        MenuBattle.SetActive(false);
        MenuParty.SetActive(false);
        MenuCombine.SetActive(false);
        CurrentMenu = new MainMenu();
        CurrentMenu.Start();
    }

    public void Reset() {
        CurrentMenu.Destroy();
        Initialize();
        Start();
    }

    void Update() {
        CurrentMenu.Update();
    }

    public void SetController(IMenuController controller) {
        CurrentMenu.Destroy();
        CurrentMenu = controller;
        CurrentMenu.Start();
    }

    public void SortInventory() {
        // You can get away with bad code in Ludum Dare
        Inventory.Sort(delegate(Creature a, Creature b) {
            // Highest Star, then Highest Rank, then Lowest ID
            if (a.Star > b.Star) {
                return -1;
            }
            if (a.Star < b.Star) {
                return 1;
            }
            if (a.Rank > b.Rank) {
                return -1;
            }
            if (a.Rank < b.Rank) {
                return 1;
            }
            if (a.ID < b.ID) {
                return -1;
            }
            if (a.ID > b.ID) {
                return 1;
            }
            return 0;
        });
    }
}

public class BattleOptions {
    public ulong Selected = 0;
    public Battle[] Battles;

    public BattleOptions(params Battle[] battles) {
        Battles = battles;
    }

    public Battle Battle {
        get => Battles[Selected];
    }

    public void Switch() {
        Selected++;
        if (Selected >= (ulong)Battles.Length) {
            Selected = 0;
        }
    }
}

public class Battle {
    public double HP;
    public ulong Gems;
    public EElement Element;
    // TODO: public EEnemyAbility EnemyAbility;
    public Battle(double hp, ulong gems, EElement element) {
        HP = hp;
        Gems = gems;
        Element = element;
    }
}

public interface IMenuController {
    public void Start();
    public void Destroy();
    public void Update();
}
