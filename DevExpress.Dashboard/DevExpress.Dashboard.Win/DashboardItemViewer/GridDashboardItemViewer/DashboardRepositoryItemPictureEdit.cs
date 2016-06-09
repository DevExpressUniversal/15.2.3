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
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[UserRepositoryItem("RegisterDashboardGridRepositoryItemPictureEdit")]
	public class DashboardRepositoryItemPictureEdit : RepositoryItemPictureEdit, IDashboardRepositoryItem {
		static DashboardRepositoryItemPictureEdit() {
			RegisterDashboardGridRepositoryItemPictureEdit();
		}
		public static void RegisterDashboardGridRepositoryItemPictureEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(TypeName,
			  typeof(PictureEdit), typeof(DashboardRepositoryItemPictureEdit),
			  typeof(DashboardPictureEditViewInfo), new DashboardPictureEditPainter(), true));
		}
		const string TypeName = "DashboardRepositoryItemPictureEdit";
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		public override string EditorTypeName { get { return TypeName; } }
		public event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public DashboardRepositoryItemPictureEdit(GridDashboardView gridView, GridColumnViewModel columnViewModel)
			: base() {
			this.gridView = gridView;
			this.columnViewModel = columnViewModel;
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new DashboardPictureEditViewInfo(this, gridView, columnViewModel);
		}
		public override BaseEditPainter CreatePainter() {
			return new DashboardPictureEditPainter();
		}
		public void OnCustomDrawCell(GridCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(this, e);
		}
	}
	public class DashboardPictureEditViewInfo : PictureEditViewInfo {
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		public string ColumnId { get { return columnViewModel.DataId; } }
		public DashboardPictureEditViewInfo(DashboardRepositoryItemPictureEdit editor, GridDashboardView gridView, GridColumnViewModel columnViewModel)
			: base(editor) {
			this.gridView = gridView;
			this.columnViewModel = columnViewModel;
		}
		public bool IsSelectedRow(int rowIndex) {
			return gridView.IsSelectedRow(rowIndex);
		}
	}
	public class DashboardPictureEditPainter : PictureEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DashboardPictureEditViewInfo viewInfo = (DashboardPictureEditViewInfo)info.ViewInfo;
			DashboardRepositoryItemPictureEdit item = (DashboardRepositoryItemPictureEdit)viewInfo.Item;
			GridCellInfo cellInfo = (GridCellInfo)(((BaseEditViewInfo)(viewInfo)).Tag);
			AppearanceObject cellAppearance = cellInfo.Appearance;
			bool selectedRow = viewInfo.IsSelectedRow(cellInfo.RowHandle);
			GridCustomDrawCellEventArgs args = new GridCustomDrawCellEventArgs(cellAppearance, viewInfo.ColumnId, cellInfo.RowHandle, DashboardWinHelper.IsDarkScheme(viewInfo.LookAndFeel), selectedRow, StyleSettingsContainerPainter.GetDefaultBackColor(viewInfo.LookAndFeel));
			item.OnCustomDrawCell(args);
			viewInfo.PaintAppearance.FillRectangle(info.Cache, cellInfo.Bounds);
			base.DrawContent(info);
		}
	}
}
