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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Design {
	[ToolboxItem(false)]
	[ToolboxItemFilter("DevExpress.ExpressApp.Controller", ToolboxItemFilterType.Require)]
	[ToolboxItemFilter("", ToolboxItemFilterType.Custom)]
	public class ControllerDesigner : ComponentDocumentDesigner, IToolboxUser {
		private readonly string designerVersion = AssemblyInfo.VersionShort;
		private readonly Dictionary<ToolboxItem, bool> toolboxItemsSupportedDictionary = new Dictionary<ToolboxItem, bool>();
		private IComponentChangeService componentChangeService;
		private PropertyDescriptor actionsPropertyDescriptor;
		static ControllerDesigner() {
			XafDesignerHelper.ShowAboutProductsEx();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(XafDesignerHelper.CheckLicenseExpired()) {
				foreach(System.Windows.Forms.Control item in Control.Controls) {
					item.Hide();
				}
				Control.Controls.Add(XafDesignerHelper.GetLicenseErrorControl());
			}
			componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if(componentChangeService != null) {
				componentChangeService.ComponentAdded += componentChangeService_ComponentAdded;
			}
			actionsPropertyDescriptor = TypeDescriptor.GetProperties(Controller)["Actions"];
		}
		protected override void Dispose(bool disposing) {
			if(componentChangeService != null) {
				componentChangeService.ComponentAdded -= componentChangeService_ComponentAdded;
				componentChangeService = null;
			}
			actionsPropertyDescriptor = null;
			base.Dispose(disposing);
		}
		private void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e) {
			if(e.Component is ActionBase && !Controller.Actions.Contains((ActionBase)e.Component)) {
				componentChangeService.OnComponentChanging(Controller, actionsPropertyDescriptor);
				Controller.Actions.Add((ActionBase)e.Component);
				componentChangeService.OnComponentChanged(Controller, actionsPropertyDescriptor, null, null);
			}
		}
		public Controller Controller {
			get { return (Controller)Component; }
		}
		bool IToolboxUser.GetToolSupported(ToolboxItem tool) {
			bool result;
			if(!toolboxItemsSupportedDictionary.TryGetValue(tool, out result)) {
				lock(toolboxItemsSupportedDictionary) {
					if(!toolboxItemsSupportedDictionary.TryGetValue(tool, out result)) {
						result = IsToolSupported(tool);
						toolboxItemsSupportedDictionary.Add(tool, result);
					}
				}
			}
			return result;
		}
		private bool IsToolSupported(ToolboxItem tool) {
			bool result = GetVersionShort(tool.AssemblyName) == designerVersion;
			if(!result) { 
				foreach(AssemblyName assemblyName in tool.DependentAssemblies) {
					if(assemblyName.Name.IndexOf("DevExpress.ExpressApp") != -1) {
						result = GetVersionShort(assemblyName) == designerVersion;
						break;
					}
				}
			}
			return result;
		}
		private string GetVersionShort(AssemblyName assemblyName) {
			return assemblyName.Version.Major + "." + assemblyName.Version.Minor;
		}
	}
}
