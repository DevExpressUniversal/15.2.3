#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Localization;
using System.Web.UI.HtmlControls;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxEnumPropertyEditor : ASPxPropertyEditor, ISupportExportCustomValue, ITestable {
		protected EnumDescriptor descriptor;
		protected virtual Type GetComboBoxValueType() {
			return descriptor.EnumType;
		}
		protected virtual ImageInfo GetImageInfo(object enumValue) {
			return descriptor.GetImageInfo(enumValue);
		}
		protected override void SetupControl(WebControl control) {
			base.SetupControl(control);
			if(control is ASPxComboBox) {
				ASPxComboBox editor = (ASPxComboBox)control;
				editor.ShowImageInEditBox = true;
				editor.SelectedIndexChanged += new EventHandler(EditValueChangedHandler);
				editor.Items.Clear();
				editor.ValueType = GetComboBoxValueType();
				foreach(object enumValue in descriptor.Values) {
					object value = ConvertEnumValueForComboBox(enumValue);
					ImageInfo imageInfo = GetImageInfo(enumValue);
					if(imageInfo.IsUrlEmpty) {
						editor.Items.Add(descriptor.GetCaption(enumValue), value);
					}
					else {
						editor.Items.Add(descriptor.GetCaption(enumValue), value, imageInfo.ImageUrl);
					}
				}
			}
		}
		protected virtual object ConvertEnumValueForComboBox(object enumValue) {
			return enumValue;
		}
		protected override WebControl CreateEditModeControlCore() {
			return RenderHelper.CreateASPxComboBox();
		}
		protected override WebControl CreateViewModeControlCore() {
			ASPxImageLabelControl control = new ASPxImageLabelControl();
			control.CssClass = "dxgvEnumItem";
			return control;
		}
		protected override WebControl GetActiveControl() {
			if(ViewEditMode == ExpressApp.Editors.ViewEditMode.View) {
				return this.InplaceViewModeEditor;
			}
			return base.GetActiveControl();
		}
		protected override object GetControlValueCore() {
			Guard.ArgumentNotNull(Editor, "Editor");
			object controlValue = null;
			if(Editor.SelectedItem != null) {
				controlValue = Editor.SelectedItem.Value;
			}
			if(Editor.SelectedIndex == -1 && Editor.Value != null) {
				controlValue = Enum.Parse(GetEnumType(), Editor.Value.ToString());
			} 
			return controlValue;
		}
		protected override void ReadEditModeValueCore() {
			Object value = PropertyValue;
			VerifyValueBelongsToEnum(value);
			if(Editor.Items.IndexOfValue(value) >= 0) {
				Editor.Value = value;
				Editor.SelectedIndex = Editor.Items.IndexOfValue(value);
			}
		}
		protected override void ReadViewModeValueCore() {
			object value = PropertyValue;
			VerifyValueBelongsToEnum(value); 
			ImageInfo imageInfo = descriptor.GetImageInfo(value);
			ASPxImageLabelControl control = (ASPxImageLabelControl)InplaceViewModeEditor;
			ASPxImageHelper.SetImageProperties(control.Image, imageInfo);
			control.ShowImage = !imageInfo.IsEmpty;
			control.Text = GetPropertyDisplayValue();
		}
		protected virtual void VerifyValueBelongsToEnum(object value) { 
			if(value == null) return;
			Type valueType = value.GetType();
			if(valueType.IsEnum && !Enum.IsDefined(valueType, value)) { 
				string exceptionMessage = SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.InvalidValueOfEnumProperty,
					CurrentObject.ToString(), MemberInfo.Name, value.ToString(), GetEnumType().FullName, CurrentObject.GetType().FullName);
				throw new ArgumentOutOfRangeException(MemberInfo.Name, value, exceptionMessage);
			}
		}
		protected override string GetPropertyDisplayValue() {
			object propertyValue = PropertyValue;
			if(propertyValue != null) {
				string caption = descriptor.GetCaption(propertyValue);
				return string.IsNullOrEmpty(DisplayFormat) ? caption : string.Format(DisplayFormat, caption);
			}
			return CaptionHelper.NullValueText;
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxComboBoxTestControl();
		}
		protected virtual Type GetEnumType() {
			return MemberInfo.MemberType;
		}
		public ASPxEnumPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			descriptor = new EnumDescriptor(GetEnumType());
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				Editor.SelectedIndexChanged -= new EventHandler(EditValueChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public new ASPxComboBox Editor {
			get { return (ASPxComboBox)base.Editor; }
		}
		#region ISupportExportCustomValue Members
		string ISupportExportCustomValue.GetExportedValue() {
			return GetPropertyDisplayValue();
		}
		#endregion
	}
}
