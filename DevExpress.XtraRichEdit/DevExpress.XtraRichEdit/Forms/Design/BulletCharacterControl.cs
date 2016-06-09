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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Design {
#if !DEBUGTEST
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
#endif
	public partial class BulletCharacterControl : DevExpress.XtraEditors.XtraUserControl {
		readonly List<SimpleSymbolListBox> symbolListBoxes;
		int selectedIndex;
		public BulletCharacterControl() {
			this.selectedIndex = 0;
			InitializeComponent();
			this.symbolListBoxes = CreateListBoxes();
			SubscribeControlEvents();
		}
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				if (value < 0 || value > symbolListBoxes.Count)
					Exceptions.ThrowArgumentException("SelectedIndex", value);
				selectedIndex = value;
				SimpleSymbolListBox newListBox = symbolListBoxes[selectedIndex];
				ExchangeSelectedListBox(newListBox);
			}
		}
		#region Events
		#region MouseDoubleClick
		static readonly object mouseDoubleClick = new object();
		public new event MouseEventHandler MouseDoubleClick {
			add { Events.AddHandler(mouseDoubleClick, value); }
			remove { Events.RemoveHandler(mouseDoubleClick, value); }
		}
		protected internal virtual void RaiseMouseDoubleClick(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseDoubleClick];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region KeyDown
		static readonly object keyDown = new object();
		public new event KeyEventHandler KeyDown {
			add { Events.AddHandler(keyDown, value); }
			remove { Events.RemoveHandler(keyDown, value); }
		}
		protected internal virtual void RaiseKeyDown(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[keyDown];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region SelectedIndexChanged
		static readonly object selectedIndexChanged = new object();
		public event EventHandler SelectedIndexChanged {
			add { Events.AddHandler(selectedIndexChanged, value); }
			remove { Events.RemoveHandler(selectedIndexChanged, value); }
		}
		protected internal virtual void RaiseSelectedIndexChanged() {
			EventHandler handler = (EventHandler)this.Events[selectedIndexChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		List<SimpleSymbolListBox> CreateListBoxes() {
			List<SimpleSymbolListBox> result = new List<SimpleSymbolListBox>();
			this.simpleSymbolListBox1.isActive = true;
			this.simpleSymbolListBox2.TabStop = false;
			this.simpleSymbolListBox3.TabStop = false;
			this.simpleSymbolListBox4.TabStop = false;
			this.simpleSymbolListBox5.TabStop = false;
			this.simpleSymbolListBox6.TabStop = false;
			result.Add(simpleSymbolListBox1);
			result.Add(simpleSymbolListBox2);
			result.Add(simpleSymbolListBox3);
			result.Add(simpleSymbolListBox4);
			result.Add(simpleSymbolListBox5);
			result.Add(simpleSymbolListBox6);
			return result;
		}
		protected internal virtual void InitializeComponents(SymbolProperties[] properties) {
			for (int i = 0; i < properties.Length; i++) {
				symbolListBoxes[i].FontName = properties[i].FontName;
				symbolListBoxes[i].Items.Clear();
				symbolListBoxes[i].Items.Add(properties[i].UnicodeChar);
			}
		}
		protected internal virtual SymbolProperties GetActiveSymbolProperties() {
			SimpleSymbolListBox activeListBox = GetActiveSymbolListBox();
			return new SymbolProperties(((char)activeListBox.Items[0]), activeListBox.FontName);
		}
		protected internal virtual void SetActiveSymbolProperties(SymbolProperties properties) {
			SimpleSymbolListBox activeListBox = GetActiveSymbolListBox();
			activeListBox.Items.Clear();
			activeListBox.Items.Add(properties.UnicodeChar);
			activeListBox.FontName = properties.FontName;
		}
		protected internal virtual void SubscribeControlEvents() {
			int count = symbolListBoxes.Count;
			for (int i = 0; i < count; i++) {
				SimpleSymbolListBox listBox = symbolListBoxes[i];
				listBox.KeyDown += OnSimpleSymbolListBoxKeyDown;
				listBox.MouseDown += OnSimpleSymbolListBoxMouseDown;
				listBox.MouseDoubleClick += OnSimpleSymbolListBoxMouseDoubleClick;
			}
		}
		void OnSimpleSymbolListBoxMouseDoubleClick(object sender, MouseEventArgs e) {
			RaiseMouseDoubleClick(e);
		}
		void OnSimpleSymbolListBoxMouseDown(object sender, MouseEventArgs e) {
			SimpleSymbolListBox newListBox = GetFocusedSymbolListBox();
			ExchangeSelectedListBox(newListBox);
		}
		void OnSimpleSymbolListBoxKeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) {
				SimpleSymbolListBox activeListBox = GetActiveSymbolListBox();
				int activeListBoxIndex = symbolListBoxes.IndexOf(activeListBox);
				int newListBoxIndex = GetNextActiveListBoxIndex(e.KeyCode, activeListBoxIndex);
				SimpleSymbolListBox newListBox = symbolListBoxes[newListBoxIndex];
				ExchangeSelectedListBox(newListBox);
			}
			RaiseKeyDown(e);
		}
		void ExchangeSelectedListBox(SimpleSymbolListBox newListBox) {
			SimpleSymbolListBox activeListBox = GetActiveSymbolListBox();
			activeListBox.TabStop = false;
			activeListBox.isActive = false;
			activeListBox.Refresh();
			newListBox.TabStop = true;
			newListBox.isActive = true;
			newListBox.Focus();
			newListBox.Refresh();
			selectedIndex = symbolListBoxes.IndexOf(newListBox);
			RaiseSelectedIndexChanged();
		}
		int GetNextActiveListBoxIndex(Keys keyDown, int activeListBoxIndex) {
			if (keyDown == Keys.Left || keyDown == Keys.Up) {
				if (activeListBoxIndex == 0)
					return 5;
				else
					return activeListBoxIndex - 1;
			}
			if (keyDown == Keys.Right || keyDown == Keys.Down) {
				if (activeListBoxIndex == 5)
					return 0;
				else
					return activeListBoxIndex + 1;
			}
			return -1;
		}
		protected internal virtual SimpleSymbolListBox GetFocusedSymbolListBox() {
			int count = symbolListBoxes.Count;
			for (int i = 0; i < count; i++) {
				SimpleSymbolListBox listBox = symbolListBoxes[i];
				if (listBox.Focused)
					return listBox;
			}
			return symbolListBoxes[0];
		}
		protected internal virtual SimpleSymbolListBox GetActiveSymbolListBox() {
			int count = symbolListBoxes.Count;
			for (int i = 0; i < count; i++) {
				SimpleSymbolListBox listBox = symbolListBoxes[i];
				if (listBox.isActive)
					return listBox;
			}
			return symbolListBoxes[0];
		}
	}
}
