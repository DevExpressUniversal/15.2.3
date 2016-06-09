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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	public interface ISupportInnerValuesItem {
		void ClearInnerValues();
	}
	[UserRepositoryItem("RegisterDashboardSparklineEdit")]
	public class DashboardSparklineEdit : RepositoryItemSparklineEdit, ISupportInnerValuesItem, IDashboardRepositoryItem {
		public const string DashboardSparklineEditName = "DashboardRepositoryItemSparklineEdit";
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		static DashboardSparklineEdit() {
			RegisterDashboardSparklineEdit();
		}
		public static void RegisterDashboardSparklineEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(DashboardSparklineEditName,
			  typeof(SparklineEdit), typeof(RepositoryItemSparklineEdit),
			  typeof(DashboardSparklineEditViewInfo), new DashboardSparklineEditPainter(), true));
		}
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		readonly GridColumn sparklineColumn;
		DashboardSparklineCalculator sparklineCalculator;
		public DashboardSparklineCalculator SparklineCalculator { get { return sparklineCalculator; } }
		public override string EditorTypeName { get { return DashboardSparklineEditName; } }
		public event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public DashboardSparklineEdit(GridDashboardView gridView, GridColumnViewModel viewModel, GridColumn sparklineColumn)
			: base() {
			this.gridView = gridView;
			this.columnViewModel = viewModel;
			this.sparklineColumn = sparklineColumn;
			SparklineViewBase view = null;
			switch(viewModel.SparklineOptions.ViewType) {
				case DashboardCommon.SparklineViewType.Area:
					view = new AreaSparklineView();
					break;
				case DashboardCommon.SparklineViewType.Bar:
					view = new BarSparklineView();
					break;
				case DashboardCommon.SparklineViewType.WinLoss:
					view = new WinLossSparklineView();
					break;
				default:
					view = new LineSparklineView();
					break;
			}
			View = view;
			view.HighlightMinPoint = view.HighlightMaxPoint = viewModel.SparklineOptions.HighlightMinMaxPoints;
			view.HighlightStartPoint = view.HighlightEndPoint = viewModel.SparklineOptions.HighlightStartEndPoints;
		}
		public void ClearInnerValues() {
			if(sparklineCalculator != null)
				sparklineCalculator.Clear();
		}
		public void PrepareData() {
			if(sparklineCalculator != null)
				return;
			List<double> data = new List<double>();
			List<string> texts = new List<string>();
			for(int i = 0; i < gridView.RowCount; i++) {
				try {
					double startValue = Helper.ConvertToDouble(gridView.GetRowCellValue(gridView.GetRowHandle(i), sparklineColumn.FieldName + GridMultiDimensionalDataSource.SparklineStartValue));
					data.Add(startValue);
					double endValue = Helper.ConvertToDouble(gridView.GetRowCellValue(gridView.GetRowHandle(i), sparklineColumn.FieldName + GridMultiDimensionalDataSource.SparklineEndValue));
					data.Add(endValue);
					string startText = gridView.GetRowCellValue(gridView.GetRowHandle(i), sparklineColumn.FieldName + GridMultiDimensionalDataSource.SparklineStartDisplayText) as string;
					if(startText != null)
						texts.Add(startText);
					string endText = gridView.GetRowCellValue(gridView.GetRowHandle(i), sparklineColumn.FieldName + GridMultiDimensionalDataSource.SparklineEndDisplayText) as string;
					if(endText != null)
						texts.Add(endText);
				}
				catch {
				}
			}
			sparklineCalculator = new DashboardSparklineCalculator(data, texts, columnViewModel.ShowStartEndValues);
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new DashboardSparklineEditViewInfo(this, gridView, columnViewModel);
		}
		public override BaseEditPainter CreatePainter() {
			return new DashboardSparklineEditPainter();
		}
		public void OnCustomDrawCell(GridCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(this, e);
		}
	}
	public class DashboardSparklineEditViewInfo : SparklineEditViewInfo {
		readonly DashboardSparklineEdit sparklineEdit;
		Rectangle bounds = Rectangle.Empty;
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		public DashboardSparklineEdit SparklineEdit { get { return sparklineEdit; } }
		public IList<double> Values { get { return ((ISparklineData)this).Values; } }
		public override Rectangle ClientRect { get { return bounds != Rectangle.Empty ? bounds : base.ClientRect; } }
		public string ColumnId { get { return columnViewModel.DataId; } }
		public DashboardSparklineEditViewInfo(DashboardSparklineEdit sparklineEdit, GridDashboardView gridView, GridColumnViewModel columnViewModel)
			: base(sparklineEdit) {
			this.sparklineEdit = sparklineEdit;
			this.gridView = gridView;
			this.columnViewModel = columnViewModel;
		}
		public void SetSparklineBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		public bool IsSelectedRow(int rowIndex) {
			return gridView.IsSelectedRow(rowIndex);
		}
	}
	public class DashboardSparklineEditPainter : SparklineEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DashboardSparklineEditViewInfo viewInfo = (DashboardSparklineEditViewInfo)info.ViewInfo;
			DashboardSparklineEdit item = (DashboardSparklineEdit)viewInfo.Item;
			GridCellInfo cellInfo = (GridCellInfo)(((BaseEditViewInfo)(viewInfo)).Tag);
			AppearanceObject cellAppearance = cellInfo.Appearance;
			bool selectedRow = viewInfo.IsSelectedRow(cellInfo.RowHandle);
			AppearanceObject paintAppearance = viewInfo.PaintAppearance;
			IList<double> values = viewInfo.Values;
			if(values != null && values.Count > 0) {
				double start = values.FirstOrDefault();
				double end = values.LastOrDefault();
				viewInfo.SparklineEdit.PrepareData();
				DashboardSparklineCalculator calculator = viewInfo.SparklineEdit.SparklineCalculator;
				Rectangle bounds = info.Bounds;
				GridCustomDrawCellEventArgs args = new GridCustomDrawCellEventArgs(cellAppearance, viewInfo.ColumnId, cellInfo.RowHandle, DashboardWinHelper.IsDarkScheme(viewInfo.LookAndFeel), selectedRow, StyleSettingsContainerPainter.GetDefaultBackColor(viewInfo.LookAndFeel));
				item.OnCustomDrawCell(args);
				viewInfo.PaintAppearance.FillRectangle(info.Cache, cellInfo.Bounds);
				if(calculator.ShowStartEndValues) {
					Font font = cellAppearance.Font;
					Graphics graphics = info.Graphics;
					paintAppearance.ForeColor = cellAppearance.ForeColor;
					string startValueString = calculator.GetValueString(start, graphics, font);
					Rectangle startStringBounds = calculator.GetStartValueBounds(start, bounds, graphics, font);
					paintAppearance.DrawString(info.Cache, startValueString, startStringBounds);
					string endValueString = calculator.GetValueString(end, graphics, font);
					Rectangle endStringBounds = calculator.GetEndValueBounds(end, bounds, graphics, font);
					viewInfo.PaintAppearance.DrawString(info.Cache, endValueString, endStringBounds);
					viewInfo.SetSparklineBounds(calculator.GetSparklineStartEndBounds(bounds, graphics, font));
				}
			}
			base.DrawContent(info);
		}
	}
}
