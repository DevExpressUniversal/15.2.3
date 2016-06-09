#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.Utils.Serializing;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Preview.Native;
namespace DevExpress.XtraPrinting.Preview.Native {
	class PreviewFormExtender : IDisposable {
		const string layoutKey = "\\Software\\Developer Express\\XtraPrinting\\";
		XtraForm form;
		PrintControl printControl;
		PrintingSystem ownPrintingSystem;
		FormLayout formLayout;
		public PrintControl PrintControl {
			get { return printControl; }
		}
		public PreviewFormExtender(XtraForm form) {
			this.form = form;
			formLayout = new FormLayout(form);
			InitializeComponent();
		}
		private void InitializeComponent() {
			this.printControl = new DevExpress.XtraPrinting.Control.PrintControl();
			form.SuspendLayout();
			this.printControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.printControl.Name = "fPrintControl";
			this.printControl.Size = new System.Drawing.Size(736, 590);
			this.printControl.TabIndex = 4;
			this.printControl.TabStop = false;
			form.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.printControl});
			form.ResumeLayout(false);
			form.Shown += new EventHandler(form_Shown);
			form.Load += new EventHandler(form_Load);
			form.FormClosed += new FormClosedEventHandler(form_FormClosed);
		}
		void form_FormClosed(object sender, FormClosedEventArgs e) {
			if(form.Owner != null && !form.Modal)
				ActivateForm(form.Owner);
		}
		static void ActivateForm(Form form) {
			if(form.WindowState == FormWindowState.Minimized)
				form.WindowState = FormWindowState.Normal;
			form.Activate();
		}
		void form_Load(object sender, EventArgs e) {
			if(printControl.PrintingSystem == null) {
				ownPrintingSystem = new PrintingSystem();
				ownPrintingSystem.PageSettings.AssignDefaultPageSettings();
				printControl.PrintingSystem = ownPrintingSystem;
				printControl.SetPanelVisibility(PrintingSystemCommand.DocumentMap, true);
				printControl.SetPanelVisibility(PrintingSystemCommand.Thumbnails, true);
			}
		}
		void form_Shown(object sender, EventArgs e) {
			printControl.SetFocus();
		}
		public virtual void Dispose() {
			if(form != null) {
				form.Shown -= new EventHandler(form_Shown);
				form.Load -= new EventHandler(form_Load);
				form.FormClosed -= new FormClosedEventHandler(form_FormClosed);
				form = null;
			}
			if(ownPrintingSystem != null) {
				ownPrintingSystem.Dispose();
				ownPrintingSystem = null;
			}
			if(formLayout != null) {
				formLayout.Dispose();
				formLayout = null;
			}
		}
		public void RestoreFormLayout() {
			XtraSerializer serializer = new RegistryXtraSerializer();
			serializer.DeserializeObject(formLayout, layoutKey, formLayout.GetType().Name);
			formLayout.RestoreFormLayout();
		}
		public void SaveFormLayout() {
			XtraSerializer serializer = new RegistryXtraSerializer();
			formLayout.SaveFormLayout();
			serializer.SerializeObject(formLayout, layoutKey, formLayout.GetType().Name);
		}
	}
}
namespace DevExpress.XtraPrinting.Preview {
	public interface IPrintPreviewForm {
		Control.PrintControl PrintControl { get; }
		int SelectedPageIndex { get; set; }
		bool SaveState { get; set;}
		PrintingSystemBase PrintingSystem { get; set; }
		void Show(UserLookAndFeel lookAndFeel);
		void Show(IWin32Window owner, UserLookAndFeel lookAndFeel);
		DialogResult ShowDialog(UserLookAndFeel lookAndFeel);
		DialogResult ShowDialog(IWin32Window owner, UserLookAndFeel lookAndFeel);
	}
	public class PrintPreviewRibbonFormExBase : RibbonForm, IPrintPreviewForm {
#if DEBUGTEST
		const bool defaultSaveState = false;
#else
		const bool defaultSaveState = true;
#endif
		private bool saveState = defaultSaveState;
		PreviewFormExtender extender;
		[Browsable(false)]
		public Control.PrintControl PrintControl { get { return extender.PrintControl; } 
		}
		[Browsable(false)]
		public int SelectedPageIndex { 
			get { return PrintControl.SelectedPageIndex; }
			set { PrintControl.SelectedPageIndex = value; }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(defaultSaveState),
		]
		public bool SaveState {
			get { return saveState; }
			set { saveState = value; }
		}
		[
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DevExpress.XtraPrinting.PrintingSystemBase PrintingSystem {
			get { return PrintControl.PrintingSystem; }
			set { PrintControl.PrintingSystem = value; }
		}
		public PrintPreviewRibbonFormExBase() {
			extender = new PreviewFormExtender(this);
			InitializeComponent();
			this.Text = PreviewLocalizer.GetString(PreviewStringId.PreviewForm_Caption);
		}
		public new void Show() {
			SetLookAndFeel(null);
			base.Show();
		}
		public new DialogResult ShowDialog() {
			SetLookAndFeel(null);
			return base.ShowDialog();
		}
		public new DialogResult ShowDialog(IWin32Window owner) {
			SetLookAndFeel(null);
			return base.ShowDialog(owner);
		}
		public void Show(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			base.Show(owner);
		}
		public void Show(UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			base.Show();
		}
		public DialogResult ShowDialog(UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			return base.ShowDialog();
		}
		public DialogResult ShowDialog(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			return base.ShowDialog(owner);
		}
		protected virtual void SetLookAndFeel(UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PrintPreviewFormExBase));
			this.ClientSize = new System.Drawing.Size(736, 590);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PrintPreviewFormExBase";
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(extender != null) {
					extender.Dispose();
					extender = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(saveState) LoadViewState();
		}
		protected override void OnClosing(CancelEventArgs e) {
			if(saveState) SaveViewState();
			base.OnClosing(e);
		}
		protected virtual void SaveViewState() {
			if(MdiParent == null)
				extender.SaveFormLayout();
		}
		protected virtual void LoadViewState() {
			if(MdiParent == null)
				extender.RestoreFormLayout();
		}
	}
	public class PrintPreviewRibbonFormEx : PrintPreviewRibbonFormExBase {
		PrintRibbonController printRibbonController;
		RibbonControl ribbonControl;
		RibbonStatusBar ribbonStatusBar;
		BarAndDockingController controller;
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewRibbonFormExPrintRibbonController"),
#endif
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PrintRibbonController PrintRibbonController { get { return printRibbonController; } }
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewRibbonFormExRibbonControl"),
#endif
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public RibbonControl RibbonControl { get { return ribbonControl; } }
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewRibbonFormExRibbonStatusBar"),
#endif
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public RibbonStatusBar RibbonStatusBar { get { return ribbonStatusBar; } }
		public PrintPreviewRibbonFormEx() {
			ribbonControl = new RibbonControl();
			ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
			ribbonControl.AllowMinimizeRibbon = false;
			ribbonControl.RibbonStyle = RibbonControlStyle.Office2010;
			ribbonControl.ToolbarLocation = RibbonQuickAccessToolbarLocation.Hidden;
			ribbonControl.ShowPageHeadersMode = ShowPageHeadersMode.ShowOnMultiplePages;
			ribbonStatusBar = new RibbonStatusBar();
			ribbonStatusBar.Ribbon = ribbonControl;
			Controls.Add(ribbonControl);
			Controls.Add(ribbonStatusBar);
			printRibbonController = new PrintRibbonController();
			printRibbonController.Initialize(ribbonControl, ribbonStatusBar);
			printRibbonController.PrintControl = PrintControl;
			controller = new BarAndDockingController();
			controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			ribbonControl.Controller = controller;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(printRibbonController != null) {
					printRibbonController.Dispose();
					printRibbonController = null;
				}
				if(controller != null) {
					controller.Dispose();
					controller = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
