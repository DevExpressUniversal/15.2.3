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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class LabelProperties: StaticEditProperties {
		public LabelProperties()
			: base() {
		}
		public LabelProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxLabel();
		}
		[AutoFormatEnable, DefaultValue(false), NotifyParentProperty(true)]
		public new bool AllowEllipsisInText {
			get { return base.AllowEllipsisInText; }
			set { base.AllowEllipsisInText = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ToolboxData("<{0}:ASPxLabel runat=\"server\" Text=\"ASPxLabel\"></{0}:ASPxLabel>"),
	DefaultProperty("Text"), ControlValueProperty("Text"), 
	DataBindingHandler(typeof(System.Web.UI.Design.TextDataBindingHandler)),
	Designer("DevExpress.Web.Design.ASPxLabelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxLabel.bmp")
	]
	public class ASPxLabel : ASPxStaticEdit, ITextControl {
		private LabelEditControl labelControl;
		public ASPxLabel()
			: base() {
		}
		protected ASPxLabel(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLabelText"),
#endif
		Localizable(true), Bindable(true), DefaultValue(""), AutoFormatDisable]
		public virtual string Text {
			get { return CommonUtils.ValueToString(Value); }
			set { Value = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLabelAssociatedControlID"),
#endif
		Category("Accessibility"), Themeable(false), TypeConverter(typeof(AssociatedControlConverter)),
		IDReferenceProperty, DefaultValue(""), Localizable(false), AutoFormatDisable]
		public virtual string AssociatedControlID {
			get { return GetStringProperty("AssociatedControlID", ""); }
			set { 
				SetStringProperty("AssociatedControlID", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLabelEncodeHtml"),
#endif
		Category("Behavior"), Browsable(true), AutoFormatDisable, EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLabelWrap"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean Wrap {
			get { return ((AppearanceStyle)ControlStyle).Wrap; }
			set { ((AppearanceStyle)ControlStyle).Wrap = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLabelRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[AutoFormatEnable, Category("Layout"), DefaultValue(false)]
		public bool AllowEllipsisInText {
			get { return Properties.AllowEllipsisInText; }
			set { Properties.AllowEllipsisInText = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
		protected LabelEditControl LabelControl {
			get { return this.labelControl; }
		}
		protected internal new LabelProperties Properties {
			get { return base.Properties as LabelProperties; }
		}
		protected internal bool IsAccessibilityAssociating {
			get {
				Control control = this.GetAssociatedControl();
				return control != null && (control is ASPxWebControl) && (control as ASPxWebControl).IsAccessibilityAssociatingSupported();
			}
		}
		protected override EditPropertiesBase CreateProperties() {
			return new LabelProperties(this);
		}
		protected override bool HasSpriteCssFile() {
			return false;
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(RenderStyles.GetDefaultLabelStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(RenderStyles.Label);
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			MergeDisableStyle(style);
		}
		protected override bool HasClientInitialization() {
			return base.HasClientInitialization() || AllowEllipsisInText || IsAccessibilityAssociating;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(AllowEllipsisInText)
				stb.AppendFormat("{0}.enableEllipsis=true;\n", localVarName);
			if(IsAccessibilityAssociating) {
				stb.AppendFormat("{0}.accessibilityAssociatedElementID = '{1}';\n", localVarName, LabelControl.AssociatedControlID);
				stb.AppendFormat("{0}.accessibilityAssociatedControlName = '{1}';\n", localVarName, AssociatedControlID);
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientLabel";
		}
		protected override void ClearControlFields() {
			this.labelControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.labelControl = new LabelEditControl(this);
			Controls.Add(LabelControl);
		}
		protected internal string GetText() {
			string text = EncodeHtml ? HtmlEncode(Text) : Text;
			if(EncodeHtml)
				text = HtmlConvertor.ToMultilineHtml(text);
			return text;
		}
		protected internal Control GetAssociatedControl() {
			if(!string.IsNullOrEmpty(AssociatedControlID) && (Parent != null))
				return (Parent.FindControl(AssociatedControlID) ?? FindControlHelper.LookupControl(this, AssociatedControlID));
			return null;
		}
		protected internal virtual string GetAssociatedControlClientID() {
			Control control = GetAssociatedControl();
			string ret = "";
			if(control == null) return ret;
			if(control is IAssociatedControlID) {
				ret = (control as IAssociatedControlID).ClientID();
			} else {
				ret = control.ClientID;
			} 
			return ret;
		}
	}
}
