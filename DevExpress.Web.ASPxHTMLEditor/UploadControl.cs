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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Forms;
using DevExpress.Web.ASPxHtmlEditor.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	[ToolboxItem(false)]
	public sealed class ASPxHtmlEditorUploadControl : DevExpress.Web.ASPxUploadControl {
		public ASPxHtmlEditorUploadControl()
			: base() {
			this.OwnerControl = FindParentHtmlEditor();
		}
		protected override StylesBase CreateStyles() {
			return new HtmlEditorUploadControlStyles(this);
		}
		protected override void OnInit(System.EventArgs e) {
			ASPxHtmlEditor editor = FindParentHtmlEditor();
			if(editor != null)
				ValidationSettings.Assign(GetValidationSettings(editor));
			base.OnInit(e);
		}
		ASPxHtmlEditorUploadValidationSettingsBase GetValidationSettings(ASPxHtmlEditor htmlEditor) {
			var dialog = TemplateControl as HtmlEditorInsertMediaDialogBase;
			return dialog != null ? dialog.GetUploadSettings().ValidationSettingsInternal : null;
		}
		private ASPxHtmlEditor FindParentHtmlEditor() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is ASPxHtmlEditor)
					return curControl as ASPxHtmlEditor;
				curControl = curControl.Parent;
			}
			return null;
		}
	}
}
