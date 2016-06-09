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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2.Win.Designers {
	public class ComponentDataSourceDesigner : ComponentDesigner {
		private IDesignerHost designerHost;
		private IComponentChangeService componentChangeService;
		private IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(DataSourceBase.IsDesignMode) {
				ITypesInfoProvider typesInfoProvider = DesignerHost.GetService(typeof(ITypesInfoProvider)) as ITypesInfoProvider;
				DesignerHost.RemoveService(typeof(ITypesInfo));
				DesignerHost.AddService(typeof(ITypesInfo), typesInfoProvider.CreatedTypesInfo(DesignerHost));
			}
			SubscribeEvents();
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(Component != null && IsComponentStorageItem(Component)) {
				Add(Component);
			}
		}
		protected IDesignerHost DesignerHost {
			get {
				if(designerHost == null) {
					designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				}
				return designerHost;
			}
		}
		protected virtual void Add(IComponent obj) {
			if(DesignerHost.RootComponent is XtraReport) {
				if(!((XtraReport)DesignerHost.RootComponent).ComponentStorage.Contains(obj)) {
					PropertyDescriptor modifiersDesignTimePD = TypeDescriptor.GetProperties(obj, new Attribute[] { new BrowsableAttribute(true) })["Modifiers"];
					if(modifiersDesignTimePD != null) {
						modifiersDesignTimePD.SetValue(obj, System.CodeDom.MemberAttributes.Family);
					}
					((XtraReport)DesignerHost.RootComponent).ComponentStorage.Add(obj);
				}
			}
		}
		protected virtual void Remove(IComponent obj) {
			if(DesignerHost.RootComponent is XtraReport) {
				((XtraReport)DesignerHost.RootComponent).ComponentStorage.Remove(obj);
			}
		}
		protected void SubscribeEvents() {
			UnsubscribeEvents();
			if(ComponentChangeService != null) {
				ComponentChangeService.ComponentRemoving += new ComponentEventHandler(ComponentChangeService_ComponentRemoving);
				ComponentChangeService.ComponentRemoved += new ComponentEventHandler(ComponentChangeService_ComponentRemoved);
			}
			DesignerHost.Deactivated += new EventHandler(DesignerHost_Deactivated);
			DesignerHost.Activated += new EventHandler(DesignerHost_Activated);
		}
		void DesignerHost_Activated(object sender, EventArgs e) {
			DesignerHost.Activated -= new EventHandler(DesignerHost_Activated);
			SubscribeEvents();
		}
		protected void UnsubscribeEvents() {
			DesignerHost.Deactivated -= new EventHandler(DesignerHost_Deactivated);
			if(ComponentChangeService != null) {
				ComponentChangeService.ComponentRemoved -= new ComponentEventHandler(ComponentChangeService_ComponentRemoved);
				ComponentChangeService.ComponentRemoving -= new ComponentEventHandler(ComponentChangeService_ComponentRemoving);
			}
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeEvents();
			base.Dispose(disposing);
		}
		private void DesignerHost_Deactivated(object sender, EventArgs e) {
			UnsubscribeEvents();
		}
		private void ComponentChangeService_ComponentRemoving(object sender, ComponentEventArgs e) {
			Remove(e);
		}
		private void ComponentChangeService_ComponentRemoved(object sender, ComponentEventArgs e) {
			Remove(e);
		}
		private void Remove(ComponentEventArgs e) {
			if(IsComponentStorageItem(e.Component)) {
				Remove(e.Component);
			}
		}
		private bool IsComponentStorageItem(IComponent item) {
			object[] result = item.GetType().GetCustomAttributes(typeof(ComponentStorageItemAttribute), true);
			return result.Length > 0;
		}
	}
}
