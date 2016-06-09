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
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Native {
	public interface IItemGenerator {
		void Begin();
		Control GenerateNext();
		void LoseNext();
		void End();
	}
	public class ItemGenerator<T> : IItemGenerator where T : Control, new() {
		int freeItemIndex = 0;
		Panel panel;
		List<Control> items = new List<Control>();
		Locker locker;
		public ItemGenerator(Panel panel) {
			this.panel = panel;
			this.locker = new Locker();
		}
		void Reset() {
			this.freeItemIndex = 0;
		}
		public void Begin() {
			if (!this.locker.IsLocked)
				Reset();
			this.locker.Lock();
		}
		public void End() {
			this.locker.Unlock();
			if (this.locker.IsLocked)
				return;
			int count = this.items.Count;
			for (int i = freeItemIndex; i < count; i++) {
				this.items[i].Visibility = Visibility.Collapsed;
			}
		}
		public Control GenerateNext() {
			if (this.freeItemIndex >= this.items.Count) {
				Control item = CreateNewItem();
				item.DataContext = null;
				AddItem(item);
			}
			Control result = this.items[freeItemIndex++];
			result.Visibility = System.Windows.Visibility.Visible;
			return result;
		}
		public void LoseNext() {
			--freeItemIndex;
		}
		void AddItem(Control item) {
			int count = this.items.Count;
			if (count > 0) {
				Control lastItem = this.items[count - 1];
				int indx = panel.Children.IndexOf(lastItem);
				panel.Children.Insert(indx, item);
			} else
				panel.Children.Add(item);
			this.items.Add(item);
		}
		protected T CreateNewItem() {
			return new T();
		}
	}
}
