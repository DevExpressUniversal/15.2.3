#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Utils;
using System.Reflection;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardDistinctCountFunction : DashboardInternalCustomFunction, ICustomFunctionOperatorFormattable, DevExpress.Data.PivotGrid.IPivotCustomSummaryAggregate {
		internal const string FunctionName = "dashboarddistinctcount";
		internal const string ExpressionEditorName = "CountDistinct";
		string name;
		public DashboardDistinctCountFunction() : this(FunctionName) { }
		public DashboardDistinctCountFunction(string name) {
			this.name = name;
		}
		public override string Name { get { return name; } }
		public override Type ResultType(params Type[] operands) {
			return typeof(int);
		}
		string ICustomFunctionOperatorFormattable.Format(Type providerType, params string[] operands) {
			if(operands == null || operands.Length != 1 || !providerType.GetInterfaces().Contains(typeof(ISqlDataStore)))
				return "-1";
			return string.Format("Count(distinct {0})", operands[0]);
		}
		object DevExpress.Data.PivotGrid.IPivotCustomSummaryAggregate.Calculate(IEnumerable<object> enumerable) {
			Dictionary<object, object> dic = new Dictionary<object, object>();
			foreach(object val in enumerable)
				if(val != null)
					dic[val] = null;
			return dic.Count;
		}
	}
}
