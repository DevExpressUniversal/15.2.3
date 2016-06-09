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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	public interface IDashboardRepositoryItem {
		event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell;
	}
	[UserRepositoryItem("RegisterDashboardGridRepositoryItemTextEdit")]
	public class DashboardGridRepositoryItemTextEdit : RepositoryItemTextEdit, IDashboardRepositoryItem {
		static DashboardGridRepositoryItemTextEdit() {
			RegisterDashboardGridRepositoryItemTextEdit();
		}
		public static void RegisterDashboardGridRepositoryItemTextEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(TypeName,
			  typeof(TextEdit), typeof(DashboardGridRepositoryItemTextEdit),
			  typeof(DashboardGridTextEditViewInfo), new DashboardGridTextEditPainter(), true));
		}
		const string TypeName = "DashboardGridRepositoryItemTextEdit";
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		readonly GridViewerDataController dataController;
		readonly GridDashboardColumn gridColumn;
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		event EventHandler lookAndFeelChanged;
		public override string EditorTypeName { get { return TypeName; } }
		public event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public event EventHandler LookAndFeelChanged {
			add { lookAndFeelChanged += value; }
			remove { lookAndFeelChanged -= value; }
		}
		protected override EditorClassInfo EditorClassInfo { get { return EditorRegistrationInfo.Default.Editors["TextEdit"]; } }
		public DashboardGridRepositoryItemTextEdit(GridDashboardView gridView, GridDashboardColumn gridColumn, GridColumnViewModel columnViewModel, GridViewerDataController dataController)
			: base() {
			this.gridView = gridView;
			this.columnViewModel = columnViewModel;
			this.dataController = dataController;
			this.gridColumn = gridColumn;
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new DashboardGridTextEditViewInfo(this, gridView, gridColumn, columnViewModel, dataController);
		}
		public override BaseEditPainter CreatePainter() {
			return new DashboardGridTextEditPainter();
		}
		public void OnCustomDrawCell(GridCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(this, e);
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			if(lookAndFeelChanged != null)
				lookAndFeelChanged(this, EventArgs.Empty);
		}
	}
	public class DashboardGridTextEditViewInfo : TextEditViewInfo {
		readonly GridDashboardView gridView;
		readonly GridColumnViewModel columnViewModel;
		readonly GridViewerDataController dataController;
		readonly GridDashboardColumn gridColumn;
		GridDeltaInfo deltaInfo;
		Color barColor;
		public bool IgnoreDeltaIndication { get { return columnViewModel.IgnoreDeltaIndication; } }
		public string ColumnId { get { return columnViewModel.DataId; } }
		public bool BarDisplayMode { get { return columnViewModel.DisplayMode == GridColumnDisplayMode.Bar; } }
		public bool DeltaDisplayMode { get { return columnViewModel.DisplayMode == GridColumnDisplayMode.Delta; } }
		public Color BarColor { get { return barColor; } }
		public bool TextIsHidden { get { return gridColumn.TextIsHidden; } set { gridColumn.TextIsHidden = value; } }
		public DashboardGridTextEditViewInfo(DashboardGridRepositoryItemTextEdit editor, GridDashboardView gridView, GridDashboardColumn gridColumn, GridColumnViewModel columnViewModel, GridViewerDataController dataController)
			: base(editor) {
			this.gridView = gridView;
			this.gridColumn = gridColumn;
			this.columnViewModel = columnViewModel;
			this.dataController = dataController;
			SetColors();
		}
		public Image GetDeltaImage(int rowIndex) {
			DeltaValue delta = GetDeltaValue(rowIndex);
			bool deltaIsGood = delta != null && delta.IsGood;
			IndicatorType? indicatorType = delta != null ? (IndicatorType?)delta.IndicatorType : null;
			return deltaInfo.GetImage(indicatorType, deltaIsGood);
		}
		public Rectangle GetTextBounds(Rectangle maskBoxRect, string text, Font font, Graphics graphics, ref bool contentWidthLessThanCellWidth) {
			int textWidth = Convert.ToInt32(graphics.MeasureString(text, font).Width);
			if(maskBoxRect.Width <= textWidth)
				return maskBoxRect;
			contentWidthLessThanCellWidth = maskBoxRect.Width > textWidth + GridDeltaInfo.ImageWidth;
			int xCoord = maskBoxRect.Right - textWidth;
			if(!IgnoreDeltaIndication && contentWidthLessThanCellWidth)
				xCoord -= GridDeltaInfo.ImageWidth;
			return new Rectangle(xCoord, maskBoxRect.Y, textWidth, maskBoxRect.Height);
		}
		public Color GetDeltaForeColor(AppearanceObject appearance, int rowIndex, ObjectState cellState) {
			DeltaValue delta = GetDeltaValue(rowIndex);
			bool deltaIsGood = delta != null && delta.IsGood;
			IndicatorType? indicatorType = delta != null ? (IndicatorType?)delta.IndicatorType : null;
			return deltaInfo.GetTextColor(indicatorType, deltaIsGood, columnViewModel.IgnoreDeltaColor, appearance, cellState);
		}
		DeltaValue GetDeltaValue(int rowIndex) {
			return gridView.GetListSourceRowCellValue(rowIndex, ColumnId + GridMultiDimensionalDataSource.DeltaDescriptorPostFix) as DeltaValue;
		}
		public bool IsSelectedRow(int rowIndex) {
			return gridView.IsSelectedRow(rowIndex);
		}
		public int GetDataSourceRowIndex(int index) {
			return gridView.GetDataSourceRowIndex(index);
		}
		public decimal NormalizeBarValue(string columnId, object value) {
			return dataController.NormalizeValue(columnId, value);
		}
		public decimal GetBarZeroPosition(string columnId) {
			return dataController.GetZeroPosition(columnId);
		}
		void SetColors() {
			GridDeltaColorsGetter deltaColorsGetter = new GridDeltaColorsGetter(LookAndFeel);
			barColor = deltaColorsGetter.BarColor;
			deltaInfo = new GridDeltaInfo(deltaColorsGetter);
		}
	}
	public class DashboardGridTextEditPainter : TextEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DashboardGridTextEditViewInfo viewInfo = (DashboardGridTextEditViewInfo)info.ViewInfo;
			DashboardGridRepositoryItemTextEdit item = (DashboardGridRepositoryItemTextEdit)viewInfo.Item;
			GridCellInfo cellInfo = (GridCellInfo)(((BaseEditViewInfo)(viewInfo)).Tag);
			GraphicsCache cache = info.Cache;
			Rectangle cellBounds = cellInfo.Bounds;
			int dataSourceRowIndex = viewInfo.GetDataSourceRowIndex(cellInfo.RowHandle);
			GridCustomDrawCellEventArgs args = CreateCustomDrawCellEventArgs(viewInfo, cellInfo, dataSourceRowIndex);
			item.OnCustomDrawCell(args);
			StringFormat format = viewInfo.PaintAppearance.GetStringFormat();
			viewInfo.PaintAppearance.FillRectangle(cache, cellBounds);
			StyleSettingsInfo styleSettings = args.StyleSettings;
			if(viewInfo.DeltaDisplayMode)
				DrawDelta(viewInfo, dataSourceRowIndex, format, cache);
			else if(viewInfo.BarDisplayMode) {
				decimal normalizedValue = viewInfo.NormalizeBarValue(viewInfo.ColumnId, viewInfo.EditValue);
				decimal zeroPosition = viewInfo.GetBarZeroPosition(viewInfo.ColumnId);
				BarPainter.DrawGridColumnBar(cellBounds, normalizedValue, zeroPosition, viewInfo.BarColor, cache, true);
			}
			else if(styleSettings.Bar != null)
				DashboardCellPainter.DrawContentWithBar(viewInfo, format, cache, styleSettings.Bar, cellBounds);
			else if(styleSettings.Image != null)
				DashboardCellPainter.DrawContentWithIcon(viewInfo, cache, format, styleSettings, cellBounds);
			else
				base.DrawContent(info);
		}
		void DrawDelta(DashboardGridTextEditViewInfo viewInfo, int rowHandle, StringFormat format, GraphicsCache cache) {
			bool contentWidthLessThanCellWidth = false;
			format.Trimming = StringTrimming.EllipsisCharacter;
			Image image = viewInfo.GetDeltaImage(rowHandle);
			Rectangle maskBoxRect = viewInfo.MaskBoxRect;
			Rectangle textBounds = viewInfo.GetTextBounds(maskBoxRect, viewInfo.DisplayText, viewInfo.PaintAppearance.Font, cache.Graphics, ref contentWidthLessThanCellWidth);
			viewInfo.PaintAppearance.DrawString(cache, viewInfo.DisplayText, textBounds, format);
			if(image != null && contentWidthLessThanCellWidth) {
				int imageYCord = maskBoxRect.Y + (maskBoxRect.Height - GridDeltaInfo.ImageHeight) / 2;
				cache.Graphics.DrawImage(image, new Point(maskBoxRect.Right - GridDeltaInfo.ImageWidth, imageYCord));
			}
		}
		GridCustomDrawCellEventArgs CreateCustomDrawCellEventArgs(DashboardGridTextEditViewInfo viewInfo, GridCellInfo cellInfo, int dataSourceRowIndex) {
			ObjectState cellState = (cellInfo != null && cellInfo.State == GridRowCellState.Selected) ? ObjectState.Selected : ObjectState.Normal;
			bool selectedRow = viewInfo.IsSelectedRow(dataSourceRowIndex);
			if(viewInfo.DeltaDisplayMode)
				viewInfo.PaintAppearance.ForeColor = viewInfo.GetDeltaForeColor(viewInfo.PaintAppearance, dataSourceRowIndex, cellState);
			return new GridCustomDrawCellEventArgs(viewInfo.PaintAppearance, viewInfo.ColumnId, dataSourceRowIndex, DashboardWinHelper.IsDarkScheme(viewInfo.LookAndFeel), selectedRow, StyleSettingsContainerPainter.GetDefaultBackColor(viewInfo.LookAndFeel));
		}
	}
}
