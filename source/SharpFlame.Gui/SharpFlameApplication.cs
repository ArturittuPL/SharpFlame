using System;
using Eto;
using Eto.Forms;
using SharpFlame.Gui.Forms;

namespace SharpFlame.Gui
{
	public class SharpFlameApplication : Application
	{
		public SharpFlameApplication(Generator generator)
			: base(generator)
		{
			this.Name = "Test Application";
			this.Style = "application";
		}

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm();

			base.OnInitialized(e);

			// show the main form
			MainForm.Show();
		}
		#if DESKTOP
		public override void OnTerminating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating(e);
			Log.Write(this, "Terminating");

			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
		#endif
	}
}

