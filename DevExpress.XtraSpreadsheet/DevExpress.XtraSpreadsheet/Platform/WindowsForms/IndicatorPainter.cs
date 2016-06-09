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

using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetIndicatorPainter
	public class SpreadsheetIndicatorPainter {
		#region Fields
		static readonly Color defaultCommentIndicatorColor = Color.Red;
		static readonly Color defaultNumberIndicatorColor = Color.Green;
		const int indicatorWidthInPixels = 5;
		SpreadsheetControl control;
		#endregion
		public SpreadsheetIndicatorPainter(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		protected internal DocumentLayoutUnitConverter LayoutConverter { get { return control.DocumentModel.LayoutUnitConverter; } }
		#endregion
		public void Draw(GraphicsCache cache, IndicatorBox box) {
			int widthInLayoutUnits = LayoutConverter.PixelsToLayoutUnits(indicatorWidthInPixels, DocumentModel.Dpi);
			IndicatorType type = box.Type;
			Rectangle bounds = box.ClipBounds;
			if (type == IndicatorType.Comment)
				DrawCommentIndicator(cache, bounds, widthInLayoutUnits);
			else if (type == IndicatorType.NumberFormatting)
				DrawNumberFormattingIndicator(cache, bounds, widthInLayoutUnits);
		}
		void DrawCommentIndicator(GraphicsCache cache, Rectangle clipBounds, int width) {
			Point topRight = new Point(clipBounds.Right, clipBounds.Top);
			Point topLeft = new Point(topRight.X - width, topRight.Y);
			Point bottomRight = new Point(topRight.X, topRight.Y + width);
			Point[] points = new Point[3] { topLeft, topRight, bottomRight };
			DrawTriangle(cache, defaultCommentIndicatorColor, clipBounds, points);
		}
		void DrawNumberFormattingIndicator(GraphicsCache cache, Rectangle clipBounds, int width) {
			Point topLeft = new Point(clipBounds.Left, clipBounds.Top);
			Point topRight = new Point(topLeft.X + width, topLeft.Y);
			Point bottomLeft = new Point(topLeft.X, topLeft.Y + width);
			Point[] points = new Point[3] { topLeft, topRight, bottomLeft };
			DrawTriangle(cache, defaultNumberIndicatorColor, clipBounds, points);
		}
		void DrawTriangle(GraphicsCache cache, Color color, Rectangle clipBounds, Point[] points) {
			Brush brush = cache.GetSolidBrush(color);
			GraphicsClip clip = cache.ClipInfo;
			GraphicsClipState clipState = clip.SaveClip();
			try {
				clip.SetClip(clipBounds);
				cache.Graphics.FillPolygon(brush, points);
			}
			finally {
				clip.RestoreClipRelease(clipState);
			}
		}
	}
	#endregion
}
