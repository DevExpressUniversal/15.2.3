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
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web.ASPxHtmlEditor;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Web {
	public class ASPxHtmlPropertyEditor : WebPropertyEditor, ITestable, ISupportSizeConstraints {
		private int editorHeight;
		private void ASPxHtmlEditor_HtmlChanged(object sender, EventArgs e) {
			EditValueChangedHandler(this, EventArgs.Empty);
		}
		private void UpdateEditorHeight() {
			if(editorHeight > 0 && ViewEditMode == Editors.ViewEditMode.Edit && Editor != null) {
				Editor.Height = Unit.Pixel(editorHeight);
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			ASPxHtmlEditor result = new ASPxHtmlEditor();
			result.EnableViewState = false;
			string[] allowedFileExtensions = new string[] { ".jpeg", ".gif", ".png", ".bmp" };
			result.SettingsDialogs.InsertImageDialog.SettingsImageUpload.ValidationSettings.AllowedFileExtensions = allowedFileExtensions;
			result.SettingsDialogs.InsertImageDialog.SettingsImageUpload.UploadFolder = string.Empty;
			result.SettingsDialogs.InsertImageDialog.SettingsImageSelector.Enabled = true;
			result.SettingsDialogs.InsertImageDialog.SettingsImageSelector.CommonSettings.AllowedFileExtensions = allowedFileExtensions;
			result.SettingsDialogs.InsertImageDialog.ShowFileUploadSection = false;
			result.CreateDefaultToolbars(true);
			RemoveUnnecessaryToolbarItems(result);
			result.HtmlChanged += new EventHandler<EventArgs>(ASPxHtmlEditor_HtmlChanged);
			return result;
		}
		protected virtual void RemoveUnnecessaryToolbarItems(ASPxHtmlEditor editor) {
			for(int i = 0; i < editor.Toolbars.Count; i++) {
				HtmlEditorToolbarItemCollection items = editor.Toolbars[i].Items;
				for(int j = 0; j < items.Count; j++) {
					if(items[j].CommandName == "checkspelling") {
						items.RemoveAt(j);
						return;
					}
				}
			}
		}
		protected override void OnControlCreated() {
			UpdateEditorHeight();
			base.OnControlCreated();
		}
		protected override void SetEditorId(string controlId) {
			string viewHashCode = "";
			if(View != null) {
				viewHashCode = View.GetHashCode().ToString();
			}
			Editor.ID = controlId + viewHashCode;
		}
		protected override void ApplyReadOnly() {
			if(Editor != null) {
				Editor.Settings.AllowHtmlView = AllowEdit;
				Editor.Settings.AllowDesignView = AllowEdit;
			}
		}
		protected override object GetControlValueCore() {
			return Editor.Html;
		}
		protected override void ReadEditModeValueCore() {
			Editor.Html = (string)PropertyValue;
		}
		protected override WebControl CreateViewModeControlCore() {
			WebControl viewModeControl = base.CreateViewModeControlCore();
			viewModeControl.CssClass += " xafHtmlContent";
			return viewModeControl;
		}
		protected override void ReadViewModeValueCore() {
			((Label)InplaceViewModeEditor).Text = GetPropertyDisplayValue();
		}
		protected override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxHtmlPropertyEditorTestControl();
		}
		public ASPxHtmlPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				Editor.HtmlChanged -= new EventHandler<EventArgs>(ASPxHtmlEditor_HtmlChanged);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public new ASPxHtmlEditor Editor {
			get { return ((ASPxHtmlEditor)base.Editor); }
		}
		void ISupportSizeConstraints.ApplyConstraints(Size minSize, Size maxSize) {
			editorHeight = Math.Max(minSize.Height, maxSize.Height);
			UpdateEditorHeight();
		}
	}
}
