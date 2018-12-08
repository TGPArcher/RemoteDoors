using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RemoteDoors
{
    class Building_RemoteController : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            using(IEnumerator<Gizmo> enumerator = base.GetGizmos().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return (Gizmo)enumerator.Current;
                }
            }

            if(this.Faction == Faction.OfPlayer)
            {
                yield return (Gizmo)new Command_Target()
                {
                    defaultLabel = "RD_Controller_Gizmo_Activate".Translate(),
                    defaultDesc = "RD_Controller_Gizmo_Activate_Desc".Translate(),
                    icon = TexCommand.Attack,
                    targetingParams = new TargetingParameters()
                    {
                        canTargetFires = false,
                        canTargetBuildings = true,
                        canTargetItems = false,
                        canTargetLocations = false,
                        canTargetPawns = false,
                        canTargetSelf = false
                    },
                    action = new Action<Thing>(ActivateRemote)
                };
            }

            yield break;
        }

        private bool IsManned()
        {
            var comp = this.GetComp<CompMannable>();
            if(comp == null)
            {
                return false;
            }

            return comp.MannedNow;
        }

        private bool InRange(Thing thing)
        {
            LocalTargetInfo targetInfo = new LocalTargetInfo(thing);
            if((targetInfo.Cell - Position).LengthHorizontal < def.specialDisplayRadius)
            {
                return true;
            }

            return false;
        }

        private void ActivateRemote(Thing thing)
        {
            if(thing == null)
            {
                return;
            }

            if (!InRange(thing))
            {
                Messages.Message("RD_Controller_TargetOutOfRange".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            if (!IsManned())
            {
                Messages.Message("RD_Controller_Unmanned".Translate(), new LookTargets(this), MessageTypeDefOf.RejectInput);
                return;
            }

            IRemoteTargetable remoteTargetable = thing as IRemoteTargetable;
            if (remoteTargetable != null)
            {
                remoteTargetable.Action();
            }
            else
            {
                Messages.Message("RD_Controller_TargetNotRemote".Translate(), new LookTargets(thing), MessageTypeDefOf.RejectInput);
            }
        }
    }
}
