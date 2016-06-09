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

using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Browsing;
using DevExpress.Data;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using System;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class WpfNativeDataSource : PivotGridNativeDataSource {
		public WpfNativeDataSource(PivotGridWpfData data) 
			: base(data) { 
		}
		protected override PivotDataController CreateDataController() {
			return new WpfDataController();
		}
		protected override void RetrieveField(DataColumnInfo columnInfo, XtraPivotGrid.PivotArea area, bool visible) {
			string name, caption;
			if(ApplyColumnAttributes(columnInfo, out name, out caption))
				CreateField(area, name, caption, visible);
		}
		bool ApplyColumnAttributes(DataColumnInfo columnInfo, out string name, out string caption) {
			DataColumnAttributes attributes = DataColumnAttributesProvider.GetAttributes(columnInfo.PropertyDescriptor);
			bool? autoGenerateField = attributes.AutoGenerateField;
			bool visible = !autoGenerateField.HasValue || autoGenerateField.HasValue && autoGenerateField.Value;
			name = columnInfo.Name;
			caption = columnInfo.Caption;
			return visible;
		}
	}
	public class WpfDataController : PivotDataController, IWeakEventListener {
		protected override void SubscribeListChanged(Data.Helpers.INotificationProvider provider, object list) {
			IBindingList blist = list as IBindingList;
			if(blist == null)
				return;
			ListChangedEventManager.AddListener(blist, this);
		}
		protected override void UnsubscribeListChanged(Data.Helpers.INotificationProvider provider, object list) {
			IBindingList blist = list as IBindingList;
			if(blist == null)
				return;
			ListChangedEventManager.RemoveListener(blist, this);
		}
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(ListChangedEventManager)) {
				OnBindingListChanged(sender, (ListChangedEventArgs)e);
				return true;
			}
			return false;
		}
		#endregion
	}
}
