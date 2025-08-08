using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TiledRenderTest.Engine;
using TiledRenderTest.Entities;
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
            Player.Position = new(-40, -100);
            Player.Color = Color.DarkBlue;
            Camera = new(ScreenWidth, ScreenHeight);

            Rectangle = new(new Vector2(0, 0), 100, 100, Color.Green)
            {
                Rotate = true
            };

            ShapeManager.AddShape(Rectangle);

            ShapeManager.AddRandomShapes(1, new(-400, -400), new(400, 400));
            var ellipse = new Ellipse(new Vector2(200, 200), 80, 40, Color.Orange, 64) { Rotate = true };
            ShapeManager.AddShape(ellipse);
            ShapeManager.AddShape(Rectangle);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            tileMap = new(Content, "Content/Catacombs1.tmx");

            DungeonMap = new(Content, "Content/Dungeon.tmx");

            //TileMap mapTest = new(TmxReader.LoadMapFromTmx("Content/Dungeon.tmx", Content));
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

            foreach (var shape in ShapeManager.Shapes)
            {
                shape.Update(gameTime);

                if (shape.Intersects(Player.ShapeRectangle))
                   shape.Color = Color.Red;
                else
                    shape.Color = shape.DefaultColor;
            }

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

            //Rectangle.DrawFilled(SpriteBatch);

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
    }
}
