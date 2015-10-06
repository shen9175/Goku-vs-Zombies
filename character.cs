using System;
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

namespace XNAtest
{


    public class wave
    {
        public int speed;
        public int damage;
        //public Point coordinate;
        public int direction;
        public Rectangle cell;
        public int hitrate;
        public string owner;
        public int owner_id;
        public wave(int x,int y,int dir,int hit,int hit_prob,string name,int id)
        {
            speed = 5;
          //  coordinate.X = x;
           // coordinate.Y = y;
            direction = dir;
            hitrate = hit_prob;
            damage = hit;
            owner = name;
            owner_id = id;
            cell = new Rectangle(x, y, 17, 17);


        }

    }
   public class character
    {
        protected Random rand;
        public string name;
        public int strength;
        public int dexterity;
        public int hit_prob;
        protected int base_hit_prob;
        public int armor;//2-9
        public int weapon;   //1 is sword   0 is missle
        public int status;   //1 is stop  2 is walking 3 is dead 4 is attacking 5 is beaten 6 is reloading 7 is changing weapon
        public int currentHealth;
        public int MaxHealth;
        public bool isAlive;
        public int speed;
        public int direction;//1 is toward right, -1 is toward left
        public Rectangle cell;
        public int id;
        protected LinkedList<Rectangle> tree;
        public bool beaten;
        public bool damage;
        public int damagepoint;
        public int reload;
        public bool isReloading;
        public int changing_weapon_time;
        public int changing_weapon_timer;
        public bool isChangingWeapon;
        public int reload_timer;
        public int reloading_time;
        public bool finish_attacking;
        public bool finish_walking;
        public bool finish_beaten;
        public bool finish_deading;

        public character(LinkedList<Rectangle> t,Random r)
        {
            rand=r;
            tree = t;
            beaten = false;
            damage = false;
            isReloading = false;
            isChangingWeapon = false;
            isAlive = true;
            finish_beaten = true;
            finish_deading = true;
            finish_attacking = true;
            finish_walking = true;
            reload_timer = 0;
            changing_weapon_timer = 0;
            reloading_time = 3000;//300
            base_hit_prob = 75;
            hit_prob = base_hit_prob;
            MaxHealth=200;
            currentHealth = MaxHealth;
            strength = rand.Next(3, 19);
            dexterity = rand.Next(3, 19);
            weapon = rand.Next(0, 2);
            armor = rand.Next(2, 10);
            changeweapon(weapon);
            status = 1;
            reload = 0;
            if (dexterity <= 12)
                changing_weapon_time = 3000;//3000
            else if (dexterity > 12 && dexterity < 18)
                changing_weapon_time = 2000;//2000
            else if(dexterity==18)
                changing_weapon_time = 0;

            if(rand.Next(0,100)>50)
                direction = 1;
            else
                direction=-1;
            id = rand.Next(0, 9999999);
        }

        public void changeweapon(int wp)
        {
            weapon = wp;
            if (weapon == 1)
            {
                //hit_prob cost due to armor
                hit_prob =base_hit_prob-(9 - armor) * 5;

                //hit_prob bonuses due to strenght
                if (strength >= 13 && strength <= 15)
                    hit_prob += 5;
                else if (strength >= 16 && strength <= 17)
                    hit_prob += 10;
                else if (strength == 18)
                    hit_prob += 15;

            }
            else//missle
            {
                //hit_prob cost due to armor
                hit_prob =base_hit_prob-(9 - armor) * 1;

                //hit_prob bonuses due to dexterity
                if (dexterity >= 13 && dexterity <= 15)
                    hit_prob += 5;
                else if (dexterity >= 16 && dexterity <= 17)
                    hit_prob += 10;
                else if (dexterity == 18)
                    hit_prob += 15;

            }

        }

        public int boundcheck(Rectangle cell)
        {
            if (cell.X < 0)
                return 0;
            if (cell.Y < 0)
                return 0;
            if (cell.X > 1024 - 30)
                return 1024 - 30;
            if (cell.Y > 768 / 4 * 3 - 40)
                return 768 / 4 * 3 - 40;
            return -1;
        }

        public bool damageby(int hit_point,int hprob)
        {
            //this.status = 5;
            beaten = true;
            if (rand.Next(0, 100) < hprob)
            {
                this.status = 5;
                this.finish_beaten = false;
                damagepoint=(int)(hit_point * (1 - (10 - armor) * 0.05));//9: 5% reduce, 8: 10% reduce, 7:15% reduce, ... 2:40% reduce
                currentHealth -= damagepoint;
                damage = true;
                if (currentHealth <= 0)
                {
                    this.finish_deading = false;
                    this.status = 3;
                    this.isAlive = false;
                    currentHealth = 0;
                }
                return true;
            }
            else
            {
                return false;//miss
            }
        }
        public int hit_point(int weapon)
        {
            if (weapon == 1)
                return rand.Next(7, 15);//(1,7)
            else
            {
                reload++;
                if(reload==10)
                    isReloading = true;
                return rand.Next(20, 30);
            }
        }

        public bool attack(character a)
        {
            if (a.isAlive)
            {
                if (a.damageby(this.hit_point(this.weapon), this.hit_prob))
                    return true;
                else
                    return false;
            }
            return false;
            
        }


    }

   public class enemy : character
   {
       public int Willing_to_hit;//(0-100):attack initiative
       string[] monster_name = new string[50] { "Merrill", "Miguel", "Laurence", "Barrett", "Kristopher", "Forrest", "Chi", "Mauro", "Antoine", "Benton", "Dale", "Lauren", "Cleveland", "Ulysses", "Leland", "Todd", "Reyes", "Wyatt", "Francisco", "Roderick", "Bennett", "Kareem", "Jesse", "Shelton", "Jon", "Jarvis", "Burl", "Demarcus", "Lowell", "Gerard", "Pablo", "Irving", "Mary", "Solomon", "Courtney", "Maynard", "Jimmy", "Calvin", "Waylon", "Ernest", "Stevie", "Fabian", "Claud", "Jermaine", "Rigoberto", "Angelo", "Kelvin", "Reuben", "Vance", "Charles" };
       int move_distance;
       int last_direction;//1-4
       public bool approach_player;
       public int moving_state;//1 is random moving  2 is chasing moving 3 is evading moving
       public string[] trash_talk_random = new string[5] { "I need to eat meat!", "Meat is better than vegetables...", "Meat,meat,meat", "Hehehe...", "hah hah..." };
       public string[] trash_talk_chase = new string[5] {"I can see you!","Where are you going?","You can't escape!","You are my dinner!","I want to eat you!"};
       public string[] trash_talk_evade = new string[5] {"Don't come any closer!","I had better to run","Let's bounce!","Help!","I don't want to die" };
       public int trash_talk_status;//1 random,2 chase, 3 evade
       public string trash_talk_string;
       public int monster_delay_timer;
       public int monster_delay_time;
       public int monster_walk_delay_timer;
       public int monster_walk_delay_time;
       public int monster_delay_walking_timer;
       public int monster_delay_walking_time;
       public int monster_delay_attacking_timer;
       public int monster_delay_attacking_time;
       public int monster_delay_beaten_timer;
       public int monster_delay_beaten_time;
       public int monster_delay_dead_timer;
       public int monster_delay_dead_time;
       public int zombie_walking_counter;
       public int zombie_attack_counter;
       public int zombie_beaten_counter;
       public int zombie_deading_counter;
       public enemy(Rectangle rect, LinkedList<Rectangle> t,Random r): base(t,r)
       {
           monster_delay_timer = 0;
           monster_delay_time = 800;//800
           monster_walk_delay_timer = 0;
           monster_walk_delay_time = 1500;//1500
           name = monster_name[rand.Next(0, 50)];
           monster_delay_attacking_time = 75;
           monster_delay_attacking_timer = 0;
           monster_delay_beaten_time = 75;
           monster_delay_beaten_timer = 0;
           monster_delay_dead_time = 75;
           monster_delay_dead_timer = 0;
           monster_delay_walking_time = 75;
           monster_delay_walking_timer = 0;
           zombie_walking_counter = 0;
           zombie_attack_counter = 0;
           zombie_beaten_counter = 0;
           zombie_deading_counter = 0;
           Willing_to_hit = rand.Next(50, 100);
           speed = rand.Next(1,3);
           cell = rect;
           move_distance = 10;
           last_direction = rand.Next(1, 5);
           if (last_direction == 1)
               direction = -1;
           else if (last_direction == 2)
               direction = 1;
           approach_player = false;
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
                   if (b.Width - (a.X - b.X) < b.Height - (a.Y - b.Y))
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

       public void trash_talk(int a)
       {
           trash_talk_status = a;
           
           switch (trash_talk_status)
           {
               case 1:
                   {
                       trash_talk_string = trash_talk_random[rand.Next(0, 5)];
                       break;
                   }
               case 2:
                   {
                   trash_talk_string = trash_talk_chase[rand.Next(0, 5)];
                       break;
                   }
               case 3:
                   {
                       trash_talk_string = trash_talk_evade[rand.Next(0, 5)];
                       break;
                   }
           }
 
       }


       public void random_move(character a, LinkedList<enemy> m)
       {
           approach_player = false;  
           if (move_distance > 0)
           {
               move_distance--;
               switch (last_direction)
               {
                   case 1://left move
                       {
                           if (cell.X < 0)
                           {
                               cell.X = 0;
                               move_distance = 0;
                           }
                           else
                           {
                               for(int i=0;i<tree.Count;i++)//check trees
                               {
                                   //if(cell.X<tree.ElementAt(i).X+tree.ElementAt(i).Width && cell.Intersects(tree.ElementAt(i)))
                                   if(cell.Intersects(tree.ElementAt(i)) && object_direction(cell,tree.ElementAt(i))==1)
                                   {
                                       //cell.X=tree.ElementAt(i).X+tree.ElementAt(i).Width;
                                       move_distance=0;
                                       return;
                                   }
                               }
                               for (int i = 0; i < m.Count; i++)//check other monsters
                               {
                                   if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                                   {
                                       if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell,m.ElementAt(i).cell) == 1)
                                       {
                                           move_distance = 0;
                                           return;
                                       }

                                   }
                               }
                               if (cell.Intersects(a.cell) && object_direction(cell,a.cell) == 1)//check players
                               {
                                   approach_player = true;
                                   cell.X -= 0;
                                   return;
                               }                          
                               cell.X -= speed;
                           }
                           break;
                       }
                   case 2://right move
                       if (cell.X > 1024 - 30)
                       {
                           cell.X = 1024 - 30;
                           move_distance = 0;
                       }
                       else
                       {
                           for (int i = 0; i < tree.Count; i++)
                           {
                               //if (cell.X+cell.Width > tree.ElementAt(i).X && cell.Intersects(tree.ElementAt(i)))
                               if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 2)
                               {
                                   //cell.X = tree.ElementAt(i).X-cell.Width;
                                   move_distance = 0;
                                   return;
                               }
                           }
                           for (int i = 0; i < m.Count; i++)//check other monsters
                           {
                               if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                               {
                                   if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell,m.ElementAt(i).cell) == 2)
                                   {
                                       move_distance = 0;
                                       return;
                                   }

                               }
                           }
                           if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 2)
                           {
                               approach_player = true;
                               cell.X += 0;
                               return;
                           }
                           cell.X += speed;
                       }

                       break;
                   case 3://up move
                       if (cell.Y < 0)
                       {
                           cell.Y = 0;
                           move_distance = 0;
                       }
                       else
                       {
                           for (int i = 0; i < tree.Count; i++)
                           {
                               //if (cell.Y < tree.ElementAt(i).Y + tree.ElementAt(i).Height && cell.Intersects(tree.ElementAt(i)))
                               if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 3)
                               {
                                   //cell.Y = tree.ElementAt(i).Y + tree.ElementAt(i).Height;
                                   move_distance = 0;
                                   return;
                               }
                           }
                           for (int i = 0; i < m.Count; i++)//check other monsters
                           {
                               if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                               {
                                   if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 3)
                                   {
                                       move_distance = 0;
                                       return;
                                   }

                               }
                           }
                           if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 3)
                           {
                               approach_player = true;
                               cell.Y -= 0;
                               return;
                           } 
                           cell.Y -= speed;
                       }
                       break;
                   case 4://down move
                       if (cell.Y >= 768 / 4 * 3 - 40)
                       {
                           cell.Y = 768 / 4 * 3 - 40;
                           move_distance = 0;
                       }
                       else
                       {
                           for (int i = 0; i < tree.Count; i++)
                           {
                               //if (cell.Y +cell.Height> tree.ElementAt(i).Y && cell.Intersects(tree.ElementAt(i)))
                               if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 4)
                               {
                                   //cell.Y = tree.ElementAt(i).Y-cell.Height;
                                   move_distance = 0;
                                   return;
                               }
                           }
                           for (int i = 0; i < m.Count; i++)//check other monsters
                           {
                               if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                               {
                                   if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 4)
                                   {
                                       move_distance = 0;
                                       return;
                                   }

                               }
                           }
                           if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 4)
                           {
                               approach_player = true;
                               cell.Y += 0;
                               return;
                           }
                           cell.Y += speed;
                       }
                       break;
               }
           }
           else
           {
               //finish_walking = true;
               move_distance = 10;
               last_direction = rand.Next(1, 5);
               if (last_direction == 1)
                   direction = -1;
               else if (last_direction == 2)
                   direction = 1;
           }

       }

   public void chasing_move(character a,LinkedList<enemy> m)
    {
        approach_player = false;  
        if(a.cell.X<cell.X)
        {
            for (int i = 0; i < tree.Count; i++)
            {
                //if (cell.X < tree.ElementAt(i).X + tree.ElementAt(i).Width && cell.Intersects(tree.ElementAt(i)))
                if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 1)
                {
                    if (cell.Y - cell.Height/2 < tree.ElementAt(i).Y - tree.ElementAt(i).Height / 2)
                        cell.Y -= speed;
                    else
                        cell.Y += speed;
                    return;
                }
            }
            for (int i = 0; i < m.Count; i++)//check other monsters
            {
                if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                {
                    if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 1)
                    {
                        if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                            cell.Y -= speed;
                        else
                            cell.Y += speed;
                        return;
                    }

                }
            }
            if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 1)
            {
                approach_player = true;
                cell.X -= 0;
                return;
            }
            cell.X-=speed;
            direction = -1;
        }
        else if (a.cell.X > cell.X)
        {
            for (int i = 0; i < tree.Count; i++)
            {
                //if (cell.X + cell.Width > tree.ElementAt(i).X && cell.Intersects(tree.ElementAt(i)))
                if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 2)
                {
                    if (cell.Y - cell.Height/2 < tree.ElementAt(i).Y - tree.ElementAt(i).Height / 2)
                        cell.Y -= speed;
                    else
                        cell.Y += speed;
                    return;
                }
            }
            for (int i = 0; i < m.Count; i++)//check other monsters
            {
                if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                {
                    if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 2)
                    {
                        if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                            cell.Y -= speed;
                        else
                            cell.Y += speed;
                        return;
                    }

                }
            }
            if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 2)
            {
                approach_player = true;
                cell.X -= 0;
                return;
            }
            cell.X += speed;
            direction = 1;
        }
        if (a.cell.Y < cell.Y)
        {
            for (int i = 0; i < tree.Count; i++)
            {
                //if (cell.Y< tree.ElementAt(i).Y + tree.ElementAt(i).Height && cell.Intersects(tree.ElementAt(i)))
                if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 3)
                {
                    if (cell.X + cell.Width / 2 < tree.ElementAt(i).X + (tree.ElementAt(i).Width / 2))
                    { cell.X -= speed; direction = -1; }
                    else
                    { cell.X += speed; direction = 1; }
                    return;
                }
            }
            for (int i = 0; i < m.Count; i++)//check other monsters
            {
                if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                {
                    if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 3)
                    {
                        if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                        { cell.X -= speed; direction = -1; }
                        else
                        { cell.X += speed; direction = 1; }
                        return;
                    }

                }
            }
            if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 3)
            {
                approach_player = true;
                cell.X -= 0;
                return;
            }
            cell.Y -= speed;
        }
        else if (a.cell.Y > cell.Y)
        {
            for (int i = 0; i < tree.Count; i++)
            {
                //if (cell.Y + cell.Height > tree.ElementAt(i).Y && cell.Intersects(tree.ElementAt(i)))
                if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell,tree.ElementAt(i)) == 4)
                {
                    if (cell.X + cell.Width / 2 < tree.ElementAt(i).X + (tree.ElementAt(i).Width / 2))
                    { cell.X -= speed; direction = -1; }
                    else
                    { cell.X += speed; direction = 1; }
                    return;
                }
            }
            for (int i = 0; i < m.Count; i++)//check other monsters
            {
                if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                {
                    if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 4)
                    {
                        if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                        { cell.X -= speed; direction = -1; }
                        else
                        { cell.X += speed; direction = 1; }
                        return;
                    }

                }
            }
            if (cell.Intersects(a.cell) && object_direction(cell, a.cell) == 4)
            {
                approach_player = true;
                cell.X -= 0;
                return;
            }
            cell.Y += speed;
        }
    }

       public void evading_move(character a,LinkedList<enemy> m)
    {
        approach_player = false;  
        if(a.cell.X<cell.X)
        {
            if (cell.X >= 1024 - 30)
            {
                cell.X = 1024 - 30;

                if (a.cell.X < 1024 - 30)
                {
                    if (rand.Next(0, 2) == 0)
                        cell.Y -= speed;
                    else
                        cell.Y += speed;
                    return;
                }
                else
                {
                    cell.X -= speed; direction = -1;
                    return;
                }


            }
            else
            {

                for (int i = 0; i < tree.Count; i++)
                {
                    if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 2)
                    {
                        if (cell.Y - cell.Height / 2 < tree.ElementAt(i).Y - tree.ElementAt(i).Height / 2)
                            cell.Y -= speed;
                        else
                            cell.Y += speed;
                        return;
                    }
                }
                for (int i = 0; i < m.Count; i++)//check other monsters
                {
                    if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                    {
                        if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell,m.ElementAt(i).cell) == 2)
                        {
                            if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                                cell.Y -= speed;
                            else
                                cell.Y += speed;
                            return;
                        }

                    }
                }
                cell.X += speed; direction = 1;
            }
        }
        else if (a.cell.X > cell.X)
        {
            if (cell.X <= 0)
            {
                cell.X = 0;
                if (a.cell.X > 0)
                {
                    if (rand.Next(0, 2) == 0)
                        cell.Y -= speed;
                    else
                        cell.Y += speed;
                    return;
                }
                else
                {
                    cell.X += speed; direction = 1;
                    return;
                }
            }
            else
            {
                for (int i = 0; i < tree.Count; i++)
                {
                    //if (cell.X + cell.Width > tree.ElementAt(i).X && cell.Intersects(tree.ElementAt(i)))
                    if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 1)
                    {
                        if (cell.Y - cell.Height / 2 < tree.ElementAt(i).Y - tree.ElementAt(i).Height / 2)
                            cell.Y -= speed;
                        else
                            cell.Y += speed;
                        return;
                    }
                }
                for (int i = 0; i < m.Count; i++)//check other monsters
                {
                    if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                    {
                        if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 1)
                        {
                            if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                                cell.Y -= speed;
                            else
                                cell.Y += speed;
                            return;
                        }

                    }
                }
                cell.X -= speed; direction = -1;
            }
        }
        if (a.cell.Y < cell.Y)
        {
            if (cell.Y + cell.Height >= 768 / 4 * 3)
            {
                cell.Y = 768 / 4 * 3 - cell.Height;
                if (a.cell.Y + a.cell.Height < 768 / 4 * 3)
                {
                    if (rand.Next(0, 2) == 0)
                    { cell.X -= speed; direction = -1; }
                    else
                    { cell.X += speed; direction = 1; }
                    return;
                }
                else
                {
                    cell.Y -= speed;
                    return;
                }

            }
            else
            {
                for (int i = 0; i < tree.Count; i++)
                {
                    if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 4)
                    {
                        if (cell.X + cell.Width / 2 < tree.ElementAt(i).X + (tree.ElementAt(i).Width / 2))
                        { cell.X -= speed; direction = -1; }
                        else
                        { cell.X += speed; direction = 1; }
                        return;
                    }
                }
                for (int i = 0; i < m.Count; i++)//check other monsters
                {
                    if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                    {
                        if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 4)
                        {
                            if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                            { cell.X -= speed; direction = -1; }
                            else
                            { cell.X += speed; direction = 1; }
                            return;
                        }

                    }
                }
                cell.Y += speed;
            }
        }
        else if (a.cell.Y > cell.Y)
        {
            if (cell.Y <= 0)
            {
                cell.Y = 0;
                if (a.cell.Y > 0)
                {
                    if (rand.Next(0, 2) == 0)
                    { cell.X -= speed; direction = -1; }
                    else
                    { cell.X += speed; direction = 1; }
                    return;
                }
                else
                {
                    cell.Y += speed;
                    return;
                }
            }
            else
            {
                for (int i = 0; i < tree.Count; i++)
                {
                    if (cell.Intersects(tree.ElementAt(i)) && object_direction(cell, tree.ElementAt(i)) == 3)
                    {
                        if (cell.X + cell.Width / 2 < tree.ElementAt(i).X + (tree.ElementAt(i).Width / 2))
                        { cell.X -= speed; direction = -1; }
                        else
                        { cell.X += speed; direction = 1; }
                        return;
                    }
                }
                for (int i = 0; i < m.Count; i++)//check other monsters
                {
                    if (m.ElementAt(i).name != name || m.ElementAt(i).id != id)//exclude itself
                    {
                        if (cell.Intersects(m.ElementAt(i).cell) && object_direction(cell, m.ElementAt(i).cell) == 3)
                        {
                            if (cell.Y - cell.Height / 2 < m.ElementAt(i).cell.Y - m.ElementAt(i).cell.Height / 2)
                            { cell.X -= speed; direction = -1; }
                            else
                            { cell.X += speed; direction = 1; }
                            return;
                        }

                    }
                }
                cell.Y -= speed;
            }
        }
    }




 }
   public class hero : character
    {
       public hero(Rectangle rect, LinkedList<Rectangle> t,Random r): base(t,r)
         {
             name = "Goku";
             speed = rand.Next(3,5);
             cell = rect;
         }


    }







}
