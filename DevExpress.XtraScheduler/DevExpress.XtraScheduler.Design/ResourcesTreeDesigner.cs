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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Utils.Design;
using DevExpress.XtraTreeList.Design;
using DevExpress.XtraScheduler.UI;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Drawing;
namespace DevExpress.XtraScheduler.Design {
	public class ResourcesTreeDesigner : TreeListDesigner, IServiceProvider {
		IComponentChangeService changeService;
		public ResourcesTreeDesigner()
			:base(){
		}
		protected IComponentChangeService ChangeService { get { return changeService; } }
		protected ResourcesTree ResourcesTree { get { return Control as ResourcesTree; } }
		protected override DesignerVerbCollection CreateDesignerVerbs() {
			return new DesignerVerbCollection();
		}
		protected override BaseDesignerForm CreateFormEditor() {
			return new ResourceTreeDesignerForm();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new ResourcesTreeDataSourceActionList(this));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			ReferencesHelper.EnsureReferences(DesignerHost, AssemblyInfo.SRAssemblyTreeList);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (changeService != null) {
						changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
						changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnDataSourceChanged(object sender, EventArgs e) {
			EditorContextHelperEx.RefreshSmartPanel(TreeList);
		}
		protected override void OnDesignerHostLoadComplete(object sender, EventArgs e) {
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SchedulerControl", typeof(SchedulerControl));
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
		}
		public void UpdateResourcesTreeBounds() {
			if (ResourcesTree.SchedulerControl == null)
				return;
			PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo((ControlDesigner)this, "SchedulerControl", typeof(SchedulerControl));
			if (pi != null) {
				Rectangle newBounds = new Rectangle(ResourcesTree.Bounds.X, 
									  ResourcesTree.SchedulerControl.Bounds.Y, 
									  ResourcesTree.Bounds.Width, 
									  ResourcesTree.SchedulerControl.Bounds.Height);
				EditorContextHelper.SetPropertyValue((ControlDesigner)this, Component, "Bounds", newBounds);
			}
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public new void RunDesigner() {
			base.RunDesigner();
		}
		public new void AddColumn() {
			base.AddColumn();
		}
	}
	public class ResourcesTreeDataSourceActionList : DesignerActionList {
		readonly ResourcesTreeDesigner designer;
		public ResourcesTreeDataSourceActionList(ResourcesTreeDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		protected ResourcesTreeDesigner Designer { get { return designer; } }
		IDesigner IDesigner { get { return Designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if (SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl)) != null)
				res.Add(new DesignerActionPropertyItem("SchedulerControl", "Scheduler Control"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer"));
			res.Add(new DesignerActionMethodItem(this, "AddColumn", "Add Column"));
			res.Add(new DesignerActionMethodItem(this, "AlignToScheduler", "Align to Scheduler"));
			return res;
		}
		public virtual void RunDesigner() {
			Designer.RunDesigner();
		}
		public virtual void AddColumn() {
			Designer.AddColumn();
		}
		public SchedulerControl SchedulerControl {
			get {
				PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl));
				return pi != null ? pi.GetValue(Component, null) as SchedulerControl : null;
			}
			set {
				PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl));
				if (pi != null)
					EditorContextHelper.SetPropertyValue(IDesigner, Component, "SchedulerControl", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public void AlignToScheduler() {
			Designer.UpdateResourcesTreeBounds();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
	}
}
