using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LogViewer
{
    public delegate List<LogEntry> AsyncSearchLog();
    public partial class MainWindow : Window
    {
        public ObservableCollection<LogEntry> LogEntries1 { get; set; }
        public ObservableCollection<LogEntry> LogEntries2 { get; set; }
        public ObservableCollection<LogEntry> searchResultLogEntries { get; set; }

        public string logfilepath1;
        public string logfilepath2;

        public List<Tuple<string, int>> data;

        public MainWindow()
        {
            InitializeComponent();
            Log1.ItemsSource = LogEntries1 = new ObservableCollection<LogEntry>();
            Log2.ItemsSource = LogEntries2 = new ObservableCollection<LogEntry>();
            searchResultLogEntries = new ObservableCollection<LogEntry>();
            searchResultsListview.ItemsSource = searchResultLogEntries;
            data = new List<Tuple<string, int>>();
        }

        private void Log1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            ObservableCollection<LogEntry> logentries = null;
            LogEntry le = null;
            int intselectedindex = 0;
            VirtualizingStackPanel vsp1, vsp2;

            vsp1 = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost",
                BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, Log1, null);

            vsp2 = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost",
                BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, Log2, null);

            try
            {
                if (lv.Name == "Log1")
                {
                    intselectedindex = Log1.SelectedIndex;
                    le = (LogEntry)Log1.Items[intselectedindex];
                    logentries = LogEntries2;
                }
                else
                {
                    intselectedindex = Log2.SelectedIndex;
                    le = (LogEntry)Log2.Items[intselectedindex];
                    logentries = LogEntries1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception : " + ex.Message);
            }


            if (intselectedindex >= 0)
            {
                //LogEntry le = (LogEntry)Log1.Items[intselectedindex];

                double scrollHeight = vsp1.ScrollOwner.ScrollableHeight;

                int index = 0;

                for (int i = 0; i < logentries.Count; i++)
                {
                    if (le.Time.CompareTo(logentries[i].Time) == 0)
                    {
                        index = i;
                        break;
                    }
                    else if (le.Time.CompareTo(logentries[i].Time) < 0 && i > 0)
                    {
                        index = i - 1;
                        break;
                    }
                }

                if (lv.Name == "Log1")
                    vsp2.SetVerticalOffset(index - (le.Index - vsp1.VerticalOffset));
                else
                    vsp1.SetVerticalOffset(index - (le.Index - vsp2.VerticalOffset));

            }
        }

        private void Log1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        void Handle(CheckBox checkBox)
        {
            bool flag = checkBox.IsChecked.Value;

            if (Log1 != null)
            {
                if (flag == false)
                {
                    Grid.SetColumnSpan(Log1, 2);
                    Log2.Visibility = Visibility.Hidden;
                }
                else if (flag == true)
                {
                    Grid.SetColumnSpan(Log1, 1);
                    Log2.Visibility = Visibility.Visible;
                }
            }
        }

        private async void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            MenuItem mi = sender as MenuItem;
            ObservableCollection<LogEntry> logentries;

            if (openFileDialog.ShowDialog() == true)
            {
                string filepath = openFileDialog.FileName;
                if (mi.Header.ToString().Contains("File 1"))
                {
                    logentries = LogEntries1;
                    LogEntries1.Clear();
                    logfilepath1 = filepath;
                }
                else
                {
                    logentries = LogEntries2;
                    LogEntries2.Clear();
                    logfilepath2 = filepath;
                }

                //using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                //{
                //    var file = new System.IO.StreamReader(fileStream, System.Text.Encoding.UTF8, true, 128);
                //    string line = "";
                //    int lineNumber = 1;

                //    while ((line = file.ReadLine()) != null)
                //    {
                //        logentries.Add(Util.parseLine(line, lineNumber));
                //        lineNumber++;
                //    }
                //}
                int lineNumber = 1;
                var logLines = await Util.ReadAllLinesAsync(filepath, Encoding.UTF8);
                logLines.ForEach(logline => logentries.Add(Util.parseLine(logline, lineNumber++)));
                Console.WriteLine("Total number of lines: " + logLines.Count);
            }
        }

        //private void LoadLog(ObservableCollection<LogEntry> logentries, string filepath)
        //{
        //    BackgroundWorker bw = new BackgroundWorker();

        //    // this allows our worker to report progress during work
        //    bw.WorkerReportsProgress = true;
        //    int progressDone = 0;
        //    // what to do in the background thread
        //    bw.DoWork += new DoWorkEventHandler(
        //    delegate (object o, DoWorkEventArgs args)
        //    {
        //        BackgroundWorker b = o as BackgroundWorker;

        //        using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        //        {
        //            var file = new System.IO.StreamReader(fileStream, System.Text.Encoding.UTF8, true, 128);
        //            string line = "";
        //            int lineNumber = 1;

        //            while ((line = file.ReadLine()) != null)
        //            {
        //                logentries.Add(Util.parseLine(line, lineNumber));
        //                lineNumber++;
        //            }
        //        }
        //        //actualSearch(items, searchtext, b);

        //    });

        //// what to do when progress changed (update the progress bar for example)
        //bw.ProgressChanged += new ProgressChangedEventHandler(
        //delegate (object o, ProgressChangedEventArgs args)
        //{
        //    //label1.Text = string.Format("{0}% Completed", args.ProgressPercentage);
        //    //update search progress bar
        //    if (args.ProgressPercentage != progressDone)
        //    {
        //        progressDone = args.ProgressPercentage;
        //        Console.WriteLine("We have completed" + progressDone + " search");
        //    }
        //});

        // what to do when worker completes its task (notify the user)
        //    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
        //    delegate (object o, RunWorkerCompletedEventArgs args)
        //    {
        //        //label1.Text = "Finished!";
        //    });

        //    bw.RunWorkerAsync();
        //}

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.InvokeShutdown();
            Application.Current.Shutdown();
        }

        //async void SearchButton_Click(object sender, RoutedEventArgs e)
        //{
        //    searchResultLogEntries.Clear();
        //    string searchtext = SearchTextBox.Text;

        //    if (searchtext == "")
        //        return;

        //    Task<List<LogEntry>> task = new Task<List<LogEntry>>(() => SearchLog(LogEntries1, searchtext));


        //    bool multiViewflag = false;
        //    if (multiViewflag)
        //    {
        //        //SearchInLog(Log1.Items, searchtext);
        //        //SearchInLog(Log2.Items, searchtext);
        //    }
        //    else
        //    {
        //        task.Start();
        //        List<LogEntry> searchResult = await task;
        //        //List<LogEntry> searchResult = await SearchLog(LogEntries1, searchtext);
        //        ObservableCollection<LogEntry> searchResultOC = new ObservableCollection<LogEntry>(searchResult);
        //        searchResultsListview.ItemsSource = searchResultOC;
        //    }
        //    //searchResultsListview.ItemsSource = searchResultLogEntries;
        //    searchResultsListview.Visibility = Visibility.Visible;
        //}

        //private List<LogEntry> SearchLog(ObservableCollection<LogEntry> logentries, string text)
        //{
        //    List<LogEntry> lentries = new List<LogEntry>();
        //    for (int i = 0; i < logentries.Count; i++)
        //    {
        //        LogEntry le = LogEntries1[i];
        //        if (le.Message.Contains(text))
        //        {
        //            lentries.Add(le);
        //        }
        //    }
        //    return lentries;
        //}

        void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchtext = SearchTextBox.Text;
            searchResultLogEntries.Clear();
            SearchButton.IsEnabled = false;

            Thread thread = new Thread(() =>
            {
                //Task<List<LogEntry>> task = new Task<List<LogEntry>>(() => SearchLog(LogEntries1, searchtext));
                List<LogEntry> ResultsList = null;
                bool multiViewflag = false;
                if (multiViewflag)
                {
                    //SearchInLog(Log1.Items, searchtext);
                    //SearchInLog(Log2.Items, searchtext);
                }
                else
                {
                    //SearchButton.IsEnabled = false;
                    //ResultsList = await Task.Run(() => SearchLog(LogEntries1, searchtext));
                    //await SearchLog(LogEntries1, searchtext);
                    ResultsList = SearchLog(LogEntries1, searchtext);
                }

                //ResultsList.ForEach(le => searchResultLogEntries.Add(le));

                //searchResultsListview.ItemsSource = searchResultLogEntries;
                Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => searchResultsListview.ItemsSource = searchResultLogEntries));
                //App.Current.Dispatcher.Invoke(new Action(() => searchResultsListview.ItemsSource = searchResultLogEntries), DispatcherPriority.ContextIdle);
                Dispatcher.Run();
            });
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            searchResultsListview.Visibility = Visibility.Visible;
            SearchButton.IsEnabled = true;
        }
        private async Task<List<LogEntry>> SearchLogAsync(ObservableCollection<LogEntry> logentries, string text)
        {
            var results = new List<LogEntry>();

            var matches = logentries.AsParallel().Where(s => s.Message.Contains(text)).ToList();

            foreach (var le in matches)
            {
                Dispatcher.Invoke(DispatcherPriority.ContextIdle,
                    new Action(() => searchResultLogEntries.Add(le)));
            }

            return matches;
            #region searchloop
            //int count = 1000;
            //for (int i = 0; i < logentries.Count; i++)
            //{
            //    if (LogEntries1[i].Message.Contains(text))
            //    {
            //        results.Add(LogEntries1[i]);

            //        //if (i >= count)
            //        //{
            //        //    count += 1000;

            //        //    await Task.Delay(100);
            //        //    results.Clear();
            //        //}
            //    }
            //}
            #endregion
        }

        private List<LogEntry> SearchLog(ObservableCollection<LogEntry> logentries, string text)
        {
            var results = new List<LogEntry>();

            var matches = logentries.AsParallel().Where(s => s.Message.Contains(text)).ToList();

            foreach (var le in matches)
            {
                Dispatcher.Invoke(DispatcherPriority.ContextIdle,
                    new Action(() => searchResultLogEntries.Add(le)));
            }

            return matches;
        }

        #region BackgroundWorker
        void SearchInLog(ItemCollection items, String searchtext)
        {
            BackgroundWorker bw = new BackgroundWorker();
            // this allows our worker to report progress during work
            bw.WorkerReportsProgress = true;
            int progressDone = 0;
            // what to do in the background thread
            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                actualSearch(items, searchtext, b);
            });

            // what to do when progress changed (update the progress bar for example)

            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
                //label1.Text = string.Format("{0}% Completed", args.ProgressPercentage);
                //update search progress bar
                if (args.ProgressPercentage != progressDone)
                {
                    progressDone = args.ProgressPercentage;
                    //Console.WriteLine("We have completed" + progressDone + " search");
                }
            });

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                //label1.Text = "Finished!";

            });

            bw.RunWorkerAsync();
        }

        private void actualSearch(ItemCollection items, String searchText, BackgroundWorker b)
        {
            //report progress using b.ReportProgress(i * 100 / items.Count);
            // do some simple processing for 10 seconds
            for (int i = 0; i < items.Count; i++)
            {
                //LogEntry le = (LogEntry)Log1.Items[i];
                LogEntry le = LogEntries1[i];
                if (le.Message.Contains(searchText))
                {
                    //Console.WriteLine(le.Index + " " + le.Message);
                    b.ReportProgress(i * 100 / items.Count);
                }
            }
        }
        #endregion

        private void treeviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (logfilepath1 != null)
            {
                Thread thread = new Thread(() =>
                {
                    using (var fileStream = new FileStream(logfilepath1, FileMode.Open, FileAccess.Read))
                    {
                        var file = new System.IO.StreamReader(fileStream, System.Text.Encoding.UTF8, true, 128);
                        string line = "";
                        int counter = 1;
                        int level = 0;

                        while ((line = file.ReadLine()) != null)
                        {
                            line = counter + "::" + line + Environment.NewLine;
                            //richTextBox1.Focus();
                            if (line.Contains("+ ENTERING"))
                            {
                                int first = line.IndexOf('>');
                                first++;
                                int last = line.LastIndexOf(':');
                                string function_name = line.Substring(first, last - first);
                                data.Add(Tuple.Create(function_name + ":" + counter, level));
                                function_name = function_name + ":" + counter + "<" + level + ">" + Environment.NewLine;
                                level++;
                            }
                            if (line.Contains("- EXITING"))
                            {
                                level--;
                                int first = line.IndexOf('>');
                                first++;
                                int last = line.LastIndexOf(':');
                                string function_name = line.Substring(first, last - first);
                                data.Add(Tuple.Create(function_name + ":" + counter, level));
                                function_name = function_name + ":" + counter + "<" + level + ">" + Environment.NewLine;
                            }
                            counter++;
                        }

                        Window tv = new TreeViewWindow(data);
                        tv.Show();

                        tv.Closed += (sender2, e2) =>
                        tv.Dispatcher.InvokeShutdown();
                        Dispatcher.Run();
                    }
                });
                thread.IsBackground = true;
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void MenuItem_Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dinfo = new DirectoryInfo(@"C:\Program Files\Veritas\NetBackup\logs");

            List<FileInfo> files = new List<FileInfo>();

            try
            {
                foreach (string d in Directory.GetDirectories(dinfo.FullName))
                {
                    DateTime dt = DateTime.MinValue;
                    FileInfo current = null;
                    string[] dfiles = Directory.GetFiles(d);

                    if (dfiles.Length != 0)
                    {
                        foreach (string f in dfiles)
                        {
                            FileInfo fi = new FileInfo(f);
                            if (fi.LastWriteTime > dt)
                            {
                                current = fi;
                                dt = fi.LastWriteTime;
                            }
                        }
                        files.Add(current);
                        if (current.Directory.Name == "nbpem" || current.Directory.Name == "nbemm"
                            || current.Directory.Name == "bpcd" || current.Directory.Name == "nbsl" || current.Directory.Name == "nbrd")
                            Console.WriteLine(current.Directory.Name + " " + current.FullName);

                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            Console.Write(files.Count);
        }
    }
}
