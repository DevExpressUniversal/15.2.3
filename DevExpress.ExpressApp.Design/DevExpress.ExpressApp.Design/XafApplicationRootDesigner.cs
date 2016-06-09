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
using System.ComponentModel.Design;
using System.Data;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils.About;
namespace DevExpress.ExpressApp.Design {
	[ToolboxItemFilter("Xaf", ToolboxItemFilterType.Custom)]
	[ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Prevent)]
	[ToolboxItemFilter("System.Web.UI", ToolboxItemFilterType.Prevent)]
	[ToolboxItemFilter("", ToolboxItemFilterType.Custom)]
	public class XafApplicationRootDesigner : XafRootDesignerBase, IEditorNavigation {
		private void OnMenuDelete(object sender, EventArgs e) {
			IDesignerHost host = DesignerHost;
			ISelectionService selectionService = SelectionService;
			if(host != null && selectionService != null && selectionService.SelectionCount > 0) {
				using(DesignerTransaction trans = host.CreateTransaction("Delete " + selectionService.SelectionCount + " component(s)")) {
					foreach(object obj in selectionService.GetSelectedComponents()) {
						if(obj is ModuleBase) {
							Application.Modules.Remove((ModuleBase)obj);
						}
						if(obj != Application) {
							host.DestroyComponent((IComponent)obj);
						}
					}
					trans.Commit();
				}
			}
		}
		private void ComponentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			const string messageTemplate = "The UserType property of the {0} object must be set to valid user type.";
			if(e.Component is XafApplication && e.Member != null && e.Member.Name == "Security") {
				ISecurityStrategyBase security = (ISecurityStrategyBase)e.NewValue;
				if(security != null && security.UserType == null) {
					AddWarning(string.Format(messageTemplate, e.NewValue.GetType()));
				}
				else {
					if(e.OldValue != null) {
						RemoveWarning(string.Format(messageTemplate, e.OldValue.GetType()));
					}
				}
			} else if(e.Component is ISecurityStrategyBase && e.Member != null && e.Member.Name == "UserType") {
				if(e.NewValue == null) {
					AddWarning(string.Format(messageTemplate, e.Component.GetType()));
				}
				else {
					RemoveWarning(string.Format(messageTemplate, e.Component.GetType()));
				}
			}
		}
		private void ComponentChangeService_ComponentRemoving(object sender, ComponentEventArgs e) {
			if(e.Component is ISecurityStrategyBase) {
				if(e.Component is SecurityStrategyBase) {
					AuthenticationBase authentication = ((SecurityStrategyBase)e.Component).Authentication;
					if(authentication != null && !SelectionService.GetComponentSelected(authentication)) {
						DesignerHost.DestroyComponent((IComponent)authentication);
					}
				}
				PropertyDescriptor securityProperty = TypeDescriptor.GetProperties(Application)["Security"];
				if(securityProperty != null && securityProperty.GetValue(Application) == e.Component) {
					securityProperty.SetValue(Application, null);
				}
			}
			else if(e.Component is AuthenticationBase) {
				SecurityStrategyBase appSecurity = Application.Security as SecurityStrategyBase;
				if(appSecurity != null) {
					PropertyDescriptor authenticationProperty = TypeDescriptor.GetProperties(appSecurity)["Authentication"];
					if(authenticationProperty != null && authenticationProperty.GetValue(appSecurity) == e.Component) {
						authenticationProperty.SetValue(appSecurity, null);
					}
				}
			}
			else if(e.Component is IDbConnection) {
				PropertyDescriptor connectionProperty = TypeDescriptor.GetProperties(Application)["Connection"];
				if(connectionProperty != null && e.Component == connectionProperty.GetValue(Application)) {
					connectionProperty.SetValue(Application, null);
				}
			}
			else if(e.Component is ModuleBase) {
				PropertyDescriptor modulesProperty = TypeDescriptor.GetProperties(Application)["Modules"];
				ComponentChangeService.OnComponentChanging(Application, modulesProperty);
			}
		}
		private void ComponentChangeService_ComponentRemoved(object sender, ComponentEventArgs e) {
			if(e.Component is ModuleBase) {
				PropertyDescriptor modulesProperty = TypeDescriptor.GetProperties(Application)["Modules"];
				ComponentChangeService.OnComponentChanged(Application, modulesProperty, null, null);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ComponentChangeService.ComponentRemoved -= new ComponentEventHandler(ComponentChangeService_ComponentRemoved);
				ComponentChangeService.ComponentRemoving -= new ComponentEventHandler(ComponentChangeService_ComponentRemoving);
				ComponentChangeService.ComponentChanged -= new ComponentChangedEventHandler(ComponentChangeService_ComponentChanged);
			}
			base.Dispose(disposing);
		}
		protected override Control CreateView() {
			return new XafApplicationRootDesignerGUI();
		}
		protected override void InitializeView() {
			((XafApplicationRootDesignerGUI)view).Initialize(this);
		}
		protected override void LogDesignerOpened() {
			base.LogDesignerOpened();
			if(Application != null) {
				string log = XpandModuleLogger.CreateXpandLogRecord(Application);
				if(log != null) {
				}
			}
		}
		protected override bool IsToolSupported(Type toolType) {
			if(typeof(SecurityStrategyBase).IsAssignableFrom(toolType)) {
				return true;
			} else if(typeof(AuthenticationBase).IsAssignableFrom(toolType)) {
				return true;
			} else if(typeof(IDbConnection).IsAssignableFrom(toolType)) {
				return true;
			}
			return base.IsToolSupported(toolType);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			try {
				if(XafRootDesignerBase.Enable_PopupMenu) {
					AddMenuCommand(new MenuCommand(new EventHandler(OnMenuDelete), StandardCommands.Delete));
				}
				ComponentChangeService.ComponentRemoved += new ComponentEventHandler(ComponentChangeService_ComponentRemoved);
				ComponentChangeService.ComponentRemoving += new ComponentEventHandler(ComponentChangeService_ComponentRemoving);
				ComponentChangeService.ComponentChanged += new ComponentChangedEventHandler(ComponentChangeService_ComponentChanged);
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
		}
		public override void AddModule(ToolboxItem item) {
			Type itemType = GetToolboxItemType(item);
			if(itemType == null) {
				return;
			}
			if(Application.Modules.FindModule(itemType) != null) {
				DialogResult dialogResult = MessageBox.Show(string.Format("Module {0} is already added.\nDo you want to add an additional instance of this module any way?", itemType), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if(dialogResult != DialogResult.Yes) {
					return;
				}
			}
			base.AddModule(item);
			if(DesignerHost != null) {
				if(XafRootDesignerBase.Enable_DTE) {
					WebDesignerHelper helper = new WebDesignerHelper(this);
					helper.AddAssemblyInWebConfig(itemType);
				}
				using(DesignerTransaction trans = DesignerHost.CreateTransaction("Creating " + item.DisplayName)) {
					IComponent[] newComponents = item.CreateComponents(DesignerHost);
					IComponentChangeService cs = ComponentChangeService;
					PropertyDescriptor ownerProperty = TypeDescriptor.GetProperties(Application)["Modules"];
					cs.OnComponentChanging(Application, ownerProperty);
					List<IComponent> addedComponents = new List<IComponent>();
					try {
						Application.Modules.BeginInit();
						foreach(IComponent comp in newComponents) {
							if(comp is ModuleBase) {
								Application.Modules.Add((ModuleBase)comp);
								addedComponents.Add(comp);
							}
						}
					}
					finally {
						Application.Modules.EndInit();
					}
					cs.OnComponentChanged(Application, ownerProperty, null, null);
					trans.Commit();
					if(XafRootDesignerBase.Enable_DTE) {
						ToolboxService.SelectedToolboxItemUsed();
					}
					if(addedComponents.Count != 0) {
						SelectionService.SetSelectedComponents(addedComponents, SelectionTypes.Replace);
					}
				}
			}
		}
		public XafApplication Application {
			get { return (XafApplication)Component; }
		}
	}
}
