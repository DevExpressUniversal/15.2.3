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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class MemoProperties : TextEditProperties {
		public MemoProperties()
			: base() {
		}
		public MemoProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MemoPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatDisable, MergableProperty(false), NotifyParentProperty(true)]
		public new TextEditClientSideEvents ClientSideEvents {
			get { return (TextEditClientSideEvents)base.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MemoPropertiesColumns"),
#endif
		Category("Layout"), DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int Columns {
			get { return GetIntProperty("Columns", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "Columns");
				SetIntProperty("Columns", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MemoPropertiesMaxLength"),
#endif
		Category("Layout"), DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int MaxLength {
			get { return GetIntProperty("MaxLength", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaxLength");
				SetIntProperty("MaxLength", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MemoPropertiesRows"),
#endif
		Category("Layout"), DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int Rows {
			get { return GetIntProperty("Rows", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "Rows");
				SetIntProperty("Rows", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MemoPropertiesNative"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MemoPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MemoProperties memoSource = source as MemoProperties;
				if(memoSource != null) {
					Columns = memoSource.Columns;
					Rows = memoSource.Rows;
					MaxLength = memoSource.MaxLength;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxMemo();
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			LiteralControl literal = (LiteralControl)base.CreateDisplayControlInstance(args);
			if(EncodeHtml)
				literal.Text = HtmlConvertor.ToMultilineHtml(literal.Text);
			return literal;
		}
		protected override bool IsNativeSupported() {
			return true;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ToolboxData("<{0}:ASPxMemo runat=\"server\" Width=\"170px\" Height=\"71px\"></{0}:ASPxMemo>"),
	Designer("DevExpress.Web.Design.ASPxMemoDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull), 
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxMemo.bmp")]
	public class ASPxMemo : ASPxTextEdit {
		protected internal const string MouseOverHandlerName = "ASPx.MMMouseOver('{0}')";
		protected internal const string MouseOutHandlerName = "ASPx.MMMouseOut('{0}')";
		public ASPxMemo()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new TextEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoColumns"),
#endif
		Category("Layout"), DefaultValue(0), AutoFormatDisable]
		public int Columns {
			get { return Properties.Columns; }
			set {
				if(value > 0) {
					Width = Unit.Empty;
					PropertyChanged("Width");
					LayoutChanged();
				}
				Properties.Columns = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoMaxLength"),
#endif
		Category("Layout"), DefaultValue(0), AutoFormatDisable]
		public int MaxLength {
			get { return Properties.MaxLength; }
			set { Properties.MaxLength = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxMemoHeight")]
#endif
		public override Unit Height {
			get { return base.Height; }
			set {
				if(!value.IsEmpty) {
					Rows = 0;
					PropertyChanged("Rows");
				}
				base.Height = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return (ControlStyle as AppearanceStyle).HorizontalAlign; }
			set { (ControlStyle as AppearanceStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoNative"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoRows"),
#endif
		Category("Layout"), DefaultValue(0), AutoFormatDisable]
		public int Rows {
			get { return Properties.Rows; }
			set {
				if(value > 0) {
					Height = Unit.Empty;
					PropertyChanged("Height");
				}
				Properties.Rows = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMemoText"),
#endif
		Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), AutoFormatDisable]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxMemoWidth")]
#endif
		public override Unit Width {
			get { return base.Width; }
			set {
				if(!value.IsEmpty) {
					Columns = 0;
					PropertyChanged("Columns");
					LayoutChanged();
				}
				base.Width = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new MemoProperties Properties {
			get { return (MemoProperties)base.Properties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MemoProperties(this);
		}
		protected internal bool HasMaxLength() {
			return MaxLength > 0;
		}
		protected internal bool NativeMaxLengthSupported() {
			if(Browser.Platform.IsAndroidMobile) return true;
			if(Browser.Platform.IsMacOSMobile) return true;
			if(Browser.IsIE && Browser.Version >= 10) return true;
			if(Browser.IsSafari && Browser.Version >= 5) return true;
			if(Browser.IsChrome && Browser.Version >= 8) return true;
			if(Browser.IsFirefox && Browser.Version >= 4) return true;
			return false;
		}
		protected internal bool MaxLengthScriptNativeImplementation() {
			return HasMaxLength() && NativeMaxLengthSupported() && this.IsEnabled();
		}
		protected internal bool MaxLengthScriptImplementation() {
			return HasMaxLength() && !NativeMaxLengthSupported() && this.IsEnabled();
		}
		protected string CutString(string value) {
			if(HasMaxLength() && value.Length > MaxLength)
				value = value.Remove(MaxLength);
			return value;
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			AppearanceStyle defaultEditStyle = new AppearanceStyle();
			defaultEditStyle.CssClass = EditorStyles.MemoSystemClassName;
			defaultEditStyle.CopyFrom(RenderStyles.GetDefaultMemoStyle());
			return defaultEditStyle;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.Memo);
			return style;
		}
		protected internal AppearanceStyle GetMemoEditAreaStyle() {
			return GetEditAreaStyleInternal(RenderStyles.GetDefaultMemoEditAreaStyle());
		}
		protected override void PrepareEditAreaStyle(EditAreaStyle style) {
			base.PrepareEditAreaStyle(style);
			AppearanceStyleBase controlStyle = GetControlStyle();
			if(controlStyle.HorizontalAlign != HorizontalAlign.NotSet &&
				style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = controlStyle.HorizontalAlign;
		}
		protected virtual MemoControl CreateMemoControl() {
			return new MemoControl(this);
		}
		protected virtual MemoNativeControl CreateMemoNativeControl() {
			return new MemoNativeControl(this);
		}
		internal sealed override bool IsInputStretched {
			get { return Columns == 0; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(IsNativeRender())
				Controls.Add(CreateMemoNativeControl());
			else
				Controls.Add(CreateMemoControl());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (HasMaxLength()) Text = CutString(Text).ToString();
		}
		protected override object GetPostBackValue(string controlName, NameValueCollection postCollection) {
			string value = base.GetPostBackValue(controlName, postCollection) as string;
			return CutString(value);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(MaxLengthScriptImplementation())
				stb.AppendFormat("{0}.maxLength = {1};\n", localVarName, MaxLength);
		}
		protected override bool HasFocusEvents() {
			return base.HasFocusEvents() || MaxLengthScriptImplementation();
		}
		protected internal virtual string GetOnMouseOut() {
			return MaxLengthScriptImplementation() ? string.Format(MouseOutHandlerName, ClientID) : "";
		}
		protected internal override string GetOnMouseOver() {
			return MaxLengthScriptImplementation() ? string.Format(MouseOverHandlerName, ClientID) : base.GetOnMouseOver();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientMemo";
		}
		protected override bool HeightCorrectionRequired { 
			get { return Height == Unit.Percentage(100) && (InplaceMode == EditorInplaceMode.StandAlone || InplaceMode == EditorInplaceMode.EditForm); } 
		}
		internal new bool IsRightToLeft() {
			return base.IsRightToLeft();
		}
	}
}
