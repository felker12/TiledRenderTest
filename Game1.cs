using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Line Line { get; set; } = new(new Vector2(100, 100), new Vector2(200, 200), Color.Red) { Thickness = 8 };
        public Line Line2 { get; set; } = new(new Vector2(200, 200), new Vector2(300, 300), Color.BlueViolet) { Thickness = 8};
        public Shapes.Rectangle Rectangle { get; set; } = new(100, 100, 100, 100);
        public Shapes.Rectangle Rectangle2 { get; set; } = new(0, 0, 200, 200);
        public Shapes.Rectangle Rectangle3 { get; set; } = new(100, 100, 100, 100, Color.RoyalBlue);

        public Star Star { get; set; } = new(new(-350, -50), Color.DarkSlateGray, 6);
        public Star Star2 { get; set; } = new(new(0, 0), Color.DarkSlateGray, 5);
        public Circle Circle { get; set; } = new(new(80, 80), 40, 32, Color.Orange);
        public Circle Circle2 { get; set; } = new(new(0, 0), 100, 50, Color.Aquamarine);
        public Circle Circle3 { get; set; } = new(new(200, 200), 300, 50, Color.Aquamarine);
        public Polygon Polygon { get; set; } = new();

        public Triangle Triangle { get; set; } =new(new(0,0), new(200,200), new(0,200));


        internal List<TileMap> Maps { get; set; } = [];


        private TileMap tileMap;
        private TileMap DungeonMap;

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


            Vector2[] points = [new(0,0), new(50,50), new(50,100), new(100,100), new(120,120), new(150,150), new(150,120), new(100,120)
                ];

            Polygon.Points = points;

            //Star2.SetData(80, 30, 5);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here



            tileMap = new(Content);
            tileMap.LoadFromTmx("Content/Catacombs1.tmx");

            DungeonMap = new(Content);
            DungeonMap.LoadFromTmx("Content/Dungeon.tmx");

            //Debug.WriteLine("\n");
            //Debug.WriteLine(tileMap.ToString());
            //Debug.WriteLine(DungeonMap.ToString());

            //Debug.WriteLine("\n");
            foreach (var layer in tileMap.Layers)
            {
                //Debug.WriteLine(layer.ToString());
            }
            //Debug.WriteLine("\n");
            foreach (var layer in DungeonMap.Layers)
            {
                //Debug.WriteLine(layer.ToString());
            }
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
            Rectangle2.DrawOutline(SpriteBatch, Color.Violet);
            //Rectangle.DrawOutlineUsingPrimitives(SpriteBatch);
            //Rectangle.DrawOutline(SpriteBatch);
            //Rectangle3.Draw(SpriteBatch);
            //Rectangle3.DrawOutline(SpriteBatch);
            //Line2.Draw(SpriteBatch);
            Rectangle.DrawOutLineWithTriangles(SpriteBatch);

            //Star.DrawOutline(SpriteBatch);

            //Polygon.DrawOutline(SpriteBatch, 2);


            Triangle.DrawOutline(SpriteBatch);

            //Star.DrawStarOutLineWithTriangles(SpriteBatch);
            Star.DrawOutLineWithTriangles(SpriteBatch);


            //Circle.DrawOutline(SpriteBatch);
            Circle2.DrawOutline(SpriteBatch);
            //Circle.DrawOutLineWithTriangles(SpriteBatch);


            //Circle3.DrawOutLineWithTriangles(SpriteBatch);


            SpriteBatch.End();

            Circle3.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);

            //Rectangle.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Line2.DrawThickUsingPrimitives(GraphicsDevice, Camera.Transformation);

            //Star.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Star2.DrawThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 3);
            Star2.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);

            //Star2.DrawStarOutlineThickWithTrianglesUsingPrimitives(GraphicsDevice, Camera.Transformation, 2);

            //Triangle.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Triangle.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 5);

            //Rectangle.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 5);

            //Triangle.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //Triangle.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 3);
            }

        public static Texture2D CreateTextureFromColor(Color color)
        {
            Texture2D texture = new(GraphicsDeviceManager.GraphicsDevice, 1, 1);
            texture.SetData([color]);

            return texture;
        }
    }
}
