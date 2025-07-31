using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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

        public Shapes.Rectangle Rectangle { get; set; } = new(100, 100, 100, 100);
        public Shapes.Rectangle Rectangle2 { get; set; } = new(0, 0, 200, 200);
        public Shapes.Rectangle Rectangle3 { get; set; } = new(100, 100, 100, 100, Color.RoyalBlue);

        public Star Star { get; set; } = new(new(-350, -50), Color.DarkSlateGray, 6);
        public Star Star2 { get; set; } = new(new(0, 0), Color.DarkSlateGray, 5);
        public Circle Circle { get; set; } = new(new(-120, -120), 40, 32, Color.Orange);
        public Circle Circle2 { get; set; } = new(new(0, 0), 100, 50, Color.Aquamarine);
        public Circle Circle3 { get; set; } = new(new(200, 200), 300, 50, Color.Aquamarine);
        public Circle Circle4 { get; set; } = new(new(-150, 200), 100, 12, Color.Crimson) { Rotate = true };
        public Polygon Polygon { get; set; } = new();
        public Triangle Triangle { get; set; } = new(new(0,0), new(200,200), new(0,200));
        internal List<TileMap> Maps { get; set; } = [];

        private TileMap tileMap;
        private TileMap DungeonMap;

        List<Shape> Shapes { get; set; } = [];
        public Random Random { get; set; } = new();

        public double totalTime = 0f;
        public int count = 0;   
        public double AverageTime => count > 0 ? totalTime / count : 0;

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
            Player.Texture = CreateTextureFromColor(Color.DarkBlue);
            Player.Position = new(-40, 0);
            Camera = new(ScreenWidth, ScreenHeight);


            //Vector2[] points = [new(0,0), new(50,50), new(50,100), new(100,100), new(120,120), new(150,150), new(150,120), new(100,120)];

            //Polygon.SetPoints(points);

            //Star2.SetData(80, 30, 5);
            Star.Rotate = true;
            Triangle.Rotate = true;
            //Rectangle.Rotate = true;
            CreateShapes();
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

        }

        public void UpdateShapes(GameTime gameTime)
        {
            //Stopwatch stopwatch = new();
            //stopwatch.Start();

            foreach (var shape in Shapes)
            {
                shape.Update(gameTime);
            }
            Star.Update(gameTime);
            Triangle.Update(gameTime);
            //Rectangle.Update(gameTime);
            Circle4.Update(gameTime);

            //var options = new ParallelOptions
            //{
            //    MaxDegreeOfParallelism = Environment.ProcessorCount / 2
            //};

            //Parallel.For(0, Shapes.Count, i =>
            //{
            //    Shapes[i].Update(gameTime);
            //});


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

            //base.Draw(gameTime);

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
            Player.Draw(SpriteBatch);

            //Line.Draw(SpriteBatch);
            //Rectangle2.DrawOutline(SpriteBatch, Color.Violet);
            //Rectangle.DrawOutlineUsingPrimitives(SpriteBatch);
            //Rectangle.DrawOutline(SpriteBatch);
            //Rectangle3.Draw(SpriteBatch);
            //Rectangle3.DrawOutline(SpriteBatch);
            //Line2.Draw(SpriteBatch);
            //Rectangle.DrawOutLineWithTriangles(SpriteBatch);

            //Star.DrawOutline(SpriteBatch);
            //Polygon.DrawOutline(SpriteBatch, 2);

            Triangle.DrawOutline(SpriteBatch);
            Circle4.DrawOutlineWithTriangles(SpriteBatch);
            
            //Circle4.DrawFilled(SpriteBatch, Color.Blue);

            //Star.DrawOutLineWithTriangles(SpriteBatch);
            //Star.DrawOutLineWithTriangles(SpriteBatch);

            //Star.Draw(SpriteBatch);

            //Circle.DrawOutline(SpriteBatch);
            //Circle2.DrawOutline(SpriteBatch);
            //Circle2.DrawOutlineWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Circle.DrawOutLineWithTriangles(SpriteBatch);

            
            foreach (var shape in Shapes)
            {
                //shape.TestDraw(SpriteBatch);
                //shape.DrawOutline(SpriteBatch);
                //shape.DrawOutLineWithTriangles(SpriteBatch);
                //shape.DrawFilled(SpriteBatch);
            }


            //Circle3.DrawOutLineWithTriangles(SpriteBatch);

            SpriteBatch.End();

            foreach (var shape in Shapes)
            {
                shape.DrawOutlineWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation);
                //shape.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);
                //shape.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            }

            //Circle3.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Circle2.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 4);

            //Rectangle.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Line2.DrawThickUsingPrimitives(GraphicsDevice, Camera.Transformation);

            //Star.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Star.DrawOutlineWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Star.DrawOutlineThickWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation, 2);
            //Star2.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 3);
            //Star2.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);

            //Star2.DrawOutlineThickWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation, 2);

            //Triangle.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Triangle.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 5);

            //Rectangle.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 5);
            //Rectangle2.DrawOutlineThickWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation, 4);

            //Triangle.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Triangle.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 3);
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

            for (int i = 0; i < 20; i++)
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

                //Shapes.Add(new Star(position, color, Random.Next(3, 10), Random.Next(70, 150), Random.Next(40, 70))
                //{
                //    Rotate = true,
                //    RotationSpeedDegreesPerSecond = speed,
                //});

                Shapes.Add(new
                    Triangle(position,
                    position + new Vector2(0, 100), 
                    position + new Vector2(50, 0))
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });
            }
        }
    }
}
