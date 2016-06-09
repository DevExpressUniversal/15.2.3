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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[NonCategorized]
	public class ElementInfoPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		public static readonly DependencyProperty StretchToAvailableSizeProperty = DependencyPropertyManager.Register("StretchToAvailableSize",
			typeof(bool), typeof(ElementInfoPanel), new PropertyMetadata(true));
		public bool StretchToAvailableSize {
			get { return (bool)GetValue(StretchToAvailableSizeProperty); }
			set { SetValue(StretchToAvailableSizeProperty, value); }
		}
		ElementInfoBase ElementInfo { get { return DataContext as ElementInfoBase; } }
		Size GetMeasureSize(Size constraint) {
			if (StretchToAvailableSize || ElementInfo == null || Children.Count == 0)
				return constraint;
			if (ElementInfo.Layout == null)
				return Children[0].DesiredSize;
			double width = ElementInfo.Layout.Width.HasValue ? ElementInfo.Layout.Width.Value : Children[0].DesiredSize.Width;
			double height = ElementInfo.Layout.Height.HasValue ? ElementInfo.Layout.Height.Value : Children[0].DesiredSize.Height;
			return new Size(width, height);
		}
		Size GetArrangeSize(UIElement child, ElementLayout layout, Size finalSize) { 
			double height = layout.Height.HasValue ? layout.Height.Value :
				(StretchToAvailableSize ? child.DesiredSize.Height : finalSize.Height);
			double width = layout.Width.HasValue ? layout.Width.Value :
				(StretchToAvailableSize ? child.DesiredSize.Width : finalSize.Width);
			return new Size(width, height);
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			Size size = new Size(0, 0);
			Visibility visibility = Visibility.Collapsed;
			if (ElementInfo != null) {
				ElementInfo.CreateLayout(constraint);
				if (ElementInfo.Layout != null) {
					size = new Size(ElementInfo.Layout.Width.HasValue ? ElementInfo.Layout.Width.Value : constraint.Width,
									ElementInfo.Layout.Height.HasValue ? ElementInfo.Layout.Height.Value : constraint.Height);
					visibility = Visibility.Visible;
				}
			}
			foreach (UIElement child in Children) {
				child.Visibility = visibility;
				child.Measure(size);				
			}
			return GetMeasureSize(constraint);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (ElementInfo != null)
				ElementInfo.CompleteLayout();
			foreach (UIElement child in Children) {
				Rect childRect = new Rect(0, 0, 0, 0);
				if (ElementInfo != null && ElementInfo.Layout != null) {
					Size size = GetArrangeSize(child, ElementInfo.Layout, finalSize);
					Point transformOrigin = ElementInfo.PresentationControl != null ? ElementInfo.PresentationControl.GetRenderTransformOrigin() : new Point(0, 0);
					Point location = MathUtils.CorrectLocationByTransformOrigin(ElementInfo.Layout.AnchorPoint, transformOrigin, size);
					childRect = new Rect(location, size);
					child.RenderTransformOrigin = transformOrigin;
					child.RenderTransform = ElementInfo.Layout.RenderTransform;
					child.Clip = ElementInfo.Layout.ClipGeometry;
				}
				child.Arrange(childRect);
			}
			return finalSize;
		}
	}
	[
	NonCategorized,
	TemplatePart(Name = "PART_ElementPresentationContainer", Type = typeof(Panel))
	]
	public class ElementInfoContainer : Control, IHitTestableElement, IGaugeLayoutElement {
		public static readonly DependencyProperty ElementInfoProperty = DependencyPropertyManager.Register("ElementInfo",
			typeof(object), typeof(ElementInfoContainer), new PropertyMetadata(ElementInfoPropertyChanged));
		public static readonly DependencyProperty StretchToAvailableSizeProperty = DependencyPropertyManager.Register("StretchToAvailableSize",
			typeof(bool), typeof(ElementInfoContainer), new PropertyMetadata(true));
		public object ElementInfo {
			get { return GetValue(ElementInfoProperty); }
			set { SetValue(ElementInfoProperty, value); }
		}
		public bool StretchToAvailableSize {
			get { return (bool)GetValue(StretchToAvailableSizeProperty); }
			set { SetValue(StretchToAvailableSizeProperty, value); }
		}
		static void ElementInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ElementInfoBase info = e.NewValue as ElementInfoBase;
			if (info != null)
				info.Container = d as ElementInfoContainer;
			info = e.OldValue as ElementInfoBase;
			if (info != null)
				info.Container = null;
		}
		ElementInfoBase ActualElementInfo { get { return ElementInfo as ElementInfoBase; } }
		internal UIElement PresentationContainer { get { return GetTemplateChild("PART_ElementPresentationContainer") as UIElement; } }
		public ElementInfoContainer() {
			DefaultStyleKey = typeof(ElementInfoContainer);
		}
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return ElementInfo != null ? ActualElementInfo.HitTestableObject : null; } }
		Object IHitTestableElement.Parent { get { return ElementInfo != null ? ActualElementInfo.HitTestableParent : null; ; } }
		bool IHitTestableElement.IsHitTestVisible { get { return ElementInfo != null ? ActualElementInfo.IsHitTestVisible : false; } }
		#endregion
		#region IGaugeLayoutElement implementation
		ElementLayout IGaugeLayoutElement.Layout {
			get { return new ElementLayout(); }
		}
		Point IGaugeLayoutElement.Offset { get { return new Point(0, 0); } }
		bool IGaugeLayoutElement.InfluenceOnGaugeSize { get { return ActualElementInfo != null ? ActualElementInfo.InfluenceOnGaugeSize : false; } }
		#endregion
	}	
}
