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
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.DataAccess.Sql {
	public sealed class RelationColumnInfo {
		public class EqualityComparer : IEqualityComparer<RelationColumnInfo> { 
			public static bool Equals(RelationColumnInfo x, RelationColumnInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				return x.ConditionOperator == y.ConditionOperator &&
					   string.Equals(x.parentKeyColumn, y.parentKeyColumn, StringComparison.Ordinal) &&
					   string.Equals(x.nestedKeyColumn, y.nestedKeyColumn, StringComparison.Ordinal);
			}
			#region IEqualityComparer<RelationColumnInfo> Members
			bool IEqualityComparer<RelationColumnInfo>.Equals(RelationColumnInfo x, RelationColumnInfo y) { return Equals(x, y); }
			int IEqualityComparer<RelationColumnInfo>.GetHashCode(RelationColumnInfo obj) { return 0; }
			#endregion
		}
		public enum ConditionType {
			Equal = BinaryOperatorType.Equal,
			NotEqual = BinaryOperatorType.NotEqual,
			Greater = BinaryOperatorType.Greater,
			GreaterOrEqual = BinaryOperatorType.GreaterOrEqual,
			Less = BinaryOperatorType.Less,
			LessOrEqual = BinaryOperatorType.LessOrEqual
		}
		string parentKeyColumn;
		string nestedKeyColumn;
		ConditionType conditionType;
		public RelationColumnInfo(string parentKeyColumn, string nestedKeyColumn, ConditionType conditionType) {
			this.parentKeyColumn = parentKeyColumn;
			this.nestedKeyColumn = nestedKeyColumn;
			this.conditionType = conditionType;
		}
		public RelationColumnInfo(string parentKeyColumn, string nestedKeyColumn) : this(parentKeyColumn, nestedKeyColumn, ConditionType.Equal) { }
		public RelationColumnInfo() : this(null, null) { }
		internal RelationColumnInfo(RelationColumnInfo other) : this(other.parentKeyColumn, other.nestedKeyColumn, other.conditionType) { }
#if !SL
	[DevExpressDataAccessLocalizedDescription("RelationColumnInfoParentKeyColumn")]
#endif
		public string ParentKeyColumn { get { return parentKeyColumn; } set { parentKeyColumn = value; } }
#if !SL
	[DevExpressDataAccessLocalizedDescription("RelationColumnInfoNestedKeyColumn")]
#endif
		public string NestedKeyColumn { get { return nestedKeyColumn; } set { nestedKeyColumn = value; } }
#if !SL
	[DevExpressDataAccessLocalizedDescription("RelationColumnInfoConditionOperator")]
#endif
		[DefaultValue(ConditionType.Equal)]
		public ConditionType ConditionOperator { get { return conditionType; } set { conditionType = value; } }
		public override string ToString() {
			return string.Format("{0} {2} {1}", parentKeyColumn ?? "<null>", nestedKeyColumn ?? "<null>", CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString((BinaryOperatorType)conditionType));
		}
	}
}
