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
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Design.Core {
	public abstract class ListManagerComponentDesigner<T> : ComponentDesigner {
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
		private IDesignerHost DesignerHost {
			get {
				if(designerHost == null) {
					designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				}
				return designerHost;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ComponentChangeService.ComponentAdded += new ComponentEventHandler(service_ComponentAdded);
			ComponentChangeService.ComponentRemoved += new ComponentEventHandler(service_ComponentRemoved);
			DesignerHost.Deactivated += new EventHandler(DesignerHost_Deactivated);
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(Component != null && ((Component)Component).Container != null) {
				foreach(object childComponent in ((Component)Component).Container.Components) {
					TryAdd(childComponent);
				}
			}
		}
		private void DesignerHost_Deactivated(object sender, EventArgs e) {
			DesignerHost.Deactivated -= new EventHandler(DesignerHost_Deactivated);
			ComponentChangeService.ComponentAdded -= new ComponentEventHandler(service_ComponentAdded);
			ComponentChangeService.ComponentRemoved -= new ComponentEventHandler(service_ComponentRemoved);
		}
		private void service_ComponentRemoved(object sender, ComponentEventArgs e) {
			object obj = e.Component;
			if(Component != null && obj is T) {
				Remove((T)obj);
			}
		}
		private void service_ComponentAdded(object sender, ComponentEventArgs e) {
			if(!DesignerHost.Loading) {
				TryAdd(e.Component);
			}
		}
		private void TryAdd(object obj) {
			if(Component != null && obj is T && obj != Component) {
				Add((T)obj);
			}
		}
		protected abstract void Add(T obj);
		protected abstract void Remove(T obj);
	}
}
