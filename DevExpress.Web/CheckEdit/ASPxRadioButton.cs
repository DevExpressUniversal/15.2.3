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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class RadioButtonProperties : CheckBoxProperties {
		public RadioButtonProperties()
			: base() {
		}
		public RadioButtonProperties(IPropertiesOwner owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("RadioButtonPropertiesDisplayImageChecked")]
#endif
		public override InternalCheckBoxImageProperties DisplayImageChecked {
			get { return Images.RadioButtonChecked; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("RadioButtonPropertiesDisplayImageUnchecked")]
#endif
		public override InternalCheckBoxImageProperties DisplayImageUnchecked {
			get { return Images.RadioButtonUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonPropertiesDisplayImageUndefined"),
#endif
		Obsolete("This method is now obsolete. Use the DisplayImageGrayed property instead.")]
		public override InternalCheckBoxImageProperties DisplayImageUndefined {
			get { return DisplayImageGrayed; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("RadioButtonPropertiesDisplayImageGrayed")]
#endif
		public override InternalCheckBoxImageProperties DisplayImageGrayed {
			get { return Images.RadioButtonUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonPropertiesRadioButtonFocusedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle RadioButtonFocusedStyle { get { return Styles.RadioButtonFocused; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonPropertiesRadioButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle RadioButtonStyle { get { return Styles.RadioButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle CheckBoxFocusedStyle {
			get { return base.CheckBoxFocusedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle CheckBoxStyle {
			get { return base.CheckBoxStyle; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxRadioButton();
		}
		protected internal override AppearanceStyleBase GetCheckBoxStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultIRBClass());
			style.CopyFrom(RadioButtonStyle);
			return style;
		}
		internal override InternalCheckBoxImageProperties GetImage(CheckState checkState, Page page) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch (checkState) {
				case CheckState.Checked:
					imageName = EditorImages.RadioButtonCheckedImageName;
					result.MergeWith(Images.RadioButtonChecked);
					break;
				default:
					imageName = EditorImages.RadioButtonUncheckedImageName;
					result.MergeWith(Images.RadioButtonUnchecked);
					break;
			}
			result.MergeWith(Images.GetImageProperties(page, imageName));
			return result;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxRadioButton.bmp")
	]
	public class ASPxRadioButton : ASPxCheckBox {
		protected const string ReadonlyClickHandlerName = "ASPx.ERBOnReadonlyClick('{0}')";
		public ASPxRadioButton()
			: base() {
		}
		protected internal new RadioButtonProperties Properties {
			get { return base.Properties as RadioButtonProperties; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Type ValueType {
			get { return typeof(object); } set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ValueChecked {
			get { return true; } set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ValueUnchecked {
			get { return false; } set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ValueGrayed {
			get { return null; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonCheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public override InternalCheckBoxImageProperties CheckedImage {
			get { return Properties.Images.RadioButtonChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonUncheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public override InternalCheckBoxImageProperties UncheckedImage {
			get { return Properties.Images.RadioButtonUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonGroupName"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string GroupName {
			get { return GetStringProperty("GroupName", ""); }
			set { SetStringProperty("GroupName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonRadioButtonFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle RadioButtonFocusedStyle {
			get { return Properties.RadioButtonFocusedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonRadioButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle RadioButtonStyle {
			get { return Properties.RadioButtonStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle CheckBoxFocusedStyle {
			get { return base.CheckBoxFocusedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle CheckBoxStyle {
			get { return base.CheckBoxStyle; }
		}
		protected override CheckBoxControlBase CreateCheckEditControl() {
			return Native ? (CheckBoxControlBase)new RadioButtonNativeControl(this) : CreateRadioButtonControl();
		}
		protected virtual RadioButtonControl CreateRadioButtonControl() {
			return new RadioButtonControl(this);
		}
		protected override EditPropertiesBase CreateProperties() {
			return new RadioButtonProperties(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override CheckState CheckState {
			get { return base.CheckState; }
			set { base.CheckState = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowGrayed {
			get { return false; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowGrayedByClick {
			get { return true; }
			set { }
		} 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override InternalCheckBoxImageProperties GrayedImage {
			get { return base.GrayedImage; }
		}
		protected override AppearanceStyleBase GetICBFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultIRBFocusedClass());
			style.CopyFrom(RadioButtonFocusedStyle);
			return style;
		}
		protected override string GetDefaultICBFocusedCssClass() {
			return RenderStyles.GetDefaultInternalRadioButtonStyle().CssClass;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRadioButton";
		}
		protected internal override string GetInputType() {
			return "radio";
		}
		protected internal override string GetInputName() {
			if(String.IsNullOrEmpty(GroupName)) {
				return "";
			} else {
				string[] path = UniqueID.Split(IdSeparator);
				path[path.Length - 1] = GroupName;
				return String.Join(IdSeparator.ToString(), path);
			}
		}
		protected override bool IsWebSourcesRegisterRequired() {
			return false;
		}
		protected internal override string GetInputValue() {
			EnsureID();
			return ID;
		}
		protected override string GetOnClickReadonly() {
			return string.Format(ReadonlyClickHandlerName, ClientID);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!Native)
				stb.Append(string.Format("{0}.groupName = '{1}';\n", localVarName, GetInputName()));
		}
	}
}
