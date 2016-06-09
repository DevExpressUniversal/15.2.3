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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
namespace DevExpress.Utils {
	internal static class WorkspaceManagerSerializer {
		public static void Serialize(IComponent root, Stream stream, string appName, bool acceptNestedObjects) {
			SerializeCore(root, stream, appName, acceptNestedObjects);
		}
		public static void Serialize(IComponent root, string path, string appName, bool acceptNestedObjects) {
			SerializeCore(root, path, appName, acceptNestedObjects);
		}
		public static void Deserialize(IComponent root, Stream stream, string appName) {
			DeserializeCore(root, stream, appName);
		}
		public static void Deserialize(IComponent root, string path, string appName) {
			DeserializeCore(root, path, appName);
		}
		static bool SerializeCore(IComponent root, Stream stream, string appName, bool acceptNestedObjects) {
			return new XmlXtraSerializer().SerializeObjects(GetObjects(root, true, acceptNestedObjects), stream, appName);
		}
		static bool SerializeCore(IComponent root, string path, string appName, bool acceptNestedObjects) {
			return new XmlXtraSerializer().SerializeObjects(GetObjects(root, true, acceptNestedObjects), path, appName);
		}
		static void DeserializeCore(IComponent root, Stream stream, string appName) {
			new XmlXtraSerializer().DeserializeObjects(GetObjects(root, false, true), stream, appName);
		}
		static void DeserializeCore(IComponent root, string path, string appName) {
			new XmlXtraSerializer().DeserializeObjects(GetObjects(root, false, true), path, appName);
		}
		static XtraObjectInfo[] GetObjects(IComponent rootComponent, bool serialization, bool acceptNestedObjects) {
			WorkspaceManager.RemoveDeadWeakreference();
			List<IComponent> componentsCache = new List<IComponent>();
			ComponentTreeNode rootNode = new ComponentTreeNode();
			List<XtraObjectInfo> objects = new List<XtraObjectInfo>();
			BuildComponentTree(rootComponent, componentsCache, rootNode);
			GetObjectsCore(new ComponentTreeNode[] { rootNode }, objects, serialization, acceptNestedObjects);
			componentsCache.Clear();
			rootNode = null;
			return objects.ToArray();
		}
		static void GetObjectsCore(ComponentTreeNode[] currentNodes, List<XtraObjectInfo> objects, bool serialization, bool acceptNestedObjects) {
			List<ComponentTreeNode> children = new List<ComponentTreeNode>();
			foreach(var item in currentNodes) {
				ComponentWeakReference reference = new ComponentWeakReference(item.Component);
				if(item.Component is IXtraSerializable && !WorkspaceManager.disabledControls.Contains(reference)) {
					if(serialization)
						objects.Add(new XtraObjectInfo(item.ExtendedKey, item.Component));
					else {
						objects.Add(new XtraObjectInfo(item.Key, item.Component));
						objects.Add(new XtraObjectInfo(item.ExtendedKey, item.Component));
					}
				}
				children.AddRange(item.Nodes.ToArray());
			}
			if(children.Count == 0) return;
			if(!acceptNestedObjects) return;
			GetObjectsCore(children.ToArray(), objects, serialization, acceptNestedObjects);
		}
		static void BuildComponentTree(IComponent currentComponent, List<IComponent> components, ComponentTreeNode currentNode) {
			if(components.Contains(currentComponent)) return;
			var control = currentComponent as Control;
			currentNode.Component = currentComponent;
			components.Add(currentComponent);
			if(currentComponent is ILogicalOwner) {
				var childComponents = (currentComponent as ILogicalOwner).GetLogicalChildren().OrderBy((x => x), new SerializationOrderComparer());
				foreach(var item in childComponents) {
					if(item != null && !components.Contains(item)) {
						var childNode = new ComponentTreeNode(currentNode);
						BuildComponentTree(item, components, childNode);
					}
				}
			}
			if(control != null) {
				var childComponents = ComponentLocator.FindComponents(control).OrderBy((x => x), new SerializationOrderComparer());
				foreach(var item in childComponents) {
					if(item != null && !components.Contains(item)) {
						var childNode = new ComponentTreeNode(currentNode);
						BuildComponentTree(item, components, childNode);
					}
				}
				foreach(Control item in control.Controls) {
					if(components.Contains(item)) continue;
					var childNode = new ComponentTreeNode(currentNode);
					BuildComponentTree(item, components, childNode);
				}
			}
		}
	}
}
