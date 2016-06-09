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

using DevExpress.Data.IO;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal.InternalCheckBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Web.Mvc {
	public class TreeViewState {
		public TreeViewState() {
			RootNode = new TreeViewNodeState();
		}
		TreeViewNodeState RootNode { get; set; }
		public IEnumerable<TreeViewNodeState> Nodes { get { return RootNode.Nodes; } }
		public TreeViewNodeState SelectedNode { get; private set; }
		internal string CreateSerializedNodesInfo() {
			using(var stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				SerializeNodesInfoCore(writer, Nodes);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		void SerializeNodesInfoCore(TypedBinaryWriter writer, IEnumerable<TreeViewNodeState> nodes) {
			writer.WriteObject(nodes.ToList().Count);
			foreach(TreeViewNodeState node in nodes) {
				writer.WriteObject(node.Index);
				writer.WriteObject(node.Name);
				writer.WriteObject(node.Text);
				SerializeNodesInfoCore(writer, node.Nodes);
			}
		}
		internal static string GetSerializedNodesInfo(TreeViewNodeCollection nodes) {
			using(var stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				SaveNodesInfoCore(writer, nodes);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		static void SaveNodesInfoCore(TypedBinaryWriter writer, TreeViewNodeCollection nodes) {
			writer.WriteObject(nodes.Count);
			for(int i = 0; i < nodes.Count; i++) {
				writer.WriteObject(nodes[i].Index);
				writer.WriteObject(nodes[i].Name);
				writer.WriteObject(nodes[i].Text);
				SaveNodesInfoCore(writer, nodes[i].Nodes);
			}
		}
		internal static TreeViewState Load(string serializedNodesInfo, ArrayList serializedNodesState) {
			if(string.IsNullOrEmpty(serializedNodesInfo))
				return null;
			TreeViewState treeViewState = new TreeViewState();
			treeViewState.LoadNodesInfo(serializedNodesInfo);
			treeViewState.LoadNodesState(serializedNodesState);
			return treeViewState;
		}
		protected void LoadNodesInfo(string serializedNodesInfoState) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(serializedNodesInfoState)))
			using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
				LoadNodesInfoCore(reader, RootNode);
			}
		}
		static void LoadNodesInfoCore(TypedBinaryReader reader, TreeViewNodeState rootNode) {
			int childNodesCount = reader.ReadObject<int>();
			rootNode.Nodes = new TreeViewNodeState[childNodesCount];
			for(int i = 0; i < childNodesCount; i++) {
				var node = new TreeViewNodeState(rootNode);
				((TreeViewNodeState[])rootNode.Nodes)[i] = node;
				node.Index = reader.ReadObject<int>();
				node.Name = reader.ReadObject<string>();
				node.Text = reader.ReadObject<string>();
				LoadNodesInfoCore(reader, node);
			}
		}
		protected void LoadNodesState(ArrayList nodesState) {
			if(nodesState == null || nodesState.Count == 0)
				return;
			LoadNodesExpandedState((Hashtable)nodesState[0]);
			SelectedNode = FindNodeStateByID((string)nodesState[1]);
			LoadNodesCheckedState((Hashtable)nodesState[2]);
		}
		void LoadNodesExpandedState(Hashtable expandedState) {
			foreach(DictionaryEntry entry in expandedState) {
				TreeViewNodeState node = FindNodeStateByID(entry.Key as string);
				if(node != null)
					node.Expanded = DeserializeBooleanValue(entry.Value as string);
			}
		}
		protected void LoadNodesCheckedState(Hashtable checkedState) {
			foreach(DictionaryEntry entry in checkedState) {
				TreeViewNodeState node = FindNodeStateByID(entry.Key as string);
				if(node != null)
					node.Checked = DeserializeCheckStateEnumValue(entry.Value as string);
			}
		}
		internal TreeViewNodeState FindNodeStateByID(string id) {
			if(string.IsNullOrEmpty(id))
				return null;
			string indexPath = id.Replace("N", string.Empty);
			string[] pathIndices = indexPath.Split('_');
			TreeViewNodeState node = RootNode;
			for(int i = 0; i < pathIndices.Length; i++) {
				int index = int.Parse(pathIndices[i]);
				if(node.Nodes.Count() - 1 < index)
					return null;
				node = node.Nodes.ElementAt(index);
			}
			return node;
		}
		bool DeserializeBooleanValue(string serializedValue) {
			return serializedValue == "T";
		}
		bool DeserializeCheckStateEnumValue(string serializedValue) {
			return serializedValue == InternalCheckboxControl.CheckedStateKey;
		}
	}
	public class TreeViewNodeState {
		public TreeViewNodeState()
			: this(null) {
		}
		internal TreeViewNodeState(TreeViewNodeState parent) {
			Parent = parent;
			Nodes = new TreeViewNodeState[0];
		}
		public TreeViewNodeState Parent { get; internal set; }
		public IEnumerable<TreeViewNodeState> Nodes { get; protected internal set;}
		public int Index { get; protected internal set; }
		public string Name { get; protected internal set; }
		public string Text { get; protected internal set; }
		public bool Checked { get; protected internal set; }
		public bool Expanded { get; protected internal set; }
	}
}
