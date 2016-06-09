#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel.Design;
using DevExpress.DashboardCommon;
using DevExpress.Data.Utils;
using DevExpress.Design.TypePickEditor;
using DevExpress.Design.VSIntegration;
using DevExpress.Utils.Design;
using DevExpress.XtraTreeList.Nodes;
using EnvDTE;
namespace DevExpress.DashboardWin.Design {
	public class DashboardSourceEditorBase : TreeViewTypePickEditor {
		const string LoadDashboardString = "Load Dashboard...";
		public override bool IsDropDownResizable { get { return true; } }
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new DashboardSourceTypePickerTreeViewBase(typeof(Dashboard));
		}
		protected override object GetEditValue(PickerNode selectedPickerNode, ITypeDescriptorContext context, IServiceProvider provider) {
			return selectedPickerNode.Tag;
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new TypePickerPanel(picker);
		}
	}
	public class DashboardSourceTypePickerTreeViewBase : TypePickerTreeView {
		const string None = "(none)";
		const string ProjectDashboardsNodeName = "Project Dashboards";
		public DashboardSourceTypePickerTreeViewBase(Type type)
			: base(type, None) {
		}
		public PickerNode PushTypeNode(Type type, TreeListNodes owner) {
			return InsertTaggedNode(-1, type.Name, type, type, owner);
		}
		protected TreeListNode FindNodeByValue(TreeListNodes nodes, object value) {
			foreach(TreeListNode node in nodes) {
				if(object.Equals(value, node.Tag)) {
					return node;
				} else {
					TreeListNode selectedNode = FindNodeByValue(node.Nodes, value);
					if(selectedNode != null)
						return selectedNode;
				}
			}
			return null;
		}
		protected PickerNode InsertTaggedNode(int index, string text, Type type, object tag, TreeListNodes owner) {
			PickerNode pickerNode = new TypePickerNode(text, type, owner);
			IList list = ((IList)owner);
			if(index < 0) {
				list.Add(pickerNode);
			} else {
				list.Insert(index, pickerNode);
			}
			pickerNode.Tag = tag;
			return pickerNode;
		}
		public override void Start(ITypeDescriptorContext context, IServiceProvider provider, object currentValue) {
			base.Start(context, provider, currentValue);
			IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
			PushNodes(designerHost);
			TreeListNode node = FindNodeByValue(Nodes, currentValue);
			node = PushCurrentValue(currentValue, node, designerHost);
			SelectNode(node);
		}
		protected virtual void PushNodes(IDesignerHost designerHost) {
			ProjectObjectsPickerNode nodeObjects = new ProjectObjectsPickerNode(ProjectDashboardsNodeName, Nodes);
			TreeListNodes nodes = nodeObjects.Nodes;
			Nodes.Add(nodeObjects);
			ITypeDiscoveryService typeDiscoveryService = designerHost.GetService<ITypeDiscoveryService>();
			if(typeDiscoveryService != null) {
				string[] types = new DTEServiceBase(designerHost).GetClassesInfo(typeof(Dashboard), new string[] { designerHost.RootComponentClassName });
				foreach(string type in types) {
					PushTypeNode(type, nodes, designerHost);
				}
			}
		}
		void PushTypeNode(string typeName, TreeListNodes nodes, IDesignerHost designerHost) {
			Type type = null;
			try {
				type = designerHost.GetType(typeName);
			} catch { }
			if(type != null)
				PushTypeNode(type, nodes);
			else {
				TypePickerNode pickerNode = new TypePickerNode(GetNewAddedClassCaption(typeName), typeof(string), nodes);
				IList list = ((IList)nodes);
				list.Add(pickerNode);
				pickerNode.Tag = typeName;
			}
		}
		static string GetNewAddedClassCaption(string typeName) {
			if(typeName == null)
				return typeName;
			try {
				Project project = DTEHelper.GetActiveProject();
				if(project == null)
					return typeName;
				string projectDefaultNameSpace = null;
				try {
					projectDefaultNameSpace = DTEHelper.GetPropertyValue(project, "DefaultNameSpace");
				} catch { }
				if(string.IsNullOrEmpty(projectDefaultNameSpace))
					projectDefaultNameSpace = project.Name;
				if(typeName.StartsWith(projectDefaultNameSpace))
					return typeName.Substring(projectDefaultNameSpace.Length + 1);
			} catch { }
			return typeName;
		}
		protected virtual TreeListNode PushCurrentValue(object currentValue, TreeListNode node, IDesignerHost designerHost) {
			return node;
		}
	}
}
