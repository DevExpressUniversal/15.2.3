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
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Utils;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public enum TitleIndexItemBulletStyle { NotSet, None, Auto, Disc, Circle, Square };
	public enum IndexPanelBehavior { Navigation, Filtering };
	public enum FilterBoxPosition { Left, Right, Center };
	public enum FilterBoxVerticalPosition { AboveIndexPanel, BelowIndexPanel };
	public class FilterBox : PropertiesBase, IPropertiesOwner {
		private const int DefaultDelayConst = 0;
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxAutoFocus"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool AutoFocus {
			get { return GetBoolProperty("AutoFocus", false); }
			set {
				SetBoolProperty("AutoFocus", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxCaption"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.TitleIndex_FilterCaption),
		Localizable(true), AutoFormatEnable]
		public string Caption {
			get { return GetStringProperty("Caption", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_FilterCaption)); }
			set {
				SetStringProperty("Caption", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_FilterCaption), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxDelay"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultDelayConst), AutoFormatDisable]
		public int Delay {
			get { return GetIntProperty("Delay", DefaultDelayConst); }
			set {
				CommonUtils.CheckNegativeValue(value, "Delay");
				SetIntProperty("Delay", DefaultDelayConst, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxInfoText"),
#endif
		DefaultValue(StringResources.TitleIndex_FilterHint), NotifyParentProperty(true),
		Localizable(true), AutoFormatEnable,
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
		public string InfoText {
			get { return GetStringProperty("InfoText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_FilterHint)); }
			set {
				SetStringProperty("InfoText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_FilterHint), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxHorizontalPosition"),
#endif
		NotifyParentProperty(true), DefaultValue(FilterBoxPosition.Right), AutoFormatEnable]
		public FilterBoxPosition HorizontalPosition {
			get { return (FilterBoxPosition)GetEnumProperty("HorizontalPosition", FilterBoxPosition.Right); }
			set {
				SetEnumProperty("HorizontalPosition", FilterBoxPosition.Right, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxVerticalPosition"),
#endif
		NotifyParentProperty(true), DefaultValue(FilterBoxVerticalPosition.BelowIndexPanel), AutoFormatEnable]
		public FilterBoxVerticalPosition VerticalPosition {
			get { return (FilterBoxVerticalPosition)GetEnumProperty("VerticalPosition", FilterBoxVerticalPosition.BelowIndexPanel); }
			set {
				SetEnumProperty("VerticalPosition", FilterBoxVerticalPosition.BelowIndexPanel, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				Changed();
			}
		}
		public FilterBox()
			: base() {
		}
		public FilterBox(ASPxTitleIndex titleIndexControl)
			: base(titleIndexControl) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if (source is FilterBox) {
					FilterBox src = source as FilterBox;
					Caption = src.Caption;
					Delay = src.Delay;
					InfoText = src.InfoText;
					HorizontalPosition = src.HorizontalPosition;
					VerticalPosition = src.VerticalPosition;
					Visible = src.Visible;
				}
			}
			finally {
				EndUpdate();
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class IndexPanel : PropertiesBase, IPropertiesOwner {
		private const string DefaultSeparatorText = "&nbsp;";
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelBehavior"),
#endif
		NotifyParentProperty(true), DefaultValue(IndexPanelBehavior.Navigation), AutoFormatDisable]
		public IndexPanelBehavior Behavior {
			get { return (IndexPanelBehavior)GetEnumProperty("Behavior", IndexPanelBehavior.Navigation); }
			set {
				SetEnumProperty("Behavior", IndexPanelBehavior.Navigation, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelCharacters"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
		public string Characters {
			get { return GetStringProperty("Characters", ""); }
			set { SetStringProperty("Characters", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelSeparator"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultSeparatorText), Localizable(false), AutoFormatEnable]
		public string Separator {
			get { return GetStringProperty("Separator", DefaultSeparatorText); }
			set {
				SetStringProperty("Separator", DefaultSeparatorText, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelShowNonExistingItems"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatEnable]
		public bool ShowNonExistingItems {
			get { return GetBoolProperty("ShowNonExistingItems", true); }
			set {
				SetBoolProperty("ShowNonExistingItems", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelPosition"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(PagerPosition.Top), AutoFormatEnable]
		public PagerPosition Position {
			get { return (PagerPosition)GetEnumProperty("Position", PagerPosition.Top); }
			set {
				SetEnumProperty("Position", PagerPosition.Top, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				Changed();
			}
		}
		public IndexPanel()
			: base() {
		}
		public IndexPanel(ASPxTitleIndex titleIndexControl)
			: base(titleIndexControl) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if (source is IndexPanel) {
					IndexPanel src = source as IndexPanel;
					Separator = src.Separator;
					Characters = src.Characters;
					Position = src.Position;
					ShowNonExistingItems = src.ShowNonExistingItems;
					Visible = src.Visible;
				}
			}
			finally {
				EndUpdate();
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxTitleIndex"),
	DefaultEvent("ItemClick"), Designer("DevExpress.Web.Design.ASPxTitleIndexDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTitleIndex.bmp")
	]
	public class ASPxTitleIndex : ASPxDataWebControl, IRequiresLoadPostDataControl, IControlDesigner {
		private static TitleIndexItemBulletStyle[] DefaultItemBulletStyles =
			new TitleIndexItemBulletStyle[] { TitleIndexItemBulletStyle.Disc, TitleIndexItemBulletStyle.Circle, TitleIndexItemBulletStyle.Square };
		private static LanguageInfo LatinLanguageInfo = LanguageInfo.CreateAlphabetInfo("en-US");
		protected internal const string IndexStateFieldName = "index";
		protected internal const string TitleIndexScriptResourceName = WebScriptsResourcePath + "TitleIndex.js";
		private const string FilterInputChangeHandlerName = "ASPx.SIFChange(event, '{0}')";
		private const string FilterInputBlurHandlerName = "ASPx.SIFBlur('{0}')";
		private const string FilterInputFocusHandlerName = "ASPx.SIFFocus('{0}')";
		private const string FilterInputKeyPressHandlerName = "return ASPx.SIFKeyPress(event, '{0}')";
		private const string FilterInputKeyUpHandlerName = "ASPx.SIFKeyUp(event, '{0}')";
		private const string IndexPanelItemClickHandlerName = "ASPx.IPItemClick('{0}', '{1}')";
		private const string ColumnCountKey = "columnCount";
		private TitleIndexItemCollection fItems = null;
		private int fIndexPanelItemCharIndex = 0;
		private List<string[]> fCharSetInLines = null;
		private TitleIndexColumnCollection fColumns = null;
		private TitleIndexColumnCollection fDummyColumns = null;
		private IndexPanel fIndexPanel = null;
		private FilterBox fFilterBox = null;
		private TitleIndexNodeCollection fSortedNodes = null;
		private List<string> fExistingCharSet = null;
		private string[] fSpecialCharSet = null;
		private bool fDataBound = false;
		private bool fIsGrouping = false;
		private MainControl fMainControl = null;
		private ITemplate fColumnSeparatorTemplate = null;
		private ITemplate fGroupHeaderTemplate = null;
		private ITemplate fItemTemplate = null;
		private ITemplate fIndexPanelItemTemplate = null;
		private static readonly object EventItemCommand = new object();
		private static readonly object EventItemDataBound = new object();
		private static readonly object EventItemClick = new object();
		private static readonly object EventGroupHeaderCommand = new object();
		private static readonly object EventIndexPanelItemCommand = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexEnableCallBacks"),
#endif
		DefaultValue(true), Category("Behavior"), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IndexPanel IndexPanel {
			get {
				if(fIndexPanel == null)
					fIndexPanel = new IndexPanel(this);
				return fIndexPanel;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexBackToTopText"),
#endif
		DefaultValue(StringResources.TitleIndex_DefaultBackToTopText), AutoFormatDisable, Localizable(true)]
		public string BackToTopText {
			get { return GetStringProperty("BackToTopText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_BackToTop)); }
			set {
				SetStringProperty("BackToTopText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_BackToTop), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexColumnCount"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true), DefaultValue(typeof(byte), "0"),
		RefreshProperties(RefreshProperties.Repaint), AutoFormatDisable]
		public byte ColumnCount {
			get { return (byte)Columns.Count; }
			set {
				if(!IsLoading())
					SetColumnCount(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexColumns"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		RefreshProperties(RefreshProperties.Repaint),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public TitleIndexColumnCollection Columns {
			get {
				if(fColumns == null)
					fColumns = new TitleIndexColumnCollection(this);
				return fColumns;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public TitleIndexItemCollection Items {
			get {
				if(fItems == null)
					fItems = new TitleIndexItemCollection(this);
				return fItems;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItemBulletStyle"),
#endif
		DefaultValue(TitleIndexItemBulletStyle.NotSet), AutoFormatEnable]
		public TitleIndexItemBulletStyle ItemBulletStyle {
			get { return (TitleIndexItemBulletStyle)GetEnumProperty("ItemBulletStyle", TitleIndexItemBulletStyle.NotSet); }
			set { SetEnumProperty("ItemBulletStyle", TitleIndexItemBulletStyle.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexFilterBox"),
#endif
		Category("Filtering"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterBox FilterBox {
			get {
				if(fFilterBox == null)
					fFilterBox = new FilterBox(this);
				return fFilterBox;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexNoDataText"),
#endif
		Category("Filtering"), NotifyParentProperty(true), DefaultValue(StringResources.TitleIndex_DefaultNoDataText),
		AutoFormatDisable, Localizable(true)]
		public string NoDataText {
			get { return GetStringProperty("NoDataText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_NoData)); }
			set { SetStringProperty("NoDataText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.TitleIndex_NoData), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexSoftFiltering"),
#endif
		Category("Filtering"), NotifyParentProperty(true), DefaultValue(false),
		AutoFormatDisable]
		public bool SoftFiltering {
			get { return GetBoolProperty("SoftFiltering", false); }
			set { SetBoolProperty("SoftFiltering", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexTarget"),
#endif
		DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return ViewStateUtils.GetStringProperty(ViewState, "Target", ""); }
			set { ViewStateUtils.SetStringProperty(ViewState, "Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexShowBackToTop"),
#endif
		NotifyParentProperty(true), Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool ShowBackToTop {
			get { return GetBoolProperty("ShowBackToTop", false); }
			set {
				SetBoolProperty("ShowBackToTop", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexDescriptionField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), AutoFormatDisable]
		public string DescriptionField {
			get { return GetStringProperty("DescriptionField", ""); }
			set {
				SetStringProperty("DescriptionField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupingField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), AutoFormatDisable]
		public string GroupingField {
			get { return GetStringProperty("GroupingField", ""); }
			set {
				SetStringProperty("GroupingField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexNameField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), AutoFormatDisable]
		public string NameField {
			get { return GetStringProperty("NameField", ""); }
			set {
				SetStringProperty("NameField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexNavigateUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)),
		AutoFormatDisable]
		public string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", ""); }
			set {
				SetStringProperty("NavigateUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupHeaderFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), AutoFormatDisable, Localizable(true)]
		public string GroupHeaderFormatString {
			get { return GetStringProperty("GroupHeaderFormatString", "{0}"); }
			set { SetStringProperty("GroupHeaderFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelItemFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), AutoFormatDisable, Localizable(true)]
		public string IndexPanelItemFormatString {
			get { return GetStringProperty("IndexPanelItemFormatString", "{0}"); }
			set { SetStringProperty("IndexPanelItemFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(false), AutoFormatDisable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), AutoFormatDisable]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set {
				SetStringProperty("TextField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit IndexPanelSpacing {
			get { return GetUnitProperty("IndexPanelSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "IndexPanelSpacing");
				SetUnitProperty("IndexPanelSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexFilterBoxSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit FilterBoxSpacing {
			get { return GetUnitProperty("FilterBoxSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "FilterBoxSpacing");
				SetUnitProperty("FilterBoxSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit GroupSpacing {
			get { return GetUnitProperty("GroupSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "GroupSpacing");
				SetUnitProperty("GroupSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexBackToTopSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit BackToTopSpacing {
			get { return BackToTopStyle.Spacing; }
			set {
				BackToTopStyle.Spacing = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexCategorized"),
#endif
		Category("Behavior"), AutoFormatEnable, DefaultValue(false)]
		public bool Categorized {
			get { return GetBoolProperty("Categorized", false); }
			set { SetBoolProperty("Categorized", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public TitleIndexClientSideEvents ClientSideEvents {
			get { return (TitleIndexClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return (ControlStyle as AppearanceStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexBackToTopImage"),
#endif
		Category("Images"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties BackToTopImage {
			get { return Images.BackToTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItemImage"),
#endif
		Category("Images"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties ItemImage {
			get { return Images.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public IndexPanelStyle IndexPanelStyle {
			get { return Styles.IndexPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public IndexPanelItemStyle IndexPanelItemStyle {
			get { return Styles.IndexPanelItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelItemLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LinkStyle IndexPanelItemLinkStyle {
			get { return Styles.IndexPanelItemLink; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelSeparatorStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public IndexPanelSeparatorStyle IndexPanelSeparatorStyle {
			get { return Styles.IndexPanelSeparator; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexBackToTopStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public BackToTopStyle BackToTopStyle {
			get { return Styles.BackToTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TitleIndexGroupHeaderStyle GroupHeaderStyle {
			get { return Styles.GroupHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupHeaderTextStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TitleIndexGroupHeaderStyle GroupHeaderTextStyle {
			get { return Styles.GroupHeaderText; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TitleIndexGroupContentStyle GroupContentStyle {
			get { return Styles.GroupContent; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexColumnSeparatorStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ColumnSeparatorStyle ColumnSeparatorStyle {
			get { return Styles.ColumnSeparator; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexColumnStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ColumnStyle ColumnStyle {
			get { return Styles.Column; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexFilterBoxStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public FilterBoxStyle FilterBoxStyle {
			get { return Styles.FilterBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexFilterBoxEditStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public FilterBoxEditorStyle FilterBoxEditStyle {
			get { return Styles.FilterBoxEdit; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexFilterBoxInfoTextStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public FilterBoxInfoTextStyle FilterBoxInfoTextStyle {
			get { return Styles.FilterBoxInfoText; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TitleIndexItemStyle ItemStyle {
			get { return Styles.Item; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TitleIndexColumnSeparatorTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ColumnSeparatorTemplate {
			get { return fColumnSeparatorTemplate; }
			set {
				fColumnSeparatorTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GroupHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupHeaderTemplate {
			get { return fGroupHeaderTemplate; }
			set {
				fGroupHeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(IndexPanelItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate IndexPanelItemTemplate {
			get { return fIndexPanelItemTemplate; }
			set {
				fIndexPanelItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TitleIndexItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTemplate {
			get { return fItemTemplate; }
			set {
				fItemTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItemCommand"),
#endif
		Category("Action")]
		public event TitleIndexItemCommandEventHandler ItemCommand
		{
			add
			{
				Events.AddHandler(EventItemCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventItemCommand, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexGroupHeaderCommand"),
#endif
		Category("Action")]
		public event GroupHeaderCommandEventHandler GroupHeaderCommand
		{
			add
			{
				Events.AddHandler(EventGroupHeaderCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventGroupHeaderCommand, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexIndexPanelItemCommand"),
#endif
		Category("Action")]
		public event IndexPanelItemCommandEventHandler IndexPanelItemCommand
		{
			add
			{
				Events.AddHandler(EventIndexPanelItemCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventIndexPanelItemCommand, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItemClick"),
#endif
		Category("Action")]
		public event TitleIndexItemEventHandler ItemClick
		{
			add
			{
				Events.AddHandler(EventItemClick, value);
			}
			remove
			{
				Events.RemoveHandler(EventItemClick, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTitleIndexItemDataBound"),
#endif
		Category("Data")]
		public event TitleIndexItemEventHandler ItemDataBound
		{
			add
			{
				Events.AddHandler(EventItemDataBound, value);
			}
			remove
			{
				Events.RemoveHandler(EventItemDataBound, value);
			}
		}
		protected int IndexPanelItemCharIndex {
			get { return fIndexPanelItemCharIndex; }
			set {
				if (!IsLoading()) {
					if (value < -1 || value >= GetExistingCharSet().Length)
						throw new Exception();
				}
				SetIndexPanelItemIndex(value);
			}
		}
		protected internal int ColumnCountActual { 
			get {
				int count = GetColumns().Count;
				if (count < 1)
					count = 1;
				if (Categorized) {
					if (GetMaximumColumnCount() == 0)
						count = 0;
					else
						if ((count == 0) || (count > GetMaximumColumnCount()))
							count = GetMaximumColumnCount();
					if (ColumnCount == 0)
						count = GetMaximumColumnCount() > 3 ? 3 : GetMaximumColumnCount();
				}
				else
					if (count >= SortedNodes.Count)
						count = SortedNodes.Count;
				return count;
			}
		}
		protected internal TitleIndexNodeCollection SortedNodes {
			get {
				if (fSortedNodes == null)
					fSortedNodes = GetSortedNodes();
				return fSortedNodes;
			}
		}
		protected TitleIndexImages Images {
			get { return (TitleIndexImages)ImagesInternal; }
		}
		protected TitleIndexStyles Styles {
			get { return (TitleIndexStyles)StylesInternal; }
		}
		public ASPxTitleIndex()
			: base() {
			EnableCallBacks = true;
		}
		public Control FindControl(TitleIndexItem item, string id) {
			return TemplateContainerBase.FindTemplateControl(this, GetItemTemplateContainerID(item), id);			
		}
		public Control FindControl(object groupValue, string id) {
			int index = GetGroupIndex(groupValue);
			TitleIndexItem item = null;
			if (index != -1)
				item = SortedNodes[index].Item;
			if (item != null) {
				return TemplateContainerBase.FindTemplateControl(this, GetGroupHeaderTemplateContainerID(item), id);
			}
			return null;
		}
		public int GetGroupItemCount(object groupValue) {
			int index = GetGroupIndex(groupValue);
			if (index != -1)
				return SortedNodes[index].ChildNodes.Count;
			else
				return 0;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is TitleIndexItemCommandEventArgs) {
				OnItemCommand(e as TitleIndexItemCommandEventArgs);
				return true;
			}
			if (e is GroupHeaderCommandEventArgs) {
				OnGroupHeaderCommand(e as GroupHeaderCommandEventArgs);
				return true;
			}
			if (e is IndexPanelItemCommandEventArgs) {
				OnIndexPanelItemCommand(e as IndexPanelItemCommandEventArgs);
				return true;
			}
			return false;
		}
		protected internal bool HasVisibleItems() {
			return Items.Count > 0;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TitleIndexClientSideEvents();
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Columns, FilterBox, Items, IndexPanel });
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result[IndexStateFieldName] = IndexPanelItemCharIndex;
			return result;
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if (!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
		}
		protected internal ITemplate GetIndexPanelItemTemplate() {
			return IndexPanelItemTemplate;
		}
		protected internal ITemplate GetItemTemplate(TitleIndexNode item) {
			if (GetNodeLevel(item) == 0)  
				return GroupHeaderTemplate;
			else
				return ItemTemplate;
		}
		private void OnGroupHeaderCommand(GroupHeaderCommandEventArgs e) {
			GroupHeaderCommandEventHandler handler = (GroupHeaderCommandEventHandler)Events[EventGroupHeaderCommand];
			if (handler != null)
				handler(this, e);
		}
		private void OnIndexPanelItemCommand(IndexPanelItemCommandEventArgs e) {
			IndexPanelItemCommandEventHandler handler = (IndexPanelItemCommandEventHandler)Events[EventIndexPanelItemCommand];
			if (handler != null)
				handler(this, e);
		}
		private void OnItemCommand(TitleIndexItemCommandEventArgs e) {
			TitleIndexItemCommandEventHandler handler = (TitleIndexItemCommandEventHandler)Events[EventItemCommand];
			if (handler != null)
				handler(this, e);
		}
		private void OnItemClick(TitleIndexItemEventArgs e) {
			TitleIndexItemEventHandler handler = (TitleIndexItemEventHandler)Events[EventItemClick];
			if (handler != null)
				handler(this, e);
		}
		protected void OnItemDataBound(TitleIndexItemEventArgs e) {
			TitleIndexItemEventHandler handler = (TitleIndexItemEventHandler)Events[EventItemDataBound];
			if (handler != null)
				handler(this, e);
		}
		protected internal void ColumnChanged() {
			if (!IsLoading())
				ResetControlHierarchy();
		}
		protected internal void ItemsChanged() {
			if (!IsLoading()) {
				fSortedNodes = null;
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected internal void GroupChanged() {
			ItemsChanged();
		}
		protected override bool HasHoverScripts() {
			if(!Categorized) {
				for(int i = 0; i < GetColumns().Count; i++) {
					if(CanColumnHotTrack(i))
						return true;
				}
			}
			return false;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			for (int i = 0; i < GetColumns().Count; i++) {
				if (CanColumnHotTrack(i))
					helper.AddStyle(GetColumnHoverCssStyle(i), GetColumnIDPrefix(i), IsEnabled());
			}
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected override bool HasContent() {
			return HasVisibleItems();
		}
		protected override void ClearControlFields() {
			fMainControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			fCharSetInLines = GetCharSetInLines();
			fMainControl = new MainControl(this);
			Controls.Add(fMainControl);
			base.CreateControlHierarchy();
		}
		protected internal string GetCategoryHeaderCellID(int rowIndex) {
			return "CH" + rowIndex.ToString();
		}
		protected internal string GetColumnIDPrefix(int columnIndex) {
			return "C" + columnIndex;
		}
		protected internal string GetColumnID(int columnIndex, int rowIndex) {
			if (Categorized)
				return GetColumnIDPrefix(columnIndex) + "_" + rowIndex.ToString();
			else
				return GetColumnIDPrefix(columnIndex);
		}
		protected internal string GetItemElementID(TitleIndexItem item) {
			if(item.Name != "")
				return item.Name;
			else if(HasItemServerClickEventHandler())
				return item.Index.ToString();
			return "";
		}
		protected internal string GetContentControlID() {
			return "CC";
		}
		protected internal string GetContentTableID() {
			return "CT";
		}
		protected internal string GetContentCellID() {
			return "CCell";
		}
		protected internal string GetEmptyFilterResultDivID() {
			return "TI_E";
		}
		protected internal string GetFilterInputID() {
			return "FI";
		}
		protected internal string GetFootIndexPanelCellID() {
			return "FIPCell";
		}
		protected internal string GetHeadIndexPanelCellID() {
			return "HIPCell";
		}
		protected internal string GetTreeViewCellID() {
			return "ICell";
		}
		protected internal string GetItemIndexPath(TitleIndexNode node) {
			string ret = GetItemIndexInLevel(node).ToString();
			TitleIndexNode curNode = GetParentNode(node);
			while (curNode != null) {
				ret = GetItemIndexInLevel(curNode).ToString() + "_" + ret;
				curNode = GetParentNode(curNode);
			}
			return ret;
		}
		protected internal string GetItemTemplateContainerID(TitleIndexItem item) {
			return "NTC" + "_" + item.Index.ToString(); 
		}
		protected internal string GetGroupHeaderTemplateContainerID(TitleIndexItem item) {
			return "NTC" + "_" + item.Text;
		}
		protected bool CanColumnHotTrack(int columnIndex) {
			return !GetColumnHoverStyleInternal(columnIndex).IsEmpty;
		}
		protected override void OnDataBinding(EventArgs e) {
			base.OnDataBinding(e);
			EnsureChildControls();
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if (!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				ClearItems();
			else if (!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindItems(data);
				ResetControlHierarchy();
			}
		}
		private void DataBindItems(IEnumerable data) {
			string textFieldName = String.IsNullOrEmpty(TextField) ? "Text" : TextField;
			string urlFieldName = String.IsNullOrEmpty(NavigateUrlField) ? "NavigateUrl" : NavigateUrlField;
			ClearItems();
			foreach (object obj in data) {
				TitleIndexItem item = new TitleIndexItem();
				DataBindItemProperties(item, obj, textFieldName, urlFieldName, GroupingField, DescriptionField, NameField);
				item.SetDataItem(obj);
				Items.Add(item);
				OnItemDataBound(new TitleIndexItemEventArgs(item));
			}
		}
		protected internal void DataBindItemProperties(TitleIndexItem item, object obj,
			string textFieldName, string urlFieldName, string groupingValueFieldName, string descriptionFieldName,
			string nameFieldName) {
			if(obj is string)
				item.Text = (string)obj;
			else
				item.Text = GetFieldValue(obj, textFieldName, TextField != "", "").ToString();
			item.NavigateUrl = GetFieldValue(obj, urlFieldName, NavigateUrlField != "", "").ToString();
			item.GroupValue = GetFieldValue(obj, groupingValueFieldName, GroupingField != "", null);
			item.Description = GetFieldValue(obj, descriptionFieldName, DescriptionField != "", "").ToString();
			item.Name = GetFieldValue(obj, nameFieldName, NameField != "", "").ToString();
		}
		protected internal void ClearItems() {
			Items.Clear();
			fSortedNodes = null;
		}
		protected override bool IsServerSideEventsAssigned() {
			return Events[EventItemClick] != null;
		}
		protected override void RegisterStandaloneScriptBlocks() {
			if (ShowBackToTop)
				RegisterBackToTopScript();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (IsClientFiltering()) {
				if (FilterBox.AutoFocus)
					stb.Append(localVarName + ".autoFocus=true;\n");
				stb.Append(localVarName + ".columnCount=" + ColumnCountActual + ";\n");
				stb.Append(localVarName + ".filterDelay=" + FilterBox.Delay + ";\n");
				if (Categorized)
					stb.Append(localVarName + ".rowCount=" + GetRowCount().ToString() + ";\n");
				stb.Append(localVarName + ".groupSpacing=" + GetItemSpacing(0).Value.ToString() + ";\n");
				stb.Append(localVarName + ".groupContentPaddingTop=" +
					GetGroupContentPadding(SortedNodes[0].ChildNodes[0]).GetPaddingTop().Value.ToString() + ";\n");
				stb.Append(localVarName + ".groupContentPaddingBottom=" +
					GetGroupContentPadding(SortedNodes[0].ChildNodes[0]).GetPaddingBottom().Value.ToString() + ";\n");
				stb.Append(localVarName + ".showBackToTop=" + IsShowBackToTop().ToString().ToLower() + ";\n");
				if(SoftFiltering)
					stb.Append(localVarName + ".softFiltering=true;\n");
			}
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if (Events[EventItemClick] != null)
				eventNames.Add("ItemClick");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTitleIndex";
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || IsClientFiltering() || IsCallBacksEnabled() || HasItemOnClick();
		}
		protected internal bool HasItemOnClick() {
			return ClientSideEvents.ItemClick != "" || HasItemServerClickEventHandler();
		}
		protected bool HasItemServerClickEventHandler() {
			return Events[EventItemClick] != null;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxTitleIndex), TitleIndexScriptResourceName);
		}
		protected internal bool HasAlphabetItemCellOnClick(string c) {
			return IsPaging() && IsExistingChar(c) &&
				!IsCurrentIndexPanelIndex(c);
		}
		protected internal bool HasFilterInputOnKeyPress() {
			return FilterBox.Visible && !Categorized;
		}
		protected internal string GetIndexPanelItemCellOnClick(string c) {
			if (IsCallBacksEnabled())
				return string.Format(IndexPanelItemClickHandlerName, ClientID, fExistingCharSet.IndexOf(c));
			else
				return RenderUtils.GetPostBackEventReference(Page, this, "ALPHITEMCLICK:" + fExistingCharSet.IndexOf(c));
		}
		protected internal string GetFilterInputOnBlur() {
			return string.Format(FilterInputBlurHandlerName, ClientID);
		}
		protected internal string GetFilterInputOnChange() {
			return string.Format(FilterInputChangeHandlerName, ClientID);
		}
		protected internal string GetFilterInputOnFocus() {
			return string.Format(FilterInputFocusHandlerName, ClientID);
		}
		protected internal string GetFilterInputOnKeyPress() {
			return string.Format(FilterInputKeyPressHandlerName, ClientID);
		}
		protected internal string GetFilterInputOnKeyUp() {
			return string.Format(FilterInputKeyUpHandlerName, ClientID);
		}
		protected internal string GetControlOnClick() {
			return string.Format(ControlClickHandlerName, ClientID);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new TitleIndexStyles(this);
		}
		protected LinkStyle GetIndexPanelLinkStyle() {
			LinkStyle style = new LinkStyle();
			style.CopyFrom(IndexPanelItemLinkStyle);
			return style;
		}
		protected override void RegisterLinkStyles() {
			if (IndexPanel.Visible &&
				(IndexPanel.Behavior == IndexPanelBehavior.Navigation) && IsEnabled()) {
				string contentID = ClientID + "_" + GetContentTableID();
				RegisterLinkVisitedStyle(LinkStyle.VisitedStyle, contentID);
				RegisterLinkHoverStyle(LinkStyle.HoverStyle, contentID);
				if (IsShowHeadIndexPanel()) {
					RegisterLinkVisitedStyle(GetIndexPanelLinkStyle().VisitedStyle,
						ClientID + "_" + GetHeadIndexPanelCellID());
					RegisterLinkHoverStyle(GetIndexPanelLinkStyle().HoverStyle,
						ClientID + "_" + GetHeadIndexPanelCellID());
				}
				if (IsShowFootIndexPanel()) {
					RegisterLinkVisitedStyle(GetIndexPanelLinkStyle().VisitedStyle,
						ClientID + "_" + GetFootIndexPanelCellID());
					RegisterLinkHoverStyle(GetIndexPanelLinkStyle().HoverStyle,
						ClientID + "_" + GetFootIndexPanelCellID());
				}
			}
			else
				base.RegisterLinkStyles();
		}
		protected internal Paddings GetEmptyFilteringResultCaptionPadding() {
			Paddings ret = new Paddings(Unit.Empty, Unit.Pixel(50), 0, Unit.Pixel(60));
			return ret;
		}
		protected internal Paddings GetIndexPanelPadding() {
			return GetIndexPanelStyle().Paddings;
		}
		protected internal Paddings GetIndexPanelItemPadding() {
			return GetIndexPanelItemStyleInternal().Paddings;
		}
		protected internal Paddings GetBackToTopPadding() {
			Paddings ret = new Paddings();
			ret.CopyFrom(Styles.GetDefaultBackToTopStyle().Paddings);
			ret.CopyFrom(GetBackToTopStyle().Paddings);
			return ret;
		}
		protected internal Paddings GetFilterBoxPaddings() {
			return GetFilterBoxStyle().Paddings;
		}
		protected internal Paddings GetFilterBoxEditPaddings() {
			return GetFilterBoxEditStyle().Paddings;
		}
		protected internal Paddings GetFilterBoxHintPaddings() {
			return GetFilterBoxInfoTextStyle().Paddings;
		}
		protected internal Paddings GetGroupContentPadding(TitleIndexNode parentNode) {
			Paddings ret = GetCustomGroupContentPadding(GetNodeLevel(parentNode));
			ret.PaddingLeft = Unit.Empty;
			ret.PaddingRight = Unit.Empty;
			return ret;
		}
		protected internal Paddings GetCustomGroupContentPadding(int level) {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Styles.GetDefaultGroupContentStyle(Categorized).Paddings);
			paddings.CopyFrom(GetGroupContentStyle().Paddings);
			return paddings;
		}
		protected internal Paddings GetItemMargins(TitleIndexNode node, bool isFirstInColumn) {
			Paddings margins = new Paddings();
			int level = GetNodeLevel(node);
			if (level > 0) {
				Paddings childPaddings = GetCustomGroupContentPadding(GetNodeLevel(GetParentNode(node)));
				margins.PaddingLeft = childPaddings.GetPaddingLeft();
				margins.PaddingRight = childPaddings.GetPaddingRight();
				if (!IsFirstChildNode(node))
					margins.PaddingTop = GetItemSpacing(level);
			}
			else
				if (!IsFirstNode(node))
					margins.PaddingTop = GetItemSpacing(level);
			if (isFirstInColumn)
				margins.PaddingTop = 0;
			return margins;
		}
		protected internal Paddings GetItemPaddings(TitleIndexNode node) {
			Paddings paddings = new Paddings();
			int level = GetNodeLevel(node);
			paddings.CopyFrom(GetItemStyleInternal(level).Paddings);
			return paddings;
		}
		protected internal Paddings GetGroupHeaderTextPaddings() {
			return GetGroupHeaderTextStyleInternal().Paddings;
		}
		protected internal Unit GetIndexPanelLineSpacing() {
			return GetIndexPanelStyle().LineSpacing;
		}
		protected internal Unit GetFilterBoxSpacing() {
			if (FilterBoxSpacing.IsEmpty)
				return Styles.GetFilterBoxSpacing();
			else
				return FilterBoxSpacing;
		}
		protected internal Unit GetIndexPanelSpacing() {
			return IndexPanelSpacing.IsEmpty ?
				Styles.GetIndexPanelSpacing(FilterBox.VerticalPosition, FilterBox.Visible) :
				IndexPanelSpacing;
		}
		protected internal Unit GetItemSpacing(int level) {
			Unit spacing = level == 0 ? GroupSpacing : GroupContentStyle.ItemSpacing;
			if ((!spacing.IsEmpty))
				return spacing;
			else {
				return level == 0 ? Styles.GetGroupSpacing() : Styles.GetDefaultItemStyle().Spacing;
			}
		}
		protected internal Unit GetBackToTopSpacing() {
			return GetBackToTopStyle().Spacing;
		}
		protected internal TitleIndexGroupHeaderStyle GetGroupHeaderTextStyle() { 
			TitleIndexGroupHeaderStyle ret = new TitleIndexGroupHeaderStyle();
			ret.CopyFontFrom(GetItemStyleInternal(0));
			ret.CopyFrom(GetGroupHeaderTextStyleInternal());
			return ret;
		}
		protected TitleIndexGroupHeaderStyle GetGroupHeaderTextStyleInternal() {
			TitleIndexGroupHeaderStyle ret = new TitleIndexGroupHeaderStyle();
			ret.CopyFrom(Styles.GetDefaultGroupHeaderTextStyle(Categorized));
			ret.CopyFrom(GroupHeaderTextStyle);
			return ret;
		}
		protected internal TitleIndexGroupContentStyle GetGroupContentStyle() {
			return GroupContentStyle;
		}
		protected AppearanceStyle GetCustomItemStyle1() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(ItemStyle);
			return style;
		}
		protected AppearanceStyle GetCustomHeaderStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GroupHeaderStyle);
			return style;
		}
		protected AppearanceSelectedStyle GetCurrentItemStyle(TitleIndexNode item) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(ItemStyle.CurrentItemStyle);
			return style;
		}
		protected AppearanceStyle GetDefaultItemStyle(int level) {
			if (level == 0)
				return Styles.GetDefaultGroupHeaderStyle(Categorized);
			else
				return Styles.GetDefaultItemStyle();
		}
		protected internal AppearanceStyleBase GetItemStyle(TitleIndexNode node) {
			AppearanceStyleBase style = GetItemStyleInternal(GetNodeLevel(node));
			if (style.Wrap == DefaultBoolean.Default)
				style.Wrap = GetControlStyle().Wrap;
			if (IsCurrentItem(node))
				style.CopyFrom(GetCurrentItemStyle(node));
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyle GetItemStyleInternal(int level) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetDefaultItemStyle(level));
			if (level == 0)
				style.CopyFrom(GetCustomHeaderStyle());
			else
				style.CopyFrom(GetCustomItemStyle1());
			return style;
		}
		protected internal LinkStyle GetCustomItemLinkStyle(TitleIndexNode item) {
			return LinkStyle;
		}
		protected internal AppearanceStyleBase GetItemLinkStyle(TitleIndexNode item) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			int level = GetNodeLevel(item);
						style.CopyFontAndCursorFrom(GetDefaultItemStyle(level));
			if (level == 0)
								style.CopyFontAndCursorFrom(GetCustomHeaderStyle());
			else
								style.CopyFontAndCursorFrom(GetCustomItemStyle1());
			style.CopyFrom(GetCustomItemLinkStyle(item).Style);
			MergeDisableStyle(style);
			return style;
		}
		protected Paddings GetColumnPaddings() {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Styles.GetDefaultColumnStyle().Paddings);
			paddings.CopyFrom(ColumnStyle.Paddings);
			return paddings;
		}
		protected internal Paddings GetColumnPaddings(int columnIndex) {
			Paddings paddings = GetColumnPaddings();
			if (IsValidColumnIndex(columnIndex))
				paddings.CopyFrom(GetColumns()[columnIndex].Paddings);
			return paddings;
		}
		protected AppearanceStyleBase GetColumnHoverStyleInternal(int columnIndex) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(ColumnStyle.HoverStyle);
			if (IsValidColumnIndex(columnIndex))
				style.CopyFrom(GetColumns()[columnIndex].HoverStyle);
			return style;
		}
		protected AppearanceStyleBase GetColumnHoverStyle(int columnIndex) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyBordersFrom(GetColumnStyle(columnIndex));
			style.CopyFrom(GetColumnHoverStyleInternal(columnIndex));
			return style;
		}
		protected internal AppearanceStyle GetColumnHoverCssStyle(int columnIndex) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetColumnHoverStyle(columnIndex));
			style.Paddings.CopyFrom(GetColumnHoverStylePaddings(columnIndex));
			return style;
		}
		protected Paddings GetColumnHoverStylePaddings(int columnIndex) {
			ColumnStyle style = GetColumnStyle(columnIndex);
			Paddings paddings = GetColumnPaddings(columnIndex);
			return UnitUtils.GetSelectedCssStylePaddings(style,
				GetColumnHoverStyle(columnIndex), paddings);
		}
		protected internal Paddings GetColumnSeparatorPaddings() {
			return GetColumnSeparatorStyle().Paddings;
		}
		protected internal ColumnSeparatorStyle GetColumnSeparatorStyle() {
			ColumnSeparatorStyle style = new ColumnSeparatorStyle();
			style.CopyFrom(Styles.GetDefaultColumnSeparatorStyle());
			style.CopyFrom(ColumnSeparatorStyle);
			return style;
		}
		protected internal Unit GetColumnSeparatorWidth() {
			return !ColumnSeparatorStyle.Width.IsEmpty ?
				ColumnSeparatorStyle.Width : Styles.GetDefaultColumnSeparatorStyle().Width;
		}
		protected internal ColumnStyle GetColumnStyle(int columnIndex) {
			ColumnStyle style = new ColumnStyle();
			style.CopyFrom(ColumnStyle);
			if (IsValidColumnIndex(columnIndex))
				style.CopyFrom(GetColumns()[columnIndex].Style);
			MergeDisableStyle(style);
			return style;
		}
		protected double GetColumnsWidth() {
			double columnsWidth = Width.Value;
			if (GetColumnSeparatorWidth().Type == Width.Type)
				columnsWidth -= GetColumnSeparatorWidth().Value * (ColumnCountActual - 1);
			if (GetColumnSeparatorPaddings().GetPaddingLeft().Type == Width.Type)
				columnsWidth -= GetColumnSeparatorPaddings().GetPaddingLeft().Value * (ColumnCountActual - 1);
			if (GetColumnSeparatorPaddings().GetPaddingRight().Type == Width.Type)
				columnsWidth -= GetColumnSeparatorPaddings().GetPaddingRight().Value * (ColumnCountActual - 1);
			if (GetColumnPaddings().GetPaddingLeft().Type == Width.Type)
				columnsWidth -= GetColumnPaddings().GetPaddingLeft().Value;
			if (GetColumnPaddings().GetPaddingRight().Type == Width.Type)
				columnsWidth -= GetColumnPaddings().GetPaddingRight().Value;
			if (GetPaddings().GetPaddingLeft().Type == Width.Type)
				columnsWidth -= GetPaddings().GetPaddingLeft().Value;
			if (GetPaddings().GetPaddingRight().Type == Width.Type)
				columnsWidth -= GetPaddings().GetPaddingRight().Value;
			return columnsWidth;
		}
		protected bool IsColumnsWidthsEmpty() {
			for (int i = 0; i < Columns.Count; i++) {
				if (!Columns[i].Width.IsEmpty)
					return false;
			}
			return true;
		}
		protected internal Unit GetColumnWidth(int columnIndex) {
			Unit columnWidth = IsValidColumnIndex(columnIndex) ? GetColumns()[columnIndex].Width :
				Unit.Empty;
			bool isEmptyWidth = Width.IsEmpty || Width.Type == UnitType.Percentage;
			double widthValue = isEmptyWidth ? 100 : GetColumnsWidth();
			UnitType widthType = isEmptyWidth ? UnitType.Percentage : Width.Type;
			if (widthValue < 0) 
				return GetActualColumnWidthInternal(Browser.Family.IsNetscape ? 100 / ColumnCountActual + 1 : 100 / ColumnCountActual, UnitType.Percentage);
			if (columnWidth.IsEmpty) {
				if (IsColumnsWidthsEmpty())
					return GetActualColumnWidthInternal(Browser.Family.IsNetscape ? widthValue / ColumnCountActual + 1 : widthValue / ColumnCountActual, widthType);
				if (widthType == UnitType.Percentage)
					return Unit.Empty;
			}
			else {
				if (!Width.IsEmpty) {
					bool canConvert = true;
					double columnsSumWidth = columnWidth.Value;
					for (int i = 0; i < ColumnCountActual; i++) {
						if (columnIndex != i) {
							Unit width = GetColumns()[i].Width;
							if (width.Type != columnWidth.Type) {
								canConvert = false;
								break;
							}
							columnsSumWidth += width.Value;
						}
					}
					if (canConvert) {
						return GetActualColumnWidthInternal(columnWidth.Value * widthValue / columnsSumWidth, widthType);
					}
				}
			}
			return columnWidth;
		}
		protected Unit GetActualColumnWidthInternal(double columnWidth, UnitType unitType) {
			return new Unit(Browser.Family.IsNetscape && unitType == UnitType.Percentage ?
				Math.Floor(columnWidth) : columnWidth, unitType);
		}
		protected void SetColumnCount(byte count) {
			while (count < Columns.Count)
				Columns.RemoveAt(Columns.Count - 1);
			while (count > Columns.Count)
				Columns.Add();
			PropertyChanged("Columns");
		}
		protected internal AppearanceStyleBase GetIndexPanelItemLinkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultIndexPanelItemStyle());
			style.CopyFrom(IndexPanelItemLinkStyle.Style);
			style.CopyFrom(GetCustomIndexPanelItemStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal IndexPanelStyle GetIndexPanelStyle() {
			IndexPanelStyle style = new IndexPanelStyle();
			style.CopyFrom(Styles.GetDefaultIndexPanelStyle(FilterBox.VerticalPosition, FilterBox.Visible));
			style.CopyFrom(GetCustomIndexPanelStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal IndexPanelSeparatorStyle GetIndexPanelSeparatorStyle() {
			IndexPanelSeparatorStyle style = new IndexPanelSeparatorStyle();
			style.CopyFrom(Styles.GetDefaultIndexPanelSeparatorStyle());
			style.CopyFrom(IndexPanelSeparatorStyle);
			return style;
		}
		protected internal Unit GetIndexPanelSeparatorHeight() {
			return GetIndexPanelSeparatorStyle().Height;
		}
		protected internal IndexPanelItemStyle GetIndexPanelItemStyleInternal() {
			IndexPanelItemStyle style = new IndexPanelItemStyle();
			style.CopyFrom(Styles.GetDefaultIndexPanelItemStyle());
			style.CopyFrom(GetCustomIndexPanelItemStyle());
			return style;
		}
		protected internal AppearanceStyle GetIndexPanelItemStyle(string c) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetIndexPanelItemStyleInternal());
			if (IsCurrentIndexPanelIndex(c))
				style.CopyFrom(GetCurrentIndexPanelItemStyle());
			if (!IsExistingChar(c))
				style.ForeColor = GetDisabledIndexPanelItemForeColor();
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyleBase GetCurrentIndexPanelItemStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultCurrentIndexPanelItemStyle();
			ret.CopyFrom(IndexPanelItemStyle.CurrentStyle);
			return ret;
		}
		protected Color GetDisabledIndexPanelItemForeColor() {
			return GetIndexPanelItemStyleInternal().DisabledForeColor;
		}
		protected internal AppearanceStyle GetBackToTopStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultBackToTopStyle());
			style.CopyFrom(LinkStyle.Style);
			style.CopyFrom(GetCustomBackToTopStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal Unit GetBackToTopImageSpacing() {
			return GetBackToTopStyle().ImageSpacing;
		}
		protected AppearanceStyle GetCustomIndexPanelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(IndexPanelStyle);
			return style;
		}
		protected AppearanceStyle GetCustomBackToTopStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(BackToTopStyle);
			return style;
		}
		protected AppearanceStyle GetCustomIndexPanelItemStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFontFrom(GetCustomIndexPanelStyle());
			style.CopyFrom(IndexPanelItemStyle);
			return style;
		}
		protected internal FilterBoxStyle GetFilterBoxStyle() {
			FilterBoxStyle style = new FilterBoxStyle();
			style.CopyFrom(Styles.GetDefaultFilterBoxStyle());
			style.CopyFrom(FilterBoxStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal FilterBoxEditorStyle GetFilterBoxEditStyle() {
			FilterBoxEditorStyle style = new FilterBoxEditorStyle();
			MergeControlStyleForInput(style);
			style.CopyFrom(Styles.GetDefaultFilterBoxEditStyle());
			style.CopyFrom(FilterBoxEditStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyle GetFilterBoxInfoTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultFilterBoxInfoTextStyle());
			style.CopyFrom(FilterBoxInfoTextStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal Unit GetFilterBoxEditorHeight() {
			return GetFilterBoxEditStyle().Height;
		}
		protected internal Unit GetFilterBoxEditorWidth() {
			return GetFilterBoxEditStyle().Width;
		}
		protected internal Unit GetBulletIndent(TitleIndexNode item) {
			return Styles.GetBulletIndent();
		}
		protected internal TitleIndexItemBulletStyle GetItemBulletStyle(TitleIndexNode item) {
			TitleIndexItemBulletStyle ret = GetItemBulletStyleInternal(GetNodeLevel(item));
			if(ret == TitleIndexItemBulletStyle.Auto)
				ret = GetItemBulletStyleAuto(item);
			return ret;
		}
		protected internal TitleIndexItemBulletStyle GetItemBulletStyleInternal(int level) {
			return level == 1 ? ItemBulletStyle : TitleIndexItemBulletStyle.NotSet;
		}
		protected TitleIndexItemBulletStyle GetItemBulletStyleAuto(TitleIndexNode item) {
			TitleIndexItemBulletStyle ret = TitleIndexItemBulletStyle.NotSet;
			int nodeBulletStyleLevel = GetItemBulletStyleIndex(item);
			if (nodeBulletStyleLevel < DefaultItemBulletStyles.Length)
				ret = DefaultItemBulletStyles[nodeBulletStyleLevel];
			else
				ret = DefaultItemBulletStyles[DefaultItemBulletStyles.Length - 1];
			return ret;
		}
		protected int GetItemBulletStyleIndex(TitleIndexNode item) {
			int result = 0;
			int i = GetNodeLevel(item);
			while((i >= 0) && (GetItemBulletStyleInternal(i) == TitleIndexItemBulletStyle.Auto)) {
				result++;
				i--;
			}
			return result != 0 ? result - 1 : 0;
		}
		protected override ImagesBase CreateImages() {
			return new TitleIndexImages(this);
		}
		protected internal ImageProperties GetItemImage(TitleIndexNode node) {
			ImageProperties ret = new ImageProperties();
			int nodeLevel = GetNodeLevel(node);
			if (nodeLevel == 1)
				ret.CopyFrom(Images.GetImageProperties(Page, TitleIndexImages.ItemImageName));
			return ret;
		}
		protected internal Unit GetImageSpacing(TitleIndexNode node) {
			int level = GetNodeLevel(node);
			if (level == 1)
				return ItemStyle.ImageSpacing;
			else
				return Unit.Empty;
		}
		protected internal ImageProperties GetBackToTopImage() {
			return Images.GetImageProperties(Page, TitleIndexImages.BackToTopImageName);
		}
		protected internal string GetIndexPanelName() {
			return ClientID + "_" + "AP";
		}
		protected internal string GetIndexPanelBookmark() {
			return "#" + GetIndexPanelName();
		}
		protected internal string GetBookmarkLinkBySymbol(string symbol) {
			return !IsPaging() ? "#" + GetBookmarkUniqueID(symbol) :
				"";
		}
		protected internal string GetBookmarkUniqueID(string param) {
			return ClientID + "__" + GetSymbolIndex(param);
		}
		protected internal override bool IsCallBacksEnabled() {
			return EnableCallBacks && (IndexPanel.Behavior == IndexPanelBehavior.Filtering) &&
				IndexPanel.Visible;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			string[] arguments = eventArgument.Split(RenderUtils.CallBackSeparator);
			if (arguments.Length == 2)
				SetIndexPanelItemIndex(int.Parse(arguments[0]));
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			TICMainControl contentControl = FindControl(GetContentControlID()) as TICMainControl;
			if(contentControl != null) {
				BeginRendering();
				try {
					result[CallbackResultProperties.Html] = RenderUtils.GetRenderResult(contentControl);
				}
				finally {
					EndRendering();
				}
			}
			result[CallbackResultProperties.Index] = IndexPanelItemCharIndex;
			result[ColumnCountKey] = ColumnCountActual;
			return result;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			SetIndexPanelItemIndex(GetClientObjectStateValue<int>(IndexStateFieldName));
			return false;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			EnsureDataBound();
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch (arguments[0]) {
				case "ALPHITEMCLICK":
					int index = int.Parse(arguments[1]);
					SetIndexPanelItemIndex(index);
					break;
				case "CLICK":
					string itemName = arguments[1];
					TitleIndexItem item = Items.FindByNameOrIndex(itemName);
					TitleIndexItemEventArgs args = item != null ? new TitleIndexItemEventArgs(item) : new TitleIndexItemEventArgs(null);
					OnItemClick(args);
					break;
			}
		}
		protected bool IsGrouping() {
			if (IsBoundUsingDataSourceID() || (DataSource != null))
				return GroupingField != "";
			else {
				foreach (TitleIndexItem item in Items)
					if ((item.GroupValue != null) && (item.GroupValue.ToString() != ""))
						return true;
				return false;
			}
		}
		protected internal string GetIndexPanelItemSeparator() {
			return IndexPanel.Separator;
		}
		protected internal string GetIndexPanelItemFormattedText(string text) {
			return string.Format(IndexPanelItemFormatString, text);
		}
		protected internal List<string[]> GetIndexPanelCharSetInLines() {
			int lineCount = GetIndexPanelLineCount();
			List<string[]> charSetInLines = new List<string[]>();
			for (int i = 0; i < lineCount; i++) {
				string[] charSet = GetIndexPanelLineCharSet(i);
				if (charSet.Length != 0)
					charSetInLines.Add(charSet);
			}
			return charSetInLines;
		}
		protected string[] GetIndexPanelLineCharSet(int index) {
			string[] ret = new string[] { };
			string[] charSetInLine = GetIndexPanelLineCharSetInternal(index);
			if (!IndexPanel.ShowNonExistingItems) {
				List<string> charSetList = new List<string>();
				List<string> existingCharSetList = new List<string>(GetExistingCharSet());
				for (int i = 0; i < charSetInLine.Length; i++)
					if (existingCharSetList.Contains(charSetInLine[i]))
						charSetList.Add(charSetInLine[i]);
				ret = charSetList.ToArray();
			}
			else
				ret = charSetInLine;
			return ret;
		}
		protected internal int GetIndexPanelLineCount() {
			int ret = 1;
			if (!fIsGrouping) {
				if (fSpecialCharSet.Length != 0)
					ret = fCharSetInLines.Count + 1;
				else
					ret = fCharSetInLines.Count;
			}
			return ret;
		}
		protected string[] GetIndexPanelLineCharSetInternal(int index) {
			List<string> ret = new List<string>();
			if ((index == 0) && (fSpecialCharSet.Length != 0))
				ret.AddRange(fSpecialCharSet);
			else {
				index += fSpecialCharSet.Length != 0 ? -1 : 0;
				if (fCharSetInLines.Count > 0) 
					ret.AddRange(fCharSetInLines[index]);
			}
			return ret.ToArray();
		}
		protected List<string[]> GetCharSetInLines() {
			List<string[]> ret = new List<string[]>();
			List<string> allCharSet = new List<string>();
			string[] chars = MultilineToStringArray(IndexPanel.Characters);
			for (int j = 0; j < chars.Length; j++) {
				List<string> lineCharSet = new List<string>();
				string charSet = chars[j].ToUpper().Trim();
				charSet = charSet.Replace(" ", "");
				for (int i = 0; i < charSet.Length; i++) {
					if (!allCharSet.Contains(charSet[i].ToString())) {
						allCharSet.Add(charSet[i].ToString());
						lineCharSet.Add(charSet[i].ToString());
					}
				}
				if (lineCharSet.Count != 0)
					ret.Add(lineCharSet.ToArray());
			}
			return ret;
		}
		protected internal string[] GetExistingCharSet() {
			return fExistingCharSet.ToArray();
		}
		protected internal string GetNodeNavigateUrl(TitleIndexNode node) {
			if(IsCurrentItem(node) || GetNodeLevel(node) != 1) return string.Empty;
			string url = string.Format(NavigateUrlFormatString, node.NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected internal string GetNodeTarget(TitleIndexNode node) {
			int level = GetNodeLevel(node);
			return level == 1 ? Target : "";
		}
		protected internal string GetNodeText(TitleIndexNode node) {
			int level = GetNodeLevel(node);
			if (level == 0) {
				if (GroupHeaderFormatString.Contains("{1}"))
					return string.Format(GroupHeaderFormatString, node.Text, GetGroupItemCount(GetGroupValue(node)));
				else
					return string.Format(GroupHeaderFormatString, node.Text);
			}
			else
				return node.Text;
		}
		protected string[] GetSpecialCharSet() {
			List<string> ret = new List<string>();
			List<string[]> charSetInLines = GetCharSetInLines();
			List<string> allCharSet = new List<string>();
			foreach (string[] charSet in charSetInLines)
				allCharSet.AddRange(charSet);
			for (int i = 0; i < fExistingCharSet.Count; i++)
				if (!allCharSet.Contains(fExistingCharSet[i]))
					ret.Add(fExistingCharSet[i]);
			return ret.ToArray();
		}
		protected internal bool IsBulletMode(TitleIndexNode item) {
			if (GetItemImage(item).IsEmpty) {
				TitleIndexItemBulletStyle bulletStyle = GetItemBulletStyle(item);
				return (bulletStyle != TitleIndexItemBulletStyle.NotSet) &&
					(bulletStyle != TitleIndexItemBulletStyle.None);
			}
			else
				return false;
		}
		protected internal bool IsShowBackToTop() {
			return IndexPanel.Visible && ShowBackToTop;
		}
		protected internal bool IsPaging() {
			return IndexPanel.Behavior == IndexPanelBehavior.Filtering;
		}
		protected internal bool IsClientFiltering() {
			return (FilterBox.Visible && (ItemTemplate == null));
		}
		protected internal bool IsShowHeadIndexPanel() {
			return (IndexPanel.Position == PagerPosition.Top ||
				IndexPanel.Position == PagerPosition.TopAndBottom) && IndexPanel.Visible;
		}
		protected internal bool IsShowInfoTextInFilterBox() {
			return FilterBox.InfoText != "";
		}
		protected internal bool IsShowFootIndexPanel() {
			return (IndexPanel.Position == PagerPosition.Bottom ||
				IndexPanel.Position == PagerPosition.TopAndBottom) && IndexPanel.Visible;
		}
		protected bool IsFirstChildNode(TitleIndexNode node) {
			TitleIndexNode parentNode = GetParentNode(node);
			return (parentNode != null) && (parentNode.ChildNodes[0] == node);
		}
		protected bool IsFirstNode(TitleIndexNode node) {
			return SortedNodes.IndexOf(node) == 0;
		}
		protected internal bool IsRootNode(TitleIndexNode item) {
			return item.ParentNode == null;
		}
		protected internal TitleIndexNode GetParentNode(TitleIndexNode node) {
			if (IsRootNode(node))
				return null;
			else
				return node.ParentNode;
		}
		protected internal TitleIndexNode GetPreviousSibling(TitleIndexNode node) {
			if ((IsRootNode(node)) && (SortedNodes.Count == 1))
				return null;
			else
				return node.PreviousSibling;
		}
		protected internal bool IsCurrentItem(TitleIndexNode item) {
			return !DesignMode && UrlUtils.IsCurrentUrl(ResolveUrl(item.NavigateUrl));
		}
		protected internal bool HasChildNodes(TitleIndexNode node) {
			return node.ChildNodes.Count != 0;
		}
		protected internal TitleIndexColumnCollection GetColumns() {
			if (Columns.Count != 0)
				return Columns;
			else {
				if (fDummyColumns == null) {
					fDummyColumns = new TitleIndexColumnCollection();
					fDummyColumns.Add(new TitleIndexColumn());
				}
				return fDummyColumns;
			}
		}
		protected internal object GetGroupValue(TitleIndexNode node) {
			return !string.IsNullOrEmpty(node.Item.GroupValue.ToString()) ? node.Item.GroupValue : node.Item.Text;
		}
		protected internal object GetGroupValue(int indexPanelItemIndex) {
			return !string.IsNullOrEmpty(SortedNodes[indexPanelItemIndex].Item.GroupValue.ToString()) ?
				SortedNodes[indexPanelItemIndex].Item.GroupValue :
				SortedNodes[indexPanelItemIndex].Item.Text;
		}
		protected internal int GetMaximumColumnCount() {
			int max = 0;
			foreach (TitleIndexNode node in SortedNodes) {
				if (HasChildNodes(node) && (node.ChildNodes.Count > max))
					max = node.ChildNodes.Count;
			}
			return max;
		}
		protected internal int GetItemIndexInLevel(TitleIndexNode node) {
			TitleIndexNode parentNode = GetParentNode(node);
			return parentNode != null ? parentNode.ChildNodes.IndexOf(node) : SortedNodes.IndexOf(node);
		}
		protected internal virtual int GetNodeLevel(TitleIndexNode node) {
			int ret = 0;
			TitleIndexNode curNode = node;
			while (!IsRootNode(curNode)) {
				ret++;
				curNode = GetParentNode(curNode);
			}
			return ret;
		}
		protected internal int GetRowCount() {
			return Categorized ? SortedNodes.Count : 1;
		}
		protected internal bool IsCurrentIndexPanelIndex(string c) {
			return IsPaging() &&
				(fExistingCharSet.IndexOf(c) == IndexPanelItemCharIndex);
		}
		protected internal bool IsExistingChar(string c) {
			return fExistingCharSet.Contains(c);
		}
		protected bool IsIndexPanelInTop() {
			return (IndexPanel.Position != PagerPosition.Bottom) && IndexPanel.Visible;
		}
		protected internal bool IsValidColumnIndex(int index) {
			return (0 <= index) && (index < Columns.Count) ? true : false;
		}
		protected internal bool NeedTableNodeRender(TitleIndexNode node) {
			int level = GetNodeLevel(node);
			if (level == 0 && !GetGroupHeaderTextStyleInternal().IsEmpty)
				return true;
			if (!GetItemImage(node).IsEmpty)
				return Browser.Family.IsNetscape || Browser.Family.IsWebKit || GetItemStyle(node).Wrap == DefaultBoolean.True;
			else
				return false;
		}
		private void SetIndexPanelItemIndex(int value) {
			if (value < -1)
				value = 0;
			fIndexPanelItemCharIndex = value;
			fSortedNodes = null;
			ResetViewStateStoringFlag();
			LayoutChanged();
		}
		private int GetSymbolIndex(string c) {
			return fExistingCharSet.IndexOf(c);
		}
		private string[] MultilineToStringArray(string str) {
			string[] separator = new string[] { "\r\n" };
			StringSplitOptions ret = new StringSplitOptions();
			return str.Split(separator, ret);
		}
		private void AddChildItemsToItem(TitleIndexNodeCollection childNodes, TitleIndexNode parentNode) {
			foreach (TitleIndexNode node in childNodes)
				parentNode.ChildNodes.Add(node);
		}
		private TitleIndexNodeCollection ArrangeItems(TitleIndexItemCollection items) {
			TitleIndexNodeCollection sortedItems = GetTitleIndexNodeCollection(items);
			Dictionary<int, string> strList = GetStrWithIndexList(sortedItems);
			Dictionary<string, int[]> sortedTitleItemPos = SortUtils.ArrangeIntoLetter(strList, LatinLanguageInfo);
			return GenerateItemHierarchy(sortedTitleItemPos, sortedItems);
		}
		private TitleIndexNodeCollection ArrangeItemsWithGrouping(TitleIndexItemCollection items) {
			Dictionary<object, List<TitleIndexNode>> nodesInGroup = new Dictionary<object, List<TitleIndexNode>>();
			for (int i = 0; i < items.Count; i++) {
				TitleIndexNode newNode = new TitleIndexNode(items[i]);
				if (!nodesInGroup.ContainsKey(items[i].GroupValue)) {
					nodesInGroup.Add(items[i].GroupValue, new List<TitleIndexNode>());
					nodesInGroup[items[i].GroupValue].Add(newNode);
				}
				else
					nodesInGroup[items[i].GroupValue].Add(newNode);
			}
			Dictionary<object, List<TitleIndexNode>> sortedGroups = SortUtils.SortGroupValues(nodesInGroup, LatinLanguageInfo);
			SortUtils.SortItemsInGroup(sortedGroups, LatinLanguageInfo);
			TitleIndexNodeCollection ret = new TitleIndexNodeCollection();
			foreach (object groupVal in sortedGroups.Keys) {
				TitleIndexNode groupItem = new TitleIndexNode();
				groupItem.Text = groupVal.ToString();
				foreach (TitleIndexNode item in sortedGroups[groupVal]) {
					groupItem.ChildNodes.Add(item);
				}
				ret.Add(groupItem);
			}
			return ret;
		}
		private TitleIndexItemCollection SortItemsByTextField(TitleIndexItemCollection items) {
			TitleIndexItemCollection ret = new TitleIndexItemCollection();
			string[] texts = GetTextFiledValues(items);
			int[] newPosition = SortUtils.SortStrings(texts, LatinLanguageInfo);
			for (int i = 0; i < newPosition.Length; i++)
				ret.Add(items[newPosition[i]]);
			return ret;
		}
		private TitleIndexNodeCollection ArrangeItemsByAlphabetIndex(TitleIndexNodeCollection nodes, bool enabledGrouping) {
			if (GetExistingCharSet().Length <= IndexPanelItemCharIndex)
				IndexPanelItemCharIndex = 0;
			string c = GetExistingCharSet()[IndexPanelItemCharIndex].ToString();
			TitleIndexNodeCollection ret = new TitleIndexNodeCollection();
			for (int i = 0; i < nodes.Count; i++) {
				if (enabledGrouping) {
					if (nodes[i].Text == c)
						ret.Add(nodes[i]);
				}
				else
					if (nodes[i].Text == c)
						ret.Add(nodes[i]);
			}
			return ret;
		}
		private List<string> GetFirstSymbols(TitleIndexNodeCollection nodes) {
			List<string> ret = new List<string>();
			foreach (TitleIndexNode node in nodes)
				if (node.Text != "")
					ret.Add(node.Text);
			return ret;
		}
		private int GetGroupIndex(object groupValue) {
			GroupValueComparer comparer = new GroupValueComparer(CultureInfo.GetCultureInfo("en-US"));
			for (int i = 0; i < SortedNodes.Count; i++) 
				if (comparer.Compare(GetGroupValue(SortedNodes[i]), groupValue) == 0)
					return i;
			return -1;
		}
		private Dictionary<int, string> GetStrWithIndexList(TitleIndexNodeCollection nodes) {
			Dictionary<int, string> ret = new Dictionary<int, string>();
			for(int i = 0; i < nodes.Count; i++)
				ret.Add(i, nodes[i].Text);
			return ret;
		}
		private string[] GetTextFiledValues(TitleIndexItemCollection items) {
			List<string> ret = new List<string>();
			foreach (TitleIndexItem item in items)
				ret.Add(item.Text);
			return ret.ToArray();
		}
		private TitleIndexNodeCollection GenerateItemHierarchy(Dictionary<string, int[]> titleNodePos, TitleIndexNodeCollection sourceNodes) {
			TitleIndexNodeCollection sortedCollection = new TitleIndexNodeCollection();
			foreach (string symbol in titleNodePos.Keys) {
				int[] positions = titleNodePos[symbol];
				TitleIndexNode titleNode = new TitleIndexNode(symbol.ToString());
				TitleIndexNodeCollection sortedNodesByLetter = GetItemsByPositions(positions, sourceNodes, titleNode);
				AddChildItemsToItem(sortedNodesByLetter, titleNode);
				sortedCollection.Add(titleNode);
			}
			return sortedCollection;
		}
		private TitleIndexNodeCollection GetItemsByPositions(int[] positions, TitleIndexNodeCollection sourceNodeCollection, TitleIndexNode parentNode) {
			TitleIndexNodeCollection ret = new TitleIndexNodeCollection(parentNode);
			for (int i = 0; i < positions.Length; i++)
				ret.Add(sourceNodeCollection[positions[i]]);
			return ret;
		}
		private TitleIndexNodeCollection GetSortedNodes() {
			TitleIndexNodeCollection ret = new TitleIndexNodeCollection();
			fIsGrouping = IsGrouping();
			if (fIsGrouping)
				ret.Assign(ArrangeItemsWithGrouping(Items));
			else
				ret.Assign(ArrangeItems(Items));
			fExistingCharSet = GetFirstSymbols(ret);
			if (IsPaging())
				ret = ArrangeItemsByAlphabetIndex(ret, fIsGrouping);
			fSpecialCharSet = GetSpecialCharSet();
			return ret;
		}
		private TitleIndexNodeCollection GetTitleIndexNodeCollection(TitleIndexItemCollection nodes) {
			TitleIndexNodeCollection ret = new TitleIndexNodeCollection();
			for (int i = 0; i < nodes.Count; i++) {
				TitleIndexNode node = new TitleIndexNode(nodes[i]);
				ret.Add(node);
			}
			return ret;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.TitleIndexCommonFormDesigner"; } }
	}
}
