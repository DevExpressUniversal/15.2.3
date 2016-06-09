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
using System.ComponentModel.Design;
using System.IO;
using System.Web.UI.Design;
using DevExpress.DashboardCommon;
using DevExpress.Design.TypePickEditor;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.DashboardWin.Design {
	public class DashboardSourceUrlEditor : DashboardSourceEditorBase {
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new DashboardSourceUrlTypePickerTreeView(typeof(Dashboard), provider);
		}
	}
	public class DashboardSourceUrlTypePickerTreeView : DashboardSourceTypePickerTreeViewBase {
		string webAppPath = string.Empty;
		public DashboardSourceUrlTypePickerTreeView(Type type, IServiceProvider provider)
			: base(type) {
		}
		protected override void PushNodes(IDesignerHost designerHost) {
			base.PushNodes(designerHost);
			IWebApplication webApplication = (IWebApplication)designerHost.GetService(typeof(IWebApplication));
			if(webApplication == null)
				return;
			webAppPath = webApplication.RootProjectItem.PhysicalPath;
			AddFakedNode(webAppPath, "Project Xml Files", null, true);
		}
		void AddFakedNode(string path, string customCaption, IFileNode parentNode, bool isDirectory) {
			string parentPath = parentNode == null ? string.Empty : (string)parentNode.Path;
			if(!parentPath.EndsWith("\\"))
				parentPath = parentPath + "\\";
			string itemCaption = path.Remove(0, parentPath.Length);
			if(string.IsNullOrEmpty(customCaption))
				customCaption = itemCaption;
			if(parentNode != null && object.Equals(parentNode.Path, webAppPath) && (itemCaption == "bin" || itemCaption == "obj"))
				return;
			TreeListNodes parentNodes = parentNode == null ? Nodes : parentNode.Nodes;
			IFileNode node = isDirectory ? (IFileNode)(new FolderPickerNode(customCaption, parentNodes)) : (IFileNode)(new XmlFilePickerNode(customCaption, typeof(string), parentNodes, "~/" + path.Remove(0, webAppPath.Length).Replace('\\', '/')));
			parentNodes.Add((TreeListNode)node);
			node.Path = path;
			if(isDirectory && (Directory.GetFiles(node.Path, "*.xml", SearchOption.TopDirectoryOnly).Length > 0 || Directory.GetDirectories(node.Path).Length > 0))
				InsertTaggedNode(-1, "fake", typeof(string), "fake", node.Nodes);
		}
		protected override void RaiseAfterExpand(TreeListNode node) {
			base.RaiseAfterExpand(node);
			FolderPickerNode folderNode = node as FolderPickerNode;
			if(folderNode == null || folderNode.Loaded)
				return;
			ExpandNode(folderNode);
		}
		protected override void RaiseAfterCollapse(TreeListNode node) {
			base.RaiseAfterCollapse(node);
		}
		void ExpandNode(FolderPickerNode node) {
			foreach(string directory in Directory.GetDirectories(node.Path))
				AddFakedNode(directory, null, node, true);
			foreach(string file in Directory.GetFiles(node.Path, "*.xml", SearchOption.TopDirectoryOnly))
				AddFakedNode(file, null, node, false);
			node.Nodes.RemoveAt(0);
			node.Loaded = true;
		}
		protected override void OnGetStateImage(object sender, XtraTreeList.GetStateImageEventArgs e) {
			base.OnGetStateImage(sender, e);
			if(e.Node is FolderPickerNode)
				e.NodeImageIndex = e.Node.Expanded ? 11 : 10;
			if(e.Node is XmlFilePickerNode)
				e.NodeImageIndex = 3;
		}
		protected override TreeListNode PushCurrentValue(object currentValue, TreeListNode node, IDesignerHost designerHost) {
			TreeListNode foundNode = base.PushCurrentValue(currentValue, node, designerHost);
			if(foundNode == null) {
				string value = currentValue as string;
				if(value == null)
					return null;
				if(value.StartsWith("~/")) {
					foreach(TreeListNode treeNode in Nodes) {
						FolderPickerNode folderNode = treeNode as FolderPickerNode;
						if(folderNode != null)
							return FindNode(folderNode, value);
					}
					return null;
				} else {
					node = new XmlFilePickerNode(value, typeof(string), Nodes, value);
					((IList)Nodes).Insert(1, node);
					return node;
				}
			} else
				return foundNode;
		}
		TreeListNode FindNode(FolderPickerNode rootNode, string path) {
			return FindNode(rootNode, path.Split('/'), 1);
		}
		TreeListNode FindNode(FolderPickerNode rootNode, string[] paths, int startIndex) {
			rootNode.Expand();
			foreach(TreeListNode node in rootNode.Nodes) {
				if(startIndex == paths.Length - 1) {
					XmlFilePickerNode xml = node as XmlFilePickerNode;
					if(xml != null && xml.Text == paths[startIndex])
						return xml;
				} else {
					FolderPickerNode folder = node as FolderPickerNode;
					if(folder != null && paths[startIndex] == folder.Text)
						return FindNode(folder, paths, ++startIndex);
				}
			}
			return null;
		}
	}
	interface IFileNode {
		string Path { get; set; }
		TreeListNodes Nodes { get; }
	}
	class FolderPickerNode : XtraListNode, IFileNode {
		public bool Loaded { get; set; }
		public string Path { get; set; }
		public FolderPickerNode(string text, TreeListNodes owner) : base(text, owner) { }
	}
	class XmlFilePickerNode : TypePickerNode, IFileNode {
		public string Path { get; set; }
		public XmlFilePickerNode(string text, Type type, TreeListNodes owner, string tag) : base(text, type, owner) {
			Tag = tag;
		}
	}
}
