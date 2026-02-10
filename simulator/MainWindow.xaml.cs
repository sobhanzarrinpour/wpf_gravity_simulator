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
    public partial class MainWindow : Window
    {
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

        private void StartInteractiveMovement()
        {
            CompositionTarget.Rendering += OnRendering;
        }

        private async void OnRendering(object sender, EventArgs e)
        {
            try
            {
                Random r = new Random();

                List<List<double>> p = await positionCalc();

                int i = 0;
                foreach (Ellipse ell in _ellipse)
                {
                    double currentX = Canvas.GetLeft(ell);
                    double currentY = Canvas.GetTop(ell);

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
            playground.Children.Clear();
            CreateEllipse();
        }
    }
}

