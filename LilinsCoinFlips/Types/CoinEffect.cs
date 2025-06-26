using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Scp939;
using InventorySystem.Items.Usables.Scp330;
using PlayerRoles;

namespace LilinsCoinFlips.Types
{
    public class CoinFlipEffect
    {
        public Action<Player> Execute { get; set; }
        public string Message { get; set; }
        public int Chance { get; set; }
        private static readonly System.Random Random = new System.Random();

        public CoinFlipEffect(string message, Action<Player> execute, int chance)
        {
            Execute = execute;
            Message = message;
            Chance = chance;
        }

        public static Action<Player> GetActionFromName(string actionName, Dictionary<string, object> parameters)
        {
            Log.Debug($"GetActionFromName called with actionName: {actionName}");

            return actionName switch
            {
                "SpawnItems" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("items", out var items) && items is IEnumerable<object> rawList)
                    {
                        Log.Debug($"Found 'items' parameter with {rawList.Count()} items.");

                        foreach (var itemObj in rawList)
                        {
                            var itemStr = itemObj.ToString();
                            Log.Debug($"Attempting to spawn item: {itemStr}");

                            Log.Debug($"Player details: {player.ToString()}");

                            if (Enum.TryParse<ItemType>(itemStr, out var itemEnum))
                            {
                                Log.Debug($"Successfully parsed ItemType: {itemEnum}");
                                Pickup.CreateAndSpawn(itemEnum, player.Position, UnityEngine.Quaternion.identity);
                                Log.Debug($"Spawned {itemEnum} at {player.Position}");
                            }
                            else
                            {
                                Log.Debug($"Invalid ItemType: {itemStr}");
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("'items' parameter not found or invalid format.");
                    }
                },
                "SpawnRandomItem" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("items", out var items) && items is IEnumerable<object> rawList)
                    {
                        var itemList = rawList.Select(i => i.ToString()).ToList();
                        Log.Debug($"Found 'items' parameter with {itemList.Count} items.");

                        if (itemList.Count > 0)
                        {
                            var random = new System.Random();
                            var randomItemStr = itemList[random.Next(itemList.Count)];
                            Log.Debug($"Selected random item: {randomItemStr}");

                            Log.Debug($"Player details: {player.ToString()}");

                            if (Enum.TryParse<ItemType>(randomItemStr, out var itemEnum))
                            {
                                Log.Debug($"Successfully parsed ItemType: {itemEnum}");
                                Pickup.CreateAndSpawn(itemEnum, player.Position, UnityEngine.Quaternion.identity);
                                Log.Debug($"Spawned {itemEnum} at {player.Position}");
                            }
                            else
                            {
                                Log.Debug($"Invalid ItemType: {randomItemStr}");
                            }
                        }
                        else
                        {
                            Log.Debug("Item list is empty.");
                        }
                    }
                    else
                    {
                        Log.Debug("'items' parameter not found or invalid format.");
                    }
                },
                "SpawnCustomItems" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("items", out var items) && items is IEnumerable<object> rawList)
                    {
                        Log.Debug($"Found 'customitems' parameter with {rawList.Count()} items.");

                        foreach (var itemObj in rawList)
                        {
                            var itemStr = itemObj.ToString();
                            Log.Debug($"Attempting to spawn item: {itemStr}");

                            Log.Debug($"Player details: {player.ToString()}");

                            if (Enum.TryParse<ItemType>(itemStr, out var itemEnum))
                            {
                                Log.Debug($"Successfully parsed CustomItem: {itemEnum}");
                                CustomItem.TrySpawn(itemStr, player.Position, out Pickup pickup);
                                Log.Debug($"Spawned {itemEnum} at {player.Position}");
                            }
                            else
                            {
                                Log.Debug($"Invalid CustomItem: {itemStr}");
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("'customitems' parameter not found or invalid format.");
                    }
                },
                "TeleportToRoom" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (Warhead.IsDetonated == true)
                    {
                        player.ShowHint("The warhead detonated. Unable to teleport.");
                        return;
                    }

                    if (parameters.TryGetValue("room", out var location) && location is string loc)
                    {
                        Log.Debug($"Teleporting player to location: {loc}");
                        player.Teleport(Room.Get((RoomType)Enum.Parse(typeof(RoomType), loc)));
                        Log.Debug("Player teleported.");
                    }
                    else
                    {
                        Log.Debug("'location' parameter not found or invalid format.");
                    }
                },
                "TeleportRandom" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("possiblerooms", out var roomListRaw) && roomListRaw is IEnumerable<object> roomList)
                    {
                        var roomArray = roomList.ToArray();

                        if (roomArray.Length == 0)
                        {
                            Log.Debug("Room list is empty.");
                            return;
                        }

                        var randomRoom = roomArray[Random.Next(roomArray.Length)];

                        if (Warhead.IsDetonated == true)
                        {
                            player.ShowHint("The warhead detonated. No teleport target available.");
                            return;
                        }

                        if (randomRoom != null)
                        {
                            var roomStr = randomRoom.ToString();
                            Log.Debug($"Selected random room string: {roomStr}");

                            if (Enum.TryParse<RoomType>(roomStr, out var roomType))
                            {
                                var targetRoom = Room.Get(roomType);
                                if (targetRoom != null)
                                {
                                    player.Teleport(targetRoom);
                                    Log.Debug($"Player {player.Nickname} teleported to room: {roomType}");
                                }
                                else
                                {
                                    Log.Debug($"RoomType {roomType} resolved, but Room.Get returned null.");
                                }
                            }
                            else
                            {
                                Log.Debug($"Invalid RoomType string: {roomStr}");
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("'rooms' parameter not found or invalid format.");
                    }
                },
                "RandomEffect" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("effects", out var effects) && effects is IEnumerable<object> effectList)
                    {
                        Log.Debug($"Found 'effects' parameter with {effectList.Count()} items.");

                        var randomEffect = effectList.ElementAtOrDefault(Random.Next(effectList.Count()));

                        if (randomEffect != null)
                        {
                            var effectStr = randomEffect.ToString();
                            Log.Debug($"Attempting to apply effect: {effectStr}");

                            if (Enum.TryParse<EffectType>(effectStr, out var effectEnum))
                            {
                                float duration = 5f;
                                byte intensity = 1;

                                if (parameters.TryGetValue("duration", out var durObj))
                                {
                                    try { duration = Convert.ToSingle(durObj); }
                                    catch (Exception e) { Log.Warn($"Invalid duration format: {durObj} ({e.Message})"); }
                                }

                                if (parameters.TryGetValue("intensity", out var intObj))
                                {
                                    try { intensity = Convert.ToByte(intObj); }
                                    catch (Exception e) { Log.Warn($"Invalid intensity format: {intObj} ({e.Message})"); }
                                }

                                Log.Debug($"Applying effect {effectEnum} with duration {duration}s and intensity {intensity}");

                                player.EnableEffect(effectEnum, intensity, duration, true);
                            }
                            else
                            {
                                Log.Debug($"Invalid EffectType: {effectStr}");
                            }
                        }
                        else
                        {
                            Log.Warn("No valid effects found in the list.");
                        }
                    }
                    else
                    {
                        Log.Warn("'effects' parameter not found or invalid format.");
                    }
                },
                "Hurt" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("amount", out var ha))
                    {
                        try
                        {
                            int hurtAmount = Convert.ToInt32(ha);
                            Log.Debug($"Hurting player {player.DisplayNickname} by {hurtAmount}");
                            player.Hurt(hurtAmount);
                        }
                        catch (Exception ex)
                        {
                            Log.Debug($"Failed to convert 'amount' to int: {ha} - {ex.Message}");
                        }
                    }
                    else
                    {
                        Log.Debug("'amount' parameter not found or invalid format.");
                    }
                },
                "Handcuff" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    Log.Debug($"Trying to handcuff {player.DisplayNickname}");
                    player.Handcuff();
                },
                "Warhead" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    Log.Debug($"Trying to activate warhead...");
                    if (Warhead.IsDetonated || !Warhead.IsInProgress)
                    {
                        Warhead.Start();
                    }
                },
                "SwapPosition" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    var playerList = Player.List.Where(p => p.IsAlive).ToList();
                    playerList.Remove(player);
                    playerList.Remove(playerList.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp079));

                    var randomPlayer = playerList.RandomItem();
                    var randomPlayerPos = randomPlayer.Position;

                    randomPlayer.Teleport(player.Position);
                    player.Teleport(randomPlayerPos);
                },
                "Explode" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                    grenade.FuseTime = 0.1f;
                    grenade.SpawnActive(player.Position, player);
                },
                "SpawnTantrum" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    player.PlaceTantrum();
                },
                "SpawnRandomItemFromSelection" => player =>
                {
                    Log.Debug($"Handling 'SpawnRandomItemFromSelection' action...");

                    if (parameters.TryGetValue("items", out var items) && items is IEnumerable<object> rawList)
                    {
                        var itemArray = rawList.ToArray();
                        Log.Debug($"Found 'items' parameter with {itemArray.Length} items.");

                        if (itemArray.Length > 0)
                        {
                            var selected = itemArray[Random.Next(itemArray.Length)];
                            var itemStr = selected.ToString();

                            Log.Debug($"Randomly selected item: {itemStr}");

                            if (Enum.TryParse<ItemType>(itemStr, out var itemEnum))
                            {
                                Log.Debug($"Successfully parsed ItemType: {itemEnum}");
                                Pickup.CreateAndSpawn(itemEnum, player.Position, UnityEngine.Quaternion.identity);
                                Log.Debug($"Spawned {itemEnum} at {player.Position}");
                            }
                            else
                            {
                                Log.Debug($"Invalid ItemType: {itemStr}");
                            }
                        }
                        else
                        {
                            Log.Debug("Item list was empty.");
                        }
                    }
                    else
                    {
                        Log.Debug("'items' parameter not found or invalid format.");
                    }
                },
                "StartWarhead" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    Warhead.Start();
                },
                "TeleportToSCP" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    Player scpPlayer = Player.List.Where(p => p.Role.Team == Team.SCPs && p.Role != RoleTypeId.Scp079).GetRandomValue();

                    if (scpPlayer != null)
                    {
                        player.Teleport(scpPlayer);
                    }
                    else
                    {
                        player.ShowHint(Plugin.Instance.Translation.NoSCPToTeleportToMessage, duration: 5f);
                    }
                },
                "ChangeRole" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("roles", out var roleListRaw) && roleListRaw is IEnumerable<object> roleList)
                    {
                        var roles = roleList
                            .Select(role => role.ToString())
                            .Where(roleStr => Enum.TryParse<RoleTypeId>(roleStr, out _))
                            .Select(roleStr => (RoleTypeId)Enum.Parse(typeof(RoleTypeId), roleStr))
                            .ToList();

                        if (roles.Count == 0)
                        {
                            Log.Debug("No valid roles found in parameterr list.");
                            return;
                        }

                        var selectedRole = roles[Random.Next(roles.Count)];

                        Log.Debug($"Assigning role {selectedRole} to player.");
                        player.Role.Set(selectedRole, RoleSpawnFlags.None);
                    }
                    else
                    {
                        Log.Debug("'roles' parameter not found or invalid format.");
                    }
                },
                "InstantFlashbang" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    FlashGrenade grenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                    grenade.FuseTime = 1f;
                    grenade.SpawnActive(player.Position, player);
                },
                "GiveCandy" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("validcandies", out var candyListRaw))
                    {
                        List<string> candyStrings = null;

                        if (candyListRaw is IEnumerable<object> objectList)
                        {
                            candyStrings = objectList.Select(c => c.ToString()).ToList();
                        }
                        else if (candyListRaw is IEnumerable<string> stringList)
                        {
                            candyStrings = stringList.ToList();
                        }

                        if (candyStrings == null || candyStrings.Count == 0)
                        {
                            Log.Debug("Candy list is empty or null.");
                            return;
                        }

                        var randomCandyStr = candyStrings.GetRandomValue();
                        Log.Debug($"Randomly selected candy string: {randomCandyStr}");

                        if (Enum.TryParse<CandyKindID>(randomCandyStr, true, out var randomCandy))
                        {
                            Log.Debug($"Parsed CandyKindID: {randomCandy}");

                            if (player.TryAddCandy(randomCandy))
                            {
                                Log.Debug($"Successfully gave {randomCandy} to {player.DisplayNickname}");
                            }
                            else
                            {
                                Log.Debug($"Failed to give candy to {player.DisplayNickname}");
                            }
                        }
                        else
                        {
                            Log.Debug($"Invalid CandyKindID: {randomCandyStr}");
                        }
                    }
                    else
                    {
                        Log.Debug("'validcandies' parameter not found.");
                    }
                }
                ,
                _ => player => Log.Debug($"Unknown action: {actionName}")
            };
        }
    }
}
