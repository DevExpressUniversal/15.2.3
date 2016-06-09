﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraDashboardLayout;
using System;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardLayoutSelectionService {
		IDashboardLayoutControlItem SelectedItem { get; set; }
		event EventHandler<DashboardLayoutItemSelectedEventArgs> ItemSelected;
		void Lock();
		void Unlock();
	}
	public class DashboardLayoutSelectionService : IDashboardLayoutSelectionService, IDisposable {
		readonly DashboardLayoutControl layoutControl;
		readonly Locker locker = new Locker();
		public IDashboardLayoutControlItem SelectedItem { 
			get { return layoutControl.SelectedItem as IDashboardLayoutControlItem; } 
			set {
				if (layoutControl.SelectedItem != value)
					layoutControl.SelectedItem = value != null ? value.Inner : null;
				else
					RaiseLayoutItemSelectionChanged();
			} 
		}
		public event EventHandler<DashboardLayoutItemSelectedEventArgs> ItemSelected;
		public DashboardLayoutSelectionService(DashboardLayoutControl layoutControl) {
			Guard.ArgumentNotNull(layoutControl, "layoutControl");
			this.layoutControl = layoutControl;
			SubscribeLayoutControlEvents();
		}
		public void Dispose() {
			UnsubscribeLayoutControlEvents();
		}
		public void Lock() {
			locker.Lock();
		}
		public void Unlock() {
			locker.Unlock();
		}
		void SubscribeLayoutControlEvents() {
			layoutControl.ItemSelectionChanged += OnLayoutItemSelectionChanged;
		}
		void UnsubscribeLayoutControlEvents() {
			layoutControl.ItemSelectionChanged -= OnLayoutItemSelectionChanged;
		}		
		void OnLayoutItemSelectionChanged(object sender, EventArgs e) {
			RaiseLayoutItemSelectionChanged();
		}
		void RaiseLayoutItemSelectionChanged() {
			if (!locker.IsLocked && ItemSelected != null)
				ItemSelected(this, new DashboardLayoutItemSelectedEventArgs(SelectedItem));
		}
	}
	public class DashboardLayoutItemSelectedEventArgs : EventArgs {
		public IDashboardLayoutControlItem SelectedItem { get; private set; }
		public DashboardLayoutItemSelectedEventArgs(IDashboardLayoutControlItem selectedItem) {
			SelectedItem = selectedItem;
		}
	}
}
