using ExifLibrary;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Nrbf;
using System.Net;
using System.Text.Json.Nodes;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ExifTweaker
{
    public partial class Form1 : Form
    {
        private bool m_stopHealth = false;
        private Thread m_threadHealth = null;
        //private Timer timer = new Timer { Interval = 500 };
        string[] exts = new string[] { ".jpg", ".jpeg", ".png", ".raw" };
        BindingList<FileObj> filesList = new BindingList<FileObj>();// { get { return listBox1.Items.Cast<FileObj>().ToList(); } }// filesDict.SelectMany(x => x.Value.Select(y => Path.Combine(x.Key, y))).ToList(); } }
                                                                    //BindingList<FileObj> bindings = new BindingList<FileObj>();
        static char dec = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        List<FileObj> selectedItems { get { return dgv.SelectedRows.Cast<DataGridViewRow>().Select(r => (FileObj)r.DataBoundItem).ToList(); } }
        DMS lat;
        DMS lon;
        float progress;
        public Form1()
        {
            InitializeComponent();
            //dgv.DataSource = new BindingSource { DataSource = filesList };
            //timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
            //timer.Start();

            //StartThreadHealth();
            //dgv.DataSource = new BindingSource { DataSource = filesList };
        }
        private void OnTimerEvent(object sender, EventArgs e)
        {
            pgb.Invoke((MethodInvoker)delegate { pgb.Value = Math.Min(100, (int)Math.Round(100.0f * progress)); });
            //labelOutput.Invoke((MethodInvoker)delegate { labelOutput.Text = ""; });
        }


        private void StartThreadHealth()
        {
            m_stopHealth = false;
            m_threadHealth = new Thread(new ThreadStart(HealthThreadAsync));
            m_threadHealth.Name = "Thread health";
            m_threadHealth.Priority = ThreadPriority.Highest;
            m_threadHealth.Start();
        }
        private void StopThreadHealth()
        {
            if (m_threadHealth != null)
            {
                m_stopHealth = true;
                m_threadHealth.Join(1000);
                if (m_threadHealth.IsAlive)
                {
                    m_threadHealth.Abort();
                }
                m_threadHealth = null;
            }
        }

        private async void HealthThreadAsync()
        {
            while (!m_stopHealth)
            {
                var rnd = new Random();

                try
                {
                    Thread.Sleep(1000);
                    pgb.InvokeAsync(() => pgb.Value = Math.Min(100, (int)Math.Round(100.0f * progress)));
                    Console.WriteLine($"Progress {(int)Math.Round(100.0f * progress)}");
                    /*await Task.Run(() =>
                    {
                        //bgw.ReportProgress(Math.Min(100, (int)Math.Round(100.0f * progress))); 
                        Console.WriteLine($"Progress {(int)Math.Round(100.0f * progress)}");
                        //pgb.Invoke((MethodInvoker)delegate { pgb.Value = Math.Min(100, (int)Math.Round(100.0f * progress)); });
                    });*/

                }
                catch { }
            }
        }

        public void Suspend(bool suspend)
        {
            if (true)
            {
                if (suspend)
                {
                    main.Enabled = false;
                    dgv.DataSource = null;
                }
                else
                {
                    main.Enabled = true;
                    dgv.DataSource = new BindingSource { DataSource = filesList };
                    dgv.Invalidate();
                }
            }
            else
            {
                if (suspend)
                {
                    main.Enabled = false;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    SuspendBinding(dgv);
                    dgv.SuspendLayout();
                    dgv.Visible = false;
                    ((BindingSource)dgv.DataSource)?.SuspendBinding();
                }
                else
                {
                    main.Enabled = true;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    ResumeBinding(dgv);
                    dgv.ResumeLayout();
                    dgv.Visible = true;
                    dgv.Refresh();
                    ((BindingSource)dgv.DataSource)?.ResumeBinding();
                }
            }
        }

        public void SuspendBinding(DataGridView dgv)
        {
            CurrencyManager currencyManager;
            if (dgv.DataSource != null)
            {
                currencyManager = (CurrencyManager)BindingContext[dgv.DataSource];
                currencyManager.SuspendBinding();
            }
        }
        public void ResumeBinding(DataGridView dgv)
        {
            CurrencyManager currencyManager;
            if (dgv.DataSource != null)
            {
                currencyManager = (CurrencyManager)BindingContext[dgv.DataSource];
                currencyManager.ResumeBinding();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (double.TryParse(tLat.Text.Replace('.', dec), out double dlat)) lat = new DMS(dlat);
            if (double.TryParse(tLon.Text.Replace('.', dec), out double dlon)) lon = new DMS(dlon);

            bgw.RunWorkerAsync(new BgwArgument { Action = BgwAction.Change, Argument = selectedItems });

            //filesList.ResetBindings();
            //dgv.Refresh();
            //dgv.Visible = true;
        }

        public static string[] OpenFile(params string[] type)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            try
            {
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.Filter = type == null || type.Length == 0 ? "All files |*.*" : string.Join("|", type.Select(x => $"{x.ToUpper()} files|*{x.ToLower()}"));
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                dlg.Multiselect = true;
                dlg.Title = "Select files...";

                switch (dlg.ShowDialog())
                {
                    case DialogResult.OK:
                        return dlg.FileNames;
                    default:
                        return null;
                }
            }
            finally
            {
                dlg.Dispose();
            }
        }

        private void AddFiles(string[] files)
        {
            bgw.RunWorkerAsync(new BgwArgument { Action = BgwAction.Add, Argument = files });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] file = OpenFile(exts);
            if (file == null) return;
            AddFiles(file);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;
            AddFiles(files);
        }

        public Coordinates GetCoordinates(string location)
        {
            string api = "698b55e8cce73086066305tvs8cfd7f";

            string query = String.Format($"https://geocode.maps.co/search?q={location}& api_key={api}");
            string result = GetRequest(query);
            if (result == "") return null;

            JsonArray test = (JsonArray)JsonNode.Parse(result);
            var gps = test.Select(it => new Coordinates { lat = (string)it["lat"], lon = (string)it["lon"], name = (string)it["display_name"], type = it["address"][0].GetPropertyName() }).ToList();

            return gps[0];
        }

        public string GetRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void bGPS_Click(object sender, EventArgs e)
        {
            var toto = GetCoordinates(tGPS.Text);

            tLat.Text = toto != null ? toto.lat : "";
            tLon.Text = toto != null ? toto.lon : "";
            tName.Text = toto != null ? toto.name : "";
            tType.Text = toto != null ? toto.type : "";
        }

        private void dgv_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var g = (DataGridView)sender;
            if (!g.RowHeadersVisible) return;

            var r = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, g.RowHeadersWidth, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, e.RowIndex.ToString(), g.RowHeadersDefaultCellStyle.Font, r, g.RowHeadersDefaultCellStyle.ForeColor);

        }

        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete && sender is DataGridView dgv)
            {
                bgw.RunWorkerAsync(new BgwArgument { Action = BgwAction.Del, Argument = selectedItems });
            }
        }

        private async Task AddAsync(string file, int i, int l)
        {
            await InvokeAsync(() => { filesList.Add(new FileObj(file, filesList.Count)); });
            progress += 1.0f / l;
            await pgb.InvokeAsync(() => pgb.Value = Math.Min(100, (int)Math.Round(100.0f * progress)));
            //Debug.WriteLine($"File {i}/{l}: {file}");
        }
        private async Task ChangeAsync(FileObj file, int i, int l)
        {
            await InvokeAsync(() => { file.SetProps(dateTimePicker1.Value, lat, lon); });
            progress += 1.0f / l;
            await pgb.InvokeAsync(() => pgb.Value = Math.Min(100, (int)Math.Round(100.0f * progress)));
            //Debug.WriteLine($"File {i}/{l}: {file}");
        }
        private async Task RemoveAsync(FileObj file, int i, int l)
        {
            await InvokeAsync(() => { filesList.Remove(file); });
            progress += 1.0f / l;
            await pgb.InvokeAsync(() => pgb.Value = Math.Min(100, (int)Math.Round(100.0f * progress)));
            //Debug.WriteLine($"File {i}/{l}: {file}");
        }

        private class TaskArg
        {
            public FileObj o;
            public string n;
            public int l;
            public int i;
        }


        private async void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            await InvokeAsync(() => Suspend(true));
            await pgb.InvokeAsync(() => pgb.Value = 0);

            List<Task> tasks = new List<Task>();
            progress = 0;
            if (e.Argument is BgwArgument arg)
            {
                if (arg.Action == BgwAction.Add)
                {
                    string[] files = arg.Argument as string[];
                    for (int f = 0; f < files.Length; f++)
                    {
                        var file = files[f];
                        if (exts.Contains(Path.GetExtension(file)) && !filesList.Select(x => x.filename).ToList().Contains(file))
                        {
                            if (tasks.Where(x => !x.IsCompleted).Count() < 10)
                            {
                                var T = new TaskArg { n = file, i = f + 1, l = files.Length };
                                tasks.Add(new Func<TaskArg, Task>(async (T) => await Task.Run(() => AddAsync(T.n, T.i, T.l))).Invoke(T));
                            }
                            else
                            {
                                f--;
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
                else if (arg.Action == BgwAction.Change)
                {
                    List<FileObj> files = arg.Argument as List<FileObj>;
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (tasks.Where(x => !x.IsCompleted).Count() < 10)
                        {
                            var T = new TaskArg { o = file, i = i + 1, l = files.Count };
                            tasks.Add(new Func<TaskArg, Task>(async (T) => await Task.Run(() => ChangeAsync(T.o, T.i, T.l))).Invoke(T));
                        }
                        else
                        {
                            i--;
                            Thread.Sleep(10);
                        }
                    }
                }
                else if (arg.Action == BgwAction.Del)
                {
                    List<FileObj> files = arg.Argument as List<FileObj>;
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (tasks.Where(x => !x.IsCompleted).Count() < 10)
                        {
                            var T = new TaskArg { o = file, i = i + 1, l = files.Count };
                            tasks.Add(new Func<TaskArg, Task>(async (T) => await Task.Run(() => RemoveAsync(T.o, T.i, T.l))).Invoke(T));
                        }
                        else
                        {
                            i--;
                            Thread.Sleep(10);
                        }
                    }
                }
                await Task.WhenAll(tasks);
                await pgb.InvokeAsync(() => pgb.Value = 100);
                await InvokeAsync(() => Suspend(false));
            }
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 100)
            {

            }
            else
                progress = e.ProgressPercentage / 100.0f;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Suspend(false);
        }

        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var data = (FileObj)dgv.Rows[e.RowIndex].DataBoundItem;
            var img = data.GetImage();
            try
            {
                picBox.Image = img.ToImage();//.GetThumbnailImage((int)(ratio*width), (int)(ratio*height), null, IntPtr.Zero);// ImageFile.FromStream(new MemoryStream(data.GetImage().Thumbnail)).ToImage();
            }
            catch (Exception)
            {
                picBox.Image = null;
            }
        }

    }

    public enum BgwAction
    {
        Add, Del, Change
    }
    public class BgwArgument
    {
        public BgwAction Action;
        public object Argument;
    }

    public class FileObj
    {
        public string filename;
        private string name;
        private string date;
        private string lat;
        private string lon;
        private string city;
        private string country;
        private int id;

        public string Name { get { return name; } }
        public string Date { get { return date; } }
        public string City { get { return city; } }
        public string Country { get { return country; } }
        public string Latitude { get { return lat; } }
        public string Longitude { get { return lon; } }


        public FileObj(string filename, int id)
        {
            this.id = id;
            this.filename = filename;
            name = Path.GetFileNameWithoutExtension(filename);
            Refresh();
        }

        public ImageFile GetImage()
        {
            return ImageFile.FromFile(filename);
        }

        public async void RefreshAsync()
        {
            await Task.Run(() => { Refresh(); });
        }
        public async void SetPropsAsync(DateTime date, DMS lat, DMS lon)
        {
            await Task.Run(() => { SetProps(date, lat, lon); });
        }

        public void SetProps(DateTime date, DMS lat, DMS lon)
        {
            var image = GetImage();// ImageFile.FromFile(file.filename);
            //var DD = image.Properties.Get<ExifDateTime>(ExifTag.DateTime);
            //DD.Value = date;// dateTimePicker1.Value;
            image.Properties.Set(ExifTag.DateTime, date);
            image.Properties.Set(ExifTag.DateTimeOriginal, date);
            image.Properties.Set(ExifTag.DateTimeDigitized, date);
            if (lat != null && lon != null)
            {
                image.Properties.Set(ExifTag.GPSLatitude, lat.deg, lat.min, lat.sec);
                image.Properties.Set(ExifTag.GPSLongitude, lon.deg, lon.min, lon.sec);
                image.Properties.Set(ExifTag.GPSLatitudeRef, lat.neg ? GPSLatitudeRef.South : GPSLatitudeRef.North);
                image.Properties.Set(ExifTag.GPSLongitudeRef, lon.neg ? GPSLongitudeRef.West : GPSLongitudeRef.East);
            }
            image.Save(filename);
            Refresh();
        }

        public void Refresh()
        {
            ImageFile img = GetImage();
            var Lat = img.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
            var Lon = img.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLongitude);
            var Lar = Lat == null ? "" : img.Properties.Get(ExifTag.GPSLatitudeRef).ToString().First().ToString();
            var Lor = Lon == null ? "" : img.Properties.Get(ExifTag.GPSLongitudeRef).ToString().First().ToString();

            date = $"{img.Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal)}";
            lat = Lat == null ? "" : $"{(int)Lat.Degrees}°{(int)Lat.Minutes}'{(float)Lat.Seconds:F2}\" {Lar}";
            lon = Lon == null ? "" : $"{(int)Lon.Degrees}°{(int)Lon.Minutes}'{(float)Lon.Seconds:F2}\" {Lor}";

            Task.Run(Geocoding);
        }


        private void Geocoding()
        {
            Dictionary<string, string> lines = new Dictionary<string, string>();
            Process pProcess = new Process();

            //string path = Path.GetFullPath(".\\exiftool\\exiftool.exe");
            //string args = $"-api geolocation \"-geolocation*\" {filename}";

            //pProcess.StartInfo.FileName = $".\\exiftool\\exiftool.exe";
            //pProcess.StartInfo.Arguments = $"-api geolocation \"-geolocation*\" {filename}";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            //pProcess.StartInfo.WorkingDirectory = $".\\exiftool\\";
            pProcess.StartInfo.CreateNoWindow = true;
            //pProcess.Start();

            pProcess.StartInfo.FileName = "\"" + Path.GetFullPath(".\\exiftool.bat") + "\"";
            pProcess.StartInfo.Arguments = $"\"{filename}\"";//$"/k {path} {args}";
            try
            {
                pProcess.Start();

                while (!pProcess.StandardOutput.EndOfStream)
                {
                    var l = pProcess.StandardOutput.ReadLine().Split(':').Select(y => y.Trim()).ToList();
                    if (!lines.Keys.Contains(l[0]))
                        lines.Add(l[0], l[1]);
                }

                if (lines.Keys.Contains("Geolocation City"))
                    city = lines["Geolocation City"];
                if (lines.Keys.Contains("Geolocation Country"))
                    country = lines["Geolocation Country"];
            }
            catch (Exception e) 
            {
                //MessageBox.Show(e.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //public override string ToString() { return $"{name} - {ImageFile.FromFile(filename).Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal)}"; }
    }

    public class Coordinates
    {
        public string lat;
        public string lon;
        public string type;
        public string name;
    }

    public class DMS
    {
        public int deg;
        public int min;
        public float sec;
        public bool neg;

        public DMS(double coord)
        {
            neg = coord < 0;
            sec = (float)(coord * 3600);
            deg = (int)Math.Floor(Math.Abs(sec / 3600));
            sec = Math.Abs(sec % 3600);
            min = (int)Math.Floor(Math.Abs(sec / 60));
            sec %= 60;
        }
    }

    public static class ExtensionMethods
    {
        public static Image ToImage(this ImageFile img)
        {
            MemoryStream stream = new MemoryStream();
            img.Save(stream);
            return Image.FromStream(stream);
        }
    }
}
