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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraPivotGrid;
namespace DevExpress.XtraPivotGrid.Design.Frames {
	[ToolboxItem(false)]
	public class PrintAppearancesDesigner : DevExpress.XtraEditors.Frames.AppearancesDesignerSimple {
		private System.ComponentModel.Container components = null;
		public PrintAppearancesDesigner() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).BeginInit();
			this.pnlAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.pnlPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.pgMain.Size = new System.Drawing.Size(423, 300);
			this.pnlControl.Location = new System.Drawing.Point(0, 38);
			this.pnlControl.Size = new System.Drawing.Size(588, 54);
			this.lbCaption.Size = new System.Drawing.Size(588, 42);
			this.horzSplitter.Location = new System.Drawing.Point(0, 92);
			this.horzSplitter.Size = new System.Drawing.Size(588, 4);
			this.Name = "PrintAppearancesDesigner";
			this.Size = new System.Drawing.Size(588, 396);
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).EndInit();
			this.pnlAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.pnlPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private PivotGridControl EditingPivotGrid { get { return EditingObject as PivotGridControl; } }
		protected override BaseAppearanceCollection Appearances { get { return EditingPivotGrid.AppearancePrint; } }
		protected override Image AppearanceImage { get { return null; } }
		protected override string DescriptionText { get { return "Select one or more of the Appearance objects to customize the printing appearances of the corresponding visual elements."; } }
		protected override XtraTabControl CreateTab() {
			return DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] {pgMain}, new string[] {"Properties"});
		}
		protected override void SetSelectedObject() {
			ArrayList arr = new ArrayList();
			BaseAppearanceCollection bc = EditingPivotGrid.PaintAppearancePrint;
			foreach(AppearanceObject obj in this.SelectedObjects) {
				arr.Add(bc.GetAppearance(obj.Name));
			}
			Preview.SetAppearance(arr.ToArray());
		}
		protected override void SetDefault() {
			EditingPivotGrid.BeginUpdate();
			base.SetDefault();
			SetSelectedObject();
			EditingPivotGrid.EndUpdate();
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			EditingPivotGrid.OptionsPrint.UsePrintAppearance = true;
			SetSelectedObject();
			base.pgMain_PropertyValueChanged(s, e);
		}
		int IsAppearanceNeedEnabled(string name) {
			if(lbcAppearances.SelectedIndices.IndexOf(lbcAppearances.FindString(name)) > -1) {
				return EditingPivotGrid.AppearancePrint.GetAppearance(name).ShouldSerialize() ? 1 : 0;
			}
			return -1;
		}
		protected override void LoadAppearances(string name) {
			base.LoadAppearances(name);
			SetSelectedObject();
			pgMain.Refresh();
		}
	}
}
