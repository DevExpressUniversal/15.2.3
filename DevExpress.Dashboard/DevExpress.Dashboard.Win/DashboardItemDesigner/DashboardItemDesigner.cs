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
namespace DevExpress.DashboardWin.Native {
	public class DashboardItemDesigner : IDisposable {
		protected DashboardDesigner DashboardDesigner { get; private set; }
		protected DashboardItem DashboardItem { get; private set; }
		protected DashboardItemViewer ItemViewer { get; private set; }
		protected IServiceProvider ServiceProvider { get { return DashboardDesigner; } }
		public void Initialize(DashboardDesigner dashboardDesigner, DashboardItem dashboardItem, DashboardItemViewer itemViewer) {
			Guard.ArgumentNotNull(dashboardDesigner, "dashboardDesigner");
			Guard.ArgumentNotNull(dashboardItem, "dashboardItem");
			Guard.ArgumentNotNull(itemViewer, "itemViewer");
			DashboardDesigner = dashboardDesigner;
			DashboardItem = dashboardItem;
			ItemViewer = itemViewer;
			InitializeInternal();
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		protected virtual void InitializeInternal() {
		}
	}
}
