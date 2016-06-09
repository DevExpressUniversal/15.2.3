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
using System.Drawing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
namespace DevExpress.DashboardWin.Native {
	public class GridDashboardViewInfo : GridViewInfo {
		readonly Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
		readonly int elementInterval;
		public const int Padding = 4;
		public GridDashboardViewInfo(GridDashboardView gridView)
			: base(gridView) {
			DrawElementInfo drawElementInfo = new DrawElementInfo(null, null);
			this.elementInterval = drawElementInfo.ElementInterval;
		}
		GridDashboardView DashboardView { get { return View as GridDashboardView; } }
		protected override AutoWidthCalculator CreateWidthCalculator() {
			return new GridDashboardWidthCalculator((GridDashboardView)View);
		}
		public double GetBestWidth(IGridColumn col) {
			GridDashboardColumn gridColumn = col as GridDashboardColumn;
			if(!View.Columns.Contains(gridColumn))
				throw new ArgumentException(string.Format("The view doesn't contains the column {0}.", col.Caption));
			const int offset = 1;
			const int textOffsetCount = 2;
			int maxTotalWidth = CalcColumnSummaryBestWidth(gridColumn, graphics);
			int maxTextWidth = GetBestTextWidth(gridColumn);
			int captionWidth = GetCaptionBestFitWidth(gridColumn);
			int imageWidth = VisualElementAreaWidth(col);
			int maxRowText = maxTextWidth + imageWidth + Padding * textOffsetCount;
			return Math.Max(Math.Max(captionWidth, maxRowText) + offset, maxTotalWidth);
		}
		int GetCaptionBestFitWidth(GridDashboardColumn column) {
			int captionTextWidth = GetTextWidth(column.AppearanceHeader, column.Caption);
			int captionImagesWidths = GetCaptionImageWidths(column);
			return captionTextWidth + captionImagesWidths + elementInterval * 3;
		}
		int GetCaptionImageWidths(GridDashboardColumn column) {
			string realCaption = column.GetCaption();
			GridColumnInfoArgs ci = CreateColumnInfo(column);
			ci.HtmlContext = View;
			ci.Caption = realCaption;
			ci.Type = GridColumnInfoType.Column;
			return GetCaptionImageWidths(ci);
		}
		int GetCaptionImageWidths(GridColumnInfoArgs ci) {
			int lastLeft = 0;
			ci.Bounds = new Rectangle(0, 0, 200, ColumnRowHeight);
			CalcColumnInfo(ci, ref lastLeft);
			Size imageSizes = new Size(0, 0);
			int sortIconWidth = 0;
			GInfo.AddGraphics(null);
			try {
				ci.Cache = GInfo.Cache;
				if(Painter != null) {
					bool canDrawMore = true;
					sortIconWidth = GetSortIconWidth();
					DrawElementInfoCollection innerElements = new DrawElementInfoCollection();
					foreach(DrawElementInfo info in ci.InnerElements)
						if(info.ElementPainter as SkinSortedShapeObjectPainter == null)
							innerElements.Add(info);
					imageSizes = innerElements.CalcMinSize(ci.Graphics, ref canDrawMore);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return imageSizes.Width + elementInterval + sortIconWidth;
		}
		int GetSortIconWidth() {
			Skin currentSkin = GridSkins.GetSkin(View.GridControl.LookAndFeel);
			SkinElement element = currentSkin[GridSkins.SkinSortShape];
			return Math.Max(element.Image.Image.Width, element.Size.MinSize.Width);
		}
		int GetBestTextWidth(GridDashboardColumn column) {
			int maxTextWidth = 0;
			GridColumnDisplayMode displayMode = ((IGridColumn)column).DisplayMode;
			if(displayMode == GridColumnDisplayMode.Sparkline) {
				DashboardSparklineEdit editor = (DashboardSparklineEdit)column.ColumnEdit;
				editor.PrepareData();
				DashboardSparklineCalculator calculator = editor.SparklineCalculator;
				maxTextWidth = calculator.ShowStartEndValues ? calculator.GetMaxValueWidth(graphics, View.Appearance.Row.Font) * 2 + Padding * 2 : 0;
			}
			else if(!column.TextIsHidden && (displayMode == GridColumnDisplayMode.Value || displayMode == GridColumnDisplayMode.Delta)) {
				using(Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
					GridColumnInfoArgs ci = CreateColumnInfo(column);
					for(int rowIndex = 0; rowIndex < View.RowCount; rowIndex++) {
						object value = View.GetRowCellValue(rowIndex, column);
						string displayText = (string)View.GetRowCellValue(rowIndex, column.FieldName + GridMultiDimensionalDataSource.DisplayTextPostfix);
						string[] lines = displayText != null ? displayText.Split((char)10) : new string[0];
						int textWidth = 0;
						foreach(string line in lines) {
							textWidth = GetTextWidth(ci.Appearance, line);
							maxTextWidth = Math.Max(maxTextWidth, textWidth);
							if(!DashboardView.WordWrap)
								break;
						}
					}
				}
			}
			return maxTextWidth;
		}
		int VisualElementAreaWidth(IGridColumn col) {
			if(col.TextIsHidden)
				return CalcBarWidth(col);
			switch(col.DisplayMode) {
				case GridColumnDisplayMode.Delta:
					return col.IgnoreDeltaIndication ? 0 : GridDeltaInfo.ImageWidth + Padding;
				case GridColumnDisplayMode.Sparkline:
				case GridColumnDisplayMode.Bar:
					return CalcBarWidth(col);
				case GridColumnDisplayMode.Image:
					return GetMaxImageWidth(col);
				case GridColumnDisplayMode.Value: {
					return col.MaxIconStyleImageWidth > 0 ? col.MaxIconStyleImageWidth + Padding : 0;
				}
				default:
					return 0;
			}
		}
		int CalcBarWidth(IGridColumn col) {
			return Convert.ToInt32(col.DefaultBestCharacterCount * DashboardView.CharWidth);
		}
		int GetMaxImageWidth(IGridColumn col) {
			GridDashboardColumn gridColumn = col as GridDashboardColumn;
			int maxImageWidth = 0;
			for(int rowIndex = 0; rowIndex < View.RowCount; rowIndex++) {
				byte[] bytes = View.GetRowCellValue(rowIndex, gridColumn) as byte[];
				int imageWidth = 0;
				Image image = null;
				if(bytes != null && bytes.Length != 0)
					image = ByteImageConverter.FromByteArray(bytes);
				if(image != null)
					imageWidth = image.Width;
				maxImageWidth = Math.Max(maxImageWidth, imageWidth);
			}
			return maxImageWidth;
		}
		int GetTextWidth(AppearanceObject appearance, string text) {
			return appearance.CalcTextSize(graphics, appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrap), text, Int32.MaxValue).ToSize().Width;
		}
	}
}
