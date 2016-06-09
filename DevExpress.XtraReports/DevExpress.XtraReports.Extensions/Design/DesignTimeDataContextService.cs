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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Data;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	public class DesignTimeDataContextService : XRDataContextServiceBase {
		IDesignerHost host;
		protected override bool SuppressListFilling {
			get {
				return true;
			}
		}
		protected override PropertyDescriptor[] FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext) {
			if(host == null || GetShowInnerLists(dataContext))
				return properties;
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach(var property in properties) {
				if(IsListType(property))
					continue;
				result.Add(property);
			}
			return result.ToArray();
		}
		protected override DataContext CreateDataContextInternal(DataContextOptions options) {
			return new XRDataContextBase(options.UseCalculatedFields ? CalculatedFields : null, SuppressListFilling);
		}
		static bool IsListType(PropertyDescriptor property) {
			return property.PropertyType != typeof(byte[]) && ListTypeHelper.IsListType(property.PropertyType);
		}
		bool GetShowInnerLists(DataContext dataContext) {
			if(dictionary.ContainsValue(dataContext)) {
				foreach(var pair in dictionary) {
					if(pair.Value == dataContext)
						return pair.Key.ShowInnerLists;
				}
			}
			return true;
		}
		public DesignTimeDataContextService(IDesignerHost host)
			: base(host.RootComponent as XtraReport) {
			this.host = host;
			SubscibeEvents();
		}
		void OnLoadComplete(object sender, System.EventArgs e) {
			((IDesignerHost)sender).LoadComplete -= new EventHandler(this.OnLoadComplete);
			DisposeDataContext();
		}
		void changeService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			DisposeDataContext();
		}
		void SubscibeEvents() {
			host.LoadComplete += new EventHandler(OnLoadComplete);
			IComponentChangeService changeService = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService == null)
				return;
			changeService.ComponentChanged += new ComponentChangedEventHandler(changeService_ComponentChanged);
		}
		void UnsubscibeEvents() {
			IComponentChangeService changeService = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService == null)
				return;
			changeService.ComponentChanged -= new ComponentChangedEventHandler(changeService_ComponentChanged);
		}
		#region IDisposable Members
		public override void Dispose() {
			UnsubscibeEvents();
			base.Dispose();
		}
		#endregion
	}
}
