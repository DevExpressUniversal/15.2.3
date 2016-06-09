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
using System.Collections.ObjectModel;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.QueryMode {
	public class QueryMember : IQueryMember {
		readonly object value;
		readonly IQueryMetadataColumn column;
		public static readonly string TotalValue = "XtraPivotGridTotal";
		public virtual bool IsTotal { get { return false; } }
		public virtual bool IsOthers { get { return false; } }
		public virtual string Caption {
			get {
				if(Value != null)
					return Value.ToString();
				return string.Empty;
			}
		}
		public object Value { get { return value; } }
		public IQueryMetadataColumn Column { get { return column; } }
		public QueryMember(IQueryMetadataColumn column, object value) {
			this.column = column;
			this.value = value;
		}
		public override string ToString() {
			return value != null ? value.ToString() : "null";
		}
		#region IQueryMember Members
		IQueryMetadataColumn IQueryMember.Column {
			get { return Column; }
		}
		public virtual object UniqueLevelValue {
			get {
				return Value;
			}
		}
		#endregion
	}
	public class QueryVirtualMember : QueryMember {
		bool isTotal;
		public override bool IsTotal { get { return isTotal; } }
		public QueryVirtualMember(IQueryMetadataColumn column, bool isTotal) : base(column, TotalValue) {
			this.isTotal = isTotal;
		}
	}
}
