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
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class NodeSelectorControlBase<TMainViewModel, TMainView> : MvvmControlBase<TMainViewModel, TMainView>, INodeSelectorControlBase where TMainView : UserControl, new() {
		#region Dependency Properties
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(INodeSelectorItem), typeof(NodeSelectorControlBase<TMainViewModel, TMainView>), new PropertyMetadata(null,
				(d, e) => ((NodeSelectorControlBase<TMainViewModel, TMainView>)d).OnSelectedItemChanged(e)));
		public static readonly DependencyProperty CloseCommandProperty =
			DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(NodeSelectorControlBase<TMainViewModel, TMainView>), new PropertyMetadata(null));
		public static readonly DependencyProperty CloseCommandParameterProperty =
			DependencyProperty.Register("CloseCommandParameter", typeof(object), typeof(NodeSelectorControlBase<TMainViewModel, TMainView>), new PropertyMetadata(null));
		#endregion
		WeakEventHandler<ThePropertyChangedEventArgs<INodeSelectorDataProvider>, EventHandler<ThePropertyChangedEventArgs<INodeSelectorDataProvider>>> dataProviderChanged;
		WeakEventHandler<EventArgs, EventHandler> selectedItemChanged;
		public INodeSelectorItem SelectedItem { get { return (INodeSelectorItem)GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
		public ICommand CloseCommand { get { return (ICommand)GetValue(CloseCommandProperty); } set { SetValue(CloseCommandProperty, value); } }
		public object CloseCommandParameter { get { return GetValue(CloseCommandParameterProperty); } set { SetValue(CloseCommandParameterProperty, value); } }
		public event EventHandler SelectedItemChanged { add { selectedItemChanged += value; } remove { selectedItemChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<INodeSelectorDataProvider>> DataProviderChanged { add { dataProviderChanged += value; } remove { dataProviderChanged -= value; } }
		protected virtual void OnDataProviderChanged(DependencyPropertyChangedEventArgs e) {
			dataProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<INodeSelectorDataProvider>(e));
		}
		protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e) {
			selectedItemChanged.SafeRaise(this, EventArgs.Empty);
		}
	}
}
