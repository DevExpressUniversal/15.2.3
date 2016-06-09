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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class GridViewLayoutItemCollection : GridLayoutItemCollection {
		public GridViewLayoutItemCollection() : base() { }
		public GridViewLayoutItemCollection(IWebControlObject owner) : base(owner) { }
		public GridViewColumnLayoutItem AddColumnItem(GridViewColumnLayoutItem layoutItem) {
			return (GridViewColumnLayoutItem)base.Add(layoutItem);
		}
		public EditModeCommandLayoutItem AddCommandItem(EditModeCommandLayoutItem commandItem) {
			return (EditModeCommandLayoutItem)base.Add(commandItem);
		}
		public GridViewLayoutGroup AddGroup(GridViewLayoutGroup layoutGroup) {
			return (GridViewLayoutGroup)base.Add(layoutGroup);
		}
		public GridViewTabbedLayoutGroup AddTabbedGroup(GridViewTabbedLayoutGroup tabbedGroup) {
			return (GridViewTabbedLayoutGroup)base.Add(tabbedGroup);
		}
		public GridViewColumnLayoutItem AddColumnItem(string columnName) {
			return AddColumnItem(columnName, null);
		}
		public GridViewLayoutGroup AddGroup(string caption) {
			return base.Add<GridViewLayoutGroup>(caption);
		}
		public GridViewTabbedLayoutGroup AddTabbedGroup(string caption) {
			return base.Add<GridViewTabbedLayoutGroup>(caption);
		}
		public GridViewColumnLayoutItem AddColumnItem(string columnName, string caption) {
			return (GridViewColumnLayoutItem)AddColumnItem(new GridViewColumnLayoutItem(), columnName, caption);
		}
		public GridViewLayoutGroup AddGroup(string caption, string name) {
			return base.Add<GridViewLayoutGroup>(caption, name);
		}
		public GridViewTabbedLayoutGroup AddTabbedGroup(string caption, string name) {
			return base.Add<GridViewTabbedLayoutGroup>(caption, name);
		}
		protected override Type[] GetKnownTypes() {
			return new Type[] {
				typeof(GridViewColumnLayoutItem),
				typeof(EditModeCommandLayoutItem),
				typeof(GridViewLayoutGroup),
				typeof(GridViewTabbedLayoutGroup),
				typeof(EmptyLayoutItem)
			};
		}
	}
	public class GridViewColumnLayoutItem : ColumnLayoutItem {
		public GridViewColumnLayoutItem()
			: base() {
		}
		protected internal GridViewColumnLayoutItem(IWebGridColumn column)
			: base(column) {
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewEditFormLayoutItemTemplateContainer), BindingDirection.TwoWay), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override ITemplate Template {
			get { return base.Template; }
			set { base.Template = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridViewColumn Column {
			get { return ColumnInternal as GridViewColumn; }
		}
		protected override string GetColumnCaption() {
			return Column.ToString();
		}
		protected override bool IsTemplateReplacement(Control control) {
			return control is ASPxGridViewTemplateReplacement;
		}
	}
	public class GridViewLayoutGroup : GridLayoutGroup {
		public GridViewLayoutGroup() : base() { }
		public GridViewLayoutGroup(string caption) : base(caption) { }
		protected internal GridViewLayoutGroup(FormLayoutProperties owner) : base(owner) { }
		protected override LayoutItemCollection CreateItems() {
			return new GridViewLayoutItemCollection(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewLayoutGroupItems"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), NotifyParentProperty(true),
		AutoFormatEnable, Themeable(true), Browsable(false)]
		public new GridViewLayoutItemCollection Items {
			get { return (GridViewLayoutItemCollection)base.Items; }
		}
	}
	public class GridViewTabbedLayoutGroup : GridTabbedLayoutGroup {
		public GridViewTabbedLayoutGroup() : base() { }
		public GridViewTabbedLayoutGroup(string caption) : base(caption) { }
		protected override LayoutItemCollection CreateItems() {
			return new GridViewLayoutItemCollection(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewTabbedLayoutGroupItems"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), NotifyParentProperty(true),
		AutoFormatEnable, Themeable(true), Browsable(false)]
		public new GridViewLayoutItemCollection Items {
			get { return (GridViewLayoutItemCollection)base.Items; }
		}
	}
	public class GridViewFormLayoutProperties : GridFormLayoutProperties {
		public GridViewFormLayoutProperties(IPropertiesOwner owner) : base(owner) { }
		public GridViewFormLayoutProperties() : this(null) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFormLayoutPropertiesItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public new GridViewLayoutItemCollection Items {
			get { return (GridViewLayoutItemCollection)base.Items; }
		}
		protected override LayoutGroup CreateRootGroup() {
			return new GridViewLayoutGroup(this);
		}
		protected override string[] GetColumnNames() {
			var grid = DataOwner as ASPxGridView;
			var result = new List<string>();
			if(grid != null) {
				foreach(var column in grid.Columns) {
					if(!(column is GridViewBandColumn))
						result.Add(column.ToString());
				}
			}
			return result.ToArray();
		}
	}
}
