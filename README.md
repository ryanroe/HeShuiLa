# HeShuiLa (喝水啦) - Water Reminder

A simple Windows application that reminds you to drink water regularly.

## Features

- Customizable reminder intervals
- Personalized reminder messages
- System tray integration
- Fade in/out notifications
- Persistent settings
- Single instance application

## Usage

1. Run the application - it will minimize to system tray
2. Double click the tray icon or right-click and select "设置" to open settings
3. In settings you can:
   - Set reminder interval (in minutes)
   - Set reminder duration (in seconds)
   - Customize reminder messages
   - Test the reminder

## System Requirements

- Windows OS
- .NET Framework 4.7.2 or higher

## Configuration

The application stores its settings and custom messages in:
`%AppData%\HeShuiLa\`

## Build

Open the solution in Visual Studio and build the project. The application requires WPF and Windows Forms references.

## License

MIT License
