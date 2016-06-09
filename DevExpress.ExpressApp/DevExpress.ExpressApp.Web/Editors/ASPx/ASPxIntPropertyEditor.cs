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
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxIntPropertyEditor : ASPxPropertyEditor, ITestable {
		protected override void SetupControl(WebControl control) {
			base.SetupControl(control);
			if(control is ASPxSpinEdit) {
				ASPxSpinEdit editor = (ASPxSpinEdit)control;
				editor.AllowNull = AllowNull;
				editor.NumberType = SpinEditNumberType.Integer;
				editor.NumberChanged += new EventHandler(EditValueChangedHandler);
			}
			CreateRestrictions(control);
		}
		protected override WebControl CreateEditModeControlCore() {
			return RenderHelper.CreateASPxSpinEdit();
		}
		protected virtual void CreateRestrictions(WebControl editor) {
			ASPxSpinEdit spinEdit = editor as ASPxSpinEdit;
			if(spinEdit != null) {
				Type memberType = GetUnderlyingType();
				spinEdit.MaxValue = (Decimal)Convert.ChangeType(memberType.GetField("MaxValue").GetValue(0), typeof(Decimal));
				spinEdit.MinValue = (Decimal)Convert.ChangeType(memberType.GetField("MinValue").GetValue(0), typeof(Decimal));
			}
		}
		protected override object GetControlValueCore() {
			ASPxSpinEdit spinEditor = (ASPxSpinEdit)Editor;
			if((spinEditor == null) || (spinEditor.Value == null)) {
				return null;
			}
			return ReflectionHelper.Convert(spinEditor.Value, MemberInfo.MemberType);
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxSpinTestControl();
		}
		protected override string GetPropertyDisplayValue() {
			return GetFormattedValue();
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				((ASPxSpinEdit)Editor).NumberChanged -= new EventHandler(EditValueChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public ASPxIntPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public new ASPxSpinEdit Editor {
			get { return (ASPxSpinEdit)base.Editor; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
	}
	public class ASPxInt64PropertyEditor : ASPxIntPropertyEditor {
		public ASPxInt64PropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) { }
	}
	public class ASPxBytePropertyEditor : ASPxIntPropertyEditor {
		public ASPxBytePropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) { }
	}
}
