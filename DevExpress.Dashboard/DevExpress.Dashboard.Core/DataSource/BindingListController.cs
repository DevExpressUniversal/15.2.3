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
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public class BindingListController {
		readonly IDashboardDataSource dataSource;
		public event EventHandler<DataSourceChangedEventArgs> CustomDataChanged;
		public BindingListController(IDashboardDataSource dataSource) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			this.dataSource = dataSource;
		}
		public void UnsubscribeDataEvents(object dataSource) {
			IBindingList bindingList = dataSource as IBindingList;
			if(bindingList != null)
				bindingList.ListChanged -= BindingListListChanged;
		}
		public void SubscribeDataEvents(object dataSource) {
			IBindingList bindingList = dataSource as IBindingList;
			if(bindingList != null)
				bindingList.ListChanged += BindingListListChanged;
		}
		public void RaiseCustomUserDataChanged() {
			if(CustomDataChanged != null)
				CustomDataChanged(this, new DataSourceChangedEventArgs(dataSource));
		}
		void BindingListListChanged(object sender, ListChangedEventArgs e) {
			ListChangedType listChangedType = e.ListChangedType;
			bool isSchemaChanged = listChangedType == ListChangedType.PropertyDescriptorAdded ||
				listChangedType == ListChangedType.PropertyDescriptorDeleted || listChangedType == ListChangedType.PropertyDescriptorChanged;
			RaiseCustomUserDataChanged();
		}
	}
}
