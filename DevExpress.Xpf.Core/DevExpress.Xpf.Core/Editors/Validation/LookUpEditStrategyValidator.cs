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
using DevExpress.Xpf.Editors.EditStrategy;
namespace DevExpress.Xpf.Editors.Validation.Native {
	public class LookUpEditStrategyValidator : StrategyValidatorBase {
		protected new LookUpEditBase Editor { get { return base.Editor as LookUpEditBase; } }
		LookUpEditStrategyBase Strategy { get; set; }
		SelectorPropertiesCoercionHelper SelectorUpdater { get; set; }
		public LookUpEditStrategyValidator(LookUpEditStrategyBase strategy, SelectorPropertiesCoercionHelper selectorUpdater, LookUpEditBase editor)
			: base(editor) {
			Strategy = strategy;
			SelectorUpdater = selectorUpdater;
		}
		public override object ProcessConversion(object value) {
			if (Editor.IsTokenMode)
				return ProcessMultipleConversion(value);
			return ProcessSingleConversion(value);
		}
		private object ProcessSingleConversion(object value) {
			LookUpEditableItem item = value as LookUpEditableItem;
			if (item == null && !Strategy.ShouldRaiseProcessValueConversion)
				return base.ProcessConversion(value);
			if (item != null) {
				string editText = Convert.ToString(item.DisplayValue);
				object editValue = item.EditValue;
				if (item.Completed)
					return editValue;
				if (!Strategy.IsInLookUpMode)
					return base.ProcessConversion(editValue);
				int index = item.ForbidFindIncremental ? SelectorUpdater.GetIndexFromEditValue(item.EditValue) : Strategy.FindItemIndexByText(editText, !Strategy.ShouldRaiseProcessValueConversion && Editor.AutoComplete);
				bool found = IndexFromItemsSource(index);
				if (!found && Strategy.ShouldRaiseProcessValueConversion && !item.ProcessNewValueCompleted) {
					item.ProcessNewValueCompleted = true;
					Strategy.ProcessNewValue(editText);
					index = Strategy.FindItemIndexByText(editText, false);
					if (!IndexFromItemsSource(index) && Editor.AutoComplete)
						index = Strategy.FindItemIndexByText(editText, true);
				}
				if (IndexFromItemsSource(index)) {
					Strategy.UpdateAutoSearchText(new ChangeTextItem() { Text = editText }, true);
					return SelectorUpdater.GetEditValueFromSelectedIndex(index);
				}
				return editValue;
			}
			return value;
		}
		private object ProcessMultipleConversion(object editValue) {
			IList<object> listEditValue = null;
			if (editValue is LookUpEditableItem)
				listEditValue = ((LookUpEditableItem)editValue).EditValue as IList<object>;
			else
				listEditValue = editValue as IList<object>;
			if (listEditValue != null) {
				var newEditValue = new List<object>();
				foreach (var value in listEditValue) {
					var newValue = ProcessSingleConversion(value);
					if (newValue != null && !string.IsNullOrEmpty(newValue.ToString()))
						newEditValue.Add(newValue);
				}
				return newEditValue.Count > 0 ? newEditValue : null;
			}
			var singleValue = ProcessSingleConversion(editValue);
			return singleValue != null ? new List<object>() { singleValue } : null;
		}
		bool IndexFromItemsSource(int index) {
			return index > -1 && !Strategy.IsInProcessNewValueDialog;
		}
		protected override bool IsValidValue(object value, object convertedValue) {
			var lookUpEditableItem = value as LookUpEditableItem;
			if (lookUpEditableItem != null) {
				if (!lookUpEditableItem.Completed && lookUpEditableItem.AsyncLoading)
					return false;
			}
			return base.IsValidValue(value, convertedValue);
		}
		public override string GetValidationError() {
			return "hidden error";
		}
	}
}
