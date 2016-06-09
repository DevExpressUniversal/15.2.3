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
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;
using System.Activities.Presentation;
using System.ComponentModel;
using System.Reflection;
using System.Activities.Presentation.Hosting;
namespace DevExpress.ExpressApp.Workflow.Win {
	[System.ComponentModel.ToolboxItem(false)]
	public class WorkflowDesignerControlBase : ElementHost {
		[DefaultValue(false)] 
		public static bool CanSetTargetFramework45 { get; set; }
		[DefaultValue(true)] 
		public static bool RemoveFramework45DebugSymbol { get; set; }
		[DefaultValue(true)] 
		public static bool CanFilterResourceAssemblyNamespaces { get; set; }
		private String xaml = null;
		static WorkflowDesignerControlBase() {
			CanSetTargetFramework45 = false;
			RemoveFramework45DebugSymbol = true;
			CanFilterResourceAssemblyNamespaces = true;
			(new System.Activities.Core.Presentation.DesignerMetadata()).Register();
			(new DevExpress.Workflow.Activities.Design.ActivitiesMetadata()).Register();
		}
		private static void FilterResourceAssemblyNamespaces(WorkflowDesigner designer) {
			AssemblyContextControlItem assemblyContextControlItem = designer.Context.Items.GetValue<AssemblyContextControlItem>();
			if(assemblyContextControlItem == null) {
				assemblyContextControlItem = new AssemblyContextControlItem(); 
				designer.Context.Items.SetValue(assemblyContextControlItem);
			}
			List<AssemblyName> referencedAssemblyNames = new List<AssemblyName>();
			foreach(AssemblyName assemblyName in assemblyContextControlItem.GetEnvironmentAssemblyNames()) {
				if(!assemblyName.Name.Contains(".resources")) {
					referencedAssemblyNames.Add(assemblyName); 
				}
			}
			assemblyContextControlItem.ReferencedAssemblyNames = referencedAssemblyNames;
		}
		protected void SetXamlCore(string value) {
			BeforeLoadActivity();
			this.xaml = value;
			LoadActivity(xaml);
			if(ActivityLoaded != null) {
				ActivityLoaded(this, EventArgs.Empty);
			}
			AfterLoadActivity();
		}
		protected virtual void BeforeLoadActivity() {
		}
		protected virtual void LoadActivity(string xaml) {
			Designer = new WorkflowDesigner();
			if(CanSetTargetFramework45) {
				Type designerConfigurationServiceType = null;
				foreach(Type t in Designer.Context.Services) {
					if(t.FullName == "System.Activities.Presentation.DesignerConfigurationService") {
						designerConfigurationServiceType = t;
					}
				}
				if(designerConfigurationServiceType != null) {
					object designerConfigurationService = Designer.Context.Services.GetService(designerConfigurationServiceType);
					if(designerConfigurationService != null) {
						System.Reflection.PropertyInfo targetFrameworkNameProperty = designerConfigurationServiceType.GetProperty("TargetFrameworkName");
						if(targetFrameworkNameProperty != null) {
							try {
								targetFrameworkNameProperty.SetValue(designerConfigurationService, new System.Runtime.Versioning.FrameworkName(".NETFramework", new Version(4, 5)), new object[] { });
							}
							catch { }
						}
					}
				}
			}
			if(WorkflowDesignerCreated != null) {
				WorkflowDesignerCreated(this, new WorkflowDesignerCreatedEventArgs(Designer));
			}
			Designer.Text = xaml;
			try {
				Designer.Load();
				if(RemoveFramework45DebugSymbol) {
					try {
						MethodInfo getAttachedWorkflowSymbol = Designer.GetType().GetMethod("GetAttachedWorkflowSymbol", BindingFlags.Instance | BindingFlags.NonPublic);
						if(getAttachedWorkflowSymbol != null) {
							getAttachedWorkflowSymbol.Invoke(Designer, new object[0]);
						}
					}
					catch { }
				}
				if(CanFilterResourceAssemblyNamespaces) {
					FilterResourceAssemblyNamespaces(Designer);
				}
			}
			catch(Exception e) {
				throw new UserFriendlyException(e);
			}
		}
		protected virtual void AfterLoadActivity() {
		}
		public WorkflowDesignerControlBase() {
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				Designer = null;
			}
		}
		public WorkflowDesigner Designer { get; private set; }
		public String Xaml {
			get {
				if(Designer != null) {
					Designer.Flush();
					xaml = Designer.Text;
				}
				return xaml;
			}
			set {
				if(value != xaml) {
					SetXamlCore(value);
				}
			}
		}
		public event EventHandler<EventArgs> ActivityLoaded;
		public static event EventHandler<WorkflowDesignerCreatedEventArgs> WorkflowDesignerCreated;
	}
	public class WorkflowDesignerCreatedEventArgs : EventArgs {
		public WorkflowDesignerCreatedEventArgs(WorkflowDesigner workflowDesigner) {
			WorkflowDesigner = workflowDesigner;
		}
		public WorkflowDesigner WorkflowDesigner { get; private set; }
	}
}
