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

using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class BindingEditorControlResourceBindingEditorViewModel : BindingEditorControlPropertyBasedBindingEditorViewModel {
		IEnumerable<BindingEditorControlResourceViewModel> resources;
		IBindingEditorControlResource selectedResource;
		public BindingEditorControlResourceBindingEditorViewModel(BindingEditorControlMainViewModel selector)
			: base(selector) {
			Header = "StaticResource";
			Main.MainControl.ResourcesProviderChanged += OnMainControlResourcesProviderChanged;
			OnMainControlResourcesProviderChanged(Main.MainControl, new ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider>(null, Main.MainControl.ResourcesProvider));
		}
		public IEnumerable<BindingEditorControlResourceViewModel> Resources {
			get { return resources; }
			private set { SetProperty(ref resources, value, () => Resources); }
		}
		public IBindingEditorControlResource SelectedResource {
			get { return selectedResource; }
			set { SetProperty(ref selectedResource, value, () => SelectedResource, OnSelectedResourceChanged); }
		}
		void OnMainControlResourcesProviderChanged(object sender, ThePropertyChangedEventArgs<IBindingEditorControlResourcesProvider> e) {
			Resources = e.NewValue == null ? null : e.NewValue.GetResources().Select(r => new BindingEditorControlResourceViewModel(r)).ToArray();
		}
		void OnSelectedResourceChanged() {
			Source = SelectedResource;
			PropertySelector.PropertiesProvider = Source;
		}
	}
	public class BindingEditorControlResourceViewModel {
		public BindingEditorControlResourceViewModel(IBindingEditorControlResource resource) {
			Resource = resource;
		}
		public IBindingEditorControlResource Resource { get; private set; }
		public string Key { get { return Resource.Key; } }
	}
}
