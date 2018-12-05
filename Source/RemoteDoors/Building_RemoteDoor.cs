using Harmony;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RemoteDoors
{
    public class Building_RemoteDoor : Building_Door
    {
        public override bool PawnCanOpen(Pawn p)
        {
            return false;
        }

        public override bool BlocksPawn(Pawn p)
        {
            return !Open;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            using (IEnumerator<Gizmo> enumerator = base.GetGizmos().GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    Gizmo g = enumerator.Current;
                    yield return g;
                }
            }

            if (Faction == Faction.OfPlayer)
            {
                var com = new Command_Action
                {
                    action = delegate
                    {
                        if (!DoorPowerOn)
                        {
                            Messages.Message("Door is not powered", new LookTargets(this), MessageTypeDefOf.RejectInput);
                            return;
                        }

                        var holdOpenField = AccessTools.Field(typeof(Building_Door), "holdOpenInt");
                        if (Open)
                        {
                            holdOpenField.SetValue(this, false);
                            if (!DoorTryClose())
                            {
                                holdOpenField.SetValue(this, true);
                                Messages.Message("Can't close the door because it is blocked momentary!", new LookTargets(this), MessageTypeDefOf.RejectInput);
                            }
                        }
                        else
                        {
                            holdOpenField.SetValue(this, true);
                            DoorOpen();
                        }
                    }
                };

                if (Open)
                {
                    com.defaultLabel = "Close";
                    com.defaultDesc = "Activate remote door's closing mechanism";
                    com.icon = TexCommand.ForbidOff;
                }
                else
                {
                    com.defaultLabel = "Open";
                    com.defaultDesc = "Activate remote door's opening mechanism";
                    com.icon = TexCommand.ForbidOn;
                }

                yield return com;
            }

            yield break;
        }
    }
}
