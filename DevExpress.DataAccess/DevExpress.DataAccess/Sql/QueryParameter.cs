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
using DevExpress.Data;
using DevExpress.DataAccess.Native;
namespace DevExpress.DataAccess.Sql {
	public sealed class QueryParameter : DataSourceParameterBase {
		public class EqualityComparer : IEqualityComparer<QueryParameter> {
			public static bool Equals(QueryParameter x, QueryParameter y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				return string.Equals(x.Name, y.Name, StringComparison.Ordinal) && x.Type == y.Type && object.Equals(x.Value, y.Value);
			}
			#region IEqualityComparer<QueryParameter> Members
			bool IEqualityComparer<QueryParameter>.Equals(QueryParameter x, QueryParameter y) { return Equals(x, y); }
			int IEqualityComparer<QueryParameter>.GetHashCode(QueryParameter obj) { return 0; }
			#endregion
		}
		public QueryParameter() : this(null, typeof(string), null) { }
		public QueryParameter(string name, Type type, object value) : base(name,type,value) { }
		internal QueryParameter(IParameter other) : this(other.Name, other.Type, other.Value) { }
		internal new static QueryParameter FromIParameter(IParameter value) {
			return value == null ? null : (value as QueryParameter ?? new QueryParameter(value));
		}
	}
}
