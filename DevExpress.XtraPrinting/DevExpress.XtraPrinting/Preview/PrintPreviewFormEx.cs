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
using DevExpress.XtraEditors;
using DevExpress.Utils.Serializing;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview.Native;
namespace DevExpress.XtraPrinting.Preview
{
	public class PrintPreviewFormExBase : XtraForm, IPrintPreviewForm {
#if DEBUGTEST
		const bool defaultSaveState = false;
#else
		const bool defaultSaveState = true;
#endif
		private bool saveState = defaultSaveState;
		PreviewFormExtender extender;
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewFormExBasePrintControl"),
#endif
 Browsable(false)]
		public Control.PrintControl PrintControl { get { return extender.PrintControl; } 
		}
		[Browsable(false)]
		public int SelectedPageIndex { 
			get { return PrintControl.SelectedPageIndex; }
			set { PrintControl.SelectedPageIndex = value; }
		}
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewFormExBaseSaveState"),
#endif
		Category(NativeSR.CatPrinting),
		DefaultValue(defaultSaveState),
		]
		public bool SaveState {
			get { return saveState; }
			set { saveState = value; }
		}
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewFormExBasePrintingSystem"),
#endif
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DevExpress.XtraPrinting.PrintingSystemBase PrintingSystem {
			get { return PrintControl.PrintingSystem; }
			set { PrintControl.PrintingSystem = value; }
		}
		public PrintPreviewFormExBase() {
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
	public class PrintPreviewFormEx : PrintPreviewFormExBase {
		protected PrintBarManager fPrintBarManager;
		protected BarAndDockingController fController;
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintPreviewFormExPrintBarManager"),
#endif
		Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PrintBarManager PrintBarManager {
			get { return fPrintBarManager; }
		}
		public PrintPreviewFormEx() {
			InitPrintBarManager();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(fPrintBarManager != null) {
					fPrintBarManager.Controller = null;
					fPrintBarManager.Dispose();
					fPrintBarManager = null;
				}
				if(fController != null) {
					fController.Dispose();
					fController = null;
				}
			}
			base.Dispose( disposing );
		}
		protected virtual void InitPrintBarManager() {
			fPrintBarManager = new PrintBarManager();
			fPrintBarManager.Form = this;
			fPrintBarManager.Initialize(PrintControl);
			fController = new BarAndDockingController();
			fPrintBarManager.Controller = fController;
			fController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
	}
}
