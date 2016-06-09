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
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using System.Collections.Generic;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
namespace DevExpress.Xpf.Grid {
	public abstract class IndicatorFormatConditionBase : FormatConditionBase {
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(object), typeof(IndicatorFormatConditionBase), new PropertyMetadata(null, (d, e) => ((IndicatorFormatConditionBase)d).OnMinMaxChanged(e)));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(object), typeof(IndicatorFormatConditionBase), new PropertyMetadata(null, (d, e) => ((IndicatorFormatConditionBase)d).OnMinMaxChanged(e)));
		public static readonly DependencyProperty SelectiveExpressionProperty = DependencyProperty.Register("SelectiveExpression", typeof(string), typeof(IndicatorFormatConditionBase), new PropertyMetadata(null, (d, e) => ((IndicatorFormatConditionBase)d).OnSelectiveExpressionChanged(e)));
		[XtraSerializableProperty]
		public object MinValue {
			get { return (object)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		[XtraSerializableProperty]
		public object MaxValue {
			get { return (object)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		[XtraSerializableProperty]
		public string SelectiveExpression {
			get { return (string)GetValue(SelectiveExpressionProperty); }
			set { SetValue(SelectiveExpressionProperty, value); }
		}
		protected override void SyncProperty(DependencyProperty property) {
			base.SyncProperty(property);
			SyncIfNeeded(property, IndicatorFormatConditionBase.SelectiveExpressionProperty, () => IndicatorInfo.SelectiveExpression = SelectiveExpression);
		}
		IndicatorFormatConditionInfo IndicatorInfo { get { return Info as IndicatorFormatConditionInfo; } }
		void OnSelectiveExpressionChanged(DependencyPropertyChangedEventArgs e) {
			IndicatorInfo.SelectiveExpression = SelectiveExpression;
			OnChanged(e);
		}
		void OnMinMaxChanged(DependencyPropertyChangedEventArgs e) {
			IndicatorInfo.OnMinMaxChanged(e.NewValue, e.Property == MaxValueProperty);
			OnChanged(e, FormatConditionBaseInfo.GetChangeType(e));
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			IndicatorEditUnit indicatorEditUnit = unit as IndicatorEditUnit;
			if(indicatorEditUnit != null) {
				indicatorEditUnit.MinValue = MinValue;
				indicatorEditUnit.MaxValue = MaxValue;
			}
		}
		IndicatorFormatBase FormatCore { get { return (IndicatorFormatBase)GetValue(FormatPropertyForBinding); } }
		protected override bool CanAttach { get { return !string.IsNullOrEmpty(FieldName); } }
	}
	public class ColorScaleFormatCondition : IndicatorFormatConditionBase {
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ColorScaleFormat Format {
			get { return (ColorScaleFormat)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(ColorScaleFormat), typeof(ColorScaleFormatCondition), new PropertyMetadata(null, OnFormatChanged, OnCoerceFreezable));
		public override DependencyProperty FormatPropertyForBinding { get { return FormatProperty; } }
		protected override FormatConditionBaseInfo CreateInfo() {
			return new ColorScaleFormatConditionInfo();
		}
		protected override bool CanAttach { get { return !string.IsNullOrEmpty(FieldName) || Expression != null; } }
		internal override FormatConditionRuleBase CreateExportWrapper() {
			if(Format != null && Format.ColorMiddle.HasValue)
				return new FormatConditionRuleColorScale3ExportWrapper(this);
			return new FormatConditionRuleColorScale2ExportWrapper(this);
		}
		protected override BaseEditUnit CreateEmptyEditUnit() {
			return new ColorScaleEditUnit();
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			ColorScaleEditUnit colorScaleEditUnit = unit as ColorScaleEditUnit;
			if(colorScaleEditUnit != null) {
				colorScaleEditUnit.Format = Format;
			}
		}
	}
	public class DataBarFormatCondition : IndicatorFormatConditionBase {
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public DataBarFormat Format {
			get { return (DataBarFormat)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(DataBarFormat), typeof(DataBarFormatCondition), new PropertyMetadata(null, OnFormatChanged, OnCoerceFreezable));
		internal override FormatConditionRuleBase CreateExportWrapper() {
			return new FormatConditionRuleDataBarExportWrapper(this);
		}
		public override DependencyProperty FormatPropertyForBinding { get { return FormatProperty; } }
		protected override FormatConditionBaseInfo CreateInfo() {
			return new DataBarFormatConditionInfo();
		}
		protected override BaseEditUnit CreateEmptyEditUnit() {
			return new DataBarEditUnit();
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			DataBarEditUnit dataBarEditUnit = unit as DataBarEditUnit;
			if(dataBarEditUnit != null) {
				dataBarEditUnit.Format = Format;
			}
		}
	}
	public class IconSetFormatCondition : IndicatorFormatConditionBase {
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IconSetFormat Format {
			get { return (IconSetFormat)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(IconSetFormat), typeof(IconSetFormatCondition), new PropertyMetadata(null, OnFormatChanged, OnCoerceFreezable));
		public override DependencyProperty FormatPropertyForBinding { get { return FormatProperty; } }
		protected override FormatConditionBaseInfo CreateInfo() {
			return new IconSetFormatConditionInfo();
		}
		internal override FormatConditionRuleBase CreateExportWrapper() {
			return new FormatConditionRuleIconSetExportWrapper(this);
		}
		protected override BaseEditUnit CreateEmptyEditUnit() {
			return new IconSetEditUnit();
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			IconSetEditUnit iconSetEditUnit = unit as IconSetEditUnit;
			if(iconSetEditUnit != null)
				iconSetEditUnit.Format = Format;
		}
	}
}
