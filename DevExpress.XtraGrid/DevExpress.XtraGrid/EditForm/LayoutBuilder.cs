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

using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.EditForm.Layout;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.XtraGrid.EditForm {
	public class LayoutBuilder {
		GridView view;
		public LayoutBuilder(GridView source) {
			this.view = source;
		}
		protected virtual LayoutControlGeneratorCore CreateGenerator(EditFormOwner owner) {
			return new LayoutTableControlGenerator(owner);
		}
		public EditFormOwner GenerateOwner() {
			EditFormOwner owner = new EditFormOwner();
			owner.ElementsLookAndFeel = view.ElementsLookAndFeel;
			owner.MenuManager = view.GridControl.MenuManager;
			owner.EditFormColumnCount = view.OptionsEditForm.EditFormColumnCount;
			owner.AllowHtmlCaptions = view.OptionsView.AllowHtmlDrawHeaders;
			foreach(GridColumn col in view.Columns) {
				EditFormColumn edColumn = new EditFormColumn();
				edColumn.StartNewRow = col.OptionsEditForm.StartNewRow;
				edColumn.UseEditorColRowSpan = col.OptionsEditForm.UseEditorColRowSpan;
				edColumn.FieldName = col.FieldName;
				edColumn.GlobalVisibleIndex = col.AbsoluteIndex;
				edColumn.ColumnVisible = col.Visible;
				edColumn.ReadOnly = col.ReadOnly;
				edColumn.RepositoryItem = view.RequestCellEditor(new Views.Grid.ViewInfo.GridCellInfo() { ColumnInfo = new Drawing.GridColumnInfoArgs(col), Editor = col.RealColumnEdit, RowInfo = new Views.Grid.ViewInfo.GridDataRowInfo(view.ViewInfo, GridControl.InvalidRowHandle, 0) });
				edColumn.VisibleIndex = col.OptionsEditForm.VisibleIndex;
				edColumn.Caption = col.OptionsEditForm.Caption;
				edColumn.Visible = col.OptionsEditForm.Visible;
				edColumn.RowSpan = col.OptionsEditForm.RowSpan;
				edColumn.ColumnSpan = col.OptionsEditForm.ColumnSpan;
				edColumn.CaptionLocation = col.OptionsEditForm.CaptionLocation;
				if(string.IsNullOrEmpty(edColumn.Caption)) edColumn.Caption = col.GetCaption() + ":";
				edColumn.ResetChanged();
				owner.Columns.Add(edColumn);
			}
			return owner;
		}
		public virtual void AssignToView(EditFormOwner owner, GridView view) {
			view.OptionsEditForm.EditFormColumnCount = owner.EditFormColumnCount;
			view.OptionsView.AllowHtmlDrawHeaders = owner.AllowHtmlCaptions;
			foreach(EditFormColumn edColumn in owner.Columns) {
				GridColumn col = view.Columns[edColumn.FieldName];
				if(col == null) continue;
				col.OptionsEditForm.StartNewRow = edColumn.StartNewRow;
				col.OptionsEditForm.UseEditorColRowSpan = edColumn.UseEditorColRowSpan;
				col.OptionsEditForm.VisibleIndex = edColumn.VisibleIndex;
				col.OptionsEditForm.Visible = edColumn.Visible;
				col.OptionsEditForm.RowSpan = edColumn.RowSpan;
				col.OptionsEditForm.ColumnSpan = edColumn.ColumnSpan;
				col.OptionsEditForm.CaptionLocation = edColumn.CaptionLocation;
				if(edColumn.captionChanged) {
					col.OptionsEditForm.Caption = edColumn.Caption == col.GetCaption() + ":" ? "" : edColumn.Caption;
				}
				edColumn.ResetChanged();
			}
		}
		public Control GenerateLayoutControl() {
			var owner = GenerateOwner();
			var generator = CreateGenerator(owner);
			var control = generator.Generate(owner);
			control.Height = generator.GetHeight(owner, control);
			return control;
		}
	}
	public abstract class LayoutControlGeneratorCore {
		EditFormOwner owner;
		public LayoutControlGeneratorCore(EditFormOwner owner) {
			this.owner = owner;
		}
		protected EditFormOwner Owner { get { return owner; } }
		public abstract int GetHeight(EditFormOwner owner, Control panel);
		protected virtual Control CreateEmptyControl() {
			return null;
		}
		protected virtual Control CreateEditor(GridViewEditFormLayoutItem item) {
			BaseEdit editor = item.Column.RepositoryItem.CreateEditor();
			editor.Properties.Assign(item.Column.RepositoryItem);
			editor.Properties.SetCloned();
			editor.Properties.ReadOnly = item.Column.ReadOnly;
			editor.Width = 10;
			editor.LookAndFeel.ParentLookAndFeel = Owner.ElementsLookAndFeel;
			editor.MenuManager = Owner.MenuManager;
			editor.Height = 10;
			editor.Dock = DockStyle.Fill;
			editor.Tag = "EditValue/" + item.Column.FieldName;
			editor.TabStop = true;
			return editor;
		}
		protected virtual Control CreateCaption(GridViewEditFormLayoutItem item) {
			LabelControl label = new LabelControl();
			label.AllowHtmlString = Owner.AllowHtmlCaptions;
			label.LookAndFeel.ParentLookAndFeel = Owner.ElementsLookAndFeel;
			label.Margin = new Padding(3, 6, 3, 3);
			label.Text = item.Column.Caption;
			label.AutoSize = true;
			return label;
		}
		public abstract Control Generate(EditFormOwner owner);
	}
}
