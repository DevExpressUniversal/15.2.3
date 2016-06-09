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

using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.Customization {
	public interface ICustomizationTreeItem : IEnumerableTreeNode<ICustomizationTreeItem> {
		IEnumerable<ICustomizationTreeItem> EnumerateChildren();
		int Level { get; }
		bool CanExpand { get; }
		bool IsExpanded { get; set; }
		bool IsVisible { get; }
		string Name { get; }
		string Caption { get; }
		PivotFieldItemBase Field { get; }
		event EventHandler<TreeNodeExpandedChangedEventArgs> ExpandedChanged;
	}
	public abstract class PivotCustomizationTreeNodeBase : IEnumerable<PivotCustomizationTreeNodeBase>, IEnumerableTreeNode<PivotCustomizationTreeNodeBase>, ICustomizationTreeItem {
		readonly PivotFieldItemBase field;
		readonly string name;
		readonly string caption;
		int level;
		bool isExpanded;
		bool isVisible;
		PivotCustomizationTreeNodeBase parent;
		List<PivotCustomizationTreeNodeBase> children = new List<PivotCustomizationTreeNodeBase>();
		public event EventHandler<TreeNodeExpandedChangedEventArgs> ExpandedChanged;
		protected PivotCustomizationTreeNodeBase() { }
		public PivotCustomizationTreeNodeBase(PivotFieldItemBase field)
			: this(field, null) {
		}
		public PivotCustomizationTreeNodeBase(PivotFieldItemBase field, string caption) {
			this.field = field;
			this.name = GetName(field);
			this.caption = string.IsNullOrEmpty(caption) ? GetCaption() : caption;
		}
		public PivotCustomizationTreeNodeBase(string name, string caption) {
			this.name = name;
			this.caption = caption;
		}
		public PivotFieldItemBase Field { get { return field; } }
		protected PivotGroupItem Group { get { return Field != null ? Field.Group : null; } }
		protected bool HasGroup { get { return Group != null; } }
		public string Name { get { return name; } }
		public string Caption { get { return caption; } }
		public virtual int Level { get { return level; } }
		public virtual int GroupPosition { get { return int.MaxValue; } }
		public virtual bool CanExpand { get { return false; } }
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if(isExpanded == value) return;
				isExpanded = value;
				if(IsVisible)
					OnExpandCollapse();
				ExpandedChanged.SafeRaise(this, new TreeNodeExpandedChangedEventArgs(this));
			}
		}
		public bool IsVisible { get { return isVisible; } set { isVisible = value; } }
		public PivotCustomizationTreeNodeBase Parent {
			get { return parent; }
			protected set {
				if(parent == value) return;
				parent = value;
				this.level = parent.Level + 1;
				this.isVisible = Level == 0;
			}
		}
		public PivotCustomizationTreeNodeBase FirstChild { get { return Children.Count == 0 ? null : Children[0]; } }
		public PivotCustomizationTreeNodeBase NextSibling {
			get {
				if(Parent == null) return null;
				int index = Parent.Children.IndexOf(this);
				while(index < Parent.Children.Count - 1 && Parent.Children.IndexOf(Parent.Children[index + 1], index) == index)
					index++;
				return index < Parent.Children.Count - 1 ? Parent.Children[index + 1] : null;
			}
		}
		List<PivotCustomizationTreeNodeBase> Children { get { return children; } }
		public void AddChild(PivotCustomizationTreeNodeBase node) {
			node.Parent = this;
			Children.Add(node);
		}
		public PivotCustomizationTreeNodeBase FindChild(string childName, PivotFieldItemBase item) {
			return children.Find(node => node.Name == childName && node.Field == item);
		}
		public List<PivotCustomizationTreeNodeBase> GetBranch() {
			List<PivotCustomizationTreeNodeBase> branch = new List<PivotCustomizationTreeNodeBase>();
			PivotCustomizationTreeNodeBase node = this;
			do {
				branch.Insert(0, node);
				node = node.Parent;
			} while(node != null);
			return branch;
		}
		protected static string Clean(string str) {
			if(string.IsNullOrEmpty(str) || str.IndexOf('[') != 0 || str.LastIndexOf(']') <= 1) return null;
			return str.Substring(1, str.Length - 2);
		}
		protected virtual string GetName(PivotFieldItemBase field) {
			return field.UniqueName;
		}
		protected virtual string GetCaption() {
			return HasGroup ? Group.ToString() : Field.HeaderDisplayText;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption))
				return Caption;
			if(Field != null)
				return Field.ToString();
			return string.Empty;
		}
		public void Sort() {
			this.children = Children.OrderBy(node => node.GroupPosition).ThenBy(node => node.Caption).ToList(); 
			foreach(PivotCustomizationTreeNodeBase child in Children)
				child.Sort();
		}
		protected void ForEachChild(Action<PivotCustomizationTreeNodeBase> action, bool recursive) {
			foreach(var node in Children) {
				if(recursive)
					node.ForEachChild(action, true);
				action(node);
			}
		}
		protected virtual void OnExpandCollapse() {
			ForEachChild(node => node.IsVisible = IsExpanded, !IsExpanded);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(obj == null) return false;
			PivotCustomizationTreeNodeBase node = obj as PivotCustomizationTreeNodeBase;
			if(node == null) return false;
			return BranchEquals(node);
		}
		bool BranchEquals(PivotCustomizationTreeNodeBase node) {
			PivotCustomizationTreeNodeBase current1 = this;
			PivotCustomizationTreeNodeBase current2 = node;
			for(; ; ) {
				if(current1 == null)
					return current2 == null;
				if(current2 == null)
					return current1 == null;
				bool equal = current1.Name == current2.Name && current1.Field == current2.Field;
				if(!equal)
					return false;
				current1 = current1.Parent;
				current2 = current2.Parent;
			}
		}
		bool BranchEquals(List<PivotCustomizationTreeNodeBase> branch) {
			PivotCustomizationTreeNodeBase current = this;
			for(int i = branch.Count - 1; i >= 1; i--) {
				if(current == null)
					return false;
				if(current.Name != branch[i].Name || current.Field != branch[i].Field)
					return false;
				current = current.Parent;
			}
			return true;
		}
		IEnumerator<PivotCustomizationTreeNodeBase> IEnumerable<PivotCustomizationTreeNodeBase>.GetEnumerator() {
			return new TreeDepthFirstEnumerator<PivotCustomizationTreeNodeBase>(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<PivotCustomizationTreeNodeBase>)this).GetEnumerator();
		}
		IEnumerable<ICustomizationTreeItem> ICustomizationTreeItem.EnumerateChildren() {
			foreach(PivotCustomizationTreeNodeBase item in Children)
				yield return item;
		}
		ICustomizationTreeItem IEnumerableTreeNode<ICustomizationTreeItem>.Parent { get { return Parent; } }
		ICustomizationTreeItem IEnumerableTreeNode<ICustomizationTreeItem>.NextSibling { get { return NextSibling; } }
		ICustomizationTreeItem IEnumerableTreeNode<ICustomizationTreeItem>.FirstChild { get { return FirstChild; } }
	}
	#region Custom tree nodes
	public class PivotCustomizationTreeNodeRootBase : PivotCustomizationTreeNodeBase {
		public PivotCustomizationTreeNodeRootBase() { }
		public override int Level { get { return -1; } }
	}
	public class PivotCustomizationTreeNodeMeasureBase : PivotCustomizationTreeNodeBase {
		public PivotCustomizationTreeNodeMeasureBase(PivotFieldItemBase field)
			: base(field) {
		}
		public override int GroupPosition { get { return 1; } }
	}
	public class PivotCustomizationTreeNodeDimensionBase : PivotCustomizationTreeNodeBase {
		bool isMeasuresFolder;
		bool isKPIsFolder;
		public PivotCustomizationTreeNodeDimensionBase(string name, string caption)
			: base(name, string.IsNullOrEmpty(caption) ? Clean(name) : caption) {
		}
		public PivotCustomizationTreeNodeDimensionBase(string name, bool isMeasuresFolder, bool isKPIsFolder, string caption)
			: base(name, string.IsNullOrEmpty(caption) ? Clean(name) : caption) {
			this.isMeasuresFolder = isMeasuresFolder;
			this.isKPIsFolder = isKPIsFolder;
		}
		public bool IsMeasuresFolder { get { return isMeasuresFolder; } }
		public bool IsKPIFolder { get { return isKPIsFolder; } }
		public override bool CanExpand { get { return true; } }
		public override int GroupPosition {
			get {
				if(IsMeasuresFolder) return 0;
				return IsKPIFolder ? 1 : 2;
			}
		}
	}
	public class PivotCustomizationTreeNodeAttributeBase : PivotCustomizationTreeNodeBase {
		public PivotCustomizationTreeNodeAttributeBase(PivotFieldItemBase field)
			: base(field) {
		}
		public override int GroupPosition { get { return 1; } }
	}
	public class PivotCustomizationTreeNodeHierarchyBase : PivotCustomizationTreeNodeBase {
		public PivotCustomizationTreeNodeHierarchyBase(PivotFieldItemBase field)
			: base(field) {
		}
		protected override string GetName(PivotFieldItemBase field) {
			string name = base.GetName(field);
			int lastDotIndex = name.LastIndexOf('.');
			return lastDotIndex != -1 ? name.Substring(0, lastDotIndex) : name;
		}
		public override int GroupPosition { get { return 2; } }
	}
	public class PivotCustomizationTreeNodeKpiBase : PivotCustomizationTreeNodeBase {
		public PivotCustomizationTreeNodeKpiBase(PivotFieldItemBase field)
			: base(field, GetCaption(field.KPIType)) {
		}
		internal static string GetCaption(PivotKPIType pivotKPIType) {
			switch (pivotKPIType) {
				case PivotKPIType.Goal:
					return PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeGoal);
				case PivotKPIType.Status:
					return PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeStatus);
				case PivotKPIType.Trend:
					return PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeTrend);
				case PivotKPIType.Value:
					return PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeValue);
				case PivotKPIType.Weight:
					return PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeWeight);
			}
			return pivotKPIType.ToString();
		}
		public override int GroupPosition {
			get {
				switch(Field.KPIType) {
					case PivotKPIType.Value: return 0;
					case PivotKPIType.Goal: return 1;
					case PivotKPIType.Status: return 2;
					case PivotKPIType.Trend: return 3;
					case PivotKPIType.Weight: return 4;
					default: return int.MaxValue;
				}
			}
		}
	}
	public class PivotCustomizationTreeNodeFolderBase : PivotCustomizationTreeNodeBase {
		public PivotCustomizationTreeNodeFolderBase(string folderName)
			: base(folderName, folderName) {
		}
		public override bool CanExpand { get { return true; } }
		public override int GroupPosition { get { return 0; } }
	}
	#endregion
	public class TreeNodeExpandedChangedEventArgs : EventArgs {
		public TreeNodeExpandedChangedEventArgs(ICustomizationTreeItem node) {
			IsExpanded = node.IsExpanded;
		}
		public bool IsExpanded { get; private set; }
	}
}
