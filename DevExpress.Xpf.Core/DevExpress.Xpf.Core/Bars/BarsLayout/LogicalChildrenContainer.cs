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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Bars.Internal {
	public class LogicalChildrenContainer : ILogicalChildrenContainer2 {
		ILogicalOwner Owner { get; set; }
		LinkedList<object> Children { get; set; }
		public LogicalChildrenContainer(ILogicalOwner owner) {
			Owner = owner;
			Children = new LinkedList<object>();
		}
		public void AddLogicalChild(object child) {
			Owner.AddChild(child);
			Children.AddLast(child);
		}
		public void RemoveLogicalChild(object child) {
			Children.Remove(child);
			Owner.RemoveChild(child);			
		}
		public IEnumerator GetEnumerator() {
			return Children.GetEnumerator();
		}
		public void ProcessChildrenChanged(NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add)
				e.NewItems.Cast<object>().ForEach(AddLogicalChild);
			else if (e.Action == NotifyCollectionChangedAction.Remove)
				e.OldItems.Cast<object>().ForEach(RemoveLogicalChild);
			else if (e.Action == NotifyCollectionChangedAction.Replace) {
				e.OldItems.Cast<object>().ForEach(RemoveLogicalChild);
				e.NewItems.Cast<object>().ForEach(AddLogicalChild);
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset) {
				Reset();
			}
		}
		public void Reset() {
			foreach (var child in Children)
				Owner.RemoveChild(child);
			Children.Clear();
		}
	}
}
