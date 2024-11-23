using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMashBox.BMX_Physics_Development.Animancer_Test.Trick_System;
using Il2CppMG_Core.C_R_I_D.Animation_System.Animancer_Test;
using MelonLoader;
using UnityEngine;
namespace MashBoxMod
{
    public class Rebind
    {
        // trick list used for rebinding. the indexes of the classes in this array are fixed.
        // classes are added in alphabetical order in the FindAnimationData() method. 
        // when adding entries make sure the animObject.name is in alphabetical order or else trick indexes will be wrong.
        // this also makes it easier to read from C++ and add to the UI.
        public List<SyncTrickAnimationData?> trickList = new List<SyncTrickAnimationData?>();

        // Every tricks animation data.
        SyncTrickAnimationData? TuckNoHander;
        SyncTrickAnimationData? TuckUp;
        SyncTrickAnimationData? Table;
        SyncTrickAnimationData? Turndown;
        SyncTrickAnimationData? Invert;
        SyncTrickAnimationData? InvertFlipped;
        SyncTrickAnimationData? Suicide;
        SyncTrickAnimationData? Euro;
        SyncTrickAnimationData? Lookback;
        SyncTrickAnimationData? Cannonball;
        SyncTrickAnimationData? XUpLeft;
        SyncTrickAnimationData? XUpRight;
        SyncTrickAnimationData? TBogLeft;
        SyncTrickAnimationData? TBogRight;
        SyncTrickAnimationData? BarspinLeft;
        SyncTrickAnimationData? BarspinRight;
        SyncTrickAnimationData? BusDriverRight;
        SyncTrickAnimationData? BusDriverLeft;
        SyncTrickAnimationData? SeatGrabRight;
        SyncTrickAnimationData? SeatGrabLeft;
        SyncTrickAnimationData? TireGrabLeft;
        SyncTrickAnimationData? TireGrabRight;
        SyncTrickAnimationData? Crankflip;
        SyncTrickAnimationData? WhopperLeft;
        SyncTrickAnimationData? WhopperRight;
        SyncTrickAnimationData? WhipLeft;
        SyncTrickAnimationData? WhipRight;
        SyncTrickAnimationData? OneHanderRight;
        SyncTrickAnimationData? OneHanderLeft;
        SyncTrickAnimationData? OneFooterRight;
        SyncTrickAnimationData? OneFooterLeft;
        SyncTrickAnimationData? CandyBarRight;
        SyncTrickAnimationData? CandyBarLeft;
        SyncTrickAnimationData? GrizAirRight;
        SyncTrickAnimationData? GrizAirLeft;
        SyncTrickAnimationData? NacRight;
        SyncTrickAnimationData? NacLeft;
        SyncTrickAnimationData? MotoRight;
        SyncTrickAnimationData? MotoLeft;
        SyncTrickAnimationData? CanRight;
        SyncTrickAnimationData? CanLeft;
        SyncTrickAnimationData? NoFootCanRight;
        SyncTrickAnimationData? NoFootCanLeft;

        Il2CppReferenceArray<TrickSetData>? trickSets;
        public bool FindTrickSets()
        {
            // Trick System
            TrickSystemBrain trickSystemBrain = GameObject.Find($"{Main.PSportsPrefix}/Proto_BMX/Trick Launcher/Brain").GetComponent<TrickSystemBrain>();
            if (trickSystemBrain != null)
            {
                trickSets = trickSystemBrain.TrickSets; // Array of Trick set Data objects for Triggers and or Bumpers.
                return true;
            }
            else
                return false; // failed to find trick sets

        }

        public bool BindTrick(TrickInput input, Direction direction, SyncTrickAnimationData newTrick)
        {
            if(newTrick == null || trickSets == null)
            {
                Log.Warning("SyncTrickAnimationData or TrickSetData object is null");
                return false;
            }

            TrickSetData TrickInput = trickSets[(int)input]; 
            TrickInput.TrickData[(int)direction] = newTrick;
            return true;
        }

        public enum TrickInput
        {
            BothBumpers,
            LeftBumper,
            RightBumper,
            BothTriggers,
            LeftTrigger,
            RightTrigger
        }

        public enum Direction
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left, 
            UpLeft,
        }

        public void usePipeControls(bool enabled = false)
        {
            if (enabled)
            {
                // These buttons were bound to animations that dont exist in mashbox 
                // Lookback was unmapped so im binding it 
                BindTrick(TrickInput.LeftBumper, Direction.DownLeft, Lookback);
                BindTrick(TrickInput.RightBumper, Direction.DownRight, Lookback);

                // Cannonballs werent in pipe but im mapping them to these inputs that aren't bound
                BindTrick(TrickInput.LeftTrigger, Direction.Up, Cannonball);
                BindTrick(TrickInput.RightTrigger, Direction.Up, Cannonball);

                // XUps
                BindTrick(TrickInput.BothBumpers, Direction.Right, XUpRight);
                BindTrick(TrickInput.BothBumpers, Direction.Left, XUpLeft);
                // Turndown
                BindTrick(TrickInput.BothBumpers, Direction.DownLeft, Turndown);
                // Euro
                BindTrick(TrickInput.BothBumpers, Direction.UpLeft, Euro);
                // Invert
                BindTrick(TrickInput.BothBumpers, Direction.DownRight, Invert);
                // Tire Grabs
                BindTrick(TrickInput.LeftBumper, Direction.UpLeft, TireGrabLeft);
                BindTrick(TrickInput.RightBumper, Direction.UpRight, TireGrabRight);
                // Moto
                BindTrick(TrickInput.LeftTrigger, Direction.Right, MotoRight);
                BindTrick(TrickInput.RightTrigger, Direction.Left, MotoLeft);
                // Tbogs
                BindTrick(TrickInput.LeftBumper, Direction.Right, TBogLeft);
                BindTrick(TrickInput.RightBumper, Direction.Left, TBogRight);
                // Seat grabs
                BindTrick(TrickInput.LeftBumper, Direction.Up, SeatGrabLeft);
                BindTrick(TrickInput.RightBumper, Direction.Up, SeatGrabRight);
            }
            else
            {
                // Original mashbox binds.
                BindTrick(TrickInput.LeftBumper, Direction.DownLeft, TBogLeft);
                BindTrick(TrickInput.RightBumper, Direction.DownRight, TBogRight);
                BindTrick(TrickInput.LeftTrigger, Direction.Up, null);
                BindTrick(TrickInput.RightTrigger, Direction.Up, null);
                BindTrick(TrickInput.LeftBumper, Direction.Up, null);
                BindTrick(TrickInput.RightBumper, Direction.Up, null);
                BindTrick(TrickInput.BothBumpers, Direction.Right, Invert);
                BindTrick(TrickInput.BothBumpers, Direction.Left, Turndown);
                BindTrick(TrickInput.BothBumpers, Direction.DownLeft, Euro);
                BindTrick(TrickInput.BothBumpers, Direction.UpLeft, Cannonball);
                BindTrick(TrickInput.BothBumpers, Direction.DownRight, Lookback);
                BindTrick(TrickInput.LeftBumper, Direction.UpLeft, XUpLeft);
                BindTrick(TrickInput.RightBumper, Direction.UpRight, XUpRight);
                BindTrick(TrickInput.LeftTrigger, Direction.Right, MotoLeft);
                BindTrick(TrickInput.RightTrigger, Direction.Left, MotoRight);
                BindTrick(TrickInput.LeftBumper, Direction.Right, SeatGrabLeft);
                BindTrick(TrickInput.RightBumper, Direction.Left, SeatGrabRight);
                BindTrick(TrickInput.LeftBumper, Direction.Up, null);
                BindTrick(TrickInput.RightBumper, Direction.Up, null);
            }
            

        }
        public void FindAnimationData()
        {
            var animObjects = Resources.FindObjectsOfTypeAll<SyncTrickAnimationData>();
            foreach (var animObject in animObjects)
            {

                if ( animObject.name == "Barspin Left_SyncAnimatorData_PlayerBMX")
                {
                    BarspinLeft = animObject;
                    if(animObject != null) trickList.Add(BarspinLeft);
                }

                if (animObject.name == "Barspin Right_SyncAnimatorData_PlayerBMX")
                {
                    BarspinRight = animObject;
                    trickList.Add(BarspinRight);
                }

                if (animObject.name == "Bus Driver Left_SyncAnimatorData_PlayerBMX")
                {
                    BusDriverLeft = animObject;
                    trickList.Add(BusDriverLeft);
                }

                if (animObject.name == "Bus Driver Right_SyncAnimatorData_PlayerBMX")
                {
                    BusDriverRight = animObject;
                    trickList.Add(BusDriverRight);
                }

                if (animObject.name == "Can Left_SyncAnimatorData_PlayerBMX")
                {
                    CanLeft = animObject;
                    trickList.Add(CanLeft);
                }

                if (animObject.name == "Can Right_SyncAnimatorData_PlayerBMX")
                {
                    CanRight = animObject;
                    trickList.Add(CanRight);
                }

                if (animObject.name == "Candy Bar Left_SyncAnimatorData_PlayerBMX")
                {
                    CandyBarLeft = animObject;
                    trickList.Add(CandyBarLeft);
                }

                if (animObject.name == "Candy Bar Right_SyncAnimatorData_PlayerBMX")
                {
                    CandyBarRight = animObject;
                    trickList.Add(CandyBarRight);
                }

                if (animObject.name == "Cannon Ball_SyncAnimatorData_PlayerBMX")
                {
                    Cannonball = animObject;
                    trickList.Add(Cannonball);
                }

                if (animObject.name == "Crankflip_SyncAnimatorData_PlayerBMX")
                {
                    Crankflip = animObject;
                    trickList.Add(Crankflip);
                }

                if (animObject.name == "Euro_SyncAnimatorData_PlayerBMX")
                {
                    Euro = animObject;
                    trickList.Add(Euro);
                }

                if (animObject.name == "Griz Air Left_SyncAnimatorData_PlayerBMX")
                {
                    GrizAirLeft = animObject;
                    trickList.Add(GrizAirLeft);
                }

                if (animObject.name == "Griz Air Right_SyncAnimatorData_PlayerBMX")
                {
                    GrizAirRight = animObject;
                    trickList.Add(GrizAirRight);
                }

                if (animObject.name == "Invert_SyncAnimatorData_PlayerBMX")
                {
                    Invert = animObject;
                    trickList.Add(Invert);
                }

                if (animObject.name == "Lookback_SyncAnimatorData_PlayerBMX")
                {
                    Lookback = animObject;
                    trickList.Add(Lookback);
                }

                if (animObject.name == "Moto Left_SyncAnimatorData_PlayerBMX")
                {
                    MotoLeft = animObject;
                    trickList.Add(MotoLeft);
                }

                if (animObject.name == "Moto Right_SyncAnimatorData_PlayerBMX")
                {
                    MotoRight = animObject;
                    trickList.Add(MotoRight);
                }

                if (animObject.name == "Nac Left_SyncAnimatorData_PlayerBMX")
                {
                    NacLeft = animObject;
                    trickList.Add(NacLeft);
                }

                if (animObject.name == "Nac Right_SyncAnimatorData_PlayerBMX")
                {
                    NacRight = animObject;
                    trickList.Add(NacRight);
                }

                if (animObject.name == "No Foot Can Left_SyncAnimatorData_PlayerBMX")
                {
                    NoFootCanLeft = animObject;
                    trickList.Add(NoFootCanLeft);
                }

                if (animObject.name == "No Foot Can Right_SyncAnimatorData_PlayerBMX")
                {
                    NoFootCanRight = animObject;
                    trickList.Add(NoFootCanRight);
                }

                if (animObject.name == "One Footer Left_SyncAnimatorData_PlayerBMX")
                {
                    OneFooterLeft = animObject;
                    trickList.Add(OneFooterLeft);
                }

                if (animObject.name == "One Footer Right_SyncAnimatorData_PlayerBMX")
                {
                    OneFooterRight = animObject;
                    trickList.Add(OneFooterRight);
                }

                if (animObject.name == "One Hander Left_SyncAnimatorData_PlayerBMX")
                {
                    OneHanderLeft = animObject;
                    trickList.Add(OneHanderLeft);
                }

                if (animObject.name == "One Hander Right_SyncAnimatorData_PlayerBMX")
                {
                    OneHanderRight = animObject;
                    trickList.Add(OneHanderRight);
                }

                if (animObject.name == "Seat Grab Left_SyncAnimatorData_PlayerBMX")
                {
                    SeatGrabLeft = animObject;
                    trickList.Add(SeatGrabLeft);
                }

                if (animObject.name == "Seat Grab Right_SyncAnimatorData_PlayerBMX")
                {
                    SeatGrabRight = animObject;
                    trickList.Add(SeatGrabRight);
                }

                if (animObject.name == "Suicide_SyncAnimatorData_PlayerBMX")
                {
                    Suicide = animObject;
                    trickList.Add(Suicide);
                }

                if (animObject.name == "Table_SyncAnimatorData_PlayerBMX")
                {
                    Table = animObject;
                    trickList.Add(Table);
                }

                if (animObject.name == "Tire Grab Left_SyncAnimatorData_PlayerBMX")
                {
                    TireGrabLeft = animObject;
                    trickList.Add(TireGrabLeft);
                }

                if (animObject.name == "Tire Grab Right_SyncAnimatorData_PlayerBMX")
                {
                    TireGrabRight = animObject;
                    trickList.Add(TireGrabRight);
                }

                if (animObject.name == "Toboggan Left_SyncAnimatorData_PlayerBMX")
                {
                    TBogLeft = animObject;
                    trickList.Add(TBogLeft);
                }

                if (animObject.name == "Toboggan Right_SyncAnimatorData_PlayerBMX")
                {
                    TBogRight = animObject;
                    trickList.Add(TBogRight);
                }

                if (animObject.name == "Tuck No_SyncAnimatorData_PlayerBMX")
                {
                    TuckNoHander = animObject;
                    trickList.Add(TuckNoHander);
                }

                if (animObject.name == "Tuck Up_SyncAnimatorData_PlayerBMX")
                {
                    TuckUp = animObject;
                    trickList.Add(TuckUp);
                }

                if (animObject.name == "Turndown_SyncAnimatorData_PlayerBMX")
                {
                    Turndown = animObject;
                    trickList.Add(Turndown);
                }

                if (animObject.name == "Whip Left_SyncAnimatorData_PlayerBMX")
                {
                    WhipLeft = animObject;
                    trickList.Add(WhipLeft);
                }

                if (animObject.name == "Whip Right_SyncAnimatorData_PlayerBMX")
                {
                    WhipRight = animObject;
                    trickList.Add(WhipRight);
                }

                if (animObject.name == "Whopper Left_SyncAnimatorData_PlayerBMX")
                {
                    WhopperLeft = animObject;
                    trickList.Add(WhopperLeft);
                }

                if (animObject.name == "Whopper Right_SyncAnimatorData_PlayerBMX")
                {
                    WhopperRight = animObject;
                    trickList.Add(WhopperRight);
                }

                if (animObject.name == "Xup Left_SyncAnimatorData_PlayerBMX")
                {
                    XUpLeft = animObject;
                    trickList.Add(XUpLeft);
                }

                if (animObject.name == "Xup Right_SyncAnimatorData_PlayerBMX")
                {
                    XUpRight = animObject;
                    trickList.Add(XUpRight);
                }

            }

        }

    }
}
