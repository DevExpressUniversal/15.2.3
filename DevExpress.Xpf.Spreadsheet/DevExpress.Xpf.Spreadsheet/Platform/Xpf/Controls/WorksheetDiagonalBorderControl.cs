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
using System.Windows;
using System.Windows.Media;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetDiagonalBorderControl
	public class WorksheetDiagonalBorderControl : WorksheetPaintControl {
		#region Fields
		List<DiagonalBorderInfo> borderInfos;
		#endregion
		public WorksheetDiagonalBorderControl() {
			this.borderInfos = new List<DiagonalBorderInfo>();
			DefaultStyleKey = typeof(WorksheetDiagonalBorderControl);
		}
		protected override void OnRender(DrawingContext dc) {
			if (Owner == null || LayoutInfo == null) 
				return;
			if (borderInfos.Count == 0)
				return;
			float zoomFactor = 1 / (float)SpreadsheetProvider.ScaleFactor;
			BorderLinePainter painter = new BorderLinePainter(dc, Owner.LayoutInfo, zoomFactor);
			foreach (DiagonalBorderInfo info in borderInfos) {
				Color color = info.DiagonalBorderColor;
				Rect bounds = info.Bounds;
				DrawDiagonalDownBorder(painter, info.DiagonalDownBorderLineStyle, color, bounds);
				DrawDiagonalUpBorder(painter, info.DiagonalUpBorderLineStyle, color, bounds);
			}
		}
		void DrawDiagonalDownBorder(BorderLinePainter painter, XlBorderLineStyle lineStyle, Color color, Rect bounds) {
			Point start = bounds.TopLeft;
			Point end = bounds.BottomRight;
			DrawDiagonalBorderCore(painter, start, end, lineStyle, color);
		}
		void DrawDiagonalUpBorder(BorderLinePainter painter, XlBorderLineStyle lineStyle, Color color, Rect bounds) {
			Point start = bounds.TopRight;
			Point end = bounds.BottomLeft;
			DrawDiagonalBorderCore(painter, start, end, lineStyle, color);
		}
		void DrawDiagonalBorderCore(BorderLinePainter painter, Point start, Point end, XlBorderLineStyle lineStyle, Color color) {
			painter.DrawLineByStyle(start, end, lineStyle, color);
		}
		internal bool ShouldAddBorder(XlBorderLineStyle downLineStyle, XlBorderLineStyle upLineStyle) {
			return downLineStyle != XlBorderLineStyle.None || upLineStyle != XlBorderLineStyle.None;
		}
		internal void AddBorderInfo(Rect bounds, XlBorderLineStyle downLineStyle, XlBorderLineStyle upLineStyle, Color color) {
			DiagonalBorderInfo info = CreateBorderInfo(bounds, downLineStyle, upLineStyle, color);
			borderInfos.Add(info);
		}
		DiagonalBorderInfo CreateBorderInfo(Rect bounds, XlBorderLineStyle downLineStyle, XlBorderLineStyle upLineStyle, Color color) {
			DiagonalBorderInfo info = new DiagonalBorderInfo();
			info.Bounds = bounds;
			info.DiagonalDownBorderLineStyle = downLineStyle;
			info.DiagonalUpBorderLineStyle = upLineStyle;
			info.DiagonalBorderColor = color;
			return info;
		}
		internal void Clear() {
			borderInfos.Clear();
		}
	}
	#endregion
	#region DiagonalBorderInfo
	public class DiagonalBorderInfo {
		public DiagonalBorderInfo() {
		}
		#region Properties
		public XlBorderLineStyle DiagonalDownBorderLineStyle { get; set; }
		public XlBorderLineStyle DiagonalUpBorderLineStyle { get; set; }
		public Color DiagonalBorderColor { get; set; }
		public Rect Bounds { get; set; }
		#endregion
	}
	#endregion
}
