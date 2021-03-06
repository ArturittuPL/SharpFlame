using System;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;


namespace SharpFlame
{
    public class ResultWarningGoto<GotoType> : Result.Warning where GotoType : clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();

            MapGoto.Perform();
        }
    }

    public class ResultProblemGoto<GotoType> : Result.Problem where GotoType : clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();

            MapGoto.Perform();
        }
    }

    public abstract class clsResultItemGotoInterface
    {
        public abstract void Perform();
    }

    public class clsResultItemTileGoto : clsResultItemGotoInterface
    {
        public XYInt TileNum;
        public ViewInfo View;

        public override void Perform()
        {
            View.LookAtTile(TileNum);
        }
    }

    public class clsResultItemPosGoto : clsResultItemGotoInterface
    {
        public XYInt Horizontal;
        public ViewInfo View;

        public override void Perform()
        {
            View.LookAtPos(Horizontal);
        }
    }

    public sealed class MapErrorHelper
    {
        public static ResultProblemGoto<clsResultItemPosGoto> CreateResultProblemGotoForObject(Unit unit, ViewInfo view)
        {
            var resultGoto = new clsResultItemPosGoto();
            if(view.Map != unit.MapLink.Owner)
            {
                throw new Exception("Map changed?");
            }

            resultGoto.Horizontal = unit.Pos.Horizontal;
            var resultProblem = new ResultProblemGoto<clsResultItemPosGoto>
                {
                    MapGoto = resultGoto
                };
            return resultProblem;
        }
    }
}