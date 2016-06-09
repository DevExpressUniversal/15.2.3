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

using DevExpress.Design.UI;
using System;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class BindingEditorControlPropertyViewModel : MvvmControlTreeNodeViewModel<IBindingEditorControlProperty, BindingEditorControlPropertyViewModel, BindingEditorControlPropertySelectorViewModel> {
		ICommand selectAndDoneCommand;
		public BindingEditorControlPropertyViewModel(BindingEditorControlPropertyViewModel parent, BindingEditorControlPropertySelectorViewModel selector) : base(parent, selector, MvvmControlFilterStrategy.Normal) { }
		public bool Highlighted { get { return TreeNode != null ? TreeNode.Highlighted : false; } }
		public string PropertyName { get { return TreeNode != null ? TreeNode.PropertyName : null; } }
		public string PropertyType { get { return TreeNode != null ? TreeNode.ShortPropertyType : null; } }
		public bool IsClass { get { return TreeNode != null ? TreeNode.IsClass : false; } }
		public string PropertyNameToolTip { get; private set; }
		public string PropertyTypeToolTip { get; private set; }
		public ICommand SelectAndDoneCommand {
			get {
				if(selectAndDoneCommand == null)
					selectAndDoneCommand = new WpfDelegateCommand(SelectAndDone);
				return selectAndDoneCommand;
			}
		}
		public void SelectAndDone() {
			if(!CanBeSelected) {
				IsExpanded = !IsExpanded;
				return;
			}
			Selector.SelectedTreeNode = TreeNode;
			Selector.Owner.Done();
		}
		protected override void OnTreeNodeChanged() {
			base.OnTreeNodeChanged();
			PropertyNameToolTip = TreeNode.PropertyType;
			PropertyTypeToolTip = TreeNode.IsPOCO ? TreeNode.PropertyType : null;
			RaisePropertyChanged(() => Highlighted);
			RaisePropertyChanged(() => PropertyName);
			RaisePropertyChanged(() => IsClass);
			RaisePropertyChanged(() => PropertyNameToolTip);
			RaisePropertyChanged(() => PropertyTypeToolTip);
		}
		protected override int CompareTreeNodes(IBindingEditorControlProperty t1, IBindingEditorControlProperty t2) {
			return string.Compare(t1.PropertyName, t2.PropertyName, StringComparison.Ordinal);
		}
		protected override bool GetIsVisible(string filter) {
			IBindingEditorControlProperty element = TreeNode;
			string propertyName = element == null ? null : element.PropertyName;
			return string.IsNullOrEmpty(propertyName) || propertyName.ToLowerInvariant().Contains(filter);
		}
	}
}
