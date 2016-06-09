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
using System.Windows.Input;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Threading;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonCaptionContentPanel : Panel {
		#region static
		public static readonly DependencyProperty HorizontalContentMarginProperty;
		public static readonly DependencyProperty ContentWidthProperty;
		static RibbonCaptionContentPanel() {
			HorizontalContentMarginProperty = DependencyPropertyManager.Register("HorizontalContentMargin", typeof(double), typeof(RibbonCaptionContentPanel),
					new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ContentWidthProperty = DependencyPropertyManager.Register("ContentWidth", typeof(double), typeof(RibbonCaptionContentPanel),
				new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		#endregion
		public double HorizontalContentMargin {
			get { return (double)GetValue(HorizontalContentMarginProperty); }
			set { SetValue(HorizontalContentMarginProperty, value); }
		}
		public double ContentWidth {
			get { return (double)GetValue(ContentWidthProperty); }
			set { SetValue(ContentWidthProperty, value); }
		}
		public double MaxHorizontalContentMargin { get; private set; }
		public RibbonCaptionContentPanel() {
		}		
		public double MinContentWidth { get; private set; }
		public double MaxContentWidth { get; private set;	}
		protected Size prevContentSize { get; private set; }
		public double ActualContentWidth {
			get {
				return double.IsNaN(ContentWidth) ? MaxContentWidth : ContentWidth;
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			Size avSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			Size maxContentSize = new Size(0, 0); 
			MaxContentWidth = 0;
			MinContentWidth = double.MaxValue;
			foreach(UIElement obj in Children) {
				obj.Measure(avSize);
				maxContentSize = new Size(Math.Max(obj.DesiredSize.Width, maxContentSize.Width),
					Math.Max(obj.DesiredSize.Height, maxContentSize.Height));
				MinContentWidth = Math.Min(obj.DesiredSize.Width, MinContentWidth);
			}
			MaxContentWidth = maxContentSize.Width;
			Size retSize = new Size(0,0);
			if(!prevContentSize.IsEmpty) {
				if(prevContentSize.Width != maxContentSize.Width || prevContentSize.Height != prevContentSize.Height)
					ContentWidth = double.NaN;
			}
			prevContentSize = maxContentSize;
			if(double.IsNaN(ContentWidth))
				retSize = new Size(MaxContentWidth + HorizontalContentMargin, maxContentSize.Height);
			else retSize = new Size(ContentWidth + HorizontalContentMargin, maxContentSize.Height);		
			if(!double.IsPositiveInfinity(constraint.Width)) retSize.Width = constraint.Width;
			if(!double.IsPositiveInfinity(constraint.Height)) retSize.Height = constraint.Height;
			return retSize;
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			MaxHorizontalContentMargin = HorizontalContentMargin;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect pos = new Rect();
			pos.X = HorizontalContentMargin / 2;
			pos.Height = finalSize.Height;
			pos.Width = finalSize.Width;
			foreach(UIElement obj in Children) {
				obj.Arrange(pos);
			}
			return finalSize;
		}
		public virtual double GetMaxEnlargeValueOfContentWidth() {			
			return MaxContentWidth - ActualContentWidth;
		}
		public virtual double GetMaxReduceValueOfContentWidth() {
			return ActualContentWidth;
		}
		public virtual double ReduceContentWidth(double desiredReduceValue) {
			double actualReduceValue = 0;
			double maxReduceValue = GetMaxReduceValueOfContentWidth();
			actualReduceValue = Math.Min(maxReduceValue, desiredReduceValue);
			if(actualReduceValue == 0) return 0;
			ContentWidth = ActualContentWidth - actualReduceValue;
			return actualReduceValue;
		}
		public virtual double EnlargeContentWidth(double desiredEnlargeValue) {
			double actualEnlargeValue = 0;
			double maxEnlargeValue = GetMaxEnlargeValueOfContentWidth();
			actualEnlargeValue = Math.Min(maxEnlargeValue, desiredEnlargeValue);
			if(actualEnlargeValue == 0) return 0;
			if(actualEnlargeValue + ActualContentWidth >= MaxContentWidth) ContentWidth = double.NaN;
			else ContentWidth = actualEnlargeValue + ActualContentWidth;
			return actualEnlargeValue;
		}
		public virtual double GetMaxEnlargeValueOfContentMargin() {
			return MaxHorizontalContentMargin - HorizontalContentMargin;
		}
		public virtual double GetMaxReduceValueOfContentMargin() {
			return HorizontalContentMargin;
		}
		public virtual double ReduceContentMargin(double desiredReduceValue) {
			double actualReduceValue = 0;
			double maxReduceValue = GetMaxReduceValueOfContentMargin();
			actualReduceValue = Math.Min(maxReduceValue, desiredReduceValue);
			if(actualReduceValue == 0) return 0;
			HorizontalContentMargin -= actualReduceValue;
			return actualReduceValue;
		}
		public virtual double EnlargeContentMargin(double desiredEnlargeValue) {
			double actualEnlargeValue = 0;
			double maxEnlargeValue = GetMaxEnlargeValueOfContentMargin();
			actualEnlargeValue = Math.Min(maxEnlargeValue, desiredEnlargeValue);
			if(actualEnlargeValue == 0) return 0;
			HorizontalContentMargin += actualEnlargeValue;
			return actualEnlargeValue;
		}
	}
}
