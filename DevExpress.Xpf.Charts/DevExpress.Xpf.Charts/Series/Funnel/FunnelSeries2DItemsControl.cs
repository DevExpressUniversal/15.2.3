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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class FunnelSeries2DItemsControl : Diagram2DItemsControl {
		#region Dependency properties
		public static readonly DependencyProperty SeriesProperty = DependencyPropertyManager.Register("Series",
			typeof(Series), typeof(FunnelSeries2DItemsControl));
		public static readonly DependencyProperty PointsContainerProperty = DependencyPropertyManager.Register("PointsContainer",
			typeof(ItemsControl), typeof(FunnelSeries2DItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SeriesLabelProperty = DependencyPropertyManager.Register("SeriesLabel",
			typeof(SeriesLabel), typeof(FunnelSeries2DItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SeriesLabelItemsProperty = DependencyPropertyManager.Register("SeriesLabelItems",
			typeof(ObservableCollection<SeriesLabelItem>), typeof(FunnelSeries2DItemsControl), new PropertyMetadata(PropertyChanged));
		#endregion
		[
		Category(Categories.Common)
		]
		public Series Series {
			get { return (FunnelSeries2D)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public ItemsControl PointsContainer {
			get { return (ItemsControl)GetValue(PointsContainerProperty); }
			set { SetValue(PointsContainerProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public SeriesLabel SeriesLabel {
			get { return (SeriesLabel)GetValue(SeriesLabelProperty); }
			set { SetValue(SeriesLabelProperty, value); }
		}
		[
		Category(Categories.Common),
		NonTestableProperty
		]
		public ObservableCollection<SeriesLabelItem> SeriesLabelItems {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(SeriesLabelItemsProperty); }
			set { SetValue(SeriesLabelItemsProperty, value); }
		}
		protected override ObservableCollection<object> CreateItems() {
			ObservableCollection<object> items = new ObservableCollection<object>();
			if (PointsContainer != null)
				items.Add(PointsContainer);
			if (SeriesLabelItems != null) {
				foreach (SeriesLabelItem labelItem in SeriesLabelItems)
					if (labelItem.ConnectorItem != null)
						items.Add(labelItem.ConnectorItem);
				foreach (SeriesLabelItem labelItem in SeriesLabelItems)
					items.Add(labelItem);
			}
			return items;
		}
	}
}
