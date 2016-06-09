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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class CellPresenter : ContentPresenter {
		public static readonly DependencyProperty ShowCellToolTipProperty;
		public static readonly DependencyProperty CellToolTipProperty;
		static CellPresenter() {
			Type ownerType = typeof(CellPresenter);
			ShowCellToolTipProperty = DependencyProperty.Register("ShowCellToolTip", typeof(bool), ownerType);
			CellToolTipProperty = DependencyProperty.Register("CellToolTip", typeof(object), ownerType);
		}
		public CellPresenter() {
			this.DefaultStyleKey = typeof(CellPresenter);
		}
		public bool ShowCellToolTip {
			get { return (bool)GetValue(ShowCellToolTipProperty); }
			set { SetValue(ShowCellToolTipProperty, value); }
		}
		public object CellToolTip {
			get { return (object)GetValue(CellToolTipProperty); }
			set { SetValue(CellToolTipProperty, value); }
		}
		private UIElement GetChild() {
			return VisualTreeHelper.GetChildrenCount(this) > 0 ? VisualTreeHelper.GetChild(this, 0) as UIElement : null;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			UIElement child = GetChild();
			if (child != null) child.Arrange(new Rect(new Point(0, 0), arrangeSize));
			return arrangeSize;
		}
	}
	public class CellControl : Control {
		public static readonly DependencyProperty TextSettingsProperty;
		static CellControl() {
			TextSettingsProperty = DependencyProperty.Register("TextSettings", typeof(TextSettings), typeof(CellControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((CellControl)d).OnTextSettingsChanged()));
		}
		private void OnTextSettingsChanged() {
			Update();
		}
		bool updateFormattedText = false;
		private void Update() {
			updateFormattedText = true;
			InvalidateVisual();
		}
		private void UpdateFormattedText() {
			if (TextSettings != null) {
				fText = CreateFormattedText();
				textTopLeft = CalcTopLeftRenderPointFromAlignment(fText);
			}
			else fText = null;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height)) {
				FormattedText fText = CreateFormattedText();
				return new Size(fText.Width, fText.Height);
			}
			return constraint;
		}
		FormattedText fText;
		Point textTopLeft;
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			ClipToBounds = true;
			if (updateFormattedText) {
				updateFormattedText = false;
				UpdateFormattedText();
			}
			drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ActualWidth, ActualHeight));
			if (fText != null)
				drawingContext.DrawText(fText, textTopLeft);
		}
		int rightOffset = 2;
		private Point CalcTopLeftRenderPointFromAlignment(FormattedText fText) {
			double x = 0;
			double y = 0;
			double xOffset = TextSettings.TextBounds.X;
			double textHeight = fText.Height;
			double layoutOffset = Math.Abs(ActualWidth - TextSettings.TextBounds.Width - TextSettings.TextBounds.X - rightOffset);
			double width = ActualWidth - TextSettings.TextBounds.X - rightOffset;
			if (layoutOffset != 0) {
				if (TextSettings.TextWrapping == TextWrapping.NoWrap) {
					if (TextSettings.TextAlignment == TextAlignment.Left) x = xOffset;
					if (TextSettings.TextAlignment == TextAlignment.Right) x = width;
					if (TextSettings.TextAlignment == TextAlignment.Center) x = width / 2;
				}
			}
			else {
				if (TextSettings.TextWrapping == TextWrapping.NoWrap) {
					if (TextSettings.TextAlignment == TextAlignment.Left) x = xOffset;
					if (TextSettings.TextAlignment == TextAlignment.Right) x = width;
					if (TextSettings.TextAlignment == TextAlignment.Center) x = width / 2;
				}
			}
			if (TextSettings.VerticalAlignment == System.Windows.VerticalAlignment.Top) y = 0;
			if (TextSettings.VerticalAlignment == System.Windows.VerticalAlignment.Bottom) {
				y = Math.Max(ActualHeight - textHeight, 0);
			}
			if (TextSettings.VerticalAlignment == System.Windows.VerticalAlignment.Center) {
				y = Math.Max(ActualHeight / 2 - textHeight / 2, 0);
			}
			return new Point(x, y);
		}
		int patchWidth = 1; 
		private FormattedText CreateFormattedText() {
			FormattedText fText = new FormattedText(TextSettings.Text, CultureInfo.CurrentCulture, this.FlowDirection,
				new Typeface(TextSettings.FontFamily, TextSettings.FontStyle, TextSettings.FontWeight, FontStretches.Normal), TextSettings.FontSize,
				new SolidColorBrush(TextSettings.Foreground));
			fText.SetTextDecorations(TextSettings.TextDecorations);
			if (TextSettings.TextWrapping != TextWrapping.NoWrap) fText.MaxTextWidth = TextSettings.TextBounds.Width + patchWidth;
			fText.TextAlignment = TextSettings.TextAlignment;
			fText.Trimming = TextSettings.TextTrimming;
			if (!double.IsNaN(Height)) fText.MaxTextHeight = Height;
			return fText;
		}
		public CellControl() {
			this.DefaultStyleKey = typeof(CellControl);
		}
		public TextSettings TextSettings {
			get { return (TextSettings)GetValue(TextSettingsProperty); }
			set { SetValue(TextSettingsProperty, value); }
		}
	}
}
