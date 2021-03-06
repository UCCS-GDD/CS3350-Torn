using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Torn
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Sprite initialScreen;
        Vector2 initialPosition, finalPosition;
        bool started, menu, menu1, emptyLoad, levelEditor, selectLevel, isDead, allowHead, allowArms, allowLegs, openingAnimation, levelEndAnimation;
        KeyboardState keyboard;
        UserControlledSprite[] body;
        UserControlledSprite builder;
        Sprite blindness, highlightedGrass;
        int[,] field;
        int[,] bField;
        List<Vector2> blockSight;
        List<Sprite> trench;
        List<Sprite> obstacles;
        List<BridgePlate> bridgePlates;
        List<Vector2> bridgesPosition;
        List<Vector2> platesPosition;
        List<Sprite> brokenBridges;
        List<Sprite> grasses;
        List<Sprite> walls;
        List<Sprite> plates;
        List<Sprite> bridges;
        List<Sprite> tempField;
        List<Sprite> blockers;
        Sprite opening;
        Sprite levelComplete;
        Sprite finalArms, finalHead, finalLegs;
        int lastLevel, custLevelNbr;

        int[] finished;
        Sprite aux, textBox, gameMenu, arrow, gameMenu1, emptyLoadScreen, emptyField, died;
        List<Vector2> indexesBellow, indexesRight, indexesLeft, indexesAbove;
        List<Sprite> hiddenAbove, hiddenBellow, hiddenRight, hiddenLeft;
        Song ambientSound;
        SoundEffect kick, nice, pressurePlateOn, thrownHead, didIt, falling, gameOver, splitting, tryAgain;
        Text text, textAux;
        List<Text> levels;
        String tutorial;
        int countBellow, countAbove, countLeft, countRight, messageIndex, partMoving;
        KeyboardState old;
        int levelNumber, levelAux, bridgesNumber;
        Sprite enter, levelList;
        float counter;
        List<Sprite> bridgeOn, bridgeOff;
        String customLevelNbrAux;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;

            ambientSound = Content.Load<Song>(@"Sounds\ambientSound");
            MediaPlayer.Play(ambientSound);
            MediaPlayer.IsRepeating = true;

            counter = 300;
            lastLevel = 0;
            custLevelNbr = 0;
            customLevelNbrAux = "0";

            //Initialize all the global variables
            MyGlobals.width = 480 * 2;
            MyGlobals.heigh = 420 * 2;
            MyGlobals.realBlockSize = 30;
            MyGlobals.blockSize = MyGlobals.realBlockSize * 2;
            MyGlobals.scale = 1.0f * 2;

            MyGlobals.steps = 10;
            MyGlobals.empty = 0;
            MyGlobals.trench = 1;
            MyGlobals.obstacle = 2;
            MyGlobals.head = 3;
            MyGlobals.arms = 4;
            MyGlobals.legs = 5;
            MyGlobals.final = 6;
            MyGlobals.blocksVision = 7;
            MyGlobals.plates = 8;
            MyGlobals.bridges = 9;
            MyGlobals.outOfSize = 10;
            MyGlobals.numberOfBlocksX = 16;
            MyGlobals.numberOfBlocksY = 14;

            tutorial = "Welcome to Torn. Currently, your body is separated into three pieces. You \ncan alternate between these parts pressing A to select Arms, H to select \nhead and L to select legs.";

            graphics.PreferredBackBufferWidth = MyGlobals.width;
            graphics.PreferredBackBufferHeight = MyGlobals.heigh;

            messageIndex = 0;
            levelNumber = 1;
            levelAux = levelNumber;
            bridgesNumber = 0;
            partMoving = 0;
            
            body = new UserControlledSprite[3];
            started = false;
            menu = false;
            menu1 = false;
            emptyLoad = false;
            levelEditor = false;
            selectLevel = false;
            allowArms = false;
            allowHead = false;
            allowLegs = false;
            openingAnimation = false;
            levelEndAnimation = false;
            isDead = false;
            field = new int[MyGlobals.numberOfBlocksY, MyGlobals.numberOfBlocksX];
            bField = new int[MyGlobals.numberOfBlocksY, MyGlobals.numberOfBlocksX];
            trench = new List<Sprite>();
            obstacles = new List<Sprite>();
            bridgePlates = new List<BridgePlate>();
            bridgesPosition = new List<Vector2>();
            platesPosition = new List<Vector2>();
            brokenBridges = new List<Sprite>();
            blockSight = new List<Vector2>();
            walls = new List<Sprite>();
            plates = new List<Sprite>();
            bridges = new List<Sprite>();
            bridgeOn = new List<Sprite>();
            bridgeOff = new List<Sprite>();
            blockers = new List<Sprite>();

            indexesAbove = new List<Vector2>();
            indexesBellow = new List<Vector2>();
            indexesLeft = new List<Vector2>();
            indexesRight = new List<Vector2>();

            hiddenAbove = new List<Sprite>();
            hiddenBellow = new List<Sprite>();
            hiddenLeft = new List<Sprite>();
            hiddenRight = new List<Sprite>();

            tempField = new List<Sprite>();
            finished = new int[3] { 0, 0, 0 };

            levels = new List<Text>();

            old = new KeyboardState();

        }
        protected override void Initialize()
        {
            //Makes the mouse pointer visible
            this.IsMouseVisible = true;
            base.Initialize();
        }
        public void readField(String levelName)
        {
            string line;
            int i = 0, j = 0, k = 0, lastJ = 0, lastI = 0, result = 0, rest = 0;
            System.IO.StreamReader file;
            //Try catcher
            if(custLevelNbr == 0)
                file = new System.IO.StreamReader(@"Levels\level" + levelName + ".txt");
            else
                file = new System.IO.StreamReader(@"Levels\Custom\" + custLevelNbr + ".txt");
            

            while ((line = file.ReadLine()) != null)
            {
                while (k < line.Length)
                {
                    if (k == line.Length)
                    {
                        field[i, j] = int.Parse(line[k].ToString());
                        j++;
                        k++;
                    }
                    else if(k < line.Length - 1 && line[k+1] == ' ')
                    {
                        field[i, j] = int.Parse(line[k].ToString());
                        j++;
                        k++;
                    }
                    else if(k == line.Length - 1)
                    {
                        field[i, j] = int.Parse(line[k].ToString());
                        j++;
                        k++;
                    }
                    else if(line[k] == ' ')
                    {
                        k++;
                    }
                    else
                    {
                        field[i, j] = int.Parse(line[k].ToString() + line[k + 1].ToString());
                        j++;
                        k = k + 2;
                    }
                }
                lastJ = j;
                j = 0;
                k = 0;
                i++;
            }

            lastI = i;

            if (lastJ < MyGlobals.numberOfBlocksX)
            {
                int aux = MyGlobals.numberOfBlocksX - lastJ;

                result = aux / 2;
                rest = aux - result;

                for (i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (j = MyGlobals.numberOfBlocksX - rest -1; j >= result; j--)
                    {
                        field[i, j] = field[i, j - result];
                    }
                }

                for (i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (j = 0; j < result; j++)
                    {
                        field[i, j] = MyGlobals.outOfSize;
                    }
                    for (j = MyGlobals.numberOfBlocksX -1; j > MyGlobals.numberOfBlocksX - rest - 1; j--)
                    {
                        field[i, j] = MyGlobals.outOfSize;
                    }
                }
            }
            MyGlobals.zeroX = MyGlobals.blockSize * result;
            MyGlobals.realWidth = MyGlobals.blockSize * lastJ + MyGlobals.blockSize * result;

            if (lastI < MyGlobals.numberOfBlocksY)
            {
                int aux = MyGlobals.numberOfBlocksY - lastI;
                
                result = aux / 2;
                rest = aux - result;

                for (i = 0; i < MyGlobals.numberOfBlocksX; i++)
                {
                    for (j = MyGlobals.numberOfBlocksY - rest - 1; j >= result; j--)
                    {
                        field[j, i] = field[j - result, i];
                    }
                }

                for (i = 0; i < MyGlobals.numberOfBlocksX; i++)
                {
                    for (j = 0; j < result; j++)
                    {
                        field[j, i] = MyGlobals.outOfSize;
                    }
                    for (j = MyGlobals.numberOfBlocksY - 1; j > MyGlobals.numberOfBlocksY - rest - 1; j--)
                    {
                        field[j, i] = MyGlobals.outOfSize;
                    }
                }
            }

            MyGlobals.zeroY = MyGlobals.blockSize * result;
            MyGlobals.realHeigh = MyGlobals.blockSize * lastI + MyGlobals.blockSize * result;
            file.Close();
            
            //Just for test of the resulting field
            //using (TextWriter tw = new StreamWriter(@"Levels\levelTeste2222.txt"))
            //{
            //    for (j = 0; j < MyGlobals.numberOfBlocksY; j++)
            //    {
            //        for (i = 0; i < MyGlobals.numberOfBlocksX; i++)
            //        {
            //            tw.Write(field[j, i] + " ");
            //        }
            //        tw.WriteLine();
            //    }
            //}
            
        }
        protected override void LoadContent()
        {
            //Displays the initial screen
            
            spriteBatch = new SpriteBatch(GraphicsDevice);


            emptyLoadScreen = new Sprite(this, @"Images\emptyLoad", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            emptyField = new Sprite(this, @"Images\emptyField", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            builder = new UserControlledSprite(this, @"Images\elements", new Vector2(MyGlobals.realBlockSize, MyGlobals.realBlockSize), false, false);
            builder.Rectangle = new Rectangle(0, 0, (int) MyGlobals.realBlockSize, (int) MyGlobals.realBlockSize);
            builder.change = true;
            builder.Walls = new List<Sprite>();
            builder.Obstacles = new List<Sprite>();
            builder.Trench = new List<Sprite>();
            builder.Bridge = new List<Sprite>();
            builder.Indexes = new List<Vector2>();
            readField(levelNumber.ToString());
            highlightedGrass = new Sprite(this, @"Images\highlightedGrass", new Vector2(0, 0));
            grasses = new List<Sprite>();

            gameMenu = new Sprite(this, @"Images\menu", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            gameMenu1 = new Sprite(this, @"Images\menu1", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            arrow = new Sprite(this, @"Images\arrow", new Vector2(100, 210));
            levelList = new Sprite(this, @"Images\levelList", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            died = new Sprite(this, @"Images\died", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));


            opening = new Sprite(this, @"Images\opening", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            opening.Rectangle = new Rectangle(0, 0, 480, 420);
            opening.Center2 = new Vector2(MyGlobals.width / 4, MyGlobals.heigh / 4);
            levelComplete = new Sprite(this, @"Images\level_complete", new Vector2(MyGlobals.width / 2, MyGlobals.heigh / 2));
            levelComplete.Rectangle = new Rectangle(0, 0, 480, 420);
            levelComplete.Center2 = new Vector2(MyGlobals.width / 4, MyGlobals.heigh / 4);

            if (isDead && !started)
            {
                Components.Add(died);
                Components.Add(arrow);
            }
            else if (!started && !menu && !menu1) 
            {
                kick = Content.Load<SoundEffect>(@"Sounds\Kick");
                nice = Content.Load<SoundEffect>(@"Sounds\Nice");
                pressurePlateOn = Content.Load<SoundEffect>(@"Sounds\PressurePlateOn");
                thrownHead = Content.Load<SoundEffect>(@"Sounds\ThrownHead");
                didIt = Content.Load<SoundEffect>(@"Sounds\didIt");
                falling = Content.Load<SoundEffect>(@"Sounds\Falling");
                gameOver = Content.Load<SoundEffect>(@"Sounds\GameOver");
                splitting = Content.Load<SoundEffect>(@"Sounds\SplittingApart");
                tryAgain = Content.Load<SoundEffect>(@"Sounds\TryAgain");

                initialPosition.X = graphics.PreferredBackBufferWidth / 2;
                initialPosition.Y = graphics.PreferredBackBufferHeight / 2;
                initialScreen = new Sprite(this, @"Images\InitialScreen", initialPosition);
                Components.Add(initialScreen);
            }
            else if (!started && menu && !menu1)
            {
                Components.Remove(initialScreen);
                Components.Add(gameMenu);
                Components.Add(arrow);
            }
            else if(!started && menu1 && !menu)
            {
                Components.Add(gameMenu1);
                Components.Add(arrow);

            }
            else if(levelFinished())
            {
                for (int i = Components.Count -1; i > 0 ; i--)
                {
                    Components.RemoveAt(i);
                }

                trench = new List<Sprite>();
                obstacles = new List<Sprite>();
                bridgesPosition = new List<Vector2>();
                platesPosition = new List<Vector2>();
                brokenBridges = new List<Sprite>();
                blockSight = new List<Vector2>();
                walls = new List<Sprite>();
                plates = new List<Sprite>();
                bridges = new List<Sprite>();
                bridgeOn = new List<Sprite>();
                grasses = new List<Sprite>();

                indexesAbove = new List<Vector2>();
                indexesBellow = new List<Vector2>();
                indexesLeft = new List<Vector2>();
                indexesRight = new List<Vector2>();

                hiddenAbove = new List<Sprite>();
                hiddenBellow = new List<Sprite>();
                hiddenLeft = new List<Sprite>();
                hiddenRight = new List<Sprite>();


                finished[0] = 0;
                finished[1] = 0;
                finished[2] = 0;
                readField(levelNumber.ToString());
            }

            if (started && !menu1 && !menu)
            {
                //Takes the initial screen off and displays the body parts
                Components.Remove(arrow);
                Components.Remove(gameMenu);

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                        initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                        aux = new Sprite(this, @"Images\grass", initialPosition);
                        grasses.Add(aux);
                        Components.Add(aux);
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.outOfSize)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\edge", initialPosition);
                            Components.Add(aux);
                            walls.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] > MyGlobals.plates * 10 && field[i, j] / 10 == MyGlobals.plates)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\pressureplate_grass", initialPosition);
                            
                            for (int k = 0; k < MyGlobals.numberOfBlocksY; k++)
                            {
                                for (int l = 0; l < MyGlobals.numberOfBlocksX; l++)
                                {
                                    if (field[k, l] / 10 == MyGlobals.bridges && field[k, l] % 10 == field[i, j] % 10)
                                    {
                                        initialPosition.X = (l + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        initialPosition.Y = (k + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        aux.EquivBridges.Add(initialPosition);
                                    }
                                }
                            }
                            plates.Add(aux);
                        }
                        else if (field[i, j] == MyGlobals.plates)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\pressureplate_grass", initialPosition);
                            for (int k = 0; k < MyGlobals.numberOfBlocksY; k++)
                            {
                                for (int l = 0; l < MyGlobals.numberOfBlocksX; l++)
                                {
                                    if (field[k, l] / 10 == MyGlobals.bridges)
                                    {
                                        initialPosition.X = (l + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        initialPosition.Y = (k + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        aux.EquivBridges.Add(initialPosition);
                                    }
                                }
                            }
                            plates.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < plates.Count; i++)
                {
                    for (int j = 0; j < plates[i].EquivBridges.Count; j++)
                    {
                        aux = new Sprite(this, @"Images\bridge", plates[i].EquivBridges[j]);
                        aux.Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                        walls.Add(aux);
                        bridges.Add(aux); 
                        Components.Add(aux);
                    }
                    Components.Add(plates[i]);
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.trench)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\pit", initialPosition);
                            trench.Add(aux);
                            Components.Add(aux);
                        }
                    }
                }
                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.obstacle)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\Block", initialPosition);
                            aux.Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                            obstacles.Add(aux);
                            Components.Add(aux);
                        }
                        else if (field[i, j] == MyGlobals.final)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            finalPosition = initialPosition;
                            aux = new Sprite(this, @"Images\final", initialPosition);

                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i) * MyGlobals.blockSize + 28;
                            finalArms = new Sprite(this, @"Images\finalArms", initialPosition);

                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i) * MyGlobals.blockSize + 8;
                            finalHead = new Sprite(this, @"Images\finalHead", initialPosition);

                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i) * MyGlobals.blockSize + 47;
                            finalLegs = new Sprite(this, @"Images\finalLegs", initialPosition);
                            Components.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
			    {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
			        {
                        if (field[i, j] == MyGlobals.head)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[0] = new UserControlledSprite(this, @"Images\Head", initialPosition, false, false);
                            Components.Add(body[0]);
                        }
                        if (field[i, j] == MyGlobals.arms)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i+ 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[1] = new UserControlledSprite(this, @"Images\Arms", initialPosition, true, false);
                            Components.Add(body[1]);
                        }
                        if (field[i, j] == MyGlobals.legs)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[2] = new UserControlledSprite(this, @"Images\Legs", initialPosition, false , true);
                            Components.Add(body[2]);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if(field[i, j] == MyGlobals.final)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[0].Final = initialPosition;
                            body[1].Final = initialPosition;
                            body[2].Final = initialPosition;
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.blocksVision)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\blockers", initialPosition);
                            blockers.Add(aux);
                            blockSight.Add(aux.Position);
                            //trench.Add(aux);
                            walls.Add(aux);
                            Components.Add(aux);
                        }
                    }
                }

                //Initializes the blind sprinte with the same position of the head
                blindness = new Sprite(this, @"Images\Blindness", new Vector2(body[0].Position.X, body[0].Position.Y));

                body[0].Trench = trench;
                body[1].Trench = trench;
                body[2].Trench = trench;

                body[0].Walls = walls;
                body[1].Walls = walls;
                body[2].Walls = walls;

                body[1].Obstacles = obstacles;

                body[0].Body1 = body[1];
                body[0].Body2 = body[2];

                body[1].Body1 = body[0];
                body[1].Body2 = body[2];

                body[2].Body1 = body[0];
                body[2].Body2 = body[1];

                //Components.Add(blindness);
                //TUTORIAL
                if(levelNumber == 1)
                {
                    textBox = new Sprite(this, @"Images\blankBox", new Vector2(MyGlobals.width / 2, MyGlobals.blockSize * 2));
                    Components.Add(textBox);
                    text = new Text(this, new Vector2(20f, 80f), @"Verdana", tutorial, Color.Black);
                    Components.Add(text);
                    enter = new Sprite(this, @"Images\enter", new Vector2(MyGlobals.width - MyGlobals.blockSize / 2 , 3 * MyGlobals.blockSize - MyGlobals.blockSize / 2));
                    Components.Add(enter);
                }
            }
        }
        protected override void UnloadContent()
        {
           
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            keyboard = Keyboard.GetState();

            if(keyboard.IsKeyDown(Keys.Space) && !started && !menu && !menu1 && !levelEditor)
            {
                menu = true;
                LoadContent();
            }

            if(!started && isDead)
            {
                if(!keyboard.IsKeyDown(Keys.Down) && old.IsKeyDown(Keys.Down) && arrow.Position.Y < 330)
                {
                    arrow.Position = new Vector2(arrow.Position.X, arrow.Position.Y + MyGlobals.blockSize * 2);
                }
                else if (!keyboard.IsKeyDown(Keys.Up) && old.IsKeyDown(Keys.Up) && arrow.Position.Y > 210)
                {
                    arrow.Position = new Vector2(arrow.Position.X, arrow.Position.Y - MyGlobals.blockSize * 2);
                }

                if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && arrow.Position.Y == 210)
                {
                    isDead = false;
                    started = true;

                    finished[0] = 1;
                    finished[1] = 1;
                    finished[2] = 1;

                    LoadContent();
                }
                else if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && arrow.Position.Y == 330)
                {
                    this.Exit();
                }

                old = keyboard;

            }

            if(started)
            {
                if(!body[0].Walls.Contains(body[1]))
                    body[0].Walls.Add(body[1]);
                if (!body[0].Walls.Contains(body[2]))
                    body[0].Walls.Add(body[2]);

                if (!body[1].Walls.Contains(body[0]))
                    body[1].Walls.Add(body[0]);
                if (!body[1].Walls.Contains(body[2]))
                    body[1].Walls.Add(body[2]);

                if (!body[2].Walls.Contains(body[0]))
                    body[2].Walls.Add(body[0]);
                if (!body[2].Walls.Contains(body[1]))
                    body[2].Walls.Add(body[1]);

                body[0].Body1 = body[1];
                body[0].Body2 = body[2];

                body[1].Body1 = body[0];
                body[1].Body2 = body[2];

                body[2].Body1 = body[0];
                body[2].Body2 = body[1];

                for (int i = 0; i < blockers.Count; i++)
                {
                    Components.Remove(blockers[i]);
                    Components.Add(blockers[i]);
                }
            }

            if(started && die())
            {
                isDead = true;
                started = false;
                gameOver.Play();
                LoadContent();
            }

            if (emptyLoad)
            {
                if (Components.IndexOf(emptyLoadScreen) == -1)
                    Components.Add(emptyLoadScreen);
                if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    emptyLoad = false;
                    Components.Remove(emptyLoadScreen);
                }
                old = keyboard;
            }
            
            if (levelEditor)
            {
                if(!keyboard.IsKeyDown(Keys.Space) && old.IsKeyDown(Keys.Space))
                {
                    builder.Rectangle = new Rectangle(builder.Rectangle.X + (int) MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                    if (builder.Rectangle.X == 330)
                        builder.Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                }
                if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    if (bField[(int)((builder.Position.Y - MyGlobals.blockSize / 2) / MyGlobals.blockSize), (int)((builder.Position.X - MyGlobals.blockSize / 2) / MyGlobals.blockSize)] == 0)
                    {
                        bField[(int)((builder.Position.Y - MyGlobals.blockSize / 2) / MyGlobals.blockSize), (int)((builder.Position.X - MyGlobals.blockSize / 2) / MyGlobals.blockSize)] = builder.Rectangle.X / (int)MyGlobals.realBlockSize;
                        aux = new Sprite(this, @"Images\elements", builder.Position);
                        aux.Rectangle = builder.Rectangle;
                        tempField.Add(aux);
                        Components.Add(aux);
                        Components.Remove(builder);
                        Components.Add(builder);
                    }
                    else
                    {
                        for (int i = 0; i < tempField.Count; i++)
                        {
                            if(tempField[i].Position == builder.Position)
                            {
                                Components.Remove(tempField[i]);
                                tempField.RemoveAt(i);
                            }
                        }
                        bField[(int)((builder.Position.Y - MyGlobals.blockSize / 2) / MyGlobals.blockSize), (int)((builder.Position.X - MyGlobals.blockSize / 2) / MyGlobals.blockSize)] = builder.Rectangle.X / (int)MyGlobals.realBlockSize;
                        aux = new Sprite(this, @"Images\elements", builder.Position);
                        aux.Rectangle = builder.Rectangle;
                        tempField.Add(aux);
                        Components.Add(aux);
                        Components.Remove(builder);
                        Components.Add(builder);
                    }
                    
                }
                if (!keyboard.IsKeyDown(Keys.Escape) && old.IsKeyDown(Keys.Escape))
                {
                    bool hasHead = false, hasArms = false, hasLegs = false, hasFinal = false;
                    for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                    {
                        for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                        {
                            if (bField[i, j] == MyGlobals.head)
                                hasHead = true;
                            if (bField[i, j] == MyGlobals.arms)
                                hasArms = true;
                            if (bField[i, j] == MyGlobals.legs)
                                hasLegs = true;
                            if (bField[i, j] == MyGlobals.final)
                                hasFinal = true;
                        }
                    }

                    if (hasArms && hasHead && hasLegs && hasFinal)
                    {
                        Components.Remove(text);
                        DirectoryInfo di = new DirectoryInfo(@"Levels\Custom");
                        foreach (var fi in di.GetFiles())
                        {
                            lastLevel++;
                        }
                        lastLevel++;

                        using (TextWriter tw = new StreamWriter(@"Levels\Custom\" + lastLevel.ToString() + ".txt"))
                        {
                            for (int j = 0; j < MyGlobals.numberOfBlocksY; j++)
                            {
                                for (int i = 0; i < MyGlobals.numberOfBlocksX; i++)
                                {
                                    if (i == MyGlobals.numberOfBlocksX - 1)
                                        tw.Write(bField[j, i]);
                                    else
                                        tw.Write(bField[j, i] + " ");
                                }
                                tw.WriteLine();
                            }
                        }
                        menu = true;
                        levelEditor = false;
                        Components.Remove(emptyField);
                        Components.Remove(builder);
                        for (int i = 0; i < tempField.Count; i++)
                        {
                            Components.Remove(tempField[i]);
                        }
                        tempField = new List<Sprite>();
                    }
                    else
                    {
                        Components.Remove(text);
                        menu = true;
                        levelEditor = false;
                        Components.Remove(emptyField);
                        Components.Remove(builder);
                        for (int i = 0; i < tempField.Count; i++)
                        {
                            Components.Remove(tempField[i]);
                        }
                        tempField = new List<Sprite>();
                    }
                }
                old = keyboard;
            }

            if(selectLevel)
            {
                if (!keyboard.IsKeyDown(Keys.D0) && old.IsKeyDown(Keys.D0))
                {
                    customLevelNbrAux += '0';
                }
                if (!keyboard.IsKeyDown(Keys.D1) && old.IsKeyDown(Keys.D1))
                {
                    customLevelNbrAux += '1';
                }
                if (!keyboard.IsKeyDown(Keys.D2) && old.IsKeyDown(Keys.D2))
                {
                    customLevelNbrAux += '2';
                }
                if (!keyboard.IsKeyDown(Keys.D3) && old.IsKeyDown(Keys.D3))
                {
                    customLevelNbrAux += '3';
                }
                if (!keyboard.IsKeyDown(Keys.D4) && old.IsKeyDown(Keys.D4))
                {
                    customLevelNbrAux += '4';
                }
                if (!keyboard.IsKeyDown(Keys.D5) && old.IsKeyDown(Keys.D5))
                {
                    customLevelNbrAux += '5';
                }
                if (!keyboard.IsKeyDown(Keys.D6) && old.IsKeyDown(Keys.D6))
                {
                    customLevelNbrAux += '6';
                }
                if (!keyboard.IsKeyDown(Keys.D7) && old.IsKeyDown(Keys.D7))
                {
                    customLevelNbrAux += '7';
                }
                if (!keyboard.IsKeyDown(Keys.D8) && old.IsKeyDown(Keys.D8))
                {
                    customLevelNbrAux += '8';
                }
                if (!keyboard.IsKeyDown(Keys.D9) && old.IsKeyDown(Keys.D9))
                {
                    customLevelNbrAux += '9';
                }
                if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && !customLevelNbrAux.Equals("0"))
                {
                    if (Components.Contains(text))
                        Components.Remove(text);
                    selectLevel = false;
                    started = true;
                    custLevelNbr = int.Parse(customLevelNbrAux);
                    Components.Remove(levelList);
                    for (int i = 0; i < levels.Count; i++)
                    {
                        Components.Remove(levels[i]);
                    }
                    LoadContent();
                }
                if (!keyboard.IsKeyDown(Keys.Escape) && old.IsKeyDown(Keys.Escape))
                {
                    if (Components.Contains(text))
                        Components.Remove(text);
                    selectLevel = false;
                    menu = true;
                    Components.Remove(levelList);
                    for (int i = 0; i < levels.Count; i++)
                    {
                        Components.Remove(levels[i]);
                    }
                    LoadContent();
                }

                old = keyboard;
            }

            if(menu && openingAnimation)
            {
                if (!Components.Contains(opening))
                    Components.Add(opening);
                
                if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && opening.Rectangle.X < 1440)
                {
                    if (opening.Rectangle.X == 960)
                        splitting.Play();
                    opening.Rectangle = new Rectangle(opening.Rectangle.X + 480, 0, 480, 420);

                }
                else if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && opening.Rectangle.X >= 1440)
                {
                    menu = false;
                    started = true;
                    openingAnimation = false;
                    LoadContent();
                }
                
            }

            if(menu)
            {
                if(!keyboard.IsKeyDown(Keys.Down) && old.IsKeyDown(Keys.Down) && arrow.Position.Y < 690)
                {
                    arrow.Position = new Vector2(arrow.Position.X, arrow.Position.Y + MyGlobals.blockSize * 2);
                }
                else if (!keyboard.IsKeyDown(Keys.Up) && old.IsKeyDown(Keys.Up) && arrow.Position.Y > 210)
                {
                    arrow.Position = new Vector2(arrow.Position.X, arrow.Position.Y - MyGlobals.blockSize * 2);
                }

                if (arrow.Position.Y == 210 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    openingAnimation = true;
                }
                else if (arrow.Position.Y == 330 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(@"Save\save.txt");
                    String line;

                    while ((line = file.ReadLine()) != null)
                    {
                        if (int.Parse(line[0].ToString()) != 0)
                        {
                            levelNumber = int.Parse(line[0].ToString());
                            started = true;
                            menu = false;
                            LoadContent();
                        }
                        else
                        {
                            emptyLoad = true;
                        }
                    }
                }
                else if (arrow.Position.Y == 450 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && !levelEditor)
                {
                    Components.Add(emptyField);
                    Components.Add(builder);
                    levelEditor = true;
                    menu = false;
                    text = new Text(this, new Vector2(MyGlobals.blockSize * 5, MyGlobals.blockSize), "Verdana", "This feature still in development.", Color.Black);
                    Components.Add(text);
                }
                else if (arrow.Position.Y == 570 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    selectLevel = true;
                    menu = false;
                       
                    Components.Add(levelList);
                    text = new Text(this, new Vector2(MyGlobals.blockSize * 5, MyGlobals.blockSize), "Verdana", "This feature still in development.", Color.Black);
                    if (!Components.Contains(text))
                        Components.Add(text);
                    keyboard = Keyboard.GetState();

                    DirectoryInfo di = new DirectoryInfo(@"Levels\Custom");
                    int counter = 0;
                    foreach (var fi in di.GetFiles())
                    {
                        counter++;
                        textAux = new Text(this, new Vector2(90, 210), @"Verdana", "", Color.Black);
                        textAux.TextContent = fi.Name.Replace(".txt", "");
                        textAux.Position = new Vector2(textAux.Position.X, counter * MyGlobals.blockSize + textAux.Position.Y);
                        if (textAux.Position.Y > 720)
                        {
                            textAux.Position = new Vector2(textAux.Position.X + 60, 210);
                        }
                        levels.Add(textAux);
                        Components.Add(textAux);
                    }
                }
                else if (arrow.Position.Y == 690 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    this.Exit();
                }
                
                old = keyboard;
            }

            if(menu1)
            {
                if (!keyboard.IsKeyDown(Keys.Down) && old.IsKeyDown(Keys.Down) && arrow.Position.Y < 570)
                {
                    arrow.Position = new Vector2(arrow.Position.X, arrow.Position.Y + MyGlobals.blockSize * 2);
                }
                else if (!keyboard.IsKeyDown(Keys.Up) && old.IsKeyDown(Keys.Up) && arrow.Position.Y > 210)
                {
                    arrow.Position = new Vector2(arrow.Position.X, arrow.Position.Y - MyGlobals.blockSize * 2);
                }

                if (arrow.Position.Y == 210 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    menu1 = false;
                    started = true;
                    Components.Remove(gameMenu1);
                    Components.Remove(arrow);
                }
                else if(arrow.Position.Y == 330 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    menu1 = false;
                    started = true;
                    if (levelNumber == 1)
                    {
                        messageIndex = 1;
                        allowArms = false;
                        allowHead = false;
                        allowLegs = false;
                    }
                    finished[0] = 1;
                    finished[1] = 1;
                    finished[2] = 1;

                    LoadContent();
                }
                else if (arrow.Position.Y == 450 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(@"Save\save.txt");

                    file.WriteLine(levelNumber.ToString());

                    file.Close();


                    body[partMoving].change = true;

                    menu1 = false;
                    started = true;
                    Components.Remove(arrow);
                    Components.Remove(gameMenu1);
                    
                }
                else if (arrow.Position.Y == 570 && !keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                {
                    this.Exit();
                }
                old = keyboard;
            }


            

            if(started)
            {
                body[0].Obstacles = body[1].Obstacles;
                body[2].Obstacles = body[1].Obstacles;

                counter -= gameTime.ElapsedGameTime.Seconds;

                if(keyboard.IsKeyDown(Keys.Escape))
                {
                    if (body[0].change)
                        partMoving = 0;
                    else if (body[1].change)
                        partMoving = 1;
                    else if (body[2].change)
                        partMoving = 2;
                    body[0].change = false;
                    body[1].change = false;
                    body[2].change = false;
                    started = false;
                    menu1 = true;
                    LoadContent();
                }
            }

            //Tutorial
            if (started && levelNumber == 1)
            {
                if (body[0].Position == new Vector2(4 * MyGlobals.blockSize - MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize - MyGlobals.blockSize / 2))
                {
                    switch (messageIndex)
                    {
                        case 1:
                            text.TextContent = "Welcome to Torn. Currently, your body is separated into three pieces. You \ncan alternate between these parts pressing A to select Arms, H to select \nhead and L to select legs.";
                            if(Components.IndexOf(enter) == -1)    
                                Components.Add(enter);
                            break;
                        case 2:
                            text.TextContent = "As you can see, some areas of the maze are invisible and you will have to \nmove the head in order to see them and solve the puzzle. To do that, you \nmust join all of your body parts at the end of the level.";
                            break;
                        case 3:
                            text.TextContent = "Press H to select the head and then use the arrow keys to move it to the \nhighlighted area.";
                            Components.Remove(enter);
                            for (int i = 0; i < grasses.Count; i++)
                            {
                                if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                                    grasses[i].Color = Color.LightSeaGreen;
                            }
                            messageIndex = 4;
                            allowHead = true;
                            break;
                    }

                    if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && messageIndex < 3)
                        messageIndex++;

                    old = keyboard;

                }
                else if (body[0].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.White;
                    }
                    switch (messageIndex)
                    {
                        case 4:
                            allowHead = false;
                            body[0].change = false;
                            text.TextContent = "Now you can see and control your arms. Arms are able to push and pull \nblocks. They can also throw body parts by pressing Left Shift and the arrow keys.";
                            if(Components.IndexOf(enter) == -1)
                                Components.Add(enter);
                            break;
                        case 5:
                            text.TextContent = "Now, press A to select the arms and then move them to the highlighted \narea.";
                            Components.Remove(enter);
                            for (int i = 0; i < grasses.Count; i++)
                            {
                                if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                                    grasses[i].Color = Color.LightSeaGreen;
                            }
                            allowArms = true;
                            break;
                    }

                    if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && messageIndex > 3 && messageIndex < 5)
                        messageIndex++;

                    old = keyboard;
                }
                else if (body[0].Position == new Vector2(10 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(8 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.White;
                    }
                    text.TextContent = "Now place the head on the pressure plate in order to restore the bridge \non the top of the level. Doing that you will be able to jump your legs \nover the trenches";
                    allowArms = false;
                }
                else if (body[0].Position == new Vector2(11 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 6 * MyGlobals.blockSize + MyGlobals.blockSize / 2) && body[2].Position == new Vector2(4 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 4 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    text.TextContent = "Excellent! Now press L to select the legs and then jump over the trenches \npressing Left shift and the direction arrows.";
                    body[1].change = false;
                    body[0].change = false;
                    allowArms = false;
                    allowHead = false;
                    allowLegs = true;
                }


                if (body[1].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.White;
                    }
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(8 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.LightSeaGreen;
                    }
                    allowHead = true;
                    text.TextContent = "Now move the head to the highlighted area and then use the arm to throw it\nover the broken bridge. To throw or pull something, you have \nto press the left shift key.";
                }

                if (body[2].Position == new Vector2(8 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 4 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    text.TextContent = "Now, place the legs on the other pressure plate at the botton of the level. \nDoing this, will enable you to join all of your body parts \nat the final portal.";
                }

                if (body[2].Position == new Vector2(10 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 9 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    allowArms = true;
                    allowLegs = true;
                    allowHead = true;
                }

            }

            //Checks which part will move according with the user input, if the game is already started and if there is any other body part moving
            if (keyboard.IsKeyDown(Keys.H) && started && !isBodyMoving() && !menu1 && allowHead)
            {
                //If the head was selected, set the bool attribute 'change' in body[0] as true and in the other parts as false, in this way, only the head will move 
                body[0].change = true;
                body[1].change = false;
                body[2].change = false;
            }
            else if (keyboard.IsKeyDown(Keys.A) && started && !isBodyMoving() && !menu1 && allowArms)
            {
                //If the arms was selected, set the bool attribute 'change' in body[1] as true and in the other parts as false, in this way, only the arms will move
                body[0].change = false;
                body[1].change = true;
                body[2].change = false;    
            }
            else if (keyboard.IsKeyDown(Keys.L) && started && !isBodyMoving() && !menu1 && allowLegs)
            {
                //If the legs was selected, set the bool attribute 'change' in body[2] as true and in the other parts as false, in this way, only the legs will move
                body[0].change = false;
                body[1].change = false;
                body[2].change = true;
            }
            //Kicks the body parts
            if (started && body[2].change && !menu1)
            {
                if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Down))
                {
                    if (body[2].kick(body[0], 'd'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y + MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'd'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X, body[1].Position.Y + MyGlobals.blockSize * 2);
                            kick.Play();
                    }
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Up))
                {
                    if (body[2].kick(body[0], 'u'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y - MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'u'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X, body[1].Position.Y - MyGlobals.blockSize * 2);
                        kick.Play();
                    }
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Left))
                {
                    if (body[2].kick(body[0], 'l'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X - MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'l'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X - MyGlobals.blockSize * 2, body[1].Position.Y);
                        kick.Play();
                    }
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Right))
                {
                    if (body[2].kick(body[0], 'r'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X + MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'r'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X + MyGlobals.blockSize * 2, body[1].Position.Y);
                        kick.Play();
                    }
                }

                if (body[0].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[0].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize/2, body[0].Position.Y);
                if (body[1].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[1].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize/2, body[1].Position.Y);

                if (body[0].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[1].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize/2);
                if (body[1].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[1].Position = new Vector2(body[1].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize/2);

                if (body[0].Position.X < MyGlobals.blockSize)
                    body[0].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize/2, body[0].Position.Y);
                if (body[1].Position.X < MyGlobals.blockSize)
                    body[1].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize/2, body[1].Position.Y);

                if (body[0].Position.Y < MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[1].Position.X, MyGlobals.blockSize + MyGlobals.blockSize/2);
                if (body[1].Position.Y < MyGlobals.blockSize)
                    body[1].Position = new Vector2(body[1].Position.X, MyGlobals.blockSize + MyGlobals.blockSize/2);
            }
            //Throws the body parts
            if (started && body[1].change && !menu1)
            {
                if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Down))
                {
                    if (body[1].kick(body[0], 'd'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y + MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'd'))
                        body[2].Position = new Vector2(body[2].Position.X, body[2].Position.Y + MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Up))
                {
                    if (body[1].kick(body[0], 'u'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y - MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'u'))
                        body[2].Position = new Vector2(body[2].Position.X, body[2].Position.Y - MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Left))
                {
                    if (body[1].kick(body[0], 'l'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X - MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'l'))
                        body[2].Position = new Vector2(body[2].Position.X - MyGlobals.blockSize * 2, body[2].Position.Y);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Right))
                {
                    if (body[1].kick(body[0], 'r'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X + MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'r'))
                        body[2].Position = new Vector2(body[2].Position.X + MyGlobals.blockSize * 2, body[2].Position.Y);
                }

                if (body[0].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[0].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize / 2, body[0].Position.Y);
                if (body[2].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[2].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize / 2, body[2].Position.Y);

                if (body[0].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[2].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize / 2);
                if (body[2].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[2].Position = new Vector2(body[2].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize / 2);

                if (body[0].Position.X < MyGlobals.blockSize)
                    body[0].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize / 2, body[0].Position.Y);
                if (body[2].Position.X < MyGlobals.blockSize)
                    body[2].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize / 2, body[2].Position.Y);

                if (body[0].Position.Y < MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[2].Position.X, MyGlobals.blockSize + MyGlobals.blockSize / 2);
                if (body[2].Position.Y < MyGlobals.blockSize)
                    body[2].Position = new Vector2(body[2].Position.X, MyGlobals.blockSize + MyGlobals.blockSize / 2);
            }

            if (started && !menu1)
            {
                int count;
                if (!keyboard.IsKeyDown(Keys.Up) && !keyboard.IsKeyDown(Keys.Down) && !keyboard.IsKeyDown(Keys.Right) && !keyboard.IsKeyDown(Keys.Left))
                {
                    indexesBellow = body[0].hidden(blockSight, 'd');
                    indexesRight = body[0].hidden(blockSight, 'r');
                    indexesLeft = body[0].hidden(blockSight, 'l');
                    indexesAbove = body[0].hidden(blockSight, 'u');
                }

                //Bellow
                if (indexesBellow.Count > 0)
                {
                    for (int i = 0; i < indexesBellow.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesBellow[i];
                        count = (int)((MyGlobals.realHeigh - initialPosition.Y) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.Y += MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenBellow.Count; k++)
                            {
                                if (hiddenBellow[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenBellow.Add(aux);
                                Components.Add(aux);
                                countBellow = indexesBellow.Count;
                            }

                        }
                    }
                }
                if (indexesBellow.Count < countBellow && hiddenBellow.Count > 0 && !keyboard.IsKeyDown(Keys.Up) && !keyboard.IsKeyDown(Keys.Down) && !keyboard.IsKeyDown(Keys.Right) && !keyboard.IsKeyDown(Keys.Left))
                {
                    count = hiddenBellow.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenBellow[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenBellow.RemoveAt(i);
                    }
                }

                //Above
                if (indexesAbove.Count > 0)
                {
                    for (int i = 0; i < indexesAbove.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesAbove[i];
                        count = (int)((initialPosition.Y - MyGlobals.zeroY) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.Y -= MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenAbove.Count; k++)
                            {
                                if (hiddenAbove[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenAbove.Add(aux);
                                Components.Add(aux);
                                countAbove = indexesAbove.Count;
                            }

                        }
                    }
                }
                if (indexesAbove.Count < countAbove && hiddenAbove.Count > 0 && !keyboard.IsKeyDown(Keys.Up) && !keyboard.IsKeyDown(Keys.Down) && !keyboard.IsKeyDown(Keys.Right) && !keyboard.IsKeyDown(Keys.Left))
                {
                    count = hiddenAbove.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenAbove[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenAbove.RemoveAt(i);
                    }
                }

                //Left
                if (indexesLeft.Count > 0)
                {
                    for (int i = 0; i < indexesLeft.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesLeft[i];
                        count = (int)((initialPosition.X - MyGlobals.zeroX) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.X -= MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenLeft.Count; k++)
                            {
                                if (hiddenLeft[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenLeft.Add(aux);
                                Components.Add(aux);
                                countLeft = indexesLeft.Count;
                            }
                        }
                    }
                }
                if (indexesLeft.Count < countLeft && hiddenLeft.Count > 0 && !keyboard.IsKeyDown(Keys.Up) && !keyboard.IsKeyDown(Keys.Down) && !keyboard.IsKeyDown(Keys.Right) && !keyboard.IsKeyDown(Keys.Left))
                {
                    count = hiddenLeft.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenLeft[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenLeft.RemoveAt(i);
                    }
                }

                //RIGHT
                if (indexesRight.Count > 0)
                {
                    for (int i = 0; i < indexesRight.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesRight[i];
                        count = (int)((MyGlobals.realWidth - initialPosition.X) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.X += MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenRight.Count; k++)
                            {
                                if (hiddenRight[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenRight.Add(aux);
                                Components.Add(aux);
                                countRight = indexesRight.Count;
                            }
                        }
                    }
                }
                if (indexesRight.Count < countRight && hiddenRight.Count > 0 && !keyboard.IsKeyDown(Keys.Up) && !keyboard.IsKeyDown(Keys.Down) && !keyboard.IsKeyDown(Keys.Right) && !keyboard.IsKeyDown(Keys.Left))
                {
                    count = hiddenRight.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenRight[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenRight.RemoveAt(i);
                    }
                }
            }

            if (started && !menu1)
            {
                //Give to blindness sprite, the same position of the head
                blindness.Position = body[0].Position;

                if(finalized(body[0].Position))
                {
                    Components.Remove(body[0]);
                    body[1].Walls.Remove(body[0]);
                    body[2].Walls.Remove(body[0]);
                    finished[0] = 1;
                    if (!Components.Contains(finalHead))
                        Components.Add(finalHead);
                }
                if (finalized(body[1].Position))
                {
                    Components.Remove(body[1]);
                    body[0].Walls.Remove(body[1]);
                    body[2].Walls.Remove(body[1]);
                    finished[1] = 1;
                    if (!Components.Contains(finalArms))        
                        Components.Add(finalArms);
                }
                if (finalized(body[2].Position))
                {
                    Components.Remove(body[2]);
                    body[0].Walls.Remove(body[2]);
                    body[1].Walls.Remove(body[2]);
                    finished[2] = 1;
                    if(!Components.Contains(finalLegs))
                        Components.Add(finalLegs);
                }

                if(levelEndAnimation)
                {
                    
                    for (int i = 0; i < blockers.Count; i++)
                    {
                        Components.Remove(blockers[i]);
                    }
                    blockers = new List<Sprite>();
                    if (!Components.Contains(levelComplete))
                    {
                        Components.Add(levelComplete);
                        didIt.Play();
                    }
                    if(levelNumber == 4)
                    {
                        if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter))
                        {
                            Components.Remove(levelComplete);
                            levelEndAnimation = false;
                            blockers = new List<Sprite>();
                            body[0].Obstacles = new List<Sprite>();
                            body[0].Trench = new List<Sprite>();
                            body[0].Walls = new List<Sprite>();
                            body[0].Bridge = new List<Sprite>();
                            body[0].Indexes = new List<Vector2>();
                            levelNumber++;
                            if (levelNumber > 5)
                            {
                                this.Exit();
                            }


                            LoadContent();
                        }
                        
                    }
                    if(!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && levelComplete.Rectangle.X < 1440 && levelNumber < 4)
                    {
                        if (levelComplete.Rectangle.X == 0)
                            splitting.Play();
                       
                        levelComplete.Rectangle = new Rectangle(levelComplete.Rectangle.X + 480, 0, 480, 420);
                    }
                    else if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && levelComplete.Rectangle.X >= 1440)
                    {
                        Components.Remove(levelComplete);
                        levelEndAnimation = false;
                        blockers = new List<Sprite>();
                        body[0].Obstacles = new List<Sprite>();
                        body[0].Trench = new List<Sprite>();
                        body[0].Walls = new List<Sprite>();
                        body[0].Bridge = new List<Sprite>();
                        body[0].Indexes = new List<Vector2>();
                        levelNumber++;
                        if (levelNumber > 5)
                        {
                            this.Exit();
                        }
                        

                        LoadContent();
                    }

                    old = keyboard;
                }

                if(levelFinished())
                {
                    levelEndAnimation = true;
                }

                List<Vector2> auxList = platePressed();

                if (auxList.Count >= bridgesNumber)
                {
                    for (int i = 0; i < auxList.Count; i++)
                    {
                        for (int j = 0; j < bridges.Count; j++)
                        {
                            if(auxList[i] == bridges[j].Position)
                            {
                                bridges[j].Rectangle = new Rectangle((int)MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                            }
                        }
                    }

                    for (int i = 0; i < auxList.Count; i++)
                    {
                        for (int j = 0; j < walls.Count; j++)
                        {
                            if (walls[j].Position == auxList[i])
                            {
                                if (walls[j] != body[0] && walls[j] != body[1] && walls[j] != body[2])
                                {
                                    bridgeOn.Add(walls[j]);
                                    walls.RemoveAt(j);
                                }
                            }
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < bridges.Count; i++)
                    {
                        bridges[i].Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                    }
                    for (int i = 0; i < bridgeOn.Count; i++)
                    {
                        walls.Add(bridgeOn[i]);
                    }
                    bridgeOn = new List<Sprite>();
                }

                bridgesNumber = auxList.Count;
                if (started)
                {
                    body[0].Walls = walls;
                    body[1].Walls = walls;
                    body[2].Walls = walls;

                    body[0].Bridge = bridges;
                    body[1].Bridge = bridges;
                    body[2].Bridge = bridges;
                }
            }

            
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            base.Draw(gameTime);
            
        }
        public bool isBodyMoving()
        {
            for (int i = 0; i < 3; i++)
            {
                if (body[i].Moving)
                    return true;
            }
            return false;
        }
        public bool finalized(Vector2 position)
        {
            if (position == finalPosition)
                return true;
            else
                return false;
        }
        public bool levelFinished()
        {
            if (finished[0] != 0 && finished[1] != 0 && finished[2] != 0)
            {
                return true;
            }
            
            return false;
        }
        public List<Vector2> platePressed()
        {
            List<Vector2> temp = new List<Vector2>();
            for (int i = 0; i < plates.Count; i++)
            {
                if(plates[i].Position == body[0].Position || plates[i].Position == body[1].Position || plates[i].Position == body[2].Position)
                {
                    temp.AddRange(plates[i].EquivBridges);
                }
            }

            return temp;
        }
        public bool die()
        {
            List<Vector2> temp = platePressed();

            for (int i = 0; i < plates.Count; i++)
            {
                for (int j = 0; j < plates[i].EquivBridges.Count; j++)
                {
                    if (body[0].Position == plates[i].EquivBridges[j] && !temp.Contains(plates[i].EquivBridges[j]))
                    {
                        return true;
                    }
                    if (body[1].Position == plates[i].EquivBridges[j] && !temp.Contains(plates[i].EquivBridges[j]))
                    {
                        return true;
                    }
                    if (body[2].Position == plates[i].EquivBridges[j] && !temp.Contains(plates[i].EquivBridges[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
