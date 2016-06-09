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
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageGroupItemsPanel : Panel {
		#region static
		public static readonly DependencyProperty IsEndOfRowProperty;
		static readonly DependencyPropertyKey IsEndOfRowPropertyKey;
		public static readonly DependencyProperty ColumnProperty;
		static readonly DependencyPropertyKey ColumnPropertyKey;
		public static readonly DependencyProperty RowProperty;
		static readonly DependencyPropertyKey RowPropertyKey;
		static RibbonPageGroupItemsPanel() {
			ColumnPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("Column", typeof(int), typeof(RibbonPageGroupItemsPanel), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange));
			ColumnProperty = ColumnPropertyKey.DependencyProperty;
			RowPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("Row", typeof(int), typeof(RibbonPageGroupItemsPanel), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange));
			RowProperty = RowPropertyKey.DependencyProperty;
			IsEndOfRowPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsEndOfRow", typeof(bool), typeof(RibbonPageGroupItemsPanel), new PropertyMetadata(false));
			IsEndOfRowProperty = IsEndOfRowPropertyKey.DependencyProperty;
		}
		public static int GetColumn(DependencyObject d) { return (int)d.GetValue(ColumnProperty); }
		internal static void SetColumn(DependencyObject d, int value) { d.SetValue(ColumnPropertyKey, value); }
		public static int GetRow(DependencyObject d) { return (int)d.GetValue(RowProperty); }
		internal static void SetRow(DependencyObject d, int value) { d.SetValue(RowPropertyKey, value); }
		public static bool GetIsEndOfRow(DependencyObject d) { return (bool)d.GetValue(IsEndOfRowProperty); }
		internal static void SetIsEndOfRow(DependencyObject d, bool value) { d.SetValue(IsEndOfRowPropertyKey, value); }
		#endregion
		public RibbonPageGroupItemsPanel() { }
		RibbonPageGroupLayoutCalculator layoutCalculator;
		protected virtual RibbonPageGroupLayoutCalculator LayoutCalculator {
			get {
				if(layoutCalculator == null || layoutCalculator.RibbonStyle != RibbonStyle) {
					layoutCalculator = RibbonPageGroupLayoutCalculator.CreateLayoutCalculator(this);
					InvalidateMeasure();
				}
				return layoutCalculator;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			return LayoutCalculator.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return LayoutCalculator.ArrangeOverride(finalSize);
		}
		protected internal RibbonStyle RibbonStyle { get { return PageGroupControl == null ? RibbonStyle.Office2007 : PageGroupControl.RibbonStyle; } }
		protected internal RibbonControl Ribbon {
			get {
				if(PageGroupControl == null)
					return null;
				return PageGroupControl.Ribbon;
			}
		}
		protected internal RibbonPageGroupControl PageGroupControl {
			get { return ItemsControl.GetItemsOwner(this) as RibbonPageGroupControl; }
		}
	}
	public class ItemsRange {
		public ItemsRange() { }
		public ItemsRange(int startIndex, int count) {
			StartIndex = startIndex;
			Count = count;
		}
		public int StartIndex { get; set; }
		public int Count { get; set; }
		public int EndIndex { get { return StartIndex + Count - 1; } }
	}
	internal class RibbonControlLayoutHelper {
		public static readonly DependencyProperty IsItemCollapsedProperty;
		static RibbonControlLayoutHelper() {
			IsItemCollapsedProperty = DependencyPropertyManager.RegisterAttached("IsItemCollapsed", typeof(bool), typeof(RibbonControlLayoutHelper), new PropertyMetadata(false, OnIsItemCollapsedPropertyChanged));
		}
		public static bool GetIsItemCollapsed(DependencyObject obj) {
			return (bool)obj.GetValue(IsItemCollapsedProperty);
		}
		public static void SetIsItemCollapsed(DependencyObject obj, bool value) {
			obj.SetValue(IsItemCollapsedProperty, value);
		}
		static void OnIsItemCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue)
				d.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Collapsed);
			else
				d.ClearValue(UIElement.VisibilityProperty);
		}
	}
}
