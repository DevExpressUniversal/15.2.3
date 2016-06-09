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
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTupleCacheSetSortOrder (enum)
	public enum PivotTupleCacheSetSortOrder {
		None,
		Ascending,
		AscendingAlphabetic,
		AscendingNatural,
		Descending,
		DescendingAlphabetic,
		DescendingNatural,
	}
	#endregion
	#region PivotTupleCache
	public class PivotTupleCache {
		PivotTupleCacheEntries entries;
		PivotTupleCacheSets sets;
		PivotTupleCacheQueries queryCache;
		PivotTupleCacheServerFormats serverFormats;
		public PivotTupleCacheEntries Entries { get { return entries; } set { entries = value; } }
		public PivotTupleCacheSets Sets { get { return sets; } set { sets = value; } }
		public PivotTupleCacheQueries QueryCache { get { return queryCache; } set { queryCache = value; } }
		public PivotTupleCacheServerFormats ServerFormats { get { return serverFormats; } set { serverFormats = value; } }
		public void Clear() {
			Entries.Clear();
			Sets.Clear();
			QueryCache.Clear();
			ServerFormats.Clear();
		}
		public void CopyFrom(PivotTupleCache source) {
			Entries.CopyFrom(source.Entries);
			Sets.CopyFrom(source.Sets);
			QueryCache.CopyFrom(source.QueryCache);
			ServerFormats.CopyFrom(source.ServerFormats);
		}
	}
	#endregion
	#region PivotTupleCacheEntries
	public class PivotTupleCacheEntries : PivotCacheRecordValueCollection {
		public void CopyFrom(PivotTupleCacheEntries source) {
			this.Clear();
			this.Capacity = source.Count;
			foreach (IPivotCacheRecordValue sourceItem in source) {
				this.Add(sourceItem.Clone());
			}
		}
	}
	#endregion
	#region PivotTupleCacheSets
	public class PivotTupleCacheSets : List<PivotTupleCacheSet> {
		public void CopyFrom(PivotTupleCacheSets source) {
			this.Clear();
			this.Capacity = source.Count;
			foreach (PivotTupleCacheSet sourceItem in source) {
				PivotTupleCacheSet targetItem = new PivotTupleCacheSet();
				targetItem.CopyFrom(sourceItem);
				this.Add(targetItem);
			}
		}
	}
	#endregion
	#region PivotTupleCacheSet
	public class PivotTupleCacheSet {
		int maxRank;
		string setDefinition;
		PivotTupleCacheSetSortOrder sortType;
		bool queryFailed;
		List<PivotTupleCollection> tuples;
		PivotTupleCollection sortByTuple;
		public List<PivotTupleCollection> Tuples { get { return tuples; } set { tuples = value; } }
		public PivotTupleCollection SortByTuple { get { return sortByTuple; } set { sortByTuple = value; } }
		public int MaxRank { get { return maxRank; } set { maxRank = value; } }
		public string SetDefinition { get { return setDefinition; } set { setDefinition = value; } }
		public PivotTupleCacheSetSortOrder SortType { get { return sortType; } set { sortType = value; } }
		public bool QueryFailed { get { return queryFailed; } set { queryFailed = value; } }
		public void CopyFrom(PivotTupleCacheSet source) {
			this.maxRank = source.MaxRank;
			this.setDefinition = source.setDefinition;
			this.sortType = source.sortType;
			this.queryFailed = source.queryFailed;
			Debug.Assert(this.tuples.Count == 0);
			this.tuples.Capacity = source.tuples.Count;
			foreach (PivotTupleCollection sourceTupleCollectionItem in source.Tuples) {
				PivotTupleCollection targetTupleCollectionItem = sourceTupleCollectionItem.Clone();
				this.tuples.Add(targetTupleCollectionItem);
			}
			this.sortByTuple = source.SortByTuple.Clone();
		}
	}
	#endregion
	#region PivotTupleCacheQueries
	public class PivotTupleCacheQueries : List<PivotTupleCacheQuery> {
		public void CopyFrom(PivotTupleCacheQueries source) {
			System.Diagnostics.Debug.Assert(this.Count == 0);
			this.Capacity = source.Count;
			foreach (PivotTupleCacheQuery sourceItem in source) {
				PivotTupleCacheQuery targetItem = new PivotTupleCacheQuery();
				targetItem.CopyFrom(sourceItem);
				this.Add(targetItem);
			}
		}
	}
	#endregion
	#region PivotTupleCacheQuery
	public class PivotTupleCacheQuery {
		string mdx;
		PivotTupleCollection tuples;
		public string Mdx { get { return mdx; } set { mdx = value; } }
		public PivotTupleCollection Tuples { get { return tuples; } set { tuples = value; } }
		public void CopyFrom(PivotTupleCacheQuery source) {
			this.mdx = source.Mdx;
			System.Diagnostics.Debug.Assert(this.tuples.Count == 0);
			this.tuples.Capacity = source.tuples.Count;
			foreach (PivotTuple sourceItem in source.Tuples) {
				PivotTuple targetItem = sourceItem.Clone();
				this.tuples.Add(targetItem);
			}
		}
	}
	#endregion
	#region PivotTupleCacheServerFormats
	public class PivotTupleCacheServerFormats : List<PivotTupleCacheServerFormat> {
		public void CopyFrom(PivotTupleCacheServerFormats source) {
			System.Diagnostics.Debug.Assert(this.Count == 0);
			this.Capacity = source.Count;
			foreach (PivotTupleCacheServerFormat sourceItem in source) {
				PivotTupleCacheServerFormat targetItem = new PivotTupleCacheServerFormat();
				targetItem.CopyFrom(sourceItem);
				this.Add(targetItem);
			}
		}
	}
	#endregion
	#region PivotTupleCacheServerFormat
	public class PivotTupleCacheServerFormat {
		string culture; 
		string format; 
		public string Culture { get { return culture; } set { culture = value; } }
		public string Format { get { return format; } set { format = value; } }
		public void CopyFrom(PivotTupleCacheServerFormat source) {
			this.culture = source.culture;
			this.format = source.format;
		}
	}
	#endregion
}
