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
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Services;
#if !SL
using DevExpress.Xpf.Editors.Validation.Native;
#else
using System.Windows;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Editors {
	public class SpinEditStrategy : RangeEditorStrategy<decimal> {
		public override bool ShouldRoundToBounds { get { return Editor.AllowRoundOutOfRangeValue; } }
		#region constructors
		public SpinEditStrategy(TextEdit editor)
			: base(editor) {
		}
		#endregion
		#region properties
		protected new SpinEdit Editor {
			get { return base.Editor as SpinEdit; }
		}
		bool IsFloatValue { get { return Editor.IsFloatValue; } }
		#endregion
		protected override MinMaxUpdateHelper CreateMinMaxHelper() {
			return new MinMaxUpdateHelper(Editor, SpinEdit.MinValueProperty, SpinEdit.MaxValueProperty);
		}
		public override object Correct(object baseValue) {
			RangeValueConverter<decimal> container = CreateValueConverter(base.Correct(baseValue));
			decimal converted = container.Value;
			decimal rounded = Editor.IsFloatValue ? container.Value : Math.Round(container.Value);
			if (rounded < GetMinValue() && converted >= GetMinValue())
				rounded += 1M;
			if(rounded > GetMaxValue() && converted <= GetMaxValue())
				rounded -= 1M;
			return rounded;
		}
		internal void UpdateEditMask() {
			string mask = Editor.Mask;
			if(IsFloatValue) {
				if(mask == "N00")
					mask = string.Empty;
			}
			else {
				if(string.IsNullOrEmpty(mask))
					mask = "N00";
			}
			Editor.Mask = mask;
		}
		#region properties syncronization
		protected internal void SyncWithDecimalValue(decimal oldValue, decimal value) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(SpinEdit.ValueProperty, oldValue, value);
		}
		#endregion
		public decimal CoerceDecimalValue(decimal baseValue) {
			CoerceValue(SpinEdit.ValueProperty, baseValue);
			return baseValue;
		}
		public decimal? CoerceMaxValue(decimal? baseValue) {
			return baseValue;
		}
		public decimal? CoerceMinValue(decimal? baseValue) {
			return baseValue;
		}
		public override object CoerceMaskType(MaskType maskType) {
			return MaskType.Numeric;
		}
		protected internal virtual void OnIsFloatValueChanged() {
			UpdateEditMask();
			Editor.CoerceValue(SpinEdit.MinValueProperty);
			Editor.CoerceValue(SpinEdit.MaxValueProperty);
			SyncWithValue();
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(SpinEdit.ValueProperty, baseValue => baseValue, Correct);
		}
		protected override object GetDefaultValue() {
			return 0M;
		}
		protected internal override decimal GetMinValue() {
			if(Editor.MinValue.HasValue)
				return Editor.MinValue.Value;
			return decimal.MinValue;
		}
		protected internal override decimal GetMaxValue() {
			if(Editor.MaxValue.HasValue)
				return Editor.MaxValue.Value;
			return decimal.MaxValue;
		}
		protected override BaseEditingSettingsService CreateTextInputSettingsService() {
			return new ButtonEditSettingsService(Editor);
		}
		protected override EditorSpecificValidator CreateEditorValidatorService() {
			return new SpinEditValidator(Editor);
		}
		protected override RangeEditorService CreateRangeEditService() {
			return new SpinEditRangeService(Editor);
		}
	}
}
