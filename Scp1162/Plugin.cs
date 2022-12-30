﻿using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Respawning.NamingRules;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Exiled.API.Enums;
using System.Drawing;
using System.Numerics;
using Exiled.API.Features.Items;
using Mirror;
using System.Collections;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using InventorySystem.Items;
using CommandSystem.Commands.Console;
using Exiled.API.Features.Pickups;
using Exiled.API.Extensions;
using Exiled.API.Features.Roles;
using PlayerRoles;
using Exiled.API.Features.DamageHandlers;
using PlayerStatsSystem;


namespace SCP1162
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "Scp-1162";
        public override string Name => "Scp-1162";
        public override string Author => "Mariki";
        public override System.Version Version { get; } = new System.Version(1, 0, 0);
        public static Plugin plugin;
        public List<Exiled.API.Features.Player> players = Exiled.API.Features.Player.List.ToList();
        private ushort Scp1162;
        public Pickup item;
        

        public override void OnEnabled()
        {
            plugin = this;
            base.OnEnabled();
            Exiled.Events.Handlers.Player.PickingUpItem += PickingScp1162;

            Exiled.Events.Handlers.Server.RoundStarted += Server_RoundStarted;
            Exiled.Events.Handlers.Player.Dying += Player_Dying;
            Exiled.Events.Handlers.Player.DroppingAmmo += Player_DroppingAmmo;
            


        }

        private void Player_DroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed= false;
        }

        private void Player_Dying(DyingEventArgs ev)
        {
            foreach(Exiled.API.Features.Items.Item ammo in ev.Player.Items.Where(x=>x.IsAmmo))
            {
                ev.Player.RemoveItem(ammo);
            }
        }

        private void Server_RoundStarted()
        {
            item = Exiled.API.Features.Items.Item.Create(ItemType.SCP500).CreatePickup(UnityEngine.Vector3.zero);
            GameObject scp1162 = item.GameObject;
            Scp1162 = item.Serial;
            NetworkServer.UnSpawn(scp1162);
            scp1162.transform.parent = Room.List.First(x => x.Type == RoomType.Lcz173).transform;
            scp1162.GetComponent<Rigidbody>().useGravity = false;
            scp1162.GetComponent<Rigidbody>().drag = 0f;
            scp1162.GetComponent<Rigidbody>().freezeRotation = true;
            scp1162.transform.localPosition=new Vector3(17f,13.1f,3f);
            scp1162.transform.localRotation = UnityEngine.Quaternion.Euler(90, 1, 0);
            scp1162.transform.localScale = new UnityEngine.Vector3(10, 10, 10);
            NetworkServer.Spawn(scp1162);
            Log.Info(scp1162.transform.position);
            Timing.CallDelayed(1f, () => {InfAmmo();});
            

        }



        private void PickingScp1162(PickingUpItemEventArgs ev)
        {
            
            if (Scp1162 == ev.Pickup.Serial)
            {
                if (ev.Player.CurrentItem != null)
                {
                    ev.Player.RemoveItem(ev.Player.CurrentItem);
                    ev.Player.AddItem(Config.ItemsToGive.RandomItem());
                    ev.Player.ShowHint(Config.InteraktingHint, 3);
                }
                else
                {
                    if (Config.ShouldHeart)
                    {
                        ev.Player.Hurt(Config.HealthMinus);
                        ev.Player.EnableEffect(EffectType.Burned, 3);
                        ev.Player.ShowHint(Config.HeartHint, 3);
                    }
                      
                }
                ev.IsAllowed = false;
            }
            



        }
        
        void InfAmmo()
        {
            
            
            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                foreach (AmmoType ammo in new List<AmmoType>() { AmmoType.Nato556, AmmoType.Nato762, AmmoType.Nato9, AmmoType.Ammo44Cal, AmmoType.Ammo12Gauge })
                {
                    if (!player.IsCuffed)
                        foreach (Exiled.API.Features.Items.Item ammo1 in player.Items.Where(x=>x.IsAmmo))
                        { 
                            player.RemoveItem(ammo1);
                        }
                        player.SetAmmo(ammo, 100);
                }
            }
            Timing.CallDelayed(0f, () => InfAmmo());
        }

    }

}
