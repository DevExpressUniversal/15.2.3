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

using System.Windows;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotConditionalFormattingDialogDirector : ConditionalFormattingDialogDirector {
		const string ConditionalFormatting_ClearRules_FromIntersection = "ConditionalFormatting_ClearRules_FromIntersection";
		public PivotConditionalFormattingDialogDirector(IDialogContext context, IConditionalFormattingCommands commands, IConditionalFormattingDialogBuilder builder, FrameworkElement resourceOwner) :
			base(context, commands, builder, resourceOwner) {
		}
		protected override void CreateClearMenuItems(Data.IDataColumnInfo info, Bars.BarSubItem clearRulesItem) {
			Builder.CreateBarButtonItem(clearRulesItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ClearRules_FromAllColumns,
								PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesClearAllRules), false, null, Commands.ClearFormatConditionsFromAllColumns, null);
			Builder.CreateBarButtonItem(clearRulesItem.ItemLinks, ConditionalFormatting_ClearRules_FromIntersection,
					  PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesClearIntersectionRules), false, null, ((IPivotConditionalFormattingCommands)Commands).ClearFormatConditionsFromIntersection, info);
			Builder.CreateBarButtonItem(clearRulesItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ClearRules_FromCurrentColumns,
								PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesClearMeasureRules), false, null, Commands.ClearFormatConditionsFromColumn, info);
		}
	}
}
