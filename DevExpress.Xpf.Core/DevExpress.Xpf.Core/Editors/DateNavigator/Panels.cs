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
using System;
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Editors.DateNavigator.Controls {
	public interface IDateNavigatorContentPanelOwner {
		bool IsHidden { get; }
		UIElement CreateItem();
		Size GetItemSize();
		void ItemCountChanged();
		void UninitializeItem(UIElement item);
		void UpdateItemPositions();
	}
	public class DateNavigatorContentPanel : Panel {
		#region Static
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty RowCountProperty;
		static DateNavigatorContentPanel() {
			Type ownerType = typeof(DateNavigatorContentPanel);
			ColumnCountProperty = DependencyPropertyManager.Register("ColumnCount", typeof(int), ownerType,
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidatePropertyValueColumnCount);
			RowCountProperty = DependencyPropertyManager.Register("RowCount", typeof(int), ownerType,
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidatePropertyValueRowCount);
		}
		static bool ValidatePropertyValueColumnCount(object value) {
			return ((int)value) >= 0;
		}
		static bool ValidatePropertyValueRowCount(object value) {
			return ((int)value) >= 0;
		}
		#endregion
		int virtColumnCount = 1;
		int virtRowCount = 1;
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
		public int RowCount {
			get { return (int)GetValue(RowCountProperty); }
			set { SetValue(RowCountProperty, value); }
		}
		protected override Size ArrangeOverride(Size finalSize) {
			IDateNavigatorContentPanelOwner owner = DateNavigatorContent.GetPanelOwner(this);
			if (owner == null || owner.IsHidden)
				return base.ArrangeOverride(finalSize);
			Size itemSize = owner.GetItemSize();
			double desiredWidth = itemSize.Width;
			double desiredHeight = itemSize.Height;
			finalSize.Width = itemSize.Width * virtColumnCount;
			finalSize.Height = itemSize.Height * virtRowCount;
			int leftOffset = (int)((finalSize.Width - virtColumnCount * desiredWidth)) / 2;
			for (int i = 0; i < virtColumnCount; i++)
				for (int j = 0; j < virtRowCount; j++)
					Children[j * virtColumnCount + i].Arrange(new Rect(leftOffset + (i * desiredWidth), j * desiredHeight, desiredWidth, desiredHeight));
			return finalSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			IDateNavigatorContentPanelOwner owner = DateNavigatorContent.GetPanelOwner(this);
			if (owner == null)
				return base.MeasureOverride(availableSize);
			if (Children.Count == 0)
				Children.Add(owner.CreateItem());
			Children[0].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			if (owner.IsHidden)
				return base.MeasureOverride(availableSize);
			Size itemSize = owner.GetItemSize();
			if (double.IsInfinity(availableSize.Width))
				availableSize.Width = itemSize.Width;
			if (double.IsInfinity(availableSize.Height))
				availableSize.Height = itemSize.Height;
			if (ColumnCount == 0)
				virtColumnCount = Math.Max((int)(availableSize.Width / itemSize.Width), 1);
			else
				virtColumnCount = ColumnCount;
			if (RowCount == 0)
				virtRowCount = Math.Max((int)(availableSize.Height / itemSize.Height), 1);
			else
				virtRowCount = RowCount;
			int sumCount = virtColumnCount * virtRowCount;
			int delta = sumCount - Children.Count;
			if (delta > 0)
				for (int i = 1; i <= delta; i++)
					Children.Add(owner.CreateItem());
			else if (delta < 0)
				for (int i = 1; i <= -delta; i++) {
					owner.UninitializeItem(Children[sumCount]);
					Children.RemoveAt(sumCount);
				}
#if !SL
			if (delta != 0)
#endif
				for (int i = 1; i < sumCount; i++)
					Children[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			if (delta != 0)
				owner.ItemCountChanged();
			availableSize.Height = itemSize.Height * virtRowCount;
			availableSize.Width = itemSize.Width * virtColumnCount;
			owner.UpdateItemPositions();
			return availableSize;
		}
	}
	public class DateNavigatorPanel : Panel {
		protected override Size ArrangeOverride(Size finalSize) {
			if (Children.Count != 2)
				return base.ArrangeOverride(finalSize);
			Children[0].Arrange(new Rect(0, 0, finalSize.Width, Children[0].DesiredSize.Height));
			Children[1].Arrange(new Rect(0, Children[0].DesiredSize.Height, finalSize.Width, Children[1].DesiredSize.Height));
			return finalSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Children.Count != 2)
				return base.MeasureOverride(availableSize);
			Children[1].Measure(new Size(availableSize.Width, Double.PositiveInfinity));
			if (Double.IsInfinity(availableSize.Height))
				Children[0].Measure(availableSize);
			else
				Children[0].Measure(new Size(availableSize.Width, Math.Max(availableSize.Height - Children[1].DesiredSize.Height, 0)));
			Size result = new Size();
			if (double.IsInfinity(availableSize.Width))
				result.Width = Math.Max(Children[0].DesiredSize.Width, Children[1].DesiredSize.Width);
			else
				result.Width = availableSize.Width;
			result.Height = Children[0].DesiredSize.Height + Children[1].DesiredSize.Height;
			return result;
		}
	}
}
