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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class DocumentSelectorPreview : psvControl {
		#region static
		public static readonly DependencyProperty TargetProperty;
		static readonly DependencyPropertyKey PreviewBrushPropertyKey;
		public static readonly DependencyProperty PreviewBrushProperty;
		static readonly DependencyPropertyKey PreviewWidthPropertyKey;
		public static readonly DependencyProperty PreviewWidthProperty;
		static readonly DependencyPropertyKey PreviewHeightPropertyKey;
		public static readonly DependencyProperty PreviewHeightProperty;
		static readonly DependencyPropertyKey CutHorizontalPropertyKey;
		public static readonly DependencyProperty CutHorizontalProperty;
		static readonly DependencyPropertyKey CutVerticalPropertyKey;
		public static readonly DependencyProperty CutVerticalProperty;
		static readonly DependencyPropertyKey BorderTemplatePropertyKey;
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty DocumentBorderTemplateProperty;
		public static readonly DependencyProperty PanelBorderTemplateProperty;
		static DocumentSelectorPreview() {
			var dProp = new DependencyPropertyRegistrator<DocumentSelectorPreview>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Target", ref TargetProperty, (object)null,
				(dObj, e) => ((DocumentSelectorPreview)dObj).OnTargetChanged(e.NewValue));
			dProp.RegisterReadonly("PreviewBrush", ref PreviewBrushPropertyKey, ref PreviewBrushProperty, (VisualBrush)null);
			dProp.RegisterReadonly("PreviewWidth", ref PreviewWidthPropertyKey, ref PreviewWidthProperty, 150.0);
			dProp.RegisterReadonly("PreviewHeight", ref PreviewHeightPropertyKey, ref PreviewHeightProperty, 250.0);
			dProp.RegisterReadonly("CutHorizontal", ref CutHorizontalPropertyKey, ref CutHorizontalProperty, false);
			dProp.RegisterReadonly("CutVertical", ref CutVerticalPropertyKey, ref CutVerticalProperty, false);
			dProp.RegisterReadonly("BorderTemplate", ref BorderTemplatePropertyKey, ref BorderTemplateProperty, (DataTemplate)null,
				null,
				(dObj, value) => ((DocumentSelectorPreview)dObj).CoerceBorderTemplate((DataTemplate)value));
			dProp.Register("DocumentBorderTemplate", ref DocumentBorderTemplateProperty, (DataTemplate)null);
			dProp.Register("PanelBorderTemplate", ref PanelBorderTemplateProperty, (DataTemplate)null);
		}
		#endregion
		public DocumentSelectorPreview() {
			PreviewBrush = new VisualBrush()
			{
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top,
				Stretch = Stretch.None
			};
		}
		public object Target {
			get { return GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		public VisualBrush PreviewBrush {
			get { return (VisualBrush)GetValue(PreviewBrushProperty); }
			private set { SetValue(PreviewBrushPropertyKey, value); }
		}
		public double PreviewHeight {
			get { return (double)GetValue(PreviewHeightProperty); }
			private set { SetValue(PreviewHeightPropertyKey, value); }
		}
		public double PreviewWidth {
			get { return (double)GetValue(PreviewWidthProperty); }
			private set { SetValue(PreviewWidthPropertyKey, value); }
		}
		public bool CutHorizontal {
			get { return (bool)GetValue(CutHorizontalProperty); }
			private set { SetValue(CutHorizontalPropertyKey, value); }
		}
		public bool CutVertical {
			get { return (bool)GetValue(CutVerticalProperty); }
			private set { SetValue(CutVerticalPropertyKey, value); }
		}
		public DataTemplate BorderTemplate {
			get { return (DataTemplate)GetValue(BorderTemplateProperty); }
			private set { SetValue(BorderTemplatePropertyKey, value); }
		}
		public DataTemplate DocumentBorderTemplate {
			get { return (DataTemplate)GetValue(DocumentBorderTemplateProperty); }
			set { SetValue(DocumentBorderTemplateProperty, value); }
		}
		public DataTemplate PanelBorderTemplate {
			get { return (DataTemplate)GetValue(PanelBorderTemplateProperty); }
			set { SetValue(PanelBorderTemplateProperty, value); }
		}
		public Border PartPreview { get; private set; }
		public FrameworkElement PartView { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartPreview = GetTemplateChild("PART_Preview") as Border;
			if(PartPreview != null)
				PartPreview.FlowDirection = System.Windows.FlowDirection.LeftToRight;
			PartView = GetTemplateChild("PART_View") as FrameworkElement;
			UpdateCut();
		}
		protected virtual DataTemplate CoerceBorderTemplate(DataTemplate value) {
			BaseLayoutItem item = Target as BaseLayoutItem;
			if(item == null) return value;
			return item.ItemType == LayoutItemType.Document ?
				DocumentBorderTemplate : PanelBorderTemplate;
		}
		DetachedElementDecorator detachedElementDecoratorCore;
		Dictionary<BaseLayoutItem, FrameworkElement> elementCache = new Dictionary<BaseLayoutItem, FrameworkElement>(4);
		protected virtual void OnTargetChanged(object target) {
			CoerceValue(BorderTemplateProperty);
			FrameworkElement element = null;
			BaseLayoutItem item = Target as BaseLayoutItem;
			if(item != null) {
				element = GetPreviewElement(item);
				if(element != null) {
					if(!element.IsMeasureValid)
						element.Measure(new Size(1000, 1000));
					if(!element.IsArrangeValid)
						element.Arrange(new Rect(new Point(), new Size(1000, 1000)));
					PreviewWidth = Math.Max(48, Math.Max(element.DesiredSize.Width, element.ActualWidth));
					PreviewHeight = Math.Max(48, Math.Max(element.DesiredSize.Height, element.ActualHeight));
					UpdateCut();
				}
			}
			if(element == null) {
				element = new TextBlock()
				{
					Text = "Preview unavailable",
					HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
					VerticalAlignment = System.Windows.VerticalAlignment.Center
				};
				ClearValue(PreviewWidthPropertyKey);
				ClearValue(PreviewHeightPropertyKey);
				ClearValue(CutHorizontalPropertyKey);
				ClearValue(CutVerticalPropertyKey);
			}
			if(detachedElementDecoratorCore != null) detachedElementDecoratorCore.Child = null;
			if(VisualTreeHelper.GetParent(element) == null) {
				if(PartPreview != null)
					PartPreview.FlowDirection = System.Windows.FlowDirection.LeftToRight;
				detachedElementDecoratorCore = new DetachedElementDecorator() { Width = PreviewWidth, Height = PreviewHeight };
				detachedElementDecoratorCore.Child = element;
				PreviewBrush.Visual = detachedElementDecoratorCore;
			}
			else {
				if(PartPreview != null)
					PartPreview.FlowDirection = FlowDirection;
				PreviewBrush.Visual = element;
			}
			InvalidateMeasure();
			UpdateLayout();
		}
		FrameworkElement GetPreviewElement(BaseLayoutItem item) {
			if(!(item is LayoutPanel)) return null;
			LayoutPanel panel = (LayoutPanel)item;
			FrameworkElement element = panel.Control as FrameworkElement;
			if(element == null) {
				if(elementCache.ContainsKey(item)) {
					element = elementCache[item];
				}
				else {
					element = new ContentPresenter()
					{
						Content = panel.Content,
						ContentTemplate = panel.ContentTemplate,
						ContentTemplateSelector = panel.ContentTemplateSelector
					};
					AddLogicalChild(element);
					elementCache.Add(item, element);
				}
			}
			return element;
		}
		internal void OnClosing() {
			detachedElementDecoratorCore.Child = null;
			elementCache.Clear();
		}
		void UpdateCut() {
			if(PartView != null) {
				CutHorizontal = PreviewWidth > PartView.ActualWidth;
				CutVertical = PreviewHeight > PartView.ActualHeight;
			}
		}
		internal class DetachedElementDecorator : Decorator {
			protected override Size MeasureOverride(Size constraint) {
				return base.MeasureOverride(IsInfiniteSize(constraint) ? new Size(1000, 1000) : constraint);
			}
			static bool IsInfiniteSize(Size size) {
				return double.IsPositiveInfinity(size.Width) && double.IsPositiveInfinity(size.Height);
			}
			UIElement childCore;
			public override UIElement Child {
				get { return childCore; }
				set {
					if(childCore == value) return;
					if(childCore != null)
						RemoveVisualChild(childCore);
					childCore = value;
					if(childCore != null)
						AddVisualChild(childCore);
				}
			}
			protected override Visual GetVisualChild(int index) {
				return childCore;
			}
			protected override int VisualChildrenCount {
				get { return childCore == null ? 0 : 1; }
			}
		}
	}
}
