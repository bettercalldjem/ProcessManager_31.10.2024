using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ProcessScheduler
{
    public partial class MainWindow : Window
    {
        private List<Process> processes = new List<Process>();
        private DispatcherTimer timer;
        private int currentTime;
        private string currentAlgorithm;
        private Process currentProcess;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            currentTime = 0;
            currentAlgorithm = "Приоритетное Планирование";
        }

        private void AddProcess_Click(object sender, RoutedEventArgs e)
        {
            Log.Text += "Попытка добавить процесс...\n"; 

            string name = ProcessName.Text;
            if (int.TryParse(ProcessPriority.Text, out int priority) && int.TryParse(ProcessTime.Text, out int time))
            {
                Process process = new Process(name, priority, time);
                processes.Add(process);
                UpdateProcessList();

                ProcessName.Clear();
                ProcessPriority.Clear();
                ProcessTime.Clear();
            }
            else
            {
                Log.Text += "Введите корректные значения приоритета и времени.\n";
            }
        }

        private void AlgorithmChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SchedulingAlgorithm.SelectedItem is ComboBoxItem selectedItem)
            {
                currentAlgorithm = selectedItem.Content.ToString();
                Log.Text += $"Алгоритм планирования изменён на: {currentAlgorithm}\n";
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            ScheduleNextProcess();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Log.Text += $"Симуляция остановлена на времени {currentTime}\n";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentProcess != null)
            {
                currentProcess.RemainingTime--;
                Log.Text += $"[{currentTime}] Процесс {currentProcess.Name} выполняется. Осталось времени: {currentProcess.RemainingTime}\n";

                if (currentProcess.RemainingTime <= 0)
                {
                    Log.Text += $"Процесс {currentProcess.Name} завершён на времени {currentTime}\n";
                    currentProcess = null;
                }
            }

            currentTime++;
            ScheduleNextProcess();
            UpdateProcessList();
        }

        private void ScheduleNextProcess()
        {
            if (currentProcess == null && processes.Count > 0)
            {
                if (currentAlgorithm == "Приоритетное Планирование")
                {
                    currentProcess = processes.OrderByDescending(p => p.Priority).First();
                }
                else if (currentAlgorithm == "Круговой Алгоритм")
                {
                    currentProcess = processes.First();
                }
                else if (currentAlgorithm == "SJF")
                {
                    currentProcess = processes.OrderBy(p => p.RemainingTime).First();
                }

                processes.Remove(currentProcess);
            }
        }

        private void UpdateProcessList()
        {
            ProcessList.Items.Clear();
            foreach (var process in processes)
            {
                ProcessList.Items.Add($"Имя: {process.Name} | Приоритет: {process.Priority} | Осталось времени: {process.RemainingTime} | Состояние: Готов");
            }

            if (currentProcess != null)
            {
                ProcessList.Items.Add($"Имя: {currentProcess.Name} | Приоритет: {currentProcess.Priority} | Осталось времени: {currentProcess.RemainingTime} | Состояние: Выполняется");
            }
        }
    }

    public class Process
    {
        public string Name { get; } 
        public int Priority { get; }
        public int RemainingTime { get; set; }

        public Process(string name, int priority, int time)
        {
            Name = name;
            Priority = priority;
            RemainingTime = time;
        }
    }
}
