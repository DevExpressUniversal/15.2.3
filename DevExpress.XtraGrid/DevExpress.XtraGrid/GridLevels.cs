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
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data.Helpers;
using System.Collections.Generic;
namespace DevExpress.XtraGrid {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridLevelTree : GridLevelNode {
		GridControl grid;
		public GridLevelTree(GridControl grid) : base(null, "Root", null) {
			SetTree(this);
			this.grid = grid;
		}
		public BaseView[] GetTemplates() {
			ArrayList list = new ArrayList();
			GetTemplates(list, Nodes);
			return list.ToArray(typeof(BaseView)) as BaseView[];
		}
		void GetTemplates(ArrayList list, GridLevelNodeCollection nodes) {
			if(nodes == null) return;
			foreach(GridLevelNode node in nodes) {
				if(node.LevelTemplate != null) list.Add(node.LevelTemplate);
				if(node.HasChildren) GetTemplates(list, node.Nodes);
			}
		}
		protected internal override GridControl Grid { get { return grid; } }
		protected internal void OnChanged(GridLevelNodeCollection collection, CollectionChangeEventArgs e) {
			if(e.Element != null) {
				GridLevelNode node = e.Element as GridLevelNode;
				OnNodeChanged(node);
			}
		}
		protected internal void OnNodeChanged(GridLevelNode node) { 
			Grid.FireChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BaseView LevelTemplate {
			get { return Grid.MainView; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string RelationName { 
			get { return "Root"; }
			set { }
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridLevelTreeIsRootLevel")]
#endif
		public override bool IsRootLevel { get { return true; } }
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class GridLevelNode : IDisposable {
		string relationName = "";
		BaseView levelTemplate = null;
		GridLevelNodeCollection nodes = null, ownerCollection = null;
		GridLevelTree tree;
		public GridLevelNode() : this(null, "", null) { }
		public GridLevelNode(GridLevelTree tree, string relationName, BaseView template) {
			this.relationName = relationName;
			this.levelTemplate = template;
			this.tree = tree;
			if(template != null) template.Connect(this);
		}
		public virtual void Dispose() {
			GridLevelNodeCollection oldCollection = OwnerCollection;
			this.ownerCollection = null;
			Nodes.Clear();
			LevelTemplate = null;
			if(oldCollection != null) oldCollection.Remove(this);
		}
		public GridLevelNode Find(string relationName) {
			if(RelationName == relationName) return this;
			if(!HasChildren) return null;
			for(int n = Nodes.Count - 1; n >= 0; n--) {
				GridLevelNode node = Nodes[n];
				GridLevelNode res = node.Find(relationName);
				if(res != null) return res;
			}
			return null;
		}
		public GridLevelNode Find(BaseView template) {
			if(template == null) return null;
			if(LevelTemplate == template) return this;
			if(!HasChildren) return null;
			for(int n = Nodes.Count - 1; n >= 0; n--) {
				GridLevelNode node = Nodes[n];
				GridLevelNode res = node.Find(template);
				if(res != null) return res;
			}
			return null;
		}
		internal bool Contains(BaseView template) { return Find(template) != null; }
		protected internal virtual GridControl Grid { get { return Tree != null ? Tree.Grid : null; } }
		protected internal GridLevelTree Tree { get { return tree; } }
		protected internal void SetTree(GridLevelTree tree) { 
			this.tree = tree; 
			if(!HasChildren) return;
			foreach(GridLevelNode node in nodes) { node.SetTree(tree); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridLevelNodeLevelTemplate"),
#endif
 DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual BaseView LevelTemplate {
			get { return levelTemplate; }
			set {
				if(LevelTemplate == value) return;
				if(levelTemplate != null) levelTemplate.Disconnect(this);
				levelTemplate = value;
				if(levelTemplate != null) {
					if(Grid != null) Grid.ViewCollection.Add(value);
					levelTemplate.Connect(this);
					levelTemplate.SetLevelName(RelationName);
				}
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridLevelNodeRelationName"),
#endif
 DefaultValue("")]
		public virtual string RelationName {
			get { return relationName; }
			set {
				if(value == null) value = "";
				if(RelationName == value) return;
				if(OwnerCollection == null || !OwnerCollection.Contains(value)) {
					if(OwnerCollection != null) OwnerCollection.Rename(this, relationName, value);
					relationName = value;
					if(LevelTemplate != null) LevelTemplate.SetLevelName(value);
					OnChanged();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Level {
			get {
				int res = 0;
				GridLevelNode parent = Parent;
				while(parent != null) {
					res ++;
					parent = parent.Parent;
				}
				return res;
			}
		}
		protected internal BaseView GetChildTemplate(string levelName) {
			if(!HasChildren) return null;
			GridLevelNode node = Nodes[levelName];
			if(node != null) return node.LevelTemplate;
			return null;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsRootLevel { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridLevelNode Parent { get { return OwnerCollection != null ? OwnerCollection.OwnerNode : null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridLevelNodeCollection OwnerCollection { get { return ownerCollection; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridLevelNodeCollection Nodes { 
			get { 
				if(nodes == null) nodes = new GridLevelNodeCollection(this);
				return nodes; 
			} 
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridLevelNodeHasChildren")]
#endif
		public bool HasChildren { get { return this.nodes != null && this.nodes.Count > 0; } }
		protected internal void SetOwner(GridLevelNodeCollection ownerCollection) {
			this.ownerCollection = ownerCollection;
		}
		protected virtual void OnChanged() {
			if(Tree != null) Tree.OnNodeChanged(this);
		}
	}
	[ListBindable(false)]
	public class GridLevelNodeCollection : CollectionBase, IEnumerable<GridLevelNode> {
		Hashtable names = new Hashtable();
		GridLevelNode ownerNode = null;
		public GridLevelNodeCollection(GridLevelNode ownerNode) {
			this.ownerNode = ownerNode;
		}
		protected Hashtable Names { get { return names; } }
		protected internal GridLevelTree Owner { get { return OwnerNode == null ? null : OwnerNode.Tree; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridLevelNodeCollectionOwnerNode")]
#endif
		public GridLevelNode OwnerNode { get { return ownerNode; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridLevelNodeCollectionItem")]
#endif
		public GridLevelNode this[int index] { get { return List[index] as GridLevelNode; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridLevelNodeCollectionItem")]
#endif
		public GridLevelNode this[string relationName] { 
			get {
				if(relationName == null) return null;
				return Names[relationName] as GridLevelNode; 
			} 
		}
		public void AddRange(GridLevelNode[] nodes) {
			foreach(GridLevelNode node in nodes) Add(node);
		}
		public void Add(GridLevelNode node) {
			if(List.Contains(node) || this[node.RelationName] != null) return;
			List.Add(node);
		}
		public GridLevelNode Add(string relationName, BaseView template) {
			GridLevelNode node = this[relationName];
			if(node == null) {
				node = new GridLevelNode(Owner, relationName, template);
				List.Add(node);
			}
			else node.LevelTemplate = template;
			return node;
		}
		public int IndexOf(GridLevelNode node) { return List.IndexOf(node); }
		public void Remove(GridLevelNode node) {
			if(List.Contains(node)) List.Remove(node);
		}
		public GridLevelNode Insert(int position, string relationName, BaseView template) {
			GridLevelNode node = this[relationName];
			if(node == null) {
				node = new GridLevelNode(Owner, relationName, template);
				if(position > Count) position = Count;
				List.Insert(position, node);
			}
			else node.LevelTemplate = template;
			return node;
		}
		protected override void OnInsertComplete(int index, object item) {
			GridLevelNode node = item as GridLevelNode;
			node.SetOwner(this);
			Names[node.RelationName] = node;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, node));
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n --) RemoveAt(n);
			Names.Clear();
		}
		protected override void OnRemoveComplete(int index, object item) {
			GridLevelNode node = item as GridLevelNode;
			node.Dispose();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, node));
			if(Names.ContainsKey(node.RelationName))
				Names.Remove(node.RelationName);
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(Owner != null) Owner.OnChanged(this, e);
		}
		internal void Rename(GridLevelNode node, string oldName, string newName) {
			Names.Remove(oldName);
			Names[newName] = node;
		}
		public bool Contains(string relationName) { return this[relationName] != null; }
		IEnumerator<GridLevelNode> IEnumerable<GridLevelNode>.GetEnumerator() {
			foreach(GridLevelNode levelNode in InnerList)
				yield return levelNode;
		}
	}
}
