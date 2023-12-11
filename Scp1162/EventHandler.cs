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
    public class EventHandler
    {
        public ushort Scp1162;
        public void SpawnScp1162()
        {
            Pickup item = Exiled.API.Features.Items.Item.Create(ItemType.SCP500).CreatePickup(UnityEngine.Vector3.zero);
            GameObject scp1162 = item.GameObject;
            Scp1162 = item.Serial;
            NetworkServer.UnSpawn(scp1162);
            scp1162.transform.parent = Room.List.First(x => x.Type == RoomType.Lcz173).transform;
            scp1162.GetComponent<Rigidbody>().useGravity = false;
            scp1162.GetComponent<Rigidbody>().drag = 0f;
            scp1162.GetComponent<Rigidbody>().freezeRotation = true;
            scp1162.isStatic = true;
            scp1162.SetActive(false);
            scp1162.transform.localPosition = new Vector3(17f, 13.1f, 3f);
            scp1162.transform.localRotation = UnityEngine.Quaternion.Euler(90, 1, 0);
            scp1162.transform.localScale = new UnityEngine.Vector3(10, 10, 10);
            NetworkServer.Spawn(scp1162);
        }
        public void OnRoundStart()
        {
            SpawnScp1162();
        }
        public void PickingScp1162(PickingUpItemEventArgs ev)
        {

            if (Scp1162 == ev.Pickup.Serial)
            {
                if (ev.Player.CurrentItem != null)
                {
                    ev.Player.RemoveItem(ev.Player.CurrentItem);
                    ev.Player.AddItem(Plugin.plugin.Config.ItemsToGive.RandomItem());
                    ev.Player.ShowHint(Plugin.plugin.Config.InteractingHint, 3);
                }
                else
                {
                    if (Plugin.plugin.Config.ShouldHurt)
                    {
                        ev.Player.Hurt(Plugin.plugin.Config.Damage_Amount, DamageType.Custom);
                        ev.Player.EnableEffect(EffectType.Burned, 3);
                        ev.Player.ShowHint(Plugin.plugin.Config.HurtHint, 3);
                    }

                }
                ev.IsAllowed = false;
            }
        }
    }
}
