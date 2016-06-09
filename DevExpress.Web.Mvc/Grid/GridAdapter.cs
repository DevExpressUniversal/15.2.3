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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Web.Internal;
using System.Web.Mvc;
namespace DevExpress.Web.Mvc.Internal {
	public interface IGridAdapterOwner : IViewContext {
		Func<string, string> GetEditorIdByFieldName { get; }
		bool ShowModelErrorsForEditors { get; }
		object CallbackRouteValues { get; }
		IDictionary<string, string> CallbackActionUrlCollection { get; }
		bool EnableCustomOperations { get; }
		GridBaseViewModel CustomOperationViewModel { get; }
		GridAdapter Adapter { get; }
		ModelStateDictionary ModelState { get; }
	}
	public class GridAdapter {
		public GridAdapter(IGridAdapterOwner owner) {
			Owner = owner;
			UnobtrusiveValidationRules = new Dictionary<string, object>();
		}
		protected internal IGridAdapterOwner Owner { get; private set; }
		protected internal IDictionary<string, object> UnobtrusiveValidationRules { get; private set; }
		public void ApplyEditorValidationSettings(ASPxEdit editor, IWebGridDataColumn column) {
			if(editor == null)
				return;
			IDictionary<string, object> validationRules = ExtensionsHelper.GetUnobtrusiveValidationRulesForColumnEditor(column.FieldName, Owner.ViewContext, Owner.GetEditorIdByFieldName);
			if(validationRules != null && validationRules.Count > 0)
				UnobtrusiveValidationRules[editor.ClientID] = validationRules;
			ExtensionsHelper.AssignValidationSettingsToColumnEditor(editor, column.FieldName, Owner.ViewContext, Owner.ShowModelErrorsForEditors);
		}
		public void AppendUnobtrusiveRules(StringBuilder stb, IEnumerable<string> editorNames) {
			var avalibleValidationRules = UnobtrusiveValidationRules.Where(r => editorNames.Contains(r.Key));
			foreach(var keyPairValue in avalibleValidationRules) {
				stb.Append(string.Format("MVCx.InitUnobtrusiveRules({0},{1});\n",
					HtmlConvertor.ToScript(keyPairValue.Key),
					HtmlConvertor.ToJSON(keyPairValue.Value)
				));
			}
		}
		public static string[] GetCommandsByActionType(int actionType) {
			switch(actionType) {
				case 0:
					return new string[] { GridViewCallbackCommand.NextPage, GridViewCallbackCommand.PreviousPage, GridViewCallbackCommand.GotoPage, GridViewCallbackCommand.PagerOnClick };
				case 1:
					return new string[] { GridViewCallbackCommand.FilterRowMenu, GridViewCallbackCommand.ApplyColumnFilter, GridViewCallbackCommand.ApplyHeaderColumnFilter,
						FilterControlCallbackCommand.Apply, GridViewCallbackCommand.ApplyFilter, GridViewCallbackCommand.SetFilterEnabled, GridViewCallbackCommand.ApplyMultiColumnFilter, GridViewCallbackCommand.ApplySearchPanelFilter };
				case 2:
					return new string[] { GridViewCallbackCommand.Sort };
				case 3:
					return new string[] { GridViewCallbackCommand.Group, GridViewCallbackCommand.UnGroup };
			}
			return new string[0];
		}
	}
}
