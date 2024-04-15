// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

public enum EElement {
    None,
    Fire,
    Water,
    Shock,
    Wind
}

public enum ECreatureID : int {
    ID1A,
    ID1B,
    ID2A,
    ID2B,
    ID3A,
    ID3B,
    ID4A,
    ID4B,
    ID5A,
    ID5B,
}

public class Creature {
    public ECreatureID ID;
    public string Name;
    public int Level = 1;
    public int Star;
    public int Rank = 1;
    public EElement Element;
    public double BaseAttack;
    public virtual string AbilityDescription { get; }

    private static Creature[][] creaturesByStar = new Creature[][] {
        new Creature[]{ new Creature1A(), new Creature1B() },
        new Creature[]{ new Creature2A(), new Creature2B() },
        new Creature[]{ new Creature3A(), new Creature3B() },
        new Creature[]{ new Creature4A(), new Creature4B() },
        new Creature[]{ new Creature5A(), new Creature5B() },
    };

    public static Creature[][] CreaturesByStar {
        get => creaturesByStar;
    }

    public static BattlingCreature[] GetBattleCreatures() {
        var system = SystemScript.System;
        var battleCreatures = new BattlingCreature[system.Party.Length];
        for (int i = 0; i < system.Party.Length; i++) {
            battleCreatures[i] = new BattlingCreature(system.Party[i]);
        }
        return battleCreatures;
    }
}

public class BattlingCreature {
    public Creature Creature;
    public double Attack;
    public int EffectTriggers = 1;
    public int AttackTriggers = 1;

    public BattlingCreature(Creature creature) {
        Creature = creature;
        Attack = creature.BaseAttack * creature.Rank;
        EffectTriggers = 1;
        AttackTriggers = 1;
    }

    public BattlingCreature Copy() {
        var newCreature = new BattlingCreature(Creature);
        newCreature.Attack = Attack;
        newCreature.EffectTriggers = EffectTriggers;
        newCreature.AttackTriggers = AttackTriggers;
        return newCreature;
    }

    public static BattlingCreature[] Copy(BattlingCreature[] party) {
        var newParty = new BattlingCreature[party.Length];
        for (int i = 0; i < party.Length; i++) {
            newParty[i] = party[i].Copy();
        }
        return newParty;
    }
}

public class Creature1A : Creature {
    public Creature1A() {
        ID = ECreatureID.ID1A;
        Name = "Bee";
        Star = 1;
        Element = EElement.Fire;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => "Attacks twice in a row";
    }
}

public class Creature1B : Creature {
    public Creature1B() {
        ID = ECreatureID.ID1B;
        Name = "Spider";
        Star = 1;
        Element = EElement.Water;
        BaseAttack = 1.5;
    }

    public override string AbilityDescription {
        get => "Deals 50% more base damage";
    }
}

public class Creature2A : Creature {
    public Creature2A() {
        ID = ECreatureID.ID2A;
        Name = "Pig";
        Star = 2;
        Element = EElement.Shock;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Increases the base attack of all creatures by {3*Rank}%";
    }
}

public class Creature2B : Creature {
    public Creature2B() {
        ID = ECreatureID.ID2B;
        Name = "Quail";
        Star = 2;
        Element = EElement.Wind;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Increases base damage by {150 + (Rank - 1) * 30}% when attacking after a Fire creature";
    }
}

public class Creature3A : Creature {
    public Creature3A() {
        ID = ECreatureID.ID3A;
        Name = "Dog";
        Star = 3;
        Element = EElement.Fire;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Increases base attack by {100 + ((Rank - 1) * 50)}% the damage dealt by the last creature";
    }
}

public class Creature3B : Creature {
    public Creature3B() {
        ID = ECreatureID.ID3B;
        Name = "Cat";
        Star = 3;
        Element = EElement.Shock;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Adds {(100 + (Rank - 1) * 40)}% the number of gems to everyone's base attack";
    }
}

public class Creature4A : Creature {
    public Creature4A() {
        ID = ECreatureID.ID4A;
        Name = "Gorilla";
        Star = 4;
        Element = EElement.Wind;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Obtains {90+(Rank*10)}% more gems from battles";
    }
}

public class Creature4B : Creature {
    public Creature4B() {
        ID = ECreatureID.ID4B;
        Name = "Lion";
        Star = 4;
        Element = EElement.Water;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Additionally deals {19+Rank}% of the damage dealt in the last battle";
    }
}

public class Creature5A : Creature {
    public Creature5A() {
        ID = ECreatureID.ID5A;
        Name = "Elephant";
        Star = 5;
        Element = EElement.None;
        BaseAttack = Star;
    }

    public override string AbilityDescription {
        get => $"Additionally deals {20 + (Rank - 1) * 4}% of the damage dealt on the current floor";
    }
}

public class Creature5B : Creature {
    public Creature5B() {
        ID = ECreatureID.ID5B;
        Name = "Whale";
        Star = 5;
        Element = EElement.None;
        BaseAttack = 20;
    }

    public override string AbilityDescription {
        get => $"Retriggers all abilities and attacks of other creatures, excluding {Name}s, and deals 300% more base damage";
    }
}
