using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BetterStardrops {
    internal class ModEntry : Mod {
        int attackIncreaseAmount => Config.AttackIncreaseAmount;
        int newAttackIncreaseAmount;

        int defenseIncreaseAmount => Config.DefenseIncreaseAmount;
        int newDefenseIncreaseAmount;

        int immunityIncreaseAmount => Config.ImmunityIncreaseAmount;
        int newImmunityIncreaseAmount;

        int healthIncreaseAmount => Config.HealthIncreaseAmount;
        int newHealthIncreaseAmount;

        int staminaIncreaseAmount => Config.StaminaIncreaseAmount;
        int newStaminaIncreaseAmount;

        int combatLevelIncreaseAmount => Config.CombatLevelIncreaseAmount;
        int newCombatLevelIncreaseAmount;

        int farminglevelIncreaseAmount => Config.FarmingLevelIncreaseAmount;
        int newFarmingLevelIncreaseAmount;

        int fishingLevelIncreaseAmount => Config.FishingLevelIncreaseAmount;
        int newFishingLevelIncreaseAmount;

        int foragingLevelIncreaseAmount => Config.ForagingLevelIncreaseAmount;
        int newForagingLevelIncreaseAmount;

        int luckLevelIncreaseAmount => Config.LuckLevelIncreaseAmount;
        int newLuckLevelIncreaseAmount;

        int miningLevelIncreaseAmount => Config.MiningLevelIncreaseAmount;
        int newMiningLevelIncreaseAmount;

        int magneticIncreaseAmount => Config.MagneticIncreaseAmount;
        int newMagneticIncreaseAmount;

        int stardropsFound;

        static ModConfig Config = new();
        BuffMaker buffMaker = new BuffMaker();

        static LogLevel DesiredLogLevel => Config.ShowLogging ? LogLevel.Debug : LogLevel.Trace;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveCreated += OnSaveCreated;
        }

        private void OnSaveCreated(object? sender, StardewModdingAPI.Events.SaveCreatedEventArgs e) {
            LevelUpMenu.RevalidateHealth(Game1.player);
            Monitor.Log("\nNew save started, resetting health just in case.", DesiredLogLevel);
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            SetUpGMCM();
        }

        private void OnDayEnding(object? sender, StardewModdingAPI.Events.DayEndingEventArgs e) {
            if (!Config.EnableHealth) return;
            if (stardropsFound < 1) return;

            Game1.player.maxHealth = Game1.player.maxHealth - newHealthIncreaseAmount;

            if (stardropsFound > 0) Monitor.Log("\nReducing max health to prepare for next day calculations", DesiredLogLevel);
        }

        private void OnDayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
            if (Config.ResetMaxHealth) {
                Monitor.Log("\nPerforming requested max health reset.", DesiredLogLevel);
                LevelUpMenu.RevalidateHealth(Game1.player);
                Config.ResetMaxHealth = false;
            }

            //SetUpInts();
            stardropsFound = Utility.numStardropsFound(Game1.player);
            if (stardropsFound < 1) {
                Monitor.Log("\nNo stardrops found, do nothing", DesiredLogLevel);
                return;
            }

            Monitor.Log($"\nPlayer has found {stardropsFound} Stardrops", DesiredLogLevel);

            if (stardropsFound > 0) {
                List<Buff> buffsToApply = new();

                if (Config.EnableAttack) {
                    newAttackIncreaseAmount = attackIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateAttackBuff(newAttackIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nAttack buff: +{newAttackIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableDefense) {
                    newDefenseIncreaseAmount = defenseIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateDefenseBuff(newDefenseIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nDefense buff: +{newDefenseIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableImmunity) {
                    newImmunityIncreaseAmount = immunityIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateImmunityBuff(newImmunityIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nImmunity buff: +{newImmunityIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableHealth) {
                    newHealthIncreaseAmount = healthIncreaseAmount * stardropsFound;
                    Game1.player.maxHealth += newHealthIncreaseAmount;
                    Game1.player.health = Game1.player.maxHealth;
                    Monitor.Log($"\nHealth buff: +{newHealthIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableStamina) {
                    newStaminaIncreaseAmount = staminaIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateStaminaBuff(newStaminaIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nStamina buff: +{newStaminaIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableCombatLevel) {
                    newCombatLevelIncreaseAmount = combatLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateCombatLevelBuff(newCombatLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nCombat level buff: +{newCombatLevelIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableFarmingLevel) {
                    newFarmingLevelIncreaseAmount = farminglevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateFarmingLevelBuff(newFarmingLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nFarming level buff: +{newFarmingLevelIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableFishingLevel) {
                    newFishingLevelIncreaseAmount = fishingLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateFishingLevelBuff(newFishingLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nFishing level buff: +{newFishingLevelIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableForagingLevel) {
                    newForagingLevelIncreaseAmount = foragingLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateForagingLevelBuff(newForagingLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nForaging level buff: +{newForagingLevelIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableLuckLevel) {
                    newLuckLevelIncreaseAmount = luckLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateLuckLevelBuff(newLuckLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nLuck level buff: +{newLuckLevelIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableMiningLevel) {
                    newMiningLevelIncreaseAmount = miningLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateMiningLevelBuff(newMiningLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nMining level buff: +{newMiningLevelIncreaseAmount}", DesiredLogLevel);
                }
                if (Config.EnableMagnetic) {
                    newMagneticIncreaseAmount = magneticIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateMagneticBuff(newMagneticIncreaseAmount);
                    buffsToApply.Add(buff);
                    Monitor.Log($"\nMagnetic radius buff: +{newMagneticIncreaseAmount}", DesiredLogLevel);
                }

                foreach (Buff buffToApply in buffsToApply) {
                    Game1.player.applyBuff(buffToApply);
                    buffToApply.visible = false;
                }
                Game1.player.stamina = Game1.player.MaxStamina;
            }
        }

        void SetUpGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.SetTitleScreenOnlyForNextOptions(mod: this.ModManifest, true);
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("attack-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("defense-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("immunity-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("health-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("stamina-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableStamina,
                setValue: value => Config.EnableStamina = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.StaminaIncreaseAmount,
                setValue: value => Config.StaminaIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("stamina-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("magnetic-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableMagnetic,
                setValue: value => Config.EnableMagnetic = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MagneticIncreaseAmount,
                setValue: value => Config.MagneticIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("magnetic-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("combat-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("farming-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("fishing-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("foraging-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("luck-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("mining-options.label")
            );
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

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("debugging.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ShowLogging,
                setValue: value => Config.ShowLogging = value,
                name: () => Helper.Translation.Get("smapi.label"),
                tooltip: () => Helper.Translation.Get("smapi.tooltip")
            );
        }
    }
}
