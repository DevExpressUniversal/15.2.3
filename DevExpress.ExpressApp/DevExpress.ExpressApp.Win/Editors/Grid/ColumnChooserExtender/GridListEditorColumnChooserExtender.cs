#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class GridListEditorColumnChooserExtender : ColumnChooserExtenderBase {
		private GridColumn selectedColumn;
		private GridView gridView;
		private WinColumnsListEditor listEditor;
		private ITypeInfo objectTypeInfo;
		public GridListEditorColumnChooserExtender(IModelListView listViewModel, WinColumnsListEditor listEditor, ITypeInfo objectTypeInfo, GridView gridView)
			: base(listViewModel) {
			Guard.ArgumentNotNull(listEditor, "gridEditor");
			Guard.ArgumentNotNull(gridView, "gridView");
			this.listEditor = listEditor;
			this.gridView = gridView;
			this.objectTypeInfo = objectTypeInfo;
			gridView.ShowCustomizationForm += new EventHandler(gridView_ShowCustomizationForm);
			gridView.HideCustomizationForm += new EventHandler(gridView_HideCustomizationForm);
			gridView.DragObjectDrop += new DragObjectDropEventHandler(gridView_DragObjectDrop);
		}
		protected IModelListView ListViewModel {
			get { return (IModelListView)ObjectViewModel; }
		}
		protected IModelColumn FindColumnModelByPropertyName(string propertyName) {
			IModelColumn columnInfo = null;
			foreach(IModelColumn colInfo in ListViewModel.Columns) {
				if(colInfo.PropertyName == propertyName) {
					columnInfo = colInfo;
					break;
				}
			}
			return columnInfo;
		}
		protected override void DisposeCore() {
			base.DisposeCore();
			if(gridView != null) {
				if(gridView.CustomizationForm != null) { 
					gridView.CustomizationForm.FormClosing -= new FormClosingEventHandler(CustomizationForm_FormClosing);
					if(gridView.CustomizationForm.ActiveListBox != null) {
						gridView.CustomizationForm.ActiveListBox.KeyDown -= new KeyEventHandler(ActiveListBox_KeyDown);
						gridView.CustomizationForm.ActiveListBox.SelectedValueChanged -= new EventHandler(columnChooser_SelectedColumnChanged);
					}
				}
				gridView.ShowCustomizationForm -= new EventHandler(gridView_ShowCustomizationForm);
				gridView.HideCustomizationForm -= new EventHandler(gridView_HideCustomizationForm);
				gridView.DragObjectDrop -= new DragObjectDropEventHandler(gridView_DragObjectDrop);
				gridView = null;
			}
			listEditor = null;
			objectTypeInfo = null;
			selectedColumn = null;
		}
		protected override List<string> GetUsedProperties() {
			List<string> result = new List<string>();
			foreach(IModelColumn columnInfoNodeWrapper in ListViewModel.Columns) {
				result.Add(columnInfoNodeWrapper.PropertyName);
			}
			return result;
		}
		protected override ITypeInfo DisplayedTypeInfo {
			get { return objectTypeInfo; }
		}
		protected override Control ActiveListBox {
			get { return gridView.CustomizationForm.ActiveListBox; }
		}
		protected override void AddColumnCore(string propertyName) {
			IModelColumn columnInfo = FindColumnModelByPropertyName(propertyName);
			if(columnInfo == null) {
				columnInfo = ListViewModel.Columns.AddNode<IModelColumn>();
				columnInfo.Id = propertyName;
				columnInfo.PropertyName = propertyName;
				columnInfo.Index = -1;
				listEditor.AddColumn(columnInfo);
			}
			else {
				throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotAddDuplicateProperty, propertyName));
			}
			gridView.CustomizationForm.CheckAndUpdate();
		}
		protected override void RemoveSelectedColumnCore() {
			if(gridView.CustomizationForm.ActiveListBox.SelectedItem is GridColumn) {
				listEditor.RemoveColumn((GridColumn)gridView.CustomizationForm.ActiveListBox.SelectedItem, gridView as IModelSynchronizersHolder);
			}
		}
		private void CustomizationForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(sender is Form) {
				((Form)sender).Owner = null;
			}
		}
		private void columnChooser_SelectedColumnChanged(object sender, EventArgs e) {
			if(selectedColumn != null && !(selectedColumn is DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn)) {
				selectedColumn.ImageIndex = -1;
			}
			if(gridView.CustomizationForm.ActiveListBox.SelectedItem is GridColumn) {
				selectedColumn = (GridColumn)gridView.CustomizationForm.ActiveListBox.SelectedItem;
				if(selectedColumn != null && !(selectedColumn is DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn)) {
					selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
				}
				RemoveButton.Enabled = selectedColumn != null;
				gridView.CustomizationForm.Refresh();
			}
		}
		private void gridView_ShowCustomizationForm(object sender, EventArgs e) {
			InsertButtons();
			selectedColumn = null;
			gridView.CustomizationForm.FormClosing += new FormClosingEventHandler(CustomizationForm_FormClosing);
			gridView.CustomizationForm.ActiveListBox.SelectedItem = null;
			gridView.CustomizationForm.ActiveListBox.KeyDown += new KeyEventHandler(ActiveListBox_KeyDown);
			gridView.CustomizationForm.ActiveListBox.SelectedValueChanged += new EventHandler(columnChooser_SelectedColumnChanged);
			gridView.Images = GridPainter.Indicator;
		}
		private void ActiveListBox_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				RemoveSelectedColumn();
			}
		}
		private void gridView_HideCustomizationForm(object sender, EventArgs e) {
			DeleteButtons();
			if(selectedColumn != null) {
				selectedColumn.ImageIndex = -1;
			}
			gridView.Images = null;
		}
		private void gridView_DragObjectDrop(object sender, DevExpress.XtraGrid.Views.Base.DragObjectDropEventArgs e) {
			if(selectedColumn != null && gridView.CustomizationForm != null &&
				!(selectedColumn is DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn)) {
				if(e.DragObject is GridColumn) {
					selectedColumn.ImageIndex = -1;
					if(gridView.CustomizationForm.ActiveListBox.Items.Count != 0) {
						selectedColumn = (GridColumn)gridView.CustomizationForm.ActiveListBox.Items[0];
						selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
						gridView.CustomizationForm.ActiveListBox.InvalidateObject(selectedColumn);
						gridView.CustomizationForm.ActiveListBox.Update();
					}
					else {
						selectedColumn = null;
					}
					gridView.CustomizationForm.ActiveListBox.SelectedItem = selectedColumn;
				}
			}
		}
	}
}
