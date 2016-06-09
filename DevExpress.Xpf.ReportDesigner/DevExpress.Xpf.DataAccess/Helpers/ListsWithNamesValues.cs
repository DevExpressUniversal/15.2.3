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
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using SortingDirection = DevExpress.DataAccess.Sql.SortingInfo.SortingDirection;
namespace DevExpress.Xpf.DataAccess.Native {
	#region Inner classes
	public class NameValueItem {
		public NameValueItem(string name, object value) {
			this.name = name;
			this.value = value;
		}
		readonly string name;
		public string Name { get { return name; } }
		readonly object value;
		public object Value { get { return value; } }
	}
	#endregion
	public static class ListsWithNamesValues {
		static ListsWithNamesValues() {
			joinTypes = new List<NameValueItem>() {
				new NameValueItem("Inner join", JoinType.Inner),
				new NameValueItem("Left outer join", JoinType.LeftOuter)
			}.AsReadOnly();
			sortingDirections = new List<NameValueItem>() {
				new NameValueItem("Unsorted", null),
				new NameValueItem("Ascending", SortingDirection.Ascending),
				new NameValueItem("Descending", SortingDirection.Descending)
			}.AsReadOnly();
			aggregationTypes = new List<NameValueItem>() {
				new NameValueItem("Count", AggregationType.Count),
				new NameValueItem("Max", AggregationType.Max),
				new NameValueItem("Min", AggregationType.Min),
				new NameValueItem("Avg", AggregationType.Avg),
				new NameValueItem("Sum", AggregationType.Sum),
				new NameValueItem("None", AggregationType.None)
			}.AsReadOnly();
			parameterTypes = new List<NameValueItem>() {
				new NameValueItem("String", typeof(String)),
				new NameValueItem("Date", typeof(DateTime)),
				new NameValueItem("Number (16 bit integer)", typeof(Int16)),
				new NameValueItem("Number (32 bit integer)", typeof(Int32)),
				new NameValueItem("Number (64 bit integer)", typeof(Int64)),
				new NameValueItem("Number (floating-point)", typeof(Single)),
				new NameValueItem("Number (double-precision floating-point)", typeof(Single)),
				new NameValueItem("Number (decimal)", typeof(Double)),
				new NameValueItem("Boolean", typeof(Boolean)),
				new NameValueItem("Guid", typeof(Guid))
			};
			fieldInfoTypes = new List<NameValueItem>() {
				new NameValueItem("Byte", typeof(Byte)),
				new NameValueItem("Boolean", typeof(Boolean)),
				new NameValueItem("Signed Byte", typeof(SByte)),
				new NameValueItem("Integer", typeof(Int32)),
				new NameValueItem("Unsigned Integer", typeof(UInt32)),
				new NameValueItem("Short Integer", typeof(Int16)),
				new NameValueItem("Unsigned Short Integer", typeof(UInt16)),
				new NameValueItem("Long Integer", typeof(Int64)),
				new NameValueItem("Unsigned Long Integer", typeof(UInt64)),
				new NameValueItem("Double", typeof(Double)),
				new NameValueItem("Float", typeof(Single)),
				new NameValueItem("Decimal", typeof(Decimal)),
				new NameValueItem("DateTime", typeof(DateTime)),
				new NameValueItem("Char", typeof(Char)),
				new NameValueItem("String", typeof(String)),
				new NameValueItem("Object", typeof(Object))
			}.AsReadOnly();
		}
		static readonly IEnumerable<NameValueItem> joinTypes;
		public static IEnumerable<NameValueItem> JoinTypes { get { return joinTypes; } }
		static readonly IEnumerable<NameValueItem> sortingDirections;
		public static IEnumerable<NameValueItem> SortingDirections { get { return sortingDirections; } }
		static readonly IEnumerable<NameValueItem> aggregationTypes;
		public static IEnumerable<NameValueItem> AggregationTypes { get { return aggregationTypes; } }
		static readonly IEnumerable<NameValueItem> parameterTypes;
		public static IEnumerable<NameValueItem> ParameterTypes { get { return parameterTypes; } }
		static readonly IEnumerable<NameValueItem> fieldInfoTypes;
		public static IEnumerable<NameValueItem> FieldInfoTypes { get { return fieldInfoTypes; } }
	}
}
