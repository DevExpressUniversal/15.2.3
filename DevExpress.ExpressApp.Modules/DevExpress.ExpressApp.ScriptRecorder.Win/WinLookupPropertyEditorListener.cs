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
using DevExpress.ExpressApp.Win.Editors;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.ScriptRecorder.Win {
	public class WinLookupPropertyEditorListener : PropertyEditorListener {
		private bool lockFillFormInternal = true;
		private void LookupProperty_QueryCloseUp(object sender, CancelEventArgs e) {
			PopUpClosed();
		}
		private void Logger_AddMessage(object sender, AddMessageEventArgs e) {
			Logger.Instance.AddMessage -= new EventHandler<AddMessageEventArgs>(Logger_AddMessage);
			if(e.Message.StartsWith(ScriptRecorderActionsListenerBase.ProcessRecordPrefix)) {
				lockFillFormInternal = false;
				e.Cancel = true;
			} else {
				AddWriteExecuteEditorActionMessages();
			}
		}
		private void AddWriteExecuteEditorActionMessages() {
			base.WriteExecuteEditorAction(null);
		}
		protected override string ConvertValueToString(object value) { 
			LookupPropertyEditor editor_ = (LookupPropertyEditor)Editor;
			string lookupProperty = Editor.Model.LookupProperty;
			if(!string.IsNullOrEmpty(lookupProperty)) {
				IMemberInfo memberInfo = editor_.Helper.LookupObjectTypeInfo.FindMember(lookupProperty);
				if(memberInfo != null) {
					return base.ConvertValueToString(memberInfo.GetValue(value));
				}
			}
			return base.ConvertValueToString(value);
		}
		protected void PopUpClosed() {
			Logger.Instance.AddMessage -= new EventHandler<AddMessageEventArgs>(Logger_AddMessage);
		}
		protected override void WriteExecuteEditorAction(string actionName) {
			Logger.Instance.AddMessage += new EventHandler<AddMessageEventArgs>(Logger_AddMessage);
		}
		protected override void WritePropertyEditorValueStored() {
			base.WritePropertyEditorValueStored();
			lockFillFormInternal = true;
		}
		protected override bool LockFillForm {
			get {
				return lockFillFormInternal;
			}
		}
		public override void RegisterControl(PropertyEditor editor) {
			base.RegisterControl(editor);
			((LookupPropertyEditor)editor).QueryCloseUp += new CancelEventHandler(LookupProperty_QueryCloseUp);
		}
		public override void UnRegisterControl(PropertyEditor editor) {
			base.UnRegisterControl(editor);
			Logger.Instance.AddMessage -= new EventHandler<AddMessageEventArgs>(Logger_AddMessage);
			((LookupPropertyEditor)editor).QueryCloseUp -= new CancelEventHandler(LookupProperty_QueryCloseUp);
		}
		public override IPropertyEditorListener Clone() {
			return new WinLookupPropertyEditorListener();
		}
	}
	public class ObjectPropertyEditorListener : PropertyEditorListener {
		protected override bool LockFillForm {
			get {
				return true;
			}
		}
		public override IPropertyEditorListener Clone() {
			return new ObjectPropertyEditorListener();
		}
	}
	public class RichTextPropertyEditorListener : PropertyEditorListener {
		protected override string ConvertValueToString(object value) {
			return null;
		}
		public override IPropertyEditorListener Clone() {
			return new RichTextPropertyEditorListener();
		}
	}
}
