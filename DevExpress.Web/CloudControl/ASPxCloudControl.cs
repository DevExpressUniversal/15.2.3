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
using System.Data; 
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Design;
	public enum Scales { Linear, Logarithmic };
	[DXWebToolboxItem(true), DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxCloudControl"),
	DefaultProperty("Items"), Designer("DevExpress.Web.Design.ASPxCloudControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCloudControl.bmp")
	]
	public class ASPxCloudControl : ASPxDataWebControl, IControlDesigner {
		protected internal const string CloudControlScriptResourceName = WebScriptsResourcePath + "CloudControl.js";
		private readonly double[] FontCoeff = new double[] { 1, 1.1, 1.3, 1.6, 2.2, 2.4, 2.9 };
		private bool fDataBound = false; 
		private CloudControlItemCollection fItems = null;
		private List<CloudControlItem> fItemsInternal = null;
		private double fFactor = 0;
		private RankPropertiesCollection fRankProperties = null;
		private static readonly object EventItemClick = new object();
		private static readonly object EventItemDataBound = new object();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[
		Category("Appearance"), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color ItemBeginEndTextColor {
			get { return GetColorProperty("ItemBeginEndTextColor", Color.Empty); }
			set { SetColorProperty("ItemBeginEndTextColor", Color.Empty, value); }
		}
		[
		Category("Appearance"), DefaultValue(typeof(Color), "#1E3695"), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color MaxColor {
			get { return ForeColor; }
			set { ForeColor = value; }
		}
		[
		Category("Appearance"), DefaultValue(typeof(Color), "#747a93"), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color MinColor {
			get { return GetColorProperty("MinColor", Color.FromArgb(0x74, 0x7a, 0x93)); }
			set { SetColorProperty("MinColor", Color.FromArgb(0x74, 0x7a, 0x93), value); }
		}
		[
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public bool ShowValues {
			get { return GetBoolProperty("ShowValues", false); }
			set { SetBoolProperty("ShowValues", false, value); }
		}
		[
		Category("Appearance"), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color ValueColor {
			get { return GetColorProperty("ValueColor", Color.Empty); }
			set { SetColorProperty("ValueColor", Color.Empty, value); }
		}
		[
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ItemClickClientSideEvents ClientSideEvents {
			get { return (ItemClickClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebScale"),
#endif
		Category("Behavior"), DefaultValue(Scales.Logarithmic), AutoFormatDisable]
		public Scales Scale {
			get { return (Scales)GetEnumProperty("Scale", Scales.Logarithmic); }
			set { SetEnumProperty("Scale", Scales.Logarithmic, value); }
		}
		[
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool Sorted {
			get { return GetBoolProperty("Sorted", true); }
			set { SetBoolProperty("Sorted", true, value); }
		}
		[
		Category("Data"), DefaultValue(""), TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)),
		AutoFormatDisable, Localizable(false)]
		public string NameField {
			get { return GetStringProperty("NameField", ""); }
			set {
				SetStringProperty("NameField", "", value);
				OnDataFieldChanged();
			}
		}
		[
		Category("Data"), DefaultValue(""), AutoFormatDisable, Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", ""); }
			set {
				SetStringProperty("NavigateUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
		Category("Data"), DefaultValue("{0}"), AutoFormatDisable, Localizable(false)]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
		Category("Data"), DefaultValue("({0:N0})"), Localizable(true), AutoFormatEnable]
		public string ValueFormatString {
			get { return GetStringProperty("ValueFormatString", "({0:N0})"); }
			set { SetStringProperty("ValueFormatString", "({0:N0})", value); }
		}
		[
		Category("Data"), DefaultValue(""), AutoFormatDisable, Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set {
				SetStringProperty("TextField", "", value);
				OnDataFieldChanged();
			}
		}
		[
		Category("Data"), DefaultValue(""), AutoFormatDisable, Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ValueField {
			get { return GetStringProperty("ValueField", ""); }
			set {
				SetStringProperty("ValueField", "", value);
				OnDataFieldChanged();
			}
		}
		[
		Category("Layout"), DefaultValue(typeof(HorizontalAlign), "Justify"), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return (HorizontalAlign)GetObjectProperty("HorizontalAlign", HorizontalAlign.Justify); }
			set { SetObjectProperty("HorizontalAlign", HorizontalAlign.Justify, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return (ControlStyle as AppearanceStyle).Paddings; }
		}
		[
		Category("Layout"), DefaultValue(typeof(FontUnit), ""), AutoFormatEnable,
		TypeConverter(typeof(FontUnitConverter))]
		public FontUnit SpacerFontSize {
			get { return (FontUnit)GetObjectProperty("SpacerFontSize", FontUnit.Empty); }
			set { SetObjectProperty("SpacerFontSize", FontUnit.Empty, value); }
		}
		[
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ValueSpacing {
			get { return GetUnitProperty("ValueSpacing", Unit.Empty); }
			set { SetUnitProperty("ValueSpacing", Unit.Empty, value); }
		}
		[
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
		DefaultValue(""), Localizable(true), AutoFormatEnable]
		public string ItemBeginText {
			get { return GetStringProperty("ItemBeginText", ""); }
			set { SetStringProperty("ItemBeginText", "", value); }
		}
		[
		DefaultValue(""), Localizable(true), AutoFormatEnable]
		public string ItemEndText {
			get { return GetStringProperty("ItemEndText", ""); }
			set { SetStringProperty("ItemEndText", "", value); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public CloudControlItemCollection Items {
			get {
				if(fItems == null)
					fItems = new CloudControlItemCollection(this);
				return fItems;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebRankProperties"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatEnable, AutoFormatDisableClear,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public RankPropertiesCollection RankProperties {
			get {
				if(fRankProperties == null)
					fRankProperties = new RankPropertiesCollection(this);
				return fRankProperties;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), DefaultValue(typeof(byte), "7"), AutoFormatEnable,
		RefreshProperties(RefreshProperties.Repaint)]
		public byte RankCount {
			get {
				if(RankProperties.Count < 1)
					return 1;
				else
					return (byte)RankProperties.Count;
			}
			set {
				CommonUtils.CheckMinimumValue(value, 1, "RankCount");
				SetRankCount(value);
				ResetFactor();
			}
		}
		[
		DefaultValue(""), TypeConverter(typeof(TargetConverter)), Localizable(false), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebLinkStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		protected double Factor {
			get {
				if (fFactor == 0)
					fFactor = CreateFactor();
				return fFactor;
			}
		}
		protected internal List<CloudControlItem> ItemsInternal {
			get {
				if (fItemsInternal == null)
					fItemsInternal = CreateItemsInternal();
				return fItemsInternal;
			}
		}
		[
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
		Category("Data")]
		public event CloudControlItemEventHandler ItemDataBound
		{
			add { Events.AddHandler(EventItemDataBound, value); }
			remove { Events.RemoveHandler(EventItemDataBound, value); }
		}
		[
		Category("Action")]
		public event CloudControlItemEventHandler ItemClick
		{
			add { Events.AddHandler(EventItemClick, value); }
			remove { Events.RemoveHandler(EventItemClick, value); }
		}
		protected CloudControlStyles Styles {
			get { return StylesInternal as CloudControlStyles; }
		}
		public ASPxCloudControl()
			: base() {
			RankCount = 7;
			ForeColor = Color.FromArgb(0x1e, 0x36, 0x95);
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if (!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				Items.Clear();
			else if (!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindItems(data);
				ResetControlHierarchy();
			}
		}
		protected override void OnDataBinding(EventArgs e) {
			base.OnDataBinding(e);
			EnsureChildControls();
		}
		protected internal void DataBindItems(IEnumerable data) {
			string textFieldName = String.IsNullOrEmpty(TextField) ? "Text" : TextField;
			string valueFieldName = String.IsNullOrEmpty(ValueField) ? "Value" : ValueField;
			string urlFieldName = String.IsNullOrEmpty(NavigateUrlField) ? "NavigateUrl" : NavigateUrlField;
			Items.Clear();
			foreach (object obj in data) {
				CloudControlItem item = new CloudControlItem();
				DataBindItemProperties(item, obj, textFieldName, valueFieldName, urlFieldName, NameField);
				item.SetDataItem(obj);
				Items.Add(item);
				OnItemDataBound(new CloudControlItemEventArgs(item));
			}
		}
		protected internal void DataBindItemProperties(CloudControlItem item, object obj,
			string textFieldName, string valueFieldName, string urlFieldName, string nameField) {
			double value;
			item.Text = GetFieldValue(obj, textFieldName, TextField != "", "").ToString();
			item.NavigateUrl = GetFieldValue(obj, urlFieldName, NavigateUrlField != "", "").ToString();
			if(double.TryParse(GetFieldValue(obj, valueFieldName, ValueField != "", "").ToString(), out value))
				item.Value = value;
			item.Name = GetFieldValue(obj, nameField, NameField != "", "").ToString();
		}
		protected internal void PrepareRanks() {
			if (Items.Count < 1)
				return;
			double min = Items[0].Value;
			double max = Items[0].Value;
			double delta, norm;
			foreach (CloudControlItem item in Items) {
				if (item.Value < min)
					min = item.Value;
				else if (item.Value > max)
					max = item.Value;
			}
			if (min == max)
				return;
			delta = max - min;
			if (RankCount > 1)
				foreach (CloudControlItem item in Items) {
					norm = (RankCount - 1) * (item.Value - min) / delta;
					if (Scale == Scales.Linear)
						item.Rank = (int)Math.Ceiling(norm);
					else if (Scale == Scales.Logarithmic)
						item.Rank = (int)Math.Ceiling((RankCount - 1) * Math.Log(1 + norm, RankCount));
					if (item.Rank > RankCount - 1) 
						item.Rank = RankCount - 1;
				}
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override void CreateControlHierarchy() {
			CCControl control = new CCControl(this);
			Controls.Add(control);
		}
		protected override bool HasContent() {
			return Items.Count > 0;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ItemClickClientSideEvents();
		}
		protected internal List<CloudControlItem> CreateItemsInternal() {
			List<CloudControlItem> list = new List<CloudControlItem>();
			PrepareRanks();
			foreach (CloudControlItem item in Items)
				list.Add(item);
			if (Sorted)
				list.Sort(CompareItems);
			return list;
		}
		int CompareItems(CloudControlItem x, CloudControlItem y) {
			int result = Comparer.Default.Compare(x.Text, y.Text);
			if(result == 0)
				result = Comparer.Default.Compare(x.Index, y.Index);
			return result;
		}
		protected internal double CreateFactor() {
			return 1 + 2 * Math.Pow(10, -0.4 * RankCount);
		}
		protected internal string GetFormattedValue(CloudControlItem item) {
			return String.Format(ValueFormatString, item.Value);
		}
		protected internal string GetNavigateUrl(CloudControlItem item) {
			string url = string.Format(NavigateUrlFormatString, item.NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasItemServerClickEventHandler();
		}
		protected bool HasItemServerClickEventHandler() {
			return Events[EventItemClick] != null;
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || HasItemServerClickEventHandler();
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasItemServerClickEventHandler())
				eventNames.Add("ItemClick");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientCloudControl";
		}
		protected internal bool HasItemOnClick() {
			return ClientSideEvents.ItemClick != "" || HasItemServerClickEventHandler();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCloudControl), CloudControlScriptResourceName);
		}
		protected internal string GetControlOnClick() {
			return string.Format(ControlClickHandlerName, ClientID);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new CloudControlStyles(this);
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			base.PrepareControlStyle(style);
			style.Font.Size = GetSpacerFontSize();
		}
		static object rankStyle = new object();
		protected internal AppearanceStyle GetRankStyle(int rank) {
			return (AppearanceStyle)CreateStyle(delegate() {
				AppearanceStyle style = new AppearanceStyle();
				MergeControlStyle(style, false);
				if(IsValidRankPropertyIndex(rank)) {
					style.CopyFontFrom(RankProperties[rank].Style);
					style.CssClass = RankProperties[rank].CssClass;
				}
				style.Font.Size = GetRankFontSize(rank);
				if(LinkStyle.Color.IsEmpty)
					style.ForeColor = GetRankColor(rank, GetMaxColor(), GetMinColor());
				style.Wrap = DefaultBoolean.False;
				if(!Enabled) {
					style.ForeColor = Color.Empty;
					MergeDisableStyle(style);
				}
				return style;
			}, rank, rankStyle);
		}
		protected internal AppearanceStyleBase GetRankLinkStyle(int rank, bool hasLinkWrapper) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			AppearanceStyle rankStyle = GetRankStyle(rank);
			if(!hasLinkWrapper)
				style.CopyFrom(rankStyle);
			else {
				if(!rankStyle.ForeColor.IsEmpty)
					style.ForeColor = rankStyle.ForeColor;
				if(DevExpress.Web.Internal.FontHelper.IsUnderlineStored(rankStyle.Font))
					style.Font.Underline = rankStyle.Font.Underline;
			}
			style.CopyFrom(LinkStyle.Style);
			return style;
		}
		protected internal AppearanceStyleBase GetRankValueStyle(int rank) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.ForeColor = Enabled ? GetValueColor(rank) : Color.Empty;
			return style;
		}
		protected internal AppearanceStyleBase GetItemBeginEndTextStyle(int rank) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.ForeColor = Enabled ? GetItemBeginEndTextColor(rank) : Color.Empty;
			return style;
		}
		protected internal Color GetMaxColor() {
			return MaxColor.IsEmpty ? Styles.GetMaxColor() : MaxColor;
		}
		protected internal Color GetMinColor() {
			return MinColor.IsEmpty ? Styles.GetMinColor() : MinColor;
		}
		protected internal Color GetRankColor(int rank, Color maxColor, Color minColor) {
			if (IsValidRankPropertyIndex(rank) && !RankProperties[rank].ForeColor.IsEmpty)
				return RankProperties[rank].ForeColor;
			else {
				if (minColor.IsEmpty && maxColor.IsEmpty)
					return Color.Empty;
				if (RankCount > 1) {
					double coef = (double)rank / (RankCount - 1);
					double red = Math.Round(coef * maxColor.R + (1 - coef) * minColor.R);
					double green = Math.Round(coef * maxColor.G + (1 - coef) * minColor.G);
					double blue = Math.Round(coef * maxColor.B + (1 - coef) * minColor.B);
					return Color.FromArgb((int)red, (int)green, (int)blue);
				}
				return maxColor;
			}
		}
		protected internal FontUnit GetRankFontSize(int rank) {
			return GetRankFontSize(rank, false);
		}
		protected internal FontUnit GetRankFontSize(int rank, bool isSpacer) {
			if (IsValidRankPropertyIndex(rank) && !RankProperties[rank].Font.Size.IsEmpty)
				return RankProperties[rank].Font.Size;
			else {
				double fontSize;
				if (ControlStyle.Font.Size.IsEmpty)
					fontSize = Styles.GetMinRankFontSize();
				else
					fontSize = Styles.GetFontSizeInPixels(ControlStyle.Font.Size);
				if (rank < FontCoeff.Length)
					fontSize *= FontCoeff[rank];
				else
					fontSize *= FontCoeff[FontCoeff.Length - 1] + 0.25 * (rank - FontCoeff.Length + 1);
				if (!isSpacer)
					fontSize *= Math.Pow(Factor, rank);
				return new FontUnit(fontSize, UnitType.Pixel);
			}
		}
		protected internal FontUnit GetSpacerFontSize() {
			FontUnit result = SpacerFontSize;
			if (result.IsEmpty)
				result = GetRankFontSize((int)Math.Log(10 + 10 * RankCount), true);			
			return result;
		}
		protected internal Color GetValueColor(int rank) {
			Color ret = Color.Empty;
			if (IsValidRankPropertyIndex(rank)) {
				ret = RankProperties[rank].ValueColor;
				if (ret.IsEmpty)
					ret = RankProperties[rank].ForeColor;
			}
			return !ret.IsEmpty ? ret : ValueColor;
		}
		protected internal Color GetItemBeginEndTextColor(int rank) {
			Color ret = Color.Empty;
			if (IsValidRankPropertyIndex(rank)) {
				ret = RankProperties[rank].ItemBeginEndTextColor;
				if (ret.IsEmpty)
					ret = RankProperties[rank].ForeColor;
			}
			return !ret.IsEmpty ? ret : ItemBeginEndTextColor;
		}
		protected internal void ResetFactor() {
			fFactor = 0;
		}
		protected internal string GetItemElementID(CloudControlItem item) {
			if(item.Name != "")
				return item.Name;
			else if(HasItemServerClickEventHandler())
				return item.Index.ToString();
			return "";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items, RankProperties });
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if (!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
		}
		private void OnItemClick(CloudControlItemEventArgs e) {
			CloudControlItemEventHandler handler = (CloudControlItemEventHandler)Events[EventItemClick];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnItemDataBound(CloudControlItemEventArgs e) {
			CloudControlItemEventHandler handler = Events[EventItemDataBound] as CloudControlItemEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			EnsureDataBound();
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch(arguments[0]) {
				case "CLICK":
					string nameOrIndex = arguments[1];
					CloudControlItem item = Items.FindByNameOrIndex(nameOrIndex);
					CloudControlItemEventArgs args = item != null ? args = new CloudControlItemEventArgs(item) : new CloudControlItemEventArgs(null);
					OnItemClick(args);
					break;
			}
		}
		protected internal void ItemsChanged() {
			if (!IsLoading()) {
				fItemsInternal = null;
				ResetControlHierarchy();
			}
		}
		private bool ShouldSerializeRankCount() {
			return false;
		}
		protected void SetRankCount(byte count) {
			while (count < RankProperties.Count)
				RankProperties.RemoveAt(RankProperties.Count - 1);
			while (count > RankProperties.Count)
				RankProperties.Add();
			PropertyChanged("RankProperties");
			PropertyChanged("RankCount");
			if (IsLoading())
				RankProperties.IsRankCountLoaded = true;
		}
		protected bool IsValidRankPropertyIndex(int index) {
			return (0 <= index) && (index < RankProperties.Count) ? true : false;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.CloudControlCommonFormDesigner"; } }
	}
}
