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
namespace DevExpress.Data.Utils {
	public interface IToolShell : IDisposable {
		IToolItem this[Guid itemKind] { get; }
		void ShowNoActivate();
		void Hide();
		void HideIfNotContains(IToolShell anotherToolShell);
		void Close();
		void AddToolItem(IToolItem item);
		void RemoveToolItem(Guid itemKind);
		void InitToolItems();
		void UpdateToolItems();
	}
	public interface IToolItem : IDisposable {
		Guid Kind { get; }
		void InitTool();
		void UpdateView();
		void Hide();
		void Close();
		void ShowNoActivate();
		void ShowActivate();
	}
	public class ToolShell : IToolShell {
		List<IToolItem> toolItems = new List<IToolItem>();
		IToolItem IToolShell.this[Guid itemKind] {
			get { return GetItemBy(itemKind); }
		}
		public ToolShell() {
		}
		IToolItem GetItemBy(Guid itemKind) {
			for(int i = 0; i < toolItems.Count; i++)
				if(itemKind == toolItems[i].Kind)
					return toolItems[i];
			return null;
		}
		public void RemoveToolItem(Guid itemKind) {
			IToolItem toolItem = GetItemBy(itemKind);
			if(toolItem != null)
				toolItems.Remove(toolItem);
		}
		public void ShowNoActivate() {
			foreach(IToolItem obj in toolItems)
				obj.ShowNoActivate();
		}
		public void Hide() {
			foreach(IToolItem obj in toolItems)
				obj.Hide();
		}
		public void HideIfNotContains(IToolShell anotherToolShell) {
			foreach(IToolItem obj in toolItems) {
				IToolItem item = anotherToolShell[obj.Kind];
				if(item == null)
					obj.Hide();
			}
		}
		public void Close() {
			for(int i = toolItems.Count - 1; i >= 0; i--) {
				IToolItem item = toolItems[i];
				if(item != null) item.Close();
			}
		}
		public void AddToolItem(IToolItem item) {
			if(GetItemBy(item.Kind) == null)
				toolItems.Add(item);
		}
		public void InitToolItems() {
			foreach(IToolItem obj in toolItems)
				obj.InitTool();
		}
		public void UpdateToolItems() {
			foreach(IToolItem obj in toolItems)
				obj.UpdateView();
		}
		public void Dispose() {
			foreach(IToolItem obj in toolItems)
				obj.Dispose();
			toolItems = null;
		}
	}
}
