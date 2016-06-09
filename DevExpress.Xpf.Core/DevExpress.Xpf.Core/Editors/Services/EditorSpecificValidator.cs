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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Data.Filtering;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Collections.Specialized;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core;
using System.Windows.Media;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
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
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using System.Collections.Specialized;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Editors.Services {
	public class EditorSpecificValidator : BaseEditBaseService {
		StrategyValidatorBase validator;
		protected StrategyValidatorBase Validator { get { return validator ?? (validator = CreateValidator()); } }
		public EditorSpecificValidator(BaseEdit editor) : base(editor) {
		}
		public bool DoValidate(object value, object convertedValue, UpdateEditorSource source) {
			return Validator.DoValidate(value, convertedValue, source);
		}
		protected virtual StrategyValidatorBase CreateValidator() {
			return new StrategyValidatorBase(OwnerEdit);
		}
		public object GetValidationError() {
			return Validator.GetValidationError();
		}
		public object ProcessConversion(object value) {
			return Validator.ProcessConversion(value);
		}
	}
	public class TextEditorValidator : EditorSpecificValidator {
		public TextEditorValidator(TextEditBase editor) : base(editor) {
		}
		protected override StrategyValidatorBase CreateValidator() {
			return new TextEditValueValidator((TextEditBase)OwnerEdit);
		}
	}
	public class LookUpEditValidator : EditorSpecificValidator {
		LookUpEditStrategyBase Strategy { get; set; }
		SelectorPropertiesCoercionHelper CoercionHelper { get; set; }
		public LookUpEditValidator(LookUpEditStrategyBase strategy, SelectorPropertiesCoercionHelper coercionHelper, LookUpEditBase editor) : base(editor) {
			Strategy = strategy;
			CoercionHelper = coercionHelper;
		}
		protected override StrategyValidatorBase CreateValidator() {
			return new LookUpEditStrategyValidator(Strategy, CoercionHelper, (LookUpEditBase)OwnerEdit);
		}
	}
	public class PopupColorEditValidator : EditorSpecificValidator {
		public PopupColorEditValidator(PopupColorEdit editor)
			: base(editor) {
		}
		protected override StrategyValidatorBase CreateValidator() {
			return new Validation.Native.PopupColorEditValidator((PopupColorEdit)OwnerEdit);
		}
	}
#if !SL
	public class PopupBrushEditValidator : EditorSpecificValidator {
		public PopupBrushEditValidator(PopupBrushEditBase editor) : base(editor) {
		}
		protected override StrategyValidatorBase CreateValidator() {
			return new Validation.Native.PopupBrushEditValidator((PopupBrushEditBase)OwnerEdit);
		}
	}
#endif
	public class SpinEditValidator : EditorSpecificValidator {
		public SpinEditValidator(BaseEdit editor)
			: base(editor) {
		}
		protected override StrategyValidatorBase CreateValidator() {
			return new RangedValueValidator<decimal>((TextEdit)OwnerEdit);
		}
	}
	public class DateEditValidator : EditorSpecificValidator {
		public DateEditValidator(BaseEdit editor)
			: base(editor) {
		}
		protected override StrategyValidatorBase CreateValidator() {
			return new RangedValueValidator<DateTime>((TextEdit)OwnerEdit);
		}
	}
}
