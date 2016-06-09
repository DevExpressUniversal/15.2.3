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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Editors;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemTokenEdit : RepositoryItem {
		StringCollection separators;
		TokenEditTokenCollection tokens;
		AppearanceObject appearanceDropDown;
		TokenEditSelectedItemCollection selectedItems;
		TokenEditCheckedItemCollection checkedItems;
		TokenEditTokenCacheBase objectCache;
		TokenEditTokenCacheBase stringCache;
		TokenEditStringFormatter stringFormatter;
		bool popupSizeable;
		bool showDropDown;
		int dropDownRowCount;
		CustomTokenEditDropDownControlBase customDropDownControl;
		TokenEditPopupFilterMode popupFilterMode;
		bool showRemoveTokenButtons;
		TokenEditDropDownShowMode dropDownShowMode;
		TokenEditMode editMode;
		char editValueSeparatorChar;
		bool showTokenGlyph;
		bool useReadOnlyAppearance;
		TokenEditGlyphLocation tokenGlyphLocation;
		TokenEditValueType editValueType;
		TokenEditAutoHeightMode autoHeightMode;
		int maxExpandLines;
		int maxTokenCount;
		int minRowCount;
		DefaultBoolean deleteTokenOnGlyphClick;
		TokenEditTokenPopupateMode tokenPopulateMode;
		IFlyoutPanel popupPanel;
		PopupPanelOptions popupPanelOptions;
		TokenEditCheckMode checkMode;
		DefaultBoolean clearCheckStatesOnLostFocus;
		TokenEditValidationRules validationRules;
		TokenEditValidationController validator;
		public const int DefaultDropDownRowCount = 7;
		public const char DefaultEditValueSeparatorChar = ',';
		public const int DefaultMaxExpandLines = 4;
		static readonly object tokenMouseEnter = new object();
		static readonly object tokenMouseLeave = new object();
		static readonly object customDrawTokenBackground = new object();
		static readonly object customDrawTokenText = new object();
		static readonly object customDrawTokenGlyph = new object();
		static readonly object tokenClick = new object();
		static readonly object tokenDoubleClick = new object();
		static readonly object tokenAdding = new object();
		static readonly object tokenAdded = new object();
		static readonly object tokenRemoving = new object();
		static readonly object tokenRemoved = new object();
		static readonly object validateToken = new object();
		static readonly object tokenMouseHover = new object();
		static readonly object beforeShowPopupPanel = new object();
		static readonly object tokenCheckStateChanged = new object();
		static readonly object selectedItemsChanged = new object();
		static readonly object beforeShowMenu = new object();
		public RepositoryItemTokenEdit() {
			this.tokens = CreateTokenCollection();
			this.tokens.ListChanged += OnTokenListChanged;
			this.selectedItems = CreateSelectedItemCollection();
			this.selectedItems.ListChanged += OnSelectedItemListChanged;
			this.checkedItems = CreateCheckedItemCollection();
			this.checkedItems.ListChanged += OnCheckedItemListChanged;
			this.separators = CreateSeparatorCollection();
			this.objectCache = CreateObjectCache();
			this.stringCache = CreateStringCache();
			this.stringFormatter = CreateStringFormatter();
			this.popupSizeable = true;
			this.dropDownRowCount = DefaultDropDownRowCount;
			this.appearanceDropDown = CreateAppearance("DropDown");
			this.customDropDownControl = null;
			this.popupFilterMode = TokenEditPopupFilterMode.StartWith;
			this.showRemoveTokenButtons = false;
			this.dropDownShowMode = TokenEditDropDownShowMode.Default;
			this.editMode = TokenEditMode.Default;
			this.showDropDown = true;
			this.editValueSeparatorChar = DefaultEditValueSeparatorChar;
			this.showTokenGlyph = true;
			this.useReadOnlyAppearance = true;
			this.tokenGlyphLocation = TokenEditGlyphLocation.Default;
			this.editValueType = TokenEditValueType.String;
			this.autoHeightMode = TokenEditAutoHeightMode.Default;
			this.maxExpandLines = DefaultMaxExpandLines;
			this.maxTokenCount = -1;
			this.deleteTokenOnGlyphClick = DefaultBoolean.Default;
			this.tokenPopulateMode = TokenEditTokenPopupateMode.Default;
			this.popupPanel = null;
			this.popupPanelOptions = CreatePopupPanelOptions();
			this.checkMode = TokenEditCheckMode.Default;
			this.clearCheckStatesOnLostFocus = DefaultBoolean.Default;
			this.minRowCount = 1;
			this.validationRules = TokenEditValidationRules.Default;
			this.validator = CreateValidationController();
		}
		protected virtual TokenEditTokenCollection CreateTokenCollection() {
			return new TokenEditTokenCollection(this);
		}
		protected virtual StringCollection CreateSeparatorCollection() {
			return new StringCollection();
		}
		protected virtual TokenEditSelectedItemCollection CreateSelectedItemCollection() {
			return new TokenEditSelectedItemCollection(this);
		}
		protected virtual TokenEditCheckedItemCollection CreateCheckedItemCollection() {
			return new TokenEditCheckedItemCollection(this);
		}
		protected virtual TokenEditTokenCacheBase CreateObjectCache() {
			return new TokenEditObjectCache();
		}
		protected virtual TokenEditTokenCacheBase CreateStringCache() {
			return new TokenEditStringCache();
		}
		protected virtual PopupPanelOptions CreatePopupPanelOptions() {
			return new PopupPanelOptions(this);
		}
		protected virtual TokenEditStringFormatter CreateStringFormatter() {
			return new TokenEditStringFormatter(this);
		}
		protected virtual TokenEditValidationController CreateValidationController() {
			return new TokenEditValidationController(this);
		}
		protected internal TokenEditTokenCacheBase GetObjectCache() {
			return this.objectCache;
		}
		protected internal TokenEditTokenCacheBase GetStringCache() {
			return this.stringCache;
		}
		protected internal TokenEditValidationController Validator { get { return validator; } }
		protected internal TokenEditStringFormatter StringFormatter { get { return stringFormatter; } }
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemTokenEdit other = item as RepositoryItemTokenEdit;
			if(other == null) return;
			BeginUpdate();
			Tokens.BeginUpdate();
			SelectedItems.BeginUpdate();
			try {
				Tokens.Assign(other.Tokens);
				AssignSeparators(other);
				SelectedItems.Assign(other.selectedItems);
				CheckedItems.Assign(other.CheckedItems);
				this.objectCache = other.objectCache;
				this.stringCache = other.stringCache;
				this.appearanceDropDown.Assign(other.AppearanceDropDown);
				this.popupSizeable = other.PopupSizeable;
				this.dropDownRowCount = other.DropDownRowCount;
				this.customDropDownControl = other.CustomDropDownControl;
				this.popupFilterMode = other.PopupFilterMode;
				this.showRemoveTokenButtons = other.ShowRemoveTokenButtons;
				this.dropDownShowMode = other.DropDownShowMode;
				this.editMode = other.EditMode;
				this.showDropDown = other.ShowDropDown;
				this.editValueSeparatorChar = other.EditValueSeparatorChar;
				this.showTokenGlyph = other.ShowTokenGlyph;
				this.useReadOnlyAppearance = other.UseReadOnlyAppearance;
				this.tokenGlyphLocation = other.TokenGlyphLocation;
				this.editValueType = other.EditValueType;
				this.autoHeightMode = other.AutoHeightMode;
				this.maxExpandLines = other.MaxExpandLines;
				this.maxTokenCount = other.MaxTokenCount;
				this.deleteTokenOnGlyphClick = other.DeleteTokenOnGlyphClick;
				this.tokenPopulateMode = other.TokenPopulateMode;
				this.popupPanel = other.PopupPanel;
				this.popupPanelOptions.Assign(other.PopupPanelOptions);
				this.checkMode = other.CheckMode;
				this.clearCheckStatesOnLostFocus = other.ClearCheckStatesOnLostFocus;
				this.minRowCount = other.MinRowCount;
				this.validationRules = other.ValidationRules;
				UpdateMapper();
				Events.AddHandler(validateToken, other.Events[validateToken]); 
			}
			finally {
				EndUpdate();
				SelectedItems.EndUpdate();
				Tokens.EndUpdate();
			}
			Events.AddHandler(tokenMouseEnter, other.Events[tokenMouseEnter]);
			Events.AddHandler(tokenMouseLeave, other.Events[tokenMouseLeave]);
			Events.AddHandler(customDrawTokenBackground, other.Events[customDrawTokenBackground]);
			Events.AddHandler(customDrawTokenText, other.Events[customDrawTokenText]);
			Events.AddHandler(customDrawTokenGlyph, other.Events[customDrawTokenGlyph]);
			Events.AddHandler(tokenClick, other.Events[tokenClick]);
			Events.AddHandler(tokenDoubleClick, other.Events[tokenDoubleClick]);
			Events.AddHandler(tokenAdding, other.Events[tokenAdding]);
			Events.AddHandler(tokenAdded, other.Events[tokenAdded]);
			Events.AddHandler(tokenRemoving, other.Events[tokenRemoving]);
			Events.AddHandler(tokenRemoved, other.Events[tokenRemoved]);
			Events.AddHandler(tokenMouseHover, other.Events[tokenMouseHover]);
			Events.AddHandler(beforeShowPopupPanel, other.Events[beforeShowPopupPanel]);
			Events.AddHandler(tokenCheckStateChanged, other.Events[tokenCheckStateChanged]);
			Events.AddHandler(selectedItemsChanged, other.Events[selectedItemsChanged]);
			Events.AddHandler(beforeShowMenu, other.Events[beforeShowMenu]);
		}
		protected void UpdateMapper() {
			Mappers.UpdateMapper(EditValueType);
		}
		protected virtual void AssignSeparators(RepositoryItemTokenEdit source) {
			Separators.Clear();
			foreach(string s in source.Separators) Separators.Add(s);
		}
		protected virtual void OnTokenListChanged(object sender, ListChangedEventArgs e) {
			GetObjectCache().Update(this);
			GetStringCache().Update(this);
		}
		protected internal override bool NeededKeysContains(Keys key) {
			switch(key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case Keys.Home:
				case Keys.End:
				case Keys.F2:
				case Keys.A:
				case Keys.Add:
				case Keys.B:
				case Keys.Back:
				case Keys.C:
				case Keys.Clear:
				case Keys.D:
				case Keys.D0:
				case Keys.D1:
				case Keys.D2:
				case Keys.D3:
				case Keys.D4:
				case Keys.D5:
				case Keys.D6:
				case Keys.D7:
				case Keys.D8:
				case Keys.D9:
				case Keys.Decimal:
				case Keys.Delete:
				case Keys.Divide:
				case Keys.E:
				case Keys.F:
				case Keys.F20:
				case Keys.G:
				case Keys.H:
				case Keys.I:
				case Keys.Insert:
				case Keys.J:
				case Keys.K:
				case Keys.L:
				case Keys.M:
				case Keys.Multiply:
				case Keys.N:
				case Keys.NumPad0:
				case Keys.NumPad1:
				case Keys.NumPad2:
				case Keys.NumPad3:
				case Keys.NumPad4:
				case Keys.NumPad5:
				case Keys.NumPad6:
				case Keys.NumPad7:
				case Keys.NumPad8:
				case Keys.NumPad9:
				case Keys.Alt:
				case (Keys.RButton | Keys.ShiftKey):
				case Keys.O:
				case Keys.Oem8:
				case Keys.OemBackslash:
				case Keys.OemCloseBrackets:
				case Keys.Oemcomma:
				case Keys.OemMinus:
				case Keys.OemOpenBrackets:
				case Keys.OemPeriod:
				case Keys.OemPipe:
				case Keys.Oemplus:
				case Keys.OemQuestion:
				case Keys.OemQuotes:
				case Keys.OemSemicolon:
				case Keys.Oemtilde:
				case Keys.P:
				case Keys.Q:
				case Keys.R:
				case Keys.S:
				case Keys.Space:
				case Keys.Subtract:
				case Keys.T:
				case Keys.U:
				case Keys.V:
				case Keys.W:
				case Keys.X:
				case Keys.Y:
				case Keys.Z:
					return true;
			}
			return base.NeededKeysContains(key);
		}
		protected internal virtual bool IsPopupNavigationKey(Keys keys) {
			switch(keys) {
				case Keys.Down:
				case Keys.Up:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.Home:
				case Keys.End:
					return true;
			}
			return false;
		}
		protected internal virtual bool IsPopupCloseKey(Keys keys) {
			switch(keys) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Escape:
				case Keys.Home:
				case Keys.End:
					return true;
			}
			return false;
		}
		protected internal bool IsSeparatorChar(char c) {
			return Separators.Contains(c.ToString());
		}
		[Browsable(false)]
		public new TokenEdit OwnerEdit { get { return base.OwnerEdit as TokenEdit; } }
		protected virtual void OnSelectedItemListChanged(object sender, ListChangedEventArgs e) {
			DoSyncCheckedItems();
			if(OwnerEdit != null) {
				OwnerEdit.UpdateEditValue();
			}
			RaiseSelectedItemsChanged(e);
			LayoutChanged();
		}
		protected void DoSyncCheckedItems() {
			if(CheckedItems.Count == 0) return;
			CheckedItems.BeginUpdate();
			try {
				for(int i = 0; i < CheckedItems.Count; i++) {
					TokenEditToken tok = CheckedItems[i];
					if(!SelectedItems.Contains(tok)) CheckedItems.Remove(tok);
				}
			}
			finally {
				CheckedItems.EndUpdate();
			}
		}
		protected virtual void OnCheckedItemListChanged(object sender, ListChangedEventArgs e) {
			RaiseTokenCheckStateChanged(EventArgs.Empty);
			LayoutChanged();
		}
		protected override void DestroyAppearances() {
			DestroyAppearance(AppearanceDropDown);
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditAppearanceDropDown"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceDropDown { get { return appearanceDropDown; } }
		bool ShouldSerializeAppearanceDropDown() {
			return AppearanceDropDown.ShouldSerialize();
		}
		void ResetAppearanceDropDown() { AppearanceDropDown.Reset(); }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditPopupSizeable"),
#endif
 DefaultValue(true)]
		public bool PopupSizeable {
			get { return popupSizeable; }
			set {
				if(PopupSizeable == value)
					return;
				popupSizeable = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokens"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false)]
		public TokenEditTokenCollection Tokens { get { return tokens; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditDropDownRowCount"),
#endif
 DefaultValue(DefaultDropDownRowCount), SmartTagProperty("DropDown Row Count", "Tokens", SortOrder = 40)]
		public int DropDownRowCount {
			get { return dropDownRowCount; }
			set {
				if(DropDownRowCount == value)
					return;
				dropDownRowCount = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditSeparators"),
#endif
 DXCategory(CategoryName.Behavior), Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual StringCollection Separators { get { return separators; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditCustomDropDownControl"),
#endif
 Browsable(false), DefaultValue(null)]
		public CustomTokenEditDropDownControlBase CustomDropDownControl {
			get { return customDropDownControl; }
			set {
				if(CustomDropDownControl == value)
					return;
				customDropDownControl = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TokenEditSelectedItemCollection SelectedItems { get { return selectedItems; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditPopupFilterMode"),
#endif
 DefaultValue(TokenEditPopupFilterMode.StartWith)]
		public TokenEditPopupFilterMode PopupFilterMode {
			get { return popupFilterMode; }
			set {
				if(PopupFilterMode == value)
					return;
				popupFilterMode = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TokenEditCheckedItemCollection CheckedItems { get { return checkedItems; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditShowRemoveTokenButtons"),
#endif
 DefaultValue(false)]
		public bool ShowRemoveTokenButtons {
			get { return showRemoveTokenButtons; }
			set {
				if(ShowRemoveTokenButtons == value)
					return;
				showRemoveTokenButtons = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemTokenEdit Properties { get { return this; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowSelectedItemsInPopup {
			get { return false; }
			set { }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditDropDownShowMode"),
#endif
 DefaultValue(TokenEditDropDownShowMode.Default), SmartTagProperty("DropDown Show Mode", "Tokens", SortOrder = 35)]
		public TokenEditDropDownShowMode DropDownShowMode {
			get { return dropDownShowMode; }
			set {
				if(DropDownShowMode == value)
					return;
				dropDownShowMode = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual TokenEditDropDownShowMode GetDropDownShowMode() {
			if(DropDownShowMode == TokenEditDropDownShowMode.Default) return TokenEditDropDownShowMode.Regular;
			return DropDownShowMode;
		}
		protected internal bool IsOutlookDropDown { get { return GetDropDownShowMode() == TokenEditDropDownShowMode.Outlook; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditEditMode"),
#endif
 DefaultValue(TokenEditMode.Default), SmartTagProperty("Edit Mode", CategoryName.Behavior, SortOrder = 50)]
		public TokenEditMode EditMode {
			get { return editMode; }
			set {
				if(EditMode == value)
					return;
				editMode = value;
				OnEditModeChanged();
			}
		}
		protected virtual void OnEditModeChanged() {
			if(OwnerEdit != null) OwnerEdit.OnEditModeChanged();
			OnPropertiesChanged();
		}
		protected internal virtual TokenEditMode GetEditMode() {
			if(EditMode == TokenEditMode.Default) return TokenEditMode.TokenList;
			return EditMode;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditShowDropDown"),
#endif
 DefaultValue(true)]
		public bool ShowDropDown {
			get { return showDropDown; }
			set {
				if(ShowDropDown == value)
					return;
				showDropDown = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Format), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditEditValueSeparatorChar"),
#endif
 DefaultValue(DefaultEditValueSeparatorChar)]
		public char EditValueSeparatorChar {
			get { return editValueSeparatorChar; }
			set {
				if(EditValueSeparatorChar == value)
					return;
				editValueSeparatorChar = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditShowTokenGlyph"),
#endif
 DefaultValue(true)]
		public bool ShowTokenGlyph {
			get { return showTokenGlyph; }
			set {
				if(ShowTokenGlyph == value)
					return;
				showTokenGlyph = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditUseReadOnlyAppearance"),
#endif
 DefaultValue(true)]
		public bool UseReadOnlyAppearance {
			get { return useReadOnlyAppearance; }
			set {
				if(UseReadOnlyAppearance == value)
					return;
				useReadOnlyAppearance = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenGlyphLocation"),
#endif
 DefaultValue(TokenEditGlyphLocation.Default)]
		public TokenEditGlyphLocation TokenGlyphLocation {
			get { return tokenGlyphLocation; }
			set {
				if(TokenGlyphLocation == value)
					return;
				tokenGlyphLocation = value;
				OnPropertiesChanged();
			}
		}
		protected internal TokenEditGlyphLocation GetTokenGlyphLocation() {
			return TokenGlyphLocation == TokenEditGlyphLocation.Default ? TokenEditGlyphLocation.Left : TokenGlyphLocation;
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditEditValueType"),
#endif
 DefaultValue(TokenEditValueType.String), SmartTagProperty("Edit Value Type", CategoryName.Behavior, SortOrder = 55)]
		public TokenEditValueType EditValueType {
			get { return editValueType; }
			set {
				if(EditValueType == value)
					return;
				editValueType = value;
				OnEditValueTypeChanged();
			}
		}
		protected virtual void OnEditValueTypeChanged() {
			Mappers.UpdateMapper(EditValueType);
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditAutoHeightMode"),
#endif
 DefaultValue(TokenEditAutoHeightMode.Default)]
		public TokenEditAutoHeightMode AutoHeightMode {
			get { return autoHeightMode; }
			set {
				if(AutoHeightMode == value)
					return;
				autoHeightMode = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditMaxExpandLines"),
#endif
 DefaultValue(DefaultMaxExpandLines)]
		public int MaxExpandLines {
			get { return maxExpandLines; }
			set {
				if(MaxExpandLines == value)
					return;
				maxExpandLines = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditMaxTokenCount"),
#endif
 DefaultValue(-1)]
		public int MaxTokenCount {
			get { return maxTokenCount; }
			set {
				if(value < -1) value = -1;
				if(MaxTokenCount == value)
					return;
				maxTokenCount = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditDeleteTokenOnGlyphClick"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean DeleteTokenOnGlyphClick {
			get { return deleteTokenOnGlyphClick; }
			set {
				if(DeleteTokenOnGlyphClick == value)
					return;
				deleteTokenOnGlyphClick = value;
				OnPropertiesChanged();
			}
		}
		protected internal bool ShouldDeleteTokenOnGlyphClick() {
			return DeleteTokenOnGlyphClick != DefaultBoolean.False;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenPopulateMode"),
#endif
 DefaultValue(TokenEditTokenPopupateMode.Default)]
		public TokenEditTokenPopupateMode TokenPopulateMode {
			get { return tokenPopulateMode; }
			set {
				if(TokenPopulateMode == value)
					return;
				tokenPopulateMode = value;
				OnPropertiesChanged();
			}
		}
		internal bool IsDisableAutoPopulate {
			get { return TokenPopulateMode == TokenEditTokenPopupateMode.DisableAutoPopulate; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditPopupPanel"),
#endif
 DefaultValue(null), SmartTagProperty("PopupPanel", CategoryName.Appearance, SortOrder = 10), TypeConverter("DevExpress.Utils.Design.FlyoutPanelTypeConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public IFlyoutPanel PopupPanel {
			get { return popupPanel; }
			set {
				if(PopupPanel == value)
					return;
				popupPanel = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditPopupPanelOptions"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupPanelOptions PopupPanelOptions {
			get { return popupPanelOptions; }
		}
		bool ShouldSerializePopupPanelOptions() { return PopupPanelOptions.ShouldSerialize(); }
		void ResetPopupPanelOptions() { PopupPanelOptions.Reset(); }
		protected internal virtual TokenEditPopupPanelShowMode GetPopupPanelShowMode() {
			TokenEditPopupPanelShowMode showMode = PopupPanelOptions.ShowMode;
			if(showMode == TokenEditPopupPanelShowMode.Default) {
				showMode = TokenEditPopupPanelShowMode.ShowOnTokenMouseHover;
			}
			return showMode;
		}
		protected internal virtual TokenEditPopupPanelLocation GetPopupPanelLocation() {
			TokenEditPopupPanelLocation loc = PopupPanelOptions.Location;
			if(loc == TokenEditPopupPanelLocation.Default) {
				loc = TokenEditPopupPanelLocation.AboveToken;
			}
			return loc;
		}
		protected internal virtual bool AllowPopupPanel {
			get {
				if(PopupPanel == null) return false;
				return PopupPanelOptions.ShowPopupPanel;
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditCheckMode"),
#endif
 DefaultValue(TokenEditCheckMode.Default)]
		public TokenEditCheckMode CheckMode {
			get { return checkMode; }
			set {
				if(CheckMode == value)
					return;
				checkMode = value;
				OnPropertiesChanged();
			}
		}
		protected internal TokenEditCheckMode GetCheckMode() {
			if(CheckMode == TokenEditCheckMode.Default) return TokenEditCheckMode.Single;
			return CheckMode;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditClearCheckStatesOnLostFocus"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ClearCheckStatesOnLostFocus {
			get { return clearCheckStatesOnLostFocus; }
			set {
				if(ClearCheckStatesOnLostFocus == value)
					return;
				clearCheckStatesOnLostFocus = value;
				OnPropertiesChanged();
			}
		}
		protected internal bool AllowClearCheckStatesOnLostFocus() {
			return ClearCheckStatesOnLostFocus == DefaultBoolean.True;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditMinRowCount"),
#endif
 DefaultValue(1)]
		public int MinRowCount {
			get { return minRowCount; }
			set {
				if(MinRowCount == value)
					return;
				if(value < 1) value = 1;
				minRowCount = value;
				OnMinRowCountChanged();
			}
		}
		protected virtual void OnMinRowCountChanged() {
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditValidationRules"),
#endif
 DefaultValue(TokenEditValidationRules.Default), Editor(typeof(AttributesEditor), typeof(UITypeEditor))]
		public TokenEditValidationRules ValidationRules {
			get { return validationRules; }
			set {
				if(ValidationRules == value)
					return;
				validationRules = value;
				OnPropertiesChanged();
			}
		}
		protected internal bool AllowValidateOnLostFocus {
			get { return (ValidationRules & TokenEditValidationRules.ValidateOnLostFocus) != 0; }
		}
		protected internal bool AllowValidateOnSeparatorInput {
			get { return (ValidationRules & TokenEditValidationRules.ValidateOnSeparatorInput) != 0; }
		}
		public void Validate() { if(OwnerEdit != null) OwnerEdit.Validate(); }
		TokenEditMapperRegistry mappers = null;
		protected TokenEditMapperRegistry Mappers {
			get {
				if(this.mappers == null) this.mappers = CreateMappersRegistry();
				return this.mappers;
			}
		}
		protected virtual TokenEditMapperRegistry CreateMappersRegistry() {
			return new TokenEditMapperRegistry();
		}
		protected internal TokenEditValueMapperBase GetMapper() {
			TokenEditValueMapperBase mapper = Mappers.GetMapper();
			if(mapper == null) {
				Mappers.UpdateMapper(EditValueType);
				mapper = Mappers.GetMapper();
			}
			return mapper;
		}
		protected internal virtual void DoUpdateSelectedItems(object editValue) {
			ICollection col = GetMapper().GetTokens(this, editValue);
			SelectedItems.Assign(col);
		}
		protected internal virtual object DoUpdateEditValue(object editValue) {
			return GetMapper().UpdateEditValue(this, SelectedItems, editValue);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenAdding"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenAddingEventHandler TokenAdding {
			add { Events.AddHandler(tokenAdding, value); }
			remove { Events.RemoveHandler(tokenAdding, value); }
		}
		protected internal virtual bool RaiseTokenAdding(TokenEditTokenAddingEventArgs e) {
			TokenEditTokenAddingEventHandler handler = (TokenEditTokenAddingEventHandler)Events[tokenAdding];
			if(handler != null) handler(GetEventSender(), e);
			return e.Cancel;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenAdded"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenAddedEventHandler TokenAdded {
			add { Events.AddHandler(tokenAdded, value); }
			remove { Events.RemoveHandler(tokenAdded, value); }
		}
		protected internal virtual void RaiseTokenAdded(TokenEditTokenAddedEventArgs e) {
			TokenEditTokenAddedEventHandler handler = (TokenEditTokenAddedEventHandler)Events[tokenAdded];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenRemoving"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenRemovingEventHandler TokenRemoving {
			add { Events.AddHandler(tokenRemoving, value); }
			remove { Events.RemoveHandler(tokenRemoving, value); }
		}
		protected internal virtual bool RaiseTokenRemoving(TokenEditTokenRemovingEventArgs e) {
			TokenEditTokenRemovingEventHandler handler = (TokenEditTokenRemovingEventHandler)Events[tokenRemoving];
			if(handler != null) handler(GetEventSender(), e);
			return e.Cancel;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenRemoved"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenRemovedEventHandler TokenRemoved {
			add { Events.AddHandler(tokenRemoved, value); }
			remove { Events.RemoveHandler(tokenRemoved, value); }
		}
		protected internal virtual void RaiseTokenRemoved(TokenEditTokenRemovedEventArgs e) {
			TokenEditTokenRemovedEventHandler handler = (TokenEditTokenRemovedEventHandler)Events[tokenRemoved];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenMouseEnter"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenMouseEnterEventHandler TokenMouseEnter {
			add { Events.AddHandler(tokenMouseEnter, value); }
			remove { Events.RemoveHandler(tokenMouseEnter, value); }
		}
		protected internal virtual void RaiseTokenMouseEnter(TokenEditTokenMouseEnterEventArgs e) {
			TokenEditTokenMouseEnterEventHandler handler = (TokenEditTokenMouseEnterEventHandler)Events[tokenMouseEnter];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenMouseLeave"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenMouseLeaveEventHandler TokenMouseLeave {
			add { Events.AddHandler(tokenMouseLeave, value); }
			remove { Events.RemoveHandler(tokenMouseLeave, value); }
		}
		protected internal virtual void RaiseTokenMouseLeave(TokenEditTokenMouseLeaveEventArgs e) {
			TokenEditTokenMouseLeaveEventHandler handler = (TokenEditTokenMouseLeaveEventHandler)Events[tokenMouseLeave];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditCustomDrawTokenBackground"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditCustomDrawTokenBackgroundEventHandler CustomDrawTokenBackground {
			add { Events.AddHandler(customDrawTokenBackground, value); }
			remove { Events.RemoveHandler(customDrawTokenBackground, value); }
		}
		protected internal virtual void RaiseCustomDrawTokenBackground(TokenEditCustomDrawTokenBackgroundEventArgs e) {
			TokenEditCustomDrawTokenBackgroundEventHandler handler = (TokenEditCustomDrawTokenBackgroundEventHandler)Events[customDrawTokenBackground];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditCustomDrawTokenText"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditCustomDrawTokenTextEventHandler CustomDrawTokenText {
			add { Events.AddHandler(customDrawTokenText, value); }
			remove { Events.RemoveHandler(customDrawTokenText, value); }
		}
		protected internal virtual void RaiseCustomDrawTokenText(TokenEditCustomDrawTokenTextEventArgs e) {
			TokenEditCustomDrawTokenTextEventHandler handler = (TokenEditCustomDrawTokenTextEventHandler)Events[customDrawTokenText];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditCustomDrawTokenGlyph"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditCustomDrawTokenGlyphEventHandler CustomDrawTokenGlyph {
			add { Events.AddHandler(customDrawTokenGlyph, value); }
			remove { Events.RemoveHandler(customDrawTokenGlyph, value); }
		}
		protected internal virtual void RaiseCustomDrawTokenGlyph(TokenEditCustomDrawTokenGlyphEventArgs e) {
			TokenEditCustomDrawTokenGlyphEventHandler handler = (TokenEditCustomDrawTokenGlyphEventHandler)Events[customDrawTokenGlyph];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenClickEventHandler TokenClick {
			add { Events.AddHandler(tokenClick, value); }
			remove { Events.RemoveHandler(tokenClick, value); }
		}
		protected internal virtual void RaiseTokenClick(TokenEditTokenClickEventArgs e) {
			TokenEditTokenClickEventHandler handler = (TokenEditTokenClickEventHandler)Events[tokenClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenDoubleClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenClickEventHandler TokenDoubleClick {
			add { Events.AddHandler(tokenDoubleClick, value); }
			remove { Events.RemoveHandler(tokenDoubleClick, value); }
		}
		protected internal virtual void RaiseTokenDoubleClick(TokenEditTokenClickEventArgs e) {
			TokenEditTokenClickEventHandler handler = (TokenEditTokenClickEventHandler)Events[tokenDoubleClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditValidateToken"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditValidateTokenEventHandler ValidateToken {
			add { Events.AddHandler(validateToken, value); }
			remove { Events.RemoveHandler(validateToken, value); }
		}
		protected internal virtual bool RaiseValidateToken(TokenEditValidateTokenEventArgs e) {
			TokenEditValidateTokenEventHandler handler = (TokenEditValidateTokenEventHandler)Events[validateToken];
			if(handler != null) handler(GetEventSender(), e);
			return e.IsValid;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenMouseHover"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditTokenMouseHoverEventHandler TokenMouseHover {
			add { Events.AddHandler(tokenMouseHover, value); }
			remove { Events.RemoveHandler(tokenMouseHover, value); }
		}
		protected internal virtual void RaiseTokenMouseHover(TokenEditTokenMouseHoverEventArgs e) {
			TokenEditTokenMouseHoverEventHandler handler = (TokenEditTokenMouseHoverEventHandler)Events[tokenMouseHover];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditBeforeShowPopupPanel"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditBeforeShowPopupPanelEventHandler BeforeShowPopupPanel {
			add { Events.AddHandler(beforeShowPopupPanel, value); }
			remove { Events.RemoveHandler(beforeShowPopupPanel, value); }
		}
		protected internal virtual void RaiseBeforeShowPopupPanel(TokenEditBeforeShowPopupPanelEventArgs e) {
			TokenEditBeforeShowPopupPanelEventHandler handler = (TokenEditBeforeShowPopupPanelEventHandler)Events[beforeShowPopupPanel];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditTokenCheckStateChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler TokenCheckStateChanged {
			add { Events.AddHandler(tokenCheckStateChanged, value); }
			remove { Events.RemoveHandler(tokenCheckStateChanged, value); }
		}
		protected internal virtual void RaiseTokenCheckStateChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[tokenCheckStateChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditSelectedItemsChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event ListChangedEventHandler SelectedItemsChanged {
			add { Events.AddHandler(selectedItemsChanged, value); }
			remove { Events.RemoveHandler(selectedItemsChanged, value); }
		}
		protected internal virtual void RaiseSelectedItemsChanged(ListChangedEventArgs e) {
			ListChangedEventHandler handler = (ListChangedEventHandler)Events[selectedItemsChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTokenEditBeforeShowMenu"),
#endif
 DXCategory(CategoryName.Events)]
		public event BeforeShowMenuEventHandler BeforeShowMenu {
			add { Events.AddHandler(beforeShowMenu, value); }
			remove { Events.RemoveHandler(beforeShowMenu, value); }
		}
		protected internal virtual void RaiseBeforeShowMenu(BeforeShowMenuEventArgs e) {
			BeforeShowMenuEventHandler handler = (BeforeShowMenuEventHandler)Events[beforeShowMenu];
			if(handler != null) handler(GetEventSender(), e);
		}
		#region Hidden & Obsolete
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoHeight {
			get { return base.AutoHeight; }
			set { base.AutoHeight = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowSeparators {
			get { return false; }
			set { }
		}
		#endregion
		[Browsable(false)]
		public override string EditorTypeName { get { return "TokenEdit"; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Tokens != null) {
					Tokens.ListChanged -= OnTokenListChanged;
				}
				if(SelectedItems != null) {
					SelectedItems.ListChanged -= OnSelectedItemListChanged;
				}
				if(CheckedItems != null) {
					CheckedItems.ListChanged -= OnCheckedItemListChanged;
				}
				if(CustomDropDownControl != null) {
					CustomDropDownControl.Dispose();
				}
				if(Validator != null) {
					Validator.Dispose();
				}
				this.validator = null;
				this.popupPanel = null;
				this.customDropDownControl = null;
			}
			base.Dispose(disposing);
		}
	}
	public class TokenEditStringFormatter {
		RepositoryItemTokenEdit properties;
		public TokenEditStringFormatter(RepositoryItemTokenEdit properties) {
			this.properties = properties;
		}
		public string GetString(ICollection col) {
			StringBuilder sb = new StringBuilder();
			int count = 0;
			foreach(TokenEditToken token in col) {
				sb.Append(DoFormatItemLine(token, count++ == col.Count - 1));
			}
			return sb.ToString();
		}
		protected virtual string DoFormatItemLine(TokenEditToken token, bool finalItem) {
			if(finalItem) return token.Value.ToString();
			return string.Format("{0}{1} ", token.Value.ToString(), GetSeparator());
		}
		protected char GetSeparator() {
			return Properties.EditValueSeparatorChar;
		}
		public RepositoryItemTokenEdit Properties { get { return properties; } }
	}
	#region Obsolete
	[Obsolete("This class is obsolete. Use the BindingList class instead.", false)]
	public class TokenEditObjectList : System.Collections.ObjectModel.Collection<object> {
		public TokenEditObjectList() { }
		public TokenEditObjectList(IEnumerable list) {
			AddRange(list);
		}
		public void BeginUpdate() { }
		public void EndUpdate() { }
		public void AddRange(IEnumerable col) {
			BeginUpdate();
			try {
				foreach(object item in col) {
					Add(item);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void InsertItem(int index, object item) {
			base.InsertItem(index, item);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}
		protected override void RemoveItem(int index) {
			base.RemoveItem(index);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}
		protected override void SetItem(int index, object item) {
			base.SetItem(index, item);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		protected override void ClearItems() {
			base.ClearItems();
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected virtual void OnListChanged(ListChangedEventArgs e) {
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
	}
	#endregion
}
namespace DevExpress.XtraEditors {
	public enum TokenEditValueType {
		String, List, Enum
	}
	public enum TokenEditPopupFilterMode {
		StartWith, Contains
	}
	public enum TokenEditDropDownShowMode {
		Default, Regular, Outlook
	}
	public enum TokenEditMode {
		Default, TokenList, Manual
	}
	public enum TokenEditAutoHeightMode {
		Default,
		Expand,
		RestrictedExpand
	}
	public enum TokenEditTokenPopupateMode {
		Default,
		DisableAutoPopulate
	}
	public enum TokenEditPopupPanelShowMode {
		Default,
		ShowOnTokenMouseHover
	}
	public enum TokenEditPopupPanelLocation {
		Default,
		AboveToken,
		BelowToken,
		OnMouseCursor
	}
	public enum TokenEditCheckMode {
		Default,
		Single,
		Multiple
	}
	public enum TokenEditGlyphLocation {
		Default,
		Left,
		Right
	}
	[Flags]
	public enum TokenEditValidationRules {
		ValidateOnLostFocus = 0x1,
		ValidateOnSeparatorInput = 0x2,
		Default = All,
		All = ValidateOnLostFocus | ValidateOnSeparatorInput
	}
	#region Delegates & EventArgs
	public abstract class TokenEditTokenBasedEventArgsBase : EventArgs {
		string description;
		object valueCore;
		Rectangle bounds;
		public TokenEditTokenBasedEventArgsBase(string description, object value, Rectangle bounds) {
			this.description = description;
			this.valueCore = value;
			this.bounds = bounds;
		}
		internal static T Create<T>(TokenEditTokenInfo tokenInfo) where T : TokenEditTokenBasedEventArgsBase {
			object value = (tokenInfo != null && tokenInfo.Token != null) ? tokenInfo.Token.Value : null;
			return Activator.CreateInstance(typeof(T), new object[] { tokenInfo.Token.Description, value, tokenInfo.Bounds }) as T;
		}
		public override string ToString() {
			return string.Format("Token: {0}, Bounds: {1}", Description, Bounds.ToString());
		}
		public string Description { get { return description; } }
		public Rectangle Bounds { get { return bounds; } }
		public object Value { get { return valueCore; } }
	}
	public class TokenEditTokenMouseEnterEventArgs : TokenEditTokenBasedEventArgsBase {
		public TokenEditTokenMouseEnterEventArgs(string description, object value, Rectangle bounds)
			: base(description, value, bounds) {
		}
	}
	public delegate void TokenEditTokenMouseEnterEventHandler(object sender, TokenEditTokenMouseEnterEventArgs e);
	public class TokenEditTokenMouseLeaveEventArgs : TokenEditTokenBasedEventArgsBase {
		public TokenEditTokenMouseLeaveEventArgs(string description, object value, Rectangle bounds)
			: base(description, value, bounds) {
		}
	}
	public delegate void TokenEditTokenMouseLeaveEventHandler(object sender, TokenEditTokenMouseLeaveEventArgs e);
	public abstract class TokenEditCustomDrawEventArgsBase : EventArgs {
		GraphicsCache cache;
		Rectangle bounds;
		bool handled;
		MethodInvoker defaultDraw;
		string description;
		object valueCore;
		TokenEditTokenInfo info;
		public TokenEditCustomDrawEventArgsBase(GraphicsCache cache, Rectangle bounds, TokenEditTokenInfo info, string description, object value) {
			this.cache = cache;
			this.info = info;
			this.handled = false;
			this.bounds = bounds;
			this.description = description;
			this.valueCore = value;
		}
		internal void SetDefaultDraw(MethodInvoker defaultDraw) {
			this.defaultDraw = defaultDraw;
		}
		public virtual Rectangle Bounds {
			get { return bounds; }
		}
		public bool Handled {
			get { return handled; }
			set {
				handled = value;
			}
		}
		public void DefaultDraw() {
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		public string Description { get { return description; } }
		public object Value { get { return valueCore; } }
		public GraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public TokenEditTokenInfo Info { get { return info; } }
	}
	public class TokenEditCustomDrawTokenBackgroundEventArgs : TokenEditCustomDrawEventArgsBase {
		public TokenEditCustomDrawTokenBackgroundEventArgs(GraphicsCache cache, Rectangle bounds, string description, object value, TokenEditTokenInfo info)
			: base(cache, bounds, info, description, value) {
		}
	}
	public delegate void TokenEditCustomDrawTokenBackgroundEventHandler(object sender, TokenEditCustomDrawTokenBackgroundEventArgs e);
	public class TokenEditCustomDrawTokenTextEventArgs : TokenEditCustomDrawEventArgsBase {
		public TokenEditCustomDrawTokenTextEventArgs(GraphicsCache cache, Rectangle bounds, string description, object value, TokenEditTokenInfo info)
			: base(cache, bounds, info, description, value) {
		}
	}
	public delegate void TokenEditCustomDrawTokenTextEventHandler(object sender, TokenEditCustomDrawTokenTextEventArgs e);
	public class TokenEditCustomDrawTokenGlyphEventArgs : TokenEditCustomDrawEventArgsBase {
		Rectangle glyphBounds;
		public TokenEditCustomDrawTokenGlyphEventArgs(GraphicsCache cache, Rectangle glyphBounds, Rectangle bounds, string description, object value, TokenEditTokenInfo info)
			: base(cache, bounds, info, description, value) {
			this.glyphBounds = glyphBounds;
		}
		public Rectangle GlyphBounds { get { return glyphBounds; } }
	}
	public delegate void TokenEditCustomDrawTokenGlyphEventHandler(object sender, TokenEditCustomDrawTokenGlyphEventArgs e);
	public class TokenEditTokenClickEventArgs : TokenEditTokenBasedEventArgsBase {
		public TokenEditTokenClickEventArgs(string description, object value)
			: base(description, value, Rectangle.Empty) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Rectangle Bounds { get { return Rectangle.Empty; } }
	}
	public delegate void TokenEditTokenClickEventHandler(object sender, TokenEditTokenClickEventArgs e);
	public class TokenEditTokenAddedEventArgs : EventArgs {
		TokenEditToken token;
		public TokenEditTokenAddedEventArgs(TokenEditToken token) {
			this.token = token;
		}
		public TokenEditToken Token { get { return token; } }
	}
	public delegate void TokenEditTokenAddedEventHandler(object sender, TokenEditTokenAddedEventArgs e);
	public class TokenEditTokenAddingEventArgs : TokenEditTokenAddedEventArgs {
		bool cancel;
		public TokenEditTokenAddingEventArgs(TokenEditToken token)
			: base(token) {
			this.cancel = false;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void TokenEditTokenAddingEventHandler(object sender, TokenEditTokenAddingEventArgs e);
	public class TokenEditTokenRemovedEventArgs : EventArgs {
		TokenEditToken token;
		public TokenEditTokenRemovedEventArgs(TokenEditToken token) {
			this.token = token;
		}
		public TokenEditToken Token { get { return token; } }
	}
	public delegate void TokenEditTokenRemovedEventHandler(object sender, TokenEditTokenRemovedEventArgs e);
	public class TokenEditTokenRemovingEventArgs : TokenEditTokenRemovedEventArgs {
		bool cancel;
		public TokenEditTokenRemovingEventArgs(TokenEditToken token)
			: base(token) {
			this.cancel = false;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void TokenEditTokenRemovingEventHandler(object sender, TokenEditTokenRemovingEventArgs e);
	public class TokenEditValidateTokenEventArgs : EventArgs {
		string description;
		bool isValid;
		object valueCore;
		public TokenEditValidateTokenEventArgs(string description) {
			this.description = description;
			this.isValid = false;
			this.valueCore = null;
		}
		public string Description { get { return description; } set { description = value; } }
		public object Value { get { return valueCore; } set { valueCore = value; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
	}
	public delegate void TokenEditValidateTokenEventHandler(object sender, TokenEditValidateTokenEventArgs e);
	public class TokenEditTokenMouseHoverEventArgs : TokenEditTokenBasedEventArgsBase {
		public TokenEditTokenMouseHoverEventArgs(string description, object value, Rectangle bounds)
			: base(description, value, bounds) {
		}
	}
	public delegate void TokenEditTokenMouseHoverEventHandler(object sender, TokenEditTokenMouseHoverEventArgs e);
	public class TokenEditBeforeShowPopupPanelEventArgs : TokenEditTokenBasedEventArgsBase {
		public TokenEditBeforeShowPopupPanelEventArgs(string description, object value, Rectangle bounds)
			: base(description, value, bounds) {
		}
	}
	public delegate void TokenEditBeforeShowPopupPanelEventHandler(object sender, TokenEditBeforeShowPopupPanelEventArgs e);
	#endregion
	[
	DXToolboxItem(DXToolboxItemKind.Free),
	Designer("DevExpress.XtraEditors.Design.TokenEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	SmartTagAction(typeof(TokenEditActions), "EditTokenEditTokens", "Edit Tokens...", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(TokenEditActions), "EditTokenEditSeparators", "Edit Separators...", SmartTagActionType.CloseAfterExecute),
	DefaultEvent("SelectedItemsChanged"),
	ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "TokenEdit")
	]
	public class TokenEdit : BaseEdit, IPopupControl, IMouseWheelSupport, IDXMenuSupport {
		TokenEditMaskBox maskBox;
		TokenEditHandler handler;
		int topRow, lockScrollUpdate;
		bool isTextEditorActive;
		bool tabStop;
		BaseTokenEditPopupController popupController;
		PopupPanelController popupPanelController;
		TokenEditListChangeListener listChangeListener;
		TokenEditScrollController scrollController;
		public TokenEdit() {
			this.isTextEditorActive = AllowFocusTextEditorOnRaiseUp;
			this.topRow = 0;
			this.tabStop = base.TabStop;
			this.handler = CreateHandler();
			this.popupController = CreatePopupController();
			this.popupPanelController = CreatePopupPanelController();
			this.listChangeListener = CreateListChangeListener();
			this.listChangeListener.ListChanged += OnEditValueListChanged;
			CreateMaskBox();
			this.scrollController = CreateScrollController();
			this.scrollController.AddControls(this);
			this.scrollController.VScrollValueChanged += OnVScrollValueChanged;
			if(AllowFocusTextEditorOnRaiseUp) base.TabStop = false;
		}
		protected virtual bool AllowFocusTextEditorOnRaiseUp { get { return true; } }
		protected virtual TokenEditListChangeListener CreateListChangeListener() {
			return new TokenEditListChangeListener();
		}
		protected virtual TokenEditHandler CreateHandler() {
			return new TokenEditHandler(this);
		}
		protected virtual BaseTokenEditPopupController CreatePopupController() {
			if(Properties.GetEditMode() == TokenEditMode.TokenList) return new TokenEditTokenListPopupController(this);
			return new TokenEditManualModePopupController(this);
		}
		protected virtual PopupPanelController CreatePopupPanelController() {
			return new PopupPanelController(this);
		}
		protected virtual TokenEditScrollController CreateScrollController() {
			return new TokenEditScrollController(this);
		}
		protected internal BaseTokenEditPopupController PopupController { get { return popupController; } }
		protected internal PopupPanelController PopupPanelController { get { return popupPanelController; } }
		protected internal TokenEditListChangeListener ListChangeListener { get { return listChangeListener; } }
		protected internal TokenEditScrollController ScrollController { get { return scrollController; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void InitializeNewComponent() {
			if(!DesignMode) return;
			Properties.Separators.Add(",");
		}
		public virtual void ActivateTextEditor() {
			if(IsTextEditorSuppressed) return;
			this.isTextEditorActive = true;
			this.mouseDownHitCount = 0;
			Properties.CheckedItems.Clear();
			if(ViewInfo.IsVScrollVisible) ScrollToEnd();
			LayoutChanged();
			if(MaskBox != null && !MaskBox.Focused) MaskBox.Focus();
		}
		public void ScrollIntoView(TokenEditToken token) {
			if(!ViewInfo.IsVScrollVisible) return;
			TokenEditTokenInfo tokInfo = ViewInfo.GetTokenInfo(token);
			if(tokInfo == null) return;
			int row = Math.Max(0, TopRow + tokInfo.RowIndex - ViewInfo.VisibleRowCount + 1);
			ScrollIntoView(row);
		}
		public void Validate() { DoValidateToken(); }
		protected internal void ScrollIntoView(int row) {
			if(!ViewInfo.IsVScrollVisible) return;
			SetTopRow(row);
		}
		protected internal void ScrollToEnd() {
			if(!ViewInfo.IsVScrollVisible) return;
			SetTopRow(ViewInfo.CalcMaxTopRow());
		}
		public virtual void CloseTextEditor(bool resetText) {
			if(!this.isTextEditorActive) return;
			this.isTextEditorActive = false;
			LayoutChanged();
			if(resetText) ResetEditorText();
		}
		public override bool AllowMouseClick(Control control, Point p) {
			if(base.AllowMouseClick(control, p)) return true;
			return IsPopupOpen ? Popup.AllowMouseClick(control, p) : false;
		}
		protected void CheckTextEditor() {
			if(!IsTextEditorActive) return;
			if(IsTextEditorSuppressed) {
				CloseTextEditor(true);
				if(SelectedItems.Count > 0) CheckedItems.Set(SelectedItems.LastToken);
			}
		}
		public override bool IsEditorActive {
			get {
				if(!this.Enabled)
					return false;
				IContainerControl container = GetContainerControl();
				if(container == null) return EditorContainsFocus;
				return container.ActiveControl == this || (MaskBox != null && container.ActiveControl == MaskBox);
			}
		}
		protected internal bool IsTextEditorSuppressed {
			get {
				if(!CanAddNewToken() && SelectedItems.Count > 0) return true;
				return false;
			}
		}
		protected virtual void OnVScrollValueChanged(object sender, EventArgs e) {
			TopRow = ScrollController.VScrollPosition;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int TopRow {
			get { return topRow; }
			set {
				if(value < 0) value = 0;
				if(TopRow == value)
					return;
				topRow = value;
				LayoutChanged();
			}
		}
		public void CheckItem(object value) {
			TokenEditToken token = Properties.GetObjectCache()[value];
			if(token != null)
				Properties.CheckedItems.SetInternal(token);
		}
		public void SelectItem(object value) {
			TokenEditToken token = Properties.GetObjectCache()[value];
			if(token != null && !Properties.SelectedItems.Contains(token)) {
				Properties.SelectedItems.Set(token);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TokenEditToken CheckedItem {
			get { return Properties.CheckedItems.Count != 0 ? Properties.CheckedItems[0] : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TokenEditSelectedItemCollection SelectedItems {
			get { return Properties.SelectedItems; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TokenEditCheckedItemCollection CheckedItems {
			get { return Properties.CheckedItems; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TokenEdit Client { get { return this; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string EditText {
			get {
				if(SelectedItems.Count == 0) return string.Empty;
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < SelectedItems.Count; i++) {
					TokenEditToken tok = SelectedItems[i];
					if(!string.IsNullOrEmpty(tok.Description)) {
						sb.Append(tok.Description);
						if(i != SelectedItems.Count - 1) sb.Append(Properties.EditValueSeparatorChar + " ");
					}
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public bool HasTokens {
			get { return SelectedItems.Count > 0; }
		}
		public TokenEditSelectedItemCollection GetTokenList() {
			return Properties.SelectedItems;
		}
		protected void DoRestoreTabStop() {
			base.TabStop = TabStop;
		}
		protected internal override void LayoutChanged() {
			if(IsDisposing)
				return;
			base.LayoutChanged();
			if(MaskBox != null && IsHandleCreated && !IsLayoutLocked) {
				if(ViewInfo.AllowMaskBox) {
					UpdateMaskBox();
					MaskBox.Bounds = ViewInfo.IsReady ? ViewInfo.MaskBoxRect : Rectangle.Empty;
					MaskBox.Visible = true;
					if(IsEditorContainsFocus) base.TabStop = false;
				}
				else {
					if(IsEditorContainsFocus) {
						DoRestoreTabStop();
						MaskBox.Visible = false;
					}
					MaskBox.Bounds = Rectangle.Empty;
				}
				CheckAutoHeight();
			}
			CheckTopRow();
			UpdateScrollBars();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			if(Enabled) {
				DoShowMaskBox();
			}
			else {
				DoHideMaskBox();
			}
		}
		protected virtual void DoShowMaskBox() {
			if(MaskBox == null || MaskBox.Visible) return;
			MaskBox.Visible = true;
		}
		protected virtual void DoHideMaskBox() {
			DoRestoreTabStop();
			MaskBox.Visible = false;
		}
		protected bool IsEditorContainsFocus {
			get { return Focused || (MaskBox != null && MaskBox.Focused); }
		}
		protected virtual void CheckTopRow() {
			int row = ViewInfo.IsVScrollVisible ? TopRow : 0;
			SetTopRow(row);
		}
		protected internal void SetTopRow(int value) {
			int row = ViewInfo.IsVScrollVisible ? CheckTopRowRange(value) : 0;
			if(TopRow != row) TopRow = row;
		}
		protected int CheckTopRowRange(int topRow) {
			int row = Math.Max(0, topRow);
			row = Math.Min(topRow, ViewInfo.CalcMaxTopRow());
			return row;
		}
		protected virtual void UpdateScrollBars() {
			if(this.lockScrollUpdate != 0) return;
			this.lockScrollUpdate++;
			try {
				ScrollController.VScrollVisible = ViewInfo.IsVScrollVisible;
				ScrollController.ClientRect = ViewInfo.ClientRect;
				if(ScrollController.VScrollVisible) ScrollController.VScrollArgs = ViewInfo.CalcVScrollArgs();
			}
			finally {
				this.lockScrollUpdate--;
			}
		}
		protected virtual void UpdateMaskBox() {
			MaskBox.SuspendLayout();
			try {
				UpdateMaskBoxProperties();
			}
			finally {
				MaskBox.ResumeLayout();
			}
		}
		protected virtual void UpdateMaskBoxProperties() {
			Color maskBackColor = Color.FromArgb(255, ViewInfo.PaintAppearance.BackColor);
			if(MaskBox.ForeColor != ViewInfo.PaintAppearance.ForeColor) MaskBox.ForeColor = ViewInfo.PaintAppearance.ForeColor;
			if(MaskBox.BackColor != maskBackColor) MaskBox.BackColor = maskBackColor;
			if(!MaskBox.Font.Equals(ViewInfo.PaintAppearance.Font)) {
				MaskBox.Font = ViewInfo.PaintAppearance.Font;
			}
			MaskBox.ReadOnly = Properties.ReadOnly;
			if(MaskBox.ContextMenuStrip != Properties.ContextMenuStrip) MaskBox.ContextMenuStrip = Properties.ContextMenuStrip;
		}
		protected override void Select(bool directed, bool forward) {
			IContainerControl container = base.GetContainerControl();
			if(container != null) {
				if(MaskBox != null && MaskBox.Visible) {
					container.ActiveControl = MaskBox;
				}
				else {
					container.ActiveControl = this;
				}
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			RaiseSizeableChanged();
		}
		protected internal virtual void OnEditModeChanged() {
			BaseTokenEditPopupController prev = PopupController;
			if(prev != null) prev.Dispose();
			this.popupController = CreatePopupController();
		}
		#region IDXMenuSupport
		event DXMenuWndProcHandler wndProcHandler;
		void IDXMenuSupport.ShowMenu(Point pos) {
			ShowMenu(pos);
		}
		DXPopupMenu IDXMenuSupport.Menu { get { return Menu; } }
		event DXMenuWndProcHandler IDXMenuSupport.WndProc {
			add { wndProcHandler += value; }
			remove { if(wndProcHandler != null) wndProcHandler -= value; }
		}
		#endregion
		#region DXMenu
		protected internal virtual bool ShowMenu(Point pt) {
			if(Properties.ContextMenu != null || MaskBox == null || Properties.ContextMenuStrip != null) return false;
			UpdateMenu();
			BeforeShowMenuEventArgs e = new BeforeShowMenuEventArgs(Menu, pt);
			Properties.RaiseBeforeShowMenu(e);
			pt = e.Location;
			MenuManagerHelper.ShowMenu(Menu, Properties.LookAndFeel, MenuManager, this, pt);
			return true;
		}
		protected virtual void UpdateMenu() {
			foreach(DXMenuItem item in Menu.Items) {
				DXTokenEditMenuItem tokenEditItem = item as DXTokenEditMenuItem;
				if(tokenEditItem != null) {
					tokenEditItem.Update();
				}
			}
		}
		DXPopupMenu menu = null;
		protected virtual DXPopupMenu Menu {
			get {
				if(menu == null) {
					menu = CreateMenu();
				}
				return menu;
			}
		}
		protected virtual DXPopupMenu CreateMenu() {
			DXPopupMenu result = new DXPopupMenu();
			DXTokenEditMenuItem item;
			item = new DXTokenEditMenuItem(StringId.TextEditMenuUndo, (sender, e) => MaskBox.MaskBoxUndo(), () => !Properties.ReadOnly && MaskBox.MaskBoxCanUndo, MenuImageList.Images[0]);
			result.Items.Add(item);
			item = new DXTokenEditMenuItem(StringId.TextEditMenuCut, (sender, e) => MaskBox.Cut(), () => MaskHasSelectedText(), MenuImageList.Images[1]);
			item.BeginGroup = true;
			result.Items.Add(item);
			item = new DXTokenEditMenuItem(StringId.TextEditMenuCopy, (sender, e) => MaskBox.Copy(), () => MaskHasSelectedText(), MenuImageList.Images[2]);
			result.Items.Add(item);
			item = new DXTokenEditMenuItem(StringId.TextEditMenuPaste, (sender, e) => MaskBox.Paste(), () => !Properties.ReadOnly && GetClipboardText().Length > 0, MenuImageList.Images[3]);
			result.Items.Add(item);
			item = new DXTokenEditMenuItem(StringId.TextEditMenuDelete, (senders, e) => MaskBox.SendMaskBoxMsg(0x303), () => MaskHasSelectedText(), MenuImageList.Images[4]);
			result.Items.Add(item);
			item = new DXTokenEditMenuItem(StringId.TextEditMenuSelectAll, (sender, e) => MaskBox.MaskBoxSelectAll(), () => !MaskBox.GetIsSelectAllMode(), null);
			item.BeginGroup = true;
			result.Items.Add(item);
			return result;
		}
		protected string GetClipboardText() {
			return DevExpress.XtraEditors.Mask.MaskBox.GetClipboardText();
		}
		protected bool MaskHasSelectedText() {
			if(Properties.ReadOnly) return false;
			return MaskBox != null && MaskBox.MaskBoxSelectedText.Length > 0;
		}
		[ThreadStatic]
		static ImageCollection menuImageList = null;
		static ImageCollection MenuImageList {
			get {
				if(menuImageList == null) {
					menuImageList = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.TextEditMenu.png", typeof(TokenEdit).Assembly, new Size(16, 16), Color.Empty);
				}
				return menuImageList;
			}
		}
		#region DXTokenEditMenuItem
		protected class DXTokenEditMenuItem : DXMenuItem {
			Func<bool> updateHandler;
			public DXTokenEditMenuItem(StringId id, EventHandler click, Func<bool> updateHandler, Image image) : base(Localizer.Active.GetLocalizedString(id), click, image) {
				this.updateHandler = updateHandler;
			}
			public void Update() {
				if(this.updateHandler != null) Enabled = updateHandler();
			}
		}
		#endregion
		#endregion
		[Browsable(false)]
		public bool IsTextEditorActive { get { return isTextEditorActive; } }
		protected override void OnEditValueChanging(ChangingEventArgs e) {
			if(e.OldValue != null && !CompareEditValue(e.OldValue, e.NewValue, false)) {
				ListChangeListener.Unsubscribe(e.OldValue);
			}
			base.OnEditValueChanging(e);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			ListChangeListener.Subscribe(EditValue);
			UpdateSelectedItems();
		}
		protected virtual void OnEditValueListChanged(object sender, EventArgs e) {
			UpdateSelectedItems();
			if(IsEditValueBound) RaiseEditValueChanged(); 
		}
		protected bool IsEditValueBound {
			get {
				foreach(Binding binding in DataBindings) {
					if(string.Equals(binding.PropertyName, "EditValue", StringComparison.Ordinal)) return true;
				}
				return false;
			}
		}
		[Browsable(false)]
		public TokenEditHandler Handler { get { return handler; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new TokenEditViewInfo GetViewInfo() {
			return ViewInfo;
		}
		public TokenEditHitInfo CalcHitInfo(int x, int y) {
			return CalcHitInfo(new Point(x, y));
		}
		public TokenEditHitInfo CalcHitInfo(Point point) {
			return ViewInfo.CalcHitInfo(point) as TokenEditHitInfo;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditTabStop"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public override bool TabStop {
			get { return tabStop; }
			set {
				if(TabStop == value)
					return;
				tabStop = value;
				OnTabStopChanged(EventArgs.Empty);
			}
		}
		protected override void OnTabStopChanged(EventArgs e) {
			base.OnTabStopChanged(e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemTokenEdit Properties { get { return base.Properties as RepositoryItemTokenEdit; } }
		bool updateSelectedItems = false;
		protected void UpdateSelectedItems() { 
			if(IsLoading || this.updateEditValue) return;
			this.updateSelectedItems = true;
			try {
				Properties.DoUpdateSelectedItems(EditValue);
			}
			finally {
				this.updateSelectedItems = false;
			}
		}
		bool updateEditValue = false;
		protected internal virtual void UpdateEditValue() { 
			if(IsLoading || this.updateSelectedItems) return;
			this.updateEditValue = true;
			try {
				EditValue = Properties.DoUpdateEditValue(EditValue);
			}
			finally {
				this.updateEditValue = false;
			}
		}
		protected internal override void OnLoaded() {
			base.OnLoaded();
			if(!IsNullValue(EditValue)) UpdateSelectedItems();
		}
		protected internal new virtual TokenEditViewInfo ViewInfo { get { return base.ViewInfo as TokenEditViewInfo; } }
		protected override void OnMouseEnter(EventArgs e) {
			Handler.OnMouseEnter(e);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			Handler.OnMouseDoubleClick(e);
			base.OnMouseDoubleClick(e);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			Handler.OnEditorKeyDown(e);
			base.OnEditorKeyDown(e);
		}
		protected bool IsTabPressed { get { return (NativeMethods.GetAsyncKeyState((int)Keys.Tab) & 1) != 0; } }
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if(ContainsFocus && (SelectedItems.IsEmpty || IsTabPressed)) ActivateTextEditor();
		}
		int mouseDownHitCount = 0;
		protected virtual void OnMaskBoxKeyDown(object sender, KeyEventArgs e) {
			bool handled = false;
			bool isRepeat = (++this.mouseDownHitCount > 1);
			if(MaskBox.Empty) {
				if(e.KeyCode == Keys.Left) handled = OnMaskBoxLeftKeyDown();
				if(e.KeyCode == Keys.Home) handled = OnMaskBoxHomeKeyDown();
				if(e.KeyCode == Keys.Back && !isRepeat) handled = OnMaskBoxBackKeyDown();
			}
			if(handled) return;
			if(IsPopupOpen) {
				Popup.ProcessKeyDown(e);
			}
			else {
				OnKeyDown(e);
			}
		}
		protected virtual bool OnMaskBoxLeftKeyDown() {
			CloseTextEditor(true);
			if(SelectedItems.LastToken != null) Properties.CheckedItems.Set(SelectedItems.LastToken);
			return true;
		}
		protected virtual bool OnMaskBoxHomeKeyDown() {
			CloseTextEditor(true);
			if(SelectedItems.FirstToken != null) Properties.CheckedItems.Set(SelectedItems.FirstToken);
			return true;
		}
		protected virtual bool OnMaskBoxBackKeyDown() {
			if(SelectedItems.LastToken != null && !ReadOnly) DoRemoveToken(SelectedItems.LastToken);
			return true;
		}
		protected virtual void OnMaskBoxKeyUp(object sender, KeyEventArgs e) {
			this.mouseDownHitCount = 0;
			OnKeyUp(e);
		}
		void OnMaskBoxKeyPress(object sender, KeyPressEventArgs e) {
			OnKeyPress(e);
			if(e.KeyChar == '\r' || e.KeyChar == '\n') {
				e.Handled = true;
			}
		}
		protected void OnMaskBoxChar(object sender, TokenEditMaskBoxCharEventArgs e) {
			if(IsPopupOpen) e.Handled = PopupController.OnMaskBoxChar(e.KeyChar);
			if(e.Handled) return;
			if(Properties.AllowValidateOnSeparatorInput && Properties.IsSeparatorChar(e.KeyChar)) {
				e.Handled = DoValidateToken();
			}
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if(fDisposing) return;
			DoRestoreTabStop();
			Handler.OnLostFocus();
		}
		protected virtual void OnMaskBoxGotFocus(object sender, EventArgs e) {
			this.isTextEditorActive = true;
			LayoutChanged();
		}
		protected virtual bool ResetTextEditorOnLostFocus { get { return true; } }
		protected void OnMaskBoxLostFocus(object sender, EventArgs e) {
			if(Properties.AllowValidateOnLostFocus) {
				DoValidateToken();
				if(ResetTextEditorOnLostFocus && !Focused) CloseTextEditor(true);
			}
			if(!Focused) DoCheckCheckedItemsOnLostFocus();
			DoRestoreTabStop();
		}
		protected internal virtual void DoCheckCheckedItemsOnLostFocus() {
			if(Properties.AllowClearCheckStatesOnLostFocus()) Properties.CheckedItems.Clear();
		}
		protected internal virtual void DoRemoveToken(TokenEditToken token) {
			if(DoRaiseTokenRemoving(token)) return;
			try {
				SelectedItems.Remove(token);
			}
			finally {
				DoRaiseTokenRemoved(token);
			}
		}
		protected virtual bool DoRaiseTokenAdding(TokenEditToken token) {
			if(Properties.RaiseTokenAdding(new TokenEditTokenAddingEventArgs(token))) return true;
			if(InplaceType == InplaceType.Standalone && Properties.HasEditValueChangingSubscribers) {
				TokenEditSelectedItemCollection col = SelectedItems.Clone();
				col.Set(token);
				object newValue = Properties.GetMapper().UpdateEditValue(Properties, col, null);
				ChangingEventArgs e = new ChangingEventArgs(EditValue, newValue);
				Properties.RaiseEditValueChanging(e);
				return e.Cancel;
			}
			return false;
		}
		protected virtual void DoRaiseTokenAdded(TokenEditToken token) {
			Properties.RaiseTokenAdded(new TokenEditTokenAddingEventArgs(token));
		}
		protected virtual bool DoRaiseTokenRemoving(TokenEditToken token) {
			if(Properties.RaiseTokenRemoving(new TokenEditTokenRemovingEventArgs(token))) return true;
			if(InplaceType == InplaceType.Standalone && Properties.HasEditValueChangingSubscribers) {
				TokenEditSelectedItemCollection col = SelectedItems.Clone();
				col.Remove(token);
				object newValue = Properties.GetMapper().UpdateEditValue(Properties, col, null);
				ChangingEventArgs e = new ChangingEventArgs(EditValue, newValue);
				Properties.RaiseEditValueChanging(e);
				return e.Cancel;
			}
			return false;
		}
		protected virtual void DoRaiseTokenRemoved(TokenEditToken token) {
			Properties.RaiseTokenRemoved(new TokenEditTokenRemovingEventArgs(token));
		}
		void OnMaskBoxMouseWheel(object sender, MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseWheel(ee);
				if(ee.Handled || Properties.ReadOnly || !Properties.AllowMouseWheel) return;
				if(IsPopupOpen) {
					Popup.OnEditorMouseWheel(e);
					ee.Handled = true;
					return;
				}
				Handler.OnMouseWheelCore(e);
			}
			finally {
				ee.Sync();
			}
		}
		bool resetEditorText = false;
		protected internal void ResetEditorText() {
			this.resetEditorText = true;
			try {
				MaskBox.ResetText();
			}
			finally {
				this.resetEditorText = false;
			}
		}
		protected void OnMaskBoxTextChanged(object sender, EventArgs e) {
			if(this.resetEditorText) return;
			PopupController.OnTextChanged();
		}
		protected void OnMaskBoxLocationChanged(object sender, EventArgs e) {
			if(this.resetEditorText) return;
		}
		protected internal bool DoValidateToken() {
			string maskText = MaskBox.MaskBoxText;
			if(string.IsNullOrEmpty(maskText)) return false;
			bool valid = Properties.Validator.DoValidate(maskText);
			if(valid) {
				ResetEditorText();
			}
			return valid;
		}
		protected override void OnMouseHover(EventArgs e) {
			Handler.OnMouseHover();
			base.OnMouseHover(e);
		}
		#region IMouseWheelSupport
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		#endregion
		#region Maskbox
		protected virtual TokenEditMaskBox CreateMaskBoxInstance() {
			return new TokenEditMaskBox(this);
		}
		protected virtual void CreateMaskBox() {
			if(MaskBox != null) DestroyMaskBox();
			this.maskBox = CreateMaskBoxInstance();
			MaskBox.TabStop = AllowFocusTextEditorOnRaiseUp;
			MaskBox.Visible = false;
			MaskBox.KeyDown += OnMaskBoxKeyDown;
			MaskBox.KeyUp += OnMaskBoxKeyUp;
			MaskBox.KeyPress += OnMaskBoxKeyPress;
			MaskBox.Char += OnMaskBoxChar;
			MaskBox.GotFocus += OnMaskBoxGotFocus;
			MaskBox.LostFocus += OnMaskBoxLostFocus;
			MaskBox.TextChanged += OnMaskBoxTextChanged;
			MaskBox.LocationChanged += OnMaskBoxLocationChanged;
			MaskBox.MouseWheel += OnMaskBoxMouseWheel;
			MaskBox.Font = Font;
			Controls.Add(MaskBox);
		}
		protected virtual void DestroyMaskBox() {
			if(MaskBox == null) return;
			TokenEditMaskBox mask = MaskBox;
			MaskBox.KeyDown -= OnMaskBoxKeyDown;
			MaskBox.KeyUp -= OnMaskBoxKeyUp;
			MaskBox.KeyPress -= OnMaskBoxKeyPress;
			MaskBox.Char -= OnMaskBoxChar;
			MaskBox.GotFocus -= OnMaskBoxGotFocus;
			MaskBox.LostFocus -= OnMaskBoxLostFocus;
			MaskBox.TextChanged -= OnMaskBoxTextChanged;
			MaskBox.LocationChanged -= OnMaskBoxLocationChanged;
			MaskBox.MouseWheel -= OnMaskBoxMouseWheel;
			this.maskBox = null;
			mask.Dispose();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual TokenEditMaskBox MaskBox { get { return maskBox; } }
		#endregion
		#region Popup Form
		public void ShowPopup() {
			DoShowPopup(true);
		}
		bool isPopupOpen = false;
		protected internal virtual void DoShowPopup(bool resetFilter) {
			if(resetFilter) {
				PopupController.UpdateFilter(string.Empty);
			}
			Size size = CalcPopupFormSize();
			if(size.IsEmpty) return;
			this.isPopupOpen = true;
			Rectangle bounds = CalcPopupFormBounds(size);
			Popup.ClientSize = bounds.Size;
			Popup.Location = bounds.Location;
			Popup.ForceCreateHandle();
			Popup.UpdateViewInfo();
			if(Parent != null) Parent.VisibleChanged += OnParentVisibleChanged;
			Popup.ShowPopupForm();
			ServiceObject.PopupShowing(this);
			PopupController.DoShowPopup();
			if(Popup != null) {
				Form parentForm = FindForm();
				if(parentForm != null) parentForm.AddOwnedForm(Popup);
			}
			RefreshVisualLayout();
		}
		public virtual void ClosePopup(PopupCloseMode closeMode) {
			if(!IsPopupOpen) return;
			PopupController.DoClosePopup(closeMode);
			if(Parent != null) Parent.VisibleChanged -= OnParentVisibleChanged;
			Popup.HidePopupForm();
			this.isPopupOpen = false;
			ServiceObject.PopupClosed(this);
			RefreshVisualLayout();
			Form parentForm = FindForm();
			if(Popup != null && parentForm != null) {
				parentForm.RemoveOwnedForm(Popup);
			}
			if(parentForm != null && (Popup == Form.ActiveForm)) parentForm.Activate();
			bool editValueUpdated = false;
			if(AllowUpdateEditValue(closeMode)) {
				editValueUpdated = UpdateEditValueOnClosePopup();
			}
			if(Properties.EditMode != TokenEditMode.Manual || editValueUpdated) ResetEditorText();
			CheckTextEditor();
		}
		protected internal virtual void TogglePopup(PopupCloseMode popupCloseMode) {
			if(IsPopupOpen) {
				ClosePopup(popupCloseMode);
			}
			else {
				ShowPopup();
			}
		}
		protected internal virtual bool UpdateEditValueOnClosePopup() {
			TokenEditToken token = Popup.QueryResultValue() as TokenEditToken;
			if(token == null) return false;
			return DoUpdateEditValueOnClosePopup(token);
		}
		protected internal virtual bool DoUpdateEditValueOnClosePopup(TokenEditToken token) {
			if(DoRaiseTokenAdding(token)) return false;
			try {
				SelectedItems.Set(token);
			}
			finally {
				DoRaiseTokenAdded(token);
			}
			if(IsTextEditorActive) ScrollToEnd();
			LayoutChanged();
			return true;
		}
		protected virtual bool AllowUpdateEditValue(PopupCloseMode closeMode) {
			return closeMode == PopupCloseMode.Normal;
		}
		protected virtual bool CanAddNewToken() {
			if(Properties.GetEditMode() == TokenEditMode.TokenList) {
				if(ViewInfo.TokenCount == Properties.Tokens.Count) return false;
			}
			if(Properties.MaxTokenCount > -1 && ViewInfo.TokenCount >= Properties.MaxTokenCount) return false;
			return true;
		}
		protected internal bool CanShowDropDown() {
			if(!CanAddNewToken()) return false;
			return Properties.ShowDropDown;
		}
		protected virtual Rectangle CalcPopupFormBounds(Size size) {
			return GetDropDownBoundsCalculator().CalcBounds(this, size);
		}
		protected internal virtual void UpdatePopupFormSize(int itemCount) {
			if(!IsPopupOpen || Properties.GetDropDownShowMode() == TokenEditDropDownShowMode.Regular || itemCount == 0) return;
			Size newSize = CalcPopupFormSize();
			if(newSize != Popup.ClientSize) {
				Popup.ClientSize = newSize;
			}
		}
		protected internal virtual void UpdatePopupFormHeight() {
			if(IsPopupOpen) Popup.ClientSize = new Size(Popup.ClientSize.Width, CalcPopupFormSize().Height);
		}
		protected internal virtual TokenEditDropDownBoundsCalculatorBase GetDropDownBoundsCalculator() {
			if(Properties.GetDropDownShowMode() == TokenEditDropDownShowMode.Regular) return new TokenEditRegularDropDownBoundsCalculator();
			return new TokenEditOutlookDropDownBoundsCalculator();
		}
		protected virtual Size CalcPopupFormSize() {
			return Popup.CalcFormSize();
		}
		void OnParentVisibleChanged(object sender, EventArgs e) {
			ClosePopup(PopupCloseMode.Cancel);
		}
		protected bool IsPopupCreated { get { return this.popupForm != null; } }
		protected internal virtual void DestroyPopupFormCore(bool dispose) {
			if(IsPopupCreated) {
				ServiceObject.PopupClosed(this);
				Form form = this.popupForm;
				this.popupForm = null;
				if(dispose) form.Dispose();
			}
		}
		[Browsable(false)]
		public bool IsPopupOpen { get { return isPopupOpen; } }
		TokenEditPopupForm popupForm;
		protected internal TokenEditPopupForm Popup {
			get {
				if(this.popupForm == null) this.popupForm = CreatePopupForm();
				return this.popupForm;
			}
		}
		protected virtual TokenEditPopupForm CreatePopupForm() {
			return new TokenEditPopupForm(this);
		}
		#endregion
		#region IPopupControl
		bool IPopupControl.AllowMouseClick(Control control, Point mousePosition) {
			if(Popup == null) return false;
			return Popup.AllowMouseClick(control, mousePosition);
		}
		Control IPopupControl.PopupWindow { get { return Popup; } }
		void IPopupControl.ClosePopup() {
			ClosePopup(PopupCloseMode.Immediate);
		}
		bool IPopupControl.SuppressOutsideMouseClick { get { return false; } }
		#endregion
		#region Popup Panel
		protected internal virtual void CheckShowPopupPanel(TokenEditTokenInfo token) {
			if(Properties.AllowPopupPanel) PopupPanelController.ShowPopupPanel(token);
		}
		protected internal void TrackMouseHoverCore() {
			if(!IsHandleCreated) return;
			NativeMethods.TRACKMOUSEEVENTStruct info = new NativeMethods.TRACKMOUSEEVENTStruct(0x1, Handle, SystemInformation.MouseHoverTime);
			NativeMethods.TrackMouseEvent(info);
		}
		#endregion
		#region Events
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditValidateToken"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditValidateTokenEventHandler ValidateToken {
			add { Properties.ValidateToken += value; }
			remove { Properties.ValidateToken -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditBeforeShowPopupPanel"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditBeforeShowPopupPanelEventHandler BeforeShowPopupPanel {
			add { Properties.BeforeShowPopupPanel += value; }
			remove { Properties.BeforeShowPopupPanel -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditCustomDrawTokenGlyph"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditCustomDrawTokenGlyphEventHandler CustomDrawTokenGlyph {
			add { Properties.CustomDrawTokenGlyph += value; }
			remove { Properties.CustomDrawTokenGlyph -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditCustomDrawTokenText"),
#endif
 DXCategory(CategoryName.Events)]
		public event TokenEditCustomDrawTokenTextEventHandler CustomDrawTokenText {
			add { Properties.CustomDrawTokenText += value; }
			remove { Properties.CustomDrawTokenText -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditSelectedItemsChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event ListChangedEventHandler SelectedItemsChanged {
			add { Properties.SelectedItemsChanged += value; }
			remove { Properties.SelectedItemsChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TokenEditTokenCheckStateChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler TokenCheckStateChanged {
			add { Properties.TokenCheckStateChanged += value; }
			remove { Properties.TokenCheckStateChanged -= value; }
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text {
			get { return string.Empty; }
			set { }
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "TokenEdit"; } }
		protected internal virtual void OnMaskBoxWndProc(ref Message msg) {
			if(wndProcHandler != null) wndProcHandler(this, ref msg);
		}
		protected override void WndProc(ref Message msg) {
			if(wndProcHandler != null) wndProcHandler(this, ref msg);
			base.WndProc(ref msg);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.wndProcHandler = null;
				DestroyMaskBox();
				if(Handler != null) {
					Handler.Dispose();
				}
				this.handler = null;
				if(PopupPanelController != null) {
					PopupPanelController.Dispose();
				}
				this.popupPanelController = null;
				if(ListChangeListener != null) {
					ListChangeListener.ListChanged -= OnEditValueListChanged;
				}
				this.listChangeListener = null;
				if(ScrollController != null) {
					ScrollController.VScrollValueChanged -= OnVScrollValueChanged;
					ScrollController.RemoveControls(this);
					ScrollController.Dispose();
				}
				if(this.menu != null) {
					this.menu.Dispose();
					this.menu = null;
				}
				this.scrollController = null;
			}
			base.Dispose(disposing);
		}
	}
	public class TokenEditValidationController : IDisposable {
		RepositoryItemTokenEdit properties;
		public TokenEditValidationController(RepositoryItemTokenEdit properties) {
			this.properties = properties;
		}
		public bool DoValidate(string text) {
			return DoValidateCore(text, true) != null;
		}
		public virtual TokenEditToken DoValidateCore(string text, bool setSelected) {
			TokenEditTokenCacheBase valueCache = Properties.GetObjectCache();
			if(valueCache.Contains(text) && !Properties.SelectedItems.ContainsValue(text)) {
				return Properties.SelectedItems.Set(valueCache[text]);
			}
			if(Properties.EditMode != TokenEditMode.Manual) return null;
			TokenEditValidateTokenEventArgs e = new TokenEditValidateTokenEventArgs(text);
			if(!Properties.RaiseValidateToken(e)) return null;
			if(e.Value == null) {
				e.Value = e.Description;
			}
			if(valueCache.Contains(e.Value)) return valueCache[e.Value];
			TokenEditToken tok = CreateToken(e.Description, e.Value);
			Properties.Tokens.Add(tok);
			if(setSelected) Properties.SelectedItems.Set(tok);
			return tok;
		}
		protected virtual TokenEditToken CreateToken(string description, object value) {
			return new TokenEditToken(description, value, true);
		}
		public bool AllowValidation { get { return Properties.EditMode == TokenEditMode.Manual; } }
		#region Dispose
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.properties = null;
		}
		#endregion
		public RepositoryItemTokenEdit Properties { get { return properties; } }
	}
	public class TokenEditScrollController : IDisposable {
		TokenEdit ownerEdit;
		VScrollBar vScroll;
		bool vScrollVisible;
		Rectangle clientRect, vscrollRect;
		public TokenEditScrollController(TokenEdit ownerEdit) {
			this.ownerEdit = ownerEdit;
			this.clientRect = this.vscrollRect = Rectangle.Empty;
			this.vScroll = CreateVScroll();
			this.VScroll.Visible = false;
			this.VScroll.SmallChange = 1;
			this.VScroll.LookAndFeel.ParentLookAndFeel = ownerEdit.LookAndFeel;
			ScrollBarBase.ApplyUIMode(VScroll);
		}
		protected virtual VScrollBar CreateVScroll() { return new VScrollBar(); }
		public virtual void AddControls(Control container) {
			if(container == null) return;
			container.Controls.Add(VScroll);
		}
		public virtual void RemoveControls(Control container) {
			if(container == null) return;
			container.Controls.Remove(VScroll);
		}
		public virtual int VScrollWidth {
			get { return VScroll.GetDefaultVerticalScrollBarWidth(); }
		}
		public int VScrollPosition { get { return VScroll.Value; } }
		public VScrollBar VScroll { get { return vScroll; } }
		public TokenEdit OwnerEdit { get { return ownerEdit; } }
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public Rectangle VScrollRect { get { return vscrollRect; } }
		public int VScrollMaximum { get { return VScroll.Maximum; } }
		public int VScrollLargeChange { get { return VScroll.LargeChange; } }
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				if(VScrollVisible == value) UpdateVisibility();
				else {
					vScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		protected virtual void CalcRects() {
			this.vscrollRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			if(VScrollVisible) {
				int x = ClientRect.Right - VScrollWidth;
				r.Location = new Point(x, ClientRect.Y);
				r.Size = new Size(VScrollWidth, ClientRect.Height);
				vscrollRect = r;
			}
		}
		[Obsolete("Use UpdateVisibility"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void UpdateVisiblity() {
			UpdateVisibility();
		}
		public void UpdateVisibility() {
			VScroll.SetVisibility(vScrollVisible && !ClientRect.IsEmpty);
			VScroll.Bounds = VScrollRect;
		}
		int lockLayout = 0;
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout++;
			try {
				CalcRects();
				UpdateVisibility();
				if(ClientRect.IsEmpty) VScroll.SetVisibility(false);
			}
			finally {
				lockLayout--;
			}
		}
		public event EventHandler VScrollValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(VScroll != null) VScroll.Dispose();
			}
		}
	}
	public class TokenEditListChangeListener {
		public event EventHandler ListChanged;
		public void Subscribe(object editValue) {
			Unsubscribe(editValue);
			IBindingList list = editValue as IBindingList;
			if(list == null) return;
			list.ListChanged += OnListChanged;
		}
		void OnListChanged(object sender, ListChangedEventArgs e) {
			RaiseListChanged();
		}
		public void Unsubscribe(object editValue) {
			IBindingList list = editValue as IBindingList;
			if(list == null) return;
			list.ListChanged -= OnListChanged;
		}
		protected virtual void RaiseListChanged() {
			if(ListChanged != null) ListChanged(this, EventArgs.Empty);
		}
	}
	#region MaskBox
	public class TokenEditMaskBox : MaskBox {
		TokenEdit ownerEdit;
		static readonly object EVENT_CHAR = new object();
		public TokenEditMaskBox(TokenEdit ownerEdit) {
			this.TabIndex = 0;
			this.BorderStyle = BorderStyle.None;
			this.AutoSize = false;
			this.Multiline = false;
			this.ownerEdit = ownerEdit;
		}
		public bool Empty { get { return MaskBoxText.Length == 0; } }
		public void SendMaskBoxMsg(int msg) {
			SendMaskBoxMsg(msg, IntPtr.Zero, IntPtr.Zero);
		}
		public void SendMaskBoxMsg(int msg, IntPtr wParam, IntPtr lParam) {
			if(IsHandleCreated) NativeMethods.SendMessage(Handle, msg, wParam, lParam);
		}
		[System.Security.SecuritySafeCritical]
		protected internal int GetCaretPos() {
			if(!IsHandleCreated) return 0;
			int pos = 0;
			IntPtr wParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
			IntPtr lParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
			try {
				Message msg = Message.Create(Handle, 0x00B0, wParamPtr, lParamPtr);
				DefWndProc(ref msg);
				pos = Marshal.ReadInt32(wParamPtr);
			}
			finally {
				if(wParamPtr != IntPtr.Zero) Marshal.FreeHGlobal(wParamPtr);
				if(lParamPtr != IntPtr.Zero) Marshal.FreeHGlobal(lParamPtr);
			}
			return pos;
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) ownerEdit.Focus();
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams res = base.CreateParams;
				res.ClassStyle |= BaseControl.CS_VREDRAW | BaseControl.CS_HREDRAW;
				return res;
			}
		}
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Tab) {
				return OwnerEdit.IsPopupOpen;
			}
			return base.IsInputKey(keyData);
		}
		public event TokenEditMaskBoxCharEventHandler Char {
			add { Events.AddHandler(EVENT_CHAR, value); }
			remove { Events.RemoveHandler(EVENT_CHAR, value); }
		}
		protected virtual bool OnChar(TokenEditMaskBoxCharEventArgs e) {
			TokenEditMaskBoxCharEventHandler handler = (TokenEditMaskBoxCharEventHandler)Events[EVENT_CHAR];
			if(handler != null) handler(this, e);
			return e.Handled;
		}
		protected virtual bool OnWmChar(ref Message msg) {
			TokenEditMaskBoxCharEventArgs e = new TokenEditMaskBoxCharEventArgs((char)msg.WParam);
			return OnChar(e);
		}
		protected Point GetPopupMenuPos(ref Message msg) {
			Point pt = new Point(msg.LParam.ToInt32());
			if(pt.X == -1 && pt.Y == -1) {
				pt = GetPositionFromCharIndex(MaskBoxSelectionStart + MaskBoxSelectionLength);
				pt.Offset(Bounds.X + 1, Bounds.Y + 16);
				return pt;
			}
			return OwnerEdit.PointToClient(Control.MousePosition);
		}
		protected override void WndProc(ref Message msg) {
			bool handled = false;
			switch(msg.Msg) {
				case MSG.WM_CHAR: handled = OnWmChar(ref msg); break;
				case MSG.WM_CONTEXTMENU:
					if(OwnerEdit != null && OwnerEdit.ShowMenu(GetPopupMenuPos(ref msg))) {
						return;
					}
					break;
			}
			if(handled) return;
			base.WndProc(ref msg);
			if(OwnerEdit != null) OwnerEdit.OnMaskBoxWndProc(ref msg);
		}
		public TokenEdit OwnerEdit { get { return ownerEdit; } }
	}
	public delegate void TokenEditMaskBoxCharEventHandler(object sender, TokenEditMaskBoxCharEventArgs e);
	public class TokenEditMaskBoxCharEventArgs : EventArgs {
		char keyChar;
		bool handled;
		public TokenEditMaskBoxCharEventArgs(char keyChar) {
			this.handled = false;
			this.keyChar = keyChar;
		}
		public char KeyChar { get { return keyChar; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	#endregion
	public class TokenEditHandler : IDisposable {
		TokenEdit tokenEdit;
		public TokenEditHandler(TokenEdit tokenEdit) {
			this.tokenEdit = tokenEdit;
		}
		public virtual void OnMouseEnter(EventArgs e) {
		}
		public virtual void OnMouseLeave(EventArgs e) {
			DoResetHotToken();
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			DoCheckTokenMouseDown(e);
			if(OwnerEdit.ReadOnly) {
				DoCheckEditorRequest(e);
				return;
			}
			EnsureClosePopup();
			DoCheckHotToken(e);
			DoCheckSelectedToken(e);
			DoCheckEditorRequest(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			DoCheckHotToken(e);
			DoCheckTokenMouseUp(e);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			DoCheckHotToken(e);
		}
		public virtual void OnMouseDoubleClick(MouseEventArgs e) {
			DoCheckTokenDoubleClick(e);
		}
		public virtual void OnMouseWheelCore(MouseEventArgs e) {
			if(!ViewInfo.IsVScrollVisible) return;
			int wheelChange = SystemInformation.MouseWheelScrollLines;
			OwnerEdit.SetTopRow(OwnerEdit.TopRow + ((e.Delta > 0) ? -wheelChange : wheelChange));
		}
		public virtual void OnEditorKeyDown(KeyEventArgs e) {
			if(OwnerEdit.ReadOnly) return;
			switch(e.KeyCode) {
				case Keys.Left:
					DoProcessLeftKey();
					break;
				case Keys.Right:
					DoProcessRightKey();
					break;
				case Keys.Up:
					DoProcessUpKey();
					break;
				case Keys.Down:
					DoProcessDownKey();
					break;
				case Keys.Home:
					DoProcessHomeKey();
					break;
				case Keys.End:
					DoProcessEndKey();
					break;
				case Keys.Delete:
					DoProcessDeleteKey();
					break;
				case Keys.Back:
					DoProcessBackspaceKey();
					break;
				case Keys.Enter:
					if(OwnerEdit.InplaceType == InplaceType.Bars) {
						if(OwnerEdit.IsPopupOpen) OwnerEdit.Popup.ProcessKeyDown(e);
					}
					break;
			}
		}
		public virtual void OnMouseHover() {
			if(OwnerEdit.ReadOnly) return;
			TokenEditHitInfo hitInfo = OwnerEdit.CalcHitInfo(GetMousePos());
			if(!hitInfo.InLink) return;
			OwnerEdit.Properties.RaiseTokenMouseHover(TokenEditTokenBasedEventArgsBase.Create<TokenEditTokenMouseHoverEventArgs>(hitInfo.TokenInfo));
			if(OwnerEdit.Properties.GetPopupPanelShowMode() == TokenEditPopupPanelShowMode.ShowOnTokenMouseHover) {
				OwnerEdit.CheckShowPopupPanel(hitInfo.TokenInfo);
			}
		}
		public virtual void OnLostFocus() {
			TokenEditMaskBox maskBox = OwnerEdit.MaskBox;
			if(maskBox == null) return;
			if(!maskBox.Focused) {
				OwnerEdit.DoCheckCheckedItemsOnLostFocus();
			}
		}
		protected Point GetMousePos() {
			return OwnerEdit.PointToClient(Control.MousePosition);
		}
		protected virtual void DoProcessLeftKey() {
			TokenEditToken tok = OwnerEdit.CheckedItems.Get();
			if(tok == null) return;
			TokenEditSelectedItemCollection col = OwnerEdit.SelectedItems;
			int n = col.IndexOf(tok);
			if(n <= 0) return;
			if(ViewInfo.IsVScrollVisible && ViewInfo.IsTopLeftToken(tok)) {
				OwnerEdit.ScrollIntoView(OwnerEdit.TopRow - 1);
			}
			TokenEditToken newTok = col[n - 1];
			OwnerEdit.CheckedItems.Set(newTok);
		}
		protected virtual void DoProcessRightKey() {
			TokenEditToken tok = OwnerEdit.CheckedItems.Get();
			if(tok == null) return;
			TokenEditSelectedItemCollection col = OwnerEdit.SelectedItems;
			int n = col.IndexOf(tok);
			if(n < col.Count - 1) {
				TokenEditToken newTok = col[n + 1];
				OwnerEdit.CheckedItems.Set(newTok);
				OwnerEdit.ScrollIntoView(newTok);
			}
			else DoActivateEdit();
		}
		protected virtual void DoProcessUpKey() {
			TokenEditToken tok = OwnerEdit.CheckedItems.Get();
			if(tok == null) return;
			if(ViewInfo.IsVScrollVisible && ViewInfo.IsTopRowToken(tok)) {
				OwnerEdit.ScrollIntoView(OwnerEdit.TopRow - 1);
			}
			TokenEditToken topTok = ViewInfo.CalcTopToken(tok);
			if(topTok == null) return;
			OwnerEdit.CheckedItems.Set(topTok);
		}
		protected TokenEditViewInfo ViewInfo { get { return OwnerEdit.ViewInfo; } }
		protected virtual void DoProcessDownKey() {
			TokenEditToken tok = OwnerEdit.CheckedItems.Get();
			if(tok == null) {
				if(OwnerEdit.IsTextEditorActive && IsAltPressed) OwnerEdit.TogglePopup(PopupCloseMode.CloseUpKey);
				return;
			}
			TokenEditToken bottomTok = ViewInfo.CalcBottomToken(tok);
			if(bottomTok != null) {
				OwnerEdit.CheckedItems.Set(bottomTok);
				OwnerEdit.ScrollIntoView(bottomTok);
			}
		}
		protected bool IsAltPressed { get { return Control.ModifierKeys.HasFlag(Keys.Alt); } }
		protected virtual void DoProcessHomeKey() {
			if(OwnerEdit.IsTextEditorActive) return;
			TokenEditSelectedItemCollection col = OwnerEdit.SelectedItems;
			if(col.FirstToken != null) {
				OwnerEdit.ScrollIntoView(0);
				OwnerEdit.CheckedItems.Set(col.FirstToken);
			}
		}
		protected virtual void DoProcessEndKey() {
			if(OwnerEdit.IsTextEditorSuppressed) {
				TokenEditSelectedItemCollection col = OwnerEdit.SelectedItems;
				if(col.LastToken != null) {
					OwnerEdit.ScrollToEnd();
					OwnerEdit.CheckedItems.Set(col.LastToken);
				}
			}
			else {
				DoActivateEdit();
			}
		}
		protected virtual void DoProcessDeleteKey() {
			if(OwnerEdit.ReadOnly) return;
			TokenEditToken tok = OwnerEdit.CheckedItems.Get();
			if(tok == null) return;
			TokenEditToken newTok = null;
			int n = OwnerEdit.SelectedItems.IndexOf(tok);
			if(n < OwnerEdit.SelectedItems.Count - 1) {
				newTok = OwnerEdit.SelectedItems[n + 1];
			}
			OwnerEdit.DoRemoveToken(tok);
			if(newTok != null) {
				OwnerEdit.CheckedItems.Set(newTok);
				OwnerEdit.ScrollIntoView(newTok);
			}
			else {
				DoActivateEdit();
			}
		}
		protected virtual void DoProcessBackspaceKey() {
			if(OwnerEdit.ReadOnly) return;
			TokenEditToken tok = OwnerEdit.CheckedItems.Get();
			if(tok == null) return;
			DoProcessLeftKey();
			OwnerEdit.DoRemoveToken(tok);
			if(OwnerEdit.SelectedItems.IsEmpty) DoActivateEdit();
		}
		protected virtual void DoActivateEdit() {
			if(!OwnerEdit.IsTextEditorSuppressed) OwnerEdit.ActivateTextEditor();
		}
		protected virtual void DoCheckHotToken(MouseEventArgs e) {
			TokenEditHitInfo hitInfo = OwnerEdit.CalcHitInfo(e.Location);
			if(!hitInfo.InToken) {
				DoResetHotToken();
				return;
			}
			ViewInfo.SetHotToken(hitInfo);
		}
		protected virtual void DoCheckTokenDoubleClick(MouseEventArgs e) {
			TokenEditHitInfo hitInfo = OwnerEdit.CalcHitInfo(e.Location);
			if(hitInfo.InToken) {
				OwnerEdit.Properties.RaiseTokenDoubleClick(new TokenEditTokenClickEventArgs(hitInfo.Token.Description, hitInfo.Token.Value));
			}
		}
		protected virtual void DoResetHotToken() {
			ViewInfo.ResetHotToken();
		}
		protected virtual void DoCheckSelectedToken(MouseEventArgs e) {
			TokenEditHitInfo hitInfo = OwnerEdit.CalcHitInfo(e.Location);
			if(!hitInfo.InToken) return;
			OwnerEdit.CheckedItems.Set(hitInfo.TokenInfo.Token);
		}
		protected virtual void DoCheckEditorRequest(MouseEventArgs e) {
			TokenEditHitInfo hitInfo = OwnerEdit.CalcHitInfo(e.Location);
			if(hitInfo.InEditorRect) DoActivateEdit();
		}
		protected void EnsureClosePopup() {
			if(OwnerEdit.IsPopupOpen) OwnerEdit.ClosePopup(PopupCloseMode.Cancel);
		}
		protected virtual void DoResetSelectedToken(MouseEventArgs e) {
			ViewInfo.ResetSelectedToken();
		}
		TokenEditHitInfo mouseDownHitInfo = null;
		protected virtual void DoCheckTokenMouseDown(MouseEventArgs e) {
			this.mouseDownHitInfo = OwnerEdit.CalcHitInfo(e.Location);
		}
		protected virtual void DoCheckTokenMouseUp(MouseEventArgs e) {
			TokenEditHitInfo hitInfo = OwnerEdit.CalcHitInfo(e.Location);
			if(this.mouseDownHitInfo != null && this.mouseDownHitInfo.InToken) {
				if(hitInfo.InToken && this.mouseDownHitInfo.Token.Equals(hitInfo.Token)) {
					OnTokenClick(hitInfo);
					OwnerEdit.CloseTextEditor(true);
				}
			}
		}
		protected virtual void OnTokenClick(TokenEditHitInfo hitInfo) {
			TokenEditToken token = hitInfo.Token;
			if(!OwnerEdit.ReadOnly && hitInfo.InGlyph && token != null && OwnerEdit.Properties.ShouldDeleteTokenOnGlyphClick()) {
				OwnerEdit.DoRemoveToken(token);
			}
			OwnerEdit.Properties.RaiseTokenClick(new TokenEditTokenClickEventArgs(token.Description, token.Value));
		}
		public TokenEdit OwnerEdit { get { return tokenEdit; } }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.tokenEdit = null;
		}
	}
	[TypeConverter("DevExpress.XtraEditors.Design.TokenEditTokenTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class TokenEditToken : ICloneable {
		string description;
		object value;
		bool autoPopulated;
		public TokenEditToken() : this(string.Empty, null) { }
		public TokenEditToken(object value)
			: this(value.ToString(), value) {
		}
		public TokenEditToken(string description, object value) {
			this.description = description;
			this.value = value;
			this.autoPopulated = false;
		}
		internal TokenEditToken(string description, object value, bool autoPopulated) : this(description, value) {
			this.autoPopulated = autoPopulated;
		}
		[DXCategory(CategoryName.Data), DefaultValue("")]
		public string Description {
			get { return description; }
			set {
				if(Description == value) return;
				description = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Data), DefaultValue(null), Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public virtual object Value {
			get { return value; }
			set {
				if(Value == value) return;
				this.value = value;
				OnChanged();
			}
		}
		internal bool AutoPopulated { get { return autoPopulated; } }
		public override bool Equals(object obj) {
			TokenEditToken item = obj as TokenEditToken;
			if(item == null) return false;
			if(!string.Equals(Description, item.Description, StringComparison.Ordinal)) return false;
			if(Value == null) return item.Value == null;
			return Value.Equals(item.Value);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			if(string.IsNullOrEmpty(Description)) return base.ToString();
			return Description;
		}
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		public virtual TokenEditToken Clone() {
			TokenEditToken item = new TokenEditToken();
			item.Assign(this);
			return item;
		}
		protected virtual void Assign(TokenEditToken item) {
			this.value = item.Value;
			this.description = item.Description;
			this.autoPopulated = item.AutoPopulated;
		}
		#region Changed
		protected internal event EventHandler Changed;
		protected virtual void OnChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		#endregion
	}
	[ListBindable(false)]
	public abstract class TokenEditTokenCollectionBase : CollectionBase {
		int lockUpdate;
		RepositoryItemTokenEdit properties;
		public TokenEditTokenCollectionBase(RepositoryItemTokenEdit properties) {
			this.lockUpdate = 0;
			this.properties = properties;
		}
		public virtual void Add(TokenEditToken item) {
			List.Add(item);
		}
		public virtual void AddRange(IEnumerable items) {
			BeginUpdate();
			try {
				foreach(TokenEditToken item in items) Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual TokenEditToken this[int index] {
			get { return List[index] as TokenEditToken; }
			set {
				if(value == null) return;
				List[index] = value;
			}
		}
		public void Assign(TokenEditTokenCollectionBase tokens) {
			if(tokens == null) return;
			Clear();
			AddRange(tokens);
		}
		public virtual bool Remove(TokenEditToken item) {
			if(!Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		public virtual void Insert(int position, TokenEditToken item) {
			if(Contains(item)) return;
			List.Insert(position, item);
		}
		public virtual bool Contains(TokenEditToken item) {
			return List.Contains(item);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		public virtual int IndexOf(TokenEditToken item) {
			return List.IndexOf(item);
		}
		protected override void OnInsert(int position, object value) {
			if(!(value is TokenEditToken)) return;
			base.OnInsert(position, value);
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		#region Changed Event
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		#endregion
		protected internal RepositoryItemTokenEdit Properties { get { return properties; } }
	}
	[ListBindable(false)]
	public abstract class TokenEditTokenReadOnlyCollectionBase : ReadOnlyCollectionBase {
		int lockChanged;
		RepositoryItemTokenEdit properties;
		public TokenEditTokenReadOnlyCollectionBase(RepositoryItemTokenEdit properties) {
			this.lockChanged = 0;
			this.properties = properties;
		}
		public TokenEditToken this[int index] { get { return (TokenEditToken)InnerList[index]; } }
		public bool Contains(TokenEditToken token) {
			return InnerList.IndexOf(token) != -1;
		}
		protected internal void Remove(TokenEditToken token) {
			int n = InnerList.IndexOf(token);
			if(n == -1) return;
			InnerList.Remove(token);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, n));
		}
		public bool IsEmpty { get { return Count == 0; } }
		protected internal TokenEditToken Get() {
			return Count != 0 ? this[0] : null;
		}
		protected internal abstract TokenEditToken Set(TokenEditToken token);
		protected internal virtual void Assign(ICollection col) {
			BeginUpdate();
			Clear();
			try {
				InnerList.AddRange(col);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void Clear() {
			InnerList.Clear();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected internal void BeginUpdate() { lockChanged++; }
		protected internal void EndUpdate() {
			if(--lockChanged == 0) RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		#region Changed Event
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockChanged != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		#endregion
		protected internal RepositoryItemTokenEdit Properties { get { return properties; } }
	}
	[TypeConverter(typeof(UniversalCollectionTypeConverter)), Editor("DevExpress.XtraEditors.Design.TokenEditTokenCollectionEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))]
	public class TokenEditTokenCollection : TokenEditTokenCollectionBase, IEnumerable<TokenEditToken> {
		public TokenEditTokenCollection(RepositoryItemTokenEdit properties)
			: base(properties) {
		}
		public TokenEditToken AddToken(object value) {
			TokenEditToken token = new TokenEditToken(value);
			Add(token);
			return token;
		}
		public TokenEditToken AddToken(string description, object value) {
			TokenEditToken token = new TokenEditToken(description, value);
			Add(token);
			return token;
		}
		public void AddEnum(Type enumType) {
			AddEnum(enumType, true, true);
		}
		public void AddEnum(Type enumType, bool skipNone, bool skipComposite) {
			BeginUpdate();
			try {
				Array values = EnumDisplayTextHelper.GetEnumValues(enumType);
				foreach(Enum value in values) {
					if((skipNone && EnumDisplayTextHelper.IsNoneEnumValue(value)) || (skipComposite && EnumDisplayTextHelper.IsCompositeValue(value))) continue;
					Add(new TokenEditToken(EnumDisplayTextHelper.GetDisplayText(value), value));
				}
			}
			finally { EndUpdate(); }
		}
		public void AddEnum(Type enumType, bool addEnumeratorIntegerValues) {
			BeginUpdate();
			try {
				Array values = EnumDisplayTextHelper.GetEnumValues(enumType);
				foreach(Enum enumValue in values) {
					object value = EnumDisplayTextHelper.GetEnumValue(addEnumeratorIntegerValues, enumValue, enumType);
					Add(new TokenEditToken(EnumDisplayTextHelper.GetDisplayText(enumValue), value));
				}
			}
			finally { EndUpdate(); }
		}
		#region IEnumerable<TokenEditToken>
		IEnumerator<TokenEditToken> IEnumerable<TokenEditToken>.GetEnumerator() {
			foreach(TokenEditToken token in this) yield return token;
		}
		#endregion
	}
	[ListBindable(false)]
	public class TokenEditSelectedItemCollection : TokenEditTokenReadOnlyCollectionBase, ICloneable {
		public TokenEditSelectedItemCollection(RepositoryItemTokenEdit properties)
			: base(properties) {
		}
		public static TokenEditSelectedItemCollection ReturnEmpty(RepositoryItemTokenEdit properties) {
			return new TokenEditSelectedItemCollection(properties);
		}
		protected internal override TokenEditToken Set(TokenEditToken token) {
			int n = InnerList.Add(token);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, n));
			return token;
		}
		public TokenEditToken FirstToken {
			get { return Count != 0 ? this[0] : null; }
		}
		public TokenEditToken LastToken {
			get { return Count != 0 ? this[Count - 1] : null; }
		}
		public int IndexOf(TokenEditToken token) {
			return InnerList.IndexOf(token);
		}
		public TokenEditSelectedItemCollection Clone() {
			TokenEditSelectedItemCollection col = new TokenEditSelectedItemCollection(Properties);
			col.Assign(this);
			return col;
		}
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		protected internal bool ContainsValue(object value) {
			if(value == null) return false;
			foreach(TokenEditToken tok in InnerList) {
				if(tok.Value != null && tok.Value.Equals(value)) return true;
			}
			return false;
		}
	}
	[ListBindable(false)]
	public class TokenEditCheckedItemCollection : TokenEditTokenReadOnlyCollectionBase {
		public TokenEditCheckedItemCollection(RepositoryItemTokenEdit properties)
			: base(properties) {
		}
		protected internal override TokenEditToken Set(TokenEditToken token) {
			return DoSet(token, true);
		}
		protected internal TokenEditToken SetInternal(TokenEditToken token) {
			return DoSet(token, false);
		}
		protected virtual TokenEditToken DoSet(TokenEditToken token, bool checkKeyboard) {
			if(Properties.GetCheckMode() == TokenEditCheckMode.Multiple && (IsControlOrShiftPressed || !checkKeyboard)) {
				DoSetMultiple(token);
			}
			else {
				DoSetSingle(token);
			}
			return token;
		}
		protected bool IsControlOrShiftPressed {
			get { return (Control.ModifierKeys & (Keys.Control | Keys.Shift)) != 0; }
		}
		protected void DoSetSingle(TokenEditToken token) {
			if(InnerList.Count == 1) {
				InnerList[0] = token;
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, 0));
			}
			else {
				BeginUpdate();
				try {
					InnerList.Clear();
					InnerList.Add(token);
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected void DoSetMultiple(TokenEditToken token) {
			int n = InnerList.IndexOf(token);
			if(n > -1) {
				InnerList.Remove(token);
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, n));
			}
			else {
				n = InnerList.Add(token);
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, n));
			}
		}
	}
	public abstract class TokenEditValueMapperBase {
		public TokenEditSelectedItemCollection GetTokens(RepositoryItemTokenEdit properties, object editValue) {
			return DoGetTokens(properties, editValue);
		}
		public object UpdateEditValue(RepositoryItemTokenEdit properties, TokenEditSelectedItemCollection col, object editValue) {
			return DoGetEditValue(properties, col, editValue);
		}
		protected void RaiseMapperPanic(string key, string editValue) {
			throw new InvalidOperationException(string.Format("Can't transform the '{0}' part of '{1}' EditValue in Tokens", key, editValue));
		}
		protected abstract TokenEditSelectedItemCollection DoGetTokens(RepositoryItemTokenEdit properties, object editValue);
		protected abstract object DoGetEditValue(RepositoryItemTokenEdit properties, TokenEditSelectedItemCollection col, object editValue);
	}
	public class TokenEditStringEditValueMapper : TokenEditValueMapperBase {
		protected override TokenEditSelectedItemCollection DoGetTokens(RepositoryItemTokenEdit properties, object editValue) {
			if(editValue == null) return TokenEditSelectedItemCollection.ReturnEmpty(properties);
			string val = editValue as string;
			if(val == null)
				throw new InvalidOperationException("EditValue must be of string type");
			if(properties.Separators.Count == 0) {
				throw new InvalidOperationException("You should initialize 'Separators' collection to complete this operation");
			}
			string[] keys = GetKeys(val, properties);
			if(keys.Length == 0)
				return TokenEditSelectedItemCollection.ReturnEmpty(properties);
			TokenEditTokenCacheBase cache = properties.GetStringCache();
			TokenEditSelectedItemCollection col = new TokenEditSelectedItemCollection(properties);
			for(int i = 0; i < keys.Length; i++) {
				TokenEditToken token = cache[keys[i]];
				if(token == null) {
					if(properties.Validator.AllowValidation) token = properties.Validator.DoValidateCore(keys[i], false);
				}
				if(token == null) {
					RaiseMapperPanic(keys[i], editValue.ToString());
				}
				col.Set(token);
			}
			return col;
		}
		protected string[] GetKeys(string editValue, RepositoryItemTokenEdit properties) {
			return StringSplitHelper.SplitSimple(editValue, GetSeparators(properties)).ToArray();
		}
		protected StringCollection GetSeparators(RepositoryItemTokenEdit properties) {
			StringCollection col = new StringCollection();
			foreach(string sep in properties.Separators) {
				col.Add(sep);
			}
			string editValueSeparator = properties.EditValueSeparatorChar.ToString();
			if(!string.IsNullOrEmpty(editValueSeparator)) {
				col.Add(editValueSeparator);
			}
			return col;
		}
		protected override object DoGetEditValue(RepositoryItemTokenEdit properties, TokenEditSelectedItemCollection col, object editValue) {
			return properties.StringFormatter.GetString(col);
		}
	}
	public class TokenEditListEditValueMapper : TokenEditValueMapperBase {
		protected override TokenEditSelectedItemCollection DoGetTokens(RepositoryItemTokenEdit properties, object editValue) {
			if(properties.IsNullValue(editValue)) return TokenEditSelectedItemCollection.ReturnEmpty(properties);
			IList list = editValue as IList;
			if(list == null) {
				ConvertEditValueEventArgs args = properties.DoParseEditValue(editValue);
				list = args.Value as IList;
				if(list == null) {
					throw new InvalidOperationException("EditValue must be an object that implements the IList interface");
				}
			}
			TokenEditTokenCacheBase cache = properties.GetObjectCache();
			TokenEditSelectedItemCollection col = new TokenEditSelectedItemCollection(properties);
			foreach(object val in list) {
				TokenEditToken token = cache[val];
				if(token == null) {
					if(properties.Validator.AllowValidation) token = properties.Validator.DoValidateCore(val.ToString(), false);
				}
				if(token == null) {
					RaiseMapperPanic(val.ToString(), editValue.ToString());
				}
				col.Set(token);
			}
			return col;
		}
		protected override object DoGetEditValue(RepositoryItemTokenEdit properties, TokenEditSelectedItemCollection col, object editValue) {
			if(editValue == null) {
				editValue = new BindingList<object>();
			}
			IList list = (IList)editValue;
			list.Clear();
			foreach(TokenEditToken token in col) {
				list.Add(token.Value);
			}
			return list;
		}
	}
	public class TokenEditEnumEditValueMapper : TokenEditValueMapperBase {
		protected override TokenEditSelectedItemCollection DoGetTokens(RepositoryItemTokenEdit properties, object editValue) {
			if(editValue == null) return TokenEditSelectedItemCollection.ReturnEmpty(properties);
			Enum enumValue = editValue as Enum;
			if(enumValue == null) {
				throw new InvalidOperationException("EditValue must be of Enum type");
			}
			int baseVal = EnumDisplayTextHelper.GetEnumUnderlyingValue(enumValue);
			if(baseVal.Equals(0)) {
				return TokenEditSelectedItemCollection.ReturnEmpty(properties);
			}
			CheckSelectedItems(properties.SelectedItems);
			TokenEditTokenCacheBase cache = properties.GetObjectCache();
			Type enumType = enumValue.GetType();
			int[] binarySeries = GetEnumBinarySeries(enumType, baseVal);
			TokenEditSelectedItemCollection col = new TokenEditSelectedItemCollection(properties);
			for(int i = binarySeries.Length - 1; i >= 0; i--) {
				Enum tv = (Enum)Enum.ToObject(enumType, binarySeries[i]);
				TokenEditToken token = cache[tv];
				if(token == null) {
					RaiseMapperPanic(tv.ToString(), editValue.ToString());
				}
				col.Set(token);
			}
			return col;
		}
		protected override object DoGetEditValue(RepositoryItemTokenEdit properties, TokenEditSelectedItemCollection col, object editValue) {
			if(col.Count == 0) {
				if(editValue != null) return EnumDisplayTextHelper.GetNoneEnumValue(editValue.GetType());
				return null;
			}
			CheckSelectedItems(col);
			int nVal = 0;
			Type enumType = col[0].Value.GetType();
			foreach(TokenEditToken token in col) {
				nVal |= EnumDisplayTextHelper.GetEnumUnderlyingValue((Enum)token.Value);
			}
			return Enum.ToObject(enumType, nVal);
		}
		protected int[] GetEnumBinarySeries(Type enumType, int baseVal) {
			int val = baseVal;
			List<int> series = new List<int>();
			for(int i = 1; val != 0; i++) {
				if((val & 0x1) != 0) {
					if(i == 1) series.Add(1);
					else series.Add(2 << (i - 2));
				}
				val >>= 1;
			}
			if(series.Count > 1) series.Reverse();
			return series.ToArray();
		}
		protected void CheckSelectedItems(TokenEditSelectedItemCollection col) {
			foreach(TokenEditToken token in col) {
				Type enumType = token.Value.GetType();
				if(!typeof(Enum).IsAssignableFrom(enumType)) {
					throw new InvalidOperationException("SelectedItems collection contains inconsistent values");
				}
			}
		}
	}
	public class TokenEditMapperRegistry {
		TokenEditValueMapperBase editValueMapper;
		public TokenEditMapperRegistry() {
			this.editValueMapper = null;
		}
		public void UpdateMapper(TokenEditValueType editValueType) {
			UpdateMapperCore(editValueType);
		}
		public TokenEditValueMapperBase GetMapper() {
			return editValueMapper;
		}
		protected void UpdateMapperCore(TokenEditValueType editValueType) {
			this.editValueMapper = GetMapperCore(editValueType);
		}
		protected virtual TokenEditValueMapperBase GetMapperCore(TokenEditValueType editValueType) {
			switch(editValueType) {
				case TokenEditValueType.List: return new TokenEditListEditValueMapper();
				case TokenEditValueType.String: return new TokenEditStringEditValueMapper();
				case TokenEditValueType.Enum: return new TokenEditEnumEditValueMapper();
			}
			throw new ArgumentException(string.Format("The '{0}' mapper type isn't supported", editValueType.ToString()));
		}
	}
	public abstract class TokenEditTokenCacheBase {
		Hashtable items;
		public TokenEditTokenCacheBase() {
			this.items = new Hashtable();
		}
		public void Update(RepositoryItemTokenEdit properties) {
			DoUpdate(properties);
		}
		public int Size { get { return Items.Count; } }
		public bool Contains(object key) {
			return Items.ContainsKey(key);
		}
		public TokenEditToken this[object key] {
			get { return Items[key] as TokenEditToken; }
		}
		protected void AddItem(TokenEditToken token) {
			object key = GetKey(token);
			try {
				Items.Add(key, token);
			}
			catch(ArgumentException e) {
				throw new InvalidOperationException(GetInvalidKeyExceptionMessage(token), e);
			}
		}
		protected object GetKey(TokenEditToken token) {
			if(token.Value == null) {
				throw new InvalidOperationException(string.Format("{0} token's value is null. Please specify the token value.", token.ToString()));
			}
			return GetKeyCore(token);
		}
		protected string GetInvalidKeyExceptionMessage(TokenEditToken val) {
			return string.Format("The TokenEdit cannot display tokens with the same {1} property values. Please set a unique value for the '{0}' token.", val.Description, KeyPath);
		}
		protected abstract object GetKeyCore(TokenEditToken token);
		protected abstract string KeyPath { get; }
		protected virtual void DoUpdate(RepositoryItemTokenEdit properties) {
			Items.Clear();
			foreach(TokenEditToken token in properties.Tokens) {
				AddItem(token);
			}
		}
		protected Hashtable Items { get { return items; } }
	}
	public class TokenEditObjectCache : TokenEditTokenCacheBase {
		public TokenEditObjectCache() {
		}
		protected override string KeyPath { get { return "TokenEditToken.Value"; } }
		protected override object GetKeyCore(TokenEditToken token) {
			return token.Value;
		}
	}
	public class TokenEditStringCache : TokenEditTokenCacheBase {
		public TokenEditStringCache() {
		}
		protected override string KeyPath { get { return "TokenEditToken.Value.ToString()"; } }
		protected override object GetKeyCore(TokenEditToken token) {
			return token.Value.ToString();
		}
	}
	public abstract class TokenEditOptionsBase : BaseOptions, IDisposable {
		RepositoryItemTokenEdit properties;
		public TokenEditOptionsBase(RepositoryItemTokenEdit properties) {
			this.properties = properties;
		}
		protected internal RepositoryItemTokenEdit Properties { get { return properties; } }
		protected override void RaiseOnChanged(BaseOptionChangedEventArgs e) {
			if(Properties != null) Properties.LayoutChanged();
			base.RaiseOnChanged(e);
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			TokenEditOptionsBase source = options as TokenEditOptionsBase;
			if(source == null) return;
			BeginUpdate();
			try {
				this.properties = source.Properties;
			}
			finally {
				EndUpdate();
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.properties = null;
		}
		#endregion
		protected internal new bool ShouldSerialize() { return base.ShouldSerialize(); }
		public override void Reset() { base.Reset(); }
	}
	public class PopupPanelOptions : TokenEditOptionsBase {
		bool showPopupPanel;
		TokenEditPopupPanelShowMode showMode;
		TokenEditPopupPanelLocation location;
		public PopupPanelOptions(RepositoryItemTokenEdit properties)
			: base(properties) {
			this.showPopupPanel = true;
			this.showMode = TokenEditPopupPanelShowMode.Default;
			this.location = TokenEditPopupPanelLocation.Default;
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(true)]
		public bool ShowPopupPanel {
			get { return showPopupPanel; }
			set {
				if(ShowPopupPanel == value)
					return;
				bool prevValue = ShowPopupPanel;
				showPopupPanel = value;
				OnChanged("ShowPopupPanel", prevValue, ShowPopupPanel);
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(TokenEditPopupPanelShowMode.Default)]
		public TokenEditPopupPanelShowMode ShowMode {
			get { return showMode; }
			set {
				if(ShowMode == value)
					return;
				TokenEditPopupPanelShowMode prevValue = ShowMode;
				showMode = value;
				OnChanged("ShowMode", prevValue, ShowMode);
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(TokenEditPopupPanelLocation.Default)]
		public TokenEditPopupPanelLocation Location {
			get { return location; }
			set {
				if(Location == value)
					return;
				TokenEditPopupPanelLocation prevValue = Location;
				location = value;
				OnChanged("Location", prevValue, Location);
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			PopupPanelOptions source = options as PopupPanelOptions;
			if(source == null) return;
			BeginUpdate();
			try {
				this.showPopupPanel = source.ShowPopupPanel;
				this.showMode = source.ShowMode;
				this.location = TokenEditPopupPanelLocation.Default;
			}
			finally {
				EndUpdate();
			}
		}
		public override void Reset() {
			base.Reset();
			this.showPopupPanel = true;
			this.showMode = TokenEditPopupPanelShowMode.Default;
			this.location = TokenEditPopupPanelLocation.Default;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class TokenEditViewInfo : BaseEditViewInfo, IHeightAdaptable {
		Hashtable items;
		int rowCount;
		Rectangle editorRect;
		Rectangle textRect;
		TokenEditHitInfo hotToken;
		bool isVScrollVisible;
		public TokenEditViewInfo(RepositoryItem item) : base(item) {
			this.rowCount = 1;
			this.editorRect = Rectangle.Empty;
			this.textRect = Rectangle.Empty;
			this.items = new Hashtable();
			this.isVScrollVisible = false;
			this.hotToken = new TokenEditHitInfo();
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			if(Item.ReadOnly && Item.UseReadOnlyAppearance) return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.ControlLight), GetDefaultFont());
			return base.CreateDefaultAppearance();
		}
		protected override void CalcConstants() {
			base.CalcConstants();
			this.rowCount = CalcRowCount(GetPrecalcContentWidth());
		}
		protected int GetPrecalcContentWidth() {
			int width = Bounds.Width - 2 * (ContentRectMargin + 1);
			if(IsVScrollVisible && OwnerEdit != null) {
				width -= OwnerEdit.ScrollController.VScrollWidth;
			}
			return Math.Max(0, width);
		}
		public Rectangle MaskBoxRect {
			get { return AllowMaskBox ? EditorRect : Rectangle.Empty; }
		}
		protected internal bool AllowMaskBox {
			get {
				if(Item.IsDesignMode) return false;
				return OwnerEdit != null && OwnerEdit.IsTextEditorActive;
			}
		}
		public int RowCount { get { return rowCount; } }
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			AdjustContent();
			CalcTextRect();
			CalcItemsInfo();
			CalcMaskBoxRect();
		}
		protected virtual void AdjustContent() {
			bool vScrollVisible = CalcVScrollBarVisibility();
			if(vScrollVisible && OwnerEdit != null) {
				this.fContentRect.Width -= OwnerEdit.ScrollController.VScrollWidth;
			}
			this.isVScrollVisible = vScrollVisible;
		}
		public override Size CalcBestFit(Graphics g) {
			if(InplaceType == InplaceType.Standalone || VisibleItems.Count == 0) return base.CalcBestFit(g);
			bool graphicsAdded = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(g);
				graphicsAdded = true;
			}
			Size sz = Size.Empty;
			try {
				sz = CalcBestFitCore();
			}
			finally {
				if(graphicsAdded) GInfo.ReleaseGraphics();
			}
			return sz;
		}
		protected virtual Size CalcBestFitCore() {
			int totalWidth = 0;
			foreach(TokenEditToken tok in Item.SelectedItems) {
				totalWidth += CalcItemSizeCore(tok).Width;
			}
			return new Size(totalWidth + 2 * ItemContentMargin, CalcContentHeight(1));
		}
		public bool IsVScrollVisible { get { return isVScrollVisible; } }
		public virtual ScrollArgs CalcVScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.Maximum = RowCount - 1;
			args.LargeChange = CalcVisibleLineCount();
			args.Value = GetTopRow();
			return args;
		}
		protected virtual int CalcVisibleLineCount() {
			return Item.MaxExpandLines;
		}
		protected virtual bool CalcVScrollBarVisibility() {
			return RowCount > VisibleRowCount;
		}
		protected void CalcMaskBoxRect() {
			this.editorRect = CalcMaskBoxRectCore();
		}
		protected void CalcTextRect() {
			this.textRect = CalcTextRectCore();
		}
		protected virtual Rectangle CalcTextRectCore() {
			Rectangle rect = ContentRect;
			rect.Offset(1, 1);
			return rect;
		}
		public Rectangle EditorRect { get { return editorRect; } }
		public Rectangle TextRect { get { return textRect; } }
		protected virtual Rectangle CalcMaskBoxRectCore() {
			if(Items.Count == 0) return ContentRect;
			TokenEditTokenInfo tokenInfo = GetLastTokenInfo();
			Rectangle rowBounds = CalcRowBounds(tokenInfo.RowIndex);
			if(RightToLeft) {
				return new Rectangle(rowBounds.X, rowBounds.Y + 1, tokenInfo.Bounds.Left - rowBounds.X, rowBounds.Height);
			}
			else {
				int dx = Math.Max(0, tokenInfo.Bounds.Right + 1 - rowBounds.X);
				return new Rectangle(rowBounds.X + dx, rowBounds.Y + 1, rowBounds.Width - dx, rowBounds.Height);
			}
		}
		protected Rectangle CalcRowBounds(int rowIndex) {
			if(Items.Count == 0) return ContentRect;
			int textHeight = TextSize.Height;
			Rectangle rect = new Rectangle(ContentRect.X, ContentRect.Y, ContentRect.Width, textHeight);
			rect.Y += (textHeight + IndentBetweenItems * 2 + IndentBetweenRows) * rowIndex;
			return rect;
		}
		protected internal TokenEditTokenInfo GetLastTokenInfo() {
			if(Items.Count == 0) return null;
			TokenEditTokenInfo tok = null;
			foreach(TokenEditTokenInfo tokenInfo in VisibleItems) {
				Point loc = tokenInfo.Bounds.Location;
				if(tok == null || loc.Y > tok.Bounds.Y || (loc.Y == tok.Bounds.Y && (RightToLeft ? (loc.X < tok.Bounds.X) : (loc.X > tok.Bounds.X)))) tok = tokenInfo;
			}
			return tok;
		}
		protected internal Hashtable Items { get { return items; } }
		protected virtual void CalcItemsInfo() {
			Items.Clear();
			int rowIndex = 0;
			Point topPt = ContentRect.Location;
			if(RightToLeft) {
				topPt.Offset(ContentRect.Width, 0);
			}
			for(int i = CalcTopTokenIndex(); i < Item.SelectedItems.Count; i++) {
				TokenEditToken token = GetSelectedToken(i);
				TokenEditTokenInfo tokenInfo = CalcItemInfo(token, topPt, rowIndex);
				UpdateToken(tokenInfo);
				Items.Add(token, tokenInfo);
				topPt = tokenInfo.Bounds.Location;
				if(RightToLeft) {
					topPt.Offset(-IndentBetweenItems, 0);
				}
				else {
					topPt.Offset(tokenInfo.Bounds.Width + IndentBetweenItems, 0);
				}
				rowIndex = tokenInfo.RowIndex;
			}
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			foreach(TokenEditTokenInfo tok in VisibleItems) {
				tok.Offset(x, y);
			}
		}
		public virtual bool AllowDrawText { get { return Item.IsNullValue(EditValue) && !string.IsNullOrEmpty(Item.NullText); } }
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(InplaceType != InplaceType.Standalone && !Item.IsDesignMode) DoUpdateSelectedItems();
		}
		bool updateSelectedItems = false;
		protected virtual void DoUpdateSelectedItems() {
			if(this.updateSelectedItems) return;
			this.updateSelectedItems = true;
			try {
				Item.DoUpdateSelectedItems(EditValue); 
				UpdateViewInfo();
			}
			finally {
				this.updateSelectedItems = false;
			}
		}
		protected virtual void UpdateViewInfo() {
			CalcViewInfo(null);
		}
		protected int CalcTopTokenIndex() {
			if(Item.SelectedItems.Count == 0 || GetTopRow() == 0) return 0;
			bool addGraphics = false;
			int n;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				addGraphics = true;
			}
			try {
				n = CalcTopTokenIndexCore(ContentRect.Width, GetTopRow());
			}
			finally {
				if(addGraphics) GInfo.ReleaseGraphics();
			}
			return n;
		}
		protected virtual int CalcTopTokenIndexCore(int clientWidth, int topRow) {
			TokenEditSelectedItemCollection col = Item.SelectedItems;
			int rowCount = 0, n = 0;
			do {
				if(n < col.Count && rowCount == topRow) return n;
				n = DoTraverseRow(n, col, clientWidth);
				rowCount++;
			}
			while(n < col.Count);
			return 0;
		}
		protected TokenEditToken GetSelectedToken(int n) {
			return Item.SelectedItems[n];
		}
		public override bool AllowDrawFocusRect {
			get { return false; }
			set { base.AllowDrawFocusRect = value; }
		}
		protected virtual int ContentRectMargin { get { return 1; } }
		protected override void UpdateContentRectByFocusRect() {
			this.fContentRect.Inflate(-ContentRectMargin, -ContentRectMargin);
		}
		protected int GetTopRow() {
			return OwnerEdit != null ? OwnerEdit.TopRow : 0;
		}
		protected virtual TokenEditTokenInfo CalcItemInfo(TokenEditToken token, Point topPt, int rowIndex) {
			TokenEditTokenInfo tokenInfo = CreateTokenEditTokenInfo(token);
			tokenInfo.SetBounds(CalcBounds(topPt, token));
			bool lineBreak = (tokenInfo.Bounds.Y != topPt.Y);
			tokenInfo.SetRowIndex(lineBreak ? rowIndex + 1 : rowIndex);
			tokenInfo.SetDescriptionBounds(CalcDescriptionBounds(token, tokenInfo.Bounds));
			tokenInfo.SetGlyphBounds(CalcGlyphBounds(token, tokenInfo.Bounds));
			return tokenInfo;
		}
		protected virtual TokenEditTokenInfo CreateTokenEditTokenInfo(TokenEditToken token) {
			return new TokenEditTokenInfo(token);
		}
		protected virtual Rectangle CalcBounds(Point topPt, TokenEditToken token) {
			Point loc = topPt;
			Size itemSize = CalcItemSize(token);
			if(RightToLeft) {
				loc.Offset(-itemSize.Width, 0);
			}
			Rectangle r = new Rectangle(loc, itemSize);
			if(RightToLeft) {
				if(r.Left < ContentRect.Left) r.Location = new Point(ContentRect.Right - itemSize.Width, r.Bottom + IndentBetweenRows);
			}
			else {
				if(r.Right > ContentRect.Right) r.Location = new Point(ContentRect.X, r.Bottom + IndentBetweenRows);
			}
			return r;
		}
		protected virtual Rectangle CalcGlyphBounds(TokenEditToken token, Rectangle bounds) {
			if(!Item.ShowTokenGlyph) return Rectangle.Empty;
			Size glyphSize = CalcGlyphSize(token);
			Rectangle r = new Rectangle(Point.Empty, glyphSize);
			if(Item.GetTokenGlyphLocation() == TokenEditGlyphLocation.Left) {
				r.Offset(bounds.X + ItemContentMargin, bounds.Y + bounds.Height / 2 - glyphSize.Height / 2);
			}
			else {
				r.Offset(bounds.Right - ItemContentMargin - glyphSize.Width, bounds.Y + bounds.Height / 2 - glyphSize.Height / 2);
			}
			return r;
		}
		protected virtual Rectangle CalcDescriptionBounds(TokenEditToken token, Rectangle bounds) {
			Size descSize = CalcDescriptionSize(token);
			Rectangle r = new Rectangle(Point.Empty, descSize);
			Rectangle gb = CalcGlyphBounds(token, bounds);
			int yPt = bounds.Y + bounds.Height / 2 - descSize.Height / 2 - 1;
			if(Item.ShowTokenGlyph) {
				if(Item.GetTokenGlyphLocation() == TokenEditGlyphLocation.Right) {
					r.Offset(bounds.X + ItemContentMargin + 1, yPt);
					if(r.Right > gb.Left) r.Width -= (r.Right - gb.Left);
				}
				else {
					r.Offset(bounds.X + ItemContentMargin * 2 + CalcGlyphWidth(token) - 1, yPt);
				}
			}
			else {
				r.Offset(bounds.X + ItemContentMargin, yPt);
			}
			if(r.Right > bounds.Right) r.Width -= (r.Right - bounds.Right);
			return r;
		}
		protected virtual Size CalcItemSize(TokenEditToken token) {
			Size sz = CalcItemSizeCore(token);
			sz.Width = Math.Min(sz.Width, GetMaximumItemWidth());
			return sz;
		}
		protected int GetMaximumItemWidth() {
			return Math.Max(0, GetPrecalcContentWidth() - MinimumEditorWidth);
		}
		protected internal virtual int MinimumEditorWidth { get { return 3; } }
		protected virtual Size CalcItemSizeCore(TokenEditToken token) {
			int descWidth = CalcDescriptionSize(token).Width;
			if(Item.ShowTokenGlyph) {
				return new Size(descWidth + CalcGlyphWidth(token) + 3 * ItemContentMargin, CalcRowHeight());
			}
			return new Size(descWidth + 2 * ItemContentMargin, CalcRowHeight());
		}
		protected int CalcRowHeight() {
			return TextSize.Height + 2 * IndentBetweenItems;
		}
		protected virtual int ItemContentMargin { get { return 4; } }
		protected Size CalcDescriptionSize(TokenEditToken token) {
			return PaintAppearance.CalcTextSizeInt(GInfo.Graphics, token.Description, 0);
		}
		protected virtual int IndentBetweenItems { get { return 1; } }
		protected int CalcGlyphWidth(TokenEditToken token) {
			return CalcGlyphSize(token).Width;
		}
		protected virtual Size CalcGlyphSize(TokenEditToken token) {
			if(!Item.ShowTokenGlyph) return Size.Empty;
			return new Size(12, 12);
		}
		public ICollection VisibleItems { get { return Items.Values; } }
		protected virtual void UpdateToken(TokenEditTokenInfo tokenInfo) {
			UpdateTokenState(tokenInfo);
			UpdateTokenAppearance(tokenInfo);
		}
		public int TokenCount { get { return VisibleItems.Count; } }
		protected virtual void UpdateTokenState(TokenEditTokenInfo tokenInfo) {
			if(!Enabled) {
				tokenInfo.State = tokenInfo.GlyphState = ObjectState.Disabled;
				return;
			}
			if(Item.ReadOnly) {
				tokenInfo.GlyphState = ObjectState.Disabled;
				if(Item.UseReadOnlyAppearance) {
					tokenInfo.State = ObjectState.Disabled;
					return;
				}
			}
			if(Item.IsDesignMode) return;
			Point pt = OwnerControl != null ? OwnerControl.PointToClient(Control.MousePosition) : MousePosition;
			TokenEditToken token = tokenInfo.Token;
			if(token != null && !Item.ReadOnly && Item.CheckedItems.Contains(token)) {
				tokenInfo.State = ObjectState.Selected;
			}
			else {
				tokenInfo.State &= ~ObjectState.Selected;
			}
			if(HotToken.IsHitObjectEquals(tokenInfo)) {
				tokenInfo.State |= ObjectState.Hot;
			}
			else {
				tokenInfo.State &= ~ObjectState.Hot;
			}
			if(!Item.ReadOnly) {
				if(tokenInfo.GlyphBounds.Contains(pt)) {
					tokenInfo.GlyphState = ObjectState.Hot;
					if((Control.MouseButtons & MouseButtons.Left) != 0) tokenInfo.GlyphState |= ObjectState.Pressed;
				}
				else {
					tokenInfo.GlyphState = ObjectState.Normal;
				}
			}
		}
		public TokenEditHitInfo HotToken { get { return hotToken; } }
		protected internal void SetHotToken(TokenEditHitInfo hitInfo) {
			TokenEditHitInfo prev = HotToken;
			ObjectState? prevState = prev.GetTokenState();
			ObjectState? prevGlyphState = prev.GetTokenGlyphState();
			this.hotToken = hitInfo;
			if(prev.TokenInfo != null) {
				UpdateToken(prev.TokenInfo);
			}
			UpdateToken(hitInfo.TokenInfo);
			if(!HotToken.IsHitObjectEquals(prev)) {
				OnHotTokenChanged(prev.TokenInfo, hitInfo.TokenInfo);
			}
			else {
				if(prevState != HotToken.GetTokenState() || prevGlyphState != HotToken.GetTokenGlyphState()) DoInvalidate();
			}
		}
		protected internal virtual void OnHotTokenChanged(TokenEditTokenInfo prev, TokenEditTokenInfo next) {
			if(prev != null) {
				Item.RaiseTokenMouseLeave(TokenEditTokenBasedEventArgsBase.Create<TokenEditTokenMouseLeaveEventArgs>(prev));
			}
			if(next != null) {
				if(Item.AllowPopupPanel && OwnerEdit != null) OwnerEdit.TrackMouseHoverCore();
				Item.RaiseTokenMouseEnter(TokenEditTokenBasedEventArgsBase.Create<TokenEditTokenMouseEnterEventArgs>(next));
			}
			if(prev != null)
				UpdateToken(prev);
			if(next != null)
				UpdateToken(next);
			DoInvalidate();
		}
		protected void DoInvalidate() {
			if(OwnerControl != null) OwnerControl.Invalidate();
		}
		public new RepositoryItemTokenEdit Item { get { return base.Item as RepositoryItemTokenEdit; } }
		protected internal virtual void ResetHotToken() {
			TokenEditTokenInfo prevTokenInfo = HotToken.TokenInfo;
			if(prevTokenInfo == null) return;
			HotToken.Reset();
			OnHotTokenChanged(prevTokenInfo, null);
		}
		protected internal virtual void ResetSelectedToken() {
		}
		protected override EditHitInfo CreateHitInfo(Point p) {
			return new TokenEditHitInfo(p);
		}
		public override EditHitInfo CalcHitInfo(Point pt) {
			TokenEditHitInfo hi = (TokenEditHitInfo)base.CalcHitInfo(pt);
			if(!Bounds.Contains(pt)) return hi;
			foreach(TokenEditTokenInfo tokenInfo in VisibleItems) {
				if(tokenInfo.Bounds.Contains(pt)) {
					hi.SetHitObject(tokenInfo);
					hi.SetHitTest(TokenEditHitTest.Link);
					if(tokenInfo.GlyphBounds.Contains(pt)) hi.SetHitTest(TokenEditHitTest.Glyph);
					return hi;
				}
			}
			if(ContentRect.Contains(pt)) {
				hi.SetHitTest(TokenEditHitTest.EditorRect);
			}
			return hi;
		}
		protected virtual void UpdateTokenAppearance(TokenEditTokenInfo tokenInfo) {
			tokenInfo.PaintAppearance.Assign(PaintAppearance);
			tokenInfo.PaintAppearance.ForeColor = GetTokenForeColor(tokenInfo);
			tokenInfo.PaintAppearance.BackColor = GetTokenBackColor(tokenInfo);
		}
		protected virtual Color GetTokenForeColor(TokenEditTokenInfo tokenInfo) {
			return PaintAppearance.ForeColor;
		}
		protected virtual Color GetTokenBackColor(TokenEditTokenInfo tokenInfo) {
			return PaintAppearance.BackColor;
		}
		protected virtual int IndentBetweenRows { get { return 1; } }
		protected override Size CalcContentSize(Graphics g) {
			Size sz = base.CalcContentSize(g);
			sz.Width = ContentRect.Width;
			sz.Height = CalcContentHeight(VisibleRowCount);
			return sz;
		}
		protected int CalcContentHeight(int rowCount) {
			if(TextSize.IsEmpty) CalcTextSize(null);
			return (TextSize.Height + 2 * IndentBetweenRows) * rowCount + 2 * IndentBetweenRows + (rowCount - 1) * IndentBetweenRows;
		}
		public int VisibleRowCount {
			get {
				if(!AllowScrolling()) return RowCount;
				if(InplaceType != InplaceType.Standalone) {
					int rowCount = CalcInplaceVisibleRowCount();
					if(rowCount != 0) return rowCount;
				}
				return Math.Min(RowCount, Item.MaxExpandLines);
			}
		}
		protected virtual bool AllowScrolling() {
			if(InplaceType != InplaceType.Standalone) return true;
			return Item.AutoHeightMode == TokenEditAutoHeightMode.RestrictedExpand;
		}
		protected int CalcInplaceVisibleRowCount() {
			int rowHeight = CalcRowHeight();
			if(rowHeight == 0) return 0;
			return Bounds.Height / rowHeight;
		}
		public int CalcMaxTopRow() {
			return Math.Max(0, RowCount - VisibleRowCount);
		}
		public bool IsTopLeftToken(TokenEditToken token) {
			foreach(TokenEditTokenInfo tok in VisibleItems) {
				if(tok.RowIndex == 0 && tok.Bounds.X == ContentRect.X) {
					return object.ReferenceEquals(tok.Token, token);
				}
			}
			return false;
		}
		public bool IsTopRowToken(TokenEditToken token) {
			TokenEditTokenInfo tok = GetTokenInfo(token);
			return tok != null && tok.RowIndex == 0;
		}
		protected int CalcRowCount(int clientWidth) {
			if(clientWidth == 0 || Item.SelectedItems.Count == 0) return Item.MinRowCount;
			bool addGraphics = false;
			int rowCount;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				addGraphics = true;
			}
			try {
				rowCount = CalcRowCountCore(clientWidth);
			}
			finally {
				if(addGraphics) GInfo.ReleaseGraphics();
			}
			return Math.Max(rowCount, Item.MinRowCount);
		}
		protected virtual int CalcRowCountCore(int clientWidth) {
			TokenEditSelectedItemCollection col = Item.SelectedItems;
			int rowCount = 0, n = 0;
			do {
				rowCount++;
				n = DoTraverseRow(n, col, clientWidth);
			}
			while(n < col.Count);
			return rowCount;
		}
		protected int DoTraverseRow(int itemIndex, TokenEditSelectedItemCollection col, int clientWidth) {
			if(itemIndex >= col.Count - 1) return col.Count;
			int n, val = 0;
			for(n = itemIndex; n < col.Count; n++) {
				val += (CalcItemSize(col[n]).Width + IndentBetweenItems * 2);
				if(val > clientWidth) break;
			}
			return n > itemIndex ? n : n + 1;
		}
		public TokenEditTokenInfo GetTokenInfo(TokenEditToken token) {
			return Items[token] as TokenEditTokenInfo;
		}
		protected internal virtual TokenEditToken CalcTopToken(TokenEditToken token) {
			TokenEditTokenInfo tokenInfo = GetTokenInfo(token);
			if(tokenInfo == null || tokenInfo.RowIndex == 0)
				return null;
			return GetRelevantToken(tokenInfo, tokenInfo.RowIndex - 1);
		}
		protected internal virtual TokenEditToken CalcBottomToken(TokenEditToken token) {
			TokenEditTokenInfo tokenInfo = GetTokenInfo(token);
			if(tokenInfo == null || tokenInfo.RowIndex == RowCount - 1)
				return null;
			return GetRelevantToken(tokenInfo, tokenInfo.RowIndex + 1);
		}
		public override bool DefaultAllowDrawFocusRect { get { return true; } }
		protected TokenEditToken GetRelevantToken(TokenEditTokenInfo tokenInfo, int row) {
			TokenEditToken token = null;
			int maxIntersect = 0;
			Rectangle rect = tokenInfo.Bounds;
			foreach(TokenEditTokenInfo it in VisibleItems) {
				if(it.RowIndex != row) continue;
				rect.Y = it.Bounds.Y;
				int w = Rectangle.Intersect(it.Bounds, rect).Width;
				if(w > 0 && maxIntersect < w) {
					maxIntersect = w;
					token = it.Token;
				}
			}
			return token;
		}
		#region IHeightAdaptable
		int IHeightAdaptable.CalcHeight(GraphicsCache cache, int width) {
			int rowCount = RowCount > 0 ? RowCount : Item.MinRowCount;
			return CalcContentHeight(rowCount);
		}
		#endregion
		public new TokenEdit OwnerEdit { get { return base.OwnerEdit as TokenEdit; } }
	}
	public enum TokenEditHitTest { None, Link, Glyph, EditorRect }
	public class TokenEditHitInfo : EditHitInfo {
		TokenEditHitTest hitTest;
		public TokenEditHitInfo() : this(Point.Empty) { }
		public TokenEditHitInfo(Point pt)
			: base(pt) {
			this.hitTest = TokenEditHitTest.None;
		}
		protected internal void SetHitTest(TokenEditHitTest hitTest) {
			this.hitTest = hitTest;
		}
		protected internal void SetHitObject(TokenEditTokenInfo newValue) {
			base.SetHitObject(newValue);
		}
		public TokenEditTokenInfo TokenInfo { get { return HitObject as TokenEditTokenInfo; } }
		public void Reset() {
			this.fHitPoint = Point.Empty;
			this.fHitObject = null;
			this.hitTest = TokenEditHitTest.None;
		}
		public TokenEditToken Token {
			get { return TokenInfo != null ? TokenInfo.Token : null; }
		}
		public bool IsHitObjectEquals(TokenEditHitInfo other) {
			return other != null && IsHitObjectEquals(other.TokenInfo);
		}
		public bool IsHitObjectEquals(TokenEditTokenInfo value) {
			if(TokenInfo == null) {
				return value == null;
			}
			if(value == null) {
				return TokenInfo == null;
			}
			return object.ReferenceEquals(TokenInfo.Token, value.Token);
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if(TokenInfo != null) {
				sb.AppendFormat("TokenInfo: {0}{1}", TokenInfo.Token.Description, Environment.NewLine);
			}
			sb.AppendFormat("HitPoint: {0}, HitTest: {1})", HitPoint.ToString(), HitTest.ToString());
			return sb.ToString();
		}
		internal ObjectState? GetTokenState() {
			if(TokenInfo == null) return null;
			return TokenInfo.State;
		}
		internal ObjectState? GetTokenGlyphState() {
			if(TokenInfo == null) return null;
			return TokenInfo.GlyphState;
		}
		public bool InLink { get { return HitTest == TokenEditHitTest.Link; } }
		public bool InToken { get { return InLink || InGlyph; } }
		public bool InGlyph { get { return HitTest == TokenEditHitTest.Glyph; } }
		public bool InEditorRect { get { return HitTest == TokenEditHitTest.EditorRect; } }
		public new TokenEditHitTest HitTest { get { return hitTest; } }
	}
	public class TokenEditTokenInfo {
		TokenEditToken token;
		Rectangle bounds;
		Rectangle descriptionBounds;
		Rectangle glyphBounds;
		ObjectState state;
		ObjectState glyphState;
		int rowIndex;
		AppearanceObject paintAppearance;
		public TokenEditTokenInfo(TokenEditToken token) {
			this.rowIndex = 0;
			this.bounds = Rectangle.Empty;
			this.token = token;
			this.state = this.glyphState = ObjectState.Normal;
			this.paintAppearance = new AppearanceObject();
		}
		protected internal void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		protected internal void SetDescriptionBounds(Rectangle bounds) {
			this.descriptionBounds = bounds;
		}
		protected internal void SetGlyphBounds(Rectangle bounds) {
			this.glyphBounds = bounds;
		}
		protected internal void SetRowIndex(int rowIndex) {
			this.rowIndex = rowIndex;
		}
		protected internal void Offset(int x, int y) {
			this.bounds.Offset(x, y);
			this.descriptionBounds.Offset(x, y);
			if(!this.glyphBounds.IsEmpty) this.glyphBounds.Offset(x, y);
		}
		public TokenEditToken Token { get { return token; } }
		public override string ToString() {
			return string.Format("TokenEditTokenInfo({0})", Description);
		}
		public int RowIndex { get { return rowIndex; } }
		public string Description { get { return token.Description; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle DescriptionBounds { get { return descriptionBounds; } }
		public Rectangle GlyphBounds { get { return glyphBounds; } }
		public ObjectState State { get { return state; } set { state = value; } }
		public ObjectState GlyphState { get { return glyphState; } set { glyphState = value; } }
		public AppearanceObject PaintAppearance { get { return paintAppearance; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Rectangle CaptionBounds { get { return DescriptionBounds; } }
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class TokenEditPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			TokenEditViewInfo viewInfo = (TokenEditViewInfo)info.ViewInfo;
			base.DrawContent(info);
			if(viewInfo.AllowDrawText) {
				DrawText(info);
			}
			else {
				foreach(TokenEditTokenInfo tokenInfo in viewInfo.VisibleItems) {
					DoDrawToken(info, tokenInfo);
				}
			}
		}
		protected virtual void DoDrawToken(ControlGraphicsInfoArgs info, TokenEditTokenInfo tokenInfo) {
			DoDrawBackground(info, tokenInfo);
			DoDrawGlyph(info, tokenInfo);
			DoDrawDescription(info, tokenInfo);
		}
		protected virtual void DoDrawBackground(ControlGraphicsInfoArgs info, TokenEditTokenInfo tokenInfo) {
			TokenEditViewInfo viewInfo = (TokenEditViewInfo)info.ViewInfo;
			TokenEditCustomDrawTokenBackgroundEventArgs e = new TokenEditCustomDrawTokenBackgroundEventArgs(info.Cache, tokenInfo.Bounds, tokenInfo.Token.Description, tokenInfo.Token.Value, tokenInfo);
			e.SetDefaultDraw(() => {
				if(tokenInfo.State == ObjectState.Normal || tokenInfo.State == ObjectState.Disabled) return;
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetBackgroundInfo(viewInfo, tokenInfo));
			});
			viewInfo.Item.RaiseCustomDrawTokenBackground(e);
			e.DefaultDraw();
		}
		protected virtual SkinElementInfo GetBackgroundInfo(TokenEditViewInfo viewInfo, TokenEditTokenInfo tokenInfo) {
			SkinElement element = GetEditorSkinElement(viewInfo, EditorsSkins.SkinTokenEdit);
			if(element == null) {
				element = GetCommonSkinElement(viewInfo, CommonSkins.SkinHighlightedItem);
			}
			int imageIndex = 0;
			SkinElementInfo backgroundInfo = new SkinElementInfo(element, tokenInfo.Bounds);
			if((tokenInfo.State & ObjectState.Hot) != 0) {
				imageIndex = 0;
			}
			if((tokenInfo.State & ObjectState.Selected) != 0) {
				imageIndex = 1;
			}
			backgroundInfo.ImageIndex = imageIndex;
			return backgroundInfo;
		}
		protected virtual void DoDrawDescription(ControlGraphicsInfoArgs info, TokenEditTokenInfo tokenInfo) {
			TokenEditViewInfo viewInfo = (TokenEditViewInfo)info.ViewInfo;
			TokenEditCustomDrawTokenTextEventArgs e = new TokenEditCustomDrawTokenTextEventArgs(info.Cache, tokenInfo.Bounds, tokenInfo.Token.Description, tokenInfo.Token.Value, tokenInfo);
			e.SetDefaultDraw(() => {
				using(StringFormat format = new StringFormat()) {
					format.Trimming = StringTrimming.EllipsisCharacter;
					tokenInfo.PaintAppearance.DrawString(info.Cache, tokenInfo.Description, tokenInfo.DescriptionBounds, info.Cache.GetSolidBrush(GetDescriptionColorCore(viewInfo, tokenInfo)), format);
				}
			});
			viewInfo.Item.RaiseCustomDrawTokenText(e);
			e.DefaultDraw();
		}
		protected virtual Color GetDescriptionColorCore(TokenEditViewInfo viewInfo, TokenEditTokenInfo tokenInfo) {
			Color color = Color.Empty;
			SkinElement element = GetEditorSkinElement(viewInfo, EditorsSkins.SkinTokenEdit);
			if(element == null) {
				element = GetCommonSkinElement(viewInfo, CommonSkins.SkinHighlightedItem);
			}
			if((tokenInfo.State & ObjectState.Hot) != 0) {
				color = GetElementColor(element, "HotTextColor");
			}
			if((tokenInfo.State & ObjectState.Selected) != 0) {
				color = GetElementColor(element, "SelectedTextColor");
			}
			if(!color.IsEmpty) return color;
			return viewInfo.GetSystemColor(SystemColors.WindowText);
		}
		protected virtual void DoDrawGlyph(ControlGraphicsInfoArgs info, TokenEditTokenInfo tokenInfo) {
			TokenEditViewInfo viewInfo = (TokenEditViewInfo)info.ViewInfo;
			TokenEditCustomDrawTokenGlyphEventArgs e = new TokenEditCustomDrawTokenGlyphEventArgs(info.Cache, tokenInfo.GlyphBounds, tokenInfo.Bounds, tokenInfo.Token.Description, tokenInfo.Token.Value, tokenInfo);
			e.SetDefaultDraw(() => {
				if(viewInfo.Item.ShowTokenGlyph) ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetGlyphInfo(viewInfo, tokenInfo));
			});
			viewInfo.Item.RaiseCustomDrawTokenGlyph(e);
			e.DefaultDraw();
		}
		protected virtual void DrawText(ControlGraphicsInfoArgs info) {
			TokenEditViewInfo viewInfo = info.ViewInfo as TokenEditViewInfo;
			AppearanceObject appearance = viewInfo.PaintAppearance;
			appearance.DrawString(info.Cache, viewInfo.DisplayText, viewInfo.TextRect, appearance.GetForeBrush(info.Cache), appearance.GetTextOptions().GetStringFormat(info.ViewInfo.DefaultTextOptions));
		}
		protected virtual SkinElementInfo GetGlyphInfo(TokenEditViewInfo viewInfo, TokenEditTokenInfo tokenInfo) {
			SkinElement element = GetEditorSkinElement(viewInfo, EditorsSkins.SkinTokenEditCloseButton);
			if(element == null) {
				element = GetEditorSkinElement(viewInfo, EditorsSkins.SkinNavigator);
			}
			SkinElementInfo glyphInfo = new SkinElementInfo(element);
			glyphInfo.ImageIndex = GetGlyphImageIndex(viewInfo, tokenInfo);
			glyphInfo.Bounds = tokenInfo.GlyphBounds;
			return glyphInfo;
		}
		protected virtual int GetGlyphImageIndex(TokenEditViewInfo viewInfo, TokenEditTokenInfo tokenInfo) {
			SkinElement element = GetEditorSkinElement(viewInfo, EditorsSkins.SkinTokenEditCloseButton);
			if(element == null) return 10;
			SkinImage skinImage = element.Image;
			if(tokenInfo.State == ObjectState.Disabled) return 0;
			if(skinImage.ImageCount == 1)
				return 0;
			if(skinImage.ImageCount == 3) {
				if(tokenInfo.GlyphState == ObjectState.Normal) return 0;
				if((tokenInfo.GlyphState & ObjectState.Pressed) != 0) return 2;
				if((tokenInfo.GlyphState & ObjectState.Hot) != 0) return 1;
			}
			if(tokenInfo.State == ObjectState.Normal) return 0;
			if((tokenInfo.State & ObjectState.Selected) != 0) {
				if(tokenInfo.GlyphState == ObjectState.Normal) return 4;
				if((tokenInfo.GlyphState & ObjectState.Pressed) != 0) return 6;
				if((tokenInfo.GlyphState & ObjectState.Hot) != 0) return 5;
			}
			if(tokenInfo.GlyphState == ObjectState.Normal) return 1;
			if((tokenInfo.GlyphState & ObjectState.Pressed) != 0) return 3;
			return 2;
		}
		protected SkinElement GetEditorSkinElement(BaseControlViewInfo viewInfo, string element) {
			return EditorsSkins.GetSkin(viewInfo.LookAndFeel)[element];
		}
		protected SkinElement GetCommonSkinElement(BaseControlViewInfo viewInfo, string element) {
			return CommonSkins.GetSkin(viewInfo.LookAndFeel)[element];
		}
		protected Color GetElementColor(SkinElement element, string propName) {
			object prop = element.Properties[propName];
			if(prop == null) return Color.Empty;
			return (Color)prop;
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class TokenEditPopupForm : SimplePopupBaseForm, IPopupSizeableForm {
		TokenEdit tokenEdit;
		ITokenEditDropDownControl dropDownControl;
		TokenEditSizablePopupHelper sizablePopupHelper;
		public TokenEditPopupForm(TokenEdit tokenEdit) : base(tokenEdit) {
			this.tokenEdit = tokenEdit;
			this.dropDownControl = CreateDropDownControl();
			this.dropDownControl.Initialize(OwnerEdit, this);
			this.sizablePopupHelper = CreateSizablePopupHelper();
			DropDownControl.InitializeAppearances(Properties.AppearanceDropDown);
			Controls.Add((Control)DropDownControl);
			ViewInfo.ShowSizeBar = this.AllowSizing;
		}
		protected virtual ITokenEditDropDownControl CreateDropDownControl() {
			if(Properties.CustomDropDownControl != null) return Properties.CustomDropDownControl;
			return new DefaultTokenEditDropDownControl();
		}
		protected internal ITokenEditDropDownControl DropDownControl { get { return dropDownControl; } }
		protected virtual TokenEditSizablePopupHelper CreateSizablePopupHelper() {
			return new TokenEditSizablePopupHelper(this);
		}
		protected TokenEditSizablePopupHelper SizablePopupHelper { get { return sizablePopupHelper; } }
		public override void ShowPopupForm() {
			SizablePopupHelper.OnShowPopupForm();
			DropDownControl.OnShowingPopupForm();
			base.ShowPopupForm();
		}
		public override void HidePopupForm() {
			if(IsDisposed) return;
			SizablePopupHelper.OnHidePopupForm();
			if(Properties.PopupSizeable && !IsListEmpty) {
				Properties.PropertyStore["TokenEditPopupSize"] = Bounds.Size;
			}
			base.HidePopupForm();
		}
		protected internal virtual void UpdateDataSource() {
			DropDownControl.SetDataSource(GetDataSource());
		}
		protected internal void SetFilter(string filter, string columnName) {
			DropDownControl.SetFilter(filter, columnName);
		}
		internal SizeGripPosition CurrentSizeGripPosition = SizeGripPosition.RightBottom;
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = SizablePopupHelper.OnMouseMove(e);
			base.OnMouseMove(ee);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = SizablePopupHelper.OnMouseDown(e);
			base.OnMouseDown(ee);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = SizablePopupHelper.OnMouseUp(e);
			base.OnMouseUp(ee);
		}
		protected internal virtual int UpdateWidthWhenRightGrip(int width) {
			return width;
		}
		protected virtual Size UpdateMinFormSize(Size minSize) {
			return minSize;
		}
		#region IPopupSizeableForm
		int IPopupSizeableForm.UpdateWidthWhenRightGrip(int width) {
			return UpdateWidthWhenRightGrip(width);
		}
		Control IPopupSizeableForm.OwnerEdit { get { return OwnerEdit; } }
		bool IPopupSizeableForm.IsTopSizeBar {
			get { return ViewInfo.IsTopSizeBar; }
			set { ViewInfo.IsTopSizeBar = value; }
		}
		Form IPopupSizeableForm.Form { get { return this; } }
		Size IPopupSizeableForm.MinFormSize { get { return MinFormSize; } }
		Size IPopupSizeableForm.UpdateMinFormSize(Size minSize) {
			return UpdateMinFormSize(minSize);
		}
		bool IPopupSizeableForm.IsGripPoint(Point pt) {
			return ViewInfo.IsGripPoint(pt);
		}
		SizeGripPosition IPopupSizeableForm.CurrentSizeGripPosition {
			get { return CurrentSizeGripPosition; }
			set { CurrentSizeGripPosition = value; }
		}
		SizeGripPosition IPopupSizeableForm.GripPosition {
			get { return ViewInfo.GripObjectInfo.GripPosition; }
		}
		Cursor IPopupSizeableForm.CalcGripCursor(SizeGripPosition gripPos) {
			return ViewInfo.GripPainter.CalcGripCursor(gripPos);
		}
		void IPopupSizeableForm.DrawSizingRect(bool show) {
		}
		ResizeMode IPopupSizeableForm.CurrentPopupResizeMode { get { return ResizeMode.LiveResize; } }
		#endregion
		protected Color GetSystemColor(Color color) {
			return ViewInfo.GetSystemColor(color);
		}
		protected override PopupBaseFormPainter CreatePainter() {
			return new TokenEditPopupFormPainter();
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new TokenEditPopupFormViewInfo(this);
		}
		public virtual new RepositoryItemTokenEdit Properties { get { return base.Properties as RepositoryItemTokenEdit; } }
		public override void ProcessKeyDown(KeyEventArgs e) {
			base.ProcessKeyDown(e);
			if(e.Handled) return;
			if(Properties.IsPopupNavigationKey(e.KeyData)) {
				DropDownControl.ProcessKeyDown(e);
				e.Handled = true;
			}
			else if(e.KeyData == Keys.Enter || e.KeyCode == Keys.Tab) {
				DropDownControl.DoPopupClosing();
				e.Handled = true;
				ClosePopup(DropDownControl.IsTokenSelected ? PopupCloseMode.Normal : PopupCloseMode.Cancel);
				return;
			}
			else if(Properties.IsPopupCloseKey(e.KeyData)) {
				e.Handled = true;
				ClosePopup(PopupCloseMode.Cancel);
			}
		}
		public void OnEditorMouseWheel(MouseEventArgs e) {
			DropDownControl.OnEditorMouseWheel(e);
		}
		protected virtual new TokenEditPopupFormViewInfo ViewInfo { get { return base.ViewInfo as TokenEditPopupFormViewInfo; } }
		public override object ResultValue {
			get { return DropDownControl.GetResultValue(); }
		}
		protected override Size CalcFormSizeCore() {
			int itemCount = Math.Min(DropDownControl.GetItemCount(), Properties.DropDownRowCount);
			if(itemCount == 0) return Size.Empty;
			Size storedSize = GetStoredPopupSize();
			Size size = new Size(CalcFormWidth(), CalcFormHeight(itemCount));
			size = CalcFormSize(size);
			if(storedSize.Height > 0) {
				storedSize.Height = Math.Max(storedSize.Height, size.Height);
				return storedSize;
			}
			if(storedSize.Width != 0) size.Width = storedSize.Width;
			return size;
		}
		protected Size GetStoredPopupSize() {
			if(Properties.IsOutlookDropDown) return Size.Empty;
			Size size = Size.Empty;
			object obj = Properties.PropertyStore["TokenEditPopupSize"];
			if(obj != null && Properties.PopupSizeable) {
				size = (Size)obj;
			}
			return size;
		}
		public virtual int CalcFormWidth() {
			return DropDownControl.CalcFormWidth();
		}
		protected virtual int CalcFormHeight(int itemCount) {
			return DropDownControl.CalcFormHeight(itemCount);
		}
		protected internal virtual IList GetDataSource() {
			bool skipAutoPopulated = Properties.IsDisableAutoPopulate;
			TokenEditTokenCollection tokCol = Properties.Tokens;
			TokenEditSelectedItemCollection selCol = Properties.SelectedItems;
			if(selCol.Count == 0 && !skipAutoPopulated) return tokCol;
			HashSet<int> indices = new HashSet<int>();
			for(int i = 0; i < selCol.Count; i++) {
				TokenEditToken tok = selCol[i];
				indices.Add(tokCol.IndexOf(tok));
			}
			List<TokenEditToken> list = new List<TokenEditToken>(tokCol.Count);
			for(int i = 0; i < tokCol.Count; i++) {
				if(indices.Contains(i) || (skipAutoPopulated && tokCol[i].AutoPopulated)) continue;
				list.Add(tokCol[i]);
			}
			return list;
		}
		protected override Control EmbeddedControl { get { return (Control)DropDownControl; } }
		protected internal override void ResetResultValue() {
			DropDownControl.ResetResultValue();
		}
		protected override void CancelPopup() {
			OwnerEdit.ClosePopup(PopupCloseMode.Cancel);
		}
		protected override void ClosePopup(PopupCloseMode closeMode) {
			OwnerEdit.ClosePopup(closeMode);
		}
		protected override void DestroyPopupForm() {
			OwnerEdit.DestroyPopupFormCore(true);
		}
		public bool IsListEmpty { get { return DropDownControl.GetItemCount() == 0; } }
		public virtual void Assign(UserLookAndFeel lookAndFeel) {
			LookAndFeel.Assign(lookAndFeel);
			DropDownControl.InitializeAppearances(Properties.AppearanceDropDown);
		}
		public void ResetSelection() {
			DropDownControl.ResetSelection();
		}
		protected override Size PopupFormMinSize { get { return Size.Empty; } }
		protected override Size DefaultMinFormSize { get { return Size.Empty; } }
		protected override bool IsImeInProgress { get { return OwnerEdit.MaskBox.IsImeInProgress; } }
		protected override bool CanShowShadow { get { return false; } }
		public bool CurrentSizing { get { return SizablePopupHelper.Sizing; } }
		public bool AllowSizing { get { return Properties.PopupSizeable; } }
		public override PopupBorderStyles PopupBorderStyle { get { return PopupBorderStyles.Default; } }
		public override AppearanceObject AppearanceDropDown { get { return Properties.AppearanceDropDown; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Control customDropDownControl = OwnerEdit.Properties.CustomDropDownControl;
				if(customDropDownControl != null && Controls.Contains(customDropDownControl)) {
					Controls.Remove(customDropDownControl);
				}
				if(SizablePopupHelper != null) {
					SizablePopupHelper.Dispose();
				}
			}
			this.tokenEdit = null;
			this.sizablePopupHelper = null;
			base.Dispose(disposing);
		}
		public virtual new TokenEdit OwnerEdit { get { return base.OwnerEdit as TokenEdit; } }
	}
	public class TokenEditSizablePopupHelper : SizablePopupHelper {
		public TokenEditSizablePopupHelper(IPopupSizeableForm owner)
			: base(owner) {
		}
	}
	public class TokenEditPopupFormViewInfo : SimplePopupBaseSizeableFormViewInfo {
		public TokenEditPopupFormViewInfo(TokenEditPopupForm form)
			: base(form) {
		}
		public virtual new TokenEditPopupForm Form { get { return base.Form as TokenEditPopupForm; } }
		protected override bool AllowSizing { get { return Form.AllowSizing; } }
		protected override bool CurrentSizing { get { return Form.CurrentSizing; } }
		protected override SizeGripPosition CurrentSizeGripPosition { get { return Form.CurrentSizeGripPosition; } }
	}
	public class TokenEditPopupFormPainter : PopupBaseSizeableFormPainter {
	}
	[ToolboxItem(false)]
	public class TokenEditPopupListBox : SimplePopupListBox {
		DefaultTokenEditDropDownControl ownerControl;
		public TokenEditPopupListBox(TokenEditPopupForm popupForm, DefaultTokenEditDropDownControl ownerControl)
			: base(popupForm) {
			this.ownerControl = ownerControl;
		}
		protected override void SetLastSelectedIndex(int index) {
			ownerControl.SelItemIndex = index;
		}
		protected override object GetDataSource() {
			return PopupForm.GetDataSource();
		}
		public override void SetFilter(string text, string column) {
			base.SetFilter(text, column);
			OwnerEdit.UpdatePopupFormSize(ItemCount);
		}
		protected override string CreateFilterExpression(string text, string column) {
			return CriteriaOperator.ToString(new FunctionOperator(GetFunctionOperator(), new OperandProperty(column), text));
		}
		protected FunctionOperatorType GetFunctionOperator() {
			FunctionOperatorType op = FunctionOperatorType.None;
			switch(Properties.PopupFilterMode) {
				case TokenEditPopupFilterMode.StartWith:
					op = FunctionOperatorType.StartsWith;
					break;
				case TokenEditPopupFilterMode.Contains:
					op = FunctionOperatorType.Contains;
					break;
				default: throw new ArgumentException("PopupFilterMode");
			}
			return op;
		}
		protected RepositoryItemTokenEdit Properties { get { return OwnerEdit.Properties; } }   
		public override int CalcItemWidth(GraphicsInfo gInfo, object item) {
			return base.CalcItemWidth(gInfo, item) + GetItemActionPlaceholderWidth();
		}
		protected virtual Size DefaultItemActionImageSize { get { return new Size(16, 16); } }
		protected virtual int GetItemActionPlaceholderWidth() {
			return DefaultItemActionImageSize.Width + 2 * ViewInfo.DefaultItemActionPadding;
		}
		protected internal override bool HasItemActions {
			get { return Properties.ShowRemoveTokenButtons; }
		}
		protected internal override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
			itemInfo.ActionInfo = new ListItemActionCollection();
			itemInfo.ActionInfo.Add(new TokenEditPopupListItemDeleteActionInfo(itemInfo));
		}
		public new TokenEdit OwnerEdit { get { return base.OwnerEdit as TokenEdit; } }
		protected internal override void OnActionItemClick(ListItemActionInfo action) {
			base.OnActionItemClick(action);
			Properties.Tokens.Remove((TokenEditToken)action.Item);
			CheckDataSource();
			OwnerEdit.UpdatePopupFormSize(ItemCount);
		}
		protected virtual void CheckDataSource() {
			PopupForm.UpdateDataSource();
		}
		protected internal new TokenEditPopupListBoxViewInfo ViewInfo { get { return base.ViewInfo as TokenEditPopupListBoxViewInfo; } }
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new TokenEditPopupListBoxViewInfo(this);
		}
		public TokenEditPopupForm PopupForm { get { return base.OwnerForm as TokenEditPopupForm; } }
	}
	public class TokenEditPopupListItemDeleteActionInfo : ListItemActionInfo {
		public TokenEditPopupListItemDeleteActionInfo(BaseListBoxViewInfo.ItemInfo info) : base(info) { }
		public override ObjectPainter Painter { get { return new ListItemDeleteActionPainter(); } }
	}
	public class TokenEditPopupListBoxViewInfo : SimplePopupListBoxViewInfo {
		public TokenEditPopupListBoxViewInfo(TokenEditPopupListBox owner)
			: base(owner) {
		}
		protected internal override int CalcItemMinHeight() {
			int minHeight = base.CalcItemMinHeight();
			if(AllowInflateItem) minHeight += GetInflateValue();
			return minHeight;
		}
		protected override bool ContainsItemAction(ItemInfo itemInfo) {
			if(!HasItemActions || !OwnerControl.IsHandleCreated) return false;
			return itemInfo.Index == HotItemIndex;
		}
		protected virtual bool AllowInflateItem { get { return true; } }
		protected virtual int GetInflateValue() {
			return 6;
		}
	}
	public abstract class TokenEditDropDownBoundsCalculatorBase {
		public abstract Rectangle CalcBounds(TokenEdit tokenEdit, Size size);
	}
	public class TokenEditRegularDropDownBoundsCalculator : TokenEditDropDownBoundsCalculatorBase {
		public override Rectangle CalcBounds(TokenEdit tokenEdit, Size size) {
			size.Width = Math.Max(size.Width, tokenEdit.Width);
			Point pt = tokenEdit.PointToScreen(new Point(0, tokenEdit.Height));
			pt.Offset(tokenEdit.Properties.PopupOffset.X, tokenEdit.Properties.PopupOffset.Y);
			Point newLoc = ControlUtils.CalcLocation(pt, new Point(pt.X, pt.Y - tokenEdit.Height), size);
			return ControlUtils.ConstrainFormBounds(tokenEdit, new Rectangle(newLoc, size));
		}
	}
	public class TokenEditOutlookDropDownBoundsCalculator : TokenEditDropDownBoundsCalculatorBase {
		public override Rectangle CalcBounds(TokenEdit tokenEdit, Size size) {
			Rectangle maskBounds = tokenEdit.MaskBox.Bounds;
			int hotPt = tokenEdit.IsRightToLeft ? maskBounds.Right : maskBounds.X;
			Point pt = tokenEdit.PointToScreen(new Point(hotPt, tokenEdit.Height));
			pt.Offset(tokenEdit.Properties.PopupOffset.X, tokenEdit.Properties.PopupOffset.Y);
			Point newLoc = ControlUtils.CalcLocation(pt, new Point(pt.X, pt.Y - tokenEdit.Height), size);
			return ControlUtils.ConstrainFormBounds(tokenEdit, new Rectangle(newLoc, size));
		}
	}
	public abstract class BaseTokenEditPopupController : IDisposable {
		TokenEdit ownerEdit;
		public BaseTokenEditPopupController(TokenEdit tokenEdit) {
			this.ownerEdit = tokenEdit;
		}
		public virtual void OnTextChanged() {
			if(!AllowShowPopup()) {
				EnsureHidePopup(PopupCloseMode.Cancel);
				return;
			}
			UpdateFilter(GetFilter());
			ShowPopupIfNeeded();
		}
		public virtual bool OnMaskBoxChar(char keyChar) {
			if(!OwnerEdit.Properties.IsSeparatorChar(keyChar)) return false;
			return OnSeparatorEntered();
		}
		public virtual void OnTokenDeleted() {
			EnsureHidePopup(PopupCloseMode.Cancel);
		}
		public virtual void UpdateFilter(string filter) {
			SetFilterCore(filter);
			OnFilterChanged();
		}
		protected virtual void ShowPopupIfNeeded() {
			if(OwnerEdit.IsPopupOpen || !OwnerEdit.CanShowDropDown()) return;
			OwnerEdit.DoShowPopup(false);
		}
		protected virtual void EnsureHidePopup(PopupCloseMode popupCloseMode) {
			if(!OwnerEdit.IsPopupOpen) return;
			OwnerEdit.ClosePopup(popupCloseMode);
		}
		protected virtual bool OnSeparatorEntered() {
			bool updated = OwnerEdit.UpdateEditValueOnClosePopup();
			if(updated) {
				UpdateFilter(string.Empty);
				OwnerEdit.ResetEditorText();
			}
			return updated;
		}
		public virtual void DoShowPopup() { }
		public virtual void DoClosePopup(PopupCloseMode closeMode) { }
		protected virtual void SetFilterCore(string filter) {
			OwnerEdit.Popup.UpdateDataSource();
			OwnerEdit.Popup.SetFilter(filter, "Description");
		}
		protected string GetFilter() {
			string maskText = OwnerEdit.MaskBox.MaskBoxText;
			return maskText.TrimStart();
		}
		protected virtual bool AllowShowPopup() {
			return !OwnerEdit.MaskBox.Empty;
		}
		protected abstract void OnFilterChanged();
		#region Dispose
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			this.ownerEdit = null;
		}
		public TokenEdit OwnerEdit { get { return ownerEdit; } }
	}
	public class TokenEditManualModePopupController : BaseTokenEditPopupController {
		public TokenEditManualModePopupController(TokenEdit tokenEdit)
			: base(tokenEdit) {
		}
		protected override void OnFilterChanged() {
			if(OwnerEdit.Popup.IsListEmpty) EnsureHidePopup(PopupCloseMode.Cancel);
		}
	}
	public class TokenEditTokenListPopupController : BaseTokenEditPopupController {
		public TokenEditTokenListPopupController(TokenEdit tokenEdit)
			: base(tokenEdit) {
		}
		public override void DoClosePopup(PopupCloseMode popupCloseMode) {
			base.DoClosePopup(popupCloseMode);
			if(popupCloseMode != PopupCloseMode.Normal) RollbackChanges();
		}
		protected virtual void RollbackChanges() {
		}
		protected override void OnFilterChanged() {
			if(OwnerEdit.Popup.IsListEmpty) OnPopupListBoxEmpty();
		}
		protected virtual void OnPopupListBoxEmpty() {
			SetFilterCore(string.Empty);
			OwnerEdit.UpdatePopupFormHeight();
			OwnerEdit.Popup.ResetSelection();
		}
	}
	public class PopupPanelController : IFlyoutPanelPopupController, IDisposable {
		TokenEdit ownerEdit;
		public PopupPanelController(TokenEdit ownerEdit) {
			this.ownerEdit = ownerEdit;
		}
		Timer delayTimer = null;
		protected Timer DelayTimer {
			get {
				if(this.delayTimer == null) this.delayTimer = CreateTimerCore();
				return delayTimer;
			}
		}
		protected virtual Timer CreateTimerCore() {
			Timer timer = new Timer();
			timer.Interval = (int)(SystemInformation.MouseHoverTime * 1.5);
			timer.Tick += (sender, e) => {
				if(!DelayTimer.Enabled) return;
				EnsureDisableTimer();
				EnsureHidePopupPanel(false);
			};
			return timer;
		}
		TokenEditTokenInfo activeToken = null;
		public virtual void ShowPopupPanel(TokenEditTokenInfo token) {
			if(IsWaitForDelayedClosing && (this.activeToken != null && this.activeToken == token)) {
				EnsureCancelClosePopup();
				return;
			}
			EnsureCancelClosePopup();
			EnsureHidePopupPanel(true);
			this.activeToken = token;
			RaiseBeforeShowPopupPanel(token);
			OwnerEdit.Properties.PopupPanel.OptionsBeakPanel.BeakLocation = GetBeakLocation();
			OwnerEdit.Properties.PopupPanel.ShowBeakForm(GetPopupLocation(token), false, OwnerEdit, Point.Empty, this);
		}
		public virtual void EnsureHidePopupPanel() {
			if(OwnerEdit.Properties.PopupPanel.IsPopupOpen) EnsureHidePopupPanel(true);
		}
		protected virtual void RaiseBeforeShowPopupPanel(TokenEditTokenInfo token) {
			TokenEditBeforeShowPopupPanelEventArgs e = TokenEditTokenBasedEventArgsBase.Create<TokenEditBeforeShowPopupPanelEventArgs>(token);
			OwnerEdit.Properties.RaiseBeforeShowPopupPanel(e);
		}
		protected virtual void EnsureHidePopupPanel(bool immediate) {
			EnsureDisableTimer();
			OwnerEdit.Properties.PopupPanel.HideBeakForm(immediate);
		}
		protected virtual void HidePopupPanelDelayed() {
			DelayTimer.Enabled = true;
		}
		protected Point GetPopupLocation(TokenEditTokenInfo token) {
			Point mousePos = Control.MousePosition;
			Rectangle tokenBounds = OwnerEdit.RectangleToScreen(token.Bounds);
			switch(OwnerEdit.Properties.GetPopupPanelLocation()) {
				case TokenEditPopupPanelLocation.OnMouseCursor:
					return mousePos;
				case TokenEditPopupPanelLocation.AboveToken:
					return new Point(tokenBounds.X + tokenBounds.Width / 2, tokenBounds.Top);
				case TokenEditPopupPanelLocation.BelowToken:
					return new Point(tokenBounds.X + tokenBounds.Width / 2, tokenBounds.Bottom);
			}
			return mousePos;
		}
		protected virtual BeakPanelBeakLocation GetBeakLocation() {
			TokenEditPopupPanelLocation panelLoc = OwnerEdit.Properties.GetPopupPanelLocation();
			if(panelLoc == TokenEditPopupPanelLocation.BelowToken) {
				return BeakPanelBeakLocation.Top;
			}
			return BeakPanelBeakLocation.Bottom;
		}
		protected bool IsWaitForDelayedClosing { get { return DelayTimer.Enabled; } }
		protected virtual void EnsureDisableTimer() {
			DelayTimer.Enabled = false;
		}
		protected virtual void EnsureCancelClosePopup() {
			EnsureDisableTimer();
		}
		public void Dispose() { Dispose(true); }
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.delayTimer != null) {
					this.delayTimer.Dispose();
				}
				this.delayTimer = null;
			}
			this.activeToken = null;
			this.delayTimer = null;
			this.ownerEdit = null;
		}
		#region IFlyoutPanelPopupController
		bool IFlyoutPanelPopupController.AllowCloseOnMouseMove(Rectangle formBounds, Point pt) {
			return AllowCloseOnMouseMoveCore(formBounds, pt);
		}
		#endregion
		protected virtual bool AllowCloseOnMouseMoveCore(Rectangle formBounds, Point pt) {
			if(formBounds.Contains(pt)) return false;
			if(this.activeToken != null) {
				Rectangle tokenBounds = OwnerEdit.RectangleToScreen(this.activeToken.Bounds);
				if(tokenBounds.Contains(pt)) return false;
			}
			HidePopupPanelDelayed();
			return false;
		}
		public TokenEdit OwnerEdit { get { return ownerEdit; } }
	}
	#region Popup Extensibility
	public interface ITokenEditDropDownControl {
		void Initialize(TokenEdit tokenEdit, TokenEditPopupForm owner);
		void InitializeAppearances(AppearanceObject appearanceDropDown);
		void OnShowingPopupForm();
		void DoPopupClosing();
		void SetDataSource(object dataSource);
		void SetFilter(string filter, string columnName);
		bool IsTokenSelected { get; }
		void ResetSelection();
		void ProcessKeyDown(KeyEventArgs e);
		void OnEditorMouseWheel(MouseEventArgs e);
		void ResetResultValue();
		int CalcFormWidth();
		int CalcFormHeight(int itemCount);
		int GetItemCount();
		object GetResultValue();
	}
	#endregion
	[ToolboxItem(false)]
	public abstract class TokenEditDropDownControlBase : XtraUserControl, ITokenEditDropDownControl {
		TokenEdit ownerEdit;
		TokenEditPopupForm popupForm;
		public TokenEditDropDownControlBase() {
			this.ownerEdit = null;
			this.popupForm = null;
		}
		public virtual void Initialize(TokenEdit ownerEdit, TokenEditPopupForm ownerPopup) {
			this.ownerEdit = ownerEdit;
			this.popupForm = ownerPopup;
		}
		public abstract void InitializeAppearances(AppearanceObject appearanceDropDown);
		public abstract void OnShowingPopupForm();
		public abstract void SetDataSource(object dataSource);
		public abstract void SetFilter(string filter, string columnName);
		public abstract bool IsTokenSelected { get; }
		public abstract void ResetSelection();
		public abstract void ProcessKeyDown(KeyEventArgs e);
		public abstract void OnEditorMouseWheel(MouseEventArgs e);
		public abstract void ResetResultValue();
		public abstract void DoPopupClosing();
		public abstract int CalcFormWidth();
		public abstract int CalcFormHeight(int itemCount);
		public abstract int GetItemCount();
		public abstract object GetResultValue();
		protected TokenEdit OwnerEdit { get { return ownerEdit; } }
	}
	[ToolboxItem(false)]
	public class DefaultTokenEditDropDownControl : TokenEditDropDownControlBase {
		TokenEditPopupListBox listBox;
		PopupListFormHelper popupListFormHelper;
		public DefaultTokenEditDropDownControl() {
			this.SelItemIndex = -1;
		}
		protected internal int SelItemIndex { get; set; }
		protected virtual PopupListFormHelper CreatePopupListFormHelper() {
			return new PopupListFormHelper(ListBox);
		}
		public override void Initialize(TokenEdit tokenEdit, TokenEditPopupForm ownerPopup) {
			base.Initialize(tokenEdit, ownerPopup);
			this.listBox = CreateListBox(ownerPopup);
			Controls.Add(ListBox);
			ListBox.Dock = DockStyle.Fill;
			this.popupListFormHelper = CreatePopupListFormHelper();
			SubscribeListBoxEvents();
		}
		protected virtual void SubscribeListBoxEvents() {
			if(ListBox == null) return;
			ListBox.SelectedIndexChanged += OnListBoxSelectedIndexChanged;
		}
		protected virtual void UnsubscribeListBoxEvents() {
			if(ListBox == null) return;
			ListBox.SelectedIndexChanged -= OnListBoxSelectedIndexChanged;
		}
		protected virtual TokenEditPopupListBox CreateListBox(TokenEditPopupForm owner) {
			return new TokenEditPopupListBox(owner, this);
		}
		protected virtual void OnListBoxSelectedIndexChanged(object sender, EventArgs e) {
			this.SelItemIndex = ListBox.SelectedIndex;
		}
		public override void OnShowingPopupForm() {
			ListBox.HotTrackItems = false;
			if(ListBox.TopIndex > ListBox.Model.GetMaxTopIndex()) ListBox.TopIndex = 0;
		}
		public override void SetDataSource(object obj) {
			ListBox.DataSource = obj;
		}
		protected RepositoryItemTokenEdit Properties { get { return OwnerEdit.Properties; } }
		public override void InitializeAppearances(AppearanceObject appearanceDropDown) {
			InitListBoxAppearance(appearanceDropDown);
			ListBox.UpdateProperties();
		}
		public SimplePopupListBox ListBox { get { return listBox; } }
		protected internal virtual void InitListBoxAppearance(AppearanceObject appearanceDropDown) {
			if(OwnerEdit.StyleController != null) {
				AppearanceHelper.Combine(ListBox.Appearance, appearanceDropDown, OwnerEdit.StyleController.AppearanceDropDown);
			}
			else {
				ListBox.Appearance.Assign(appearanceDropDown);
			}
			ListBox.LookAndFeel.ParentLookAndFeel = Properties.LookAndFeel;
		}
		public override void SetFilter(string filter, string columnName) {
			ListBox.SetFilter(filter, columnName);
		}
		public override bool IsTokenSelected { get { return SelItemIndex >= 0; } }
		public override object GetResultValue() {
			if(SelItemIndex < 0 || SelItemIndex >= ListBox.ItemCount) return OwnerEdit.EditValue;
			return ListBox.GetItem(SelItemIndex);
		}
		public override void ResetSelection() {
			ListBox.SelectedIndex = -1;
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			ListBox.ProcessKeyDown(e);
		}
		public override void OnEditorMouseWheel(MouseEventArgs e) {
			int step = SystemInformation.MouseWheelScrollLines > 0 ? SystemInformation.MouseWheelScrollLines : 1;
			if(e.Delta > 0) step *= -1;
			ListBox.TopIndex += step;
		}
		public override void ResetResultValue() {
			SelItemIndex = -1;
		}
		public override void DoPopupClosing() {
			SelItemIndex = ListBox.SelectedIndex;
		}
		public override int CalcFormWidth() {
			return popupListFormHelper.CalcMinimumComboWidth(Properties.Tokens, Properties.DropDownRowCount);
		}
		public override int CalcFormHeight(int itemCount) {
			return popupListFormHelper.CalcFormHeight(itemCount);
		}
		public override int GetItemCount() {
			return ListBox.ItemCount;
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeListBoxEvents();
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false)]
	public class CustomTokenEditDropDownControlBase : TokenEditDropDownControlBase {
		public CustomTokenEditDropDownControlBase() {
		}
		public override void Initialize(TokenEdit ownerEdit, TokenEditPopupForm ownerPopup) {
			base.Initialize(ownerEdit, ownerPopup);
		}
		public override void OnShowingPopupForm() {
		}
		public override void SetDataSource(object dataSource) {
		}
		public override void InitializeAppearances(AppearanceObject appearanceDropDown) {
		}
		public override void SetFilter(string filter, string columnName) {
		}
		public override object GetResultValue() {
			return null;
		}
		public override bool IsTokenSelected {
			get { return false; }
		}
		public override void ResetSelection() {
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
		}
		public override void OnEditorMouseWheel(MouseEventArgs e) {
		}
		public override void ResetResultValue() {
		}
		public override void DoPopupClosing() {
		}
		public override int CalcFormWidth() {
			return 0;
		}
		public override int CalcFormHeight(int itemCount) {
			return 0;
		}
		public override int GetItemCount() {
			return 0;
		}
	}
}
