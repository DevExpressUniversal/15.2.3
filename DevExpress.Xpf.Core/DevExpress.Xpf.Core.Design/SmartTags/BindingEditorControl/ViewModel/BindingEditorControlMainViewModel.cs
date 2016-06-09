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
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Design.UI;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class BindingEditorControlMainViewModel : BindableBase {
		BindingEditorControlBindingEditorViewModel selectedEditor;
		BindingEditorControlDataContextBindingEditorViewModel dataContextBindingEditor;
		BindingEditorControlElementBindingEditorViewModel elementNameBindingEditor;
		BindingEditorControlRelativeSourceBindingEditorViewModel relativeSourceBindingEditor;
		BindingEditorControlResourceBindingEditorViewModel resourceBindingEditorViewModel;
		BindingEditorControlPage selectedPage = BindingEditorControlPage.None;
		WeakEventHandler<EventArgs, EventHandler> selectedEditorChanged;
		public BindingEditorControlMainViewModel(IBindingEditorControl mainControl) {
			MainControl = mainControl;
			MainControl.DefaultPageChanged += OnMainControlDefaultPageChanged;
			OnMainControlDefaultPageChanged(MainControl, new ThePropertyChangedEventArgs<Func<BindingEditorControlPage>>(null, MainControl.DefaultPage));
		}
		void OnMainControlDefaultPageChanged(object sender, ThePropertyChangedEventArgs<Func<BindingEditorControlPage>> e) {
			if(SelectedPage == BindingEditorControlPage.None && e.NewValue != null)
				SelectedPage = e.NewValue();
		}
		public IBindingEditorControl MainControl { get; private set; }
		public BindingEditorControlDataContextBindingEditorViewModel DataContextBindingEditor {
			get {
				if(dataContextBindingEditor == null)
					dataContextBindingEditor = new BindingEditorControlDataContextBindingEditorViewModel(this);
				return dataContextBindingEditor;
			}
		}
		public BindingEditorControlElementBindingEditorViewModel ElementNameBindingEditor {
			get {
				if(elementNameBindingEditor == null)
					elementNameBindingEditor = new BindingEditorControlElementBindingEditorViewModel(this);
				return elementNameBindingEditor;
			}
		}
		public BindingEditorControlRelativeSourceBindingEditorViewModel RelativeSourceBindingEditor {
			get {
				if(relativeSourceBindingEditor == null)
					relativeSourceBindingEditor = new BindingEditorControlRelativeSourceBindingEditorViewModel(this);
				return relativeSourceBindingEditor;
			}
		}
		public BindingEditorControlResourceBindingEditorViewModel ResourceBindingEditorViewModel {
			get {
				if(resourceBindingEditorViewModel == null)
					resourceBindingEditorViewModel = new BindingEditorControlResourceBindingEditorViewModel(this);
				return resourceBindingEditorViewModel;
			}
		}
		public IEnumerable<BindingEditorControlBindingEditorViewModel> BindingEditors {
			get {
				yield return DataContextBindingEditor;
				yield return ElementNameBindingEditor;
				yield return RelativeSourceBindingEditor;
				yield return ResourceBindingEditorViewModel;
			}
		}
		public BindingEditorControlBindingEditorViewModel SelectedEditor {
			get { return selectedEditor; }
			set { SetProperty(ref selectedEditor, value, () => SelectedEditor, OnSelectedEditorChanged); }
		}
		public BindingEditorControlPage SelectedPage {
			get { return selectedPage; }
			set { SetProperty(ref selectedPage, value, () => SelectedPage, UpdateSelectedEditor); }
		}
		public event EventHandler SelectedEditorChanged { add { selectedEditorChanged += value; } remove { selectedEditorChanged -= value; } }
		void OnSelectedEditorChanged() {
			UpdateSelectedPage();
			selectedEditorChanged.SafeRaise(this, EventArgs.Empty);
		}
		void UpdateSelectedEditor() {
			switch(SelectedPage) {
				case BindingEditorControlPage.ElementNameBinding: SelectedEditor = ElementNameBindingEditor; break;
				case BindingEditorControlPage.RelativeSourceBinding: SelectedEditor = RelativeSourceBindingEditor; break;
				case BindingEditorControlPage.ResourceBinding: SelectedEditor = ResourceBindingEditorViewModel; break;
				case BindingEditorControlPage.DataContextBinding: SelectedEditor = DataContextBindingEditor; break;
				default: SelectedEditor = null; break;
			}
		}
		void UpdateSelectedPage() {
			if(SelectedEditor == ElementNameBindingEditor)
				SelectedPage = BindingEditorControlPage.ElementNameBinding;
			else if(SelectedEditor == RelativeSourceBindingEditor)
				SelectedPage = BindingEditorControlPage.RelativeSourceBinding;
			else if(SelectedEditor == ResourceBindingEditorViewModel)
				SelectedPage = BindingEditorControlPage.ResourceBinding;
			else if(SelectedEditor == DataContextBindingEditor)
				SelectedPage = BindingEditorControlPage.DataContextBinding;
			else
				SelectedPage = BindingEditorControlPage.None;
		}
	}
	public enum BindingEditorControlPage { None, DataContextBinding, ElementNameBinding, RelativeSourceBinding, ResourceBinding }
	public interface IBindingEditorControl {
		Func<BindingEditorControlPage> DefaultPage { get; }
		IBindingEditorControlResourcesProvider ResourcesProvider { get; }
		IBindingEditorControlRelativeSourceProvider RelativeSourceProvider { get; }
		IBindingEditorControlElementsProvider ElementsProvider { get; }
		IBindingEditorControlDataContextProvider DataContextProvider { get; }
		IBindingEditorControlBindingSettingsProvider BindingSettingsProvider { get; }
		BindingDescription Binding { get; set; }
		ICommand CloseCommand { get; }
		object CloseCommandParameter { get; }
		event EventHandler<ThePropertyChangedEventArgs<Func<BindingEditorControlPage>>> DefaultPageChanged;
		event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider>> ResourcesProviderChanged;
		event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlRelativeSourceProvider>> RelativeSourceProviderChanged;
		event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider>> ElementsProviderChanged;
		event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlDataContextProvider>> DataContextProviderChanged;
		event EventHandler<ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider>> BindingSettingsProviderChanged;
	}
}
