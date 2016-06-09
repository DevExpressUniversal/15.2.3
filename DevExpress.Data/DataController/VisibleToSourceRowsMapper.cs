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
using System.Text;
using System.Diagnostics;
namespace DevExpress.Data.Helpers {
	public abstract class VisibleToSourceRowsMapper {
		public abstract bool IsReadOnly { get; }
		public abstract int GetListSourceIndex(int visibleIndex);
		public abstract int? GetVisibleIndex(int listSourceIndex);
		public abstract int VisibleRowCount { get; }
		public abstract int? HideRow(int sourceIndex);
		public abstract void ShowRow(int sourceIndex, int visibleIndex);
		public abstract void InsertRow(int sourceIndex, int? visibleIndex = null);
		public abstract int? RemoveRow(int sourceIndex);
		public abstract void MoveVisiblePosition(int oldVisibleIndex, int newVisibleIndex);
		public abstract void MoveSourcePosition(int oldSourcePosition, int newSourcePosition);
		public virtual bool IsSetRangeAble { get { return false; } }
		public virtual void SetRange(int startPos, int[] newValues) {
			throw new NotSupportedException();
		}
		public virtual void SetValue(int visibleIndex, int sourceIndex) {
			throw new NotSupportedException();
		}
		public abstract int[] ToArray();
		public abstract IEnumerable<int> ToEnumerable();
		public virtual bool Contains(int listSourceIndex) {
			return GetVisibleIndex(listSourceIndex).HasValue;
		}
	}
	public class VisibleToSourceRowsListMapper: VisibleToSourceRowsMapper {
		readonly List<int> Map;
		public VisibleToSourceRowsListMapper(IEnumerable<int> initialVisibleState, int hintCount) {
			this.Map = new List<int>(hintCount + hintCount / 4 + 256);
			this.Map.AddRange(initialVisibleState);
		}
		public VisibleToSourceRowsListMapper(VisibleToSourceRowsMapper mapper)
			: this(mapper.ToEnumerable(), mapper.VisibleRowCount) {
		}
		public override int GetListSourceIndex(int visibleIndex) {
			return Map[visibleIndex];
		}
		public override int VisibleRowCount {
			get { return Map.Count; }
		}
		public override int? HideRow(int sourceIndex) {
			var visibleIndex = GetVisibleIndex(sourceIndex);
			if(visibleIndex.HasValue) {
				int visibleIndexValue = visibleIndex.Value;
				Map.RemoveAt(visibleIndexValue);
				if(visibleIndexValue == Map.Count) {
					PatchCacheWithJustLastVisibleItemRemoved(sourceIndex, visibleIndexValue);
				} else {
					ResetCache();
				}
			}
			return visibleIndex;
		}
		public override void ShowRow(int sourceIndex, int visibleIndex) {
			if(Contains(sourceIndex))
				throw new InvalidOperationException("Already visible");
			int countBefore = Map.Count;
			Map.Insert(visibleIndex, sourceIndex);
			if(visibleIndex == countBefore) {
				PatchCacheWithJustLastVisibleItemShown(sourceIndex, visibleIndex);
			} else {
				ResetCache();
			}
		}
		public override void InsertRow(int sourceIndex, int? visibleIndex = null) {
			bool mapChanged = false;
			for(int i = 0; i < Map.Count; ++i) {
				int pos = Map[i];
				if(pos >= sourceIndex) {
					Map[i] = pos + 1;
					mapChanged = true;
				}
			}
			int countBefore = Map.Count;
			if(visibleIndex.HasValue)
				Map.Insert(visibleIndex.Value, sourceIndex);
			if(mapChanged) {
				ResetCache();
			} else if(!visibleIndex.HasValue) {
			} else if(visibleIndex.Value == countBefore) {
				PatchCacheWithJustLastVisibleItemShown(sourceIndex, visibleIndex.Value);
			} else {
				ResetCache();
			}
		}
		public override int? RemoveRow(int sourceIndex) {
			var oldVisibleIndex = HideRow(sourceIndex);
			bool mapChanged = false;
			for(int i = 0; i < Map.Count; ++i) {
				var pos = Map[i];
				if(pos >= sourceIndex) {
					Map[i] = pos - 1;
					mapChanged = true;
				}
			}
			if(mapChanged)
				ResetCache();
			return oldVisibleIndex;
		}
		public override void MoveVisiblePosition(int oldVisibleIndex, int newVisibleIndex) {
			if(oldVisibleIndex == newVisibleIndex)
				return;
			int sourceIndex = Map[oldVisibleIndex];
			Map.RemoveAt(oldVisibleIndex);
			Map.Insert(newVisibleIndex, sourceIndex);
			ResetCache();
		}
		public override void MoveSourcePosition(int oldSourcePosition, int newSourcePosition) {
			if(oldSourcePosition == newSourcePosition)
				return;
			int? visiblePosition = GetVisibleIndex(oldSourcePosition);
			bool mapChanged = false;
			if(oldSourcePosition < newSourcePosition) {
				for(int i = 0; i < Map.Count; ++i) {
					int pos = Map[i];
					if(pos > oldSourcePosition && pos <= newSourcePosition) {
						Map[i] = pos - 1;
						mapChanged = true;
					}
				}
			} else {
				for(int i = 0; i < Map.Count; ++i) {
					int pos = Map[i];
					if(pos < oldSourcePosition && pos >= newSourcePosition) {
						Map[i] = pos + 1;
						mapChanged = true;
					}
				}
			}
			if(visiblePosition.HasValue) {
				Map[visiblePosition.Value] = newSourcePosition;
				mapChanged = true;
			}
			if(mapChanged)
				ResetCache();
		}
		Dictionary<int, int> SourceRowsToVisibleIndexesCache, _CacheParking;
		int _ParkedCacheUseCount;
		int? hotSourceIndex, hotVisibleIndex;
		void ResetCache() {
			hotSourceIndex = null;
			if(SourceRowsToVisibleIndexesCache != null) {
				_CacheParking = SourceRowsToVisibleIndexesCache;
				SourceRowsToVisibleIndexesCache = null;
			}
		}
		void PatchCacheWithJustLastVisibleItemShown(int sourceIndex, int visibleIndex) {
			hotSourceIndex = null;
			if(SourceRowsToVisibleIndexesCache != null) {
				SourceRowsToVisibleIndexesCache.Add(sourceIndex, visibleIndex);
			}
		}
		void PatchCacheWithJustLastVisibleItemRemoved(int sourceIndex, int visibleIndex) {
			hotSourceIndex = null;
			if(SourceRowsToVisibleIndexesCache != null) {
				SourceRowsToVisibleIndexesCache.Remove(sourceIndex);
			}
		}
		int? GetVisibleIndexCore(int listSourceIndex) {
			if(SourceRowsToVisibleIndexesCache != null) {
				int visibleIndex;
				if(SourceRowsToVisibleIndexesCache.TryGetValue(listSourceIndex, out visibleIndex))
					return visibleIndex;
				else
					return null;
			}
			if(Map.Count >= 1 && Map[0] == listSourceIndex)
				return 0;
			if(Map.Count >= 2 && Map[Map.Count - 1] == listSourceIndex)
				return Map.Count - 1;
			Dictionary<int, int> newCache;
			if(_CacheParking != null) {
				if(!IsChangedEnoughForSmart)
					++_ParkedCacheUseCount;
				newCache = _CacheParking;
				_CacheParking = null;
				newCache.Clear();
			} else {
				newCache = new Dictionary<int, int>(Map.Count + Map.Count / 5 + 128);
			}
			int? rv = null;
			for(int i = 0; i < Map.Count; ++i) {
				int srcRow = Map[i];
				if(srcRow == listSourceIndex)
					rv = i;
				newCache.Add(srcRow, i);
			}
			SourceRowsToVisibleIndexesCache = newCache;
			return rv;
		}
		public override int? GetVisibleIndex(int listSourceIndex) {
			if(listSourceIndex != hotSourceIndex) {
				hotVisibleIndex = GetVisibleIndexCore(listSourceIndex);
				hotSourceIndex = listSourceIndex;
			}
			return hotVisibleIndex;
		}
		public override void SetRange(int startPos, int[] newValues) {
			for(int i = 0; i < newValues.Length; ++i)
				Map[i + startPos] = newValues[i];
			ResetCache();
		}
		public override void SetValue(int visibleIndex, int sourceIndex) {
			Map[visibleIndex] = sourceIndex;
			ResetCache();
		}
		public override bool IsSetRangeAble {
			get { return true; }
		}
		public override int[] ToArray() {
			return Map.ToArray();
		}
		public override IEnumerable<int> ToEnumerable() {
			return Map;
		}
		public override bool IsReadOnly { get { return false; } }
		public bool IsChangedEnoughForSmart { get { return _ParkedCacheUseCount > 2; } }
	}
	public class VisibleToSourceRowsSmartMapper: VisibleToSourceRowsMapper {
		class Page {
			public readonly Catalog ThisCatalog, OtherCatalog;
			public readonly int PageHandle; 
			public Page NextPage, PrevPage;
			public int PageStart;
			public int Count;
			public int NonEmptyCount;
			public Record[] Data;
			public Page(Catalog thisCatalog, int pageHandle) {
				ThisCatalog = thisCatalog;
				OtherCatalog = thisCatalog.OtherCatalog;
				PageHandle = pageHandle;
			}
			public int? GetOtherIndex(int offsetFromPageStart) {
				if(Data != null) {
					var record = Data[offsetFromPageStart];
					if(record.IsEmpty)
						return null;
					return OtherCatalog.GetPageByHandle(record.PageHandle).PageStart + record.OffsetWithinPage;
				} else {
					return null;
				}
			}
			public void EnforceNonEmptyData() {
				if(Data != null)
					return;
				if(NonEmptyCount != 0)
					throw new InvalidOperationException("NonEmptyCount != 0");
				Data = new Record[Catalog.PageCapacity];
				for(int i = 0; i < Count; ++i) {
					Data[i] = Record.Empty;
				}
			}
			public bool Contains(int globalIndex) {
				int offset = globalIndex - PageStart;
				return offset >= 0 && offset < Count;
			}
			public IEnumerable<int?> GetEnumerable() {
				if(Data != null) {
					for(int i = 0; i < Count; ++i) {
						yield return GetOtherIndex(i);
					}
				} else {
					for(int i = 0; i < Count; ++i) {
						yield return null;
					}
				}
			}
			public void ForAllPagesAfterDo(Action<Page> action) {
				for(var p = NextPage; p != null; p = p.NextPage)
					action(p);
			}
			public int? HideThereAndRemoveFromOther(int sourceOffset) {
				if(Data == null) {
					return null;
				}
				var record = Data[sourceOffset];
				Data[sourceOffset] = Record.Empty;
				--NonEmptyCount;
				EmptyIfNeeded();
				var otherPage = OtherCatalog.GetPageByHandle(record.PageHandle);
				var rv = otherPage.PageStart + record.OffsetWithinPage;
				otherPage.RemoveRecord(record.OffsetWithinPage);
				return rv;
			}
			public int? RemoveThereAndRemoveFromOther(int offset) {
				EnforceNonEmptyData();
				var record = Data[offset];
				RemoveRecord(offset);
				if(record.IsEmpty) {
					return null;
				} else {
					var otherPage = OtherCatalog.GetPageByHandle(record.PageHandle);
					var rv = otherPage.PageStart + record.OffsetWithinPage;
					otherPage.RemoveRecord(record.OffsetWithinPage);
					return rv;
				}
			}
			public void EmptyIfNeeded() {
				if(Data != null && NonEmptyCount == 0)
					Data = null;
			}
			public void RemoveRecord(int offset) {
				int oldCount = Count;
				--Count;
				if(Data != null) {
					if(!Data[offset].IsEmpty) {
						--NonEmptyCount;
						EmptyIfNeeded();
					}
				}
				ForAllPagesAfterDo(p => --p.PageStart);
				if(Count >= Catalog.MinRecordsPerPageExceptFirstAndLast || this.PrevPage == null || this.NextPage == null) {
					if(Count == 0) {
						var fixPage = PrevPage ?? NextPage;
						ThisCatalog.RemovePage(this);
						if(fixPage != null) {
							fixPage.FixMileStones();
						} else {
							ThisCatalog.FixMileStones(null);
						}
					} else {
						if(Data != null) {
							Array.Copy(Data, offset + 1, Data, offset, Count - offset);
						}
						this.FixOtherSideRecords(offset, Count);
						this.FixMileStones();
					}
				} else if(PrevPage.Count + this.Count <= Catalog.PageCapacity) {
					var prevPage = PrevPage;
					if(this.Data != null)
						prevPage.EnforceNonEmptyData();
					int oldPrevCount = prevPage.Count;
					prevPage.Count += Count;
					prevPage.NonEmptyCount += NonEmptyCount;
					if(Data != null) {
						Array.Copy(Data, 0, prevPage.Data, oldPrevCount, offset);
						Array.Copy(Data, offset + 1, prevPage.Data, oldPrevCount + offset, Count - offset);
					} else if(prevPage.Data != null) {
						for(int i = oldPrevCount; i < prevPage.Count; ++i)
							prevPage.Data[i] = Record.Empty;
					}
					Count = NonEmptyCount = 0;
					ThisCatalog.RemovePage(this);
					prevPage.FixOtherSideRecords(oldPrevCount, prevPage.Count);
					prevPage.FixMileStones();
				} else if(NextPage.Count + this.Count <= Catalog.PageCapacity) {
					var nextPage = NextPage;
					if(nextPage.Data != null)
						this.EnforceNonEmptyData();
					if(nextPage.Data != null) {
						Array.Copy(Data, offset + 1, Data, offset, Count - offset);
						Array.Copy(nextPage.Data, 0, Data, Count, nextPage.Count);
					} else if(this.Data != null) {
						for(int i = 0; i < nextPage.Count; ++i) {
							this.Data[i + Count] = Record.Empty;
						}
					}
					Count += nextPage.Count;
					NonEmptyCount += nextPage.NonEmptyCount;
					nextPage.Count = nextPage.NonEmptyCount = 0;
					ThisCatalog.RemovePage(nextPage);
					this.FixOtherSideRecords(offset, Count);
					this.FixMileStones();
				} else {
					Record recFromPrevPage;
					if(PrevPage.Data != null)
						recFromPrevPage = PrevPage.Data[PrevPage.Count - 1];
					else
						recFromPrevPage = Record.Empty;
					--PrevPage.Count;
					++this.Count;
					--this.PageStart;
					if(!recFromPrevPage.IsEmpty) {
						this.EnforceNonEmptyData();
						++NonEmptyCount;
						--PrevPage.NonEmptyCount;
						PrevPage.EmptyIfNeeded();
					}
					if(this.Data != null) {
						Array.Copy(Data, 0, Data, 1, offset);
						Data[0] = recFromPrevPage;
					}
					this.FixOtherSideRecords(0, offset + 1);
					this.FixMileStones();
				}
			}
			public void FixOtherSideRecords(int offsetFirstToFix, int offsetBeyondLastToFix) {
				if(this.Data == null)
					return;
				for(int off = offsetFirstToFix; off < offsetBeyondLastToFix; ++off) {
					var rec = this.Data[off];
					if(rec.IsEmpty)
						continue;
					var otherPage = OtherCatalog.GetPageByHandle(rec.PageHandle);
					otherPage.EnforceNonEmptyData();
					if(otherPage.Data[rec.OffsetWithinPage].IsEmpty)
						++otherPage.NonEmptyCount;
					otherPage.Data[rec.OffsetWithinPage] = new Record(this.PageHandle, off);
				}
			}
			void FixMileStones() {
				ThisCatalog.FixMileStones(this);
			}
		}
		struct Record {
			public int PageHandle;  
			public int OffsetWithinPage;
			const int EmptyPageHandle = -1;
			public bool IsEmpty { get { return PageHandle == EmptyPageHandle; } }
			public Record(int pageHandle, int offset) {
				PageHandle = pageHandle;
				OffsetWithinPage = offset;
			}
			public static readonly Record Empty = new Record(EmptyPageHandle, -1);
			public override string ToString() {
				if(IsEmpty)
					return "Empty";
				else
					return "(" + PageHandle + ", " + OffsetWithinPage + ")";
			}
		}
		struct PageAndOffset {
			public readonly Page Page;
			public readonly int Offset;
			public PageAndOffset(Page _Page, int _Offset) {
				Page = _Page;
				Offset = _Offset;
			}
			public bool IsEmpty { get { return Page == null; } }
			public static readonly PageAndOffset Empty = new PageAndOffset(null, -1);
			public static implicit operator Record(PageAndOffset pof) {
				return pof.Page == null ? Record.Empty : new Record(pof.Page.PageHandle, pof.Offset);
			}
			public override string ToString() {
				return ((Record)this).ToString();
			}
		}
		class Catalog {
			readonly List<Page> HandlesToPages = new List<Page>();
			readonly Queue<int> FreeHandles = new Queue<int>();
			Page FirstPage, LastPage;
			readonly List<Page> Milestones = new List<Page>();
			bool MileStonesComplete;
			public Catalog OtherCatalog;
			public const int MinRecordsPerPageExceptFirstAndLast = 500;
			const int MileStoneRange = MinRecordsPerPageExceptFirstAndLast; 
			public const int PageCapacity = 1024;	
			const int PageFill = 1000;
			Page CreatePageWithHandle() {
				Page rv;
				if(FreeHandles.Count > 0) {
					rv = new Page(this, FreeHandles.Dequeue());
					HandlesToPages[rv.PageHandle] = rv;
				} else {
					rv = new Page(this, HandlesToPages.Count);
					HandlesToPages.Add(rv);
				}
				return rv;
			}
			public Page CreateNewPageAfter(Page pageToInsertAfter) {
				var nextPage = pageToInsertAfter.NextPage;
				var newPage = CreatePageWithHandle();
				newPage.PrevPage = pageToInsertAfter;
				newPage.NextPage = nextPage;
				pageToInsertAfter.NextPage = newPage;
				if(nextPage == null)
					LastPage = newPage;
				else
					nextPage.PrevPage = newPage;
				return newPage;
			}
			public Page CreateNewPageBefore(Page pageToInsertBefore) {
				var prevPage = pageToInsertBefore.PrevPage;
				var newPage = CreatePageWithHandle();
				newPage.PrevPage = prevPage;
				newPage.NextPage = pageToInsertBefore;
				if(prevPage == null)
					FirstPage = newPage;
				else
					prevPage.NextPage = newPage;
				pageToInsertBefore.PrevPage = newPage;
				return newPage;
			}
			public Page CreateFirstPage() {
				if(FirstPage != null || LastPage != null)
					throw new InvalidOperationException("FirstPage != null || LastPage != null");
				var newPage = CreatePageWithHandle();
				newPage.PrevPage = newPage.NextPage = null;
				FirstPage = LastPage = newPage;
				return newPage;
			}
			public Page GetPageByHandle(int handle) {
				return HandlesToPages[handle];
			}
			Page GetMileStone(int targetMileStone) {
				if(!MileStonesComplete && Milestones.Count <= targetMileStone) {
					if(Milestones.Count == 0) {
						if(FirstPage == null)
							return null;
						Milestones.Add(FirstPage);
					}
					for(Page runningPage = Milestones[Milestones.Count - 1]; ; ) {
						var nextIndex = Milestones.Count * Catalog.MileStoneRange;
						if(!runningPage.Contains(nextIndex)) {
							runningPage = runningPage.NextPage;
							if(runningPage == null) {
								MileStonesComplete = true;
								break;
							} else if(Milestones.Count > targetMileStone) {
								break;
							}
							if(!runningPage.Contains(nextIndex) && runningPage.NextPage != null)
								throw new InvalidOperationException("!runningPage.Contains(nextIndex) && runningPage.NextPage != null");
						}
						Milestones.Add(runningPage);
					}
				}
				if(Milestones.Count <= targetMileStone)
					return null;
				else
					return Milestones[targetMileStone];
			}
			public PageAndOffset GetPageAndOffsetForIndex(int index) {
				if(index < 0)
					throw new IndexOutOfRangeException("index(" + index + ") <0");
				int mileStone = index / MileStoneRange;
				var page = GetMileStone(mileStone);
				if(page == null)
					return PageAndOffset.Empty;
				if(!page.Contains(index)) {
					page = page.NextPage;
					if(page == null)
						return PageAndOffset.Empty;
					if(!page.Contains(index)) {
						if(page.NextPage == null)
							return PageAndOffset.Empty;
						else
							throw new InvalidOperationException("!page.Contains(index) && !page.NextPage.Contains(index) && page.NextPage != null");
					}
				}
				return new PageAndOffset(page, index - page.PageStart);
			}
			public PageAndOffset GetPageAndOffsetForIndex(int index, bool padEmptyIfNeeded) {
				var rv = GetPageAndOffsetForIndex(index);
				if(!padEmptyIfNeeded || !rv.IsEmpty)
					return rv;
				if(FirstPage == null) {
					CreateFirstPage();
				}
				Page fixPage = LastPage;
				for(var page = fixPage; ; ) {
					int countNeeded = 1 + index - page.PageStart;
					if(page.Count >= countNeeded)
						throw new InvalidOperationException("page.Count >= countNeeded");
					if(countNeeded <= PageFill) {
						if(page.Data != null)
							for(int i = page.Count; i < countNeeded; ++i)
								page.Data[i] = Record.Empty;
						page.Count = countNeeded;
						FixMileStones(fixPage);
						return new PageAndOffset(page, countNeeded - 1);
					} else {
						if(page.Count < PageFill) {
							if(page.Data != null)
								for(int i = page.Count; i < PageFill; ++i)
									page.Data[i] = Record.Empty;
							page.Count = PageFill;
						}
						int nextPageStart = page.PageStart + page.Count;
						page = CreateNewPageAfter(page);
						page.PageStart = nextPageStart;
						page.Count = page.NonEmptyCount = 0;
					}
				}
			}
			public int? GetOtherIndex(int thisIndex) {
				var thisPageAndOffset = GetPageAndOffsetForIndex(thisIndex);
				if(thisPageAndOffset.IsEmpty)
					return null;
				else
					return thisPageAndOffset.Page.GetOtherIndex(thisPageAndOffset.Offset);
			}
			public IEnumerable<int?> GetEnumerable() {
				Page page = GetPageAndOffsetForIndex(0).Page;
				for(; page != null; page = page.NextPage)
					foreach(var ind in page.GetEnumerable())
						yield return ind;
			}
			public void FixMileStones(Page page) {
				if(page == null) {
					if(FirstPage != null)
						throw new InvalidOperationException("page == null && FirstPage != null");
					Milestones.Clear();
					return;
				}
				int firstMileStone = (page.PageStart + MileStoneRange - 1) / MileStoneRange;
				if(!page.Contains(firstMileStone * MileStoneRange) && page.NextPage != null)
					throw new InvalidOperationException("!page.Contains(firstMileStone * MileStoneRange) && page.NextPage != null");
				if(Milestones.Count < firstMileStone)
					return;
				Milestones.RemoveRange(firstMileStone, Milestones.Count - firstMileStone);
				MileStonesComplete = false;
			}
			public void RemovePage(Page page) {
				if(page.Count != 0 || page.NonEmptyCount != 0)
					throw new InvalidOperationException("page.Count != 0 || page.NonEmptyCount != 0");
				if(page.PrevPage == null)
					FirstPage = page.NextPage;
				else
					page.PrevPage.NextPage = page.NextPage;
				if(page.NextPage == null)
					LastPage = page.PrevPage;
				else
					page.NextPage.PrevPage = page.PrevPage;
				HandlesToPages[page.PageHandle] = null;
				FreeHandles.Enqueue(page.PageHandle);
			}
			public int GetContainedItemsCount() {
				if(LastPage == null)
					return 0;
				else
					return LastPage.PageStart + LastPage.Count;
			}
#if DEBUGTEST
			public void Validate() {
				try {
					Dictionary<int, Page> traversedPages = new Dictionary<int, Page>();
					var firstPage = FirstPage;
					if(firstPage != null) {
						if(firstPage.PrevPage != null)
							throw new InvalidOperationException("firstPage != null && firstPage.PrevPage != null");
						if(firstPage.PageStart != 0)
							throw new InvalidOperationException("firstPage.PageStart != 0");
					}
					for(var page = firstPage; page != null; page = page.NextPage) {
						traversedPages.Add(page.PageHandle, page);
						if(page.Count < Catalog.MinRecordsPerPageExceptFirstAndLast && page.PrevPage != null && page.NextPage != null)
							throw new InvalidOperationException("page.Count < Catalog.MinRecordsPerPageExceptFirstAndLast && page.PrevPage != null && page.NextPage != null");
						if(page.Data != null) {
							var actualNonEmptyCount = page.Data.Take(page.Count).Count(p => !p.IsEmpty);
							if(actualNonEmptyCount != page.NonEmptyCount)
								throw new InvalidOperationException("actualNonEmptyCount != page.NonEmptyCount");
							if(page.NonEmptyCount == 0)
								throw new InvalidOperationException("page.Data != null && page.NonEmptyCount == 0");
						}
						if(page.Data == null && page.NonEmptyCount != 0)
							throw new InvalidOperationException("page.Data == null && page.NonEmptyCount != 0");
						var nextPage = page.NextPage;
						if(nextPage != null) {
							if(nextPage.PrevPage != page)
								throw new InvalidOperationException("nextPage.PrevPage != page");
							if(nextPage.PageStart != page.PageStart + page.Count)
								throw new InvalidOperationException("nextPage.PageStart ! =page.PageStart + page.Count");
						} else {
							if(page != LastPage)
								throw new InvalidOperationException("page.NextPage == null && page != LastPage");
						}
					}
					for(int i = 0; i < HandlesToPages.Count; ++i) {
						var pageAtI = GetPageByHandle(i);
						if(pageAtI == null && traversedPages.ContainsKey(i))
							throw new InvalidOperationException("pageAtI == null && traversedPages.ContainsKey(i)");
						if(pageAtI != null && traversedPages[i] != pageAtI)
							throw new InvalidOperationException("pageAtI != null && traversedPages[i] != pageAtI");
						if(pageAtI != null && pageAtI.PageHandle != i)
							throw new InvalidOperationException("pageAtI.PageHandle != i");
					}
					for(int i = 0; i < Milestones.Count; ++i) {
						var mileStonePageAtI = Milestones[i];
						if(!mileStonePageAtI.Contains(i * MileStoneRange))
							throw new InvalidOperationException("!mileStonePageAtI.Contains(i * MileStoneRange)");
					}
					foreach(var i in FreeHandles)
						if(HandlesToPages[i] != null)
							throw new InvalidOperationException("HandlesToPages[i] != null");
					if(FreeHandles.Count != HandlesToPages.Count(p => p == null))
						throw new InvalidOperationException("FreeHandles.Count != HandlesToPages.Count(p => p == null)");
				} catch(Exception e) {
					e.ToString();
					throw;
				}
			}
#endif
			PageAndOffset GetInsertable(int indexToInsert) {
				var poff = GetPageAndOffsetForIndex(indexToInsert);
				if(poff.IsEmpty)
					throw new InvalidOperationException("poff.IsEmpty");
				if(poff.Offset == 0 && poff.Page.PrevPage != null && poff.Page.PrevPage.Count < PageCapacity) {
					return new PageAndOffset(poff.Page.PrevPage, poff.Page.PrevPage.Count);
				}
				if(poff.Page.Count < PageCapacity)
					return poff;
				var thisPage = poff.Page;
				int count1 = thisPage.Count / 2;
				int count2 = thisPage.Count - count1;
				if(count1 < MinRecordsPerPageExceptFirstAndLast || count2 < MinRecordsPerPageExceptFirstAndLast)
					throw new InvalidOperationException("count1 < MinRecordsPerPageExceptFirstAndLast || count2 < MinRecordsPerPageExceptFirstAndLast");
				var newPage = CreateNewPageAfter(thisPage);
				newPage.PageStart = thisPage.PageStart + count1;
				newPage.Count = count2;
				thisPage.Count = count1;
				if(thisPage.Data != null) {
					newPage.Data = new Record[PageCapacity];
					Array.Copy(thisPage.Data, count1, newPage.Data, 0, count2);
					newPage.FixOtherSideRecords(0, count2);
					int newPageNonEmpty = 0;
					for(int i = 0; i < count2; ++i)
						if(!newPage.Data[i].IsEmpty)
							++newPageNonEmpty;
					newPage.NonEmptyCount = newPageNonEmpty;
					thisPage.NonEmptyCount -= newPageNonEmpty;
					thisPage.EmptyIfNeeded();
					newPage.EmptyIfNeeded();
				}
				FixMileStones(thisPage);
				var newPoff = GetPageAndOffsetForIndex(indexToInsert);
				if(newPoff.IsEmpty || !(newPoff.Page == thisPage || newPoff.Page == newPage))
					throw new InvalidOperationException("newPoff.IsEmpty || !(newPoff.Page == thisPage || newPoff.Page == newPage)");
				return newPoff;
			}
			PageAndOffset InsertEmpty(int indexToInsert) {
				if(indexToInsert >= GetContainedItemsCount())
					return GetPageAndOffsetForIndex(indexToInsert, true);
				if(indexToInsert == 0) {
					var firstPage = FirstPage;
					if(firstPage.Count >= PageFill) {
						var newFirstPage = CreateNewPageBefore(firstPage);
						newFirstPage.PageStart = 0;
						newFirstPage.Count = 1;
						newFirstPage.NonEmptyCount = 0;
						newFirstPage.ForAllPagesAfterDo(p => ++p.PageStart);
						FixMileStones(newFirstPage);
						return new PageAndOffset(newFirstPage, 0);
					}
				}
				var insertablePoff = GetInsertable(indexToInsert);
				var page = insertablePoff.Page;
				var offset = insertablePoff.Offset;
				if(page.Data != null) {
					Array.Copy(page.Data, offset, page.Data, offset + 1, page.Count - offset);
					page.Data[offset] = Record.Empty;
				}
				++page.Count;
				page.ForAllPagesAfterDo(p => ++p.PageStart);
				page.FixOtherSideRecords(offset + 1, page.Count);
				FixMileStones(page);
				return insertablePoff;
			}
			public PageAndOffset Insert(int indexToInsert, Record valueToInsert) {
				var poff = InsertEmpty(indexToInsert);
				if(!valueToInsert.IsEmpty) {
					poff.Page.EnforceNonEmptyData();
					poff.Page.Data[poff.Offset] = valueToInsert;
					++poff.Page.NonEmptyCount;
				}
				return poff;
			}
			public void TruncateExcessiveEmpty(int countExisistingForSure) {
				bool somethingRemoved = false;
				while(LastPage != null && LastPage.Data == null && LastPage.PageStart > countExisistingForSure) {
					RemovePage(LastPage);
					somethingRemoved = true;
				}
				if(somethingRemoved)
					FixMileStones(LastPage);
			}
		}
		VisibleToSourceRowsSmartMapper(IEnumerable<int> visibleSourceIndices, int count) {
			this.VisibleCatalog = new Catalog();
			this.SourceCatalog = new Catalog();
			this.VisibleCatalog.OtherCatalog = SourceCatalog;
			this.SourceCatalog.OtherCatalog = VisibleCatalog;
			int visibleIndex = 0;
			foreach(var visibleSourceIndex in visibleSourceIndices) {
				this.ShowRow(visibleSourceIndex, visibleIndex);
				++visibleIndex;
			}
			ValidateCatalogs();
		}
		public VisibleToSourceRowsSmartMapper(VisibleToSourceRowsMapper parentMapper)
			: this(parentMapper.ToEnumerable(), parentMapper.VisibleRowCount) {
		}
		readonly Catalog VisibleCatalog;
		readonly Catalog SourceCatalog;
		public override int GetListSourceIndex(int visibleIndex) {
			var rv = VisibleCatalog.GetOtherIndex(visibleIndex);
			if(rv.HasValue)
				return rv.Value;
			else
				throw new ArgumentException("No visible row at index " + visibleIndex);
		}
		public override int? GetVisibleIndex(int listSourceIndex) {
			return SourceCatalog.GetOtherIndex(listSourceIndex);
		}
		public override int VisibleRowCount {
			get { return VisibleCatalog.GetContainedItemsCount(); }
		}
		[Conditional("DEBUGTEST")]
		void ValidateCatalogs() {
			if(!VisibleToSourceRowsDoublecheckMapper.Use)
				return;
#if DEBUGTEST
			VisibleCatalog.Validate();
			SourceCatalog.Validate();
			var visibles = VisibleCatalog.GetEnumerable().ToArray();
			var sources = SourceCatalog.GetEnumerable().ToArray();
			if(visibles.Length != VisibleCatalog.GetContainedItemsCount())
				throw new InvalidOperationException("visibles.Length != VisibleCatalog.GetContainedItemsCount()");
			for(int i = 0; i < visibles.Length; ++i) {
				var s = visibles[i];
				if(!s.HasValue)
					throw new InvalidOperationException("Empty visible " + i);
				if(sources[s.Value] != i)
					throw new InvalidOperationException("sources[s.Value] != i visible(" + i + ") source(" + sources[s.Value]+")");
			}
			for(int i = 0; i < sources.Length; ++i) {
				var v = sources[i];
				if(!v.HasValue)
					continue;
				if(visibles[v.Value] != i)
					throw new InvalidOperationException("visibles[v.Value] != i");
			}
#endif
		}
		public override int? HideRow(int sourceIndex) {
			var sourcePageAndOffset = SourceCatalog.GetPageAndOffsetForIndex(sourceIndex);
			if(sourcePageAndOffset.IsEmpty)
				return null;
			var rv = sourcePageAndOffset.Page.HideThereAndRemoveFromOther(sourcePageAndOffset.Offset);
			ValidateCatalogs();
			return rv;
		}
		public override void ShowRow(int sourceIndex, int visibleIndex) {
			PageAndOffset sourcePageAndOffset = SourceCatalog.GetPageAndOffsetForIndex(sourceIndex, true);
			var srcPage = sourcePageAndOffset.Page;
			int srcOffset = sourcePageAndOffset.Offset;
			srcPage.EnforceNonEmptyData();
			if(!srcPage.Data[srcOffset].IsEmpty)
				throw new InvalidOperationException("Row already visible!");
			srcPage.Data[srcOffset] = VisibleCatalog.Insert(visibleIndex, sourcePageAndOffset);
			srcPage.NonEmptyCount++;
			ValidateCatalogs();
		}
		void InsertHiddenRow(int sourceIndex) {
			if(sourceIndex >= SourceCatalog.GetContainedItemsCount())
				return;
			SourceCatalog.Insert(sourceIndex, Record.Empty);
			ValidateCatalogs();
		}
		public override void InsertRow(int sourceIndex, int? visibleIndex = null) {
			InsertHiddenRow(sourceIndex);
			if(visibleIndex.HasValue)
				ShowRow(sourceIndex, visibleIndex.Value);
			ValidateCatalogs();
		}
		public override int? RemoveRow(int sourceIndex) {
			var sourcePageAndOffset = SourceCatalog.GetPageAndOffsetForIndex(sourceIndex);
			if(sourcePageAndOffset.IsEmpty) {
				return null;
			} else {
				var rv = sourcePageAndOffset.Page.RemoveThereAndRemoveFromOther(sourcePageAndOffset.Offset);
				SourceCatalog.TruncateExcessiveEmpty(sourceIndex);
				ValidateCatalogs();
				return rv;
			}
		}
		public override void MoveVisiblePosition(int oldVisibleIndex, int newVisibleIndex) {
			var oldSource = GetListSourceIndex(oldVisibleIndex);
			var testOldPos = RemoveRow(oldSource);
			if(oldVisibleIndex != testOldPos)
				throw new InvalidOperationException("oldVisibleIndex != testOldPos");
			InsertRow(oldSource, newVisibleIndex);
		}
		public override void MoveSourcePosition(int oldSourcePosition, int newSourcePosition) {
			var oldVisible = RemoveRow(oldSourcePosition);
			InsertRow(newSourcePosition, oldVisible);
		}
		public override int[] ToArray() {
			var arr = VisibleCatalog.GetEnumerable().Select(i => i.Value).ToArray();   
			if(arr.Length != this.VisibleRowCount)
				throw new InvalidOperationException();
			return arr;
		}
		public override IEnumerable<int> ToEnumerable() {
			return VisibleCatalog.GetEnumerable().Select(v => v.Value);
		}
		public override bool IsReadOnly { get { return false; } }
	}
	public abstract class VisibleToSourceRowsReadOnlyMapperBase: VisibleToSourceRowsMapper {
		public override int? HideRow(int sourceIndex) {
			throw new NotSupportedException();
		}
		public override void ShowRow(int sourceIndex, int visibleIndex) {
			throw new NotSupportedException();
		}
		public override void InsertRow(int sourceIndex, int? visibleIndex = null) {
			throw new NotSupportedException();
		}
		public override int? RemoveRow(int sourceIndex) {
			throw new NotSupportedException();
		}
		public override void MoveVisiblePosition(int oldVisibleIndex, int newVisibleIndex) {
			throw new NotSupportedException();
		}
		public override void MoveSourcePosition(int oldSourcePosition, int newSourcePosition) {
			throw new NotSupportedException();
		}
		public override bool IsReadOnly { get { return true; } }
	}
	public class VisibleToSourceRowsFixedIdentityMapper: VisibleToSourceRowsReadOnlyMapperBase {
		readonly int _Count;
		public VisibleToSourceRowsFixedIdentityMapper(int count) {
			this._Count = count;
		}
		public override int GetListSourceIndex(int visibleIndex) {
			return visibleIndex;
		}
		public override int? GetVisibleIndex(int listSourceIndex) {
			return listSourceIndex;
		}
		public override int VisibleRowCount {
			get { return this._Count; }
		}
		public override int[] ToArray() {
			var rv = new int[VisibleRowCount];
			for(int i = 0; i < VisibleRowCount; ++i)
				rv[i] = i;
			return rv;
		}
		public override IEnumerable<int> ToEnumerable() {
			return Enumerable.Range(0, VisibleRowCount);
		}
	}
	public class VisibleToSourceRowsReadOnlyProxyMapper: VisibleToSourceRowsReadOnlyMapperBase {
		readonly VisibleToSourceRowsMapper Target;
		public VisibleToSourceRowsReadOnlyProxyMapper(VisibleToSourceRowsMapper _Target) {
			this.Target = _Target;
		}
		public override int GetListSourceIndex(int visibleIndex) {
			return Target.GetListSourceIndex(visibleIndex);
		}
		public override int? GetVisibleIndex(int listSourceIndex) {
			return Target.GetVisibleIndex(listSourceIndex);
		}
		public override int VisibleRowCount {
			get { return Target.VisibleRowCount; }
		}
		public override int[] ToArray() {
			return Target.ToArray();
		}
		public override IEnumerable<int> ToEnumerable() {
			return Target.ToEnumerable();
		}
	}
	public class VisibleToSourceRowsDoublecheckMapper: VisibleToSourceRowsMapper {
		public static bool Use;
		public readonly VisibleToSourceRowsListMapper ListMapper;
		public readonly VisibleToSourceRowsSmartMapper SmartMapper;
		void Do(Action<VisibleToSourceRowsMapper> a) {
#if !DXPORTABLE
			try {
#endif
				a(ListMapper);
				a(SmartMapper);
				if(ListMapper.VisibleRowCount != SmartMapper.VisibleRowCount)
					throw new InvalidOperationException("ListMapper.VisibleRowCount != SmartMapper.VisibleRowCount");
#if !DXPORTABLE
			} catch(Exception e) {
				Console.Error.WriteLine(e.ToString());
				throw;
			}
#endif
		}
		R Do<R>(Func<VisibleToSourceRowsMapper, R> f) {
#if !DXPORTABLE
			try {
#endif
				var rL = f(ListMapper);
				var rS = f(SmartMapper);
				if(!EqualityComparer<R>.Default.Equals(rL, rS)) {
					throw new InvalidOperationException("ListMapper result is '" + rL + "', SmartMapper result is '" + rS + "'");
				}
				if(ListMapper.VisibleRowCount != SmartMapper.VisibleRowCount)
					throw new InvalidOperationException("ListMapper.VisibleRowCount != SmartMapper.VisibleRowCount");
				return rS;
#if !DXPORTABLE
			} catch(Exception e) {
				Console.Error.WriteLine(e.ToString());
				throw;
			}
#endif
		}
		public VisibleToSourceRowsDoublecheckMapper(VisibleToSourceRowsMapper parentMapper) {
			ListMapper = new VisibleToSourceRowsListMapper(parentMapper);
			SmartMapper = new VisibleToSourceRowsSmartMapper(parentMapper);
		}
		public override bool IsReadOnly {
			get { return Do(m => m.IsReadOnly); }
		}
		public override int GetListSourceIndex(int visibleIndex) {
			return Do(m => m.GetListSourceIndex(visibleIndex));
		}
		public override int? GetVisibleIndex(int listSourceIndex) {
			return Do(m => m.GetVisibleIndex(listSourceIndex));
		}
		public override int VisibleRowCount {
			get {
				return Do(m => m.VisibleRowCount);
			}
		}
		public override int? HideRow(int sourceIndex) {
			return Do(m => m.HideRow(sourceIndex));
		}
		public override void ShowRow(int sourceIndex, int visibleIndex) {
			Do(m => m.ShowRow(sourceIndex, visibleIndex));
		}
		public override void InsertRow(int sourceIndex, int? visibleIndex = null) {
			Do(m => m.InsertRow(sourceIndex, visibleIndex));
		}
		public override int? RemoveRow(int sourceIndex) {
			return Do(m => m.RemoveRow(sourceIndex));
		}
		public override void MoveVisiblePosition(int oldVisibleIndex, int newVisibleIndex) {
			Do(m => m.MoveVisiblePosition(oldVisibleIndex, newVisibleIndex));
		}
		public override void MoveSourcePosition(int oldSourcePosition, int newSourcePosition) {
			Do(m => m.MoveSourcePosition(oldSourcePosition, newSourcePosition));
		}
		public override int[] ToArray() {
			var aL = ListMapper.ToArray();
			var aS = SmartMapper.ToArray();
			if(!aL.SequenceEqual(aS))
				throw new InvalidOperationException("!aL.SequenceEqual(aS)");
			return aS;
		}
		public override IEnumerable<int> ToEnumerable() {
			ToArray();
			return SmartMapper.ToEnumerable();
		}
	}
}
