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

using System.ComponentModel;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	using System;
	using DevExpress.Web;
	using DevExpress.Web.ASPxHtmlEditor;
	public interface IHERibbonItem {
		string ToolTip { get; }
		string DefaultToolTip {get;}
		string DefaultText { get; }
		bool ShowText { get; }
		string GetToolTip();
	}
	public static class RibbonItemHelper {
		public static ASPxHtmlEditor GetHtmlEditor(this RibbonItemBase item) {
			if(item.Ribbon is HtmlEditorRibbon)
				return ((HtmlEditorRibbon)item.Ribbon).HtmlEditor;
			return null;
		}
		public static string MergeToolTips(this IHERibbonItem item) {
			if(!string.IsNullOrEmpty(item.ToolTip))
				return item.ToolTip;
			return string.IsNullOrEmpty(item.DefaultToolTip) ? item.DefaultText : item.DefaultToolTip;
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor {
	using DevExpress.Utils;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	public class HERibbonTabBase : RibbonTab {
		public HERibbonTabBase() { }
		public HERibbonTabBase(string text) {
			Text = text;
		}
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return string.Format("he{0}", DefaultName);
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(DefaultName))
				return string.Format("{0} ({1})", base.ToString(), GetName());
			return base.ToString();
		}
	}
	public class HERibbonGroupBase : RibbonGroup {
		public HERibbonGroupBase() { }
		public HERibbonGroupBase(string text) {
			Text = text;
		}
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		protected virtual string ResourceImageName {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return DefaultName;
		}
		protected override ItemImagePropertiesBase GetImage() {
			if(!base.GetImage().IsEmpty)
				return base.GetImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, string.Empty);
			properties.CopyFrom(Image);
			return properties;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(DefaultName))
				return string.Format("{0} ({1})", base.ToString(), DefaultName);
			return base.ToString();
		}
	}
	public class HERibbonToggleCommandBase : RibbonToggleButtonItem, IHERibbonItem {
		public HERibbonToggleCommandBase()
			: base() { }
		public HERibbonToggleCommandBase(string text)
			: base(string.Empty, text) { }
		public HERibbonToggleCommandBase(RibbonItemSize size)
			: base(string.Empty, size) { }
		public HERibbonToggleCommandBase(string text, RibbonItemSize size)
			: base(string.Empty, text, size) { }
		protected virtual string CommandID {
			get { return string.Empty; }
		}
		protected virtual string DefaultGroupName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string DefaultToolTip {
			get { return DefaultText; }
		}
		protected virtual string ResourceImageName {
			get { return string.Empty; }
		}
		protected virtual bool ShowText {
			get { return true; }
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return CommandID;
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			ASPxHtmlEditor HtmlEditor = this.GetHtmlEditor();
			if(HtmlEditor != null)
				return HtmlEditor.AddShortcutToTooltip(this.MergeToolTips(), GetName());
			return this.MergeToolTips(); 
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, string.Empty);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, "Large");
			properties.CopyFrom(LargeImage);
			return properties;
		}
		string IHERibbonItem.GetToolTip() {
			return GetToolTip();
		}
		string IHERibbonItem.DefaultText {
			get { return DefaultText; }
		}
		string IHERibbonItem.DefaultToolTip {
			get { return DefaultToolTip; }
		}
		bool IHERibbonItem.ShowText {
			get { return ShowText; }
		}
	}
	public class HERibbonCommandBase : RibbonButtonItem, IHERibbonItem {
		public HERibbonCommandBase()
			: base() { }
		public HERibbonCommandBase(string text)
			: base(string.Empty, text) { }
		public HERibbonCommandBase(RibbonItemSize size)
			: base(string.Empty, size) { }
		public HERibbonCommandBase(string text, RibbonItemSize size)
			: base(string.Empty, text, size) { }
		protected virtual string CommandID {
			get { return string.Empty; }
		}
		protected virtual string DefaultGroupName {
			get { return string.Empty; }
		}
		protected virtual string Shortcut {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string DefaultToolTip {
			get { return DefaultText; }
		}
		protected virtual string ResourceImageName {
			get { return string.Empty; }
		}
		protected virtual bool ShowText {
			get { return true; }
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return CommandID;
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			ASPxHtmlEditor HtmlEditor = this.GetHtmlEditor();
			if(HtmlEditor != null)
				return HtmlEditor.AddShortcutToTooltip(this.MergeToolTips(), GetName());
			return this.MergeToolTips(); 
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, string.Empty);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, "Large");
			properties.CopyFrom(LargeImage);
			return properties;
		}
		 string IHERibbonItem.GetToolTip() {
			return GetToolTip();
		}
		string IHERibbonItem.DefaultText {
			get { return DefaultText; }
		}
		string IHERibbonItem.DefaultToolTip {
			get { return DefaultToolTip; }
		}
		bool IHERibbonItem.ShowText {
			get { return ShowText; }
		}
	}
	public class HERibbonComboBoxCommandBase : RibbonComboBoxItem, IHERibbonItem {
		public HERibbonComboBoxCommandBase()
			: this(string.Empty) { }
		public HERibbonComboBoxCommandBase(string text)
			: base(text) {
			this.PropertiesComboBox.Width = System.Web.UI.WebControls.Unit.Pixel(DefaultWidth);
			this.PropertiesComboBox.NullText = string.Format("({0})", DefaultToolTip);
			this.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.None;
		}
		protected virtual string CommandID {
			get { return string.Empty; }
		}
		protected virtual string DefaultGroupName {
			get { return string.Empty; }
		}
		protected virtual string DefaultToolTip {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return CommandID;
		}
		protected override string GetToolTip() {
			ASPxHtmlEditor HtmlEditor = this.GetHtmlEditor();
			if(HtmlEditor != null)
				return HtmlEditor.AddShortcutToTooltip(this.MergeToolTips(), GetName());
			return this.MergeToolTips(); 
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected virtual ListEditItemCollection DefaultItems {
			get { return null; }
		}
		protected virtual int DefaultWidth {
			get { return 50; }
		}
		public void CreateDefaultItems(bool clearExistingItems) {
			if(DefaultItems != null) {
				if(clearExistingItems)
					Items.Clear();
				Items.AddRange(DefaultItems);
			}
		}
		string IHERibbonItem.GetToolTip() {
			return GetToolTip();
		}
		string IHERibbonItem.DefaultText {
			get { return string.Empty; }
		}
		string IHERibbonItem.DefaultToolTip {
			get { return DefaultToolTip; }
		}
		bool IHERibbonItem.ShowText {
			get { return false; }
		}
	}
	public class HERibbonColorCommandBase : RibbonColorButtonItemBase, IHERibbonItem {
		DefaultBoolean enableCustomColors = DefaultBoolean.Default;
		bool enableCustomColorsDefaultValue;
		public HERibbonColorCommandBase()
			: base() { }
		public HERibbonColorCommandBase(string text)
			: base(string.Empty, text) { }
		public HERibbonColorCommandBase(RibbonItemSize size)
			: base(string.Empty, size) { }
		public HERibbonColorCommandBase(string text, RibbonItemSize size)
			: base(string.Empty, text, size) { }
		protected virtual string CommandID {
			get { return string.Empty; }
		}
		protected virtual string DefaultGroupName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string DefaultToolTip {
			get { return DefaultText; }
		}
		protected virtual string ResourceImageName {
			get { return string.Empty; }
		}
		protected virtual bool ShowText {
			get { return true; }
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected internal bool EnableCustomColorsDefaultValue { 
			get { return enableCustomColorsDefaultValue; }
			set {
				enableCustomColorsDefaultValue = value;
				if(enableCustomColors == DefaultBoolean.Default)
					EnableCustomColorsInternal = enableCustomColorsDefaultValue;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HERibbonColorCommandBaseEnableCustomColors"),
#endif
		AutoFormatDisable, DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean EnableCustomColors
		{
			get { return enableCustomColors; }
			set
			{
				enableCustomColors = value;
				EnableCustomColorsInternal = enableCustomColors == DefaultBoolean.Default ? EnableCustomColorsDefaultValue
					: enableCustomColors == DefaultBoolean.True;
			}
		}
		protected override string GetName() {
			return CommandID;
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			ASPxHtmlEditor HtmlEditor = this.GetHtmlEditor();
			if(HtmlEditor != null)
				return HtmlEditor.AddShortcutToTooltip(this.MergeToolTips(), GetName());
			return this.MergeToolTips(); 
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, string.Empty);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, "Large");
			properties.CopyFrom(LargeImage);
			return properties;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HERibbonColorCommandBase src = source as HERibbonColorCommandBase;
			if(src != null) {
				enableCustomColorsDefaultValue = src.enableCustomColorsDefaultValue;
				EnableCustomColors = src.EnableCustomColors;
			}
		}
		string IHERibbonItem.GetToolTip() {
			return GetToolTip();
		}
		string IHERibbonItem.DefaultText {
			get { return DefaultText; }
		}
		string IHERibbonItem.DefaultToolTip {
			get { return DefaultToolTip; }
		}
		bool IHERibbonItem.ShowText {
			get { return ShowText; }
		}
	}
	public class HERibbonDropDownCommandBase : RibbonDropDownButtonItem {
		public HERibbonDropDownCommandBase()
			: this(string.Empty) { }
		public HERibbonDropDownCommandBase(RibbonItemSize size)
			: this(string.Empty, size) { }
		public HERibbonDropDownCommandBase(string text)
			: this(text, RibbonItemSize.Small) { }
		public HERibbonDropDownCommandBase(string text, RibbonItemSize size)
			: this(text, size, null) { }
		public HERibbonDropDownCommandBase(string text, params RibbonDropDownButtonItem[] items)
			: this(text, RibbonItemSize.Small, items) { }
		public HERibbonDropDownCommandBase(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items)
			: base(string.Empty, text, size) {
			if(items != null)
				Items.AddRange(items);
			DropDownMode = DefaultDropDownMode;
		}
		protected virtual string CommandID {
			get { return string.Empty; }
		}
		protected virtual string DefaultGroupName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string DefaultToolTip {
			get { return string.Empty; }
		}
		protected virtual string ResourceImageName {
			get { return string.Empty; }
		}
		protected virtual bool ShowText {
			get { return true; }
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return CommandID;
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip != DefaultText || !ShowText ? DefaultToolTip : string.Empty;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, string.Empty);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = HtmlEditorRibbonHelper.GetRibbonItemImageProperty(this, ResourceImageName, "Large");
			properties.CopyFrom(LargeImage);
			return properties;
		}
		protected virtual bool DefaultDropDownMode {
			get { return true; }
		}
		protected virtual void FillItems() {
		}
		protected override RibbonDropDownButtonCollection GetItems() {
			if(base.Items.Count == 0)
				FillItems();
			return base.GetItems();
		}
	}
}
