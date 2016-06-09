#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[UserRepositoryItem("RegisterDashboardGridRepositoryItemMemoEdit")]
	public class DashboardGridRepositoryItemMemoEdit : RepositoryItemMemoEdit, IDashboardRepositoryItem {
		static DashboardGridRepositoryItemMemoEdit() {
			RegisterDashboardGridRepositoryItemMemoEdit();
		}
		public static void RegisterDashboardGridRepositoryItemMemoEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(TypeName,
			  typeof(MemoEdit), typeof(DashboardGridRepositoryItemMemoEdit),
			  typeof(DashboardGridMemoEditViewInfo), new DashboardGridMemoEditPainter(), true));
		}
		const string TypeName = "DashboardGridRepositoryItemMemoEdit";
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		readonly GridDashboardColumn gridColumn;
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		public override string EditorTypeName { get { return TypeName; } }
		public string ColumnId { get; private set; }
		public event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public DashboardGridRepositoryItemMemoEdit(GridDashboardView gridView, GridDashboardColumn gridColumn, GridColumnViewModel columnViewModel, string columnId)
			: base() {
			this.gridView = gridView;
			this.columnViewModel = columnViewModel;
			this.gridColumn = gridColumn;
			ColumnId = columnId;
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new DashboardGridMemoEditViewInfo(this, gridView, gridColumn, columnViewModel);
		}
		public override BaseEditPainter CreatePainter() {
			return new DashboardGridMemoEditPainter();
		}
		public void OnCustomDrawCell(GridCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(this, e);
		}
	}
	public class DashboardGridMemoEditViewInfo : MemoEditViewInfo {
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		readonly GridDashboardColumn gridColumn;
		public string ColumnId { get { return columnViewModel.DataId; } }
		public bool TextIsHidden { get { return gridColumn.TextIsHidden; } set { gridColumn.TextIsHidden = value; } }
		public DashboardGridMemoEditViewInfo(DashboardGridRepositoryItemMemoEdit editor, GridDashboardView gridView, GridDashboardColumn gridColumn, GridColumnViewModel columnViewModel)
			: base(editor) {
			this.gridView = gridView;
			this.columnViewModel = columnViewModel;
			this.gridColumn = gridColumn;
		}
		public bool IsSelectedRow(int rowIndex) {
			return gridView.IsSelectedRow(rowIndex);
		}
	}
	public class DashboardGridMemoEditPainter : MemoEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DashboardGridMemoEditViewInfo viewInfo = (DashboardGridMemoEditViewInfo)info.ViewInfo;
			DashboardGridRepositoryItemMemoEdit item = (DashboardGridRepositoryItemMemoEdit)viewInfo.Item;
			GridCellInfo cellInfo = (GridCellInfo)(((BaseEditViewInfo)(viewInfo)).Tag);
			AppearanceObject cellAppearance = cellInfo.Appearance;
			GraphicsCache cache = info.Cache;
			bool selectedRow = viewInfo.IsSelectedRow(cellInfo.RowHandle);
			Rectangle cellBounds = cellInfo.Bounds;
			GridCustomDrawCellEventArgs args = new GridCustomDrawCellEventArgs(cellAppearance, viewInfo.ColumnId, cellInfo.RowHandle, DashboardWinHelper.IsDarkScheme(viewInfo.LookAndFeel), selectedRow, StyleSettingsContainerPainter.GetDefaultBackColor(viewInfo.LookAndFeel));
			item.OnCustomDrawCell(args);
			StyleSettingsInfo styleSettings = args.StyleSettings;
			StringFormat format = viewInfo.PaintAppearance.GetStringFormat();
			viewInfo.PaintAppearance.FillRectangle(info.Cache, cellInfo.Bounds);
			if(styleSettings.Bar != null) {
				viewInfo.TextIsHidden = styleSettings.Bar.ShowBarOnly;
				DashboardCellPainter.DrawContentWithBar(viewInfo, format, cache, styleSettings.Bar, cellBounds);
			}
			else if(styleSettings.Image != null)
				DashboardCellPainter.DrawContentWithIcon(viewInfo, cache, format, styleSettings, cellBounds);
			else
				base.DrawContent(info);
		}
	}
}
