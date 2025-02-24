using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace LilinsCoinFlips.Configs
{
    public class Translations : ITranslation
    {
        public List<string> HintMessages { get; set; } = new()
        {
            "Your coin landed on tails.",
            "Your coin landed on heads."
        };

        public string TossOnCoodownMessage { get; set; } = "Warte bis du die nächste Münze werfen kannst...";
        public string CoinNoUsesMessage { get; set; } = "Deine Münze hatte von anfang an keine uses!";
        public string CoinBreaksMessage { get; set; } = "\nDeine Münze ist veschwunden...";




        public string MedikitMessage { get; set; } = "Du hast was erhalten um dich zu verarzten.";
        public string RandomGoodEffectMessage { get; set; } = "Du hast einen zufälligen Effekt erhalten.";
        public string SiletStepMessage { get; set; } = "Im tip toeing in my jordans... (keine Footsteps)";
        public string ContainmentEngineerCardMessage { get; set; } = "Oh, jemand hat seine Keycard fallen lassen...";
        public string RedCardMessage { get; set; } = "Oh, jemand hat seine Keycard fallen lassen...";
        public string TpToEscapeMessage { get; set; } = "Somehow bist du ans Ende gelangt.";
        public string MagicHealMessage { get; set; } = "Du wurdest geheilt.";
        public string HealthIncreaseMessage { get; set; } = "Du hast 10% mehr Max HP erhalten.";
        public string RandomItemMessage { get; set; } = "Du hast ein random Item erhalten.";
        public string RandomCustomItemMessage { get; set; } = "Du hast ein random Custom Item erhalten.";



        public string ShitPantsMessage { get; set; } = "Shits pants aggressively.";
        public string RandomRoomTPMessage { get; set; } = "Du wurdest in einen zufälligen Raum teleportiert.";
        public string InstaGrenadeMessage { get; set; } = "BOOM!";
        public string PlayerSwapMessage { get; set; } = "Du hast den Platz mit jemand fremden getauscht. (Falls nicht gab es keine gültigen Spieler mehr.)";
        public string PlayerSwapIfOneAliveMessage { get; set; } = "Es gibt niemanden zum swappen :(";
        public string FentanylMessage { get; set; } = "Living the american dream :)";
        public string RandomBadEffectMessage { get; set; } = "Du hast einen zufälligen Effekt erhalten.";
        public string BouncyBallMessage { get; set; } = "Jemand hat seine Flummis fallen lassen.";
        public string HpReductionMessage { get; set; } = "Du hast 30% deiner HP verloren.";
        public string HugeDamageMessage { get; set; } = "Yikes... Da hat wohl jemand 99% seiner HP verloren.";
        public string PrimedVaseMessage { get; set; } = "Es wird etwas kälter.";
        public string FakeScpKillMessage { get; set; } = "Ein SCP ist gestorben (nope).";
        public string InventoryResetMessage { get; set; } = "Da hat wohl jemand seine Items verloren.";
        public string SpectSwapNoSpectsMessage { get; set; } = "Schade. Gerade gibt es keine Spectator.";
        public string SpectSwapPlayerMessage { get; set; } = "Auszeit. Du wurdest mit einem Spectator getauscht.";
        public string SpectSwapSpectMessage { get; set; } = "Jemand hat dich 'wiederbelebt'";
        public string InventorySwapOnePlayerMessage { get; set; } = "Niemand ist da um das Inventar zu tauschen.";
        public string InventorySwapMessage { get; set; } = "Dein Inventar wurde mit dem eines anderen getauscht.";
    }
}
