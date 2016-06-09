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
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class LayoutFrame : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		public LayoutFrame()
			: base(6) {
			InitializeComponent();
		}
		#region Init & Ctor
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(EditingView != null)
					((IDesignTimeSupport)EditingView).Unload();
				previewView.Layout -= View_Layout;
				previewView.DocumentClosing -= View_DocumentClosing;
				((IDesignTimeSupport)previewView).Unload();
				if(layoutChanged)
					if(MessageBox.Show(this, "The layout has been modified.\r\nDo you want to save the changes?", lbCaption.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						Apply();
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void InitImages() {
			base.InitImages();
			btnLoad.Image = DesignerImages16.Images[DesignerImages16LoadIndex];
			btnSave.Image = DesignerImages16.Images[DesignerImages16SaveIndex];
		}
		protected Control previewControl = null;
		protected virtual string PreviewPanelText { get { return "DocumentManager Preview"; } }
		public override void InitComponent() {
			InitEditingView();
			previewControl = CreatePreviewControl();
			previewControl.Parent = pnlPreview;
			previewControl.Dock = DockStyle.Fill;
			SetLayoutChanged(false);
			previewView.Layout += View_Layout;
			previewView.DocumentClosing += View_DocumentClosing;
		}
		void InitEditingView() {
			if(EditingView != null)
				((IDesignTimeSupport)EditingView).Load();
		}
		#endregion
		bool layoutChanged = false;
		protected virtual void SetLayoutChanged(bool enabled) {
			layoutChanged = enabled;
			btnApply.Enabled = enabled;
		}
		#region Editing
		void Apply() {
			currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			ApplyLayouts();
			Cursor.Current = currentCursor;
		}
		void btnApply_Click(object sender, System.EventArgs e) {
			Apply();
			SetLayoutChanged(false);
		}
		void btnLoad_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = Localizer.Active.GetLocalizedString(StringId.RestoreLayoutDialogFileFilter);
			dlg.Title = Localizer.Active.GetLocalizedString(StringId.RestoreLayoutDialogTitle);
			if(dlg.ShowDialog() == DialogResult.OK) {
				Refresh(true);
				try {
					RestoreLayoutFromXml(dlg.FileName);
				}
				catch { }
				Refresh(false);
			}
		}
		void btnSave_Click(object sender, System.EventArgs e) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = Localizer.Active.GetLocalizedString(StringId.SaveLayoutDialogFileFilter);
			dlg.Title = Localizer.Active.GetLocalizedString(StringId.SaveLayoutDialogTitle);
			if(dlg.ShowDialog() == DialogResult.OK) {
				Refresh(true);
				SaveLayoutToXml(dlg.FileName);
				Refresh(false);
			}
		}
		Cursor currentCursor;
		void Refresh(bool isWait) {
			if(isWait) {
				currentCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			}
			else
				Cursor.Current = currentCursor;
			this.Refresh();
		}
		#endregion
		protected override string DescriptionText {
			get { return "Modify the view's layout and click the Apply button to apply the modifications to the current view. You can also save the layout to an XML file (this can be loaded and applied to other views at design time and runtime)."; }
		}
		protected BaseView EditingView {
			get { return EditingObject as BaseView; }
		}
		DocumentManager previewManager;
		BaseView previewView;
		protected BaseView PreviewView { get { return previewView; } }
		IContainer components;
		Control CreatePreviewControl() {
			components = new Container();
			ContainerControl pcDocumentManager = new ContainerControl();
			previewManager = new DocumentManager(components);
			previewView = previewManager.CreateView(EditingView.Type);
			((IDesignTimeSupport)previewView).IsSerializing = true;
			((ISupportInitialize)previewManager).BeginInit();
			((ISupportInitialize)previewView).BeginInit();
			previewManager.ContainerControl = pcDocumentManager;
			previewManager.MdiParent = null;
			previewManager.ViewCollection.AddRange(new BaseView[] { previewView });
			previewManager.View = previewView;
			BeforeOnInit();			
			((ISupportInitialize)previewView).EndInit();
			((ISupportInitialize)previewManager).EndInit();
			((IDesignTimeSupport)previewView).Load();
			AfterOnInit();
			InitLayouts();
			((IDesignTimeSupport)previewView).IsSerializing = false;
			return pcDocumentManager;
		}
		protected virtual void BeforeOnInit() { }
		protected virtual void AfterOnInit() {
			BaseDocument[] documents = EditingView.Documents.ToArray();
			for(int i = 0; i < documents.Length; i++) {
				previewView.AddDocument(documents[i].Caption, documents[i].ControlName);
			}
		}
		void View_DocumentClosing(object sender, DocumentCancelEventArgs e) {
			e.Cancel = true;
		}
		protected void InitLayouts() {
			try {
				using(System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
					EditingView.SaveLayoutToStream(ms);
					ms.Seek(0, SeekOrigin.Begin);
					previewView.RestoreLayoutFromStream(ms);
				}
			}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected void ApplyLayouts() {
			try {
				using(BatchUpdate.Enter(EditingView)) {
					using(System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
						previewView.SaveLayoutToStream(ms);
						ms.Seek(0, System.IO.SeekOrigin.Begin);
						EditingView.RestoreLayoutFromStream(ms);
					}
				}
			}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected void RestoreLayoutFromXml(string fileName) {
			try {
				previewView.RestoreLayoutFromXml(fileName);
			}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected void SaveLayoutToXml(string fileName) {
			try {
				previewView.SaveLayoutToXml(fileName);
			}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		void View_Layout(object sender, EventArgs e) {
			SetLayoutChanged(true);
		}
	}
	[ToolboxItem(false)]
	public partial class WindowsUIViewLayoutFrame : LayoutFrame {
		public WindowsUIViewLayoutFrame()
			: base() {
		}
		protected override void BeforeOnInit() {
			if(PreviewView is Docking2010.Views.WindowsUI.WindowsUIView) {
				(PreviewView as Docking2010.Views.WindowsUI.WindowsUIView).UseSplashScreen = DevExpress.Utils.DefaultBoolean.False;
			}
			BaseDocument[] documents = EditingView.Documents.ToArray();
			for(int i = 0; i < documents.Length; i++) {
				PreviewView.AddDocument(documents[i].Caption, documents[i].ControlName);
			}
		}
		protected override void AfterOnInit() { }
	}
}
