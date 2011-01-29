using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace Shattered
{
    class LevelLoader
    {
        // The background music for the level
        public Song BackgroundMusic;

        // The ContentManager we will use to load our Content
        public ContentManager Content;

        // The FileName used to load our Level
        public string File;

        // The Level we are placing these elements into
        public Level l;

        /// <summary>
        /// Creates a new LevelLoader.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="c"></param>
        public LevelLoader(string fileName, ContentManager c, Level lev)
        {
            File = fileName;
            Content = c;
            l = lev;
        }

        /// <summary>
        /// Loops through our XML and loads our level.
        /// </summary>
        public void LoadLevel()
        {
            // Cycles through the XML file with the level data
            // For each different kind of element we encounter, load that element with those properties
            XmlTextReader reader = new XmlTextReader("Content/Levels/" + File);

            while (reader.Read())
            {
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.Name)
                    {
                        case "tile":
                            {
                                LoadTile(reader);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a Tile, with all the GridPieces.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="c"></param>
        public void LoadTile(XmlTextReader reader)
        {
            Tile t = new Tile(l);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("tile", StringComparison.OrdinalIgnoreCase))
                {
                    l.Tiles.Add(t);
                    break;
                }

                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.Name)
                    {
                        case "gridpiece":
                            {
                                LoadGridPiece(reader, t);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a GridPiece and all its elements.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t">The Tile we are loading the GridPiece into.</param>
        public void LoadGridPiece(XmlTextReader reader, Tile t)
        {
            GridPiece g = new GridPiece();

            // We must read in x before y for proper placement
            int x = 0;
            int y = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("gridpiece", StringComparison.OrdinalIgnoreCase))
                {
                    t.Add(g);
                    break;
                }

                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.Name)
                    {
                        case "x":
                            {
                                x = reader.ReadElementContentAsInt();
                                break;
                            }
                        case "y":
                            {
                                y = reader.ReadElementContentAsInt();

                                // We must read in y AFTER x
                                l.Grid[x, y].PutInPiece(g);
                                break;
                            }
                        case "hitbox":
                            {
                                LoadHitBox(reader, g);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a HitBox into a GridPiece
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="g">The GridPiece we are loading the HitBox in to.</param>
        public void LoadHitBox(XmlTextReader reader, GridPiece g)
        {
            HitBox h;
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            float rot = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("hitbox", StringComparison.OrdinalIgnoreCase))
                {
                    // Adds the HitBox with the (x, y) coordinates relative to the top left corner of the GridPiece
                    h = new HitBox(x, y, (int)(g.gSlot.Position.X + x), (int)(g.gSlot.Position.Y + y), width, height, rot);

                    // Adds the HitBox to the GridPiece
                    g.AddHitBox(h);
                    break;
                }

                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.Name)
                    {
                        case "x":
                            {
                                x = reader.ReadElementContentAsInt();
                                break;
                            }
                        case "y":
                            {
                                y = reader.ReadElementContentAsInt();
                                break;
                            }
                        case "width":
                            {
                                width = reader.ReadElementContentAsInt();
                                break;
                            }
                        case "height":
                            {
                                height = reader.ReadElementContentAsInt();
                                break;
                            }
                        case "rotation":
                            {
                                rot = reader.ReadElementContentAsFloat();
                                break;
                            }
                    }
                }
            }
        }


    }
}
