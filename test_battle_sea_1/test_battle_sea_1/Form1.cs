using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 0 - easy         Difficulty

namespace test_battle_sea_1
{
    public partial class Form1 : Form
    {
        //Переменные

        int difficulty = 0; // сложность 
        bool player_move = true;
        int score_player = 0;
        int score_bot = 0;

        Random rnd = new Random();
        Ships[] ship = new Ships[10];
        Ships[] ship_bot = new Ships[10];
        Sqare[,] field_player = new Sqare[10, 10];
        Sqare[,] field_bot = new Sqare[10, 10];

        //картинки
        Image Ship_there_image = Properties.Resources.Ship_there;
        Image Ship_image = Properties.Resources.ship;
        Image Sqare_image = Properties.Resources.Sqare;
        Image Ship_hit_image = Properties.Resources.Sqare_hit;
        Image Ship_miss_image = Properties.Resources.Sqare_miss;
        Image Ship_hit_last_image = Properties.Resources.Ship_hit_last;
        Image Ship_miss_last_image = Properties.Resources.Ship_miss_last;
        Image Ship_bot_image = Properties.Resources.Sqare;
        Image Ship_miss_player_image = Properties.Resources.Sqare_miss_player;
        Image Ship_miss_player_ship_image = Properties.Resources.Sqare_miss_player;

        int x = 200, y = 100;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    field_player[i, j] = new Sqare();
                    field_player[i, j].Size = new Size(50, 50);
                    field_player[i, j].Location = new Point(x, y);
                    field_player[i, j].X = (x - 200)/50 + 1;
                    field_player[i, j].Y = (y - 100)/50 + 1;
                    if (j == 9)
                    {
                        x = 200;
                        y += 50;
                    }
                    else
                    {
                        x += 50;
                    }
                    field_player[i, j].BackgroundImage = Sqare_image;
                    this.Controls.Add(field_player[i, j]);
                }
            }

            x = 750;
            y = 100;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    field_bot[i, j] = new Sqare();
                    field_bot[i, j].Size = new Size(50, 50);
                    field_bot[i, j].Location = new Point(x, y);
                    field_bot[i, j].X = (x - 750)/50 + 1;
                    field_bot[i, j].Y = (y -100)/50 + 1;
                    if (j == 9)
                    {
                        x = 750;
                        y += 50;
                    }
                    else
                    {
                        x += 50;
                    }
                    field_bot[i, j].Click += new EventHandler(Click_Field_Bot);
                    field_bot[i, j].KeyDown += new KeyEventHandler(Field_bot_key_up); 
                    field_bot[i, j].MouseMove += new MouseEventHandler(Mouse_Move_Field_bot);
                    field_bot[i, j].BackgroundImage = Sqare_image;
                    this.Controls.Add(field_bot[i, j]);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                ship[i] = new Ships();
                if (i == 0)
                {
                    ship[i].Size_ship = 4;
                }
                if (i > 0 && i < 3)
                {
                    ship[i].Size_ship = 3;
                }
                if (i > 2 && i < 6)
                {
                    ship[i].Size_ship = 2;
                }
                if (i > 5)
                {
                    ship[i].Size_ship = 1;
                }
                this.Controls.Add(ship[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                ship_bot[i] = new Ships();
                if (i == 0)
                {
                    ship_bot[i].Size_ship = 4;
                }
                if (i > 0 && i < 3)
                {
                    ship_bot[i].Size_ship = 3;
                }
                if (i > 2 && i < 6)
                {
                    ship_bot[i].Size_ship = 2;
                }
                if (i > 5)
                {
                    ship_bot[i].Size_ship = 1;
                }
                this.Controls.Add(ship_bot[i]);
            }
            Respawn();
        }
        class Ships:Sqare
        {
            public int Size_ship;
        }
        class Sqare:PictureBox
        {
            public int X;
            public int Y;
            public bool busy = false;
            public bool old_click = false;

        }


        //расстановка кораблей
        private void button1_Click(object sender, EventArgs e)
        {
            Respawn();
            label2.Visible = false;
        }
        
        //отключение спавна в радиусе 1 клетки от корабля 
        private void Busy_XY_player(int X_Ships, int Y_Ships)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (X_Ships + i < 10 && X_Ships + i >= 0 && 
                        Y_Ships + j >= 0 && Y_Ships + j < 10)
                    {
                        field_player[X_Ships + i, Y_Ships + j].busy = true;
                    }
                }
            }
        }
        private void Busy_XY_bot(int X_Ships, int Y_Ships)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (X_Ships + i < 10 && X_Ships + i >= 0 &&
                        Y_Ships + j >= 0 && Y_Ships + j < 10)
                    {
                        field_bot[X_Ships + i, Y_Ships + j].busy = true;
                    }
                }
            }
        }

        //spawn кораблей
        private void Spawn_ships_player(Ships ship)
        {
            int o = 0;
            int random_y = 0;
            int random_x = 0;
            int random_dis = 0;
            while (o != ship.Size_ship)
            {
                random_dis = rnd.Next(0, 4);
                random_y = rnd.Next(0, 10);
                random_x = rnd.Next(0, 10);
                if (random_dis == 0)
                {
                    if (random_y + ship.Size_ship < 10)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_player[random_y + i, random_x].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 1;
                    }
                }
                if (random_dis == 1)
                {
                    if (random_y - ship.Size_ship >= 0)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_player[random_y - i, random_x].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 0;
                    }
                }
                if (random_dis == 2)
                {
                    if (random_x + ship.Size_ship < 10)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_player[random_y, random_x + i].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 3;
                    }
                }
                if (random_dis == 3)
                {
                    if (random_x - ship.Size_ship >= 0)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_player[random_y, random_x - i].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 2;
                    }
                }
            }
            if (o == ship.Size_ship)
            {
                if (random_dis == 0)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_player[random_y + i, random_x].BackgroundImage = Ship_image;
                        Busy_XY_player(random_y + i, random_x);
                    }
                }
                if (random_dis == 1)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_player[random_y - i, random_x].BackgroundImage = Ship_image;
                        Busy_XY_player(random_y - i, random_x);
                    }
                }
                if (random_dis == 2)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_player[random_y, random_x + i].BackgroundImage = Ship_image;
                        Busy_XY_player(random_y, random_x + i);
                    }
                }
                if (random_dis == 3)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_player[random_y, random_x - i].BackgroundImage = Ship_image;
                        Busy_XY_player(random_y, random_x - i);
                    }
                }
            }
        }
        private void Spawn_ships_bot(Ships ship)
        {
            int o = 0;
            int random_y = 0;
            int random_x = 0;
            int random_dis = 0;
            while (o != ship.Size_ship)
            {
                random_dis = rnd.Next(0, 4);
                random_y = rnd.Next(0, 10);
                random_x = rnd.Next(0, 10);
                if (random_dis == 0)
                {
                    if (random_y + ship.Size_ship < 10)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_bot[random_y + i, random_x].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 1;
                    }
                }
                if (random_dis == 1)
                {
                    if (random_y - ship.Size_ship >= 0)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_bot[random_y - i, random_x].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 0;
                    }
                }
                if (random_dis == 2)
                {
                    if (random_x + ship.Size_ship < 10)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_bot[random_y, random_x + i].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 3;
                    }
                }
                if (random_dis == 3)
                {
                    if (random_x - ship.Size_ship >= 0)
                    {
                        for (int i = 0; i < ship.Size_ship; i++)
                        {
                            if (field_bot[random_y, random_x - i].busy)
                            {
                                o = 0;
                            }
                            else
                            {
                                o++;
                            }
                        }
                    }
                    else
                    {
                        random_dis = 2;
                    }
                }
            }
            if (o == ship.Size_ship)
            {
                if (random_dis == 0)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_bot[random_y + i, random_x].BackgroundImage = Ship_bot_image;
                        Busy_XY_bot(random_y + i, random_x);
                    }
                }
                if (random_dis == 1)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_bot[random_y - i, random_x].BackgroundImage = Ship_bot_image;
                        Busy_XY_bot(random_y - i, random_x);
                    }
                }
                if (random_dis == 2)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_bot[random_y, random_x + i].BackgroundImage = Ship_bot_image;
                        Busy_XY_bot(random_y, random_x + i);
                    }
                }
                if (random_dis == 3)
                {
                    for (int i = 0; i < ship.Size_ship; i++)
                    {
                        field_bot[random_y, random_x - i].BackgroundImage = Ship_bot_image;
                        Busy_XY_bot(random_y, random_x - i);
                    }
                }
            }
        }


        private void Respawn()
        {
            visible_label();
            label2.Visible = false;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    field_bot[i, j].Visible = true;
                    field_player[i, j].Visible = true;
                }
            }
            player_move = true;
            score_bot = 0;
            score_player = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    field_player[i, j].busy = false;
                    field_bot[i, j].busy = false;
                    field_player[i, j].BackgroundImage = Sqare_image;
                    field_bot[i, j].BackgroundImage = Sqare_image;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Spawn_ships_player(ship[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                Spawn_ships_bot(ship_bot[i]);
            }
        }


        // следующие 2 функции - клик по полю /////////////////////////
        private void Click_Field_Bot(object sender, EventArgs e)
        {
            if (player_move)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (sender == field_bot[i, j])
                        {
                            if (field_bot[i, j].BackgroundImage == Ship_bot_image ||
                                field_bot[i, j].BackgroundImage == Ship_there_image)
                            {
                                field_bot[i, j].BackgroundImage = Ship_hit_image;
                                score_player++;
                                if (Proverka_II(i,j) == false)
                                {
                                    Plac_Point(i, j);
                                }
                                if (score_player == 20)
                                {
                                    label2.Visible = true;
                                    label2.Text = "Вы победили!";

                                    visible_label();
                                }
                            }
                            if (field_bot[i, j].BackgroundImage == Sqare_image ||
                                field_bot[i, j].BackgroundImage == Ship_miss_player_image)
                            {
                                field_bot[i, j].BackgroundImage = Ship_miss_image;
                                player_move = false;
                                timer1.Start();
                            }
                        }
                    }
                }
            }
        } // по полю бота
       
        private void Click_Field_Player(Sqare sender)
        {
            if (player_move == false && score_bot < 20)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (field_player[i, j].BackgroundImage == Ship_hit_last_image)
                        {
                            field_player[i, j].BackgroundImage = Ship_hit_image;
                        }
                        if (field_player[i, j].BackgroundImage == Ship_miss_last_image)
                        {
                            field_player[i, j].BackgroundImage = Ship_miss_image;
                        }
                        if (sender == field_player[i, j])
                        {
                            if (field_player[i, j].BackgroundImage == Ship_image)
                            {
                                field_player[i, j].BackgroundImage = Ship_hit_last_image;
                                score_bot++;
                                if (score_bot == 20)
                                {
                                    label2.Visible = true;
                                    label2.Text = "Вы проиграли!";
                                    visible_label();
                                }
                                if (Proverka_II_player_field(i, j) == false)
                                {
                                    Plac_Point_field_player(i, j);
                                    Bot_Logik();
                                    break;
                                }
                                timer1.Start();
                            }
                            else if (field_player[i, j].BackgroundImage == Sqare_image)
                            { 
                                field_player[i, j].BackgroundImage = Ship_miss_last_image;
                                player_move = true;
                            }
                            else
                            {
                                timer1.Start();
                            }
                        }
                    }
                }
            }
        }//по полю игрока

        //Расстановка точек вокруг потопленных кораблей
        private bool Plac_Point(int y_prov, int x_prov)
        {     
            bool ship_bool = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (i == 0)
                    {
                        if (y_prov + j < 10)
                        {
                            if (field_bot[y_prov + j, x_prov].BackgroundImage == Ship_hit_image ||
                                field_bot[y_prov + j, x_prov].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov + j + k < 10 && y_prov + j + k >= 0 && x_prov + ik < 10 && x_prov + ik >= 0)
                                        {
                                            if (field_bot[y_prov + j + k, x_prov + ik].BackgroundImage == Sqare_image)
                                            {
                                                field_bot[y_prov + j + k, x_prov + ik].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    if (i == 1)
                    {
                        if (y_prov - j >= 0)
                        {
                            if (field_bot[y_prov - j, x_prov].BackgroundImage == Ship_hit_image ||
                                field_bot[y_prov - j, x_prov].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov - j + k < 10 && y_prov - j + k >= 0 && x_prov + ik < 10 && x_prov + ik >= 0)
                                        {
                                            if (field_bot[y_prov - j + k, x_prov + ik].BackgroundImage == Sqare_image)
                                            {
                                                field_bot[y_prov - j + k, x_prov + ik].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    if (i == 2)
                    {
                        if (x_prov + j < 10)
                        {
                            if (field_bot[y_prov, x_prov + j].BackgroundImage == Ship_hit_image ||
                                field_bot[y_prov, x_prov + j].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov + k < 10 && y_prov + k >= 0 && x_prov + ik + j < 10 && x_prov + ik + j >= 0)
                                        {
                                            if (field_bot[y_prov + k, x_prov + ik + j].BackgroundImage == Sqare_image)
                                            {
                                                field_bot[y_prov + k, x_prov + ik + j].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    if (i == 3)
                    {
                        if (x_prov - j >= 0)
                        {
                            if (field_bot[y_prov, x_prov - j].BackgroundImage == Ship_hit_image ||
                                field_bot[y_prov, x_prov - j].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov + k < 10 && y_prov + k >= 0 && x_prov + ik - j < 10 && x_prov + ik - j >= 0)
                                        {
                                            if (field_bot[y_prov + k, x_prov + ik - j].BackgroundImage == Sqare_image)
                                            {
                                                field_bot[y_prov + k, x_prov + ik - j].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }


                }
            }
            return ship_bool;
        }
        private bool Plac_Point_field_player(int y_prov, int x_prov)
        {
            bool ship_bool = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (i == 0)
                    {
                        if (y_prov + j < 10)
                        {
                            if (field_player[y_prov + j, x_prov].BackgroundImage == Ship_hit_image ||
                                field_player[y_prov + j, x_prov].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov + j + k < 10 && y_prov + j + k >= 0 && x_prov + ik < 10 && x_prov + ik >= 0)
                                        {
                                            if (field_player[y_prov + j + k, x_prov + ik].BackgroundImage == Sqare_image)
                                            {
                                                field_player[y_prov + j + k, x_prov + ik].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    if (i == 1)
                    {
                        if (y_prov - j >= 0)
                        {
                            if (field_player[y_prov - j, x_prov].BackgroundImage == Ship_hit_image ||
                                field_player[y_prov - j, x_prov].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov - j + k < 10 && y_prov - j + k >= 0 && x_prov + ik < 10 && x_prov + ik >= 0)
                                        {
                                            if (field_player[y_prov - j + k, x_prov + ik].BackgroundImage == Sqare_image)
                                            {
                                                field_player[y_prov - j + k, x_prov + ik].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    if (i == 2)
                    {
                        if (x_prov + j < 10)
                        {
                            if (field_player[y_prov, x_prov + j].BackgroundImage == Ship_hit_image ||
                                field_player[y_prov, x_prov + j].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov + k < 10 && y_prov + k >= 0 && x_prov + ik + j < 10 && x_prov + ik + j >= 0)
                                        {
                                            if (field_player[y_prov + k, x_prov + ik + j].BackgroundImage == Sqare_image)
                                            {
                                                field_player[y_prov + k, x_prov + ik + j].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    if (i == 3)
                    {
                        if (x_prov - j >= 0)
                        {
                            if (field_player[y_prov, x_prov - j].BackgroundImage == Ship_hit_image ||
                                field_player[y_prov, x_prov - j].BackgroundImage == Ship_hit_last_image)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int ik = -1; ik <= 1; ik++)
                                    {
                                        if (y_prov + k < 10 && y_prov + k >= 0 && x_prov + ik + j < 10 && x_prov + ik - j >= 0)
                                        {
                                            if (field_player[y_prov + k, x_prov + ik - j].BackgroundImage == Sqare_image)
                                            {
                                                field_player[y_prov + k, x_prov + ik - j].BackgroundImage = Ship_miss_image;
                                            }
                                        }
                                    }
                                }
                            }
                            else break;
                        }
                    }


                }
            }
            return ship_bool;
        }

        //действие бота в зависимости от выбранной сложности
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop(); 
            if (difficulty == 0)
            {
                Bot_Logik();
            }
        }
     

        //наведение стрелки мыши на клетку
        private void Mouse_Move_Field_bot(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (sender == field_bot[i, j])
                    {
                        field_bot[i, j].Focus();
                    }
                }
            }
        }
        //нажатие на клавиши
        private void Field_bot_key_up(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (field_bot[i, j].BackgroundImage == Ship_bot_image)
                        {
                            field_bot[i, j].BackgroundImage = Ship_there_image;
                        }
                    }
                }
            }
        }

        //Вызов/закрытие "Info"
        private void button2_Click(object sender, EventArgs e)
        {
            label3.Visible = !label3.Visible;
        }

        // Проверка утоплен ли корабль
        private bool Proverka_II(int y_prov, int x_prov)
        {
            bool ship_bool = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    if (i == 0)
                    {
                        if (y_prov + j < 10)
                        {
                            if (field_bot[y_prov + j, x_prov].BackgroundImage == Ship_bot_image ||
                                field_bot[y_prov + j, x_prov].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_bot[y_prov + j, x_prov].BackgroundImage == Sqare_image ||
                                field_bot[y_prov + j, x_prov].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_bot[y_prov + j, x_prov].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }
                    if (i == 1)
                    {
                        if (y_prov - j >= 0)
                        {
                            if (field_bot[y_prov - j, x_prov].BackgroundImage == Ship_bot_image ||
                                field_bot[y_prov - j, x_prov].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_bot[y_prov-j , x_prov].BackgroundImage == Sqare_image ||
                                field_bot[y_prov -j, x_prov].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_bot[y_prov - j, x_prov].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }
                    if (i == 2)
                    {
                        if (x_prov + j < 10)
                        {
                            if (field_bot[y_prov, x_prov + j].BackgroundImage == Ship_bot_image ||
                                field_bot[y_prov, x_prov + j].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_bot[y_prov, x_prov + j].BackgroundImage == Sqare_image ||
                                field_bot[y_prov , x_prov + j].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_bot[y_prov, x_prov + j].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }
                    if (i == 3)
                    {
                        if (x_prov - j >= 0)
                        {
                            if (field_bot[y_prov, x_prov - j].BackgroundImage == Ship_bot_image ||
                                field_bot[y_prov, x_prov - j].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_bot[y_prov, x_prov - j].BackgroundImage == Sqare_image ||
                                field_bot[y_prov, x_prov - j].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_bot[y_prov, x_prov - j].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }

                }
            }
            return ship_bool;
        }
        private bool Proverka_II_player_field(int y_prov, int x_prov)
        {
            bool ship_bool = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    if (ship_bool)
                    {
                        ship_bool = true;
                        break;
                    }
                    if (i == 0)
                    {
                        if (y_prov + j < 10)
                        {
                            if (field_player[y_prov + j, x_prov].BackgroundImage == Ship_image ||
                                field_player[y_prov + j, x_prov].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_player[y_prov + j, x_prov].BackgroundImage == Sqare_image ||
                                field_player[y_prov + j, x_prov].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_player[y_prov + j, x_prov].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }
                    if (i == 1)
                    {
                        if (y_prov - j >= 0)
                        {
                            if (field_player[y_prov - j, x_prov].BackgroundImage == Ship_image ||
                                field_player[y_prov - j, x_prov].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_player[y_prov - j, x_prov].BackgroundImage == Sqare_image ||
                                field_player[y_prov - j, x_prov].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_player[y_prov - j, x_prov].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }
                    if (i == 2)
                    {
                        if (x_prov + j < 10)
                        {
                            if (field_player[y_prov, x_prov + j].BackgroundImage == Ship_image ||
                                field_player[y_prov, x_prov + j].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_bot[y_prov, x_prov + j].BackgroundImage == Sqare_image ||
                                field_player[y_prov, x_prov + j].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_player[y_prov, x_prov + j].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }
                    if (i == 3)
                    {
                        if (x_prov - j >= 0)
                        {
                            if (field_player[y_prov, x_prov - j].BackgroundImage == Ship_image ||
                                field_player[y_prov, x_prov - j].BackgroundImage == Ship_there_image)
                            {
                                ship_bool = true;
                            }
                            else if (field_bot[y_prov, x_prov - j].BackgroundImage == Sqare_image ||
                                field_player[y_prov, x_prov - j].BackgroundImage == Ship_miss_player_image)
                            {
                                break;
                            }
                            else if (field_player[y_prov, x_prov - j].BackgroundImage == Ship_hit_image)
                            {
                                continue;
                            }
                        }
                    }

                }
            }
            return ship_bool;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
        private void visible_label()
        {
            Form1 form = new Form1();
            label2.Visible = true;
            label2.Location = new Point(form.Width / 2, form.Height / 2 - label2.Height);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    field_bot[i, j].Visible = !field_bot[i, j].Visible;
                    field_player[i, j].Visible = !field_player[i, j].Visible;
                }
            }
        }


        //Бот "Easy"
        private void Bot_Logik()
        {
            int random_X = rnd.Next(0, 10);
            int random_Y = rnd.Next(0, 10);
            while (field_player[random_Y, random_X].BackgroundImage == Ship_miss_image ||
                field_player[random_Y, random_X].BackgroundImage == Ship_hit_image)
            {
                random_X = rnd.Next(0, 10);
                random_Y = rnd.Next(0, 10);
            }
            Click_Field_Player(field_player[random_Y, random_X]);
        }
    }
}
