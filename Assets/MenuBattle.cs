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

public class MenuBattle : IMenuController {
    private Battle battle;
    private int battleOptionIndex;
    private BattleStats stats;
    private BattlingCreature[] battleCreatures;
    private double elapsedSinceLast = 0.0;
    private List<BattleEvent> events;
    private int eventIndex;

    private TextMeshProUGUI totalDamage;
    private TextMeshProUGUI remainingStats;
    private TextMeshProUGUI creatureSlot1Damage;
    private TextMeshProUGUI creatureSlot2Damage;
    private TextMeshProUGUI creatureSlot3Damage;
    private TextMeshProUGUI creatureSlot4Damage;

    public MenuBattle(int battleOptionSelected, BattleStats battleStats) {
        var system = SystemScript.System;
        battleOptionIndex = battleOptionSelected;
        stats = battleStats;
        if (battleOptionSelected >= system.BattleOptions.Length) {
            return;
        }
        battle = system.BattleOptions[battleOptionSelected].Battle;
        battleCreatures = Creature.GetBattleCreatures();
        events = new List<BattleEvent>();
        eventIndex = 0;
    }

    public void Start() {
        var system = SystemScript.System;
        if (battleOptionIndex >= system.BattleOptions.Length) {
            system.Floor += 1;
            system.Gems += (ulong)Math.Ceiling(stats.GemsCollected);
            system.BattleOptions = null;
            system.SetController(new MainMenu());
            return;
        }
        var menu = system.MenuBattle;
        var canvas = menu.transform.Find("Canvas").gameObject;

        menu.SetActive(true);
        totalDamage = canvas.transform.Find("TotalDamage").GetComponent<TextMeshProUGUI>();
        remainingStats = canvas.transform.Find("RemainingStats").GetComponent<TextMeshProUGUI>();
        creatureSlot1Damage = canvas.transform.Find("CreatureSlot1Damage").GetComponent<TextMeshProUGUI>();
        creatureSlot2Damage = canvas.transform.Find("CreatureSlot2Damage").GetComponent<TextMeshProUGUI>();
        creatureSlot3Damage = canvas.transform.Find("CreatureSlot3Damage").GetComponent<TextMeshProUGUI>();
        creatureSlot4Damage = canvas.transform.Find("CreatureSlot4Damage").GetComponent<TextMeshProUGUI>();

        canvas.transform.Find("Floor").GetComponent<TextMeshProUGUI>().text = $"Floor {system.Floor} Battle {battleOptionIndex+1}";
        canvas.transform.Find("Gems").GetComponent<TextMeshProUGUI>().text = (system.Gems != 1) ? $"{system.Gems} Gems" : "1 Gem";
        canvas.transform.Find("CreatureSlot1Element").GetComponent<TextMeshProUGUI>().text = Utils.ToString(battleCreatures[0].Creature.Element);
        canvas.transform.Find("CreatureSlot2Element").GetComponent<TextMeshProUGUI>().text = Utils.ToString(battleCreatures[1].Creature.Element);
        canvas.transform.Find("CreatureSlot3Element").GetComponent<TextMeshProUGUI>().text = Utils.ToString(battleCreatures[2].Creature.Element);
        canvas.transform.Find("CreatureSlot4Element").GetComponent<TextMeshProUGUI>().text = Utils.ToString(battleCreatures[3].Creature.Element);
        canvas.transform.Find("EnemyHP").GetComponent<TextMeshProUGUI>().text = $"HP\n{(ulong)battle.HP}";
        canvas.transform.Find("EnemyElement").GetComponent<TextMeshProUGUI>().text = Utils.ToString(battle.Element);
        menu.transform.Find("CreatureSlot1").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(battleCreatures[0].Creature.ID);
        menu.transform.Find("CreatureSlot2").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(battleCreatures[1].Creature.ID);
        menu.transform.Find("CreatureSlot3").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(battleCreatures[2].Creature.ID);
        menu.transform.Find("CreatureSlot4").GetComponent<SpriteRenderer>().sprite = Utils.MiniTexture(battleCreatures[3].Creature.ID);
        switch (battleOptionIndex) {
            case 0:
                menu.transform.Find("Enemy").GetComponent<SpriteRenderer>().sprite = system.Enemy1;
                break;
            case 1:
                menu.transform.Find("Enemy").GetComponent<SpriteRenderer>().sprite = system.Enemy2;
                break;
            case 2:
                menu.transform.Find("Enemy").GetComponent<SpriteRenderer>().sprite = system.Enemy3;
                break;
        }

        totalDamage.text = "Total Damage\n0";
        remainingStats.text = $"Last Attack\n0\nDamage Dealt Last Round\n{stats.DamageDealtLastRound}\nDamage Dealt This Floor\n{stats.DamageDealtThisFloor}\nGems Collected\n{stats.GemsCollected}";
        creatureSlot1Damage.text = $"{(ulong)battleCreatures[0].Attack}";
        creatureSlot2Damage.text = $"{(ulong)battleCreatures[1].Attack}";
        creatureSlot3Damage.text = $"{(ulong)battleCreatures[2].Attack}";
        creatureSlot4Damage.text = $"{(ulong)battleCreatures[3].Attack}";
        // TODO: set art for party and enemy

        // Simulate the battle and add to the event queue
        var currentPhase = EBattlePhase.PreRound;
        var battleSimulationLoop = true;
        while (battleSimulationLoop) {
            switch (currentPhase) {
                case EBattlePhase.PreRound:
                    for (int battleCreatureIndex = battleCreatures.Length - 1; battleCreatureIndex >= 0; battleCreatureIndex--) {
                        var battleCreature = battleCreatures[battleCreatureIndex];
                        if (battleCreature.EffectTriggers <= 0) {
                            continue;
                        }
                        switch (battleCreature.Creature.ID) {
                            case ECreatureID.ID2A:
                                battleCreature.EffectTriggers--;
                                foreach (var bc in battleCreatures) {
                                    bc.Attack *= 1.0 + (battleCreature.Creature.Rank * 0.03);
                                }
                                events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, EBattleEventType.Buff));
                                break;
                            case ECreatureID.ID3B:
                                battleCreature.EffectTriggers--;
                                var totalGems = stats.GemsCollected + system.Gems;
                                foreach (var bc in battleCreatures) {
                                    bc.Attack += (1.0 + ((battleCreature.Creature.Rank - 1) * 0.4)) * totalGems;
                                }
                                events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, EBattleEventType.Buff));
                                break;
                            case ECreatureID.ID4A:
                                battleCreature.EffectTriggers--;
                                stats.GemsCollected *= 1.9 + (battleCreature.Creature.Rank * 0.1);
                                break;
                        }
                    }
                    currentPhase = EBattlePhase.Turn;
                    break;
                case EBattlePhase.PostRound:
                    for (int battleCreatureIndex = battleCreatures.Length - 1; battleCreatureIndex >= 0; battleCreatureIndex--) {
                        var battleCreature = battleCreatures[battleCreatureIndex];
                        if (battleCreature.EffectTriggers <= 0) {
                            continue;
                        }
                        switch (battleCreature.Creature.ID) {
                            case ECreatureID.ID5B:
                                battleCreature.EffectTriggers--;
                                foreach (var bc in battleCreatures) {
                                    if (bc.Creature.ID == ECreatureID.ID5B) {
                                        continue;
                                    }
                                    bc.EffectTriggers++;
                                    bc.AttackTriggers++;
                                }
                                currentPhase = EBattlePhase.PreRound;
                                goto WhileLoop;
                        }
                    }
                    // Determine whether we won or lost
                    if (Math.Ceiling(stats.DamageDealtThisRound) >= battle.HP) {
                        events.Add(new BattleEvent(stats, battleCreatures, 0, EBattleEventType.Win));
                    } else {
                        events.Add(new BattleEvent(stats, battleCreatures, 0, EBattleEventType.Lose));
                    }
                    battleSimulationLoop = false;
                    break;
                case EBattlePhase.Turn:
                    for (int battleCreatureIndex = battleCreatures.Length - 1; battleCreatureIndex >= 0; battleCreatureIndex--) {
                        var battleCreature = battleCreatures[battleCreatureIndex];
                        // Pre-Turn Phase
                        if (battleCreature.EffectTriggers > 0) {
                            switch (battleCreature.Creature.ID) {
                                case ECreatureID.ID2B:
                                    battleCreature.EffectTriggers--;
                                    if (stats.LastElement == EElement.Fire) {
                                        battleCreature.Attack *= 1.5 + ((battleCreature.Creature.Rank - 1) * 0.3);
                                        events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, EBattleEventType.Buff));
                                    }
                                    break;
                                case ECreatureID.ID3A:
                                    battleCreature.EffectTriggers--;
                                    battleCreature.Attack *= 1.0 + ((battleCreature.Creature.Rank - 1) * 0.5);
                                    events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, EBattleEventType.Buff));
                                    break;
                            }
                        }
                        // Attack Phase
                        var attack = battleCreature.Attack;
                        if (battleCreature.AttackTriggers > 0) {
                            battleCreature.AttackTriggers--;
                            var eventType = EBattleEventType.NormalAttack;
                            if (Utils.HasCombo(battleCreature.Creature.Element, stats.LastElement)) {
                                attack *= 2.0;
                                eventType = EBattleEventType.DoubleAttack;
                            }
                            if (Utils.HasAdvantage(battleCreature.Creature.Element, battle.Element)) {
                                attack *= 2.0;
                                eventType = (eventType == EBattleEventType.DoubleAttack) ? EBattleEventType.QuadAttack : EBattleEventType.DoubleAttack;
                            } else if (Utils.HasDisadvantage(battleCreature.Creature.Element, battle.Element)) {
                                attack /= 2.0;
                                eventType = (eventType == EBattleEventType.DoubleAttack) ? EBattleEventType.NormalAttack : EBattleEventType.HalfAttack;
                            }
                            stats.LastDamage = attack;
                            stats.LastElement = battleCreature.Creature.Element;
                            stats.DamageDealtThisRound += attack;
                            stats.DamageDealtThisFloor += attack;
                            events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, eventType));
                        }
                        // Post-Turn Phase
                        if (battleCreature.EffectTriggers > 0) {
                            switch (battleCreature.Creature.ID) {
                                case ECreatureID.ID1A:
                                    battleCreature.EffectTriggers--;
                                    battleCreature.AttackTriggers++;
                                    battleCreatureIndex++;
                                    break;
                                case ECreatureID.ID4B:
                                    battleCreature.EffectTriggers--;
                                    var additionalDamage4B = stats.DamageDealtLastRound * (0.19 + (battleCreature.Creature.Rank * 0.01));
                                    stats.LastDamage = additionalDamage4B;
                                    stats.DamageDealtThisRound += additionalDamage4B;
                                    stats.DamageDealtThisFloor += additionalDamage4B;
                                    events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, EBattleEventType.NormalAttack));
                                    break;
                                case ECreatureID.ID5A:
                                    battleCreature.EffectTriggers--;
                                    var additionalDamage5A = stats.DamageDealtThisFloor * (0.2 + ((battleCreature.Creature.Rank - 1) * 0.04));
                                    stats.LastDamage = additionalDamage5A;
                                    stats.DamageDealtThisRound += additionalDamage5A;
                                    stats.DamageDealtThisFloor += additionalDamage5A;
                                    events.Add(new BattleEvent(stats, battleCreatures, battleCreatureIndex, EBattleEventType.NormalAttack));
                                    break;
                            }
                        }
                    }
                    currentPhase = EBattlePhase.PostRound;
                    break;
                default:
                    throw new Exception("Unhandled EBattlePhase during simulation");
            }
            WhileLoop:
            continue;
        }
    }

    public void Update() {
        elapsedSinceLast += Time.deltaTime;
        if (elapsedSinceLast >= 0.75) {
            elapsedSinceLast = 0;
            UpdateStep();
        }
    }

    public void Destroy() {
        SystemScript.System.MenuBattle.SetActive(false);
    }

    public void UpdateText(BattleStats stats, BattlingCreature[] battleCreatures) {
        totalDamage.text = $"Total Damage\n{stats.DamageDealtThisRound}";
        remainingStats.text = $"Last Attack\n{stats.LastDamage}\nDamage Dealt Last Round\n{stats.DamageDealtLastRound}\nDamage Dealt This Floor\n{stats.DamageDealtThisFloor}\nGems Collected\n{stats.GemsCollected}";
        creatureSlot1Damage.text = $"{(ulong)battleCreatures[0].Attack}";
        creatureSlot2Damage.text = $"{(ulong)battleCreatures[1].Attack}";
        creatureSlot3Damage.text = $"{(ulong)battleCreatures[2].Attack}";
        creatureSlot4Damage.text = $"{(ulong)battleCreatures[3].Attack}";
    }

    public void UpdateStep() {
        if (eventIndex >= events.Count) {
            throw new Exception("Win/Lose event was never captured");
        }
        var system = SystemScript.System;
        var currentEvent = events[eventIndex];
        eventIndex++;
        switch (currentEvent.EventType) {
            case EBattleEventType.Buff:
                // TODO: do something for buffs
                UpdateText(currentEvent.Stats, currentEvent.Party);
                break;
            case EBattleEventType.NormalAttack:
                // TODO: animation for normal attack
                UpdateText(currentEvent.Stats, currentEvent.Party);
                break;
            case EBattleEventType.DoubleAttack:
                // TODO: animation for double attack
                UpdateText(currentEvent.Stats, currentEvent.Party);
                break;
            case EBattleEventType.QuadAttack:
                // TODO: animation for quad attack
                UpdateText(currentEvent.Stats, currentEvent.Party);
                break;
            case EBattleEventType.HalfAttack:
                // TODO: animation for half attack
                UpdateText(currentEvent.Stats, currentEvent.Party);
                break;
            case EBattleEventType.Win:
                var updatedStats = stats.Copy();
                updatedStats.DamageDealtLastRound = updatedStats.DamageDealtThisRound;
                updatedStats.DamageDealtThisRound = 0;
                updatedStats.LastElement = EElement.None;
                updatedStats.GemsCollected += battle.Gems;
                system.SetController(new MenuBattle(battleOptionIndex+1, updatedStats));
                break;
            case EBattleEventType.Lose:
                system.SetController(new MenuLose());
                break;
            default:
                throw new Exception("Unhandled event encountered during simulation playback");
        }
    }
}

public enum EBattlePhase {
    PreRound,
    PostRound,
    Turn,
}

public enum EBattleEventType {
    Buff,
    NormalAttack,
    DoubleAttack,
    QuadAttack,
    HalfAttack,
    Win,
    Lose
}

public class BattleEvent {
    public BattleStats Stats;
    public BattlingCreature[] Party;
    public int PartyMember;
    public EBattleEventType EventType;

    public BattleEvent(BattleStats stats, BattlingCreature[] party, int partyMember, EBattleEventType eventType) {
        Stats = stats.Copy();
        Party = BattlingCreature.Copy(party);
        PartyMember = partyMember;
        EventType = eventType;
    }
}

public class BattleStats {
    public double GemsCollected = 0.0;
    public double DamageDealtThisRound = 0.0;
    public double DamageDealtLastRound = 0.0;
    public double DamageDealtThisFloor = 0.0;
    public EElement LastElement = EElement.None;
    public double LastDamage = 0.0;

    public BattleStats Copy() {
        var newStats = new BattleStats();
        newStats.GemsCollected = GemsCollected;
        newStats.DamageDealtThisRound = DamageDealtThisRound;
        newStats.DamageDealtLastRound = DamageDealtLastRound;
        newStats.DamageDealtThisFloor = DamageDealtThisFloor;
        newStats.LastElement = LastElement;
        newStats.LastDamage = LastDamage;
        return newStats;
    }
}
