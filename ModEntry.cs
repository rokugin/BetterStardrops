using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Menus;
using StardewModdingAPI.Events;

namespace BetterStardrops {
    public class ModEntry : Mod {

        int stardropsFound;

        public static ModConfig Config = new();
        
        LogLevel DesiredLogLevel => Config.ShowLogging ? LogLevel.Info : LogLevel.Trace;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveCreated += OnSaveCreated;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) {
            SetUpGMCM();
        }

        private void OnSaveCreated(object? sender, SaveCreatedEventArgs e) {
            LevelUpMenu.RevalidateHealth(Game1.player);
            Monitor.Log("\nNew save started, resetting health just in case.", DesiredLogLevel);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            SetUpGMCM();
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e) {
            if (!Config.EnableHealth) return;
            if (stardropsFound < 1) return;

            LevelUpMenu.RevalidateHealth(Game1.player);

            if (stardropsFound > 0) Monitor.Log("\nHealth reset to prepare for next day calculations", DesiredLogLevel);
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e) {
            SetUpGMCM();
            if (Config.ResetMaxHealth) {
                Monitor.Log("\nPerforming requested max health reset.", DesiredLogLevel);
                LevelUpMenu.RevalidateHealth(Game1.player);
                Config.ResetMaxHealth = false;
            }

            stardropsFound = Utility.numStardropsFound(Game1.player);
            if (stardropsFound < 1) {
                Monitor.Log("\nNo stardrops found, do nothing", DesiredLogLevel);
                return;
            }

            Monitor.Log($"\nPlayer has found {stardropsFound} Stardrops", DesiredLogLevel);
            if (stardropsFound > 0) {
                Buff buff = new Buff(
                id: $"{ModManifest.UniqueID}_StardropBuffs",
                source: $"{ModManifest.UniqueID}",
                displaySource: $"{stardropsFound} Stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    Attack = { Config.EnableAttack ? Config.AttackIncreaseAmount * stardropsFound : 0 },
                    Defense = { Config.EnableDefense ? Config.DefenseIncreaseAmount * stardropsFound : 0 },
                    Immunity = { Config.EnableImmunity ? Config.ImmunityIncreaseAmount * stardropsFound : 0 },
                    MaxStamina = { Config.EnableStamina ? Config.MaxStaminaIncreaseAmount * stardropsFound : 0 },
                    CombatLevel = { Config.EnableCombatLevel ? Config.CombatLevelIncreaseAmount * stardropsFound : 0 },
                    FarmingLevel = { Config.EnableFarmingLevel ? Config.FarmingLevelIncreaseAmount * stardropsFound : 0 },
                    FishingLevel = { Config.EnableFishingLevel ? Config.FishingLevelIncreaseAmount * stardropsFound : 0 },
                    ForagingLevel = { Config.EnableForagingLevel ? Config.ForagingLevelIncreaseAmount * stardropsFound : 0 },
                    LuckLevel = { Config.EnableLuckLevel ? Config.LuckLevelIncreaseAmount * stardropsFound : 0 },
                    MiningLevel = { Config.EnableMiningLevel ? Config.MiningLevelIncreaseAmount * stardropsFound : 0 },
                    MagneticRadius = { Config.EnableMagnetic ? Config.MagneticRadiusIncreaseAmount * stardropsFound : 0 },
                });
                
                Game1.player.applyBuff(buff);
                buff.visible = Config.ShowLogging;

                Game1.player.stamina = Game1.player.MaxStamina;
                Game1.player.maxHealth += Config.EnableHealth ? Config.HealthIncreaseAmount * stardropsFound : 0;
                Game1.player.health = Game1.player.maxHealth;
            }
        }

        void SetUpGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Unregister(ModManifest);
            
            configMenu.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, true);
            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("attack-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableAttack,
                setValue: value => Config.EnableAttack = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.AttackIncreaseAmount,
                setValue: value => Config.AttackIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("attack-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("defense-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableDefense,
                setValue: value => Config.EnableDefense = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.DefenseIncreaseAmount,
                setValue: value => Config.DefenseIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("defense-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("immunity-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableImmunity,
                setValue: value => Config.EnableImmunity = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.ImmunityIncreaseAmount,
                setValue: value => Config.ImmunityIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("immunity-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("health-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableHealth,
                setValue: value => Config.EnableHealth = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.HealthIncreaseAmount,
                setValue: value => Config.HealthIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("health-buff.tooltip"),
                min: 0,
                max: null
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ResetMaxHealth,
                setValue: value => Config.ResetMaxHealth = value,
                name: () => Helper.Translation.Get("health-reset.label"),
                tooltip: () => Helper.Translation.Get("health-reset.tooltip")
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("stamina-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableStamina,
                setValue: value => Config.EnableStamina = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MaxStaminaIncreaseAmount,
                setValue: value => Config.MaxStaminaIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("stamina-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("magnetic-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableMagnetic,
                setValue: value => Config.EnableMagnetic = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MagneticRadiusIncreaseAmount,
                setValue: value => Config.MagneticRadiusIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("magnetic-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("combat-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableCombatLevel,
                setValue: value => Config.EnableCombatLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.CombatLevelIncreaseAmount,
                setValue: value => Config.CombatLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("combat-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("farming-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableFarmingLevel,
                setValue: value => Config.EnableFarmingLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.FarmingLevelIncreaseAmount,
                setValue: value => Config.FarmingLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("farming-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("fishing-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableFishingLevel,
                setValue: value => Config.EnableFishingLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.FishingLevelIncreaseAmount,
                setValue: value => Config.FishingLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("fishing-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("foraging-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableForagingLevel,
                setValue: value => Config.EnableForagingLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.ForagingLevelIncreaseAmount,
                setValue: value => Config.ForagingLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("foraging-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("luck-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableLuckLevel,
                setValue: value => Config.EnableLuckLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.LuckLevelIncreaseAmount,
                setValue: value => Config.LuckLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("luck-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("mining-options.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableMiningLevel,
                setValue: value => Config.EnableMiningLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MiningLevelIncreaseAmount,
                setValue: value => Config.MiningLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("mining-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, false);
            configMenu.AddSectionTitle(ModManifest, () => Helper.Translation.Get("debugging.label"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ShowLogging,
                setValue: value => Config.ShowLogging = value,
                name: () => Helper.Translation.Get("smapi.label"),
                tooltip: () => Helper.Translation.Get("smapi.tooltip")
            );
            if (Game1.activeClickableMenu is not TitleMenu) {
                configMenu.AddParagraph(ModManifest, () => Helper.Translation.Get("more-settings.text"));
            }
        }
    }
}
