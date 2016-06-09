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
using DevExpress.Xpf.Editors.Controls;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows;
#else
using System.Windows;
using DevExpress.Xpf.Editors.Helpers;
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
namespace DevExpress.Xpf.Editors.EditStrategy {
	public class CalcEditStrategy : PopupBaseEditStrategy {
		new PopupCalcEdit Editor { get { return (PopupCalcEdit)base.Editor; } }
		Calculator Calculator { get { return Editor.Calculator; } }
		bool IsDisplayTextFromCalculator { get { return Editor.IsPopupOpen && Calculator != null; } }
		bool isCalculatorValueChanged;
		public CalcEditStrategy(PopupCalcEdit editor) : base(editor) {
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(PopupCalcEdit.ValueProperty, baseValue => baseValue, baseValue => baseValue.TryConvertToDecimal());
		}
		public decimal CoerceDecimalValue(decimal baseValue) {
			CoerceValue(PopupCalcEdit.ValueProperty, baseValue);
			return baseValue;
		}
		protected virtual internal void SyncWithDecimalValue(decimal oldValue, decimal value) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(PopupCalcEdit.ValueProperty, oldValue, value);
		}
		protected override string GetDisplayText() {
			if(IsDisplayTextFromCalculator) 
				return Calculator.DisplayText;
			return base.GetDisplayText();
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			if (Calculator != null) {
				Calculator.Value = ValueContainer.EditValue.TryConvertToDecimal();
			}
		}
		public override void UpdateDisplayText() {
			base.UpdateDisplayText();
			if(EditBox == null)
				return;
			if(IsDisplayTextFromCalculator)
				SelectAll();
		}
		public virtual void AcceptCalculatorValue() {
			if (Calculator == null)
				return;
			ValueContainer.SetEditValue(Calculator.Value, UpdateEditorSource.ValueChanging);
			SelectAll();
		}
		public virtual void CalculatorAssigned(PopupCalcEditCalculator oldCalculator) {
			if (oldCalculator != null)
				oldCalculator.ValueChanged -= OnCalculatorValueChanged;
			if (Calculator == null)
				return;
			isCalculatorValueChanged = false;
			UpdateDataContext();
			Calculator.Value = ValueContainer.EditValue.TryConvertToDecimal();
			Calculator.ValueChanged += OnCalculatorValueChanged;
		}
		protected virtual void OnCalculatorValueChanged(object sender, CalculatorValueChangedEventArgs e) {
			isCalculatorValueChanged = true;
		}
		void UpdateDataContext() {
			DependencyObject obj = Calculator.DataContext as DependencyObject;
			if (obj == null) {
				obj = new System.Windows.Controls.Button();
				Calculator.DataContext = obj;
			}
			BaseEdit.SetOwnerEdit(obj, Editor);
		}
		internal override void FlushPendingEditActions(UpdateEditorSource updateEditor) {
			base.FlushPendingEditActions(updateEditor);
			if (Editor.EditMode != EditMode.Standalone && Editor.IsPopupOpen)
				Editor.ClosePopupOnClick();
		}
		public override bool IsValueChanged {
			get {
				return base.IsValueChanged || Editor.EditMode != EditMode.Standalone && Editor.IsPopupOpen && isCalculatorValueChanged;
			}
		}
	}
}
