using Harmony;
using RimWorld;
using Verse;

namespace RemoteDoors
{
    class Building_RemoteEmbrasure : Building, IRemoteTargetable
    {
        bool openInt;

        public bool OpenInt => openInt;
        private bool IsPowered
        {
            get
            {
                var comp = GetComp<CompPowerTrader>();
                if (comp == null)
                {
                    return false;
                }

                return comp.PowerOn;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref openInt, "open", defaultValue: true);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            def = ThingDefOf.RemoteEmbrasure_Close;
            base.Destroy(mode);
        }

        public void Action()
        {
            ChangeState();
        }

        private void RefreshGraphic()
        {
            var field = AccessTools.Field(typeof(Thing), "graphicInt");
            field.SetValue(this, null);

            var graphic = DefaultGraphic;
        }

        private void CloseEmbrasure()
        {
            openInt = false;
            def = ThingDefOf.RemoteEmbrasure_Close;
        }

        private void OpenEmbrasure()
        {
            openInt = true;
            def = ThingDefOf.RemoteEmbrasure_Open;
        }

        private void ChangeState()
        {
            if (!IsPowered)
            {
                return;
            }
            
            if (openInt)
            {
                CloseEmbrasure();
            }
            else
            {
                OpenEmbrasure();
            }

            RefreshGraphic();
            DirtyMapMesh(Map);
        }
    }
}
