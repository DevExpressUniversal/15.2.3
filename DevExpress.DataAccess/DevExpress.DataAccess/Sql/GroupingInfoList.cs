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
namespace DevExpress.DataAccess.Sql {
	public sealed class GroupingInfoList : List<GroupingInfo> {
		public class EqualityComparer : IEqualityComparer<GroupingInfoList> {
			public static bool Equals(GroupingInfoList x, GroupingInfoList y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				if(x.Count != y.Count)
					return false;
				Enumerator xEnumerator = x.GetEnumerator();
				Enumerator yEnumerator = y.GetEnumerator();
				while(xEnumerator.MoveNext() && yEnumerator.MoveNext())
					if(!GroupingInfo.EqualityComparer.Equals(xEnumerator.Current, yEnumerator.Current))
						return false;
				return true;
			}
			#region Implementation of IEqualityComparer<in GroupingInfoList>
			bool IEqualityComparer<GroupingInfoList>.Equals(GroupingInfoList x, GroupingInfoList y) { return Equals(x, y); }
			public int GetHashCode(GroupingInfoList obj) { return 0; }
			#endregion
		}
		public GroupingInfoList() { }
		public GroupingInfoList(int capacity) : base(capacity) { }
		public GroupingInfoList(IEnumerable<GroupingInfo> collection) : base(collection.Select(item => new GroupingInfo(item))) { }
		public GroupingInfoList(GroupingInfoList other) : this(other.AsEnumerable()) { }
		public void AddRange(GroupingInfo[] collection) { base.AddRange(collection); }
		public void Add(string table, string column) { Add(new GroupingInfo(table, column)); }
		public void Remove(string table, string column) {
			GroupingInfo value = new GroupingInfo(table, column);
			RemoveAll(item => GroupingInfo.EqualityComparer.Equals(item, value));
		}
	}
}
