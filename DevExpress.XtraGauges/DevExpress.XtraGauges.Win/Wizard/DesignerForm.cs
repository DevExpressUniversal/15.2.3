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
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
namespace DevExpress.XtraGauges.Win.Wizard {
	public class GaugeDesignerForm : XtraForm {
		PropertyStore storeCore;
		IGauge gaugeCore;
		GaugeDesignerControl designerControlCore;
		BaseGaugeDesignerPage[] pagesCore;
#if !DEBUGTEST
		static string modifiedWarning = "The layout has been modified.\r\nDo you want to save the changes?";
#endif
		public GaugeDesignerForm(IGauge gauge) {
			this.gaugeCore = gauge;
			OnCreate();
		}
		protected override bool GetAllowSkin() {
			return (LookAndFeel != null) && LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin;
		}
		protected void OnCreate() {
			this.AllowFormSkin = true;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Element Designer";
			this.Size = new Size(800, 600);
			this.ShowIcon = false;
			this.MinimumSize = new Size(640, 480);
			this.designerControlCore = new GaugeDesignerControl();
			DesignerControl.SetGaugeCore(Gauge);
			DesignerControl.Finish += OnDesignerControlFinish;
			DesignerControl.Cancel += OnDesignerControlCancel;
			designerControlCore.Parent = this;
			this.pagesCore = new BaseGaugeDesignerPage[0];
			this.storeCore = new PropertyStore(this.RegistryStorePath);
			Store.Restore();
		}
		protected string RegistryStorePath {
			get { return @"Software\Developer Express\XtraGauges\Designer\"; }
		}
		protected PropertyStore Store {
			get { return storeCore; }
		}
		protected void StoreProperties() {
			if(Store != null) {
				Store.AddForm(this);
			}
		}
		protected void RestoreProperties() {
			if(Store != null) {
				foreach(BaseGaugeDesignerPage page in Pages) {
					string settings = (string)Store.RestoreProperty(page.Caption, null);
					if(!string.IsNullOrEmpty(settings))
						page.LoadSettings(settings);
				}
				Store.RestoreForm(this);
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			DesignerControl.Pages.AddRange(Pages);
			RestoreProperties();
		}
		protected override void OnClosed(EventArgs e) {
			if(Store != null) {
				foreach(BaseGaugeDesignerPage page in Pages) {
					string settings = page.SaveSettings();
					if(!string.IsNullOrEmpty(settings)) {
						Store.AddProperty(page.Caption, settings);
					}
				}
				StoreProperties();
				Store.Store();
			}
			base.OnClosed(e);
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			base.OnFormClosing(e);
			for(int i = 0; i < Pages.Length; i++) {
				Pages[i].OnDesignerClosing();
			}
			if(needCheckClosing) OnDesignerControlFinish(this, e);
		}
		bool needCheckClosing = true;
		void OnDesignerControlCancel(object sender, EventArgs e) {
			needCheckClosing = false;
		}
		void OnDesignerControlFinish(object sender, EventArgs e) {
			bool modified = CheckModified();
			if(modified) {
#if DEBUGTEST
				bool fNeedApply = true;
#else
				bool fNeedApply = XtraMessageBox.Show(
						this, 
						modifiedWarning, 
						"Gauge Designer", 
						MessageBoxButtons.YesNo, 
						MessageBoxIcon.Question, 
						MessageBoxDefaultButton.Button2)
					== DialogResult.Yes;
#endif
				if(fNeedApply)
					ApplyChanges();
				DialogResult = fNeedApply ?
					DialogResult.OK : DialogResult.Cancel;
			}
			needCheckClosing = false;
			Close();
		}
		void ApplyChanges() {
			for(int i = 0; i < Pages.Length; i++) {
				if(Pages[i].IsModified) Pages[i].ApplyChanges();
			}
		}
		bool CheckModified() {
			bool modified = false;
			for(int i = 0; i < Pages.Length; i++) modified |= Pages[i].IsModified;
			return modified;
		}
		protected internal IGauge Gauge {
			get { return gaugeCore; }
		}
		protected internal GaugeDesignerControl DesignerControl {
			get { return designerControlCore; }
		}
		public BaseGaugeDesignerPage[] Pages {
			get { return pagesCore; }
			set { pagesCore = value; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(DesignerControl != null) {
					DesignerControl.Finish -= OnDesignerControlFinish;
					DesignerControl.Cancel -= OnDesignerControlCancel;
					DesignerControl.Dispose();
					designerControlCore = null;
				}
				gaugeCore = null;
			}
			base.Dispose(disposing);
		}
	}
}
