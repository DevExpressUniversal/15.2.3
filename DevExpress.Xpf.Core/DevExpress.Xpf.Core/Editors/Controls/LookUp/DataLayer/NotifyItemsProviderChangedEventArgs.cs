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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors.Helpers {
	public class NotifyItemsProviderSelectionChangedEventArgs : EventArgs {
		public bool IsSelected { get; private set; }
		public ListBoxItem Item { get; private set; }
		public NotifyItemsProviderSelectionChangedEventArgs(ListBoxItem item, bool isSelected) {
			Item = item;
			IsSelected = isSelected;
		}
	}
	public class NotifyItemsProviderChangedEventArgs : EventArgs {
		public int NewIndex { get; private set; }
		public object Item { get; private set; }
		public ListChangedType ChangedType { get; private set; }
		protected NotifyItemsProviderChangedEventArgs(ListChangedType changedType, object item, int index) {
			NewIndex = index;
			Item = item;
			ChangedType = changedType;
		}
		public NotifyItemsProviderChangedEventArgs(ListChangedType changedType, object item)
			: this(changedType, item, -1) {
		}
		public NotifyItemsProviderChangedEventArgs(ListChangedType changedType, int newIndex)
			: this(changedType, null, newIndex) {
		}
	}
}
