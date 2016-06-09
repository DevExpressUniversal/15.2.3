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

using DevExpress.XtraSpreadsheet.Layout;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Globalization;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using LayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
using DrawingRectangle = System.Drawing.Rectangle;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetDataValidationPanel
	public class WorksheetDataValidationPanel : WorksheetPaintPanel {
		#region Fields
		public static readonly DependencyProperty DataValidationCircleBrushProperty;
		public static readonly DependencyProperty DataValidationCircleWidthProperty;
		public static readonly DependencyProperty DataValidationBackgroundBrushProperty;
		public static readonly DependencyProperty DataValidationBorderColorProperty;
		public static readonly DependencyProperty DataValidationMessageForeBrushProperty;
		public static readonly DependencyProperty DataValidationMessageBorderWidthProperty;
		public static readonly DependencyProperty DataValidationMessageBorderBrushProperty;
		public static readonly DependencyProperty DataValidationMessageBackgroudBrushProperty;
		#endregion
		static WorksheetDataValidationPanel() {
			Type ownerType = typeof(WorksheetDataValidationPanel);
			DataValidationCircleBrushProperty = DependencyProperty.Register("DataValidationCircleBrush", typeof(Brush), ownerType);
			DataValidationCircleWidthProperty = DependencyProperty.Register("DataValidationCircleWidth", typeof(double), ownerType);
			DataValidationBackgroundBrushProperty = DependencyProperty.Register("DataValidationBackgroundBrush", typeof(Brush), ownerType);
			DataValidationBorderColorProperty = DependencyProperty.Register("DataValidationBorderColor", typeof(Color), ownerType);
			DataValidationMessageForeBrushProperty = DependencyProperty.Register("DataValidationMessageForeBrush", typeof(Brush), ownerType);
			DataValidationMessageBorderWidthProperty = DependencyProperty.Register("DataValidationMessageBorderWidth", typeof(double), ownerType);
			DataValidationMessageBorderBrushProperty = DependencyProperty.Register("DataValidationMessageBorderBrush", typeof(Brush), ownerType);
			DataValidationMessageBackgroudBrushProperty = DependencyProperty.Register("DataValidationMessageBackgroudBrush", typeof(Brush), ownerType);
		}
		public WorksheetDataValidationPanel() {
			DefaultStyleKey = typeof(WorksheetDataValidationPanel);
		}
		#region Properties
		public Brush DataValidationCircleBrush {
			get { return (Brush)GetValue(DataValidationCircleBrushProperty); }
			set { SetValue(DataValidationCircleBrushProperty, value); }
		}
		public double DataValidationCircleWidth {
			get { return (double)GetValue(DataValidationCircleWidthProperty); }
			set { SetValue(DataValidationCircleWidthProperty, value); }
		}
		public Brush DataValidationBackgroundBrush {
			get { return (Brush)GetValue(DataValidationBackgroundBrushProperty); }
			set { SetValue(DataValidationBackgroundBrushProperty, value); }
		}
		public Color DataValidationBorderColor {
			get { return (Color)GetValue(DataValidationBorderColorProperty); }
			set { SetValue(DataValidationBorderColorProperty, value); }
		}
		public Brush DataValidationMessageForeBrush {
			get { return (Brush)GetValue(DataValidationMessageForeBrushProperty); }
			set { SetValue(DataValidationMessageForeBrushProperty, value); }
		}
		public double DataValidationMessageBorderWidth {
			get { return (double)GetValue(DataValidationMessageBorderWidthProperty); }
			set { SetValue(DataValidationMessageBorderWidthProperty, value); }
		}
		public Brush DataValidationMessageBorderBrush {
			get { return (Brush)GetValue(DataValidationMessageBorderBrushProperty); }
			set { SetValue(DataValidationMessageBorderBrushProperty, value); }
		}
		public Brush DataValidationMessageBackgroudBrush {
			get { return (Brush)GetValue(DataValidationMessageBackgroudBrushProperty); }
			set { SetValue(DataValidationMessageBackgroudBrushProperty, value); }
		}
		#endregion
		protected override void OnRender(DrawingContext dc) {
			if (LayoutInfo == null)
				return;
			HotZonePainter hotZonePainter = new HotZonePainter(dc, null, Owner.AutoFilterImageCache, DataValidationBackgroundBrush, DataValidationBorderColor);
			foreach (LayoutPage page in LayoutInfo.Pages)
				DrawDataValidation(dc, page, hotZonePainter);
		}
		void DrawDataValidation(DrawingContext dc, LayoutPage page, HotZonePainter hotZonePainter) {
			DataValidationLayout layout = SpreadsheetProvider.ActiveView.DataValidationLayout;
			layout.Update(page);
			DrawDataValidationHotZone(layout.HotZone, hotZonePainter);
			DrawDataValidationInvalidCircle(dc, layout.InvalidDataRectangles);
			DrawDataValidationMessage(dc, layout.MessageLayout);
		}
		void DrawDataValidationHotZone(DataValidationHotZone hotZone, HotZonePainter hotZonePainter) {
			if (hotZone == null)
				return;
			hotZonePainter.DrawHotZone(hotZone);
		}
		void DrawDataValidationInvalidCircle(DrawingContext dc, List<DrawingRectangle> rectangles) {
			if (rectangles.Count == 0)
				return;
			Pen pen = new Pen(DataValidationCircleBrush, DataValidationCircleWidth);
			foreach (DrawingRectangle rect in rectangles) {
				double halfX = rect.Width / 2;
				double halfY = rect.Height / 2;
				double centerX = rect.Left + halfX - 1;
				double centerY = rect.Top + halfY - 1;
				dc.DrawEllipse(null, pen, new Point(centerX, centerY), halfX, halfY);
			}
		}
		void DrawDataValidationMessage(DrawingContext dc, DataValidationMessageLayout messageLayout) {
			if (messageLayout == null || messageLayout.BoundsMessage.IsEmpty)
				return;
			Pen pen = new Pen(DataValidationMessageBorderBrush, DataValidationMessageBorderWidth - 0.5);
			Rect bounds = messageLayout.BoundsMessage.ToRect();
			bounds.Offset(-0.5, -0.5); 
			dc.DrawRectangle(DataValidationMessageBackgroudBrush, pen, bounds);
			if (!String.IsNullOrEmpty(messageLayout.Title)) {
				DrawingRectangle titleBounds = messageLayout.BoundsTitle;
				dc.DrawText(GetFontStyle(messageLayout.Title, messageLayout.TitleFontInfo, true, titleBounds.Width), titleBounds.Location.ToPoint());
			}
			DrawingRectangle texstBounds = messageLayout.BoundsText;
			dc.DrawText(GetFontStyle(messageLayout.Text, messageLayout.TextFontInfo, false, texstBounds.Width), texstBounds.Location.ToPoint());
		}
		FormattedText GetFontStyle(string text, FontInfo fontInfo, bool isTitle, int drawTextMaxWidth) {
			FontWeight weightStyle = isTitle ? FontWeights.Bold : FontWeights.Normal;
			double size = Units.DocumentsToPixelsF(Units.PointsToDocumentsF(fontInfo.Size), DocumentModel.DpiY);
			Typeface typeface = new Typeface(new FontFamily(fontInfo.Name), FontStyles.Normal, weightStyle, FontStretches.Normal);
			FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, this.FlowDirection, typeface, size, DataValidationMessageForeBrush);
			formattedText.MaxTextWidth = drawTextMaxWidth;
			formattedText.Trimming = TextTrimming.None;
			return formattedText;
		}
	}
	#endregion
}
