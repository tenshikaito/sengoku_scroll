﻿using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Region = Library.Model.Region;

namespace Client
{
    public class GameWorldProcessor
    {
        private const string DirName = "Map";

        private const string OuterMapDataName = "/outer_map.dat";
        private const string InnerMapDataName = "/inner_map_{0}.dat";
        private const string MasterDataName = "/data.dat";

        public static readonly Encoding encoding = Encoding.UTF8;

        private string gameWorldName;
        private string gameWorldPath;
        private string gameWorldFullPath;

        public GameWorldProcessor(string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            gameWorldPath = $"{DirName}/{gameWorldName}";
            gameWorldFullPath = $"{Directory.GetCurrentDirectory()}/{gameWorldPath}";
        }

        public static List<string> getGameWorldList()
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{DirName}";

            Directory.CreateDirectory(path);

            return Directory.EnumerateDirectories(path).Select(o => new DirectoryInfo(o).Name).ToList();
        }

        public bool createGameWorldDirectory(int width, int height)
        {
            if (Directory.Exists(gameWorldFullPath)) return false;

            Directory.CreateDirectory(gameWorldFullPath);

#warning 完成后作为创建初始数据用

            //var gmgd = new GameWorldGameData()
            //{
            //    force = new IncreasedIdDictionary<Force>() { map = new Dictionary<int, Force>() },
            //    province = new IncreasedIdDictionary<Province> { map = new Dictionary<int, Province>() },
            //    stronghold = new IncreasedIdDictionary<Stronghold> { map = new Dictionary<int, Stronghold>() }
            //};

            var gm = new GameWorldOuterMapData()
            {
                data = new OuterTileMap(new TileMap.Size(width, height)) { tiles = new OuterMapTile[width * height] },

                territory = new Dictionary<int, int>(),
                road = new Dictionary<int, int>(),
                stronghold = new Dictionary<int, int>(),
                unit = new Dictionary<int, int>(),
            };

            File.WriteAllText(gameWorldFullPath + OuterMapDataName, compressTileMap(gm.toJson()), encoding);

            var gwmd = new GameWorldMasterData()
            {
                terrain = new Dictionary<int, Terrain>()
                {
                    { 0, new Terrain()
                    {
                        id = 0,
                        name = "plain",
                        isWater = false
                    }
                    },
                    { 1, new Terrain()
                    {
                        id = 1,
                        name = "water",
                        isWater = true
                    }
                    }
                },
                region = new Dictionary<int, Region>(),
                culture = new Dictionary<int, Culture>(),
                religion = new Dictionary<int, Religion>(),
                road = new Dictionary<int, Road>(),

                strongholdType = new Dictionary<int, Stronghold.Type>(),

                outerTileMapImageInfo = new Dictionary<int, OuterTileMapImageInfo>()
                {
                    { 1, new OuterTileMapImageInfo()
                    {
                        id = 1,
                        name="default view map",
                        tileSize = new Size(24, 24),
                        terrainAnimation = new Dictionary<byte, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_plain.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            },
                            { 1, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_water.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            }
                        },
                        strongholdAnimation = new Dictionary<int, TileAnimation>()
                        {
                        }
                    }
                    },
                    { 2, new OuterTileMapImageInfo()
                    {
                        id = 2,
                        name="default detail map",
                        tileSize = new Size(48, 48),
                        terrainAnimation = new Dictionary<byte, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames= new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "detail_terrain_plain.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            },
                            { 1, new TileAnimation()
                            {
                                id = 1,
                                interval = 0.5f,
                                frames= new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "detail_terrain_water.png",
                                        vertex = new Point(0, 0)
                                    },
                                    new TileAnimation.Frame()
                                    {
                                        fileName="detail_terrain_water.png",
                                        vertex = new Point(0, 144),
                                    },
                                    new TileAnimation.Frame()
                                    {
                                        fileName="detail_terrain_water.png",
                                        vertex = new Point(0, 288),
                                    },
                                }
                            }
                            }
                        },
                        strongholdAnimation = new Dictionary<int, TileAnimation>()
                        {
                        }
                    }
                    }
                },
                innerTileMapImageInfo = new Dictionary<int, InnerTileMapImageInfo>() {
                    { 1, new InnerTileMapImageInfo()
                    {
                        id = 1,
                        name="default view map",
                        tileSize = new Size(24, 24),
                        terrainAnimation = new Dictionary<byte, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_plain.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            },
                            { 1, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_water.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            }
                        }
                    }
                    },
                    { 2, new InnerTileMapImageInfo()
                    {
                        id = 2,
                        name="default detail map",
                        tileSize = new Size(48, 48),
                        terrainAnimation = new Dictionary<byte, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames= new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "detail_terrain_plain.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            },
                            { 1, new TileAnimation()
                            {
                                id = 1,
                                interval = 0.5f,
                                frames= new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "detail_terrain_water.png",
                                        vertex = new Point(0, 0)
                                    },
                                    new TileAnimation.Frame()
                                    {
                                        fileName="detail_terrain_water.png",
                                        vertex = new Point(0, 144),
                                    },
                                    new TileAnimation.Frame()
                                    {
                                        fileName="detail_terrain_water.png",
                                        vertex = new Point(0, 288),
                                    },
                                }
                            }
                            }
                        },
                    }
                    }
                    },
                innerTileMapInfo = new Dictionary<int, InnerTileMapInfo>()
            };

            File.WriteAllText(gameWorldFullPath + MasterDataName, gwmd.toJson(), encoding);

            // todo 数据固定后将所有文件全部.dat数据文件添加到目标文件夹

            Directory.GetFiles($"{Directory.GetCurrentDirectory()}/Data/GameWorld").ToList().ForEach(o =>
            {
                var fi = new FileInfo(o);
                File.Copy(o, $"{gameWorldFullPath}/{fi.Name}");
            });

            return true;
        }

        public string getFilePath(string fileName, bool isAbsolute = false)
            => isAbsolute ? $"{gameWorldPath}/{fileName}" : $"{gameWorldFullPath}/{fileName}";

        public void delete() => Directory.Delete(gameWorldFullPath, true);

        public GameWorld load(int screenWidth, int screenHeight)
        {
            var gw = new GameWorld(gameWorldName)
            {
                camera = new Camera(screenWidth, screenHeight)
            };

            gw.gameOuterMapData = File.ReadAllText(gameWorldFullPath + OuterMapDataName, encoding).fromJson<GameWorldOuterMapData>();
            gw.gameWorldMasterData = File.ReadAllText(gameWorldFullPath + MasterDataName, encoding).fromJson<GameWorldMasterData>();

            return gw;
        }

        public void save(GameWorld gw)
        {
            File.WriteAllText(gameWorldFullPath + OuterMapDataName, gw.gameOuterMapData.toJson(), encoding);

            File.WriteAllText(gameWorldFullPath + MasterDataName, gw.gameWorldMasterData.toJson(), encoding);
        }

        public InnerTileMap loadInnerTileMap(int id, int width, int height)
        {
            var path = gameWorldFullPath + string.Format(InnerMapDataName, id);

            if (!File.Exists(path)) return new InnerTileMap(new TileMap.Size(height, width)) { tiles = new InnerMapTile[width * height] };

            return uncompressTileMap( File.ReadAllText(path, encoding)).fromJson<InnerTileMap>();
        }

        public void saveInnerTileMap(int id, InnerTileMap tm)
        {
            var path = gameWorldFullPath + string.Format(InnerMapDataName, id);

            File.WriteAllText(path, compressTileMap(tm.toJson()), encoding);
        }

        private string compressTileMap(string json)
            => json.Replace($@"""{ nameof(OuterMapTile.terrain) }""", @"""t""")
            .Replace($@"""{ nameof(OuterMapTile.region) }""", @"""r""")
            .Replace($@"""{ nameof(InnerMapTile.height) }""", @"""h""")
            .Replace($@"""{ nameof(InnerMapTile.functionType) }""", @"""f""");

        private string uncompressTileMap(string json)
            => json.Replace(@"""t""", $@"""{ nameof(OuterMapTile.terrain) }""")
            .Replace(@"""r""", $@"""{ nameof(OuterMapTile.region) }""")
            .Replace(@"""h""", $@"""{ nameof(InnerMapTile.height) }""")
            .Replace(@"""f""", $@"""{ nameof(InnerMapTile.functionType) }""");
    }
}
