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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Skins;
using System.Windows.Forms;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.SearchControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign), ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(SearchControlFilter)), Description("Provides the search and filter functionality for the attached object."), ToolboxBitmap(typeof(ToolboxIconsRootNS), "SearchControl")]
	public class SearchControl : MRUEdit, ISearchControl {
		Timer autoSearchTimer;
		int lockAutoSearch;
		public SearchControl() {
			CreateAutoSearchTimer();
			lockAutoSearch = 0;
		}
		protected override void Dispose(bool disposing) {
			DestroyAutoSearchTimer();
			base.Dispose(disposing);
		}
		public override string EditorTypeName {
			get { return "SearchControl"; }
		}
		protected override bool AllowSelectAllOnPopupClose { get { return false; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SearchControlProperties"),
#endif
 DXCategory(CategoryName.Properties),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemSearchControl Properties {
			get { return base.Properties as RepositoryItemSearchControl; }
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SearchControlClient"),
#endif
 DXCategory(CategoryName.Behavior)]
		public ISearchControlClient Client {
			get { return Properties.Client; }
			set { Properties.Client = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SearchControlQueryIsSearchColumn"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryIsSearchColumnEventHandler QueryIsSearchColumn {
			add { Properties.QueryIsSearchColumn += value; }
			remove { Properties.QueryIsSearchColumn -= value; }
		}
		protected override void OnDefaultPressButton(Drawing.EditorButtonObjectInfoArgs buttonInfo) {
			IDefaultEditorButton defButtron = buttonInfo.Button as IDefaultEditorButton;
			if(defButtron != null) {
				if(defButtron is SearchButton)
					ActionSearch(true);
				if(defButtron is ClearButton)
					ActionClear();
				if(defButtron is MRUButton)
					ActionShowPopup(buttonInfo);
			}
		}
		protected virtual void ActionSearch() {
			if(Properties.IsProviderRegistered)
				Client.ApplyFindFilter(Properties.Provider.GetCriteriaInfo());
		}
		protected virtual void ActionSearch(bool validation) {
			ActionSearch();
			if(validation)
				DoValidate();
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			if(e.Handled) return;
			if(e.KeyData == Keys.Enter)
				ActionSearch();
			if(e.KeyData == Keys.Escape) {
				ActionClear();
				e.Handled = true;
			}
			base.OnEditorKeyDown(e);
		}
		public void ClearFilter() { ActionClear(); }
		public void SetFilter(string filter) {
			lockAutoSearch++;
			this.EditValue = filter;
			ActionSearch();
			lockAutoSearch--;
		}
		protected virtual void ActionClear() {
			lockAutoSearch++;
			this.EditValue = null;
			ActionSearch();
			lockAutoSearch--;
		}
		#region AutoSearch
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(Properties.AllowAutoApply && lockAutoSearch == 0)
				StartAutoFilterTimer();
			if(IsMaskBoxUpdate)
				LayoutChanged();
		}
		void StartAutoFilterTimer() {
			autoSearchTimer.Stop();
			autoSearchTimer.Interval = Properties.FindDelay;
			autoSearchTimer.Start();
		}
		void AutoSearch(object sender, EventArgs e) {
			autoSearchTimer.Stop();
			if(!Properties.IsProviderRegistered) return;
			if((InplaceType == XtraEditors.Controls.InplaceType.Standalone) && !Visible) return;
			ActionSearch();
		}
		void CreateAutoSearchTimer() {
			autoSearchTimer = new Timer();
			autoSearchTimer.Tick += AutoSearch;
		}
		void DestroyAutoSearchTimer() {
			if(autoSearchTimer == null) return;
			autoSearchTimer.Tick -= AutoSearch;
			autoSearchTimer.Dispose();
			autoSearchTimer = null;
		}
		#endregion
	}
	public delegate void QueryIsSearchColumnEventHandler(object sender, QueryIsSearchColumnEventArgs args);
	public class QueryIsSearchColumnEventArgs {
		public QueryIsSearchColumnEventArgs() {
			IsSearchColumn = true;
		}
		public bool IsSearchColumn { get; set; }
	}
}
namespace DevExpress.XtraEditors.Repository {
	public enum ShowDefaultButtonsMode { Default, AutoShowClear, AutoChangeSearchToClear, Always }
	public class RepositoryItemSearchControl : RepositoryItemMRUEdit {
		ShowDefaultButtonsMode showDefaultButtonsMode;
		FilterCondition filterConditionCore;
		bool allowAutoApply;
		int findDelay;
		static readonly object queryIsSearchColumn = new object();
		ISearchControlClient clientCore;
		ISearchControlCriteriaProvider providerCore;
		public RepositoryItemSearchControl()
			: base() {
			SetDefaultSettings();
		}
		protected virtual void SetDefaultSettings() {
			filterConditionCore = Data.Filtering.FilterCondition.Default;
			allowAutoApply = true;
			findDelay = 1000;
			NullValuePromptShowForEmptyValue = true;
			ShowSearchButton = true;
			ShowClearButton = true;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlShowDefaultButtonsMode"),
#endif
 DefaultValue(ShowDefaultButtonsMode.Default), SmartTagProperty("Show Default Buttons Mode", "", 6)]
		public ShowDefaultButtonsMode ShowDefaultButtonsMode {
			get { return showDefaultButtonsMode; }
			set {
				if(showDefaultButtonsMode == value) return;
				showDefaultButtonsMode = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlFindDelay"),
#endif
 DefaultValue(1000), SmartTagProperty("Find Delay", "", 5)]
		public virtual int FindDelay {
			get { return findDelay; }
			set {
				if(value < 100) value = 100;
				if(FindDelay == value) return;
				findDelay = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlAllowAutoApply"),
#endif
 SmartTagProperty("Allow Auto Apply", "", 4, SmartTagActionType.RefreshContentAfterExecute)]
		public bool AllowAutoApply {
			get { return allowAutoApply; }
			set {
				if(allowAutoApply == value) return;
				allowAutoApply = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlFilterCondition"),
#endif
 DefaultValue(FilterCondition.Default)]
		public FilterCondition FilterCondition {
			get { return filterConditionCore; }
			set {
				if(filterConditionCore == value) return;
				filterConditionCore = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlShowSearchButton"),
#endif
 DefaultValue(true), SmartTagProperty("Show Search Button", "", 0)]
		public bool ShowSearchButton {
			get { return GetShowButton(typeof(SearchButton)); }
			set {
				SetShowButton(typeof(SearchButton), value);
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlShowClearButton"),
#endif
 DefaultValue(true), SmartTagProperty("Show Clear Button", "", 1)]
		public bool ShowClearButton {
			get { return GetShowButton(typeof(ClearButton)); }
			set {
				SetShowButton(typeof(ClearButton), value);
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlShowMRUButton"),
#endif
 DefaultValue(false), SmartTagProperty("Show MRU Button", "", 2)]
		public bool ShowMRUButton {
			get { return GetShowButton(typeof(MRUButton)); }
			set {
				SetShowButton(typeof(MRUButton), value);
				OnPropertiesChanged();
			}
		}
		protected bool GetShowButton(Type type) {
			IDefaultEditorButton button = Buttons.GetDefaultButton(type);
			if(button != null)
				return button.Visible;
			return false;
		}
		protected void SetShowButton(Type type, bool newValue) {
			IDefaultEditorButton button = Buttons.GetDefaultButton(type);
			if(button != null)
				button.Visible = newValue;
		}
		protected override EditorButtonCollection CreateButtonCollection() {
			return new SearchControlButtonCollection();
		}
		[Localizable(true), DXCategory(CategoryName.Behavior), RefreshProperties(RefreshProperties.All),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlButtons"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SearchControlButtonCollection Buttons { get { return base.Buttons as SearchControlButtonCollection; } }
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlClient"),
#endif
 DXCategory(CategoryName.Behavior), SmartTagProperty("Client", "")]
		public ISearchControlClient Client {
			get { return clientCore; }
			set {
				if(clientCore == value) return;
				UnregisterClient();
				this.clientCore = value;
				RegisterClient(value);
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemSearchControlQueryIsSearchColumn"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryIsSearchColumnEventHandler QueryIsSearchColumn {
			add { Events.AddHandler(queryIsSearchColumn, value); }
			remove { Events.RemoveHandler(queryIsSearchColumn, value); }
		}
		protected internal ISearchControlCriteriaProvider Provider {
			get { return providerCore; }
		}
		protected internal virtual bool IsProviderRegistered {
			get { return (Provider != null) && (Client != null); }
		}
		protected virtual void UnregisterClient() {
			if(Provider != null) {
				Provider.QueryCriteriaParams -= OnQueryCriteriaParams;
				(Provider as IDisposable).Dispose();
				this.providerCore = null;
			}
			if(Client != null) {
				Client.SetSearchControl(null);
				this.clientCore = null;
			}
		}
		protected virtual void RegisterClient(ISearchControlClient client) {
			if(client == null) return;
			this.providerCore = client.CreateSearchProvider();
			Provider.QueryCriteriaParams += OnQueryCriteriaParams;
			Client.SetSearchControl(OwnerEdit);
		}
		protected virtual void OnQueryCriteriaParams(SearchControlQueryParamsEventArgs args) {
			args.SearchText = (OwnerEdit.EditValue == null) ? null : OwnerEdit.EditValue.ToString();
			args.FilterCondition = FilterCondition;
			ISearchControlColumnsProvider columnsProvider = Provider as ISearchControlColumnsProvider;
			if(columnsProvider != null) {
				SearchControlQueryColumnsEventArgs columnsArgs = args as SearchControlQueryColumnsEventArgs;
				List<object> searchColumns = new List<object>();
				foreach(object column in columnsProvider.Columns) {
					if(!RaiseQueryIsSearchColumn(column)) continue;
					searchColumns.Add(column);
				}
				columnsArgs.SearchColumns = searchColumns;
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemSearchControl source = item as RepositoryItemSearchControl;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.Client = source.Client;
				this.showDefaultButtonsMode = source.ShowDefaultButtonsMode;
				this.ShowClearButton = source.ShowClearButton;
				this.ShowMRUButton = source.ShowMRUButton;
				this.ShowSearchButton = source.ShowSearchButton;
				this.allowAutoApply = source.AllowAutoApply;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(queryIsSearchColumn, source.Events[queryIsSearchColumn]);
		}
		protected virtual bool RaiseQueryIsSearchColumn(object column) {
			QueryIsSearchColumnEventArgs args = new QueryIsSearchColumnEventArgs();
			QueryIsSearchColumnEventHandler handler = (QueryIsSearchColumnEventHandler)Events[queryIsSearchColumn];
			if(handler != null)
				handler(column, args);
			return args.IsSearchColumn;
		}
		public override void EndInit() {
			base.EndInit();
			Buttons.EndInit();
		}
		public override void BeginInit() {
			Buttons.BeginInit();
			base.BeginInit();
		}
		DefaultEditorButton[] GetDefaultButtons() {
			return new DefaultEditorButton[] { 
				new ClearButton(),
				new SearchButton(),								 
				new MRUButton()
			};
		}
		public override void CreateDefaultButton() {
			Buttons.AddRange(GetDefaultButtons());
		}
		protected internal virtual bool IsNullOrEmpty(object value) {
			return IsEmptyValue(value) || IsNullValue(value);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (OwnerEdit == null || OwnerEdit.InplaceType == InplaceType.Standalone)) {
				UnregisterClient();
			}
			base.Dispose(disposing);
		}
		#region OldProperties
		[Browsable(false)]
		public new SearchControl OwnerEdit {
			get { return base.OwnerEdit as SearchControl; }
		}
		public override string EditorTypeName { get { return "SearchControl"; } }
		[DefaultValue("Enter text to search..."), Localizable(true)]
		public override string NullValuePrompt {
			get {
				if(string.IsNullOrEmpty(base.NullValuePrompt))
					return GetDefaultNullValuePrompt();
				return base.NullValuePrompt;
			}
			set { base.NullValuePrompt = value; }
		}
		protected virtual string GetDefaultNullValuePrompt() {
			return "Enter text to search...";
		}
		[DefaultValue(true)]
		public override bool NullValuePromptShowForEmptyValue {
			get { return base.NullValuePromptShowForEmptyValue; }
			set { base.NullValuePromptShowForEmptyValue = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int ActionButtonIndex {
			get { return base.ActionButtonIndex; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ExportMode ExportMode {
			get { return base.ExportMode; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new DevExpress.Utils.ImageCollection HtmlImages {
			get { return base.HtmlImages; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override char PasswordChar {
			get { return base.PasswordChar; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool UseSystemPasswordChar {
			get { return base.UseSystemPasswordChar; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string NullText {
			get { return base.NullText; }
			set { }
		}
		#endregion OldProperties
	}
	public interface IDefaultEditorButton {
		event BaseOptionChangedEventHandler VisibleChanged;
		bool Visible { get; set; }
		EditorButton Button { get; }
		int Index { get; }
	}
	[TypeConverter(typeof(EditorButtonTypeConverter))]
	public abstract class DefaultEditorButton : EditorButton, IDefaultEditorButton {
		bool visibleCore;
		bool allowGlyph;
		BaseOptionChangedEventHandler visibleChangedEvent;
		[EditorButtonPreferredConstructor]
		public DefaultEditorButton() : base() { }
		[EditorButtonPreferredConstructor]
		public DefaultEditorButton(int width, ImageLocation imageLocation, Image image, bool allowGlyph, bool enableImageTransparency) : this(String.Empty, width, true, true, imageLocation, image, allowGlyph, KeyShortcut.Empty, null, String.Empty, null, null, enableImageTransparency) { }
		[EditorButtonPreferredConstructor]
		public DefaultEditorButton(string caption, string toolTip, object tag, SuperToolTip superTip, AppearanceObject appearance)
			: this(caption, -1, true, true, ImageLocation.Default, null, false, KeyShortcut.Empty, appearance, String.Empty, null, null, false) { }
		[EditorButtonPreferredConstructor]
		public DefaultEditorButton(string caption, int width, bool enabled, bool visible, ImageLocation imageLocation, Image image, bool allowGlyph, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency)
			: this() {
			this.allowGlyph = allowGlyph;
			Caption = caption;
			ImageLocation = imageLocation;
			Image = image;
			Enabled = enabled;
			Visible = visible;
			Shortcut = shortcut;
			ToolTip = toolTip;
			Tag = tag;
			SuperTip = superTip;
			EnableImageTransparency = enableImageTransparency;
			Width = width;
			InitAppearance(appearance);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsDefaultButton {
			get { return true; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsLeft {
			get { return false; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ButtonPredefines Kind {
			get {
				if(AllowGlyph) return ButtonPredefines.Glyph;
				return GetKind();
			}
			set { }
		}
		[ DefaultValue(false)]
		public bool AllowGlyph {
			get { return allowGlyph; }
			set {
				if(AllowGlyph == value) return;
				allowGlyph = value;
				OnChanged();
			}
		}
		bool IDefaultEditorButton.Visible {
			get { return visibleCore; }
			set {
				if(visibleCore == value) return;
				visibleCore = value;
				OnVisibleChanged(value);
			}
		}
		event BaseOptionChangedEventHandler IDefaultEditorButton.VisibleChanged {
			add { visibleChangedEvent += value; }
			remove { visibleChangedEvent -= value; }
		}
		protected virtual void OnVisibleChanged(bool value) {
			if(visibleChangedEvent != null)
				visibleChangedEvent(this, new BaseOptionChangedEventArgs("Visible", !value, value));
		}
		protected override void OnCollectionChanged() {
			if(Collection == null) visibleCore = false;
			else visibleCore = true;
		}
		protected abstract ButtonPredefines GetKind();
		EditorButton IDefaultEditorButton.Button { get { return this; } }
		protected override int GetDefaultIndex() { return -1; }
	}
	public class SearchButton : DefaultEditorButton {
		[EditorButtonPreferredConstructor]
		public SearchButton() : base() { }
		[EditorButtonPreferredConstructor]
		public SearchButton(int width, ImageLocation imageLocation, Image image, bool allowGlyph, bool enableImageTransparency) : base(width, imageLocation, image, allowGlyph, enableImageTransparency) { }
		[EditorButtonPreferredConstructor]
		public SearchButton(string caption, string toolTip, object tag, SuperToolTip superTip, AppearanceObject appearance)
			: base(caption, toolTip, tag, superTip, appearance) { }
		[EditorButtonPreferredConstructor]
		public SearchButton(string caption, int width, bool enabled, bool visible, ImageLocation imageLocation, Image image, bool allowGlyph, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency) : base(caption, width, true, true, imageLocation, image, allowGlyph, shortcut, appearance, toolTip, tag, superTip, enableImageTransparency) { }
		protected override ButtonPredefines GetKind() { return ButtonPredefines.Search; }
	}
	public class MRUButton : DefaultEditorButton {
		[EditorButtonPreferredConstructor]
		public MRUButton() : base() { }
		[EditorButtonPreferredConstructor]
		public MRUButton(int width, ImageLocation imageLocation, Image image, bool allowGlyph, bool enableImageTransparency) : base(width, imageLocation, image, allowGlyph, enableImageTransparency) { }
		[EditorButtonPreferredConstructor]
		public MRUButton(string caption, string toolTip, object tag, SuperToolTip superTip, AppearanceObject appearance)
			: base(caption, toolTip, tag, superTip, appearance) { }
		[EditorButtonPreferredConstructor]
		public MRUButton(string caption, int width, bool enabled, bool visible, ImageLocation imageLocation, Image image, bool allowGlyph, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency) : base(caption, width, true, true, imageLocation, image, allowGlyph, shortcut, appearance, toolTip, tag, superTip, enableImageTransparency) { }
		protected override ButtonPredefines GetKind() { return ButtonPredefines.Combo; }
	}
	public class ClearButton : DefaultEditorButton {
		[EditorButtonPreferredConstructor]
		public ClearButton() : base() { }
		[EditorButtonPreferredConstructor]
		public ClearButton(int width, ImageLocation imageLocation, Image image, bool allowGlyph, bool enableImageTransparency) : base(width, imageLocation, image, allowGlyph, enableImageTransparency) { }
		[EditorButtonPreferredConstructor]
		public ClearButton(string caption, string toolTip, object tag, SuperToolTip superTip, AppearanceObject appearance)
			: base(caption, toolTip, tag, superTip, appearance) { }
		[EditorButtonPreferredConstructor]
		public ClearButton(string caption, int width, bool enabled, bool visible, ImageLocation imageLocation, Image image, bool allowGlyph, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency) : base(caption, width, true, true, imageLocation, image, allowGlyph, shortcut, appearance, toolTip, tag, superTip, enableImageTransparency) { }
		protected override ButtonPredefines GetKind() { return ButtonPredefines.Clear; }
	}
	public class SearchControlDefaultButtonCollection : IEnumerable<IDefaultEditorButton> {
		Dictionary<Type, IDefaultEditorButton> defaultButtonCollection;
		ISearchEditorButtonCollection parentCollection;
		public SearchControlDefaultButtonCollection(ISearchEditorButtonCollection parentCollection)
			: base() {
			defaultButtonCollection = new Dictionary<Type, IDefaultEditorButton>();
			this.parentCollection = parentCollection;
		}
		public void Add(IDefaultEditorButton button) {
			if(defaultButtonCollection.ContainsKey(button.GetType())) return;
			defaultButtonCollection.Add(button.GetType(), button);
			button.VisibleChanged += ButtonVisibleChanged;
		}
		protected virtual void ButtonVisibleChanged(object sender, BaseOptionChangedEventArgs e) {
			IDefaultEditorButton btn = sender as IDefaultEditorButton;
			bool show = (bool)e.NewValue;
			if(show) btn.Button.SetIndex(IndexOf(btn));
			CollectionChangeEventArgs args = new CollectionChangeEventArgs(show ? CollectionChangeAction.Add : CollectionChangeAction.Remove, sender);
			OnDefaultButtonVisibleChanged(args);
		}
		public void Remove(IDefaultEditorButton button) {
			if(!defaultButtonCollection.ContainsKey(button.GetType())) return;
			button.VisibleChanged -= ButtonVisibleChanged;
			defaultButtonCollection.Remove(button.GetType());
		}
		public IDefaultEditorButton this[Type type] {
			get {
				IDefaultEditorButton defBtn;
				defaultButtonCollection.TryGetValue(type, out defBtn);
				return defBtn;
			}
			set {
				if(defaultButtonCollection == null || !(value is IDefaultEditorButton)) return;
				defaultButtonCollection[type].VisibleChanged -= ButtonVisibleChanged;
				value.VisibleChanged += ButtonVisibleChanged;
				defaultButtonCollection[type] = value;
			}
		}
		public int IndexOf(IDefaultEditorButton button) {
			int index = -1;
			foreach(var defaultButton in this) {
				if(defaultButton.Visible) index++;
				if(button == defaultButton) return index;
			}
			return -1;
		}
		void OnDefaultButtonVisibleChanged(CollectionChangeEventArgs args) {
			parentCollection.OnDefaultButtonsCollectionChanged(args);
		}
		public IEnumerator<IDefaultEditorButton> GetEnumerator() {
			foreach(var defaultButton in defaultButtonCollection)
				yield return defaultButton.Value;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	public interface ISearchEditorButtonCollection {
		void BeginInit();
		void EndInit();
		void Remove(EditorButton button);
		void OnDefaultButtonsCollectionChanged(CollectionChangeEventArgs args);
	}
	public class SearchControlButtonCollection : EditorButtonCollection, ISearchEditorButtonCollection {
		SearchControlDefaultButtonCollection defaultButtons;
		int _initialize = 0;
		public SearchControlButtonCollection()
			: base() {
			defaultButtons = new SearchControlDefaultButtonCollection(this);
		}
		public override int Add(EditorButton button) {
			if(!AddDefaultButton(button))
				return 0;
			return base.Add(button);
		}
		bool AddDefaultButton(EditorButton btn) {
			IDefaultEditorButton defButton = btn as IDefaultEditorButton;
			if(defButton == null) return true;
			if(Initialize) {
				if(defaultButtons[defButton.GetType()] != null)
					defaultButtons[defButton.GetType()] = defButton;
				return true;
			}
			defaultButtons.Add(defButton);
			if(defButton.Visible)
				base.Insert(defButton.Index, defButton.Button);
			return false;
		}
		public void Remove(EditorButton button) {
			if(!Contains(button)) return;
			List.Remove(button);
		}
		public void BeginInit() { _initialize++; }
		public void EndInit() { _initialize--; }
		protected bool Initialize { get { return _initialize > 0; } }
		void ISearchEditorButtonCollection.OnDefaultButtonsCollectionChanged(CollectionChangeEventArgs args) {
			if(Initialize) return;
			IDefaultEditorButton btn = args.Element as IDefaultEditorButton;
			if(btn == null) return;
			if(args.Action == CollectionChangeAction.Add)
				Add(btn.Button);
			if(args.Action == CollectionChangeAction.Remove)
				Remove(btn.Button);
		}
		protected internal IDefaultEditorButton GetDefaultButton(Type key) {
			if(defaultButtons == null) return null;
			return defaultButtons[key];
		}
		public override void Assign(EditorButtonCollection collection) {
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < collection.Count; n++) {
					EditorButton button = collection[n];
					if(button is DefaultEditorButton) continue;
					EditorButton newButton = new EditorButton();
					newButton.Assign(button);
					Add(newButton);
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class SearchControlViewInfo : MRUEditViewInfo {
		public SearchControlViewInfo(RepositoryItem item)
			: base(item) {
		}
		public new RepositoryItemSearchControl Item { get { return base.Item as RepositoryItemSearchControl; } }
		protected override bool CanDisplayButton(EditorButton btn) {
			switch(Item.ShowDefaultButtonsMode) {
				case ShowDefaultButtonsMode.Always: return base.CanDisplayButton(btn);
				case ShowDefaultButtonsMode.Default:
				case ShowDefaultButtonsMode.AutoChangeSearchToClear:
					if(btn is SearchButton) return Item.IsNullOrEmpty(EditValue);
					break;
			}
			if(btn is ClearButton)
				return !Item.IsNullOrEmpty(EditValue);
			return base.CanDisplayButton(btn);
		}
	}
}
