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
using System.Collections;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public abstract class ViewListener<T> : ScriptRecorderListenerBase<T> {
		protected virtual void WriteSelectRecord(ListEditor editor, ObjectView view, bool isCheckRowsValue, int visibleColumnIndex) {
			IList selectedObject = editor.GetSelectedObjects();
			IMemberInfo memberInfo = null;
			IList<IModelColumn> visibleColumns = editor.Model.Columns.GetVisibleColumns();
			if(visibleColumns.Count < 1) {
				return;
			}
			if(visibleColumns.Count > visibleColumnIndex
				&& visibleColumns[visibleColumnIndex] != null) {
				memberInfo = view.ObjectTypeInfo.FindMember(visibleColumns[visibleColumnIndex].PropertyName);
			}
			if(memberInfo == null) {
				memberInfo = view.ObjectTypeInfo.DefaultMember;
			}
			string columnName = CaptionHelper.GetLastMemberPartCaption(memberInfo.Owner.Type, memberInfo.Name);
			List<string> rowsValue = new List<string>();
			foreach(object o in selectedObject) {
				rowsValue.Add(ConvertValueToString(memberInfo.GetValue(o)));
			}
			if(!CheckRowsValue(rowsValue) && !memberInfo.IsKey &&
				visibleColumns.Count > visibleColumnIndex + 1) {
				if(visibleColumns[visibleColumnIndex + 1] != null) {
					WriteSelectRecord(editor, view, true, visibleColumnIndex + 1);
				}
				else {
					WriteSelectRecord(editor, view, false, 0);
				}
			}
			else {
				if(rowsValue.Count > 0) {
					WriteMessage(Logger.SelectRecords + " " + editor.Name);
					WriteMessage(" Columns = " + columnName);
					foreach(string value in rowsValue) {
						WriteMessage(" Row = " + value);
					}
				}
			}
		}
		protected virtual void WriteMessage(string message) {
			Logger.Instance.WriteMessage(message);
		}
		private bool CheckRowsValue(List<string> rowsValue) {
			List<string> values = new List<string>();
			foreach(string value in rowsValue) {
				int x;
				if(string.IsNullOrEmpty(value) || int.TryParse(value, out x)) {
					return false;
				}
				if(!values.Contains(value)) {
					values.Add(value);
				}
				else {
					return false;
				}
			}
			return true;
		}
	}
}
