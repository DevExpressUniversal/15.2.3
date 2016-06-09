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
using DevExpress.Web;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using System.Web.UI.HtmlControls;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.Utils.Controls;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxBooleanPropertyEditor : ASPxPropertyEditor, ITestable, ICustomValueParser, ICustomBehaviorInListView, ISupportExportCustomValue {
		private IJScriptTestControl GetTestControl(bool viewMode) {
			if(IsDefinedBoolCaption) {
				if(viewMode) {
					return new JSLabelTestControl();
				}
				else {
					return new JSASPxComboBoxTestControl();
				}
			}
			else {
				return new JSASPxCheckBoxTestControl();
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			if(IsDefinedBoolCaption || IsDefinedBoolImages) {
				string trueText = IsDefinedBoolCaption ? Model.CaptionForTrue : true.ToString();
				string falseText = IsDefinedBoolCaption ? Model.CaptionForFalse : false.ToString();
				ASPxComboBox result = RenderHelper.CreateASPxComboBox();
				result.ShowImageInEditBox = true;
				result.ID = "Cb";
				result.EnableViewState = false;
				if(IsDefinedBoolImages) {
					result.Items.Add(falseText, false, ImageLoader.Instance.GetImageInfo(Model.ImageForFalse).ImageUrl);
					result.Items.Add(trueText, true, ImageLoader.Instance.GetImageInfo(Model.ImageForTrue).ImageUrl);
				}
				else {
					result.Items.Add(falseText);
					result.Items.Add(trueText);
				}
				result.SelectedIndexChanged += new EventHandler(EditValueChangedHandler);
				return result;
			}
			else {
				ASPxCheckBox result = RenderHelper.CreateASPxCheckBox();
				result.ID = "Ch";
				result.EnableViewState = false;
				result.CssClass = "Caption";
				result.CheckedChanged += new EventHandler(EditValueChangedHandler);
				result.Text = ((IModelMemberViewItem)Model).Caption;
				return result;
			}
		}
		protected override WebControl CreateViewModeControlCore() {
			WebControl result;
			if(IsDefinedBoolImages || IsDefinedBoolCaption) {
				ASPxImageLabelControl imageLabeControl = new ASPxImageLabelControl();
				imageLabeControl.Label.EnableClientSideAPI = false;
				result = imageLabeControl;
			}
			else {
				ASPxCheckBox checkBox = RenderHelper.CreateASPxCheckBox();
				checkBox.EnableClientSideAPI = true;
				checkBox.ClientEnabled = false;
				checkBox.Text = ((IModelMemberViewItem)Model).Caption;
				result = checkBox;
			}
			return result;
		}
		protected override object GetControlValueCore() {
			bool isComboBoxUsed = (IsDefinedBoolCaption || IsDefinedBoolImages);
			return isComboBoxUsed ? (((ASPxComboBox)Editor).SelectedIndex == 1) : (object)((ASPxCheckBox)Editor).Checked;
		}
		protected string GetDisplayValue(object value) {
			if(IsDefinedBoolCaption) {
				if(value != null) {
					return (bool)value ? Model.CaptionForTrue : Model.CaptionForFalse;
				}
			}
			return null;
		}
		protected override string GetPropertyDisplayValue() {
			return GetDisplayValue(PropertyValue) ?? base.GetPropertyDisplayValue();
		}
		protected override void ReadEditModeValueCore() {
			base.ReadEditModeValueCore();
			if(PropertyValue != null) {
				bool value = (Boolean)PropertyValue;
				if(IsDefinedBoolImages || IsDefinedBoolCaption) {
					((ASPxComboBox)Editor).SelectedIndex = value ? 1 : 0; 
				}
				else {
					((ASPxCheckBox)Editor).Checked = value;
				}
			}
		}
		protected override void ReadViewModeValueCore() {
			bool propertyValue = PropertyValue == null ? false : (bool)PropertyValue;
			string resultText = "";
			if(InplaceViewModeEditor is ASPxImageLabelControl) {
				ASPxImageLabelControl control = (ASPxImageLabelControl)InplaceViewModeEditor;
				control.ShowImage = IsDefinedBoolImages;
				if(IsDefinedBoolImages) {
					ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(propertyValue ? Model.ImageForTrue : Model.ImageForFalse);
					ASPxImageHelper.SetImageProperties(control.Image, imageInfo);
				}
				if(IsDefinedBoolCaption) {
					resultText = propertyValue ? Model.CaptionForTrue : Model.CaptionForFalse;
				}
				control.Text = resultText;
			}
			else {
				ASPxCheckBox checkBox = (ASPxCheckBox)InplaceViewModeEditor;
				checkBox.Checked = propertyValue;
			}
		}
		protected override void SetControlAlignmentCore(HorizontalAlign alignment) {
			if(!IsDefinedBoolCaption && !IsDefinedBoolImages) {
				base.SetControlAlignmentCore(alignment);
			}
		}
		protected override string GetTestCaption() {
			return ((IModelMemberViewItem)Model).Caption;
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return GetTestControl(false);
		}
		protected internal override IJScriptTestControl GetInplaceViewModeEditorTestControlImpl() {
			return GetTestControl(true);
		}
		protected bool IsDefinedBoolImages {
			get { return Model != null && !string.IsNullOrEmpty(Model.ImageForFalse) && !string.IsNullOrEmpty(Model.ImageForTrue); }
		}
		protected bool IsDefinedBoolCaption {
			get { return Model != null && !string.IsNullOrEmpty(Model.CaptionForFalse) && !string.IsNullOrEmpty(Model.CaptionForTrue); }
		}
		public ASPxBooleanPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			ASPxComboBox dropDownList = Editor as ASPxComboBox;
			if(dropDownList != null) {
				dropDownList.SelectedIndexChanged -= new EventHandler(EditValueChangedHandler);
			}
			ASPxCheckBox checkBox = Editor as ASPxCheckBox;
			if(checkBox != null) {
				checkBox.CheckedChanged -= new EventHandler(EditValueChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public override string Caption {
			get {
				if(IsDefinedBoolCaption || IsDefinedBoolImages) {
					return ((IModelMemberViewItem)Model).Caption;
				}
				else {
					return " ";
				}
			}
		}
		#region ICustomValueParser Members
		public object TryParse(string text, IFilterablePropertyInfo propertyInfo) {
			if(propertyInfo == null || propertyInfo.PropertyType != typeof(bool)) {
				return null;
			}
			object result = null;
			if(propertyInfo is FilterControlPropertyEditorColumn) {
				ASPxPropertyEditor editor = ((FilterControlPropertyEditorColumn)(propertyInfo)).PropertiesASPxPropertyEditor.ASPxPropertyEditor;
				if(editor is ASPxBooleanPropertyEditor) {
					ASPxBooleanPropertyEditor booleanPropertyEditor = ((ASPxBooleanPropertyEditor)editor);
					if(booleanPropertyEditor.IsDefinedBoolCaption) {
						if(text == booleanPropertyEditor.Model.CaptionForFalse) {
							result = false;
						}
						if(text == booleanPropertyEditor.Model.CaptionForTrue) {
							result = true;
						}
					}
					else {
						bool boolResult;
						if(bool.TryParse(text, out boolResult)) {
							result = boolResult;
						}
					}
				}
			}
			return result;
		}
		public string GetDisplayText(object value, IFilterablePropertyInfo propertyInfo) {
			string result = string.Empty;
			if(propertyInfo == null || propertyInfo.PropertyType != typeof(bool) || value == null) {
				return null;
			}
			if(propertyInfo is FilterControlPropertyEditorColumn) {
				ASPxPropertyEditor editor = ((FilterControlPropertyEditorColumn)(propertyInfo)).PropertiesASPxPropertyEditor.ASPxPropertyEditor;
				if(editor is ASPxBooleanPropertyEditor) {
					ASPxBooleanPropertyEditor booleanPropertyEditor = ((ASPxBooleanPropertyEditor)editor);
					result = booleanPropertyEditor.GetDisplayValue(value) ?? ReflectionHelper.GetObjectDisplayText(value);
				}
			}
			return result;
		}
		#endregion
		#region ICustomBehaviorInListView Members
		public void CustomizeForListView() {
			if(ViewEditMode == ViewEditMode.Edit) {
				ResetCheckBoxText(Editor as ASPxCheckBox);
			}
			else {
				ResetCheckBoxText(InplaceViewModeEditor as ASPxCheckBox);
			}
		}
		private void ResetCheckBoxText(ASPxCheckBox checkBox) {
			if(checkBox != null) {
				checkBox.Text = string.Empty;
				checkBox.Width = Unit.Empty;
			}
		}
		#endregion
		#region ISupportExportCustomValue Members
		string ISupportExportCustomValue.GetExportedValue() {
			if(IsDefinedBoolCaption || IsDefinedBoolImages) {
				return GetPropertyDisplayValue();
			}
			else {
				return null;
			}
		}
		#endregion
	}
}
