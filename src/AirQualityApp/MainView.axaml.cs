using Avalonia.Controls;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Threading;
using System.ComponentModel;
using System;

namespace AirQualityApp
{
    public partial class MainView : UserControl
    {
        private DispatcherTimer _disTimer = new DispatcherTimer();
        SensorAQ sensor;
        SensorVM vm;
        public MainView()
        {
            InitializeComponent();
            sensor = new SensorAQ();
            vm = new SensorVM(sensor);
            this.DataContext = vm;

            _disTimer.Interval = TimeSpan.FromSeconds(5);
            _disTimer.Tick += DispatcherTimer_Tick;
            _disTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            vm.Refresh();
        }
    }
    [ObservableObject]
    public partial class SensorVM
    {
        SensorAQ sensor;

        public string Desc { get; set; } = string.Empty;
        public SensorVM(SensorAQ sensor)
        {
            this.sensor = sensor;
        }
        public ISeries[] Series { get; set; } =
        {
        new LineSeries<double>
        {
            Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
            Fill = null
        }
        };
        public void OnStartCommand() {
            sensor.Start();
        }
        public void OnStopCommand() {
            sensor.Stop();
        }

        public void Refresh()
        {
            if (!sensor.IsDataAvailable()) return;
            var series = new List<LineSeries<int>>();
            series.Add(new LineSeries<int>
            {
                Values = sensor.GetData().Select(x => x.PM1).TakeLast(10).ToArray(),
                Fill = null
            });
            series.Add(new LineSeries<int>
            {
                Values = sensor.GetData().Select(x => x.PM25).TakeLast(10).ToArray(),
                Fill = null
            });
            series.Add(new LineSeries<int>
            {
                Values = sensor.GetData().Select(x => x.PM10).TakeLast(10).ToArray(),
                Fill = null
            });
            Series = series.ToArray();
            Desc = $"PM1: {sensor.Current.PM1}, PM2.5: {sensor.Current.PM25}, PM10: {sensor.Current.PM10}";
        }

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "PM 1, 2.5, 10",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
    }
}
