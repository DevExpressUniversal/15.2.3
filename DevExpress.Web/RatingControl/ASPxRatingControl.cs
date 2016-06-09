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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum RatingControlItemFillPrecision {
		Exact = 0,
		Half = 1,
		Full = 2
	};
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxRatingControl"),
	DefaultProperty("ItemCount"), DefaultEvent("ItemClick"),
	Designer("DevExpress.Web.Design.ASPxRatingControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxRatingControl.bmp")
	]
	public class ASPxRatingControl : ASPxWebControl, IRequiresLoadPostDataControl {
		internal const string ResourceScriptPath = WebScriptsResourcePath + "RatingControl.js";
		internal const string ImageMapResourceName = "rcMapImage";
		const int
			DefaultItemWidth = 19,
			DefaultItemHeight = 19,
			DefaultStripeIndex = 0,
			UserStripeIndex = 1,
			CheckedStripeIndex = 2,
			HoverStripeIndex = 3;
		static readonly object EventItemClick = new object();
		public ASPxRatingControl() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlItemCount"),
#endif
		DefaultValue(5), AutoFormatDisable, RefreshProperties(RefreshProperties.Repaint)]
		public int ItemCount {
			get { return GetIntProperty("ItemCount", 5); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "ItemCount");
				SetIntProperty("ItemCount", 5, value);
				if(DesignMode && !IsLoading()) {
					if(value < Value) {
						Value = value;
						PropertyChanged("Value");
					}
				}
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlValue"),
#endif
		DefaultValue(typeof(Decimal), "5"), Bindable(true), AutoFormatDisable]
		public decimal Value {
			get { return GetDecimalProperty("Value", 5M); }
			set {
				if(DesignMode && !IsLoading())
					CommonUtils.CheckValueRange(value, 0, ItemCount, "Value");
				SetDecimalProperty("Value", 5M, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlReadOnly"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ReadOnly {
			get { return GetBoolProperty("ReadOnly", false); }
			set { SetBoolProperty("ReadOnly", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlItemWidth"),
#endif
		DefaultValue(DefaultItemWidth), AutoFormatEnable]
		public int ItemWidth {
			get { return GetIntProperty("ItemWidth", DefaultItemWidth); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "ItemWidth");
				SetIntProperty("ItemWidth", DefaultItemWidth, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlItemHeight"),
#endif
		DefaultValue(DefaultItemHeight), AutoFormatEnable]
		public int ItemHeight {
			get { return GetIntProperty("ItemHeight", DefaultItemHeight); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "ItemHeight");
				SetIntProperty("ItemHeight", DefaultItemHeight, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlFillPrecision"),
#endif
		DefaultValue(RatingControlItemFillPrecision.Half), AutoFormatDisable]
		public RatingControlItemFillPrecision FillPrecision {
			get { return (RatingControlItemFillPrecision)GetEnumProperty("FillPrecision", RatingControlItemFillPrecision.Half); }
			set { SetEnumProperty("FillPrecision", RatingControlItemFillPrecision.Half, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlTitles"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string Titles {
			get { return GetStringProperty("Titles", ""); }
			set { SetStringProperty("Titles", "", value.Trim()); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlToolTip"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(true), AutoFormatDisable]
		public override string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value.Trim()); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlImageMapUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatUrlProperty, AutoFormatEnable]
		public string ImageMapUrl {
			get { return GetStringProperty("ImageMapUrl", ""); }
			set { SetStringProperty("ImageMapUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public RatingControlClientSideEvents ClientSideEvents {
			get { return (RatingControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width { get { return base.Width; } set { base.Width = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Color BorderColor { get { return base.BorderColor; } set { base.BorderColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyle BorderStyle { get { return base.BorderStyle; } set { base.BorderStyle = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit BorderWidth { get { return base.BorderWidth; } set { base.BorderWidth = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		#endregion
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRatingControlItemClick"),
#endif
		Category("Action")]
		public event RatingControlItemEventHandler ItemClick
		{
			add { Events.AddHandler(EventItemClick, value); }
			remove { Events.RemoveHandler(EventItemClick, value); }
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRatingControl";
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RatingControlClientSideEvents();
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasItemServerClickEventHandler())
				eventNames.Add("ItemClick");
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasHoverScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxRatingControl), ResourceScriptPath);
			RegisterIncludeScript(typeof(ASPxEditBase), ASPxEditBase.EditScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.SetDimensions({1},{2},{3});\n", localVarName, HtmlConvertor.ToJSON(ItemCount), HtmlConvertor.ToJSON(ItemWidth), HtmlConvertor.ToJSON(ItemHeight));
			stb.AppendFormat("{0}.titles = {1};\n", localVarName, HtmlConvertor.ToJSON(CreateTitleList()));
			stb.AppendFormat("{0}.toolTip = {1};\n", localVarName, HtmlConvertor.ToJSON(GetToolTip()));
			stb.AppendFormat("{0}.fillPrecision = {1};\n", localVarName, HtmlConvertor.ToJSON((int)FillPrecision));
			stb.AppendFormat("{0}.value = {1};\n", localVarName, HtmlConvertor.ToJSON(Value));
			if(ReadOnly)
				stb.AppendFormat("{0}.readOnly = true;\n", localVarName);
		}
		string GetToolTip() {
			return string.IsNullOrEmpty(ToolTip) ? null : ToolTip;
		}
		internal IList<string> CreateTitleList() {
			if(string.IsNullOrEmpty(Titles))
				return null;
			return Titles.Split(',').Select(i => i.Trim()).ToList();
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasItemServerClickEventHandler();
		}
		protected bool HasItemServerClickEventHandler() {
			return Events[EventItemClick] != null;
		}
		protected override bool HasRenderCssFile() {
			return false;
		}
		protected override void EnsurePreRender() {
			if(!DesignMode)
				CommonUtils.CheckValueRange(Value, 0, ItemCount, "Value");
			base.EnsurePreRender();
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(CreateDiv());
			Controls[0].Controls.Add(CreateDiv());
		}
		WebControl CreateDiv() {
			return RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.SetVisibility(this, IsClientVisible(), true);
			Style[HtmlTextWriterStyle.TextAlign] = IsRightToLeft() ? "right" : "left";
			Width = ItemCount * ItemWidth;
			Height = ItemHeight;
			SetDivImageSettings(this, DefaultStripeIndex);
			PrepareCheckedDiv();
			PrepareHoverDiv();
		}
		void PrepareCheckedDiv() {
			WebControl div = (WebControl)Controls[0];
			div.Width = (int)(ItemWidth * QuantizeValue());
			div.Height = ItemHeight;
			SetDivImageSettings(div, CheckedStripeIndex);
		}
		void PrepareHoverDiv() {
			WebControl div = (WebControl)Controls[0].Controls[0];
			div.Height = ItemHeight;
			div.Width = 0;
			SetDivImageSettings(div, HoverStripeIndex);
		}
		void SetDivImageSettings(WebControl div, int stripeIndex) {
			div.Style.Add("background",
				string.Format("{0} url({1}) repeat-x {2} {3}px", BackColor.IsEmpty ? "" : System.Drawing.ColorTranslator.ToHtml(BackColor),
				HttpUtility.UrlPathEncode(GetRatingMapImageUrl()), IsRightToLeft() ? "right" : "0", -stripeIndex * ItemHeight));
		}
		protected virtual decimal QuantizeValue() {
			return QuantizeValueDefault(Value, FillPrecision);
		}
		internal static decimal QuantizeValueDefault(decimal input, RatingControlItemFillPrecision precision) {
			switch(precision) {
				case RatingControlItemFillPrecision.Exact:
					return input;
				case RatingControlItemFillPrecision.Full:
					return Math.Round(input, MidpointRounding.AwayFromZero);
				case RatingControlItemFillPrecision.Half:
					return Math.Round(input * 2, MidpointRounding.AwayFromZero) / 2;
			}
			throw new NotSupportedException();
		}	   
		string GetRatingMapImageUrl() {
			if(string.IsNullOrEmpty(ImageMapUrl)) {
				string imagePostfix = DesignMode ? ".gif" : ".png";
				return ResourceManager.GetResourceUrl(Page, typeof(ASPxRatingControl), WebImagesResourcePath + ImageMapResourceName + imagePostfix);
			}
			return ResourceManager.ResolveClientUrl(this, ImageMapUrl);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			bool newReadOnly = GetClientObjectStateValue<bool>("readOnly");
			decimal newValue =  GetClientObjectStateValue<IConvertible>("value").ToDecimal(System.Globalization.CultureInfo.InvariantCulture);
			bool changed = Value != newValue || ReadOnly != newReadOnly;
			Value = newValue;
			ReadOnly = newReadOnly;
			return changed;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			RatingControlItemEventHandler handler = (RatingControlItemEventHandler)Events[EventItemClick];
			if(handler != null)
				handler(this, new RatingControlItemEventArgs(int.Parse(eventArgument)));
		}
	}
}
