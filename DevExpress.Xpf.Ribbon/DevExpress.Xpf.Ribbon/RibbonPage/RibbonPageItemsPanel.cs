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
using DevExpress.Xpf.Ribbon;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Ribbon {
	public enum MeasureType {
		Default,
		AllowCollapse,
		AllowHide,
		Collapsed
	}
	public class RibbonPageGroupsItemsPanel : RibbonOrderedOnMergeItemsPanel {
		public static readonly DependencyProperty MeasureTypeProperty;
		static RibbonPageGroupsItemsPanel() {
			MeasureTypeProperty = DependencyProperty.RegisterAttached("MeasureType", typeof(MeasureType), typeof(RibbonPageGroupsItemsPanel), new FrameworkPropertyMetadata(MeasureType.Default, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		public static MeasureType GetMeasureType(DependencyObject obj) {
			return (MeasureType)obj.GetValue(MeasureTypeProperty);
		}
		public static void SetMeasureType(DependencyObject obj, MeasureType value) {
			obj.SetValue(MeasureTypeProperty, value);
		}
		public RibbonPageGroupsItemsPanel() { }
		protected override bool IsOrdered {
			get { return true; } }
		protected override int GetChildOrder(UIElement child) {
			RibbonPageGroupControl groupControl = child as RibbonPageGroupControl;
			var pGroup = groupControl.With(x => x.PageGroup);
			var isRibbonMerged = pGroup.With(x => x.Ribbon).Return(x => x.IsMerged, () => false);
			if(pGroup==null)
				return base.GetChildOrder(child);
			if (isRibbonMerged)
				return pGroup.ActualMergeOrder;
			return pGroup.Index;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Ribbon == null || (Ribbon.RibbonStyle != RibbonStyle.TabletOffice && Ribbon.RibbonStyle != RibbonStyle.OfficeSlim))
				return base.MeasureOverride(availableSize);
			Dictionary<UIElement, Size> desiredSizeMin = GetMinDesiredSize();
			Size desiredSize = new Size();
			Size measureSize = SizeHelper.Infinite;
			foreach(UIElement child in Children) {
				SetMeasureType(child, MeasureType.Default);
				child.Measure(measureSize);
				desiredSize.Width += child.DesiredSize.Width;
				desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
			}
			if(desiredSize.Width > availableSize.Width) {
				desiredSize = ReduceAdapt(availableSize, desiredSize, desiredSizeMin);
				desiredSize = ReduceCollapse(availableSize, desiredSize);
			}
			return desiredSize;
		}
		protected override RibbonControl GetRibbon() {
			var ribbonPagesControl = ItemsControl.GetItemsOwner(this) as RibbonPageGroupsControl;
			return ribbonPagesControl.Ribbon;
		}
		double GetMinWidthCollapsed(RibbonPageGroupControl pgc) {
			double minDesiredWidthCollapsed = 0d;
			if(Children.Count == 0)
				return minDesiredWidthCollapsed;
			SetMeasureType(pgc, MeasureType.Collapsed);
			pgc.Measure(SizeHelper.Infinite);
			minDesiredWidthCollapsed = pgc.DesiredSize.Width;
			pgc.InvalidateMeasure();
			return minDesiredWidthCollapsed;
		}
		Dictionary<UIElement, Size> GetMinDesiredSize() {
			Dictionary<UIElement, Size> result = new Dictionary<UIElement, Size>();
			foreach(UIElement child in Children) {
				SetMeasureType(child, MeasureType.AllowCollapse);
				child.Measure(SizeHelper.Infinite);
				result.Add(child, child.DesiredSize);
			}
			return result;
		}
		Size ReduceAdapt(Size availableSize, Size currentSize, Dictionary<UIElement, Size> minSize) {
			Size totalSize = currentSize;
			double currentWidth = 0d;
			Size measureSize = new Size(0d, availableSize.Height);
			for(int i = Children.Count - 1; i >= 0 && totalSize.Width > availableSize.Width; i--) {
				UIElement child = Children[i];
				currentWidth = child.DesiredSize.Width;
				SetMeasureType(child, MeasureType.Default);
				measureSize.Width = Math.Max(minSize[child].Width, availableSize.Width - GetSumWidth(child) - GetAfterSumWidth(child));
				child.Measure(measureSize);
				totalSize.Width -= currentWidth - child.DesiredSize.Width;
				totalSize.Height = Math.Max(totalSize.Height, child.DesiredSize.Height);
			}
			return totalSize;
		}
		Size ReduceCollapse(Size availableSize, Size currentSize) {
			Size totalSize = currentSize;
			double currentWidth = 0d;
			Size measureSize = new Size(0d, availableSize.Height);
			double minWidthCollapsed = double.NaN;
			for(int i = Children.Count - 1; i >= 0 && totalSize.Width > availableSize.Width; i--) {
				UIElement child = Children[i];
				currentWidth = child.DesiredSize.Width;
				var pageGroup = child as RibbonPageGroupControl;
				if(pageGroup == null)
					continue;
				minWidthCollapsed = GetMinWidthCollapsed(pageGroup);
				SetMeasureType(child, MeasureType.AllowHide);
				measureSize.Width = Math.Max(minWidthCollapsed, availableSize.Width - GetSumWidth(child) - GetAfterSumWidth(child));
				child.Measure(measureSize);
				totalSize.Width -= currentWidth - child.DesiredSize.Width;
				totalSize.Height = Math.Max(totalSize.Height, child.DesiredSize.Height);
			}
			return totalSize;
		}
		double GetSumWidth(UIElement child) {
			double totalWidth = 0d;
			int max = Children.IndexOf(child);
			for(int i = 0; i < max; i++) {
				totalWidth += Children[i].DesiredSize.Width;
			}
			return totalWidth;
		}
		double GetAfterSumWidth(UIElement child) {
			double sum = 0d;
			for(int i = Children.IndexOf(child) + 1; i < Children.Count; i++) {
				sum += Children[i].DesiredSize.Width;
			}
			return sum;
		}
	}
	public class RibbonPagesItemsPanel : Panel {
		RibbonControl Ribbon { get; set; }
		public RibbonPagesItemsPanel() {
			SizeChanged += OnSizeChanged;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		static Size emptySize = new Size();
		void OnUnloaded(object sender, RoutedEventArgs e) {
			Ribbon = null;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			Ribbon = LayoutHelper.FindParentObject<RibbonControl>(this);
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if (Ribbon == null || e.PreviousSize.Equals(emptySize) || Ribbon.RibbonStyle != RibbonStyle.TabletOffice)
				return;
			TranslateTransform transform = new TranslateTransform();
			RenderTransform = transform;
			double offset = Math.Min((e.NewSize.Width - e.PreviousSize.Width) / 2, 30);
			DoubleAnimation animation = new DoubleAnimation() { FillBehavior = FillBehavior.Stop };
			animation.Duration = new Duration(TimeSpan.FromMilliseconds(80));
			animation.From = offset;
			animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
			transform.BeginAnimation(TranslateTransform.XProperty, animation);   
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size retValue = new Size(0, 0);
			foreach(UIElement child in Children) {
				RibbonPageGroupsControl groupsControl = child as RibbonPageGroupsControl;
				if(groupsControl != null) {
					child.Measure(availableSize);
					if(groupsControl.Page.IsSelected) {
						retValue = child.DesiredSize;
					}
				} else
					child.Measure(availableSize);
			}
			return retValue;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach(UIElement child in Children) {
				RibbonPageGroupsControl groupsControl = child as RibbonPageGroupsControl;
				if(groupsControl != null && (groupsControl.Page.IsSelected || !groupsControl.IsArranged)) {
					child.Arrange(new Rect(0, 0, child.DesiredSize.Width, finalSize.Height));
				}
			}
			return finalSize;
		}
	}
}
