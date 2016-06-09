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

using System.Collections.Generic;
using System.Windows;
using DevExpress.Mvvm.Native;
using System.Windows.Media;
using System;
using DevExpress.Xpf.Core.Native;
using System.Linq;
namespace DevExpress.Xpf.Bars {
	public class RadialMenuItem : FrameworkElement {
		#region static
		public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(RadialMenuItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
		#endregion
		#region dep props
		public double Angle {
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}
		#endregion 
		#region props
		public double ContentTopPadding { get; set; }
		UIElement border;
		public UIElement Border {
			get {
				return border;
			}
			set {
				if(border == value)
					return;
				UIElement oldValue = border;
				border = value;
				OnLogicalAndVisualChildChanged(oldValue, Border);
			}
		}
		UIElement arrow;
		public UIElement Arrow {
			get {
				return arrow;
			}
			set {
				if(arrow == value)
					return;
				UIElement oldValue = arrow;
				arrow = value;
				OnLogicalAndVisualChildChanged(oldValue, Arrow);
			}
		}
		UIElement content;
		public UIElement Content {
			get {
				return content;
			}
			set {
				if(content == value)
					return;
				UIElement oldValue = content;
				content = value;
				OnLogicalAndVisualChildChanged(oldValue, Content);
			}
		}
		protected List<UIElement> Children {
			get {
				List<UIElement> children = new List<UIElement>();
				Border.Do(v => children.Add(Border));
				Arrow.Do(v => children.Add(Arrow));
				Content.Do(v => children.Add(Content));
				return children;
			}
		}
		internal int SectorCount { get { return this.VisualParents().OfType<RadialMenuItemsPanel>().FirstOrDefault().Return(i => i.SectorCount, () => 8); } }
		#endregion        
		protected virtual void OnLogicalAndVisualChildChanged(UIElement oldValue, UIElement newValue) {
			oldValue.Do(v => { RemoveLogicalChild(v); RemoveVisualChild(v); });
			newValue.Do(v => { AddLogicalChild(v); AddVisualChild(v); });
		}
		protected override System.Collections.IEnumerator LogicalChildren { get { return Children.GetEnumerator(); } }
		protected override int VisualChildrenCount { get { return Children.Count; } }
		protected override Visual GetVisualChild(int index) {
			return Children[index];
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0)
				return base.MeasureOverride(availableSize);
			Size retValue = new Size();
			foreach(FrameworkElement child in Children) {
				child.Measure(availableSize);
				retValue.Width = Math.Max(retValue.Width, child.DesiredSize.Width);
				retValue.Height = Math.Max(retValue.Height, child.DesiredSize.Height);
			}
			double contentRadius = CalcContentRadius(retValue);
			double rMin = CalcRmin(contentRadius, 2 * Math.PI / SectorCount);			
			retValue.Height = rMin + ContentTopPadding;
			retValue.Width = Math.Round(2 * retValue.Height * Math.Sin(Math.PI / SectorCount));
			if(!double.IsPositiveInfinity(availableSize.Width))
				retValue.Width = Math.Min(availableSize.Width, retValue.Width);
			if(!double.IsPositiveInfinity(availableSize.Height))
				retValue.Height = Math.Min(availableSize.Height, retValue.Height);
			return retValue;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Children.Count == 0)
				return base.ArrangeOverride(finalSize);
			if(Border != null) {
				Border.RenderTransformOrigin = new Point(0.5, 1);
				Border.RenderTransform = new RotateTransform(Angle, 0, 0);
				Border.Arrange(new Rect(new Point(), finalSize));
			}
			if(Arrow != null) {
				Arrow.RenderTransformOrigin = new Point(0.5, 1);
				Arrow.RenderTransform = new RotateTransform(Angle, 0, 0);
				Arrow.Arrange(new Rect(new Point(), finalSize));
			}
			if(Content != null) {
				Content.RenderTransformOrigin = new Point(0.5, 0.5);
				double contentRadius = CalcContentRadius(Content.DesiredSize);
				double angleInRadians = Angle * Math.PI / 180d;
				double contentTopPointRadius = finalSize.Height - contentRadius - ContentTopPadding;
				Content.RenderTransform = new TranslateTransform(Math.Round(contentTopPointRadius * Math.Sin(angleInRadians)), Math.Round(contentTopPointRadius - contentTopPointRadius * Math.Cos(angleInRadians)));
				Content.Arrange(new Rect(0, contentRadius - Content.DesiredSize.Height / 2 + ContentTopPadding, finalSize.Width, Content.DesiredSize.Height));
			}
			return finalSize;
		}
		double CalcContentRadius(Size contentSize) {
			return Math.Round(Math.Sqrt(contentSize.Width * contentSize.Width + contentSize.Height * contentSize.Height) / 2d);
		}
		double CalcRmin(double contentRadius, double w) {			
			return Math.Round(contentRadius / Math.Sin(w / 2d) + contentRadius);
		}
	}	
}
