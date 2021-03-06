﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Torn
{
    //Creates a sprite controlled by the user using the keyboard or the mouse
    class UserControlledSprite : Sprite
    {
        KeyboardState keyboard;
        bool update, moving, isArms, isLegs, isVisible, pullingDown, pullingUp, pullingRight, pullingLeft, stoped, pushing;
        char direction;
        int steps;
        int obstacleIndex;
        int obstacleIndexLeft, obstacleIndexRight, obstacleIndexUp, obstacleIndexDown; 
        List<Sprite> obstacles;
        List<Sprite> trench;
        List<Sprite> walls;
        List<Sprite> bridges;
        List<Vector2> indexes;
        SoundEffect movingBox;
        SoundEffectInstance instance;
        UserControlledSprite body1, body2;
        Vector2 final;

        public Vector2 Final
        {
            get { return final; }
            set { final = value; }
        }
        public UserControlledSprite Body1
        {
            get { return body1; }
            set { body1 = value; }
        }
        public UserControlledSprite Body2
        {
            get { return body2; }
            set { body2 = value; }
        }
        public UserControlledSprite(Game game, String textureFile, Vector2 position, bool isArms, bool isLegs): base(game, textureFile, position)
        {
            //Initializes all the attributes with false in order to control the execution of the program
            update = false;
            moving = false;
            pullingDown = false;
            pullingLeft = false;
            pullingUp = false;
            pullingRight = false;
            stoped = true;
            //Attribute to control how many steps the body part will walk during each interaction
            steps = MyGlobals.steps;
            this.isArms = isArms;
            this.isLegs = isLegs;
            isVisible = true;
            pushing = false;


            movingBox = Game.Content.Load<SoundEffect>(@"Sounds\MovingBox");
            instance = movingBox.CreateInstance();

            if(isLegs)
            {
                rectangle = new Rectangle(0, 0, (int) MyGlobals.realBlockSize, (int) MyGlobals.realBlockSize);
            }

            if(isArms)
            {
                rectangle = new Rectangle(90, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
            }
        }
        public List<Sprite> Walls
        {
            get { return walls; }
            set { walls = value; }
        }
        public List<Sprite> Bridge
        {
            get { return bridges; }
            set { bridges = value; }
        }
        public List<Vector2> Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }

        }

        public bool Stoped
        {
            get { return stoped; }
            set { stoped = value; }
        }
        public List<Sprite> Obstacles
        {
            get { return obstacles; }
            set { obstacles = value; }
        }

        public List<Sprite> Trench
        {
            get { return trench; }
            set { trench = value; }
        }

        public override void Update(GameTime gameTime)
        {
            if (update)
            {
                keyboard = Keyboard.GetState();

                /*Makes the sprite walk accondingly with the keyboard
                 *checking if there are any other parts moving or if the this part will no collide with another one
                 */
                if (keyboard.IsKeyDown(Keys.Down) && !moving) 
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pullingDown = true;
                    direction = 'd';
                    moving = true;
                    stoped = true;
                }
                else if (keyboard.IsKeyDown(Keys.Up) && !moving)
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pullingUp = true;
                    direction = 'u';
                    moving = true;
                    stoped = true;
                }
                else if (keyboard.IsKeyDown(Keys.Left) && !moving)
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pullingLeft = true;
                    direction = 'l';
                    moving = true;
                    stoped = true;
                }
                else if (keyboard.IsKeyDown(Keys.Right) && !moving) 
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pullingRight = true;
                    direction = 'r';
                    moving = true;
                    stoped = true;
                }

                //makes the body part 'walk' to the destination with the grid effect
                if (moving && steps > 0)
                {
                    //moves 3 pixels according with the last key pressed
                    switch (direction)
                    {
                            
                        case 'd':
                            if (hastrench(trench, 'd') || haswalls(walls, 'd') || hasBody('d'))
                                steps = 0;
                            else if (this.position.Y >= MyGlobals.heigh - MyGlobals.blockSize / 2)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'd') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'd') && isArms && obstacleIndexDown == -1)
                            {
                                steps = 0;
                                obstacleIndexDown = 0;
                            }
                            else if (pull(obstacles, 'd') && pullingDown)
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].Y += MyGlobals.blockSize / 10;
                                this.position.Y += MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle(((steps % 2) + 1) * (int)MyGlobals.realBlockSize, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle((steps % 2) * (int)MyGlobals.realBlockSize + 120, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'd') && isArms && obstacleIndexDown != -1 && !haswalls(walls, 'd'))
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].Y += MyGlobals.blockSize / 10;
                                this.position.Y += MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle(((steps % 2) + 1) * (int)MyGlobals.realBlockSize, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = true;
                                    base.Rectangle = new Rectangle((steps % 2) * (int)MyGlobals.realBlockSize + 120, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'd'))
                            {
                                this.position.Y += MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle(((steps % 2) + 1) * (int)MyGlobals.realBlockSize, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle((steps % 2) * (int)MyGlobals.realBlockSize + 120, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            break;
                        case 'u':
                            if (hastrench(trench, 'u') || haswalls(walls, 'u') || hasBody('u'))
                                steps = 0;
                            else if (this.position.Y <= MyGlobals.blockSize / 2)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'u') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'u') && isArms && obstacleIndexUp == -1)
                            {
                                obstacleIndexUp = 1;
                                steps = 0;
                            }
                            else if (pull(obstacles, 'u') && pullingUp)
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].Y -= MyGlobals.blockSize / 10;
                                this.position.Y -= MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 120, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle(((steps % 2) + 1) * (int)MyGlobals.realBlockSize, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'u') && isArms && obstacleIndexUp != -1 && !haswalls(walls, 'u'))
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].Y -= MyGlobals.blockSize / 10;
                                this.position.Y -= MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 120, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = true;
                                    base.Rectangle = new Rectangle(((steps % 2) + 1) * (int)MyGlobals.realBlockSize, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'u'))
                            {
                                this.position.Y -= MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 120, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle(((steps % 2) + 1) * (int)MyGlobals.realBlockSize, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            break;
                        case 'r':
                            if (hastrench(trench, 'r') || haswalls(walls, 'r') || hasBody('r'))
                                steps = 0;
                            else if (this.position.X >= MyGlobals.width - MyGlobals.blockSize / 2)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'r') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'r') && isArms && obstacleIndexRight == -1)
                            {
                                obstacleIndexRight = 1;
                                steps = 0;
                            }
                            else if (pull(obstacles, 'r') && pullingRight)
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].X += MyGlobals.blockSize / 10;
                                this.position.X += MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * (int)MyGlobals.realBlockSize + 300, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle(360, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'r') && isArms && obstacleIndexRight != -1 && !haswalls(walls, 'r'))
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].X += MyGlobals.blockSize / 10;
                                this.position.X += MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 300, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = true;
                                    base.Rectangle = new Rectangle(450, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'r'))
                            {
                                this.position.X += MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 300, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle((steps % 2) * (int)MyGlobals.realBlockSize + 300, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            break;
                        case 'l':
                            if (hastrench(trench, 'l') || haswalls(walls, 'l') || hasBody('l'))
                                steps = 0;
                            else if (this.position.X <= MyGlobals.blockSize / 2)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'l') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'l') && isArms && obstacleIndexLeft == -1)
                            {
                                obstacleIndexLeft = 1;
                                steps = 0;
                            }
                            else if (pull(obstacles, 'l') && pullingLeft)
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].X -= MyGlobals.blockSize / 10;
                                this.position.X -= MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 210, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle(420, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'l') && isArms && obstacleIndexLeft != -1 && !haswalls(walls, 'l'))
                            {
                                if (instance.State == SoundState.Stopped)
                                    instance.Play();
                                obstacles[obstacleIndex].X -= MyGlobals.blockSize / 10;
                                this.position.X -= MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 210, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = true;
                                    base.Rectangle = new Rectangle(390, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'l'))
                            {

                                this.position.X -= MyGlobals.blockSize / 10;
                                if (isLegs && steps % 5 == 0)
                                {
                                    base.Rectangle = new Rectangle((steps % 2) * ((int)MyGlobals.realBlockSize) + 210, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                if (isArms && steps % 5 == 0)
                                {
                                    pushing = false;
                                    base.Rectangle = new Rectangle((steps % 2) * (int)MyGlobals.realBlockSize + 210, 0, ((int)MyGlobals.realBlockSize), ((int)MyGlobals.realBlockSize));
                                }
                                steps--;
                            }
                            break;
                    }
                }

                /*Stop after 10 interactions of 3 pixels
                 * set the direction with a meaningless caracter and return the other attributes to their default values*/
                if (steps == 0)
                {
                    pullingDown = false;
                    pullingLeft = false;
                    pullingRight = false;
                    pullingUp = false;
                    moving = false;
                    steps = MyGlobals.steps;
                    if (isLegs)
                    {
                        switch (direction)
                        {
                            case 'd':
                                rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                break;
                            case 'u':
                                rectangle = new Rectangle(90, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                break;
                            case 'l':
                                rectangle = new Rectangle(180, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                break;
                            case 'r':
                                rectangle = new Rectangle(270, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                break;
                        }
                    }

                    if (isArms && !pushing)
                    {
                        switch (direction)
                        {
                            case 'd':
                                rectangle = new Rectangle(90, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                pushing = false;
                                break;
                            case 'u':
                                rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                pushing = false;
                                break;
                            case 'l':
                                rectangle = new Rectangle(180, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                pushing = false;
                                break;
                            case 'r':
                                rectangle = new Rectangle(270, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                pushing = false;
                                break;
                        }
                    }
                    direction = '-';
                }

                if (keyboard.IsKeyDown(Keys.Down) && keyboard.IsKeyDown(Keys.LeftShift) )
                    jump('d');
                else if (keyboard.IsKeyDown(Keys.Up) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('u');
                else if (keyboard.IsKeyDown(Keys.Right) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('r');
                else if (keyboard.IsKeyDown(Keys.Left) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('l');
            }
            base.Update(gameTime);
        }

        public bool change
        {
            get { return update; }
            set { update = value; }
        }

        public bool Moving
        {
            get { return moving; }
        }

        public bool hastrench(List<Sprite> trench, char direction)
        {
            switch (direction)
            {
                case 'd':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.Y + MyGlobals.blockSize == trench[i].Position.Y && this.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
                case 'u':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.Y - MyGlobals.blockSize == trench[i].Position.Y && this.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
                case 'r':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.X + MyGlobals.blockSize == trench[i].Position.X && this.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
                case 'l':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.X - MyGlobals.blockSize == trench[i].Position.X && this.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
            }

            return false;
        }

        public bool haswalls(List<Sprite> walls, char direction)
        {

            switch (direction)
            {
                case 'd':
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (this.Position.Y + MyGlobals.blockSize == walls[i].Position.Y && this.Position.X == walls[i].Position.X || hasObstacle(obstacles, 'd') && this.Position.Y + 2 * MyGlobals.blockSize == walls[i].Position.Y && this.Position.X == walls[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (walls[i].Position == obstacles[j].Position)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    break;
                case 'u':
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (this.Position.Y - MyGlobals.blockSize == walls[i].Position.Y && this.Position.X == walls[i].Position.X || this.Position.Y - 2 * MyGlobals.blockSize == walls[i].Position.Y && this.Position.X == walls[i].Position.X && hasObstacle(obstacles, 'u'))
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (walls[i].Position == obstacles[j].Position)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    break;
                case 'r':
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (this.Position.X + MyGlobals.blockSize == walls[i].Position.X && this.Position.Y == walls[i].Position.Y || this.Position.X + 2 * MyGlobals.blockSize == walls[i].Position.X && this.Position.Y == walls[i].Position.Y && hasObstacle(obstacles, 'r'))
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (walls[i].Position == obstacles[j].Position)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    break;
                case 'l':
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (this.Position.X - MyGlobals.blockSize == walls[i].Position.X && this.Position.Y == walls[i].Position.Y || this.Position.X - 2 * MyGlobals.blockSize == walls[i].Position.X && this.Position.Y == walls[i].Position.Y && hasObstacle(obstacles, 'l'))
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (walls[i].Position == obstacles[j].Position)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public bool hasbridges(List<Sprite> bridges, char direction)
        {
            switch (direction)
            {
                case 'd':
                    for (int i = 0; i < bridges.Count; i++)
                    {
                        if (this.Position.Y + MyGlobals.blockSize == bridges[i].Position.Y && this.Position.X == bridges[i].Position.X || hasObstacle(obstacles, 'd') && this.Position.Y + 2 * MyGlobals.blockSize == bridges[i].Position.Y && this.Position.X == bridges[i].Position.X)
                        {
                            return true;
                        }
                    }
                    break;
                case 'u':
                    for (int i = 0; i < bridges.Count; i++)
                    {
                        if (this.Position.Y - MyGlobals.blockSize == bridges[i].Position.Y && this.Position.X == bridges[i].Position.X || this.Position.Y - 2 * MyGlobals.blockSize == bridges[i].Position.Y && this.Position.X == bridges[i].Position.X && hasObstacle(obstacles, 'u'))
                        {
                            return true;
                        }
                    }
                    break;
                case 'r':
                    for (int i = 0; i < bridges.Count; i++)
                    {
                        if (this.Position.X + MyGlobals.blockSize == bridges[i].Position.X && this.Position.Y == bridges[i].Position.Y || this.Position.X + 2 * MyGlobals.blockSize == bridges[i].Position.X && this.Position.Y == bridges[i].Position.Y && hasObstacle(obstacles, 'r'))
                        {
                            return true;
                        }
                    }
                    break;
                case 'l':
                    for (int i = 0; i < bridges.Count; i++)
                    {
                        if (this.Position.X - MyGlobals.blockSize == bridges[i].Position.X && this.Position.Y == bridges[i].Position.Y || this.Position.X - 2 * MyGlobals.blockSize == bridges[i].Position.X && this.Position.Y == bridges[i].Position.Y && hasObstacle(obstacles, 'l'))
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public bool hasObstacle(List<Sprite> obstacles, char direction)
        {
            int i, j;

            switch (direction)
            {
                case 'd':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.Y + MyGlobals.blockSize == obstacles[i].Position.Y && this.Position.X == obstacles[i].Position.X)
                        {
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (obstacles[i].Position == obstacles[j].Position && i != j)
                                    return true;
                            }
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                {
                                    obstacles[i].Rectangle = new Rectangle((int)MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                    return false;
                                }
                            }

                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pullingUp && (obstacles[i].Position.Y + MyGlobals.blockSize == obstacles[j].Position.Y && obstacles[i].Position.X == obstacles[j].Position.X || obstacles[i].Position.Y>= MyGlobals.heigh - MyGlobals.blockSize/2))
                                {
                                    for (int k = 0; k < trench.Count; k++)
                                    {
                                        if (obstacles[j].Position == trench[k].Position)
                                            return true;
                                    }
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndexDown = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
                case 'u':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.Y - MyGlobals.blockSize == obstacles[i].Position.Y && this.Position.X == obstacles[i].Position.X)
                        {
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (obstacles[i].Position == obstacles[j].Position && i != j)
                                    return true;
                            }
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                {
                                    obstacles[i].Rectangle = new Rectangle((int)MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                    return false;
                                }
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pullingDown && (obstacles[i].Position.Y - MyGlobals.blockSize == obstacles[j].Position.Y && obstacles[i].Position.X == obstacles[j].Position.X || obstacles[i].Position.Y <= MyGlobals.blockSize/2))
                                {
                                    for (int k = 0; k < trench.Count; k++)
                                    {
                                        if (obstacles[j].Position == trench[k].Position)
                                            return true;
                                    }
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndexUp = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
                case 'r':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.X + MyGlobals.blockSize == obstacles[i].Position.X && this.Position.Y == obstacles[i].Position.Y)
                        {
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (obstacles[i].Position == obstacles[j].Position && i != j)
                                    return true;
                            }
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                {
                                    obstacles[i].Rectangle = new Rectangle((int)MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                    return false;
                                }
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pullingLeft && (obstacles[i].Position.X + MyGlobals.blockSize == obstacles[j].Position.X && obstacles[i].Position.Y == obstacles[j].Position.Y || obstacles[i].Position.X >= MyGlobals.width -MyGlobals.blockSize / 2))
                                {
                                    for (int k = 0; k < trench.Count; k++)
                                    {
                                        if (obstacles[j].Position == trench[k].Position)
                                            return true;
                                    }
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndexRight = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
                case 'l':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.X - MyGlobals.blockSize == obstacles[i].Position.X && this.Position.Y == obstacles[i].Position.Y)
                        {
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (obstacles[i].Position == obstacles[j].Position && i != j)
                                    return true;
                            }
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                {
                                    obstacles[i].Rectangle = new Rectangle((int)MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                                    return false;
                                }
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pullingRight && (obstacles[i].Position.X - MyGlobals.blockSize == obstacles[j].Position.X && obstacles[i].Position.Y == obstacles[j].Position.Y || obstacles[i].Position.X <= MyGlobals.blockSize / 2))
                                {
                                    for (int k = 0; k < trench.Count; k++)
                                    {
                                        if (obstacles[j].Position == trench[k].Position)
                                            return true;
                                    }
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndexLeft = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public bool hasBody(char direction)
        {
            switch (direction)
            {
                case 'd':
                    if ((this.position.Y + MyGlobals.blockSize == body1.Position.Y && this.position.X == body1.Position.X || this.position.Y + MyGlobals.blockSize == body2.Position.Y && this.position.X == body2.Position.X) && body1.Position != final && body2.Position != final)
                        return true;
                    break;
                case 'u':
                    if ((this.position.Y - MyGlobals.blockSize == body1.Position.Y && this.position.X == body1.Position.X || this.position.Y - MyGlobals.blockSize == body2.Position.Y && this.position.X == body2.Position.X) && body1.Position != final && body2.Position != final)
                        return true;
                    break;
                case 'r':
                    if ((this.position.X + MyGlobals.blockSize == body1.Position.X && this.position.Y == body1.Position.Y || this.position.X + MyGlobals.blockSize == body2.Position.X && this.position.Y == body2.Position.Y) && body1.Position != final && body2.Position != final)
                        return true;
                    break;
                case 'l':
                    if ((this.position.X - MyGlobals.blockSize == body1.Position.X && this.position.Y == body1.Position.Y || this.position.X - MyGlobals.blockSize == body2.Position.X && this.position.Y == body2.Position.Y) && body1.Position != final && body2.Position != final)
                        return true;
                    break;
            }
            return false;
        }
              
        public bool kick(Sprite body, char direction)
        {
            keyboard = Keyboard.GetState();
            if (this.isArms || this.isLegs)
            {
                for (int i = 0; i < walls.Count; i++)
                {
                    if (body.Position.Y + MyGlobals.blockSize * 2 == walls[i].Position.Y && body.Position.X == walls[i].Position.X && direction == 'd')
                        return false;
                    if (body.Position.Y - MyGlobals.blockSize * 2 == walls[i].Position.Y && body.Position.X == walls[i].Position.X && direction == 'u')
                        return false;
                    if (body.Position.X - MyGlobals.blockSize * 2 == walls[i].Position.X && body.Position.Y == walls[i].Position.Y && direction == 'l')
                        return false;
                    if (body.Position.X + MyGlobals.blockSize * 2 == walls[i].Position.X && body.Position.Y == walls[i].Position.Y && direction == 'r')
                        return false;
                }

                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (body.Position.Y + MyGlobals.blockSize * 2 == obstacles[i].Position.Y && body.Position.X == obstacles[i].Position.X && direction == 'd')
                        return false;
                    if (body.Position.Y - MyGlobals.blockSize * 2 == obstacles[i].Position.Y && body.Position.X == obstacles[i].Position.X && direction == 'u')
                        return false;
                    if (body.Position.X - MyGlobals.blockSize * 2 == obstacles[i].Position.X && body.Position.Y == obstacles[i].Position.Y && direction == 'l')
                        return false;
                    if (body.Position.X + MyGlobals.blockSize * 2 == obstacles[i].Position.X && body.Position.Y == obstacles[i].Position.Y && direction == 'r')
                        return false;
                }

                if (this.position.Y + MyGlobals.blockSize == body.Position.Y && this.position.X == body.Position.X && direction == 'd')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.Y + MyGlobals.blockSize * 2 == trench[i].Position.Y && body.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else if (this.position.Y - MyGlobals.blockSize == body.Position.Y && this.position.X == body.Position.X && direction == 'u')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.Y - MyGlobals.blockSize * 2 == trench[i].Position.Y && body.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else if (this.position.X - MyGlobals.blockSize == body.Position.X && this.position.Y == body.Position.Y && direction == 'l')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.X - MyGlobals.blockSize * 2 == trench[i].Position.X && body.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else if (this.position.X + MyGlobals.blockSize == body.Position.X && this.position.Y == body.Position.Y && direction == 'r')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.X + MyGlobals.blockSize * 2 == trench[i].Position.X && body.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public void jump(char direction)
        {
            int i, j;

            if (isLegs)
            {
                switch (direction)
                {
                    case 'd':
                        if ((hastrench(this.trench, 'd') || hasbridges(this.bridges, 'd')) && this.position.Y + MyGlobals.blockSize * 2 < MyGlobals.heigh - MyGlobals.blockSize/2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.Y + MyGlobals.blockSize * 2 == trench[i].Position.Y && this.position.X == trench[i].Position.X)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.Y += MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            for (i = 0; i < walls.Count; i++)
                            {
                                if (this.position.Y + MyGlobals.blockSize * 2 == walls[i].Position.Y && this.position.X == walls[i].Position.X)
                                    return;
                            }
                            this.position.Y += MyGlobals.blockSize;
                        }
                        break;
                    case 'u':
                        if ((hastrench(this.trench, 'u') || hasbridges(this.bridges, 'u')) && this.position.Y - MyGlobals.blockSize * 2 > MyGlobals.blockSize / 2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.Y - MyGlobals.blockSize * 2 == trench[i].Position.Y && this.position.X == trench[i].Position.X)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.Y -= MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            for (i = 0; i < walls.Count; i++)
                            {
                                if (this.position.Y - MyGlobals.blockSize * 2 == walls[i].Position.Y && this.position.X == walls[i].Position.X)
                                    return;
                            }
                            this.position.Y -= MyGlobals.blockSize;
                        }
                        break;
                    case 'l':
                        for (i = 0; i < walls.Count; i++)
                        {
                            if (this.position.X - MyGlobals.blockSize * 2 == walls[i].Position.X && this.position.Y == walls[i].Position.Y)
                                return;
                        }
                        if ((hastrench(this.trench, 'l') || hasbridges(this.bridges, 'l')) && this.position.X - MyGlobals.blockSize * 2 > MyGlobals.blockSize / 2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.X - MyGlobals.blockSize * 2 == trench[i].Position.X && this.position.Y == trench[i].Position.Y)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.X -= MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            
                            this.position.X -= MyGlobals.blockSize;
                        }

                        break;
                    case 'r':
                        for (i = 0; i < walls.Count; i++)
                        {
                            if (this.position.X + MyGlobals.blockSize * 2 == walls[i].Position.X && this.position.Y == walls[i].Position.Y)
                                return;
                        }
                        if ((hastrench(this.trench, 'r') || hasbridges(this.bridges, 'r'))  && this.position.X + MyGlobals.blockSize * 2 < MyGlobals.width - MyGlobals.blockSize / 2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.X + MyGlobals.blockSize * 2 == trench[i].Position.X && this.position.Y == trench[i].Position.Y)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.X += MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            
                            this.position.X += MyGlobals.blockSize;
                        }
                        break;
                }
            }
        }

        public bool pull(List<Sprite> obstacles, char direction)
        {
            if (isArms)
            {
                switch (direction)
                {
                    case 'd':
                        if (!haswalls(walls, 'd') && !hastrench(trench, 'd') && !hasObstacle(obstacles, 'd') && hasObstacle(obstacles, 'u') && this.position.Y < MyGlobals.heigh - MyGlobals.blockSize / 2)
                            return true;
                        break;
                    case 'u':
                        if (!haswalls(walls, 'u') && !hastrench(trench, 'u') && !hasObstacle(obstacles, 'u') && hasObstacle(obstacles, 'd') && this.position.Y > MyGlobals.blockSize / 2)
                            return true;
                        break;
                    case 'l':
                        if (!haswalls(walls, 'l') && !hastrench(trench, 'l') && !hasObstacle(obstacles, 'l') && hasObstacle(obstacles, 'r') && this.position.X >  MyGlobals.blockSize / 2)
                            return true;
                        break;
                    case 'r':
                        if (!haswalls(walls, 'r') && !hastrench(trench, 'r') && !hasObstacle(obstacles, 'r') && hasObstacle(obstacles, 'l') && this.position.Y < MyGlobals.width - MyGlobals.blockSize / 2)
                            return true;
                        break;
                }
            }
            return false;
        }

        public List<Vector2> hidden(List<Vector2> positions, char direction)
        {

            indexes = new List<Vector2>();
            switch (direction)
            {
                case 'd':
                    for (int i = 0; i < positions.Count; i++)
                    {
                        if (this.position.Y < positions[i].Y)
                            indexes.Add(positions[i]);
                    }
                    break;
                case 'u':
                    for (int i = 0; i < positions.Count; i++)
                    {
                        if (this.position.Y > positions[i].Y)
                            indexes.Add(positions[i]);
                    }
                    break;
                case 'l':
                    for (int i = 0; i < positions.Count; i++)
                    {
                        if (this.position.X > positions[i].X)
                            indexes.Add(positions[i]);
                    }
                    break;
                case 'r':
                    for (int i = 0; i < positions.Count; i++)
                    {
                        if (this.position.X < positions[i].X)
                            indexes.Add(positions[i]);
                    }
                    break;
            }
            return indexes;
        }
    }
}
