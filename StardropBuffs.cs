using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;

namespace BetterStardrops;

public class StardropBuffs {

    static int stardropsFound;

    public static void DoBuffs(string id, ModConfig Config, IModHelper helper) {
        stardropsFound = Utility.numStardropsFound(Game1.player);

        if (stardropsFound < 1) {
            ModEntry.SMonitor.Log("\nNo stardrops found, do nothing", ModEntry.DesiredLogLevel);
            return;
        }

        ModEntry.SMonitor.Log($"\nPlayer has found {stardropsFound} Stardrops", ModEntry.DesiredLogLevel);
        if (stardropsFound > 0) {
            Buff buff = new Buff(
            id: $"{id}_StardropBuffs",
            source: $"{id}",
            displaySource: "Better Stardrops",
            displayName: $"Stardrops: {stardropsFound}",
            description: GetDescription(),
            iconTexture: helper.ModContent.Load<Texture2D>("assets/Stardrop.png"),
            duration: Buff.ENDLESS,
            effects: new BuffEffects() {
                Attack = { GetPower(Config.EnableAttack, Config.AttackAmount) },
                Defense = { GetPower(Config.EnableDefense, Config.DefenseAmount) },
                Immunity = { GetPower(Config.EnableImmunity, Config.ImmunityAmount) },
                MaxStamina = { GetPower(Config.EnableStamina, Config.StaminaAmount) },
                CombatLevel = { GetPower(Config.EnableCombat, Config.CombatAmount) },
                FarmingLevel = { GetPower(Config.EnableFarming, Config.FarmingAmount) },
                FishingLevel = { GetPower(Config.EnableFishing, Config.FishingAmount) },
                ForagingLevel = { GetPower(Config.EnableForaging, Config.ForagingAmount) },
                LuckLevel = { GetPower(Config.EnableLuck, Config.LuckAmount) },
                MiningLevel = { GetPower(Config.EnableMining, Config.MiningAmount) },
                MagneticRadius = { GetPower(Config.EnableMagnetic, Config.MagneticAmount) },
                AttackMultiplier = { GetPower(Config.EnableAttackMult, Config.AttackMultAmount) },
                CriticalChanceMultiplier = { GetPower(Config.EnableCriticalChance, Config.CriticalChanceAmount) },
                CriticalPowerMultiplier = { GetPower(Config.EnableCriticalPower, Config.CriticalPowerAmount) },
                KnockbackMultiplier = { GetPower(Config.EnableKnockback, Config.KnockbackAmount) },
                Speed = { GetPower(Config.EnableSpeed, Config.SpeedAmount) },
                WeaponSpeedMultiplier = { GetPower(Config.EnableWeaponSpeed, Config.WeaponSpeedAmount) }
            });

            Game1.player.applyBuff(buff);
            buff.visible = Config.ShowBuff;

            Game1.player.stamina += GetPower(Config.EnableStamina, Config.StaminaAmount);

            RevalidateHealth(Game1.player);
            Game1.player.maxHealth += GetPower(Config.EnableHealth, Config.HealthAmount);
            Game1.player.health = Game1.player.maxHealth;
        }
    }

    static string GetDescription() {
        ModConfig config = ModEntry.Config;
        string finalString = "Oops, something went wrong. You shouldn't be seeing this.";
        string healthAmount = null!;
        string healthRegen = null!;
        string energyRegen = null!;

        if (config.EnableHealth) {
            healthAmount = $"+{GetPower(config.EnableHealth, config.HealthAmount)} Health";
        }
        if (config.EnableHealthRegen) {
            healthRegen = $"+{GetPower(config.EnableHealthRegen, config.HealthRegenAmount)} Health/sec";
        }
        if (config.EnableStaminaRegen) {
            energyRegen = $"+{GetPower(config.EnableStaminaRegen, config.StaminaRegenAmount)} Energy/sec";
        }

        if (healthAmount != null && healthRegen != null && energyRegen != null) {
            finalString = healthAmount + "\n" + healthRegen + "\n" + energyRegen;
        } else if (healthAmount != null && healthRegen != null && energyRegen == null) {
            finalString = healthAmount + "\n" + healthRegen;
        } else if (healthAmount != null && healthRegen == null && energyRegen != null) {
            finalString = healthAmount + "\n" + energyRegen;
        }  else if (healthAmount == null && healthRegen != null && energyRegen != null) {
            finalString = healthRegen + "\n" + energyRegen;
        } else if (healthAmount != null && healthRegen == null && energyRegen == null) {
            finalString = healthAmount;
        } else if (healthAmount == null && healthRegen != null && energyRegen == null) {
            finalString = healthRegen;
        } else if (healthAmount == null && healthRegen == null && energyRegen != null) {
            finalString = energyRegen;
        } else {
            finalString = null!;
        }

        return finalString;
    }

    static float GetPower(bool enabled, float power) {
        return (float)Math.Round(enabled ? power * stardropsFound : 0, 3);
    }

    static int GetPower(bool enabled, int power) {
        return enabled ? power * stardropsFound : 0;
    }

    public static void RevalidateHealth(Farmer farmer) {
        int expected_max_health = 100;
        if (farmer.mailReceived.Contains("qiCave")) {
            expected_max_health += 25;
        }
        for (int i = 1; i <= farmer.GetUnmodifiedSkillLevel(4); i++) {
            if (!farmer.newLevels.Contains(new Point(4, i)) && i != 5 && i != 10) {
                expected_max_health += 5;
            }
        }
        if (farmer.professions.Contains(24)) {
            expected_max_health += 15;
        }
        if (farmer.professions.Contains(27)) {
            expected_max_health += 25;
        }
        if (farmer.maxHealth != expected_max_health) {
            ModEntry.SMonitor.Log("\nMax health not expected value, adjusting.", ModEntry.DesiredLogLevel);
            farmer.maxHealth = expected_max_health;
            farmer.health = farmer.maxHealth;
        }
    }

}