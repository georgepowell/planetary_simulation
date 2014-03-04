using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Planetary_Simulation
{
  public sealed partial class MainPage : Page
  {
    const int NUMBER_OF_BALLS = 10;
    const int FPS = 20;
    const double G = 100.0;
    const double FRICTION = 0.9;
    const double MAX_MASS = 100;
    const double MAX_ACC = 30;

    double[] masses = new double[NUMBER_OF_BALLS];
    double[,] positions = new double[NUMBER_OF_BALLS, 2];
    double[,] velocities = new double[NUMBER_OF_BALLS, 2];

    Rectangle[] particles = new Rectangle[NUMBER_OF_BALLS];
    DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1.0 / FPS) };

    public MainPage()
    {
      this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      InitialiseParticles();
      RedrawParticles();
      timer.Tick += timer_Tick;
      timer.Start();
    }

    void timer_Tick(object sender, object e)
    {
      UpdateParticlePositions();
      UpdateParticleVelocities();
      RedrawParticles();
    }

    private void UpdateParticleVelocities()
    {
      for (int i = 0; i < NUMBER_OF_BALLS; i++)
      {
        double[] force = new double[2];
        for (int j = 0; j < NUMBER_OF_BALLS; j++)
        {
          if (i == j)
            continue;

          double dx = positions[j, 0] - positions[i, 0];
          double dy = positions[j, 1] - positions[i, 1];

          double distanceSquared = dx * dx + dy * dy;
          double distance = Math.Sqrt(distanceSquared);
          double magnitude = G * masses[i] * masses[j] / distanceSquared;

          force[0] += magnitude * dx / distance;
          force[1] += magnitude * dy / distance;

        }
        double accX = force[0] / masses[i];
        double accY = force[1] / masses[i];
        if (accX > MAX_ACC)
          accX = MAX_ACC;
        else if (accX < -MAX_ACC)
          accX = -MAX_ACC;
        if (accY > MAX_ACC)
          accY = MAX_ACC;
        else if (accY < -MAX_ACC)
          accY = -MAX_ACC;

        velocities[i, 0] += accX;
        velocities[i, 1] += accY;
        velocities[i, 0] *= FRICTION;
        velocities[i, 1] *= FRICTION;

      }
    }

    private void UpdateParticlePositions()
    {
      for (int i = 0; i < NUMBER_OF_BALLS; i++)
      {
        positions[i, 0] += velocities[i, 0];
        positions[i, 1] += velocities[i, 1];
      }
    }

    private void RedrawParticles()
    {
      for (int i = 0; i < NUMBER_OF_BALLS; i++)
      {
        particles[i].SetValue(Canvas.LeftProperty, positions[i, 0]);
        particles[i].SetValue(Canvas.TopProperty, positions[i, 1]);
      }
    }

    void InitialiseParticles()
    {
      Random rand = new Random();
      for (int i = 0; i < NUMBER_OF_BALLS; i++)
      {
        velocities[i, 0] = velocities[i, 1] = 0.0;

        masses[i] = 1 + (MAX_MASS - 1) * rand.NextDouble();

        positions[i, 0] = rand.NextDouble() * 500;
        positions[i, 1] = rand.NextDouble() * 500;

        double size = 10 * Math.Sqrt(masses[i]);
        particles[i] = new Rectangle()
        {
          Width = size,
          Height = size,
          RadiusX = size / 2,
          RadiusY = size / 2,
          Fill = new SolidColorBrush(Colors.White),
          Margin = new Thickness(-size / 2, -size / 2, 0, 0)
        };

        particles[i].SetValue(Canvas.LeftProperty, positions[i, 0]);
        particles[i].SetValue(Canvas.TopProperty, positions[i, 1]);

        root.Children.Add(particles[i]);
      }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      root.Children.Clear();
      InitialiseParticles();
    }
  }
}
