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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.Persistent.Base {
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReportInplaceActionsHandler {
		void SuspendEvents();
		void ProcessActionItem(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs);
		List<ReportInplaceActionInfo> GetReportInplaceActionInfo(Type objectType);
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ReportInplaceActionInfo {
		public string ActionName { get;  private set; }
		public object ActionData { get; private set; }
		public ReportInplaceActionInfo(string actionName, object actionData) {
			ActionName = actionName;
			ActionData = actionData;
		}
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReportNavigationItemsHandler {
		void SuspendEvents();
		void ProcessNavigationItem(object navigationData);
		List<ReportNavigationItemInfo> GetReportNavigationItemsInfo(Type targetType);
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ReportNavigationItemInfo {
		public string ItemKey { get; private set; }
		public string ItemName { get; private set; }
		public object ItemData { get; private set; }
		public ReportNavigationItemInfo(string navigationItemKey, string navigationItemName, object navigationItemData) {
			ItemKey = navigationItemKey;
			ItemName = navigationItemName;
			ItemData = navigationItemData;
		}
	}
}
