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

using DevExpress.Web;
using System;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	public enum MVCxRibbonItemType {
		TextBoxItem, SpinEditItem, DateEditItem, ComboBoxItem, CheckBoxItem, ButtonItem, DropDownButtonItem,
		ToggleButtonItem, OptionButtonItem, ColorButtonItem, TemplateItem, DropDownToggleButtonItem,
		GalleryDropDownItem, GalleryBarItem
	};
	public class MVCxRibbonTab: RibbonTab {
		public MVCxRibbonTab() {
		}
		public MVCxRibbonTab(string text)
			: base(text) {
		}
		public new MVCxRibbonGroupCollection Groups { get { return (MVCxRibbonGroupCollection)base.Groups; } }
		protected override RibbonGroupCollection CreateRibbonGroupCollection() {
			return new MVCxRibbonGroupCollection(this);
		}
	}
	public class MVCxRibbonContextTabCategory : RibbonContextTabCategory {
		public MVCxRibbonContextTabCategory() {
		}
		public MVCxRibbonContextTabCategory(string name)
			: base(name) {
		}
		public new MVCxRibbonTabCollection Tabs { get { return (MVCxRibbonTabCollection)base.Tabs; } }
		protected override RibbonTabCollection CreateRibbonTabCollection() {
			return new MVCxRibbonTabCollection(this, true);
		}
	}
	public class MVCxRibbonGroup: RibbonGroup {
		public MVCxRibbonGroup()
			: base() {
		}
		public MVCxRibbonGroup(string text)
			: base(text) {
		}
		public MVCxRibbonGroup(string text, string name)
			: base(text, name) {
		}
		public new MVCxRibbonItemCollection Items { get { return (MVCxRibbonItemCollection)base.Items; } }
		protected override RibbonItemCollection CreateRibbonItemCollection() {
			return new MVCxRibbonItemCollection(this);
		}
	}
	public class MVCxRibbonTemplateItem: RibbonTemplateItem {
		public MVCxRibbonTemplateItem() {
		}
		public MVCxRibbonTemplateItem(string name)
			: base(name) {
		}
		protected internal string Content { get; set; }
		protected internal Action<RibbonTemplateItemControl> ContentMethod { get; set; }
		public void SetContent(Action<RibbonTemplateItemControl> contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var src = source as MVCxRibbonTemplateItem;
			if(src != null) {
				Content = src.Content;
				ContentMethod = src.ContentMethod;
			}
		}
	}
	public class MVCxRibbonTabCollection: RibbonTabCollection {
		public MVCxRibbonTabCollection(IWebControlObject owner, bool isContext = false)
			: base(owner, isContext) {
		}
		public MVCxRibbonTabCollection(bool isContext = false)
			: base(null, isContext) {
		}
		public new MVCxRibbonTab Add(string text) {
			var tab = new MVCxRibbonTab(text);
			Add(tab);
			return tab;
		}
	}
	public class MVCxRibbonContextTabCategoryCollection : RibbonContextTabCategoryCollection {
		public MVCxRibbonContextTabCategoryCollection()
			: base(null) {
		}
		public new MVCxRibbonContextTabCategory Add(string name) {
			var category = new MVCxRibbonContextTabCategory(name);
			Add(category);
			return category;
		}
	}
	public class MVCxRibbonGroupCollection: RibbonGroupCollection {
		public MVCxRibbonGroupCollection(IWebControlObject owner)
			: base(owner) {
		}
		public new MVCxRibbonGroup Add() {
			return Add(string.Empty);
		}
		public new MVCxRibbonGroup Add(string text) {
			return Add(g => g.Text = text );
		}
		public MVCxRibbonGroup Add(Action<MVCxRibbonGroup> method) {
			var group = new MVCxRibbonGroup();
			if(method != null)
				method(group);
			Add(group);
			return group;
		}
	}
	public class MVCxRibbonItemCollection: RibbonItemCollection {
		public MVCxRibbonItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonItemBase Add(MVCxRibbonItemType type) {
			return Add(type, string.Empty);
		}
		public RibbonItemBase Add(MVCxRibbonItemType type, string name) {
			return Add(type, name, string.Empty);
		}
		public RibbonItemBase Add(MVCxRibbonItemType type, string name, string text) {
			return Add(type, i => { i.Name = name; i.Text = text; });
		}
		public RibbonItemBase Add(MVCxRibbonItemType type, Action<RibbonItemBase> method) {
			var item = CreateItemByType(type);
			if(method != null)
				method(item);
			Add(item);
			return item;
		}
		protected RibbonItemBase CreateItemByType(MVCxRibbonItemType type) {
			switch(type) {
				case MVCxRibbonItemType.TextBoxItem:
					return new RibbonTextBoxItem();
				case MVCxRibbonItemType.SpinEditItem:
					return new RibbonSpinEditItem();
				case MVCxRibbonItemType.DateEditItem:
					return new RibbonDateEditItem();
				case MVCxRibbonItemType.ComboBoxItem:
					return new RibbonComboBoxItem();
				case MVCxRibbonItemType.CheckBoxItem:
					return new RibbonCheckBoxItem();
				case MVCxRibbonItemType.ButtonItem:
					return new RibbonButtonItem();
				case MVCxRibbonItemType.DropDownButtonItem:
					return new RibbonDropDownButtonItem();
				case MVCxRibbonItemType.ToggleButtonItem:
					return new RibbonToggleButtonItem();
				case MVCxRibbonItemType.OptionButtonItem:
					return new RibbonOptionButtonItem();
				case MVCxRibbonItemType.ColorButtonItem:
					return new RibbonColorButtonItem();
				case MVCxRibbonItemType.TemplateItem:
					return new MVCxRibbonTemplateItem();
				case MVCxRibbonItemType.DropDownToggleButtonItem:
					return new RibbonDropDownToggleButtonItem();
				case MVCxRibbonItemType.GalleryDropDownItem:
					return new RibbonGalleryDropDownItem();
				case MVCxRibbonItemType.GalleryBarItem:
					return new RibbonGalleryBarItem();
			}
			return null;
		}
	}
}
