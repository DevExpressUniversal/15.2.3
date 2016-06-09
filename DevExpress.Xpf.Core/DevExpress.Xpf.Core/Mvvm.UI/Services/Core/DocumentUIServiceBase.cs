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

using DevExpress.Mvvm.Native;
using System;
using System.Globalization;
using System.Windows;
using System.ComponentModel;
#if !NETFX_CORE
using DevExpress.Mvvm.UI.Native;
using System.Windows.Data;
#else
#if FREE
using DevExpress.Mvvm.UI.Native;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif
#if !MVVM
using DevExpress.Data.Utils;
#endif
namespace DevExpress.Mvvm.UI {
	public abstract class DocumentUIServiceBase : ViewServiceBase {
#if !SILVERLIGHT && !NETFX_CORE
#if !FREE
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
#endif
		static readonly DependencyProperty TitleListenProperty =
			DependencyProperty.RegisterAttached("DocumentTitleListen", typeof(object), typeof(DocumentUIServiceBase), new PropertyMetadata(null, (d, e) => ViewModelExtensions.SetDocumentTitle(d, e.NewValue)));
#endif
		public static readonly DependencyProperty DocumentProperty =
			DependencyProperty.RegisterAttached("Document", typeof(IDocument), typeof(DocumentUIServiceBase), new PropertyMetadata(null));
		public static IDocument GetDocument(DependencyObject obj) {
			return (IDocument)obj.GetValue(DocumentProperty);
		}
		public static void SetDocument(DependencyObject obj, IDocument value) {
			obj.SetValue(DocumentProperty, value);
		}
		public static void CheckDocumentAccess(bool isDocumentAlive) {
			if(!isDocumentAlive)
				throw new InvalidOperationException("Cannot access the destroyed document.");
		}
		public static void SetTitleBinding(object documentContentView, DependencyProperty property, DependencyObject target, bool convertToString = false) {
#if !SILVERLIGHT && !NETFX_CORE
			BindingOperations.SetBinding(target, TitleListenProperty, new Binding() { Source = target, Path = new PropertyPath(property), Mode = BindingMode.OneWay });
#endif
			object viewModel = ViewHelper.GetViewModelFromView(documentContentView);
			if(!DocumentViewModelHelper.IsDocumentContentOrDocumentViewModel(viewModel)) return;
			if(DocumentViewModelHelper.TitlePropertyHasImplicitImplementation(viewModel)) {
				Binding binding = new Binding() { Path = new PropertyPath("Title"), Source = viewModel };
#if !SILVERLIGHT && !NETFX_CORE
				if(convertToString)
					binding.Converter = new ObjectToStringConverter();
#endif
				BindingOperations.SetBinding(target, property, binding);
			} else {
				new TitleUpdater(convertToString, viewModel, target, property).Update(target, viewModel);
			}
		}
		public static void CloseDocument(IDocumentManagerService documentManagerService, IDocumentContent documentContent, bool force) {
			IDocument document = documentManagerService.FindDocument(documentContent);
			if(document != null)
				document.Close(force);
		}
		class TitleUpdater : IDisposable {
			#region Dependency Properties
			static readonly DependencyProperty TitleUpdaterInternalProperty =
				DependencyProperty.RegisterAttached("TitleUpdaterInternal", typeof(TitleUpdater), typeof(TitleUpdater), new PropertyMetadata(null));
			#endregion
			bool convertToString;
			DependencyProperty targetProperty;
			PropertyChangedWeakEventHandler<DependencyObject> updateHandler;
			WeakReference documentContentRef;
			public TitleUpdater(bool convertToString, object documentContentOrDocumentViewModel, DependencyObject target, DependencyProperty targetProperty) {
				TitleUpdater oldUpdater = (TitleUpdater)target.GetValue(TitleUpdaterInternalProperty);
				if(oldUpdater != null)
					oldUpdater.Dispose();
				this.convertToString = convertToString;
				target.SetValue(TitleUpdaterInternalProperty, this);
				this.targetProperty = targetProperty;
				this.updateHandler = new PropertyChangedWeakEventHandler<DependencyObject>(target, OnDocumentViewModelPropertyChanged);
				INotifyPropertyChanged notify = documentContentOrDocumentViewModel as INotifyPropertyChanged;
				DocumentContent = notify;
				if(notify != null)
					notify.PropertyChanged += this.updateHandler.Handler;
			}
			public void Dispose() {
				INotifyPropertyChanged notify = DocumentContent;
				if(notify != null)
					notify.PropertyChanged -= this.updateHandler.Handler;
				this.updateHandler = null;
				DocumentContent = null;
			}
			public void Update(DependencyObject target, object documentContentOrDocumentViewModel) {
				object title = DocumentViewModelHelper.GetTitle(documentContentOrDocumentViewModel);
#if !SILVERLIGHT && !NETFX_CORE
				if(convertToString)
					title = title == null ? string.Empty : title.ToString();
#endif
				target.SetValue(targetProperty, title);
			}
			INotifyPropertyChanged DocumentContent {
				get { return documentContentRef == null ? null : (INotifyPropertyChanged)documentContentRef.Target; }
				set { documentContentRef = value == null ? null : new WeakReference(value); }
			}
			static void OnDocumentViewModelPropertyChanged(DependencyObject target, object sender, PropertyChangedEventArgs e) {
				if(e.PropertyName != "Title") return;
				TitleUpdater updater = (TitleUpdater)target.GetValue(TitleUpdaterInternalProperty);
				updater.Update(target, sender);
			}
		}
#if !SILVERLIGHT && !NETFX_CORE
		class ObjectToStringConverter : IValueConverter {
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				return value == null ? string.Empty : value.ToString();
			}
			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				throw new NotSupportedException();
			}
		}
#endif
	}
}
