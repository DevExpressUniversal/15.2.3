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
namespace DevExpress.DemoData.Helpers {
	public class EmptyPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			double width = 0.0;
			double height = 0.0;
			foreach(UIElement child in Children) {
				child.Measure(availableSize);
				if(child.DesiredSize.Width > width)
					width = child.DesiredSize.Width;
				if(child.DesiredSize.Height > height)
					height = child.DesiredSize.Height;
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach(UIElement child in Children) {
				child.Arrange(new Rect(new Point(), finalSize));
			}
			return finalSize;
		}
	}
	public class SelectedItemHelper : EmptyPanel {
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(SelectedItemHelper), new PropertyMetadata(null,
				(d, e) => ((SelectedItemHelper)d).OnSelectedItemChanged(e)));
		public static readonly DependencyProperty ComparingItemProperty =
			DependencyProperty.Register("ComparingItem", typeof(object), typeof(SelectedItemHelper), new PropertyMetadata(null,
				(d, e) => ((SelectedItemHelper)d).OnComparingItemChanged(e)));
		public static readonly DependencyProperty IsComparingItemSelectedProperty =
			DependencyProperty.Register("IsComparingItemSelected", typeof(bool), typeof(SelectedItemHelper), new PropertyMetadata(false,
				(d, e) => ((SelectedItemHelper)d).IsComparingItemSelectedChanged(e)));
		public object SelectedItem { get { return GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
		public object ComparingItem { get { return GetValue(ComparingItemProperty); } set { SetValue(ComparingItemProperty, value); } }
		public bool IsComparingItemSelected { get { return (bool)GetValue(IsComparingItemSelectedProperty); } set { SetValue(IsComparingItemSelectedProperty, value); } }
		void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e) {
			IsComparingItemSelected = object.Equals(e.NewValue, ComparingItem);
		}
		void OnComparingItemChanged(DependencyPropertyChangedEventArgs e) {
			IsComparingItemSelected = object.Equals(e.NewValue, SelectedItem);
		}
		void IsComparingItemSelectedChanged(DependencyPropertyChangedEventArgs e) {
			bool newValue = (bool)e.NewValue;
			if(!newValue && object.Equals(ComparingItem, SelectedItem))
				SelectedItem = null;
			if(newValue)
				SelectedItem = ComparingItem;
		}
	}
}
