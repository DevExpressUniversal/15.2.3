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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum ValidationSummaryRenderMode { Table, BulletedList, OrderedList }
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxValidationSummary"),
	Designer("DevExpress.Web.Design.ASPxValidationSummaryDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxValidationSummary.bmp")]
	public class ASPxValidationSummary : ASPxWebControl, IRequiresLoadPostDataControl {
		internal const string ValidationSummaryScriptResourceName = "DevExpress.Web.Scripts.Editors.ValidationSummary.js";
		internal const string FakeItemID = "FI";
		private List<ValidationSummaryError> errors = new List<ValidationSummaryError>();
		private IList<ValidationSummaryError> designModeErrorList;
		private ValidationSummaryControl validationSummaryControl;
		public ASPxValidationSummary()
			: this(null) {
		}
		protected ASPxValidationSummary(ASPxWebControl owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return ((AppearanceStyle)ControlStyle).HorizontalAlign; }
			set { ((AppearanceStyle)ControlStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryVerticalAlign"),
#endif
		Category("Layout"), DefaultValue(VerticalAlign.NotSet), AutoFormatEnable]
		public VerticalAlign VerticalAlign {
			get { return ((AppearanceStyle)ControlStyle).VerticalAlign; }
			set { ((AppearanceStyle)ControlStyle).VerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryPaddings"),
#endif
		Category("Layout"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public Paddings Paddings {
			get { return (ControlStyle as AppearanceStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryRenderMode"),
#endif
		Category("Layout"), DefaultValue(ValidationSummaryRenderMode.Table), AutoFormatDisable]
		public ValidationSummaryRenderMode RenderMode {
			get { return (ValidationSummaryRenderMode)GetEnumProperty("RenderMode", ValidationSummaryRenderMode.Table); }
			set {
				SetEnumProperty("RenderMode", ValidationSummaryRenderMode.Table, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryErrorStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ValidationSummaryErrorStyle ErrorStyle {
			get { return Styles.Error; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryHeaderStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ValidationSummaryHeaderStyle HeaderStyle {
			get { return Styles.Header; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryHeaderText"),
#endif
		Category("Misc"), DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string HeaderText {
			get { return GetStringProperty("HeaderText", ""); }
			set {
				SetStringProperty("HeaderText", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryValidationGroup"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValidationGroup {
			get { return GetStringProperty("ValidationGroup", ""); }
			set {
				string oldValidationGroup = ValidationGroup;
				SetStringProperty("ValidationGroup", "", value);
				ValidationSummaryCollection.Instance.ReregisterValidationSummary(this, oldValidationGroup);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryShowErrorsInEditors"),
#endif
		Category("Behavior"), DefaultValue(false), Localizable(false), AutoFormatDisable]
		public bool ShowErrorsInEditors {
			get { return GetBoolProperty("ShowErrorsInEditors", false); }
			set { SetBoolProperty("ShowErrorsInEditors", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryShowErrorAsLink"),
#endif
		Category("Behavior"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool ShowErrorAsLink {
			get { return GetBoolProperty("ShowErrorAsLink", true); }
			set { SetBoolProperty("ShowErrorAsLink", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxValidationSummaryClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ValidationSummaryClientSideEvents ClientSideEvents {
			get { return (ValidationSummaryClientSideEvents)base.ClientSideEventsInternal; }
		}
		#region Hidden Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		#endregion
		internal bool HasHeader {
			get { return !string.IsNullOrEmpty(HeaderText); }
		}
		internal IList<ValidationSummaryError> Errors {
			get { return DesignMode ? DesignModeErrorList : errors; }
		}
		private IList<ValidationSummaryError> DesignModeErrorList {
			get {
				if(designModeErrorList == null) {
					const int DesignModeErrorCount = 3;
					designModeErrorList = new List<ValidationSummaryError>(DesignModeErrorCount);
					for(byte i = 1; i <= DesignModeErrorCount; i++)
						designModeErrorList.Add(new ValidationSummaryError(null, "Error message " + i + "."));
					designModeErrorList = new ReadOnlyCollection<ValidationSummaryError>(designModeErrorList);
				}
				return designModeErrorList;
			}
		}
		protected internal new void PropertyChanged(string name) {
			base.PropertyChanged(name);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ValidationSummaryCollection.Instance.RegisterValidationSummary(this);
		}
		public override void Dispose() {
			Unregister();
			base.Dispose();
		}
		private void Unregister() {
			ValidationSummaryCollection collection = ValidationSummaryCollection.Instance;
			if(collection != null)
				collection.UnregisterValidationSummary(this);
		}
		internal void SetError(IValidationSummaryEditor edit) {
			ValidationSummaryError error = null;
			string editorName = edit.ClientID;
			for(int i = 0; i < Errors.Count; i++) {
				ValidationSummaryError e = Errors[i];
				if(string.Equals(e.EditorName, editorName, StringComparison.InvariantCulture))
					error = e;
			}
			if(error == null) {
				error = new ValidationSummaryError(editorName);
				Errors.Add(error);
				if(Page != null && Page.IsCallback)
					edit.NotifyValidationSummariesToAcceptNewError = true;
			}
			if(error != null) {
				error.ErrorText = edit.ErrorText;
				LayoutChanged();
			}
		}
		internal void RemoveError(IValidationSummaryEditor edit) {
			for(int i = 0; i < Errors.Count; i++) {
				ValidationSummaryError error = Errors[i];
				if(string.Equals(error.EditorName, edit.ClientID, StringComparison.InvariantCulture))
					Errors.Remove(error);
			}
			edit.NotifyValidationSummariesToAcceptNewError = false;
			LayoutChanged();
		}
		protected ValidationSummaryControl ValidationSummaryControl {
			get { return validationSummaryControl; }
			private set { validationSummaryControl = value; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ValidationSummaryControl = null;
		}
		protected override void CreateControlHierarchy() {
			if(RenderMode == ValidationSummaryRenderMode.Table)
				ValidationSummaryControl = new ValidationSummaryTableControl(this);
			else if(RenderMode == ValidationSummaryRenderMode.BulletedList || RenderMode == ValidationSummaryRenderMode.OrderedList)
				ValidationSummaryControl = new ValidationSummaryListControl(this, RenderMode);
			Controls.Add(ValidationSummaryControl);
		}
		protected internal object[] GetInvalidEditorNames() {
			object[] editorsClientNames = new object[Errors.Count];
			for(int i = 0; i < Errors.Count; i++)
				editorsClientNames[i] = Errors[i].EditorName;
			return editorsClientNames;
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			ArrayList invalidEditorNames = GetClientObjectStateValue < ArrayList>("invalidEditors");
			for (int i = 0; i < invalidEditorNames.Count; i++) {
				string editorName = invalidEditorNames[i] as string;
				AddError(editorName);
			}
			return false;
		}
		protected void AddError(string editorName) {
			if(string.IsNullOrEmpty(editorName))
				return;
			Errors.Add(new ValidationSummaryError(editorName));
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxValidationSummary), ValidationSummaryScriptResourceName);
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientValidationSummary";
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ValidationSummaryClientSideEvents(this);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Errors.Count > 0)
				stb.AppendFormat("{0}.invalidEditorNames = {1};\n", localVarName, HtmlConvertor.ToJSON(GetInvalidEditorNames()));
			if(RenderMode != ValidationSummaryRenderMode.Table)
				stb.Append(localVarName + ".renderMode = \"l\";\n");
			if(!string.IsNullOrEmpty(ValidationGroup))
				stb.AppendFormat("{0}.validationGroup = \"{1}\";\n", localVarName, ValidationGroup);
			if(!ShowErrorAsLink)
				stb.Append(localVarName + ".showErrorAsLink = false;\n");
			if(HasHeader)
				stb.Append(localVarName + ".hasHeader = true;\n");
			AppendSampleErrorContainer(stb, localVarName);
		}
		private void AppendSampleErrorContainer(StringBuilder stb, string localVarName) {
			string sampleErrorContainerRenderResult = ValidationSummaryControl.GetSampleErrorContainerRenderResult();
			sampleErrorContainerRenderResult = sampleErrorContainerRenderResult.Replace("\t", " ").Replace("\r", "").Replace("\n", "");
			stb.AppendFormat("{0}.sampleErrorContainer = {1};\n", localVarName, HtmlConvertor.ToScript(sampleErrorContainerRenderResult));
		}
		protected ValidationSummaryStyles Styles {
			get { return (ValidationSummaryStyles)StylesInternal; }
		}
		protected ValidationSummaryStyles RenderStyles {
			get { return (ValidationSummaryStyles)RenderStylesInternal; }
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new ValidationSummaryStyles(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxValidationSummary), ASPxEditBase.EditDefaultCssResourceName);
		}
		protected internal Unit GetBulletIndent() {
			return Styles.GetBulletIndent();
		}
		private static object rootTableStyleKey = new object();
		protected internal AppearanceStyleBase GetRootTableStyle() {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CssClass = Styles.GetDefaultValidationSummaryStyle().CssClass;
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, ControlStyle.CssClass);
				return style;
			}, rootTableStyleKey);
		}
		private static object rootCellStyleKey = new object();
		protected internal AppearanceStyle GetRootCellStyle() {
			return (AppearanceStyle)CreateStyle(delegate() {
				AppearanceStyle style = new AppearanceStyle();
				style.CopyFrom(Styles.GetDefaultValidationSummaryStyle());
				style.CssClass = "";
				style.CopyFrom(Styles.GetDefaultRootCellStyle());
				style.CopyFrom(ControlStyle);
				return style;
			}, rootCellStyleKey);
		}
		private static object errorStyleKey = new object();
		protected internal ValidationSummaryErrorStyle GetErrorStyle() {
			return (ValidationSummaryErrorStyle)CreateStyle(delegate() {
				ValidationSummaryErrorStyle style = new ValidationSummaryErrorStyle();
				style.CopyFrom(Styles.GetDefaultErrorStyle());
				style.CopyFrom(RenderStyles.Error, RenderMode == ValidationSummaryRenderMode.Table );
				return style;
			}, errorStyleKey, RenderMode);
		}
		private static object headerTableStyleKey = new object();
		protected internal AppearanceStyleBase GetHeaderTableStyle() {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				return Styles.GetDefaultHeaderTableStyle();
			}, headerTableStyleKey);
		}
		private static object headerStyleKey = new object();
		protected internal ValidationSummaryHeaderStyle GetHeaderStyle() {
			return (ValidationSummaryHeaderStyle)CreateStyle(delegate() {
				ValidationSummaryHeaderStyle style = new ValidationSummaryHeaderStyle();
				style.CopyFrom(Styles.GetDefaultHeaderStyle());
				style.CopyFrom(RenderStyles.Header);
				return style;
			}, headerStyleKey);
		}
		private static object linkStyleKey = new object();
		protected internal AppearanceStyleBase GetLinkStyle() {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFrom(RenderStyles.GetDefaultHyperlinkStyle());
				style.CopyFrom(LinkStyle.Style);
				return style;
			}, linkStyleKey);
		}
		private static object tableErrorContainerStyleKey = new object();
		protected internal AppearanceStyleBase GetTableErrorContainerStyle() {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				return Styles.GetDefaultTableErrorContainerStyle();
			}, tableErrorContainerStyleKey);
		}
		private static object errorTextCellStyleKey = new object();
		protected internal AppearanceStyle GetErrorTextCellStyle() {
			return (AppearanceStyle)CreateStyle(delegate() {
				AppearanceStyle style = new AppearanceStyle();
				style.CopyFrom(Styles.GetDefaultErrorTextCellStyle());
				style.CopyBordersFrom(RenderStyles.Error);
				style.Paddings.CopyFrom(GetErrorStyle().Paddings);
				return style;
			}, errorTextCellStyleKey);
		}
		private static object listErrorContainerStyleKey = new object();
		protected internal AppearanceStyleBase GetListErrorContainerStyle() {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyle style = new AppearanceStyle();
				style.CopyFrom(Styles.GetDefaultListErrorContainerStyle());
				style.Paddings.CopyFrom(GetErrorStyle().Paddings);
				return style;
			}, listErrorContainerStyleKey);
		}
	}
}
namespace DevExpress.Web.Internal {
	public delegate void ValidationSummaryProcessingCallback(ASPxValidationSummary summary);
	public class ValidationSummaryError {
		private string editorName;
		private string errorText;
		public ValidationSummaryError(string editorName)
			: this(editorName, null) {
		}
		public ValidationSummaryError(string editorName, string errorText) {
			this.editorName = editorName;
			this.errorText = errorText;
		}
		public string EditorName {
			get { return editorName; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
	}
	public class ValidationSummaryCollection {
		#region Nested Types
		internal class EmptyValidationSummaryCollection : ValidationSummaryCollection {
			public static new readonly EmptyValidationSummaryCollection Instance = new EmptyValidationSummaryCollection();
			private EmptyValidationSummaryCollection() {
			}
			public override void RegisterValidationSummary(ASPxValidationSummary validationSummary) {
			}
			public override void UnregisterValidationSummary(ASPxValidationSummary validationSummary) {
			}
			public override void ReregisterValidationSummary(ASPxValidationSummary validationSummary, string oldValidationGroup) {
			}
			public override void OnEditorIsValidStateChanged(IValidationSummaryEditor editor) {
			}
			public override bool EditorsAllowedToShowErrors(string validationGroup) {
				return true;
			}
		}
		#endregion
		internal const string ValidationSummaryCollectionContextKey = "DxValidationSummaryCollection";
		private Dictionary<string, List<ASPxValidationSummary>> summaries = new Dictionary<string, List<ASPxValidationSummary>>();
		protected ValidationSummaryCollection() {
		}
		public static ValidationSummaryCollection Instance {
			get {
				ValidationSummaryCollection instance = null;
				HttpContext context = HttpContext.Current;
				if(context != null) {
					instance = (ValidationSummaryCollection)context.Items[ValidationSummaryCollectionContextKey];
					if(instance == null) {
						instance = new ValidationSummaryCollection();
						context.Items[ValidationSummaryCollectionContextKey] = instance;
					}
				}
				if(instance == null && EmptyInstanceRequired)
					instance = EmptyValidationSummaryCollection.Instance;
				return instance;
			}
		}
		private static bool EmptyInstanceRequired {
			get {
#if DebugTest
				return true;
#else
				HttpContext context = HttpContext.Current;
				if(context == null)
					return true;
				else {
					HttpApplication application = context.ApplicationInstance;
					if(application != null) {
						ISite site = application.Site;
						if(site != null)
							return site.DesignMode;
					}
				}
				return false;
#endif
			}
		}
		internal Dictionary<string, List<ASPxValidationSummary>> SummariesByGroup {
			get { return summaries; }
		}
		public virtual void RegisterValidationSummary(ASPxValidationSummary validationSummary) {
			List<ASPxValidationSummary> summaries = GetValidationGroupSummaries(validationSummary.ValidationGroup);
			if(!summaries.Contains(validationSummary))
				summaries.Add(validationSummary);
		}
		public virtual void UnregisterValidationSummary(ASPxValidationSummary validationSummary) {
			List<ASPxValidationSummary> summaries = GetValidationGroupSummaries(validationSummary.ValidationGroup);
			summaries.Remove(validationSummary);
		}
		public virtual void ReregisterValidationSummary(ASPxValidationSummary validationSummary, string oldValidationGroup) {
			List<ASPxValidationSummary> oldGroupSummaries = GetValidationGroupSummaries(oldValidationGroup);
			oldGroupSummaries.Remove(validationSummary);
			List<ASPxValidationSummary> newGroupSummaries = GetValidationGroupSummaries(validationSummary.ValidationGroup);
			if(!newGroupSummaries.Contains(validationSummary))
				newGroupSummaries.Add(validationSummary);
		}
		public virtual void OnEditorIsValidStateChanged(IValidationSummaryEditor editor) {
			if(editor.IsValid || !editor.IsValidationEnabled())
				RemoveError(editor);
			else
				SetError(editor);
		}
		public void OnEditorPropertyAffectingValidationSettingsChanged(IValidationSummaryEditor editor) {
			OnEditorIsValidStateChanged(editor);
		}
		public virtual bool EditorsAllowedToShowErrors(string validationGroup) {
			if(MvcUtils.RenderMode != MvcRenderMode.None)
				return true;
			List<ASPxValidationSummary> groupSummaries = GetValidationGroupVisibleSummaries(validationGroup);
			if(groupSummaries.Count == 0)
				return true;
			bool? allowed = null;
			for(int i = 0; i < groupSummaries.Count; i++) {
				ASPxValidationSummary summary = groupSummaries[i];
				if(!allowed.HasValue)
					allowed = summary.ShowErrorsInEditors;
				else {
					if(allowed.Value != summary.ShowErrorsInEditors)
						throw new InvalidOperationException("Several summaries of the same validation group have different values of the ShowErrorsInEditors property.");
				}
			}
			return allowed.HasValue ? allowed.Value : false;
		}
		protected List<ASPxValidationSummary> GetValidationGroupVisibleSummaries(string validationGroup) {
			List<ASPxValidationSummary> result = new List<ASPxValidationSummary>();
			foreach(ASPxValidationSummary summary in GetValidationGroupSummaries(validationGroup))
				if(summary.Visible)
					result.Add(summary);
			return result;
		}
		internal virtual void SetError(IValidationSummaryEditor edit) {
			ProcessValidationGroupSummaries(edit.ValidationGroup, delegate(ASPxValidationSummary summary) {
				summary.SetError(edit);
			});
		}
		internal virtual void RemoveError(IValidationSummaryEditor edit) {
			ProcessValidationGroupSummaries(edit.ValidationGroup, delegate(ASPxValidationSummary summary) {
				summary.RemoveError(edit);
			});
		}
		internal List<ASPxValidationSummary> GetValidationGroupSummaries(string validationGroup) {
			List<ASPxValidationSummary> groupSummaries;
			if(validationGroup == null) {
				groupSummaries = new List<ASPxValidationSummary>();
				foreach(KeyValuePair<string, List<ASPxValidationSummary>> pair in SummariesByGroup)
					groupSummaries.AddRange(pair.Value);
			} else {
				if(!SummariesByGroup.TryGetValue(validationGroup, out groupSummaries)) {
					groupSummaries = new List<ASPxValidationSummary>();
					SummariesByGroup.Add(validationGroup, groupSummaries);
				}
			}
			return groupSummaries;
		}
		private void ProcessValidationGroupSummaries(string validationGroup, ValidationSummaryProcessingCallback processingProc) {
			List<ASPxValidationSummary> summaries = GetValidationGroupSummaries(validationGroup);
			foreach(ASPxValidationSummary summary in summaries)
				processingProc(summary);
		}
	}
	public interface IValidationSummaryEditor {
		string ID { get; set; }
		string ClientID { get; }
		string ErrorText { get; set; }
		bool IsValid { get; set; }
		bool NotifyValidationSummariesToAcceptNewError { get; set; }
		string ValidationGroup { get; set; }
		bool IsValidationEnabled();
	}
}
