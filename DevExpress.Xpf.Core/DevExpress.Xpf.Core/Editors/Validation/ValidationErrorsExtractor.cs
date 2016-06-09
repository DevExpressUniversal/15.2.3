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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Media;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections.Specialized;
using DevExpress.Data.Utils;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Validation.Native;
#else
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
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
#endif
namespace DevExpress.Xpf.Editors {
#if !SL
	class ValidationErrorsExtractor : DependencyObject {
#else
	partial class ValidationErrorsExtractor : FrameworkElement {
#endif
		public static readonly DependencyProperty ValidationErrorsProperty;
		static ValidationErrorsExtractor() {
			Type ownerType = typeof(ValidationErrorsExtractor);
			ValidationErrorsProperty = DependencyPropertyManager.Register("ValidationErrors", typeof(INotifyCollectionChanged), ownerType,
				new PropertyMetadata(ValidationErrorsPropertyChanged));
		}
		static void ValidationErrorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ValidationErrorsExtractor extractor = (ValidationErrorsExtractor)d;
			extractor.UpdateValidationErrors((INotifyCollectionChanged)e.OldValue, (INotifyCollectionChanged)e.NewValue);
		}
		CollectionChangedWeakEventHandler<ValidationErrorsExtractor> CollectionChangedEventManager { get; set; }
		IList<ValidationError> ValidationErrorsInternal { get { return (IList<ValidationError>)GetValue(ValidationErrorsProperty); } }
		BaseEdit Editor { get; set; }
		BaseValidationError error;
		public BaseValidationError ValidationError { 
			get {
				IList<ValidationError> errors = ValidationErrorsInternal;
				if(errors == null || errors.Count == 0)
					return null;
				ValidationError validationError = errors[0];
				if(error == null)
					error = new BaseValidationError(validationError.ErrorContent, validationError.Exception, ErrorType.Critical);
				if (!object.Equals(validationError.ErrorContent, error.ErrorContent))
					error = new BaseValidationError(validationError.ErrorContent, validationError.Exception, ErrorType.Critical);
				return error;
			} 
		}
		public FrameworkElement GetValidationToolTip(bool isToolTip) {
			return Editor.EditStrategy.CreateValidationToolTip(ValidationError, isToolTip);
		}
		static Action<ValidationErrorsExtractor, object, NotifyCollectionChangedEventArgs> reset = (owner, o, r) => owner.ResetErrorProvider();
		public ValidationErrorsExtractor(BaseEdit editor) {
			Editor = editor;
			CollectionChangedEventManager = new CollectionChangedWeakEventHandler<ValidationErrorsExtractor>(this, reset);
#if SL
			ConstructorSLPart();
#endif
		}
		public void UpdateValidationErrors(INotifyCollectionChanged oldErrors, INotifyCollectionChanged errors) {
			if (oldErrors != null)
				oldErrors.CollectionChanged -= CollectionChangedEventManager.Handler;
			if(errors != null)
				errors.CollectionChanged += CollectionChangedEventManager.Handler; 
			ResetErrorProvider();
		}
		void ResetErrorProvider() {
			Editor.EditStrategy.ResetErrorProvider();
		}
		public INotifyCollectionChanged ValidationErrors {
			get { return (INotifyCollectionChanged)GetValue(ValidationErrorsProperty); }
			set { SetValue(ValidationErrorsProperty, value); }
		}
	}
}
