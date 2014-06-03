using System;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Extensions;
using SharpFlame.Gui.Sections;
using SharpFlame.Mapping;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Forms
{
	public class MainForm : Form, IInitializable
	{
        [Inject]
        internal TextureTab TextureTab { get; set; }
        [Inject]
        internal TerrainTab TerrainTab { get; set; }
        [Inject]
        internal HeightTab HeightTab { get; set; }
        [Inject]
        internal ResizeTab ResizeTab { get; set; }
        [Inject]
        internal PlaceObjectsTab PlaceObjectsTab { get; set; }
        [Inject]
        internal ObjectTab ObjectTab { get; set; }
        [Inject]
        internal LabelsTab LabelsTab { get; set; }
        [Inject]
        internal MainMapView MainMapView { get; set; }

        [Inject]
        internal Actions.LoadMap LoadMapAction { get; set; }

        [Inject]
        internal SettingsManager Settings { get; set; }

	    [Inject]
	    internal ViewInfo ViewInfo { get; set; }

        private string mainMapName;   

        /// <summary>
        /// Sets the title Map.
        /// </summary>
        /// <value>The name of the main map.</value>
        public string MainMapName { 
            get { return mainMapName; } 
            set { 
                mainMapName = value;
                Title = string.Format("{0} - {1} {2}", mainMapName, Constants.ProgramName, Constants.ProgramVersion());
            }
        }

	    //Ninject initializer
	    void IInitializable.Initialize()
	    {
	        ClientSize = new Size(1024, 768);
            MainMapName = "No Map";
	        Icon = Resources.SharpFlameIcon();

	        var tabControl = new TabControl();
	        tabControl.TabPages.Add(new TabPage {Text = "Textures", Content = this.TextureTab});
	        tabControl.TabPages.Add(new TabPage {Text = "Terrain", Content = this.TerrainTab});
	        tabControl.TabPages.Add(new TabPage {Text = "Height", Content = this.HeightTab});
	        tabControl.TabPages.Add(new TabPage {Text = "Resize", Content = this.ResizeTab});
	        tabControl.TabPages.Add(new TabPage {Text = "Place Objects", Content = this.PlaceObjectsTab});
	        tabControl.TabPages.Add(new TabPage {Text = "Object", Content = this.ObjectTab});
	        tabControl.TabPages.Add(new TabPage {Text = "Label", Content = this.LabelsTab});

	        tabControl.SelectedIndexChanged += delegate
	            {
	                tabControl.SelectedPage.Content.OnShown(EventArgs.Empty);
	            };

            this.Closing += MainForm_Closing;


	        var splitter = new Splitter
	            {
	                Position = 392,
	                FixedPanel = SplitterFixedPanel.Panel1,
	                Panel1 = tabControl,
                    Panel2 = this.MainMapView
	            };

	        // Set the content of the form to use the layout
	        Content = splitter;

	        GenerateMenuToolBar();
	        Maximize();

            if (Settings.UpdateOnStartup) 
            { 
                var updater = App.Kernel.Get<Updater> ();
                updater.CheckForUpdatesAsync ().ThenOnUI(updatesAvailable => {
                    if (updatesAvailable > 0)
                    {
                        if (MessageBox.Show (
                            "Theres an Update available, do you want to download and apply it now?",
                            "Update available",
                            MessageBoxButtons.OKCancel,
                            MessageBoxType.Question) == DialogResult.Ok)
                        {
                            updater.PrepareUpdatesAsync ().ThenOnUI(worked => {
                                // TODO: Save the maps and ask the user for a restart here.
                                    if (worked) {
                                    updater.DoUpdate();
                                }
                            });
                        }
                    }
                });
            }
        }

        void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Kernel.Dispose();
        }

	    private void GenerateMenuToolBar()
		{
			var about = new Actions.About();
			var quit = new Actions.Quit();
            var settings = new Actions.Settings();

			var menu = new MenuBar();
			// create standard system menu (e.g. for OS X)
			Application.Instance.CreateStandardMenu(menu.Items);

			// add our own items to the menu

			var file = menu.Items.GetSubmenu("&File", 100);
			file.Items.GetSubmenu ("&New Map", 100);
			file.Items.AddSeparator ();
            file.Items.Add(LoadMapAction);
			file.Items.AddSeparator ();

			var saveMenu = file.Items.GetSubmenu ("&Save", 300);
			file.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("&Map fmap");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("&Quick Save fmap");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("Export map &LND");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("Export &Tile Types");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("Minimap Bitmap");
			saveMenu.Items.GetSubmenu ("Heightmap Bitmap");

			file.Items.GetSubmenu ("&Import", 400);
			file.Items.AddSeparator ();
			file.Items.GetSubmenu ("&Compile Map", 500);
			file.Items.AddSeparator ();
			file.Items.Add(quit);

			var toolsMenu = menu.Items.GetSubmenu("&Tools", 600);
			toolsMenu.Items.GetSubmenu ("Reinterpret Terrain", 100);
			toolsMenu.Items.GetSubmenu ("Water Triangle Correction", 200);
			toolsMenu.Items.AddSeparator ();
			toolsMenu.Items.GetSubmenu ("Flaten Under Oils", 300);
			toolsMenu.Items.GetSubmenu ("Flaten Under Structures", 400);
			toolsMenu.Items.AddSeparator ();
			toolsMenu.Items.GetSubmenu ("Generator", 500);

            var editMenu = menu.Items.GetSubmenu ("&Edit", 700);
            editMenu.Items.Add (settings);

			var help = menu.Items.GetSubmenu("&Help", 1000);
			help.Items.Add(about);

			// optional, removes empty submenus and duplicate separators
			// menu.Items.Trim();

	        var testing = menu.Items.GetSubmenu("TESTING");
	        testing.Items.GetSubmenu("CMD1").Click += (sender, args) =>
	            {
	                this.ViewInfo.LookAtTile(new XYInt(1, 1));
	            };
	        testing.Items.GetSubmenu("CMD2 - ViewPos").Click += (sender, args) =>
	            {
	                this.ViewInfo.ViewPosChange(new XYZInt(1024, 1024, 1024));
	            };
            testing.Items.GetSubmenu("CMD3 - Check Screen Calculation").Click += (sender, args) =>
            {
                var posWorld = new WorldPos();
                this.ViewInfo.ScreenXyGetTerrainPos(new XYInt(500, 1000), ref posWorld);
            };
            testing.Items.GetSubmenu("CMD4 - MousePos").Click += (sender, args) =>
                {
                    this.ViewInfo.MouseOver = new ViewInfo.clsMouseOver();
                    this.ViewInfo.MouseOver.ScreenPos.X = 500;
                    this.ViewInfo.MouseOver.ScreenPos.Y = 1000;
                    this.ViewInfo.MouseOverPosCalc();
                };
            

			Menu = menu;
		}
	}
}
