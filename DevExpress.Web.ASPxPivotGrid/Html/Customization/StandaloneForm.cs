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
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Web.ASPxPivotGrid {
	[
	DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxPivotGrid"),
	Designer("DevExpress.Web.ASPxPivotGrid.Design.PivotCustomizationControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	ToolboxData("<{0}:ASPxPivotCustomizationControl Width=\"250px\" Height=\"330px\" runat=\"server\"></{0}:ASPxPivotCustomizationControl>")
	]
	public class ASPxPivotCustomizationControl : 
				ASPxWebControl, IRelatedControl, ISupportsFieldsCustomization, IRequiresLoadPostDataControl {
		public ASPxPivotCustomizationControl() {
			this.layout = CustomizationFormLayout.StackedDefault;
			this.allowedLayouts = CustomizationFormAllowedLayouts.All;
			this.deferredUpdates = false;
			this.allowSort = false;
			this.allowFilter = false;
			this.EnableViewState = true;
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlClientInstanceName"),
#endif
 AutoFormatDisable,
		Category("Client-Side"), DefaultValue(""), Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPivotCustomization";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(localVarName).Append(".ContainerID = \"").Append(this.ClientID).AppendLine("\";");
			if(IsPivotGridBound)
				stb.Append(localVarName).Append(".PivotGridID = \"").Append(PivotGrid.ClientID).AppendLine("\";");
			if(!IsPivotGridBound)
				stb.Append(localVarName).Append(".InitForm();");
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override string GetSkinControlName() {
			return "PivotGrid";
		}
		protected internal new void PropertyChanged(string name) {
			base.PropertyChanged(name);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(IsPivotGridBound && !DesignMode)
				PivotGrid.ContainsCustomizationFieldsForm = true;
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			CustomizationFields.ReadPostData();
			return base.LoadPostData(postCollection);
		}
		bool ContentVisible { get { return Visible; } }
		ASPxPivotGrid MasterPivotGrid { get { return (this as IRelatedControl).MasterControl as ASPxPivotGrid; } }
		ASPxPivotGrid fakePivotGrid;
		ASPxPivotGrid FakePivotGrid {
			get {
				if(fakePivotGrid == null)
					fakePivotGrid = new ASPxPivotGrid();
				return fakePivotGrid;
			}
		}
		bool IsPivotGridBound { get { return MasterPivotGrid != null; } }
		ASPxPivotGrid PivotGrid {
			get { return IsPivotGridBound ? MasterPivotGrid : FakePivotGrid; }
		}
		ISkinOwner SkinOwner { get { return IsPivotGridBound ? PivotGrid as ISkinOwner : this as ISkinOwner; } }
		PivotGridHtmlCustomizationFields customizationFields;
		RelatedControlDefaultImplementation relatedControlImpl;
		protected RelatedControlDefaultImplementation RelatedControlImpl {
			get { 
				if (relatedControlImpl == null)
					relatedControlImpl = CreateRelatedControl();
				return relatedControlImpl;
			}
		}
		protected virtual RelatedControlDefaultImplementation CreateRelatedControl() {
			return new RelatedControlDefaultImplementation(this, this);
		}
		WebControl innerDiv;
		CustomizationFormLayout layout;
		CustomizationFormAllowedLayouts allowedLayouts;
		bool deferredUpdates;
		bool allowSort;
		bool allowFilter;
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			RelatedControlImpl.OnLoad();
			if(IsPivotGridBound)
				ParentStyles = MasterPivotGrid.Styles;
		}
		protected override void CreateControlHierarchy() {
			this.Enabled = DesignMode ? ASPxPivotGridID.Length != 0 : IsPivotGridBound;
			CreateOuterControlHierarchy();
			if(ContentVisible)
				CreateControlContentHierarchy();
			base.CreateControlHierarchy();
		}
		protected internal PivotGridHtmlCustomizationFields CustomizationFields {
			get {
				if(customizationFields == null)
					customizationFields = CreateCustomizationFields();
				return customizationFields;
			}
		}
		protected virtual PivotGridHtmlCustomizationFields CreateCustomizationFields() {
			return new PivotGridHtmlCustomizationFields(this);
		}
		WebControl InnerDiv {
			get {
				if(innerDiv == null)
					CreateInnerDiv();
				return innerDiv;
			}
		}
		void CreateInnerDiv() {
			innerDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			innerDiv.Height = Unit.Percentage(100);
			innerDiv.ID = "InnerDiv";
		}
		void ResetCustomizationFields() {
			customizationFields = null;
			innerDiv = null;
		}
		internal void ResetContentControlHierarchy() {
			base.ResetControlHierarchy();
		}
		void CreateOuterControlHierarchy() {
			if(Border.BorderStyle == System.Web.UI.WebControls.BorderStyle.None)
				this.Style.Add("border", "none");
		}
		void CreateControlContentHierarchy() {
			ResetCustomizationFields();
			InnerDiv.Controls.Add(CustomizationFields);
			Controls.Add(InnerDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(HasContent()) {
				PrepareOuterControlHierarchy();
				if(ContentVisible)
					PrepareControlContentHierarchy();
			}
		}
		void PrepareOuterControlHierarchy() {
			RenderUtils.SetStyleStringAttribute(this, "display", ContentVisible ? "" : "none");
			(this as ISupportsFieldsCustomization).Styles.ApplyCustomizationFormStyle(this);
		}
		void PrepareControlContentHierarchy() {
			CustomizationFields.Width = Unit.Percentage(100);
			CustomizationFields.Height = Unit.Percentage(100);
		}
		internal void PrepareLayoutMenu() {
			CustomizationFields.PrepareLayoutMenu();
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlImages"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()]
		public PivotCustomizationFormImages Images { get { return (PivotCustomizationFormImages)ImagesInternal; } }
		protected override ImagesBase CreateImages() {
			return new PivotCustomizationFormImages(SkinOwner);
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlASPxPivotGridID"),
#endif
		DefaultValue(""), Themeable(false), AutoFormatDisable(), IDReferenceProperty(typeof(ASPxPivotGrid)),
		TypeConverter("DevExpress.Web.ASPxPivotGrid.Design.PivotGridIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)
		]
		public string ASPxPivotGridID {
			get { return (this as IRelatedControl).MasterControlID; }
			set { 
				string lastID = ASPxPivotGridID;
				if(Initialized && lastID != value && PivotGrid != null && PivotGrid.ID != value && PivotGrid.ID == lastID)
					PivotGrid.ContainsCustomizationFieldsForm = false;			
				(this as IRelatedControl).MasterControlID = value; 
				if(Loaded && lastID != value) {
 					if(PivotGrid != null)
	 					PivotGrid.MasterControlImplementation.RemoveInnerRelatedControls();
   					RelatedControlImpl.OnLoad();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlLayout"),
#endif
		DefaultValue(CustomizationFormLayout.StackedDefault), XtraSerializableProperty(), NotifyParentProperty(true),
		Browsable(true)
		]
		public CustomizationFormLayout Layout {
			get { return layout; }
			set {
				if(layout == value)
					return;
				if(!CustomizationFormEnumExtensions.IsLayoutAllowed(AllowedLayouts, value))
					return;
				layout = value;
				SettingsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlAllowedLayouts"),
#endif
		Editor(typeof(EditorLoader), typeof(UITypeEditor)),
		EditorLoader("DevExpress.Utils.Editors.AttributesEditor", AssemblyInfo.SRAssemblyUtils),
		DefaultValue(CustomizationFormAllowedLayouts.All), XtraSerializableProperty(), NotifyParentProperty(true),
		Browsable(true)
		]
		public CustomizationFormAllowedLayouts AllowedLayouts {
			get { return allowedLayouts; }
			set {
				if(allowedLayouts == value)
					return;
				allowedLayouts = value;
				SettingsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlDeferredUpdates"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		Browsable(true)
		]
		public bool DeferredUpdates {
			get {
				return deferredUpdates;
			}
			set {
				if(deferredUpdates == value)
					return;
				deferredUpdates = value;
				SettingsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlAllowSort"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		Browsable(true)
		]
		public bool AllowSort {
			get {
				return allowSort;
			}
			set {
				if(allowSort == value)
					return;
				allowSort = value;
				SettingsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotCustomizationControlAllowFilter"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		Browsable(true)
		]
		public bool AllowFilter {
			get {
				return allowFilter;
			}
			set {
				if(allowFilter == value)
					return;
				allowFilter = value;
				SettingsChanged();
			}
		}
		internal void Refresh() {
			ResetControlHierarchy();
		}
		protected void SettingsChanged() {
			ResetControlHierarchy();
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxPivotCustomizationControl), PivotGridWebData.PivotGridDefaultCssResourceName);
		}
		#region IRelatedControl Members
		string IRelatedControl.ClientObjectId { get { return this.ClientID; } }
		bool IRelatedControl.IsExternal { get { return RelatedControlImpl.IsExternal; } set { RelatedControlImpl.IsExternal = value; } }
		IMasterControl IRelatedControl.MasterControl { get { return RelatedControlImpl.MasterControl; } }
		bool IRelatedControl.SuppressCallbackResult { get { return RelatedControlImpl.SuppressCallbackResult; } set { RelatedControlImpl.SuppressCallbackResult = value; } }
		[Browsable(false)]
		string IRelatedControl.MasterControlID {
			get { return GetStringProperty("MasterCtrlId", String.Empty); }
			set {
				SetStringProperty("MasterCtrlId", String.Empty, value);
				LayoutChanged();
			}
		}
		#endregion
		#region ISupportsCallbackResult Members
		CallbackResult ISupportsCallbackResult.CalcCallbackResult() {
			if(RelatedControlImpl.SuppressCallbackResult)
				return CallbackResult.Empty;
			EnsureChildControls();
			CreateControlHierarchy();
			FinalizeCreateControlHierarchy();
			PrepareControlHierarchy();
			return CalcCallbackResultCore();
		}
		CallbackResult CalcCallbackResultCore() {
			CalcCallbackResultHelper helper = new CalcCallbackResultHelper(this, InnerDiv);
			CallbackResult result = helper.CalcCallbackResult();
			if(!ContentVisible)
				result.InnerHtml = String.Empty;
			return result;
		}
		#endregion
		#region ISupportsFieldsCustomization Members
		PivotGridWebData ISupportsFieldsCustomization.Data {
			get { return PivotGrid.Data; }
		}
		PivotGridStyles ISupportsFieldsCustomization.Styles {
			get { return PivotGrid.Styles; }
		}
		string ISupportsFieldsCustomization.UniqueID {
			get { return IsPivotGridBound ? PivotGrid.UniqueID : this.UniqueID; }
		}
		string ISupportsFieldsCustomization.ClientID {
			get { return PivotGrid.ClientID; }
		}
		CustomizationFormStyle ISupportsFieldsCustomization.FormStyle {
			get { return CustomizationFormStyle.Excel2007; }
		}
		CustomizationFormLayout ISupportsFieldsCustomization.FormLayout {
			get { return Layout; }
			set { Layout = value; }
		}
		bool ISupportsFieldsCustomization.DeferredUpdates {
			get { return DeferredUpdates; }
			set { DeferredUpdates = value; }
		}
		bool ISupportsFieldsCustomization.AllowSortInForm {
			get { return AllowSort; }
		}
		bool ISupportsFieldsCustomization.AllowFilterInForm {
			get { return AllowFilter; }
		}
		ASPxPivotGridPopupMenu ISupportsFieldsCustomization.LayoutMenu {
			get { return (PivotGrid as ISupportsFieldsCustomization).LayoutMenu; }
		}
		PivotCustomizationFormImages ISupportsFieldsCustomization.Images {
			get { return Images; }
		}
		CustomizationFormAllowedLayouts ISupportsFieldsCustomization.AllowedFormLayouts {
			get { return AllowedLayouts; }
		}
		#endregion
	}
}
