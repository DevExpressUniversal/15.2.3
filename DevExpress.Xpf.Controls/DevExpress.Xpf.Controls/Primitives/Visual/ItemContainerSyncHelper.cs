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

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Controls.Primitives {
	class ItemContainerSyncHelper {
		readonly ItemContainerGenerator Generator;
		IItemContainerGenerator IGenerator { get { return ((IItemContainerGenerator)Generator); } }
		Panel Panel;
		public ItemContainerSyncHelper(ItemContainerGenerator generator) {
			Generator = generator;
			Generator.ItemsChanged += OnItemsChanged;
		}
		public void Attach(Panel panel) {
			Panel = panel;
			ReGenerateChildren();
		}
		public void Detach(Panel panel) {
			Panel = null;
			panel.Children.Clear();
		}
		public void Reset() {
			ReGenerateChildren();
		}
		void OnItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(Panel == null) return;
			OnItemsChangedInternal(e);
		}
		private void OnItemsChangedInternal(ItemsChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					AddChildren(e.Position, e.ItemCount);
					return;
				case NotifyCollectionChangedAction.Remove:
					RemoveChildren(e.Position, e.ItemUICount);
					return;
				case NotifyCollectionChangedAction.Replace:
					ReplaceChildren(e.Position, e.ItemCount);
					return;
				case NotifyCollectionChangedAction.Move:
					MoveChildren(e.OldPosition, e.Position, e.ItemUICount);
					return;
				case NotifyCollectionChangedAction.Reset:
					ReGenerateChildren();
					return;
			}
			Panel.InvalidateMeasure();
		}
		private void AddChildren(GeneratorPosition pos, int itemCount) {
			using(IGenerator.StartAt(pos, GeneratorDirection.Forward)) {
				for(int i = 0; i < itemCount; i++) {
					UIElement element = IGenerator.GenerateNext() as UIElement;
					if(element != null) {
						Panel.Children.Insert((pos.Index + 1) + i, element);
						IGenerator.PrepareItemContainer(element);
					}
				}
			}
		}
		private void RemoveChildren(GeneratorPosition pos, int containerCount) {
			Panel.Children.RemoveRange(pos.Index, containerCount);
		}
		private void ReplaceChildren(GeneratorPosition pos, int itemCount) {
			using(IGenerator.StartAt(pos, GeneratorDirection.Forward)) {
				for(int i = 0; i < itemCount; i++) {
					UIElement item = IGenerator.GenerateNext() as UIElement;
					if(item != null) {
						Panel.Children[(pos.Index + 1) + i] = item;
						IGenerator.PrepareItemContainer(item);
					}
				}
			}
		}
		private void MoveChildren(GeneratorPosition fromPos, GeneratorPosition toPos, int containerCount) {
			if(fromPos != toPos) {
				int num = IGenerator.IndexFromGeneratorPosition(toPos);
				UIElement[] elementArray = new UIElement[containerCount];
				for(int i = 0; i < containerCount; i++) {
					elementArray[i] = Panel.Children[fromPos.Index + i];
				}
				Panel.Children.RemoveRange(fromPos.Index, containerCount);
				for(int j = 0; j < containerCount; j++) {
					Panel.Children.Insert(num + j, elementArray[j]);
				}
			}
		}
		protected virtual void ReGenerateChildren() {
			IGenerator.RemoveAll();
			Panel.Children.Clear();
			using(IGenerator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward)) {
				UIElement element;
				while((element = IGenerator.GenerateNext() as UIElement) != null) {
					Panel.Children.Add(element);
					IGenerator.PrepareItemContainer(element);
				}
			}
		}
	}
}
