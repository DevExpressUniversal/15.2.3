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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardWin.ServiceModel {
	public abstract class DashboardDesignerSelectionBehavior : IDefaultSelectionService {
		readonly IServiceProvider serviceProvider;
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected IDashboardOwnerService OwnerService { get { return serviceProvider.RequestServiceStrictly<IDashboardOwnerService>(); } }
		protected IDashboardDesignerSelectionService SelectionService { get { return serviceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>(); } }
		protected DashboardDesignerSelectionBehavior(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.serviceProvider = serviceProvider;
		}
		public virtual void SetDefaultSelection() {
			SelectionService.ForceDashboardItemSelection(OwnerService.GetFirstDashboardItemOrGroup());
		}
		protected void SelectDashboardItemOnUpdate(IList<string> addedItemNames) {
			if (addedItemNames.Count > 0)
				SelectionService.ForceDashboardItemSelection(OwnerService.FindDashboardItemOrGroup(addedItemNames.Last()));
			else if (SelectionService.SelectedDashboardItem == null)
				SelectionService.ForceDashboardItemSelection(OwnerService.GetFirstDashboardItemOrGroup());
		}
	}
}
