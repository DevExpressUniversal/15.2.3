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
using System.Drawing.Printing;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Commands;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using System.Drawing;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public class SRTab : RibbonTab {
		protected virtual string DefaultName {
			get {
				return string.Empty;
			}
		}
		protected virtual string DefaultText {
			get {
				return string.Empty;
			}
		}
		public SRTab() {
			ResetText();
		}
		public SRTab(string text) {
			Text = text;
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string GetName() {
			return string.Format("ss{0}Tab", DefaultName);
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText;
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		#endregion
	}
	public class SRGroup : RibbonGroup {
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string DefaultImage {
			get { return string.Empty; }
		}
		public SRGroup() {
			ResetText();
		}
		public SRGroup(string text) {
			Text = text;
		}
		[Browsable(false)]
		public new string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string GetName() {
			return string.Format("ss{0}Group", DefaultName);
		}
		protected override ItemImagePropertiesBase GetImage() {
			if(!base.Image.IsEmpty)
				return base.GetImage();
			ItemImagePropertiesBase properties;
			if(CommandID != SpreadsheetCommandId.None)
				properties = SpreadsheetRibbonHelper.GetRibbonGroupImageProperty(this, CommandID);
			else
				properties = SpreadsheetRibbonHelper.GetRibbonGroupImageProperty(this, DefaultImage);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText;
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		#endregion
	}
	public class SRContextTabCategory : RibbonContextTabCategory {
		public SRContextTabCategory() {
			ResetName();
			ResetColor();
		}
		public SRContextTabCategory(string name) {
			Name = name;
			ResetColor();
		}
		public SRContextTabCategory(string name, Color color) {
			Name = name;
			Color = color;
		}
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		protected virtual Color DefaultColor {
			get { return Color.Empty; }
		}
		[Browsable(false)]
		public new string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		#region Serialization
		protected bool ShouldSerializeName() {
			return Name != DefaultName;
		}
		protected void ResetName() {
			Name = DefaultName;
		}
		protected bool ShouldSerializeColor() {
			return Color != DefaultColor;
		}
		protected void ResetColor() {
			Color = DefaultColor;
		}
		#endregion
	}
	public class SRToggleButtonCommandBase : RibbonToggleButtonItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Small;
			}
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		protected virtual bool DefaultShowText {
			get { return false; }
		}
		public SRToggleButtonCommandBase() {
			ResetText();
			ResetSize();
		}
		public SRToggleButtonCommandBase(string text) {
			Text = text;
			ResetSize();
		}
		public SRToggleButtonCommandBase(RibbonItemSize size) {
			ResetText();
			Size = size;
		}
		public SRToggleButtonCommandBase(string text, RibbonItemSize size) {
			Text = text;
			Size = size;
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		public override RibbonItemSize Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemSmallImageProperty(this, CommandID);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemLargeImageProperty(this, CommandID);
			properties.CopyFrom(LargeImage);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeSize() {
			return Size != DefaultItemSize;
		}
		protected void ResetSize() {
			Size = DefaultItemSize;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRToggleButtonCommandBase item = source as SRToggleButtonCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	public class SRButtonCommandBase : RibbonButtonItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Small;
			}
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		protected virtual bool DefaultShowText {
			get { return true; }
		}
		public SRButtonCommandBase() {
			ResetText();
			ResetSize();
		}
		public SRButtonCommandBase(string text) {
			Text = text;
			ResetSize();
		}
		public SRButtonCommandBase(RibbonItemSize size) {
			ResetText();
			Size = size;
		}
		public SRButtonCommandBase(string text, RibbonItemSize size) {
			Text = text;
			Size = size;
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		public override RibbonItemSize Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemSmallImageProperty(this, CommandID);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemLargeImageProperty(this, CommandID);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeSize() {
			return Size != DefaultItemSize;
		}
		protected void ResetSize() {
			Size = DefaultItemSize;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRButtonCommandBase item = source as SRButtonCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	public class SRTextBoxCommandBase : RibbonTextBoxItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		public SRTextBoxCommandBase()
			: base() {
				ResetText();
		}
		public SRTextBoxCommandBase(string name)
			: base(name) {
				ResetText();
		}
		public SRTextBoxCommandBase(string name, string text)
			: base(name, text) {
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(Text))
				return base.GetText();
			return DefaultText;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		#endregion
	}
	public class SRComboBoxCommandBase : RibbonComboBoxItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual ListEditItemCollection DefaultItems {
			get {
				return null;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		protected virtual string DefaultNullText {
			get { return string.Empty; }
		}
		protected virtual bool DefaultShowText {
			get { return false; }
		}
		protected virtual int DefaultWidth {
			get { return 50; }
		}
		public SRComboBoxCommandBase() {
			PropertiesComboBox.Width = System.Web.UI.WebControls.Unit.Pixel(DefaultWidth);
			PropertiesComboBox.NullText = DefaultNullText;
			ResetText();
		}
		public SRComboBoxCommandBase(string text)
			: this() {
			Text = text;
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		public void FillItems() {
			if((DefaultItems != null) && (DefaultItems.Count > 0))
				Items.AddRange(DefaultItems);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRComboBoxCommandBase item = source as SRComboBoxCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	public class SRColorCommandBase : RibbonColorButtonItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Small;
			}
		}
		protected virtual bool DefaultShowText {
			get { return false; }
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		public SRColorCommandBase() {
			ResetSize();
			ResetText();
		}
		public SRColorCommandBase(string text) {
			Text = text;
			ResetSize();
		}
		public SRColorCommandBase(RibbonItemSize size) {
			Size = size;
			ResetText();
		}
		public SRColorCommandBase(string text, RibbonItemSize size) {
			Size = size;
			Text = text;
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		public override RibbonItemSize Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		protected override RibbonItemSize GetSize() {
			if(base.GetSize() != RibbonItemSize.Small)
				return base.GetSize();
			return DefaultItemSize;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemSmallImageProperty(this, CommandID);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemLargeImageProperty(this, CommandID);
			properties.CopyFrom(LargeImage);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeSize() {
			return Size != DefaultItemSize;
		}
		protected void ResetSize() {
			Size = DefaultItemSize;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRColorCommandBase item = source as SRColorCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	public class SRDropDownCommandBase : RibbonDropDownButtonItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Small;
			}
		}
		protected virtual bool DefaultDropDownMode {
			get { return true; }
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		protected virtual bool DefaultShowText {
			get { return true; }
		}
		public SRDropDownCommandBase() {
			ResetDropDownMode();
			ResetText();
			ResetSize();
		}
		public SRDropDownCommandBase(string text) {
			ResetDropDownMode();
			Text = text;
			ResetSize();
		}
		public SRDropDownCommandBase(RibbonItemSize size) {
			ResetDropDownMode();
			ResetText();
			Size = size;
		}
		public SRDropDownCommandBase(string text, RibbonItemSize size) {
			ResetDropDownMode();
			Size = size;
			Text = text;
		}
		public SRDropDownCommandBase(string text, params RibbonDropDownButtonItem[] items) {
			ResetDropDownMode();
			Text = text;
			ResetSize();
			Items.AddRange(items);
		}
		public SRDropDownCommandBase(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) {
			ResetDropDownMode();
			Text = text;
			Size = size;
			Items.AddRange(items);
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		public override RibbonItemSize Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		public new bool DropDownMode {
			get { return base.DropDownMode; }
			set { base.DropDownMode = value; }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override RibbonDropDownButtonCollection GetItems() {
			if(base.Items.Count == 0) {
				FillItems();
				return Items;
			}
			return base.GetItems();
		}
		protected virtual void FillItems() { }
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemSmallImageProperty(this, CommandID);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemLargeImageProperty(this, CommandID);
			properties.CopyFrom(LargeImage);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeSize() {
			return Size != DefaultItemSize;
		}
		protected void ResetSize() {
			Size = DefaultItemSize;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		protected bool ShouldSerializeDropDownMode() {
			return DropDownMode != DefaultDropDownMode;
		}
		protected void ResetDropDownMode() {
			DropDownMode = DefaultDropDownMode;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRDropDownCommandBase item = source as SRDropDownCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	public class SRDropDownToggleCommandBase : RibbonDropDownToggleButtonItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Small;
			}
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		protected virtual bool DefaultShowText {
			get { return true; }
		}
		public SRDropDownToggleCommandBase() {
			ResetText();
			ResetSize();
		}
		public SRDropDownToggleCommandBase(string text) {
			Text = text;
			ResetSize();
		}
		public SRDropDownToggleCommandBase(RibbonItemSize size) {
			ResetText();
			Size = size;
		}
		public SRDropDownToggleCommandBase(string text, RibbonItemSize size) {
			Size = size;
			Text = text;
		}
		public SRDropDownToggleCommandBase(string text, params RibbonDropDownButtonItem[] items) {
			Text = text;
			ResetSize();
			Items.AddRange(items);
		}
		public SRDropDownToggleCommandBase(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) {
			Text = text;
			Size = size;
			Items.AddRange(items);
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		public override RibbonItemSize Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override RibbonDropDownButtonCollection GetItems() {
			if(base.Items.Count == 0) {
				FillItems();
				return Items;
			}
			return base.GetItems();
		}
		protected virtual void FillItems() { }
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemSmallImageProperty(this, CommandID);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemLargeImageProperty(this, CommandID);
			properties.CopyFrom(LargeImage);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeSize() {
			return Size != DefaultItemSize;
		}
		protected void ResetSize() {
			Size = DefaultItemSize;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRDropDownToggleCommandBase item = source as SRDropDownToggleCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
		internal SpreadsheetCommandId GetCommandId() {
			return CommandID;
		}
	}
	public class SRCheckBoxCommandBase : RibbonCheckBoxItem, IRibbonInternalItem {
		protected virtual WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.None;
			}
		}
		protected virtual SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.None;
			}
		}
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual string DefaultText {
			get { return SpreadsheetRibbonHelper.GetCommandText(CommandID); }
		}
		protected virtual bool DefaultShowText {
			get { return true; }
		}
		public SRCheckBoxCommandBase() {
			ResetText();
		}
		public SRCheckBoxCommandBase(string text) {
			Text = text;
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		protected override string GetName() {
			if(WebCommandID != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return SpreadsheetRibbonHelper.GetCommandToolTip(CommandID);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRCheckBoxCommandBase item = source as SRCheckBoxCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	public class SRClientToggleButtonCommandBase : RibbonToggleButtonItem, IRibbonInternalItem {
		protected virtual string DefaultGroupName {
			get {
				return string.Empty;
			}
		}
		protected virtual RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Small;
			}
		}
		protected virtual string DefaultLargeImageName {
			get { return string.Empty; }
		}
		protected virtual string DefaultSmallImageName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string DefaultToolTip {
			get { return string.Empty; }
		}
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		protected virtual bool DefaultShowText {
			get { return true; }
		}
		public SRClientToggleButtonCommandBase() {
			ResetText();
			ResetSize();
		}
		public SRClientToggleButtonCommandBase(string text) {
			Text = text;
			ResetSize();
		}
		public SRClientToggleButtonCommandBase(RibbonItemSize size) {
			ResetText();
			Size = size;
		}
		public SRClientToggleButtonCommandBase(string text, RibbonItemSize size) {
			Text = text;
			Size = size;
		}
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[
		AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", DefaultShowText); }
			set { SetBoolProperty("ShowText", DefaultShowText, value); }
		}
		public override RibbonItemSize Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		protected override string GetName() {
			return DefaultName;
		}
		protected override string GetText() {
			if(ShowText && !string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? DefaultText : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemImageProperty(this, DefaultSmallImageName);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = SpreadsheetRibbonHelper.GetRibbonItemImageProperty(this, DefaultLargeImageName);
			properties.CopyFrom(LargeImage);
			return properties;
		}
		#region Serialization
		protected bool ShouldSerializeText() {
			return Text != DefaultText && !string.IsNullOrEmpty(Text);
		}
		protected void ResetText() {
			Text = DefaultText;
		}
		protected bool ShouldSerializeSize() {
			return Size != DefaultItemSize;
		}
		protected void ResetSize() {
			Size = DefaultItemSize;
		}
		protected bool ShouldSerializeShowText() {
			return ShowText != DefaultShowText;
		}
		protected void ResetShowText() {
			ShowText = DefaultShowText;
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SRClientToggleButtonCommandBase item = source as SRClientToggleButtonCommandBase;
			if(item != null) {
				ShowText = item.ShowText;
			}
		}
	}
	internal class SRFunctionCommandBase : RibbonDropDownButtonItem, IRibbonInternalItem {
		protected string functionName = string.Empty;
		protected SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInsertSpecificFunction; 
			}
		}
		protected virtual string FunctionName {
			get {
				return functionName;
			}
			private set {
				functionName = value;
			}
		}
		public SRFunctionCommandBase()
			: base() {
		}
		public SRFunctionCommandBase(string functionName)
			: base() {
			this.functionName = functionName;
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(FunctionName))
				return SpreadsheetRibbonHelper.GetCommandName(CommandID) + SpreadsheetRibbonHelper.CommandSeparator + FunctionName;
			return SpreadsheetRibbonHelper.GetCommandName(CommandID);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return FunctionName;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return FunctionName;
		}
	}
	internal class SRBorderLineStyleCommandBase : RibbonDropDownButtonItem, IRibbonInternalItem {
		protected WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatBorderLineStyle;
			}
		}
		protected virtual XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.None;
			}
		}
		protected virtual string BorderCaption {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		protected override string GetName() {
			return WebSpreadsheetCommands.GetWebCommandName(WebCommandID) + SpreadsheetRibbonHelper.CommandSeparator + BorderStyle.ToString();
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return BorderCaption;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return BorderCaption;
		}
	}
	internal class SRPagePaperKindBase : RibbonDropDownToggleButtonItem, IRibbonInternalItem {
		protected PaperKind pagePaperKind = PaperKind.Custom;
		protected ITemplate textItemTmplate = null;
		protected SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupSetPaperKind; 
			}
		}
		protected WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.SetPaperKind;
			}
		}
		protected virtual PaperKind PagePaperKind {
			get {
				return pagePaperKind;
			}
			private set {
				pagePaperKind = value;
			}
		}
		public SRPagePaperKindBase()
			: base() { }
		public SRPagePaperKindBase(PaperKind pagePaperKind)
			: base() {
			this.pagePaperKind = pagePaperKind;
		}
		[Browsable(false)]
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = value;
			}
		}
		protected override ITemplate TextTemplate {
			get {
				if(textItemTmplate == null)
					textItemTmplate = new PaperTemplate(GetText());
				return textItemTmplate;
			}
			set {
				textItemTmplate = value;
			}
		}
		internal void ResetTemplateCaption(string defaultText) {
			if(TextTemplate != null) {
				((PaperTemplate)TextTemplate).ResetHierarchy(defaultText);
			}
		}
		protected override string GetName() {
			if(PagePaperKind != PaperKind.Custom)
				return WebSpreadsheetCommands.GetWebCommandName(WebCommandID) + SpreadsheetRibbonHelper.CommandSeparator + PagePaperKind.ToString();
			return WebSpreadsheetCommands.GetWebCommandName(WebCommandID);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return SpreadsheetRibbonHelper.GetPaperKindCaption(PagePaperKind, null);
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return PagePaperKind.ToString();
		}
		internal PaperKind GetPagePaperKind() {
			return PagePaperKind;
		}
	}
	public class PaperTemplate : ITemplate {
		Control ContainerControl = null;
		protected string TemplateText { get; private set; }
		public PaperTemplate(string itemText) {
			TemplateText = itemText;
		}
		public void InstantiateIn(Control container) {
			ContainerControl = container;
			WebControl div = RenderUtils.CreateDiv();
			div.Style.Add("display", "inline-table");
			div.Width = Unit.Percentage(100);
			div.Controls.Add(new LiteralControl(TemplateText));
			ContainerControl.Controls.Add(div);
		}
		public void ResetHierarchy(string itemText) {
			if(ContainerControl != null) {
				TemplateText = itemText;
				ContainerControl.Controls.Clear();
				InstantiateIn(ContainerControl);
			}
		}
	}
	public class SRChartLargeDropDownToggleCommandBase : SRDropDownToggleCommandBase {
		protected ITemplate textItemTmplate = null;
		protected override ITemplate TextTemplate {
			get {
				if(textItemTmplate == null)
					textItemTmplate = new ChartDropDownButtonTemplate(CommandID);
				return textItemTmplate;
			}
			set {
				textItemTmplate = value;
			}
		}
		internal void ResetTemplateCaption(string defaultText) {
			if(TextTemplate != null) {
				((ChartDropDownButtonTemplate)TextTemplate).ResetHierarchy(CommandID);
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
	}
	public class ChartDropDownButtonTemplate : ITemplate {
		private Control ContainerControl = null;
		protected SpreadsheetCommandId CommandID { get; private set; }
		public ChartDropDownButtonTemplate(SpreadsheetCommandId commandID) {
			CommandID = commandID;
		}
		public void InstantiateIn(Control container) {
			ContainerControl = container;
			WebControl div = RenderUtils.CreateDiv();
			RenderUtils.AppendDefaultDXClassName(div, "dxss-ch-container");
			Label textLabel = RenderUtils.CreateLabel();
			textLabel.Text = SpreadsheetRibbonHelper.GetCommandText(CommandID);
			RenderUtils.AppendDefaultDXClassName(textLabel, "dxss-ch-text");
			Label descriptionLabel = RenderUtils.CreateLabel();
			descriptionLabel.Text = SpreadsheetRibbonHelper.GetCommandDescription(CommandID);
			div.Controls.Add(textLabel);
			div.Controls.Add(RenderUtils.CreateBr());
			div.Controls.Add(descriptionLabel);
			ContainerControl.Controls.Add(div);
		}
		public void ResetHierarchy(SpreadsheetCommandId commandID) {
			if(ContainerControl != null) {
				CommandID = commandID;
				ContainerControl.Controls.Clear();
				InstantiateIn(ContainerControl);
			}
		}
	}
}
