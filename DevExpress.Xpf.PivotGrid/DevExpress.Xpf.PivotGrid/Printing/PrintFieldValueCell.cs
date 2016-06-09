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
using System.Windows.Controls;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using System;
using DevExpress.Data.PivotGrid;
using System.Windows.Media;
#if SL
using ContentPresenter = DevExpress.Xpf.Core.XPFContentPresenter;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public abstract class BasePrintContentPresenter : ContentPresenter {
		ScrollableAreaItemBase valueItem;
		public ScrollableAreaItemBase ValueItem {
			get { return valueItem; }
			set {
				if(value == null)
					return;
				PivotGridField field = value.Field;
				DataTemplate template;
				DataTemplateSelector templateSelector;
				if(field != null) {
					template = GetContentTemplate(field);
					templateSelector = GetContentTemplateSelector(field);
				} else {
					template = null;
					templateSelector = null;
				}
				bool needOverride = templateSelector != ContentTemplateSelector || template != ContentTemplate;
				if(needOverride) {
					ContentTemplate = null;
					ContentTemplateSelector = null;
				}
				Content = value;
				if(needOverride) {
					ContentTemplateSelector = templateSelector;
					ContentTemplate = template;
				}
				valueItem = value;
				OnValueItemChanged();
			}
		}
		protected virtual void OnValueItemChanged() { }
		protected abstract DataTemplateSelector GetContentTemplateSelector(PivotGridField field);
		protected abstract DataTemplate GetContentTemplate(PivotGridField field);
	}
	public class PrintFieldValueCell : BasePrintContentPresenter {
		protected override DataTemplateSelector GetContentTemplateSelector(PivotGridField field) {
			return field.ActualPrintValueTemplateSelector;
		}
		protected override DataTemplate GetContentTemplate(PivotGridField field) {
			return field.ActualPrintValueTemplate;
		}
	}
	public class PivotPrintCell : BasePrintContentPresenter, IConditionalFormattingClient<PivotPrintCell>, IChrome, IFormatInfoProvider {
		readonly Locker locker = new Locker();
		readonly ConditionalFormattingHelper<PivotPrintCell> formattingHelper;
		readonly ConditionalFormatContentRenderHelper<PivotPrintCell> conditionalFormatContentRenderHelper;
		public PivotPrintCell() {
			formattingHelper = new ConditionalFormattingHelper<PivotPrintCell>(this);
			conditionalFormatContentRenderHelper = new ConditionalFormatContentRenderHelper<PivotPrintCell>(this);
		}
		protected override DataTemplateSelector GetContentTemplateSelector(PivotGridField field) {
			return field.ActualPrintCellTemplateSelector;
		}
		protected override DataTemplate GetContentTemplate(PivotGridField field) {
			return field.ActualPrintCellTemplate;
		}
		protected override void OnValueItemChanged() {
			if(ValueItem == null)
				return;
			SetValue(TextBlock.ForegroundProperty, ValueItem.Foreground);
			formattingHelper.UpdateConditionalAppearance();
			ValueItem.Foreground = (Brush)GetValue(TextBlock.ForegroundProperty);
		}
		ConditionalFormattingHelper<PivotPrintCell> IConditionalFormattingClient<PivotPrintCell>.FormattingHelper {
			get { return formattingHelper; }
		}
		System.Collections.Generic.IList<FormatConditionBaseInfo> IConditionalFormattingClient<PivotPrintCell>.GetRelatedConditions() {
			if(ValueItem != null && ValueItem.Field != null)
				return ValueItem.Field.PivotGrid.FormatConditions.GetInfoByFieldName(ValueItem.Field.Name, (CellsAreaItem)ValueItem);
			return null;
		}
		FormatValueProvider? IConditionalFormattingClient<PivotPrintCell>.GetValueProvider(string fieldName) {
			return new FormatValueProvider(this, ValueItem.Value, ValueItem.Field.Name);
		}
		bool IConditionalFormattingClient<PivotPrintCell>.IsReady {
			get { return true; }
		}
		bool IConditionalFormattingClient<PivotPrintCell>.IsSelected {
			get { return false; }
		}
		Core.Locker IConditionalFormattingClient<PivotPrintCell>.Locker {
			get { return locker; }
		}
		void IConditionalFormattingClient<PivotPrintCell>.UpdateBackground() {
			ValueItem.Background = formattingHelper.CoerceBackground(ValueItem.Background);
		}
		void IConditionalFormattingClient<PivotPrintCell>.UpdateDataBarFormatInfo(DataBarFormatInfo info) {
			conditionalFormatContentRenderHelper.UpdateDataBarFormatInfo(info);
		}
		void IChrome.AddChild(FrameworkElement element) {
		}
		void IChrome.GoToState(string stateName) {
		}
		void IChrome.RemoveChild(FrameworkElement element) {
		}
		FrameworkRenderElementContext IChrome.Root {
			get { return null; }
		}
		object IFormatInfoProvider.GetCellValue(string fieldName) {
			CellsAreaItem cell = ValueItem as CellsAreaItem;
			PivotGridData data = ValueItem.Field.Data;
			PivotGridFieldBase field = data.GetFieldByNameOrDataControllerColumnName(fieldName);
			if(field == null || !field.Visible)
				return false;
			switch(field.Area) {
				case PivotArea.RowArea:
					return data.VisualItems.GetFieldValue(field, cell.RowIndex);
				case PivotArea.ColumnArea:
					return data.VisualItems.GetFieldValue(field, cell.ColumnIndex);
				case PivotArea.DataArea:
					return data.GetCellValue(cell.Item.ColumnFieldIndex, cell.Item.RowFieldIndex, field);
				case PivotArea.FilterArea:
					return null;
				default:
					throw new ArgumentException("fieldDescriptor.Field.Area");
			}
		}
		object IFormatInfoProvider.GetCellValueByListIndex(int listIndex, string fieldName) {
			throw new NotImplementedException();
		}
		object IFormatInfoProvider.GetTotalSummaryValue(string fieldName, ConditionalFormatSummaryType summaryType) {
			return ValueItem.Field.Data.GetAggregation(fieldName, ((CellsAreaItem)ValueItem).Item, summaryType);
		}
		static PivotValueComparerBase comparer = new PivotValueComparerBase();
		Data.ValueComparer IFormatInfoProvider.ValueComparer {
			get { return comparer; }
		}
	}
}
