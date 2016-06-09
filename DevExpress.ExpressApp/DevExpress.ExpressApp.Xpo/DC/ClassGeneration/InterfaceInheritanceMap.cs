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
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	sealed class InterfaceInheritanceMap {
		private readonly Dictionary<Type, MapNode> nodesByTypes;
		internal InterfaceInheritanceMap() {
			nodesByTypes = new Dictionary<Type, MapNode>();
		}
		private void FilterRelation(MapNode child, MapNode parent) {
			foreach(MapNode node in parent.GetParents()) {
				if(MapNode.HaveLink(child, node)) {
					MapNode.RemoveLink(child, node);
				}
			}
		}
		private MapNode CreateNodeByType(Type type) {
			MapNode node = new MapNode(type);
			nodesByTypes.Add(type, node);
			return node;
		}
		private MapNode FindNodeByType(Type type) {
			MapNode node;
			nodesByTypes.TryGetValue(type, out node);
			return node;
		}
		internal void Build(IEnumerable<Type> types) {
			nodesByTypes.Clear();
			foreach(Type type in types) {
				CreateNodeByType(type);
			}
			Queue<MapNode> queue = new Queue<MapNode>(nodesByTypes.Values);
			while(queue.Count > 0) {
				MapNode node = queue.Dequeue();
				foreach(Type implementedInterface in node.Type.GetInterfaces()) {
					MapNode otherNode = FindNodeByType(implementedInterface);
					if(otherNode == null) {
						otherNode = CreateNodeByType(implementedInterface);
						queue.Enqueue(otherNode);
					}
					MapNode.AddLink(node, otherNode);
				}
			}
			foreach(MapNode node in nodesByTypes.Values) {
				foreach(MapNode child in node.GetChildren()) {
					FilterRelation(child, node);
				}
			}
		}
		internal Type[] GetProcessedTypes() {
			Type[] types = new Type[nodesByTypes.Count];
			nodesByTypes.Keys.CopyTo(types, 0);
			return types;
		}
		internal bool IsProcessedType(Type type) {
			return nodesByTypes.ContainsKey(type);
		}
		internal Type[] GetProcessedTypesOrderedByAssignabilityAscending() {
			List<Type> types = new List<Type>();
			Queue<Type> queue = new Queue<Type>(GetProcessedTypes());
			while(queue.Count > 0) {
				Type type = queue.Dequeue();
				bool skip = false;
				foreach(Type child in GetChildrenTypes(type)) {
					if(queue.Contains(child)) {
						skip = true;
						break;
					}
				}
				if(!skip) {
					types.Add(type);
				}
				else {
					queue.Enqueue(type);
				}
			}
			return types.ToArray();
		}
		internal Type[] GetProcessedTypesOrderedByAssignabilityDescending() {
			List<Type> types = new List<Type>();
			Queue<Type> queue = new Queue<Type>(GetProcessedTypes());
			while(queue.Count > 0) {
				Type type = queue.Dequeue();
				bool skip = false;
				foreach(Type child in GetParentTypes(type)) {
					if(queue.Contains(child)) {
						skip = true;
						break;
					}
				}
				if(!skip) {
					types.Add(type);
				}
				else {
					queue.Enqueue(type);
				}
			}
			return types.ToArray();
		}
		internal Type[] GetChildrenTypes(Type type) {
			MapNode node = FindNodeByType(type);
			if(node != null) {
				MapNode[] children = node.GetChildren();
				Type[] types = new Type[children.Length];
				for(int i = 0; i < children.Length; ++i) {
					types[i] = children[i].Type;
				}
				return types;
			}
			return Type.EmptyTypes;
		}
		internal Type[] GetParentTypes(Type type) {
			MapNode node = FindNodeByType(type);
			if(node != null) {
				MapNode[] parents = node.GetParents();
				Type[] types = new Type[parents.Length];
				for(int i = 0; i < parents.Length; ++i) {
					types[i] = parents[i].Type;
				}
				return types;
			}
			return Type.EmptyTypes;
		}
	}
	sealed class MapNode {
		internal static bool HaveLink(MapNode child, MapNode parent) {
			return child.parents.ContainsKey(parent);
		}
		internal static void AddLink(MapNode child, MapNode parent) {
			child.parents.Add(parent, null);
			parent.children.Add(child, null);
		}
		internal static void RemoveLink(MapNode child, MapNode parent) {
			child.parents.Remove(parent);
			parent.children.Remove(child);
		}
		private readonly Dictionary<MapNode, object> parents;
		private readonly Dictionary<MapNode, object> children;
		private readonly Type type;
		internal MapNode(Type type) {
			parents = new Dictionary<MapNode, object>();
			children = new Dictionary<MapNode, object>();
			this.type = type;
		}
		internal Type Type { get { return type; } }
		internal MapNode[] GetParents() {
			MapNode[] result = new MapNode[parents.Count];
			parents.Keys.CopyTo(result, 0);
			return result;
		}
		internal MapNode[] GetChildren() {
			MapNode[] result = new MapNode[children.Count];
			children.Keys.CopyTo(result, 0);
			return result;
		}
		public override string ToString() {
			return string.Format("MapNode: {0}", Type);
		}
	}
}
