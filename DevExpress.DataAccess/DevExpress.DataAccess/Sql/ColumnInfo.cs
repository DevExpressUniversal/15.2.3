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
using System.ComponentModel;
using DevExpress.Data.Filtering;
namespace DevExpress.DataAccess.Sql {
	public sealed class ColumnInfo {
		public class EqualityComparer : IEqualityComparer<ColumnInfo> {
			public static bool Equals(ColumnInfo x, ColumnInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				return x.aggregation == y.aggregation && string.Equals(x.name, y.name, StringComparison.Ordinal) && string.Equals(x.alias, y.alias, StringComparison.Ordinal);
			}
			#region IEqualityComparer<ColumnInfo> Members            
			bool IEqualityComparer<ColumnInfo>.Equals(ColumnInfo x, ColumnInfo y) { return Equals(x, y); }
			int IEqualityComparer<ColumnInfo>.GetHashCode(ColumnInfo obj) { return 0; }
			#endregion
		}
		string name;
		string alias;
		AggregationType aggregation;
		public ColumnInfo(string name, AggregationType aggregation, string alias) {
			this.name = name;
			this.alias = alias;
			this.aggregation = aggregation;
		}
		public ColumnInfo(string name, string alias) : this(name, AggregationType.None, alias) { }
		public ColumnInfo(string name, AggregationType aggregation) : this(name, aggregation, null) { }
		public ColumnInfo(string name) : this(name, AggregationType.None, null) { }
		public ColumnInfo() : this(null, AggregationType.None, null) { }
		internal ColumnInfo(ColumnInfo other) : this(other.name, other.aggregation, other.alias) { }
		public string Name { get { return name; } set { name = value; } }
		[DefaultValue(null)]
		public string Alias {
			get { return alias; }
			set {
				if(value != null && value.Length == 0)
					throw  new ArgumentException();
				alias = value;
			}
		}
		[DefaultValue(AggregationType.None)]
		public AggregationType Aggregation { get { return aggregation; } set { aggregation = value; } }
		public bool HasAlias { get { return alias != null; } }
		public string ActualName { get { return alias ?? name; } }
	}
	public enum AggregationType {
		Count = Aggregate.Count,
		Max = Aggregate.Max,
		Min = Aggregate.Min,
		Avg = Aggregate.Avg,
		Sum = Aggregate.Sum,
		None
	}
}
