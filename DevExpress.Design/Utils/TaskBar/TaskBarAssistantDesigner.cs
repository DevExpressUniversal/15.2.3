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

using DevExpress.Utils.Taskbar;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
namespace DevExpress.Utils.Design.Taskbar {
	public class TaskbarAssistantDesigner : BaseComponentDesigner {
		public TaskbarAssistantDesigner() { }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			TaskbarAssistant.DesignTimeManager = new TaskbarAssistantDesignTimeManager(component);
			TaskbarAssistant.ParentControl = RootControl;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new TaskbarAssistantDesignerActionList(Component));
			base.RegisterActionLists(list);
		}
		protected Control RootControl {
			get { return DesignerHost.RootComponent as Control; }
		}
		IDesignerHost designerHostCore = null;
		protected IDesignerHost DesignerHost {
			get { return designerHostCore ?? (designerHostCore = GetService<IDesignerHost>()); }
		}
		protected IServiceProvider ServiceProvider {
			get { return TaskbarAssistant.Site; }
		}
		protected T GetService<T>() where T : class {
			return ServiceProvider.GetService(typeof(T)) as T;
		}
		TaskbarAssistant TaskbarAssistant { get { return Component as TaskbarAssistant; } }
	}
	public class TaskbarAssistantDesignTimeManager : TaskbarAssistantDesignTimeManagerBase {
		IComponent component;
		public TaskbarAssistantDesignTimeManager(IComponent component) : base(component) {
			this.component = component;
		}
		internal IComponent Component { get { return component; } }
		protected override string GetProjectPathDesignTime() {
			EnvDTE.ProjectItem item = TaskbarAssistant.Site.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			if(item == null)
				return null;
			EnvDTE.Project project = item.ContainingProject;
			return Path.Combine(TaskbarAssistantDesignerHelper.RootProjectDirectory(project), TaskbarAssistantDesignerHelper.ConfigurationPath(project));
		}
		TaskbarAssistant TaskbarAssistant { get { return this.component as TaskbarAssistant; } }
	}
	public class TaskbarAssistantDesignerActionList : DesignerActionList {
		IComponent component;
		public TaskbarAssistantDesignerActionList(IComponent component)
			: base(component) {
			this.component = component;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "AddNativeResourceFile", "Add Icons to Project Resources", "Taskbar Helpers", "Add Native Resource Template file from project."));
			return res;
		}
		public void AddNativeResourceFile() {
			TaskbarAssistantDesignerHelper.AddNativeResourceFile(CurrentProject);
		}
		EnvDTE.ProjectItem CurrentItem {
			get { return GetService<EnvDTE.ProjectItem>(); }
		}
		EnvDTE.Project CurrentProject {
			get { return CurrentItem.ContainingProject; }
		}
		protected IServiceProvider ServiceProvider { 
			get { return TaskbarAssistant.Site; } 
		}
		protected T GetService<T>() where T : class { 
			return ServiceProvider.GetService(typeof(T)) as T; 
		}
		TaskbarAssistant TaskbarAssistant { 
			get { return this.component as TaskbarAssistant; } 
		}
	}
}
