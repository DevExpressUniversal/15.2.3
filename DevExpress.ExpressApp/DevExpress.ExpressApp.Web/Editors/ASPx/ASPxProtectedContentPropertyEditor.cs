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
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxProtectedContentPropertyEditor : WebPropertyEditor, IProtectedContentEditor, ITestable {
		private string protectedContentText;
		protected override WebControl CreateEditModeControlCore() {
			ASPxTextBox result = RenderHelper.CreateASPxTextBox();
			result.Text = protectedContentText;
			result.ReadOnly = true;
			return result;
		}
		protected override void ApplyReadOnly() {
			ASPxTextBox textBox = Control as ASPxTextBox;
			if(textBox != null) {
				textBox.ReadOnly = true;
				textBox.ClientEnabled = false;
			}
		}
		protected override void ReadEditModeValueCore() { }
		protected override void WriteValueCore() { }
		protected override object GetControlValueCore() {
			return null;
		}
		protected override string GetPropertyDisplayValue() {
			return protectedContentText;
		}
		public ASPxProtectedContentPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
			skipEditModeDataBind = true;
		}
		public string ProtectedContentText {
			get { return protectedContentText; }
			set { protectedContentText = value; }
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxTextBoxTestControl();
		}
	}
}
