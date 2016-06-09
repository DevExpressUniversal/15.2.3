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

using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
namespace DevExpress.Utils.Design {
	public class VSDevExpressMenuItem {
		#region props
		private string headerCore = string.Empty;
		private string toolTipCore = string.Empty;
		private Image iconCore = null;
		private object VSSourceCore = null;
		public VSDevExpressMenuItem Parent { get; protected set; }
		public string Header {
			get { return headerCore; }
			set {
				if(headerCore == value) return;
				string oldValue = headerCore;
				headerCore = value;
				OnHeaderChanged(oldValue);
			}
		}
		public string ToolTip {
			get { return toolTipCore; }
			set {
				if(toolTipCore == value) return;
				string oldValue = toolTipCore;
				toolTipCore = value;
				OnToolTipChanged(oldValue);
			}
		}
		public Image Icon {
			get { return iconCore; }
			set {
				if(iconCore == value) return;
				Image oldValue = iconCore;
				iconCore = value;
				OnIconChanged(oldValue);
			}
		}
		protected object VSSource {
			get { return VSSourceCore; }
			set {
				if(VSSourceCore == value) return;
				object oldValue = VSSourceCore;
				VSSourceCore = value;
				OnVSSourceChanged(oldValue); 
			}
		}
		public List<VSDevExpressMenuItem> itemsInternal = new List<VSDevExpressMenuItem>();
		IEnumerable<VSDevExpressMenuItem> Items {
			get {
				foreach(VSDevExpressMenuItem childItem in itemsInternal) {
					yield return childItem;
				}
				yield break;
			}
		}
		public event EventHandler Click;
		#endregion
		public VSDevExpressMenuItem() {
		}
		[CLSCompliant(false)]
		protected internal VSDevExpressMenuItem(CommandBarControl source) {
			VSSourceCore = source;
			if(source != null) {
				if(source.Type == MsoControlType.msoControlButton)
					((CommandBarButton)source).Click += OnButtonClick;
				headerCore = source.Caption;
				toolTipCore = source.TooltipText;
			}
			CreateChildrenFromSource();
		}
		public VSDevExpressMenuItem CreateItem() {
			if(VSSource == null) return null;
			CommandBarControl commandBarControl = (CommandBarControl)VSSource;
			if(commandBarControl.Type != MsoControlType.msoControlPopup) {
				CommandBar parentCommandBar = commandBarControl.Parent;
				if(parentCommandBar == null) return null;
				int savedIndex = commandBarControl.Index;
				VSSource = parentCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, savedIndex, Type.Missing) as CommandBarPopup;
				commandBarControl.Delete();
			}
			CommandBarPopup commandBarPopup = (CommandBarPopup)VSSource;
			VSDevExpressMenuItem childItem = new VSDevExpressMenuItem();
			childItem.VSSource = ((CommandBarPopup)VSSource).Controls.Add(MsoControlType.msoControlButton, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			itemsInternal.Add(childItem);
			childItem.Parent = this;
			return childItem;
		}
		protected void CreateChildrenFromSource() {
			CommandBarPopup commandBarPopup = VSSource as CommandBarPopup;
			if(commandBarPopup == null) return;
			foreach(CommandBarControl control in commandBarPopup.Controls) {
				if(control.Type == MsoControlType.msoControlButton || control.Type == MsoControlType.msoControlPopup) {
					VSDevExpressMenuItem item = new VSDevExpressMenuItem(control);
					item.Parent = this;
					itemsInternal.Add(item);
				}
			}
		}
		VSDevExpressMenuItem GetItemByHeader(string header) {
			foreach(VSDevExpressMenuItem child in itemsInternal) {
				if(child.Header == header) return child;
			}
			return null;
		}
		public VSDevExpressMenuItem CreateOrGetItem(string header) {
			VSDevExpressMenuItem childItem = GetItemByHeader(header);
			if(childItem != null)
				return childItem;
			childItem = CreateItem();
			childItem.Header = header;
			return childItem;
		}
		protected virtual void OnIconChanged(Image oldValue) {
			UpdateIcon();
		}
		protected virtual void OnToolTipChanged(string oldValue) {
			UpdateToolTip();
		}
		protected virtual void OnHeaderChanged(string oldValue) {
			UpdateHeader();
		}
		protected virtual void OnVSSourceChanged(object oldValue) {
			CommandBarControl commandBarControl = oldValue as CommandBarControl;
			if(commandBarControl != null) {
				if(commandBarControl.Type == MsoControlType.msoControlButton)
					((CommandBarButton)commandBarControl).Click -= OnButtonClick;
			}
			commandBarControl = VSSource as CommandBarControl;
			if(commandBarControl != null) {
				if(commandBarControl.Type == MsoControlType.msoControlButton)
					((CommandBarButton)VSSource).Click += OnButtonClick;
				UpdateHeader();
				UpdateIcon();
				UpdateToolTip();
			}
		}
		void OnButtonClick(CommandBarButton Ctrl, ref bool CancelDefault) {
			RaiseClickEvent();
		}
		protected virtual void RaiseClickEvent() {
			if(Click != null)
				Click(this, new EventArgs());
		}
		protected void UpdateHeader() {
			if(VSSource == null || string.IsNullOrEmpty(Header)) return;
			((CommandBarControl)VSSource).Caption = Header;
		}
		protected void UpdateToolTip() {
			if(VSSource == null) return;
			((CommandBarControl)VSSource).TooltipText = ToolTip;
		}
		protected void UpdateIcon() {
			if(VSSource == null) return;
			if(((CommandBarControl)VSSource).Type != MsoControlType.msoControlButton) return;
			((CommandBarButton)VSSource).Picture = MenuItemPictureHelper.ConvertImageToPicture(Icon);
		}
	}
	[CLSCompliant(false)]
	public class VSDevExpressMenu : VSDevExpressMenuItem {
		const string DevExpressMenuName = "DevE&xpress";
		public VSDevExpressMenu(DTE dte) {
			Header = DevExpressMenuName;
			CommandBars commandBars = dte.CommandBars as CommandBars;
			CommandBar mainMenuBar = commandBars["MenuBar"];
			CommandBarPopup devExpressMenu = null;
			foreach(CommandBarControl commandBarControl in mainMenuBar.Controls) {
				if(commandBarControl.Type == MsoControlType.msoControlPopup) {
					CommandBarPopup commandBarPopup = (CommandBarPopup)commandBarControl;
					if(commandBarPopup.CommandBar.Name == DevExpressMenuName) {
						devExpressMenu = commandBarPopup;
						break;
					}
				}
			}		  
			if(devExpressMenu == null)
				devExpressMenu = mainMenuBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarPopup;
			VSSource = devExpressMenu;
			CreateChildrenFromSource();
		}
	}
}
