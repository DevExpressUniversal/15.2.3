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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class BackToTopControl : ASPxInternalWebControl {
		protected const string UrlTemplate = "javascript:{0}";
		protected const double OffsetToTop = 8;
		private string fAnchorName = "";
		private string fText = "";
		private ImageProperties fImageProperties = new ImageProperties();
		private AppearanceStyle backToTopStyle = null;
		private Paddings backToTopPaddings = null;
		private Unit fBackToTopImageSpacing = Unit.Empty;
		private Unit fBackToTopSpacing = Unit.Empty;
		private HyperLink fTextControl = null;
		private WebControl fDiv = null;
		private Image fImage = null;
		private HyperLink fImageHyperLihk = null;
		bool isRtl;
		public ImageProperties BackToTopImage {
			get { return fImageProperties; }
			set { fImageProperties = value; }
		}
		public Paddings BackToTopPaddings {
			get { return backToTopPaddings; }
			set { backToTopPaddings = value; }
		}
		public AppearanceStyle BackToTopStyle {
			get { return backToTopStyle; }
			set { backToTopStyle = value; }		
		}
		public Unit BackToTopImageSpacing {
			get { return fBackToTopImageSpacing; }
			set { fBackToTopImageSpacing = value; }
		}
		public Unit BackToTopSpacing {
			get { return fBackToTopSpacing; }
			set { fBackToTopSpacing = value; }
		}
		public string Text {
			get { return fText; }
			set { fText = value; }
		}
		public BackToTopControl(string anchorName)
			: this(anchorName, false) {
		}
		public BackToTopControl(string anchorName, bool isRtl) {
			fAnchorName = anchorName;
			this.isRtl = isRtl;
		}
		protected override void ClearControlFields() {
			fTextControl = null;
			fDiv = null;
			fImage = null;
			fImageHyperLihk = null;
		}
		protected override void CreateControlHierarchy() {
			fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(fDiv);
			if (!BackToTopImage.IsEmpty) {
				fImageHyperLihk = RenderUtils.CreateHyperLink();
				fDiv.Controls.Add(fImageHyperLihk);
				fImage = RenderUtils.CreateImage();
				fImageHyperLihk.Controls.Add(fImage);
			}
			fTextControl = RenderUtils.CreateHyperLink();
			fDiv.Controls.Add(fTextControl);
		}
		protected override void PrepareControlHierarchy() {
			if (fDiv != null) {
				if(BackToTopStyle != null)
					BackToTopStyle.AssignToControl(fDiv);
				if(BackToTopPaddings != null)
					RenderUtils.SetPaddings(fDiv, BackToTopPaddings);
				string url = string.Format(UrlTemplate,
					RenderUtils.GetBackToTopFunctionReference(fAnchorName, Browser.Family.IsWebKit ? 0 : OffsetToTop));
				RenderUtils.PrepareHyperLink(fTextControl, HttpUtility.HtmlEncode(fText), url, "", "", Enabled);
				if(BackToTopStyle != null)
					BackToTopStyle.AssignToHyperLink(fTextControl);
				if (fImage != null) {
					RenderUtils.PrepareHyperLink(fImageHyperLihk, "", url, "", "", true);
					Paddings padding = new Paddings(this.isRtl ? fBackToTopImageSpacing : Unit.Empty, Unit.Empty, this.isRtl ? Unit.Empty : fBackToTopImageSpacing, Unit.Empty);
					padding.AssignToControl(fImageHyperLihk);
					RenderUtils.SetVerticalAlign(fImage, VerticalAlign.Middle);
					BackToTopImage.AssignToControl(fImage, false);
				}
				if (!BackToTopSpacing.IsEmpty && BackToTopSpacing.Value != 0)
					RenderUtils.SetMargins(fDiv, Unit.Empty, BackToTopSpacing, Unit.Empty, Unit.Empty);
			}
		}
	}
	public abstract class InternalTableColGroupBase {
		int span = 1;
		Unit width = Unit.Empty;
		public int Span { get { return span; } set { span = value; } }
		public Unit Width { get { return width; } set { width = value; } }
		protected internal virtual void Render(HtmlTextWriter writer) {
			if(Span > 1)
				writer.AddAttribute("span", Span.ToString());
			if(!Width.IsEmpty)
				writer.AddAttribute("style", string.Format("width:{0}", Width));
			writer.RenderBeginTag(WriterTag);
			RenderChildren(writer);
			writer.RenderEndTag();
		}
		protected abstract HtmlTextWriterTag WriterTag { get; }
		protected virtual void RenderChildren(HtmlTextWriter writer) { }
	}
	public class InternalTableColGroupColumn : InternalTableColGroupBase {
		protected override HtmlTextWriterTag WriterTag { get { return HtmlTextWriterTag.Col; } }
	}
	public class InternalTableColGroup : InternalTableColGroupBase {
		List<InternalTableColGroupColumn> cols;
		public InternalTableColGroup() {
			this.cols = new List<InternalTableColGroupColumn>();
		}
		public List<InternalTableColGroupColumn> Cols { get { return cols; } }
		public InternalTableColGroupColumn AddCol() {
			return AddCol(Unit.Empty);
		}
		public InternalTableColGroupColumn AddCol(Unit width) {
			InternalTableColGroupColumn col = new InternalTableColGroupColumn();
			Cols.Add(col);
			col.Width = width;
			return col;
		}
		protected override HtmlTextWriterTag WriterTag { get { return HtmlTextWriterTag.Colgroup; } }
		protected override void RenderChildren(HtmlTextWriter writer) {
			foreach(InternalTableColGroupColumn col in Cols) {
				col.Render(writer);
			}
		}
	}
	public enum BorderCollapse { NotSet, Separate, Collapse };
	[ToolboxItem(false)]
	public class InternalTable : Table, IASPxWebControl {
		private List<InternalTableColGroup> colGroups;
		private BorderCollapse borderCollapse;
		private readonly static Version RenderingHtml5CompatibilityVersion = new Version(4, 0);
		public InternalTable() {
			this.colGroups = new List<InternalTableColGroup>();
		}
		public List<InternalTableColGroup> ColGroups {
			get { return colGroups; }
		}
		public BorderCollapse BorderCollapse {
			get { return borderCollapse; }
			set { borderCollapse = value; }
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		public override void RenderBeginTag(HtmlTextWriter writer) {
			string caption = Caption;
			Caption = "";
			try {
				base.RenderBeginTag(writer);
			} finally {
				Caption = caption;
			}
		}
		public override Version RenderingCompatibility {
			get {
				if(RenderUtils.IsHtml5Mode(this))
					return RenderingHtml5CompatibilityVersion; 
				return base.RenderingCompatibility;
			}
			set {
				base.RenderingCompatibility = value;
			}
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(RenderUtils.IsHtml5Mode(this)) {
				if(!BorderWidth.IsEmpty && BorderWidth != 0)
					Style.Add("border-width", BorderWidth.ToString());
				if(CellPadding > -1)
					CellPadding = -1;
				if(CellSpacing > -1) {
					if(CellSpacing > 0) {
						Style.Add("border-spacing", CellSpacing.ToString() + "px");
						Style.Add("border-collapse", "separate");
					}
					CellSpacing = -1;
				}
				if(HorizontalAlign != HorizontalAlign.NotSet) {
					RenderUtils.SetHorizontalAlignCssAttributes(this, HorizontalAlign.ToString());
					HorizontalAlign = HorizontalAlign.NotSet;
				}
				RenderUtils.ReplaceAlignAttributes(this);
			}
			base.AddAttributesToRender(writer);
		}
		protected override void RenderContents(HtmlTextWriter writer) {
			if(ColGroups.Count > 0)
				RenderColGroups(writer);
			if(!String.IsNullOrEmpty(Caption))
				RenderCaption(writer);
			base.RenderContents(writer);
		}
		protected virtual void RenderColGroups(HtmlTextWriter writer) {
			foreach(InternalTableColGroup colGroup in ColGroups) {
				colGroup.Render(writer);
			}
		}
		protected void RenderCaption(HtmlTextWriter writer) {
			writer.RenderBeginTag(HtmlTextWriterTag.Caption);
			writer.Write(Caption);
			writer.RenderEndTag();
		}
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState(){
			return null;
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			EnsureChildControls();
		}
		protected override void Render(HtmlTextWriter writer) {
			EnsureChildControls();
			PrepareControlHierarchy();
			if(BorderCollapse != BorderCollapse.NotSet)
				RenderUtils.SetStyleAttribute(this, "border-collapse", BorderCollapse == BorderCollapse.Separate ? "separate" : "collapse", "");
			base.Render(writer);
		}
		protected override void CreateChildControls() {
			ClearControlFields();
			CreateControlHierarchy();
			base.CreateChildControls();
		}
		protected internal void ResetControlHierarchy() {
			ChildControlsCreated = false;
		}
		protected virtual void ClearControlFields() {
		}
		protected virtual void PrepareControlHierarchy() {
		}
		protected virtual void CreateControlHierarchy() {
		}
		void IASPxWebControl.EnsureChildControls() {
			EnsureChildControls();
		}
		void IASPxWebControl.PrepareControlHierarchy() {
			PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public class InternalTableRow : TableRow, IASPxWebControl {
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState() {
			return null;
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			EnsureChildControls();
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(RenderUtils.IsHtml5Mode(this)) {
				if(HorizontalAlign != HorizontalAlign.NotSet) {
					RenderUtils.SetHorizontalAlignCssAttributes(this, HorizontalAlign.ToString());
					HorizontalAlign = HorizontalAlign.NotSet;
				}
				if(VerticalAlign != VerticalAlign.NotSet) {
					Style.Add("vertical-align", VerticalAlign.ToString());
					VerticalAlign = VerticalAlign.NotSet;
				}
				RenderUtils.ReplaceAlignAttributes(this);
			}
			base.AddAttributesToRender(writer);
		}
		protected override void Render(HtmlTextWriter writer) {
			EnsureChildControls();
			PrepareControlHierarchy();
			base.Render(writer);
		}
		protected override void CreateChildControls() {
			CreateControlHierarchy();
			base.CreateChildControls();
		}
		protected internal void ResetControlHierarchy() {
			ChildControlsCreated = false;
		}
		protected virtual void PrepareControlHierarchy() {
		}
		protected virtual void CreateControlHierarchy() {
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		void IASPxWebControl.EnsureChildControls() {
			EnsureChildControls();
		}
		void IASPxWebControl.PrepareControlHierarchy() {
			PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public class InternalTextBox : TextBox {
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
	}
	[ToolboxItem(false)]
	public class InternalTableCell : TableCell, IASPxWebControl {
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState() {
			return null;
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			EnsureChildControls();
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(RenderUtils.IsHtml5Mode(this)) {
				if(HorizontalAlign != HorizontalAlign.NotSet) {
					RenderUtils.SetHorizontalAlignCssAttributes(this, HorizontalAlign.ToString());
					HorizontalAlign = HorizontalAlign.NotSet;
				}
				if(VerticalAlign != VerticalAlign.NotSet) {
					Style.Add("vertical-align", VerticalAlign.ToString());
					VerticalAlign = VerticalAlign.NotSet;
				}
				RenderUtils.ReplaceAlignAttributes(this);
			}
			base.AddAttributesToRender(writer);
		}
		protected override void Render(HtmlTextWriter writer) {
			EnsureChildControls();
			PrepareControlHierarchy();
			base.Render(writer);
		}
		protected override void CreateChildControls() {
			ClearControlFields();
			CreateControlHierarchy();
			base.CreateChildControls();
		}
		protected internal void ResetControlHierarchy() {
			ChildControlsCreated = false;
		}
		protected virtual void ClearControlFields() {
		}
		protected virtual void CreateControlHierarchy() {
		}
		protected virtual void PrepareControlHierarchy() {
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		void IASPxWebControl.EnsureChildControls() {
			EnsureChildControls();
		}
		void IASPxWebControl.PrepareControlHierarchy() {
			PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public class InternalHiddenField : HiddenField {
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}		
		protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection) {
			string newValue = postCollection[postDataKey];
			if(!Value.Equals(newValue, StringComparison.Ordinal)) {
				Value = newValue;
				return true;
			}
			return false;
		}
		protected override void Render(HtmlTextWriter writer) {
			if(Page != null)
				Page.VerifyRenderingInServerForm(this);
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
			if(UniqueID != null)
				writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
			if(ID != null)
				writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
			if(!string.IsNullOrEmpty(Value))
				writer.AddAttribute(HtmlTextWriterAttribute.Value, Value);
			writer.RenderBeginTag(HtmlTextWriterTag.Input);
			writer.RenderEndTag();
		}
	}
	[ToolboxItem(false)]
	public class InternalHtmlControl : WebControl, IASPxWebControl {
		public InternalHtmlControl(HtmlTextWriterTag tag)
			: base(tag) {
		}
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState() {
			return null;
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			EnsureChildControls();
		}
		protected override void Render(HtmlTextWriter writer) {
			EnsureChildControls();
			PrepareControlHierarchy();
			base.Render(writer);
		}
		protected override void CreateChildControls() {
			ClearControlFields();
			CreateControlHierarchy();
			base.CreateChildControls();
		}
		protected internal void ResetControlHierarchy() {
			ChildControlsCreated = false;
		}
		protected virtual void ClearControlFields() {
		}
		protected virtual void CreateControlHierarchy() {
		}
		protected virtual void PrepareControlHierarchy() {
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		void IASPxWebControl.EnsureChildControls() {
			EnsureChildControls();
		}
		void IASPxWebControl.PrepareControlHierarchy() {
			PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public class InternalWebControl: WebControl {
		public InternalWebControl(HtmlTextWriterTag tag)
			: base(tag) {
		}
		public InternalWebControl(string tag)
			: base(tag) {
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(RenderUtils.IsHtml5Mode(this))
				RenderUtils.ReplaceAlignAttributes(this);
			base.AddAttributesToRender(writer);
		}
		protected override object SaveViewState() {
			return null; 
		}
		protected override object SaveControlState() {
			return null;
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
	}
	[ToolboxItem(false)]
	public class InternalLabel : Label {
		public InternalLabel()
			: base() {
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
	}
	[ToolboxItem(false)]
	public class InternalImage : Image {
		private bool preventFixIETitle;
		private readonly static Version RenderingCompatibilityVersion = new Version(4, 0);
		public bool PreventFixIETitle {
			get { return preventFixIETitle; }
			set { preventFixIETitle = value; }
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		public override Version RenderingCompatibility {
			get {
				return RenderingCompatibilityVersion; 
			}
			set {
			}
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(RenderUtils.IsHtml5Mode(this)) {
				if(ImageAlign != ImageAlign.NotSet) {
					switch(ImageAlign) {
						case ImageAlign.AbsBottom:
						case ImageAlign.Bottom:
							Style["vertical-align"] = "bottom";
							break;
						case ImageAlign.AbsMiddle:
						case ImageAlign.Middle:
							Style["vertical-align"] = "middle";
							break;
						case ImageAlign.Baseline:
							Style["vertical-align"] = "baseline";
							break;
						case ImageAlign.TextTop:
							Style["vertical-align"] = "text-top";
							break;
						case ImageAlign.Top:
							Style["vertical-align"] = "top";
							break;
						case ImageAlign.Left:
							Style["float"] = "left";
							break;
						case ImageAlign.Right:
							Style["float"] = "right";
							break;
					}
					ImageAlign = ImageAlign.NotSet;
				}
				if(!string.IsNullOrEmpty(Attributes["align"])) {
					switch(Attributes["align"]) {
						case "absbottom":
						case "bottom":
							Style["vertical-align"] = "bottom";
							break;
						case "absmiddle":
						case "middle":
							Style["vertical-align"] = "middle";
							break;
						case "baseline":
							Style["vertical-align"] = "baseline";
							break;
						case "texttop":
							Style["vertical-align"] = "text-top";
							break;
						case "top":
							Style["vertical-align"] = "top";
							break;
						case "left":
							Style["float"] = "left";
							break;
						case "right":
							Style["float"] = "right";
							break;
					}
					Attributes.Remove("align");
				}
			}
			if (MvcUtils.RenderMode != MvcRenderMode.None) 
				ImageUrl = MvcUtils.ResolveClientUrl(ImageUrl);
			base.AddAttributesToRender(writer);
		}
	}
	[ToolboxItem(false)]
	public class InternalHyperLink : HyperLink {
		bool needResolveClientUrl = true;
		bool isAlwaysHyperLink = false;
		bool requiresTooltip = false;
		public InternalHyperLink()
			: base() {
			this.needResolveClientUrl = true;
		}
		public InternalHyperLink(bool needResolveClientUrl)
			: base() {
			this.needResolveClientUrl = needResolveClientUrl;
		}
		public bool IsAlwaysHyperLink {
			get { return isAlwaysHyperLink; }
			set { isAlwaysHyperLink = value; }
		}
		public bool NeedResolveClientUrl {
			get { return needResolveClientUrl; }
		}
		public bool RequiresTooltip {
			get { return requiresTooltip; }
			set { requiresTooltip = value; }
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		protected void AddCustomAttributesToRender(HtmlTextWriter writer) {
			foreach(string key in Attributes.Keys)
				writer.AddAttribute(key, Attributes[key]);
		}
		protected void AddStyleAttributesToRender(HtmlTextWriter writer) {
			ControlStyle.AddAttributesToRender(writer, this);
		}
		void AddTooltipToRender(HtmlTextWriter writer) {
			if(ToolTip != "")
				writer.AddAttribute(HtmlTextWriterAttribute.Title, ToolTip);
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(ID != null)
				writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
			if(AccessKey != "")
				writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey);
			if(TabIndex != 0)
				writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString(NumberFormatInfo.InvariantInfo));
			AddTooltipToRender(writer);
			AddStyleAttributesToRender(writer);
			AddCustomAttributesToRender(writer);
			if(Enabled) {
				if(NavigateUrl != "") {
					string url = string.Empty;
					if (NeedResolveClientUrl) {
						url = MvcUtils.RenderMode != MvcRenderMode.None ? MvcUtils.ResolveClientUrl(NavigateUrl) :
							ResolveClientUrl(NavigateUrl);
					} else
						url =  NavigateUrl;
					writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
				}
				if(Target != "")
					writer.AddAttribute(HtmlTextWriterAttribute.Target, Target);
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			if(IsAlwaysHyperLink || (NavigateUrl != "" && Enabled)) {
				base.Render(writer);
			}
			else {
				if(HasControls())
					RenderContents(writer);
				else {
					if(NeedSpanIfNotLinkRender()) {
						if(ID != null)
							writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
						AddCustomAttributesToRender(writer);
						AddStyleAttributesToRender(writer);
						if(RequiresTooltip)
							AddTooltipToRender(writer);
						writer.RenderBeginTag(HtmlTextWriterTag.Span);
						writer.Write(Text);
						writer.RenderEndTag();
					}
					else
						writer.Write(Text);
				}
			}
		}
		public new string ResolveClientUrl(string relativeUrl) {
			if(DesignMode && relativeUrl.ToLower().StartsWith("javascript:")) 
				return "#";
			return base.ResolveClientUrl(relativeUrl);
		}
		protected bool NeedSpanIfNotLinkRender() {
			return RequiresTooltip || !string.IsNullOrEmpty(ID) || !ControlStyle.IsEmpty || (Attributes.Count > 0);
		}
	}
	public class KeyboardSupportInputHelper : ASPxInternalWebControl {
		private const string DefaultInputID = "KBS";
		protected const string DefaultKBSDivClassName = "dxKBSW";
		private WebControl input = null;
		private WebControl inputWrapper = null; 
		private bool inlineMode = false;
		private string inputID = "";
		bool isAccessibilityCompliant = false;
		public WebControl Input {
			get { return input; }
			set { input = value; }
		}
		public KeyboardSupportInputHelper()
			: this(DefaultInputID) {
		}
		public KeyboardSupportInputHelper(string id)
			: base() {
			this.inputID = id;
		}
		public KeyboardSupportInputHelper(string id, bool inlineMode, bool isAccessibilityCompliant)
			: this(id) {
			this.inlineMode = inlineMode;
			this.isAccessibilityCompliant = isAccessibilityCompliant;
		}
		protected override void ClearControlFields() {
			Input = null;
		}
		protected override void CreateControlHierarchy() {
			Input = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			WebControl parent = this;
			if (Browser.IsOpera || Browser.Family.IsWebKit) {
				inputWrapper = RenderUtils.CreateWebControl(inlineMode ? HtmlTextWriterTag.Span : HtmlTextWriterTag.Div);
				Controls.Add(inputWrapper);
				parent = inputWrapper;
			}
			parent.Controls.Add(Input);
		}
		protected override void PrepareControlHierarchy() {
			input.ID = inputID;
			RenderUtils.SetStringAttribute(input, "type", this.isAccessibilityCompliant ? "hidden" : "text");
			if(!this.isAccessibilityCompliant) {
				string[] attrs = new string[] { "border-width", "width", "height", "padding", "margin" };
				foreach(string attr in attrs)
					RenderUtils.SetStyleStringAttribute(input, attr, "0");
				RenderUtils.SetStyleStringAttribute(input, "position", "relative");
				RenderUtils.SetStyleStringAttribute(input, "background-color", "transparent");
				RenderUtils.SetStyleStringAttribute(input, "display", "block");
				if(Browser.IsOpera)
					RenderUtils.SetOpacity(input, 1);
				if(Browser.Family.IsWebKit) {
					if(Browser.IsChrome || (Browser.Platform.IsMacOSMobile && Browser.IsSafari)) 
						RenderUtils.SetStyleStringAttribute(input, "font-size", "0pt");
					RenderUtils.SetStyleStringAttribute(input, "opacity", "0");
				}
				RenderUtils.SetStringAttribute(input, "readonly", "readonly");
				if(inputWrapper != null) {
					inputWrapper.CssClass = DefaultKBSDivClassName;
				}
				Input.TabIndex = TabIndex;
			}
		}
	}
	public delegate TableCell LoadingPanelCreateTemplate(TableRow parent);
	public class LoadingPanelControl : ASPxInternalWebControl {
		private const string TextLabelID = "TL";
		private string id = "";
		private ImageProperties image = new ImageProperties();
		private LoadingPanelStyle style = new LoadingPanelStyle();
		private Paddings paddings = new Paddings();
		private Unit imageSpacing = Unit.Empty;
		private bool designModeVisible = false;
		private bool hasAbsolutePosition = true;
		private SettingsLoadingPanel settings = null;
		private LoadingPanelCreateTemplate templateCreateDelegate;
		bool rtl;
		public LoadingPanelControl(bool rtl)
			: base() {
			this.rtl = rtl;
		}
		public new string ID {
			get { return id; }
			set { id = value; }
		}
		public string Text {
			get { return (Settings != null) ? Settings.Text : ""; }
		}
		public ImageProperties Image {
			get { return image; }
			set { image = value; }
		}
		public ImagePosition ImagePosition {
			get { return (Settings != null) ? Settings.ImagePosition : ImagePosition.Left; }
		}
		public new LoadingPanelStyle Style {
			get { return style; }
			set { style = value; }
		}
		public CssStyleCollection StyleAttributes {
			get { return base.Style; }
		}
		public Paddings Paddings {
			get { return paddings; }
			set { paddings = value; }
		}
		public Unit ImageSpacing {
			get { return imageSpacing; }
			set { imageSpacing = value; }
		}
		public bool ShowImage {
			get { return (Settings != null) ? Settings.ShowImage : true; }
		}
		public bool DesignModeVisible {
			get { return designModeVisible; }
			set { designModeVisible = value; }
		}
		public bool HasAbsolutePosition {
			get { return hasAbsolutePosition; }
			set { hasAbsolutePosition = value; }
		}
		public SettingsLoadingPanel Settings {
			get { return settings; }
			set { settings = value; }
		}
		public LoadingPanelCreateTemplate TemplateCreateDelegate {
			get { return templateCreateDelegate; }
			set { templateCreateDelegate = value; }
		}
		public WebControl Div { get; private set; }
		public Table Table { get; private set; }
		public TableCell ImageCell { get; private set; }
		public Image ImageControl { get; private set; }
		public TableCell TextCell { get; private set; }
		public ITextControl TextControl { get; private set; }
		public TableCell TemplateCell { get; private set; }
		protected override void ClearControlFields() {
			Div = null;
			Table = null;
			ImageControl = null;
			ImageCell = null;
			TextCell = null;
			TextControl = null;
			TemplateCell = null;
		}
		protected override void CreateControlHierarchy() {
			WebControl parentControl = this;
			Table = RenderUtils.CreateTable();
			if(Browser.IsOpera || Browser.IsFirefox) {
				Div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(Div);
				parentControl = Div;
				Table.Height = Unit.Percentage(100);
			}
			parentControl.Controls.Add(Table);
			TableRow row = RenderUtils.CreateTableRow();
			Table.Controls.Add(row);
			TemplateCell = (TemplateCreateDelegate != null) ? TemplateCreateDelegate(row) : null;
			if(TemplateCell == null) {
				if(ShowImage && ImagePosition == ImagePosition.Left)
					CreateImageCell(row);
				if(Text != "" || ImagePosition == ImagePosition.Top ||
					ImagePosition == ImagePosition.Bottom)
					CreateTextCell(row);
				if(ShowImage && ImagePosition == ImagePosition.Right)
					CreateImageCell(row);
				if(!ShowImage && string.IsNullOrEmpty(Text))
					CreateFakeCell(row);
			}
		}
		protected override void PrepareControlHierarchy() {
			WebControl parentControl = (Div != null) ? Div : Table;
			if (parentControl != null){
				RenderUtils.AssignAttributes(this, parentControl, true); 
				parentControl.Width = Width;
				parentControl.Height = Height;
				parentControl.ID = ID;
				if(!DesignMode) {
					if(HasAbsolutePosition) {
						parentControl.Style.Add("left", "0px");
						parentControl.Style.Add("top", "0px");
						parentControl.Style.Add("z-index", RenderUtils.LoadingPanelZIndex.ToString());
						parentControl.Style.Add("display", "none");
					}
				}
				else
					parentControl.Visible = DesignModeVisible;
			}
			if(Div != null)
				Div.Attributes.Add("align", "center");
			if(Table != null) {
				Style.AssignToControl(Table, AttributesRange.Common | AttributesRange.Font);
				if(this.rtl)
					Table.Attributes["dir"] = "rtl";
			}
			if (ImageCell != null)
				PrepareImageCell();
			if (ImageControl != null)
				PrepareImage();
			if (TextCell != null)
				PrepareTextCell();
		}
		private void CreateFakeCell(TableRow parent) {
			TableCell fakeCell = RenderUtils.CreateTableCell();
			parent.Cells.Add(fakeCell);
		}
		protected virtual void CreateImageCell(TableRow parent) {
			ImageCell = RenderUtils.CreateTableCell();
			parent.Cells.Add(ImageCell);
			CreateImage(ImageCell);
		}
		protected virtual void CreateImage(TableCell parent) {
			ImageControl = RenderUtils.CreateImage();
			parent.Controls.Add(ImageControl);
		}
		protected virtual void CreateTextCell(TableRow parent) {
			TextCell = RenderUtils.CreateTableCell();
			parent.Cells.Add(TextCell);
			if (ShowImage && ImagePosition == ImagePosition.Top) {
				CreateImage(TextCell);
				if (Text != "")
					TextCell.Controls.Add(RenderUtils.CreateBr());
			}
			if(Text != "") {
				Label label = RenderUtils.CreateLabel();
				TextCell.Controls.Add(label);
				label.ID = TextLabelID;
				TextControl = label;
			}
			if (ShowImage && ImagePosition == ImagePosition.Bottom) {
				if(Text != "")
					TextCell.Controls.Add(RenderUtils.CreateBr());
				CreateImage(TextCell);
			}
		}
		protected virtual void PrepareImageCell() {
			Image.AssignToControl(ImageControl, DesignMode);
			RenderUtils.SetPaddings(ImageCell, Paddings);
			if(TextCell != null)
				CorrectSpacingWithPaddings(ImageCell, "padding-right", "padding-left");
			if(ImageCell != null)
				RenderUtils.AppendDefaultDXClassName(ImageCell, "dx");
		}
		protected virtual void PrepareImage() {
			Image.AssignToControl(ImageControl, DesignMode);
			ImageControl.ImageAlign = ImageAlign.Middle;
			RenderUtils.AppendDefaultDXClassName(ImageControl, GetImagePositionCssClassName());
			if (TextControl != null) {
				ImagePosition imagePosition = GetImagePositionAccordingToRtl();
				if(imagePosition == ImagePosition.Top)
					RenderUtils.SetVerticalMargins(ImageControl, Unit.Empty, ImageSpacing);
				else if(imagePosition == ImagePosition.Bottom)
					RenderUtils.SetVerticalMargins(ImageControl, ImageSpacing, Unit.Empty);
				else if(imagePosition == ImagePosition.Left)
					RenderUtils.SetHorizontalMargins(ImageControl, Unit.Empty, ImageSpacing);
				else if(imagePosition == ImagePosition.Right)
					RenderUtils.SetHorizontalMargins(ImageControl, ImageSpacing, Unit.Empty);
			}
		}
		protected virtual string GetImagePositionCssClassName() {
			return string.Format("{0}-imgPos{1}", LoadingPanelStyles.DefaultLoadingPanelClassNamePrefix, GetImagePositionAccordingToRtl());
		}
		private ImagePosition GetImagePositionAccordingToRtl() {
			if(this.rtl) {
				switch(ImagePosition) {
					case ImagePosition.Right:
						return ImagePosition.Left;
					case ImagePosition.Left:
						return ImagePosition.Right;
				}
			}
			return ImagePosition;
		}
		protected virtual void PrepareTextCell() {
			AttributesRange range = AttributesRange.Cell | AttributesRange.Font; 
			Style.AssignToControl(TextCell, range);
			RenderUtils.SetPaddings(TextCell, Paddings);
			if (ImageCell != null) {
				CorrectSpacingWithPaddings(TextCell, "padding-left", "padding-right");
				if(!Width.IsEmpty)
					TextCell.Width = Unit.Percentage(100);
			}
			RenderUtils.AppendDefaultDXClassName(TextCell, "dx");
			if(TextControl != null)
				TextControl.Text = Text;
		}
		private void CorrectSpacingWithPaddings(TableCell cell, string paddingNameForLeftPosition, string paddingNameForRightPosition) {
			ImagePosition imagePosition = GetImagePositionAccordingToRtl();
			if(imagePosition == ImagePosition.Left)
				RenderUtils.SetStyleUnitAttribute(cell, paddingNameForLeftPosition, 0);
			else if(imagePosition == ImagePosition.Right)
				RenderUtils.SetStyleUnitAttribute(cell, paddingNameForRightPosition, 0);
		}
	}
	public class ButtonControlBase : ASPxInternalWebControl {
		private string buttonImageID = string.Empty;
		private ImagePropertiesBase buttonImage = null;
		private ImagePosition buttonImagePosition = ImagePosition.Left;
		private Unit buttonImageSpacing = Unit.Empty;
		private AppearanceStyleBase buttonStyle = null;
		private string buttonText = string.Empty;
		private string buttonUrl = string.Empty;
		bool isRtl;
		public ButtonControlBase()
			: base() {
		}
		public ButtonControlBase(string text, ImagePropertiesBase image, ImagePosition imagePosition, string url)
			: this() {
			this.buttonText = text;
			this.buttonImage = image;
			this.buttonImagePosition = imagePosition;
			this.buttonUrl = url;
		}
		public string ButtonImageID {
			get { return buttonImageID; }
			set { buttonImageID = value; }
		}
		public ImagePropertiesBase ButtonImage {
			get { return buttonImage; }
			set {
				buttonImage = value;
				OnButtonPropertyChanged();
			}
		}
		public ImagePosition ButtonImagePosition {
			get { return buttonImagePosition; }
			set {
				if(buttonImagePosition != value) {
					buttonImagePosition = value;
					OnButtonPropertyChanged();
				}
			}
		}
		public Unit ButtonImageSpacing {
			get { return buttonImageSpacing; }
			set { buttonImageSpacing = value; }
		}
		public AppearanceStyleBase ButtonStyle {
			get { return buttonStyle; }
			set { buttonStyle = value; }
		}
		public string ButtonText {
			get { return buttonText; }
			set {
				if(buttonText != value) {
					buttonText = value;
					OnButtonPropertyChanged();
				}
			}
		}
		public string ButtonUrl {
			get { return buttonUrl; }
			set {
				if(buttonUrl != value) {
					buttonUrl = value;
					OnButtonPropertyChanged();
				}
			}
		}
		public bool IsRightToLeft {
			get { return isRtl; }
			set {
				if(isRtl != value) {
					isRtl = value;
					OnButtonPropertyChanged();
				}
			}
		}
		protected virtual void OnButtonPropertyChanged() { 
		}
	}
	public class SimpleButtonControl : ButtonControlBase {
		private LiteralControl textControl = null;
		private WebControl textSpan = null;
		private HyperLink hyperlink = null;
		private Image imageControl = null;
		public SimpleButtonControl()
			: base() {
		}
		public SimpleButtonControl(string text, ImagePropertiesBase image, ImagePosition imagePosition, string url)
			: base(text, image, imagePosition, url) {
		}
		protected LiteralControl TextControl {
			get { return textControl; }
		}
		protected WebControl TextSpan {
			get { return textSpan; }
		}
		protected HyperLink Hyperlink {
			get { return hyperlink; }
			set { hyperlink = value; }
		}
		protected Image ImageControl {
			get { return imageControl;  }
		}
		protected override void ClearControlFields() {
			this.imageControl = null;
			this.hyperlink = null;
			this.textControl = null;
			this.textSpan = null;
		}
		protected override void CreateControlHierarchy() {
			WebControl parent = this;
			if(!string.IsNullOrEmpty(ButtonUrl)) {
				this.hyperlink = RenderUtils.CreateHyperLink();
				Controls.Add(Hyperlink);
				parent = Hyperlink;
			}
			if(!IsImageEmpty() && (ButtonImagePosition == ImagePosition.Left || ButtonImagePosition == ImagePosition.Top)) {
				CreateImage(parent);
				if(ButtonImagePosition == ImagePosition.Top && !IsTextEmpty() && !IsImageEmpty())
					parent.Controls.Add(RenderUtils.CreateBr());
			}
			if(!IsTextEmpty())
				CreateText(parent);
			if(!IsImageEmpty() && (ButtonImagePosition == ImagePosition.Right || ButtonImagePosition == ImagePosition.Bottom)) {
				if(ButtonImagePosition == ImagePosition.Bottom && !IsTextEmpty() && !IsImageEmpty())
					parent.Controls.Add(RenderUtils.CreateBr());
				CreateImage(parent);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(Hyperlink != null) {
				RenderUtils.PrepareHyperLink(Hyperlink, "", ButtonUrl, "", "", Enabled);
				if(ButtonStyle != null && !ButtonStyle.IsEmpty)
					ButtonStyle.AssignToControl(Hyperlink, AttributesRange.Font);
			}
			if(ImageControl != null)
				PrepareImage();
			if(TextControl != null)
				PrepareText();
		}
		protected void CreateImage(WebControl parent) {
			this.imageControl = RenderUtils.CreateImage();
			parent.Controls.Add(ImageControl);
		}
		protected void CreateText(WebControl parent) {
			if(!IsImageEmpty()) {
				this.textSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				parent.Controls.Add(TextSpan);
				parent = TextSpan;
			}
			this.textControl = RenderUtils.CreateLiteralControl();
			parent.Controls.Add(TextControl);
		}
		protected void PrepareImage() {
			if(!string.IsNullOrEmpty(ButtonImageID))
				ImageControl.ID = ButtonImageID;
			if(ButtonImage != null && !ButtonImage.IsEmpty)
				ButtonImage.AssignToControl(ImageControl, DesignMode, !Enabled);
			if(TextControl != null) {
				RenderUtils.SetMargins(ImageControl, GetImageMargins());
				if(ButtonStyle != null && !ButtonStyle.IsEmpty)
					RenderUtils.SetVerticalAlignClass(ImageControl, ButtonStyle.VerticalAlign);
			}
		}
		protected void PrepareText() {
			TextControl.Text = ButtonText;
			if(TextSpan != null) {
				if(ButtonStyle != null && !ButtonStyle.IsEmpty) {
					ButtonStyle.AssignToControl(TextSpan, AttributesRange.Font);
					RenderUtils.SetVerticalAlignClass(TextSpan, ButtonStyle.VerticalAlign);
				}
			}
		}
		protected override void OnButtonPropertyChanged() {
			ResetControlHierarchy();
		}
		private Paddings GetImageMargins() {
			Paddings margins = new Paddings();
			if(TextControl != null) {
				switch(ButtonImagePosition) {
					case ImagePosition.Top:
						margins.PaddingBottom = ButtonImageSpacing;
						break;
					case ImagePosition.Bottom:
						margins.PaddingTop = ButtonImageSpacing;
						break;
					case ImagePosition.Right:
						if(IsRightToLeft)
							margins.PaddingRight = ButtonImageSpacing;
						else
							margins.PaddingLeft = ButtonImageSpacing;
						break;
					case ImagePosition.Left:
						if(IsRightToLeft)
							margins.PaddingLeft = ButtonImageSpacing;
						else
							margins.PaddingRight = ButtonImageSpacing;
						break;
				}
			}
			return margins;
		}
		protected bool IsImageEmpty() {
			return ButtonImage == null || ButtonImage.IsEmpty;
		}
		protected bool IsTextEmpty() {
			return string.IsNullOrEmpty(ButtonText);
		}
	}
	public class TableCellButtonControl : InternalTableCell {
		private string buttonID = string.Empty;
		private string buttonImageID = string.Empty;
		private ImagePropertiesBase buttonImage = null;
		private ImagePosition buttonImagePosition = ImagePosition.Left;
		private Unit buttonImageSpacing = Unit.Empty;
		private string buttonOnClick = string.Empty;
		private string buttonOnMouseDown = string.Empty;
		private string buttonOnMouseUp = string.Empty;
		private Paddings buttonPaddings = null;
		private AppearanceStyleBase buttonStyle = null;
		private AppearanceStyleBase buttonDisabledStyle = null;
		private string buttonText = string.Empty;
		private string buttonUrl = string.Empty;
		private SimpleButtonControl buttonControl = null;
		public TableCellButtonControl()
			: base() {
		}
		public string ButtonID {
			get { return buttonID; }
			set { buttonID = value; }
		}
		public string ButtonImageID {
			get { return buttonImageID; }
			set { buttonImageID = value; }
		}
		public ImagePropertiesBase ButtonImage {
			get { return buttonImage; }
			set {
				buttonImage = value;
				ResetControlHierarchy();
			}
		}
		public ImagePosition ButtonImagePosition {
			get { return buttonImagePosition; }
			set { 
				buttonImagePosition = value;
				ResetControlHierarchy();
			}
		}
		public Unit ButtonImageSpacing {
			get { return buttonImageSpacing; }
			set { buttonImageSpacing = value; }
		}
		public string ButtonOnClick {
			get { return buttonOnClick; }
			set { buttonOnClick = value; }
		}
		public string ButtonOnMouseDown {
			get { return buttonOnMouseDown; }
			set { buttonOnMouseDown = value; }
		}
		public string ButtonOnMouseUp {
			get { return buttonOnMouseUp; }
			set { buttonOnMouseUp = value; }
		}
		public Paddings ButtonPaddings {
			get { return buttonPaddings; }
			set { buttonPaddings = value; }
		}
		public AppearanceStyleBase ButtonStyle {
			get { return buttonStyle; }
			set { buttonStyle = value; }
		}
		public AppearanceStyleBase ButtonButtonStyle {
			get { return buttonDisabledStyle; }
			set { buttonDisabledStyle = value; }
		}
		public string ButtonText {
			get { return buttonText; }
			set { 
				buttonText = value;
				ResetControlHierarchy();
			}
		}
		public string ButtonUrl {
			get { return buttonUrl; }
			set { 
				buttonUrl = value;
				ResetControlHierarchy();
			}
		}
		protected SimpleButtonControl ButtonControl {
			get { return buttonControl; }
		}
		protected override void ClearControlFields() {
			this.buttonControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.buttonControl = new SimpleButtonControl(ButtonText, ButtonImage, ButtonImagePosition, ButtonUrl);
			Controls.Add(ButtonControl);
		}
		protected override void PrepareControlHierarchy() {
			ButtonControl.ButtonImageID = ButtonImageID;
			ButtonControl.ButtonImageSpacing = ButtonImageSpacing;
			ButtonControl.Enabled = Enabled;
			ButtonControl.ButtonStyle = ButtonStyle;
			ButtonControl.Visible = Visible;
			if(ButtonStyle != null)
				ButtonStyle.AssignToControl(this, AttributesRange.All);
			if(ButtonPaddings != null)
				RenderUtils.SetPaddings(this, ButtonPaddings);
			if(!string.IsNullOrEmpty(ButtonID))
				ID = ButtonID;
			if(!string.IsNullOrEmpty(ButtonOnClick))
				RenderUtils.SetStringAttribute(this, "onclick", ButtonOnClick);
			if(!string.IsNullOrEmpty(ButtonOnMouseDown))
				RenderUtils.SetStringAttribute(this, TouchUtils.TouchMouseDownEventName, ButtonOnMouseDown);
			if(!string.IsNullOrEmpty(ButtonOnMouseUp))
				RenderUtils.SetStringAttribute(this, TouchUtils.TouchMouseUpEventName, ButtonOnMouseUp);
		}
	}
	public class DivButtonControl: InternalHtmlControl {
		private string buttonID = string.Empty;
		private string buttonImageID = string.Empty;
		private ImagePropertiesBase buttonImage = null;
		private ImagePosition buttonImagePosition = ImagePosition.Left;
		private Unit buttonImageSpacing = Unit.Empty;
		private string buttonOnClick = string.Empty;
		private string buttonOnMouseDown = string.Empty;
		private string buttonOnMouseUp = string.Empty;
		private Paddings buttonPaddings = null;
		private AppearanceStyleBase buttonStyle = null;
		private AppearanceStyleBase buttonDisabledStyle = null;
		private string buttonText = string.Empty;
		private string buttonUrl = string.Empty;
		private SimpleButtonControl buttonControl = null;
		public DivButtonControl()
			: base(HtmlTextWriterTag.Div) {
		}
		public string ButtonID {
			get { return buttonID; }
			set { buttonID = value; }
		}
		public string ButtonImageID {
			get { return buttonImageID; }
			set { buttonImageID = value; }
		}
		public ImagePropertiesBase ButtonImage {
			get { return buttonImage; }
			set {
				buttonImage = value;
				ResetControlHierarchy();
			}
		}
		public ImagePosition ButtonImagePosition {
			get { return buttonImagePosition; }
			set {
				buttonImagePosition = value;
				ResetControlHierarchy();
			}
		}
		public Unit ButtonImageSpacing {
			get { return buttonImageSpacing; }
			set { buttonImageSpacing = value; }
		}
		public string ButtonOnClick {
			get { return buttonOnClick; }
			set { buttonOnClick = value; }
		}
		public string ButtonOnMouseDown {
			get { return buttonOnMouseDown; }
			set { buttonOnMouseDown = value; }
		}
		public string ButtonOnMouseUp {
			get { return buttonOnMouseUp; }
			set { buttonOnMouseUp = value; }
		}
		public Paddings ButtonPaddings {
			get { return buttonPaddings; }
			set { buttonPaddings = value; }
		}
		public AppearanceStyleBase ButtonStyle {
			get { return buttonStyle; }
			set { buttonStyle = value; }
		}
		public AppearanceStyleBase ButtonButtonStyle {
			get { return buttonDisabledStyle; }
			set { buttonDisabledStyle = value; }
		}
		public string ButtonText {
			get { return buttonText; }
			set {
				buttonText = value;
				ResetControlHierarchy();
			}
		}
		public string ButtonUrl {
			get { return buttonUrl; }
			set {
				buttonUrl = value;
				ResetControlHierarchy();
			}
		}
		protected SimpleButtonControl ButtonControl {
			get { return buttonControl; }
		}
		protected override void ClearControlFields() {
			this.buttonControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.buttonControl = new SimpleButtonControl(ButtonText, ButtonImage, ButtonImagePosition, ButtonUrl);
			Controls.Add(ButtonControl);
		}
		protected override void PrepareControlHierarchy() {
			ButtonControl.ButtonImageID = ButtonImageID;
			ButtonControl.ButtonImageSpacing = ButtonImageSpacing;
			ButtonControl.Enabled = Enabled;
			ButtonControl.ButtonStyle = ButtonStyle;
			ButtonControl.Visible = Visible;
			if(ButtonStyle != null)
				ButtonStyle.AssignToControl(this, AttributesRange.All);
			if(ButtonPaddings != null)
				RenderUtils.SetPaddings(this, ButtonPaddings);
			if(!string.IsNullOrEmpty(ButtonID))
				ID = ButtonID;
			if(!string.IsNullOrEmpty(ButtonOnClick))
				RenderUtils.SetStringAttribute(this, "onclick", ButtonOnClick);
			if(!string.IsNullOrEmpty(ButtonOnMouseDown))
				RenderUtils.SetStringAttribute(this, "onmousedown", ButtonOnMouseDown);
			if(!string.IsNullOrEmpty(ButtonOnMouseUp))
				RenderUtils.SetStringAttribute(this, "onmouseup", ButtonOnMouseUp);
		}
	}
	public class ItemsControlCellInfo {
		public TableCell Cell = null;
		public bool IsIndentCell = false;
		public int ColumnSpan = 1;
		public ItemsControlCellInfo(TableCell cell, bool isIndentCell, int columnSpan) {
			Cell = cell;
			IsIndentCell = isIndentCell;
			ColumnSpan = columnSpan;
		}
	}
	public abstract class ItemsControl<T> : ASPxInternalWebControl {
		private List<T> items = new List<T>();
		private int repeatColumns = 0;
		private RepeatDirection repeatDirection = RepeatDirection.Vertical;
		private RepeatLayout repeatLayout = RepeatLayout.Table;
		private AppearanceStyleBase itemsControlStyle = null;
		private Paddings itemsControlPaddings = null;
		private string itemsControlMainCellCssClass = string.Empty;
		private Unit itemSpacing = Unit.Empty;
		private Table mainTable = null;
		private TableCell mainCell = null;
		private Table table = null;
		private WebControl span = null;
		private List<TableCell> itemCells = null;
		private Dictionary<int, Control> itemControls = null;
		private List<ItemsControlCellInfo> indentCells = null;
		public ItemsControl(List<T> items, int repeatColumns, RepeatDirection repeatDirection, RepeatLayout repeatLayout) {
			this.items = items;
			this.repeatColumns = repeatColumns;
			this.repeatDirection = repeatDirection;
			this.repeatLayout = repeatLayout;
		}
		public List<T> Items {
			get { return items; }
			set {
				items = value;
				ResetControlHierarchy();
			}
		}
		public int RepeatColumns {
			get { return repeatColumns; }
			set { 
				repeatColumns = value;
				ResetControlHierarchy();
			}
		}
		public RepeatDirection RepeatDirection {
			get { return repeatDirection; }
			set { 
				repeatDirection = value;
				ResetControlHierarchy();
			}
		}
		public RepeatLayout RepeatLayout {
			get { return repeatLayout; }
			set { 
				repeatLayout = value;
				ResetControlHierarchy();
			}
		}
		public Unit ItemSpacing {
			get { return itemSpacing; }
			set {
				if (value != itemSpacing) {
					itemSpacing = value;
					ResetControlHierarchy();
				}
			}
		}
		public AppearanceStyleBase ItemsControlStyle {
			get { return itemsControlStyle; }
			set { itemsControlStyle = value; }
		}
		public Paddings ItemsControlPaddings {
			get { return itemsControlPaddings; }
			set { itemsControlPaddings = value; }
		}
		public string ItemsControlMainCellCssClass {
			get { return itemsControlMainCellCssClass; }
			set { itemsControlMainCellCssClass = value; }
		}
		protected internal int ItemCount {
			get { return (Items != null) ? Items.Count : 0; }
		}
		protected internal Table MainTable {
			get { return mainTable; }
		}
		protected internal TableCell MainCell {
			get { return mainCell; }
		}
		protected internal Table Table {
			get { return table; }
		}
		protected internal WebControl Span {
			get { return span; }
		}
		protected internal List<TableCell> ItemCells {
			get { return itemCells; }
		}
		protected internal Dictionary<int, Control> ItemControls {
			get { return itemControls; }
		}
		protected internal List<ItemsControlCellInfo> IndentCells {
			get { return indentCells; }
		}
		protected override bool HasContent() {
			return ItemCount > 0;
		}
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.mainCell = null;
			this.table = null;
			this.span = null;
			this.itemCells = null;
			this.itemControls = null;
			this.indentCells = null;
		}
		protected override void CreateControlHierarchy() {
			this.itemControls = new Dictionary<int, Control>();
			this.mainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow mainRow = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(mainRow);
			this.mainCell = RenderUtils.CreateTableCell();
			mainRow.Cells.Add(MainCell);
			OnCreatingMainCellContent(MainCell);
			if(HasTable()) {
				this.table = RenderUtils.CreateTable(ItemSpacing.Value > 0);
				MainCell.Controls.Add(Table);
				this.itemCells = new List<TableCell>();
				this.indentCells = new List<ItemsControlCellInfo>();
			}
			int rowCount = GetRowCount();
			for(int i = 0; i < rowCount; i++) {
				if(HasTable()) {
					TableRow row = RenderUtils.CreateTableRow();
					Table.Rows.Add(row);
					CreateTableRowHierarchy(row, i);
					if(i != (rowCount - 1))
						CreateTableIndentRow(Table);
				} else {
					CreateTableRowHierarchy(MainCell, i);
				}
			}
		}
		protected virtual void OnCreatingMainCellContent(TableCell mainCell) {
		}
		protected override void PrepareControlHierarchy() {
			if(MainTable != null && MainCell != null) {
				RenderUtils.AssignAttributes(this, MainTable);
				if(ItemsControlStyle != null) {
					ItemsControlStyle.AssignToControl(MainTable, AttributesRange.Font | AttributesRange.Common, true);
					ItemsControlStyle.AssignToControl(MainCell, AttributesRange.Cell);
				}
				if(ItemsControlPaddings != null)
					RenderUtils.SetPaddings(MainCell, ItemsControlPaddings);
				if(!string.IsNullOrEmpty(ItemsControlMainCellCssClass))
					RenderUtils.AppendDefaultDXClassName(MainCell, ItemsControlMainCellCssClass);
			}
			if(this.table != null && (Browser.IsFirefox && Browser.Version >= 3 || this.MainTable.Width.Type == UnitType.Percentage))
				this.table.Width = Unit.Percentage(100);
			if(ItemCells != null)
				PrepareItemCells();
			if(ItemControls != null)
				PrepareItemControls();
			if(IndentCells != null) 
				PrepareIndentCells();
		}
		protected internal void CreateTableIndentRow(Table parent) {
			TableRow row = RenderUtils.CreateTableRow();
			parent.Rows.Add(row);
			TableCell cell = RenderUtils.CreateIndentCell();
			row.Cells.Add(cell);
			ItemsControlCellInfo info = new ItemsControlCellInfo(cell, false, 2 * GetColumnCount() - 1);
			IndentCells.Add(info);
		}
		protected internal void CreateTableIndentCell(TableRow parent) {
			TableCell cell = RenderUtils.CreateIndentCell();
			parent.Cells.Add(cell);
			ItemsControlCellInfo info = new ItemsControlCellInfo(cell, true, 1);
			IndentCells.Add(info);
		}
		protected internal void CreateTableRowHierarchy(WebControl parent, int i) {
			int itemIndex = 0;
			int rowCount = GetRowCount();
			int columnCount = GetColumnCount();
			for(int j = 0; j < columnCount; j++) {
				if(RepeatDirection == RepeatDirection.Horizontal)
					itemIndex = i * columnCount + j;
				else
					itemIndex = j * rowCount + i;
				if(itemIndex < ItemCount) {
					if(HasTable()) {
						CreateItemCellInternal((TableRow)parent, itemIndex, Items[itemIndex]);
						if(j != (columnCount - 1))
							CreateTableIndentCell((TableRow)parent);
					} else {
						CreateItemControlInternal(parent, itemIndex, Items[itemIndex]);
					}
				} else if(HasTable()) {
					TableCell indentCell = RenderUtils.CreateIndentCell();
					((TableRow)parent).Cells.Add(indentCell);
					if(j != (columnCount - 1))
						CreateTableIndentCell((TableRow)parent);
				}
			}
			if(HasBr(i))
				parent.Controls.Add(RenderUtils.CreateBr());
		}
		protected virtual TableCell CreateItemCell(int index, T item) {
			return RenderUtils.CreateTableCell();
		}
		protected void CreateItemCellInternal(TableRow parent, int index, T item) {
			TableCell cell = CreateItemCell(index, item);
			parent.Cells.Add(cell);
			ItemCells.Add(cell);
			CreateItemControlInternal(cell, index, item);
		}
		protected abstract Control CreateItemControl(int index, T item);
		protected void CreateItemControlInternal(Control parent, int index, T item){
			Control control = CreateItemControl(index, item);
			if(control != null) {
				parent.Controls.Add(control);
				ItemControls.Add(index, control);
			}
		}
		protected void PrepareItemControls() {
			foreach (int i in ItemControls.Keys)
				PrepareItemControl(ItemControls[i], i, Items[i]);
		}
		protected virtual void PrepareItemControl(Control control, int index, T item) {
		}
		protected void PrepareItemCells() {
			for(int i = 0; i < ItemCells.Count; i++)
				PrepareItemCell(ItemCells[i], i, Items[i]);
		}
		protected virtual void PrepareItemCell(TableCell cell, int index, T item) {
		}
		protected void PrepareIndentCells() {
			foreach(ItemsControlCellInfo info in IndentCells) {
				RenderUtils.PrepareIndentCell(info.Cell,
					info.IsIndentCell ? ItemSpacing : Unit.Pixel(0),
					info.IsIndentCell ? Unit.Pixel(0) : ItemSpacing);
				if(info.ColumnSpan > 1)
					info.Cell.ColumnSpan = info.ColumnSpan;
				if(info.IsIndentCell)
					info.Cell.Visible = !(ItemSpacing.IsEmpty || ItemSpacing.Value == 0);
				else
					info.Cell.Parent.Visible = !(ItemSpacing.IsEmpty || ItemSpacing.Value == 0);
			}
		}
		protected internal int GetColumnCount() {
			if(RepeatColumns > 0) {
				if(RepeatDirection == RepeatDirection.Horizontal)
					return (ItemCount > RepeatColumns) ? RepeatColumns : ItemCount;
				else {
					int rowCount = ItemCount / RepeatColumns;
					if(ItemCount % RepeatColumns > 0) rowCount++;
					int colCount = ItemCount / rowCount;
					if(ItemCount % rowCount > 0) colCount++;
					return colCount;
				}
			} else {
				if(RepeatDirection == RepeatDirection.Horizontal)
					return ItemCount;
				else
					return 1;
			}
		}
		protected internal int GetRowCount() {
			if(RepeatColumns > 0) {
				int rowCount = ItemCount / RepeatColumns;
				if(ItemCount % RepeatColumns > 0) rowCount++;
				return rowCount;
			} else {
				if(RepeatDirection == RepeatDirection.Horizontal)
					return 1;
				else if(ItemCount > 0)
					return ItemCount;
				else
					return 0;
			}
		}
		protected internal bool HasTable() {
			return RepeatLayout == RepeatLayout.Table;
		}
		protected internal bool HasBr(int rowIndex) {
			return RepeatLayout == RepeatLayout.Flow &&
				(rowIndex < (GetRowCount() - 1)) && (RepeatColumns < ItemCount);
		}
	}
	public class SharedCacheControl : ASPxInternalWebControl {
		Dictionary<string, object> cache;
		SharedCacheControl owner = null;
		public SharedCacheControl() {
			this.cache = new Dictionary<string, object>();
		}
		public SharedCacheControl(SharedCacheControl owner) {
			this.cache = owner.Cache;
			this.owner = owner;
		}
		Dictionary<string, object> Cache { get { return cache; } }
		protected SharedCacheControl Owner { get { return owner; } }
		protected delegate T GetValueDelegate<T>();
		protected T GetCachedValue<T>(string name, GetValueDelegate<T> getValue) {
			if(!Cache.ContainsKey(name))
				Cache.Add(name, getValue());
			return (T)Cache[name];
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			if(Owner == null)
				Cache.Clear();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Owner == null)
				Cache.Clear();
		}
	}
}
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ContentControlCollection : ControlCollection {
		public ContentControlCollection(Control owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("ContentControlCollectionItem")]
#endif
		public new ContentControl this[int i] {
			get { return (ContentControl)base[i]; }
		}
		public override void Add(Control child) {
			CheckCollectionAndPotentialChild(child);
			base.Add(child);
		}
		public override void AddAt(int index, Control child) {
			CheckCollectionAndPotentialChild(child);
			base.AddAt(index, child);
		}
		private void CheckCollectionAndPotentialChild(Control child) {
			if (!IsChildTypeValid(child))
				throw new ArgumentException(string.Format(StringResources.ContentCollectionInvalidChild, GetType().Name, GetChildType().Name));
			if(Count > 0)
				throw new Exception(string.Format(StringResources.ContentCollectionInvalidChildCount, GetType().Name));
		}
		internal virtual bool IsChildTypeValid(Control child) {
			return child.GetType() == GetChildType();
		}
		protected virtual Type GetChildType() {
			return typeof(ContentControl);
		}
	}
	[Browsable(false), ToolboxItem(false), ParseChildren(false), PersistChildren(true)]
	public class ContentControl : ASPxInternalWebControl, IContentContainer, IAttributeAccessor {
		private string designerRegionAttribute = null;
#if !SL
	[DevExpressWebLocalizedDescription("ContentControlID")]
#endif
		public override string ID {
			get { return base.ID; }
			set { base.ID = value; }
		}
		protected bool ShouldSerializeID() { return false; }
#if !SL
	[DevExpressWebLocalizedDescription("ContentControlVisible")]
#endif
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		protected bool ShouldSerializeVisible() { return false; }
#if !SL
	[DevExpressWebLocalizedDescription("ContentControlSupportsDisabledAttribute")]
#endif
		public override bool SupportsDisabledAttribute {
			get { return base.SupportsDisabledAttribute; }
		}
		protected bool ShouldSerializeSupportsDisabledAttribute() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DesignerRegionAttribute {
			get { return designerRegionAttribute; }
			set { designerRegionAttribute = value; }
		}
		public override bool HasControls() {
			if((Controls == null) || (Controls.Count == 0))
				return false;
			foreach(Control control in Controls) {
				if(!(control is LiteralControl))
					return true;
				else {
					string text = (control as LiteralControl).Text;
					if(!string.IsNullOrEmpty(text) && text.Trim(new char[] { ' ', '\n', '\r', '\t' }) != String.Empty)
						return true;
				}
			}
			return false;
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			if(!string.IsNullOrEmpty(DesignerRegionAttribute))
				writer.AddAttribute(GetDesignerRegionAttributeName(), DesignerRegionAttribute);
		}
		[System.Security.SecuritySafeCritical]
		string GetDesignerRegionAttributeName() {   
			return System.Web.UI.Design.DesignerRegion.DesignerRegionAttributeName;
		}
		protected void RenderContentsWithoutControls(HtmlTextWriter writer) {
			StringBuilder contentBuilder = new StringBuilder();
			using(StringWriter contentWriter = new StringWriter(contentBuilder)) {
				using(HtmlTextWriter tempWriter = new HtmlTextWriter(contentWriter)) {
					base.RenderChildren(tempWriter);
				}
			}
			string renderedContent = contentBuilder.ToString();
			if(string.IsNullOrEmpty(renderedContent.Trim()))
				writer.Write("&nbsp;");
			else
				writer.Write(renderedContent);
		}
		protected override void RenderContents(HtmlTextWriter writer) {
			if(DesignMode) {
				ASPxWebControl owningControl = FindOwningControl();
				if(owningControl != null) {
					ISite site = owningControl.Site;
					if(site != null && !IsDummySite(site))
						return;
				}
			}
			if(HasControls())
				base.RenderChildren(writer);
			else
				RenderContentsWithoutControls(writer);
		}
		static bool IsDummySite(ISite site) {
			Type siteType = site.GetType();
			return siteType.Name.IndexOf("DummySite") >= 0;
		}
		private ASPxWebControl FindOwningControl() { 
			for(Control control = NamingContainer; control != null; control = control.NamingContainer) {
				ASPxWebControl owningControl = control as ASPxWebControl;
				if(owningControl != null)
					return owningControl;
			}
			return null;
		}
		string IAttributeAccessor.GetAttribute(string key) {
			return string.Empty;
		}
		void IAttributeAccessor.SetAttribute(string key, string value) {
		}
	}
	public class CollectionItemControlCollection : ControlCollection {
		private ContentControlCollectionItem collectionItem = null;
		public CollectionItemControlCollection(Control owner, ContentControlCollectionItem ownerCollectionItem)
			: base(owner) {
			this.collectionItem = ownerCollectionItem;
		}
		public override void Add(Control child) {
			this.collectionItem.ContentControl.Controls.Add(child);
		}
		public override void AddAt(int index, Control child) {
			this.collectionItem.ContentControl.Controls.AddAt(index, child);
		}
		public override void Clear() {
			this.collectionItem.ContentControl.Controls.Clear();
		}
		public override void CopyTo(Array array, int index) {
			this.collectionItem.ContentControl.Controls.CopyTo(array, index);
		}
		public override bool Contains(Control child) {
			return this.collectionItem.ContentControl.Controls.Contains(child);
		}
		public override int IndexOf(Control child) {
			return this.collectionItem.ContentControl.Controls.IndexOf(child);
		}
		public override void Remove(Control child) {
			this.collectionItem.ContentControl.Controls.Remove(child);
		}
		public override void RemoveAt(int index) {
			this.collectionItem.ContentControl.Controls.RemoveAt(index);
		}
	}
	[ToolboxItem(false)]
	public class CollectionItemControl : Control {
		private ContentControlCollectionItem collectionItem = null;
		public CollectionItemControl(ContentControlCollectionItem ownerCollectionItem)
			: base() {
			this.collectionItem = ownerCollectionItem;
		}
		protected override ControlCollection CreateControlCollection() {
			return new CollectionItemControlCollection(this, this.collectionItem);
		}
	}
}
