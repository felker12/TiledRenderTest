using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TiledRenderTest.Engine;
using TiledRenderTest.Shapes;

namespace TiledRenderTest
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        private SpriteBatch SpriteBatch { get; set; }
        public static int ScreenWidth { get; set; } = 1280;
        public static int ScreenHeight { get; set; } = 720;
        Sprite Sprite { get; set; } = new();
        Sprite Sprite2 { get; set; } = new();
        Player Player { get; set; } = new();
        Camera Camera { get; set; }
        ShapeManager ShapeManager { get; set; } = new();

        internal List<TileMap> Maps { get; set; } = [];

        private TileMap tileMap;
        private TileMap DungeonMap;

        List<Shape> Shapes { get; set; } = [];
        public Random Random { get; set; } = new();

        public double totalTime = 0f;
        public int count = 0;   
        public double AverageTime => count > 0 ? totalTime / count : 0;

        Shapes.Rectangle Rectangle { get; set; }

        public Game1()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ScreenWidth,
                PreferredBackBufferHeight = ScreenHeight,
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            Sprite.Texture = CreateTextureFromColor(Color.Chocolate);
            Sprite2.Texture = CreateTextureFromColor(Color.Red);
            Sprite2.Position = new(300, 50);
            Player.Texture = CreateTextureFromColor(Color.White);
            Player.Position = new(-40, 0);
            Player.Color = Color.DarkBlue;
            Camera = new(ScreenWidth, ScreenHeight);

            Rectangle = new(new Vector2(0, 0), 100, 100, Color.Green)
            {
                Rotate = true
            };

            ShapeManager.AddRandomShapes(10, new(-400, -400), new(400, 400));

            //CreateShapes();

            Shapes.Add(new Star(new Vector2(-300, -200), 6) 
            { 
                Rotate = true 
            });
            Shapes.Add(new Triangle(new Vector2(120,120), new Vector2(150, 180), new Vector2(200,200))
            { 
                Rotate = true 
            });
            Shapes.Add(new Circle(new Vector2(200, -200), 50, 32, Color.Aquamarine)
            {
                Rotate = true,
                RotationSpeedDegreesPerSecond = 30
            });
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            tileMap = new(Content);
            tileMap.LoadFromTmx("Content/Catacombs1.tmx");

            DungeonMap = new(Content);
            DungeonMap.LoadFromTmx("Content/Dungeon.tmx");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            DungeonMap.Update(gameTime);

            //Sprite.Update(gameTime);
            //Sprite2.Update(gameTime);

            Player.Update(gameTime);
            Camera.Update(Player.Position);

            base.Update(gameTime);

            //Debug.WriteLine($"Player Position: {Player.Position}");

            UpdateShapes(gameTime);
            ShapeManager.Update(gameTime);

        }

        public void UpdateShapes(GameTime gameTime)
        {
            //Stopwatch stopwatch = new();
            //stopwatch.Start();

            foreach (var shape in Shapes)
            {
                shape.Update(gameTime);

                if (shape.Contains(Player.Position))
                {
                    shape.Color = Color.Red;
                    //Debug.WriteLine("contains");
                }
                else
                {
                    shape.Color = shape.DefaultColor;
                }
            }

            Rectangle.Update(gameTime);


            if (Rectangle.Contains(Player.Position))
                Rectangle.Color = Color.Red;
            else
                Rectangle.Color = Color.Green;

                //stopwatch.Stop();

                //Debug.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
                //totalTime += stopwatch.Elapsed.TotalMilliseconds;
                //count++;

                //Debug.WriteLine($"Average Time: {AverageTime} ms, Total Time: {totalTime}");
            }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here


            //Stopwatch stopwatch = new();
            //stopwatch.Start();

            SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                Camera.Transformation);

            //tileMap.Draw(SpriteBatch, Camera.Transformation);
            //DungeonMap.Draw(SpriteBatch);

            //Sprite.Draw(SpriteBatch);
            //Sprite2.Draw(SpriteBatch);

            //ShapeManager.DrawOutline(SpriteBatch);
            //ShapeManager.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 12);
            //ShapeManager.DrawTriangulated(SpriteBatch);
            ShapeManager.DrawTriangulatedUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //ShapeManager.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //ShapeManager.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //ShapeManager.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 4);

            foreach (var shape in Shapes)
            {
                shape.DrawTriangulated(SpriteBatch);
            }

            Rectangle.DrawFilled(SpriteBatch);

            Player.Draw(SpriteBatch);
            SpriteBatch.End();



            //stopwatch.Stop();

            //Debug.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            //totalTime += stopwatch.Elapsed.TotalMilliseconds;
            //count++;

            //Debug.WriteLine($"Average Time: {AverageTime} ms, Total Time: {totalTime}");
        }

        public static Texture2D CreateTextureFromColor(Color color)
        {
            Texture2D texture = new(GraphicsDeviceManager.GraphicsDevice, 1, 1);
            texture.SetData([color]);

            return texture;
        }


        public void CreateShapes()
        {
            Random = new Random();
            Shapes.Clear();

            float x, y;
            int r, g, b, alpha;
            Vector2 position;
            Color color;
            int speed;

            for (int i = 0; i < 5; i++)
            {
                x = Random.Next(-400, 400);
                y = Random.Next(-400, 400);

                r = Random.Next(0, 256);
                g = Random.Next(0, 256);
                b = Random.Next(0, 256);
                alpha = Random.Next(150, 256);
                speed = Random.Next(60, 151);

                color = new(r, g, b, alpha);
                position = new(x, y);
                /*
                if (Random.Next(0, 2) == 0)
                {
                    Shapes.Add(new Circle(new(Random.Next(-500, 500), Random.Next(-500, 500)), Random.Next(20, 100), Random.Next(3, 64), Color.Aquamarine));
                }
                else
                {
                    Shapes.Add(new Star(new(Random.Next(-500, 500), Random.Next(-500, 500)), Color.Aquamarine, Random.Next(3, 10), Random.Next(50, 100), Random.Next(10, 50)));
                }
                */

                Shapes.Add(new Star(position, color, Random.Next(3, 10), Random.Next(70, 150), Random.Next(40, 70))
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });

                Shapes.Add(new Circle(position, Random.Next(20, 100), Random.Next(3, 64), color)
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });

                Shapes.Add(new Shapes.Rectangle(position, Random.Next(50, 150), Random.Next(50, 150), color)
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });

                /*
                Shapes.Add(new
                    Triangle(position,
                    position + new Vector2(0, 100), 
                    position + new Vector2(50, 0))
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });
                */
            }
        }
    }
}
