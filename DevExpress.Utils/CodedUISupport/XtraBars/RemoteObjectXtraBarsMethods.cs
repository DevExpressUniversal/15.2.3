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
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.Utils.CodedUISupport {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoteObjectXtraBarsMethods : IXtraBarsCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraBarsMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public string GetLinkNameFromPoint(IntPtr windowHandle, int pointX, int pointY, out BarElementTypes itemType) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { itemType = BarElementTypes.Default; return null; }
			return clientSideHelper.BarsMethods.GetLinkNameFromPoint(windowHandle, pointX, pointY, out itemType);
		}
		public string GetLinkNameFromOldName(IntPtr windowHandle, string oldName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetLinkNameFromOldName(windowHandle, oldName);
		}
		public string GetLinkRectangleFromName(IntPtr windowHandle, string name) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetLinkRectangleFromName(windowHandle, name);
		}
		public string GetBarItemOpenArrowRectangle(IntPtr windowHandle, string linkName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetBarItemOpenArrowRectangle(windowHandle, linkName);
		}
		public IntPtr GetSubMenuBarControlHandleFromOwnerLink(IntPtr ownerLinkWindowHandle, string ownerLinkName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(ownerLinkWindowHandle);
			if(clientSideHelper == null)
				return IntPtr.Zero;
			return new IntPtr(clientSideHelper.BarsMethods.GetSubMenuBarControlHandleFromOwnerLink(ownerLinkWindowHandle, ownerLinkName));
		}
		public string GetGalleryItemCaptionFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { groupCaption = null; return null; }
			return clientSideHelper.BarsMethods.GetGalleryItemCaptionFromPoint(windowHandle, pointX, pointY, out groupCaption);
		}
		public string GetGalleryItemRectangleFromCaption(IntPtr windowHandle, string groupCaption, string itemCaption) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return null;
			return clientSideHelper.BarsMethods.GetGalleryItemRectangleFromCaption(windowHandle, groupCaption, itemCaption);
		}
		public string GetNameOfLinkFromProperty(IntPtr windowHandle, string propertyThatContainsLink) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return null;
			return clientSideHelper.BarsMethods.GetNameOfLinkFromProperty(windowHandle, propertyThatContainsLink);
		}
		public ValueStruct GetLinkProperty(IntPtr windowHandle, string name, XtraBarsPropertyNames property) {
			return HelperManager.Get(windowHandle) == null ? new ValueStruct() : HelperManager.Get(windowHandle).BarsMethods.GetLinkProperty(windowHandle, name, property);
		}
		public void SetLinkProperty(IntPtr windowHandle, string linkName, XtraBarsPropertyNames property, ValueStruct value) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return;
			clientSideHelper.BarsMethods.SetLinkProperty(windowHandle, linkName, property, value);
		}
		public IntPtr GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, string name) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return IntPtr.Zero;
			return new IntPtr(clientSideHelper.BarsMethods.GetLinkEditorHandleOrShowEditor(windowHandle, name));
		}
		public IntPtr GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return IntPtr.Zero;
			return new IntPtr(clientSideHelper.BarsMethods.GetLinkEditorHandleOrShowEditor(windowHandle, elementInfo));
		}
		public IntPtr GetBarActiveEditorHandle(IntPtr windowHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return IntPtr.Zero;
			return new IntPtr(clientSideHelper.BarsMethods.GetBarActiveEditorHandle(windowHandle));
		}
		public void CloseLinkEditors(IntPtr windowHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return;
			clientSideHelper.BarsMethods.CloseLinkEditors(windowHandle);
		}
		public AlertFormElements GetAlertFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string name) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { name = null; return AlertFormElements.Default; }
			return clientSideHelper.BarsMethods.GetAlertFormElementFromPoint(windowHandle, pointX, pointY, out name);
		}
		public string GetAlertFormElementRectangle(IntPtr windowHandle, AlertFormElements elementType, string elementName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null)
				return null;
			return clientSideHelper.BarsMethods.GetAlertFormElementRectangle(windowHandle, elementType, elementName);
		}
		public void SetBarDockInfo(IntPtr windowHandle, string dockStyle, int row, int column, int offset, string floatLocation, string standaloneBarDockControlName) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.SetBarDockInfo(windowHandle, dockStyle, row, column, offset, floatLocation, standaloneBarDockControlName);
		}
		public DockPanelDockInfo GetDockPanelLastDockingOperationInfo(IntPtr handleForIpcConnection, Guid dockPanelId) {
			return HelperManager.Get(handleForIpcConnection) == null ? new DockPanelDockInfo() : HelperManager.Get(handleForIpcConnection).BarsMethods.GetDockPanelLastDockingOperationInfo(handleForIpcConnection, dockPanelId);
		}
		public DockPanelDockInfo GetDockPanelCurrentDockInfo(IntPtr dockPanelHandle, Guid dockPanelId) {
			return HelperManager.Get(dockPanelHandle) == null ? new DockPanelDockInfo() : HelperManager.Get(dockPanelHandle).BarsMethods.GetDockPanelCurrentDockInfo(dockPanelHandle, dockPanelId);
		}
		public void SetDockPanelDockInfo(IntPtr windowHandle, Guid dockPanelId, DockPanelDockInfo dockInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.SetDockPanelDockInfo(windowHandle, dockPanelId, dockInfo);
		}
		public DockPanelElementInfo GetDockPanelElementInfoFromPoint(IntPtr windowHandle, int pointX, int PointY) {
			return HelperManager.Get(windowHandle) == null ? new DockPanelElementInfo() : HelperManager.Get(windowHandle).BarsMethods.GetDockPanelElementInfoFromPoint(windowHandle, pointX, PointY);
		}
		public string GetDockPanelButtonRectangle(IntPtr windowHandle, DockPanelElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetDockPanelButtonRectangle(windowHandle, elementInfo);
		}
		public string GetAutoHideContainerButtonName(IntPtr windowHandle, int pointX, int PointY) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetAutoHideContainerButtonName(windowHandle, pointX, PointY);
		}
		public string GetAutoHideContainerButtonRectangle(IntPtr windowHandle, string buttonName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetAutoHideContainerButtonRectangle(windowHandle, buttonName);
		}
		public IntPtr GetChildDockPanelHandleAndShowIt(IntPtr windowHandle, string panelName) {
			return HelperManager.Get(windowHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(windowHandle).BarsMethods.GetChildDockPanelHandleAndShowIt(windowHandle, panelName));
		}
		public RibbonElementSearchInfo GetRibbonElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new RibbonElementSearchInfo() : HelperManager.Get(windowHandle).BarsMethods.GetRibbonElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetRibbonElementName(IntPtr ribbonHandle, RibbonElementSearchInfo elementInfo) {
			return HelperManager.Get(ribbonHandle) == null ? null : HelperManager.Get(ribbonHandle).BarsMethods.GetRibbonElementName(ribbonHandle, elementInfo);
		}
		public string GetRibbonElementRectangle(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetRibbonElementRectangle(windowHandle, elementInfo);
		}
		public void MakeRibbonGalleryItemVisible(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.MakeRibbonGalleryItemVisible(windowHandle, elementInfo);
		}
		public void MakeGalleryItemVisible(IntPtr windowHandle, string groupCaption, string itemCaption) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.MakeGalleryItemVisible(windowHandle, groupCaption, itemCaption);
		}
		public string GetBackstageViewControlItemFromPoint(IntPtr windowHandle, int pointX, int PointY) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetBackstageViewControlItemFromPoint(windowHandle, pointX, PointY);
		}
		public string GetBackstageViewItemRectangle(IntPtr windowHandle, string itemName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetBackstageViewItemRectangle(windowHandle, itemName);
		}
		public string GetBackstageViewItemText(IntPtr windowHandle, string itemName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetBackstageViewItemText(windowHandle, itemName);
		}
		public ValueStruct GetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property) {
			return HelperManager.Get(windowHandle) == null ? new ValueStruct() : HelperManager.Get(windowHandle).BarsMethods.GetRibbonElementProperty(windowHandle, elementInfo, property);
		}
		public void SetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property, ValueStruct value) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.SetRibbonElementProperty(windowHandle, elementInfo, property, value);
		}
		public void PerformBarLinkClick(IntPtr windowHandle, string name, bool isDblClick) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.PerformBarLinkClick(windowHandle, name, isDblClick);
		}
		public void PerformRibbonLinkClick(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, bool isDblClick) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.PerformRibbonLinkClick(windowHandle, elementInfo, isDblClick);
		}
		public void PerformFloatDocumentFormButtonClick(IntPtr windowHandle, string caption) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.PerformFloatDocumentFormButtonClick(windowHandle, caption);
		}
		public RibbonElementSearchInfo[] GetRibbonGallerySelectedItems(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? new RibbonElementSearchInfo[] { } : HelperManager.Get(windowHandle).BarsMethods.GetRibbonGallerySelectedItems(windowHandle, elementInfo);
		}
		public MdiClientElementInfo GetMdiClientElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new MdiClientElementInfo() : HelperManager.Get(windowHandle).BarsMethods.GetMdiClientElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetMdiClientElementRectangle(IntPtr windowHandle, MdiClientElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetMdiClientElementRectangle(windowHandle, elementInfo);
		}
		public MdiClientDocumentDockInfo GetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? new MdiClientDocumentDockInfo() : HelperManager.Get(windowHandle).BarsMethods.GetMdiClientDocumentDockInfo(windowHandle, elementInfo);
		}
		public void SetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo, MdiClientDocumentDockInfo dockInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.SetMdiClientDocumentDockInfo(windowHandle, elementInfo, dockInfo);
		}
		public MdiClientElementInfo IfFormIsMdiChildGetDocumentInfo(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? new MdiClientElementInfo() : HelperManager.Get(windowHandle).BarsMethods.IfFormIsMdiChildGetDocumentInfo(windowHandle);
		}
		public FormElementInfo GetFloatDocumentFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new FormElementInfo() : HelperManager.Get(windowHandle).BarsMethods.GetFloatDocumentFormElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetFloatDocumentFormElementRectangle(IntPtr windowHandle, FormElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetFloatDocumentFormElementRectangle(windowHandle, elementInfo);
		}
		public Guid GetDockPanelIdFromTabHeader(IntPtr windowHandle, string tabName) {
			return HelperManager.Get(windowHandle) == null ? Guid.Empty : HelperManager.Get(windowHandle).BarsMethods.GetDockPanelIdFromTabHeader(windowHandle, tabName);
		}
		public string HandleDockPanelViaReflection(IntPtr dockPanelHandle, Guid dockPanelId, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			return HelperManager.Get(dockPanelHandle) == null ? null : HelperManager.Get(dockPanelHandle).BarsMethods.HandleDockPanelViaReflection(dockPanelHandle, dockPanelId, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
		}
		public string HandleDocumentManagerViaReflection(IntPtr mdiClientHandle, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			return HelperManager.Get(mdiClientHandle) == null ? null : HelperManager.Get(mdiClientHandle).BarsMethods.HandleDocumentManagerViaReflection(mdiClientHandle, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
		}
		public string GetDocumentSelectorItemNameFromPoint(IntPtr mdiClientHandle, int pointX, int pointY) {
			return HelperManager.Get(mdiClientHandle) == null ? null : HelperManager.Get(mdiClientHandle).BarsMethods.GetDocumentSelectorItemNameFromPoint(mdiClientHandle, pointX, pointY);
		}
		public string GetDocumentSelectorItemRectangle(IntPtr mdiClientHandle, string itemName) {
			return HelperManager.Get(mdiClientHandle) == null ? null : HelperManager.Get(mdiClientHandle).BarsMethods.GetDocumentSelectorItemRectangle(mdiClientHandle, itemName);
		}
		public void MakeMdiClientDocumentActive(IntPtr mdiClientHandle, IntPtr formHandle) {
			if(HelperManager.Get(mdiClientHandle) != null)
				HelperManager.Get(mdiClientHandle).BarsMethods.MakeMdiClientDocumentActive(mdiClientHandle, formHandle);
		}
		public Dictionary<string, BarElementTypes> GetBarLinksInfo(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetBarLinksInfo(windowHandle);
		}
		public Dictionary<string, BarElementTypes> GetLinkSubLinksInfo(IntPtr windowHandle, string linkName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetLinkSubLinksInfo(windowHandle, linkName);
		}
		public RibbonElementSearchInfo[] GetRibbonElementChildren(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetRibbonElementChildren(windowHandle, elementInfo);
		}
		public MdiClientElementInfo GetDocumentContainerDocumentInfo(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? new MdiClientElementInfo() : HelperManager.Get(windowHandle).BarsMethods.GetDocumentContainerDocumentInfo(windowHandle);
		}
		public DocumentContainerButtonType GetDocumentContainerButtonTypeFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? DocumentContainerButtonType.Undefined : HelperManager.Get(windowHandle).BarsMethods.GetDocumentContainerButtonTypeFromPoint(windowHandle, pointX, pointY);
		}
		public string GetDocumentContainerButtonRectangle(IntPtr windowHandle, DocumentContainerButtonType buttonType) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetDocumentContainerButtonRectangle(windowHandle, buttonType);
		}
		public AccordionControlElementInfo GetAccordionControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new AccordionControlElementInfo() : HelperManager.Get(windowHandle).BarsMethods.GetAccordionControlElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetAccordionControlElementRectangle(IntPtr windowHandle, AccordionControlElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).BarsMethods.GetAccordionControlElementRectangle(windowHandle, elementInfo);
		}
		public void MakeAccordionControlElementVisible(IntPtr windowHandle, AccordionControlElementInfo elementInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).BarsMethods.MakeAccordionControlElementVisible(windowHandle, elementInfo);
		}
	}
}
