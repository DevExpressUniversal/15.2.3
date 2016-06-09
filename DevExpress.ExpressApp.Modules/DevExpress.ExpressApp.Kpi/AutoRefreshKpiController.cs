#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Collections;
namespace DevExpress.ExpressApp.Kpi {
	public class AutoRefreshKpiController : RefreshKpiController {
		private Object modifiedObject;
		private void RefreshDetailView() {
			IKpiInstance kpiInstance = ((DetailView)View).CurrentObject as IKpiInstance;
			if(kpiInstance != null) {
				RefreshKpiObject(kpiInstance, false);
				View.ObjectSpace.ReloadObject(kpiInstance);
			}
		}
		private void RefreshListView() {
			if(typeof(PropertyCollectionSource).IsAssignableFrom(((ListView)View).CollectionSource.GetType())) {
				IEnumerable collection = ((ListView)View).CollectionSource.Collection as IEnumerable;
				if(collection != null) {
					bool needReload = false;
					object master = ((PropertyCollectionSource)(((ListView)View).CollectionSource)).MasterObject;
					if(master != null && !View.ObjectSpace.IsNewObject(master)) {
						needReload = true;
					}
					foreach(object item in collection) {
						RefreshKpiObject((IKpiInstance)item, false);
						if(needReload) {
							View.ObjectSpace.ReloadObject(item);
						}
					}
				}
			}
		}
		private void CollectionSource_CollectionChanged(object sender, EventArgs e) {
			RefreshListView();
		}
		private void AutoRefreshKpiController_CurrentObjectChanged(object sender, EventArgs e) {
			RefreshDetailView();
		}
		private void ObjectSpace_ObjectChanged(Object sender, ObjectChangedEventArgs e) {
			modifiedObject = e.Object;
		}
		private void ObjectSpace_ModifiedChanged(Object sender, EventArgs e) {
			if((View != null) && ((IObjectSpace)sender).IsModified && (modifiedObject is IKpiInstance)) {
				modifiedObject = null;
				((IObjectSpace)sender).CommitChanges();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			modifiedObject = null;
			if(View is ListView) {
				RefreshListView();
				((ListView)View).CollectionSource.CollectionChanged += new EventHandler(CollectionSource_CollectionChanged);
			}
			if(View is DetailView) {
				RefreshDetailView();
				((DetailView)View).CurrentObjectChanged += new EventHandler(AutoRefreshKpiController_CurrentObjectChanged);
			}
			View.ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
		}
		protected override void OnDeactivated() {
			View.ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			if(View is DetailView) {
				((DetailView)View).CurrentObjectChanged -= new EventHandler(AutoRefreshKpiController_CurrentObjectChanged);
			}
			if(View is ListView) {
				((ListView)View).CollectionSource.CollectionChanged -= new EventHandler(CollectionSource_CollectionChanged);
			}
			base.OnDeactivated();
		}
	}
}
