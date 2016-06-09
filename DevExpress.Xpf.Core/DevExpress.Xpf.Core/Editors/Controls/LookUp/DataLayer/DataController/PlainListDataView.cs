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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class PlainListDataView : DefaultDataView {
		readonly string filterCriteria;
		readonly IEnumerable<GroupingInfo> groups;
		readonly IEnumerable<SortingInfo> sorts;
		new IEnumerable<DataProxy> View { get { return base.View as LocalDataProxyViewCache; } }
		public PlainListDataView(IEnumerable serverSource, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filterCriteria)
			: base(serverSource, valueMember, displayMember) {
			this.filterCriteria = filterCriteria;
			this.groups = groups;
			this.sorts = sorts;
		}
		protected override void FetchDescriptorsInternal(DataAccessor accessor) {
			base.FetchDescriptorsInternal(accessor);
			foreach (var group in groups)
				accessor.Fetch(@group.FieldName);
			foreach (var sort in sorts)
				accessor.Fetch(sort.FieldName);
			var filter = CriteriaOperator.Parse(filterCriteria);
			if (!object.ReferenceEquals(filter, null)) {
				var visitor = new Visitor();
				filter.Accept(visitor);
				foreach (var property in visitor.RequestedProperties)
					accessor.Fetch(property);
			}
		}
		public override IEnumerator<DataProxy> GetEnumerator() {
			return View.GetEnumerator();
		}
	}
}
