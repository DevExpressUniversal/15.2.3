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
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonItemsPanel : Panel {
		#region static
		public static readonly DependencyProperty ControlBoxWidthProperty;
		public static readonly DependencyProperty DefaultHeaderWidthProperty;
		public static readonly DependencyProperty OffsetProperty;
		static RibbonItemsPanel() {
			ControlBoxWidthProperty = DependencyPropertyManager.Register("ControlBoxWidth", typeof(double), typeof(RibbonItemsPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			DefaultHeaderWidthProperty = DependencyPropertyManager.Register("DefaultHeaderWidth", typeof(double), typeof(RibbonItemsPanel), new FrameworkPropertyMetadata(0d));
			OffsetProperty = DependencyPropertyManager.Register("Offset", typeof(double), typeof(RibbonItemsPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		#endregion
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}		
		public double ControlBoxWidth {
			get { return (double)GetValue(ControlBoxWidthProperty); }
			set { SetValue(ControlBoxWidthProperty, value); }
		}
		public double DefaultHeaderWidth {
			get { return (double)GetValue(DefaultHeaderWidthProperty); }
			set { SetValue(DefaultHeaderWidthProperty, value); }
		}
		public RibbonControl Ribbon {
			get { return Owner.Ribbon; }
		}
		public RibbonPageCategoriesPane Owner { get { return ItemsControl.GetItemsOwner(this) as RibbonPageCategoriesPane; } }
		public ScrollViewer ScrollHost { get { return Owner.With(owner => owner.ScrollHost); } }
		public RibbonItemsPanel() { }
		protected virtual IEnumerable<RibbonPageCategoryHeaderInfo> GetInfos(Size availableSize, ref RibbonPageCategoryHeaderInfo defaultInfo) {
			var infos = new List<RibbonPageCategoryHeaderInfo>();
			foreach(UIElement child in Children) {
				child.Measure(availableSize);
				var category = child as RibbonPageCategoryControl;
				if(category == null)
					continue;
				if(category.PageCategory.IsDefault)
					defaultInfo = category.SizeInfo ?? new RibbonPageCategoryHeaderInfo() { Owner = category };
				else
					infos.Add(category.SizeInfo ?? new RibbonPageCategoryHeaderInfo() { Owner = category });
			}
			return infos;
		}
		protected bool Reduce(Size availableSize, IEnumerable<RibbonPageCategoryHeaderInfo> infos, RibbonPageCategoryHeaderInfo defaultInfo) {
			var commonInfo = new RibbonPageCategoryHeaderInfo();
			double toolbarMinWidth = GetDefaultInfoMinWidth();
			defaultInfo.MinWidth = Math.Max(defaultInfo.MinWidth, toolbarMinWidth);
			defaultInfo.MaxWidth = Math.Max(defaultInfo.MaxWidth, toolbarMinWidth);
			defaultInfo.DesiredWidth = Math.Min(defaultInfo.MaxWidth, Math.Max(defaultInfo.MinWidth, defaultInfo.DesiredWidth));
			infos.ForEach(info => {
				commonInfo.MinWidth += info.MinWidth;
				commonInfo.MaxWidth += info.MaxWidth;
				commonInfo.DesiredWidth += info.DesiredWidth;
			});
			if(commonInfo.MaxWidth + defaultInfo.MaxWidth <= availableSize.Width) {
				infos.ForEach(info => info.MeasureSize = new Size(info.MaxWidth, availableSize.Height));
				defaultInfo.MeasureSize = new Size(defaultInfo.MaxWidth, availableSize.Height);
				return true;
			}
			if(commonInfo.MaxWidth + defaultInfo.DesiredWidth <= availableSize.Width) {
				infos.ForEach(info => info.MeasureSize = new Size(info.MaxWidth, availableSize.Height));
				defaultInfo.MeasureSize = new Size(availableSize.Width - commonInfo.MaxWidth, availableSize.Height);
				return true;
			}
			if(commonInfo.DesiredWidth + defaultInfo.DesiredWidth <= availableSize.Width) {
				defaultInfo.MeasureSize = new Size(defaultInfo.DesiredWidth, availableSize.Height);
				double infoWidth = (availableSize.Width - defaultInfo.DesiredWidth) / (double)infos.Count();
				double coeff = (availableSize.Width - (defaultInfo.DesiredWidth)) / commonInfo.MaxWidth;
				infos.ForEach(info => info.MeasureSize = new Size(info.MaxWidth * coeff, availableSize.Height));
				return true;
			}
			if(commonInfo.DesiredWidth + defaultInfo.MinWidth <= availableSize.Width) {
				infos.ForEach(info => info.MeasureSize = new Size(info.DesiredWidth, availableSize.Height));
				defaultInfo.MeasureSize = new Size(availableSize.Width - commonInfo.DesiredWidth, availableSize.Height);
				return true;
			}
			if(commonInfo.MinWidth + defaultInfo.MinWidth <= availableSize.Width) {
				defaultInfo.MeasureSize = new Size(defaultInfo.MinWidth, availableSize.Height);
				double coeff = (availableSize.Width - (defaultInfo.MinWidth)) / commonInfo.DesiredWidth;
				infos.ForEach(info => info.MeasureSize = new Size(info.DesiredWidth * coeff, availableSize.Height));
				return true;
			}
			defaultInfo.MeasureSize = new Size(defaultInfo.MinWidth, availableSize.Height);
			return false;
		}
		protected override Size MeasureOverride(Size availableSize) {
			RibbonPageCategoryHeaderInfo defaultInfo = new RibbonPageCategoryHeaderInfo();
			IEnumerable<RibbonPageCategoryHeaderInfo> infos = GetInfos(availableSize, ref defaultInfo);
			if(!Reduce(availableSize, infos, defaultInfo)) {
				infos.ForEach(info => {
					info.MeasureSize = new Size(info.MinWidth, availableSize.Height);
					info.Owner.SetCurrentValue(RibbonPageCategoryControl.ShowHeaderProperty, false);
				});
			} else {
				infos.ForEach(info => info.Owner.SetCurrentValue(RibbonPageCategoryControl.ShowHeaderProperty, true));
				Offset = 0d;
			}
			Size res = new Size();
			if(defaultInfo.Owner != null) {
				defaultInfo.Owner.Measure(defaultInfo.MeasureSize);
				res.Height = Math.Max(res.Height, defaultInfo.Owner.DesiredSize.Height);
			}
			res.Width += defaultInfo.MeasureSize.Width;
			for(int i = 0; i < infos.Count(); i++) {
				var info = infos.ElementAt(i);
				info.Owner.Measure(info.MeasureSize);
				res.Width += Math.Max(info.MinWidth, info.Owner.DesiredSize.Width);
				res.Height = Math.Max(res.Height, info.Owner.DesiredSize.Height);
			}
			return res;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect arrangeRect = new Rect(new Point(Offset, 0), new Size(0, finalSize.Height));
			var children = InternalChildren.OfType<RibbonPageCategoryControl>();
			var defaultCategory = children.FirstOrDefault(cat => cat.PageCategory.IsDefault);
			children = children.Where(child => child != defaultCategory).OrderBy(child => GetChildOrder(child)).ToList();
			arrangeRect.Width = GetDefaultInfoMinWidth();
			if (defaultCategory != null) {
				arrangeRect.Width = Math.Max(defaultCategory.DesiredSize.Width, arrangeRect.Width);
				defaultCategory.Arrange(arrangeRect);
			}
			arrangeRect.X += arrangeRect.Width;
			DefaultHeaderWidth = arrangeRect.Width;
			UpdateControlBoxWidth(finalSize.Width);
			double available = finalSize.Width - ControlBoxWidth;
			for (int i = 0; i < children.Count(); i++) {
				var child = children.ElementAt(i);
				arrangeRect.Width = Math.Max(child.DesiredSize.Width, available - arrangeRect.X + Offset - GetLeftOffset(children, i));
				child.Arrange(arrangeRect);
				arrangeRect.X += child.RenderSize.Width;
			}
			return finalSize;
		}
		protected double GetDefaultInfoMinWidth() {
			if(Ribbon == null || Ribbon.HeaderToolbarContainer == null || !Ribbon.HeaderToolbarContainer.IsVisible)
				return 0d;
			double minWidth = Ribbon.Toolbar.GetMinDesiredWidth();
			minWidth = Ribbon.HeaderToolbarContainer.TranslatePoint(new Point(minWidth, 0), Ribbon).X;
			return Math.Max(Ribbon.TranslatePoint(new Point(minWidth, 0), this).X, 0d);
		}
		Point GetControlBoxOffset(RibbonControl ribbon) {
			if (ribbon.AutoHideMode && ribbon.ControlBoxContainer != null)
				return ribbon.ControlBoxContainer.TranslatePoint(new Point(), this);
			var container = ribbon.WindowHelper.GetControlBoxContainer();
			if (container != null)
				return container.TranslatePoint(new Point(), this);
			var window = Window.GetWindow(ribbon) as DXRibbonWindow;
			if(window != null) {
				var controlBoxRect = window.GetControlBoxRect();
				return ribbon.TranslatePoint(controlBoxRect.Location, this);
			}
			return new Point();
		}
		protected void UpdateControlBoxWidth(double available) {
			if (Ribbon.IsInRibbonWindow) {
				Point controlBoxOverlap = GetControlBoxOffset(Ribbon);
				ControlBoxWidth = Math.Max(0d, available - controlBoxOverlap.X);
			} else
				ClearValue(ControlBoxWidthProperty);
		}
		protected double GetLeftOffset(IEnumerable<FrameworkElement> children, int idx) {
			return children.Skip(idx + 1).Sum(elem => elem.DesiredSize.Width);
		}
		protected virtual int GetChildOrder(UIElement child) {
			RibbonPageCategoryControl cat = child as RibbonPageCategoryControl;
			if(cat == null || cat.Ribbon == null || !Ribbon.IsMerged)
				return 0;
			return cat.PageCategory.ActualMergeOrder;
		}
		protected internal virtual void DecreaseOffset() {
			Offset += GetNextOffsetValue(true);
			CheckOffset();
			InvalidateArrange();
		}
		private double GetNextOffsetValue(bool left) {
			if (ScrollHost == null) return 0d; 
			foreach (RibbonPageCategoryControl cControl in Children) {
				if(cControl.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
					continue;
				for (int i = 0; i < cControl.Items.Count; i++) {
					RibbonPageHeaderControl pHeader = cControl.ItemContainerGenerator.ContainerFromIndex(i) as RibbonPageHeaderControl;
					Point p = pHeader.TranslatePoint(new Point(), this);
					if(left && (p.X + pHeader.RenderSize.Width) > 0) {
						return p.X - pHeader.RenderSize.Width;
					} else if(p.X > 0.1) {
						return p.X;
					}
				}
			}
			return 0d;
		}
		protected internal virtual void IncreaseOffset() {
			Offset += GetNextOffsetValue(false);
			CheckOffset();
			InvalidateArrange();
		}
		protected internal virtual void CheckOffset() {
			if (ScrollHost != null) {
				Offset = Math.Max(Offset, Math.Min(ScrollHost.ViewportWidth - RenderSize.Width, 0));
			}
			Offset = Math.Min(0, Offset);
		}
	}
	public class RibbonItemsPanelOfficeSlim : RibbonItemsPanel {
		protected override Size MeasureOverride(Size availableSize) {
			Size desiredSize = new Size();
			List<RibbonPageCategoryHeaderInfo> infos = new List<RibbonPageCategoryHeaderInfo>(InternalChildren.Count);
			foreach (RibbonPageCategoryControl child in InternalChildren) {
				child.Measure(availableSize);
				var info = child.SizeInfo ?? new RibbonPageCategoryHeaderInfo() { Owner = child };
				desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
				infos.Add(info);
			}
			desiredSize.Width = infos.Sum(info => info.MinWidth);
			infos.Reverse();
			double newWidth = 0d;
			Size measureSize = new Size(0, availableSize.Height);
			foreach (var info in infos) {
				newWidth = desiredSize.Width - info.MinWidth + info.MaxWidth;
				desiredSize.Width -= info.MinWidth;
				measureSize.Width = newWidth <= availableSize.Width ? info.MaxWidth : Math.Max(availableSize.Width - desiredSize.Width, info.MinWidth);
				info.Owner.Measure(measureSize);
				desiredSize.Width += info.Owner.DesiredSize.Width;
			}
			return desiredSize;
		}
	}
	public class RibbonPageCategoryHeaderInfo : ICloneable {
		public RibbonPageCategoryHeaderInfo() : this(null) { }
		public RibbonPageCategoryHeaderInfo(UIElement owner) {
			Owner = owner;
			MaxWidth = 0d;
			DesiredWidth = 0d;
			MinWidth = 0d;
			MeasureSize = new Size(0d, 0d);
		}
		public UIElement Owner { get; set; }
		public double MaxWidth { get; set; }
		public double DesiredWidth { get; set; }
		public double MinWidth { get; set; }
		public Size MeasureSize { get; set; }
		public override bool Equals(object obj) {
			var second = obj as RibbonPageCategoryHeaderInfo;
			if (second == null) return false;
			return Owner.Equals(second.Owner)
				&& MaxWidth == second.MaxWidth
				&& DesiredWidth == second.DesiredWidth
				&& MinWidth == second.MinWidth;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		object ICloneable.Clone() {
			return new RibbonPageCategoryHeaderInfo() {
				Owner = this.Owner,
				MaxWidth = this.MaxWidth,
				DesiredWidth = this.DesiredWidth,
				MinWidth = this.MinWidth,
				MeasureSize = this.MeasureSize
			};
		}
		public RibbonPageCategoryHeaderInfo Clone() {
			return ((ICloneable)this).Clone() as RibbonPageCategoryHeaderInfo;
		}
	}
	public class RibbonOrderedPanel : Panel, INotifyPropertyChanged {
		public RibbonOrderedPanel() {
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateRibbon();
		}
		private RibbonControl ribbonCore = null;
		public RibbonControl Ribbon {
			get { return ribbonCore; }
			private set {
				if(ribbonCore == value)
					return;
				RibbonControl oldValue = ribbonCore;
				ribbonCore = value;
				if(PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Ribbon"));
				OnRibbonChanged(oldValue);
			}
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			InvalidateArrange();
		}
		void UpdateRibbon() {
			Ribbon = GetRibbon();
		}
		protected virtual RibbonControl GetRibbon() {
			return LayoutHelper.FindLayoutOrVisualParentObject<RibbonControl>(this, true);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			UpdateRibbon();
		}
		protected virtual int GetChildOrder(UIElement child) { return 0; }
		protected virtual bool IsOrdered { get { return false; } }
		protected override Size MeasureOverride(Size availableSize) {
			Size res = new Size(0, 0);
			foreach(UIElement page in Children) {
				page.Measure(availableSize);
				res.Width += page.DesiredSize.Width;
				res.Height = Math.Max(res.Height, page.DesiredSize.Height);
			}
			return res;
		}
		protected RibbonOrderedItemInfo[] GetOrderedChildren() {
			RibbonOrderedItemInfo[] orderedChildren = new RibbonOrderedItemInfo[Children.Count];
			for(int i = 0; i < Children.Count; i++) {
				UIElement child = Children[i];
				orderedChildren[i] = new RibbonOrderedItemInfo(i, GetChildOrder(child), child);
			}
			if(IsOrdered) {
				RibbonOrderedItemsHelper.SortByOrder(orderedChildren);
			}
			return orderedChildren;
		}
		sealed protected override Size ArrangeOverride(Size finalSize) {
			ArrangeOrderedChildren(GetOrderedChildren(), finalSize);
			return finalSize;
		}
		protected virtual Size ArrangeOrderedChildren(RibbonOrderedItemInfo[] items, Size finalSize) {
			Rect arrangRect = new Rect(new Size(0, finalSize.Height));
			foreach(RibbonOrderedItemInfo info in items) {
				UIElement child = info.Item as UIElement;
				arrangRect.Width = child.DesiredSize.Width;
				child.Arrange(arrangRect);
				arrangRect.X += arrangRect.Width;
			}
			return new Size(arrangRect.X, finalSize.Height);
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public class RibbonOrderedOnMergeItemsPanel : RibbonOrderedPanel {
#region static
		public static readonly DependencyProperty IsRibbonMergedProperty;
		static RibbonOrderedOnMergeItemsPanel() {
			IsRibbonMergedProperty = DependencyPropertyManager.Register("IsRibbonMerged", typeof(bool), typeof(RibbonOrderedOnMergeItemsPanel), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRibbonMergedPropertyChanged)));			
		}
		protected static void OnIsRibbonMergedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonOrderedOnMergeItemsPanel)d).OnIsRibbonMergedChanged((bool)e.OldValue);
		}											
#endregion
#region dep props
		public bool IsRibbonMerged {
			get { return (bool)GetValue(IsRibbonMergedProperty); }
			set { SetValue(IsRibbonMergedProperty, value); }
		}
#endregion
		public RibbonOrderedOnMergeItemsPanel() {
			SetBindings();
		}
		protected virtual void OnIsRibbonMergedChanged(bool oldValue) {
			InvalidateArrange();
		}
		protected virtual void SetBindings() {
			Binding bnd = new Binding("Ribbon.IsMerged");
			bnd.Source = this;
			bnd.Mode = BindingMode.OneWay;
			SetBinding(IsRibbonMergedProperty, bnd);
		}
		protected override bool IsOrdered {
			get {
				return IsRibbonMerged;
			}
		}
	}	
}
