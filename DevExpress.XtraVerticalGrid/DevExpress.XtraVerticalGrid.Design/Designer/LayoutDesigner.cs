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
using System.Windows.Forms;
using System.Data;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Frames;
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraVerticalGrid;
namespace DevExpress.XtraVerticalGrid.Frames {
	[ToolboxItem(false)]
	public class Layouts : LayoutsBase{
		public Layouts() : base(6) {
		}
		protected override string PreviewPanelText { get { return "Vertical Grid Preview"; } }
		protected override string DescriptionText { get { return "Modify the vertical grid layout and click the Apply button to apply the modifications to the current vertical grid. You can also save the layout to an XML file (this can be loaded and applied to other vertical grids at design time and runtime)."; } }
		public VGridControl EditingVGrid { get { return EditingObject as VGridControl; } }
		VGridControl grid = null;
		protected override Control CreatePreviewControl() {
			grid = DevExpress.XtraVerticalGrid.Design.GridAssign.CreateGridControlAssign(EditingVGrid);
			grid.Tag = "Design";
			if(dataSet != null) 
				grid.DataSource = dataSet.Tables[tableName];
			else
				grid.DataSource = EditingVGrid.DataSource;
			InitVGrid();
			return grid;
		}
		protected override void ShowColumnsCustomization() {
			grid.RowsCustomization();
		}
		protected override void HideColumnsCustomization() {
			grid.DestroyCustomization();
		}
		protected override void ApplyLayouts() {
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			grid.SaveLayoutToStream(str);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			EditingVGrid.RestoreLayoutFromStream(str);
			str.Close();
		}
		protected override void RestoreLayoutFromXml(string fileName) {
			grid.RestoreLayoutFromXml(fileName);
		}
		protected override void SaveLayoutToXml(string fileName) {
			try {
			grid.SaveLayoutToXml(fileName);
		}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected override void SetControlDataSource(DataView dataView) {
			grid.DataSource = dataView;
			if(EditingVGrid.Rows.Count == 0)
				grid.RetrieveFields();
			else SetLayoutChanged(false);
		}
		protected override DBAdapter CreateDBAdapter() {
			ArrayList adapters = new ArrayList();
			foreach(object comp in EditingVGrid.Container.Components)
				adapters.Add(comp);
			return new DBAdapter(adapters, EditingVGrid.DataSource, EditingVGrid.DataMember);
		}
		protected override DataTable CreateDataTableAdapter() {
			if(EditingVGrid.DataSource == null)  return null;
			try {
				CurrencyManager manager = EditingVGrid.BindingContext[EditingVGrid.DataSource, EditingVGrid.DataMember] as CurrencyManager;
				if(manager == null) return null;
				DataView dv = manager.List as DataView;
				if(dv != null) return dv.Table;
			}
			catch {
			}
			return null;
		}
		protected override void OnFillGrid() {
			grid.FullExpandRow(null);
		}
		void InitVGrid() {
			ChangeColumnSelectorButtonVisibility(grid != null);
			if(grid != null) {
				grid.ShowCustomizationForm += new System.EventHandler(this.grid_ShowCustomizationForm);
				grid.HideCustomizationForm += new System.EventHandler(this.grid_HideCustomizationForm);
			}
		}
		void grid_ShowCustomizationForm(object sender, System.EventArgs e) {
			OnShowCustomizationForm();
		}
		void grid_HideCustomizationForm(object sender, System.EventArgs e) {
			OnHideCustomizationForm();
		}
		void grid_Layout(object sender, EventArgs e) {
			SetLayoutChanged(true);
		}
		protected override void SetLayoutChanged(bool enabled) {
			btnApply.Enabled = true;
		}
		protected override void SetColumnSelectorText(bool showing) {
			SetColumnSelectorCaption(showing ? "Hide rows &selector" : "Show rows &selector");	
		}
	}
}
