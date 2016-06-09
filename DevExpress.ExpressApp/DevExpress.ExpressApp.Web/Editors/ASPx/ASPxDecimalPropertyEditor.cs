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
using DevExpress.Web;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxDecimalPropertyEditor : ASPxIntPropertyEditor, ITestable {
		protected override void SetupControl(WebControl control) {
			base.SetupControl(control);
			if (control is ASPxSpinEdit) {
				ASPxSpinEdit editor = (ASPxSpinEdit)control;
				editor.NumberType = SpinEditNumberType.Float;
			}
		}
		protected override void CreateRestrictions(WebControl spinEdit) {
		}
		protected override void ReadEditModeValueCore() {
			if(ASPxEditor is ASPxSpinEdit) {
				ASPxSpinEdit editor = (ASPxSpinEdit)ASPxEditor;
				editor.Value = PropertyValue;
				Decimal dummy;
				string displayValue = GetPropertyDisplayValue();
				if(Decimal.TryParse(displayValue, out dummy)) {
					editor.Text = displayValue;
				}
				else if(PropertyValue != null) {
					editor.Text = PropertyValue.ToString();
				}
			}
		}
		public ASPxDecimalPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
	}
}
