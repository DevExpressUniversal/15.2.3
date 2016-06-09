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

using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Text;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class BindingEditorControlPropertySelectorViewModel : MvvmControlTreeNodeSelectorViewModel<IBindingEditorControlProperty, BindingEditorControlPropertyViewModel, BindingEditorControlPropertySelectorViewModel> {
		IBindingEditorControlBindingSource propertiesProvider;
		string path;
		WeakEventHandler<EventArgs, EventHandler> pathChanged;
		public BindingEditorControlPropertySelectorViewModel(IBindingEditorControlPropertySelectorOwner owner) {
			Owner = owner;
		}
		public IBindingEditorControlPropertySelectorOwner Owner { get; private set; }
		public IBindingEditorControlBindingSource PropertiesProvider {
			get { return propertiesProvider; }
			set { SetProperty(ref propertiesProvider, value, () => PropertiesProvider, OnPropertiesProviderChanged); }
		}
		public string Path {
			get { return path; }
			set { SetProperty(ref path, value, () => Path, () => pathChanged.SafeRaise(this, EventArgs.Empty)); }
		}
		public event EventHandler PathChanged { add { pathChanged += value; } remove { pathChanged -= value; } }
		protected override BindingEditorControlPropertyViewModel CreateTreeNodeViewModelCore(BindingEditorControlPropertyViewModel parent) {
			return new BindingEditorControlPropertyViewModel(parent, this);
		}
		void OnPropertiesProviderChanged() {
			RootTreeNode = PropertiesProvider == null ? null : PropertiesProvider.RootProperty;
		}
		protected override void OnSelectedTreeNodeChanged() {
			base.OnSelectedTreeNodeChanged();
			Path = GetPath(SelectedTreeNode);
		}
		string GetPath(IBindingEditorControlProperty selectedProperty) {
			if(selectedProperty == null) return string.Empty;
			StringBuilder path = new StringBuilder();
			bool first = true;
			for(var property = selectedProperty; property != null; property = property.Parent) {
				if(string.IsNullOrEmpty(property.PropertyName)) continue;
				if(!first)
					path.Insert(0, '.');
				else
					first = false;
				path.Insert(0, property.PropertyName);
			}
			return path.ToString();
		}
	}
	public interface IBindingEditorControlPropertySelectorOwner {
		void Done();
	}
}
