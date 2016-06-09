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
using System.Drawing;
using System.IO;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Drawing;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Utils;
using System.Reflection;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region PictureSpreadsheetSelectionPainter
	public class PictureSpreadsheetSelectionPainter : SpreadsheetSelectionPainter {
		public PictureSpreadsheetSelectionPainter(GraphicsCache cache)
			: base(cache) {
		}
		protected override void FillRectangle(Rectangle bounds) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawRectangle(Rectangle bounds) {
			Cache.DrawRectangle(Cache.GetPen(ShapeBorderColor, ShapeBorderWidth), bounds);
		}
		protected override void DrawLine(Point from, Point to) {
			Graphics.DrawLine(Cache.GetPen(ShapeBorderColor, ShapeBorderWidth), from, to);
		}
		protected override void DrawDashRectangle(Rectangle bounds) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawPrintRangeRectangle(Rectangle bounds) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawRangeResizeHotZone(Rectangle bounds) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawColumnAutoFilterBackground(Rectangle bounds) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawPivotTableExpandCollapseHotZone(Rectangle bounds, bool isCollapsed) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawColumnAutoFilter(IFilterHotZone hotZone) {
			Exceptions.ThrowInternalException(); 
		}
		protected override int GetConvertedMailMergeRangeWidth() {
			Exceptions.ThrowInternalException(); 
			return -1;
		}
	}
	#endregion
}
