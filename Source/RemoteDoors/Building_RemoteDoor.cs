using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RemoteDoors
{
    public class Building_RemoteDoor : Building_Door, IRemoteTargetable
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            var gizmos = base.GetGizmos().ToList();
            gizmos.RemoveLast();

            using (IEnumerator<Gizmo> enumerator = gizmos.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Gizmo g = enumerator.Current;
                    yield return g;
                }
            }

            yield break;
        }

        public override bool PawnCanOpen(Pawn p)
        {
            return false;
        }

        public override bool BlocksPawn(Pawn p)
        {
            return !Open;
        }

        private void ActionDoor()
        {
            if (!DoorPowerOn)
            {
                Messages.Message("RD_Door_NotPowered".Translate(), new LookTargets(this), MessageTypeDefOf.RejectInput);
                return;
            }

            var holdOpenField = AccessTools.Field(typeof(Building_Door), "holdOpenInt");
            if (Open)
            {
                holdOpenField.SetValue(this, false);
                if (!DoorTryClose())
                {
                    holdOpenField.SetValue(this, true);
                    Messages.Message("RD_Door_Blocked".Translate(), new LookTargets(this), MessageTypeDefOf.RejectInput);
                }
            }
            else
            {
                holdOpenField.SetValue(this, true);
                DoorOpen();
            }
        }

        public void Action()
        {
            ActionDoor();
        }
    }
}
