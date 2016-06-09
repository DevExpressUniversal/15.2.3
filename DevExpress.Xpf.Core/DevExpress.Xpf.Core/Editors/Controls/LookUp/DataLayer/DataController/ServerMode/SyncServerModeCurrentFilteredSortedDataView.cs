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
	public class SyncServerModeCurrentFilteredSortedDataView : SyncServerModeCurrentDataView {
		public SyncServerModeCurrentFilteredSortedDataView(SyncListServerDefaultDataView view, object handle, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria)
			: base(view, handle, valueMember, displayMember, groups, sorts, filterCriteria, displayFilterCriteria) {
			Wrapper.Initialize(this);
		}
		protected override void InitializeView(object source) {
			SetView(new SyncDataProxyViewCache(this));
			InitializeSource();
		}
		protected override SyncListWrapper CreateWrapper(IListServerDataView view) {
			return new SyncListWrapper(view.Wrapper.Server);
		}
		protected override DataControllerItemsCache CreateItemsCache() {
			return new DataControllerItemsCache(this);
		}
		public override void InitializeSource() {
			ApplySortFilterForWrapper(Wrapper);
			base.InitializeSource();
		}
		public override bool ProcessInconsistencyDetected() {
			bool result = base.ProcessInconsistencyDetected();
			ProcessInconsistencyDetectedForVisibleListWrapper((SyncVisibleListWrapper)GetVisibleListInternal());
			return result;
		}
		public override bool ProcessRefresh() {
			base.ProcessRefresh();
			Wrapper.Refresh();
			ProcessRefreshForVisibleListWrapper((SyncVisibleListWrapper)GetVisibleListInternal());
			return true;
		}
	}
}
