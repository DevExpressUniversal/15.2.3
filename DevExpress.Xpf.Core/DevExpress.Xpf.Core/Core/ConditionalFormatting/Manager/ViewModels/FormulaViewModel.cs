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
using System.Collections.ObjectModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Native;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class FormulaViewModel : FormatEditorOwnerViewModel {
		public static Func<IDialogContext, FormulaViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new FormulaViewModel(x)); } }
		public FormulaViewModel(IDialogContext context)
			: base(context) {
			ExpressionEditorViewModel = CustomConditionConditionalFormattingDialogViewModel.Factory(context.PredefinedFormatsOwner);
			ExpressionEditorViewModel.Initialize(context);
		}
		public CustomConditionConditionalFormattingDialogViewModel ExpressionEditorViewModel { get; private set; }
		protected override void AddChanges(ConditionEditUnit unit) {
			base.AddChanges(unit);
			string fieldName = Context.ColumnInfo.FieldName;
			unit.FieldName = fieldName;
			unit.Expression = ExpressionEditorViewModel.GetExpression(fieldName);
			unit.ValueRule = ConditionRule.Expression;
			unit.Value1 = null;
			unit.Value2 = null;
		}
		protected override void InitCore(ConditionEditUnit unit) {
			base.InitCore(unit);
			string expression = unit.GetActualExpressionString();
			ExpressionEditorViewModel.Value = ManagerHelper.ConvertExpression(expression, Context.ColumnInfo, UnboundExpressionConvertHelper.ConvertToCaption);
		}
		protected override bool CanInitCore(ConditionEditUnit unit) {
			return unit is ConditionEditUnit;
		}
		protected override bool ValidateExpression() {
			return ExpressionEditorViewModel.TryClose();
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormulaDescription); } }
	}
}
