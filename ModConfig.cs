namespace BetterStardrops {
    public class ModConfig {

        public bool ShowLogging { get; set; } = false;

        public bool EnableAttack { get; set; } = true;
        public float AttackAmount { get; set; } = 3f;

        public bool EnableDefense { get; set; } = true;
        public float DefenseAmount { get; set; } = 3;

        public bool EnableImmunity { get; set; } = true;
        public float ImmunityAmount { get; set; } = 0.43f;

        public bool EnableHealth { get; set; } = true;
        public int HealthAmount { get; set; } = 14;

        public bool EnableStamina { get; set; } = true;
        public float StaminaAmount { get; set; } = 6;

        public bool EnableCombat { get; set; } = true;
        public float CombatAmount { get; set; } = 0.72f;

        public bool EnableFarming { get; set; } = true;
        public float FarmingAmount { get; set; } = 0.72f;

        public bool EnableFishing { get; set; } = true;
        public float FishingAmount { get; set; } = 0.72f;

        public bool EnableForaging { get; set; } = true;
        public float ForagingAmount { get; set; } = 0.72f;

        public bool EnableLuck { get; set; } = true;
        public float LuckAmount { get; set; } = 0.72f;

        public bool EnableMining { get; set; } = true;
        public float MiningAmount { get; set; } = 0.72f;

        public bool EnableMagnetic { get; set; } = true;
        public float MagneticAmount { get; set; } = 64;

        public bool EnableAttackMult { get; set; } = true;
        public float AttackMultAmount { get; set; } = 0.03f;

        public bool EnableCriticalChance { get; set; } = true;
        public float CriticalChanceAmount { get; set; } = 0.03f;

        public bool EnableCriticalPower { get; set; } = true;
        public float CriticalPowerAmount { get; set; } = 0.03f;

        public bool EnableKnockback { get; set; } = true;
        public float KnockbackAmount { get; set; } = 0.03f;

        public bool EnableSpeed { get; set; } = true;
        public float SpeedAmount { get; set; } = 0.3f;

        public bool EnableWeaponSpeed { get; set; } = true;
        public float WeaponSpeedAmount { get; set; } = 0.03f;

        public bool EnableHealthRegen { get; set; } = true;
        public float HealthRegenAmount { get; set; } = 0.15f;

        public bool EnableStaminaRegen { get; set; } = true;
        public float StaminaRegenAmount { get; set; } = 0.15f;

    }
}
