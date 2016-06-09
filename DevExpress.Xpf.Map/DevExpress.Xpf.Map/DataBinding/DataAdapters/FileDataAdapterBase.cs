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

using DevExpress.Map.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Xpf.Map.Native {
	public interface IFileDataAdapter {
		MapLoaderCore<MapItem> ItemsLoader { get; }
		MapVectorItemCollection ItemsCollection { get; }
		event EventHandler<ItemsLoadedEventArgs<MapItem>> ItemsLoaded;
		event EventHandler<BoundsCalculatedEventArgs> BoundsCalculated;
	}
	public class FileDataAdapter : IFileDataAdapter {
		readonly MapLoaderCore<MapItem> itemsLoader;
		readonly MapVectorItemCollection itemsCollection;
		public MapLoaderCore<MapItem> ItemsLoader { get { return itemsLoader; } }
		public MapVectorItemCollection ItemsCollection { get { return itemsCollection; } }
		public event EventHandler<ItemsLoadedEventArgs<MapItem>> ItemsLoaded;
		public event EventHandler<BoundsCalculatedEventArgs> BoundsCalculated;
		public FileDataAdapter(MapLoaderCore<MapItem> itemsLoader) {
			this.itemsLoader = itemsLoader;
			this.itemsLoader.ItemsLoaded += OnItemsLoaded;
			this.itemsLoader.BoundsCalculated += OnBoundsCalculated;
			this.itemsCollection = new MapVectorItemCollection();
		}
		void RaiseItemsLoadedEvent(List<MapItem> mapItems) {
			if (ItemsLoaded != null)
				ItemsLoaded(this, new ItemsLoadedEventArgs<MapItem>(ItemsLoader.Items));
		}
		void RaiseBoundsCalculatedEvent(CoordBounds bounds) {
			if (BoundsCalculated != null)
				BoundsCalculated(this, new BoundsCalculatedEventArgs(bounds));
		}
		void OnItemsLoaded(object sender, ItemsLoadedEventArgs<MapItem> e) {
			RaiseItemsLoadedEvent(e.Items);
			ItemsCollection.BeginUpdate();
			CopyItems(e.Items, itemsCollection);
			ItemsCollection.EndUpdate();
		}
		void OnBoundsCalculated(object sender, BoundsCalculatedEventArgs e) {
			RaiseBoundsCalculatedEvent(e.Bounds);
		}
		void CopyItems(IList<MapItem> source, MapVectorItemCollection target) {
			target.Clear();
			foreach (MapItem item in source)
				target.Insert(0, item);
		}
	}
	public class FileDataAdapterController {
		readonly IFileDataAdapter dataAdapter;
		public FileDataAdapterController(IFileDataAdapter dataAdapter) {
			this.dataAdapter = dataAdapter;
		}
	}
}
