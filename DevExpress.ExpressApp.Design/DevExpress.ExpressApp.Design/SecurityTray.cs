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
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using System.ComponentModel.Design;
using System.Drawing.Design;
namespace DevExpress.ExpressApp.Design {
	public class SecurityTray : ListViewTray<XafApplication>{
		private void Application_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "Security") {
				RefreshItems();
			}
		}
		private void InitAuthentication(AuthenticationBase newAuthentication) {
			SecurityStrategyBase securityBase = DataSource.Security as SecurityStrategyBase;
			if(securityBase != null) {
				AuthenticationBase oldAuthentication = securityBase.Authentication;
				securityBase.Authentication = newAuthentication;
				designer.ComponentChangeService.OnComponentChanged(securityBase, TypeDescriptor.GetProperties(securityBase)["Authentication"], oldAuthentication, newAuthentication);
				if(oldAuthentication != null && oldAuthentication is IComponent) {
					designer.DesignerHost.DestroyComponent((IComponent)oldAuthentication);
				}
			}
			else {
				throw new InvalidOperationException("Security is not inherited from " + typeof(SecurityStrategyBase).FullName);
			}
		}
		private void InitSecurity(ISecurityStrategyBase newSecurity) {
			ISecurityStrategyBase oldSecurity = DataSource.Security;
			DataSource.Security = newSecurity;
			designer.ComponentChangeService.OnComponentChanged(DataSource, TypeDescriptor.GetProperties(DataSource)["Security"], oldSecurity, DataSource.Security);
			SecurityStrategyBase oldSecurityBase = oldSecurity as SecurityStrategyBase;
			if(oldSecurityBase != null) {
				if(oldSecurityBase.Authentication != null) {
					SecurityStrategyBase securityBase = DataSource.Security as SecurityStrategyBase;
					if(securityBase != null) {
						AuthenticationBase oldAuthentication = securityBase.Authentication;
						securityBase.Authentication = oldSecurityBase.Authentication;
						designer.ComponentChangeService.OnComponentChanged(securityBase, TypeDescriptor.GetProperties(securityBase)["Authentication"], oldAuthentication, securityBase.Authentication);
						designer.ComponentChangeService.OnComponentChanged(securityBase.Authentication, null, null, null);
						oldSecurityBase.Authentication = null;
					}
				}
			}
			if(oldSecurity != null) {
				designer.DesignerHost.DestroyComponent((IComponent)oldSecurity);
			}
		}
		private void ComponentChangeService_ComponentRemoved(object sender, ComponentEventArgs e) {
			if(typeof(AuthenticationBase).IsAssignableFrom(e.Component.GetType())) {
				RefreshItems();
			}
		}
		protected override bool CanProcessToolboxItem(ToolboxItem item) {
			Type itemType = designer.GetToolboxItemType(item);
			if(typeof(ISecurityStrategyBase).IsAssignableFrom(itemType)) {
				return true;
			}
			else if(typeof(AuthenticationBase).IsAssignableFrom(itemType)) {
				SecurityStrategyBase owner = DataSource.Security as SecurityStrategyBase;
				if(owner != null) {
					return true;
				}
			}
			return false;
		}
		protected override void ComponentsCreated(IComponent[] createdComponents) {
			base.ComponentsCreated(createdComponents);
			if(createdComponents[0] is ISecurityStrategyBase) {
				InitSecurity((ISecurityStrategyBase)createdComponents[0]);
			}
			else if(createdComponents[0] is AuthenticationBase) {
				InitAuthentication((AuthenticationBase)createdComponents[0]);
			}
			RefreshItems();
		}
		protected override bool AllowAddItem(ToolboxItem item) {
			if(base.AllowAddItem(item)) {
				Type itemType = designer.GetToolboxItemType(item);
				if(typeof(ISecurityStrategyBase).IsAssignableFrom(itemType)) {
					return true;
				}
				else if(typeof(AuthenticationBase).IsAssignableFrom(itemType)) {
					return true;
				}
			}
			return false;
		}
		protected override void OnDataSourceChanged(XafApplication oldDataSource, XafApplication newDataSource) {
			if(oldDataSource != null) {
				oldDataSource.PropertyChanged -= new PropertyChangedEventHandler(Application_PropertyChanged);
			}
			if(newDataSource != null) {
				newDataSource.PropertyChanged += new PropertyChangedEventHandler(Application_PropertyChanged);
			}
		}
		protected override void OnSetDesigner(XafRootDesignerBase designer) {
			designer.ComponentChangeService.ComponentRemoved += new ComponentEventHandler(ComponentChangeService_ComponentRemoved);
		}
		protected override void RefreshItemsCore() {
			this.Items.Clear();
			LargeImageList.Images.Clear();
			ISecurityStrategyBase security = DataSource.Security;
			if(security != null) {
				AddListViewItem(security, security.GetType().Name);
				SecurityStrategyBase securityBase = security as SecurityStrategyBase;
				if(securityBase != null && securityBase.Authentication != null) {
					AddListViewItem(securityBase.Authentication, securityBase.Authentication.GetType().Name);
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(designer != null && designer.ComponentChangeService != null) {
					designer.ComponentChangeService.ComponentRemoved -= new ComponentEventHandler(ComponentChangeService_ComponentRemoved);
				}
			}
			base.Dispose(disposing);
		}
		public SecurityTray() : base()	{
			View = System.Windows.Forms.View.Tile;
			ToolTipMessage = "To add Security and Authentication strategies, drag them from the Toolbox, and use the Properties window to set their properties.";
			canShowPlaceholder = true;
			canShowToolTip = true;
			SetTooltip();
		}
	}
}
