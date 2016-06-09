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

using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class CustomizationTreeView : ASPxTreeView {
		readonly ISupportsFieldsCustomization customizationControl;
		public CustomizationTreeView(ASPxWebControl owner, ISupportsFieldsCustomization customizationControl)
			: base(owner) {
			this.customizationControl = customizationControl;
			ParentSkinOwner = PivotGrid;
			Initialize();
		}
		void Initialize() {
			ID = "treeCF";
			ShowTreeLines = false;
			EnableClientSideAPI = true;
		}
		public ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)OwnerControl; } }
		public ISupportsFieldsCustomization CustomizationControl { get { return customizationControl; } }
		public void AddNodes(PivotCustomizationFieldsTree tree) {
			AddNodesCore(tree.Root, Nodes);
		}
		static void AddNodesCore(IVisualCustomizationTreeItem logicalNode, TreeViewNodeCollection nodes) {
			foreach(IVisualCustomizationTreeItem item in logicalNode.EnumerateChildren()) {
				CustomizationTreeViewNode node = new CustomizationTreeViewNode(item);
				nodes.Add(node);
				if(item.CanExpand) {
					AddNodesCore(item, node.Nodes);
					node.ClientVisible = node.Nodes.Any(n => n.ClientVisible);
				}
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.CustomizationTreeView";
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat(localVarName + ".folderOpenClass='{0}';\n", CustomizationTreeNodeCssClasses.FolderOpen);
			stb.AppendFormat(localVarName + ".folderClosedClass='{0}';\n", CustomizationTreeNodeCssClasses.FolderClosed);
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(this.GetType(), PivotGridWebData.CustomizationTreeScriptResourceName);
		}
	}
	public class CustomizationTreeViewNode : TreeViewNode {
		const int ImageSize = 16;
		public CustomizationTreeViewNode(IVisualCustomizationTreeItem node) : base(node.Caption) {
			IsFolder = node is PivotCustomizationTreeNodeFolder;
			AllowCheck = false;
			Image.Height = Image.Width = Unit.Pixel(ImageSize);
			Image.SpriteProperties.CssClass = node.CssClass;
			if(!node.CanExpand) {
				PivotFieldItemBase field = node.Field;
				ClientVisible = !field.Visible;
				SetProperties(field);
			}
		}
		public bool IsFolder { get; protected set; }
		public override bool Expanded {
			get { return base.Expanded; }
			set {
				if(Expanded == value) return;
				base.Expanded = value;
				UpdateImage(value);
			}
		}
		void SetProperties(PivotFieldItemBase field) {
			string[] properties = new string[] {
				field.Group != null ? "pgGroupHeader" + field.GroupIndex : "pgHeader" + field.Index, 
				field.CanDragInCustomizationForm ? "T" : "F" 
			};
			Name = string.Join(",", properties);
		}
		void UpdateImage(bool expanded) {
			if(IsFolder)
				this.Image.SpriteProperties.CssClass = expanded ? CustomizationTreeNodeCssClasses.FolderOpen : CustomizationTreeNodeCssClasses.FolderClosed;
		}
	}
}
