using System;
using System.Net;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using Photon.Pun;
using Utilla;

namespace ConfigurableCodeAnnouncements
{
    
    [BepInPlugin("dev.sohdah.mods.gtag.cca", "ConfigurableCodeAnnouncements", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;

        public ConfigEntry<string> Webhook;

        public ConfigEntry<string> nameOfPlayer;

        public ConfigEntry<string> TimeZone;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized; 
            Utilla.Events.RoomJoined += RoomJoined;
            Utilla.Events.RoomLeft += RoomLeft;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            Webhook = Config.Bind("Config","Webhook","WebhookHere","The Webhook that the code wil be sent by");
            if (Webhook.Value == "WebhookHere"){Logger.LogFatal("Webhook Not Set");}
            nameOfPlayer = Config.Bind("Config", "Player Name", "NameHere", "The name the Webhook will say (ex: NameHere has joined code Code 7364)");
            if (nameOfPlayer.Value == "NameHere")
            {Logger.LogFatal("Name Not Set");}
            TimeZone = Config.Bind("Config", "TimeZone", "TimeZoneHere", "The Timezone that you are in");
            if (TimeZone.Value == "TimeZoneHere"){ Logger.LogFatal("Time Zone Not Set"); }
            SendMsg(nameOfPlayer.Value + " has opened Gorilla Tag at " + DateTime.Now + " (" + TimeZone.Value + ").", Webhook.Value);
        }

        private void RoomJoined(object sender, Events.RoomJoinedArgs e)
        {
            var GameMode = GorillaComputer.instance.currentGameMode;
            // Not a switch cause it wasnt working :skull:
            if(GameMode == "MODDED_MODDED_CASUALCASUAL")
                GameMode = "Modded Casual";
            if(GameMode == "MODDED_MODDED_DEFAULTINFECTION")
                GameMode = "Modded Infection";
            if(GameMode == "MODDED_MODDED_HUNTHUNT")
                GameMode = "Modded Hunt";
            if(GameMode == "MODDED_MODDED_BATTLEPAINTBRAWL")
                GameMode = "Modded Paintbrawl";
            SendMsg(nameOfPlayer.Value + " has joined the code " + PhotonNetwork.CurrentRoom.Name + " at " + DateTime.UtcNow + " (" + TimeZone.Value + ")." + " It is in the " + GameMode + " Gamemode.", Webhook.Value);
        }

        private void RoomLeft(object sender, Events.RoomJoinedArgs e)
        {
            SendMsg(nameOfPlayer.Value + " has left the code at " + DateTime.Now + " (" + TimeZone.Value + ").", Webhook.Value);
        }


        public static void SendMsg(string message, string url)
        {
            WebClient client = new WebClient();
            string NiceEmbedMessage = "> " + message;
            client.Headers.Add("Content-Type", "application/json");
            string payload = "{\"content\": \"" + NiceEmbedMessage + "\"}";
            client.UploadData(url, Encoding.UTF8.GetBytes(payload));
        }
    }
}