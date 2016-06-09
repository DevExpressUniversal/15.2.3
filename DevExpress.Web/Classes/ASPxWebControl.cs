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
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;
using DevExpress.Utils;
using DevExpress.Web.Internal.InternalCheckBox;
#if ASP
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
#else
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	#if ASP
	using DevExpress.Web.Localization;
	#endif
	public delegate void ControlResolveEventInitiator(ControlResolveEventArgs args);
	public class ControlResolveEventArgs : EventArgs {
		private string controlID;
		private Control resolvedControl;
		public ControlResolveEventArgs(string controlID) {
			this.controlID = controlID;
		}
		public string ControlID { get { return controlID; } }
		public Control ResolvedControl { get { return resolvedControl; } set { resolvedControl = value; } }
	}
	[AutoFormatUrlPropertyClass]
	public abstract class ASPxWebControlBase : WebControl, IASPxWebControl, IUrlResolutionService {
		private const string fUseViewStateKey = "_!UseViewState";
		private bool isInEnsureChildControlsRecursive = false;
		private bool isInitialized = false;
		private bool isLoaded = false;
		private bool isPreRendered = false;
		private bool isRendering = false;
		private bool generateClientID = false;
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlBaseClientID")]
#endif
		public override string ClientID {
			get {
				if(MvcUtils.RenderMode == MvcRenderMode.RenderWithSimpleIDs)
					return ID;
				return GenerateClientID ? ClientIDHelper.GenerateClientID(this, base.ClientID) : base.ClientID;
			}
		}
		internal bool GenerateClientID { get { return generateClientID; } set { generateClientID = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlBaseControls")]
#endif
		public override ControlCollection Controls {
			get {
				if(!this.isInEnsureChildControlsRecursive)
					EnsureChildControls();
				return base.Controls;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Initialized {
			get { return isInitialized; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Loaded {
			get { return isLoaded; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool PreRendered {
			get { return isPreRendered; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsRendering {
			get { return isRendering; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ViewStateLoading {
			get { return false; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual bool DesignMode {
			get { return base.DesignMode; }
		}
		protected static BrowserInfo Browser { get { return RenderUtils.Browser; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsIE { get { return RenderUtils.IsIE; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsIE7 { get { return RenderUtils.IsIE7; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsIE8 { get { return RenderUtils.IsIE8; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsIEVersionLessThan7 { get { return RenderUtils.IsIEVersionLessThan7; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsNetscape { get { return RenderUtils.IsNetscape; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsMozilla { get { return RenderUtils.IsMozilla; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsFirefox { get { return RenderUtils.IsFirefox; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsFirefox3 { get { return RenderUtils.IsFirefox3; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsOpera { get { return RenderUtils.IsOpera; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsNetscapeFamily { get { return RenderUtils.IsNetscapeFamily; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsSafariFamily { get { return RenderUtils.IsSafariFamily; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsSafari { get { return RenderUtils.IsSafari; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsSafari3 { get { return RenderUtils.IsSafari3; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsSafariVersionNonLessThan3 { get { return RenderUtils.IsSafariVersionNonLessThan3; } }
		[Obsolete("This property is now obsolete. Use the Browser property instead.")]
		protected static bool IsChrome { get { return RenderUtils.IsChrome; } }
		public ASPxWebControlBase()
			: base() {
			if(IsMvcRender())
				ClientIDHelper.EnableClientIDGeneration(this);
		}
		public virtual new bool IsEnabled() {
			return base.IsEnabled;
		}
		public virtual bool IsVisible() {
			for(Control control = this; control != null; control = control.Parent) {
				if(!control.Visible)
					return false;
			}
			return true;
		}
		public virtual bool IsLoading() {
			return !Initialized || ViewStateLoading;
		}
		protected bool IsMvcRender() {
			return IsMvcRenderInternal();
		}
		protected static bool IsMvcRenderInternal() {
			return MvcUtils.RenderMode != MvcRenderMode.None;
		}
		protected bool IsMvcResourcesRender() {
			return MvcUtils.RenderMode == MvcRenderMode.RenderResources;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ClientIDHelper.UpdateClientIDMode(this);
			InitInternal();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.isLoaded = true;
		}
		protected override void OnPreRender(EventArgs e) {
			EnsurePreRender();
			this.isPreRendered = true;
			base.OnPreRender(e);
			EnsureChildControls();
			FinalizeCreateControlHierarchy();
		}
		protected virtual void EnsurePreRender() {
		}
		protected virtual void AfterRender() {
		}
		protected virtual void BeforeRender() {
			if(!PreRendered)
				EnsurePreRender();
			EnsureChildControls();
			if(!PreRendered)
				FinalizeCreateControlHierarchy(); 
			PrepareControlHierarchy();
		}
		protected virtual void RenderInternal(HtmlTextWriter writer) {
			if(HasRootTag())
				base.Render(writer);
			else
				RenderContents(writer);
		}
		protected override void Render(HtmlTextWriter writer) {
			BeginRendering();
			try {
				BeforeRender();
				RenderInternal(writer);
				AfterRender();
			} finally {
				EndRendering();
			}
		}
		protected internal virtual void SetInitialized(bool value) {
			this.isInitialized = value;
		}
		protected internal virtual void InitInternal() {
			SetInitialized(true);
		}
		public new string ResolveClientUrl(string relativeUrl) {
			if(MvcUtils.RenderMode != MvcRenderMode.None)
				return MvcUtils.ResolveClientUrl(relativeUrl);
			return base.ResolveClientUrl(relativeUrl);
		}
		protected virtual bool HasContent() {
			return true;
		}
		protected virtual bool HasRootTag() {
			return false;
		}
		protected virtual void ClearControlFields() {
		}
		protected virtual void CreateControlHierarchy() {
		}
		protected override void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected void EnsureChildControlsRecursive(Control control) {
			this.isInEnsureChildControlsRecursive = true;
			try {
				EnsureChildControlsRecursive(control, true);
			} finally {
				this.isInEnsureChildControlsRecursive = false;
			}
		}
		protected virtual void EnsureChildControlsRecursive(Control control, bool skipContentContainers) {
			RenderUtils.EnsureChildControlsRecursive(control, skipContentContainers);
		}
		protected virtual void FinalizeCreateControlHierarchy() {
		}
		protected virtual void PrepareControlHierarchy() {
		}
		protected internal void RecreateControlHierarchy() {
			ResetControlHierarchy();
			EnsureChildControls();
		}
		protected virtual internal void ResetControlHierarchy() {
			ChildControlsCreated = false;
		}
		protected internal void CompleteControlHierarchy() {
			EnsureChildControls();
		}
		protected override void CreateChildControls() {
			ClearControlFields();
			if(HasContent()) {
				try {
					CreateControlHierarchy();
					EnsureChildControlsRecursive(this);
				} catch {
					base.Controls.Clear();
					throw;
				}
			}
		}
		protected static void DataBindContainers(Control parent, bool bindContents, bool bindTemplates) {
			if(parent.HasControls()) {
				foreach(Control child in parent.Controls) {
					if(child is TemplateContainerBase) {
						if(bindTemplates)
							child.DataBind();
					} else {
						if(child is IContentContainer && bindContents)
							child.DataBind();
						else
							DataBindContainers(child, bindContents, bindTemplates);
					}
				}
			}
		}
		protected void BeginRendering() {
			this.isRendering = true;
		}
		protected void EndRendering() {
			this.isRendering = false;
		}
		public bool IsViewStateStoring() {
			return ViewStateUtils.GetBoolProperty(ViewState, fUseViewStateKey, false);
		}
		public void SetViewStateStoringFlag() {
			ViewStateUtils.SetBoolProperty(ViewState, fUseViewStateKey, false, true);
		}
		public void ResetViewStateStoringFlag() {
			ViewStateUtils.SetBoolProperty(ViewState, fUseViewStateKey, false, false);
		}
		protected string EncodeUnit(Unit unit) {
			return HtmlConvertor.EncodeUnit(unit);
		}
		void IASPxWebControl.EnsureChildControls() {
			EnsureChildControls();
		}
		void IASPxWebControl.PrepareControlHierarchy() {
			PrepareControlHierarchy();
		}
	}
	public interface IClientObjectOwner {
		string ClientInstanceName { get; set; }
		bool ClientEnabled { get; set; }
		bool ClientVisible { get; set; }
		Dictionary<string, object> JSProperties { get; }
		ClientSideEventsBase ClientSideEvents { get; }
	}
	public enum SyncSelectionMode { None, CurrentPathAndQuery, CurrentPath }
	[Designer("DevExpress.Web.Design.ASPxWebControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	[ControlBuilder(typeof(ThemableControlBuilder))]
	public abstract class ASPxWebControl : ASPxWebControlBase, INamingContainer,
		IPostBackDataHandler, IPostBackEventHandler, ICallbackEventHandler,
		IWebControlObject, IClientObjectOwner, ISkinOwner, IPostBackDataHandlerEx, IPostDataCollection, IHandleCallbackError {
		protected internal const string WebImagesResourcePath = "DevExpress.Web.Images.";
		protected internal const string WebScriptsResourcePath = "DevExpress.Web.Scripts.";
		protected internal const string UtilsScriptResourceName = WebScriptsResourcePath + "Utils.js";
		protected internal const string ClassesScriptResourceName = WebScriptsResourcePath + "Classes.js";
		protected internal const string MobileScriptResourceName = WebScriptsResourcePath + "Mobile.js";
		protected internal const string DebugScriptResourceName = WebScriptsResourcePath + "Debug.js";
		protected internal const string PopupUtilsScriptResourceName = WebScriptsResourcePath + "PopupUtils.js";
		protected internal const string DialogUtilsScriptResourceName = WebScriptsResourcePath + "DialogUtils.js";
		protected internal const string DragAndDropUtilsScriptResourceName = WebScriptsResourcePath + "DragAndDropUtils.js";
		protected internal const string RelatedControlManagerScriptResourceName = WebScriptsResourcePath + "RelatedControlManager.js";
		protected internal const string DateFormatterScriptResourceName = WebScriptsResourcePath + "DateFormatter.js";
		protected internal const string FormatterScriptResourceName = WebScriptsResourcePath + "Formatter.js";
		protected internal const string StateControllerScriptResourceName = WebScriptsResourcePath + "StateController.js";
		protected internal const string AnimationScriptResourceName = WebScriptsResourcePath + "Animation.js";
		protected internal const string ScrollUtilsScriptResourceName = WebScriptsResourcePath + "ScrollUtils.js";
		protected internal const string ControlResizeManagerScriptResourceName = WebScriptsResourcePath + "ControlResizeManager.js";
		protected internal const string TableScrollUtilsScriptResourceName = WebScriptsResourcePath + "TableScrollUtils.js";
		protected internal const string ImageControlUtilsScriptResourceName = WebScriptsResourcePath + "ImageControlUtils.js";
		protected internal const string WebCssResourcePath = "DevExpress.Web.Css.";
		protected internal const string WebSystemCssResourceName = WebCssResourcePath + "System.css";
		protected internal const string WebSystemHtml5CssResourceName = WebCssResourcePath + "SystemHtml5.css";
		protected internal const string WebSystemDesignModeCssResourceName = WebCssResourcePath + "SystemDesignMode.css";
		protected internal const string WebClientUIControlSystemDesignModeCssResourceName = WebCssResourcePath + "WebClientUIControlSystemDesignMode.css";
		protected internal const string WebDefaultCssResourceName = WebCssResourcePath + "Default.css";
		protected internal const string WebSpriteCssResourceName = WebCssResourcePath + "Sprite.css";
		protected internal const string SSLSecureBlankUrlResourceName = "DevExpress.Web.Classes.SSLSecureBlank.htm";
		protected internal const string TrialMessageCloseImageName = WebImagesResourcePath + "Close.png";
		protected internal const string ImageScriptResourceName = WebScriptsResourcePath + "Editors.Image.js";
		private const string FakeHiddenInputWasRenderedKey = "aspxFakeHiddenInputWasRendered";
		private const string GeneralScriptBlockKey = "GeneralScript";
		protected internal const string FocusedControlIDKey = "FocusedControlID";
		protected internal const string ControlClickHandlerName = "ASPx.CClick('{0}', event)";
		protected internal const string FireDefaultButtonHandlerName = "return ASPx.FireDefaultButton(event, '{0}');";
		protected const string ShortClientLocalVariableName = "dxo";
		private const char CustomDataCallbackPrefix = 'd';
		private const char CommonCallbackPrefix = 'c';
		private const char CallbackSeparator = ':';
		private enum CallbackType { Common, Data };
		protected const bool DefaultEnableCallbackAnimation = false;
		protected const bool DefaultEnableSlideCallbackAnimation = false;
		private static object redirectOnCallbackKey = new object();
		protected internal const string ErrorMessageQueryParamName = "DXCallbackErrorMessage";
		private bool isAutoFormatPreview = false;
		private int propertyChangedLockCount = 0;
		private bool viewStateLoading = false;
		private bool viewStateLoaded = false;
		private ASPxWebControl ownerControl = null;
		private bool savedIsEnabled = true;
		private bool isClientStateLoaded = false;
		private bool isInDataBind = false;
		private bool isDesignMode = false;
		private static NameValueCollection defaultPostDataCollection = new NameValueCollection();
		private NameValueCollection postDataCollection;
		private Hashtable clientObjectState;
		private ISkinOwner parentSkinOwner = null;
		private ImagesBase images;
		private StylesBase styles;
		private ImagesBase parentImages;
		private StylesBase parentStyles;
		private ImagesBase renderImages;
		private StylesBase renderStyles;
		private ClientSideEventsBase clientSideEvents = null;
		private Dictionary<string, object> jsProperties = new Dictionary<string, object>();
		private static EmptyImageProperties emptyImage = new EmptyImageProperties();
		private static ShadowImageProperties shadowImage = new ShadowImageProperties();
		private LoadingPanelControl loadingPanelControl = null;
		private WebControl loadingDiv = null;
		private string startupScript = string.Empty;
		private bool postDataLoaded = false;
		private bool loadPostDataResult = false;
		private bool styleSheetsRegistered = false;
		private SettingsLoadingPanel settingsLoadingPanel = null;
		private ExpandoAttributes expandoAttributes = new ExpandoAttributes();
		Dictionary<ComplexKey, AppearanceStyleBase> stylesCache = new Dictionary<ComplexKey, AppearanceStyleBase>(new ComplexKey.KeyComparer());
		private string callbackResult;
		private CallbackType callbackType;
		private int callbackID;
		protected static readonly object EventCustomJsProperties = new object();
		protected static readonly object EventCustomDataCallback = new object();
		protected static readonly object EventBeforeGetCallbackResult = new object();
		protected static readonly object EventClientLayout = new object();
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlCallbackError")]
#endif
		public static event EventHandler CallbackError;
		protected ASPxWebControl(ASPxWebControl ownerControl)
			: this() {
			this.ownerControl = ownerControl;
		}
		public ASPxWebControl()
			: base() {
			styles = CreateStyles();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BorderColor {
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyle BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit BorderWidth {
			get { return base.BorderWidth; }
			set { base.BorderWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlCursor"),
#endif
		Category("Appearance"), DefaultValue(""), Localizable(false),
		TypeConverter("DevExpress.Web.Design.CursorConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatEnable]
		public virtual string Cursor {
			get { return ((AppearanceStyleBase)ControlStyle).Cursor; }
			set { ((AppearanceStyleBase)ControlStyle).Cursor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlBackgroundImage"),
#endif
	   Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage BackgroundImage {
			get { return ((AppearanceStyleBase)ControlStyle).BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlBorder"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper Border {
			get { return ((AppearanceStyleBase)ControlStyle).Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlBorderLeft"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderLeft {
			get { return ((AppearanceStyleBase)ControlStyle).BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlBorderTop"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderTop {
			get { return ((AppearanceStyleBase)ControlStyle).BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlBorderRight"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderRight {
			get { return ((AppearanceStyleBase)ControlStyle).BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlBorderBottom"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderBottom {
			get { return ((AppearanceStyleBase)ControlStyle).BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlEnableDefaultAppearance"),
#endif
		Obsolete("This property is now obsolete. Use the corresponding style settings to override control elements' appearance."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public virtual bool EnableDefaultAppearance {
			get { return true; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlEnabled"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public override bool Enabled {
			get { return base.Enabled; }
			set {
				base.Enabled = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlEncodeHtml"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public virtual bool EncodeHtml {
			get { return GetBoolProperty("EncodeHtml", true); }
			set { SetBoolProperty("EncodeHtml", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlCssPostfix"),
#endif
		Category("Styles"), DefaultValue(""), AutoFormatEnable, Localizable(false)]
		public virtual string CssPostfix {
			get { return StylesInternal.CssPostfix; }
			set { StylesInternal.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlCssFilePath"),
#endif
		Category("Styles"), DefaultValue(""), UrlProperty,
		AutoFormatEnable, AutoFormatCssUrlProperty, AutoFormatUrlProperty, Localizable(false),
		Editor("DevExpress.Web.Design.CssFileNameEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string CssFilePath {
			get { return StylesInternal.CssFilePath; }
			set { StylesInternal.CssFilePath = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlDisabledStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DisabledStyle DisabledStyle {
			get { return StylesInternal.DisabledInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlTheme"),
#endif
		Category("Styles"), DefaultValue(""), AutoFormatDisable, Localizable(false),
		Themeable(false), TypeConverter("DevExpress.Web.Design.ThemeTypeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),
		EditorAttribute("DevExpress.Web.Design.ThemeUITypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string Theme {
			get { return StylesInternal.Theme; }
			set { this.StylesInternal.Theme = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlWidth"),
#endif
		AutoFormatEnable, AutoFormatCannotBeEmpty]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlHeight"),
#endif
		AutoFormatEnable, AutoFormatCannotBeEmpty]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlEmptyImage"),
#endif
		Obsolete("This property is now obsolete."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public static EmptyImageProperties EmptyImage {
			get { return emptyImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxWebControlShadowImage"),
#endif
		Obsolete("This property is now obsolete."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public static ShadowImageProperties ShadowImage {
			get { return shadowImage; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ISkinOwner ParentSkinOwner {
			get { return parentSkinOwner; }
			set { parentSkinOwner = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ImagesBase ParentImages {
			get { return parentImages; }
			set { parentImages = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual StylesBase ParentStyles {
			get { return parentStyles; }
			set { parentStyles = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlGlobalStyleSheetTheme")]
#endif
		public static string GlobalStyleSheetTheme {
			get {
				string resultTheme = HttpUtils.GetContextValue<string>("DXStyleSheetTheme", null);
				return !string.IsNullOrEmpty(resultTheme) ? resultTheme : ConfigurationSettings.StyleSheetTheme;
			}
			set { HttpUtils.SetContextValue<string>("DXStyleSheetTheme", value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlGlobalTheme")]
#endif
		public static string GlobalTheme {
			get {
				string resultTheme = HttpUtils.GetContextValue<string>("DXTheme", null);
				return !string.IsNullOrEmpty(resultTheme) ? resultTheme : ConfigurationSettings.Theme;
			}
			set { HttpUtils.SetContextValue<string>("DXTheme", value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlGlobalRightToLeft")]
#endif
		public static DefaultBoolean GlobalRightToLeft {
			get {
				DefaultBoolean contextValue = HttpUtils.GetContextValue<DefaultBoolean>("DXRightToLeft", DefaultBoolean.Default);
				if(contextValue != DefaultBoolean.Default)
					return contextValue;
				if(ConfigurationSettings.RightToLeft)
					return DefaultBoolean.True;
				return DefaultBoolean.Default;
			}
			set { HttpUtils.SetContextValue<DefaultBoolean>("DXRightToLeft", value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebControlGlobalEmbedRequiredClientLibraries")]
#endif
		public static bool GlobalEmbedRequiredClientLibraries {
			get {
				return HttpUtils.GetContextValue<bool>("DXEmbedRequiredClientLibraries", false);
			}
			set { HttpUtils.SetContextValue<bool>("DXEmbedRequiredClientLibraries", value); }
		}
		protected virtual StylesBase RenderStylesInternal {
			get {
				if(ParentStyles == null)
					return StylesInternal;
				if(this.renderStyles == null || !UseCachedObjects()) {
					this.renderStyles = CreateStyles();
					this.renderStyles.CopyFrom(ParentStyles);
					this.renderStyles.CopyFrom(StylesInternal);
				}
				return this.renderStyles;
			}
		}
		protected virtual ImagesBase RenderImagesInternal {
			get {
				if(ParentImages == null)
					return ImagesInternal;
				if(this.renderImages == null || !UseCachedObjects()) {
					this.renderImages = CreateImages();
					this.renderImages.CopyFrom(ParentImages);
					this.renderImages.CopyFrom(ImagesInternal);
				}
				return this.renderImages;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ViewStateLoading {
			get { return viewStateLoading; }
		}
		private bool ViewStateLoaded {
			get { return viewStateLoaded; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAutoFormatPreview {
			get { return (MainOwnerControl != null) ? MainOwnerControl.IsAutoFormatPreview : this.isAutoFormatPreview; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsCallback {
			get { return (Request != null) && HttpUtils.GetValueFromRequest(Request, RenderUtils.CallbackControlIDParamName) == UniqueID; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HttpRequest Request {
			get { return (HttpContext.Current != null) ? HttpContext.Current.Request : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HttpResponse Response {
			get { return (HttpContext.Current != null) ? HttpContext.Current.Response : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DesignMode {
			get { return isDesignMode || ((MainOwnerControl != null) ? MainOwnerControl.DesignMode : base.DesignMode); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Control BindingContainer {
			get {
				Control container = base.BindingContainer;
				var pageControl = container as DevExpress.Web.ASPxPageControl;
				if(pageControl != null && pageControl.IsNotBindingContainer)
					return pageControl.BindingContainer;
				return container;
			}
		}
		[Category("Appearance"), DefaultValue(false)]
		protected virtual bool Native {
			get { return StylesInternal.Native; }
			set { StylesInternal.Native = value; }
		}
		protected virtual string ImageFolderInternal {
			get { return ImagesInternal.ImageFolder; }
			set { ImagesInternal.ImageFolder = value; }
		}
		protected internal virtual string SpriteImageUrlInternal {
			get { return ImagesInternal.SpriteImageUrl; }
			set { ImagesInternal.SpriteImageUrl = value; }
		}
		protected virtual string SpriteCssFilePathInternal {
			get { return ImagesInternal.SpriteCssFilePath; }
			set { ImagesInternal.SpriteCssFilePath = value; }
		}
		protected virtual ImagesBase ImagesInternal {
			get {
				if(images == null)
					images = CreateImages();
				return images;
			}
		}
		protected virtual StylesBase StylesInternal {
			get {
				if(styles == null)
					styles = CreateStyles();
				return styles;
			}
		}
		protected string StartupScript {
			get { return startupScript; }
			set { startupScript = value; }
		}
		protected virtual bool AutoPostBackInternal {
			get { return GetBoolProperty("AutoPostBack", false); }
			set { SetBoolProperty("AutoPostBack", false, value); }
		}
		protected internal virtual ClientSideEventsBase ClientSideEventsInternal {
			get {
				if(clientSideEvents == null)
					clientSideEvents = CreateClientSideEvents();
				return clientSideEvents;
			}
		}
		protected virtual string ClientInstanceNameInternal {
			get { return GetStringProperty("ClientInstanceNameInternal", ""); }
			set { SetStringProperty("ClientInstanceNameInternal", "", value); }
		}
		protected virtual bool EnableClientSideAPIInternal {
			get { return GetBoolProperty("EnableClientSideAPIInternal", false); }
			set { SetBoolProperty("EnableClientSideAPIInternal", false, value); }
		}
		protected virtual bool EnableHierarchyRecreationInternal {
			get { return GetBoolProperty("EnableHierarchyRecreationInternal", true); }
			set { SetBoolProperty("EnableHierarchyRecreationInternal", true, value); }
		}
		protected bool EnableCallBacksInternal {
			get { return GetBoolProperty("EnableCallBacksInternal", false); }
			set {
				SetBoolProperty("EnableCallBacksInternal", false, value);
				LayoutChanged();
			}
		}
		protected bool EnableCallbackAnimationInternal {
			get { return GetBoolProperty("EnableCallBacksAnimationInternal", DefaultEnableCallbackAnimation); }
			set { SetBoolProperty("EnableCallBacksAnimationInternal", DefaultEnableCallbackAnimation, value); }
		}
		protected bool EnableCallbackCompressionInternal {
			get { return GetBoolProperty("EnableCallbackCompressionInternal", true); }
			set { SetBoolProperty("EnableCallbackCompressionInternal", true, value); }
		}
		protected AutoBoolean EnableSwipeGesturesInternal {
			get { return (AutoBoolean)GetEnumProperty("EnableSwipeGesturesInternal", AutoBoolean.Auto); }
			set { SetEnumProperty("EnableSwipeGesturesInternal", AutoBoolean.Auto, value); }
		}
		protected bool EnableHotTrackInternal {
			get { return GetBoolProperty("EnableHotTrackInternal", true); }
			set { SetBoolProperty("EnableHotTrackInternal", true, value); }
		}
		protected bool EnableSlideCallbackAnimationInternal {
			get { return GetBoolProperty("EnableSlideCallbackAnimationInternal", DefaultEnableSlideCallbackAnimation); }
			set { SetBoolProperty("EnableSlideCallbackAnimationInternal", DefaultEnableSlideCallbackAnimation, value); }
		}
		protected bool ClientVisibleInternal {
			get { return GetBoolProperty("ClientVisibleInternal", true); }
			set { SetBoolProperty("ClientVisibleInternal", true, value); }
		}
		protected bool ClientEnabledInternal {
			get { return GetBoolProperty("ClientEnabledInternal", true); }
			set { SetBoolProperty("ClientEnabledInternal", true, value); }
		}
		protected virtual Dictionary<string, object> JSPropertiesInternal {
			get { return jsProperties; }
		}
		protected LinkStyle LinkStyle {
			get { return StylesInternal.LinkInternal; }
		}
		protected bool HasLinkStyle {
			get { return StylesInternal.HasLink; }
		}
		protected SettingsLoadingPanel SettingsLoadingPanel {
			get {
				if(settingsLoadingPanel == null)
					settingsLoadingPanel = CreateSettingsLoadingPanel();
				return settingsLoadingPanel;
			}
		}
		protected ImageProperties LoadingPanelImage {
			get { return ImagesInternal.LoadingPanel; }
		}
		protected LoadingPanelStyle LoadingPanelStyle {
			get { return StylesInternal.LoadingPanelInternal; }
		}
		protected LoadingDivStyle LoadingDivStyle {
			get { return StylesInternal.LoadingDivInternal; }
		}
		protected bool SyncSelectionWithCurrentPath {
			get { return SyncSelectionMode == SyncSelectionMode.CurrentPathAndQuery; }
			set { SyncSelectionMode = value ? SyncSelectionMode.CurrentPathAndQuery : SyncSelectionMode.None; }
		}
		protected SyncSelectionMode SyncSelectionMode {
			get { return (SyncSelectionMode)GetEnumProperty("SyncSelectionMode", SyncSelectionMode.CurrentPathAndQuery); }
			set { SetEnumProperty("SyncSelectionMode", SyncSelectionMode.CurrentPathAndQuery, value); }
		}
		protected bool SaveStateToCookies {
			get { return GetBoolProperty("SaveStateToCookies", false); }
			set { SetBoolProperty("SaveStateToCookies", false, value); }
		}
		protected bool AccessibilityCompliantInternal {
			get { return StylesInternal.AccessibilityCompliant; }
			set {
				if(StylesInternal.AccessibilityCompliant != value) {
					StylesInternal.AccessibilityCompliant = value;
					LayoutChanged();
				}
			}
		}
		protected DefaultBoolean RightToLeftInternal {
			get { return StylesInternal.RightToLeft; }
			set {
				if(value == RightToLeftInternal)
					return;
				StylesInternal.RightToLeft = value;
				LayoutChanged();
			}
		}
		protected virtual bool IsStateSavedToCookies {
			get { return !DesignMode && SaveStateToCookies; }
		}
		protected virtual bool IsClientLayoutExists { get { return Events[EventClientLayout] != null; } }
		const string CallbackErrorMessageContextKey = "31a09531b4a3465dbe98ab48501e2838";
		string CallbackErrorMessage {
			get { return HttpUtils.GetContextValue<string>(CallbackErrorMessageContextKey, null); }
			set { HttpUtils.SetContextValue<string>(CallbackErrorMessageContextKey, value); }
		}
		static readonly object CallbackErrorMessageInternalContextKey = new object();
		protected internal static string CallbackErrorMessageInternal {
			get {
				string result = HttpUtils.GetContextValue<string>(CallbackErrorMessageInternalContextKey, null);
				if(string.IsNullOrEmpty(result)) {
					if(HttpContext.Current != null) {
						Exception exception = HttpContext.Current.Server.GetLastError();
						if(exception != null) {
							if(exception.InnerException != null)
								exception = exception.InnerException;
							return OnCallbackExceptionInternal(exception);
						}
					}
					return string.Empty;
				}
				return result;
			}
			set {
				HttpUtils.SetContextValue<string>(CallbackErrorMessageInternalContextKey, value);
			}
		}
		protected virtual bool PostDataLoaded {
			get { return postDataLoaded; }
		}
		protected virtual bool IsErrorOnCallback {
			get { return CallbackErrorMessage != null; }
		}
		protected string SaveStateToCookiesID {
			get { return GetStringProperty("SaveStateToCookiesID", ""); }
			set { SetStringProperty("SaveStateToCookiesID", "", value); }
		}
		protected DefaultBoolean RenderIFrameForPopupElementsInternal {
			get { return (DefaultBoolean)GetEnumProperty("RenderIFrameForPopupElementsInternal", DefaultBoolean.Default); }
			set { SetEnumProperty("RenderIFrameForPopupElementsInternal", DefaultBoolean.Default, value); }
		}
		protected ASPxWebControl OwnerControl {
			get { return ownerControl; }
			set { ownerControl = value; }
		}
		protected ASPxWebControl MainOwnerControl {
			get {
				ASPxWebControl control = OwnerControl;
				while(control != null && control.OwnerControl != null)
					control = control.OwnerControl;
				return control;
			}
		}
		protected virtual NameValueCollection PostDataCollection {
			get {
				if(postDataCollection != null)
					return postDataCollection;
				var request = HttpUtils.GetRequest();
				if(request != null) {
					NameValueCollection result = new NameValueCollection();
					result.Add(request.Form);
					result.Add(request.QueryString);
					return result;
				}
				return GetDefaultPostDataCollection();
			}
		}
		protected virtual NameValueCollection GetDefaultPostDataCollection(){
			return defaultPostDataCollection;
		}
		protected Hashtable ClientObjectState {
			get {
				if(clientObjectState == null)
					clientObjectState = LoadClientObjectState(PostDataCollection);
				return clientObjectState;
			}
		}
		protected LoadingPanelControl LoadingPanelControl {
			get { return loadingPanelControl; }
		}
		protected WebControl LoadingDiv {
			get { return loadingDiv; }
		}
		protected virtual string FocusedControlIDValue {
			get { return ClientID; }
		}
		public static void SetIECompatibilityMode(int IEVersion) {
			Browser.SetIECompatibilityMode(IEVersion);
		}
		public static void SetIECompatibilityModeEdge() {
			Browser.SetIECompatibilityModeEdge();
		}
		[Obsolete("Use another overload of this method instead.")]
		public static void SetIECompatibilityMode(int IEVersion, Control pageOrMasterPage) {
			SetIECompatibilityMode(IEVersion);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static void SetIECompatibilityModeEdge(Control pageOrMasterPage) {
			SetIECompatibilityModeEdge();
		}
		public override void Focus() {
			HttpUtils.SetContextValue(FocusedControlIDKey, FocusedControlIDValue);
		}
		protected virtual bool IsFocused() {
			return HttpUtils.GetContextValue(FocusedControlIDKey, string.Empty) == FocusedControlIDValue;
		}
		protected void AssignAttributesTo(WebControl destination) {
			AssignAttributesTo(destination, false);
		}
		protected virtual void AssignAttributesTo(WebControl destination, bool skipID) {
			RenderUtils.AssignAttributes(this, destination, skipID);
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			if(NeedCreateHierarchyOnInit() && !DesignMode)
				EnsureChildControls(); 
		}
		protected internal void SetDesignMode(bool value) {
			this.isDesignMode = value;
		}
		protected internal void ApplyTheme(string themeName) {
			ApplyTheme(themeName, true);
		}
		protected internal void ApplyTheme(string themeName, bool ignoreDesignMode) {
			if(!string.IsNullOrEmpty(themeName) && EnableTheming) {
				if(!DesignMode || (DesignMode && !ignoreDesignMode))
					ThemesHelper.ApplyTheme(this, themeName, DesignMode);
			}
		}
		protected override void OnInit(EventArgs e) {
			AssertModuleRegistration();
			ApplyThemeInternal();
			if(CanCreateControlForCssResourcesInHeader())
				Page.InitComplete += new EventHandler(OnInitComplete);
			base.OnInit(e);
		}
		protected void OnInitComplete(object sender, EventArgs e) {
			if(CanCreateControlForCssResourcesInHeader())
				CreateControlForCssResourcesInHeader();
		}
		void AssertModuleRegistration() {
#if !DEBUG
			if(Context != null && Page != null && !DesignMode && HttpContext.Current != null) {
				if(HttpContext.Current.Application[ResourceManager.HandlerRegistrationFlag] == null || !(bool)HttpContext.Current.Application[ResourceManager.HandlerRegistrationFlag])
					throw new Exception(StringResources.Error_ModuleIsNotRegistered);
			}
#endif
		}
		protected override void OnLoad(EventArgs e) {
			if(CanLoadPostDataOnLoad() && CanLoadPostData())
				LoadPostDataInternal(PostDataCollection, true);
			if(Request == null || (NeedLoadClientState() && !this.isClientStateLoaded))
				LoadClientStateInternal();
			base.OnLoad(e);
		}
		protected override void EnsurePreRender() {
			ValidateIsEnabled();
			base.EnsurePreRender();
		}
		protected void OnPreRenderComplete(object sender, EventArgs e) {
			 RenderCssResourcesInHeader();
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(Visible && HasContent()) {
				if(!DesignMode) {
					RegisterPostBackScripts();
					if(!ASPxScriptManager.Active)
						RegisterClientIncludeScripts();
				}
				RegisterLinkStyles();
				if(!ASPxStyleSheetManager.Active) {
					RegisterStyleSheets();
				}
				if(CanCreateControlForCssResourcesInHeader())
					Page.PreRenderComplete += new EventHandler(OnPreRenderComplete);
			}
		}
		public override void DataBind() {
			this.isInDataBind = true;
			try {
				DataBindInternal();
			} finally {
				this.isInDataBind = false;
			}
		}
		protected virtual void DataBindInternal() {
			this.OnDataBinding(EventArgs.Empty);
			EnsureChildControls();
			DataBindContainers(this, true, true);
		}
		public bool IsNativeRender() {
			return IsNative() && IsNativeSupported();
		}
		protected override void AfterRender() {
			SaveClientStateInternal();
		}
		private const bool ForceEventValidationFieldCreation = true;
		protected override void BeforeRender() {
			AddDirectionAttributesIfRequired();
			base.BeforeRender();
			if(Page != null && NeedVerifyRenderingInServerForm() && !IsMvcRender())
				Page.VerifyRenderingInServerForm(this);
			if(IsScriptEnabled() && Page != null && IsCallBacksEnabled() && !IsMvcRender())
				RenderUtils.GetPostBackEventReference(Page, new PostBackOptions(this), ForceEventValidationFieldCreation);
			if(!DesignMode && Visible && HasContent()) {
				RegisterClientIncludeScripts();
				RegisterClientScriptBlocks();
				RegisterClientScripts();
			}
		}
		protected void AddDirectionAttributesIfRequired() {
			if(IsRightToLeft())
				AddRightToLeftAttributes();
			if(RightToLeftInternal == DefaultBoolean.False)
				AddLeftToRightAttributes();
		}
		protected virtual void AddRightToLeftAttributes() {
			Attributes["dir"] = "rtl";
		}
		protected virtual void AddLeftToRightAttributes() {
			Attributes["dir"] = "ltr";
		}
		protected override void ClearControlFields() {
			ClearLoadingPanel();
			ClearLoadingDiv();
		}
		protected override void CreateControlHierarchy() {
			CreateLoadingPanel();
			CreateLoadingDiv();
		}
		protected override void PrepareControlHierarchy() {
			PrepareLoadingPanel();
			PrepareLoadingDiv();
			if(DesignMode)
				RenderUtils.AppendDefaultDXClassName(this, GetCssClassNamePrefix("DesignMode"));
		}
		protected override void RenderInternal(HtmlTextWriter writer) {
			if(!DesignMode)
				RenderHiddenFields(writer);
			if((!IsMvcRender() && !DesignMode && !ASPxScriptManager.Active)
				|| (IsMvcRender() && (MvcUtils.RenderScriptsCalled || RenderUtils.IsAnyCallback(Page)))) {
				RenderIncludeScripts(writer);
				RenderScriptBlocks(writer);
			}
			if(!IsMvcRender() && !ASPxStyleSheetManager.Active)
				RenderStyleSheet(writer);
			base.RenderInternal(writer);
			if(!DesignMode)
				RenderStartupScripts(writer);
		}
		protected override void CreateChildControls() {
			this.savedIsEnabled = IsEnabled();
			if(CanLoadPostDataOnCreateControls() && CanLoadPostData())
				LoadPostDataInternal(PostDataCollection, false);
			if(!DesignMode && SyncSelectionMode != SyncSelectionMode.None)
				SelectCurrentPath(SyncSelectionMode == SyncSelectionMode.CurrentPath);
			base.CreateChildControls();
			if(!this.isInDataBind && BindContainersOnCreate())
				DataBindContainers(this, false, !IsViewStateStoring());
		}
		protected virtual bool BindContainersOnCreate() {
			return true;
		}
		protected virtual Control CreateUserControl(string virtualPath) {
			if(Page == null)
				return null;
			return Page.LoadControl(virtualPath);
		}
		protected virtual void PrepareUserControl(Control userControl, Control parent, string id, bool builtInUserControl) {
			if(builtInUserControl && userControl is UserControl && Page != null)
				((UserControl)userControl).AppRelativeVirtualPath = Page.AppRelativeVirtualPath;
			userControl.ID = id;
			parent.Controls.Add(userControl);
		}
		protected virtual bool NeedCreateHierarchyOnInit() {
			return false;
		}
		protected virtual void RenderIncludeScripts(HtmlTextWriter writer) {
			ResourceManager.RenderScriptResources(Page, writer);
		}
		protected virtual void RenderScriptBlocks(HtmlTextWriter writer) {
			ResourceManager.RenderScriptBlocks(Page, writer);
		}
		protected void RenderHiddenFields(HtmlTextWriter writer) {
			if(Browser.IsIE && RenderResourcesExist() && !HttpUtils.GetContextValue(FakeHiddenInputWasRenderedKey, false)) {
				RenderFakeHiddenField(writer);
				HttpUtils.SetContextValue(FakeHiddenInputWasRenderedKey, true);
			}
			else if(IsAutoFormatPreview && MainOwnerControl == null) 
				RenderFakeHiddenField(writer);
		}
		bool RenderResourcesExist() {
			return ResourceManager.ScriptBlocksRegistrator.ScriptBlocksForRender.Count > 0 ||
				ResourceManager.ScriptRegistrator.ResourcesForRender.Count > 0 ||
				ResourceManager.CssRegistrator.ResourcesForRender.Count > 0;
		}
		protected void RenderFakeHiddenField(HtmlTextWriter writer) {
			writer.Write("<input type=\"hidden\"/>");
		}
		protected void RenderStartupScripts(HtmlTextWriter writer) {
			if(!string.IsNullOrEmpty(StartupScript))
				RenderUtils.WriteScriptHtml(writer, StartupScript);
		}
		protected virtual bool IsStyleSheetsRegistered() {
			return this.styleSheetsRegistered;
		}
		protected virtual void RenderStyleSheet(HtmlTextWriter writer) {
			if(HasContent() && !CanCreateControlForCssResourcesInHeader() && !CanRenderStyleSheetViaDesigner()) {
				if(!IsStyleSheetsRegistered() || DesignMode)
					RegisterStyleSheets();
				bool renderAsStyleImports = DesignMode && !IsAutoFormatPreview;
				ResourceManager.RenderCssResources(Page, writer, renderAsStyleImports);
			}
		}
		protected internal virtual bool NeedRenderIFrameBehindPopupElement() {
			if(DesignMode)
				return false;
			else
				return RenderIFrameForPopupElementsInternal == DefaultBoolean.Default ? false : RenderIFrameForPopupElementsInternal == DefaultBoolean.True;
		}
		protected virtual bool NeedVerifyRenderingInServerForm() {
			return true;
		}
		protected virtual bool HasLoadingPanel() {
			return IsCallBacksEnabled();
		}
		protected virtual bool HasLoadingDiv() {
			return false;
		}
		protected bool IsLoadingPanelEmpty() {
			return (!SettingsLoadingPanel.ShowImage && string.IsNullOrEmpty(SettingsLoadingPanel.Text));
		}
		protected virtual string GetLoadingPanelID() {
			return "LP";
		}
		protected virtual string GetLoadingDivID() {
			return "LD";
		}
		protected virtual bool LoadingPanelHasAbsolutePosition() {
			return true;
		}
		protected virtual TableCell CreateLoadingPanelTemplateCell(TableRow parent) {
			return null;
		}
		void ClearLoadingPanel() {
			this.loadingPanelControl = null;
		}
		void ClearLoadingDiv() {
			this.loadingDiv = null;
		}
		void CreateLoadingPanel() {
			if(HasLoadingPanel() && !IsLoadingPanelEmpty() && SettingsLoadingPanel.Enabled)
				this.loadingPanelControl = CreateLoadingPanelInternal(this);
		}
		protected internal virtual LoadingPanelControl CreateLoadingPanelInternal(WebControl parent) {
			LoadingPanelControl loadingPanelControl = new LoadingPanelControl(IsRightToLeft());
			loadingPanelControl.Settings = SettingsLoadingPanel;
			loadingPanelControl.TemplateCreateDelegate = CreateLoadingPanelTemplateCell;
			parent.Controls.Add(loadingPanelControl);
			return loadingPanelControl;
		}
		void CreateLoadingDiv() {
			if(HasLoadingDiv() && SettingsLoadingPanel.Enabled)
				this.loadingDiv = CreateLoadingDiv(this);
		}
		protected internal virtual WebControl CreateLoadingDiv(WebControl parent) {
			WebControl loadingDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(loadingDiv);
			return loadingDiv;
		}
		void PrepareLoadingPanel() {
			if(LoadingPanelControl != null)
				PrepareLoadingPanel(LoadingPanelControl);
		}
		protected internal virtual void PrepareLoadingPanel(LoadingPanelControl loadingPanel) {
			loadingPanel.ID = GetLoadingPanelID();
			loadingPanel.Image = GetLoadingPanelImage();
			loadingPanel.Style = GetLoadingPanelStyle();
			loadingPanel.Paddings = GetLoadingPanelPaddings();
			loadingPanel.ImageSpacing = GetLoadingPanelImageSpacing();
			loadingPanel.HasAbsolutePosition = LoadingPanelHasAbsolutePosition();
		}
		void PrepareLoadingDiv() {
			if(LoadingDiv != null)
				PrepareLoadingDiv(LoadingDiv);
		}
		protected internal virtual void PrepareLoadingDiv(WebControl loadingDiv) {
			loadingDiv.ID = GetLoadingDivID();
			GetLoadingDivStyle().AssignToControl(loadingDiv);
			RenderUtils.AppendFullTransparentCssClass(loadingDiv);
			if(!DesignMode) {
				loadingDiv.Style.Add("left", "0px");
				loadingDiv.Style.Add("top", "0px");
				loadingDiv.Style.Add("z-index", RenderUtils.LoadingDivZIndex.ToString());
				loadingDiv.Style.Add("display", "none");
				loadingDiv.Style.Add("position", "absolute");
				if(IsSwipeGesturesEnabled())
					RenderUtils.AppendMSTouchDraggableClassNameIfRequired(loadingDiv);
			} else
				loadingDiv.Visible = false;
		}
		protected void ValidateIsEnabled() {
			bool enabled = IsEnabled();
			if(enabled != this.savedIsEnabled) {
				this.savedIsEnabled = enabled;
				ResetControlHierarchy();
			}
		}
		public string HtmlEncode(string text) {
			return EncodeHtml ? HttpUtility.HtmlEncode(text) : text;
		}
		public string HtmlEncode(string text, bool isInput) {
			if(isInput)
				return EncodeHtml ? text : HttpUtility.HtmlDecode(text);
			else
				return EncodeHtml ? HttpUtility.HtmlEncode(text) : text;
		}
		public static void RedirectOnCallback(string url) {
			HttpUtils.SetContextValue(redirectOnCallbackKey, url);
		}
		public static string GetCallbackErrorMessage() {
			return HttpContext.Current.Request.QueryString[ErrorMessageQueryParamName];
		}
		protected internal static string ErrorQueryString(string error) {
			return string.Format("?{0}={1}", ASPxWebControl.ErrorMessageQueryParamName, HttpUtility.UrlEncode(error));
		}
		public static void MakeResponseCompressed() {
			HttpUtils.MakeResponseCompressed(false);
		}
		public static void MakeResponseCompressed(bool applyToIE6) {
			HttpUtils.MakeResponseCompressed(false, applyToIE6);
		}
		public override bool IsLoading() {
			return base.IsLoading() && !IsMvcRender();
		}
		public bool IsAccessibilityCompliantRender() {
			return IsAccessibilityCompliantRender(false);
		}
		public bool IsAccessibilityCompliantRender(bool checkEnabled) {
			return IsAccessibilityCompliant() && (!checkEnabled || IsEnabled());
		}
		protected internal virtual bool IsAccessibilityAssociatingSupported() {
			return false;
		}
		protected internal bool IsAriaSupported() {
			return RenderUtils.IsHtml5Mode(this);
		}
		protected virtual bool IsAnimationScriptNeeded() {
			return IsCallbackAnimationEnabled() || IsSlideCallbackAnimationEnabled() || IsSwipeGesturesEnabled();
		}
		protected internal virtual bool IsCallBacksEnabled() {
			return EnableCallBacksInternal;
		}
		protected internal virtual bool IsCallbackAnimationEnabled() {
			return EnableCallbackAnimationInternal;
		}
		protected internal virtual bool IsSlideCallbackAnimationEnabled() {
			return EnableSlideCallbackAnimationInternal || IsSwipeGesturesEnabled();
		}
		protected internal virtual bool IsSwipeGesturesEnabled() {
			return EnableSwipeGesturesInternal == AutoBoolean.True || (EnableSwipeGesturesInternal == AutoBoolean.Auto && Browser.Platform.IsTouchUI);
		}
		public bool IsClientSideEventsAssigned() {
			return !ClientSideEventsInternal.IsEmpty();
		}
		protected virtual bool IsServerSideEventsAssigned() {
			return false;
		}
		protected virtual bool IsScriptEnabled() {
			return IsEnabled(); 
		}
		protected virtual bool IsStandaloneScriptEnable() {
			return false;
		}
		protected virtual ClientSideEventsBase CreateClientSideEvents() {
			return new ClientSideEventsBase();
		}
		protected virtual bool UseClientSideVisibility() {
			return !DesignMode && (!AutoPostBackInternal || IsClientSideAPIEnabled());
		}
		protected virtual SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new SettingsLoadingPanel(this);
		}
		protected internal void SetAutoFormatPreview(bool value) {
			this.isAutoFormatPreview = value;
		}
		protected virtual void SelectCurrentPath(bool ignoreQueryString) {
		}
		public bool UseCachedObjects() {
			return IsRendering && !DesignMode;
		}
		protected string GetCookie(string name) {
			return RenderUtils.GetCookie(Request, name);
		}
		protected void SetCookie(string name, string value) {
			RenderUtils.SetCookie(Request, Response, name, value);
		}
		protected virtual string GetStateCookieName() {
			return GetStateCookieName(SaveStateToCookiesID);
		}
		protected string GetStateCookieName(string cookieID) {
			if(string.IsNullOrEmpty(cookieID))
				cookieID = (Page != null) ? Page.GetType().Name + "_" + ClientID : ClientID;
			return cookieID;
		}
		protected void EnsureClientStateLoaded() {
			if(Request == null || (NeedLoadClientState() && !this.isClientStateLoaded))
				LoadClientStateInternal();
		}
		protected virtual bool NeedLoadClientState() {
			return false;
		}
		protected void LoadClientStateInternal() {
			string state = string.Empty;
			if(IsClientLayoutExists) {
				ASPxClientLayoutArgs args = new ASPxClientLayoutArgs(ClientLayoutMode.Loading);
				RaiseClientLayout(args);
				state = args.LayoutData;
			}
			if(String.IsNullOrEmpty(state) && IsStateSavedToCookies)
				state = GetCookie(GetStateCookieName());
			if(!String.IsNullOrEmpty(state)) {
				try {
					LoadClientState(state);
				} catch { }
			}
			this.isClientStateLoaded = true;
		}
		protected internal virtual void LoadClientState(string state) {
		}
		protected void SaveClientStateInternal() {
			if(IsClientLayoutExists || IsStateSavedToCookies) {
				string state = SaveClientState();
				if(IsClientLayoutExists)
					RaiseClientLayout(new ASPxClientLayoutArgs(ClientLayoutMode.Saving, state));
				if(IsStateSavedToCookies)
					SetCookie(GetStateCookieName(), state);
			}
		}
		protected internal virtual string SaveClientState() {
			return string.Empty;
		}
		protected virtual bool IsStateScriptEnabled() {
			return IsScriptEnabled() || RenderUtils.IsAnyCallback(Page);
		}
		protected virtual bool CanHoverScript() {
			return !Browser.Platform.IsWebKitTouchUI;
		}
		protected virtual bool HasHoverScripts() {
			return false;
		}
		protected virtual void RegisterHoverIncludeScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), UtilsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), ClassesScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), StateControllerScriptResourceName);
		}
		protected virtual void RegisterHoverStartupScripts() {
			StartupScript += GetHoverScript();
		}
		protected virtual void AddHoverItems(StateScriptRenderHelper helper) {
		}
		private string GetHoverScript() {
			StringBuilder stb = new StringBuilder();
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			AddHoverItems(helper);
			helper.GetCreateHoverScript(stb);
			return stb.ToString();
		}
		protected virtual bool HasPressedScripts() {
			return false;
		}
		protected virtual void RegisterPressedIncludeScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), UtilsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), ClassesScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), StateControllerScriptResourceName);
		}
		protected virtual void RegisterPressedStartupScripts() {
			StartupScript += GetPressedScript();
		}
		protected virtual void AddPressedItems(StateScriptRenderHelper helper) {
		}
		private string GetPressedScript() {
			StringBuilder stb = new StringBuilder();
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			AddPressedItems(helper);
			helper.GetCreatePressedScript(stb);
			return stb.ToString();
		}
		protected virtual bool HasSelectedScripts() {
			return false;
		}
		protected virtual void RegisterSelectedIncludeScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), UtilsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), ClassesScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), StateControllerScriptResourceName);
		}
		protected virtual void RegisterSelectedStartupScripts() {
			StartupScript += GetSelectedScript();
		}
		protected virtual void AddSelectedItems(StateScriptRenderHelper helper) {
		}
		private string GetSelectedScript() {
			StringBuilder stb = new StringBuilder();
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			AddSelectedItems(helper);
			helper.GetCreateSelectedScript(stb);
			return stb.ToString();
		}
		protected virtual bool HasDisabledScripts() {
			return !IsNativeRender() && IsEnabled() && (IsClientSideAPIEnabled() || CanBeDisabledByParent());
		}
		protected virtual void RegisterDisabledIncludeScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), UtilsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), ClassesScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), StateControllerScriptResourceName);
		}
		protected virtual void RegisterDisabledStartupScripts() {
			StartupScript += GetDisabledScript();
		}
		protected virtual void AddDisabledItems(StateScriptRenderHelper helper) {
		}
		protected virtual bool CanBeDisabledByParent() {
			return (HasClientInitialization() || (IsScriptEnabled() && HasFunctionalityScripts())) && CanParentDisableChild(Parent);
		}
		static bool CanParentDisableChild(Control parent) {
			if(parent == null)
				return false;
			ASPxWebControl control = parent as ASPxWebControl;
			return control != null && control.IsClientSideAPIEnabled() || CanParentDisableChild(parent.Parent);
		}
		private string GetDisabledScript() {
			StringBuilder stb = new StringBuilder();
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			AddDisabledItems(helper);
			helper.GetCreateDisabledScript(stb);
			return stb.ToString();
		}
		public string GetClientSideEventHandler(string name) {
			return ClientSideEventsInternal.GetEventHandler(name);
		}
		public void SetClientSideEventHandler(string name, string value) {
			ClientSideEventsInternal.SetEventHandler(name, value);
		}
		protected internal virtual void RegisterExpandoAttributes(ExpandoAttributes expandoAttributes) {
		}
		private void RegisterExpandoAttributesInternal() {
			this.expandoAttributes.Clear();
			RegisterExpandoAttributes(this.expandoAttributes);
			StartupScript += this.expandoAttributes.GetCreationScript();
		}
		protected virtual bool HasClientInitialization() {
			string focusedControlID = HttpUtils.GetContextValue<string>(FocusedControlIDKey, string.Empty);
			return !string.IsNullOrEmpty(focusedControlID) && object.Equals(focusedControlID, ClientID);
		}
		protected virtual bool HasFunctionalityScripts() {
			return IsClientSideAPIEnabled();
		}
		public virtual bool IsClientSideAPIEnabled() {
			return EnableClientSideAPIInternal || !string.IsNullOrEmpty(ClientInstanceNameInternal) ||
				IsClientSideEventsAssigned() || !ClientVisibleInternal || !ClientEnabledInternal;
		}
		public virtual bool IsClientVisible() {
			return ClientVisibleInternal || DesignMode;
		}
		protected bool IsVisibleAndClientVisible() {
			for(Control control = this; control != null; control = control.Parent) {
				if(!control.Visible)
					return false;
				ASPxWebControl webControl = control as ASPxWebControl;
				if(webControl != null && !webControl.ClientVisibleInternal)
					return false;
			}
			return true;
		}
		protected virtual void RegisterPostBackScripts() {
			if(IsScriptEnabled()) {
				if(AutoPostBackInternal || IsClientSideEventsAssigned() || IsServerSideEventsAssigned())
					RenderUtils.GetPostBackEventReference(Page, this, "");
				if(this is IRequiresLoadPostDataControl)
					RenderUtils.RegisterRequiresPostBack(Page, this);
				if(IsCallBacksEnabled())
					GetCallbackEventReference(); 
			}
		}
		private bool RequiresIncludeScriptsEvenIfNoClientInstance {
			get {
#if !DebugTest
				return ASPxScriptManager.Active;
#else
				return false;
#endif
			}
		}
		public void RegisterClientIncludeScripts() {
			RegisterCustomWebResourceHandlers();
			if(IsStateScriptEnabled() || HasClientInitialization()) {
				if(CanHoverScript() && HasHoverScripts())
					RegisterHoverIncludeScripts();
				if(HasPressedScripts())
					RegisterPressedIncludeScripts();
				if(HasSelectedScripts())
					RegisterSelectedIncludeScripts();
				if(HasDisabledScripts())
					RegisterDisabledIncludeScripts();
			}
			if(IsScriptEnabled() || HasClientInitialization()) {
				bool hasScripts = HasFunctionalityScripts() || HasClientInitialization();
				if(hasScripts || RequiresIncludeScriptsEvenIfNoClientInstance)
					RegisterIncludeScripts();
			}
		}
		public void RegisterClientScriptBlocks() {
			if(IsScriptEnabled() || HasClientInitialization()) {
				RegisterStandaloneScriptBlocks();
				bool hasScripts = HasFunctionalityScripts() || HasClientInitialization();
				if(hasScripts || RequiresIncludeScriptsEvenIfNoClientInstance)
					RegisterScriptBlocks();
			}
		}
		public void RegisterClientScripts() {
			StartupScript = string.Empty;
			if(IsStateScriptEnabled()) {
				if(CanHoverScript() && HasHoverScripts())
					RegisterHoverStartupScripts();
				if(HasPressedScripts())
					RegisterPressedStartupScripts();
				if(HasSelectedScripts())
					RegisterSelectedStartupScripts();
				if(HasDisabledScripts())
					RegisterDisabledStartupScripts();
			}
			if(IsScriptEnabled() || HasClientInitialization()) {
				RegisterExpandoAttributesInternal();
				bool hasScripts = HasFunctionalityScripts() || HasClientInitialization();
				if(hasScripts) {
					RegisterStartupScripts();
				}
			}
			if(IsStandaloneScriptEnable())
				RegisterStandaloneScripts();
		}
		protected void RegisterScriptBlock(string key, string script) {
			ResourceManager.RegisterScriptBlock(Page, key, script);
		}
		public static void RegisterBaseScript(Page page) {
			ResourceManager.RegisterScriptResource(page, typeof(ASPxWebControl), UtilsScriptResourceName, true);
			ResourceManager.RegisterScriptResource(page, typeof(ASPxWebControl), ClassesScriptResourceName, true);
		}
		public static void RegisterBaseScript(Page page, bool registerMobileScript) {
			RegisterBaseScript(page);
			if(registerMobileScript && Browser.Platform.IsMacOSMobile || Browser.Platform.IsAndroidMobile) {
				ResourceManager.RegisterScriptResource(page, typeof(ASPxWebControl), MobileScriptResourceName, true);
				ResourceManager.RegisterCssResource(page, typeof(ASPxWebControl), WebSystemCssResourceName);
			}
		}
		[Obsolete("This method is now obsolete. Use the RegisterBaseScript(Page page) method instead.")]
		public static void RegisterUtilsScript(Page page) {
			RegisterBaseScript(page);
		}
		protected static void RegisterJQueryScript(Page page) {
			RegisterJQueryScript(page, false);
		}
		protected static void RegisterJQueryScript(Page page, bool alwaysRegisterJQueryScript) {
			if(alwaysRegisterJQueryScript || ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.JQueryScriptResourceName);
		}
		protected static void RegisterJQueryUIScript(Page page) {
			if(ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.JQueryUIScriptResourceName);
		}
		protected static void RegisterJQueryUICss(Page page) {
			if(ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterCssResource(page, typeof(RenderUtils), RenderUtils.JQueryUICssResourceName);
		}
		protected static void RegisterJQueryValidateScript(Page page) {
			if(ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.JQueryValidateScriptResourceName);
		}
		protected static void RegisterJQueryUnobtrusiveScript(Page page) {
			if(ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.JQueryUnobtrusiveScriptResourceName);
		}
		protected static void RegisterJQueryUnobtrusiveAjaxScript(Page page) {
			if(ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.JQueryUnobtrusiveAjaxScriptResourceName);
		}
		protected virtual void RegisterIncludeScript(Type type, string resourceName) {
			RegisterIncludeScript(type, resourceName, true);
		}
		protected virtual void RegisterIncludeScript(Type type, string resourceName, bool condition) {
			if(condition || IsMvcResourcesRender() || ASPxScriptManager.Active)
				ResourceManager.RegisterScriptResource(Page, type, resourceName);
		}
		protected virtual void RegisterIncludeScripts() {
			if(IsMvcRender()) {
				RegisterJQueryScript(Page);
				RegisterJQueryValidateScript(Page);
				RegisterJQueryUnobtrusiveScript(Page);
				RegisterJQueryUnobtrusiveAjaxScript(Page);
			}
			RegisterIncludeScript(typeof(ASPxWebControl), UtilsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), ImageScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), ClassesScriptResourceName);
			RegisterAnimationScript(IsAnimationScriptNeeded());
			RegisterIncludeScript(typeof(ASPxWebControl), MobileScriptResourceName, Browser.Platform.IsMacOSMobile || Browser.Platform.IsAndroidMobile);
			RegisterRelatedControlManagerScripts(this is IMasterControl);
			#region DebugScripts
#if DEBUG || DEBUGTEST
			RegisterIncludeScript(typeof(ASPxWebControl), DebugScriptResourceName);
#endif
			#endregion
		}
		protected virtual void RegisterCustomWebResourceHandlers() {
			ResourceManager.RegisterCustomWebResourceHandler("EmptyImageUrl", GetEmptyImageUrlResourceDelegate(EmptyImageProperties.GetEmptyImageUrl(Page)));
			ResourceManager.RegisterCustomWebResourceHandler("VersionInfo", GetVersionResourceDelegate());
		}
		static Function<string, string> GetEmptyImageUrlResourceDelegate(string emptyImageUrl) {
			return delegate(string p) {
				return emptyImageUrl;
			};
		}
		static Function<string, string> GetVersionResourceDelegate() {
			return delegate(string p) {
				return string.Format("Version=\\'{0}\\', File Version=\\'{1}\\', Date Modified=\\'{2}\\'", AssemblyInfo.Version, AssemblyInfo.FileVersion,
					ResourceManager.GetAssemblyModificationUniversalDate(typeof(ASPxWebControl).Assembly));
			};
		}
		protected virtual void RegisterScriptBlocks() {
			if(NeedRenderIFrameBehindPopupElement()) {
				string script = string.Format("ASPx.AccessibilitySR.AccessibilityIFrameTitle = {0};", 
					HtmlConvertor.ToScript(ASPxperienceLocalizer.GetString(ASPxperienceStringId.AccessibilityIFrameTitle)));
				RegisterScriptBlock("AccessibilitySR_IFrame", RenderUtils.GetScriptHtml(script));
			}
		}
		protected virtual void RegisterStandaloneScriptBlocks() {
		}
		protected virtual void RegisterAnimationScript() {
			RegisterAnimationScript(true);
		}
		protected virtual void RegisterAnimationScript(bool condition) {
			RegisterIncludeScript(typeof(ASPxWebControl), AnimationScriptResourceName, condition);
		}
		protected virtual void RegisterPopupUtilsScripts() {
			RegisterPopupUtilsScripts(true);
		}
		protected virtual void RegisterPopupUtilsScripts(bool condition) {
			RegisterAnimationScript(condition);
			RegisterIncludeScript(typeof(ASPxWebControl), PopupUtilsScriptResourceName, condition);
		}
		protected virtual void RegisterScrollUtilsScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), ScrollUtilsScriptResourceName);
		}
		protected virtual void RegisterDialogUtilsScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), DialogUtilsScriptResourceName);
		}
		protected virtual void RegisterDragAndDropUtilsScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), DragAndDropUtilsScriptResourceName);
		}
		protected virtual void RegisterRelatedControlManagerScripts() {
			RegisterRelatedControlManagerScripts(true);
		}
		protected virtual void RegisterRelatedControlManagerScripts(bool condition) {
			RegisterIncludeScript(typeof(ASPxWebControl), RelatedControlManagerScriptResourceName, condition);
		}
		protected virtual void RegisterTableScrollUtilsScript() {
			RegisterTableScrollUtilsScript(true);
		}
		protected virtual void RegisterTableScrollUtilsScript(bool condition) {
			RegisterIncludeScript(typeof(ASPxWebControl), TableScrollUtilsScriptResourceName, condition);
		}
		protected void RegisterDateFormatterScript() {
			RegisterIncludeScript(typeof(ASPxWebControl), DateFormatterScriptResourceName);
		}
		protected void RegisterFormatterScript() {
			RegisterDateFormatterScript();
			RegisterIncludeScript(typeof(ASPxWebControl), FormatterScriptResourceName);
		}
		protected void RegisterControlResizeManagerScripts() {
			RegisterIncludeScript(typeof(ASPxWebControl), ControlResizeManagerScriptResourceName);
		}
		protected void RegisterImageControlUtilsScript() {
			RegisterIncludeScript(typeof(ASPxWebControl), ImageControlUtilsScriptResourceName);
		}
		protected void RegisterBackToTopScript() {
			RegisterScriptBlock("BackToTopScript", RenderUtils.GetScriptHtml(RenderUtils.GetBackToTopScript()));
		}
		protected void RegisterCultureInfoScript() {
			RegisterScriptBlock("CultureInfo", RenderUtils.GetScriptHtml(RenderUtils.GetClientDateFormatInfoScript()));
		}
		protected void RegisterStartupScripts() {
			StartupScript += GetStartupScript();
		}
		protected void RegisterStandaloneScripts() {
			StartupScript += GetStandaloneScript();
		}
		protected virtual string GetStartupScript() {
			if(!string.IsNullOrEmpty(GetClientObjectClassName()))
				return GetCreateClientObjectScript(GetClientObjectClassName(), GetClientInstanceName(), ClientID);
			return string.Empty;
		}
		protected string GetCreateClientObjectScript(string clientObjectClass, string localVarName, string clientName) {
			StringBuilder stb = new StringBuilder();
			stb.AppendFormat("\nvar {0} = new {1}({2});\n", ShortClientLocalVariableName, clientObjectClass, HtmlConvertor.ToScript(clientName));
			stb.AppendFormat("{0}.InitGlobalVariable({1});\n", ShortClientLocalVariableName, HtmlConvertor.ToScript(localVarName));
			GetCreateClientObjectScript(stb, ShortClientLocalVariableName, clientName);
			GetFinalizeClientObjectScript(stb, ShortClientLocalVariableName, clientName);
			return stb.ToString();
		}
		protected virtual void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			if(!EncodeHtml)
				stb.AppendFormat("{0}.encodeHtml = false;\n", localVarName);
			if(AutoPostBackInternal)
				stb.Append(localVarName + ".autoPostBack = true;\n");
			if(IsCallBacksEnabled())
				stb.AppendFormat("{0}.callBack = function(arg) {{ {1}; }};\n", localVarName, GetCallbackEventReference());
			if(IsCallbackAnimationEnabled())
				stb.AppendFormat("{0}.enableCallbackAnimation = true;\n", localVarName);
			if(IsSlideCallbackAnimationEnabled())
				stb.AppendFormat("{0}.enableSlideCallbackAnimation = true;\n", localVarName);
			if(IsSwipeGesturesEnabled())
				stb.AppendFormat("{0}.enableSwipeGestures = true;\n", localVarName);
			if(SettingsLoadingPanel.Delay != SettingsLoadingPanel.DefaultDelay)
				stb.AppendFormat("{0}.lpDelay = {1};\n", localVarName, SettingsLoadingPanel.Delay);
			if(ClientID != UniqueID)
				stb.AppendFormat("{0}.uniqueID = '{1}';\n", localVarName, UniqueID);
			if(IsStateSavedToCookies)
				stb.AppendFormat("{0}.cookieName = '{1}';\n", localVarName, GetStateCookieName());
			if(IsNativeRender())
				stb.AppendFormat("{0}.isNative = true;\n", localVarName);
			if(NeedRenderIFrameBehindPopupElement()) {
				stb.Append(localVarName);
				stb.Append(".renderIFrameForPopupElements = true;\n");
			}
			if(!ClientVisibleInternal)
				stb.Append(localVarName + ".clientVisible = false;\n");
			if(!this.IsEnabled())
				stb.Append(localVarName + ".enabled = false;\n");
			else {
				if(IsClientSideAPIEnabled()) {
					if(!ClientEnabledInternal)
						stb.Append(localVarName + ".clientEnabled = false;\n");
				}
				if(IsFocused())
					stb.AppendFormat("{0}.initialFocused = true;\n", localVarName);
				stb.Append(ClientSideEventsInternal.GetStartupScript(localVarName));
				InitializeAssignedServerEventsScript(stb, localVarName, clientName);
			}
			if(IsAccessibilityCompliant())
				stb.AppendFormat("{0}.accessibilityCompliant = true;\n", localVarName);
			if(IsRightToLeft())
				stb.AppendFormat("{0}.rtl = true;\n", localVarName);
			GetClientObjectStateScript(stb, localVarName);
			GetCustomJSPropertiesScript(stb, localVarName);
		}
		protected virtual void GetFinalizeClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			stb.Append(localVarName + ".AfterCreate();\n");
		}
		protected void GetClientObjectStateScript(StringBuilder stb, string localVarName) {
			Hashtable stateObj = GetClientObjectState();
			if(stateObj != null && stateObj.Count > 0)
				 stb.AppendFormat("{0}.{1} = {2};\n", localVarName, ClientStateProperties.StateObject, HtmlConvertor.ToJSON(stateObj));
		}
		protected virtual Hashtable GetClientObjectState() {
			return null;
		}
		protected virtual string GetClientObjectStateInputID() {
			return UniqueID;
		}
		protected virtual Hashtable LoadClientObjectState(NameValueCollection postCollection) {
			return LoadClientObjectState(postCollection, GetClientObjectStateInputID());
		}
		protected static Hashtable LoadClientObjectState(NameValueCollection postCollection, string name) {
			if(string.IsNullOrEmpty(name)) return null;
			return LoadClientObjectState(postCollection[name]);
		}
		protected static Hashtable LoadClientObjectState(string postValue) {
			string clientStateString = HtmlConvertor.DecodeHtml(postValue);
			return !string.IsNullOrEmpty(clientStateString) ? HtmlConvertor.FromJSON<Hashtable>(clientStateString) : null;
		}
		protected string GetClientObjectStateValueString(string key) {
			return GetClientObjectStateValueString(ClientObjectState, key);
		}
		protected T GetClientObjectStateValue<T>(string key) {
			return GetClientObjectStateValue<T>(ClientObjectState, key);
		}
		protected static string GetClientObjectStateValueString(Hashtable clientObjectState, string key) {
			if(clientObjectState == null)
				return string.Empty;
			return (clientObjectState.ContainsKey(key) && clientObjectState[key] != null) ? clientObjectState[key].ToString() : string.Empty;
		}
		protected static T GetClientObjectStateValue<T>(Hashtable clientObjectState, string key) {
			if(clientObjectState == null)
				return default(T);
			return (T)clientObjectState[key];
		}
		protected virtual string GetStandaloneScript() {
			return CreateStandaloneScript();
		}
		protected virtual string CreateStandaloneScript() {
			return string.Empty;
		}
		protected void GetCustomJSPropertiesScript(StringBuilder stb, string localVarName) {
			IDictionary<string, object> properties = GetCustomJSProperties();
			if(properties != null) {
				CheckCustomJSProperties(properties);
				foreach(string name in properties.Keys)
					stb.AppendFormat("{0}.{1}={2};\n", localVarName, name, HtmlConvertor.ToJSON(properties[name]));
			}
		}
		protected void CheckCustomJSProperties(IDictionary<string, object> properties) {
			foreach(string name in properties.Keys)
				CommonUtils.CheckCustomPropertyName(name);
		}
		protected virtual IDictionary<string, object> GetCustomJSProperties() {
			CustomJSPropertiesEventArgs e = new CustomJSPropertiesEventArgs(JSPropertiesInternal);
			RaiseCustomJSProperties(e);
			if(e.Properties.Count > 0)
				return e.Properties;
			return null;
		}
		protected virtual void RaiseBeforeGetCallbackResult() {
			EventHandler handler = Events[EventBeforeGetCallbackResult] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void InitializeAssignedServerEventsScript(StringBuilder stb, string localVarName, string clientName) {
			List<string> list = new List<string>();
			GetClientObjectAssignedServerEvents(list);
			if(list.Count > 0)
				stb.Append(localVarName + ".RegisterServerEventAssigned(" + HtmlConvertor.ToJSON(list) + ");\n");
		}
		protected virtual void GetClientObjectAssignedServerEvents(List<string> eventNames) {
		}
		protected string GetCallBackHandlerName() {
			return "ASPx.Callback";
		}
		protected string GetCallBackErrorHandlerName() {
			return "ASPx.CallbackError";
		}
		protected string GetCallbackEventReference() {
			return RenderUtils.GetCallbackEventReference(Page, this, "arg", GetCallBackHandlerName(), "'" + ClientID + "'", GetCallBackErrorHandlerName());
		}
		protected string GetClientInstanceName() {
			return string.IsNullOrEmpty(ClientInstanceNameInternal) ? ClientID : ClientInstanceNameInternal;
		}
		protected virtual string GetClientObjectClassName() {
			return "";
		}
		protected internal void RaiseCustomJSProperties(CustomJSPropertiesEventArgs e) {
			CustomJSPropertiesEventHandler handler = Events[EventCustomJsProperties] as CustomJSPropertiesEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected virtual void RaiseClientLayout(ASPxClientLayoutArgs e) {
			ASPxClientLayoutHandler handler = (ASPxClientLayoutHandler)Events[EventClientLayout];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void RaiseCallbackError() {
			RaiseCallbackErrorInternal(this);
		}
		protected internal static void RaiseCallbackErrorInternal(object sender) {
			if(CallbackError != null)
				CallbackError(sender, new EventArgs());
		}
		protected internal void LayoutChanged() {
			if(!IsLoading())
				ResetControlHierarchy();
		}
		protected internal void TemplatesChanged() {
			if(!IsLoading())
				ResetControlHierarchy();
		}
		protected virtual ImagesBase CreateImages() {
			return new ImagesBase(this);
		}
		protected internal virtual ImageProperties GetLoadingPanelImage() {
			ImageProperties result = new ImageProperties();
			result.CopyFrom(ImagesInternal.GetDefaultLoadingImageProperties());
			result.CopyFrom(LoadingPanelImage);
			return result;
		}
		public struct CacheKey<T> {
			public class KeyComparer : IEqualityComparer<CacheKey<T>> {
				public bool Equals(CacheKey<T> x, CacheKey<T> y) {
					return (object)x.Item == (object)y.Item && x.ExtraOption == y.ExtraOption;
				}
				public int GetHashCode(CacheKey<T> obj) {
					return obj.Item.GetHashCode() ^ obj.ExtraOption;
				}
			}
			public T Item;
			public int ExtraOption;
			public CacheKey(T item, int extraOption) {
				Item = item;
				ExtraOption = extraOption;
			}
		}
		public class ImagePropertiesCache<T, P> : Dictionary<CacheKey<T>, P> {
			public delegate P Create(CacheKey<T> key);
			public Create Creator;
			public ImagePropertiesCache(Create creator)
				: base(new CacheKey<T>.KeyComparer()) {
				Creator = creator;
			}
		}
		protected P GetItemImage<T, P>(ImagePropertiesCache<T, P> cache, T item) {
			return GetItemImage(cache, item, 0);
		}
		protected P GetItemImage<T, P>(ImagePropertiesCache<T, P> cache, T item, int extraOption) {
			CacheKey<T> key = new CacheKey<T>(item, extraOption);
			P props;
			if(!UseCachedObjects() || !cache.TryGetValue(key, out props)) {
				props = cache.Creator(key);
				if(UseCachedObjects())
					cache.Add(key, props);
			}
			return props;
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyleBase();
		}
		protected virtual StylesBase CreateStyles() {
			return new StylesBase(null);
		}
		protected internal void RecreateStyles() {
			PropertiesBase old = this.styles;
			this.styles = null;
			StylesInternal.Assign(old);
		}
		protected void RegisterStyle(AppearanceStyleBase style, string name) {
			if(Page != null && Page.Header != null && !style.IsEmpty)
				Page.Header.StyleSheet.CreateStyleRule(style, Page, name);
		}
		protected virtual void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(StylesInternal.GetDefaultControlStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(ControlStyle);
		}
		static object controlStyleKey = new object();
		public AppearanceStyleBase GetControlStyle() {
			return CreateStyle(delegate() {
				AppearanceStyleBase style = (AppearanceStyleBase)CreateControlStyle();
				PrepareControlStyle(style);
				if(!IsNative())
					MergeDisableStyle(style, IsEnabled(), GetDisabledStyle(), MainOwnerControl != null);
				return style;
			}, controlStyleKey);
		}
		public virtual Paddings GetPaddings() {
			return GetControlStyle().Paddings;
		}
		protected internal void MergeControlStyleForInput(AppearanceStyleBase style) {
			style.CopyTextDecorationFrom(ControlStyle);
		}
		protected virtual void MergeControlStyle(AppearanceStyleBase style, bool mergeBackColor) {
			style.CopyFontFrom(ControlStyle);
			if(mergeBackColor)
				style.BackColor = ControlStyle.BackColor;
		}
		protected internal void MergeParentSkinOwnerControlStyle(AppearanceStyleBase style) {
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				parentSkinOwner.MergeControlStyle(style);
		}
		protected internal void MergeDisableStyle(AppearanceStyleBase style) {
			MergeDisableStyle(style, IsEnabled(), GetDisabledStyle(), true);
		}
		protected internal void MergeDisableStyle(AppearanceStyleBase style, bool enabled) {
			MergeDisableStyle(style, enabled, GetDisabledStyle(), true);
		}
		protected internal void MergeDisableStyle(AppearanceStyleBase style, AppearanceStyleBase disabledStyle) {
			MergeDisableStyle(style, IsEnabled(), disabledStyle, true);
		}
		protected internal void MergeDisableStyle(AppearanceStyleBase style, bool enabled, AppearanceStyleBase disabledStyle) {
			MergeDisableStyle(style, enabled, disabledStyle, true);
		}
		protected virtual void MergeDisableStyle(AppearanceStyleBase style, bool enabled, AppearanceStyleBase disabledStyle, bool mergeWithoutBorders) {
			if(!enabled)
				style.CopyFrom(disabledStyle, mergeWithoutBorders);
		}
		static object disabledStyleKey = new object();
		protected virtual DisabledStyle GetDisabledStyle() {
			return (DisabledStyle)CreateStyle(delegate() {
				DisabledStyle style = new DisabledStyle();
				style.CopyFrom(StylesInternal.GetDefaultDisabledStyle());
				style.CopyFrom(DisabledStyle);
				return style;
			}, disabledStyleKey);
		}
		protected virtual LoadingPanelStyle GetLoadingPanelStyle() {
			LoadingPanelStyle style = new LoadingPanelStyle();
			style.CopyFrom(StylesInternal.GetDefaultLoadingPanelStyle(CanAppendDefaultLoadingPanelCssClass()));
			MergeControlStyle(style, false);
			style.CopyFrom(LoadingPanelStyle);
			return style;
		}
		protected virtual Paddings GetLoadingPanelPaddings() {
			return GetLoadingPanelStyle().Paddings;
		}
		protected virtual Unit GetLoadingPanelImageSpacing() {
			return GetLoadingPanelStyle().ImageSpacing;
		}
		protected virtual LoadingDivStyle GetLoadingDivStyle() {
			LoadingDivStyle style = new LoadingDivStyle();
			style.CopyFrom(StylesInternal.GetDefaultLoadingDivStyle(CanAppendDefaultLoadingPanelCssClass()));
			style.CopyFrom(LoadingDivStyle);
			return style;
		}
		protected virtual bool CanAppendDefaultLoadingPanelCssClass() {
			return true;
		}
		static object boolTrue = true;
		static object boolFalse = false;
		protected object GetBoolParam(bool value) {
			return value ? boolTrue : boolFalse;
		}
		protected AppearanceStyleBase CreateStyle(CreateStyleHandler create, params object[] keys) {
			AppearanceStyleBase style;
			bool useCachedObjects = UseCachedObjects();
			if(!useCachedObjects || !stylesCache.TryGetValue(new ComplexKey(keys), out style)) {
				style = create();
				if(useCachedObjects)
					stylesCache.Add(new ComplexKey(keys), style);
			}
			return style;
		}
		public virtual void RegisterStyleSheets() {
			if(HasSpriteCssFile() && !IsNativeRender())
				RegisterSpriteCssFile();
			if(HasRenderCssFile() && !IsNativeRender()) {
				RegisterSystemCssFile();
				RegisterRenderCssFile();
			}
			RegisterIconSpriteCssFiles();
			this.styleSheetsRegistered = true;
		}
		protected virtual void RegisterSystemCssFile() {
			if(DesignMode)
				ResourceManager.RegisterCssResource(Page, typeof(ASPxWebControl), WebSystemDesignModeCssResourceName);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxWebControl), WebSystemCssResourceName);
			if(RenderUtils.IsHtml5Mode(this))
				ResourceManager.RegisterCssResource(Page, typeof(ASPxWebControl), WebSystemHtml5CssResourceName);
		}
		protected virtual void RegisterIconSpriteCssFiles() {
			IconsHelper.RegisterIconSpriteCssFiles(Page);
		}
		protected virtual bool HasSpriteCssFile() {
			return true;
		}
		protected void RegisterSpriteCssFile() {
			string spriteCssFile = GetCustomSpriteCssFilePath();
			if(!string.IsNullOrEmpty(spriteCssFile)) {
				RegisterCustomSpriteCssFile(spriteCssFile);
				if(IsWebSourcesRegisterRequired())
					ResourceManager.RegisterCssResource(Page, GetCustomSpriteCssFilePath(InternalCheckboxControl.WebSpriteControlName));
			} else if(string.IsNullOrEmpty(GetCssPostFix())) {
				RegisterDefaultSpriteCssFile();
				if(IsWebSourcesRegisterRequired())
					ResourceManager.RegisterCssResource(Page, typeof(ASPxWebControl), InternalCheckboxControl.WebSpriteCssPath);
			}
		}
		protected virtual void RegisterDefaultSpriteCssFile() {
			ImagesInternal.RegisterDefaultSpriteCssFile(Page);
		}
		protected virtual void RegisterCustomSpriteCssFile(string spriteCssFile) {
			ResourceManager.RegisterCssResource(Page, spriteCssFile);
		}
		protected string GetCustomSpriteCssFilePath() {
			return GetCustomSpriteCssFilePath(GetSkinControlName());
		}
		protected string GetCustomSpriteCssFilePath(string controlName) {
			if(!string.IsNullOrEmpty(GetSpriteCssFilePath())) {
				string path = string.Format(GetSpriteCssFilePath(), controlName);
				return GetResolvedCssFilePath(path);
			}
			return string.Empty;
		}
		protected virtual bool HasRenderCssFile() {
			return true;
		}
		protected virtual bool IsWebSourcesRegisterRequired() {
			return false;
		}
		protected void RegisterRenderCssFile() {
			string renderCssFile = GetCustomRenderCssFilePath();
			if(!string.IsNullOrEmpty(renderCssFile)) {
				ResourceManager.RegisterCssResource(Page, renderCssFile);
				if(IsWebSourcesRegisterRequired())
					ResourceManager.RegisterCssResource(Page, GetCustomRenderCssFilePath(InternalCheckboxControl.WebSpriteControlName));
			} else if(IsDefaultAppearanceEnabled() && string.IsNullOrEmpty(GetCssPostFix())) {
				RegisterDefaultRenderCssFile();
				if(IsWebSourcesRegisterRequired())
					ResourceManager.RegisterCssResource(Page, typeof(ASPxWebControl), WebDefaultCssResourceName);
			}
		}
		protected virtual void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxWebControl), WebDefaultCssResourceName);
		}
		protected string GetCustomRenderCssFilePath() {
			return GetCustomRenderCssFilePath(GetSkinControlName());
		}
		protected string GetCustomRenderCssFilePath(string controlName) {
			if(!string.IsNullOrEmpty(GetCssFilePath())) {
				string path = string.Format(GetCssFilePath(), controlName);
				return GetResolvedCssFilePath(path);
			}
			return string.Empty;
		}
		protected string GetResolvedCssFilePath(string path) {
			return ResolveClientUrl(path);
		}
		protected internal string GetStyleSheetsDesignHtml() {
			RegisterStyleSheets();
			return ResourceManager.GetCssResourcesDesignHtml(Page, !IsAutoFormatPreview);
		}
		protected bool CanRenderStyleSheetViaDesigner() {
			return DesignMode && !IsAutoFormatPreview && (OwnerControl == null) && Site != null;
		}
		protected bool CanCreateControlForCssResourcesInHeader() {
			return CanModifyHeaderControl();
		}
		protected bool CanModifyHeaderControl() {
			return !DesignMode && Page != null && Page.Header != null && !RenderUtils.IsAnyCallback(Page);
		}
		protected void CreateControlForCssResourcesInHeader() {
			ResourceManager.CreateControlForCssResourcesInHeader(Page);
		}
		protected void RenderCssResourcesInHeader() {
			ResourceManager.RenderCssResourcesInHeader(Page);
		}
		public string GetCssClassNamePrefix() {
			return StylesInternal.GetCssClassNamePrefix();
		}
		public string GetCssClassNamePrefix(string postfix) {
			return string.Format("{0}{1}", GetCssClassNamePrefix(), postfix);
		}
		protected virtual void RegisterLinkStyles() {
			if(HasLinkStyle) {
				RegisterLinkVisitedStyle(LinkStyle.VisitedStyle, ClientID);
				RegisterLinkHoverStyle(LinkStyle.HoverStyle, ClientID);
			}
		}
		protected void RegisterLinkHoverStyle(AppearanceStyleBase style, string id) {
			RegisterLinkSelectorStyle(style, id, "hover");
		}
		protected void RegisterLinkVisitedStyle(AppearanceStyleBase style, string id) {
			RegisterLinkSelectorStyle(style, id, "visited");
		}
		protected void RegisterLinkSelectorStyle(AppearanceStyleBase style, string id, string selector) {
			RegisterStyle(style, "#" + id + " a:" + selector);
			RegisterStyle(style, "#" + id + " a:" + selector + " *");
		}
		protected internal void LockPropertyChanged() {
			this.propertyChangedLockCount++;
		}
		protected internal void UnlockPropertyChanged() {
			this.propertyChangedLockCount--;
		}
		protected internal void PropertyChanged(string propName) {
			PropertyChanged(this, propName, null, null);
		}
		protected internal void PropertyChanged(object obj, string propName, object oldValue, object newValue) {
			if(this.propertyChangedLockCount > 0 || IsLoading())
				return;
			if(Site != null) {
				IComponentChangeService changeService = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				changeService.OnComponentChanged(obj, TypeDescriptor.GetProperties(obj)[propName], oldValue, newValue);
			}
		}
		protected object GetObjectProperty(string key, object defaultValue) {
			return ViewStateUtils.GetObjectProperty(ViewState, key, defaultValue);
		}
		protected void SetObjectProperty(string key, object defaultValue, object value) {
			ViewStateUtils.SetObjectProperty(ViewState, key, value);
		}
		protected object GetEnumProperty(string key, object defaultValue) {
			return ViewStateUtils.GetEnumProperty(ViewState, key, defaultValue);
		}
		protected void SetEnumProperty(string key, object defaultValue, object value) {
			ViewStateUtils.SetEnumProperty(ViewState, key, defaultValue, value);
		}
		protected bool GetBoolProperty(string key, bool defaultValue) {
			return ViewStateUtils.GetBoolProperty(ViewState, key, defaultValue);
		}
		protected void SetBoolProperty(string key, bool defaultValue, bool value) {
			ViewStateUtils.SetBoolProperty(ViewState, key, defaultValue, value);
		}
		public DefaultBoolean GetDefaultBooleanProperty(string key, DefaultBoolean defaultValue) {
			return ViewStateUtils.GetDefaultBooleanProperty(ViewState, key, defaultValue);
		}
		public void SetDefaultBooleanProperty(string key, DefaultBoolean defaultValue, DefaultBoolean value) {
			ViewStateUtils.SetDefaultBooleanProperty(ViewState, key, defaultValue, value);
		}
		protected string GetStringProperty(string key, string defaultValue) {
			return ViewStateUtils.GetStringProperty(ViewState, key, defaultValue);
		}
		protected void SetStringProperty(string key, string defaultValue, string value) {
			ViewStateUtils.SetStringProperty(ViewState, key, defaultValue, value);
		}
		protected int GetIntProperty(string key, int defaultValue) {
			return ViewStateUtils.GetIntProperty(ViewState, key, defaultValue);
		}
		protected void SetIntProperty(string key, int defaultValue, int value) {
			ViewStateUtils.SetIntProperty(ViewState, key, defaultValue, value);
		}
		protected Decimal GetDecimalProperty(string key, Decimal defaultValue) {
			return ViewStateUtils.GetDecimalProperty(ViewState, key, defaultValue);
		}
		protected void SetDecimalProperty(string key, Decimal defaultValue, Decimal value) {
			ViewStateUtils.SetDecimalProperty(ViewState, key, defaultValue, value);
		}
		protected Unit GetUnitProperty(string key, Unit defaultValue) {
			return ViewStateUtils.GetUnitProperty(ViewState, key, defaultValue);
		}
		protected void SetUnitProperty(string key, Unit defaultValue, Unit value) {
			ViewStateUtils.SetUnitProperty(ViewState, key, defaultValue, value);
		}
		protected Color GetColorProperty(string key, Color defaultValue) {
			return ViewStateUtils.GetColorProperty(ViewState, key, defaultValue);
		}
		protected void SetColorProperty(string key, Color defaultValue, Color value) {
			ViewStateUtils.SetColorProperty(ViewState, key, defaultValue, value);
		}
		protected virtual bool IsSettingsLoadingPanelStoreToViewState() {
			return true;
		}
		protected virtual bool IsClientSideEventsStoreToViewState() {
			return true;
		}
		protected virtual bool IsImagesStoreToViewState() {
			return true;
		}
		protected virtual bool IsStylesStoreToViewState() {
			return true;
		}
		protected virtual IStateManager[] GetStateManagedObjects() {
			List<IStateManager> objects = new List<IStateManager>();
			objects.Add(ControlStyle);
			if(IsSettingsLoadingPanelStoreToViewState())
				objects.Add(SettingsLoadingPanel);
			if(IsClientSideEventsStoreToViewState())
				objects.Add(ClientSideEventsInternal);
			if(IsStylesStoreToViewState())
				objects.Add(StylesInternal);
			if(IsImagesStoreToViewState())
				objects.Add(ImagesInternal);
			return objects.ToArray();
		}
		protected override void LoadViewState(object savedState) {
			this.viewStateLoading = true;
			try {
				if(savedState != null) {
					object[] stateArray = savedState as object[];
					object baseState = (stateArray.Length > 0 && stateArray[0] != null) ? stateArray[0] : null;
					base.LoadViewState(baseState);
					ViewStateUtils.LoadObjectsViewState(stateArray, GetStateManagedObjects());
				} else
					base.LoadViewState(null);
			} finally {
				this.viewStateLoading = false;
				this.viewStateLoaded = true;
			}
			if(EnableHierarchyRecreationInternal)
				ResetControlHierarchy();
		}
		protected override object SaveViewState() {
			if(!Visible) 
				this.isClientStateLoaded = false;
			return ViewStateUtils.SaveViewState(base.SaveViewState(), GetStateManagedObjects());
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			ViewStateUtils.TrackObjectsViewState(GetStateManagedObjects());
		}
		protected void EnsurePostDataLoaded() {
			EnsurePostDataLoaded(false);
		}
		protected void EnsurePostDataLoaded(bool forcePostDataLoading) {
			if(PostDataCollection != null && PostDataCollection.Count > 0) {
				if(forcePostDataLoading)
					this.postDataLoaded = false;
				LoadPostDataInternal(PostDataCollection, false);
			}
		}
		protected bool LoadPostDataInternal(NameValueCollection postCollection, bool raisePostDataEvent) {
			if(!this.postDataLoaded) {
				try {
					this.loadPostDataResult = LoadPostData(postCollection);
					if(raisePostDataEvent && this.loadPostDataResult)
						RaisePostDataChangedEvent();
				} catch(Exception e) {
					this.postDataLoaded = true;
					if(Page != null && Page.IsCallback) {
						CallbackErrorMessage = OnCallbackException(e);
						AddErrorForHandler(e);
						return false;
					} else
						throw;
				}
				this.postDataLoaded = true;
			}
			return this.loadPostDataResult;
		}
		protected virtual bool LoadPostData(NameValueCollection postCollection) {
			return false;
		}
		protected bool CanLoadPostData() {
			return !DesignMode && Page != null && Request != null && (Page.IsCallback || Page.IsPostBack);
		}
		protected virtual bool CanLoadPostDataOnCreateControls() {
			return false;
		}
		protected virtual bool CanLoadPostDataOnLoad() {
			return !RenderUtils.PreventLoadPostDataOnLoad;
		}
		protected virtual void RaisePostDataChangedEvent() {
		}
		NameValueCollection IPostDataCollection.PostDataCollection {
			get { return PostDataCollection; }
		}
		void IPostBackDataHandlerEx.ForceLoadPostData(string postDataKey, NameValueCollection postCollection) {
			this.postDataLoaded = false;
			LoadPostDataInternal(postCollection, RenderUtils.RaiseChangedOnLoadPostData);
		}
		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) {
			this.postDataCollection = postCollection;
			return LoadPostDataInternal(postCollection, false);
		}
		void IPostBackDataHandler.RaisePostDataChangedEvent() {
			try {
				RaisePostDataChangedEvent();
			} catch(Exception e) {
				if(Page != null && Page.IsCallback) {
					CallbackErrorMessage = OnCallbackException(e);
					AddErrorForHandler(e);
				} else
					throw;
			}
		}
		protected virtual void RaisePostBackEvent(string eventArgument) {
		}
		void IPostBackEventHandler.RaisePostBackEvent(string eventArgument) {
			RaisePostBackEvent(eventArgument);
		}
		void IHandleCallbackError.HandleErrorOnGetCallbackResult(IDictionary callbackResult, string errorMessage) {
			HandleErrorOnGetCallbackResult(callbackResult, errorMessage);
		}
		string IHandleCallbackError.OnCallbackException(Exception e) {
			return OnCallbackException(e);
		}
		string ICallbackEventHandler.GetCallbackResult() {
			Hashtable result = new Hashtable();
			if(!String.IsNullOrEmpty(CallbackErrorMessage)) {
				HandleErrorOnGetCallbackResult(result, CallbackErrorMessage);
				CallbackErrorMessage = null;
			}
			try {
				RaiseBeforeGetCallbackResult();
				if(this.callbackType == CallbackType.Data)
					result[CallbackResultProperties.Result] = this.callbackResult;
				else
					result[CallbackResultProperties.Result] = GetCallbackResult();
				IDictionary<string, object> customProperties = GetCustomJSProperties();
				if(customProperties != null) {
					CheckCustomJSProperties(customProperties);
					result[CallbackResultProperties.CustomProperties] = customProperties;
				}
				SaveClientStateInternal();
				string redirect = HttpUtils.GetContextValue(redirectOnCallbackKey, string.Empty);
				if(!string.IsNullOrEmpty(redirect))
					result[CallbackResultProperties.Redirect] = GetCallbackRedirectResult(redirect.ToString());
			} catch(Exception e) {
				CallbackErrorMessage = OnCallbackException(e);
				AddErrorForHandler(e);
			}
			result[CallbackResultProperties.ID] = this.callbackID;
			if(!String.IsNullOrEmpty(CallbackErrorMessage))
				HandleErrorOnGetCallbackResult(result, CallbackErrorMessage);
			ClearErrorForHandler();
			if((ConfigurationSettings.EnableCallbackCompression && EnableCallbackCompressionInternal) || ConfigurationSettings.EnableHtmlCompression)
				HttpUtils.MakeResponseCompressed(true);
			if(Response != null) {
				Response.ContentType = "text/plain";
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
			}
			return RenderUtils.CallBackResultPrefix + HtmlConvertor.ToJSON(result);
		}
		void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument) {
			EnsurePostDataLoaded(); 
			try {
				ProcessCallbackArgument(ref eventArgument);
				if(this.callbackType == CallbackType.Data)
					RaiseCustomDataCallbackEvent(eventArgument);
				else
					RaiseCallbackEvent(eventArgument);
			} catch(Exception e) {
				CallbackErrorMessage = OnCallbackException(e);
				AddErrorForHandler(e);
			}
		}
		protected internal static void AddErrorForHandler(Exception ex) {
			if(HttpContext.Current != null)
				HttpContext.Current.AddError(ex);
		}
		protected internal static void ClearErrorForHandler() {
			HttpContext context = HttpContext.Current;
			if(context == null || IsMvcRenderInternal() && context.AllErrors == null) 
				return;
			context.ClearError();
		}
		private void ProcessCallbackArgument(ref string eventArgument) {
			this.callbackType = GetCallbackType(eventArgument);
			ProcessCallbackArgumentCore(ref this.callbackID, ref eventArgument);
		}
		protected void ProcessCallbackArgumentCore(ref int callbackID, ref string eventArgument) {
			int separatorPosition = eventArgument.IndexOf(CallbackSeparator);
			callbackID = int.Parse(eventArgument.Substring(1, separatorPosition - 1));
			eventArgument = eventArgument.Substring(separatorPosition + 1);
		}
		private CallbackType GetCallbackType(string eventArgument) {
			return eventArgument[0] == CustomDataCallbackPrefix ? CallbackType.Data : CallbackType.Common;
		}
		protected virtual object GetCallbackResult() {
			return string.Empty;
		}
		protected object GetCallbackErrorObject(string errorMessage) {
			Hashtable result = new Hashtable();
			object errorResult = GetCallbackErrorResult(errorMessage);
			if(errorResult != null)
				result[CallbackResultProperties.ErrorMessage] = errorResult;
			object errorData = GetCallbackErrorData();
			if(errorData != null)
				result[CallbackResultProperties.ErrorData] = errorData;
			return result;
		}
		protected virtual object GetCallbackErrorResult(string errorMessage) {
			return errorMessage;
		}
		protected virtual object GetCallbackErrorData() {
			return null;
		}
		protected virtual string OnCallbackException(Exception e) {
			return OnCallbackExceptionInternal(e);
		}
		protected string ProcessCallbackException(Exception ex) {
			return ProcessCallbackExceptionInternal(ex, this);
		}
		protected static string ProcessCallbackExceptionInternal(Exception ex, ASPxWebControl instance) {
			AddErrorForHandler(ex);
			CallbackErrorMessageInternal = OnCallbackExceptionInternal(ex);
			RaiseCallbackErrorInternal(instance);
			ClearErrorForHandler();
			return CallbackErrorMessageInternal;
		}
		protected internal static string OnCallbackExceptionInternal(Exception e) {
			return HttpUtils.IsCustomErrorEnabled() ? ASPxperienceLocalizer.GetString(ASPxperienceStringId.CallbackGenericErrorText) : e.Message;
		}
		protected string GetCallbackRedirectResult(string locationUrl) {
			return (Page as Control ?? this).ResolveClientUrl(locationUrl);
		}
		protected void HandleErrorOnGetCallbackResult(IDictionary callbackResult, string errorMessage) {
			CallbackErrorMessageInternal = errorMessage;
			RaiseCallbackError();
			errorMessage = CallbackErrorMessageInternal;
			string url = ConfigurationSettings.ErrorPageUrl;
			if(!string.IsNullOrEmpty(url)) {
				url = GetCallbackRedirectResult(url); 
				callbackResult[CallbackResultProperties.Redirect] = url + ErrorQueryString(errorMessage);
			} else if(callbackResult[CallbackResultProperties.Error] == null) {
				callbackResult[CallbackResultProperties.Error] = GetCallbackErrorObject(errorMessage);
			}
		}
		protected virtual void RaiseCallbackEvent(string eventArgument) {
		}
		protected virtual void RaiseCustomDataCallbackEvent(string eventArgument) {
			CustomDataCallbackEventArgs args = new CustomDataCallbackEventArgs(eventArgument);
			OnCustomDataCallback(args);
			this.callbackResult = args.Result;
		}
		protected virtual void OnCustomDataCallback(CustomDataCallbackEventArgs e) {
			CustomDataCallbackEventHandler handler = (CustomDataCallbackEventHandler)Events[EventCustomDataCallback];
			if(handler != null)
				handler(this, e);
		}
		public static void SetCallbackErrorMessage(string message) {
			CallbackErrorMessageInternal = message;
		}
		bool IWebControlObject.IsLoading() {
			return IsLoading();
		}
		bool IWebControlObject.IsRendering() {
			return IsRendering;
		}
		bool IWebControlObject.IsDesignMode() {
			return DesignMode;
		}
		void IWebControlObject.LayoutChanged() {
			LayoutChanged();
		}
		void IWebControlObject.TemplatesChanged() {
			TemplatesChanged();
		}
		string IClientObjectOwner.ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		bool IClientObjectOwner.ClientEnabled {
			get { return ClientEnabledInternal; }
			set { ClientEnabledInternal = value; }
		}
		bool IClientObjectOwner.ClientVisible {
			get { return ClientVisibleInternal; }
			set { ClientVisibleInternal = value; }
		}
		Dictionary<string, object> IClientObjectOwner.JSProperties {
			get { return JSPropertiesInternal; }
		}
		ClientSideEventsBase IClientObjectOwner.ClientSideEvents {
			get { return ClientSideEventsInternal; }
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			PropertiesChanged(properties);
		}
		protected virtual void PropertiesChanged(PropertiesBase properties) {
			if(properties != this.renderImages && properties != this.renderStyles)
				LayoutChanged();
		}
		protected virtual ISkinOwner GetParentSkinOwner() {
			if(ParentSkinOwner != null)
				return ParentSkinOwner;
			ISkinOwner skinOwner = RenderUtils.FindParentSkinOwner(this);
			if(UseCachedObjects())
				ParentSkinOwner = skinOwner;
			return skinOwner;
		}
		protected virtual string GetSkinControlName() {
			return "Web";
		}
		protected virtual string[] GetChildControlNames() {
			return new string[] { };
		}
		protected internal string GetSSLSecureBlankUrl() {
			return Browser.IsIE ? ResourceManager.GetResourceUrl(Page, typeof(ASPxWebControl), SSLSecureBlankUrlResourceName) : "";
		}
		protected virtual string GetCssFilePath() {
			if(!string.IsNullOrEmpty(StylesInternal.CssFilePath))
				return StylesInternal.CssFilePath;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.CssFilePath))
				return ParentStyles.CssFilePath;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetCssFilePath() : string.Empty;
			}
		}
		protected virtual string GetTheme() {
			if(!string.IsNullOrEmpty(StylesInternal.Theme))
				return StylesInternal.Theme;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.Theme))
				return ParentStyles.Theme;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetTheme() : string.Empty;
			}
		}
		protected virtual void MergeControlStyle(AppearanceStyleBase style) {
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				parentSkinOwner.MergeControlStyle(style);
			MergeControlStyle(style, false);
		}
		protected virtual string GetImageFolder() {
			if(!string.IsNullOrEmpty(ImagesInternal.ImageFolder))
				return ImagesInternal.ImageFolder;
			else if(ParentImages != null && !string.IsNullOrEmpty(ParentImages.ImageFolder))
				return ParentImages.ImageFolder;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetImageFolder() : string.Empty;
			}
		}
		protected virtual string GetSpriteImageUrl() {
			if(!string.IsNullOrEmpty(ImagesInternal.SpriteImageUrl))
				return ImagesInternal.SpriteImageUrl;
			else if(ParentImages != null && !string.IsNullOrEmpty(ParentImages.SpriteImageUrl))
				return ParentImages.SpriteImageUrl;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetSpriteImageUrl() : string.Empty;
			}
		}
		protected virtual string GetSpriteCssFilePath() {
			if(!string.IsNullOrEmpty(ImagesInternal.SpriteCssFilePath))
				return ImagesInternal.SpriteCssFilePath;
			else if(ParentImages != null && !string.IsNullOrEmpty(ParentImages.SpriteCssFilePath))
				return ParentImages.SpriteCssFilePath;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetSpriteCssFilePath() : string.Empty;
			}
		}
		protected virtual string GetCssPostFix() {
			if(!string.IsNullOrEmpty(StylesInternal.CssPostfix))
				return StylesInternal.CssPostfix;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.CssPostfix))
				return ParentStyles.CssPostfix;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetCssPostFix() : string.Empty;
			}
		}
		protected virtual bool IsDefaultAppearanceEnabled() {
			return true;
		}
		protected virtual bool IsAccessibilityCompliant() {
			bool ret = AccessibilityCompliantInternal;
			if(ParentStyles != null)
				ret = ret || ParentStyles.AccessibilityCompliant;
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				ret = ret || parentSkinOwner.IsAccessibilityCompliant();
			return ret;
		}
		protected virtual bool IsNative() {
			bool ret = StylesInternal.Native;
			if(ParentStyles != null)
				ret = ret || ParentStyles.Native;
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				ret = ret || parentSkinOwner.IsNative();
			return ret;
		}
		protected virtual bool IsNativeSupported() {
			return false;
		}
		protected virtual bool IsRightToLeft() {
			DefaultBoolean value = StylesInternal.RightToLeft;
			if(value == DefaultBoolean.Default && ParentStyles != null)
				value = ParentStyles.RightToLeft;
			if(value == DefaultBoolean.Default) {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				if(parentSkinOwner != null)
					return parentSkinOwner.IsRightToLeft();
			}
			if(value == DefaultBoolean.True)
				return true;
			if(value == DefaultBoolean.False)
				return false;
			return GlobalRightToLeft == DefaultBoolean.True;
		}
		string ISkinOwner.GetControlName() {
			return GetSkinControlName();
		}
		string ISkinOwner.GetTheme() {
			return GetTheme();
		}
		void ISkinOwner.MergeControlStyle(AppearanceStyleBase style) {
			MergeControlStyle(style);
		}
		string[] ISkinOwner.GetChildControlNames() {
			return GetChildControlNames();
		}
		string ISkinOwner.GetCssFilePath() {
			return GetCssFilePath();
		}
		string ISkinOwner.GetImageFolder() {
			return GetImageFolder();
		}
		string ISkinOwner.GetSpriteImageUrl() {
			return GetSpriteImageUrl();
		}
		string ISkinOwner.GetSpriteCssFilePath() {
			return GetSpriteCssFilePath();
		}
		string ISkinOwner.GetCssPostFix() {
			return GetCssPostFix();
		}
		bool ISkinOwner.IsDefaultAppearanceEnabled() {
			return IsDefaultAppearanceEnabled();
		}
		bool ISkinOwner.IsAccessibilityCompliant() {
			return IsAccessibilityCompliant();
		}
		bool ISkinOwner.IsNative() {
			return IsNative();
		}
		bool ISkinOwner.IsNativeSupported() {
			return IsNativeSupported();
		}
		bool ISkinOwner.IsRightToLeft() {
			return IsRightToLeft();
		}
		public static string GetResourceUrl(Page page, string url) {
			return ResourceManager.GetResourceUrl(page, url);
		}
		public static string GetResourceUrl(Page page, Type type, string resourceName) {
			return ResourceManager.GetResourceUrl(page, type, resourceName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ApplyThemeInternal() {
			string resultTheme = ASPxWebControl.GlobalTheme;
			ISkinOwner parentSkinOwner = this.GetParentSkinOwner();
			if(parentSkinOwner != null && !string.IsNullOrEmpty(parentSkinOwner.GetTheme()))
				resultTheme = parentSkinOwner.GetTheme();
			if(!string.IsNullOrEmpty(Theme))
				resultTheme = Theme;
			ApplyTheme(resultTheme);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ApplyStyleSheetThemeInternal() {
			string resultTheme = !string.IsNullOrEmpty(ASPxWebControl.GlobalStyleSheetTheme) ? ASPxWebControl.GlobalStyleSheetTheme : ConfigurationSettings.StyleSheetTheme;
			ApplyTheme(resultTheme);
		}
	}
	public class ThemableControlBuilder : ControlBuilder {
		public const string
			ApplyStyleSheetSkinMethodName = "ApplyStyleSheetSkin",
			ApplyStyleSheetThemeMethodName = "ApplyStyleSheetThemeInternal",
			ThemeAttributeName = "Theme";
		private void AddApplyStyleSheetThemeMethodInvoke(CodeMemberMethod buildMethod) {
			CodeExpressionStatement applyStyleSheetThemeExpressionStatement = CreateApplyThemeExpressionStatement(buildMethod);
			if(applyStyleSheetThemeExpressionStatement != null) {
				int applyStyleSheetStatementIndex = GetApplyStyleSheetSkinStatementIndex(buildMethod);
				int applyStyleSheetThemeStatementIndex = applyStyleSheetStatementIndex != -1 ? applyStyleSheetStatementIndex + 1 : 3;
				buildMethod.Statements.Insert(applyStyleSheetThemeStatementIndex, applyStyleSheetThemeExpressionStatement);
			}
		}
		private CodeExpressionStatement CreateApplyThemeExpressionStatement(CodeMemberMethod buildMethod) {
			if(buildMethod.Statements.Count < 1) 
				return null;
			CodeVariableDeclarationStatement variableDeclarationStatement = buildMethod.Statements[0] as CodeVariableDeclarationStatement;
			if(variableDeclarationStatement == null)
				return null;
			string targetName = variableDeclarationStatement.Name;
			CodeExpression targetExpression = new CodeVariableReferenceExpression(targetName);
			CodeMethodReferenceExpression methodReferenceExpression =
				new CodeMethodReferenceExpression(targetExpression, ApplyStyleSheetThemeMethodName);
			CodeExpression methodInvokeExpression = new CodeMethodInvokeExpression(methodReferenceExpression);
			return new CodeExpressionStatement(methodInvokeExpression);
		}
		private int GetApplyStyleSheetSkinStatementIndex(CodeMemberMethod buildMethod) {
			foreach(CodeStatement statement in buildMethod.Statements) {
				CodeExpressionStatement codeExpressionStatement = statement as CodeExpressionStatement;
				if(codeExpressionStatement != null) {
					CodeMethodInvokeExpression methodInvokeExpression = codeExpressionStatement.Expression as CodeMethodInvokeExpression;
					if(methodInvokeExpression != null && methodInvokeExpression.Method.MethodName == ApplyStyleSheetSkinMethodName)
						return buildMethod.Statements.IndexOf(statement);
				}
			}
			return -1;
		}
		public override void ProcessGeneratedCode(System.CodeDom.CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType, CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod) {
			if(buildMethod != null)
				AddApplyStyleSheetThemeMethodInvoke(buildMethod);
			base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
		}
	}
	[ToolboxItem(false)]
	public abstract class ASPxInternalWebControl : ASPxWebControlBase {
		public ASPxInternalWebControl()
			: base() {
			ClientIDHelper.EnableClientIDGeneration(this);
		}
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState() {
			return null;
		}
	}
}
namespace DevExpress.Web.Internal {
	public interface IHandleCallbackError {
		string OnCallbackException(Exception e);
		void HandleErrorOnGetCallbackResult(IDictionary callbackResult, string errorMessage);
	}
	public interface IPostDataCollection {
		NameValueCollection PostDataCollection { get; }
	}
	public interface IPostBackDataHandlerEx {
		void ForceLoadPostData(string postDataKey, NameValueCollection postCollection);
	}
	public interface IWebControlObject {
		bool IsLoading();
		bool IsRendering();
		bool IsDesignMode();
		void LayoutChanged();
		void TemplatesChanged();
	}
	public interface IContentContainer {
	}
	public interface IASPxWebControl {
		void EnsureChildControls();
		void PrepareControlHierarchy();
	}
	public interface IPagerOwner {
		int InitialPageSize { get; }
	}
	public interface ISkinOwner : IPropertiesOwner {
		string GetControlName();
		string GetCssFilePath();
		string GetImageFolder();
		string GetSpriteImageUrl();
		string GetSpriteCssFilePath();
		string GetCssPostFix();
		string GetTheme();
		void MergeControlStyle(AppearanceStyleBase style);
		bool IsDefaultAppearanceEnabled();
		bool IsAccessibilityCompliant();
		bool IsNative();
		bool IsNativeSupported();
		bool IsRightToLeft();
		string[] GetChildControlNames();
	}
	public interface IParentSkinOwner : ISkinOwner {
	}
	public interface IRequiresLoadPostDataControl {
	}
	public interface IDialogFormElementRequiresLoad {
		void ForceInit();
		void ForceLoad();
	}
	public struct ComplexKey {
		public class KeyComparer : IEqualityComparer<ComplexKey> {
			public bool Equals(ComplexKey x, ComplexKey y) {
				if(x.Keys.Length == y.Keys.Length) {
					for(int i = 0; i < x.Keys.Length; i++) {
						if(x.Keys[i] != y.Keys[i])
							return false;
					}
					return true;
				}
				return false;
			}
			public int GetHashCode(ComplexKey obj) {
				int result = 0;
				object[] keys = obj.Keys;
				for(int i = 0; i < keys.Length; i++)
					result = result ^ keys[i].GetHashCode();
				return result;
			}
		}
		public object[] Keys;
		public ComplexKey(params object[] keys) {
			Keys = keys;
		}
	}
	public delegate AppearanceStyleBase CreateStyleHandler();
	public delegate ImageProperties CreateImageHandler();
	public static class ClientStateProperties {
		public const string StateObject = "stateObject";
		public const string CallbackState = "callbackState";
	}
	public static class CallbackResultProperties {
		public const string CustomProperties = "cp";
		public const string Result = "result";
		public const string Error = "error";
		public const string GeneralError = "generalError";
		public const string ErrorMessage = "message";
		public const string ErrorData = "data";
		public const string Redirect = "redirect";
		public const string ID = "id";
		public const string Html = "html";
		public const string Index = "index";
		public const string StateObject = ClientStateProperties.StateObject;
	}
}
