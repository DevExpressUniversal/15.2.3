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
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Kpi {
	public class ForceRefreshKpiController : RefreshKpiController {
		private const String isModifiedKey = "Is modified";
		private IEnumerable<IKpiInstance> GetSelectedObjects() {
			List<IKpiInstance> indicators = new List<IKpiInstance>();
			if(View is ISelectionContext) {
				foreach(object item in ((ISelectionContext)View).SelectedObjects) {
					IKpiInstance kpiObject = item as IKpiInstance;
					if(kpiObject != null) {
						indicators.Add(kpiObject);
					}
				}
			}
			else {
				IKpiInstance kpiObject = View.CurrentObject as IKpiInstance;
				if(kpiObject != null) {
					indicators.Add(kpiObject);
				}
			}
			return indicators.ToArray();
		}
		private void RefreshInstances(IEnumerable<IKpiInstance> indicators, bool clearCache) {
			foreach(IKpiInstance indicator in indicators) {
				if(clearCache) {
					ClearKpiObjectCache(indicator);
				}
				else {
					RefreshKpiObject(indicator, true);
				}
			}
			ObjectSpace.Refresh();
		}
		private void RefreshAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			RefreshInstances(GetSelectedObjects(), false);
		}
		private void ClearCacheAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			RefreshInstances(GetSelectedObjects(), true);
		}
		private void ObjectSpace_ModifiedChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		protected virtual void UpdateActionState() {
			RefreshAction.Enabled[isModifiedKey] = !ObjectSpace.IsModified;
			ClearCacheAction.Enabled[isModifiedKey] = !ObjectSpace.IsModified;
		}
		protected override void OnActivated() {
			base.OnActivated();
			ClearCacheAction.Active["IKpiHistoryCacheManager"] = typeof(IKpiHistoryCacheManager).IsAssignableFrom(View.ObjectTypeInfo.Type);
			ObjectSpace.ModifiedChanged += ObjectSpace_ModifiedChanged;
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ObjectSpace.ModifiedChanged -= ObjectSpace_ModifiedChanged;
		}
		public ForceRefreshKpiController()
			: base() {
			RefreshAction = new SimpleAction(this, "RefreshKpi", PredefinedCategory.Edit);
			RefreshAction.Caption = "Refresh KPI";
			RefreshAction.ImageName = "Action_Refresh_KPI";
			RefreshAction.Execute += new SimpleActionExecuteEventHandler(RefreshAction_Execute);
			ClearCacheAction = new SimpleAction(this, "ClearKpiCache", PredefinedCategory.Edit);
			ClearCacheAction.Caption = "Clear KPI Cache";
			ClearCacheAction.ImageName = "Action_Clear_KPI_Cache";
			ClearCacheAction.Execute += new SimpleActionExecuteEventHandler(ClearCacheAction_Execute);
		}
		public SimpleAction RefreshAction { get; private set; }
		public SimpleAction ClearCacheAction { get; private set; }
	}
}
