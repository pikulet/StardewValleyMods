﻿using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAssets.PackData
{
    public class ContentPack
    {
        internal IContentPack smapiPack;

        internal Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        private Dictionary<string, CommonPackData> items = new Dictionary<string, CommonPackData>();

        public ContentPack( IContentPack pack )
        {
            smapiPack = pack;
            LoadAndValidate<ObjectPackData>( "objects.json" );
        }

        public CommonPackData Find( string item )
        {
            return items.ContainsKey( item ) ? items[ item ] : null;
        }

        private void LoadAndValidate< T >( string json ) where T : CommonPackData
        {
            var data = smapiPack.LoadAsset<List<T>>( json ) ?? new List<T>();
            foreach ( var d in data )
            {
                if ( items.ContainsKey( d.ID ) )
                    throw new ArgumentException( "Duplicate found! " + d.ID );
                items.Add( d.ID, d );
                d.parent = this;
            }
        }

        internal TexturedRect GetTexture( string path, int xSize, int ySize )
        {
            int colon = path.IndexOf( ':' );
            if ( colon == -1 )
                return new TexturedRect() { Texture = smapiPack.LoadAsset< Texture2D >( path ), Rect = null };
            var tex = smapiPack.LoadAsset< Texture2D >( path.Substring( 0, colon ) );
            int sections = tex.Width / xSize;
            int ind = int.Parse( path.Substring( colon + 1 ) );
            return new TexturedRect()
            {
                Texture = tex,
                Rect = new Microsoft.Xna.Framework.Rectangle( ind % sections * xSize, ind / sections * ySize, xSize, ySize )
            };
        }
    }
}
