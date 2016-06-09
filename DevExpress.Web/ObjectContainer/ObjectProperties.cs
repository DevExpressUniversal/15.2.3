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
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Drawing;
using System.Text;
using System.Drawing.Design;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class ObjectPropertiesBuilder : ControlBuilder {
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			ObjectContainerBuilder containerBuilder = parentBuilder as ObjectContainerBuilder;
			if(containerBuilder != null)
				type = ASPxObjectContainer.ObjectPropertiesTypes[containerBuilder.ObjectType];
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
}
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public enum AllowScriptAccess { SameDomain, Always, Never };
	public enum FlashAlign { NotSet, Left, Top, Right, Bottom, TopLeft, TopRight, BottomLeft, BottomRight };
	public enum HtmlAlign { NotSet, Left, Right, Top, Bottom };
	public enum EmbedMethod { TwiceCooked, Satay };
	public enum RenderTag { IMG, OBJECT };
	public enum Quality { Low, High, AutoLow, AutoHigh, Best };
	public enum Scale { ShowAll, NoBorder, ExactFit, NoScale };
	public enum UIMode { Invisible, None, Mini, Full };
	public enum WindowMode { None, Window, Opaque, Transparent };
	public enum Html5PreloadMode { None, Metadata, Auto };
	[TypeConverter(typeof(ExpandableObjectConverter)), ControlBuilder(typeof(ObjectPropertiesBuilder))]
	public class ObjectProperties : StateManager {
		private ASPxObjectContainer fObjectContainer;
		private LiteralControl fText = null;
		protected internal virtual ObjectType ObjectType {
			get { return ObjectType.Auto; }
		}
		protected virtual HtmlTextWriterTag TagKey {
			get { return RenderUtils.Browser.IsIE ? HtmlTextWriterTag.Object : HtmlTextWriterTag.Embed; }
		}
		protected virtual string Type {
			get { return ""; }
		}
		protected string ObjectUrl {
			get { return ObjectContainer.ResolveClientUrl(ObjectContainer.ObjectUrl); }
		}
		protected ASPxObjectContainer ObjectContainer {
			get { return fObjectContainer; }
		}
		public ObjectProperties() {
		}
		public ObjectProperties(ASPxObjectContainer objectContainer) {
			fObjectContainer = objectContainer;
		}
		public virtual void Assign(ObjectProperties objectProperties) {
		}
		protected internal virtual void ClearControlFields() {
			fText = null;
		}
		protected internal WebControl CreateObjectControl() {
			WebControl containerControl;
			switch(ObjectType) {
				case ObjectType.Html5Video:
					containerControl = RenderUtils.CreateWebControl("video");
					break;
				case ObjectType.Html5Audio:
					containerControl = RenderUtils.CreateWebControl("audio");
					break;
				default:
					containerControl = RenderUtils.CreateWebControl(TagKey);
					break;
			}
			if(NeedCreateSubControls())
				CreateSubControls(containerControl);
			return containerControl;
		}
		protected virtual void CreateSubControls(WebControl containerControl) {
			if(ObjectContainer.AlternateText != "") {
				fText = RenderUtils.CreateLiteralControl();
				containerControl.Controls.Add(fText);
			}
		}
		protected internal virtual void PrepareControlHierarchy(WebControl containerControl) {
			if(fText != null) {
				string altText = ObjectContainer.HtmlEncode(ObjectContainer.AlternateText);
				if(!IsObject())
					altText = "<noembed>" + altText + "</noembed>";
				fText.Text = altText;
			}
			AddAttributes(containerControl);
		}
		protected virtual void AddAttributes(WebControl containerControl) {
			if(Type != "")
				containerControl.Attributes.Add("type", Type);
		}
		protected internal virtual bool RenderAsObject() {
			return true;
		}
		protected internal virtual bool NeedFixObjectBounds() {
			return false;
		}
		protected internal virtual bool NeedCreateSubControls() {
			return true;
		}
		protected internal virtual bool HasClientInitialization() {
			return false;
		}
		protected internal virtual bool HasFunctionalityScripts() {
			return false;
		}
		protected internal void SetObjectContainer(ASPxObjectContainer objectContainer) {
			fObjectContainer = objectContainer;
		}
		protected bool IsObject() {
			return TagKey.Equals(HtmlTextWriterTag.Object);
		}
		protected void LayoutChanged() {
			if(ObjectContainer as IWebControlObject != null)
				(ObjectContainer as IWebControlObject).LayoutChanged();
		}
	}
	public class ImageObjectProperties : ObjectProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageObjectPropertiesDescriptionUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string DescriptionUrl {
			get { return GetStringProperty("DescriptionUrl", String.Empty); }
			set { SetStringProperty("DescriptionUrl", String.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageObjectPropertiesImageAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(ImageAlign.NotSet)]
		public ImageAlign ImageAlign {
			get { return (ImageAlign)GetEnumProperty("ImageAlign", ImageAlign.NotSet); }
			set {
				SetEnumProperty("ImageAlign", ImageAlign.NotSet, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageObjectPropertiesImageMapName"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public virtual string ImageMapName {
			get { return GetStringProperty("ImageMapName", ""); }
			set { SetStringProperty("ImageMapName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageObjectPropertiesIsPng"),
#endif
		NotifyParentProperty(true), DefaultValue(false),
		Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsPng {
			get { return GetBoolProperty("IsPng", false); }
			set { SetBoolProperty("IsPng", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageObjectPropertiesRenderTag"),
#endif
		NotifyParentProperty(true), DefaultValue(RenderTag.IMG)]
		public RenderTag RenderTag {
			get { return (RenderTag)GetEnumProperty("RenderTag", RenderTag.IMG); }
			set {
				SetEnumProperty("RenderTag", RenderTag.IMG, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageObjectPropertiesGenerateEmptyAlternateText"),
#endif
		NotifyParentProperty(true), DefaultValue(false), Localizable(true)]
		public bool GenerateEmptyAlternateText {
			get { return GetBoolProperty("GenerateEmptyAlternateText", false); }
			set { SetBoolProperty("GenerateEmptyAlternateText", false, value); }
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.Image; }
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return RenderAsObject() ? HtmlTextWriterTag.Object : HtmlTextWriterTag.Img;
			}
		}
		protected override string Type {
			get {
				return RenderAsObject() ? MimeTypeManager.GetType(ObjectUrl, false) : "";
			}
		}
		public ImageObjectProperties()
			: base() {
		}
		public ImageObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			ImageObjectProperties imageProperties = objectProperties as ImageObjectProperties;
			if(imageProperties != null) {
				DescriptionUrl = imageProperties.DescriptionUrl;
				ImageAlign = imageProperties.ImageAlign;
				ImageMapName = imageProperties.ImageMapName;
				RenderTag = imageProperties.RenderTag;
				GenerateEmptyAlternateText = imageProperties.GenerateEmptyAlternateText;
			}
		}
		protected override void AddAttributes(WebControl containerControl) {
			base.AddAttributes(containerControl);
			if(ImageMapName != "")
				containerControl.Attributes.Add("usemap", ImageMapName);
			if(!RenderAsObject()) {
				if(ObjectContainer.AlternateText != "" || GenerateEmptyAlternateText)
					containerControl.Attributes.Add("alt", ObjectContainer.AlternateText);
				if(DescriptionUrl != "")
					containerControl.Attributes.Add("longdesc", DescriptionUrl);
				if(ImageAlign != ImageAlign.NotSet)
					containerControl.Attributes.Add("align", ImageAlign.ToString().ToLower());
				if(ObjectUrl != "") {
					SetImageSrc(containerControl, "src");
				}
			} else {
				if(ObjectUrl != "") {
					SetImageSrc(containerControl, "data");
				}
			}
		}
		private void SetImageSrc(WebControl control, string attrName) {
			control.Attributes.Add(attrName, ObjectUrl);
		}
		protected internal override bool NeedFixObjectBounds() {
			return RenderUtils.Browser.IsIE && RenderAsObject() &&
				ObjectTypeManager.HasType(ObjectUrl, true);
		}
		protected internal override bool HasClientInitialization() {
			return NeedFixObjectBounds();
		}
		protected internal override bool RenderAsObject() {
			return RenderTag == RenderTag.OBJECT && (!RenderUtils.Browser.IsIE ||
				(!ObjectContainer.Width.IsEmpty && !ObjectContainer.Height.IsEmpty &&
				ObjectTypeManager.HasType(ObjectUrl, true)));
		}
		protected internal override bool NeedCreateSubControls() {
			return RenderAsObject();
		}
	}
	public abstract class MediaObjectProperties : ObjectProperties {
		private LiteralControl fParams = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("MediaObjectPropertiesPluginVersion"),
#endif
		NotifyParentProperty(true), DefaultValue("6,4,7,1112"), Localizable(false)]
		public virtual string PluginVersion {
			get { return GetStringProperty("PluginVersion", "6,4,7,1112"); }
			set { SetStringProperty("PluginVersion", "6,4,7,1112", value); }
		}
		protected override string Type {
			get { return MimeTypeManager.GetType(ObjectUrl, false); }
		}
		protected virtual string ClassId {
			get { return "clsid:6BF52A52-394A-11d3-B153-00C04F79FAA6"; }
		}
		protected virtual string PluginsPage {
			get { return string.Format("http{0}://www.microsoft.com/Windows/MediaPlayer/", RenderUtils.IsSecureConnection ? "s" : ""); }
		}
		protected virtual string CodeBase {
			get { return string.Format("http{0}://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab", RenderUtils.IsSecureConnection ? "s" : ""); }
		}
		public MediaObjectProperties()
			: base() {
		}
		public MediaObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			MediaObjectProperties mediaProperties = objectProperties as MediaObjectProperties;
			if(mediaProperties != null)
				PluginVersion = mediaProperties.PluginVersion;
		}
		protected internal override void ClearControlFields() {
			base.ClearControlFields();
			fParams = null;
		}
		protected override void CreateSubControls(WebControl containerControl) {
			base.CreateSubControls(containerControl);
			if(IsObject())
				CreateParamsControl(containerControl);
		}
		protected internal override void PrepareControlHierarchy(WebControl containerControl) {
			base.PrepareControlHierarchy(containerControl);
			if(fParams != null)
				PrepareParamsControl(fParams);
		}
		protected override void AddAttributes(WebControl containerControl) {
			base.AddAttributes(containerControl);
			if(IsObject())
				AddObjectAttributes(containerControl);
			else
				AddEmbedAttributes(containerControl);
		}
		protected virtual void AddObjectAttributes(WebControl containerControl) {
			if(ClassId != "")
				containerControl.Attributes.Add("classid", ClassId);
			if(CodeBase != "")
				containerControl.Attributes.Add("codebase", CodeBase + "#version=" + PluginVersion);
		}
		protected virtual void AddEmbedAttributes(WebControl containerControl) {
			if(!RenderUtils.IsHtml5Mode(containerControl))
				containerControl.Attributes.Add("name", ObjectContainer.ClientID);
			if(PluginsPage != "")
				containerControl.Attributes.Add("pluginspage", PluginsPage);
			if(ObjectUrl != "")
				containerControl.Attributes.Add("src", ObjectUrl);
		}
		protected virtual void CreateParamsControl(WebControl containerControl) {
			fParams = RenderUtils.CreateLiteralControl();
			containerControl.Controls.Add(fParams);
		}
		protected void PrepareParamsControl(LiteralControl control) {
			StringBuilder stb = new StringBuilder();
			GetParamsControlText(stb);
			control.Text = stb.ToString();
		}
		protected virtual void GetParamsControlText(StringBuilder stb) {
		}
		protected string ColorToHexString(System.Drawing.Color color) {
			return "#" + color.R.ToString("X2", null) + color.G.ToString("X2", null) + color.B.ToString("X2", null);
		}
	}
	public class FlashObjectProperties : MediaObjectProperties {
		private static Dictionary<FlashAlign, string> fFlashAlignValues = new Dictionary<FlashAlign, string>();
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesPluginVersion"),
#endif
		NotifyParentProperty(true), DefaultValue("6,0,0,0"), Localizable(false)]
		public override string PluginVersion {
			get { return GetStringProperty("PluginVersion", "6,0,0,0"); }
			set { SetStringProperty("PluginVersion", "6,0,0,0", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesAllowScriptAccess"),
#endif
		NotifyParentProperty(true), DefaultValue(AllowScriptAccess.SameDomain)]
		public AllowScriptAccess AllowScriptAccess {
			get { return (AllowScriptAccess)GetEnumProperty("AllowScriptAccess", AllowScriptAccess.SameDomain); }
			set { SetEnumProperty("AllowScriptAccess", AllowScriptAccess.SameDomain, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesBase"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string Base {
			get { return GetStringProperty("Base", ""); }
			set { SetStringProperty("Base", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesDeviceFont"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool DeviceFont {
			get { return GetBoolProperty("DeviceFont", false); }
			set { SetBoolProperty("DeviceFont", false, value); }
		}
#pragma warning disable 618
		protected internal EmbedMethod EmbedMethodInternal {
			get { return EmbedMethod; }
			set { EmbedMethod = value; }
		}
#pragma warning restore 618
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property is obsolete. Now a property value is selected automatically."),
		NotifyParentProperty(true), DefaultValue(EmbedMethod.TwiceCooked)]
		public EmbedMethod EmbedMethod {
			get { return (EmbedMethod)GetEnumProperty("EmbedMethod", EmbedMethod.TwiceCooked); }
			set { SetEnumProperty("EmbedMethod", EmbedMethod.TwiceCooked, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesFlashAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(FlashAlign.NotSet)]
		public FlashAlign FlashAlign {
			get { return (FlashAlign)GetEnumProperty("FlashAlign", FlashAlign.NotSet); }
			set { SetEnumProperty("FlashAlign", FlashAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesFlashVars"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string FlashVars {
			get { return GetStringProperty("FlashVars", ""); }
			set { SetStringProperty("FlashVars", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesHtmlAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(HtmlAlign.NotSet)]
		public HtmlAlign HtmlAlign {
			get { return (HtmlAlign)GetEnumProperty("HtmlAlign", HtmlAlign.NotSet); }
			set { SetEnumProperty("HtmlAlign", HtmlAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesLoop"),
#endif
		Browsable(true), NotifyParentProperty(true), DefaultValue(true)]
		public bool Loop {
			get { return GetBoolProperty("Loop", true); }
			set { SetBoolProperty("Loop", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesEnableContextMenu"),
#endif
		Browsable(true), NotifyParentProperty(true), DefaultValue(true)]
		public bool EnableContextMenu {
			get { return GetBoolProperty("EnableContextMenu", true); }
			set { SetBoolProperty("EnableContextMenu", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesPlay"),
#endif
		Browsable(true), NotifyParentProperty(true), DefaultValue(true)]
		public bool Play {
			get { return GetBoolProperty("Play", true); }
			set { SetBoolProperty("Play", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesScale"),
#endif
		NotifyParentProperty(true), DefaultValue(Scale.ShowAll)]
		public Scale Scale {
			get { return (Scale)GetEnumProperty("Scale", Scale.ShowAll); }
			set { SetEnumProperty("Scale", Scale.ShowAll, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesSWLiveConnect"),
#endif
		Browsable(true), NotifyParentProperty(true), DefaultValue(false)]
		public bool SWLiveConnect {
			get { return GetBoolProperty("SWLiveConnect", false); }
			set { SetBoolProperty("SWLiveConnect", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesQuality"),
#endif
		NotifyParentProperty(true), DefaultValue(Quality.Best)]
		public Quality Quality {
			get { return (Quality)GetEnumProperty("Quality", Quality.Best); }
			set { SetEnumProperty("Quality", Quality.Best, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FlashObjectPropertiesWindowMode"),
#endif
		NotifyParentProperty(true), DefaultValue(WindowMode.None)]
		public WindowMode WindowMode {
			get { return (WindowMode)GetEnumProperty("WindowMode", WindowMode.None); }
			set { SetEnumProperty("WindowMode", WindowMode.None, value); }
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.Flash; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Object; } 
		}
		protected override string ClassId {
			get { return "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"; }
		}
		protected override string CodeBase {
			get { return string.Format("http{0}://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab", RenderUtils.IsSecureConnection ? "s" : ""); }
		}
		protected override string PluginsPage {
			get { return string.Format("http{0}://www.macromedia.com/go/getflashplayer", RenderUtils.IsSecureConnection ? "s" : ""); }
		}
		static FlashObjectProperties() {
			fFlashAlignValues.Add(FlashAlign.Bottom, "b");
			fFlashAlignValues.Add(FlashAlign.BottomLeft, "bl");
			fFlashAlignValues.Add(FlashAlign.BottomRight, "br");
			fFlashAlignValues.Add(FlashAlign.Left, "l");
			fFlashAlignValues.Add(FlashAlign.Right, "r");
			fFlashAlignValues.Add(FlashAlign.Top, "t");
			fFlashAlignValues.Add(FlashAlign.TopLeft, "tl");
			fFlashAlignValues.Add(FlashAlign.TopRight, "tr");
		}
		public FlashObjectProperties()
			: base() {
		}
		public FlashObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		protected internal override bool HasFunctionalityScripts() {
			return ObjectContainer.IsClientSideEventsAssigned();
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			FlashObjectProperties flashProperties = objectProperties as FlashObjectProperties;
			if(flashProperties != null) {
				AllowScriptAccess = flashProperties.AllowScriptAccess;
				Base = flashProperties.Base;
				DeviceFont = flashProperties.DeviceFont;
				EmbedMethodInternal = flashProperties.EmbedMethodInternal;
				FlashAlign = flashProperties.FlashAlign;
				FlashVars = flashProperties.FlashVars;
				HtmlAlign = flashProperties.HtmlAlign;
				Loop = flashProperties.Loop;
				EnableContextMenu = flashProperties.EnableContextMenu;
				Play = flashProperties.Play;
				Scale = flashProperties.Scale;
				SWLiveConnect = flashProperties.SWLiveConnect;
				Quality = flashProperties.Quality;
				WindowMode = flashProperties.WindowMode;
			}
		}
		protected override void AddAttributes(WebControl containerControl) {
			base.AddAttributes(containerControl);
			if(HtmlAlign != HtmlAlign.NotSet)
				containerControl.Attributes.Add("align", HtmlAlign.ToString());
		}
		protected override void AddObjectAttributes(WebControl containerControl) {
			base.AddObjectAttributes(containerControl);
			if(ObjectUrl != "")
				containerControl.Attributes.Add("data", ObjectUrl);
			if(!RenderUtils.Browser.IsIE) {
				containerControl.Attributes.Remove("classid");
				containerControl.Attributes.Remove("codebase");
			}
			containerControl.Attributes.Add("name", ObjectContainer.ClientID);
		}
		protected override void AddEmbedAttributes(WebControl containerControl) {
			base.AddEmbedAttributes(containerControl);
			if(!ObjectContainer.BackColor.IsEmpty)
				containerControl.Attributes.Add("bgcolor", ColorToHexString(ObjectContainer.BackColor));
			if(AllowScriptAccess != AllowScriptAccess.SameDomain)
				containerControl.Attributes.Add("allowscriptaccess", AllowScriptAccess.ToString().ToLower());
			if(Base != "")
				containerControl.Attributes.Add("base", Base);
			if(DeviceFont)
				containerControl.Attributes.Add("devicefont", "true");
			if(FlashAlign != FlashAlign.NotSet)
				containerControl.Attributes.Add("salign", fFlashAlignValues[FlashAlign].ToString());
			if(FlashVars != "")
				containerControl.Attributes.Add("flashvars", FlashVars);
			if(HtmlAlign != HtmlAlign.NotSet)
				containerControl.Attributes.Add("align", HtmlAlign.ToString());
			if(!Loop)
				containerControl.Attributes.Add("loop", "false");
			if(!EnableContextMenu)
				containerControl.Attributes.Add("menu", "false");
			if(!Play)
				containerControl.Attributes.Add("play", "false");
			if(Scale != Scale.ShowAll)
				containerControl.Attributes.Add("scale", Scale.ToString().ToLower());
			if(SWLiveConnect)
				containerControl.Attributes.Add("swliveconnect", "true");
			if(Quality != Quality.Best)
				containerControl.Attributes.Add("quality", Quality.ToString().ToLower());
			if(WindowMode != WindowMode.None)
				containerControl.Attributes.Add("wmode", WindowMode.ToString().ToLower());
		}
		protected override void GetParamsControlText(StringBuilder stb) {
			base.GetParamsControlText(stb);
			if(!ObjectContainer.BackColor.IsEmpty)
				stb.Append("<param name=\"bgcolor\" value=\"" + ColorToHexString(ObjectContainer.BackColor) + "\" />");
			if(ObjectUrl != "")
				stb.Append("<param name=\"movie\" value=\"" + ObjectUrl + "\" />");
			stb.Append(AllowScriptAccess != AllowScriptAccess.SameDomain ? "<param name=\"allowscriptaccess\" value=\"" + AllowScriptAccess.ToString().ToLower() + "\" />" : "");
			stb.Append(DeviceFont ? "<param name=\"devicefont\" value=\"true\" />" : "");
			stb.Append(Base != "" ? "<param name=\"base\" value=\"" + Base + "\" />" : "");
			stb.Append(FlashAlign != FlashAlign.NotSet ? "<param name=\"salign\" value=\"" + fFlashAlignValues[FlashAlign] + "\" />" : "");
			stb.Append(FlashVars != "" ? "<param name=\"flashvars\" value=\"" + FlashVars + "\" />" : "");
			stb.Append(!Loop ? "<param name=\"loop\" value=\"false\" />" : "");
			stb.Append(!EnableContextMenu ? "<param name=\"menu\" value=\"false\" />" : "");
			stb.Append(!Play ? "<param name=\"play\" value=\"false\" />" : "");
			stb.Append(Scale != Scale.ShowAll ? "<param name=\"scale\" value=\"" + Scale.ToString().ToLower() + "\" />" : "");
			stb.Append(SWLiveConnect ? "<param name=\"swliveconnect\" value=\"true\" />" : "");
			stb.Append(Quality != Quality.Best ? "<param name=\"quality\" value=\"" + Quality.ToString().ToLower() + "\" />" : "");
			stb.Append(WindowMode != WindowMode.None ? "<param name=\"wmode\" value=\"" + WindowMode.ToString().ToLower() + "\" />" : "");
		}
	}
	public class VideoObjectProperties : MediaObjectProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesAutoStart"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool AutoStart {
			get { return GetBoolProperty("AutoStart", true); }
			set { SetBoolProperty("AutoStart", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesBalance"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public virtual int Balance {
			get { return GetIntProperty("Balance", 0); }
			set {
				CommonUtils.CheckValueRange(value, -100, 100, "Balance");
				SetIntProperty("Balance", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesBaseURL"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public virtual string BaseURL {
			get { return GetStringProperty("BaseURL", ""); }
			set { SetStringProperty("BaseURL", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesCaptioningID"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public virtual string CaptioningID {
			get { return GetStringProperty("CaptioningID", ""); }
			set { SetStringProperty("CaptioningID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesCurrentMarker"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int CurrentMarker {
			get { return GetIntProperty("CurrentMarker", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "CurrentMarker");
				SetIntProperty("CurrentMarker", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesCurrentPosition"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(decimal), "0.0")]
		public virtual decimal CurrentPosition {
			get { return GetDecimalProperty("CurrentPosition", (decimal)0); }
			set {
				CommonUtils.CheckNegativeValue((double)value, "CurrentPosition");
				SetDecimalProperty("CurrentPosition", (decimal)0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesDefaultFrame"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public virtual string DefaultFrame {
			get { return GetStringProperty("DefaultFrame", ""); }
			set { SetStringProperty("DefaultFrame", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesEnableContextMenu"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool EnableContextMenu {
			get { return GetBoolProperty("EnableContextMenu", true); }
			set { SetBoolProperty("EnableContextMenu", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesEnabled"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set { SetBoolProperty("Enabled", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesFullScreen"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool FullScreen {
			get { return GetBoolProperty("FullScreen", false); }
			set { SetBoolProperty("FullScreen", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesInvokeURLs"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool InvokeURLs {
			get { return GetBoolProperty("InvokeURLs", true); }
			set { SetBoolProperty("InvokeURLs", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesMute"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool Mute {
			get { return GetBoolProperty("Mute", false); }
			set { SetBoolProperty("Mute", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesPlayCount"),
#endif
		NotifyParentProperty(true), DefaultValue(1)]
		public virtual int PlayCount {
			get { return GetIntProperty("PlayCount", 1); }
			set {
				CommonUtils.CheckNegativeValue(value, "PlayCount");
				SetIntProperty("PlayCount", 1, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesRate"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(decimal), "1.0")]
		public virtual decimal Rate {
			get { return GetDecimalProperty("Rate", (decimal)1.0); }
			set { SetDecimalProperty("Rate", (decimal)1.0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesStandByMessage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public virtual string StandByMessage {
			get { return GetStringProperty("StandByMessage", ""); }
			set { SetStringProperty("StandByMessage", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesStretchToFit"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool StretchToFit {
			get { return GetBoolProperty("StretchToFit", false); }
			set { SetBoolProperty("StretchToFit", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesUIMode"),
#endif
		NotifyParentProperty(true), DefaultValue(UIMode.Full)]
		public virtual UIMode UIMode {
			get { return (UIMode)GetEnumProperty("UIMode", UIMode.Full); }
			set { SetEnumProperty("UIMode", UIMode.Full, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesVolume"),
#endif
		NotifyParentProperty(true), DefaultValue(-1)]
		public virtual int Volume {
			get { return GetIntProperty("Volume", -1); }
			set {
				CommonUtils.CheckValueRange(value, -1, 100, "Volume");
				SetIntProperty("Volume", -1, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("VideoObjectPropertiesWindowlessVideo"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool WindowlessVideo {
			get { return GetBoolProperty("WindowlessVideo", false); }
			set { SetBoolProperty("WindowlessVideo", false, value); }
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.Video; }
		}
		public VideoObjectProperties()
			: base() {
		}
		public VideoObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			VideoObjectProperties videoProperties = objectProperties as VideoObjectProperties;
			if(videoProperties != null) {
				AutoStart = videoProperties.AutoStart;
				Balance = videoProperties.Balance;
				BaseURL = videoProperties.BaseURL;
				CaptioningID = videoProperties.CaptioningID;
				CurrentMarker = videoProperties.CurrentMarker;
				CurrentPosition = videoProperties.CurrentPosition;
				DefaultFrame = videoProperties.DefaultFrame;
				EnableContextMenu = videoProperties.EnableContextMenu;
				Enabled = videoProperties.Enabled;
				FullScreen = videoProperties.FullScreen;
				InvokeURLs = videoProperties.InvokeURLs;
				Mute = videoProperties.Mute;
				PlayCount = videoProperties.PlayCount;
				Rate = videoProperties.Rate;
				StandByMessage = videoProperties.StandByMessage;
				StretchToFit = videoProperties.StretchToFit;
				UIMode = videoProperties.UIMode;
				Volume = videoProperties.Volume;
				WindowlessVideo = videoProperties.WindowlessVideo;
			}
		}
		protected override void AddAttributes(WebControl containerControl) {
			base.AddAttributes(containerControl);
			if(StandByMessage != "")
				containerControl.Attributes.Add("standby", StandByMessage);
		}
		protected override void AddEmbedAttributes(WebControl containerControl) {
			base.AddEmbedAttributes(containerControl);
			if(!AutoStart)
				containerControl.Attributes.Add("autoStart", "false");
			if(Balance != 0)
				containerControl.Attributes.Add("balance", Balance.ToString());
			if(BaseURL != "")
				containerControl.Attributes.Add("baseURL", BaseURL);
			if(CaptioningID != "")
				containerControl.Attributes.Add("captioningID", CaptioningID);
			if(CurrentPosition != 0)
				containerControl.Attributes.Add("currentPosition", CurrentPosition.ToString());
			if(CurrentMarker != 0)
				containerControl.Attributes.Add("currentMarker", CurrentMarker.ToString());
			if(DefaultFrame != "")
				containerControl.Attributes.Add("defaultFrame", DefaultFrame);
			if(!EnableContextMenu)
				containerControl.Attributes.Add("enableContextMenu", "false");
			if(!Enabled)
				containerControl.Attributes.Add("enabled", "false");
			if(FullScreen)
				containerControl.Attributes.Add("fullScreen", "true");
			if(!InvokeURLs)
				containerControl.Attributes.Add("invokeURLs", "false");
			if(Mute)
				containerControl.Attributes.Add("mute", "true");
			if(PlayCount != 1)
				containerControl.Attributes.Add("playCount", PlayCount.ToString());
			if(Rate != (decimal)1.0)
				containerControl.Attributes.Add("rate", Rate.ToString());
			if(StretchToFit)
				containerControl.Attributes.Add("stretchToFit", "true");
			if(UIMode != UIMode.Full)
				containerControl.Attributes.Add("uiMode", UIMode.ToString());
			if(WindowlessVideo)
				containerControl.Attributes.Add("windowlessVideo", "true");
			AddCommonEmbedAttributes(containerControl);
		}
		protected virtual void AddCommonEmbedAttributes(WebControl containerControl) {
			if(Volume != -1)
				containerControl.Attributes.Add("volume", Volume.ToString());
		}
		protected override void GetParamsControlText(StringBuilder stb) {
			base.GetParamsControlText(stb);
			stb.Append(!AutoStart ? "<param name=\"autoStart\" value=\"false\" />" : "");
			stb.Append(Balance != 0 ? "<param name=\"balance\" value=\"" + Balance.ToString() + "\" />" : "");
			stb.Append(BaseURL != "" ? "<param name=\"baseURL\" value=\"" + BaseURL + "\" />" : "");
			stb.Append(CaptioningID != "" ? "<param name=\"captioningID\" value=\"" + CaptioningID + "\" />" : "");
			stb.Append(CurrentPosition != 0 ? "<param name=\"currentPosition\" value=\"" + CurrentPosition.ToString() + "\" />" : "");
			stb.Append(CurrentMarker != 0 ? "<param name=\"currentMarker\" value=\"" + CurrentMarker.ToString() + "\" />" : "");
			stb.Append(DefaultFrame != "" ? "<param name=\"defaultFrame\" value=\"" + DefaultFrame + "\" />" : "");
			stb.Append(!EnableContextMenu ? "<param name=\"enableContextMenu\" value=\"false\" />" : "");
			stb.Append(!Enabled ? "<param name=\"enabled\" value=\"false\" />" : "");
			stb.Append(FullScreen ? "<param name=\"fullScreen\" value=\"true\" />" : "");
			stb.Append(!InvokeURLs ? "<param name=\"invokeURLs\" value=\"false\" />" : "");
			stb.Append(Mute ? "<param name=\"mute\" value=\"true\" />" : "");
			stb.Append(PlayCount != 1 ? "<param name=\"playCount\" value=\"" + PlayCount.ToString() + "\" />" : "");
			stb.Append(Rate != (decimal)1.0 ? "<param name=\"rate\" value=\"" + Rate.ToString() + "\" />" : "");
			stb.Append(StretchToFit ? "<param name=\"stretchToFit\" value=\"true\" />" : "");
			stb.Append(UIMode != UIMode.Full ? "<param name=\"uiMode\" value=\"" + UIMode.ToString() + "\" />" : "");
			stb.Append(WindowlessVideo ? "<param name=\"windowlessVideo\" value=\"true\" />" : "");
			GetCommonParamsControlText(stb);
		}
		protected virtual void GetCommonParamsControlText(StringBuilder stb) {
			stb.Append(ObjectUrl != "" ? "<param name=\"URL\" value=\"" + ObjectUrl + "\" />" : "");
			stb.Append(Volume != -1 ? "<param name=\"volume\" value=\"" + Volume.ToString() + "\" />" : "");
		}
	}
	public abstract class Html5ObjectPropertiesBase : ObjectProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("Html5ObjectPropertiesBaseAutoPlay"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AutoPlay {
			get { return GetBoolProperty("AutoPlay", false); }
			set { SetBoolProperty("AutoPlay", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("Html5ObjectPropertiesBaseLoop"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool Loop {
			get { return GetBoolProperty("Loop", false); }
			set { SetBoolProperty("Loop", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("Html5ObjectPropertiesBaseMuted"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool Muted {
			get { return GetBoolProperty("Muted", false); }
			set { SetBoolProperty("Muted", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("Html5ObjectPropertiesBasePreloadMode"),
#endif
		NotifyParentProperty(true), DefaultValue(Html5PreloadMode.Metadata)]
		public virtual Html5PreloadMode PreloadMode {
			get { return (Html5PreloadMode)GetEnumProperty("PreloadMode", Html5PreloadMode.Metadata); }
			set { SetEnumProperty("PreloadMode", Html5PreloadMode.Metadata, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("Html5ObjectPropertiesBaseShowControls"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowControls {
			get { return GetBoolProperty("ShowControls", true); }
			set { SetBoolProperty("ShowControls", true, value); }
		}
		public Html5ObjectPropertiesBase()
			: base() {
		}
		public Html5ObjectPropertiesBase(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		protected internal override bool RenderAsObject() {
			return false;
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.Html5Audio; }
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			Html5ObjectPropertiesBase html5Properties = objectProperties as Html5ObjectPropertiesBase;
			if(html5Properties != null) {
				AutoPlay = html5Properties.AutoPlay;
				ShowControls = html5Properties.ShowControls;
				Loop = html5Properties.Loop;
				Muted = html5Properties.Muted;
				PreloadMode = html5Properties.PreloadMode;
			}
		}
		protected override void AddAttributes(WebControl containerControl) {
			if(ObjectUrl != "")
				containerControl.Attributes.Add("src", ObjectUrl);
			if(AutoPlay)
				containerControl.Attributes.Add("autoplay", "autoplay");
			if(ShowControls)
				containerControl.Attributes.Add("controls", "controls");
			if(Loop)
				containerControl.Attributes.Add("loop", "loop");
			if(Muted)
				containerControl.Attributes.Add("muted", "muted");
			containerControl.Attributes.Add("preload", PreloadMode.ToString().ToLower());
		}
	}
	public class Html5AudioObjectProperties : Html5ObjectPropertiesBase {
		public Html5AudioObjectProperties()
			: base() {
		}
		public Html5AudioObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
	}
	public class Html5VideoObjectProperties : Html5ObjectPropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("Html5VideoObjectPropertiesPosterUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public virtual string PosterUrl {
			get { return GetStringProperty("PosterUrl", String.Empty); }
			set { SetStringProperty("PosterUrl", String.Empty, value); }
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.Html5Video; }
		}
		public Html5VideoObjectProperties()
			: base() {
		}
		public Html5VideoObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			Html5VideoObjectProperties html5VideoProperties = objectProperties as Html5VideoObjectProperties;
			if(html5VideoProperties != null)
				PosterUrl = html5VideoProperties.PosterUrl;
		}
		protected override void AddAttributes(WebControl containerControl) {
			base.AddAttributes(containerControl);
			if(PosterUrl != "")
				containerControl.Attributes.Add("poster", ObjectContainer.ResolveClientUrl(PosterUrl));
		}
	}
	public class AudioObjectProperties : VideoObjectProperties {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CaptioningID {
			get { return base.CaptioningID; }
			set { base.CaptioningID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AudioObjectPropertiesBalance"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public override int Balance {
			get { return GetIntProperty("Balance", 0); }
			set {
				CommonUtils.CheckValueRange(value, -10000, 10000, "Balance");
				SetIntProperty("Balance", 0, value);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool FullScreen {
			get { return base.FullScreen; }
			set { base.FullScreen = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AudioObjectPropertiesPlayCount"),
#endif
		NotifyParentProperty(true), DefaultValue(1)]
		public override int PlayCount {
			get { return GetIntProperty("PlayCount", 1); }
			set {
				CommonUtils.CheckNegativeValue(value, "PlayCount");
				SetIntProperty("PlayCount", 1, value);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool StretchToFit {
			get { return base.StretchToFit; }
			set { base.StretchToFit = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override UIMode UIMode {
			get { return base.UIMode; }
			set { base.UIMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AudioObjectPropertiesVolume"),
#endif
		NotifyParentProperty(true), DefaultValue(-600)]
		public override int Volume {
			get { return GetIntProperty("Volume", -600); }
			set {
				CommonUtils.CheckValueRange(value, -10000, 0, "Volume");
				SetIntProperty("Volume", -600, value);
			}
		}
		protected override string ClassId {
			get { return "clsid:22D6F312-B0F6-11D0-94AB-0080C74C7E95"; }
		}
		public AudioObjectProperties()
			: base() {
		}
		public AudioObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.Audio; }
		}
		protected override void AddCommonEmbedAttributes(WebControl containerControl) {
			if(ObjectUrl != "")
				containerControl.Attributes.Add("FileName", ObjectUrl);
			if(Volume != -600)
				containerControl.Attributes.Add("volume", Volume.ToString());
		}
		protected override void GetCommonParamsControlText(StringBuilder stb) {
			stb.Append(ObjectUrl != "" ? "<param name=\"FileName\" value=\"" + ObjectUrl + "\" />" : "");
			stb.Append(Volume != -600 ? "<param name=\"volume\" value=\"" + Volume.ToString() + "\" />" : "");
		}
	}
	public class QuickTimeObjectProperties : MediaObjectProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesPluginVersion"),
#endif
		NotifyParentProperty(true), DefaultValue("3,0,0,0"), Localizable(false)]
		public override string PluginVersion {
			get { return GetStringProperty("PluginVersion", "3,0,0,0"); }
			set { SetStringProperty("PluginVersion", "3,0,0,0", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesAutoPlay"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool AutoPlay {
			get { return GetBoolProperty("AutoPlay", true); }
			set { SetBoolProperty("AutoPlay", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesController"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool Controller {
			get { return GetBoolProperty("Controller", true); }
			set { SetBoolProperty("Controller", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesLoop"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Loop {
			get { return GetBoolProperty("Loop", false); }
			set { SetBoolProperty("Loop", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesFieldOfView"),
#endif
		NotifyParentProperty(true), DefaultValue(50)]
		public int FieldOfView {
			get { return GetIntProperty("FieldOfView", 50); }
			set {
				CommonUtils.CheckValueRange(value, 8, 64, "FieldOfView");
				SetIntProperty("FieldOfView", 50, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesPanAngle"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int PanAngle {
			get { return GetIntProperty("PanAngle", 0); }
			set {
				CommonUtils.CheckValueRange(value, 0, 360, "Volume");
				SetIntProperty("PanAngle", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesTiltAngle"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int TiltAngle {
			get { return GetIntProperty("TiltAngle", 0); }
			set {
				CommonUtils.CheckValueRange(value, -42, 42, "Volume");
				SetIntProperty("TiltAngle", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("QuickTimeObjectPropertiesVolume"),
#endif
		NotifyParentProperty(true), DefaultValue(100)]
		public int Volume {
			get { return GetIntProperty("Volume", 100); }
			set {
				CommonUtils.CheckValueRange(value, 0, 100, "Volume");
				SetIntProperty("Volume", 100, value);
			}
		}
		protected override string Type {
			get { return MimeTypeManager.GetType(ObjectUrl, false); }
		}
		protected internal override ObjectType ObjectType {
			get { return ObjectType.QuickTime; }
		}
		protected override string ClassId {
			get { return "clsid:02BF25D5-8C17-4B23-BC80-D3488ABDDC6B"; }
		}
		protected override string CodeBase {
			get { return string.Format("http{0}://www.apple.com/qtactivex/qtplugin.cab", RenderUtils.IsSecureConnection ? "s" : ""); }
		}
		protected override string PluginsPage {
			get { return string.Format("http{0}://www.apple.com/quicktime/download/", RenderUtils.IsSecureConnection ? "s" : ""); }
		}
		public QuickTimeObjectProperties()
			: base() {
		}
		public QuickTimeObjectProperties(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		public override void Assign(ObjectProperties objectProperties) {
			base.Assign(objectProperties);
			QuickTimeObjectProperties quickTimeProperties = objectProperties as QuickTimeObjectProperties;
			if(quickTimeProperties != null) {
				AutoPlay = quickTimeProperties.AutoPlay;
				Controller = quickTimeProperties.Controller;
				Loop = quickTimeProperties.Loop;
				FieldOfView = quickTimeProperties.FieldOfView;
				PanAngle = quickTimeProperties.PanAngle;
				TiltAngle = quickTimeProperties.TiltAngle;
				Volume = quickTimeProperties.Volume;
			}
		}
		protected override void AddEmbedAttributes(WebControl containerControl) {
			base.AddEmbedAttributes(containerControl);
			if(!AutoPlay)
				containerControl.Attributes.Add("autoplay", "false");
			containerControl.Attributes.Add("controller", Controller.ToString());
			if(Loop)
				containerControl.Attributes.Add("loop", Loop ? "true" : "false");
			if(FieldOfView != 50)
				containerControl.Attributes.Add("fov", FieldOfView.ToString());
			if(PanAngle != 0)
				containerControl.Attributes.Add("pan", PanAngle.ToString());
			if(TiltAngle != 0)
				containerControl.Attributes.Add("tilt", TiltAngle.ToString());
			containerControl.Attributes.Add("volume", Volume.ToString());
		}
		protected override void GetParamsControlText(StringBuilder stb) {
			base.GetParamsControlText(stb);
			if(!ObjectContainer.BackColor.IsEmpty)
				stb.Append("<param name=\"bgcolor\" value=\"" + ColorToHexString(ObjectContainer.BackColor) + "\" />");
			if(ObjectUrl != "")
				stb.Append("<param name=\"src\" value=\"" + ObjectUrl + "\" />");
			stb.Append(!AutoPlay ? "<param name=\"autoplay\" value=\"false\" />" : "");
			stb.Append("<param name=\"controller\" value=\"" + Controller.ToString() + "\" />");
			stb.Append(Loop ? "<param name=\"loop\" value=\"true\" />" : "");
			stb.Append(FieldOfView != 50 ? "<param name=\"fov\" value=\"" + FieldOfView.ToString() + "\" />" : "");
			stb.Append(PanAngle != 0 ? "<param name=\"pan\" value=\"" + PanAngle.ToString() + "\" />" : "");
			stb.Append(TiltAngle != 0 ? "<param name=\"tilt\" value=\"" + TiltAngle.ToString() + "\" />" : "");
			stb.Append("<param name=\"volume\" value=\"" + Volume.ToString() + "\" />");
		}
	}
}
