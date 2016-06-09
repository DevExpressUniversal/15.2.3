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

extern alias Platform;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Threading;
using Guard = Platform::DevExpress.Utils.Guard;
using DevExpress.Mvvm;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class TreeViewPropertyLinePopupViewModelBase : PropertyLineWithPopupEditorPopupViewModel {
		INodeSelectorItem selectedNode;
		public TreeViewPropertyLinePopupViewModelBase(PropertyLineWithPopupEditorViewModel propertyLine) : base(propertyLine) { }
		public INodeSelectorItem SelectedNode {
			get { return selectedNode; }
			set { SetProperty(ref selectedNode, value, () => SelectedNode, OnSelectedNodeChanged); }
		}
		protected virtual void OnSelectedNodeChanged() { }
	}
	public abstract class TreeViewPropertyLinePopupViewModelDataProvider : BindableBase, INodeSelectorDataProvider {
		bool loaded = false;
		INodeSelectorItem root;
		WeakEventHandler<EventArgs, EventHandler> rootAsyncChanged;
		INodeSelectorItem defaultSelectedItem;
		WeakEventHandler<EventArgs, EventHandler> defaultSelectedItemAsyncChanged;
		Dispatcher dispatcher;
		public TreeViewPropertyLinePopupViewModelDataProvider() {
			dispatcher = Dispatcher.CurrentDispatcher;
		}
		public INodeSelectorItem Root {
			get { return root; }
			protected set {
				if(SetProperty(ref root, value, () => Root))
					AsyncHelper.DoWithDispatcher(dispatcher, () => rootAsyncChanged.SafeRaise(this, EventArgs.Empty));
			}
		}
		public INodeSelectorItem DefaultSelectedItem {
			get { return defaultSelectedItem; }
			protected set {
				if(SetProperty(ref defaultSelectedItem, value, () => DefaultSelectedItem))
					AsyncHelper.DoWithDispatcher(dispatcher, () => defaultSelectedItemAsyncChanged.SafeRaise(this, EventArgs.Empty));
			}
		}
		public event EventHandler RootAsyncChanged { add { rootAsyncChanged += value; } remove { rootAsyncChanged -= value; } }
		public event EventHandler DefaultSelectedItemAsyncChanged { add { defaultSelectedItemAsyncChanged += value; } remove { defaultSelectedItemAsyncChanged -= value; } }
		public void Load() {
			if(loaded) return;
			loaded = true;
			UpdateAsyncFunc(false);
		}
		public void Reset() {
			if(!loaded) return;
			loaded = false;
			UpdateAsyncFunc(true);
		}
		protected Func<bool, Thread> UpdateAsyncFunc { get; set; }
		INodeSelectorItem INodeSelectorDataProvider.Root { get { return Root; } }
		event EventHandler INodeSelectorDataProvider.RootChanged { add { RootAsyncChanged += value; } remove { RootAsyncChanged -= value; } }
		INodeSelectorItem INodeSelectorDataProvider.DefaultSelectedItem { get { return DefaultSelectedItem; } }
		event EventHandler INodeSelectorDataProvider.DefaultSelectedItemChanged { add { DefaultSelectedItemAsyncChanged += value; } remove { DefaultSelectedItemAsyncChanged -= value; } }
	}
}
