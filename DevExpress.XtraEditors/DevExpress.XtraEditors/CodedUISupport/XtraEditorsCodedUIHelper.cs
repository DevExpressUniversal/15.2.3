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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.Utils.CodedUISupport;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraEditors.CodedUISupport {
	public class XtraEditorsCodedUIHelper : IXtraEditorsCodedUIHelper {
		RemoteObject remoteObject;
		EventHandler editValueChangedEventHandler;
		public XtraEditorsCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
			editValueChangedEventHandler = new EventHandler(baseEdit_EditValueChanged);
		}
		public void ListenEditValueChanged(IntPtr windowHandle) {
			BaseEdit baseEdit = Control.FromHandle(windowHandle) as BaseEdit;
			if(baseEdit != null) {
				baseEdit.BeginInvoke(new MethodInvoker(delegate {
					baseEdit.EditValueChanged -= editValueChangedEventHandler;
					baseEdit.EditValueChanged += editValueChangedEventHandler;
				}));
			}
		}
		void baseEdit_EditValueChanged(object sender, EventArgs e) {
			try {
				remoteObject.ServerSideHelper.SaveEditValue((sender as BaseEdit).Handle, new ValueStruct((sender as BaseEdit).EditValue));
			}
			catch {
			}
		}
		public int GetButtonEditButtonIndex(IntPtr buttonEditHandle, int pointX, int pointY) {
			ButtonEdit buttonEditControl = Control.FromHandle(buttonEditHandle) as ButtonEdit;
			if(buttonEditControl == null) return -1;
			Point clientPoint = new Point(pointX, pointY);
			ButtonEditViewInfo viewInfo = buttonEditControl.ViewInfo;
			if(viewInfo == null) return -1;
			EditorButtonObjectInfoArgs button = viewInfo.ButtonInfoByPoint(clientPoint);
			if(button != null)
				return button.Button.Index;
			return -1;
		}
		public string GetButtonEditButtonRectangle(IntPtr buttonEditHandle, int buttonIndex) {
			ButtonEdit buttonEditControl = Control.FromHandle(buttonEditHandle) as ButtonEdit;
			if(buttonEditControl == null) return null;
			if(buttonEditControl.Properties.Buttons.Count <= buttonIndex || buttonIndex < 0) return null;
			ButtonEditViewInfo viewInfo = buttonEditControl.ViewInfo;
			if(viewInfo == null) return null;
			EditorButtonObjectInfoArgs buttonEditButton = viewInfo.ButtonInfoByButton(buttonEditControl.Properties.Buttons[buttonIndex]);
			if(buttonEditButton == null) return null;
			return CodedUIUtils.ConvertToString(buttonEditButton.Bounds);
		}
		public int GetBaseListBoxControlItemIndex(IntPtr baseListBoxControlHandle, int pointX, int pointY) {
			BaseListBoxControl baseListBoxControl = Control.FromHandle(baseListBoxControlHandle) as BaseListBoxControl;
			if(baseListBoxControl == null) return -1;
			Point clientPoint = new Point(pointX, pointY);
			DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo item = baseListBoxControl.ViewInfo.GetItemInfoByPoint(clientPoint);
			if(item == null) return -1;
			return item.Index;
		}
		public string GetBaseListBoxControlItemRectangle(IntPtr baseListBoxControlHandle, int itemIndex) {
			BaseListBoxControl baseListBoxControl = Control.FromHandle(baseListBoxControlHandle) as BaseListBoxControl;
			if(baseListBoxControl == null) return null;
			if(baseListBoxControl.ItemCount <= itemIndex || itemIndex < 0) return null;
			DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo item = baseListBoxControl.ViewInfo.GetItemByIndex(itemIndex);
			if(item == null) return null;
			Rectangle itemRect = item.Bounds;
			if(itemRect == Rectangle.Empty) return null;
			return CodedUIUtils.ConvertToString(itemRect);
		}
		public void SetCheckedComboBoxEditCheckedIndices(IntPtr windowHandle, int[] checkedIndices) {
			CheckedComboBoxEdit comboBox = Control.FromHandle(windowHandle) as CheckedComboBoxEdit;
			SetCheckedComboBoxEditCheckedIndices(comboBox, checkedIndices);
		}
		public void SetCheckedComboBoxEditCheckedItems(IntPtr windowHandle, string[] checkedItems) {
			CheckedComboBoxEdit comboBox = Control.FromHandle(windowHandle) as CheckedComboBoxEdit;
			if(comboBox != null) {
				List<int> checkedIndices = new List<int>();
				CheckedListBoxItemCollection items = comboBox.Properties.GetItems();
				foreach(string checkedItem in checkedItems)
					for(int i = 0; i < items.Count; i++)
						if(checkedItem == CodedUIUtils.ConvertToString(items[i].Value)) {
							checkedIndices.Add(i);
							break;
						}
				SetCheckedComboBoxEditCheckedIndices(comboBox, checkedIndices.ToArray());
			}
		}
		void SetCheckedComboBoxEditCheckedIndices(CheckedComboBoxEdit comboBox, int[] checkedIndices) {
			if(comboBox != null)
				comboBox.BeginInvoke(new MethodInvoker(delegate() {
					bool popupWasOpened = comboBox.IsPopupOpen;
					if(popupWasOpened)
						comboBox.CancelPopup();
					CheckedListBoxItemCollection items = comboBox.Properties.GetItems();
					foreach(CheckedListBoxItem item in items)
						item.CheckState = CheckState.Unchecked;
					foreach(int index in checkedIndices)
						items[index].CheckState = CheckState.Checked;
					comboBox.Properties.SynchronizeEditValue();
					if(popupWasOpened)
						comboBox.ShowPopup();
				}));
		}
		public void SetCheckedListBoxCheckedIndices(IntPtr windowHandle, int[] indices) {
			CheckedListBoxControl control = Control.FromHandle(windowHandle) as CheckedListBoxControl;
			SetCheckedListBoxCheckedIndices(control, indices);
		}
		public void SetCheckedListBoxCheckedItems(IntPtr windowHandle, string[] checkedItems) {
			CheckedListBoxControl listBox = Control.FromHandle(windowHandle) as CheckedListBoxControl;
			if(listBox == null) return;
			List<int> indices = new List<int>();
			foreach(string itemText in checkedItems)
				for(int i = 0; i < listBox.ItemCount; i++)
					if(itemText == listBox.GetItemText(i)) {
						indices.Add(i);
						break;
					}
			SetCheckedListBoxCheckedIndices(listBox, indices.ToArray());
		}
		protected void SetCheckedListBoxCheckedIndices(CheckedListBoxControl listBox, int[] indices) {
			if(listBox == null) return;
			listBox.BeginInvoke(new MethodInvoker(delegate() {
				DevExpress.XtraEditors.BaseListBoxControl.SelectedIndexCollection selectedIndices = listBox.SelectedIndices;
				SetCheckedListBoxItemsChecked(listBox, false);
				foreach(int index in indices)
					listBox.SetItemChecked(index, true);
				foreach(int index in selectedIndices)
					listBox.SetSelected(index, true);
			}));
		}
		protected internal virtual void SetCheckedListBoxItemsChecked(CheckedListBoxControl control, bool check) {
			CheckState checkState = check ? CheckState.Checked : CheckState.Unchecked;
			List<int> list = new List<int>();
			for(int i = 0; i < control.ItemCount; i++) {
				ItemCheckingEventArgs itemCheckingEventArgs = new ItemCheckingEventArgs();
				CheckState itemCheckState = control.GetItemCheckState(i);
				if(itemCheckState != checkState) {
					itemCheckingEventArgs.Index = i;
					itemCheckingEventArgs.OldValue = itemCheckState;
					itemCheckingEventArgs.NewValue = checkState;
					control.GetType().GetMethod("RaiseItemChecking", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(control, new object[] { itemCheckingEventArgs });
					if(!itemCheckingEventArgs.Cancel && itemCheckingEventArgs.NewValue != itemCheckingEventArgs.OldValue) {
						control.GetType().GetMethod("SetItemCheckStateCore", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(control, new object[] { i, itemCheckingEventArgs.NewValue });
						list.Add(i);
					}
				}
			}
			control.LayoutChanged();
			DevExpress.XtraEditors.Controls.ItemCheckEventArgs itemCheckEventArgs = new DevExpress.XtraEditors.Controls.ItemCheckEventArgs();
			for(int i = 0; i < list.Count; i++) {
				itemCheckEventArgs.Index = list[i];
				itemCheckEventArgs.State = control.GetItemCheckState(list[i]);
				control.GetType().GetMethod("RaiseItemCheck", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(control, new object[] { itemCheckEventArgs });
			}
		}
		public ZoomTrackBarButtons GetZoomTrackBarControlButton(IntPtr windowHandle, int pointX, int pointY) {
			ZoomTrackBarControl zoomTrackBarControl = Control.FromHandle(windowHandle) as ZoomTrackBarControl;
			if(zoomTrackBarControl == null) return ZoomTrackBarButtons.None;
			Point clientPoint = new Point(pointX, pointY);
			ZoomTrackBarViewInfo viewInfo = zoomTrackBarControl.ViewInfo as ZoomTrackBarViewInfo;
			if(viewInfo.ZoomInButtonInfo.Bounds.Contains(clientPoint))
				return ZoomTrackBarButtons.ZoomIn;
			if(viewInfo.ZoomOutButtonInfo.Bounds.Contains(clientPoint))
				return ZoomTrackBarButtons.ZoomOut;
			return ZoomTrackBarButtons.None;
		}
		public string GetZoomTrackBarControlButtonRectangle(IntPtr windowHandle, ZoomTrackBarButtons zoomTrackBarButton) {
			ZoomTrackBarControl zoomTrackBarControl = Control.FromHandle(windowHandle) as ZoomTrackBarControl;
			if(zoomTrackBarControl == null) return null;
			ZoomTrackBarViewInfo viewInfo = zoomTrackBarControl.ViewInfo as ZoomTrackBarViewInfo;
			Rectangle buttonRectangle = Rectangle.Empty;
			if(zoomTrackBarButton == ZoomTrackBarButtons.ZoomIn)
				buttonRectangle = viewInfo.ZoomInButtonInfo.Bounds;
			else if(zoomTrackBarButton == ZoomTrackBarButtons.ZoomOut)
				buttonRectangle = viewInfo.ZoomOutButtonInfo.Bounds;
			if(buttonRectangle != Rectangle.Empty)
				return CodedUIUtils.ConvertToString(buttonRectangle);
			return null;
		}
		public void SetRangeTrackBarControlValue(IntPtr windowHandle, int valueMin, int valueMax) {
			RangeTrackBarControl rangeTrackBarControl = Control.FromHandle(windowHandle) as RangeTrackBarControl;
			if(rangeTrackBarControl == null) return;
			TrackBarRange newValue = new TrackBarRange(valueMin, valueMax);
			rangeTrackBarControl.BeginInvoke(new MethodInvoker(delegate() {
				rangeTrackBarControl.Value = newValue;
			}));
		}
		public int GetNavigatorBaseButtonIndex(IntPtr windowHandle, int pointX, int pointY) {
			NavigatorBase navigatorBase = Control.FromHandle(windowHandle) as NavigatorBase;
			if(navigatorBase == null) return -1;
			Point clientPoint = new Point(pointX, pointY);
			NavigatorButtonsViewInfo viewInfo = navigatorBase.ButtonsCore.ViewInfo;
			for(int index = 0; index < viewInfo.ButtonCollection.Count; index++) {
				NavigatorButtonViewInfo button = viewInfo.ButtonCollection[index];
				if(button.Bounds.Contains(clientPoint)) return index;
			}
			return -1;
		}
		public string GetNavigatorBaseButtonRectangle(IntPtr windowHandle, int buttonIndex) {
			NavigatorBase navigatorBase = Control.FromHandle(windowHandle) as NavigatorBase;
			if(navigatorBase == null) return null;
			NavigatorButtonsViewInfo viewInfo = navigatorBase.ButtonsCore.ViewInfo;
			if(buttonIndex < 0 && buttonIndex >= viewInfo.ButtonCollection.Count) return null;
			NavigatorButtonViewInfo button = viewInfo.ButtonCollection[buttonIndex];
			if(button != null)
				return CodedUIUtils.ConvertToString(button.Bounds);
			else return null;
		}
		public bool IsNavigatorBaseButtonEnabled(IntPtr windowHandle, int buttonIndex) {
			NavigatorBase navigatorBase = Control.FromHandle(windowHandle) as NavigatorBase;
			if(navigatorBase != null) {
				NavigatorButtonsViewInfo viewInfo = navigatorBase.ButtonsCore.ViewInfo;
				if(buttonIndex >= 0 && buttonIndex < viewInfo.ButtonCollection.Count) {
					NavigatorButtonViewInfo button = viewInfo.ButtonCollection[buttonIndex];
					if(button != null)
						return button.Enabled;
				}
			}
			return false;
		}
		public int GetRadioGroupItemIndex(IntPtr windowHandle, int pointX, int pointY) {
			RadioGroup radioGroup = Control.FromHandle(windowHandle) as RadioGroup;
			if(radioGroup == null) return -1;
			return radioGroup.ViewInfo.GetItemIndexByPoint(new Point(pointX, pointY));
		}
		public bool IsColorDialogOpeningClick(IntPtr windowHandle, int pointX, int pointY) {
			Popup.ColorCellsControl control = Control.FromHandle(windowHandle) as Popup.ColorCellsControl;
			if(control == null) return false;
			Point clientPoint = new Point(pointX, pointY);
			Popup.ColorCellsControlViewInfo viewInfo = control.ViewInfo as Popup.ColorCellsControlViewInfo;
			if(viewInfo == null) return false;
			int cellIndex = viewInfo.GetCellIndex(clientPoint);
			if(cellIndex >= Popup.ColorCellsControlViewInfo.CellColors.Length && control.Properties.ShowColorDialog)
				return true;
			return false;
		}
		public IntPtr GetFilterControlActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int nodeIndex, int elementIndex) {
			FilterControl filterControl = Control.FromHandle(windowHandle) as FilterControl;
			if(filterControl == null) return IntPtr.Zero;
			if(filterControl.FocusInfo.ElementIndex != elementIndex || filterControl.FocusInfo.Node.Index != nodeIndex) {
				Node focusedNode = GetNodeFromIndex(filterControl.Model.RootNode, nodeIndex);
				if(focusedNode == null || elementIndex < 0 || elementIndex >= focusedNode.Elements.Count - 1) return IntPtr.Zero;
				FilterControlFocusInfo focusInfo = new FilterControlFocusInfo(focusedNode, elementIndex);
				filterControl.BeginInvoke(new MethodInvoker(delegate() {
					filterControl.DisposeActiveEditor();
					filterControl.FocusInfo = focusInfo;
					filterControl.CreateActiveEditor();
				}));
				return IntPtr.Zero;
			}
			else {
				if(filterControl.ActiveEditor != null)
					return filterControl.ActiveEditor.Handle;
				else {
					filterControl.BeginInvoke(new MethodInvoker(delegate() {
						filterControl.CreateActiveEditor();
					}));
					return IntPtr.Zero;
				}
			}
		}
		protected Node GetNodeFromIndex(GroupNode groupNode, int index) {
			foreach(Node subNode in groupNode.SubNodes) {
				if(subNode.Index == index)
					return subNode;
				else if(subNode is GroupNode) {
					Node node = GetNodeFromIndex(subNode as GroupNode, index);
					if(node != null)
						return node;
				}
			}
			return null;
		}
		public void SetFilterControlFilterCriteria(IntPtr windowHandle, string criteriaString) {
			FilterControl filterControl = Control.FromHandle(windowHandle) as FilterControl;
			if(filterControl != null) {
				filterControl.BeginInvoke(new MethodInvoker(delegate() {
					filterControl.FocusInfo = new FilterControlFocusInfo(filterControl.RootNode, 0);
					CriteriaOperator criteria = filterControl.Model.CriteriaParse(criteriaString);
					filterControl.Model.CreateTree(criteria);
				}));
			}
		}
		public int GetXtraTabControlHeaderIndex(IntPtr windowHandle, int pointX, int pointY) {
			XtraTabControl control = Control.FromHandle(windowHandle) as XtraTabControl;
			if(control == null) return -1;
			DevExpress.XtraTab.ViewInfo.XtraTabHitInfo hitInfo = control.CalcHitInfo(new Point(pointX, pointY));
			if(hitInfo.HitTest == DevExpress.XtraTab.ViewInfo.XtraTabHitTest.PageHeader) {
				for(int i = 0; i < control.TabPages.Count; i++)
					if(control.TabPages[i] == hitInfo.Page)
						return i;
			}
			return -1;
		}
		public string GetXtraTabControlHeaderRectangle(IntPtr windowHandle, int pageIndex) {
			XtraTabControl control = Control.FromHandle(windowHandle) as XtraTabControl;
			if(control == null) return null;
			if(pageIndex >= 0 && pageIndex < control.TabPages.Count) {
				XtraTabPage page = control.TabPages[pageIndex];
				foreach(BaseTabPageViewInfo pageInfo in control.ViewInfo.HeaderInfo.AllPages) {
					if(pageInfo != null && pageInfo.Page == page) {
						return CodedUIUtils.ConvertToString(pageInfo.Bounds);
					}
				}
			}
			return null;
		}
		public bool IsXtraTabControlHeaderEnabled(IntPtr windowHandle, int pageIndex) {
			XtraTabControl control = Control.FromHandle(windowHandle) as XtraTabControl;
			if(control != null) {
				if(pageIndex >= 0 && pageIndex < control.TabPages.Count) {
					XtraTabPage page = control.TabPages[pageIndex];
					if(page != null)
						return page.PageEnabled;
				}
			}
			return false;
		}
		public void SetMRUEditItems(IntPtr windowHandle, string[] mRUItems) {
			MRUEdit control = Control.FromHandle(windowHandle) as MRUEdit;
			if(control != null) {
				control.BeginInvoke(new MethodInvoker(delegate() {
					control.Properties.Items.Clear();
					for(int index = mRUItems.Length - 1; index >= 0; index--)
						control.Properties.Items.Add(mRUItems[index]);
				}));
			}
		}
		public void SetColorEditColor(IntPtr windowHandle, string colorAsString) {
			ColorEdit colorEdit = Control.FromHandle(windowHandle) as ColorEdit;
			if(colorEdit != null) {
				Color newColor = CodedUIUtils.ConvertFromString<Color>(colorAsString);
				int i;
				if(colorAsString.Contains(",") && Int32.TryParse(colorAsString.Substring(colorAsString.LastIndexOf(",") + 2), out i))
					newColor = Color.FromArgb(newColor.ToArgb());
				colorEdit.BeginInvoke(new MethodInvoker(delegate {
					colorEdit.Color = newColor;
				}));
			}
		}
		public int[] GetCheckedComboBoxEditCheckedIndices(IntPtr checkedComboBoxEditHandle, IntPtr listBoxHandle) {
			CheckedComboBoxEdit comboBox = Control.FromHandle(checkedComboBoxEditHandle) as CheckedComboBoxEdit;
			CheckedListBoxControl listBox = Control.FromHandle(listBoxHandle) as CheckedListBoxControl;
			if(comboBox != null) {
				List<int> indices = new List<int>();
				if(comboBox.IsPopupOpen && listBox != null) {
					int customItemsShift = comboBox.Properties.SelectAllItemVisible ? 1 : 0;
					for(int i = 0; i < comboBox.Properties.Items.Count; i++)
						if(listBox.Items[i + customItemsShift].CheckState == CheckState.Checked)
							indices.Add(i);
				}
				else
					for(int i = 0; i < comboBox.Properties.Items.Count; i++)
						if(comboBox.Properties.Items[i].CheckState == CheckState.Checked)
							indices.Add(i);
				return indices.ToArray();
			}
			return null;
		}
		public int[] GetCheckedListBoxCheckedIndices(IntPtr windowHandle) {
			BaseCheckedListBoxControl listBox = Control.FromHandle(windowHandle) as CheckedListBoxControl;
			List<int> checkedIndices = new List<int>();
			if(listBox != null) {
				if(listBox.CheckedIndices.Count > 0) {
					for(int i = 0; i < listBox.CheckedIndices.Count; i++)
						checkedIndices.Add(listBox.CheckedIndices[i]);
				}
				else {
					for(int i = 0; i < listBox.Items.Count; i++)
						if(listBox.Items[i].CheckState == CheckState.Checked)
							checkedIndices.Add(i);
				}
			}
			return checkedIndices.ToArray();
		}
		public IntPtr GetXtraTabControlPageHandleOrSetPageSelected(IntPtr windowHandle, string pageName) {
			XtraTabControl control = Control.FromHandle(windowHandle) as XtraTabControl;
			if(control != null) {
				XtraTabPage tabPage = null;
				foreach(XtraTabPage page in control.TabPages)
					if(page.Name == pageName) {
						tabPage = page;
						break;
					}
				if(tabPage == null)
					if(pageName.Contains("[") && pageName.Contains("]")) {
						try {
							string pageIndexAsString = pageName.Substring(pageName.LastIndexOf("[") + 1, pageName.LastIndexOf("]") - pageName.LastIndexOf("[") - 1);
							int pageIndex = Int32.Parse(pageIndexAsString);
							tabPage = control.TabPages[pageIndex];
						}
						catch { }
					}
				if(tabPage != null) {
					if(control.SelectedTabPage != tabPage)
						control.BeginInvoke(new MethodInvoker(delegate() {
							control.SelectedTabPage = tabPage;
						}));
					else if(tabPage.IsHandleCreated)
						return tabPage.Handle;
				}
			}
			return IntPtr.Zero;
		}
		public void SetDateTimeEditValue(IntPtr windowHandle, string valueAsString) {
			Control control = Control.FromHandle(windowHandle);
			if(control is BaseEdit) {
				control.BeginInvoke(new MethodInvoker(delegate() {
					if(valueAsString == null) {
						(control as BaseEdit).EditValue = null;
						return;
					}
					DateTime value = CodedUIUtils.ConvertFromString<DateTime>(valueAsString);
					if(control is DateEdit) {
						DateEdit editor = control as DateEdit;
						DateTime newValue;
						if(value.TimeOfDay != TimeSpan.FromMilliseconds(0))
							newValue = value;
						else {
							newValue = new DateTime(value.Year, value.Month, value.Day, editor.DateTime.Hour, editor.DateTime.Minute, editor.DateTime.Second, editor.DateTime.Millisecond);
						}
						editor.DateTime = newValue;
						if(editor.IsPopupOpen) {
							DevExpress.XtraEditors.Popup.PopupDateEditForm popup = editor.PopupForm as DevExpress.XtraEditors.Popup.PopupDateEditForm;
							if(popup != null && popup.Calendar != null)
								popup.Calendar.ResetState(editor.DateTime, editor.DateTime);
						}
					}
					else if(control is TimeEdit) {
						TimeEdit editor = control as TimeEdit;
						if(value.Date != DateTime.MinValue)
							editor.Time = value;
						else {
							editor.Time = new DateTime(editor.Time.Year, editor.Time.Month, editor.Time.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
						}
					}
				}));
			}
		}
		public void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember) {
			LookUpEditBase lookUpBase = Control.FromHandle(windowHandle) as LookUpEditBase;
			if(lookUpBase is LookUpEdit)
				SetLookUpEditValue(lookUpBase as LookUpEdit, value, byDisplayMember);
			else
				SetLookUpEditBaseValue(lookUpBase, value, null, byDisplayMember);
		}
		public void SetLookUpEditValue(LookUpEdit lookUp, ValueStruct value, bool byDisplayMember) {
			if(lookUp != null)
				SetLookUpEditBaseValue(lookUp, value, (GetLookUpValueByIndexDelegate)(lookUp).Properties.GetDataSourceValue, byDisplayMember);
		}
		public delegate object GetLookUpValueByIndexDelegate(string fieldName, int rowIndex);
		public void SetLookUpEditBaseValue(LookUpEditBase lookUpBase, ValueStruct value, GetLookUpValueByIndexDelegate getValueByIndexDelegate, bool byDisplayMember) {
			if(lookUpBase != null)
				lookUpBase.BeginInvoke(new MethodInvoker(delegate() {
					if(lookUpBase.IsPopupOpen)
						lookUpBase.CancelPopup();
					object convertedValue = CodedUIUtils.GetValue(value);
					if(getValueByIndexDelegate == null ||
						(byDisplayMember && string.IsNullOrEmpty(lookUpBase.Properties.DisplayMember) && (convertedValue == null || convertedValue.GetType().IsValueType || value.ValueTypeName == typeof(string).FullName)) ||
						(!byDisplayMember && (convertedValue == null || convertedValue.GetType().IsValueType || value.ValueTypeName == typeof(string).FullName))
						)
						lookUpBase.EditValue = convertedValue;
					else {
						string valueMember = lookUpBase.Properties.ValueMember == null ? String.Empty : lookUpBase.Properties.ValueMember;
						string displayMember = lookUpBase.Properties.DisplayMember == null ? String.Empty : lookUpBase.Properties.DisplayMember;
						int itemsCount = -1;
						if(lookUpBase.Properties.DataSource is IList)
							itemsCount = (lookUpBase.Properties.DataSource as IList).Count;
						else if(lookUpBase.Properties.DataSource is ITypedList) {
							PropertyDescriptorCollection pdc = (lookUpBase.Properties.DataSource as ITypedList).GetItemProperties(null);
							if(pdc != null)
								itemsCount = pdc.Count;
						}
						else if(lookUpBase.Properties.DataSource is IListSource) {
							IList list = (lookUpBase.Properties.DataSource as IListSource).GetList();
							if(list != null)
								itemsCount = list.Count;
						}
						int maxItemsToCompareCount = 100000;
						int maxNullValuesCount = 1000;
						int itemsToCompareCount = itemsCount == -1 ? maxItemsToCompareCount : itemsCount;
						int nullValuesCount = 0;
						for(int i = 0; i < itemsToCompareCount; i++) {
							object valueByIndex = getValueByIndexDelegate(byDisplayMember ? displayMember : valueMember, i);
							if(CodedUIUtils.ConvertToString(valueByIndex) == value.ValueAsString)
								if(valueByIndex.GetType().FullName == value.ValueTypeName) {
									lookUpBase.EditValue = byDisplayMember ? getValueByIndexDelegate(valueMember, i) : valueByIndex;
									return;
								}
							if(valueByIndex == null)
								nullValuesCount++;
							if(itemsCount == -1 && nullValuesCount > maxNullValuesCount)
								break;
						}
						lookUpBase.EditValue = convertedValue;
					}
				}));
		}
		public int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet) {
			LookUpEditBase lookUpBase = Control.FromHandle(windowHandle) as LookUpEditBase;
			if(lookUpBase != null) {
				if(isSet)
					lookUpBase.BeginInvoke(new MethodInvoker(delegate() {
						if(lookUpBase is LookUpEdit)
							(lookUpBase as LookUpEdit).ItemIndex = newValue;
					}));
				else
					if(lookUpBase is LookUpEdit)
						return (lookUpBase as LookUpEdit).ItemIndex;
			}
			return -1;
		}
		public void SetListBoxSelectedItems(IntPtr windowHandle, string[] selectedItems) {
			Control control = Control.FromHandle(windowHandle);
			if(control is BaseListBoxControl) {
				control.BeginInvoke(new MethodInvoker(delegate() {
					BaseListBoxControl listBox = control as BaseListBoxControl;
					listBox.SelectedIndices.Clear();
					foreach(string itemText in selectedItems)
						for(int i = 0; i < listBox.ItemCount; i++)
							if(itemText == listBox.GetItemText(i)) {
								listBox.SelectedIndices.AddRemove(i);
								break;
							}
				}));
			}
		}
	}
}
