// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils {
    private static double[] summonRates = new[] { 0.01, 0.1, 0.19, 0.3, 0.4 };

    public static string ToString(EElement e) {
        switch (e) {
            case EElement.None:
                return "Void";
            case EElement.Fire:
                return "Fire";
            case EElement.Water:
                return "Water";
            case EElement.Shock:
                return "Shock";
            case EElement.Wind:
                return "Wind";
            default:
                throw new Exception("Unhandled EElement enum to string");
        }
    }

    public static bool HasAdvantage(EElement attackType, EElement defenseType) {
        switch (attackType) {
            case EElement.None:
                return false;
            case EElement.Fire:
                return defenseType == EElement.Wind;
            case EElement.Water:
                return defenseType == EElement.Shock;
            case EElement.Shock:
                return defenseType == EElement.Water;
            case EElement.Wind:
                return defenseType == EElement.Fire;
            default:
                throw new Exception("Unhandled EElement for advantage checking");
        }
    }

    public static bool HasCombo(EElement currentElement, EElement lastElement) {
        // Combos are just advantage checks for now
        return HasAdvantage(currentElement, lastElement);
    }

    public static bool HasDisadvantage(EElement attackType, EElement defenseType) {
        if (attackType == EElement.None || defenseType == EElement.None) {
            return false;
        }
        return attackType == defenseType;
    }

    public static Creature RandomCreature() {
        double val = (double)Random.Range(0.0f, 1.0f);
        double total = 0.0;
        for (int i = 0; i < summonRates.Length; i++) {
            total += summonRates[i];
            if (val <= total) {
                var starIndex = (summonRates.Length - 1) - i;
                var creaturesByStar = Creature.CreaturesByStar;
                if (starIndex >= creaturesByStar.Length) {
                    Debug.Log("Added additional stars?");
                    return new Creature1A();
                }
                var starArray = creaturesByStar[starIndex];
                var creature = starArray[Random.Range(0, starArray.Length)];
                return (Creature)Activator.CreateInstance(creature.GetType());
            }
        }
        // Woohoo for floating-point accumulation errors...or I can't add
        Debug.Log("Summon rates do not add up to 100%");
        return new Creature1A();
    }

    public static Creature[] RandomCreatureMultiple(int count) {
        var creatures = new Creature[count];
        bool guaranteed3Star = false;
        for (int i = 0; i < creatures.Length; i++) {
            creatures[i] = RandomCreature();
            if (creatures[i].Star >= 3) {
                guaranteed3Star = true;
            }
        }
        if (!guaranteed3Star) {
            Creature creature3Star;
            while (true) {
                creature3Star = RandomCreature();
                if (creature3Star.Star >= 3) {
                    break;
                }
            }
            // Insert into a random position for better "dramatics"
            creatures[Random.Range(0, creatures.Length)] = creature3Star;
        }
        return creatures;
    }

    public static EElement RandomElement() {
        switch (Random.Range(0, 5)) {
            case 0:
                return EElement.None;
            case 1:
                return EElement.Fire;
            case 2:
                return EElement.Water;
            case 3:
                return EElement.Shock;
            case 4:
                return EElement.Wind;
            default:
                return EElement.None;
        }
    }

    public static Sprite SummonTexture(ECreatureID creature) {
        var system = SystemScript.System;
        switch (creature) {
            case ECreatureID.ID1A:
                return system.SummonBee;
            case ECreatureID.ID1B:
                return system.SummonSpider;
            case ECreatureID.ID2A:
                return system.SummonPig;
            case ECreatureID.ID2B:
                return system.SummonQuail;
            case ECreatureID.ID3A:
                return system.SummonDog;
            case ECreatureID.ID3B:
                return system.SummonCat;
            case ECreatureID.ID4A:
                return system.SummonGorilla;
            case ECreatureID.ID4B:
                return system.SummonLion;
            case ECreatureID.ID5A:
                return system.SummonElephant;
            case ECreatureID.ID5B:
                return system.SummonWhale;
            default:
                return system.SummonBee;
        }
    }

    public static Sprite MiniTexture(ECreatureID creature) {
        var system = SystemScript.System;
        switch (creature) {
            case ECreatureID.ID1A:
                return system.MiniBee;
            case ECreatureID.ID1B:
                return system.MiniSpider;
            case ECreatureID.ID2A:
                return system.MiniPig;
            case ECreatureID.ID2B:
                return system.MiniQuail;
            case ECreatureID.ID3A:
                return system.MiniDog;
            case ECreatureID.ID3B:
                return system.MiniCat;
            case ECreatureID.ID4A:
                return system.MiniGorilla;
            case ECreatureID.ID4B:
                return system.MiniLion;
            case ECreatureID.ID5A:
                return system.MiniElephant;
            case ECreatureID.ID5B:
                return system.MiniWhale;
            default:
                return system.MiniBee;
        }
    }
}
