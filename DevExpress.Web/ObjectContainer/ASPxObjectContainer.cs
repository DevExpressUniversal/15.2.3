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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class ObjectContainerBuilder : ControlBuilder {
		protected ObjectType fObjectType;
		public ObjectType ObjectType {
			get { return fObjectType; }
		}
		protected ObjectType GetObjectType(Type type, IDictionary attribs) {
			string objectTypeAttrib = (string)attribs["ObjectType"];
			return !string.IsNullOrEmpty(objectTypeAttrib) ? (ObjectType)TypeDescriptor.GetConverter(typeof(ObjectType)).ConvertFromString(objectTypeAttrib) : ObjectType.Auto;
		}
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			fObjectType = GetObjectType(type, attribs);
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
}
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public enum ObjectType { Auto, Image, Flash, Video, Audio, QuickTime, Html5Video, Html5Audio };
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxObjectContainer"),
	DataBindingHandler("DevExpress.Web.Design.ObjectContainerDataBindingHandler, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DefaultProperty("ObjectUrl"), ControlBuilder(typeof(ObjectContainerBuilder)),
	Designer("DevExpress.Web.Design.ASPxObjectContainerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxObjectContainer.bmp")
	]
	public class ASPxObjectContainer : ASPxWebControl {
		protected internal const string ObjectContainerScriptResourceName = WebScriptsResourcePath + "ObjectContainer.js";
		private static Dictionary<ObjectType, Type> fObjectPropertiesTypes = new Dictionary<ObjectType, Type>();
		private WebControl fObjectContainer = null;
		private ObjectProperties fObjectProperties = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerAlternateText"),
#endif
		DefaultValue(""), Category("Accessibility"), AutoFormatDisable, Localizable(true)]
		public string AlternateText {
			get { return GetStringProperty("AlternateText", String.Empty); }
			set { SetStringProperty("AlternateText", String.Empty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border {
			get { return base.Border; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom {
			get { return base.BorderBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft {
			get { return base.BorderLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight {
			get { return base.BorderRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop {
			get { return base.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ObjectContainerClientSideEvents ClientSideEvents {
			get { return (ObjectContainerClientSideEvents)base.ClientSideEventsInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return StylesInternal.CssPostfix; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath {
			get { return StylesInternal.CssFilePath; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxObjectContainerWidth")]
#endif
		public override Unit Width {
			get { return base.Width; }
			set {
				base.Width = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxObjectContainerHeight")]
#endif
		public override Unit Height {
			get { return base.Height; }
			set {
				base.Height = value;
				LayoutChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ObjectType ActualObjectType {
			get {
				if(ObjectType == ObjectType.Auto && ObjectUrl != "")
					return ObjectTypeManager.GetTypeByUrl(ObjectUrl, false);
				else if(ObjectType == ObjectType.Auto && !DesignMode) 
					return ObjectType.Image;
				return ObjectType;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerObjectProperties"),
#endif
		Editor("DevExpress.Web.Design.ObjectPropertiesEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		RefreshProperties(RefreshProperties.All),
		TypeConverter("DevExpress.Web.Design.ObjectPropertiesConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public ObjectProperties ObjectProperties {
			get {
				return fObjectProperties;
			}
			set {
				fObjectProperties = value;
				if(fObjectProperties != null)
					fObjectProperties.SetObjectContainer(this);
				LayoutChanged();
			}
		}
		[DefaultValue(ObjectType.Auto), Browsable(false), AutoFormatDisable,
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ObjectType ObjectType {
			get {
				return (ObjectType)GetEnumProperty("ObjectType", ObjectType.Auto);
			}
			set {
				if(ObjectType != value) {
					SetEnumProperty("ObjectType", ObjectType.Auto, value);
					ResetObjectProperties();
					if(ObjectType != ObjectType.Auto)
						fObjectProperties = ObjectPropertiesInternal;
					PropertyChanged("ObjectType");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxObjectContainerObjectUrl"),
#endif
		Category("Misc"), DefaultValue(""), Localizable(false), AutoFormatDisable, Bindable(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, RefreshProperties(RefreshProperties.All)]
		public string ObjectUrl {
			get { return GetStringProperty("ObjectUrl", String.Empty); }
			set {
				SetStringProperty("ObjectUrl", String.Empty, value);
				if(ObjectType == ObjectType.Auto)
					ResetObjectProperties();
				LayoutChanged();
			}
		}
		protected internal void ResetObjectProperties() {
			fObjectProperties = null;
		}
		protected internal ObjectProperties ObjectPropertiesInternal {
			get {
				if(ObjectProperties == null) {
					ObjectType type = ActualObjectType;
					if(ObjectType == ObjectType.Auto && !ObjectTypeManager.HasType(ObjectUrl, false) &&
							ObjectUrl != "" && !DesignMode)
						throw new HttpException(StringResources.ObjectContainer_ExtensionRecognized);
					Type objectPropertiesType = ObjectPropertiesTypes[type];
					if(objectPropertiesType != null) {
						ObjectProperties properties = CreateObjectProperties(objectPropertiesType);
						if(DesignMode)
							return properties;
						else
							fObjectProperties = properties;
					} else {
						throw new HttpException(string.Format(StringResources.ObjectContainer_CantCreateObjectProperties, type));
					}
				}
				return ObjectProperties;
			}
		}
		protected internal static Dictionary<ObjectType, Type> ObjectPropertiesTypes {
			get { return fObjectPropertiesTypes; }
			set { fObjectPropertiesTypes = value; }
		}
		protected internal ObjectContainerImages Images {
			get { return (ObjectContainerImages)ImagesInternal; }
		}
		static ASPxObjectContainer() {
			ObjectPropertiesTypes.Add(ObjectType.Auto, typeof(ObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.Image, typeof(ImageObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.Flash, typeof(FlashObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.Video, typeof(VideoObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.Audio, typeof(AudioObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.QuickTime, typeof(QuickTimeObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.Html5Video, typeof(Html5VideoObjectProperties));
			ObjectPropertiesTypes.Add(ObjectType.Html5Audio, typeof(Html5AudioObjectProperties));
		}
		public ASPxObjectContainer()
			: base() {
		}
		protected internal ObjectProperties CreateObjectProperties(Type type) {
			object[] parameters = new Object[] { this };
			return (ObjectProperties)Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override void ClearControlFields() {
			fObjectContainer = null;
			ObjectPropertiesInternal.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			fObjectContainer = ObjectPropertiesInternal.CreateObjectControl();
			Controls.Add(fObjectContainer);
		}
		protected override void PrepareControlHierarchy() {
			if(fObjectContainer != null) {
				RenderUtils.AssignAttributes(this, fObjectContainer);
				RenderUtils.SetVisibility(fObjectContainer, IsClientVisible(), true);
				GetControlStyle().AssignToControl(fObjectContainer, AttributesRange.All);
				ObjectPropertiesInternal.PrepareControlHierarchy(fObjectContainer);
			}
		}
		protected override bool HasContent() {
			return ObjectUrl != "";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ObjectPropertiesInternal });
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ObjectContainerClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ObjectContainerImages(this);
		}
		protected internal ImageProperties GetErrorImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.ErrorImageName);
		}
		protected internal ImageProperties GetImageImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.ImageImageName);
		}
		protected internal ImageProperties GetFlashImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.FlashImageName);
		}
		protected internal ImageProperties GetVideoImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.VideoImageName);
		}
		protected internal ImageProperties GetAudioImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.AudioImageName);
		}
		protected internal ImageProperties GetQuickTimeImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.QuickTimeImageName);
		}
		protected internal ImageProperties GetHtml5VideoImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.Html5VideoImageName);
		}
		protected internal ImageProperties GetHtml5AudioImage() {
			return Images.GetImageProperties(Page, ObjectContainerImages.Html5AudioImageName);
		}
		protected override bool HasClientInitialization() {
			return base.HasClientInitialization() || ObjectPropertiesInternal.HasClientInitialization();
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || ObjectPropertiesInternal.HasFunctionalityScripts();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxObjectContainer), ObjectContainerScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(ObjectPropertiesInternal.NeedFixObjectBounds())
				stb.Append(localVarName + ".needFixObjectBounds = true;\n");
			if(IsClientSideEventsAssigned() && ClientSideEvents.FlashScriptCommand != "") {
				stb.AppendFormat("function {0}_DoFSCommand(command, args)", this.ClientID);
				stb.Append(" {\n  ASPx.GetControlCollection().Get('" + this.ClientID + "').DoFlashScriptCommand(command, args);\n");
				stb.Append("}\n");
				stb.Append("if (navigator.appName && navigator.appName.indexOf(\"Microsoft\") != -1 && navigator.userAgent.indexOf(\"Windows\") != -1 && navigator.userAgent.indexOf(\"Windows 3.1\") == -1) {\n");
				stb.Append("  document.write('<script type=\"text/vbscript\" language=\"vbscript\"\\> \\n');\n");
				stb.Append("  document.write('on error resume next \\n');\n");
				stb.AppendFormat("  document.write('Sub {0}_FSCommand(ByVal command, ByVal args)\\n');\n", this.ClientID);
				stb.AppendFormat("  document.write('  call {0}_DoFSCommand(command, args)\\n');\n", this.ClientID);
				stb.Append("  document.write('end sub\\n');\n");
				stb.Append("  document.write('</script\\> \\n');\n");
				stb.Append("}\n");
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientObjectContainer";
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(ControlStyle);
		}
		protected internal string GetResourceImageUrl(string name) {
			return ResourceManager.GetResourceUrl(Page, typeof(ASPxObjectContainer), name);
		}
		protected bool ShouldSerializeObjectProperties() {
			return ObjectType != ObjectType.Auto;
		}
	}
}
