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
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Diagram2DItemsControl : ChartItemsControl {
		protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Diagram2DItemsControl itemsControl = d as Diagram2DItemsControl;
			if (itemsControl != null)
				itemsControl.InvalidateMeasure();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return !ShouldCreateItemPresentation(item) && base.IsItemItsOwnContainerOverride(item);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new LayoutElementPresentation();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			LayoutElementPresentation layoutElementPresentation = element as LayoutElementPresentation;
			if (layoutElementPresentation != null)
				layoutElementPresentation.Content = GetItemPresentation(item);
		}
		protected override Size MeasureOverride(Size availableSize) {
			ObservableCollection<object> items = ItemsSource as ObservableCollection<object>;
			if (items == null) {
				ItemsSource = CreateItems();
				DetachOldItemsParents();
			}
			else {
				int itemsCount = items.Count;
				ObservableCollection<object> newItems = CreateItems();
				int newItemsCount = newItems.Count;
				for (int i = 0; i < newItemsCount; i++) {
					object newItem = newItems[i];
					if (i >= itemsCount) {
						items.Add(newItem);
						itemsCount++;
					}
					else {
						object existingItem = items[i];
						if (!Object.ReferenceEquals(existingItem, newItem))
							if (items.Contains(newItem))
								while (!Object.ReferenceEquals(existingItem, newItem) && existingItem != null) {
									items.RemoveAt(i);
									if (i == items.Count)
										existingItem = null;
									else
										existingItem = items[i];
									itemsCount--;
								}
							else {
								items.Insert(i, newItem);
								itemsCount++;
							}
					}
				}
				while (itemsCount > newItemsCount)
					items.RemoveAt(--itemsCount);
			}
			return base.MeasureOverride(availableSize);
		}
		protected virtual bool ShouldCreateItemPresentation(object item) {
			return (item is SeriesLabelItem )|| (item is SeriesLabelConnectorItem);
		}
		protected virtual ILayoutElement GetItemPresentation(object item) {
			SeriesLabelItem seriesLabelItem = item as SeriesLabelItem;
			if (seriesLabelItem != null)
				return new SeriesLabelPresentation(seriesLabelItem);
			SeriesLabelConnectorItem seriesLabelConnectorItem = item as SeriesLabelConnectorItem;
			if (seriesLabelConnectorItem != null)
				return new SeriesLabelConnectorPresentation(seriesLabelConnectorItem);
			return null;				
		}
		protected abstract ObservableCollection<object> CreateItems();
	}
}
