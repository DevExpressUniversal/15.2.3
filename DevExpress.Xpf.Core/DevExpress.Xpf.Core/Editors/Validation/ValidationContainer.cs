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

using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
#if !SL
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Validation;
#else
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.WPFToSLUtils;
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
#endif
namespace DevExpress.Xpf.Editors {
	public class ValidationService : DependencyObject {
		#region inner classes
		class EditorsCache {
			List<WeakReference> editors = new List<WeakReference>();
			public bool HasElements { get { return editors.Count > 0; } }
			public EditorsCache() {
			}
			public void Add(DependencyObject d) {
				if(Contains(d))
					return;
				editors.Add(new WeakReference(d));
			}
			public void Remove(DependencyObject d) {
				WeakReference result = Find(d);
				if(result == null)
					return;
				editors.Remove(result);
			}
			public bool Contains(DependencyObject d) {
				WeakReference result = Find(d);
				return result != null;
			}
			public WeakReference Find(DependencyObject d) {
				Flush();
				return editors.FirstOrDefault(reference => object.ReferenceEquals(reference.Target, d));
			}
			public ReadOnlyObservableCollection<BaseValidationError> GetErrors() {
				ObservableCollection<BaseValidationError> errors = new ObservableCollection<BaseValidationError>();
				foreach(WeakReference editor in editors) {
					DependencyObject dEditor = (DependencyObject)editor.Target;
					if (dEditor != null)
						errors.Add(BaseEdit.GetValidationError(dEditor));
				}
				return new ReadOnlyObservableCollection<BaseValidationError>(errors);
			}
			public void Flush() {
				for (int i = editors.Count - 1; i >= 0; i--) {
					object editor = editors[i].Target;
					if (editor == null)
						editors.RemoveAt(i);
				}
			}
		}
		#endregion
		public static readonly DependencyProperty IsValidationContainerProperty;
		static readonly DependencyPropertyKey ValidationServicePropertyKey;
		public static readonly DependencyProperty ValidationServiceProperty;
		static readonly DependencyPropertyKey HasValidationErrorPropertyKey;
		public static readonly DependencyProperty HasValidationErrorProperty;
		static readonly DependencyPropertyKey ValidationErrorsPropertyKey;
		public static readonly DependencyProperty ValidationErrorsProperty;
		static ValidationService() {
			Type ownerType = typeof(ValidationService);
			IsValidationContainerProperty = DependencyPropertyManager.RegisterAttached("IsValidationContainer", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, IsValidationContainerPropertyChanged));
			ValidationServicePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ValidationService", typeof(ValidationService), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, ValidationServicePropertyChanged));
			ValidationServiceProperty = ValidationServicePropertyKey.DependencyProperty;
			HasValidationErrorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("HasValidationError", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, HasValidationErrorPropertyChanged));
			HasValidationErrorProperty = HasValidationErrorPropertyKey.DependencyProperty;
			ValidationErrorsPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ValidationErrors", typeof(ReadOnlyObservableCollection<BaseValidationError>), ownerType,
				new FrameworkPropertyMetadata(null));
			ValidationErrorsProperty = ValidationErrorsPropertyKey.DependencyProperty;
		}
		public static bool GetIsValidationContainer(DependencyObject d) {
			return (bool)d.GetValue(IsValidationContainerProperty);
		}
		public static void SetIsValidationContainer(DependencyObject d, bool service) {
			d.SetValue(IsValidationContainerProperty, service);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static ValidationService GetValidationService(DependencyObject d) {
			return (ValidationService)DependencyObjectHelper.GetValueWithInheritance(d, ValidationServiceProperty);
		}
		static void SetValidationService(DependencyObject d, ValidationService service) {
			d.SetValue(ValidationServicePropertyKey, service);
		}
		public static bool GetHasValidationError(DependencyObject d) {
			return (bool)d.GetValue(HasValidationErrorProperty);
		}
		static void SetHasValidationError(DependencyObject d, bool value) {
			d.SetValue(HasValidationErrorPropertyKey, value);
		}
		public static ReadOnlyObservableCollection<BaseValidationError> GetValidationErrors(DependencyObject d) {
			return (ReadOnlyObservableCollection<BaseValidationError>)d.GetValue(ValidationErrorsProperty);
		}
		internal static void SetValidationErrors(DependencyObject d, ReadOnlyObservableCollection<BaseValidationError> value) {
			d.SetValue(ValidationErrorsPropertyKey, value);
		}
		static void IsValidationContainerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			bool isContainer = (bool)e.NewValue;
			if(isContainer)
				SetValidationService(d, new ValidationService() { Owner = d });
			else
				d.ClearValue(ValidationServicePropertyKey);
		}
		static void HasValidationErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static void ValidationServicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null) {
				ValidationService service = (ValidationService)e.OldValue;
				service.RemoveEditor(d);
			}
			if(e.NewValue != null) {
				ValidationService service = (ValidationService)e.NewValue;
				service.AddEditor(d);
			}
		}
		WeakReference owner;
		readonly EditorsCache editorsCache = new EditorsCache();
		DependencyObject Owner {
			get { return (DependencyObject)owner.Target; }
			set { owner = new WeakReference(value); }
		}
		public ValidationService() {
		}
		void UpdateErrors() {
			if(editorsCache.HasElements) {
				SetHasValidationError(Owner, editorsCache.HasElements);
				SetValidationErrors(Owner, editorsCache.GetErrors());
			}
			else {
				Owner.ClearValue(HasValidationErrorPropertyKey);
				Owner.ClearValue(ValidationErrorsPropertyKey);
			}
		}
		public void AddEditor(DependencyObject d) {
			if(BaseEdit.GetHasValidationError(d)) {
				editorsCache.Add(d);
				UpdateErrors();
			}
		}
		public void RemoveEditor(DependencyObject d) {
			editorsCache.Remove(d);
			UpdateErrors();
		}
		public void UpdateEditor(DependencyObject d) {
			editorsCache.Remove(d);
			AddEditor(d);
		}
	}
}
