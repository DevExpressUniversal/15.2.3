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
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxDateTimePropertyEditor : ASPxPropertyEditor, ITestable {
		private void SelectedDateChangedHandler(object source, EventArgs e) {
			EditValueChangedHandler(source, e);
		}
		protected override string GetPropertyDisplayValue() {
			if(DateTime.Equals(PropertyValue, DateTime.MinValue)) {
				return String.Empty;
			}
			else {
				return GetFormattedValue();
			}
		}
		protected override void SetupControl(WebControl control) {
			base.SetupControl(control);
			if(control is ASPxDateEdit) {
				ASPxDateEdit editor = (ASPxDateEdit)control;
				editor.CalendarProperties.DisplayFormatString = DisplayFormat;
				editor.EditFormat = EditFormat.Custom;
				editor.EditFormatString = EditMask;
				editor.CalendarProperties.DaySelectedStyle.CssClass = "ASPxSelectedItem";
				editor.DateChanged += new EventHandler(SelectedDateChangedHandler);
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			return RenderHelper.CreateASPxDateEdit();
		}
		public ASPxDateTimePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				Editor.DateChanged -= new EventHandler(SelectedDateChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public new ASPxDateEdit Editor {
			get { return (ASPxDateEdit)base.Editor; }
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxDateTestControl();
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
	}
}
