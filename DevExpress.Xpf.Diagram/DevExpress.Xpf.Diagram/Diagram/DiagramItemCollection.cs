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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
namespace DevExpress.Xpf.Diagram {
	public class DiagramItemCollection : ObservableCollection<DiagramItem> {
		class PanelItemsHost : IItemsHost {
			internal readonly Canvas Panel;
			public PanelItemsHost(Canvas panel) {
				this.Panel = panel;
			}
			void IItemsHost.Add(IDiagramItem item) {
				Panel.Children.Add((DiagramItem)item);
			}
			void IItemsHost.Insert(int index, IDiagramItem item) {
				Panel.Children.Insert(index, (DiagramItem)item);
			}
			void IItemsHost.Remove(IDiagramItem item) {
				Panel.Children.Remove((DiagramItem)item);
			}
		}
		readonly ItemCollectionController<DiagramItem> controller;
#if DEBUGTEST
		internal Panel ItemsPanelForTests { get { return ((PanelItemsHost)controller.ItemsHostsForTests).Panel; } }
#endif
		internal DiagramItemCollection(DiagramItem ownerItem) {
			this.controller = new ItemCollectionController<DiagramItem>(ownerItem, this);
		}
		protected override void InsertItem(int index, DiagramItem item) {
			controller.InsertItem(index, item, () => base.InsertItem(index, item));
		}
		protected override void MoveItem(int oldIndex, int newIndex) {
			controller.MoveItem(oldIndex, newIndex, () => base.MoveItem(oldIndex, newIndex));
		}
		protected override void SetItem(int index, DiagramItem item) {
			controller.SetItem(index, item, () => base.SetItem(index, item));
		}
		protected override void RemoveItem(int index) {
			controller.RemoveItem(index, () => base.RemoveItem(index));
		}
		protected override void ClearItems() {
			controller.ClearItems(() => base.ClearItems());
		}
		internal void SetHost(Canvas panel) {
			controller.SetHost(panel.With(x => new PanelItemsHost(x)));
		}
	}
}
