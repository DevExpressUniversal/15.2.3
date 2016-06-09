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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Printing.Native.Lines {
	internal class LinesGrid : Grid {
		protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint) {
			var baseResult = base.MeasureOverride(constraint);
			return new Size(Math.Min(baseResult.Width, constraint.Width), Math.Min(baseResult.Height, constraint.Height));
		}
		public LinesGrid()
			: base() {
		}
	}
	internal class AutoWidthColumnBehavior : Behavior<LinesGrid> {
		public int AutoWidthColumnIndex {
			get { return (int)GetValue(AutoWidthColumnIndexProperty); }
			set { SetValue(AutoWidthColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty AutoWidthColumnIndexProperty =
			DependencyProperty.Register("AutoWidthColumnIndex", typeof(int), typeof(AutoWidthColumnBehavior), new PropertyMetadata(-1));
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.SizeChanged += OnSizeChanged;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.SizeChanged -= OnSizeChanged;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(AssociatedObject.ColumnDefinitions.Count <= 1 || AutoWidthColumnIndex < 0 || AutoWidthColumnIndex >= AssociatedObject.ColumnDefinitions.Count)
				return;
			var availableWidth = CalculateAvailableWidth();
			var measuredWidth = CalculateColumnChildrenWidth();
			AssociatedObject.ColumnDefinitions[AutoWidthColumnIndex].Width = new GridLength(Math.Min(availableWidth, measuredWidth), GridUnitType.Pixel);
		}
		double CalculateColumnChildrenWidth() {
			double childrenMaxWidth = 0;
			var children = GetColumnChildren();
			foreach(var child in children) {
				child.Measure(new Size(Double.PositiveInfinity, child.DesiredSize.Height));
				childrenMaxWidth = Math.Max(childrenMaxWidth, child.DesiredSize.Width);
			}
			return childrenMaxWidth + 1;
		}
		double CalculateAvailableWidth() {
			double columnsMinWidth = 0;
			foreach(var column in AssociatedObject.ColumnDefinitions) {
				if(AssociatedObject.ColumnDefinitions.IndexOf(column) == AutoWidthColumnIndex)
					continue;
				columnsMinWidth += Math.Min(column.ActualWidth, column.MinWidth);
			}
			var result = AssociatedObject.ActualWidth - columnsMinWidth;
			return result < 0 ? 0 : result;
		}
		IEnumerable<UIElement> GetColumnChildren() {
			return from c in AssociatedObject.Children.Cast<FrameworkElement>()
				   where Grid.GetColumn(c) == AutoWidthColumnIndex
				   select c;
		}
	}
}
