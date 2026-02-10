using System.Windows;
using System.Windows.Input;

namespace simulator
{
    public static class PhysicsProperties
    {
        // Register attached Mass property
        public static readonly DependencyProperty MassProperty =
            DependencyProperty.RegisterAttached(
                "Mass",
                typeof(double),
                typeof(PhysicsProperties),
                new PropertyMetadata(1.0)); // Default mass = 1.0

        public static double GetMass(DependencyObject obj) =>
            (double)obj.GetValue(MassProperty);

        public static void SetMass(DependencyObject obj, double value) =>
            obj.SetValue(MassProperty, value);


        // Register attached Velocity property
        public static readonly DependencyProperty VelocityProperty =
            DependencyProperty.RegisterAttached(
                "Velocity",
                typeof(List<double>),
                typeof(PhysicsProperties),
                new PropertyMetadata(new List<double> { 0.0, 0.0})); // Default velocity = (0.0 , 0.0)

        public static List<double> GetVelocity(DependencyObject obj) =>
            (List<double>)obj.GetValue(VelocityProperty);

        public static void SetVelocity(DependencyObject obj, List<double> value) =>
            obj.SetValue(VelocityProperty, value);

    }
}
