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
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	public class BarItemLinkInfoCollection : ObservableCollection<BarItemLinkInfo> {
		public event EventHandler RequestReset;				
		public BarItemLinkInfoCollection(IList<BarItemLinkBase> sourceCollection) {
			Source = sourceCollection;
		}
		BarItemLinkInfoSegmentCollection segmentsCore;
		internal BarItemLinkInfoSegmentCollection Segments {
			get {
				if(segmentsCore == null) {
					segmentsCore = new BarItemLinkInfoSegmentCollection();
				}
				return segmentsCore;
			}
		}
		IList<BarItemLinkBase> sourceCore;
		int enableModifyCollectionCounter;
		bool CanModifyCollection { get { return enableModifyCollectionCounter > 0; } }
		private bool haveVisibleInfos;
		public bool HaveVisibleInfos {
			get { return haveVisibleInfos; }
			set {
				if (value == haveVisibleInfos)
					return;
				bool oldValue = haveVisibleInfos;
				haveVisibleInfos = value;
				OnHaveVisibleInfosChanged(oldValue);
			}
		}
		protected virtual void OnHaveVisibleInfosChanged(bool oldValue) {
			if (HaveVisibleInfosChanged != null) {
				HaveVisibleInfosChanged(this, new EventArgs());
			}
		}
		public event EventHandler HaveVisibleInfosChanged;
		void EnableModifyCollection() {
			enableModifyCollectionCounter++;
		}
		void DisableModifyCollection() {
			enableModifyCollectionCounter--;
			if(enableModifyCollectionCounter < 0)
				throw new InvalidOperationException("EnableModifyCollectionCounter cannot be negative.");
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(!CanModifyCollection) {
				throw new NotSupportedException("Cannot modify the ReadOnly collection.");
			}
			base.OnCollectionChanged(e);
			UpdateVisibleInfos();
		}
		protected internal void UpdateVisibleInfos() {
			HaveVisibleInfos = this.Any(info => info.With(x => x.Link).Return(x => x.ActualIsVisible, () => true));
		}
		protected internal virtual void UpdateFromSource(NotifyCollectionChangedEventArgs args = null) {
			if(IsLocked) return;
			args = args ?? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			int index = -1;
			switch(args.Action) {
				case NotifyCollectionChangedAction.Add: {
						BarItemLinkBase linkBase = args.NewItems[0] as BarItemLinkBase;
						index = args.NewStartingIndex;
						OnLinkInserted(index, null, linkBase);
					}
					break;
				case NotifyCollectionChangedAction.Remove: {
						BarItemLinkBase linkBase = args.OldItems[0] as BarItemLinkBase;
						index = args.OldStartingIndex;
						OnLinkRemoved(index, null, linkBase);
					}
					break;
				case NotifyCollectionChangedAction.Replace: {
						BarItemLinkBase linkBase = args.OldItems[0] as BarItemLinkBase;
						index = args.NewStartingIndex;
						OnLinkRemoved(index, null, linkBase);
						linkBase = args.NewItems[0] as BarItemLinkBase;
						index = args.NewStartingIndex;
						OnLinkInserted(index, null, linkBase);
					}
					break;
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Reset: {
						ClearLinkInfos();
						if(Source == null)
							return;
						index = 0;
						foreach(BarItemLinkBase linkBase in Source) {
							OnLinkInserted(index, null, linkBase);
							index++;
						}
					}
					break;
			}
		}
		public void LinkInfoChanged(BarItemLinkInfo barItemLinkInfo) {
			if (RequestReset == null)
				return;
			RequestReset(this, EventArgs.Empty);
		}
		protected virtual void OnLinkInserted(int index, BarItemLinkInfo header, BarItemLinkBase linkBase) {
			InsertLink(header, linkBase, index);
		}
		protected virtual void OnLinkRemoved(int index, BarItemLinkInfo header, BarItemLinkBase linkBase) {
			BarItemLinkInfoSegmentPosition pos = Segments.GetPositionInfo(header, index);
			if(pos.IsLevelHeader) {
				RemoveHeader(pos.LinearIndex, pos.HeaderSegmentIndex, false);
			} else {
				RemoveLinkInfo(pos.LinearIndex, pos.SegmentIndex);
				RemoveUnusedSegment(pos.SegmentIndex);
			}
		}
		protected virtual void RemoveUnusedSegment(int segmentIndex) {
			if(Segments[segmentIndex].Count == 0 && !Segments[segmentIndex].IsLevelHeader && segmentIndex != 0) {
				Segments.RemoveAt(segmentIndex);
			}
		}
		protected virtual void RemoveLinkInfo(int linearIndex, int segmentIndex) {
			RemoveLinkInfoCore(linearIndex);
			Segments[segmentIndex].Count--;
			if(Segments[segmentIndex].Count < 0)
				throw new InvalidOperationException("Count cannot be negative.");
		}
		internal virtual void ClearSegment(BarItemLinkInfoSegment segment) {
			if(segment.Header == null)
				return;
			segment.Header.LinkBase = null;
			segment.Header = null;
		}
		protected virtual void RemoveHeader(int linearIndex, int headerSegmentIndex, bool removeChildrenOnly) {
			int level = Segments[headerSegmentIndex].Level;
			bool isFirstSegment = true;
			do {
				while(Segments[headerSegmentIndex].Count != 0) {
					RemoveLinkInfo(linearIndex, headerSegmentIndex);
				}
				if(isFirstSegment && removeChildrenOnly) {					
					headerSegmentIndex++;
				}
				else {
					ClearSegment(Segments[headerSegmentIndex]);
					Segments.RemoveAt(headerSegmentIndex);			   
				}
				isFirstSegment = false;
			} while(headerSegmentIndex != Segments.Count && (Segments[headerSegmentIndex].Level == level && !Segments[headerSegmentIndex].IsLevelHeader || Segments[headerSegmentIndex].Level > level));
			if(headerSegmentIndex > 0 && headerSegmentIndex < Segments.Count && !removeChildrenOnly) {
				if(Segments[headerSegmentIndex - 1].Level == Segments[headerSegmentIndex].Level && !Segments[headerSegmentIndex].IsLevelHeader) {
					Segments[headerSegmentIndex - 1].Count += Segments[headerSegmentIndex].Count;
					Segments.RemoveAt(headerSegmentIndex);
				}
			}
		}
		IInplaceLinksHolder GetInplaceLinksHolder(BarItemLinkBase linkBase) {
			return GetInplaceLinksHolder((linkBase as BarItemLink).With(i => i.Item));
		}
		IInplaceLinksHolder GetInplaceLinksHolder(BarItem item) {			
			return item as IInplaceLinksHolder;
		}
		protected virtual void InsertSingleLinkInfo(int linearIndex, BarItemLinkInfo info, int segmentIndex) {
			EnableModifyCollection();
			this.Insert(linearIndex, info);
			DisableModifyCollection();
			Segments[segmentIndex].Count++;
		}
		protected virtual void InsertLink(BarItemLinkInfo header, BarItemLinkBase link, int index) {
			BarItemLinkInfo linkInfo = new BarItemLinkInfo(link);
			BarItemLinkInfoSegmentPosition pos = Segments.GetPositionInfo(header, index);
			IInplaceLinksHolder holder = GetInplaceLinksHolder(linkInfo.Link);
			linkInfo.OwnerCollection = this;
			if(holder == null) {
				if(pos.NeedSegmentCreation)
					Segments.Insert(pos.SegmentIndex, new BarItemLinkInfoSegment() { Level = pos.SegmentLevel });
				InsertSingleLinkInfo(pos.LinearIndex, linkInfo, pos.SegmentIndex);
				return;
			}
			int segmentIndex = pos.SegmentIndex;
			if(pos.NeedSegmentCreation)
				Segments.Insert(pos.SegmentIndex, new BarItemLinkInfoSegment() { Level = pos.SegmentLevel + 1, Header = linkInfo });
			else {
				Segments.SplitSegment(pos.SegmentIndex, pos.SegmentOffset, linkInfo);
				segmentIndex++;
			}
			int linearIndex = pos.LinearIndex;
			InsertChildrenLinkInfos(ref linearIndex, ref segmentIndex);
		}
		protected virtual void InsertChildrenLinkInfos(ref int linearIndex, ref int segmentIndex) {
			BarItemLink link = Segments[segmentIndex].Header.LinkBase as BarItemLink;
			IInplaceLinksHolder holder = GetInplaceLinksHolder(Segments[segmentIndex].Header.LinkBase);
			bool needCreateNewSegment = false;
			int level = Segments[segmentIndex].Level;
			if(holder != null) {
				foreach(BarItemLinkBase linkBase in holder.ActualLinks) {
					BarItemLinkInfo info = new BarItemLinkInfo(linkBase);
					info.OwnerCollection = this;
					IInplaceLinksHolder linkHolder = GetInplaceLinksHolder(linkBase);
					if(linkHolder != null) {
						Segments.Insert(segmentIndex + 1, new BarItemLinkInfoSegment() { Header = info, Level = Segments[segmentIndex].Level + 1 });
						segmentIndex++;
						InsertChildrenLinkInfos(ref linearIndex, ref segmentIndex);
						needCreateNewSegment = true;
					} else {
						if(needCreateNewSegment) {
							segmentIndex++;
							Segments.Insert(segmentIndex, new BarItemLinkInfoSegment() { Level = level });
							needCreateNewSegment = false;
						}
						InsertSingleLinkInfo(linearIndex, info, segmentIndex);
						linearIndex++;
					}
				}
			}
		}
		protected virtual void OnLinkInfoRemoved(BarItemLinkInfo linkInfo) {
			linkInfo.LinkBase = null;
			linkInfo.OwnerCollection = null;
		}
		protected void RemoveLinkInfoCore(int linearIndex) {
			BarItemLinkInfo info = this[linearIndex];
			EnableModifyCollection();
			this.RemoveAt(linearIndex);
			DisableModifyCollection();
			OnLinkInfoRemoved(info);
		}
		protected void ClearLinkInfos() {
			while(this.Count != 0) {
				RemoveLinkInfoCore(0);
			}
			for(int i = 0; i < Segments.Count; i++) {
				ClearSegment(Segments[i]);
			}
			Segments.Clear();		   
			Segments.Add(new BarItemLinkInfoSegment());
		}
		public IList<BarItemLinkBase> Source {
			get { return sourceCore; }
			set {
				if(Source == value)
					return;
				UnsubscribeCollectionEvents();
				sourceCore = value;
				SubscribeCollectionEvents();
				UpdateFromSource();
			}
		}
		public void LockCollection() {
			lockCollectionCounter++;
		}
		public void UnlockCollection() {
			lockCollectionCounter--;
			if(lockCollectionCounter < 0)
				throw new InvalidOperationException("Cannot unlock collection. Collection is already unlocked.");
			if(lockCollectionCounter == 0)
				UpdateFromSource();
		}
		public bool IsLocked { get { return lockCollectionCounter > 0; } } 
		private void SubscribeCollectionEvents() {
			var incc = Source as INotifyCollectionChanged;
			if (incc != null)
				incc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSourceCollectionChanged);
		}
		void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateFromSource(e);
		}
		private void UnsubscribeCollectionEvents() {
			var incc = Source as INotifyCollectionChanged;
			if (incc != null)
				incc.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSourceCollectionChanged);
		}
		int lockCollectionCounter = 0;
		internal void OnLinkItemChanged(BarItemLinkInfo header, BarItem oldItem) {
			if(header.Link == null)
				return;
			IInplaceLinksHolder newInplaceLinksHolder = GetInplaceLinksHolder(header.Link);
			IInplaceLinksHolder oldInplaceLinksHolder = GetInplaceLinksHolder(oldItem);
			if(newInplaceLinksHolder == null && oldInplaceLinksHolder == null)
				return;
			int index = Source.IndexOf(header.Link);
			OnLinkRemoved(index, null, Source[index]);
			OnLinkInserted(index, null, Source[index]);
		}
		protected internal void OnChildLinkCollectionChanged(BarItemLinkInfo header, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					OnLinkInserted(e.NewStartingIndex, header, e.NewItems[0] as BarItemLinkBase);
					break;
				case NotifyCollectionChangedAction.Remove:
					OnLinkRemoved(e.OldStartingIndex, header, e.OldItems[0] as BarItemLinkBase);
					break;
				case NotifyCollectionChangedAction.Replace:
					OnLinkRemoved(e.NewStartingIndex, header, e.OldItems[0] as BarItemLinkBase);
					OnLinkInserted(e.NewStartingIndex, header, e.NewItems[0] as BarItemLinkBase);
					break;
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Reset: {
					Reset(header);
					break;
					}
			}
		}
		private void Reset(BarItemLinkInfo header) {
			BarItemLinkInfoSegmentPosition pos = Segments.GetPositionInfo(header, 0);
			RemoveHeader(pos.LinearIndex, pos.SegmentIndex, true);
			int linearIndex = pos.LinearIndex;
			int segmentIndex = pos.SegmentIndex;
			InsertChildrenLinkInfos(ref linearIndex, ref segmentIndex);
		}
	}
	internal class BarItemLinkInfoSegment {
		public int Level { get; set; }
		public BarItemLinkInfo Header { get; set; }
		public int Count { get; set; }
		public bool IsLevelHeader { get { return Header != null; } }
	}
	internal class BarItemLinkInfoSegmentPosition {
		public int LinearIndex { get; set; }
		public int SegmentIndex { get; set; }
		public int SegmentOffset { get; set; }
		public int SegmentLevel { get; set; }
		public bool NeedSegmentCreation { get; set; }
		public bool IsLevelHeader { get; set; }
		public int HeaderSegmentIndex { get; set; }
	}
	internal class BarItemLinkInfoSegmentCollection : List<BarItemLinkInfoSegment> {
		public int GetSegmentIndexFromHeader(BarItemLinkInfo header, out int startIndex) {
			startIndex = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].Header == header)
					return i;
				startIndex += this[i].Count;
			}
			return -1;
		}
		public void SplitSegment(int targetSegmentIndex, int splitIndex, BarItemLinkInfo header) {
			BarItemLinkInfoSegment targetSegment = this[targetSegmentIndex];
			if(splitIndex > targetSegment.Count)
				throw new IndexOutOfRangeException("SplitIndex is invalid.");
			BarItemLinkInfoSegment newSegment = new BarItemLinkInfoSegment() { Level = targetSegment.Level + 1, Count = 0, Header = header };
			this.Insert(targetSegmentIndex + 1, newSegment);
			if(targetSegment.Count == splitIndex) return;
			BarItemLinkInfoSegment targetSegmentRemainder = new BarItemLinkInfoSegment() { Level = targetSegment.Level, Count = targetSegment.Count - splitIndex, Header = null };
			targetSegment.Count = splitIndex;
			this.Insert(targetSegmentIndex + 2, targetSegmentRemainder);
		}
		public void CombineSegments(int removeSegmentIndex) {
			this.RemoveAt(removeSegmentIndex);
			if(removeSegmentIndex > 0 && removeSegmentIndex < this.Count) {
				if(this[removeSegmentIndex - 1].Level == this[removeSegmentIndex].Level && !this[removeSegmentIndex].IsLevelHeader) {
					this[removeSegmentIndex - 1].Count += this[removeSegmentIndex].Count;
					RemoveAt(removeSegmentIndex);
				}
			}
		}
		public BarItemLinkInfoSegmentPosition GetPositionInfo(BarItemLinkInfo levelHeader, int childLinkIndex) {
			BarItemLinkInfoSegmentPosition pos = new BarItemLinkInfoSegmentPosition();
			pos.LinearIndex = 0;
			pos.SegmentIndex = 0;
			pos.NeedSegmentCreation = false;
			pos.SegmentOffset = 0;
			pos.IsLevelHeader = false;
			pos.HeaderSegmentIndex = 0;
			if(levelHeader != null) {
				int startIndex;
				pos.SegmentIndex = GetSegmentIndexFromHeader(levelHeader, out startIndex);
				pos.LinearIndex = startIndex;				
			}
			pos.SegmentLevel = this[pos.SegmentIndex].Level;
			if(childLinkIndex < this[pos.SegmentIndex].Count) {
				pos.LinearIndex += childLinkIndex;
				pos.SegmentOffset = childLinkIndex;
				return pos;
			}
			int currentLevelIndex = 0;
			pos.LinearIndex += this[pos.SegmentIndex].Count;
			currentLevelIndex += this[pos.SegmentIndex].Count;
			pos.SegmentIndex++;
			for(; pos.SegmentIndex < this.Count; pos.SegmentIndex++) {
				if(this[pos.SegmentIndex].Level > pos.SegmentLevel) {
					if(this[pos.SegmentIndex].Level - pos.SegmentLevel == 1 && this[pos.SegmentIndex].IsLevelHeader) {						
						if(currentLevelIndex == childLinkIndex) {
							pos.IsLevelHeader = true;
							pos.HeaderSegmentIndex = pos.SegmentIndex;
							if(this[pos.SegmentIndex - 1].Level == pos.SegmentLevel) {
								pos.SegmentIndex--;
								pos.SegmentOffset = this[pos.SegmentIndex].Count;
								return pos;
							} else {
								pos.NeedSegmentCreation = true;
								return pos;
							}
						}
						currentLevelIndex++;
					}
					pos.LinearIndex += this[pos.SegmentIndex].Count;
					continue;
				}
				if(this[pos.SegmentIndex].Level < pos.SegmentLevel || this[pos.SegmentIndex].IsLevelHeader) {
					if(currentLevelIndex == childLinkIndex) {
						if(this[pos.SegmentIndex - 1].Level == pos.SegmentLevel) {
							pos.SegmentIndex--;
							pos.SegmentOffset = this[pos.SegmentIndex].Count;
							return pos;
						} else {
							pos.NeedSegmentCreation = true;
							return pos;
						}
					}
					throw new IndexOutOfRangeException("LevelIndex is invalid.");
				}
				if(childLinkIndex - currentLevelIndex < this[pos.SegmentIndex].Count) {
					pos.LinearIndex += childLinkIndex - currentLevelIndex;
					pos.SegmentOffset = childLinkIndex - currentLevelIndex;
					return pos;
				}
				currentLevelIndex += this[pos.SegmentIndex].Count;
				pos.LinearIndex += this[pos.SegmentIndex].Count;
			}
			if(currentLevelIndex == childLinkIndex) {
				if(this[this.Count - 1].Level == pos.SegmentLevel) {
					pos.SegmentIndex = this.Count - 1;
					pos.SegmentOffset = this[this.Count - 1].Count;
					return pos;
				};
				pos.NeedSegmentCreation = true;
				return pos;
			}
			throw new IndexOutOfRangeException("LevelIndex is invalid.");
		}
	}
}
