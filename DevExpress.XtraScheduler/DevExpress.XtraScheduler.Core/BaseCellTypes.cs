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
using System.Drawing;
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	#region ITimeCell
	public interface ITimeCell {
		TimeInterval Interval { get; }
		Resource Resource { get; }
	}
	#endregion
	#region TimeCellStartDateComparerCore
	public class TimeCellStartDateComparerCore : IComparer<ITimeCell> {
		#region IComparer<ITimeCell> Members
		public int Compare(ITimeCell x, ITimeCell y) {
			return x.Interval.Start.CompareTo(y.Interval.Start);
		}
		#endregion
	}
	#endregion
	#region TimeCellEndDateComparerCore
	public class TimeCellEndDateComparerCore : IComparer<ITimeCell> {		
		#region IComparer<ITimeCell> Members
		public int Compare(ITimeCell x, ITimeCell y) {
			return x.Interval.End.CompareTo(y.Interval.End);
		}
		#endregion
	}
	#endregion
	#region TestSchedulerViewCellBase
	public class TestSchedulerViewCellBase : ITimeCell {
		TimeInterval interval = new TimeInterval();
		#region ITimeCell Members
		public TimeInterval Interval { get { return interval; } set { interval = value; } }
		public Resource Resource { get { return ResourceBase.Empty; } } 
		#endregion
	}
	#endregion
	#region ITimeIntervalCollection
	public interface ITimeIntervalCollection {
		int BinarySearchEndDate(DateTime date);
		int BinarySearchNextStartDate(DateTime date);
		int BinarySearchPreviousEndDate(DateTime date);
		int BinarySearchStartDate(DateTime date);
		ITimeIntervalCollection CreateEmptyCollection();
		ITimeCell this[int index] { get; }
		int Count { get; }
	}
	#endregion
	#region SchedulerViewCellBaseCollectionCore
	public abstract class SchedulerViewCellBaseCollectionCore<TSchedulerViewCellBase> : DXCollectionBase<ITimeCell>, ITimeIntervalCollection
		where TSchedulerViewCellBase : ITimeCell {
		TimeCellStartDateComparerCore startDateComparer;
		TimeCellEndDateComparerCore endDateComparer;
		protected SchedulerViewCellBaseCollectionCore()
			: base() {
			startDateComparer = new TimeCellStartDateComparerCore();
			endDateComparer = new TimeCellEndDateComparerCore();
		}
		ITimeCell ITimeIntervalCollection.this[int index] { get { return this[index]; } }
		internal IComparer<ITimeCell> StartDateComparer { get { return startDateComparer; } }
		internal IComparer<ITimeCell> EndDateComparer { get { return endDateComparer; } }
		public abstract ITimeIntervalCollection CreateEmptyCollection();
		public TSchedulerViewCellBase this[int index] {
			get { return (TSchedulerViewCellBase)List[index]; }
		}
		public int BinarySearchStartDate(DateTime date) {
			TestSchedulerViewCellBase cell = new TestSchedulerViewCellBase();
			cell.Interval.Start = date;
			return BinarySearch(cell, StartDateComparer);
		}
		public int BinarySearchEndDate(DateTime date) {
			TestSchedulerViewCellBase cell = new TestSchedulerViewCellBase();
			cell.Interval.End = date;
			return BinarySearch(cell, EndDateComparer);
		}
		public int BinarySearchNextStartDate(DateTime date) {
			int result;
			int binarySearchResult = BinarySearchStartDate(date);
			if(binarySearchResult >= 0)
				result = binarySearchResult + 1;
			else
				result = ~binarySearchResult;
			if(result >= Count)
				result = -1;
			return result;
		}
		public int BinarySearchPreviousEndDate(DateTime date) {
			int result;
			int binarySearchResult = BinarySearchEndDate(date);
			if(binarySearchResult >= 0)
				result = binarySearchResult - 1;
			else
				result = ~binarySearchResult - 1;
			return result;
		}
		public void ForEach(Action<TSchedulerViewCellBase> action) {
			int count = this.Count;
			for(int i = 0; i < count; i++)
				action(this[i]);
		}
	}
	#endregion
}
