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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class LegendItemsControl : ChartItemsControl {
		public static readonly DependencyProperty LegendItemsProperty = DependencyPropertyManager.Register("LegendItems",
			typeof(List<LegendItem>), typeof(LegendItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty ReverseItemsProperty = DependencyPropertyManager.Register("ReverseItems",
			typeof(bool), typeof(LegendItemsControl), new PropertyMetadata(PropertyChanged));
		[
		Category(Categories.Common),
		NonTestableProperty
		]
		public List<LegendItem> LegendItems {
			get { return (List<LegendItem>)GetValue(LegendItemsProperty); }
			set { SetValue(LegendItemsProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public bool ReverseItems {
			get { return (bool)GetValue(ReverseItemsProperty); }
			set { SetValue(ReverseItemsProperty, value); }
		}
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LegendItemsControl itemsControl = d as LegendItemsControl;
			if (itemsControl != null)
				itemsControl.Update();
		}
		IEnumerable<LegendItem> ActualItems {
			get {
				if (ReverseItems) {
					for (int i = LegendItems.Count - 1; i >= 0; i--)
						yield return LegendItems[i];
				}
				else {
					foreach (LegendItem legendItem in LegendItems)
						yield return legendItem;
				}
			}
		}
		void Update() {
			ObservableCollection<LegendItem> items = new ObservableCollection<LegendItem>();
			if (LegendItems != null) {
				foreach (LegendItem legendItem in ActualItems) {
					items.Add(legendItem);
				}
			}
			ItemsSource = items;
		}
	}
}
