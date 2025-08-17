using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public static Player Player { get; set; } = new();
        Camera Camera { get; set; }
        ShapeManager ShapeManager { get; set; } = new();

        private TileMap tileMap;
        private TileMap DungeonMap; 

        Shapes.Rectangle Rectangle { get; set; }
        Shapes.Rectangle TestingBounds { get; set; }

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
            Player.Texture = CreateTextureFromColor(Color.White);
            Player.Position = new(-40, -100);
            Player.Color = Color.DarkBlue;
            Camera = new(ScreenWidth, ScreenHeight);

            Rectangle = new(new Vector2(0, 0), 100, 100, Color.Green)
            {
                Rotate = true
            };

            TestingBounds = new(new(0, 0), 1000, 1000, Color.Crimson);

            ShapeManager.AddShape(Rectangle);

            ShapeManager.AddRandomShapes(30, new(0, 0), new(800, 800));
            var ellipse = new Ellipse(new Vector2(200, 200), 80, 40, Color.Orange, 64) { Rotate = true };
            ShapeManager.AddShape(ellipse);
            ShapeManager.AddShape(Rectangle);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            tileMap = new(Content, "Content/Catacombs1.tmx", GraphicsDevice);

            DungeonMap = new(Content, "Content/Dungeon.tmx", GraphicsDevice);
            EntityLayer entityLayer = new();
            entityLayer.AddEntity(Player);

            //DungeonMap.AddLayer(entityLayer);
            DungeonMap.InsertLayerAt(2, entityLayer);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Camera.Update(Player.Position);
            Player.Update(gameTime);

            //DungeonMap.Update(gameTime);

            base.Update(gameTime);

            ShapeManager.Update(gameTime);
            TestingBounds.Update(gameTime); 
            
            foreach (var shape in ShapeManager.Shapes)
            {
                if (shape.Intersects(Player.ShapeRectangle))
                    shape.Color = Color.Red;
                else
                    shape.Color = shape.DefaultColor;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

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
            SpriteBatch.End();

            SpriteBatch.Begin(
               SpriteSortMode.Deferred,
               BlendState.AlphaBlend,
               SamplerState.PointClamp,
               null,
               null,
               null,
               Camera.Transformation);

            //ShapeManager.DrawOutline(SpriteBatch);
            ShapeManager.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 12);
            //ShapeManager.DrawTriangulated(SpriteBatch);
            //ShapeManager.DrawTriangulatedUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //ShapeManager.DrawOutlineUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //ShapeManager.DrawFilledUsingPrimitives(GraphicsDevice, Camera.Transformation);
            //ShapeManager.DrawOutlineThickUsingPrimitives(GraphicsDevice, Camera.Transformation, 4);

            Player.Draw(SpriteBatch);

            TestingBounds.DrawOutline(SpriteBatch);
            SpriteBatch.End();
        }

        public static Texture2D CreateTextureFromColor(Color color)
        {
            Texture2D texture = new(GraphicsDeviceManager.GraphicsDevice, 1, 1);
            texture.SetData([color]);

            return texture;
        }
    }
}
