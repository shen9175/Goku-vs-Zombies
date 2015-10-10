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

namespace XNAtest
{

    struct maptype
    {
        public Rectangle rect;
        public bool ocupied;
    };

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerpic,monsterpic,treepic,skullpic,healthpic;
        Vector2 position,player_spawn;
        Rectangle standing, running, attacking, beaten, deading, Kamehameha,zombie_standing,zombie_attacking,zombie_beaten,zombie_deading,zombie_walking;
        SpriteFont font;
        string buffer;
        int timeSinceLastFrame, timeSinceLastFrame_deading;
        int millisecondsPerFrame, millisecondsPerFrame_attacking, millisecondsPerFrame_deading;
        int [] frameSize;
        int[] attacking_frameSize, beaten_frameSize, deading_frameSize, Kamehameha_frameSize,zombie_attacking_frameSize,zombie_beaten_frameSize,zombie_deading_frameSize,zombie_walking_frameSize;
        int frameheight;
        int attack_frameheight, beaten_frameheight, deading_frameheight,Kamehameha_frameheight,zombie_attack_frameheight,zombie_beaten_frameheight,zombie_deading_frameheight,zombie_walking_frameheight;
        Point starting_running_frame;
        Point starting_attacking_frame, starting_beaten_frame, starting_deading_frame, starting_Kamehameha_frame,starting_zombie_attack,starting_zombie_beaten,starting_zombie_deading,start_zombie_walking;
        int attacking_frame_counter, beaten_frame_counter, deading_frame_counter, Kamehameha_frame_counter,damage_counter;
        int running_frame_counter;
        hero player;
        LinkedList<wave> missle;
        LinkedList<enemy> monster;
        LinkedList<Rectangle> tree;
        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;
        maptype[,] map;
        bool playerisdead;
        int col, row;
        Random rand;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth=1024;
            graphics.PreferredBackBufferHeight=768;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            rand=new Random();
            missle = new LinkedList<wave>();
            missle.Clear();
            monster = new LinkedList<enemy>();
            monster.Clear();
            tree = new LinkedList<Rectangle>();
            tree.Clear();
            timeSinceLastFrame = 0;
            timeSinceLastFrame_deading=0;
            millisecondsPerFrame = 75;
            millisecondsPerFrame_attacking = 50;
            millisecondsPerFrame_deading = 150;
            col=Window.ClientBounds.Width/30;
            row=Window.ClientBounds.Height/4*3/40;
            map = new maptype[col, row];


            Rectangle temp;
            //spawning trees
            for (int i = 0; i < col; i ++)
            {
                for (int j = 0; j < row; j++)
                {
                    temp = new Rectangle(i * 30, j * 40, 30, 40);
                    map[i,j].rect =temp;
                    if(rand.Next(1,100)<5)
                    {
                        map[i,j].ocupied=true;     //probably there 489 cell in the whole screen, we let 5% of them are trees randomly
                        tree.AddLast(map[i,j].rect);
                    }
                    else
                    {

                    }

                }
            }

            // spawning monsters which are not in the trees
             for (int k = 0; k < 5; k++)
             {
                 int tempX=rand.Next(0,col);int tempY=rand.Next(0,row);
                 if (!tree.Contains(map[tempX,tempY].rect))
                 {
                     map[tempX,tempY].ocupied=true;
                     monster.AddLast(new enemy(map[tempX,tempY].rect,tree,rand ));
                 }
             }
            
            //spawning hero which is either not in the trees places nor monsters places
            do
            {
                temp = new Rectangle(rand.Next(0, col), rand.Next(0, row), 30, 40);
            }
             while(map[temp.X,temp.Y].ocupied);
            player=new hero(map[temp.X,temp.Y].rect,tree,rand);
            player_spawn=new Vector2(player.cell.X,player.cell.Y);



            standing = new Rectangle(10, 150, 30, 40);
            zombie_standing = new Rectangle(0, 0, 50, 80);
            frameSize = new int[9]{8,42,75,110,142,173,205,240,275};
            attacking_frameSize = new int[7]{ 410,440,480,520,570,630,700};
            beaten_frameSize = new int[4] { 195, 222,155, 189  };
            deading_frameSize = new int[10] { 195, 222, 155, 189, 375, 406, 335, 370, 455, 495 };
            Kamehameha_frameSize = new int[12] { 3, 32, 37, 64, 69, 95, 101, 127, 134, 167, 172, 205 };
            zombie_attacking_frameSize = new int[12] {0,50,100,150,200,250,300,350,390,440,480,530 };
            zombie_beaten_frameSize = new int[12] {0,50,90,140,190,240,281,332,371,436,480,530 };
            zombie_deading_frameSize = new int[12] {672,716,767,820,870,920,967,1013,1064,1110,1160,1210 };
            zombie_walking_frameSize = new int[20] {10,60,105,155,208,258,309,359,408,458,509,559,601,654,701,751,804,858,1010,1060};
            //zombie_walking_frameSize = new int[12] {10,54,105,155,208,252,309,348,408,457,509,552};
            zombie_attack_frameheight=261-184;
            zombie_beaten_frameheight=347-277;
            zombie_deading_frameheight=347-277;
            zombie_walking_frameheight=165-90;
            deading_frameheight = 373 - 337;
            beaten_frameheight = 373 - 337;
            Kamehameha_frameheight = 598 - 561;
            frameheight = 40;
            attack_frameheight = 418 - 375;
            starting_attacking_frame = new Point(410, 375);
            starting_running_frame = new Point(8, 195);
            starting_beaten_frame = new Point(195,337);
            starting_deading_frame = new Point(195, 337);
            starting_Kamehameha_frame = new Point(3, 561);
            start_zombie_walking = new Point(10, 90);
            starting_zombie_attack = new Point(0, 184);
            starting_zombie_beaten = new Point(0, 277);
            starting_zombie_deading = new Point(672, 277);
            running_frame_counter = 0;
            attacking_frame_counter = 0;
            beaten_frame_counter = 0;
            deading_frame_counter = 0;
            Kamehameha_frame_counter = 0;
            damage_counter = 0;
            playerisdead = false;
            currentKeyboard = new KeyboardState();
            oldKeyboard = new KeyboardState();



            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            playerpic = Content.Load<Texture2D>("goku") as Texture2D;
            monsterpic=Content.Load<Texture2D>("zombie1") as Texture2D;
            treepic = Content.Load<Texture2D>("Tree1") as Texture2D;
            //grasspic = Content.Load<Texture2D>("Grass") as Texture2D;
            skullpic = Content.Load<Texture2D>("skull") as Texture2D;
            healthpic=Content.Load<Texture2D>("HealthBar") as Texture2D;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("SpriteFont1");
            IsMouseVisible = true;

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            oldKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState(); 

            // TODO: Add your update logic here
            if (player.isAlive)
            {
                if (JustPressed(Keys.D1))
                {
                    if (!player.isChangingWeapon)
                    {
                        player.changeweapon(1);
                        player.isChangingWeapon = true;
                        player.status = 7;
                    }
                    else
                    {
                        player.status = 7;
                    }
                }
                if (JustPressed(Keys.D2))
                {
                    if (!player.isChangingWeapon)
                    {
                        player.changeweapon(0);
                        player.isChangingWeapon = true;
                        player.status = 7;
                    }
                    else
                    {
                        player.status = 7;
                    }
                }
                if (JustPressed(Keys.D3))
                {
                    player.armor = 2;
                }
                if (JustPressed(Keys.D4))
                {
                    player.armor = 3;
                }
                if (JustPressed(Keys.D5))
                {
                    player.armor = 4;
                }
                if (JustPressed(Keys.D6))
                {
                    player.armor = 5;
                }
                if (JustPressed(Keys.D7))
                {
                    player.armor = 6;
                }
                if (JustPressed(Keys.D8))
                {
                    player.armor = 7;
                }
                if (JustPressed(Keys.D9))
                {
                    player.armor = 8;
                }
                if (JustPressed(Keys.D0))
                {
                    player.armor = 9;
                }

                /*
                            if (JustPressed(Keys.S))
                            {
                                player.status = 3;
                            }
                            if (IsHeld(Keys.S))
                            {
                                player.status = 3;
                            }
                            if (IsReleased(Keys.S))
                            {
                                player.status = 1 ;
                            }
                            if (JustPressed(Keys.F))
                            {
                                player.status = 5;
                            }
                            if (IsHeld(Keys.F))
                            {
                                player.status = 5;
                            }
                            if (IsReleased(Keys.F))
                            {
                                flag_finish_beaten = true;
                            }
                */

                if (JustPressed(Keys.Left))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else if (player.direction == 1)
                    {
                        player.direction = -1;
                        player.status = 2;
                        player.finish_walking = false;
                    }
                }
                if (IsHeld(Keys.Left))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < tree.Count; i++)
                        {
                            // if (tree.ElementAt(i).Intersects(player.cell) && tree.ElementAt(i).X + tree.ElementAt(i).Width >= player.cell.X&&intersect_direction==2)
                            if (tree.ElementAt(i).Intersects(player.cell) && object_direction(player.cell, tree.ElementAt(i)) == 1)
                            {
                                flag = true;
                            }

                        }
                        for (int i = 0; i < monster.Count; i++)
                        {

                            // if (monster.ElementAt(i).cell.Intersects(player.cell) && monster.ElementAt(i).cell.X + monster.ElementAt(i).cell.Width >= player.cell.X&&intersect_direction==2)
                            if (monster.ElementAt(i).cell.Intersects(player.cell) && object_direction(player.cell, monster.ElementAt(i).cell) == 1)
                            {
                                flag = true;
                                monster.ElementAt(i).approach_player = true;
                            }
                            else
                                monster.ElementAt(i).approach_player = false;
                        }

                        if (!flag)
                            player.cell.X -= player.speed;

                        else
                            player.cell.X -= 0;
                        player.status = 2;
                    }
                }
                if (IsReleased(Keys.Left))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    player.status = 1;
                }
                ///////////////////////////////////////////////////////////

                if (JustPressed(Keys.Right))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else if (player.direction == -1)
                    {
                        player.direction = 1;
                        player.status = 2;
                        player.finish_walking = false;
                    }
                }
                if (IsHeld(Keys.Right))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < tree.Count; i++)
                        {

                            // if (tree.ElementAt(i).Intersects(player.cell) && tree.ElementAt(i).X <= player.cell.X + player.cell.Width&&intersect_direction==1)
                            if (tree.ElementAt(i).Intersects(player.cell) && object_direction(player.cell, tree.ElementAt(i)) == 2)
                            {
                                flag = true;
                            }
                        }
                        for (int i = 0; i < monster.Count; i++)
                        {
                            // if (monster.ElementAt(i).cell.Intersects(player.cell) && monster.ElementAt(i).cell.X <= player.cell.X + player.cell.Width&&intersect_direction==1)
                            if (monster.ElementAt(i).cell.Intersects(player.cell) && object_direction(player.cell, monster.ElementAt(i).cell) == 2)
                            {
                                flag = true;
                                monster.ElementAt(i).approach_player = true;
                            }
                            else
                                monster.ElementAt(i).approach_player = false;
                        }
                        if (!flag)
                            player.cell.X += player.speed;

                        else
                            player.cell.X += 0;
                        player.status = 2;
                    }
                }
                if (IsReleased(Keys.Right))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    player.status = 1;
                }
                ////////////////////////////////////////////////////////////////


                if (JustPressed(Keys.Up))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        player.status = 2; player.finish_walking=true;
                    }
                }
                if (IsHeld(Keys.Up))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < tree.Count; i++)
                        {

                            // if (tree.ElementAt(i).Intersects(player.cell) && tree.ElementAt(i).Y + tree.ElementAt(i).Height >= player.cell.Y&&intersect_direction==3)
                            if (tree.ElementAt(i).Intersects(player.cell) && object_direction(player.cell, tree.ElementAt(i)) == 3)
                            {
                                flag = true;

                            }
                        }
                        for (int i = 0; i < monster.Count; i++)
                        {

                            //  if (monster.ElementAt(i).cell.Intersects(player.cell) && monster.ElementAt(i).cell.Y + monster.ElementAt(i).cell.Height >= player.cell.Y&&intersect_direction==3)
                            if (monster.ElementAt(i).cell.Intersects(player.cell) && object_direction(player.cell, monster.ElementAt(i).cell) == 2)
                            {
                                flag = true;
                                monster.ElementAt(i).approach_player = true;

                            }
                            else
                                monster.ElementAt(i).approach_player = false;
                        }


                        if (!flag)
                            player.cell.Y -= player.speed;

                        else
                            player.cell.Y -= 0;

                        player.status = 2;
                    }
                }
                if (IsReleased(Keys.Up))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    player.status = 1;
                }



                ///////////////////////////////////////////////////////////////////////


                if (JustPressed(Keys.Down))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        player.status = 2; player.finish_walking = true;
                    }
                }
                if (IsHeld(Keys.Down))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < tree.Count; i++)
                        {

                            //   if (tree.ElementAt(i).Intersects(player.cell) && tree.ElementAt(i).Y <= player.cell.Y + player.cell.Height&&intersect_direction==4)
                            if (tree.ElementAt(i).Intersects(player.cell) && object_direction(player.cell, tree.ElementAt(i)) == 4)

                                flag = true;


                        }
                        for (int i = 0; i < monster.Count; i++)
                        {

                            //  if (monster.ElementAt(i).cell.Intersects(player.cell) && monster.ElementAt(i).cell.Y <= player.cell.Y + player.cell.Height&&intersect_direction==4)
                            if (monster.ElementAt(i).cell.Intersects(player.cell) && object_direction(player.cell, monster.ElementAt(i).cell) == 4)
                            {
                                flag = true;
                                monster.ElementAt(i).approach_player = true;

                            }
                            else
                                monster.ElementAt(i).approach_player = false;
                        }
                        if (!flag)

                            player.cell.Y += player.speed;
                        else
                            player.cell.Y += 0;

                        player.status = 2;
                    }
                }
                if (IsReleased(Keys.Down))
                {
                    if (!player.finish_beaten)
                        player.status = 5;
                    else
                    player.status = 1;
                }


                /////////////////////////////////////////////////////////////////////////////////
                if (JustPressed(Keys.D))
                {
                    if (player.isReloading)
                        player.status = 6;
                    else if (player.isChangingWeapon)
                        player.status = 7;
                    else if (!player.finish_attacking)
                        player.status = 4;
                    else  if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        player.finish_attacking = false;
                        player.status = 4;
                        if (player.weapon == 1)
                        {
                            for (int i = 0; i < monster.Count; i++)
                            {
                                //if (monster.ElementAt(i).approach_player)
                                if (Math.Abs(player.cell.X - monster.ElementAt(i).cell.X) <= 30 && Math.Abs(player.cell.Y - monster.ElementAt(i).cell.Y) <= 40)
                                    player.attack(monster.ElementAt(i));
                            }
                        }
                        else
                        {
                            wave bullet;
                            if (player.direction == 1)
                                bullet = new wave((int)player_spawn.X + 30, (int)player_spawn.Y + 10, player.direction, player.hit_point(player.weapon), player.hit_prob, player.name, player.id);
                            else
                                bullet = new wave((int)player_spawn.X, (int)player_spawn.Y + 10, player.direction, player.hit_point(player.weapon), player.hit_prob, player.name, player.id);
                            missle.AddLast(bullet);
                        }
                    }
                }
                if (IsHeld(Keys.D))
                {
                    if (player.isReloading)
                        player.status = 6;
                    else if (player.isChangingWeapon)
                        player.status = 7;
                    else if (!player.finish_attacking)
                        player.status = 4;
                    else if (!player.finish_beaten)
                        player.status = 5;
                    else
                    {
                        player.finish_attacking = false;
                        player.status = 4;
                        if (player.weapon == 1)
                        {
                            for (int i = 0; i < monster.Count; i++)
                            {
                                //if (monster.ElementAt(i).approach_player)
                                if (Math.Abs(player.cell.X - monster.ElementAt(i).cell.X) <= 30 && Math.Abs(player.cell.Y - monster.ElementAt(i).cell.Y) <= 40)
                                    player.attack(monster.ElementAt(i));
                            }
                        }
                        else
                        {
                            wave bullet;
                            if (player.direction == 1)
                                bullet = new wave((int)player_spawn.X + 30, (int)player_spawn.Y + 10, player.direction, player.hit_point(player.weapon), player.hit_prob, player.name, player.id);
                            else
                                bullet = new wave((int)player_spawn.X, (int)player_spawn.Y + 10, player.direction, player.hit_point(player.weapon), player.hit_prob, player.name, player.id);
                            missle.AddLast(bullet);
                        }
                    }
                }
                if (IsReleased(Keys.D))
                {
                    //flag_finish_attacking = true;
                }

                //////////////////////////////////////////////////////////////////
                //  keep sprites within the window
                /////////////////////////////////////////////////////////////////

                if (player.cell.X < 0)
                    player.cell.X = 0;
                if (player.cell.Y < 0)
                    player.cell.Y = 0;
                if (player.cell.X > Window.ClientBounds.Width - 30)
                    player.cell.X = Window.ClientBounds.Width - 30;
                if (player.cell.Y > Window.ClientBounds.Height / 4 * 3 - 40)
                    player.cell.Y = Window.ClientBounds.Height / 4 * 3 - 40;
                if (player.isReloading)
                {
                    player.reload_timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (player.reload_timer > player.reloading_time)
                    {
                        player.reload_timer -= player.reloading_time;
                        player.isReloading = false;
                        player.status = 1;
                        player.reload = 0;
                    }
                }
                if (player.isChangingWeapon)
                {
                    player.changing_weapon_timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (player.changing_weapon_timer > player.changing_weapon_time)
                    {
                        player.changing_weapon_timer -= player.changing_weapon_time;
                        player.isChangingWeapon = false;
                        player.status = 1;
                    }
                }
                player_spawn.X = player.cell.X;
                player_spawn.Y = player.cell.Y;
            }
////////////////////////////////////////////////////////////////////////////////////////
////////     Finishing updating player                                          ///////
///////////////////////////////////////////////////////////////////////////////////////

//////////////Update Missle////////////////////////////////////////////////////////////

            for (int i = 0; i < missle.Count(); i++)
            {
                
                missle.ElementAt(i).cell.X += missle.ElementAt(i).direction * missle.ElementAt(i).speed;//missle flying
                //check if missle has flied out of window
                if (missle.ElementAt(i).cell.X < 0)
                { missle.Remove(missle.ElementAt(i)); i--; }
                else if (missle.ElementAt(i).cell.X > Window.ClientBounds.Width)
                { missle.Remove(missle.ElementAt(i)); i--; }

                //check if missle has hit the player
                else if (missle.ElementAt(i).cell.Intersects(player.cell))
                {
                    if (missle.ElementAt(i).owner != player.name || missle.ElementAt(i).owner_id != player.id)//cannot be damaged by self missle
                    {
                        player.damageby(missle.ElementAt(i).damage, missle.ElementAt(i).hitrate);
                        missle.Remove(missle.ElementAt(i)); i--;
                    }
                }
                else
                {
                    //check if missle has hit the monster
                    for (int j = 0; j < monster.Count(); j++)
                    {
                        if (missle.ElementAt(i).cell.Intersects(monster.ElementAt(j).cell))
                        {
                            //if (missle.ElementAt(i).owner != monster.ElementAt(j).name || missle.ElementAt(i).owner_id != monster.ElementAt(j).id) //cannot damaged by self missle
                            if(missle.ElementAt(i).owner==player.name)//only player's missle can damage the monsters
                            {
                                monster.ElementAt(j).damageby(missle.ElementAt(i).damage, missle.ElementAt(i).hitrate);
                                missle.Remove(missle.ElementAt(i)); i--;
                                break;
                            }
                        }
                    }
                }
            }


///////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////        Finishing Updating Missles                                //////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////Update Monsters////////////////////////////////////////////////////////////

            for (int i = 0; i < monster.Count; i++)
            {
               
                   /* if (!monster.ElementAt(i).finish_walking)
                   monster.ElementAt(i).status = 2;
                else */if (!monster.ElementAt(i).finish_attacking)
                    monster.ElementAt(i).status = 4;
                else if (!monster.ElementAt(i).finish_beaten)
                    monster.ElementAt(i).status = 5;
                else if (!monster.ElementAt(i).finish_deading)
                    monster.ElementAt(i).status = 3;
                else//*/
                if (monster.ElementAt(i).status == 3)
                       { monster.Remove(monster.ElementAt(i));i--; }
                else if (rand.Next(0, 100) > 35)  //65% possibility the monster is running
                       {
                           monster.ElementAt(i).status = 2;
                           monster.ElementAt(i).finish_walking = false;
                           if ((monster.ElementAt(i).currentHealth < monster.ElementAt(i).MaxHealth / 3) && (Math.Abs(monster.ElementAt(i).cell.X - player.cell.X) <= 100 && Math.Abs(monster.ElementAt(i).cell.Y - player.cell.Y) <= 100)) //if monster's HP is lower than one third of the whole HP, use evading move running
                               monster.ElementAt(i).moving_state = 3;

                           else if (Math.Abs(monster.ElementAt(i).cell.X - player.cell.X) <= 100 && Math.Abs(monster.ElementAt(i).cell.Y - player.cell.Y) <= 100)// && monster.ElementAt(i).moving_state != 3)
                               monster.ElementAt(i).moving_state = 2;

                           //if (monster.ElementAt(i).moving_state != 3 || monster.ElementAt(i).moving_state != 2)
                           else monster.ElementAt(i).moving_state = 1;
                           ////////////////////////////////Finishing Updating monsters moving status///////////////////////////////////////

                           switch (monster.ElementAt(i).moving_state)
                           {
                               case 1://random moving
                                   {
                                       monster.ElementAt(i).random_move(player, monster);
                                       break;
                                   }
                               case 2://chase moving
                                   {
                                       monster.ElementAt(i).chasing_move(player, monster);
                                       break;
                                   }
                               case 3://evading moving
                                   {
                                       monster.ElementAt(i).evading_move(player, monster);
                                       break;
                                   }
                           }
                           ////////////////////////////Finishing Updating Monsters moving////////////////////////////////////
                           if (rand.Next(0, 99) < monster.ElementAt(i).Willing_to_hit)
                           {
                               if (!monster.ElementAt(i).isChangingWeapon)
                               {
                                   if (monster.ElementAt(i).weapon == 1)
                                   {
                                       // if (monster.ElementAt(i).approach_player)
                                       if (Math.Abs(player.cell.X - monster.ElementAt(i).cell.X) <= 30 && Math.Abs(player.cell.Y - monster.ElementAt(i).cell.Y) <= 40)
                                       {
                                           monster.ElementAt(i).attack(player);
                                           monster.ElementAt(i).status = 4;
                                           monster.ElementAt(i).finish_attacking = false;
                                       }
                                   }
                                   else
                                   {
                                       if (monster.ElementAt(i).isReloading)
                                           monster.ElementAt(i).status = 6;
                                       else
                                       {
                                           wave bullet = new wave((int)monster.ElementAt(i).cell.X, (int)monster.ElementAt(i).cell.Y, monster.ElementAt(i).direction, monster.ElementAt(i).hit_point(monster.ElementAt(i).weapon), monster.ElementAt(i).hit_prob, monster.ElementAt(i).name, monster.ElementAt(i).id);
                                           missle.AddLast(bullet);
                                           monster.ElementAt(i).status = 4;
                                           monster.ElementAt(i).finish_attacking = false;
                                       }
                                   }
                               }
                               else
                                   monster.ElementAt(i).status = 7;
                           }

/////////////////////////////////////Finishing Updating attacking////////////////////////////////////////////////
                          monster.ElementAt(i).monster_delay_timer += gameTime.ElapsedGameTime.Milliseconds;
                           if (monster.ElementAt(i).monster_delay_timer > monster.ElementAt(i).monster_delay_time)
                           {
                               monster.ElementAt(i).monster_delay_timer -= monster.ElementAt(i).monster_delay_time;

                               if (rand.Next(0, 100) < 30)//changing weapon prob is set 30%
                               {
                                   if (!monster.ElementAt(i).isChangingWeapon)
                                   {
                                       monster.ElementAt(i).changeweapon(rand.Next(0, 2));
                                       monster.ElementAt(i).isChangingWeapon = true;
                                       monster.ElementAt(i).status = 7;
                                   }
                                   else
                                       monster.ElementAt(i).status = 7;
                               }
                               ///////////////////////////Finishing Updating Monsters changing weapon///////////////////////////

                               ///////////////////////////Updating trash talk////////////////////////////////////////////////
                               if (rand.Next(0, 99) < 30)
                               {
                                   monster.ElementAt(i).trash_talk(monster.ElementAt(i).moving_state);
                               }

                           }
                       }
                       else
                       {
                           monster.ElementAt(i).monster_walk_delay_timer += gameTime.ElapsedGameTime.Milliseconds;
                           if (monster.ElementAt(i).monster_walk_delay_timer > monster.ElementAt(i).monster_walk_delay_time)
                           {
                               monster.ElementAt(i).monster_walk_delay_timer -= monster.ElementAt(i).monster_walk_delay_time;
                               monster.ElementAt(i).status = 1;
                           }


                           if (monster.ElementAt(i).isReloading)
                           {
                               monster.ElementAt(i).reload_timer += gameTime.ElapsedGameTime.Milliseconds;
                               if (monster.ElementAt(i).reload_timer > monster.ElementAt(i).reloading_time)
                               {
                                   monster.ElementAt(i).reload_timer -= monster.ElementAt(i).reloading_time;
                                   monster.ElementAt(i).isReloading = false;
                                   monster.ElementAt(i).status = 1;
                                   monster.ElementAt(i).reload = 0;
                               }
                           }
                           if (monster.ElementAt(i).isChangingWeapon)
                           {
                               monster.ElementAt(i).changing_weapon_timer += gameTime.ElapsedGameTime.Milliseconds;
                               if (monster.ElementAt(i).changing_weapon_timer > monster.ElementAt(i).changing_weapon_time)
                               {
                                   monster.ElementAt(i).changing_weapon_timer -= monster.ElementAt(i).changing_weapon_time;
                                   monster.ElementAt(i).isChangingWeapon = false;
                                   monster.ElementAt(i).status = 1;
                               }
                           }
                       }
            }
////////////////////////////Finishing Updating Monsters attacking/////////////////////////////////

////////////////////////////////Finishing Updating Monsters///////////////////////////////////////////////////


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.SandyBrown);
            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);


            switch (player.status)
            {
                case 1:
                    {
                        //spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, 30, 4), Color.Red);
                        //spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, (int)(30 * (double)player.currentHealth / player.MaxHealth), 4), Color.GreenYellow);
                          spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 5, (int)(30 * (double)player.currentHealth / player.MaxHealth), 3), Color.GreenYellow);
                          spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X+(int)(30 * (double)player.currentHealth / player.MaxHealth), (int)player_spawn.Y - 5, (int)(30 * (1-(double)player.currentHealth /player.MaxHealth)),3), Color.Red);
                        if(player.direction==1)
                        spriteBatch.Draw(playerpic, player_spawn, standing, Color.White); 
                        else
                            spriteBatch.Draw(playerpic, player_spawn, standing, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                case 2:
                    {
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 5, (int)(30 * (double)player.currentHealth / player.MaxHealth), 3), Color.GreenYellow);
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X + (int)(30 * (double)player.currentHealth / player.MaxHealth), (int)player_spawn.Y - 5, (int)(30 * (1 - (double)player.currentHealth / player.MaxHealth)), 3), Color.Red);


                       // spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, 30, 4), Color.Red);
                       // spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, (int)(30 * (double)player.currentHealth / player.MaxHealth), 4), Color.GreenYellow);


                        running = new Rectangle(frameSize[running_frame_counter], starting_running_frame.Y, frameSize[running_frame_counter+1]-frameSize[running_frame_counter], frameheight);
                       // running=new Rectangle(zombie_walking_frameSize[zombie_walking_counter],start_zombie_walking.Y,zombie_walking_frameSize[zombie_walking_counter+1]-zombie_walking_frameSize[zombie_walking_counter],zombie_walking_frameheight);
                       if (player.direction == 1)
                           spriteBatch.Draw(playerpic, player_spawn, running, Color.White);
                       else
                           spriteBatch.Draw(playerpic, player_spawn, running, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);


                        timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                        if (timeSinceLastFrame > millisecondsPerFrame)
                        {
                            timeSinceLastFrame -= millisecondsPerFrame;
                            if (running_frame_counter < 7)
                                running_frame_counter++;
                          //  if (zombie_walking_counter < 9)
                            //    zombie_walking_counter+=2;
                            else
                            {
                                player.finish_walking = true;
                                running_frame_counter = 0;
                                //zombie_walking_counter = 0;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (!playerisdead)
                        {
                            deading = new Rectangle(deading_frameSize[deading_frame_counter], starting_deading_frame.Y, deading_frameSize[deading_frame_counter + 1] - deading_frameSize[deading_frame_counter], deading_frameheight);
                            if (player.direction == 1)
                                spriteBatch.Draw(playerpic, player_spawn, deading, Color.White);
                            else
                                spriteBatch.Draw(playerpic, player_spawn, deading, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                            timeSinceLastFrame_deading += gameTime.ElapsedGameTime.Milliseconds;
                            if (timeSinceLastFrame_deading > millisecondsPerFrame_deading)
                            {
                                timeSinceLastFrame_deading -= millisecondsPerFrame_deading;

                                if (deading_frame_counter < 7)
                                    deading_frame_counter += 2;
                                else
                                {
                                    deading_frame_counter = 0;
                                    player.finish_deading = true;
                                    player.status = 3;
                                    player.isAlive = false;
                                    playerisdead = true;
                                }
                            }

                        }
                        else
                        {
                            deading = new Rectangle(deading_frameSize[8], starting_deading_frame.Y, deading_frameSize[9] - deading_frameSize[8], deading_frameheight);
                            if (player.direction == 1)
                                spriteBatch.Draw(playerpic, player_spawn, deading, Color.White);
                            else
                                spriteBatch.Draw(playerpic, player_spawn, deading, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                            spriteBatch.DrawString(font, "YOU DIED!", new Vector2(300, 250), Color.Red, 0, Vector2.Zero, 5f, SpriteEffects.None, 0);
                        }
                        break; 
                    }
                case 4:
                    {
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 5, (int)(30 * (double)player.currentHealth / player.MaxHealth), 3), Color.GreenYellow);
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X + (int)(30 * (double)player.currentHealth / player.MaxHealth), (int)player_spawn.Y - 5, (int)(30 * (1 - (double)player.currentHealth / player.MaxHealth)), 3), Color.Red);

                       // spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, 30, 4), Color.Red);
                       // spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, (int)(30 * (double)player.currentHealth / player.MaxHealth), 4), Color.GreenYellow);


                        if (player.weapon == 1)
                        {
                            attacking = new Rectangle(attacking_frameSize[attacking_frame_counter], starting_attacking_frame.Y, attacking_frameSize[attacking_frame_counter + 1] - attacking_frameSize[attacking_frame_counter], attack_frameheight);
                            if (player.direction == 1)
                                spriteBatch.Draw(playerpic, player_spawn, attacking, Color.White);
                            else
                                spriteBatch.Draw(playerpic, player_spawn, attacking, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);


                            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                            if (timeSinceLastFrame > millisecondsPerFrame_attacking)
                            {
                                timeSinceLastFrame -= millisecondsPerFrame_attacking;

                                if (attacking_frame_counter < 5)
                                    attacking_frame_counter++;
                                else
                                {
                                    //if (flag_finish_attacking)
                                    player.finish_attacking = true;
                                        player.status = 1;
                                    attacking_frame_counter = 0;
                                }
                            }
                        }
                        else 
                        {
                            Kamehameha = new Rectangle(Kamehameha_frameSize[Kamehameha_frame_counter], starting_Kamehameha_frame.Y, Kamehameha_frameSize[Kamehameha_frame_counter + 1] - Kamehameha_frameSize[Kamehameha_frame_counter], Kamehameha_frameheight);
                            if (player.direction == 1)
                                spriteBatch.Draw(playerpic, player_spawn, Kamehameha, Color.White);
                            else
                                spriteBatch.Draw(playerpic, player_spawn, Kamehameha, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);

                            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                            if (timeSinceLastFrame > millisecondsPerFrame_attacking)
                            {
                                timeSinceLastFrame -= millisecondsPerFrame_attacking;

                                if (Kamehameha_frame_counter < 9)
                                    Kamehameha_frame_counter += 2;
                                else
                                {
                                    //if (flag_finish_attacking)
                                    player.finish_attacking = true;
                                        player.status = 1;
                                    Kamehameha_frame_counter = 0;

                                }
                            }
                        }
                        break;
                    }
                case 5:
                    {
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 5, (int)(30 * (double)player.currentHealth / player.MaxHealth), 3), Color.GreenYellow);
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X + (int)(30 * (double)player.currentHealth / player.MaxHealth), (int)player_spawn.Y - 5, (int)(30 * (1 - (double)player.currentHealth / player.MaxHealth)), 3), Color.Red);

                       // spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, 30, 4), Color.Red);
                       // spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 4, (int)(30 * (double)player.currentHealth / player.MaxHealth), 4), Color.GreenYellow);
                        beaten = new Rectangle(beaten_frameSize[beaten_frame_counter], starting_beaten_frame.Y, beaten_frameSize[beaten_frame_counter + 1] - beaten_frameSize[beaten_frame_counter], beaten_frameheight);

                        if (player.direction == 1)
                            spriteBatch.Draw(playerpic, player_spawn, beaten, Color.White);
                        else
                            spriteBatch.Draw(playerpic, player_spawn, beaten, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);

                        timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                        if (timeSinceLastFrame > millisecondsPerFrame)
                        {
                            timeSinceLastFrame -= millisecondsPerFrame;
                            if (beaten_frame_counter < 2)
                                beaten_frame_counter += 2;
                            else
                            {
                                //flag_finish_beaten=true;
                                player.finish_beaten = true;
                                    player.status = 1;
                                beaten_frame_counter = 0;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                          spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 5, (int)(30 * (double)player.currentHealth / player.MaxHealth), 3), Color.GreenYellow);
                          spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X+(int)(30 * (double)player.currentHealth / player.MaxHealth), (int)player_spawn.Y - 5, (int)(30 * (1-(double)player.currentHealth /player.MaxHealth)),3), Color.Red);
                        if(player.direction==1)
                        spriteBatch.Draw(playerpic, player_spawn, standing, Color.White); 
                        else
                            spriteBatch.Draw(playerpic, player_spawn, standing, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                case 7:
                    {
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X, (int)player_spawn.Y - 5, (int)(30 * (double)player.currentHealth / player.MaxHealth), 3), Color.GreenYellow);
                        spriteBatch.Draw(healthpic, new Rectangle((int)player_spawn.X + (int)(30 * (double)player.currentHealth / player.MaxHealth), (int)player_spawn.Y - 5, (int)(30 * (1 - (double)player.currentHealth / player.MaxHealth)), 3), Color.Red);
                        if (player.direction == 1)
                            spriteBatch.Draw(playerpic, player_spawn, standing, Color.White);
                        else
                            spriteBatch.Draw(playerpic, player_spawn, standing, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
            }

            if (player.beaten)
            {
                if (player.damage)
                {
                    buffer = "-" + player.damagepoint;
                    Vector2 animation = new Vector2(player_spawn.X, player_spawn.Y + 10 - damage_counter);
                    spriteBatch.DrawString(font, buffer, animation, Color.Red);


                    if (damage_counter < 40)
                        damage_counter += 2;
                    else
                    {
                        damage_counter = 0;
                        player.damage = false;
                        player.beaten = false;
                    }

                }
                else//miss
                {
                    buffer = "Miss";
                    Vector2 animation = new Vector2(player_spawn.X, player_spawn.Y + 10 - damage_counter);
                    spriteBatch.DrawString(font, buffer, animation, Color.Red);

                    if (damage_counter < 40)
                        damage_counter += 2;
                    else
                    {
                        damage_counter = 0;
                        player.damage = false;
                        player.beaten = false;
                    }

                }
            }

            position.X = Window.ClientBounds.Width / 4;
            position.Y = Window.ClientBounds.Height / 4 * 3;
            spriteBatch.Draw(healthpic, new Rectangle(0, (int)position.Y, Window.ClientBounds.Width, 1), Color.HotPink);

            if (playerisdead)
                spriteBatch.Draw(skullpic, new Rectangle(0,(int) position.Y, 200, 200), Color.White);
            else
                spriteBatch.Draw(playerpic, new Vector2(0, position.Y), new Rectangle(0, 0, 200, 100), Color.White);

            buffer = "Name: " + player.name;
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            position.Y += font.LineSpacing;
            buffer = "HP: " + player.currentHealth + "/" + player.MaxHealth;
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            buffer = "Strength: " + player.strength;
            position.Y += font.LineSpacing;
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            position.Y += font.LineSpacing;
            buffer = "Dexterity: " + player.dexterity;
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            position.Y += font.LineSpacing;
            if (player.weapon == 1)
                buffer = "Weapon: Power Pole(Sword)";
            else
                buffer = "Weapon: Turtle Wave(Missle)";
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            position.Y += font.LineSpacing;
            buffer = "Armor: " + player.armor;
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            position.Y += font.LineSpacing;
            switch (player.status)
            {
                case 1:
                    buffer = "Status: Standing";
                    break;
                case 2:
                    buffer = "Status: Running";
                    break;
                case 3:
                    buffer = "Status: Dead";
                    break;
                case 4:
                    buffer = "Status: Attacking";
                    break;
                case 5:
                    buffer = "Status: Beaten";
                    break;
                case 6:
                    buffer = "Reloading...";
                    break;
                case 7:
                    buffer="Changing weapon...";
                    break;
            }
            spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
            if (player.isAlive)
            {
                spriteBatch.Draw(healthpic, new Rectangle(10, Window.ClientBounds.Height / 4 * 3 + 105, (int)(200 * (double)player.currentHealth / player.MaxHealth), 15), Color.YellowGreen);
                spriteBatch.Draw(healthpic, new Rectangle(10 + (int)(200 * (double)player.currentHealth / player.MaxHealth), (int)Window.ClientBounds.Height / 4 * 3 + 105, (int)(200 * (1 - (double)player.currentHealth / player.MaxHealth)), 15), Color.Red);
            }
//////////////////////Draw Trees////////////////////////////////////////////////////////////////



            for (int i = 0; i < tree.Count; i++)
            {
                spriteBatch.Draw(treepic, tree.ElementAt(i), Color.White);
            }

////////////////////////Draw Monster///////////////////////////////////////////////////////////////////


            for (int i = 0; i < monster.Count; i++)
            {
               // spriteBatch.Draw(healthpic, new Rectangle((int)monster.ElementAt(i).cell.X, (int)monster.ElementAt(i).cell.Y - 4, 30, 4), Color.Red);
               // spriteBatch.Draw(healthpic, new Rectangle((int)monster.ElementAt(i).cell.X, (int)monster.ElementAt(i).cell.Y - 4, (int)(30 * (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth), 4), Color.GreenYellow);
                spriteBatch.Draw(healthpic, new Rectangle((int)monster.ElementAt(i).cell.X, (int)monster.ElementAt(i).cell.Y - 5, (int)(30 * (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth), 3), Color.GreenYellow);
                spriteBatch.Draw(healthpic, new Rectangle((int)monster.ElementAt(i).cell.X + (int)(30 * (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth), (int)monster.ElementAt(i).cell.Y - 5, (int)(30 * (1 - (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth)), 3), Color.Red);
                switch (monster.ElementAt(i).status)
                {
                    case 1: 
                    {
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_standing, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_standing, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                    case 2: 
                    {
                        zombie_walking = new Rectangle(zombie_walking_frameSize[monster.ElementAt(i).zombie_walking_counter], start_zombie_walking.Y, zombie_walking_frameSize[monster.ElementAt(i).zombie_walking_counter + 1] - zombie_walking_frameSize[monster.ElementAt(i).zombie_walking_counter], zombie_walking_frameheight);
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_walking, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_walking, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);


                       // zombie_timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                        //if (zombie_timeSinceLastFrame > zombie_millisecondsPerFrame)
                        //{
                            //zombie_timeSinceLastFrame -= zombie_millisecondsPerFrame;
                            monster.ElementAt(i).monster_delay_walking_timer += gameTime.ElapsedGameTime.Milliseconds;
                            if (monster.ElementAt(i).monster_delay_walking_timer > monster.ElementAt(i).monster_delay_walking_time)
                            {
                                monster.ElementAt(i).monster_delay_walking_timer -= monster.ElementAt(i).monster_delay_walking_time;
                                if (monster.ElementAt(i).zombie_walking_counter < 17)
                                    monster.ElementAt(i).zombie_walking_counter += 2;
                                else
                                {
                                monster.ElementAt(i).zombie_walking_counter = 0;
                                monster.ElementAt(i).finish_walking = true;
                                }
                            }
                        break;
                    }
                    case 3: 
                    {
                        zombie_deading = new Rectangle(zombie_deading_frameSize[monster.ElementAt(i).zombie_deading_counter], starting_zombie_deading.Y, zombie_deading_frameSize[monster.ElementAt(i).zombie_deading_counter + 1] - zombie_deading_frameSize[monster.ElementAt(i).zombie_deading_counter], zombie_deading_frameheight);
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_deading, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_deading, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);


                     /*   zombie_timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                        if (zombie_timeSinceLastFrame > zombie_millisecondsPerFrame)
                        {
                            zombie_timeSinceLastFrame -= zombie_millisecondsPerFrame;*/
                        monster.ElementAt(i).monster_delay_dead_timer += gameTime.ElapsedGameTime.Milliseconds;
                        if (monster.ElementAt(i).monster_delay_dead_timer > monster.ElementAt(i).monster_delay_dead_time)
                        {
                            monster.ElementAt(i).monster_delay_dead_timer -= monster.ElementAt(i).monster_delay_dead_time;
                            if (monster.ElementAt(i).zombie_deading_counter < 9)
                                monster.ElementAt(i).zombie_deading_counter += 2;
                            else
                            {
                                monster.ElementAt(i).zombie_deading_counter = 0;
                                monster.ElementAt(i).finish_deading = true;
                            }
                        }
                        break; 
                    }
                    case 4: 
                    {
                        zombie_attacking = new Rectangle(zombie_attacking_frameSize[monster.ElementAt(i).zombie_attack_counter], starting_zombie_attack.Y, zombie_attacking_frameSize[monster.ElementAt(i).zombie_attack_counter + 1] - zombie_attacking_frameSize[monster.ElementAt(i).zombie_attack_counter], zombie_attack_frameheight);
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_attacking, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_attacking, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);

                        /*
                        zombie_timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                        if (zombie_timeSinceLastFrame > zombie_millisecondsPerFrame)
                        {
                            zombie_timeSinceLastFrame -= zombie_millisecondsPerFrame;*/
                        monster.ElementAt(i).monster_delay_attacking_timer += gameTime.ElapsedGameTime.Milliseconds;
                        if (monster.ElementAt(i).monster_delay_attacking_timer > monster.ElementAt(i).monster_delay_attacking_time)
                        {
                            monster.ElementAt(i).monster_delay_attacking_timer -= monster.ElementAt(i).monster_delay_attacking_time;
                            if (monster.ElementAt(i).zombie_attack_counter < 9)
                                monster.ElementAt(i).zombie_attack_counter += 2;
                            else
                            {
                                monster.ElementAt(i).zombie_attack_counter = 0;
                                monster.ElementAt(i).finish_attacking = true;
                            }
                        }
                        break; 
                    }
                    case 5: 
                    {
                        zombie_beaten = new Rectangle(zombie_beaten_frameSize[monster.ElementAt(i).zombie_beaten_counter], starting_zombie_beaten.Y, zombie_beaten_frameSize[monster.ElementAt(i).zombie_beaten_counter + 1] - zombie_beaten_frameSize[monster.ElementAt(i).zombie_beaten_counter], zombie_beaten_frameheight);
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_beaten, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_beaten, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);


                   /*     zombie_timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                        if (zombie_timeSinceLastFrame > zombie_millisecondsPerFrame)
                        {
                            zombie_timeSinceLastFrame -= zombie_millisecondsPerFrame;*/
                        monster.ElementAt(i).monster_delay_beaten_timer += gameTime.ElapsedGameTime.Milliseconds;
                        if (monster.ElementAt(i).monster_delay_beaten_timer > monster.ElementAt(i).monster_delay_beaten_time)
                        {
                            monster.ElementAt(i).monster_delay_beaten_timer -= monster.ElementAt(i).monster_delay_beaten_time;
                            if (monster.ElementAt(i).zombie_beaten_counter < 9)
                                monster.ElementAt(i).zombie_beaten_counter += 2;
                            else
                            {
                                monster.ElementAt(i).zombie_beaten_counter = 0;
                                monster.ElementAt(i).finish_beaten = true;
                            }
                        }
                        break; 
                    }
                    case 6:
                    {
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_standing, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_standing, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                    case 7:
                    {
                        if (monster.ElementAt(i).direction == 1)
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_standing, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(monsterpic, new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y), zombie_standing, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                }

                if (monster.ElementAt(i).trash_talk_status != 0)
                {
                    buffer = monster.ElementAt(i).trash_talk_string;
                    Vector2 animation = new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y + 10 - damage_counter);
                    //spriteBatch.DrawString(font, buffer, animation, Color.Ivory);
                    spriteBatch.DrawString(font, buffer, animation, Color.Ivory, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, buffer, new Vector2(Window.ClientBounds.Width / 2 + 10, Window.ClientBounds.Height / 4 * 3 + 300), Color.Ivory, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                    if (damage_counter < 40)
                        damage_counter += 2;
                    else
                    {
                        damage_counter = 0;
                        monster.ElementAt(i).trash_talk_status = 0;
                    }

                }
                if (monster.ElementAt(i).beaten)
                {
                    if (monster.ElementAt(i).damage)
                    {
                        buffer = "-" + monster.ElementAt(i).damagepoint;
                        Vector2 animation = new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y + 10 - damage_counter);
                        spriteBatch.DrawString(font, buffer, animation, Color.Yellow);

                        if (damage_counter < 40)
                            damage_counter += 2;
                        else
                        {
                            damage_counter = 0;
                            monster.ElementAt(i).damage = false;
                            monster.ElementAt(i).beaten = false;
                        }

                    }
                    else//miss
                    {
                        buffer = "Miss";
                        Vector2 animation = new Vector2(monster.ElementAt(i).cell.X, monster.ElementAt(i).cell.Y + 10 - damage_counter);
                        spriteBatch.DrawString(font, buffer, animation, Color.Yellow);

                        if (damage_counter < 40)
                            damage_counter += 2;
                        else
                        {
                            damage_counter = 0;
                            monster.ElementAt(i).damage = false;
                            monster.ElementAt(i).beaten = false;
                        }

                    }
                }





                if (Math.Abs(monster.ElementAt(i).cell.X - player.cell.X) <= 100 && Math.Abs(monster.ElementAt(i).cell.Y - player.cell.Y) <= 100)
                {
                    position.X = Window.ClientBounds.Width / 2;
                    position.Y = Window.ClientBounds.Height / 4 * 3;
                    if(monster.ElementAt(i).status==3)
                        spriteBatch.Draw(skullpic, new Rectangle((int)position.X, (int)position.Y, 200, 200), Color.White);
                    else
                        spriteBatch.Draw(monsterpic, new Vector2(position.X+70, position.Y),zombie_standing, Color.White,0,Vector2.Zero,1.7f,SpriteEffects.None,0);

                    position.X = Window.ClientBounds.Width / 4 * 3;

                    buffer = "Name: " + monster.ElementAt(i).name;
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    position.Y += font.LineSpacing;
                    buffer = "HP: " + monster.ElementAt(i).currentHealth + "/" + monster.ElementAt(i).MaxHealth;
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    buffer = "Strength: " + monster.ElementAt(i).strength;
                    position.Y += font.LineSpacing;
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    position.Y += font.LineSpacing;
                    buffer = "Dexterity: " + monster.ElementAt(i).dexterity;
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    position.Y += font.LineSpacing;
                    if (monster.ElementAt(i).weapon == 1)
                        buffer = "Sword";
                    else
                        buffer = "Missle";
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    position.Y += font.LineSpacing;
                    buffer = "Armor: " + monster.ElementAt(i).armor;
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    position.Y += font.LineSpacing;
                    switch (monster.ElementAt(i).status)
                    {
                        case 1:
                            buffer = "Status: Standing";
                            break;
                        case 2:
                            buffer = "Status: Running";
                            break;
                        case 3:
                            buffer = "Status: Dead";
                            break;
                        case 4:
                            buffer = "Status: Attacking";
                            break;
                        case 5:
                            buffer = "Status: Beaten";
                            break;
                        case 6:
                            buffer = "Reloading...";
                            break;
                        case 7:
                            buffer = "Changing weapon...";
                            break;
                    }
                    spriteBatch.DrawString(font, buffer, new Vector2((int)position.X, (int)position.Y), Color.White);
                    if (monster.ElementAt(i).isAlive)
                    {
                        spriteBatch.Draw(healthpic, new Rectangle(Window.ClientBounds.Width / 2 + 10, Window.ClientBounds.Height / 4 * 3 + 135, (int)(200 * (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth), 15), Color.YellowGreen);
                        spriteBatch.Draw(healthpic, new Rectangle(Window.ClientBounds.Width / 2 + 10 + (int)(200 * (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth), (int)Window.ClientBounds.Height / 4 * 3 + 135, (int)(200 * (1 - (double)monster.ElementAt(i).currentHealth / monster.ElementAt(i).MaxHealth)), 15), Color.Red);
                    }
                    if (monster.ElementAt(i).trash_talk_status != 0)
                    {
                        buffer = monster.ElementAt(i).name+" says, "+monster.ElementAt(i).trash_talk_string;
                        //buffer = "Hello world";
                        spriteBatch.DrawString(font, buffer, new Vector2(Window.ClientBounds.Width / 2 + 10, Window.ClientBounds.Height / 4 * 3 + 170), Color.White, 0, Vector2.Zero, 0.9f, SpriteEffects.None, 0);



                    }
                }
            }
          //  spriteBatch.DrawString(font, "hello world", new Vector2(Window.ClientBounds.Width / 2 + 10, Window.ClientBounds.Height / 4 * 3 + 180), Color.Ivory, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
//////////////////////////Draw Missle/////////////////////////////////////////////////////////////


            for (int i = 0; i < missle.Count; i++)
            {
                spriteBatch.Draw(playerpic,new Vector2(missle.ElementAt(i).cell.X,missle.ElementAt(i).cell.Y),new Rectangle(138,632,17,17),Color.White);
            }

            if(monster.Count<=0)
                spriteBatch.DrawString(font, "YOU WIN!", new Vector2(300, 250), Color.Red, 0, Vector2.Zero, 5f, SpriteEffects.None, 0);

            //spriteBatch.Draw(grasspic, new Rectangle(0, 0, 1024, (int)(768 / 4.0 * 3)),null, Color.White,0,Vector2.Zero, SpriteEffects.None,0);
            spriteBatch.End();

            base.Draw(gameTime);
        }



        public bool IsHeld(Keys key)
        {

            if (currentKeyboard.IsKeyDown(key))
            {

                return true;

            }

            else
            {

                return false;

            }

        }


        public bool IsReleased(Keys key)
        {

            if (currentKeyboard.IsKeyUp(key) && oldKeyboard.IsKeyDown(key))
            {

                return true;

            }

            else
            {

                return false;

            }



        }


        public bool JustPressed(Keys key)
        {

            if (currentKeyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key))
            {

                return true;

            }

            else
            {

                return false;

            }

        }



        public int object_direction(Rectangle a, Rectangle b)
        {
            if (a.X > b.X)
            {
                //probably b is on the left of a
                if (a.Y > b.Y)
                {
                    //probably b is on the top of a
                    //so probably b is on the top left of a
                    if (b.Width-(a.X - b.X)  < b.Height-(a.Y - b.Y)  )
                        return 1;//b is on the left of a
                    else
                        return 3;//b is on the top of a

                }
                else
                {
                    //probably b is on the bottom of a
                    // so probably b is on the bottom left of a
                    if (b.Width - (a.X - b.X) < a.Height - (b.Y - a.Y))
                        return 1;//b is on the left of a;
                    else
                        return 4;//b is on the bottom of a
                }
            }
            else
            { 
                //probably b is on the right of a
                if (a.Y > b.Y)
                {
                    //probably b is on the top of a
                    //so probably b is on the top right of a
                    if (a.Width - (b.X - a.X) < b.Height - (a.Y - b.Y))
                        return 2;//b is on the right of a
                    else
                        return 3;//b is on the top a
                }
                else
                {
                    //probably b is on the bottom of a
                    //so probably b is on the bottom right of a
                    if (a.Width - (b.X - a.X) < a.Height - (b.Y - a.Y))
                        return 2;//b is on the right of a
                    else
                        return 4; //b is on the bottom o a

                }
            }
        }




    }
}
