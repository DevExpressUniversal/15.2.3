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

using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public class ContentViewport : Decorator {
		#region static
		public static readonly DependencyProperty MaxHorizontalIndentProperty;
		public static readonly DependencyProperty MaxVerticalIndentProperty;
		public static readonly DependencyProperty HorizontalContentAlignmentProperty;
		public static readonly DependencyProperty VerticalContentAlignmentProperty;
		public static readonly DependencyProperty KeepPositiveLeftContentPositionProperty;
		static ContentViewport() {
			VerticalContentAlignmentProperty = DependencyPropertyManager.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(ContentViewport),
				new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));
			HorizontalContentAlignmentProperty = DependencyPropertyManager.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ContentViewport),
				new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));
			MaxVerticalIndentProperty = DependencyPropertyManager.Register("MaxVerticalIndent", typeof(double), typeof(ContentViewport), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MaxHorizontalIndentProperty = DependencyPropertyManager.Register("MaxHorizontalIndent", typeof(double), typeof(ContentViewport), new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure));
			KeepPositiveLeftContentPositionProperty = DependencyPropertyManager.Register("KeepPositiveLeftContentPosition", typeof(bool), typeof(ContentViewport), new FrameworkPropertyMetadata(false));
		}
		#endregion
		public ContentViewport() {
			Focusable = false;
		}
		public bool KeepPositiveLeftContentPosition {
			get { return (bool)GetValue(KeepPositiveLeftContentPositionProperty); }
			set { SetValue(KeepPositiveLeftContentPositionProperty, value); }
		}
		public double MaxHorizontalIndent {
			get { return (double)GetValue(MaxHorizontalIndentProperty); }
			set { SetValue(MaxHorizontalIndentProperty, value); }
		}
		public double MaxVerticalIndent {
			get { return (double)GetValue(MaxVerticalIndentProperty); }
			set { SetValue(MaxVerticalIndentProperty, value); }
		}
		public HorizontalAlignment HorizontalContentAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
			set { SetValue(HorizontalContentAlignmentProperty, value); }
		}
		public VerticalAlignment VerticalContentAlignment {
			get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
			set { SetValue(VerticalContentAlignmentProperty, value); }
		}
		public Size ContentSize { get; private set; }	  
		protected override Size MeasureOverride(Size constraint) {
			ContentSize = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Size retVal = new Size(0, 0);
			retVal.Width = double.IsInfinity(constraint.Width) ? ContentSize.Width : constraint.Width;
			retVal.Height = double.IsInfinity(constraint.Height) ? ContentSize.Height : constraint.Height;
			if(!double.IsPositiveInfinity(MaxHorizontalIndent)) {
				if(retVal.Width > ContentSize.Width + MaxHorizontalIndent) retVal.Width = ContentSize.Width + MaxHorizontalIndent;
			}
			if(!double.IsPositiveInfinity(MaxVerticalIndent)) {
				if(retVal.Height > ContentSize.Height + MaxVerticalIndent) retVal.Height = ContentSize.Height + MaxVerticalIndent;
			}
			return retVal;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if(Child == null)
				return arrangeSize;
			Rect pos = new Rect(new Point(), Child.DesiredSize);
			switch(HorizontalContentAlignment) {
				case HorizontalAlignment.Center:
					pos.X = (arrangeSize.Width - Child.DesiredSize.Width) / 2;
					break;
				case HorizontalAlignment.Stretch:
					pos.Width = arrangeSize.Width;
					break;
				case HorizontalAlignment.Right:
					pos.X = (arrangeSize.Width - Child.DesiredSize.Width);
					break;
			}
			switch(VerticalContentAlignment) {
				case VerticalAlignment.Center:
					pos.Y = (arrangeSize.Height - Child.DesiredSize.Height) / 2;
					break;
				case VerticalAlignment.Stretch:
					pos.Height = arrangeSize.Height;
					break;
				case VerticalAlignment.Bottom:
					pos.Y = (arrangeSize.Height - Child.DesiredSize.Height);
					break;
			}
			if(KeepPositiveLeftContentPosition && pos.X < 0) pos.X = 0;
			Child.Arrange(pos);
			return arrangeSize;
		}
	}
}
