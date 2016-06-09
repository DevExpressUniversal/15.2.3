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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class DesignerFormBase : XtraForm {
		const double InvisibleAreaPercent = 0.2;
		public static bool ShowDesignerOnChartAdding {
			get {
				PropertyStore store = new PropertyStore(ChartDesigner.XtraChartsRegistryPath);
				if (store == null)
					return true;
				store.Restore();
				return store.RestoreBoolProperty(ChartDesigner.XtraChartsShowDesignerRegistryEntry, true);
			}
			set {
				PropertyStore store = new PropertyStore(ChartDesigner.XtraChartsRegistryPath);
				if (store == null)
					return;
				store.AddProperty(ChartDesigner.XtraChartsShowDesignerRegistryEntry, value);
				store.Store();
			}
		}
		readonly DesignerFormLayout layout;
		IDesignerHost designerHost;
		public Chart Chart { get { return designerController.Chart; } }
		public IDesignerHost DesignerHost { get { return designerHost; } }
		DesignerFormBase() {
			InitializeComponent();
			designerController.SetTabs(tbpData, tbpProperties, tbpOptions);
			this.Disposed += DesignerFormBase_Disposed;
		}
		public DesignerFormBase(ChartDesigner designer, UserLookAndFeel lookAndFeel) : this() {
			designerHost = designer.DesignerHost;
			cbShowOnNextStart.Checked = ShowDesignerOnChartAdding;
			layout = new DesignerFormLayout();
			InitializeFormByLayout();
			DevExpress.Skins.SkinManager.EnableFormSkins();
			LookAndFeel.Assign(lookAndFeel);
			designerController.ChartContainer = designer.ChartContainer;
			designerController.SetIsDesignTime(designerHost != null);
		}
		void InitializeFormByLayout() {
			StartPosition = FormStartPosition.CenterScreen;
			if (layout.Size != Size.Empty)
				Size = layout.Size;
			WindowState = layout.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
		}
		void DesignerFormBase_Disposed(object sender, EventArgs e) {
			this.Disposed -= DesignerFormBase_Disposed;
			ucPointGridControl.Dispose();
			chartDataControl.Dispose();
		}
		void DesignerFormBase_ResizeEnd(object sender, EventArgs e) {
			layout.Size = Size;
		}
		DialogResult ShowCloseDialog() {
			return XtraMessageBox.Show((UserLookAndFeel)LookAndFeel, "Do you want to save the changes?", "Close chart designer", MessageBoxButtons.YesNoCancel);
		}
		void DesignerFormBase_FormClosing(object sender, FormClosingEventArgs e) {
			if (DialogResult == DialogResult.Cancel && designerController.ChartChanged) {
				DialogResult closeDialogResult = ShowCloseDialog();
				if (closeDialogResult == DialogResult.Yes)
					DialogResult = DialogResult.OK;
				else if (closeDialogResult == DialogResult.No)
					DialogResult = DialogResult.Cancel;
				else
					e.Cancel = true;
			}
		}	   
		void ShowOnNextStart_CheckedChanged(object sender, EventArgs e) {
			ShowDesignerOnChartAdding = cbShowOnNextStart.Checked;
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch (keyData) {
				case Keys.Z | Keys.Control:
					designerController.Undo();
					return true;
				case Keys.Y | Keys.Control:
					designerController.Redo();
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		internal void SaveLayoutToRegistry() {
			layout.Maximized = WindowState == FormWindowState.Maximized;
			layout.SaveLayoutToRegistry();
		}
		public void HideStartupCheckBox() {
			cbShowOnNextStart.Visible = false;
		}
	}
	public class DesignerSplitter : SplitterControl {
		protected override void OnPaint(PaintEventArgs e) {
			using (GraphicsCache cache = new GraphicsCache(e)) {
				using (SolidBrush sb = new SolidBrush(CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.Control)))
					cache.FillRectangle(sb, ClientRectangle);
				ObjectPainter.DrawObject(cache, Painter, null);
			}
		}
	}
}
