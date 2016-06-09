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

using DevExpress.Mvvm.Native;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Windows.Controls;
namespace DevExpress.Mvvm.DataAnnotations {
	public class DataFormLayoutBuilder<T, TParentBuilder> : LayoutBuilderBase<T, DataFormLayoutBuilder<T, TParentBuilder>>
		where TParentBuilder : ClassMetadataBuilder<T> {
		internal override LayoutType LayoutType { get { return LayoutType.DataForm; } }
		readonly DataFormLayoutBuilder<T, TParentBuilder> parent;
		internal DataFormLayoutBuilder(ClassMetadataBuilder<T> owner)
			: base(owner) {
		}
		internal DataFormLayoutBuilder(string groupName, ClassMetadataBuilder<T> owner, DataFormLayoutBuilder<T, TParentBuilder> parent, GroupView groupView, Orientation? orientation, string start)
			: base(groupName, owner, groupView, orientation, start) {
			this.parent = parent;
		}
		public DataFormLayoutBuilder<T, TParentBuilder> ContainsProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			return ContainsPropertyCore(propertyExpression);
		}
		public DataFormLayoutBuilder<T, TParentBuilder> TabbedGroup(string groupName) {
			return new DataFormLayoutBuilder<T, TParentBuilder>(groupName, owner, this, GroupView.Tabs, null, GroupPathStart);
		}
		public DataFormLayoutBuilder<T, TParentBuilder> GroupBox(string groupName, Orientation? orientation = null) {
			return new DataFormLayoutBuilder<T, TParentBuilder>(groupName, owner, this, GroupView.GroupBox, orientation, GroupPathStart);
		}
		public DataFormLayoutBuilder<T, TParentBuilder> Group(string groupName, Orientation? orientation = null) {
			return new DataFormLayoutBuilder<T, TParentBuilder>(groupName, owner, this, GroupView.Group, orientation, GroupPathStart);
		}
		public DataFormLayoutBuilder<T, TParentBuilder> EndGroup() {
			return parent;
		}
	}
	public class GroupBuilder<T, TParentBuilder>
		where TParentBuilder : ClassMetadataBuilder<T> {
		readonly TParentBuilder owner;
		readonly string groupName;
		internal GroupBuilder(TParentBuilder owner, string groupName) {
			this.owner = owner;
			this.groupName = groupName;
		}
		public GroupBuilder<T, TParentBuilder> ContainsProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			owner.PropertyCore(propertyExpression).AddOrModifyAttribute<DisplayAttribute>(x => {
				x.GroupName = groupName;
				x.Order = owner.CurrentDisplayAttributeOrder++;
			});
			return this;
		}
		public TParentBuilder EndGroup() {
			return owner;
		}
	}
	public abstract class LayoutBuilderBase<T, TBuilder> where TBuilder : LayoutBuilderBase<T, TBuilder> {
		protected readonly ClassMetadataBuilder<T> owner;
		readonly bool isRoot;
		readonly string groupName;
		readonly GroupView groupView;
		readonly string start;
		readonly Orientation? orientation;
		protected string GroupPathStart { get { return isRoot ? string.Empty : (start + LayoutGroupInfoConstants.GroupPathSeparator + CurrentLevelPath); } }
		string CurrentLevelPath { get { return GetPrefix() + groupName + GetOrientation() + GetSuffix(); } }
		internal abstract LayoutType LayoutType { get; }
		protected LayoutBuilderBase(ClassMetadataBuilder<T> owner) {
			this.owner = owner;
			this.isRoot = true;
		}
		protected LayoutBuilderBase(string groupName, ClassMetadataBuilder<T> owner, GroupView groupView, Orientation? orientation, string start) {
			this.owner = owner;
			this.groupName = groupName;
			this.groupView = groupView;
			this.start = start;
			this.orientation = orientation;
		}
		protected TBuilder ContainsPropertyCore<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			owner.PropertyCore(propertyExpression).GroupName(GroupPathStart, LayoutType);
			return (TBuilder)this;
		}
		char GetPrefix() {
			switch(groupView) {
				case GroupView.Group:
					return LayoutGroupInfoConstants.BorderlessGroupMarkStart;
				case GroupView.GroupBox:
					return LayoutGroupInfoConstants.GroupBoxMarkStart;
				case GroupView.Tabs:
					return LayoutGroupInfoConstants.TabbedGroupMarkStart;
				default:
					throw new NotSupportedException();
			}
		}
		char? GetOrientation() {
			switch(orientation) {
				case Orientation.Vertical:
					return LayoutGroupInfoConstants.VerticalGroupMark;
				case Orientation.Horizontal:
					return LayoutGroupInfoConstants.HorizontalGroupMark;
				default:
					return null;
			}
		}
		char GetSuffix() {
			switch(groupView) {
				case GroupView.Group:
					return LayoutGroupInfoConstants.BorderlessGroupMarkEnd;
				case GroupView.GroupBox:
					return LayoutGroupInfoConstants.GroupBoxMarkEnd;
				case GroupView.Tabs:
					return LayoutGroupInfoConstants.TabbedGroupMarkEnd;
				default:
					throw new NotSupportedException();
			}
		}
	}
	public class TableGroupContainerLayoutBuilder<T> : LayoutBuilderBase<T, TableGroupContainerLayoutBuilder<T>> {
		internal override LayoutType LayoutType { get { return LayoutType.Table; } }
		readonly TableGroupContainerLayoutBuilder<T> parent;
		internal TableGroupContainerLayoutBuilder(ClassMetadataBuilder<T> owner)
			: base(owner) {
		}
		internal TableGroupContainerLayoutBuilder(string groupName, ClassMetadataBuilder<T> owner, TableGroupContainerLayoutBuilder<T> parent, string start)
			: base(groupName, owner, GroupView.Group, null, start) {
			this.parent = parent;
		}
		public TableGroupContainerLayoutBuilder<T> GroupContainer(string groupName) {
			return new TableGroupContainerLayoutBuilder<T>(groupName, owner, this, GroupPathStart);
		}
		public TableGroupLayoutBuilder<T> Group(string groupName) {
			return new TableGroupLayoutBuilder<T>(groupName, owner, this, GroupPathStart);
		}
		public TableGroupContainerLayoutBuilder<T> EndGroupContainer() {
			return parent;
		}
	}
	public class TableGroupLayoutBuilder<T> : LayoutBuilderBase<T, TableGroupLayoutBuilder<T>> {
		internal override LayoutType LayoutType { get { return LayoutType.Table; } }
		readonly TableGroupContainerLayoutBuilder<T> parent;
		internal TableGroupLayoutBuilder(string groupName, ClassMetadataBuilder<T> owner, TableGroupContainerLayoutBuilder<T> parent, string start)
			: base(groupName, owner, GroupView.Group, null, start) {
			this.parent = parent;
		}
		public TableGroupLayoutBuilder<T> ContainsProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			return ContainsPropertyCore(propertyExpression);
		}
		public TableGroupContainerLayoutBuilder<T> EndGroup() {
			return parent;
		}
	}
}
