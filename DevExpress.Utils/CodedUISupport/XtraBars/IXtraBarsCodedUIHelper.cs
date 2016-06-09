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
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IXtraBarsCodedUIHelper {
		string GetLinkNameFromPoint(IntPtr windowHandle, int pointX, int pointY, out BarElementTypes itemType);
		string GetLinkNameFromOldName(IntPtr windowHandle, string oldName);
		string GetLinkRectangleFromName(IntPtr windowHandle, string name);
		string GetBarItemOpenArrowRectangle(IntPtr windowHandle, string linkName);
		IntPtr GetSubMenuBarControlHandleFromOwnerLink(IntPtr ownerLinkWindowHandle, string ownerLinkName);
		string GetGalleryItemCaptionFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption);
		string GetGalleryItemRectangleFromCaption(IntPtr windowHandle, string groupCaption, string itemCaption);
		string GetNameOfLinkFromProperty(IntPtr windowHandle, string propertyThatContainsLink);
		ValueStruct GetLinkProperty(IntPtr windowHandle, string name, XtraBarsPropertyNames property);
		void SetLinkProperty(IntPtr windowHandle, string linkName, XtraBarsPropertyNames property, ValueStruct value);
		IntPtr GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, string name);
		IntPtr GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, RibbonElementSearchInfo elementInfo);
		IntPtr GetBarActiveEditorHandle(IntPtr windowHandle);
		void CloseLinkEditors(IntPtr windowHandle);
		AlertFormElements GetAlertFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string name);
		string GetAlertFormElementRectangle(IntPtr windowHandle, AlertFormElements elementType, string elementName);
		void SetBarDockInfo(IntPtr windowHandle, string dockStyle, int row, int column, int offset, string floatLocation, string standaloneBarDockControlName);
		DockPanelDockInfo GetDockPanelLastDockingOperationInfo(IntPtr handleForIpcConnection, Guid dockPanelId);
		DockPanelDockInfo GetDockPanelCurrentDockInfo(IntPtr dockPanelHandle, Guid dockPanelId);
		void SetDockPanelDockInfo(IntPtr windowHandle, Guid dockPanelId, DockPanelDockInfo dockInfo);
		DockPanelElementInfo GetDockPanelElementInfoFromPoint(IntPtr windowHandle, int pointX, int PointY);
		string GetDockPanelButtonRectangle(IntPtr windowHandle, DockPanelElementInfo elementInfo);
		string GetAutoHideContainerButtonName(IntPtr windowHandle, int pointX, int PointY);
		string GetAutoHideContainerButtonRectangle(IntPtr windowHandle, string buttonName);
		IntPtr GetChildDockPanelHandleAndShowIt(IntPtr windowHandle, string panelName);
		RibbonElementSearchInfo GetRibbonElementFromPoint(IntPtr windowHandle, int pointX, int pointY);
		string GetRibbonElementName(IntPtr ribbonHandle, RibbonElementSearchInfo elementInfo);
		string GetRibbonElementRectangle(IntPtr windowHandle, RibbonElementSearchInfo elementInfo);
		void MakeRibbonGalleryItemVisible(IntPtr windowHandle, RibbonElementSearchInfo elementInfo);
		void MakeGalleryItemVisible(IntPtr windowHandle, string groupCaption, string itemCaption);
		string GetBackstageViewControlItemFromPoint(IntPtr windowHandle, int pointX, int PointY);
		string GetBackstageViewItemRectangle(IntPtr windowHandle, string itemName);
		string GetBackstageViewItemText(IntPtr windowHandle, string itemName);
		ValueStruct GetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property);
		void SetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property, ValueStruct value);
		void PerformBarLinkClick(IntPtr windowHandle, string name, bool isDblClick);
		void PerformRibbonLinkClick(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, bool isDblClick);
		void PerformFloatDocumentFormButtonClick(IntPtr windowHandle, string caption);
		RibbonElementSearchInfo[] GetRibbonGallerySelectedItems(IntPtr windowHandle, RibbonElementSearchInfo elementInfo);
		MdiClientElementInfo GetMdiClientElementFromPoint(IntPtr windowHandle, int pointX, int pointY);
		string GetMdiClientElementRectangle(IntPtr windowHandle, MdiClientElementInfo elementInfo);
		MdiClientDocumentDockInfo GetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo);
		void SetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo, MdiClientDocumentDockInfo dockInfo);
		MdiClientElementInfo IfFormIsMdiChildGetDocumentInfo(IntPtr windowHandle);
		FormElementInfo GetFloatDocumentFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY);
		string GetFloatDocumentFormElementRectangle(IntPtr windowHandle, FormElementInfo elementInfo);
		Guid GetDockPanelIdFromTabHeader(IntPtr windowHandle, string tabName);
		string HandleDockPanelViaReflection(IntPtr dockPanelHandle, Guid dockPanelId, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet);
		string HandleDocumentManagerViaReflection(IntPtr mdiClientHandle, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet);
		string GetDocumentSelectorItemNameFromPoint(IntPtr mdiClientHandle, int pointX, int pointY);
		string GetDocumentSelectorItemRectangle(IntPtr mdiClientHandle, string itemName);
		void MakeMdiClientDocumentActive(IntPtr mdiClientHandle, IntPtr formHandle);
		Dictionary<string, BarElementTypes> GetBarLinksInfo(IntPtr windowHandle);
		Dictionary<string, BarElementTypes> GetLinkSubLinksInfo(IntPtr windowHandle, string linkName);
		RibbonElementSearchInfo[] GetRibbonElementChildren(IntPtr windowHandle, RibbonElementSearchInfo elementInfo);
		MdiClientElementInfo GetDocumentContainerDocumentInfo(IntPtr windowHandle);
		DocumentContainerButtonType GetDocumentContainerButtonTypeFromPoint(IntPtr windowHandle, int pointX, int pointY);
		string GetDocumentContainerButtonRectangle(IntPtr windowHandle, DocumentContainerButtonType buttonType);
		AccordionControlElementInfo GetAccordionControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY);
		string GetAccordionControlElementRectangle(IntPtr windowHandle, AccordionControlElementInfo elementInfo);
		void MakeAccordionControlElementVisible(IntPtr windowHandle, AccordionControlElementInfo elementInfo);
	}
}
