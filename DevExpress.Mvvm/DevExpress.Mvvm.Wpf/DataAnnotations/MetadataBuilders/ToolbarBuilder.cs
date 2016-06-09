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
using System.Linq.Expressions;
using System.Windows.Input;
namespace DevExpress.Mvvm.DataAnnotations {
	public class ToolBarLayoutBuilder<T> {
		readonly ClassMetadataBuilder<T> owner;
		internal ToolBarLayoutBuilder(ClassMetadataBuilder<T> owner) {
			this.owner = owner;
		}
		public ToolBarCategoryLayoutBuilder<T> DefaultCategory() {
			return new ToolBarCategoryLayoutBuilder<T>(owner);
		}
	}
	public class ToolBarCategoryLayoutBuilder<T> {
		readonly ClassMetadataBuilder<T> owner;
		internal ToolBarCategoryLayoutBuilder(ClassMetadataBuilder<T> owner) {
			this.owner = owner;
		}
		public ToolBarPageLayoutBuilder<T> Page(string pageName) {
			return new ToolBarPageLayoutBuilder<T>(owner, this, pageName);
		}
	}
	public class ToolBarPageLayoutBuilder<T> {
		readonly ClassMetadataBuilder<T> owner;
		readonly ToolBarCategoryLayoutBuilder<T> parent;
		readonly string pageName;
		internal ToolBarPageLayoutBuilder(ClassMetadataBuilder<T> owner, ToolBarCategoryLayoutBuilder<T> parent, string pageName) {
			this.owner = owner;
			this.parent = parent;
			this.pageName = pageName;
		}
		public ToolBarPageGroupLayoutBuilder<T> PageGroup(string pageGroupName = null) {
			return new ToolBarPageGroupLayoutBuilder<T>(owner, this, pageName, pageGroupName);
		}
		public ToolBarCategoryLayoutBuilder<T> EndPage() {
			return parent;
		}
	}
	public abstract class CommandGroupLayoutBuilderBase<T, TBuilder> where TBuilder : CommandGroupLayoutBuilderBase<T, TBuilder> {
		protected readonly ClassMetadataBuilder<T> owner;
		internal CommandGroupLayoutBuilderBase(ClassMetadataBuilder<T> owner) {
			this.owner = owner;
		}
		public TBuilder ContainsCommand(Expression<Func<T, ICommand>> propertyExpression) {
			return ContainsCommandCore(owner.CommandCore(propertyExpression));
		}
		public TBuilder ContainsCommand(Expression<Action<T>> methodExpression) {
			return ContainsCommandCore(owner.CommandFromMethodInternal(methodExpression));
		}
		protected abstract TBuilder ContainsCommandCore<TCommandBuilder>(CommandMetadataBuilderBase<T, TCommandBuilder> commandBuilder)
			where TCommandBuilder : CommandMetadataBuilderBase<T, TCommandBuilder>;
	}
	public class ToolBarPageGroupLayoutBuilder<T> : CommandGroupLayoutBuilderBase<T, ToolBarPageGroupLayoutBuilder<T>> {
		readonly ToolBarPageLayoutBuilder<T> parent;
		readonly string pageName;
		readonly string pageGroupName;
		internal ToolBarPageGroupLayoutBuilder(ClassMetadataBuilder<T> owner, ToolBarPageLayoutBuilder<T> parent, string pageName, string pageGroupName)
			: base(owner) {
			this.parent = parent;
			this.pageName = pageName;
			this.pageGroupName = pageGroupName;
		}
		protected override ToolBarPageGroupLayoutBuilder<T> ContainsCommandCore<TCommandBuilder>(CommandMetadataBuilderBase<T, TCommandBuilder> commandBuilder) {
			commandBuilder.AddOrModifyAttribute<ToolBarItemAttribute>(x => {
				x.Page = pageName;
				x.PageGroup = pageGroupName;
				x.Order = owner.CurrentToolbarLayoutOrder++;
			});
			return this;
		}
		public ToolBarPageLayoutBuilder<T> EndPageGroup() {
			return parent;
		}
	}
	public class ContextMenuLayoutBuilder<T> {
		readonly ClassMetadataBuilder<T> owner;
		internal ContextMenuLayoutBuilder(ClassMetadataBuilder<T> owner) {
			this.owner = owner;
		}
		public ContextMenuGroupLayoutBuilder<T> Group(string pageGroupName = null) {
			return new ContextMenuGroupLayoutBuilder<T>(owner, this, pageGroupName);
		}
	}
	public class ContextMenuGroupLayoutBuilder<T> : CommandGroupLayoutBuilderBase<T, ContextMenuGroupLayoutBuilder<T>> {
		readonly ContextMenuLayoutBuilder<T> parent;
		readonly string pageGroupName;
		internal ContextMenuGroupLayoutBuilder(ClassMetadataBuilder<T> owner, ContextMenuLayoutBuilder<T> parent, string pageGroupName)
			: base(owner) {
			this.parent = parent;
			this.pageGroupName = pageGroupName;
		}
		protected override ContextMenuGroupLayoutBuilder<T> ContainsCommandCore<TCommandBuilder>(CommandMetadataBuilderBase<T, TCommandBuilder> commandBuilder) {
			commandBuilder.AddOrModifyAttribute<ContextMenuItemAttribute>(x => {
				x.Group = pageGroupName;
				x.Order = owner.CurrentContextMenuLayoutOrder++;
			});
			return this;
		}
		public ContextMenuLayoutBuilder<T> EndGroup() {
			return parent;
		}
	}
}
