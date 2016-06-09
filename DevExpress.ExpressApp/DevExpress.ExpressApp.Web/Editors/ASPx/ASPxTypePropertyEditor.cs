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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxTypePropertyEditor : ASPxPropertyEditor, IComplexViewItem, ITestable {
		private TypeConverter typeConverter;
		protected override void SetupControl(WebControl control) {
			base.SetupControl(control);
			if(control is ASPxComboBox) {
				ASPxComboBox editor = (ASPxComboBox)control;
				editor.SelectedIndexChanged += new EventHandler(EditValueChangedHandler);
				editor.Items.Clear();
				foreach(Type type in typeConverter.GetStandardValues()) {
					if(type != null && IsSuitableType(type)) {
						editor.Items.Add(typeConverter.ConvertToString(type), type.FullName);
					}						
				}
			}
		}
		protected virtual bool IsSuitableType(Type type) {
			return true;
		}
		protected override WebControl CreateEditModeControlCore() {
			return RenderHelper.CreateASPxComboBox();
		}
		protected override object GetControlValueCore() {
			if(Editor.SelectedItem != null) {
				return ReflectionHelper.GetType(Editor.SelectedItem.Value as string);
			}
			else {
				return null;
			}
		}
		protected override void ReadEditModeValueCore() {
			base.ReadEditModeValueCore();
			object value = PropertyValue;
			if((value != null) && (typeConverter != null)) {
				Editor.SelectedIndex = Editor.Items.IndexOfText(typeConverter.ConvertToString(value));
			}
		}
		protected override string GetPropertyDisplayValue() {
			if(typeConverter != null) {
				string result = typeConverter.ConvertToString(PropertyValue);
				return !string.IsNullOrEmpty(DisplayFormat) ? string.Format(DisplayFormat, result) : result; 
			}
			return base.GetPropertyDisplayValue();
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxComboBoxTestControl();
		}
		public ASPxTypePropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				Editor.SelectedIndexChanged -= new EventHandler(EditValueChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			TypeConverterAttribute typeConverterAttribute = MemberInfo.FindAttribute<TypeConverterAttribute>();
			if(typeConverterAttribute != null) {
				typeConverter =
					(TypeConverter)TypeHelper.CreateInstance(Type.GetType(typeConverterAttribute.ConverterTypeName));
			}
			else {
				typeConverter = new LocalizedClassInfoTypeConverter();
			}
		}
		public new ASPxComboBox Editor {
			get { return (ASPxComboBox)base.Editor; }
		}
	}
}
