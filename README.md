# .NET HMI Control Documentation

## Overview


### Required Libraries

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img1.jpg)

**System** - Contains fundamental classes and base classes that define commonly-used value and reference data types, events, event handlers, interfaces, attributes, and processing exceptions.

**System.Drawing** - Provides access to GDI+ basic graphics functionality. 

**System.Windows.Forms** - Contains classes for creating Windows-based applications that take advantage of the rich user interface features available in the Microsoft Windows operating system. 

**ScottPlot** - free and open-source plotting library for .NET that makes it easy to interactively display large datasets. 

**TwinCAT.Ads** - The Automation Device Specification (ADS) describes a device-independent and fieldbus-independent interface governing the type of access to ADS devices. 

**System.IO** - Contains types that allow reading and writing to files and data streams, and types that provide basic file and directory support. 


### Required Variables

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img2.jpg)

**private string NetID_TwinCAT** - string containing the TwinCAT Network ID for connecting to the PLC 

**Private int PortNumber** - port number for the ADS. The default value is 851. Subsequent PLCs will be numbered 852, 853, etc...

**Private double maximum** - the maximum magnitude number. This value cannot be lower than zero. The magnitude is displayed on the y-axis of the plot and represents the torque in ft-lbs. 

**Private double minimum** - the minimum magnitude number. This value cannot go below zero or exceed the value of the maximum magnitude. The magnitude is displayed on the y-axis of the plot and represents the torque in ft-lbs. 

**Private int hVar** - variable handle for connecting to the PLC.

**Private TcAdsCLient tcClient** - TwinCAT ADS client object.

**AdsStream dataStream** - Alternate Data Stream object, stream object used for ADS communication. 

**BinaryReader binRead** - object that reads primitive data types as binary values

**Double[] TwinCatArray** = new double[512] - array of 512 double values (read from the PLC as REAL values) that correspond to the torque values associated with each of the 512 frequencies. 

### Program Functions

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img3.jpg)

The **InitializeComponent()** initializes the form. PlotCreation() generates the plot from the ScottPlot library and specifies all plot settings. OpenStream() is a utility function that performs various tasks.

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img4.jpg)

The **timer1_Tick function** updates all values that are mapped to the plot and subsequently re-renders the plot. The timer is set to refresh the space of the plot every 100ms. 

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img5.jpg)

The **UpdateValues()** function reads from the data stream. Initially, the position is set to zero, and then a for loop runs through the stream, extracting all necessary values and converting them to doubles. In TwinCat3, these values were REALs, and the data-type equivalent in C# is a double. Min-max normalization is performed on these values, and the old values in the array are written over. 

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img6.jpg)

The **PlotCreation()** function sets the plot parameters for styling and visibility. 
A signal plot is used due to the rendering speed required for a large set of data points. 
The TwinCatArray values are loaded as an array of doubles. 
FillBelow is used to color below the plot line, improving visibility. 
Setting the outer and inner view limits to the same values disables the ability to move the graph. 
The style, Seaborn, sets a color scheme that is slightly darker than the original white graph theme. 
The axis limits determine the boundaries of what will be plotted in regards to the x and y axes. 
The benchmark is an optional parameter that displays the time needed to render the current contents of the plot. This is rendered in both milliseconds (ms) and hertz (Hz). This needs to be disabled during production, and should only be used to ensure the target computer can refresh the plot at 10 Hz. 
Title, XLabel, and YLabel are used for naming and labeling, respectively. 
Once all parameters are set, the plot is refreshed. 

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img7.jpg)

The **OpenStream()** function begins by enabling the timer. The timer is a windows form component that raises an event at user-defined intervals, in this case, every 100ms. A new tcClient object is created and subsequently connected using the Net Id and Port Number values. A variable handle is created (this matches the desired variable name given in the TwinCat3 project.). An ADS data stream is opened with 512 * 4 bytes. 512 REAL values are read from the PLC, each 32 bits/4bytes. A new binary reader object is created. 

![alt text](https://github.com/collinbennett1999/C-Beckhoff-HMI-Visualization/blob/main/img8.jpg)

The **MinMaxNormalization()** function takes every array value, transforming the minimum value into a zero and the maximum value into a one. Every other value gets transformed into a decimal between 0 and 1. 
