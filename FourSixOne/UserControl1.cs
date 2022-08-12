using System;
using System.Drawing;
using System.Windows.Forms;
using ScottPlot;
using TwinCAT.Ads;
using System.IO;

namespace ScottsPlots {
    public partial class Form1 : UserControl {
        // declaring all global program variables
        private string NetID_TwinCAT = "5.49.19.168.1.1";   // TwinCAT Network ID
        private int portNumber = 851;                       // ADS Port Number
        private double maximum = 4;                         // maximum magnitude number
        private double minimum = 0;                         // minimum magnitude number, cannot go below zero
        private int hVar;                                   // variable handle for connecting to PLC
        private TcAdsClient tcClient;                       // TwinCAT ADS client object
        AdsStream dataStream;                               // Alternate Data Stream Object - stream object used for ADS communication
        BinaryReader binRead;                               // reads primitive data types as binary values 
        double[] TwinCatArray = new double[512];            // array of 512 values measured from the PLC

        public Form1() {
            InitializeComponent();                          // initialization of the form component
			PlotCreation();                                 // setting of plot attributes
			OpenStream();                                   // opening of connection and setup of binary data stream
        }

        private void timer1_Tick(object sender, EventArgs e) {
            UpdateValues();                                 // on timer, refresh and update the data points (ten times a second)
            formsPlot1.Render();                            // rendering of the form
		}

        //
        // Helper Functions Below
        //

        public void UpdateValues() {
            try {
                tcClient.Read(hVar, dataStream);            // read data from the ADS device into the given stream
                dataStream.Position = 0;                    // set the current position within the stream to the beginning
                for (int i = 0; i < 512; i++) {             // for loop to iterate through all 512 array values              
                    TwinCatArray[i] = Convert.ToDouble(binRead.ReadSingle());               
                }
                MinMaxNormalization(TwinCatArray);          // given minimum and maximum values, perform normalization on the array of values                        
            }
            catch (Exception err) {
                MessageBox.Show(err.Message);               // if reading from datastream fails, display error
            }
        }

        public void PlotCreation() {
            var signal = formsPlot1.Plot.AddSignal(TwinCatArray, sampleRate: 1);    // generate a signal
            signal.FillBelow(Color.Maroon);                                         // coloring the graph maroon (Whoop '22)
            formsPlot1.Plot.SetOuterViewLimits(0, 511, -0.10, 1);                   // blocking panning function
            formsPlot1.Plot.SetInnerViewLimits(0, 511, -0.10, 1);                   // blocking zoom function
            formsPlot1.Plot.Style(Style.Seaborn);                                   // graph style
            formsPlot1.Plot.SetAxisLimitsX(0, 511);                                 // setting the x-axis limits to only cover the 0 -511 Hz frequencies
            formsPlot1.Plot.SetAxisLimitsY(-0.10, 1);                               // axis set to account for min-max normalization
            formsPlot1.Plot.Benchmark(enable: true);                                // benchmarking tool, displays the rendering time in hertz and milliseconds
            formsPlot1.Plot.Title("Reactive Torque");                               // graph title
            formsPlot1.Plot.XLabel("Frequency (Hz)");                               // x-axis represents frequency
            formsPlot1.Plot.YLabel("Magnitude (Normalized)");                       // y-axis represents magnitude
            formsPlot1.Refresh();                                                   // re-renders the plot
        }

        public void OpenStream() {
            timer1.Enabled = true;                                              // on button click, enable the timer (100ms clock)
            tcClient = new TcAdsClient();                                       // create an instance of the class TcAdsClient                          
            tcClient.Connect(NetID_TwinCAT, portNumber);                        // connecting to the specified PLC on the specified port
            try {
                hVar = tcClient.CreateVariableHandle("FFT.TorquePvMagnitude");  // attempt to connect to tag with specified name
            }
            catch (Exception err) {
                MessageBox.Show(err.Message);                                   // on error, display message
            }
            dataStream = new AdsStream(512 * 4);                                // allocation of memory for the data stream, 512 instances of 2-byte integers
            binRead = new BinaryReader(dataStream);                             // creation of new BinaryReader object
        }

        public void MinMaxNormalization(double[] TwinCatArray) {
            for (int i = 0; i < 512; i++) {
                TwinCatArray[i] = (TwinCatArray[i] - minimum) / (maximum - minimum);    // covert array values to min-max normalization
            }
        }
    }
}

