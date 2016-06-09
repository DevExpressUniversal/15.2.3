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

using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public enum ToolbarItemPickerImagePosition {
		Left,
		Top
	}
	public abstract class ToolbarCustomItem : ToolbarItemBase {
		bool synchronizeValue = true;
		bool synchronizeText = true;
		public ToolbarCustomItem() {
			synchronizeText = true;
			synchronizeValue = true;
		}
		public ToolbarCustomItem(string value, string text)
			: this(value, text, string.Empty, string.Empty) {
		}
		public ToolbarCustomItem(string value, string text, string imageUrl)
			: this(value, text, imageUrl, string.Empty) {
		}
		public ToolbarCustomItem(string value, string text, string imageUrl, string toolTip)
			: base(text, toolTip) {
			Image.Url = imageUrl;
			Value = value;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomItemValue"),
#endif
		DefaultValue(""), RefreshProperties(RefreshProperties.All)]
		public virtual string Value {
			get { return GetStringProperty("Value", string.Empty); }
			set {
				synchronizeValue = false;
				SetStringProperty("Value", string.Empty, value);
				if(SynchronizeText && string.IsNullOrEmpty(Text))
					Text = value;
				PropertyChanged();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomItemText"),
#endif
		RefreshProperties(RefreshProperties.All)]
		public override string Text
		{
			get { return base.Text; }
			set
			{
				synchronizeText = false;
				base.Text = value;
				if (SynchronizeValue && string.IsNullOrEmpty(Value))
					Value = value;
				PropertyChanged();
				LayoutChanged();
			}
		}
		protected internal override string GetCommandName() {
			return Value;
		}
		protected void PropertyChanged() {
			if(Collection != null) {
				ASPxHtmlEditor editor = Collection.Owner as ASPxHtmlEditor;
				if(editor != null)
					editor.PropertyChanged("Toolbars");
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarCustomItem sourceItem = source as ToolbarCustomItem;
			if(sourceItem != null) {
				Value = sourceItem.Value;
			}
		}
		public override string ToString() {
			return string.IsNullOrEmpty(Text) ? base.ToString() : Text;
		}
		protected bool SynchronizeValue { get { return synchronizeValue && IsDesignMode(); } }
		protected bool SynchronizeText { get { return synchronizeText && IsDesignMode(); } }
	}
	public class ToolbarItemPickerItem : ToolbarCustomItem {
		public ToolbarItemPickerItem() : base() { }
		public ToolbarItemPickerItem(string value, string text) : base(value, text) { }
		public ToolbarItemPickerItem(string value, string text, string imageUrl) : base(value, text, imageUrl) { }
		public ToolbarItemPickerItem(string value, string text, string imageUrl, string toolTip) : base(value, text, imageUrl, toolTip) { }
	}
	public class ToolbarMenuItem : ToolbarCustomItem {
		public ToolbarMenuItem() : base() { }
		public ToolbarMenuItem(string value, string text) : base(value, text) { }
		public ToolbarMenuItem(string value, string text, string imageUrl) : base(value, text, imageUrl) { }
		public ToolbarMenuItem(string value, string text, string imageUrl, string toolTip) : base(value, text, imageUrl, toolTip) { }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMenuItemBeginGroup"),
#endif
		DefaultValue(false)]
		public bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", false); }
			set { SetBoolProperty("BeginGroup", false, value); }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarMenuItem sourceItem = source as ToolbarMenuItem;
			if(sourceItem != null)
				BeginGroup = sourceItem.BeginGroup;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class ToolbarItemPickerItemCollection : ToolbarItemCollectionBase<ToolbarItemPickerItem> {
		public ToolbarItemPickerItemCollection() : base() { }
		public ToolbarItemPickerItemCollection(IWebControlObject owner) : base(owner) { }
		public ToolbarItemPickerItem Add() {
			return Add(string.Empty, string.Empty);
		}
		public ToolbarItemPickerItem Add(string value, string text) {
			return Add(value, text, "");
		}
		public ToolbarItemPickerItem Add(string value, string text, string imageUrl) {
			return AddInternal(new ToolbarItemPickerItem(value, text, imageUrl));
		}
		public ToolbarItemPickerItem Add(string value, string text, string imageUrl, string toolTip) {
			return AddInternal(new ToolbarItemPickerItem(value, text, imageUrl, toolTip));
		}
		protected internal IEnumerable<string> GetValuesArray() {
			foreach(ToolbarItemPickerItem item in this)
				yield return item.Value;
		}
	}
	public class ToolbarMenuItemCollection : ToolbarItemCollectionBase<ToolbarMenuItem> {
		public ToolbarMenuItemCollection() { }
		public ToolbarMenuItemCollection(IWebControlObject owner) : base(owner) { }
		public ToolbarMenuItem Add() {
			return Add(string.Empty, string.Empty);
		}
		public ToolbarMenuItem Add(string value, string text) {
			return Add(value, text, "");
		}
		public ToolbarMenuItem Add(string value, string text, string imageUrl) {
			return AddInternal(new ToolbarMenuItem(value, text, imageUrl));
		}
		public ToolbarMenuItem Add(string value, string text, string imageUrl, string toolTip) {
			return AddInternal(new ToolbarMenuItem(value, text, imageUrl, toolTip));
		}
	}
}
