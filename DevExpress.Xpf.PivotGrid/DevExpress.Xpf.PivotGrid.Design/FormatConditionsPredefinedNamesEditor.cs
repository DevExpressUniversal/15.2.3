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
using System.Windows;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.PivotGrid.Internal;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.PivotGrid.Design {
	internal class ConditionFormatNameEditor : PropertyValueEditor {
		public ConditionFormatNameEditor() {
			this.InlineEditorTemplate = (DataTemplate)PivotGridDesignTimeHelper.EditorsTemplates["PredefinedRulesNames"];
		}
	}
	public abstract class DropDownComboBoxDesigTimeDecorator : ComboBoxDesigTimeDecorator {
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			Control.DropDownOpened += new EventHandler(OnDropDownOpened);
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			Control.DropDownOpened -= new EventHandler(OnDropDownOpened);
		}
		protected abstract void OnDropDownOpened(object sender, EventArgs e);
	}
	public class ConditionFormatNameComboBoxDesigTimeDecorator : DropDownComboBoxDesigTimeDecorator {
		protected override void OnDropDownOpened(object sender, EventArgs e) {
			PropertyValue propertyValue = (PropertyValue)DataContext;
			ModelItem selectedItem = PivotGridDesignTimeHelper.GetPrimarySelection(propertyValue);
			PivotGridControl pivot = PivotGridDesignTimeHelper.GetPivotGrid(selectedItem);
			if(pivot != null) {
				FormatConditionBase condition = FindFormatCondition(propertyValue);
				if(condition == null)
					condition = PivotGridDesignTimeHelper.GetPrimarySelection(propertyValue).GetCurrentValue() as FormatConditionBase;
				if(condition != null) {
					Control.ItemsSource = GetFormatNames(condition, pivot);
					return;
				}
			}
			MessageBox.Show(SR.CantShowFormatNameEditorMessage);
		}
		FormatConditionBase FindFormatCondition(PropertyValue propertyValue) {
			PropertyValue value = propertyValue;
			while(value != null && value.ParentProperty != null) {
				PropertyEntry entry = value.ParentProperty;
				if(typeof(FormatConditionBase).IsAssignableFrom(entry.PropertyType))
					return value.Value as FormatConditionBase;
				value = entry.ParentValue;
			}
			return null;
		}
		IEnumerable<string> GetFormatNames(FormatConditionBase format, PivotGridControl pivot) {
			if(format == null)
				return null;
			string predefinedFormatsPropertyName = PivotGridHelper.GetFormatConditionPredefinedNamesProperty(format);
			FormatInfoCollection predefinedFormats = typeof(PivotGridControl).GetProperty(predefinedFormatsPropertyName).GetValue(pivot, null) as FormatInfoCollection;
			return predefinedFormats != null ? predefinedFormats.Select(x => x.FormatName) : null;
		}
	}
}
