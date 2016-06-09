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
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncServerModeCurrentPlainDataView : AsyncServerModeCurrentDataView {
		new AsyncVisibleListWrapper VisibleList { get { return (AsyncVisibleListWrapper)base.VisibleList; } }
		AsyncListServerDefaultDataView DefaultView { get { return (AsyncListServerDefaultDataView)ListSource; } }
		public AsyncServerModeCurrentPlainDataView(AsyncListServerDefaultDataView view, object handle, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, 
			string filterCriteria, string displayFilterCriteria)
			: base(view, handle, valueMember, displayMember, groups, sorts, filterCriteria, displayFilterCriteria) {
		}
		protected override AsyncListWrapper CreateWrapper(AsyncListServerDefaultDataView view) {
			return view.Wrapper;
		}
		protected override void InitializeView(object source) {
			SetView(DefaultView.View);
		}
		public override bool ProcessChangeSortFilter(IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			base.ProcessChangeSortFilter(groups, sorts, filterCriteria, displayFilterCriteria);
			SetupGroupSortFilter(groups, sorts, filterCriteria);
			ApplySortFilterForVisibleListWrapper((AsyncVisibleListWrapper)GetVisibleListInternal());
			return true;
		}
		protected override DataAccessor CreateEditorsDataAccessor() {
			return DefaultView.DataAccessor;
		}
		protected override DataControllerItemsCache CreateItemsCache() {
			return DefaultView.ItemsCache;
		}
	}
}
