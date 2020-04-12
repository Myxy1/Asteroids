﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace Asteroids
{
    //TESZT
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String merreFordul = "";

        IFirebaseConfig firebaseConfig = new FirebaseConfig
        {
            AuthSecret = "exgxivog1ZOUP0dsczcfx6k5tjhxlpj8qX7SM4PQ",
            BasePath = "https://asteroids-1fadb.firebaseio.com/"
        };

        IFirebaseClient client;

        public MainWindow()
        {
            client = new FireSharp.FirebaseClient(firebaseConfig);
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += Animation;
            
            
        }

        DispatcherTimer timer = new DispatcherTimer();

        List<Asteroid> asteroids = new List<Asteroid>();
        
        SpaceShip spaceShip;

        List<LaserShooting> laserShootings = new List<LaserShooting>();

        List<GameObject> gameObject = new List<GameObject>();

        
        void Animation(object sender, EventArgs e)
        {
            List<LaserShooting> laserShootingsOverTheEdge = new List<LaserShooting>();
            foreach (var s in gameObject)
            {
                bool overTheEdge = s.Animation(timer.Interval, drawingArea);
                if (s is LaserShooting && overTheEdge)
                {
                    laserShootingsOverTheEdge.Add((LaserShooting)s);
                }
            }
            foreach (var laser in laserShootingsOverTheEdge)
            {
                laserShootings.Remove(laser);
                gameObject.Remove(laser);
            }
            //gameObject.ForEach(s => s.Animation(timer.Interval, drawingArea));

            List<Asteroid> deletedAsteroids = new List<Asteroid>();
            List<LaserShooting> deletedLasers = new List<LaserShooting>();
            foreach (Asteroid asteroid in asteroids)
            {
                foreach (LaserShooting laser in laserShootings)
                {
                    if (asteroid.ContainsPoint(laser.X, laser.Y))
                    {
                        deletedAsteroids.Add(asteroid);
                        deletedLasers.Add(laser);
                    }
                }
            }
            asteroids = asteroids.Except(deletedAsteroids).ToList();
            gameObject = gameObject.Except(deletedAsteroids).ToList();
            foreach (var laser in deletedLasers)
            {
                laserShootings.Remove(laser);
                gameObject.Remove(laser);
            }

            drawingArea.Children.Clear();

            foreach (var s in gameObject)
            {
                s.Draw(drawingArea);
            }

            bool defeated = false;

            foreach (var asteroid in asteroids)
            {
                if(asteroid.ContainsPoint(spaceShip.X, spaceShip.Y))
                {
                    defeated = true;
                    break;
                }
            }
            if (defeated)
            {
                MessageBox.Show("Vesztettél!");
                asteroids.Clear();
                laserShootings.Clear();
                gameObject.Clear();
                gameObject.Add(spaceShip);

                for (int i = 0; i < 10; i++)
                {
                    asteroids.Add(new Asteroid(drawingArea));
                    gameObject.Add(asteroids.Last());
                }
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;

            for (int i = 0; i < 10; i++)
            {
                asteroids.Add(new Asteroid(drawingArea));
                gameObject.Add(asteroids.Last());
            }

            spaceShip = new SpaceShip(drawingArea);
            gameObject.Add(spaceShip);

            timer.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (timer.IsEnabled)
            {
                switch (e.Key)
                {
                    case Key.Left:
                    case Key.A:
                        spaceShip.TurnLeft(true);
                        break;
                    case Key.Right:
                    case Key.D:
                        spaceShip.TurnLeft(false);
                        break;
                    case Key.Up:
                    case Key.W:
                        spaceShip.Accelerate(true);
                        break;
                    case Key.Down:
                    case Key.S:
                        spaceShip.Accelerate(false);
                        break;
                    case Key.Space:
                        laserShootings.Add(new LaserShooting(spaceShip));
                        gameObject.Add(laserShootings.Last());
                        break;
                }
            }
        }




        //private async void button_Click(object sender, RoutedEventArgs e)
        //{
        //    var data = new Data
        //    {
        //        Id = "1",
        //        Name = "Miki",
        //        Score = 1
        //    };

        //    SetResponse response = await client.SetAsync("Felhasználók/" + "1", data);
        //    Data result = response.ResultAs<Data>();

        //}
    }
}
