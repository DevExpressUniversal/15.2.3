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
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.ScriptRecorder.Web {
	public class WebPropertyEditorListenersFactory : PropertyEditorListenersFactoryBase {
		protected override void RegisterListeners() {
			RegisteredListeners.Add(typeof(ASPxDateTimePropertyEditor), new DatePropertyEditorListener());
			RegisteredListeners.Add(typeof(ASPxLookupPropertyEditor), new ASPxLookupPropertyEditorListener());
		}
	}
	public class ASPxLookupPropertyEditorListener : PropertyEditorListener {
		private bool lockFillFormInternal = false;
		private void Logger_AddMessage(object sender, AddMessageEventArgs e) {
			Logger.Instance.AddMessage -= new EventHandler<AddMessageEventArgs>(Logger_AddMessage);
			e.Cancel = true; 
		}
		protected override string ConvertValueToString(object value) { 
			ASPxLookupPropertyEditor editor_ = (ASPxLookupPropertyEditor)Editor;
			string lookupProperty = Editor.Model.LookupProperty;
			if(!string.IsNullOrEmpty(lookupProperty)) {
				IMemberInfo memberInfo = editor_.WebLookupEditorHelper.LookupObjectTypeInfo.FindMember(lookupProperty);
				if(memberInfo != null) {
					return base.ConvertValueToString(memberInfo.GetValue(value));
				}
			}
			return base.ConvertValueToString(value);
		}
		protected override void WriteExecuteEditorAction(string actionName) {
			ASPxLookupPropertyEditor editor = (ASPxLookupPropertyEditor)Editor;
			bool useFindEdit = editor.UseFindEdit();
			string actionCaption = useFindEdit ? CaptionHelper.GetLocalizedText("DialogButtons", "Find") : CaptionHelper.GetActionCaption("New");
			base.WriteExecuteEditorAction(actionCaption);
			lockFillFormInternal = true;
			if(!useFindEdit) {
				Logger.Instance.AddMessage += new EventHandler<AddMessageEventArgs>(Logger_AddMessage);
			}
		}
		protected override bool LockFillForm {
			get {
				bool result = lockFillFormInternal;
				lockFillFormInternal = false;
				return result;
			}
		}
		public override IPropertyEditorListener Clone() {
			return new ASPxLookupPropertyEditorListener();
		}
		public override string EmptyValue {
			get {
				return CaptionHelper.DefaultNullValueText;
			}
		}
	}
}
