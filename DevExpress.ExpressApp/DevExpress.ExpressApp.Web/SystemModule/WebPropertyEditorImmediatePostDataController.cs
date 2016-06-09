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

using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Templates;
using System;
using System.Collections.Generic;
using System.Web.UI;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public interface ISupportImmediatePostData {
		void SetImmediatePostDataCompanionScript(string script);
		void SetImmediatePostDataScript(string script);
		bool ImmediatePostData { get; }
	}
	public class WebPropertyEditorImmediatePostDataController : ViewController<DetailView> {
		public const string ImmediatePostDataCompanionScript = @"
                    function(s, e) {
                        document.isMenuClicked = false;
                    }
                ";
		protected const string ImmediatePostDataScriptPattern = @"function(s,e) {{ window.setTimeout(function() {{
                            if(!document.isMenuClicked) {{
                                {0}
                            }}
                            document.isMenuClicked = false;
                        }}, 500); }}";
		private Dictionary<Control, ISupportImmediatePostData> controlToPropertyEditorMap = new Dictionary<Control, ISupportImmediatePostData>();
		private void Editor_Load(object sender, EventArgs e) {
			Control control = (Control)sender;
			control.Load -= Editor_Load;
			ProcessControlLoad(control);
		}
		private void editor_ControlCreated(object sender, EventArgs e) {
			WebPropertyEditor editor = (WebPropertyEditor)sender;
			editor.ControlCreated -= editor_ControlCreated;
			SubscribeControlEvents(editor);
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			controlToPropertyEditorMap.Clear();
			if(View.ViewEditMode == ExpressApp.Editors.ViewEditMode.Edit) {
				ProcessEditors();
			}
		}
		protected void ProcessEditors() {
			foreach(WebPropertyEditor editor in GetWebPropertyEditors()) {
				SubscribeControlEvents(editor);
			}
		}
		protected virtual void SubscribeControlEvents(WebPropertyEditor editor) {
			if(editor.Editor != null) {
				editor.Editor.Load += Editor_Load;
				controlToPropertyEditorMap[editor.Editor] = editor;
			}
			else {
				editor.ControlCreated += editor_ControlCreated;
			}
		}
		protected virtual IList<WebPropertyEditor> GetWebPropertyEditors() {
			return View.GetItems<WebPropertyEditor>();
		}
		protected void ProcessControlLoad(Control control) {
			ISupportImmediatePostData supportImmediatePostDataMangment = controlToPropertyEditorMap[control];
			supportImmediatePostDataMangment.SetImmediatePostDataCompanionScript(ImmediatePostDataCompanionScript);
			if(supportImmediatePostDataMangment.ImmediatePostData) {
				string immediatePostDataScript = GetImmediatePostDataScript();
				supportImmediatePostDataMangment.SetImmediatePostDataScript(immediatePostDataScript);
			}
		}
		public static string GetImmediatePostDataScript() {
			return string.Format(ImmediatePostDataScriptPattern, ScriptGenerator.GetScript());
		}
	}
}
