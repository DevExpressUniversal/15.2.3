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
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Win {
	public class CustomizeHtmlHeaderEventArgs : EventArgs {
		private string headerInnerHtml;
		public string HeaderInnerHtml {
			get { return headerInnerHtml; }
			set { headerInnerHtml = value; }
		}
	}
	public class HtmlPropertyEditor : WinPropertyEditor, IInplaceEditSupport {
		private string headerInnerHtml;
		private int rowCount;
		private void Editor_modified(object sender, EventArgs e) {
			OnControlValueChanged();
		}
		private void OnCustomizeHtmlHeader(CustomizeHtmlHeaderEventArgs args) {
			if(CustomizeHtmlHeader != null) {
				CustomizeHtmlHeader(this, args);
			}
		}
		private void SetupHtmlEditor(HtmlEditor editor) {
			editor.ReadOnly = !AllowEdit;
			if(RowCount > 0) {
				editor.Memo.MinimumSize = new Size(editor.Memo.MinimumSize.Width, editor.Memo.CalcMinHeight() * RowCount);
			}
		}
		protected override object CreateControlCore() {
			CustomizeHtmlHeaderEventArgs args = new CustomizeHtmlHeaderEventArgs();
			OnCustomizeHtmlHeader(args);
			headerInnerHtml = args.HeaderInnerHtml;
			HtmlEditor editor = new HtmlEditor(headerInnerHtml);
			SetupHtmlEditor(editor);
			editor.Modified += new EventHandler(Editor_modified);
			return editor;
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			if(Editor != null) {
				SetupHtmlEditor(Editor);
				Editor.RecreateBody(headerInnerHtml);
				Editor.Html = PropertyValue as string;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				CustomizeHtmlHeader = null;
				if(Editor != null) {
					Editor.Modified -= new EventHandler(Editor_modified);
				}
			}
			base.Dispose(disposing);
		}
		public HtmlPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "Html";
			RowCount = model.RowCount;
		}
		public HtmlEditor Editor {
			get { return (HtmlEditor)Control; }
		}
		public int RowCount {
			get { return rowCount; }
			set { rowCount = value; }
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			if(Editor != null) {
				Editor.LockUndo();
				Editor.UpdateButtonState();
			}
		}
		public event EventHandler<CustomizeHtmlHeaderEventArgs> CustomizeHtmlHeader;
		#region IInplaceEditSupport Members
		public RepositoryItem CreateRepositoryItem() {
			RepositoryItemHtmlStringEdit result = new RepositoryItemHtmlStringEdit();
			result.ReadOnly = !AllowEdit;
			return result;
		}
		#endregion
	}
	public class RepositoryItemHtmlStringEdit : RepositoryItemTextEdit {
		internal const string EditorName = "HtmlStringEdit";
		private Regex clearHtmlRegex;
		private Regex clearSpaceRegex;
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(TextEdit),
					typeof(RepositoryItemTextEdit),
					typeof(TextEditViewInfo), new TextEditPainter(), true, EditImageIndexes.TextEdit, 
					typeof(DevExpress.Accessibility.TextEditAccessible)));
			}
		}
		static RepositoryItemHtmlStringEdit() {
			RepositoryItemHtmlStringEdit.Register();
		}
		public RepositoryItemHtmlStringEdit() {
			clearHtmlRegex = new Regex("(</?[^<]+>)|(\n)", RegexOptions.Compiled);
			clearSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
		}
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			string displayText = base.GetDisplayText(format, editValue);
			displayText = clearHtmlRegex.Replace(displayText, " ");
			displayText = clearSpaceRegex.Replace(displayText, " ");
			displayText = DXHttpUtility.HtmlDecode(displayText);
			return displayText;
		}
	}
}
