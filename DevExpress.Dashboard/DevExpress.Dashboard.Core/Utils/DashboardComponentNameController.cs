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
using System.Collections.Generic;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardComponentNameController<T> : ComponentNameControllerBase<T> where T : class, IDashboardComponent {
		public DashboardComponentNameController(NotifyingCollection<T> collection, IObjectNameGenerator context)
			: base(collection, context) {
		}
		protected override string GetComponentName(T item) {
			return item.ComponentName;
		}
		protected override void SetComponentName(T item, string name) {
			item.ComponentName = name;
		}
		protected override void SubscribeItemEvents(T item) {
			item.NameChanging += OnItemNameChanging;
		}
		protected override void UnsubscribeItemEvents(T item) {
			item.NameChanging -= OnItemNameChanging;
		}
		protected override void CheckComponentName(string name) {
			Context.CheckName(name);
		}
		void OnItemNameChanging(object sender, NameChangingEventArgs e) {
			CheckComponentName(e.NewName);
		}
	}
}
