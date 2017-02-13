namespace Tomataboard.Models
{
    public class SettingsDto
    {
        public string DefaultQuestion { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Location { get; set; }
        public bool ShowBackgroundPhoto { get; set; }
        public bool ShowFocus { get; set; }
        public bool ShowWeather { get; set; }
        public string TemperatureUnits { get; set; }
        public bool ShowQuote { get; set; }
        public bool ShowTodo { get; set; }
        public bool ShowTimers { get; set; }
        public string ClockFormat { get; set; }
        public string ActiveTimer { get; set; }
        public bool VolumeOn { get; set; }

        public int TimerMinutesSelection { get; set; }
        public int timerSecondsSelection { get; set; }

        public int PomodoroTime { get; set; }
        public int PomodoroShortBreak { get; set; }
        public int PomodoroLongBreak { get; set; }
        public string PomodoroTaskPlaceholder { get; set; }
        public string PomodoroTaskDescription { get; set; }
        public int PomodoroTotal { get; set; }

        public CountDownDto CountDown { get; set;}
        public GreetingDto Greeting { get; set; }
        public TodoDto Todo { get; set; }

        public bool CanSync { get; set; }
    }
}