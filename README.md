# TractorSupporter

## Development 
You will encounter two scenarios when developing the application:

### Isolated Development
If you work on tractor-supporter-win independently, you have to comment lines:  
```c#
_dataSender.SendDistanceData(distanceMeasured);
```  
```c#
_dataSender = new DistanceDataSender("DistancePipe");
```  
in **TSWindow.xaml.cs**

### Integrated Development
If you want to work with AgOpenGPS application is set correctly

## Switching to Mock Data

To configure the application to use mock data, follow these steps:

1. Open the `App.config` file located in the `TractorSupporter` directory.
2. Locate the `<appSettings>` section.
3. Ensure the following key-value pair is present:

```
<add key="UseMockData" value="true"/>
```

This setting enables the application to use mock data.

4. Save the `App.config` file.
