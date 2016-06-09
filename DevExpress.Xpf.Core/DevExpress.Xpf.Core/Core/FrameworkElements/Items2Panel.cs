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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
#if SILVERLIGHT
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Core {
	[Browsable(false)]
	public enum StretchedContentType { None, Content1, Content2 }
	[Browsable(false)]
	public class Items2Panel : Panel {
		#region Dependency Properties
		public static readonly DependencyProperty Content1Property =
			DependencyPropertyManager.Register("Content1", typeof(UIElement), typeof(Items2Panel),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((Items2Panel)d).OnContent1Changed(e)));
		public static readonly DependencyProperty Content2Property =
			DependencyPropertyManager.Register("Content2", typeof(UIElement), typeof(Items2Panel),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((Items2Panel)d).OnContent2Changed(e)));
		public static readonly DependencyProperty Content1PaddingProperty =
			DependencyPropertyManager.Register("Content1Padding", typeof(Thickness), typeof(Items2Panel),
			new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty Content2PaddingProperty =
			DependencyPropertyManager.Register("Content2Padding", typeof(Thickness), typeof(Items2Panel),
			new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty VerticalPaddingProperty =
			DependencyPropertyManager.Register("VerticalPadding", typeof(Thickness), typeof(Items2Panel),
			new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty HorizontalPaddingProperty =
			DependencyPropertyManager.Register("HorizontalPadding", typeof(Thickness), typeof(Items2Panel),
			new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty VerticalIndentProperty =
			DependencyPropertyManager.Register("VerticalIndent", typeof(double), typeof(Items2Panel),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty HorizontalIndentProperty =
			DependencyPropertyManager.Register("HorizontalIndent", typeof(double), typeof(Items2Panel),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty AlignmentProperty =
			DependencyPropertyManager.Register("Alignment", typeof(Dock), typeof(Items2Panel),
			new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty FillContent1Property =
			DependencyPropertyManager.Register("FillContent1", typeof(bool), typeof(Items2Panel),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty FillContent2Property =
			DependencyPropertyManager.Register("FillContent2", typeof(bool), typeof(Items2Panel),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty EmptyPaddingProperty =
			DependencyPropertyManager.Register("EmptyPadding", typeof(Thickness), typeof(Items2Panel),
			new FrameworkPropertyMetadata(new Thickness(0.0), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty StretchedContentProperty =
			DependencyPropertyManager.Register("StretchedContent", typeof(StretchedContentType), typeof(Items2Panel),
			new FrameworkPropertyMetadata(StretchedContentType.None, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty IgnoreContent2HorizontalAlignmentProperty =
			DependencyPropertyManager.Register("IgnoreContent2HorizontalAlignment", typeof(bool), typeof(Items2Panel), 
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));				
		#endregion Dependency Properties
		public UIElement Content1 {
			get { return (UIElement)GetValue(Content1Property); }
			set { SetValue(Content1Property, value); }
		}
		public UIElement Content2 {
			get { return (UIElement)GetValue(Content2Property); }
			set { SetValue(Content2Property, value); }
		}
		public Thickness EmptyPadding {
			get { return (Thickness)GetValue(EmptyPaddingProperty); }
			set { SetValue(EmptyPaddingProperty, value); }
		}
		public Thickness Content1Padding {
			get { return (Thickness)GetValue(Content1PaddingProperty); }
			set { SetValue(Content1PaddingProperty, value); }
		}
		public Thickness Content2Padding {
			get { return (Thickness)GetValue(Content2PaddingProperty); }
			set { SetValue(Content2PaddingProperty, value); }
		}
		public Thickness VerticalPadding {
			get { return (Thickness)GetValue(VerticalPaddingProperty); }
			set { SetValue(VerticalPaddingProperty, value); }
		}
		public Thickness HorizontalPadding {
			get { return (Thickness)GetValue(HorizontalPaddingProperty); }
			set { SetValue(HorizontalPaddingProperty, value); }
		}
		public double VerticalIndent {
			get { return (double)GetValue(VerticalIndentProperty); }
			set { SetValue(VerticalIndentProperty, value); }
		}
		public double HorizontalIndent {
			get { return (double)GetValue(HorizontalIndentProperty); }
			set { SetValue(HorizontalIndentProperty, value); }
		}
		public Dock Alignment {
			get { return (Dock)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		public bool FillContent1 {
			get { return (bool)GetValue(FillContent1Property); }
			set { SetValue(FillContent1Property, value); }
		}
		public bool FillContent2 {
			get { return (bool)GetValue(FillContent2Property); }
			set { SetValue(FillContent2Property, value); }
		}
		public StretchedContentType StretchedContent {
			get { return (StretchedContentType)GetValue(StretchedContentProperty); }
			set { SetValue(StretchedContentProperty, value); }
		}
		public bool IgnoreContent2HorizontalAlignment {
			get { return (bool)GetValue(IgnoreContent2HorizontalAlignmentProperty); }
			set { SetValue(IgnoreContent2HorizontalAlignmentProperty, value); }
		}		
		public virtual bool HasContent1 {
			get { return Content1 != null && Content1.Visibility != Visibility.Collapsed; }
		}
		public virtual bool HasContent2 {
			get { return Content2 != null && Content2.Visibility != Visibility.Collapsed; }
		}
		protected virtual Thickness GetActualPadding() {
			if((!HasContent1 || Content1.DesiredSize == new Size(0, 0)) && (!HasContent2 || Content2.DesiredSize == new Size(0, 0))) return EmptyPadding;
			if(HasContent1) {
				if(!HasContent2) return Content1Padding;
				if(IsVerticalAlignment && Content2.DesiredSize.Height == 0) return Content1Padding;
				if(Content2.DesiredSize.Width == 0) {
					if(Content1.DesiredSize.Width == 0) {
						if(Content1.DesiredSize.Height != 0)
							return new Thickness(0, Content1Padding.Top, 0, Content1Padding.Bottom);
						else
							return new Thickness(0);
					}
					return Content1Padding;
				}
			}
			if(HasContent2) {
				if(!HasContent1) return Content2Padding;
				if(IsVerticalAlignment && Content1.DesiredSize.Height == 0) return Content2Padding;
				if(Content1.DesiredSize.Width == 0) {
					if(Content2.DesiredSize.Width == 0)
						return new Thickness(0);
					return Content2Padding;
				}
			}
			if(IsVerticalAlignment) return VerticalPadding;
			return HorizontalPadding;
		}
		protected virtual double GetActualIndent() {
			if(!HasContent1 || !HasContent2) return 0.0;
			if(Alignment == Dock.Top || Alignment == Dock.Bottom) {
				if(Content1.DesiredSize.Height == 0 || Content2.DesiredSize.Height == 0) return 0.0;
				return VerticalIndent;
			}
			if(Content1.DesiredSize.Width == 0 || Content2.DesiredSize.Width == 0) return 0.0;
			return HorizontalIndent;
		}
		protected virtual void OnContent1Changed(DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null)
				Children.Remove((UIElement)e.OldValue);
			if(e.NewValue != null)
				Children.Insert(0, (UIElement)e.NewValue);
		}
		protected virtual void OnContent2Changed(DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null)
				Children.Remove((UIElement)e.OldValue);
			if(e.NewValue != null)
				Children.Add((UIElement)e.NewValue);
		}
		protected virtual Size CalcBestSize() {
			Size res = new Size(0, 0);
			Size c1Size = new Size(0, 0);
			Size c2Size = new Size(0, 0);
			if(HasContent1)
				c1Size = Content1.DesiredSize;
			if(HasContent2)
				c2Size = Content2.DesiredSize;
			if(Alignment == Dock.Top || Alignment == Dock.Bottom) {
				res.Width = Math.Max(c1Size.Width, c2Size.Width);
				res.Height += c1Size.Height + c2Size.Height;
				res.Height += GetActualIndent();
			} else if(Alignment == Dock.Left || Alignment == Dock.Right) {
				res.Height = Math.Max(c1Size.Height, c2Size.Height);
				res.Width += c1Size.Width + c2Size.Width;
				res.Width += GetActualIndent();
			}
			Thickness t = GetActualPadding();
			res.Width += t.Left + t.Right;
			res.Height += t.Top + t.Bottom;
			return res;
		}
		protected virtual void GetContentPaddings(ref Thickness content1Padding, ref Thickness content2Padding) {
			content1Padding = new Thickness();
			content2Padding = new Thickness();
			if(!HasContent1 && !HasContent2) {
				content1Padding = EmptyPadding;
				return;
			}
			if(HasContent1 && !HasContent2) {
				content1Padding = Content1Padding;
				return;
			}
			if(!HasContent1 && HasContent2) {
				content2Padding = Content2Padding;
				return;
			}
			if(Alignment == Dock.Bottom || Alignment == Dock.Top) {
				if(Alignment == Dock.Top) {
					content1Padding = new Thickness(VerticalPadding.Left, VerticalPadding.Top, VerticalPadding.Right, VerticalIndent);
					content2Padding = new Thickness(VerticalPadding.Left, 0, VerticalPadding.Right, VerticalPadding.Bottom);
					return;
				}
				content1Padding = new Thickness(VerticalPadding.Left, 0, VerticalPadding.Right, VerticalPadding.Bottom);
				content2Padding = new Thickness(VerticalPadding.Left, VerticalPadding.Top, VerticalPadding.Right, VerticalIndent);
				return;
			}
			if(Alignment == Dock.Left) {
				content1Padding = new Thickness(HorizontalPadding.Left, HorizontalPadding.Top, HorizontalIndent, HorizontalPadding.Bottom);
				content2Padding = new Thickness(0, HorizontalPadding.Top, HorizontalPadding.Right, HorizontalPadding.Bottom);
				return;
			}
			content1Padding = new Thickness(0, HorizontalPadding.Top, HorizontalPadding.Right, HorizontalPadding.Bottom);
			content2Padding = new Thickness(HorizontalPadding.Left, HorizontalPadding.Top, HorizontalIndent, HorizontalPadding.Bottom);				
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size measureSize = availableSize;
			Thickness content1Padding = new Thickness();
			Thickness content2Padding = new Thickness();
			GetContentPaddings(ref content1Padding, ref content2Padding);
			if(!double.IsPositiveInfinity(measureSize.Width)) {
				measureSize.Width = Math.Max(0, measureSize.Width - content1Padding.Left - content1Padding.Right);
			}
			if(!double.IsPositiveInfinity(measureSize.Height)) {
				measureSize.Height = Math.Max(0, measureSize.Height - content1Padding.Top - content1Padding.Bottom);
			}
			if(Content1 != null) {
				Content1.Measure(measureSize);
			}
			if(!double.IsPositiveInfinity(measureSize.Width)) {
				measureSize.Width = Math.Max(0, measureSize.Width - content2Padding.Left - content2Padding.Right);
			}
			if(!double.IsPositiveInfinity(measureSize.Height)) {
				measureSize.Height = Math.Max(0, measureSize.Height - content2Padding.Top - content2Padding.Bottom);
			}
			if(Content2 != null)
				Content2.Measure(measureSize);
			return CalcBestSize();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Size retSize = CalcBestSize();
			if(VerticalAlignment == VerticalAlignment.Stretch) retSize.Height = finalSize.Height;
			if(HorizontalAlignment == HorizontalAlignment.Stretch) retSize.Width = finalSize.Width;
			Rect finalRect = new Rect(new Point(0, 0), retSize);
			Rect c1Rect, c2Rect;
			Thickness t = GetActualPadding();
			finalRect = new Rect(t.Left, t.Top, Math.Max(0, finalRect.Width - t.Left - t.Right), Math.Max(0, finalRect.Height - t.Bottom - t.Top));
			if(HasContent1 && !HasContent2) {
				Content1.Arrange(finalRect);
				return retSize;
			} else if(!HasContent1 && HasContent2) {
				Content2.Arrange(finalRect);
				return retSize;
			}
			bool hasContent = HasContent1 && HasContent2;
			if(hasContent) {
				if(Alignment == Dock.Top) {
					c1Rect = new Rect(new Point(finalRect.X + (finalRect.Width - Content1.DesiredSize.Width) / 2, finalRect.Top), Content1.DesiredSize);
					c2Rect = new Rect(new Point(finalRect.X + (finalRect.Width - Content2.DesiredSize.Width) / 2, c1Rect.Bottom + GetActualIndent()), Content2.DesiredSize);
				} else if(Alignment == Dock.Bottom) {
					c2Rect = new Rect(new Point(finalRect.X + (finalRect.Width - Content2.DesiredSize.Width) / 2, finalRect.Top), Content2.DesiredSize);
					c1Rect = new Rect(new Point(finalRect.X + (finalRect.Width - Content1.DesiredSize.Width) / 2, c2Rect.Bottom + GetActualIndent()), Content1.DesiredSize);
				} else if(Alignment == Dock.Left) {
					c1Rect = new Rect(new Point(finalRect.X, finalRect.Y + (finalRect.Height - Content1.DesiredSize.Height) / 2), Content1.DesiredSize);
					c2Rect = new Rect(new Point(c1Rect.Right + GetActualIndent(), finalRect.Y + (finalRect.Height - Content2.DesiredSize.Height) / 2), Content2.DesiredSize);
				} else {
					c2Rect = new Rect(new Point(finalRect.X, finalRect.Y + (finalRect.Height - Content2.DesiredSize.Height) / 2), Content2.DesiredSize);
					c1Rect = new Rect(new Point(c2Rect.Right + GetActualIndent(), finalRect.Y + (finalRect.Height - Content1.DesiredSize.Height) / 2), Content1.DesiredSize);
				}
				CorrectBoundsByFinalRect(finalRect, ref c1Rect, ref c2Rect);
				StretchContentByFillType(finalRect, ref c1Rect, ref c2Rect);
				StretchContentStretchContentType(finalRect, ref c1Rect, ref c2Rect);
				StretchContentVertical(finalRect, ref c1Rect, ref c2Rect);
				Content1.Arrange(c1Rect);
				Content2.Arrange(c2Rect);
			}
			return retSize;
		}
		void CorrectBoundsByFinalRect(Rect finalRect, ref Rect c1Rect, ref Rect c2Rect) {
			if(Content2 is FrameworkElement && Alignment == Dock.Left) {
				if(IgnoreContent2HorizontalAlignment) {
					if(((FrameworkElement)Content2).HorizontalAlignment == System.Windows.HorizontalAlignment.Right) {
						c2Rect.Width = finalRect.Width - c2Rect.X;
					}
				} else {
					if(((FrameworkElement)Content2).HorizontalAlignment == System.Windows.HorizontalAlignment.Right) {
						c2Rect.X = finalRect.Width - c2Rect.Width;
					}
					if(((FrameworkElement)Content2).HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch) {
						c2Rect.Width = finalRect.Right - c2Rect.X;
					}
					if(((FrameworkElement)Content2).HorizontalAlignment == System.Windows.HorizontalAlignment.Center) {
						c2Rect.X += (finalRect.Right - c2Rect.Right) / 2;
					}
				}
			}
		}
		void StretchContentByFillType(Rect finalRect, ref Rect c1Rect, ref Rect c2Rect) {
			if(Alignment == Dock.Top || Alignment == Dock.Bottom) {
				if(FillContent1) {
					c1Rect.Width = finalRect.Width;
					c1Rect.X = finalRect.X;
				}
				if(FillContent2) {
					c2Rect.Width = finalRect.Width;
					c2Rect.X = finalRect.X;
				}
			} else {
				if(FillContent1) {
					c1Rect.Height = finalRect.Height;
					c1Rect.Y = finalRect.Y;
				}
				if(FillContent2) {
					c2Rect.Height = finalRect.Height;
					c2Rect.Y = finalRect.Y;
				}
			}
		}
		void StretchContentVertical(Rect finalRect, ref Rect c1Rect, ref Rect c2Rect) {
			FrameworkElement cnt1 = Content1 as FrameworkElement;
			FrameworkElement cnt2 = Content1 as FrameworkElement;
			if(cnt1 == null || cnt2 == null) return;
			if(cnt1.VerticalAlignment == VerticalAlignment.Stretch && Alignment == Dock.Top) {
				c1Rect.Height = Math.Max(finalRect.Height - c2Rect.Height, 0);
				c2Rect.Y = finalRect.Y + c1Rect.Height;
				return;
			}
			if(cnt1.VerticalAlignment == VerticalAlignment.Stretch && Alignment == Dock.Bottom) {
				c1Rect.Height = Math.Max(finalRect.Height - c2Rect.Height, 0);
				return;
			}
			if(cnt2.VerticalAlignment == VerticalAlignment.Stretch && Alignment == Dock.Top) {
				c2Rect.Height = Math.Max(finalRect.Height - c1Rect.Height, 0);
				return;
			}
			if(cnt2.VerticalAlignment == VerticalAlignment.Stretch && Alignment == Dock.Bottom) {
				c2Rect.Height = Math.Max(finalRect.Height - c1Rect.Height, 0);
				c1Rect.Y = finalRect.Y + c2Rect.Height;
				return;
			}
		}
		void StretchContentStretchContentType(Rect finalRect, ref Rect c1Rect, ref Rect c2Rect) {
			if(StretchedContent == StretchedContentType.None)
				return;
			if(Alignment == Dock.Top || Alignment == Dock.Bottom) {
				if(StretchedContent == StretchedContentType.Content1) {
					c1Rect.Y = finalRect.Y;
					c2Rect.Y = finalRect.Y + finalRect.Height - c2Rect.Height;
					c1Rect.Height = c2Rect.Y - finalRect.Y;
				} else {
					c2Rect.Y = finalRect.Y;
					c1Rect.Y = finalRect.Y + finalRect.Height - c1Rect.Height;
					c2Rect.Height = c1Rect.Y - finalRect.Y;
				}
			} else {
				if(StretchedContent == StretchedContentType.Content1) {
					c1Rect.X = finalRect.X;
					c2Rect.X = finalRect.X + finalRect.Width - c2Rect.Width;
					c1Rect.Width = c2Rect.X - finalRect.X;
				} else {
					c2Rect.X = finalRect.X;
					c1Rect.X = finalRect.X + finalRect.Width - c1Rect.Width;
					c2Rect.Width = c1Rect.X - finalRect.X;
				}
			}
		}
		bool IsVerticalAlignment { get { return Alignment == Dock.Top || Alignment == Dock.Bottom; } }
	}
}
