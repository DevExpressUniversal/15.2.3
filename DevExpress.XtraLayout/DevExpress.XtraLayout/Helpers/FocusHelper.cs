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
namespace DevExpress.XtraLayout.Helpers {
	public class FocusHelperBase : IDisposable {
		protected ILayoutControl layoutControl;
		protected Component focusedComponentCore;
		protected ArrayList collection;
		protected LayoutControlWalker walker;
		protected ShortcutManager shortcutManager;
		public FocusHelperBase(ILayoutControl control) {
			layoutControl = control;
			shortcutManager = new ShortcutManager(control);
		}
		public Control FindFocusedControl() {
			Form form = Form.ActiveForm;
			if((form == null) || (form.ActiveControl == null)) {
				Control root = layoutControl.Control;
				if(root != null) {
					form = root.FindForm();
					if(form != null) {
						root = form.ActiveControl;
					}
					return this.FindFocusedChild(root);
				}
				return null;
			}
			return form.ActiveControl;
		}
		private Control FindFocusedChild(Control control) {
			if(control == null) {
				return null;
			}
			if(control.HasChildren) {
				for(int n = 0; n < control.Controls.Count; n++) {
					Control fc = this.FindFocusedChild(control.Controls[n]);
					if(fc != null) {
						return fc;
					}
				}
				return null;
			}
			return (control.Focused ? control : null);
		}
		protected virtual void FocusEdge(bool last) {
			if(layoutControl.RootGroup == null) return;
			ArrayList list = GetArrangedFocusList(true, false);
			if(list.Count > 0)
				FocusElement((Component)list[last ? list.Count - 1 : 0], true);
		}
		public virtual void FocusLast() {
			FocusEdge(true);
		}
		public virtual void FocusFirst() {
			FocusEdge(false);
		}
		protected internal virtual void PlaceItemIntoViewRestricted(BaseLayoutItem bitem) {
			if(!layoutControl.OptionsView.AlwaysScrollActiveControlIntoView) return;
			PlaceItemIntoView(bitem);
		}
		public virtual void PlaceItemIntoView(BaseLayoutItem bitem) {
			if(bitem == null) return;
			BaseLayoutItem tempItem = bitem;
			int watchdog = 100;
			while(tempItem.Parent != null) {
				watchdog--;
				LayoutGroup tempGroup = tempItem as LayoutGroup;
				if(tempGroup != null) {
					if(tempGroup.ParentTabbedGroup != null) {
						if(tempGroup.ParentTabbedGroup.SelectedTabPage != tempGroup) {
							tempGroup.Handler.AllowChangeSelection = false;
							tempGroup.ParentTabbedGroup.SelectedTabPage = tempGroup;
							tempGroup.Handler.AllowChangeSelection = true;
						}
					}
					if(!tempGroup.Expanded && tempGroup != bitem) tempGroup.Expanded = true;
				}
				if(tempItem.Parent != null) tempItem = tempItem.Parent;
				if(watchdog == 0) return;
			}
			Rectangle itemBounds = bitem.ViewInfo.BoundsRelativeToControl;
			LayoutControlItem citem = bitem as LayoutControlItem;
			if(citem != null) itemBounds = citem.ViewInfo.ClientAreaRelativeToControl;
			if(citem != null && citem.Control != null) {
				itemBounds = citem.Control.Bounds;
			}
			LayoutGroup tempGroup1 = bitem as LayoutGroup;
			if(tempGroup1 != null && tempGroup1.ParentTabbedGroup != null && tempGroup1.ActualItemVisibility) {
				itemBounds = tempGroup1.ParentTabbedGroup.ViewInfo.GetScreenTabCaptionRect(tempGroup1.ParentTabbedGroup.VisibleTabPages.IndexOf(tempGroup1));
			}
			TabbedGroup tempTabbedGroup = bitem as TabbedGroup;
			if(tempTabbedGroup != null && tempTabbedGroup.SelectedTabPage != null) {
				itemBounds = tempTabbedGroup.ViewInfo.GetScreenTabCaptionRect(tempTabbedGroup.VisibleTabPages.IndexOf(tempTabbedGroup.SelectedTabPage));
			}
			int diffx = 0, diffy = 0;
			if(itemBounds.X < 0 || itemBounds.Left > layoutControl.ClientWidth) diffx = -itemBounds.X;
			if(itemBounds.Y < 0 || itemBounds.Top > layoutControl.ClientHeight) diffy = -itemBounds.Y;
			if(itemBounds.Width < layoutControl.ClientWidth && itemBounds.Right > layoutControl.ClientWidth) {
				diffx = layoutControl.ClientWidth - itemBounds.Right;
			}
			if(itemBounds.Height < layoutControl.ClientHeight && itemBounds.Bottom > layoutControl.ClientHeight) {
				diffy = layoutControl.ClientHeight - itemBounds.Bottom;
			}
			if(diffy != 0) layoutControl.Scroller.VScrollPos -= diffy;
			if(diffx != 0) layoutControl.Scroller.HScrollPos -= diffx;
		}
		protected internal void FakeFocusContainerLostFocus() {
			if(allowProcessLostFocus) {
				SelectedComponent = null;
				layoutControl.Invalidate(); 
			}
		}
		protected bool allowProcessLostFocus = true;
		int focusWatchDog = 0;
		public virtual bool FocusElement(Component component, bool tabStopOnly) {
			if(((ILayoutControl)layoutControl).DisposingFlag) return false;
			bool result = false;
			LayoutControlItem citem = component as LayoutControlItem;
			BaseLayoutItem bitem = component as BaseLayoutItem;
			LayoutControlGroup group = component as LayoutControlGroup;
			TabbedGroup tgroup = component as TabbedControlGroup;
			Component prevComponent = SelectedComponent;
			LayoutControlItem pCitem = prevComponent as LayoutControlItem;
			if(citem != null && citem.Control != null) {
				Control controlToFocus = FindChildControlToFocus(citem.Control, tabStopOnly);
				if(controlToFocus != null) {
					allowProcessLostFocus = false;
					ContainerControl ccToFocus = controlToFocus as ContainerControl;
					if(ccToFocus != null && !(ccToFocus is LayoutControl)) {
						if(!ccToFocus.SelectNextControl(null, !MoveBack, true, true, true))
							if(!ccToFocus.SelectNextControl(null, !MoveBack, false, true, true))
								controlToFocus.Focus();
					}
					else controlToFocus.Focus(); 
					if(!controlToFocus.Focused && !controlToFocus.ContainsFocus && focusWatchDog < 5 && (pCitem == null || pCitem.Control == null || !pCitem.Control.ContainsFocus)) {
						focusWatchDog++;
						SelectedComponent = prevComponent;
						result = FocusNextComponent(true);
						focusWatchDog--;
						return true;
					}
					if(controlToFocus.Focused) {
						ChangeActiveControl(controlToFocus); 
						PlaceItemIntoViewRestricted(bitem);
					}
					allowProcessLostFocus = true;
					if(controlToFocus.Focused) result = true;
				}
			}
			if(group != null || tgroup != null) { layoutControl.FakeFocusContainer.Focus(); result = layoutControl.FakeFocusContainer.ContainsFocus; }
			Component prevSelected = null;
			if(result) { prevSelected = SelectedComponent; SelectedComponent = component; }
			if(prevSelected is BaseLayoutItem || SelectedComponent is BaseLayoutItem) {
				layoutControl.Invalidate(); 
			}
			if(result) PlaceItemIntoViewRestricted(bitem);
			return true;
		}
		protected virtual bool ChangeActiveControl(Control control) {
			if(layoutControl.Control == null) return false;
			ContainerControl cc = layoutControl.Control as ContainerControl;
			if(cc != null) {
				if(cc.ActiveControl != control) cc.ActiveControl = control; 
				return cc.ActiveControl == control;
			}
			return false;
		}
		public virtual bool FocusFirstInGroup(LayoutGroup group, bool isTabStop) {
			if(group == null) return false;
			if(FocusOptions.AllowFocusTabbedGroups && !FocusOptions.AllowFocusControlOnActivatedTabPage && group.ParentTabbedGroup != null)
				return false;
			ArrayList candidates = FillItemsInGroup(group);
			if(candidates.Count == 0)
				return false;
			int iterator = 0;
			BaseLayoutItem candidate = candidates[iterator] as BaseLayoutItem;
			while(candidate != null && !CheckItem(candidate, isTabStop, false)) {
				iterator++;
				candidate = (iterator < candidates.Count) ?
					candidates[iterator] as BaseLayoutItem : null;
			}
			if(CheckItem(candidate, isTabStop, false)) {
				return FocusElement(candidate, isTabStop);
			}
			else {
				if(candidates[0] is LayoutGroup) {
					return FocusFirstInGroup(candidates[0] as LayoutGroup, isTabStop);
				}
			}
			(layoutControl as LayoutControl).ActiveControl = null;
			return false;
		}
		protected virtual bool AreThereControlsToSelect(Control control, Control prevControl) {
			foreach(Control tControl in control.Controls) {
				if(tControl == layoutControl) continue;
				if(prevControl == tControl) continue;
				if(AreThereControlsToSelect(tControl, null)) return true;
				if(tControl.CanSelect && tControl != layoutControl) return true;
			}
			return false;
		}
		protected virtual void SelectControlOutside(bool forward, bool tabStopOnly) {
			if(!(layoutControl as LayoutControl).ContainsFocus || (layoutControl.Parent == null)) {
				return;
			}
			IContainerControl container = layoutControl.Parent.GetContainerControl();
			int watchDog = 100;
			Control prevControl = layoutControl.Control;
			while(container is UserControl && ((UserControl)container).Parent != null && watchDog > 0 && !AreThereControlsToSelect((Control)container, prevControl)) {
				watchDog--;
				prevControl = container as Control;
				container = ((UserControl)container).Parent.GetContainerControl();
			}
			SelectedComponent = null;
			if(container != null) {
				LayoutControl lcContainer = container as LayoutControl;
				if(lcContainer != null && container != layoutControl) {
					lcContainer.FocusHelper.MoveBack = MoveBack;
					lcContainer.FocusHelper.FocusNextComponent(tabStopOnly);
				}
				else
					((Control)container).SelectNextControl(prevControl, forward, tabStopOnly, true, true);
			}
		}
		public virtual Component SelectedComponent {
			get {
				return focusedComponentCore;
			}
			set {
				focusedComponentCore = value;
			}
		}
		void IDisposable.Dispose() {
			layoutControl = null;
			focusedComponentCore = null;
			collection = null;
		}
		protected ArrayList FillItemsInGroup(LayoutGroup root) {
			if(root == null) return null;
			walker = new LayoutControlWalker(root);
			ArrayList list = walker.ArrangeElements(FocusOptions);
			ArrayList result = new ArrayList();
			foreach(BaseLayoutItem item in list) {
				LayoutGroup group = item as LayoutGroup;
				TabbedGroup tabbedGroup = item as TabbedGroup;
				result.Add(item);
				if(group != null) {
					result.AddRange(FillItemsInGroup(group));
				}
				if(tabbedGroup != null) {
					foreach(LayoutGroup lg in tabbedGroup.TabPages) {
						result.AddRange(FillItemsInGroup(lg));
					}
				}
			}
			return result;
		}
		public bool IsValidSelectCandidate(Component c) {
			if(c == null) return false;
			return FilterElements(FillItemsInGroup(layoutControl.RootGroup), true, false).Contains(c);
		}
		protected ArrayList GetArrangedFocusList(bool tabStopOnly, bool includeReadonly) {
			return FilterElements(FillItemsInGroup(layoutControl.RootGroup), tabStopOnly, includeReadonly);
		}
		protected virtual Control FindChildControlToFocus(Control control, bool tabStopOnly) {
			if(control is SimpleButton && !((SimpleButton)control).AllowFocus) return null;
			if(control.CanFocus && !tabStopOnly) return control;
			if(control.CanFocus && tabStopOnly && control.TabStop) return control;
			if(tabStopOnly && !control.TabStop && control is ContainerControl) return null;
			foreach(Control tempControl in control.Controls) {
				Control candidate = FindChildControlToFocus(tempControl, tabStopOnly);
				if(candidate != null) return candidate;
			}
			return null;
		}
		protected virtual bool CheckItem(BaseLayoutItem item, bool tabStopOnly, bool includeReadonly) {
			if(item == null || !item.Visible) {
				return false;
			}
			LayoutControlItem citem = item as LayoutControlItem;
			LayoutControlGroup group = item as LayoutControlGroup;
			TabbedGroup tgroup = item as TabbedControlGroup;
			if(citem != null) {
				if(citem.Control != null && FindChildControlToFocus(citem.Control, tabStopOnly) != null) {
					if(citem.GetControlReadonlyStatus() && !FocusOptions.AllowFocusReadonlyEditors) {
						citem.Control.TabStop = false;
						return includeReadonly ? true : false;
					}
					return true;
				}
			}
			if(group != null && group.GroupBordersVisible && group.ExpandButtonVisible && group.TextVisible && FocusOptions.AllowFocusGroups) {
				return true;
			}
			if(tgroup != null && FocusOptions.AllowFocusTabbedGroups && tgroup.SelectedTabPage != null) {
				return true;
			}
			return false;
		}
		public class FilterFocusListEventArgs {
			public bool MoveBack { get; set; }
			public ArrayList FocusList { get; set; }
		}
		public delegate ArrayList FilterFocusListEventHandler(object sender, FilterFocusListEventArgs e);
		public event FilterFocusListEventHandler FilterFocusList;
		ArrayList RaiseFilterArrangeFocusList(ArrayList list) {
			if(FilterFocusList != null) {
				ArrayList result = FilterFocusList(this, new FilterFocusListEventArgs() { MoveBack = MoveBack, FocusList = list });
				if(result != null) return result;
			}
			return list;
		}
		protected virtual ArrayList FilterElements(ArrayList list, bool tabStopOnly, bool includeReadonly) {
			ArrayList result = new ArrayList();
			if(list == null) return result;
			foreach(BaseLayoutItem item in list) {
				if(CheckItem(item, tabStopOnly, includeReadonly)) {
					result.Add(item);
				}
			}
			result = RaiseFilterArrangeFocusList(result);
			return result;
		}
		protected OptionsFocus FocusOptions {
			get {
				return layoutControl.OptionsFocus;
			}
		}
		protected bool moveBackCore = false;
		protected internal bool MoveBack {
			get {
				return moveBackCore;
			}
			set { moveBackCore = value; }
		}
		private bool NeedPassFocusOutside(int index, ArrayList list) {
			return list.Count == 0 || index == 0 && MoveBack || !MoveBack && (index == (list.Count - 1));
		}
		protected virtual TabbedGroup FindParentTabbedGroup(Component component) {
			if(component == null) return null;
			Control tempControl = component as Control;
			TabbedGroup tGroup = component as TabbedGroup;
			LayoutGroup lGroup = component as LayoutGroup;
			BaseLayoutItem bItem = component as BaseLayoutItem;
			if(tempControl != null) {
				FindParentTabbedGroup(layoutControl.GetItemByControl(tempControl));
			}
			if(tGroup != null) {
				return tGroup;
			}
			if(lGroup != null) {
				if(lGroup.ParentTabbedGroup != null)
					return lGroup.ParentTabbedGroup;
			}
			if(bItem != null) {
				return FindParentTabbedGroup(bItem.Parent);
			}
			return null;
		}
		protected virtual bool ChangeExpand() {
			LayoutGroup layoutGroup = SelectedComponent as LayoutGroup;
			if(layoutGroup != null && layoutGroup.ExpandButtonVisible && layoutGroup.GroupBordersVisible && layoutGroup.ParentTabbedGroup == null) {
				FocusElement(layoutGroup, false);
				layoutGroup.Expanded = !layoutGroup.Expanded;
				return true;
			}
			return false;
		}
		protected virtual bool ChangeExpand(bool newExpanedeState) {
			LayoutGroup layoutGroup = SelectedComponent as LayoutGroup;
			if(layoutGroup != null && layoutGroup.ExpandButtonVisible && layoutGroup.GroupBordersVisible && layoutGroup.ParentTabbedGroup == null) {
				layoutGroup.Expanded = newExpanedeState;
				return true;
			}
			return false;
		}
		protected virtual void FocusTopElementOnThePage(TabbedGroup tg) {
			bool resultCore = true;
			if(layoutControl.OptionsFocus.AllowFocusTabbedGroups && layoutControl.OptionsFocus.AllowFocusControlOnActivatedTabPage || !layoutControl.OptionsFocus.AllowFocusTabbedGroups) {
				resultCore = FocusFirstInGroup(tg.SelectedTabPage, true);
			}
			else
				resultCore = false;
			if(!resultCore) {
				FocusElement(tg, true);
			}
		}
		protected virtual bool SwapTabCore(bool swapLeft, TabbedGroup tg) {
			bool result = false;
			if(tg != null) {
				result = tg.SelectNextPage(swapLeft);
				if(result) {
					FocusTopElementOnThePage(tg);
				}
				else {
					FocusElement(tg, true);
				}
			}
			return result;
		}
		protected virtual bool SwapTab(bool swapLeft) {
			TabbedGroup tg = SelectedComponent as TabbedGroup;
			return SwapTabCore(swapLeft, tg);
		}
		protected virtual void SwapTabToEdge(bool isLeftEdge) {
			TabbedGroup tg = SelectedComponent as TabbedGroup;
			if(tg == null) return;
			bool result = false;
			if(isLeftEdge)
				result = tg.SelectFirstPage();
			else
				result = tg.SelectLastPage();
			if(result) FocusTopElementOnThePage(tg);
			else FocusElement(tg, true);
		}
		protected virtual bool SwapParentTab(bool moveBack) {
			if(SelectedComponent != null) {
				TabbedGroup group = FindParentTabbedGroup(SelectedComponent);
				return SwapTabCore(moveBack, group);
			}
			return false;
		}
		protected virtual bool ScrollPage(bool scrollUp) {
			int delta = scrollUp ? -layoutControl.Scroller.VScroll.LargeChange : layoutControl.Scroller.VScroll.LargeChange;
			layoutControl.Scroller.VScrollPos += delta;
			return true;
		}
		protected FakeFocusContainer FindFakeFocusContainerAndUpdateTabIndex(int newTabIndex) {
			foreach(Control c in layoutControl.Control.Controls) {
				FakeFocusContainer result = c as FakeFocusContainer;
				if(result != null) {
					result.TabIndex = newTabIndex;
					return result;
				}
			}
			return null;
		}
		protected virtual void UpdateTabIndexes(ArrayList list) {
			if(!layoutControl.OptionsFocus.EnableAutoTabOrder) return;
			int counter = 0;
			foreach(object item in list) {
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci != null && lci.Control != null)
					if(lci.Control.TabIndex != counter) lci.Control.TabIndex = counter;
				if(counter == 0) counter++;
				counter++;
			}
			if(layoutControl is LayoutControl) {
				LayoutControl castedControl = layoutControl as LayoutControl;
				for(int i = 4; i < castedControl.Controls.Count; i++) {
					if(ListContains(castedControl, list, i)) continue;
					castedControl.Controls[i].TabIndex = 1;
				}
			}
			if(list.Count > 0) {
				if(list[0] is LayoutItemContainer && MoveBack) {
					FindFakeFocusContainerAndUpdateTabIndex(0);
				}
				if(list[list.Count - 1] is LayoutItemContainer && !MoveBack) {
					FindFakeFocusContainerAndUpdateTabIndex(list.Count - 1);
				}
			}
		}
		bool ListContains(LayoutControl castedControl, ArrayList list, int i) {
			foreach(object item in list) {
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci == null) continue;
				if(lci.Control == castedControl.Controls[i])
					return true;
			}
			return false;
		}
		protected virtual bool FocusNextComponent(bool tabStopOnly) {
			ArrayList list = GetArrangedFocusList(tabStopOnly, false);
			UpdateTabIndexes(list);
			int index = 0;
			if(list.Count != 0) {
				if(SelectedComponent == null) {
					if(MoveBack) {
						index = list.Count - 1;
					} else
						index = 0;
				} else {
					index = list.IndexOf(SelectedComponent);
					if(index < 0) {
						ArrayList list2 = GetArrangedFocusList(tabStopOnly, true);
						int index2 = list2.IndexOf(SelectedComponent);
						if(index2 >= 0) {
							if(MoveBack)
								index2--;
							else
								index2++;
							if(index2 < 0) index = 0;
							if(list2.Count != 0 && list.Contains(list2[index2])) {
								return FocusElement((Component)list2[index2], tabStopOnly);
							}
						}
					}
					if(index < 0 && SelectedComponent is LayoutControlItem && tabStopOnly) {
						return FocusNextComponent(false);
					}
				}
			}
			if(NeedPassFocusOutside(index, list)) {
				if(IsSingleFocusManagerOnTheForm(layoutControl.Control.Parent)) {
					if(list.Count != 0) {
						if(index < 0) index = 0;
						if(!MoveBack) index = 0;
						else index = list.Count - 1;
						return FocusElement((Component)list[index], tabStopOnly);
					}
				} else return false;
			}
			if(MoveBack)
				index--;
			else
				index++;
			if(list.Count != 0) {
				if(index < 0) index = 0;
				return FocusElement((Component)list[index], tabStopOnly);
			}
			return false;
		}
		protected virtual Control FindParentControlForSendingFocus(Control control) {
			if(control.Parent != null)
				if(control.Parent.Controls.Count > 1)
					return layoutControl.Parent;
				else
					return FindParentControlForSendingFocus(control.Parent);
			else
				return null;
		}
		protected virtual Control FindFirstControl(ArrayList list, int startIndex) {
			int i = startIndex;
			while(i < list.Count) {
				LayoutControlItem lci = list[i] as LayoutControlItem;
				if(lci != null && lci.Control != null) return lci.Control;
				i++;
			}
			return null;
		}
		public virtual Control GetNextControl(object current) {
			Control currentControl = current as Control;
			BaseLayoutItem bItem = current as BaseLayoutItem;
			if(currentControl != null) {
				bItem = layoutControl.GetItemByControl(currentControl);
				if(bItem == null) return null;
			}
			if(bItem != null) {
				ArrayList list = GetArrangedFocusList(false, false);
				if(list.Count > 0) {
					int index = list.IndexOf(bItem);
					if(index >= 0) {
						return FindFirstControl(list, index + 1);
					}
				}
			}
			return null;
		}
		public virtual Control GetFirstControl() {
			ArrayList list = GetArrangedFocusList(false, false);
			if(list.Count > 0) {
				return FindFirstControl(list, 0);
			}
			return null;
		}
		public virtual bool ProcessMnemonic(char key) {
			List<BaseLayoutItem> items = shortcutManager.GetNextItemByKey(focusedComponentCore as LayoutControlItem, key);
			BaseLayoutItem item;
			switch(items.Count) {
				case 0: item = null; break;
				case 1: item = items[0]; break;
				default:
					ArrayList list = GetArrangedFocusList(false, false);
					int index = list.IndexOf(SelectedComponent);
					BaseLayoutItem nearestItem = items[0];
					int nearestItemDistance = int.MaxValue;
					foreach(BaseLayoutItem temp in items) {
						int candidateIndex = list.IndexOf(temp);
						int distance = candidateIndex - index;
						if(distance > 0 && distance < nearestItemDistance) {
							nearestItemDistance = distance;
							nearestItem = temp;
						}
					}
					item = nearestItem;
					break;
			}
			if(item != null) {
				LayoutGroup layoutGroup = item as LayoutGroup;
				if(layoutGroup != null && layoutGroup.ParentTabbedGroup != null) {
					layoutGroup.ParentTabbedGroup.SelectedTabPage = layoutGroup;
					item = layoutGroup.ParentTabbedGroup;
					if(layoutControl.OptionsFocus.AllowFocusControlOnActivatedTabPage) FocusFirstInGroup(layoutGroup, false);
					else FocusElement(item, false);
				}
				else
					if(layoutGroup != null)
						FocusFirstInGroup(layoutGroup, false);
					else FocusElement(item, false);
				return true;
			}
			else
				return false;
		}
		public virtual void ProcessOnKeyDown(Keys keyData) {
			if(((ILayoutControl)layoutControl).EnableCustomizationMode) return;
			Keys keyCode = keyData & Keys.KeyCode;
			switch(keyCode) {
				case Keys.Space:
					ChangeExpand();
					break;
				case Keys.Subtract:
					ChangeExpand(false);
					break;
				case Keys.Add:
					ChangeExpand(true);
					break;
				case Keys.Left:
					if(layoutControl != null && WindowsFormsSettings.GetIsRightToLeft(layoutControl.Control)) SwapTab(false); 
					else SwapTab(true);
					break;
				case Keys.Right:
					if(layoutControl != null && WindowsFormsSettings.GetIsRightToLeft(layoutControl.Control)) SwapTab(true);
					else SwapTab(false);
					break;
				case Keys.Home:
					SwapTabToEdge(true);
					break;
				case Keys.End:
					SwapTabToEdge(false);
					break;
			}
		}
		protected virtual bool ProcessControlTab() {
			if(!SwapParentTab(MoveBack))
				return SwapTab(MoveBack);
			return true;
		}
		[Obsolete("Use ProcessDialogKey method instead")]
		public virtual bool ProcessKey(Keys keyData) {
			return ProcessDialogKey(keyData);
		}
		public bool IsSingleFocusManagerOnTheForm(Control parent) {
			if(parent == null) return true;
			if(parent != null && parent.Controls.Count == 1 && IsSingleFocusManagerOnTheForm(parent.Parent)) return true;
			return false;
		}
		public virtual bool ProcessDialogKey(Keys keyData) {
			if(((ILayoutControl)layoutControl).EnableCustomizationMode) return false;
			Keys keyCode = keyData & Keys.KeyCode;
			switch(keyCode) {
				case Keys.Tab:
					MoveBack = (keyData & Keys.Shift) == Keys.Shift;
					bool result = false;
					if(((keyData & Keys.Control) != Keys.Control)) {
						if(!layoutControl.OptionsFocus.EnableAutoTabOrder) return false;
						result = FocusNextComponent(true);
					}
					else {
						if((layoutControl as LayoutControl).ContainsFocus)
							return ProcessControlTab();
						else
							result = FocusNextComponent(true);
					}
					if(!result) SelectedComponent = null;
					return result;
				case Keys.Left:
				case Keys.Right:
					if(SelectedComponent is TabbedGroup) return false;
					if(layoutControl.OptionsFocus.EnableAutoTabOrder) {
						MoveBack = keyCode == Keys.Left;
						if(layoutControl != null && WindowsFormsSettings.GetIsRightToLeft(layoutControl.Control)) MoveBack = !MoveBack;
						FocusNextComponent(true);
						return true;
					}
					else return false;
				case Keys.Up:
				case Keys.Down:
					bool altPressed = (keyData & Keys.Alt) == Keys.Alt;
					if(layoutControl.OptionsFocus.EnableAutoTabOrder && !altPressed) {
						MoveBack = keyCode == Keys.Up;
						if(layoutControl != null && WindowsFormsSettings.GetIsRightToLeft(layoutControl.Control)) MoveBack = !MoveBack;
						FocusNextComponent(true);
						return true;
					}
					else return false;
				case Keys.PageDown:
					ScrollPage(false);
					break;
				case Keys.PageUp:
					ScrollPage(true);
					break;
			}
			return false;
		}
	}
	[ToolboxItem(false)]
	public class FakeFocusContainer : Control {
		protected ILayoutControl owner;
		public FakeFocusContainer(ILayoutControl owner) {
			this.owner = owner;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			owner.ActiveHandler.OnKeyDown(this, e);
		}
		protected virtual bool IsAllowedDialogKey(Keys keyData) {
			if(owner.FocusHelper.SelectedComponent is TabbedGroup && (keyData == Keys.Left || keyData == Keys.Right)) return false;
			return true;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(this.ContainsFocus && IsAllowedDialogKey(keyData)) return base.ProcessDialogKey(keyData);
			return false;
		}
	}
}
