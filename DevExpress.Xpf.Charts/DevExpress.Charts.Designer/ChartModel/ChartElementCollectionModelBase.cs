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
using System.Collections.ObjectModel;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ChartElementCollectionModelBase : ChartModelElement {
		public delegate void ChartCollectionUpdateEvent(ChartCollectionUpdateEventArgs args);
		readonly ObservableCollection<ChartModelElement> modelCollection;
		int nameIndexer;
		public override IEnumerable<ChartModelElement> Children {
			get { return modelCollection; }
		}
		public IList<ChartModelElement> ModelCollection {
			get { return modelCollection; }
		}
		public ChartElementCollectionModelBase(ChartModelElement parent, object chartElement)
			: base(parent, chartElement) {
			this.modelCollection = new ObservableCollection<ChartModelElement>();
			this.nameIndexer = 0;
		}
		public event ChartCollectionUpdateEvent CollectionUpdated;
		protected abstract ChartModelElement CreateModel(object chartElement, int index);
		protected string GetNameIndexer() {
			return (++nameIndexer).ToString();
		}
		protected void OnCollectionUpdated(IEnumerable<ChartModelElement> removedItems, IEnumerable<InsertedItem> addedItems) {
			if (CollectionUpdated != null)
				CollectionUpdated(new ChartCollectionUpdateEventArgs(this, removedItems, addedItems));
		}
		[ SkipOnPropertyChangedMethodCall ]
		protected override void UpdateChildren() {
			List<ChartModelElement> remains = new List<ChartModelElement>(modelCollection);
			List<InsertedItem> added = new List<InsertedItem>();
			IEnumerable<object> sourceCollection = (IEnumerable<object>)ChartElement;
			int index = 0;
			foreach (object existing in sourceCollection) {
				ChartModelElement founded = PerformChildSearch(existing);
				if (founded == null) {
					ChartModelElement model = CreateModel(existing, index);
					if (model != null)
						added.Add(new InsertedItem(index, model));
				}
				else
					remains.Remove(founded);
				index++;
			}
			foreach (ChartModelElement remove in remains)
				modelCollection.Remove(remove);
			foreach (InsertedItem add in added)
				modelCollection.Insert(add.Index, add.Item);
			if (remains.Count > 0 || added.Count > 0)
				OnCollectionUpdated(remains, added);
			OnPropertyChanged("Children");
		}
		public int IndexOf(ChartModelElement childElement) {
			return modelCollection.IndexOf(childElement);
		}
	}
	public class ChartCollectionUpdateEventArgs : EventArgs {
		IEnumerable<InsertedItem> addedItems;
		IEnumerable<ChartModelElement> removedItems;
		ChartModelElement sender;
		public IEnumerable<InsertedItem> AddedItems {
			get { return addedItems; }
		}
		public IEnumerable<ChartModelElement> RemovedItems {
			get { return removedItems; }
		}
		public ChartModelElement Sender {
			get { return sender; }
		}
		public ChartCollectionUpdateEventArgs(ChartModelElement sender, IEnumerable<ChartModelElement> removedItems, IEnumerable<InsertedItem> addedItems) {
			this.sender = sender;
			this.removedItems = removedItems;
			this.addedItems = addedItems;
		}
	}
}
