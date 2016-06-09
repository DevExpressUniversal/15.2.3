#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
namespace DevExpress.ExpressApp.Utils {
	public interface INavigationHistorySequenceStrategy<ItemType> {
		void Setup(List<HistoryItem<ItemType>> items);
		bool Add(string caption, string imageName, ItemType item, ref int currentPosition);
		bool SetCurrentItem(ItemType item, ref int currentPosition);
		bool DeleteCurrentItem(ref int currentPosition);
	}
	public class IENavigationHistorySequenceStrategy<ItemType> : INavigationHistorySequenceStrategy<ItemType> {
		protected List<HistoryItem<ItemType>> items;
		protected bool IsEmpty {
			get { return (items.Count == 0); }
		}
		public IENavigationHistorySequenceStrategy(int historyDepth) {
			this.historyDepth = historyDepth;
		}
		private int historyDepth;
		public int HistoryDepth {
			get { return historyDepth; }
			set { historyDepth = value; }
		}
		#region INavigationHistoryCollectSequenceStrategy<ItemType> Members
		public virtual void Setup(List<HistoryItem<ItemType>> items) {
			this.items = items;
		}
		public virtual bool Add(string caption, string imageName, ItemType item, ref int currentPosition) {
			if(IsEmpty || !items[currentPosition].Item.Equals(item)) {
				int nextPosition = currentPosition + 1;
				if(nextPosition < items.Count) {
					items.RemoveRange(nextPosition, items.Count - nextPosition);
				}
				if(nextPosition == historyDepth) {
					items.RemoveAt(0);
					nextPosition = historyDepth - 1;
				}
				items.Add(new HistoryItem<ItemType>(caption, imageName, item));
				currentPosition = items.Count - 1;
				return true;
			}
			return false;
		}
		public virtual bool SetCurrentItem(ItemType item, ref int currentPosition) {
			if(item != null) {
				for(int i = items.Count - 1; i >= 0; i--) {
					if(items[i].Item.Equals(item)) {
						currentPosition = i;
						return true;
					}
				}
			}
			return false;
		}
		public virtual bool DeleteCurrentItem(ref int currentPosition) {
			items.RemoveAt(currentPosition);
			currentPosition--;
			return true;
		}
		#endregion
	}
	public class BreadCrumbsNavigationHistorySequenceStrategy<ItemType> : IENavigationHistorySequenceStrategy<ItemType> {
		public BreadCrumbsNavigationHistorySequenceStrategy()
			: this(-1) {
		}
		public BreadCrumbsNavigationHistorySequenceStrategy(int historyDepth)
			: base(historyDepth) {
		}
		public override bool Add(string caption, string imageName, ItemType item, ref int currentPosition) {
			HistoryItem<ItemType> newItem = new HistoryItem<ItemType>(caption, imageName, item);
			int indexOfNewItem = items.IndexOf(newItem);
			if(IsEmpty || indexOfNewItem == -1) {
				AddToTheEnd(newItem, ref currentPosition);
				return true;
			}
			else if(indexOfNewItem != items.Count - 1) {
				SetCurrentItem(indexOfNewItem, ref currentPosition);
				return true;
			}
			return false;
		}
		private void AddToTheEnd(HistoryItem<ItemType> newItem, ref int currentPosition) {
			if(items.Count + 1 == HistoryDepth) {
				items.RemoveAt(0);
			}
			items.Add(newItem);
			currentPosition = GetCurrentPosition();
		}
		private int GetCurrentPosition() {
			return items.Count - 1;
		}
		public override bool SetCurrentItem(ItemType item, ref int currentPosition) {
			if(item != null) {
				for(int i = 0; i < items.Count; i++) {
					if(items[i].Item.Equals(item)) {
						SetCurrentItem(i, ref currentPosition);
						return true;
					}
				}
			}
			return false;
		}
		private void SetCurrentItem(int index, ref int currentPosition) {
			HistoryItem<ItemType> item = items[index];
			items.RemoveAt(index);
			AddToTheEnd(item, ref currentPosition);
		}
		public override bool DeleteCurrentItem(ref int currentPosition) {
			items.RemoveAt(currentPosition);
			currentPosition = GetCurrentPosition();
			return true;
		}
	}
	public struct HistoryItem<ItemType> {
		private ItemType item;
		private string caption;
		private string imageName;
		public HistoryItem(string caption, string image, ItemType item) {
			this.imageName = image;
			this.caption = caption;
			this.item = item;
		}
		public ItemType Item {
			get { return item; }
			set { item = value; }
		}
		public string Caption {
			get { return caption; }
			set { caption = value; }
		}
		public string ImageName {
			get { return imageName; }
			set { imageName = value; }
		}
		public override bool Equals(object obj) {
			if(obj is HistoryItem<ItemType>) {
				HistoryItem<ItemType> item = (HistoryItem<ItemType>)obj;
				if( item.Item.Equals(Item)) {
					return true;
				}
			}
			return false;
		}
		public override int GetHashCode() {
			return (Item.GetHashCode().ToString() + Caption).GetHashCode();
		}
		public override string ToString() {
			return Caption;
		}
	}
	public class NavigationHistory<ItemType> : IEnumerable<HistoryItem<ItemType>> {
		private INavigationHistorySequenceStrategy<ItemType> collectSequenceStrategy;
		private List<HistoryItem<ItemType>> list = new List<HistoryItem<ItemType>>();
		private int currentPosition = -1;
		private void SetCollectSequenceStrategy(INavigationHistorySequenceStrategy<ItemType> strategy) {
			collectSequenceStrategy = strategy;
			collectSequenceStrategy.Setup(list);
		}
		private bool IsEmpty {
			get { return (list.Count == 0); }
		}
		protected void RaiseOnChanged() {
			if(OnChanged != null) {
				OnChanged(this, EventArgs.Empty);
			}
		}
		public void Add(ItemType item) {
			Add(string.Empty, item);
		}
		public void Add(string caption, ItemType item) {
			if(collectSequenceStrategy.Add(caption, null, item, ref currentPosition)) {
				RaiseOnChanged();
			}
		}
		public void Add(string caption, string imageName, ItemType item) {
			if(collectSequenceStrategy.Add(caption, imageName, item, ref currentPosition)) {
				RaiseOnChanged();
			}
		}
		public HistoryItem<ItemType> Forward() {
			if(!IsEmpty) {
				if (CanForward) {
					currentPosition++;
				}
				return list[currentPosition];
			}
			return default(HistoryItem<ItemType>);
		}
		public HistoryItem<ItemType> Back() {
			if(!IsEmpty) {
				if(CanBack) {
					currentPosition--;
				}
				if(!IsEmpty) {
					return list[currentPosition];
				}
			}
			return default(HistoryItem<ItemType>);
		}
		public HistoryItem<ItemType> Peek() {
			if(!IsEmpty) {
				if(CanBack) {
					return list[currentPosition - 1];
				}
			}
			return default(HistoryItem<ItemType>);
		}
		public void SetCurrentPosition(ItemType item) {
			if(collectSequenceStrategy.SetCurrentItem(item, ref currentPosition)) {
				RaiseOnChanged();
			}
		}
		public void Clear() {
			currentPosition = -1;
			list.Clear();
			RaiseOnChanged();
		}
		public bool CanBack {
			get { return (currentPosition > 0) && (list.Count > 0); }
		}
		public bool CanForward {
			get { return (currentPosition + 1 < list.Count); }
		}
		public void UpdateCurrentItem(string caption, string imageName, ItemType item) {
			list[currentPosition] = new HistoryItem<ItemType>(caption, imageName, item);
			RaiseOnChanged();
		}
		public void DeleteCurrentItem() {
			if(currentPosition > -1 && collectSequenceStrategy.DeleteCurrentItem(ref currentPosition)) {
				RaiseOnChanged();
			}
		}
		public void DeleteItem(ItemType item) {
			list.Remove(new HistoryItem<ItemType>(string.Empty, null, item));
			currentPosition--;
			RaiseOnChanged();
		}
		public int IndexOf(ItemType item) {
			return list.IndexOf(new HistoryItem<ItemType>(string.Empty, null, item));
		}
		public int IndexOf(string caption, ItemType item) {
			return list.IndexOf(new HistoryItem<ItemType>(caption, null, item));
		}
		public INavigationHistorySequenceStrategy<ItemType> CollectSequenceStrategy {
			get { return collectSequenceStrategy; }
			set { SetCollectSequenceStrategy(value); }
		}
		public int Count {
			get { return list.Count; }
		}
		public int CurrentPositionIndex {
			get { return currentPosition; }
			set { currentPosition = value; }
		}
		public HistoryItem<ItemType> CurrentPosition {
			get {
				if(!IsEmpty) {
					return list[currentPosition];
				}
				return default(HistoryItem<ItemType>);
			}
		}
		public HistoryItem<ItemType> GetCurrentPositionByIndex(int index) {
			if(!IsEmpty) {
				return list[index];
			}
			return default(HistoryItem<ItemType>);
		}
		public NavigationHistory(INavigationHistorySequenceStrategy<ItemType> collectSequenceStrategy) {
			CollectSequenceStrategy = collectSequenceStrategy;
		}
		public NavigationHistory() : this(-1) {
		}
		public NavigationHistory(int historyDepth) {
			CollectSequenceStrategy = new IENavigationHistorySequenceStrategy<ItemType>(historyDepth);
		}
		#region IEnumerable<ItemType> Members
		public IEnumerator<HistoryItem<ItemType>> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)list).GetEnumerator();
		}
		#endregion
		public event EventHandler OnChanged;
	}
}
