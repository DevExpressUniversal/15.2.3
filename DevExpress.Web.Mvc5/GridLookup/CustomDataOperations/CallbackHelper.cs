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
namespace DevExpress.Web.Mvc.Internal {
	public class GridLookupCustomOperationCallbackHelper {
		public static void ProcessModelBinding(GridLookupFilteringState filteringState) {
			filteringState.GridViewModel = GridLookupViewModel.Load(GridStateKey);
			if(filteringState.GridViewModel == null)
				return;
			string commandName;
			string[] args = GridViewCustomOperationCallbackHelper.GetCallbackArguments(out commandName, false);
			if(commandName != GridViewCallbackCommand.CustomCallback) {
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(filteringState, GridStateKey, null);
				return;
			}
			GridLookupCallbackArgumentsReader argument = new GridLookupCallbackArgumentsReader(string.Join("|", args));
			if(argument.IsFilteringCallback)
				ProcessFilteringCallback(filteringState, argument.Filter);
		}
		public static string GridStateKey {
			get {
				string key = MvcUtils.CallbackName.Replace(GridViewExtension.FilterControlCallbackPostfix, string.Empty);
				if(!string.IsNullOrEmpty(key) && key.EndsWith(GridLookupViewModel.GridPostfixClientID))
					key = key.Replace(GridLookupViewModel.GridPostfixClientID, GridLookupViewModel.GridPostfix);
				return key;
			}
		}
		static void ProcessFilteringCallback(GridLookupFilteringState filteringState, string filter) {
			var viewModel = filteringState.GridViewModel;
			var expressionCreator = CreateExpressionCreator(viewModel.IncrementalFilteringMode);
			var textFormatString = GetTextFormatString(viewModel);
			var fieldNames = viewModel.Columns.Select(c => c.FieldName).ToList();
			filteringState.FilterExpression = expressionCreator.CreateFilterExpression(textFormatString, fieldNames, filter);
		}
		static string GetTextFormatString(GridLookupViewModel viewModel) {
			if(!string.IsNullOrEmpty(viewModel.TextFormatString))
				return viewModel.TextFormatString;
			return CommonUtils.GetDefaultTextFormatString(viewModel.Columns.Count, false);
		}
		static FilterExpressionCreatorBase CreateExpressionCreator(IncrementalFilteringMode filteringMode) {
			if(filteringMode == IncrementalFilteringMode.StartsWith)
				return new StartWithFilterExpressionCreator();
			if(filteringMode == IncrementalFilteringMode.Contains)
				return new ContainsFilterExpressionCreator();
			return new DisabledFilterExpressionCreator();
		}
	}
}
