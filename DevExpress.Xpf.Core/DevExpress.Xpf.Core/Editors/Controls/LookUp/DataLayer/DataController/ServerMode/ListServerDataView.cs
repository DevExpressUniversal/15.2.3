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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class ListServerDataView : ListServerDataViewBase {
		readonly IListServer server;
		string filterCriteria;
		readonly IEnumerable<SortingInfo> actualSorting;
		readonly int groupCount;
		public ListServerDataView(IListServer server, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filter)
			: base(server, valueMember, displayMember) {
			this.server = server;
			this.filterCriteria = filter;
			List<SortingInfo> resultSorting = groups != null ? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList() : new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			actualSorting = resultSorting;
			groupCount = groups.Count();
			server.InconsistencyDetected += list_InconsistencyDetected;
			server.ExceptionThrown += list_ExceptionThrown;
		}
		protected override void InitializeView(object source) {
			InitializeSource();
		}
		protected virtual void InitializeSource() {
			server.Apply(CriteriaOperator.Parse(filterCriteria),
				actualSorting.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount, null, null);
		}
		public override bool ProcessChangeFilter(string filterCriteria) {
			this.filterCriteria = filterCriteria;
			InitializeSource();
			return true;
		}
		void list_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
		}
		void list_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			server.InconsistencyDetected -= list_InconsistencyDetected;
			server.ExceptionThrown -= list_ExceptionThrown;
		}
		protected override int FindListSourceIndexByValue(object value) {
			return server.LocateByValue(GetCriteriaForValueColumn(), value, -1, false);
		}
	}
}
