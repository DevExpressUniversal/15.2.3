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

using DevExpress.XtraBars.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Tile;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Design.Tile {
	class TileTemplateDesignerControl : TileItemElementsCollectionEditorControl {
		public Action OnViewChanged { get; set; }
		protected override bool NeedCreateTilePreview {
			get { return false; }
		}
		protected override bool ControlPanelIsHorizontal {
			get { return false; }
		}
		protected override bool ShowGridSearchPanel {
			get { return true; }
		}
		protected override XtraEditors.TileItemElement CreateTileItemElement() {
			return new TileViewItemElement();
		}
		protected override void OnChanged() {
			base.OnChanged();
			if(OnViewChanged != null)
				OnViewChanged.Invoke();
		}
		public TileItemElementCollection GetValue() {
			return Elements;
		}
		TileViewColumnListControl columnsControl;
		TileViewColumnListControl ColumnsControl {
			get {
				if(columnsControl == null) columnsControl = CreateColumnsControl();
				return columnsControl;
			}
		}
		private TileViewColumnListControl CreateColumnsControl() {
			var res = new TileViewColumnListControl();
			res.Dock = DockStyle.Fill;
			res.btnCreateColumnElement.Click += btnCreateColumnElement_Click;
			res.listBox.MouseDoubleClick += listBox_MouseDoubleClick;
			return res;
		}
		void listBox_MouseDoubleClick(object sender, MouseEventArgs e) {
			AddColumnElement();
		}
		void AddColumnElement() {
			var selectedColumn = (ListBoxItem)ColumnsControl.listBox.SelectedItem;
			if(selectedColumn == null || selectedColumn.Value == null) return;
			var columnElement = selectedColumn.Value as ColumnElement;
			var column = columnElement.Column;
			AddNewItem();
			RefreshBtnsState();
			var element = GetLastElement() as TileViewItemElement;
			if(element != null) 
				element.Column = column;
			OnChanged();
		}
		void btnCreateColumnElement_Click(object sender, EventArgs e) {
			AddColumnElement();
		}
		protected override void UpdateUI() {
			base.UpdateUI();
			panelControl3.Visible = false;
			PanelControl bufferPanel = new PanelControl();
			bufferPanel.Size = splitContainerControl2.Panel1.Size;
			bufferPanel.BorderStyle = BorderStyles.NoBorder;
			bufferPanel.Controls.Add(panelControl1);
			bufferPanel.Controls.Add(panelControl2);
			var padding1 = panelControl1.Padding;
			panelControl1.Padding = new Padding(0, padding1.Top, 0, padding1.Bottom);
			var padding2 = panelControl2.Padding;
			panelControl2.Padding = new Padding(0, padding2.Top, 2, padding2.Bottom);
			bufferPanel.Dock = DockStyle.Right;
			bufferPanel.Width = 220;
			PanelControl resultPanel = new PanelControl();
			resultPanel.Dock = DockStyle.Fill;
			resultPanel.BorderStyle = BorderStyles.NoBorder;
			resultPanel.Controls.Add(ColumnsControl);
			resultPanel.Controls.Add(bufferPanel);
			splitContainerControl2.Panel1.Controls.Clear();
			splitContainerControl2.Panel1.Controls.Add(resultPanel);
			splitContainerControl2.Panel1.MinSize = 200;
			splitContainerControl2.Panel2.Padding = splitContainerControl2.Panel1.Padding;
			int btnWidth = (panelControl2.Width / 2) - 4;
			btnAddElement.Width = btnWidth;
			btnAddElement.Location = new Point(0, btnAddElement.Location.Y);
			btnRemoveElement.Width = btnWidth;
			btnRemoveElement.Location = new Point(btnAddElement.Right + 8, btnRemoveElement.Location.Y);
		}
		public void PopulateColumns(GridColumnCollection columns) {
			foreach(TileViewColumn col in columns) { 
				ColumnsControl.listBox.Items.Add(new ColumnElement(col));
			}
		}
		class ColumnElement {
			public ColumnElement(TileViewColumn column) {
				this.column = column;
			}
			TileViewColumn column;
			public TileViewColumn Column { get { return column; } }
			public override string ToString() {
				if(Column == null) return "null";
				return Column.ToString();
			}
		}
	}
}
