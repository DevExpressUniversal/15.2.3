#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public class BarItemFindStrategy : Singleton<BarItemFindStrategy> {
		public const string DuplicateMessageString = "There are multiplebar items with the '{0}' caption. Use one of full name variant's: {1}";
		private void FindBarItemLinksInSubBars(string path, string caption, BarItemLink barItemLink, IDictionary<BarItemLink, string> controls) {
			if(!(barItemLink is BarLinkContainerExItemLink)) {
				if(IsCompatibleControl(path, barItemLink, caption) && !controls.ContainsKey(barItemLink)) {
					bool isSubItemOfFoundItem = barItemLink.OwnerItem != null && controls.ContainsKey(barItemLink.OwnerItem.Links[0]);
					if(!isSubItemOfFoundItem) {
						bool isFound = false;
						foreach(BarItemLink link in controls.Keys) {
							if(link.Item == barItemLink.Item) {
								isFound = true;
								break;
							}
						}
						if(!isFound) {
							controls.Add(barItemLink, path);
						}
					}
				}
			}
			if(barItemLink is BarCustomContainerItemLink) {
				foreach(BarItemLink bil in ((BarCustomContainerItemLink)barItemLink).VisibleLinks) {
					if(bil != barItemLink) {
						if(bil is BarLinkContainerExItemLink) {
							FindBarItemLinksInSubBars(path, caption, bil, controls);
						}
						else {
							string bilCaption = bil.Caption == "" ? bil.Item.Name : bil.Caption;
							FindBarItemLinksInSubBars(path + "." + bilCaption, caption, bil, controls);
						}
					}
				}
			}
			else {
				BarButtonItem buttonItem = barItemLink.Item as BarButtonItem;
				if(buttonItem != null && (buttonItem.Visibility == BarItemVisibility.Always || buttonItem.Visibility == BarItemVisibility.OnlyInRuntime)) {
					PopupMenu popup = buttonItem.DropDownControl as PopupMenu;
					if(popup != null) {
						EnsureBarItemLinksFilled(popup);
						foreach(BarItemLink barItemLink_ in popup.ItemLinks) {
							FindBarItemLinksInSubBars(path + '.' + barItemLink_.Caption, caption, barItemLink_, controls);
						}
					}
				}
			}
		}
		private void FindBarItemLinks(string path, string caption, BarItemLinkReadOnlyCollection barItemLinkCollection, IDictionary<BarItemLink, string> controls) {
			foreach(BarItemLink barItemLink in barItemLinkCollection) {
				string path_ = string.IsNullOrEmpty(path) ? "" : path + ".";
				FindBarItemLinksInSubBars(path_ + barItemLink.Caption, caption, barItemLink, controls);
			}
		}
		private BarItemLink FindBarItemLinkInBarManager(BarManager manager, string caption) {
			if(manager != null) {
				string containerName = GetContainerName(manager);
				IDictionary<BarItemLink, string> controls = new Dictionary<BarItemLink, string>();
				foreach(Bar bar in manager.Bars) {
					string path = string.IsNullOrEmpty(containerName) ? bar.BarName : containerName + '.' + bar.BarName;
					FindBarItemLinks(path, caption, bar.ItemLinks, controls);
				}
				return CheckDuplicate(controls, caption);
			}
			return null;
		}
		private void EnsureBarItemLinksFilled(PopupMenuBase popup) {
			if(popup.ItemLinks.Count == 0) {
				popup.ShowPopup(new Point(0, 0));
				Application.DoEvents();
				popup.HidePopup();
				Application.DoEvents();
			}
		}
		private BarItemLink CheckDuplicate(IDictionary<BarItemLink, string> controls, string caption) {
			if(controls.Count > 1) {
				List<String> duplicates = new List<string>();
				foreach(KeyValuePair<BarItemLink, string> value in controls) {
					duplicates.Add("'" + value.Value + "'");
				}
				throw new EasyTestException(string.Format(DuplicateMessageString,
					caption, string.Join(", ", duplicates.ToArray())));
			}
			else {
				foreach(BarItemLink key in controls.Keys) {
					return key;
				}
			}
			return null;
		}
		private string GetFullName(string containerName, string name) {
			return string.IsNullOrEmpty(containerName) ? name : containerName + "." + name;
		}
		private string GetContainerName(BarManager barManager) {
			for(Control control = barManager.Form; control != null; control = control.Parent) {
				if(control.Tag != null) {
					return EasyTestTagHelper.GetTestValue(EasyTestTagHelper.TestContainer, control.Tag);
				}
			}
			return null;
		}
		protected virtual bool IsCompatibleControl(string path, BarItemLink link, string caption) {
			string displayCaption = ButtonEditControl.GetDisplayCaption(link.DisplayCaption);
			bool isSameCaption = String.Compare(displayCaption, caption) == 0;
			bool isSamePath = String.Compare(path, caption) == 0;
			bool isSameName = String.Compare(link.Item.Name, caption) == 0;
			if(
				(link.Item.Visibility == BarItemVisibility.Always || link.Item.Visibility == BarItemVisibility.OnlyInRuntime)
				&&
				(isSameCaption || isSameName || isSamePath)
				) {
				EasyTestTracer.Tracer.LogVerboseText("isSameCaption: {0} isSameName: {1} isSamePath: {2}", isSameCaption, isSameName, isSamePath);
				return true;
			}
			return false;
		}
		public BarItemLink FindControl(PopupMenuBase popupMenu, string caption) {
			EnsureBarItemLinksFilled(popupMenu);
			return FindControl(popupMenu.ItemLinks, caption);
		}
		public BarItemLink FindControl(Form form, string caption) {
			IList<BarManager> barManagers = (new BarManagerFindStrategy()).FindBarManagers(form);
			BarItemLink result;
			foreach(BarManager barManager in barManagers) {
				result = FindBarItemLinkInBarManager(barManager, caption);
				if(result != null) {
					return result;
				}
			}
			return null;
		}
		public BarItemLink FindControl(BarItemLinkReadOnlyCollection barItemLinkCollection, string caption) {
			IDictionary<BarItemLink, string> controls = new Dictionary<BarItemLink, string>();
			FindBarItemLinks("", caption, barItemLinkCollection, controls);
			return CheckDuplicate(controls, caption);
		}
		public object FindControlInPopupMenu(Control control, string actionName) {
			object result = null;
			control.Invoke(new ThreadStart(delegate() {
				BarManager bm = BarManagerFindStrategy.Instance.FindPopupMenuBarManager(control);
				if(bm != null) {
					PopupMenuBase popup = bm.GetPopupContextMenu(control);
					if(popup != null) {
						result = FindControl(popup, actionName);
					}
				}
			}));
			return result;
		}
	}
}
