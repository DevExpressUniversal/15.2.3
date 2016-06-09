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
using DevExpress.DashboardCommon;
using DevExpress.Utils;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public abstract class DashboardHistoryItem : HistoryItem {
		readonly Dashboard dashboard;
		protected Dashboard Dashboard { get { return dashboard; } }
		protected virtual string PropertyName { get { return null; } }
		protected DashboardHistoryItem(Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			this.dashboard = dashboard;
		}
		public override void Redo() {
			IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
			DesignerTransaction transaction = designerHost != null ? designerHost.CreateTransaction(Caption) : null;
			try {
				IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
				PropertyDescriptor property = null;
				object oldValue = null;
				object newValue = null;
				if(changeService != null) {
					if(!string.IsNullOrEmpty(PropertyName))
						property = Helper.GetProperty(dashboard, PropertyName);
					changeService.OnComponentChanging(dashboard, property);
					if(property != null)
						oldValue = property.GetValue(dashboard);
				}
				PerformRedo();
				if(changeService != null) {
					if(property != null)
						newValue = property.GetValue(dashboard);
					changeService.OnComponentChanged(dashboard, property, oldValue, newValue);
				}
			}
			finally {
				if(transaction != null)
					transaction.Commit();
			}
		}
	}
}
