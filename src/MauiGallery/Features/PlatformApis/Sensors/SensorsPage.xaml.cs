using System.ComponentModel;
using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class SensorsPage : SamplePage
{
    private readonly SensorViewModel[] _sensors;

    public SensorsPage(
        SamplePageServices services,
        AccelerometerViewModel accelerometer,
        GyroscopeViewModel gyroscope,
        MagnetometerViewModel magnetometer,
        CompassViewModel compass,
        BarometerViewModel barometer,
        OrientationViewModel orientation)
        : base(services)
    {
        InitializeComponent();
        _sensors = [accelerometer, gyroscope, magnetometer, compass, barometer, orientation];

        Bind(accelerometer, AccelerometerExample, AccelerometerSample, AccelerometerOptions, AccelerometerToggle);
        Bind(gyroscope, GyroscopeExample, GyroscopeSample, GyroscopeOptions, GyroscopeToggle);
        Bind(magnetometer, MagnetometerExample, MagnetometerSample, MagnetometerOptions, MagnetometerToggle);
        Bind(compass, CompassExample, CompassSample, CompassOptions, CompassToggle);
        Bind(barometer, BarometerExample, BarometerSample, BarometerOptions, BarometerToggle);
        Bind(orientation, OrientationExample, OrientationSample, OrientationOptions, OrientationToggle);

        LevelView.Drawable = new BubbleLevelDrawable(accelerometer);
        GyroscopeView.Drawable = new AxisBarsDrawable(() => gyroscope.AngularVelocity, range: 5);
        MagnetometerView.Drawable = new AxisBarsDrawable(() => magnetometer.MagneticField, range: 100);
        CompassView.Drawable = new CompassDrawable(compass);
        BarometerView.Drawable = new BarometerDrawable(barometer);

        accelerometer.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AccelerometerViewModel.Acceleration))
            {
                LevelView.Invalidate();
            }
            else if (e.PropertyName == nameof(AccelerometerViewModel.ShakeCount))
            {
                AccelerometerExample.Output = $"ShakeDetected ({accelerometer.ShakeCount})";
            }
        };
        Redraw(gyroscope, nameof(GyroscopeViewModel.AngularVelocity), GyroscopeView);
        Redraw(magnetometer, nameof(MagnetometerViewModel.MagneticField), MagnetometerView);
        Redraw(compass, nameof(CompassViewModel.Heading), CompassView);
        Redraw(barometer, nameof(BarometerViewModel.Pressure), BarometerView);
        compass.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CompassViewModel.Heading))
            {
                SemanticProperties.SetDescription(CompassView, $"Heading {compass.Reading}");
            }
        };
        barometer.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(BarometerViewModel.Pressure))
            {
                SemanticProperties.SetDescription(BarometerView, $"Air pressure {barometer.Reading}");
            }
        };
    }

    // Every running sensor costs battery: stop them all when the page goes away.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        foreach (var sensor in _sensors)
        {
            sensor.Stop();
        }
    }

    private static void Bind(SensorViewModel sensor, ControlExample example, View sample, View options, View toggle)
    {
        sample.BindingContext = sensor;
        options.BindingContext = sensor;
        example.NativeTarget = toggle;
        example.Output = sensor.IsSupported ? "IsSupported: true" : "IsSupported: false";
        example.Notice = sensor.Message;
        sensor.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SensorViewModel.IsMonitoring))
            {
                example.Output = sensor.IsMonitoring ? $"Started at SensorSpeed.{(SensorSpeed)sensor.SpeedIndex}" : "Stopped";
            }
            else if (e.PropertyName == nameof(SensorViewModel.Message))
            {
                example.Notice = sensor.Message;
            }
        };
    }

    private static void Redraw(INotifyPropertyChanged source, string propertyName, GraphicsView view) =>
        source.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == propertyName)
            {
                view.Invalidate();
            }
        };
}
