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
using System.Threading;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class BindingEditorControlElementBindingEditorViewModel : BindingEditorControlPropertyBasedBindingEditorViewModel {
		Dispatcher dispatcher;
		readonly Func<IBindingEditorControlElement, Thread> updatePropertySelector;
		BindingEditorControlElementSelectorViewModel elementSelector;
		public BindingEditorControlElementBindingEditorViewModel(BindingEditorControlMainViewModel selector)
			: base(selector) {
			dispatcher = Dispatcher.CurrentDispatcher;
			updatePropertySelector = AsyncHelper.Create<IBindingEditorControlElement>(UpdatePropertySelector);
			Header = "ElementName";
			Main.MainControl.ElementsProviderChanged += OnMainControlElementsProviderChanged;
			OnMainControlElementsProviderChanged(Main.MainControl, new ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider>(null, Main.MainControl.ElementsProvider));
		}
		public BindingEditorControlElementSelectorViewModel ElementSelector {
			get {
				if(elementSelector == null)
					CreateElementSelector();
				return elementSelector;
			}
		}
		public Thread UpdatePropertySelectorThread { get; private set; }
		void CreateElementSelector() {
			elementSelector = new BindingEditorControlElementSelectorViewModel();
			elementSelector.SelectedTreeNodeChanged += OnElementSelectorSelectedTreeNodeChanged;
		}
		void OnMainControlElementsProviderChanged(object sender, ThePropertyChangedEventArgs<IBindingEditorControlElementsProvider> e) {
			ElementSelector.ElementsProvider = e.NewValue;
		}
		void OnElementSelectorSelectedTreeNodeChanged(object sender, EventArgs e) {
			IBindingEditorControlElement source = ElementSelector.SelectedTreeNode ?? ElementSelector.RootTreeNode;
			Source = source;
			PropertySelector.PropertiesProvider = null;
			UpdatePropertySelectorThread = updatePropertySelector(source);
		}
		IEnumerable<ManualResetEvent> UpdatePropertySelector(IBindingEditorControlElement selectedElement, CancellationToken cancellationToken) {
			AsyncHelper.DoEvents(dispatcher, DispatcherPriority.ApplicationIdle);
			return new ManualResetEvent[] { AsyncHelper.DoWithDispatcher(dispatcher, () => PropertySelector.PropertiesProvider = selectedElement, DispatcherPriority.ApplicationIdle) };
		}
	}
}
