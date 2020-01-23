# IPList Mac
A tool to list IP addresses in a network while optionally pinging and performing DNS lookups. This can be useful when you need a comma or newline delimited list of IP addresses in a network or if you just want to see which hosts are up on a given subnet. IPList is written in C# using Visual Studio 2019 and Xamarin as an introduction to macOS development with C#.

# Features
* List all IP address in a network
* Perform a ping scan of a network to determine host availability
* Perform a port scan of any network host to determine service availability
* Perform a constant ping against a network host to monitor availability
* Copy IP and port lists to the clipboard with a user defined delimiter
* Multithreaded for performance

# Dependencies
IPList relies on the awesome [IPNetwork](https://github.com/lduchosal/ipnetwork) library and runs on macOS 10.10 or higher. A binary is available [here](https://github.com/mcherry/IPList.macOS/raw/master/Binary/IPList.app.tgz) otherwise it can be easily compiled with [Visual Studio 2019](https://visualstudio.microsoft.com/vs/).

# Resources
In no particular order, here are some of the resources I used to hack this project together:
* [Windows in Xamarin.Mac](https://docs.microsoft.com/en-us/xamarin/mac/user-interface/window)
* [Table Views in Xamarin.Mac](https://docs.microsoft.com/en-us/xamarin/mac/user-interface/table-view)
* [ThreadPool Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool?view=netframework-4.8)
* [Thread Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread?view=netframework-4.8)
* [Xamarin Documentation](https://docs.microsoft.com/en-us/xamarin/)
* [Xamarin.Mac Community Forums](https://forums.xamarin.com/categories/xamarin-mac)
* [StackOverflow](https://stackoverflow.com/questions/tagged/xamarin)

# Screenshots
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/005.png?raw=true "Screenshot 1")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/006.png?raw=true "Screenshot 2")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/007.png?raw=true "Screenshot 3")
![Screenshot](https://github.com/mcherry/IPList.macOS/blob/master/Screenshots/004.png?raw=true "Screenshot 4")
