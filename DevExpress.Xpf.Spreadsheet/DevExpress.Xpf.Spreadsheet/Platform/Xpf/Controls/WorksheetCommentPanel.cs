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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Windows;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using LayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetCommentPanel
	public class WorksheetCommentPanel : WorksheetPaintPanel {
		#region Fields
		static CommentTextSettings textSettings = CreateTextSetting();
		public static readonly DependencyProperty CommentLineBrushProperty;
		public static readonly DependencyProperty CommentLineWidthProperty;
		int childIndex = 0;
		#endregion
		static WorksheetCommentPanel() {
			Type ownerType = typeof(WorksheetCommentPanel);
			CommentLineBrushProperty = DependencyProperty.Register("CommentLineBrush", typeof(Brush), ownerType);
			CommentLineWidthProperty = DependencyProperty.Register("CommentLineWidth", typeof(double), ownerType);
		}
		public WorksheetCommentPanel() {
			DefaultStyleKey = typeof(WorksheetCommentPanel);
		}
		#region Properties
		public Brush CommentLineBrush {
			get { return (Brush)GetValue(CommentLineBrushProperty); }
			set { SetValue(CommentLineBrushProperty, value); }
		}
		public double CommentLineWidth {
			get { return (double)GetValue(CommentLineWidthProperty); }
			set { SetValue(CommentLineWidthProperty, value); }
		}
		#endregion
		internal static CommentTextSettings CreateTextSetting() {
			return TextInfoCalculator.CalculateCommentTextSettings(Comment.DefaultFont, Comment.DefaultForeColor);
		}
		#region Arrange
		int index = 0;
		protected override Size ArrangeOverride(Size finalSize) {
			if (LayoutInfo == null) {
				HideChildren();
				return finalSize;
			}
			index++;
			foreach (LayoutPage page in LayoutInfo.Pages) {
				ArrangeComments(page.CommentBoxes);
			}
			HideChildren();
			return finalSize;
		}
		void ArrangeComments(List<CommentBox> comments) {
			foreach (CommentBox comment in comments) {
				ArrangeComment(comment);
			}
		}
		void ArrangeComment(CommentBox comment) {
			if (!comment.CanDraw())
				return;
			CommentItem item = GetChild();
			item.Text = comment.GetNormalizedPlainText();
			item.Background = new SolidColorBrush(comment.FillColor.ToWpfColor());
			item.TextSettings = textSettings;
			item.Cursor = SpreadsheetControl.Cursor;
			Rect bounds = comment.Bounds.ToRect();
			bounds.Offset(-1, -1);
			item.Measure(bounds.Size);
			item.Arrange(bounds);
		}
		CommentItem GetChild() {
			if (childIndex >= Children.Count) {
				CommentItem item = new CommentItem();
				Children.Add(item);
			}
			return Children[childIndex++] as CommentItem;
		}
		void HideChildren() {
			for (int i = childIndex; i < Children.Count; i++)
				Children[i].Arrange(new Rect(0, 0, 0, 0));
			childIndex = 0;
		}
		#endregion
		#region Render
		protected override void OnRender(DrawingContext dc) {
			if (LayoutInfo == null)
				return;
			dc.PushTransform(new TranslateTransform(-1, -1));
			try {
				HotZonePainter hotZonePainter = new HotZonePainter(dc);
				foreach (LayoutPage page in LayoutInfo.Pages)
					DrawComments(dc, page, hotZonePainter);
			}
			finally {
				dc.Pop();
			}
		}
		void DrawComments(DrawingContext dc, LayoutPage page, HotZonePainter hotZonePainter) {
			SpreadsheetProvider.ActiveView.CommentLayout.Update(page);
			foreach (CommentBox box in page.CommentBoxes) {
				DrawCommentLine(dc, box);
			}
			DrawHotZones(hotZonePainter);
		}
		void DrawCommentLine(DrawingContext dc, CommentBox box) {
			if (!box.CanDraw())
				return;
			Pen pen = new Pen(CommentLineBrush, CommentLineWidth);
			Point start = box.GetIndicatorLineStartPoint().ToPoint();
			Point end = box.GetIndicatorLineEndPoint().ToPoint();
			dc.DrawLine(pen, start, end);
		}
		void DrawHotZones(HotZonePainter hotZonePainter) {
			CommentLayoutItemCollection items = SpreadsheetProvider.ActiveView.CommentLayout.Items;
			foreach (CommentLayoutItem item in items) {
				hotZonePainter.DrawHotZones(item.ResizeHotZones);
			}
		}
		#endregion
	}
	#endregion
	#region CommentItem
	public class CommentItem : Control {
		#region Fields
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty TextSettingsProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		#endregion
		static CommentItem() {
			Type ownerType = typeof(CommentItem);
			TextProperty = DependencyProperty.Register("Text", typeof(string), ownerType);
			TextSettingsProperty = DependencyProperty.Register("TextSettings", typeof(CommentTextSettings), ownerType);
			IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), ownerType);
		}
		public CommentItem() {
			DefaultStyleKey = typeof(CommentItem);
		}
		#region Properties
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public CommentTextSettings TextSettings {
			get { return (CommentTextSettings)GetValue(TextSettingsProperty); }
			set { SetValue(TextSettingsProperty, value); }
		}
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		#endregion
	}
	#endregion
	#region CommentTextSettings
	public class CommentTextSettings : TextSettingsBase {
		public Brush Foreground { get; set; }
	}
	#endregion
}
