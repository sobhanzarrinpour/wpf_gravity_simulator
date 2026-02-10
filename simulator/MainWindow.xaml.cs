using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //Ellipse el1 = new Ellipse();

            //el1.Height = 50;
            //el1.Width = 50;
            //el1.Fill = new SolidColorBrush(Colors.Red);
            //el1.Stroke = new SolidColorBrush(Colors.Black);
            //el1.StrokeThickness = 4;
            //Canvas.SetTop(el1, 0);
            //Canvas.SetLeft(el1, 0);
            //Canvas.SetZIndex(el1, 1);

            //playground.Children.Add(el1);

            Thread.Sleep(10);

            //update_ellipse_position(playground, el1);
            update_ellipse_position(playground);
        }

        [MTAThread]
        private void update_ellipse_position(Canvas c, Ellipse e)
        {
            //ThreadLocal<Canvas> threadLocalCanvas = new ThreadLocal<Canvas>(() => c);
            //ThreadLocal<Ellipse> threadLocalEllipse = new ThreadLocal<Ellipse>(() => e);

            //Thread th1 = new Thread(() =>
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        Canvas.SetTop(threadLocalEllipse.Value, i);
            //        Canvas.SetLeft(threadLocalEllipse.Value, i);
            //        threadLocalCanvas.Value.UpdateLayout();
            //        Thread.Sleep(100);
            //    }
            //});
            //th1.Start();

            List<int> t = new List<int>();

            double left = Canvas.GetLeft(e);
            double top = Canvas.GetTop(e);

            for (int i=0; i< 200; i++)
            {
                e.SetValue(Canvas.LeftProperty, left + i);
                e.SetValue(Canvas.TopProperty, top + i);
                c.UpdateLayout();
                Thread.Sleep(5);
            }

            

            
            Parallel.For(0, 50, i =>
            {
                //Canvas.SetTop(e, i);
                //Canvas.SetLeft(e, i);
                //c.UpdateLayout();
                //Thread.Sleep(10);


                t.Add(i);
            });

            string ss = "";
            foreach (int s in t)
            {
                ss += s.ToString()+",";
            }

            my_label.Content = ss;

        }


        private void update_ellipse_position(Canvas c)
        {
            for (int i = 0; i < 200; i++)
            {
                c.Children.Clear();

                Ellipse el1 = new Ellipse();

                el1.Height = 50;
                el1.Width = 50;
                el1.Fill = new SolidColorBrush(Colors.Red);
                el1.Stroke = new SolidColorBrush(Colors.Black);
                el1.StrokeThickness = 4;
                Canvas.SetTop(el1, i);
                Canvas.SetLeft(el1, i);
                Canvas.SetZIndex(el1, 1);

                c.Children.Add(el1);
                c.UpdateLayout();
                Thread.Sleep(10);
            }

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            start_btn.IsEnabled = !enable_start.IsChecked.Value;
        }
        */


        private List<Ellipse> _ellipse;
        private int number_of_planets = 150;
        private double G = 6.67 * 1e-1; // 6.67 * 1e-11;
        private double dt = 0.1;
        private List<int> space_width_height = new List<int>() { 1200, 600 };
        private List<int> min_max_mass_range = new List<int>() { 10, 100000 };
        private double min_distance_allowed = 20.0f;
        private TextBlock txt1, txt2;

        public MainWindow()
        {
            WindowState = WindowState.Maximized;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CreateEllipse();
        }

        private void CreateEllipse()
        {

            txt1 = new TextBlock();
            txt1.FontSize = 14;
            txt1.Text = "";
            txt1.Background = new SolidColorBrush(Colors.Blue);
            txt1.Foreground = Brushes.White;
            txt1.Padding = new Thickness(15);
            Canvas.SetTop(txt1, 0);
            Canvas.SetLeft(txt1, 0);
            Canvas.SetZIndex(txt1, number_of_planets + 1);
            playground.Children.Add(txt1);

            txt2 = new TextBlock();
            txt2.FontSize = 14;
            txt2.Text = "";
            txt2.Padding = new Thickness(15, 10, 15, 10);
            txt2.Opacity = 0;
            txt2.Background = new SolidColorBrush(Colors.Blue);
            txt2.Foreground = Brushes.White;
            Canvas.SetTop(txt2, 0);
            Canvas.SetLeft(txt2, 0);
            Canvas.SetZIndex(txt2, number_of_planets + 2);
            playground.Children.Add(txt2);

            Random r = new Random();
            Brush defaultFill = new SolidColorBrush(Colors.DodgerBlue);

            _ellipse = new List<Ellipse>();
            _ellipse.Clear();

            for (int i = 0; i < number_of_planets; i++) {
                int radius = r.Next(10) + 5;
                Ellipse ellipse = new Ellipse
                {
                    Width = radius,
                    Height = radius,
                    Fill = defaultFill,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    SnapsToDevicePixels = true,      // Sharper rendering
                    UseLayoutRounding = true,         // Prevent blurry edges
                };

                // Attach custom property
                PhysicsProperties.SetMass(ellipse, (r.Next(min_max_mass_range[1]) - r.Next(min_max_mass_range[0])));
                PhysicsProperties.SetVelocity(ellipse, new List<double>() { (r.Next(10) - r.Next(10)) / 5, (r.Next(10) - r.Next(10)) / 5 });

                if (i == 0)
                {
                    //ellipse.Loaded += (s, e) => txt1.Text = PhysicsProperties.GetMass(ellipse).ToString() + " --- (" + PhysicsProperties.GetVelocity(ellipse)[0].ToString() + " , " + PhysicsProperties.GetVelocity(ellipse)[1].ToString() + " )";
                    ellipse.LayoutUpdated += (s, e) => txt1.Text = "(" + Canvas.GetLeft(ellipse).ToString() + " , " + Canvas.GetTop(ellipse).ToString() + " )";
                    PhysicsProperties.SetMass(ellipse, 1000000);
                }

                // Store original brush for later restoration
                ellipse.Tag = defaultFill;

                ellipse.MouseEnter += (s, e) => {
                    ellipse.Fill = Brushes.Red;
                    txt2.Text = PhysicsProperties.GetMass(ellipse).ToString() + " --- (" + PhysicsProperties.GetVelocity(ellipse)[0].ToString() + " , " + PhysicsProperties.GetVelocity(ellipse)[1].ToString() + " )";

                    Canvas.SetTop(txt2, Mouse.GetPosition(playground).Y - 30);
                    Canvas.SetLeft(txt2, Mouse.GetPosition(playground).X - 30);

                    txt2.Opacity = 1;
                };
                ellipse.MouseLeave += (s, e) => { ellipse.Fill = (Brush)ellipse.Tag; txt2.Opacity = 0; };

                Canvas.SetLeft(ellipse, r.Next(space_width_height[0]));
                Canvas.SetTop(ellipse, r.Next(space_width_height[1]));
                Canvas.SetZIndex(ellipse, i + 1);

                playground.Children.Add(ellipse);

                _ellipse.Add(ellipse);
            }
        }

        private void AnimateEllipse(double targetX, double targetY, double durationSeconds)
        {
            var duration = TimeSpan.FromSeconds(durationSeconds);

            var leftAnim = new DoubleAnimation
            {
                To = targetX,
                Duration = duration,
                //EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut } // Natural motion
                //EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseInOut } // Natural motion
            };

            var topAnim = new DoubleAnimation
            {
                To = targetY,
                Duration = duration,
                //EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(leftAnim);
            storyboard.Children.Add(topAnim);

            foreach (var ell in _ellipse)
            {
                Storyboard.SetTarget(leftAnim, ell);
                Storyboard.SetTargetProperty(leftAnim, new PropertyPath("(Canvas.Left)"));

                Storyboard.SetTarget(topAnim, ell);
                Storyboard.SetTargetProperty(topAnim, new PropertyPath("(Canvas.Top)"));
            }

            // Optional: Enable FPS monitoring during dev
            // Timeline.DesiredFrameRate = 60;

            storyboard.Begin();
            //storyboard.Freeze();
        }


        /*
         🔁 For Continuous/Interactive Movement (e.g., Mouse Tracking)
            If you need dynamic path changes (e.g., following mouse), use CompositionTarget.Rendering — but only when necessary:
         */
        private void StartInteractiveMovement()
        {
            CompositionTarget.Rendering += OnRendering;
        }

        //private void OnRendering(object sender, EventArgs e)
        //{
        //    // Example: Move toward mouse position
        //    var pos = Mouse.GetPosition(playground);
        //    double currentX = Canvas.GetLeft(_ellipse);
        //    double currentY = Canvas.GetTop(_ellipse);

        //    // Smooth interpolation (avoid snapping)
        //    Canvas.SetLeft(_ellipse, currentX + (pos.X - currentX) * 0.1);
        //    Canvas.SetTop(_ellipse, currentY + (pos.Y - currentY) * 0.1);
        //}

        private async void OnRendering(object sender, EventArgs e)
        {
            try
            {
                Random r = new Random();

                List<List<double>> p = await positionCalc();

                int i = 0;
                foreach (Ellipse ell in _ellipse)
                {
                    // Example: Move toward mouse position
                    double currentX = Canvas.GetLeft(ell);
                    double currentY = Canvas.GetTop(ell);

                    //// Smooth interpolation (avoid snapping)
                    //Canvas.SetLeft(ell, currentX + (r.Next(5) - r.Next(5)));
                    //Canvas.SetTop(ell, currentY + (r.Next(5) - r.Next(5)));

                    Canvas.SetLeft(ell, Math.Round(p[i][0], 1));
                    Canvas.SetTop(ell, Math.Round(p[i][1], 1));

                    i++;
                }
            } catch (Exception err)
            {
                my_label.Content = err.ToString();
            }
        }

        private Task<List<List<double>>> positionCalc()
        {
            List<List<double>> positions = new List<List<double>>();

            foreach (Ellipse ell in _ellipse)
            {
                // Example: Move toward mouse position
                double currentX = Canvas.GetLeft(ell);
                double currentY = Canvas.GetTop(ell);
                double mass1 = PhysicsProperties.GetMass(ell);
                List<double> Fv = new List<double>() { 0, 0 };
                List<double> vel = PhysicsProperties.GetVelocity(ell);

                foreach (Ellipse _ell in _ellipse)
                {
                    if (ell != _ell)
                    {
                        double _currentX = Canvas.GetLeft(_ell);
                        double _currentY = Canvas.GetTop(_ell);
                        double mass2 = PhysicsProperties.GetMass(_ell);

                        double _r = Math.Sqrt(Math.Pow((currentX - _currentX), 2) + Math.Pow((currentY - _currentY), 2));
                        if (_r < 0.01)
                        {
                            _r = 0.01;
                        }

                        List<double> vec = new List<double>() { (currentX - _currentX) / _r, (currentY - _currentY) / _r };

                        double F;
                        if (_r > min_distance_allowed)
                            F = -G * mass1 * mass2 / Math.Pow(_r, 3);
                        else
                            F = 0;

                        Fv[0] += F * vec[0];
                        Fv[1] += F * vec[1];
                    }
                }

                List<double> a = new List<double>() { Fv[0] / mass1, Fv[1] / mass1 };
                List<double> dv = new List<double>() { 0.5 * a[0] * dt + vel[0], 0.5 * a[1] * dt + vel[1] };

                PhysicsProperties.SetVelocity(ell, dv);
                positions.Add(new List<double>() { currentX + dv[0] * dt, currentY + dv[1] * dt });
            }


            return Task.FromResult(positions);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            start_btn.IsEnabled = !enable_start.IsChecked.Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            my_label.Content = "process starting...";

            playground.Background = Brushes.LightSteelBlue;

            //// Add a "Hello World!" text element to the Canvas
            //TextBlock txt1 = new TextBlock();
            //txt1.FontSize = 14;
            //txt1.Text = "Hello World!";
            //Canvas.SetTop(txt1, 100);
            //Canvas.SetLeft(txt1, 10);
            //playground.Children.Add(txt1);

            //// Add a second text element to show how absolute positioning works in a Canvas
            //TextBlock txt2 = new TextBlock();
            //txt2.FontSize = 22;
            //txt2.Text = "Isn't absolute positioning handy?";
            //Canvas.SetTop(txt2, 200);
            //Canvas.SetLeft(txt2, 75);
            //playground.Children.Add(txt2);


            //AnimateEllipse(300, 200, durationSeconds: 3); // Move to (300,400) over 3 seconds

            StartInteractiveMovement();
        }

        private void StopInteractiveMovement()
        {
            CompositionTarget.Rendering -= OnRendering;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopInteractiveMovement();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            //StopInteractiveMovement();
            playground.Children.Clear();
            CreateEllipse();
        }
    }
}

