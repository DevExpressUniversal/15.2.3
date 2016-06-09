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
using DevExpress.Utils.CodedUISupport;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.ViewInfo;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.CodedUISupport {
	class XtraLayoutCodedUIHelper : IXtraLayoutCodedUIHelper {
		RemoteObject remoteObject;
		public XtraLayoutCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public LayoutControlElementInfo GetLayoutControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			LayoutControlElementInfo result = new LayoutControlElementInfo();
			LayoutControl layoutControl = Control.FromHandle(windowHandle) as LayoutControl;
			if(layoutControl != null) {
				BaseLayoutItemHitInfo hitInfo = layoutControl.CalcHitInfo(new Point(pointX, pointY));
				if(hitInfo.TabPageIndex >= 0 && hitInfo.Item as TabbedGroup != null) {
					BaseLayoutItem item = (hitInfo.Item as TabbedGroup).TabPages[hitInfo.TabPageIndex];
					result = GetElementInfo(item);
					result.ElementType = LayoutControlElements.TabbedGroupTabHeader;
				}
				else if(hitInfo.IsExpandButton) {
					result = GetElementInfo(hitInfo.Item);
					result.ElementType = LayoutControlElements.LayoutGroupExpandButton;
				}
				else result = GetElementInfo(hitInfo.Item);
			}
			return result;
		}
		LayoutControlElementInfo GetElementInfo(BaseLayoutItem item) {
			LayoutControlElementInfo result = new LayoutControlElementInfo();
			if(item == null)
				return result;
			result.Name = GetItemName(item);
			if(item is LayoutGroup)
				result.ElementType = LayoutControlElements.LayoutGroup;
			else if(item is TabbedGroup)
				result.ElementType = LayoutControlElements.TabbedGroup;
			else if(item is LayoutControlItem)
				result.ElementType = LayoutControlElements.LayoutControlItem;
			else
				result.ElementType = LayoutControlElements.LayoutItem;
			BaseLayoutItem parent = item.Parent;
			if(item is LayoutGroup && (item as LayoutGroup).ParentTabbedGroup != null)
				parent = (item as LayoutGroup).ParentTabbedGroup;
			if(parent != null)
				result.Parent = GetElementInfo(parent);
			return result;
		}
		string GetItemName(BaseLayoutItem item) {
			return item.Name;
		}
		BaseLayoutItem GetItemFromElementInfo(LayoutControl layoutControl, LayoutControlElementInfo elementInfo) {
			List<LayoutControlElementInfo> hierarchyArray = new List<LayoutControlElementInfo>();
			hierarchyArray.Insert(0, elementInfo);
			while(hierarchyArray[0].Parent is LayoutControlElementInfo && ((LayoutControlElementInfo)hierarchyArray[0].Parent).ElementType != LayoutControlElements.Unknown)
				hierarchyArray.Insert(0, (LayoutControlElementInfo)hierarchyArray[0].Parent);
			BaseLayoutItem currentItem = GetItemFromCollection(layoutControl.Items, hierarchyArray[0]);
			for(int i = 1; i < hierarchyArray.Count; i++) {
				if(currentItem is TabbedGroup)
					currentItem = GetItemFromCollection((currentItem as TabbedGroup).TabPages, hierarchyArray[i]);
				else if(currentItem is LayoutGroup)
					currentItem = GetItemFromCollection((currentItem as LayoutGroup).Items, hierarchyArray[i]);
				else break;
			}
			return currentItem;
		}
		BaseLayoutItem GetItemFromCollection(BaseItemCollection items, LayoutControlElementInfo elementInfo) {
			foreach(BaseLayoutItem item in items)
				if(GetItemName(item) == elementInfo.Name)
					return item;
			return null;
		}
		public string GetLayoutControlElementRectangle(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			LayoutControl layoutControl = Control.FromHandle(windowHandle) as LayoutControl;
			if(layoutControl != null) {
				Rectangle rectangle = GetLayoutControlElementRectangle(layoutControl, elementInfo);
				if(rectangle != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rectangle);
			}
			return null;
		}
		Rectangle GetLayoutControlElementRectangle(LayoutControl layoutControl, LayoutControlElementInfo elementInfo) {
			BaseLayoutItem item = GetItemFromElementInfo(layoutControl, elementInfo);
			if(item != null) {
				if(elementInfo.ElementType == LayoutControlElements.TabbedGroupTabHeader && item is LayoutGroup) {
					LayoutGroup group = item as LayoutGroup;
					if(group.ParentTabbedGroup != null)
						return group.ParentTabbedGroup.ViewInfo.GetScreenTabCaptionRect(group.ParentTabbedGroup.TabPages.IndexOf(group));
				}
				else if(elementInfo.ElementType == LayoutControlElements.LayoutGroupExpandButton && item is LayoutGroup) {
					return (item as LayoutGroup).ViewInfo.BorderInfo.ButtonBounds;
				}
				else
					return item.ViewInfo.BoundsRelativeToControl;
			}
			return Rectangle.Empty;
		}
		public LayoutControlElementInfo GetControlParentItem(IntPtr windowHandle) {
			Control control = Control.FromHandle(windowHandle);
			if(control != null) {
				LayoutControl layoutControl = control.Parent as LayoutControl;
				if(layoutControl != null) {
					LayoutControlItem item = layoutControl.GetItemByControl(control);
					return GetElementInfo(item);
				}
			}
			return new LayoutControlElementInfo();
		}
		public IntPtr GetLayoutControlItemControlHandle(IntPtr layoutControlHandle, LayoutControlElementInfo elementInfo) {
			LayoutControl layoutControl = Control.FromHandle(layoutControlHandle) as LayoutControl;
			if(layoutControl == null)
				return IntPtr.Zero;
			BaseLayoutItem item = GetItemFromElementInfo(layoutControl, elementInfo);
			if(item is LayoutControlItem) {
				Control control = (item as LayoutControlItem).Control;
				if(control != null && control.IsHandleCreated)
					return control.Handle;
			}
			return IntPtr.Zero;
		}
		public ValueStruct GetLayoutControlElementProperty(IntPtr windowHandle, LayoutControlElementInfo elementInfo, LayoutControlElementsProperties property) {
			LayoutControl layoutControl = Control.FromHandle(windowHandle) as LayoutControl;
			if(layoutControl == null)
				return new ValueStruct();
			return new ValueStruct(GetLayoutControlElementProperty(layoutControl, elementInfo, property));
		}
		object GetLayoutControlElementProperty(LayoutControl layoutControl, LayoutControlElementInfo elementInfo, LayoutControlElementsProperties property) {
			BaseLayoutItem item = GetItemFromElementInfo(layoutControl, elementInfo);
			if(item != null)
				switch(property) {
					case LayoutControlElementsProperties.Visible:
						if(elementInfo.ElementType == LayoutControlElements.TabbedGroupTabHeader && item is LayoutGroup && (item as LayoutGroup).ParentTabbedGroup != null)
							return (item as LayoutGroup).ParentTabbedGroup.Visible;
						else return item.Visible;
					case LayoutControlElementsProperties.Selected:
						if(elementInfo.ElementType == LayoutControlElements.TabbedGroupTabHeader && item is LayoutGroup && (item as LayoutGroup).ParentTabbedGroup != null)
							return (item as LayoutGroup).ParentTabbedGroup.SelectedTabPage == item;
						else return item.Selected;
					case LayoutControlElementsProperties.Expanded:
						return item.Expanded;
					case LayoutControlElementsProperties.Text:
						return item.Text;
				}
			return null;
		}
		public void MakeVisible(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			LayoutControl layoutControl = Control.FromHandle(windowHandle) as LayoutControl;
			if(layoutControl == null)
				return;
			BaseLayoutItem item = GetItemFromElementInfo(layoutControl, elementInfo);
			if(item != null)
				layoutControl.BeginInvoke(new MethodInvoker(delegate() {
					layoutControl.FocusHelper.PlaceItemIntoView(item);
				}));
		}
	}
}
