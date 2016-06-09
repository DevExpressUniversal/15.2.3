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
using System.ComponentModel;
using System.Linq;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class DefaultDataView : PlainDataView {
		public int ListSourceCount {
			get { return ListSource.Count; }
		}
		new IList ListSource {
			get { return base.ListSource as IList; }
		}
		protected DefaultDataView(object serverSource, string valueMember, string displayMember)
			: base(serverSource, valueMember, displayMember) {
		}
		public void ProcessListChanged(ListChangedEventArgs e) {
			ProcessChangeSource(e);
			RaiseListChanged(e);
		}
		public IEnumerable<string> AvailableColumns {
			get { return DataAccessor.Descriptors.Select(x => x.Name); }
		}
		protected override void InitializeView(object source) {
			var list = (IList)source;
			SetView(new LocalDataProxyViewCache(DataAccessor, list.Cast<object>().Select(x => DataAccessor.CreateProxy(x, -1))));
		}
		public virtual CurrentDataView CreateCurrentDataView(object handle, IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			bool createFiltered = GetIsCurrentViewFIltered(groups, sorts, filterCriteria) || !string.IsNullOrEmpty(displayFilterCriteria);
			return createFiltered
				? (CurrentDataView)new LocalCurrentFilteredSortedDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember, groups, sorts, filterCriteria, displayFilterCriteria)
				: new LocalCurrentPlainDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember);
		}
		public virtual bool ProcessChangeFilter(string toString) {
			return true;
		}
		public object GetItemByIndex(int index) {
			return GetItemByProxy(View[index]);
		}
		protected override DataAccessor CreateEditorsDataAccessorInstance() {
			return new DataAccessor();
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			UnsubscribeFromEvents();
		}
		public virtual void ProcessFindIncremental(ItemsProviderFindIncrementalCompletedEventArgs e) {
		}
		public virtual bool ProcessInconsistencyDetected() {
			return true;
		}
	}
}
