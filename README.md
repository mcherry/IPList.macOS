# IPList Mac
A tool to list IP addresses in a network while optionally pinging and performing DNS lookups. This can be useful when you need a comma or newline delimited list of IP addresses in a network or if you just want to see which hosts are up on a given subnet. The port scanner allows you to quickly view open TCP services on a host. IPList is written in C# using Visual Studio 2019 and Xamarin as an introduction to macOS development with C#.

# Features
* List all IP address in a network
* Perform a DNS lookup on all IP addresses in a network
* Perform a ping scan of a network to determine host availability
* Perform a port scan of any network host to determine service availability
* Read data from open ports
* Read HTML from open http server ports
* Read available shares from open CIFS port
* Perform a constant ping against a network host to monitor availability
* Copy IP and port lists to the clipboard with a user defined delimiter
* Copy DNS hostname to the clipboard
* User definable timeouts and port scanning list (top 1000 and 100 ports, all ports, custom list)
* Check for updates from the main menu or on program startup
* Multithreaded for performance

# Dependencies
IPList relies on the awesome [IPNetwork](https://github.com/lduchosal/ipnetwork) library and runs on macOS 10.10 or higher. A binary is available [here](https://github.com/mcherry/IPList.macOS/raw/master/Binary/IPList.app.tgz) otherwise it can be easily compiled with [Visual Studio 2019](https://visualstudio.microsoft.com/vs/).

# Resources
In no particular order, here are some of the resources I used to hack this project together:
* [Windows in Xamarin.Mac](https://docs.microsoft.com/en-us/xamarin/mac/user-interface/window)
* [Images in Xamarin.Mac](https://docs.microsoft.com/en-us/xamarin/mac/app-fundamentals/image)
* [Menus in Xamarin.Mac](https://docs.microsoft.com/en-us/xamarin/mac/user-interface/menu)
* [Table Views in Xamarin.Mac](https://docs.microsoft.com/en-us/xamarin/mac/user-interface/table-view)
* [ThreadPool Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool?view=netframework-4.8)
* [Thread Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread?view=netframework-4.8)
* [Xamarin Documentation](https://docs.microsoft.com/en-us/xamarin/)
* [Xamarin.Mac Community Forums](https://forums.xamarin.com/categories/xamarin-mac)
* [StackOverflow](https://stackoverflow.com/questions/tagged/xamarin)
* [SettingsPlugin Documentation](https://github.com/jamesmontemagno/SettingsPlugin/tree/master/docs)
* [Nmap Default Ports](https://nullsec.us/top-1-000-tcp-and-udp-ports-nmap-default/)

# Screenshots
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/012.png?raw=true "Screenshot 1")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/009.png?raw=true "Screenshot 2")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/0010.png?raw=true "Screenshot 3")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/0011.png?raw=true "Screenshot 4")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/014.png?raw=true "Screenshot 5")
