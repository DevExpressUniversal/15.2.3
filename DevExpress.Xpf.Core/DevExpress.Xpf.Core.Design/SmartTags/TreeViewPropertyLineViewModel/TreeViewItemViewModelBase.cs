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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Design.AssignDataContextDialog {
		public class TreeViewItemViewModel : WpfBindableBase, ITreeViewItemViewModel {
		ITreeViewItemViewModel parent;
		bool isExpanded;
		bool isSelected;
		ITreeViewItemViewModel[] children;
		bool isVisible = true;
		public TreeViewItemViewModel() {
		}
		public ITreeViewItemViewModel[] Children {
			get {
				if (children != null)
					return children;
				children = GetChildren();
				UpdateParent();
				return children;
			}
			set { SetProperty(ref children, value ?? new ITreeViewItemViewModel[0], () => Children, () => UpdateParent()); }
		}
		void UpdateParent() {
			foreach(TreeViewItemViewModel item in Children)
				item.Parent = this;
		}
		protected virtual ITreeViewItemViewModel[] GetChildren() { return new ITreeViewItemViewModel[0]; }
		public virtual ITreeViewItemViewModel Find(Predicate<ITreeViewItemViewModel> predicate) {
			if (predicate == null)
				return null;
			if (predicate(this))
				return this;
			foreach (ITreeViewItemViewModel item in Children) {
				ITreeViewItemViewModel result = item.Find(predicate);
				if (result != null)
					return result;
			}
			return null;
		}
		IEnumerable<ITreeViewItemViewModel> ITreeViewItemViewModel.Children {
			get { return this.Children; }
		}
		public virtual void SetFilter(Predicate<ITreeViewItemViewModel> filter) {
			bool thisFilter = filter(this);
			if(!thisFilter)
				SetFilterRecursive(this, filter);
			if(!thisFilter && Children.Length != 0)
				IsVisible = Children.Any(child => child.IsVisible);
			else
				IsVisible = thisFilter;
		}
		public static void SetFilterRecursive(ITreeViewItemViewModel parent, Predicate<ITreeViewItemViewModel> filter) {
			if(filter == null || parent == null || parent.Children == null)
				return;
			foreach(ITreeViewItemViewModel item in parent.Children)
				item.SetFilter(filter);
		}
		public ITreeViewItemViewModel Parent {
			get {
				return parent;
			}
			set { SetProperty(ref parent, value, () => Parent); }
		}
		public bool IsVisible {
			get { return isVisible; }
			set { SetProperty(ref isVisible, value, () => IsVisible); }
		}
		public bool IsSelected {
			get {
				return isSelected;
			}
			set { SetProperty(ref isSelected, value, () => IsSelected); }
		}
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if (isExpanded == value)
					return;
				if (value && parent != null)
					parent.IsExpanded = value;
				SetProperty(ref isExpanded, value, () => IsExpanded);
			}
		}
		public virtual string DisplayText { get; set; }
		public virtual bool CanSelect { get; set; }
		public virtual ImageSource Image { get; set; }
		public virtual object SelectValue { get; set; }
	}
}
