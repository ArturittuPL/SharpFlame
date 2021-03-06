

using System;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Gui.Dialogs;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Core.Domain;


namespace SharpFlame.Mapping.IO.FMap
{
    public class FMapInfo
    {
        public InterfaceOptions InterfaceOptions = new InterfaceOptions();
        public XYInt TerrainSize = new XYInt(-1, -1);
        public Tileset Tileset;

        public FMapInfo() 
        {
            TerrainSize = new XYInt (-1, -1);
            Tileset = new Tileset ();
        }
    }
}