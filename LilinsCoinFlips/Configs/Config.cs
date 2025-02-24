using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace LilinsCoinFlips.Configs
{
    public class Config : IConfig
    {
        [Description("Whether or not the plugin should be enabled. Default: true")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug logs should be shown. Default: false")]
        public bool Debug { get; set; } = false;

        [Description("Sets the coin cooldown.")]
        public double CoinCooldown { get; set; } = 5;

        public List<int> MinMaxDefaultCoins { get; set; } = new()
        {
            1,
            4
        };

        public int DefaultCoinsAmount { get; set; } = 4;

        public Dictionary<ItemType, int> ItemToReplace { get; set; } = new()
        {
            { ItemType.SCP500, 2 }
        };

        public HashSet<EffectType> BadEffects { get; set; } = new()
        {
            EffectType.Asphyxiated,
            EffectType.Bleeding,
            EffectType.Blinded,
            EffectType.Burned,
            EffectType.Concussed,
            EffectType.Corroding,
            EffectType.CardiacArrest,
            EffectType.Deafened,
            EffectType.Decontaminating,
            EffectType.Disabled,
            EffectType.Ensnared,
            EffectType.Exhausted,
            EffectType.Flashed,
            EffectType.Hemorrhage,
            EffectType.Hypothermia,
            EffectType.InsufficientLighting,
            EffectType.Poisoned,
            EffectType.PocketCorroding,
            EffectType.SeveredHands,
            EffectType.SinkHole,
            EffectType.Stained,
            EffectType.Traumatized
        };

        public HashSet<EffectType> GoodEffects { get; set; } = new()
        {
            EffectType.BodyshotReduction,
            EffectType.DamageReduction,
            EffectType.Invigorated,
            EffectType.Invisible,
            EffectType.MovementBoost,
            EffectType.RainbowTaste,
            EffectType.Scp1853,
            EffectType.Scp207,
            EffectType.Vitality
        };

        public HashSet<RoomType> RoomsToTeleport { get; set; } = new()
        {
            RoomType.EzCafeteria,
            RoomType.EzCheckpointHallwayA,
            RoomType.EzCheckpointHallwayB,
            RoomType.EzConference,
            RoomType.EzCrossing,
            RoomType.EzCurve,
            RoomType.EzDownstairsPcs,
            RoomType.EzGateA,
            RoomType.EzGateB,
            RoomType.EzIntercom,
            RoomType.EzPcs,
            RoomType.EzStraight,
            RoomType.EzTCross,
            RoomType.EzUpstairsPcs,
            RoomType.EzVent,
            RoomType.Hcz049,
            RoomType.Hcz079,
            RoomType.Hcz096,
            RoomType.Hcz106,
            RoomType.Hcz939,
            RoomType.HczArmory,
            RoomType.HczCrossing,
            RoomType.HczCurve,
            RoomType.HczElevatorA,
            RoomType.HczElevatorB,
            RoomType.HczEzCheckpointA,
            RoomType.HczEzCheckpointB,
            RoomType.HczHid,
            RoomType.HczNuke,
            RoomType.HczStraight,
            RoomType.HczTesla,
            RoomType.HczTestRoom,
            RoomType.Lcz173,
            RoomType.Lcz330,
            RoomType.Lcz914,
            RoomType.LczAirlock,
            RoomType.LczArmory,
            RoomType.LczCafe,
            RoomType.LczCheckpointA,
            RoomType.LczCheckpointB,
            RoomType.LczClassDSpawn,
            RoomType.LczCrossing,
            RoomType.LczCurve,
            RoomType.LczGlassBox,
            RoomType.LczPlants,
            RoomType.LczStraight,
            RoomType.LczTCross,
            RoomType.LczToilets,
            RoomType.Surface,
        };

        [Description("List of ignored roles for the PlayerSwap effect")]
        public HashSet<RoleTypeId> PlayerSwapIgnoredRoles { get; set; } = new()
        {
            RoleTypeId.Spectator,
            RoleTypeId.Filmmaker,
            RoleTypeId.Overwatch,
            RoleTypeId.Scp079,
            RoleTypeId.Tutorial,
        };

        [Description("List of ignored roles for the InventorySwap effect (#17)")]
        public HashSet<RoleTypeId> InventorySwapIgnoredRoles { get; set; } = new()
        {
            RoleTypeId.Spectator,
            RoleTypeId.Filmmaker,
            RoleTypeId.Overwatch,
            RoleTypeId.Scp079,
            RoleTypeId.Tutorial,
            RoleTypeId.Scp049,
            RoleTypeId.Scp079,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp0492,
            RoleTypeId.Scp939,
            RoleTypeId.Scp3114,
        };

        public HashSet<RoleTypeId> JSIgnoredRoles { get; set; } = new()
        {
            RoleTypeId.Spectator,
            RoleTypeId.Filmmaker,
            RoleTypeId.Overwatch,
        };

        public HashSet<uint> CustomItemIDs { get; set; } = new()
        {
            102,
            101,
            200,
            201,
            202,
            106,
            107,
            105,
            300,
            301,
            800
        };

        public HashSet<ItemType> ItemsToGive { get; set; } = new()
        {
            ItemType.Adrenaline,
            ItemType.Coin,
            ItemType.Flashlight,
            ItemType.Jailbird,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Radio,
            ItemType.ArmorCombat,
            ItemType.ArmorHeavy,
            ItemType.ArmorLight,
            ItemType.GrenadeFlash,
            ItemType.GrenadeHE,
            ItemType.GunA7,
            ItemType.GunCom45,
            ItemType.GunCrossvec,
            ItemType.GunLogicer,
            ItemType.GunRevolver,
            ItemType.GunShotgun,
            ItemType.GunAK,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunE11SR,
            ItemType.GunFSP9,
            ItemType.GunFRMG0,
        };

        [Description("Chances for all the good effects")]
        public int MedkitChance { get; set; } = 20;
        public int RedCardChance { get; set; } = 15;
        public int RandomEffectChance { get; set; } = 15;
        public int SilentStepChance { get; set; } = 10;
        public int KeyCardChance { get; set; } = 15;
        public int EscapeTPChance { get; set; } = 5;
        public int HealChance { get; set; } = 20;
        public int MaxHPChance { get; set; } = 15;
        public int RandomItemChance { get; set; } = 10;
        public int RandomCustomItemChance { get; set; } = 5;


        [Description("Chances for all the bad effects")]
        public int ShitPantsChance { get; set; } = 15;
        public int RandomRoomTPChance { get; set; } = 5;
        public int InstaGrenadeChance { get; set; } = 5;
        public int PlayerSwapChance { get; set; } = 5;
        public int FentanylChance { get; set; } = 15;
        public int BouncyBallEffectChance { get; set; } = 5;
        public int HpReductionChance { get; set; } = 10;
        public int HugeDamageChance { get; set; } = 5;
        public int PrimedVaseChance { get; set; } = 5;
        public int FakeScpKillChance { get; set; } = 10;
        public int InventoryResetChance { get; set; } = 5;
        public int SpectSwapChance { get; set; } = 5;
        public int InventorySwapChance { get; set; } = 5;   
        public int JSChance { get; set; } = 2;

    }
}
