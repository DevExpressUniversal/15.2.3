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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraPivotGrid.Data {
#if SL
   public class SortedList<TKey, TValue> : Dictionary<TKey, TValue> {
		public SortedList() { }
		public SortedList(IEqualityComparer<TKey> comparer)
			: base(comparer) { }
	}
#endif
	class FieldValuePositionContainer : LifeTimeListContainer<int, PositionDic> {
		public FieldValuePositionContainer(int countLimit, TimeSpan lifeTime, TimeSpan minClearCheckPeriod)
			: base(countLimit, lifeTime, minClearCheckPeriod) {
			List = new PositionList();
		}
		protected new PositionList List {
			get { return (PositionList)base.List; }
			set { base.List = value; }
		}
		public bool TryGetValue(int start, int level, out int result, out PositionDic dic) {
			if(!List.TryGetValue(start, out dic)) {
				dic = new PositionDic(DateTime.Now);
				List.Add(start, dic);
				result = 0;
				return false;
			}
#if DEBUGTEST
			if(dic == null) {
				System.Text.StringBuilder str = new System.Text.StringBuilder();
				str.AppendLine().Append("List.Count == '").Append(List.Count).Append("'");
				str.AppendLine().Append("List.Keys.Count == '").Append(List.Keys.Count).Append("'");
				str.AppendLine().Append("List.Values.Count == '").Append(List.Values.Count).Append("'");
				str.AppendLine().Append("<Key>").Append(". ").Append("<Value>");
				foreach(KeyValuePair<int, PositionDic> pair in List) {
					str.AppendLine().Append(pair.Key).Append(". ").Append(pair.Value == null ? "(null)" : pair.Value.ToString());
				}
				throw new ArgumentNullException(string.Format("dic" + str.ToString()), "FieldValuePositionContainer");
			}
#endif
			return dic.Dic.TryGetValue(level, out result);
		}
	}
	class PositionList : SortedList<int, PositionDic> {
		public PositionList()
			: base() {
		}
		public new void Add(int key, PositionDic value) {
			if(value == null)
				throw new ArgumentNullException(string.Format("value, key={0}", key), "PositionList");
			base.Add(key, value);
		}
	}
	class PositionDic : ITimeRecord {
		Dictionary<int, int> dic;
		DateTime time;
		public PositionDic(DateTime time) {
			this.dic = new Dictionary<int, int>();
			this.time = time;
		}
		public Dictionary<int, int> Dic { get { return dic; } }
		DateTime ITimeRecord.Time { get { return time; } }
	}
	public class SizeRecord : ITimeRecord {
		Size size;
		DateTime time;
		public SizeRecord(Size size, DateTime time) {
			this.size = size;
			this.time = time;
		}
		public Size Size { get { return size; } }
		DateTime ITimeRecord.Time { get { return time; } }
	}
	public interface ITimeRecord {
		DateTime Time { get; }
	}
	public abstract class LifeTimeListContainer<T1, T2> where T2 : ITimeRecord {
		int countLimit;
		TimeSpan lifeTime, minClearCheckPeriod;
		DateTime lastClearCheck;
		protected LifeTimeListContainer(int countLimit, TimeSpan lifeTime, TimeSpan minClearCheckPeriod) {
			this.countLimit = countLimit;
			this.lifeTime = lifeTime;
			this.minClearCheckPeriod = minClearCheckPeriod;
			this.lastClearCheck = DateTime.Now;
		}
		public int Count { get { return List.Count; } }
		public void Clear() {
			List.Clear();
		}
		protected SortedList<T1, T2> List { get; set; }
		protected int CountLimit { get { return countLimit; } }
		protected TimeSpan LifeTime { get { return lifeTime; } }
		protected TimeSpan MinClearCheckPeriod { get { return minClearCheckPeriod; } }
		protected virtual DateTime Now() { return DateTime.Now; }
		DateTime LastClearCheck {
			get { return lastClearCheck; }
		}
		public void OnAdd() {
			if(List.Count >= CountLimit && DateTime.Now - LastClearCheck > MinClearCheckPeriod)
				RemoveOldRecords();
		}
		protected void RemoveOldRecords() {
			lastClearCheck = Now();
#if !SL
			for(int i = List.Values.Count - 1; i >= 0; i--)
				if(lastClearCheck.Subtract(List.Values[i].Time) >= LifeTime)
					List.RemoveAt(i);
#else
			List<T1> toRemove = new List<T1>();
			foreach(KeyValuePair<T1, T2> pair in List)
				if(lastClearCheck.Subtract(pair.Value.Time) >= LifeTime)
					toRemove.Add(pair.Key);
			for(int i = 0; i < toRemove.Count; i++)
				List.Remove(toRemove[i]);
#endif
		}
	}
}
