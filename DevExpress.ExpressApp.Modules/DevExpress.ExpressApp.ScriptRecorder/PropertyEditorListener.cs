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
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public interface IPropertyEditorListener : IScriptRecorderControlListener {
		IPropertyEditorListener Clone();
		PropertyEditor Editor { get; }
	}
	public class PropertyEditorListener : ScriptRecorderListenerBase<PropertyEditor>, IPropertyEditorListener {
		public const string ExecuteEditorActionPrefix = "*ExecuteEditorAction ";
		public const string FillForm = "*FillForm";
		private bool valueChanged = false;
		private void _editor_ValueStoring(object sender, DevExpress.ExpressApp.ValueStoringEventArgs e) {
			SetValueChanged(e);
		}
		private void _editor_ValueStored(object sender, EventArgs e) {
			if(IsValueChanged) {
				ControlNameHelper.Instance.CheckActiveTabForPropertyEditor(Editor.View, Editor.Id);
				valueChanged = false;
				WritePropertyEditorValueStored();
			}
		}
		protected void PropertyEditor_ViewShowingNotification(object sender, EventArgs e) {
			ControlNameHelper.Instance.CheckActiveTabForPropertyEditor(Editor.View, Editor.Id);
			WriteExecuteEditorAction(null);
		}
		protected virtual bool LockFillForm {
			get {
				return Editor is ISupportViewShowing;
			}
		}
		protected void SetValueChanged(ValueStoringEventArgs e) {
			valueChanged = base.ConvertValueToString(e.NewValue) != base.ConvertValueToString(e.OldValue);
		}
		protected bool IsValueChanged {
			get { return valueChanged; }
		}
		protected virtual void WriteExecuteEditorAction(string actionName) {
			string postfix = string.IsNullOrEmpty(actionName) ? null : "(" + actionName + ")";
			AddMessage(ExecuteEditorActionPrefix + EditorFullName + postfix);
		}
		protected virtual void WritePropertyEditorValueStored() {
			if(!LockFillForm) {
				string editorName = EditorFullName;
				AddMessage(FillForm);
				string value = ConvertValueToString(Editor.PropertyValue);
				if(!string.IsNullOrEmpty(value)) {
					if(!value.Contains(Environment.NewLine)) {
						AddMessage(String.Format(" {0} = {1}", editorName, value));
					}
					else {
						string[] values = value.Split('\n');
						for(int counter = 0; counter < values.Length; counter++) {
							AddMessage(String.Format(" {0}[{2}] = {1}", editorName, base.ConvertValueToString(values[counter].TrimEnd('\r')), counter));
						}
					}
				}
			}
		}
		protected void AddMessage(string message) {
			Logger.Instance.WriteMessage(message);
		}
		public override void RegisterControl(PropertyEditor editor) {
			obj = editor;
			editor.ValueStored += new EventHandler(_editor_ValueStored);
			editor.ValueStoring += new EventHandler<DevExpress.ExpressApp.ValueStoringEventArgs>(_editor_ValueStoring);
			if(editor is ISupportViewShowing) {
				((ISupportViewShowing)editor).ViewShowingNotification += new EventHandler<EventArgs>(PropertyEditor_ViewShowingNotification);
			}
		}
		public override void UnRegisterControl(PropertyEditor editor) {
			editor.ValueStored -= new EventHandler(_editor_ValueStored);
			editor.ValueStoring -= new EventHandler<DevExpress.ExpressApp.ValueStoringEventArgs>(_editor_ValueStoring);
			if(editor is ISupportViewShowing) {
				((ISupportViewShowing)editor).ViewShowingNotification -= new EventHandler<EventArgs>(PropertyEditor_ViewShowingNotification);
			}
		}
		public PropertyEditor Editor {
			get { return obj; }
		}
		public string EditorFullName {
			get {
				if(Editor != null) { 
					return ControlNameHelper.Instance.GetFullName(Editor.View, EditorName, Editor);
				}
				else {
					return string.Empty;
				}
			}
		}
		public string EditorName {
			get {
				return (Editor.Caption == null || string.IsNullOrEmpty(Editor.Caption.Trim()))
					? ((IModelViewItem)(Editor.Model)).Caption
					: Editor.Caption;
			}
		}
		public virtual IPropertyEditorListener Clone() {
			return new PropertyEditorListener();
		}
	}
}
