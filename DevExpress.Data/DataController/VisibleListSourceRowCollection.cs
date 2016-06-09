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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Helpers;
namespace DevExpress.Data {
	public class VisibleListSourceRowCollection: IDisposable {
		protected DataControllerBase controller;	
		VisibleToSourceRowsMapper _Mapper;
		VisibleToSourceRowsMapper GetMapper() {
			if(_Mapper == null)
				throw new InvalidOperationException("MapperNotCreated");
			return _Mapper;
		}
		string appliedFilterExpression;
		bool hasUserFilter;
		public string AppliedFilterExpression { get { return appliedFilterExpression; } }
		public bool HasUserFilter { get { return hasUserFilter; } }
		public VisibleListSourceRowCollection(DataControllerBase controller) {
			this.controller = controller;
		}
		public abstract class ValuesGetterCacheBuilder: GenericInvoker<Func<int, Func<int, ExpressiveSortInfo.CondencedAndSourceIndicesPair>, Delegate, bool, int, Delegate>, ValuesGetterCacheBuilder.Impl<object>> {
			public class Impl<T>: ValuesGetterCacheBuilder {
				static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> MakeFullyCached(int rowsCount, Func<int, ExpressiveSortInfo.CondencedAndSourceIndicesPair> condenced2FullIndex, Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> getter) {
					T[] cache = new T[rowsCount];
					for(int c = 0; c < rowsCount; ++c) {
						cache[c] = getter(condenced2FullIndex(c));
					}
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> advanced = r => cache[r.CondencedIndex];
					return advanced;
				}
				static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> MakeLazyCached(int rowsCount, Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> getter) {
					BitArray cacheFlags = new BitArray(rowsCount);
					T[] cache = new T[rowsCount];
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> advanced =
						r => {
							int cindex = r.CondencedIndex;
							if(cacheFlags[cindex])
								return cache[cindex];
							T value = getter(r);
							cache[cindex] = value;
							cacheFlags[cindex] = true;
							return value;
						};
					return advanced;
				}
				static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> Make(int rowsCount, Func<int, ExpressiveSortInfo.CondencedAndSourceIndicesPair> condenced2FullIndex, Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> getter, bool isThreadSafe, int pos) {
					if(isThreadSafe || pos < 2)
						return MakeFullyCached(rowsCount, condenced2FullIndex, (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T>)getter);
					else
						return MakeLazyCached(rowsCount, (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T>)getter);
				}
				protected override Func<int, Func<int, ExpressiveSortInfo.CondencedAndSourceIndicesPair>, Delegate, bool, int, Delegate> CreateInvoker() {
					return (rowsCount, condenced2FullIndex, getter, isThreadSafe, pos) =>
						Make(rowsCount, condenced2FullIndex, (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T>)getter, isThreadSafe, pos);
				}
			}
		}
		public static int PlinqSortThreshold = 1024;
		public void SortRows(DataColumnSortInfo[] sortInfo) {
			if(this.VisibleRowCount < 2) {
				ForceNonIdentity();
				return;
			}
			IComparer<ExpressiveSortInfo.CondencedAndSourceIndicesPair> comparer;
			int degreeOfParallelism;
			BuildSortDelegatesAndChooseDegreeOfParallelism(sortInfo, out comparer, out degreeOfParallelism);
			List<ExpressiveSortInfo.CondencedAndSourceIndicesPair> indicesPairs = new List<ExpressiveSortInfo.CondencedAndSourceIndicesPair>(this.VisibleRowCount);
			indicesPairs.AddRange(Enumerable.Range(0, this.VisibleRowCount).Select(condenced => new ExpressiveSortInfo.CondencedAndSourceIndicesPair(condenced, this.GetListSourceRow(condenced))));
			List<ExpressiveSortInfo.CondencedAndSourceIndicesPair> sorted;
			if(degreeOfParallelism == 1) {
				sorted = indicesPairs;
				sorted.Sort(comparer);
			} else {
				var parallelized = indicesPairs.AsParallel();
				if(degreeOfParallelism != 0)
					parallelized = parallelized.WithDegreeOfParallelism(degreeOfParallelism);
				sorted = StaSafeHelper.Invoke(
					() =>
						parallelized.OrderBy(ri => ri, comparer).ToList()
					);
			}
			Assign(sorted.Select(s => s.SourceIndex), sorted.Count);
		}
		void BuildSortDelegatesAndChooseDegreeOfParallelism(DataColumnSortInfo[] sortInfo, out IComparer<ExpressiveSortInfo.CondencedAndSourceIndicesPair> comparer, out int degreeOfParallelism) {
			bool plinqAble = this.VisibleRowCount >= PlinqSortThreshold;
			if(plinqAble) {
				int maxDoP = AvailableDegreeOfParallelismCalculator.GetMaxDoP();
				if(maxDoP > 1) {
					int realCores = AvailableDegreeOfParallelismCalculator.GetRealCoresToUse();
					if(realCores > 1) {
						var dopcalculator = AvailableDegreeOfParallelismCalculator.Start(Math.Min(maxDoP, realCores));
						try {
							BuildSortDelegatesCore(sortInfo, ref plinqAble, out comparer);
						} finally {
							dopcalculator.Finish();
						}
						if(plinqAble) {
							if(dopcalculator.Result <= 1) {
								degreeOfParallelism = 1;
							} else {
								if(dopcalculator.Result == maxDoP)
									degreeOfParallelism = 0;
								else
									degreeOfParallelism = dopcalculator.Result;
							}
						} else {
							degreeOfParallelism = 1;
						}
						return;
					}
				}
				plinqAble = false;
			}
			BuildSortDelegatesCore(sortInfo, ref plinqAble, out comparer);
			degreeOfParallelism = 1;
		}
		void BuildSortDelegatesCore(DataColumnSortInfo[] sortInfo, ref bool plinqAble, out IComparer<ExpressiveSortInfo.CondencedAndSourceIndicesPair> comparer) {
			Func<int, ExpressiveSortInfo.CondencedAndSourceIndicesPair> condenced2fullIndexConverter =
			c => new ExpressiveSortInfo.CondencedAndSourceIndicesPair(c, this.GetListSourceRow(c));
			Func<Type, Delegate, bool, int, Delegate> finalCompareValueGetterCacher
				= (type, valueGetter, threadSafe, pos)
					=> ValuesGetterCacheBuilder.GetInvoker(type)(this.VisibleRowCount, condenced2fullIndexConverter, valueGetter, threadSafe, pos);
			var compare = ExpressiveSortHelpers.MakeRowsCompare(controller.SortClient, sortInfo, controller.Helper, finalCompareValueGetterCacher, ref plinqAble);
			comparer = new ExpressiveSortHelpers.ComparisonComparer<ExpressiveSortInfo.CondencedAndSourceIndicesPair>(compare);
		}
		public int FindControllerRowForInsert(DataColumnSortInfo[] sortInfoCollection, int listSourceRow, int? oldVisiblePosition = null) {
			if(IsIdentity) return listSourceRow;
			if(sortInfoCollection.Length == 0) {
				if(oldVisiblePosition != null) return oldVisiblePosition.Value;
			}
			return FindControllerRowForInsertExpressive(sortInfoCollection, listSourceRow, oldVisiblePosition);
		}
		public abstract class SpecificRowCacheBuilder: GenericInvoker<Func<Delegate, int, Delegate>, SpecificRowCacheBuilder.Impl<object>> {
			public class Impl<T>: SpecificRowCacheBuilder {
				static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> MakeOneRowCachedCore(Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> getter, int listSourceRowCached) {
					bool cached = false;
					T value = default(T);
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> advancedGetter = r => {
						if(r.SourceIndex != listSourceRowCached)
							return getter(r);
						if(!cached) {
							value = getter(r);
							cached = true;
						}
						return value;
					};
					return advancedGetter;
				}
				protected override Func<Delegate, int, Delegate> CreateInvoker() {
					return (simpleGetter, sourceRowIndexForCache) => MakeOneRowCachedCore((Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T>)simpleGetter, sourceRowIndexForCache);
				}
			}
		}
		Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> BuildFindControllerRowForInsertExpressiveComparer(DataColumnSortInfo[] sortInfoCollection, int listSourceRow) {
			Func<Type, Delegate, bool, int, Delegate> finalCompareValueGetterCacher
				= (type, valueGetter, threadSafe, pos)
					=> SpecificRowCacheBuilder.GetInvoker(type)(valueGetter, listSourceRow);
			bool isPlinqAble = false;
			return ExpressiveSortHelpers.MakeRowsCompare(controller.SortClient, sortInfoCollection, controller.Helper, finalCompareValueGetterCacher, ref isPlinqAble);
		}
		int FindControllerRowForInsertExpressive(DataColumnSortInfo[] sortInfoCollection, int listSourceRow, int? oldVisiblePosition) {
			Func<int, int, int> comparison;
			{
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> coreCompare = null;
				comparison = (r1, r2) => {
					if(r1 == r2)
						return 0;
					if(coreCompare == null)
						coreCompare = BuildFindControllerRowForInsertExpressiveComparer(sortInfoCollection, listSourceRow);
					return coreCompare(new ExpressiveSortInfo.CondencedAndSourceIndicesPair(int.MinValue, r1), new ExpressiveSortInfo.CondencedAndSourceIndicesPair(int.MinValue, r2));
				};
			}
			return FindControllerRowForInsertExpressiveCore(listSourceRow, oldVisiblePosition, comparison);
		}
		int FindControllerRowForInsertExpressiveCore(int listSourceRow, int? oldVisiblePosition, Func<int, int, int> comparison) {
			int low = 0;
			int high = VisibleRowCount;
			int? puncture = (oldVisiblePosition.HasValue && oldVisiblePosition.Value >= 0 && GetListSourceRow(oldVisiblePosition.Value) == listSourceRow) ? (int?)oldVisiblePosition.Value : null;
			for(; ; ) {
				int pivot;
				if(puncture.HasValue && low <= puncture.Value && puncture.Value < high) {
					int span = high - low - 1;
					if(span == 0)
						break;
					int relativePuncture = puncture.Value - low;
					int relativeAfterPuncture = span - relativePuncture;
					if(relativePuncture >= relativeAfterPuncture) {
						pivot = puncture.Value - 1;
					} else {
						pivot = puncture.Value + 1;
					}
					if(pivot == puncture.Value)
						throw new InvalidOperationException("internal error (pivot == puncture.Value)");
				} else {
					int span = high - low;
					if(span == 0)
						break;
					if(high == VisibleRowCount && low > 0 && low < VisibleRowCount - 2) {
						if(GetListSourceRow(VisibleRowCount - 1) == listSourceRow) {
							pivot = VisibleRowCount - 2;
						} else {
							pivot = VisibleRowCount - 1;
						}
					} else if(low == 0 && high < VisibleRowCount && high > 2) {
						if(GetListSourceRow(0) == listSourceRow) {
							pivot = 1;
						} else {
							pivot = 0;
						}
					} else {
						pivot = low + span / 2;
					}
				}
				if(pivot < low)
					throw new InvalidOperationException("internal error (pivot < low)");
				if(pivot >= high)
					throw new InvalidOperationException("internal error (pivot >= high)");
				int pivotListSourceIndex = GetListSourceRow(pivot);
				if(pivotListSourceIndex == listSourceRow) {
					if(puncture.HasValue)
						throw new InvalidOperationException("internal error (puncture.HasValue already)");
					puncture = pivot;
					continue;
				}
				var cmp = comparison(listSourceRow, pivotListSourceIndex);
				if(cmp == 0)
					cmp = listSourceRow.CompareTo(pivotListSourceIndex);
				if(cmp < 0) {
					high = pivot;
				} else {
					low = pivot + 1;
				}
			}
			return high;
		}
		public void Dispose() {
			Clear();
			this.controller = null;
		}
		public bool IsIdentity { get { return _Mapper == null; } }
		public int VisibleRowCount {
			get {
				if(IsIdentity)
					return this.controller.ListSourceRowCount;
				else
					return GetMapper().VisibleRowCount;
			}
		}
		public int GetListSourceRow(int visibleRow) {
			if(IsIdentity)
				return visibleRow;
			else
				return GetMapper().GetListSourceIndex(visibleRow);
		}
		public int? GetVisibleRow(int listSourceRow) {
			if(listSourceRow < 0)
				return null;
			if(IsIdentity)
				return listSourceRow;
			else
				return GetMapper().GetVisibleIndex(listSourceRow);
		}
		public bool Contains(int listSourceRow) {
			if(IsIdentity)
				return true;
			else
				return GetMapper().Contains(listSourceRow);
		}
		public void Clear() {
			this.appliedFilterExpression = "";
			this.hasUserFilter = false;
			ClearMapper();
		}
		public void Assign(ICollection<int> records) {
			Assign(records, records.Count);
		}
		public void Assign(IEnumerable<int> records, int count) {
			ClearMapper();
			_Mapper = CreateMapper(records, count);
		}
		public void Init(int[] list, int? count = null, string appliedFilterExpression = "", bool hasUserFilter = false) {
			this.hasUserFilter = hasUserFilter;
			this.appliedFilterExpression = appliedFilterExpression;
			int actualCount = count ?? list.Length;
			if(actualCount == list.Length)
				Assign(list);
			else if(actualCount > list.Length)
				throw new InvalidOperationException("actualCount > list.Length");
			else
				Assign(list.Take(actualCount), actualCount);
		}
		public void ClearAndForceNonIdentity() {
			Clear();
			ForceNonIdentity();
		}
		public void ForceNonIdentity() {
			if(IsIdentity) {
				ClearMapper();
				_Mapper = CreateMapper();
			}
		}
		public void InsertHiddenRow(int listSourceRow) {
			if(IsIdentity)
				return;
			GetMapperForChange().InsertRow(listSourceRow, null);
		}
		public int HideSourceRow(int listSourceRow) {
			return GetMapperForChange().HideRow(listSourceRow) ?? DataController.InvalidRow;
		}
		public int RemoveSourceRow(int listSourceRow) {
			if(IsIdentity)
				return listSourceRow;
			return GetMapperForChange().RemoveRow(listSourceRow) ?? DataController.InvalidRow;
		}
		public void MoveSourcePosition(int oldSourcePosition, int newSourcePosition) {
			if(IsIdentity)
				return;
			GetMapperForChange().MoveSourcePosition(oldSourcePosition, newSourcePosition);
		}
		public void MoveVisiblePosition(int oldVisibleIndex, int newVisibleIndex) {
			GetMapperForChange().MoveVisiblePosition(oldVisibleIndex, newVisibleIndex);
		}
		VisibleListSourceRowCollection CloneBase() {
			var clone = new VisibleListSourceRowCollection(this.controller);
			clone.hasUserFilter = this.HasUserFilter;
			clone.appliedFilterExpression = this.AppliedFilterExpression;
			return clone;
		}
		public VisibleListSourceRowCollection ClonePersistent() {
			var clone = CloneBase();
			if(!IsIdentity){
				var baseCloneMapper = new VisibleToSourceRowsListMapper(this._Mapper);
				if(VisibleToSourceRowsDoublecheckMapper.Use)
					clone._Mapper = new VisibleToSourceRowsDoublecheckMapper(baseCloneMapper);
				else
					clone._Mapper = baseCloneMapper;
			}
			return clone;
		}
		public VisibleListSourceRowCollection CloneThatWouldBeForSureModifiedAndOrForgottenBeforeAnythingHappensToOriginal() {
			var clone = CloneBase();
			if(!IsIdentity) {
				if(this._Mapper.IsReadOnly)
					clone._Mapper = this._Mapper;
				else
					clone._Mapper = new VisibleToSourceRowsReadOnlyProxyMapper(this._Mapper);
			}
			return clone;
		}
		public void InsertVisibleRow(int sourceRowIndex, int visibleRowIndex) {
			if(IsIdentity)
				return;
			GetMapperForChange().InsertRow(sourceRowIndex, visibleRowIndex);
		}
		public void ShowRow(int sourceRowIndex, int visibleIndex) {
			GetMapperForChange().ShowRow(sourceRowIndex, visibleIndex);
		}
		public int[] ToArray() {
			return GetMapper().ToArray();
		}
		public void SetRange(int startPos, int[] newValues) {
			GetMapperForSetRange().SetRange(startPos, newValues);
		}
		void ClearMapper() {
			var mapper = _Mapper;
			if(mapper == null)
				return;
			_Mapper = null;
		}
		VisibleToSourceRowsMapper CreateMapper() {
			return new VisibleToSourceRowsFixedIdentityMapper(controller.ListSourceRowCount);
		}
		VisibleToSourceRowsMapper CreateMapper(IEnumerable<int> state, int count) {
			var listMapper = new VisibleToSourceRowsListMapper(state, count);
			if(VisibleToSourceRowsDoublecheckMapper.Use)
				return new VisibleToSourceRowsDoublecheckMapper(listMapper);
			else
				return listMapper;
		}
		VisibleToSourceRowsMapper GetMapperForChange() {
			var mapper = GetMapper();
			if(VisibleToSourceRowsDoublecheckMapper.Use) {
				if(mapper.IsReadOnly || mapper.IsSetRangeAble)
					_Mapper = new VisibleToSourceRowsDoublecheckMapper(mapper);
				return _Mapper;
			}
			if(mapper.IsReadOnly) {
				return _Mapper = new VisibleToSourceRowsListMapper(mapper);
			} else if(mapper is VisibleToSourceRowsSmartMapper && IsTooSmallForSmart(mapper.VisibleRowCount)) {
				return _Mapper = new VisibleToSourceRowsListMapper(mapper);
			} else {
				var listMapper = mapper as VisibleToSourceRowsListMapper;
				if(listMapper != null && IsBigEnoughForSmart(listMapper.VisibleRowCount) && listMapper.IsChangedEnoughForSmart) {
					return _Mapper = new VisibleToSourceRowsSmartMapper(mapper);
				}
			}
			return mapper;
		}
		protected VisibleToSourceRowsMapper GetMapperForSetRange() {
			var mapper = GetMapper();
			if(!mapper.IsSetRangeAble)
				_Mapper = new VisibleToSourceRowsListMapper(mapper);
			return _Mapper;
		}
		bool IsTooSmallForSmart(int visibleCount) {
			return visibleCount < 4096;
		}
		bool IsBigEnoughForSmart(int visibleCount) {
			return visibleCount >= 8192;
		}
	}
}
