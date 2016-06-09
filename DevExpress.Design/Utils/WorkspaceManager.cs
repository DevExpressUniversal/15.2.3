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
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.Utils.Animation;
using System.Windows.Forms;
using System.Collections;
using DevExpress.Design.CodeGenerator;
namespace DevExpress.Utils.Design {
	public class WorkspaceManagerDesigner : BaseComponentDesigner {
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitialize();
		}
		protected static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null) return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj)) return obj;
			}
			return null;
		}
		protected WorkspaceManager Manager {
			get { return Component as WorkspaceManager; }
		}
		protected virtual void OnInitialize() {
			if(Manager == null || Manager.Container == null) return;
			ContainerControl container = GetContainerControl(Manager.Container);
			Manager.TargetControl = container;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateWorkspaceManagerActionList());
			base.RegisterActionLists(list);
		}
		protected virtual WorkspaceManagerActionList CreateWorkspaceManagerActionList() {
			return new WorkspaceManagerActionList(this);
		}
		public static Form GetForm(IContainer container) {
			return GetTypeFromContainer(container, typeof(Form)) as Form;
		}
		public static ContainerControl GetContainerControl(IContainer container) {
			if(GetForm(container) != null) return GetForm(container);
			return GetUserControl(container);
		}
		public static ContainerControl GetUserControl(IContainer container) {
			foreach(object obj in container.Components) {
				ContainerControl ctrl = obj as ContainerControl;
				if(ctrl != null && ctrl.ParentForm == null) return ctrl;
			}
			return null;
		}
	}
	public class WorkspaceManagerActionList : DesignerActionList {
		WorkspaceManagerDesigner designerCore;
		public WorkspaceManagerActionList(WorkspaceManagerDesigner designer)
			: base(designer.Component) {
			this.designerCore = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("TransitionType", "Transition Type", "WorkspaceManager"));
			return res;
		}
		protected WorkspaceManagerDesigner Designer {
			get { return designerCore; }
		}
		protected WorkspaceManager Manager {
			get { return Component as WorkspaceManager; }
		}
		[RefreshProperties(RefreshProperties.All), TypeConverter(typeof(TransitionAnimatorTypeConverter))]
		public ITransitionAnimator TransitionType {
			get { return Manager.TransitionType; }
			set {
				EditorContextHelper.SetPropertyValue(Designer, Component, "TransitionType", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
	}
}
