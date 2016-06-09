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
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataControllerSnapshotDescriptor {
		public event EventHandler Refreshed;
		public object Handle { get; private set; }
		public ObservableCollection<GroupingInfo> Groups { get; private set; }
		public ObservableCollection<SortingInfo> Sorts { get; private set; }
		public string FilterCriteria { get; private set; }
		public string DisplayFilterCriteria { get; private set; }
		public DataControllerSnapshotDescriptor(object handle) {
			Handle = handle;
			Groups = new ObservableCollection<GroupingInfo>();
			Sorts = new ObservableCollection<SortingInfo>();
			Groups.CollectionChanged += (sender, args) => RaiseRefreshed();
			Sorts.CollectionChanged += (sender, args) => RaiseRefreshed();
		}
		public void SetFilterCriteria(CriteriaOperator criteria) {
			string filterCriteria = criteria.With(x => x.ToString());
			if (object.Equals(FilterCriteria, filterCriteria))
				return;
			FilterCriteria = filterCriteria;
			RaiseRefreshed();
		}
		public void SetDisplayFilterCriteria(CriteriaOperator criteria) {
			string filterCriteria = criteria.With(x => x.ToString());
			if (object.Equals(DisplayFilterCriteria, filterCriteria))
				return;
			DisplayFilterCriteria = filterCriteria;
			RaiseRefreshed();
		}
		void RaiseRefreshed() {
			Refreshed.Do(x => x(this, EventArgs.Empty));
		}
	}
}
