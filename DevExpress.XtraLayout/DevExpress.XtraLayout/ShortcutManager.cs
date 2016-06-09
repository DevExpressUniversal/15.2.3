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
using System.IO;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DXUtils = DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Printing;
using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Accessibility;
using System.Collections.Generic;
namespace DevExpress.XtraLayout {
	public struct Shortcut {
		public Shortcut(char key, BaseLayoutItem item) {
			this.key = key;
			this.item = item;
		}
		public char key;
		public BaseLayoutItem item;
	}
	public class ShortcutManager {
		ILayoutControl control;
		public ShortcutManager(ILayoutControl control) {
			this.control = control;
		}
		List<Shortcut> GetShortcuts(LayoutControlItem focusedItem) {
			List<Shortcut> shortcuts = new List<Shortcut>();
			if(control != null) {
				foreach(BaseLayoutItem item in control.Items) {
					if(AlreadyFocused(item, focusedItem)) continue;
					int index = item.Text.LastIndexOf('&');
					if(index > 0 && item.Text[index - 1] == '&') {
						continue;
					}
					if(index >= 0 && index != item.Text.Length - 1) {
						char ch = Char.ToLower(item.Text[index + 1]);
						shortcuts.Add(new Shortcut(ch, item));
					}
				}
			}
			return shortcuts;
		}
		bool AlreadyFocused(BaseLayoutItem item, LayoutControlItem focusedItem) {
			return item == focusedItem && focusedItem.Control != null && focusedItem.Control.ContainsFocus;
		}
		public List<BaseLayoutItem> GetNextItemByKey(LayoutControlItem focusedItem, char key) {
			char lowerChar = Char.ToLower(key);
			List<Shortcut> shortcuts = GetShortcuts(focusedItem);
			List<BaseLayoutItem> candidates = new List<BaseLayoutItem>();
			foreach(Shortcut sh in shortcuts) {
				if(sh.key == lowerChar && sh.item != null && sh.item.ActualItemVisibility) candidates.Add(sh.item);
			}
			return candidates;
		}
	}
}
