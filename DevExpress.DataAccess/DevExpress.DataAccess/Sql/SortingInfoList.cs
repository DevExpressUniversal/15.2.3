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
namespace DevExpress.DataAccess.Sql {
	public sealed class SortingInfoList : List<SortingInfo> {
		public class EqualityComparer : IEqualityComparer<SortingInfoList> {
			public static bool Equals(SortingInfoList x, SortingInfoList y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				if(x.Count != y.Count)
					return false;
				Enumerator xEnumerator = x.GetEnumerator();
				Enumerator yEnumerator = y.GetEnumerator();
				while(xEnumerator.MoveNext() && yEnumerator.MoveNext())
					if(!SortingInfo.EqualityComparer.Equals(xEnumerator.Current, yEnumerator.Current))
						return false;
				return true;
			}
			#region Implementation of IEqualityComparer<in SortingInfoList>
			bool IEqualityComparer<SortingInfoList>.Equals(SortingInfoList x, SortingInfoList y) { return Equals(x, y); }
			int IEqualityComparer<SortingInfoList>.GetHashCode(SortingInfoList obj) { return 0; }
			#endregion
		}
		public SortingInfoList() { }
		public SortingInfoList(int capacity) : base(capacity) { }
		public SortingInfoList(IEnumerable<SortingInfo> collection) : base(collection) { }
		public void AddRange(SortingInfo[] collection) { base.AddRange(collection); }
		public void Add(string table, string column) { Add(new SortingInfo(table, column)); }
		public void Add(string table, string column, SortingInfo.SortingDirection direction) { Add(new SortingInfo(table, column, direction)); }
		public bool Remove(string table, string column) { return RemoveAll(info => string.Equals(info.Column, column, StringComparison.Ordinal) && string.Equals(info.Table, table, StringComparison.Ordinal)) > 0; }
	}
}
