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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.GridData;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public class ConditionalFormatContentRenderHelper<T> where T : FrameworkElement, IChrome {
		static readonly RenderTemplate conditionalFormatTemplate;
		public const int ImageLeftRightMargin = 1;
		static ConditionalFormatContentRenderHelper() {
			var panel = new RenderPanel() { LayoutProvider = LayoutProvider.GridInstance, ShouldCalcDpiAwareThickness = false };
			panel.Children.Add(new RenderBorder() { ShouldCalcDpiAwareThickness = false });
			panel.Children.Add(new RenderBorder() { ShouldCalcDpiAwareThickness = false });
			panel.Children.Add(new RenderImage() { Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(ImageLeftRightMargin, 0, ImageLeftRightMargin, 0), ShouldCalcDpiAwareThickness = false });
			conditionalFormatTemplate = new RenderTemplate() { RenderTree = panel };
		}
		readonly T owner;
		RenderPanelContext context;
		Thickness margin;
		public RenderBorderContext BarBorder { get { return (RenderBorderContext)((IFrameworkRenderElementContext)context).GetRenderChild(0); } }
		public RenderBorderContext ZeroLineBorder { get { return (RenderBorderContext)((IFrameworkRenderElementContext)context).GetRenderChild(1); } }
		public RenderImageContext IconImage { get { return (RenderImageContext)((IFrameworkRenderElementContext)context).GetRenderChild(2); } }
#if DEBUGTEST
		public RenderPanelContext ContextForTests { get { return context; } }
		public int MeasureCountForTests { get; private set; }
#endif
		DataBarFormatInfo dataBarFormatInfo;
		public ConditionalFormatContentRenderHelper(T owner) {
			this.owner = owner;
			context = null;
		}
		public void UpdateDataBarFormatInfo(DataBarFormatInfo info) {
			if(dataBarFormatInfo != info) {
				dataBarFormatInfo = info;
				owner.InvalidateMeasure();
				owner.InvalidateArrange();
				owner.InvalidateVisual();
			}
		}
		public void Measure(Size availableSize) {
#if DEBUGTEST
			MeasureCountForTests++;
#endif
			if(dataBarFormatInfo != null) {
				if(context == null) {
					context = (RenderPanelContext)ChromeHelper.CreateContext(owner, conditionalFormatTemplate);
					UpdateMargin();
				}
				context.Visibility = null;
				ValidataDataBar(availableSize);
				ValidateIcon();
				context.Measure(availableSize);
			}
			else {
				if(context != null) {
					context.Visibility = Visibility.Collapsed;
				}
			}
		}
		void ValidateIcon() {
			if(dataBarFormatInfo.Icon != null) {
				IconImage.Visibility = null;
				IconImage.Source = dataBarFormatInfo.Icon;
				IconImage.VerticalAlignment = dataBarFormatInfo.IconVerticalAlignment;
				(owner as ISupportHorizonalContentAlignment).Do(x => IconImage.HorizontalAlignment = ToIconAlignment(x.HorizonalContentAlignment));
			}
			else {
				IconImage.Visibility = Visibility.Collapsed;
			}
		}
		static HorizontalAlignment ToIconAlignment(HorizontalAlignment alignment) {
			switch(alignment) {
				case HorizontalAlignment.Left:
					return HorizontalAlignment.Right;
				default:
					return HorizontalAlignment.Left;
			}
		}
		void ValidataDataBar(Size availableSize) {
			if(dataBarFormatInfo.Format != null) {
				BarBorder.Visibility = null;
				ZeroLineBorder.Visibility = null;
				Thickness formatMargin = dataBarFormatInfo.Format.Margin;
				double zeroLineWidth = (dataBarFormatInfo.ZeroPosition > 0 && dataBarFormatInfo.ZeroPosition < 1) ? dataBarFormatInfo.Format.ZeroLineThickness : 0;
				double availableRange = (availableSize.Width - formatMargin.Left - formatMargin.Right - this.margin.Left - this.margin.Right - zeroLineWidth);
				double zeroPosition = Math.Round(dataBarFormatInfo.ZeroPosition * availableRange);
				double width = Math.Round(availableRange * Math.Abs(dataBarFormatInfo.ZeroPosition - Math.Min(1, Math.Max(0, dataBarFormatInfo.ValuePosition))));
				double leftMargin;
				double rightMargin;
				Brush barFill = dataBarFormatInfo.Format.Fill;
				Brush barBorderBrush = dataBarFormatInfo.Format.BorderBrush;
				if(dataBarFormatInfo.ValuePosition > dataBarFormatInfo.ZeroPosition) {
					leftMargin = zeroPosition + zeroLineWidth;
					rightMargin = availableRange - zeroPosition - width;
				}
				else {
					rightMargin = availableRange - zeroPosition + zeroLineWidth;
					leftMargin = zeroPosition - width;
					barFill = dataBarFormatInfo.Format.FillNegative ?? barFill;
					barBorderBrush = dataBarFormatInfo.Format.BorderBrushNegative ?? barBorderBrush;
				}
				BarBorder.BorderThickness = dataBarFormatInfo.Format.BorderThickness;
				BarBorder.Background = barFill;
				BarBorder.BorderBrush = barBorderBrush;
				BarBorder.Margin = new Thickness(formatMargin.Left + leftMargin, formatMargin.Top, formatMargin.Right + rightMargin, formatMargin.Bottom);
				ZeroLineBorder.Margin = new Thickness(formatMargin.Left + zeroPosition, 0, formatMargin.Right + availableRange - zeroPosition, 0);
				ZeroLineBorder.Background = dataBarFormatInfo.Format.ZeroLineBrush;
			}
			else {
				BarBorder.Visibility = Visibility.Collapsed;
				ZeroLineBorder.Visibility = Visibility.Collapsed;
			}
		}
		public void Arrange(Size finalSize) {
			if(context != null)
				context.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
		}
		public void Render(DrawingContext dc) {
			if(context != null)
				context.Render(dc);
		}
		public void SetMargin(Thickness margin) {
			if(this.margin != margin) {
				this.margin = margin;
				if(context != null)
					UpdateMargin();
			}
		}
		void UpdateMargin() {
			context.Margin = margin;
		}
	}
	public interface ISupportHorizonalContentAlignment {
		HorizontalAlignment HorizonalContentAlignment { get; }
	}
	public static class ChromeHelper {
		public static FrameworkRenderElementContext CreateContext(IChrome chrome, RenderTemplate template) {
			var namescope = new Namescope(chrome);
			return template.CreateContext(namescope, namescope, chrome);
		}
	}
}
