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
using DevExpress.XtraTreeList;
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)]
	public class Layouts : LayoutsBase {
		public Layouts()
			: base(6) {
			this.SizeChanged += new EventHandler(this.Layouts_Paint);
		}
		private void Layouts_Paint(object sender, EventArgs e) {
			SetLayoutChanged(false);
		}
		protected override string PreviewPanelText { get { return "TreeList Preview"; } }
		protected override string DescriptionText { get { return "Modify the treelist layout (sorting and grouping settings, column arrangement) and click the Apply button to apply the modifications to the current treelist. You can also save the layout to an XML file (this can be loaded and applied to other treelists at design time and runtime)."; } }
		public TreeList EditingTreeList { get { return EditingObject as TreeList; } }
		TreeList tlPreview = null;
		protected override Control CreatePreviewControl() {
			tlPreview = DevExpress.XtraTreeList.Design.TreeListAssign.CreateTreeListControlAssign(EditingTreeList);
			if(dataSet != null)
				tlPreview.DataSource = dataSet.Tables[tableName];
			else
				tlPreview.DataSource = EditingTreeList.DataSource;
			tlPreview.ForceInitialize();
			InitTreeList();
			tlPreview.LayoutUpdated += new EventHandler(this.treeList_LayoutUpdate);
			return tlPreview;
		}
		protected override void ShowColumnsCustomization() {
			tlPreview.ColumnsCustomization();
		}
		protected override void HideColumnsCustomization() {
			tlPreview.DestroyCustomization();
		}
		protected override void ApplyLayouts() {
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			tlPreview.SaveLayoutToStream(str);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			EditingTreeList.RestoreLayoutFromStream(str);
			str.Close();
		}
		protected override void RestoreLayoutFromXml(string fileName) {
			tlPreview.RestoreLayoutFromXml(fileName);
		}
		protected override void SaveLayoutToXml(string fileName) {
			try {
			tlPreview.SaveLayoutToXml(fileName);
		}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected override void SetControlDataSource(DataView dataView) {
			tlPreview.BeginUpdate();
			try {
				tlPreview.DataSource = null;
				if(EditingTreeList.Columns.Count == 0 && tlPreview.Columns.Count > 0)
					tlPreview.Columns.Clear();
				tlPreview.DataSource = dataView;
				if(EditingTreeList.Columns.Count == 0)
					tlPreview.PopulateColumns();
			} finally {
				tlPreview.EndUpdate();
			}
		}
		protected override DBAdapter CreateDBAdapter() {
			ArrayList adapters = new ArrayList();
			foreach(object comp in EditingTreeList.Container.Components)
				adapters.Add(comp);
			return new DBAdapter(adapters, EditingTreeList.DataSource, EditingTreeList.DataMember);
		}
		protected override DataTable CreateDataTableAdapter() {
			if(EditingTreeList.DataSource == null)
				return null;
			try {
				CurrencyManager manager = EditingTreeList.BindingContext[EditingTreeList.DataSource, EditingTreeList.DataMember] as CurrencyManager;
				if(manager == null)
					return null;
				DataView dv = manager.List as DataView;
				if(dv != null)
					return dv.Table;
			} catch {
			}
			return null;
		}
		protected override void OnFillGrid() {
			tlPreview.ExpandAll();
		}
		void InitTreeList() {
			ChangeColumnSelectorButtonVisibility(tlPreview != null);
			if(tlPreview != null) {
				tlPreview.ShowCustomizationForm += new System.EventHandler(this.treeList_ShowCustomizationForm);
				tlPreview.HideCustomizationForm += new System.EventHandler(this.treeList_HideCustomizationForm);
			}
		}
		private void treeList_ShowCustomizationForm(object sender, System.EventArgs e) {
			OnShowCustomizationForm();
		}
		private void treeList_HideCustomizationForm(object sender, System.EventArgs e) {
			OnHideCustomizationForm();
		}
		private void treeList_LayoutUpdate(object sender, System.EventArgs e) {
			SetLayoutChanged(true);
		}
		protected override void InitUnboundData() {
			if(!EditingTreeList.IsUnboundMode)
				return;
			tlPreview.BeginUnboundLoad();
			DevExpress.XtraTreeList.Design.xNodesEditor.AddNodes(EditingTreeList.Nodes, tlPreview.Nodes, null);
			tlPreview.EndUnboundLoad();
		}
		protected override void SetUnboundData() {
			if(!EditingTreeList.IsUnboundMode)
				return;
			EditingTreeList.ClearNodes();
			EditingTreeList.BeginUnboundLoad();
			DevExpress.XtraTreeList.Design.xNodesEditor.AddNodes(tlPreview.Nodes, EditingTreeList.Nodes, null);
			EditingTreeList.EndUnboundLoad();
			EditingTreeList.FireChanged();
		}
		public override void EndInitialize() {
			SetLayoutChanged(false);
		}
		protected override void SetColumnSelectorText(bool showing) {
			SetColumnSelectorCaption(showing ? "&Hide Column/Band Selector" : "&Show Column/Band Selector");	
		}
	}
}
