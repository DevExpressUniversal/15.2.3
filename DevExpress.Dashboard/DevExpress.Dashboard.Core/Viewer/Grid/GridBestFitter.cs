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

using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraEditors.Controls;
using System;
using System.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class GridBestFitter {
		readonly ICellValueGetter cellValueGetter;
		public GridBestFitter(ICellValueGetter cellValueGetter) {
			this.cellValueGetter = cellValueGetter;
		}
		public int GetBestFitWidth(IGridBestFitColumnInfo colInfo, float charWidth, int contentMargin, int headerMargin) {
			if(colInfo.DisplayMode == GridColumnDisplayMode.Sparkline)
				return 0;
			int captionWidth = colInfo.CaptionImageWidths + colInfo.CaptionTextWidth;
			int maxTextWidth = colInfo.DisplayMode != GridColumnDisplayMode.Bar && colInfo.DisplayMode != GridColumnDisplayMode.Image ? colInfo.MaxTextWidth : 0;
			int imageWidth = VisualElementAreaWidth(colInfo, charWidth, contentMargin);
			return Math.Max(captionWidth + headerMargin * 2, maxTextWidth + imageWidth + contentMargin * 2) + 1;
		}
		public int GetSparklineBestFitWidth(IGridBestFitColumnInfo colInfo, Font font, float charWidth, DashboardSparklineCalculator calculator, int contentMargin, int headerMargin) {
			int captionWidth = colInfo.CaptionImageWidths + colInfo.CaptionTextWidth;
			int maxTextWidth = 0;
			try {
				using(Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
					maxTextWidth = calculator.ShowStartEndValues ? calculator.GetMaxValueWidth(graphics, font) * 2 + contentMargin * 2 : 0;
				}
			}
			catch { }
			int imageWidth = VisualElementAreaWidth(colInfo, charWidth, contentMargin);
			return Math.Max(captionWidth + headerMargin * 2, maxTextWidth + imageWidth + contentMargin * 2) + 1;
		}
		int VisualElementAreaWidth(IGridBestFitColumnInfo colInfo, float charWidth, int contentMargin) {
			if(colInfo.TextIsHidden)
				return CaclBarWidth(colInfo, charWidth);
			switch(colInfo.DisplayMode) {
				case GridColumnDisplayMode.Delta:
					return colInfo.IgnoreDeltaIndication ? 0 : GridDeltaInfo.ImageWidth + contentMargin;
				case GridColumnDisplayMode.Sparkline:
				case GridColumnDisplayMode.Bar:
					return CaclBarWidth(colInfo, charWidth);
				case GridColumnDisplayMode.Image:
					return GetMaxImageWidth(colInfo);
				case GridColumnDisplayMode.Value: {
					return colInfo.MaxIconStyleImageWidth > 0 ? colInfo.MaxIconStyleImageWidth + contentMargin : 0;
					}
				default:
					return 0;
			}
		}
		int CaclBarWidth(IGridBestFitColumnInfo colInfo, float charWidth) {
			return Convert.ToInt32(colInfo.DefaultBestCharacterCount * charWidth);
		}
		int GetMaxImageWidth(IGridBestFitColumnInfo colInfo) {
			int rowCount = cellValueGetter.RowCount;
			int maxImageWidth = 0;
			for(int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				byte[] bytes = cellValueGetter.GetCellValue(colInfo.Index, rowIndex) as byte[];
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
	}
	public interface IGridBestFitColumnInfo {
		GridColumnDisplayMode DisplayMode { get; set; }
		double DefaultBestCharacterCount { get; set; }
		bool IgnoreDeltaIndication { get; set; }
		string Caption { get; set; }
		int Index { get; set; }
		int CaptionImageWidths { get; set; }
		int MaxTextWidth { get; set; }
		int CaptionTextWidth { get; set; }
		int MaxIconStyleImageWidth { get; set; }
		bool TextIsHidden { get; set; }
	}
	public interface ICellValueGetter {
		int RowCount { get; }
		object GetCellValue(int columnIndex, int rowIndex);
	}
	public class GridBestFitColumnInfo : IGridBestFitColumnInfo {
		public GridColumnDisplayMode DisplayMode { get; set; }
		public double DefaultBestCharacterCount { get; set; }
		public bool IgnoreDeltaIndication { get; set; }
		public string Caption { get; set; }
		public int Index { get; set; }
		public int CaptionImageWidths { get; set; }
		public int MaxTextWidth { get; set; }
		public int CaptionTextWidth { get; set; }
		public int MaxIconStyleImageWidth { get; set; }
		public bool TextIsHidden { get; set; }
	}
}
