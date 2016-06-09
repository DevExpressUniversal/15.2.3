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
namespace DevExpress.Utils.CodedUISupport {
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientSideXtraBarsMethods : ClientSideHelperBase {
		internal ClientSideXtraBarsMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraBars.CodedUISupport.XtraBarsCodedUIHelper";
		IXtraBarsCodedUIHelper xtraBarsCodedUIHelper;
		IXtraBarsCodedUIHelper Helper {
			get {
				if(xtraBarsCodedUIHelper == null)
					xtraBarsCodedUIHelper = this.GetHelper<IXtraBarsCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyBars);
				return xtraBarsCodedUIHelper;
			}
		}
		public string GetLinkNameFromPoint(IntPtr windowHandle, int pointX, int pointY, out BarElementTypes itemType) {
			BarElementTypes _itemType = BarElementTypes.Default;
			string result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkNameFromPoint(windowHandle, pointX, pointY, out _itemType);
			});
			itemType = _itemType;
			return result;
		}
		public string GetLinkNameFromOldName(IntPtr windowHandle, string oldName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkNameFromOldName(windowHandle, oldName);
			});
		}
		public string GetLinkRectangleFromName(IntPtr windowHandle, string name) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkRectangleFromName(windowHandle, name);
			});
		}
		public string GetBarItemOpenArrowRectangle(IntPtr windowHandle, string linkName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBarItemOpenArrowRectangle(windowHandle, linkName);
			});
		}
		public int GetSubMenuBarControlHandleFromOwnerLink(IntPtr ownerLinkWindowHandle, string ownerLinkName) {
			return RunClientSideMethod(Helper, ownerLinkWindowHandle, delegate() {
				return Helper.GetSubMenuBarControlHandleFromOwnerLink(ownerLinkWindowHandle, ownerLinkName).ToInt32();
			});
		}
		public string GetGalleryItemCaptionFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption) {
			string _groupCaption = null;
			string result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGalleryItemCaptionFromPoint(windowHandle, pointX, pointY, out _groupCaption);
			});
			groupCaption = _groupCaption;
			return result;
		}
		public string GetGalleryItemRectangleFromCaption(IntPtr windowHandle, string groupCaption, string itemCaption) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGalleryItemRectangleFromCaption(windowHandle, groupCaption, itemCaption);
			});
		}
		public string GetNameOfLinkFromProperty(IntPtr windowHandle, string propertyThatContainsLink) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetNameOfLinkFromProperty(windowHandle, propertyThatContainsLink);
			});
		}
		public ValueStruct GetLinkProperty(IntPtr windowHandle, string name, XtraBarsPropertyNames property) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkProperty(windowHandle, name, property);
			});
		}
		public void SetLinkProperty(IntPtr windowHandle, string linkName, XtraBarsPropertyNames property, ValueStruct value) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetLinkProperty(windowHandle, linkName, property, value);
			});
		}
		public int GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, string name) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkEditorHandleOrShowEditor(windowHandle, name).ToInt32();
			});
		}
		public int GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkEditorHandleOrShowEditor(windowHandle, elementInfo).ToInt32();
			});
		}
		public int GetBarActiveEditorHandle(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBarActiveEditorHandle(windowHandle).ToInt32();
			});
		}
		public void CloseLinkEditors(IntPtr windowHandle) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.CloseLinkEditors(windowHandle);
			});
		}
		public AlertFormElements GetAlertFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string name) {
			string _name = null;
			AlertFormElements result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAlertFormElementFromPoint(windowHandle, pointX, pointY, out _name);
			});
			name = _name;
			return result;
		}
		public string GetAlertFormElementRectangle(IntPtr windowHandle, AlertFormElements elementType, string elementName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAlertFormElementRectangle(windowHandle, elementType, elementName);
			});
		}
		public void SetBarDockInfo(IntPtr windowHandle, string dockStyle, int row, int column, int offset, string floatLocation, string standaloneBarDockControlName) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetBarDockInfo(windowHandle, dockStyle, row, column, offset, floatLocation, standaloneBarDockControlName);
			});
		}
		public DockPanelDockInfo GetDockPanelLastDockingOperationInfo(IntPtr handleForIpcConnection, Guid dockPanelId) {
			return RunClientSideMethod(Helper, handleForIpcConnection, delegate() {
				return Helper.GetDockPanelLastDockingOperationInfo(handleForIpcConnection, dockPanelId);
			});
		}
		public DockPanelDockInfo GetDockPanelCurrentDockInfo(IntPtr dockPanelHandle, Guid dockPanelId) {
			return RunClientSideMethod(Helper, dockPanelHandle, delegate() {
				return Helper.GetDockPanelCurrentDockInfo(dockPanelHandle, dockPanelId);
			});
		}
		public void SetDockPanelDockInfo(IntPtr windowHandle, Guid dockPanelId, DockPanelDockInfo dockInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetDockPanelDockInfo(windowHandle, dockPanelId, dockInfo);
			});
		}
		public DockPanelElementInfo GetDockPanelElementInfoFromPoint(IntPtr windowHandle, int pointX, int PointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDockPanelElementInfoFromPoint(windowHandle, pointX, PointY);
			});
		}
		public string GetDockPanelButtonRectangle(IntPtr windowHandle, DockPanelElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDockPanelButtonRectangle(windowHandle, elementInfo);
			});
		}
		public string GetAutoHideContainerButtonName(IntPtr windowHandle, int pointX, int PointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAutoHideContainerButtonName(windowHandle, pointX, PointY);
			});
		}
		public string GetAutoHideContainerButtonRectangle(IntPtr windowHandle, string buttonName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAutoHideContainerButtonRectangle(windowHandle, buttonName);
			});
		}
		public int GetChildDockPanelHandleAndShowIt(IntPtr windowHandle, string panelName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetChildDockPanelHandleAndShowIt(windowHandle, panelName).ToInt32();
			});
		}
		public RibbonElementSearchInfo GetRibbonElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRibbonElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetRibbonElementName(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRibbonElementName(windowHandle, elementInfo);
			});
		}
		public string GetRibbonElementRectangle(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRibbonElementRectangle(windowHandle, elementInfo);
			});
		}
		public void MakeRibbonGalleryItemVisible(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeRibbonGalleryItemVisible(windowHandle, elementInfo);
			});
		}
		public void MakeGalleryItemVisible(IntPtr windowHandle, string groupCaption, string itemCaption) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeGalleryItemVisible(windowHandle, groupCaption, itemCaption);
			});
		}
		public string GetBackstageViewControlItemFromPoint(IntPtr windowHandle, int pointX, int PointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBackstageViewControlItemFromPoint(windowHandle, pointX, PointY);
			});
		}
		public string GetBackstageViewItemRectangle(IntPtr windowHandle, string itemName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBackstageViewItemRectangle(windowHandle, itemName);
			});
		}
		public string GetBackstageViewItemText(IntPtr windowHandle, string itemName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBackstageViewItemText(windowHandle, itemName);
			});
		}
		public ValueStruct GetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRibbonElementProperty(windowHandle, elementInfo, property);
			});
		}
		public void SetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property, ValueStruct value) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetRibbonElementProperty(windowHandle, elementInfo, property, value);
			});
		}
		public void PerformBarLinkClick(IntPtr windowHandle, string name, bool isDblClick) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.PerformBarLinkClick(windowHandle, name, isDblClick);
			});
		}
		public void PerformRibbonLinkClick(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, bool isDblClick) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.PerformRibbonLinkClick(windowHandle, elementInfo, isDblClick);
			});
		}
		public void PerformFloatDocumentFormButtonClick(IntPtr windowHandle, string caption) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.PerformFloatDocumentFormButtonClick(windowHandle, caption);
			});
		}
		public RibbonElementSearchInfo[] GetRibbonGallerySelectedItems(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRibbonGallerySelectedItems(windowHandle, elementInfo);
			});
		}
		public MdiClientElementInfo GetMdiClientElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetMdiClientElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetMdiClientElementRectangle(IntPtr windowHandle, MdiClientElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetMdiClientElementRectangle(windowHandle, elementInfo);
			});
		}
		public MdiClientDocumentDockInfo GetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetMdiClientDocumentDockInfo(windowHandle, elementInfo);
			});
		}
		public void SetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo, MdiClientDocumentDockInfo dockInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetMdiClientDocumentDockInfo(windowHandle, elementInfo, dockInfo);
			});
		}
		public MdiClientElementInfo IfFormIsMdiChildGetDocumentInfo(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.IfFormIsMdiChildGetDocumentInfo(windowHandle);
			});
		}
		public FormElementInfo GetFloatDocumentFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetFloatDocumentFormElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetFloatDocumentFormElementRectangle(IntPtr windowHandle, FormElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetFloatDocumentFormElementRectangle(windowHandle, elementInfo);
			});
		}
		public Guid GetDockPanelIdFromTabHeader(IntPtr windowHandle, string tabName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDockPanelIdFromTabHeader(windowHandle, tabName);
			});
		}
		public string HandleDockPanelViaReflection(IntPtr dockPanelHandle, Guid dockPanelId, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			return RunClientSideMethod(Helper, dockPanelHandle, delegate() {
				return Helper.HandleDockPanelViaReflection(dockPanelHandle, dockPanelId, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
			});
		}
		public string HandleDocumentManagerViaReflection(IntPtr mdiClientHandle, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			return RunClientSideMethod(Helper, mdiClientHandle, delegate() {
				return Helper.HandleDocumentManagerViaReflection(mdiClientHandle, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
			});
		}
		public string GetDocumentSelectorItemNameFromPoint(IntPtr mdiClientHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, mdiClientHandle, delegate() {
				return Helper.GetDocumentSelectorItemNameFromPoint(mdiClientHandle, pointX, pointY);
			});
		}
		public string GetDocumentSelectorItemRectangle(IntPtr mdiClientHandle, string itemName) {
			return RunClientSideMethod(Helper, mdiClientHandle, delegate() {
				return Helper.GetDocumentSelectorItemRectangle(mdiClientHandle, itemName);
			});
		}
		public void MakeMdiClientDocumentActive(IntPtr mdiClientHandle, IntPtr formHandle) {
			RunClientSideMethod(Helper, mdiClientHandle, delegate() {
				Helper.MakeMdiClientDocumentActive(mdiClientHandle, formHandle);
			});
		}
		public Dictionary<string, BarElementTypes> GetBarLinksInfo(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBarLinksInfo(windowHandle);
			});
		}
		public Dictionary<string, BarElementTypes> GetLinkSubLinksInfo(IntPtr windowHandle, string linkName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinkSubLinksInfo(windowHandle, linkName);
			});
		}
		public RibbonElementSearchInfo[] GetRibbonElementChildren(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRibbonElementChildren(windowHandle, elementInfo);
			});
		}
		public MdiClientElementInfo GetDocumentContainerDocumentInfo(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDocumentContainerDocumentInfo(windowHandle);
			});
		}
		public DocumentContainerButtonType GetDocumentContainerButtonTypeFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDocumentContainerButtonTypeFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetDocumentContainerButtonRectangle(IntPtr windowHandle, DocumentContainerButtonType buttonType) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDocumentContainerButtonRectangle(windowHandle, buttonType);
			});
		}
		public AccordionControlElementInfo GetAccordionControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAccordionControlElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetAccordionControlElementRectangle(IntPtr windowHandle, AccordionControlElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAccordionControlElementRectangle(windowHandle, elementInfo);
			});
		}
		public void MakeAccordionControlElementVisible(IntPtr windowHandle, AccordionControlElementInfo elementInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeAccordionControlElementVisible(windowHandle, elementInfo);
			});
		}
	}
}
