using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace BetterStardrops;

public class ModEntry : Mod {

    public static ModConfig Config = new();
    public static IMonitor SMonitor = null!;

    public static LogLevel DesiredLogLevel => Config.ShowLogging ? LogLevel.Info : LogLevel.Trace;

    bool doHealthRegen;
    bool doStaminaRegen;
    float healthRecoveryTick = 0;
    float staminaRecoveryTick = 0;
    int stardropsFound => Utility.numStardropsFound(Game1.player);

    public override void Entry(IModHelper helper) {
        SMonitor = Monitor;
        Config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
        if (!Context.IsPlayerFree) return;

        if (doHealthRegen) {
            if (e.IsOneSecond) {
                healthRecoveryTick += Config.HealthRegenAmount * stardropsFound;
            }

            if (healthRecoveryTick >= 1) {
                int healthRecoveryAmount = (int)healthRecoveryTick;
                healthRecoveryTick -= (int)healthRecoveryTick;
                if (Game1.player.health < Game1.player.maxHealth) {
                    Game1.player.health += healthRecoveryAmount;
                }
            }
        }

        if (doStaminaRegen) {
            if (e.IsOneSecond) {
                staminaRecoveryTick += Config.StaminaRegenAmount * stardropsFound;
            }

            if (staminaRecoveryTick >= 1) {
                int staminaRecoveryAmount = (int)staminaRecoveryTick;
                staminaRecoveryTick -= (int)staminaRecoveryTick;
                if (Game1.player.stamina < Game1.player.MaxStamina) {
                    Game1.player.stamina += staminaRecoveryAmount;
                }
            }
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
        SetUpGMCM();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e) {
        doHealthRegen = Config.EnableHealthRegen;
        doStaminaRegen = Config.EnableStaminaRegen;
        StardropBuffs.DoBuffs(ModManifest.UniqueID, Config, Helper);
    }

    void SetUpGMCM() {
        var CM = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (CM is null) return;

        CM.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

        CM.AddParagraph(ModManifest, () => I18n.Info_Text());

        CM.AddPageLink(ModManifest, "General", () => I18n.GeneralPage_Label());
        CM.AddPageLink(ModManifest, "Combat", () => I18n.CombatPage_Label());
        CM.AddPageLink(ModManifest, "Skills", () => I18n.SkillsPage_Label());

        CM.AddParagraph(ModManifest, () => " ");

        CM.AddSectionTitle(ModManifest, () => I18n.ShowBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.ShowBuff, v => Config.ShowBuff = v, () => I18n.Enabled_Label(), () => I18n.ShowBuff_Tooltip());

        CM.AddParagraph(ModManifest, () => " ");

        CM.AddSectionTitle(ModManifest, () => I18n.Debugging_Label());
        CM.AddBoolOption(ModManifest, () => Config.ShowLogging, v => Config.ShowLogging = v,
            () => I18n.EnableLogging_Label(), () => I18n.EnableLogging_Tooltip());

        // General Section
        CM.AddPage(ModManifest, "General", () => I18n.GeneralPage_Label());

        CM.AddSectionTitle(ModManifest, () => I18n.HealthBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableHealth, v => Config.EnableHealth = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.HealthAmount, v => Config.HealthAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.HealthBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.HealthRegen_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableHealthRegen, v => Config.EnableHealthRegen = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.HealthRegenAmount, v => Config.HealthRegenAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.HealthRegen_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.StaminaBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableStamina, v => Config.EnableStamina = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.StaminaAmount, v => Config.StaminaAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.StaminaBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.StaminaRegen_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableStaminaRegen, v => Config.EnableStaminaRegen = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.StaminaRegenAmount, v => Config.StaminaRegenAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.StaminaRegen_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.MagneticBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableMagnetic, v => Config.EnableMagnetic = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.MagneticAmount, v => Config.MagneticAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.MagneticBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.SpeedBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableSpeed, v => Config.EnableSpeed = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.SpeedAmount, v => Config.SpeedAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.SpeedBuff_Tooltip(), 0, null);

        // Combat Section
        CM.AddPage(ModManifest, "Combat", () => I18n.CombatPage_Label());

        CM.AddSectionTitle(ModManifest, () => I18n.AttackBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableAttack, v => Config.EnableAttack = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.AttackAmount, v => Config.AttackAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.AttackBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.AttackMultiplierBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableAttackMult, v => Config.EnableAttackMult = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.AttackMultAmount, v => Config.AttackMultAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.AttackMultiplierBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.CriticalChanceMultiplierBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableCriticalChance, v => Config.EnableCriticalChance = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.CriticalChanceAmount, v => Config.CriticalChanceAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.CriticalChanceMultiplierBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.CriticalPowerMultiplierBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableCriticalPower, v => Config.EnableCriticalPower = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.CriticalPowerAmount, v => Config.CriticalPowerAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.CriticalPowerMultiplierBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.KnockbackMultiplierBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableKnockback, v => Config.EnableKnockback = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.KnockbackAmount, v => Config.KnockbackAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.KnockbackMultiplierBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.WeaponSpeedMultiplierBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableWeaponSpeed, v => Config.EnableWeaponSpeed = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.WeaponSpeedAmount, v => Config.WeaponSpeedAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.WeaponSpeedMultiplierBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.DefenseBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableDefense, v => Config.EnableDefense = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.DefenseAmount, v => Config.DefenseAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.DefenseBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.ImmunityBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableImmunity, v => Config.EnableImmunity = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.ImmunityAmount, v => Config.ImmunityAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.ImmunityBuff_Tooltip(), 0, null);

        // Skill Section
        CM.AddPage(ModManifest, "Skills", () => I18n.SkillsPage_Label());
        CM.AddParagraph(ModManifest, () => I18n.SkillsInfo_Text());

        CM.AddSectionTitle(ModManifest, () => I18n.CombatBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableCombat, v => Config.EnableCombat = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.CombatAmount, v => Config.CombatAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.CombatBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.FarmingBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableFarming, v => Config.EnableFarming = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.FarmingAmount, v => Config.FarmingAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.FarmingBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.FishingBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableFishing, v => Config.EnableFishing = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.FishingAmount, v => Config.FishingAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.FishingBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.ForagingBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableForaging, v => Config.EnableForaging = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.ForagingAmount, v => Config.ForagingAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.ForagingBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.LuckBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableLuck, v => Config.EnableLuck = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.LuckAmount, v => Config.LuckAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.LuckBuff_Tooltip(), 0, null);

        CM.AddSectionTitle(ModManifest, () => I18n.MiningBuff_Label());
        CM.AddBoolOption(ModManifest, () => Config.EnableMining, v => Config.EnableMining = v, () => I18n.Enabled_Label());
        CM.AddNumberOption(ModManifest, () => Config.MiningAmount, v => Config.MiningAmount = v,
            () => I18n.BuffPower_Label(), () => I18n.MiningBuff_Tooltip(), 0, null);
    }
}