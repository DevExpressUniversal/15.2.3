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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.CodedUISupport;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Dragging;
using DevExpress.XtraBars.Docking2010.Dragging.Tabbed;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTabbedMdi;
namespace DevExpress.XtraBars.CodedUISupport {
	class XtraBarsCodedUIHelper : IXtraBarsCodedUIHelper {
		public const string BarCustomizationItem = "BarCustomizationItem";
		RemoteObject remoteObject;
		public XtraBarsCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
			instanceCore = this;
		}
		static XtraBarsCodedUIHelper instanceCore;
		internal static XtraBarsCodedUIHelper Instance {
			get {
				if(instanceCore == null)
					instanceCore = new XtraBarsCodedUIHelper(new RemoteObject(new Func<IntPtr, bool>(delegate(IntPtr param) { return false; })));
				return instanceCore;
			}
		}
		public Dictionary<string, BarElementTypes> GetBarLinksInfo(IntPtr windowHandle) {
			Control control = Control.FromHandle(windowHandle);
			BarItemLinkReadOnlyCollection links = null;
			if(control is CustomLinksControl)
				links = (control as CustomLinksControl).VisibleLinks;
			else if(control is RibbonStatusBar)
				links = (control as RibbonStatusBar).ItemLinks;
			if(links != null)
				return GetLinksInfo(links);
			return null;
		}
		public Dictionary<string, BarElementTypes> GetLinkSubLinksInfo(IntPtr windowHandle, string linkName) {
			Control control = Control.FromHandle(windowHandle);
			if(control != null) {
				BarItemLink link = GetLinkFromName(control, linkName);
				if(link != null && link is BarCustomContainerItemLink)
					return GetLinksInfo((link as BarCustomContainerItemLink).VisibleLinks);
			}
			return null;
		}
		Dictionary<string, BarElementTypes> GetLinksInfo(BarItemLinkReadOnlyCollection links) {
			Dictionary<string, BarElementTypes> result = new Dictionary<string, BarElementTypes>();
			if(links != null)
				foreach(BarItemLink link in links) {
					string name = GetLinkName(links, link);
					if(link is BarBaseButtonItemLink)
						result.Add(name, BarElementTypes.BarBaseButtonItemLink);
					else if(link is BarEditItemLink)
						result.Add(name, BarElementTypes.BarEditItemLink);
					else
						result.Add(name, BarElementTypes.BarItemLink);
				}
			return result;
		}
		public string GetLinkNameFromPoint(IntPtr windowHandle, int pointX, int pointY, out BarElementTypes itemType) {
			itemType = BarElementTypes.Default;
			Control control = Control.FromHandle(windowHandle);
			Point point = new Point(pointX, pointY);
			if(control is CustomLinksControl) {
				CustomLinksControl customLinksControl = control as CustomLinksControl;
				if(control is CustomBarControl)
					if((control as CustomBarControl).ViewInfo.DragBorder.Contains(point)) {
						itemType = BarElementTypes.DragBorder;
						return itemType.ToString();
					}
				foreach(BarItemLink link in customLinksControl.VisibleLinks) {
					if(link.Bounds.Contains(point)) {
						if(link is BarBaseButtonItemLink) {
							if(link.LinkViewInfo != null && link.LinkViewInfo.Rects.Rectangles.ContainsKey(ViewInfo.BarLinkParts.OpenArrow) && link.LinkViewInfo.Rects.Rectangles[ViewInfo.BarLinkParts.OpenArrow].Contains(point))
								itemType = BarElementTypes.BarItemOpenArrow;
							else
								itemType = BarElementTypes.BarBaseButtonItemLink;
						} else if(link is BarEditItemLink)
							itemType = BarElementTypes.BarEditItemLink;
						else
							itemType = BarElementTypes.BarItemLink;
						return GetLinkName(customLinksControl.VisibleLinks, link);
					}
				}
			} else if(control is RibbonStatusBar) {
				RibbonStatusBar statusBar = control as RibbonStatusBar;
				if(statusBar.ShowSizeGrip && statusBar.ViewInfo.SizeGripBounds.Contains(point)) {
					itemType = BarElementTypes.DragBorder;
					return BarElementTypes.DragBorder.ToString();
				}
				foreach(BarItemLink link in statusBar.ItemLinks)
					if(link.Bounds.Contains(point))
						return GetLinkName(statusBar.ItemLinks, link);
			}
			return null;
		}
		public string GetLinkNameFromOldName(IntPtr windowHandle, string oldName) {
			Control control = Control.FromHandle(windowHandle);
			if(control is CustomLinksControl) {
				CustomLinksControl customLinksControl = control as CustomLinksControl;
				BarItemLink link = GetLinkFromName(customLinksControl.VisibleLinks, oldName);
				if(link != null)
					return GetLinkName(customLinksControl.VisibleLinks, link);
			} else if(control is RibbonStatusBar) {
				RibbonStatusBar statusBar = control as RibbonStatusBar;
				BarItemLink link = GetLinkFromName(statusBar.ItemLinks, oldName);
				if(link != null)
					return GetLinkName(statusBar.ItemLinks, link);
			}
			return null;
		}
		string GetLinkName(BarItemLinkReadOnlyCollection links, BarItemLink itemLink) {
			if(string.IsNullOrEmpty(itemLink.Item.Name)) {
				if(itemLink is BarQBarCustomizationItemLink)
					return BarCustomizationItem;
				else if(itemLink is BarQMenuCustomizationItemLink && (itemLink as BarQMenuCustomizationItemLink).InnerLink != null) {
					BarItemLink innerLink = (itemLink as BarQMenuCustomizationItemLink).InnerLink;
					if(!string.IsNullOrEmpty(innerLink.Item.Name))
						return innerLink.Item.Name;
				}
				return GetLinkNameFromLinkType(links, itemLink);
			} else
				return itemLink.Item.Name;
		}
		string GetLinkCaption(BarItemLink itemLink) {
			if(!string.IsNullOrEmpty(itemLink.Caption))
				return itemLink.Caption;
			if(!string.IsNullOrEmpty(itemLink.DisplayCaption))
				return itemLink.DisplayCaption;
			if(itemLink is BarQMenuCustomizationItemLink && (itemLink as BarQMenuCustomizationItemLink).InnerLink != null)
				return GetLinkCaption((itemLink as BarQMenuCustomizationItemLink).InnerLink);
			return null;
		}
		string GetLinkNameOld(BarItemLinkReadOnlyCollection links, BarItemLink itemLink) {
			string caption = GetLinkCaption(itemLink);
			if(string.IsNullOrEmpty(caption)) {
				if(itemLink is BarQBarCustomizationItemLink)
					return BarCustomizationItem;
				else
					return GetLinkNameFromLinkType(links, itemLink);
			} else
				return caption;
		}
		string GetLinkNameFromLinkType(BarItemLinkReadOnlyCollection links, BarItemLink itemLink) {
			int index = 0;
			if(links != null)
				foreach(BarItemLink link in links)
					if(link.GetType().Name == itemLink.GetType().Name) {
						if(link == itemLink)
							break;
						else
							index++;
					}
			return itemLink.GetType().Name + "[" + index + "]";
		}
		BarItemLink GetLinkFromName(Control container, string name) {
			if(container is CustomLinksControl)
				return GetLinkFromName((container as CustomLinksControl).VisibleLinks, name);
			else if(container is RibbonStatusBar)
				return GetLinkFromName((container as RibbonStatusBar).ItemLinks, name);
			else
				return null;
		}
		BarItemLink GetLinkFromName(BarItemLinkReadOnlyCollection links, string name) {
			foreach(BarItemLink link in links)
				if(GetLinkName(links, link) == name)
					return link;
			foreach(BarItemLink link in links)
				if(GetLinkNameOld(links, link) == name)
					return link;
			return null;
		}
		IButton GetFloatDocumentFormButtonFromCaption(BaseButtonCollection buttons, string caption) {
			foreach(IButton button in buttons) {
				if(button.Properties.ToolTip == caption)
					return button;
			}
			return null;
		}
		public string GetLinkRectangleFromName(IntPtr windowHandle, string name) {
			Control control = Control.FromHandle(windowHandle);
			BarItemLink link = GetLinkFromName(control, name);
			if(link != null)
				return CodedUIUtils.ConvertToString(link.Bounds);
			return null;
		}
		public string GetBarItemOpenArrowRectangle(IntPtr windowHandle, string linkName) {
			Control control = Control.FromHandle(windowHandle);
			BarItemLink link = GetLinkFromName(control, linkName);
			if(link != null && link.LinkViewInfo != null && link.LinkViewInfo.Rects.Rectangles.ContainsKey(ViewInfo.BarLinkParts.OpenArrow))
				return CodedUIUtils.ConvertToString(link.LinkViewInfo.Rects.Rectangles[ViewInfo.BarLinkParts.OpenArrow]);
			return null;
		}
		public IntPtr GetSubMenuBarControlHandleFromOwnerLink(IntPtr ownerLinkWindowHandle, string ownerLinkName) {
			BarItemLink link = GetLinkFromName(Control.FromHandle(ownerLinkWindowHandle), ownerLinkName);
			if(link != null) {
				if(link is BarSubItemLink) {
					BarSubItemLink subItemLink = link as BarSubItemLink;
					if(subItemLink.SubControl != null && subItemLink.SubControl.IsHandleCreated)
						return subItemLink.SubControl.Handle;
					else
						return IntPtr.Zero;
				} else {
					PropertyInfo timerPopupProperty = link.GetType().GetProperty("TimerPopup", BindingFlags.NonPublic | BindingFlags.Instance);
					if(timerPopupProperty != null) {
						Control timerPopup = timerPopupProperty.GetValue(link, new object[] { }) as Control;
						if(timerPopup != null && timerPopup.IsHandleCreated)
							return timerPopup.Handle;
					}
				}
			}
			return IntPtr.Zero;
		}
		public string GetNameOfLinkFromProperty(IntPtr windowHandle, string propertyThatContainsLink) {
			Control control = Control.FromHandle(windowHandle);
			if(control != null) {
				PropertyInfo pi = control.GetType().GetProperty(propertyThatContainsLink, BindingFlags.Public | BindingFlags.Instance);
				if(pi != null) {
					BarItemLink link = pi.GetValue(control, new object[] { }) as BarItemLink;
					if(link != null)
						return GetLinkName((link.BarControl is CustomLinksControl) ? (link.BarControl as CustomLinksControl).VisibleLinks : link.Links, link);
				}
			}
			return null;
		}
		public ValueStruct GetLinkProperty(IntPtr windowHandle, string linkName, XtraBarsPropertyNames property) {
			return GetLinkProperty(GetLinkFromName(Control.FromHandle(windowHandle), linkName), property);
		}
		ValueStruct GetLinkProperty(BarItemLink link, XtraBarsPropertyNames property) {
			if(link != null)
				switch(property) {
					case XtraBarsPropertyNames.EditValue:
						if(link is BarEditItemLink) {
							BarEditItemLink editItemLink = link as BarEditItemLink;
							if(editItemLink.ActiveEditor != null && editItemLink.EditorActive)
								if(editItemLink.ActiveEditor.DoValidate())
									editItemLink.PostEditor();
							return new ValueStruct(editItemLink.EditValue);
						}
						break;
					case XtraBarsPropertyNames.Checked:
						if(link is BarBaseButtonItemLink) {
							return new ValueStruct(link.Item != null && (link as BarBaseButtonItemLink).Item.Down);
						}
						break;
					case XtraBarsPropertyNames.Text:
						return new ValueStruct(GetLinkCaption(link));
					case XtraBarsPropertyNames.Enabled:
						return new ValueStruct(link.Enabled);
				}
			return new ValueStruct();
		}
		public void SetLinkProperty(IntPtr windowHandle, string linkName, XtraBarsPropertyNames property, ValueStruct value) {
			Control control = Control.FromHandle(windowHandle);
			BarItemLink link = GetLinkFromName(control, linkName) as BarItemLink;
			if(link != null)
				SetLinkProperty(control, link, property, value);
		}
		void SetLinkProperty(Control control, BarItemLink link, XtraBarsPropertyNames property, ValueStruct value) {
			if(control != null && link != null) {
				switch(property) {
					case XtraBarsPropertyNames.EditValue:
						if(link is BarEditItemLink)
							control.BeginInvoke(new MethodInvoker(delegate {
								(link as BarEditItemLink).EditValue = CodedUIUtils.GetValue(value);
							}));
						break;
					case XtraBarsPropertyNames.Checked:
						if(link is BarBaseButtonItemLink)
							control.BeginInvoke(new MethodInvoker(delegate {
								(link as BarBaseButtonItemLink).Item.Down = CodedUIUtils.ConvertFromString<bool>(value.ValueAsString);
							}));
						break;
				}
			}
		}
		public void PerformBarLinkClick(IntPtr windowHandle, string name, bool isDblClick) {
			Control control = Control.FromHandle(windowHandle);
			if(control != null) {
				BarItemLink link = GetLinkFromName(control, name) as BarItemLink;
				if(link != null && link.Item != null)
					control.BeginInvoke(new MethodInvoker(delegate {
						if(isDblClick) {
							link.Item.OnDoubleClick(link);
							link.Manager.SelectionInfo.DoubleClickLink(link);
						} else {
							link.Item.PerformClick(link);
							link.Manager.SelectionInfo.PressLink(link);
						}
					}));
			}
		}
		public void PerformFloatDocumentFormButtonClick(IntPtr windowHandle, string caption) {
			FloatDocumentForm form = Control.FromHandle(windowHandle) as FloatDocumentForm;
			if(form != null) {
				if(form.ButtonsPanel != null && form.ButtonsPanel.Buttons != null) {
					IButton button = GetFloatDocumentFormButtonFromCaption(form.ButtonsPanel.Buttons, caption);
					if(button != null) {
						form.BeginInvoke(new MethodInvoker(delegate {
							(form.ButtonsPanel as IButtonsPanel).PerformClick(button);
						}));
					}
				}
			}
		}
		public IntPtr GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, string name) {
			Control control = Control.FromHandle(windowHandle);
			if(control != null) {
				BarEditItemLink link = GetLinkFromName(control, name) as BarEditItemLink;
				return GetLinkEditorHandleOrShowEditor(control, link);
			}
			return IntPtr.Zero;
		}
		public IntPtr GetLinkEditorHandleOrShowEditor(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				BarEditItemLink link = GetRibbonItemLink(ribbon, elementInfo) as BarEditItemLink;
				return GetLinkEditorHandleOrShowEditor(ribbon, link);
			}
			return IntPtr.Zero;
		}
		IntPtr GetLinkEditorHandleOrShowEditor(Control control, BarEditItemLink link) {
			if(link != null) {
				if(link.ActiveEditor == null || !link.EditorActive)
					control.BeginInvoke(new MethodInvoker(delegate {
						link.CloseEditor();
						link.ShowEditor();
					}));
				else
					return link.ActiveEditor.Handle;
			}
			return IntPtr.Zero;
		}
		public IntPtr GetBarActiveEditorHandle(IntPtr windowHandle) {
			CustomLinksControl control = Control.FromHandle(windowHandle) as CustomLinksControl;
			if(control != null) {
				foreach(BarItemLink link in control.VisibleLinks)
					if(link is BarEditItemLink) {
						BaseEdit activeEditor = (link as BarEditItemLink).ActiveEditor;
						if(activeEditor != null && activeEditor.IsHandleCreated)
							return activeEditor.Handle;
					}
			}
			return IntPtr.Zero;
		}
		public void CloseLinkEditors(IntPtr windowHandle) {
			CustomLinksControl control = Control.FromHandle(windowHandle) as CustomLinksControl;
			if(control != null) {
				control.BeginInvoke(new MethodInvoker(delegate {
					foreach(BarItemLink link in control.VisibleLinks)
						if(link as BarEditItemLink != null) {
							BarEditItemLink editItemLink = link as BarEditItemLink;
							if(editItemLink.ActiveEditor != null && editItemLink.EditorActive) {
								editItemLink.PostEditor();
								object value = editItemLink.EditValue;
								PopupBaseEdit popupEditor = editItemLink.ActiveEditor as PopupBaseEdit;
								if(popupEditor != null)
									popupEditor.ClosePopup();
								editItemLink.CloseEditor();
								editItemLink.EditValue = value;
								break;
							}
						}
				}));
			}
		}
		public string GetGalleryItemCaptionFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption) {
			Control galleryControl = Control.FromHandle(windowHandle);
			StandaloneGallery gallery = null;
			if(galleryControl is GalleryDropDownBarControl)
				gallery = (galleryControl as GalleryDropDownBarControl).Gallery;
			else if(galleryControl is GalleryControl)
				gallery = (galleryControl as GalleryControl).Gallery;
			else if(galleryControl is GalleryControlClient)
				gallery = (galleryControl as GalleryControlClient).Gallery;
			if(gallery != null) {
				StandaloneGalleryViewInfo viewInfo = gallery.ViewInfo;
				Point clientPoint = new Point(pointX, pointY);
				if(viewInfo.Bounds.Contains(clientPoint)) {
					foreach(GalleryItemGroupViewInfo group in viewInfo.Groups)
						if(group.Bounds.Contains(clientPoint))
							foreach(GalleryItemViewInfo item in group.Items)
								if(item.Bounds.Contains(clientPoint)) {
									groupCaption = group.Group.Caption;
									return item.Item.Caption;
								}
				}
			}
			groupCaption = null;
			return null;
		}
		public string GetGalleryItemRectangleFromCaption(IntPtr windowHandle, string groupCaption, string itemCaption) {
			Control galleryControl = Control.FromHandle(windowHandle);
			StandaloneGallery gallery = null;
			if(galleryControl is GalleryDropDownBarControl)
				gallery = (galleryControl as GalleryDropDownBarControl).Gallery;
			else if(galleryControl is GalleryControl)
				gallery = (galleryControl as GalleryControl).Gallery;
			else if(galleryControl is GalleryControlClient)
				gallery = (galleryControl as GalleryControlClient).Gallery;
			if(gallery != null) {
				if(!gallery.ViewInfo.IsReady && gallery.ViewInfo.Groups.Count == 0) {
					galleryControl.BeginInvoke(new MethodInvoker(delegate() { gallery.ViewInfo.CalcViewInfo(gallery.ViewInfo.Bounds); }));
					return null;
				}
				Rectangle rect = GetGalleryItemOrGroupRectangle(gallery.ViewInfo, groupCaption, itemCaption);
				if(rect != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rect);
			}
			return null;
		}
		public void MakeGalleryItemVisible(IntPtr windowHandle, string groupCaption, string itemCaption) {
			Control galleryControl = Control.FromHandle(windowHandle);
			StandaloneGallery gallery = null;
			if(galleryControl is GalleryDropDownBarControl)
				gallery = (galleryControl as GalleryDropDownBarControl).Gallery;
			else if(galleryControl is GalleryControl)
				gallery = (galleryControl as GalleryControl).Gallery;
			else if(galleryControl is GalleryControlClient)
				gallery = (galleryControl as GalleryControlClient).Gallery;
			if(gallery != null)
				MageGalleryItemVisible(gallery.ViewInfo, groupCaption, itemCaption);
		}
		public void MakeRibbonGalleryItemVisible(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				InRibbonGalleryRibbonItemViewInfo galleryItemViewInfo = GetRibbonItemViewInfo(ribbon, elementInfo) as InRibbonGalleryRibbonItemViewInfo;
				if(galleryItemViewInfo != null)
					MageGalleryItemVisible(galleryItemViewInfo.GalleryInfo, elementInfo.GalleryGroup, elementInfo.GalleryItem);
			}
		}
		public RibbonElementSearchInfo[] GetRibbonGallerySelectedItems(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			List<RibbonElementSearchInfo> result = new List<RibbonElementSearchInfo>();
			if(ribbon != null) {
				RibbonGalleryBarItemLink link = GetRibbonItemLink(ribbon, elementInfo) as RibbonGalleryBarItemLink;
				foreach(GalleryItemGroup group in link.Gallery.Groups)
					if(elementInfo.ElementType != RibbonElements.GalleryGroup || elementInfo.GalleryGroup == group.Caption)
						foreach(GalleryItem item in group.Items)
							if(item.Checked) {
								RibbonElementSearchInfo itemInfo = elementInfo;
								itemInfo.GalleryGroup = group.Caption;
								itemInfo.GalleryItem = item.Caption;
								itemInfo.ElementType = RibbonElements.GalleryItem;
								result.Add(itemInfo);
							}
			}
			return result.ToArray();
		}
		void MageGalleryItemVisible(BaseGalleryViewInfo galleryViewInfo, string groupName, string itemName) {
			if(galleryViewInfo != null)
				foreach(GalleryItemGroupViewInfo group in galleryViewInfo.Groups)
					if(group.Group.Caption == groupName)
						foreach(GalleryItemViewInfo item in group.Items)
							if(item.Item.Caption == itemName)
								if(!galleryViewInfo.Bounds.Contains(item.Bounds)) {
									galleryViewInfo.OwnerControl.BeginInvoke(new MethodInvoker(delegate() {
										galleryViewInfo.MakeVisible(item.Item);
									}));
									break;
								}
		}
		Rectangle GetGalleryItemOrGroupRectangle(BaseGalleryViewInfo galleryViewInfo, string groupCaption, string itemCaption) {
			if(groupCaption == null && itemCaption == null)
				return galleryViewInfo.Bounds;
			foreach(GalleryItemGroupViewInfo group in galleryViewInfo.Groups)
				if((String.IsNullOrEmpty(groupCaption) && String.IsNullOrEmpty(group.Group.Caption)) || group.Group.Caption == groupCaption) {
					if(itemCaption != null) {
						foreach(GalleryItemViewInfo item in group.Items)
							if(item.Item.Caption == itemCaption)
								return GetGalleryItemOrGroupVisibleRectangle(galleryViewInfo, item.Bounds);
					} else
						return GetGalleryItemOrGroupVisibleRectangle(galleryViewInfo, group.Bounds);
				}
			return Rectangle.Empty;
		}
		Rectangle GetGalleryItemOrGroupVisibleRectangle(BaseGalleryViewInfo galleryViewInfo, Rectangle elementRectangle) {
			Rectangle result = elementRectangle;
			Rectangle galleryBoundsWithoutPaddings = new Rectangle(
	galleryViewInfo.GalleryContentBounds.X + galleryViewInfo.BackgroundPaddings.Left,
	galleryViewInfo.GalleryContentBounds.Y + galleryViewInfo.BackgroundPaddings.Top,
	galleryViewInfo.GalleryContentBounds.Width - galleryViewInfo.BackgroundPaddings.Left - galleryViewInfo.BackgroundPaddings.Right,
	galleryViewInfo.GalleryContentBounds.Height - galleryViewInfo.BackgroundPaddings.Top - galleryViewInfo.BackgroundPaddings.Bottom
	);
			result.Intersect(galleryBoundsWithoutPaddings);
			if(result.Width <= 0 || result.Height <= 0)
				result = Rectangle.Empty;
			return result;
		}
		public AlertFormElements GetAlertFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string name) {
			name = null;
			AlertForm alertForm = Control.FromHandle(windowHandle) as AlertForm;
			if(alertForm != null) {
				Point clientPoint = new Point(pointX, pointY);
				foreach(AlertButton button in alertForm.Buttons)
					if(button.Bounds.Contains(clientPoint)) {
						if(button is AlertPinButton)
							return AlertFormElements.AlertPinButton;
						else if(button is AlertCloseButton)
							return AlertFormElements.AlertCloseButton;
						else if(button is AlertPopupMenuButton)
							return AlertFormElements.AlertPopupMenuButton;
						else {
							name = button.Name;
							return AlertFormElements.AlertButton;
						}
					}
				if(alertForm.ViewInfo.TextRectangle.Contains(clientPoint)) {
					name = alertForm.Info.Text;
					return AlertFormElements.AlertText;
				}
				if(alertForm.ViewInfo.CaptionRectangle.Contains(clientPoint)) {
					name = alertForm.Info.Caption;
					return AlertFormElements.AlertCaption;
				}
			}
			return AlertFormElements.Default;
		}
		public string GetAlertFormElementRectangle(IntPtr windowHandle, AlertFormElements elementType, string elementName) {
			AlertForm alertForm = Control.FromHandle(windowHandle) as AlertForm;
			if(alertForm != null) {
				Rectangle rectangle = GetAlertFormElementRectangle(alertForm, elementType, elementName);
				if(rectangle != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rectangle);
			}
			return null;
		}
		Rectangle GetAlertFormElementRectangle(AlertForm alertForm, AlertFormElements elementType, string elementName) {
			switch(elementType) {
				case AlertFormElements.AlertCloseButton:
					foreach(AlertButton button in alertForm.Buttons)
						if(button is AlertCloseButton)
							return button.Bounds;
					break;
				case AlertFormElements.AlertPinButton:
					foreach(AlertButton button in alertForm.Buttons)
						if(button is AlertPinButton)
							return button.Bounds;
					break;
				case AlertFormElements.AlertPopupMenuButton:
					foreach(AlertButton button in alertForm.Buttons)
						if(button is AlertPopupMenuButton)
							return button.Bounds;
					break;
				case AlertFormElements.AlertButton:
					foreach(AlertButton button in alertForm.Buttons)
						if(button.Name == elementName)
							return button.Bounds;
					break;
				case AlertFormElements.AlertText:
					return alertForm.ViewInfo.TextRectangle;
				case AlertFormElements.AlertCaption:
					return alertForm.ViewInfo.CaptionRectangle;
			}
			return Rectangle.Empty;
		}
		public void SetBarDockInfo(IntPtr windowHandle, string dockStyle, int row, int column, int offset, string floatLocation, string standaloneBarDockControlName) {
			Control control = Control.FromHandle(windowHandle);
			Bar bar = null;
			if(control is CustomBarControl)
				bar = (control as CustomBarControl).Bar;
			else if(control is FloatingBarControlForm)
				bar = (control as FloatingBarControlForm).Bar;
			if(bar != null)
				control.BeginInvoke(new MethodInvoker(delegate {
					SetBarDockInfo(bar, dockStyle, row, column, offset, floatLocation, standaloneBarDockControlName);
				}));
		}
		void SetBarDockInfo(Bar bar, string dockStyle, int row, int column, int offset, string floatLocation, string standaloneBarDockControlName) {
			if(floatLocation != null)
				SetBarDockInfo(bar, CodedUIUtils.ConvertFromString<BarDockStyle>(dockStyle), CodedUIUtils.ConvertFromString<Point>(floatLocation));
			else
				SetBarDockInfo(bar, CodedUIUtils.ConvertFromString<BarDockStyle>(dockStyle), row, column, offset, standaloneBarDockControlName);
		}
		void SetBarDockInfo(Bar bar, BarDockStyle dockStyle, int row, int column, int offset, string standaloneBarDockControlName) {
			BarDockInfo dockInfo = null;
			if(dockStyle == BarDockStyle.Standalone && standaloneBarDockControlName != null) {
				foreach(BarDockControl control in bar.Manager.DockControls) {
					StandaloneBarDockControl scontrol = control as StandaloneBarDockControl;
					if(scontrol != null && scontrol.Name == standaloneBarDockControlName) {
						dockInfo = new BarDockInfo(scontrol, dockStyle, row, column, offset);
					}
				}
			}
			if(dockInfo == null)
				dockInfo = new BarDockInfo(dockStyle, row, column, offset);
			bar.DockInfo = dockInfo;
		}
		void SetBarDockInfo(Bar bar, BarDockStyle dockStyle, Point floatLocation) {
			BarDockInfo dockInfo = new BarDockInfo(dockStyle, bar.DockRow, bar.DockCol, bar.Offset);
			bar.DockInfo = dockInfo;
			bar.FloatLocation = floatLocation;
		}
		public DockPanelDockInfo GetDockPanelCurrentDockInfo(IntPtr dockPanelHandle, Guid dockPanelId) {
			DockPanel panel = Control.FromHandle(dockPanelHandle) as DockPanel;
			DockPanelDockInfo result = new DockPanelDockInfo();
			if(panel != null)
				if(dockPanelId != Guid.Empty)
					panel = GetDockPanelFromId(panel.DockManager, dockPanelId);
			if(panel != null) {
				result.Index = panel.Index;
				result.FloatLocation = panel.FloatLocation;
				result.DockingStyleAsString = CodedUIUtils.ConvertToString(panel.Dock);
				result.IsTab = panel.IsTab;
				result.LayoutAsString = GetDockPanelLayoutAsString(panel);
			}
			return result;
		}
		public string HandleDockPanelViaReflection(IntPtr dockPanelHandle, Guid dockPanelId, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			DockPanel panel = Control.FromHandle(dockPanelHandle) as DockPanel;
			if(panel != null)
				if(dockPanelId != Guid.Empty)
					panel = GetDockPanelFromId(panel.DockManager, dockPanelId);
			if(panel != null) {
				ClientSideHelper csh = new ClientSideHelper(this.remoteObject);
				return csh.HandleViaReflection(panel, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
			}
			return null;
		}
		public DockPanelDockInfo GetDockPanelLastDockingOperationInfo(IntPtr handleForIpcConnection, Guid dockPanelId) {
			return DockManagerCodedUIHelper.GetDockingOperationInfo(dockPanelId);
		}
		internal void WriteTargetPanelInfoInDockInfo(DockPanel panel, ref DockPanelDockInfo dockInfo) {
			string targetPanelLastChildName = null;
			int hierarchyLevel = 0;
			targetPanelLastChildName = GetDockPanelLastChildName(panel, ref hierarchyLevel);
			if(targetPanelLastChildName != null && targetPanelLastChildName != String.Empty) {
				if(hierarchyLevel == 0)
					dockInfo.TargetPanelName = targetPanelLastChildName;
				else {
					dockInfo.TargetPanelLastChildName = targetPanelLastChildName;
					dockInfo.TargetPanelLastChildHierarchyLevel = hierarchyLevel;
				}
			} else
				dockInfo.TargetPanelLayoutAsString = GetDockPanelLayoutAsString(panel);
		}
		string GetDockPanelLayoutAsString(DockPanel panel) {
			return GetDockPanelLayoutAsString(panel, true);
		}
		string GetDockPanelLayoutAsString(DockPanel panel, bool includeLastIndex) {
			string result = String.Empty;
			while(true) {
				result = dockPanelLayoutAsStringSeparator.ToString() + panel.Index.ToString() + result;
				if(panel.ParentPanel == null) {
					result = CodedUIUtils.ConvertToString(panel.Dock) + result;
					break;
				} else
					panel = panel.ParentPanel;
			}
			if(!includeLastIndex)
				result = result.Substring(0, result.LastIndexOf(dockPanelLayoutAsStringSeparator));
			return result;
		}
		DockPanel GetDockPanelFromLayoutAsString(string layoutAsString, DockManager manager) {
			DockPanel result = null;
			DockingStyle style = CodedUIUtils.ConvertFromString<DockingStyle>(layoutAsString.Substring(0, layoutAsString.IndexOf(dockPanelLayoutAsStringSeparator)));
			string[] indicesAsString = layoutAsString.Substring(layoutAsString.IndexOf(dockPanelLayoutAsStringSeparator)).Split(new char[] { dockPanelLayoutAsStringSeparator }, StringSplitOptions.RemoveEmptyEntries);
			List<int> indices = new List<int>();
			foreach(string index in indicesAsString)
				indices.Add(CodedUIUtils.ConvertFromString<int>(index));
			foreach(DockPanel panel in manager.Panels) {
				if(panel.ParentPanel == null && panel.Dock == style && panel.Index == indices[0])
					result = panel;
			}
			if(result != null && indices.Count > 1) {
				for(int i = 1; i < indices.Count; i++)
					if(result.Count > indices[i])
						result = result[indices[i]];
			}
			return result;
		}
		const char dockPanelLayoutAsStringSeparator = '-';
		string GetDockPanelLastChildName(DockPanel panel, List<DockPanel> excludedChildren, ref int hierarchyLevel) {
			if(panel.Count == 0)
				return panel.Name;
			for(int i = 0; i < panel.Count; i++)
				if(!excludedChildren.Contains(panel[i])) {
					hierarchyLevel++;
					return GetDockPanelLastChildName(panel[i], ref hierarchyLevel);
				}
			return null;
		}
		string GetDockPanelLastChildName(DockPanel panel, ref int hierarchyLevel) {
			if(panel.Count > 0) {
				hierarchyLevel++;
				return GetDockPanelLastChildName(panel[0], ref hierarchyLevel);
			} else
				return panel.Name;
		}
		DockManager GetDockManagerFromDockPanelOrMdiClientHandle(IntPtr handle) {
			DockPanel panel = Control.FromHandle(handle) as DockPanel;
			if(panel != null)
				return panel.DockManager;
			else {
				MdiClientSubclasser mdiClientSubclasser = MdiClientSubclasser.FromHandle(handle) as MdiClientSubclasser;
				if(mdiClientSubclasser != null && mdiClientSubclasser.Owner is DocumentManager)
					return (mdiClientSubclasser.Owner as DocumentManager).DockManager;
				else {
					IDocumentsHost documentsHost = Control.FromHandle(handle) as IDocumentsHost;
					if(documentsHost != null && documentsHost.Owner is DocumentManager)
						return (documentsHost.Owner as DocumentManager).DockManager;
				}
			}
			return null;
		}
		public void SetDockPanelDockInfo(IntPtr windowHandle, Guid dockPanelId, DockPanelDockInfo dockInfo) {
			DockManager manager = GetDockManagerFromDockPanelOrMdiClientHandle(windowHandle);
			if(manager != null) {
				DockPanel panel = GetDockPanelFromId(manager, dockPanelId);
				if(panel != null)
					SetDockPanelDockInfo(panel, dockPanelId, dockInfo);
			}
		}
		void SetDockPanelDockInfo(DockPanel panel, Guid dockPanelId, DockPanelDockInfo dockInfo) {
			panel.BeginInvoke(new MethodInvoker(delegate() {
				if(panel != null) {
					if(panel.IsMdiDocument && panel.DockManager.DocumentManager != null) {
						BaseDocument document = panel.DockManager.DocumentManager.GetDocument(panel);
						if(document == null)
							document = panel.DockManager.DocumentManager.GetDocument(panel.Parent);
						if(document != null) {
							document.Manager.View.Controller.Float(document);
							panel = (document.Control as FloatForm).FloatLayout.Panel;
						}
					} else if(dockInfo.KeepCurrentParent) {
						panel.Index = dockInfo.Index;
						if(panel != null && !panel.Disposing && panel.DockManager != null)
							panel.DockManager.ActivePanel = panel;
						return;
					} else {
						if(dockInfo.FloatLocation != Point.Empty) {
							panel.Dock = DockingStyle.Float;
							panel.FloatLocation = dockInfo.FloatLocation;
							return;
						} else if(panel.Dock != DockingStyle.Float) {
							Point location = panel.TopLevelControl.PointToClient(panel.PointToScreen(panel.Location));
							panel.Dock = DockingStyle.Float;
							panel.FloatLocation = location;
						}
					}
					DockPanel targetPanel = GetDockingTargetPanelFromDockInfo(panel.DockManager, dockInfo);
					DockingStyle dockingStyle = CodedUIUtils.ConvertFromString<DockingStyle>(dockInfo.DockingStyleAsString);
					if(targetPanel != null)
						SetDockPanelDockInfo(panel, targetPanel, dockingStyle, dockInfo.Index, dockInfo.IsTab);
					else {
						panel.DockTo(dockingStyle, dockInfo.Index);
					}
					if(panel != null && !panel.Disposing && panel.DockManager != null)
						panel.DockManager.ActivePanel = panel;
				}
			}));
		}
		DockPanel GetDockingTargetPanelFromDockInfo(DockManager manager, DockPanelDockInfo dockInfo) {
			if(dockInfo.TargetPanelName != null) {
				return GetDockPanelFromName(manager, dockInfo.TargetPanelName);
			} else if(dockInfo.TargetPanelLastChildName != null) {
				DockPanel lastChild = GetDockPanelFromName(manager, dockInfo.TargetPanelLastChildName);
				if(lastChild != null) {
					DockPanel targetPanel = lastChild;
					for(int i = 0; i < dockInfo.TargetPanelLastChildHierarchyLevel; i++)
						if(targetPanel.ParentPanel != null)
							targetPanel = targetPanel.ParentPanel;
					return targetPanel;
				}
			} else if(dockInfo.TargetPanelLayoutAsString != null) {
				return GetDockPanelFromLayoutAsString(dockInfo.TargetPanelLayoutAsString, manager);
			}
			return null;
		}
		void SetDockPanelDockInfo(DockPanel panel, DockPanel targetPanel, DockingStyle dockingStyle, int index, bool isTab) {
			if(targetPanel != null) {
				if(isTab)
					panel.DockAsTab(targetPanel, index);
				else
					panel.DockTo(targetPanel, dockingStyle, index);
			}
		}
		public DockPanelElementInfo GetDockPanelElementInfoFromPoint(IntPtr windowHandle, int pointX, int PointY) {
			DockPanel panel = Control.FromHandle(windowHandle) as DockPanel;
			DockPanelElementInfo result = new DockPanelElementInfo();
			if(panel != null) {
				Point clientPoint = new Point(pointX, PointY);
				panel.GetHitInfo(clientPoint);
				foreach(BaseButtonInfo button in panel.ButtonsPanel.ViewInfo.Buttons)
					if(button.Bounds.Contains(clientPoint)) {
						result.IsButton = true;
						if(button.Button is DefaultButton || button.Caption == null || button.Caption == String.Empty)
							result.ButtonName = button.Button.GetType().Name;
						else
							result.ButtonName = button.Caption;
					}
				DockLayout dockLayout = panel.DockLayout;
				for(int i = dockLayout.FirstVisibleTabIndex; i < dockLayout.TabsBounds.Length; i++)
					if(dockLayout.TabsBounds[i].Contains(clientPoint)) {
						result.IsTabButton = true;
						result.ButtonName = GetDockPanelTabButtonName(dockLayout, i);
						result.TabButtonPanelId = dockLayout[i].Panel.ID;
					}
				ResizeZoneCollection resizeZoneCollection = GetResizeZoneCollection(dockLayout);
				if(resizeZoneCollection != null)
					foreach(ResizeZone zone in resizeZoneCollection)
						if(zone.Bounds.Contains(clientPoint)) {
							result.IsResizeZone = true;
							result.ResizeZoneSide = CodedUIUtils.ConvertToString(zone.Side);
						}
			}
			return result;
		}
		ResizeZoneCollection GetResizeZoneCollection(DockLayout dockLayout) {
			ResizeZoneCollection collection = null;
			PropertyInfo pi = dockLayout.GetType().GetProperty("ResizeZones", BindingFlags.NonPublic | BindingFlags.Instance);
			if(pi != null)
				collection = pi.GetValue(dockLayout, new object[] { }) as ResizeZoneCollection;
			return collection;
		}
		public string GetDockPanelButtonRectangle(IntPtr windowHandle, DockPanelElementInfo elementInfo) {
			DockPanel panel = Control.FromHandle(windowHandle) as DockPanel;
			if(panel != null) {
				if(elementInfo.IsButton)
					foreach(BaseButtonInfo button in panel.ButtonsPanel.ViewInfo.Buttons)
						if(button.Button.GetType().Name == elementInfo.ButtonName || button.Caption == elementInfo.ButtonName)
							return CodedUIUtils.ConvertToString(button.Bounds);
				if(elementInfo.IsTabButton) {
					int buttonIndex = GetDockPanelTabButtonIndexFromName(panel.DockLayout, elementInfo.ButtonName);
					if(buttonIndex != -1)
						return CodedUIUtils.ConvertToString(panel.DockLayout.TabsBounds[buttonIndex]);
				}
				if(elementInfo.IsResizeZone) {
					ResizeZoneCollection resizeZoneCollection = GetResizeZoneCollection(panel.DockLayout);
					if(resizeZoneCollection != null)
						foreach(ResizeZone zone in resizeZoneCollection)
							if(CodedUIUtils.ConvertFromString<DockingStyle>(elementInfo.ResizeZoneSide) == zone.Side) {
								return CodedUIUtils.ConvertToString(zone.Bounds);
							}
				}
			}
			return null;
		}
		string GetDockPanelTabButtonName(DockLayout dockLayout, int buttonIndex) {
			int num = 0;
			string name = dockLayout[buttonIndex].Panel.Text;
			for(int i = dockLayout.FirstVisibleTabIndex; i < dockLayout.TabsBounds.Length; i++) {
				if(i == buttonIndex)
					break;
				if(dockLayout[i].Panel.Text == name)
					num++;
			}
			if(num > 0)
				name = name + "[" + num + "]";
			return name;
		}
		int GetDockPanelTabButtonIndexFromName(DockLayout dockLayout, string buttonName) {
			for(int i = dockLayout.FirstVisibleTabIndex; i < dockLayout.TabsBounds.Length; i++) {
				if(GetDockPanelTabButtonName(dockLayout, i) == buttonName)
					return i;
			}
			return -1;
		}
		public string GetAutoHideContainerButtonName(IntPtr windowHandle, int pointX, int pointY) {
			AutoHideContainer control = Control.FromHandle(windowHandle) as AutoHideContainer;
			if(control != null) {
				Point clientPoint;
				if(control.IsHorizontal)
					clientPoint = new Point(pointX, pointY);
				else
					clientPoint = new Point(pointY, pointX);
				foreach(AutoHideButtonInfo button in control.ViewInfo.ButtonInfos)
					if(button.Bounds.Contains(clientPoint)) {
						clientPoint.X -= button.Bounds.X;
						clientPoint.Y -= button.Bounds.Y;
						foreach(AutoHideButtonHeaderInfo header in button.Headers)
							if(header.Bounds.Contains(clientPoint))
								return GetAutoHideContainerButtonName(header, control.ViewInfo.ButtonInfos);
					}
			}
			return null;
		}
		string GetAutoHideContainerButtonName(AutoHideButtonHeaderInfo header, AutoHideButtonInfo[] buttons) {
			int num = 0;
			string name = header.Text;
			foreach(AutoHideButtonInfo button in buttons)
				foreach(AutoHideButtonHeaderInfo buttonHeader in button.Headers)
					if(buttonHeader == header) {
						if(num != 0)
							name += "[" + num + "]";
						return name;
					} else if(buttonHeader.Header.Panel.Text == name)
						num++;
			return name;
		}
		public string GetAutoHideContainerButtonRectangle(IntPtr windowHandle, string buttonName) {
			AutoHideContainer control = Control.FromHandle(windowHandle) as AutoHideContainer;
			if(control != null) {
				foreach(AutoHideButtonInfo button in control.ViewInfo.ButtonInfos)
					foreach(AutoHideButtonHeaderInfo header in button.Headers)
						if(GetAutoHideContainerButtonName(header, control.ViewInfo.ButtonInfos) == buttonName) {
							Rectangle rect;
							if(control.IsHorizontal)
								rect = new Rectangle(header.Bounds.X + button.Bounds.X, header.Bounds.Y + button.Bounds.Y, header.Bounds.Width, header.Bounds.Height);
							else
								rect = new Rectangle(header.Bounds.Y + button.Bounds.Y, header.Bounds.X + button.Bounds.X, header.Bounds.Height, header.Bounds.Width);
							return CodedUIUtils.ConvertToString(rect);
						}
			}
			return null;
		}
		public IntPtr GetChildDockPanelHandleAndShowIt(IntPtr windowHandle, string panelName) {
			Control control = Control.FromHandle(windowHandle);
			if(control is AutoHideContainer) {
				foreach(AutoHideButtonInfo button in (control as AutoHideContainer).ViewInfo.ButtonInfos)
					foreach(AutoHideButtonHeaderInfo header in button.Headers) {
						DockPanel panel = header.Header.Panel;
						if(panel.Name == panelName)
							return GetDockPanelHandleAndShowIt(panel);
						else if(panel.Parent.Name == panelName && panel.Parent is DockPanel)
							return GetDockPanelHandleAndShowIt(panel.Parent as DockPanel);
					}
			} else if(control is AutoHideControl) {
				DockPanel panel = (control as AutoHideControl).ClientControl as DockPanel;
				if(panel != null && panel.Name == panelName)
					return GetDockPanelHandleAndShowIt(panel);
			} else if(control is DockPanel) {
				foreach(Control panel in control.Controls)
					if(panel is DockPanel && panel.Name == panelName)
						return GetDockPanelHandleAndShowIt(panel as DockPanel);
			}
			return IntPtr.Zero;
		}
		IntPtr GetDockPanelHandleAndShowIt(DockPanel panel) {
			if(panel != null) {
				if(!panel.ControlVisible)
					panel.BeginInvoke(new MethodInvoker(delegate() {
						panel.DockManager.ActivePanel = panel;
					}));
				if(panel.IsHandleCreated)
					return panel.Handle;
			}
			return IntPtr.Zero;
		}
		public RibbonElementSearchInfo GetRibbonElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			RibbonElementSearchInfo result = new RibbonElementSearchInfo();
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				Point clientPoint = new Point(pointX, pointY);
				RibbonHitInfo hitInfo = ribbon.CalcHitInfo(clientPoint);
				switch(hitInfo.HitTest) {
					case RibbonHitTest.PageHeader:
						return GetRibbonElementInfo(hitInfo.Page);
					case RibbonHitTest.PageGroup:
					case RibbonHitTest.PageGroupCaption:
						return GetRibbonElementInfo(hitInfo.PageGroup);
					case RibbonHitTest.PageHeaderCategory:
						return GetRibbonElementInfo(hitInfo.PageCategory);
					case RibbonHitTest.Item:
					case RibbonHitTest.ItemDrop:
						result = GetRibbonElementInfo(hitInfo.Item, hitInfo.PageGroup);
						result.InToolbar = hitInfo.InToolbar;
						if(hitInfo.HitTest == RibbonHitTest.ItemDrop)
							result.ElementType = RibbonElements.ItemDrop;
						return result;
					case RibbonHitTest.ItemSeparator:
						if(hitInfo.PageGroup != null)
							return GetRibbonElementInfo(hitInfo.PageGroup);
						else {
							result.ElementType = RibbonElements.QuickAccessToolbar;
							return result;
						}
					case RibbonHitTest.PageGroupCaptionButton:
						result.ElementType = RibbonElements.GroupCaptionButton;
						FillRibbonElementInfo(hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.GalleryItem:
					case RibbonHitTest.GalleryImage:
						result.ElementType = RibbonElements.GalleryItem;
						FillRibbonElementInfo(hitInfo.GalleryItem, hitInfo.Item, hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.GalleryItemGroup:
						result.ElementType = RibbonElements.GalleryGroup;
						FillRibbonElementInfo(hitInfo.GalleryItemGroup, hitInfo.Item, hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.Gallery:
					case RibbonHitTest.GalleryFilter:
					case RibbonHitTest.GalleryLeftButton:
					case RibbonHitTest.GalleryRightButton:
					case RibbonHitTest.GallerySizeGrip:
					case RibbonHitTest.GallerySizingPanel:
						result.ElementType = RibbonElements.Gallery;
						FillRibbonElementInfo(hitInfo.Item, hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.GalleryUpButton:
						result.ElementType = RibbonElements.GalleryUpButton;
						FillRibbonElementInfo(hitInfo.Item, hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.GalleryDownButton:
						result.ElementType = RibbonElements.GalleryDownButton;
						FillRibbonElementInfo(hitInfo.Item, hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.GalleryDropDownButton:
						result.ElementType = RibbonElements.GalleryDropDownButton;
						FillRibbonElementInfo(hitInfo.Item, hitInfo.PageGroup, ref result);
						return result;
					case RibbonHitTest.Toolbar:
						if(ribbon.ViewInfo.ToolbarBounds.Contains(clientPoint))
							result.ElementType = RibbonElements.QuickAccessToolbar;
						return result;
					case RibbonHitTest.FormCaption:
						result.ElementType = RibbonElements.FormCaption;
						return result;
					case RibbonHitTest.FormCloseButton:
						result.ElementType = RibbonElements.FormCaptionButton;
						result.FormCaptionButtonKind = FormCaptionButtonKind.Close.ToString();
						return result;
					case RibbonHitTest.FormHelpButton:
						result.ElementType = RibbonElements.FormCaptionButton;
						result.FormCaptionButtonKind = FormCaptionButtonKind.Help.ToString();
						return result;
					case RibbonHitTest.FormMaximizeButton:
						result.ElementType = RibbonElements.FormCaptionButton;
						result.FormCaptionButtonKind = FormCaptionButtonKind.Maximize.ToString();
						return result;
					case RibbonHitTest.FormMinimizeButton:
						result.ElementType = RibbonElements.FormCaptionButton;
						result.FormCaptionButtonKind = FormCaptionButtonKind.Minimize.ToString();
						return result;
					case RibbonHitTest.ApplicationButton:
						result.ElementType = RibbonElements.ApplicationButton;
						return result;
					case RibbonHitTest.PanelLeftScroll:
						result.ElementType = RibbonElements.PanelLeftScroll;
						return result;
					case RibbonHitTest.PanelRightScroll:
						result.ElementType = RibbonElements.PanelRightScroll;
						return result;
				}
			}
			return result;
		}
		public string GetRibbonElementName(IntPtr ribbonHandle, RibbonElementSearchInfo elementInfo) {
			RibbonControl ribbon = Control.FromHandle(ribbonHandle) as RibbonControl;
			if(ribbon != null) {
				switch(elementInfo.ElementType) {
					case RibbonElements.Item:
					case RibbonElements.EditItem:
					case RibbonElements.BaseButtonItem:
					case RibbonElements.ItemDrop:
					case RibbonElements.Gallery:
						BarItemLink link = GetRibbonItemLink(ribbon, elementInfo);
						if(link != null)
							return GetLinkName(link.Links, link);
						break;
				}
			}
			return null;
		}
		RibbonElementSearchInfo GetRibbonElementInfo(RibbonPageCategory category) {
			RibbonElementSearchInfo elementInfo = new RibbonElementSearchInfo() { ElementType = RibbonElements.PageCategory };
			FillRibbonElementInfo(category, ref elementInfo);
			return elementInfo;
		}
		RibbonElementSearchInfo GetRibbonElementInfo(RibbonPage page) {
			RibbonElementSearchInfo elementInfo = new RibbonElementSearchInfo() { ElementType = RibbonElements.Page };
			FillRibbonElementInfo(page, ref elementInfo);
			return elementInfo;
		}
		RibbonElementSearchInfo GetRibbonElementInfo(RibbonPageGroup group) {
			RibbonElementSearchInfo elementInfo = new RibbonElementSearchInfo() { ElementType = RibbonElements.PageGroup };
			FillRibbonElementInfo(group, ref elementInfo);
			return elementInfo;
		}
		RibbonElementSearchInfo GetRibbonElementInfo(BarItemLink item, RibbonPageGroup group) {
			RibbonElementSearchInfo elementInfo = new RibbonElementSearchInfo();
			if(item is BarEditItemLink)
				elementInfo.ElementType = RibbonElements.EditItem;
			else if(item is BarBaseButtonItemLink)
				elementInfo.ElementType = RibbonElements.BaseButtonItem;
			else
				elementInfo.ElementType = RibbonElements.Item;
			FillRibbonElementInfo(item, group, ref elementInfo);
			return elementInfo;
		}
		void FillRibbonElementInfo(RibbonPageCategory category, ref RibbonElementSearchInfo elementInfo) {
			if(category != null) {
				if(category.Ribbon.PageCategories.Contains(category))
					elementInfo.PageCategory = GetRibbonCategoryName(category);
			}
		}
		void FillRibbonElementInfo(RibbonPage page, ref RibbonElementSearchInfo elementInfo) {
			if(page != null) {
				elementInfo.Page = GetRibbonPageName(page);
				FillRibbonElementInfo(page.Category, ref elementInfo);
			}
		}
		void FillRibbonElementInfo(RibbonPageGroup group, ref RibbonElementSearchInfo elementInfo) {
			if(group != null) {
				elementInfo.PageGroup = GetRibbonGroupName(group);
				FillRibbonElementInfo(group.Page, ref elementInfo);
			}
		}
		void FillRibbonElementInfo(BarItemLink item, RibbonPageGroup group, ref RibbonElementSearchInfo elementInfo) {
			if(item != null) {
				if(item.OwnerItem != null) {
					BarCustomContainerItemLink ownerLink = null;
					if(item.OwnerItem.Links.Count == 1)
						ownerLink = item.OwnerItem.Links[0] as BarCustomContainerItemLink;
					else
						foreach(BarItemLink ownerItemLink in item.OwnerItem.Links) {
							if(ownerItemLink is BarCustomContainerItemLink && ownerItemLink.Visible)
								foreach(BarItemLink link in (ownerItemLink as BarCustomContainerItemLink).VisibleLinks)
									if(link.Item == item.Item) {
										ownerLink = ownerItemLink as BarCustomContainerItemLink;
										break;
									}
							if(ownerLink != null)
								break;
						}
					if(ownerLink != null)
						elementInfo.OwnerItem = GetLinkName(ownerLink.Links, ownerLink);
				}
				elementInfo.Item = GetLinkName(item.Links, item);
				if(group != null)
					FillRibbonElementInfo(group, ref elementInfo);
			}
		}
		void FillRibbonElementInfo(GalleryItem galleryItem, BarItemLink groupItem, RibbonPageGroup group, ref RibbonElementSearchInfo elementInfo) {
			if(group != null) {
				elementInfo.GalleryItem = galleryItem.Caption;
				elementInfo.GalleryGroup = galleryItem.GalleryGroup.Caption;
				FillRibbonElementInfo(groupItem, group, ref elementInfo);
			}
		}
		void FillRibbonElementInfo(GalleryItemGroup galleryItemGroup, BarItemLink groupItem, RibbonPageGroup group, ref RibbonElementSearchInfo elementInfo) {
			if(group != null) {
				elementInfo.GalleryGroup = galleryItemGroup.Caption;
				FillRibbonElementInfo(groupItem, group, ref elementInfo);
			}
		}
		public string GetRibbonElementRectangle(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				Rectangle rectangle = GetRibbonElementRectangle(ribbon, elementInfo);
				if(rectangle != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rectangle);
			}
			return null;
		}
		Rectangle GetRibbonElementRectangle(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			switch(elementInfo.ElementType) {
				case RibbonElements.PageCategory:
					foreach(RibbonPageCategoryViewInfo categoryInfo in ribbon.ViewInfo.Header.PageCategories)
						if(GetRibbonCategoryName(categoryInfo.Category) == elementInfo.PageCategory)
							return categoryInfo.Bounds;
					break;
				case RibbonElements.Page:
					RibbonPageViewInfo pageInfo = GetRibbonPageInfo(ribbon, elementInfo);
					if(pageInfo != null)
						return pageInfo.Bounds;
					break;
				case RibbonElements.PageGroup:
					RibbonPageGroupViewInfo groupInfo = GetRibbonGroupInfo(ribbon, elementInfo);
					if(groupInfo != null)
						return groupInfo.Bounds;
					break;
				case RibbonElements.Item:
				case RibbonElements.EditItem:
				case RibbonElements.BaseButtonItem:
					RibbonItemViewInfo linkInfo = GetRibbonItemViewInfo(ribbon, elementInfo);
					if(linkInfo != null)
						return linkInfo.Bounds;
					break;
				case RibbonElements.ItemDrop:
					RibbonSplitButtonItemViewInfo splitButtonItemViewInfo = GetRibbonItemViewInfo(ribbon, elementInfo) as RibbonSplitButtonItemViewInfo;
					if(splitButtonItemViewInfo != null)
						return splitButtonItemViewInfo.DropButtonBounds;
					break;
				case RibbonElements.QuickAccessToolbar:
					return ribbon.ViewInfo.ToolbarBounds;
				case RibbonElements.FormCaption:
					return ribbon.ViewInfo.Toolbar.DropDownButtonBounds;
				case RibbonElements.GroupCaptionButton:
					RibbonPageGroupViewInfo pageGroupInfo = GetRibbonGroupInfo(ribbon, elementInfo);
					if(pageGroupInfo != null)
						return pageGroupInfo.ButtonBounds;
					break;
				case RibbonElements.GalleryItem:
				case RibbonElements.Gallery:
				case RibbonElements.GalleryGroup:
				case RibbonElements.GalleryUpButton:
				case RibbonElements.GalleryDownButton:
				case RibbonElements.GalleryDropDownButton:
					return GetRibbonGalleryElementRectangle(ribbon, elementInfo);
				case RibbonElements.FormCaptionButton:
					foreach(FormCaptionButton button in ribbon.ViewInfo.Caption.FormPainter.Buttons)
						if(button.Kind.ToString() == elementInfo.FormCaptionButtonKind)
							return button.Bounds;
					break;
				case RibbonElements.ApplicationButton:
					if(ribbon.ViewInfo.ApplicationButton != null)
						return ribbon.ViewInfo.ApplicationButton.Bounds;
					break;
				case RibbonElements.PanelLeftScroll:
					return ribbon.ViewInfo.Panel.LeftScrollButtonBounds;
				case RibbonElements.PanelRightScroll:
					return ribbon.ViewInfo.Panel.RightScrollButtonBounds;
			}
			return Rectangle.Empty;
		}
		Rectangle GetRibbonGalleryElementRectangle(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			InRibbonGalleryRibbonItemViewInfo galleryItemViewInfo = GetRibbonItemViewInfo(ribbon, elementInfo) as InRibbonGalleryRibbonItemViewInfo;
			if(galleryItemViewInfo != null) {
				InRibbonGalleryViewInfo galleryInfo = galleryItemViewInfo.GalleryInfo;
				if(galleryInfo != null)
					switch(elementInfo.ElementType) {
						case RibbonElements.Gallery:
							return galleryInfo.Bounds;
						case RibbonElements.GalleryGroup:
							return GetGalleryItemOrGroupRectangle(galleryInfo, elementInfo.GalleryGroup, null);
						case RibbonElements.GalleryItem:
							return GetGalleryItemOrGroupRectangle(galleryInfo, elementInfo.GalleryGroup, elementInfo.GalleryItem);
						case RibbonElements.GalleryUpButton:
							return galleryInfo.ButtonUpBounds;
						case RibbonElements.GalleryDownButton:
							return galleryInfo.ButtonDownBounds;
						case RibbonElements.GalleryDropDownButton:
							return galleryInfo.ButtonCommandBounds;
					}
			}
			return Rectangle.Empty;
		}
		RibbonPageViewInfo GetRibbonPageInfo(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			foreach(RibbonPageViewInfo pageInfo in ribbon.ViewInfo.Header.Pages)
				if(GetRibbonPageName(pageInfo.Page) == elementInfo.Page)
					return pageInfo;
			return null;
		}
		RibbonPageGroupViewInfo GetRibbonGroupInfo(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			foreach(RibbonPageGroupViewInfo groupInfo in ribbon.ViewInfo.Panel.Groups)
				if(GetRibbonGroupName(groupInfo.PageGroup) == elementInfo.PageGroup)
					return groupInfo;
			return null;
		}
		RibbonItemViewInfo GetRibbonItemViewInfo(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			if(elementInfo.PageGroup == null)
				if(elementInfo.InToolbar)
					return GetQuickAccessToolbarItemViewInfo(ribbon, elementInfo);
				else
					return GetPageHeaderItemViewInfo(ribbon, elementInfo);
			RibbonPageGroupViewInfo groupInfo = GetRibbonGroupInfo(ribbon, elementInfo);
			if(groupInfo != null)
				if(elementInfo.OwnerItem == null)
					return GetRibbonItemViewInfo(groupInfo.Items, elementInfo.Item);
				else {
					RibbonButtonGroupItemViewInfo ownerItemViewInfo = GetRibbonItemViewInfo(groupInfo.Items, elementInfo.OwnerItem) as RibbonButtonGroupItemViewInfo;
					if(ownerItemViewInfo != null)
						return GetRibbonItemViewInfo(ownerItemViewInfo.Items, elementInfo.Item);
				}
			return null;
		}
		RibbonItemViewInfo GetRibbonItemViewInfo(RibbonItemViewInfoCollection items, string name) {
			foreach(RibbonItemViewInfo itemInfo in items)
				if(!(itemInfo is RibbonSeparatorItemViewInfo) && itemInfo.Item is BarItemLink)
					if(GetLinkName((itemInfo.Item as BarItemLink).Links, itemInfo.Item as BarItemLink) == name)
						return itemInfo;
			foreach(RibbonItemViewInfo itemInfo in items)
				if(!(itemInfo is RibbonSeparatorItemViewInfo) && itemInfo.Item is BarItemLink)
					if(GetLinkNameOld((itemInfo.Item as BarItemLink).Links, itemInfo.Item as BarItemLink) == name)
						return itemInfo;
			return null;
		}
		RibbonItemViewInfo GetQuickAccessToolbarItemViewInfo(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			if(elementInfo.OwnerItem != null) {
				RibbonButtonGroupItemViewInfo ownerItem = GetRibbonItemViewInfo(ribbon.ViewInfo.Toolbar.Items, elementInfo.OwnerItem) as RibbonButtonGroupItemViewInfo;
				if(ownerItem != null)
					return GetRibbonItemViewInfo(ownerItem.Items, elementInfo.Item);
				else
					return null;
			} else
				return GetRibbonItemViewInfo(ribbon.ViewInfo.Toolbar.Items, elementInfo.Item);
		}
		RibbonItemViewInfo GetPageHeaderItemViewInfo(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			if(elementInfo.OwnerItem != null) {
				RibbonButtonGroupItemViewInfo ownerItem = GetRibbonItemViewInfo(ribbon.ViewInfo.Header.PageHeaderItems, elementInfo.OwnerItem) as RibbonButtonGroupItemViewInfo;
				if(ownerItem != null)
					return GetRibbonItemViewInfo(ownerItem.Items, elementInfo.Item);
				else
					return null;
			} else
				return GetRibbonItemViewInfo(ribbon.ViewInfo.Header.PageHeaderItems, elementInfo.Item);
		}
		public string GetBackstageViewControlItemFromPoint(IntPtr windowHandle, int pointX, int PointY) {
			BackstageViewControl control = Control.FromHandle(windowHandle) as BackstageViewControl;
			if(control != null) {
				Point clientPoint = new Point(pointX, PointY);
				foreach(BackstageViewItemBaseViewInfo itemInfo in control.ViewInfo.Items)
					if(itemInfo.Bounds.Contains(clientPoint))
						return GetBackstageViewItemName(itemInfo.Item);
			}
			return null;
		}
		public string GetBackstageViewItemRectangle(IntPtr windowHandle, string itemName) {
			BackstageViewControl control = Control.FromHandle(windowHandle) as BackstageViewControl;
			if(control != null) {
				foreach(BackstageViewItemBaseViewInfo itemInfo in control.ViewInfo.Items)
					if(GetBackstageViewItemName(itemInfo.Item) == itemName)
						return CodedUIUtils.ConvertToString(itemInfo.Bounds);
				foreach(BackstageViewItemBaseViewInfo itemInfo in control.ViewInfo.Items)
					if(GetBackstageViewItemCaption(itemInfo.Item) == itemName)
						return CodedUIUtils.ConvertToString(itemInfo.Bounds);
			}
			return null;
		}
		public string GetBackstageViewItemText(IntPtr windowHandle, string itemName) {
			BackstageViewControl control = Control.FromHandle(windowHandle) as BackstageViewControl;
			if(control != null) {
				foreach(BackstageViewItemBase item in control.Items)
					if(GetBackstageViewItemName(item) == itemName)
						return GetBackstageViewItemCaption(item);
				foreach(BackstageViewItemBase item in control.Items)
					if(GetBackstageViewItemCaption(item) == itemName)
						return itemName;
			}
			return null;
		}
		string GetBackstageViewItemName(BackstageViewItemBase item) {
			if(!string.IsNullOrEmpty(item.Name))
				return item.Name;
			else {
				return item.GetType().Name + item.Control.Items.IndexOf(item).ToString();
			}
		}
		string GetBackstageViewItemCaption(BackstageViewItemBase item) {
			if(item is BackstageViewItem)
				return (item as BackstageViewItem).Caption;
			return null;
		}
		public ValueStruct GetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				switch(elementInfo.ElementType) {
					case RibbonElements.Item:
					case RibbonElements.EditItem:
					case RibbonElements.BaseButtonItem:
						BarItemLink link = GetRibbonItemLink(ribbon, elementInfo);
						if(link != null)
							return GetLinkProperty(link, property);
						break;
					case RibbonElements.PageCategory:
						RibbonPageCategory category = GetRibbonCategory(ribbon, elementInfo);
						if(property == XtraBarsPropertyNames.Text)
							if(category != null)
								return new ValueStruct(category.Text);
						break;
					case RibbonElements.Page:
						RibbonPage page = GetRibbonPage(ribbon, elementInfo);
						if(page != null) {
							if(property == XtraBarsPropertyNames.Text)
								return new ValueStruct(page.Text);
							else if(property == XtraBarsPropertyNames.Selected)
								return new ValueStruct(ribbon.SelectedPage == page);
						}
						break;
					case RibbonElements.PageGroup:
						RibbonPageGroup pageGroup = GetRibbonGroup(ribbon, elementInfo);
						if(property == XtraBarsPropertyNames.Text)
							if(pageGroup != null)
								return new ValueStruct(pageGroup.Text);
						break;
				}
			}
			return new ValueStruct();
		}
		public void SetRibbonElementProperty(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, XtraBarsPropertyNames property, ValueStruct value) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				switch(elementInfo.ElementType) {
					case RibbonElements.Item:
					case RibbonElements.EditItem:
					case RibbonElements.BaseButtonItem:
						BarItemLink link = GetRibbonItemLink(ribbon, elementInfo);
						if(link != null)
							SetLinkProperty(ribbon, link, property, value);
						break;
				}
			}
		}
		public RibbonElementSearchInfo[] GetRibbonElementChildren(IntPtr windowHandle, RibbonElementSearchInfo elementInfo) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				List<RibbonElementSearchInfo> children = new List<RibbonElementSearchInfo>();
				switch(elementInfo.ElementType) {
					case RibbonElements.Ribbon:
						foreach(RibbonPageCategory childCategory in ribbon.PageCategories)
							children.Add(GetRibbonElementInfo(childCategory));
						foreach(RibbonPageCategory childCategory in ribbon.MergedCategories)
							children.Add(GetRibbonElementInfo(childCategory));
						foreach(RibbonPage childPage in ribbon.Pages)
							children.Add(GetRibbonElementInfo(childPage));
						foreach(RibbonPage childPage in ribbon.MergedPages)
							children.Add(GetRibbonElementInfo(childPage));
						break;
					case RibbonElements.PageCategory:
						RibbonPageCategory category = GetRibbonCategory(ribbon, elementInfo);
						foreach(RibbonPage childPage in category.Pages)
							children.Add(GetRibbonElementInfo(childPage));
						break;
					case RibbonElements.Page:
						RibbonPage page = GetRibbonPage(ribbon, elementInfo);
						foreach(RibbonPageGroup childGroup in page.Groups)
							children.Add(GetRibbonElementInfo(childGroup));
						break;
					case RibbonElements.PageGroup:
						RibbonPageGroup pageGroup = GetRibbonGroup(ribbon, elementInfo);
						foreach(BarItemLink link in pageGroup.ItemLinks)
							children.Add(GetRibbonElementInfo(link, pageGroup));
						break;
					case RibbonElements.Item:
						BarItemLink itemLink = GetRibbonItemLink(ribbon, elementInfo);
						RibbonPageGroup group = GetRibbonGroup(ribbon, elementInfo);
						if(itemLink is BarCustomContainerItemLink && group != null) {
							foreach(BarItemLink link in (itemLink as BarCustomContainerItemLink).VisibleLinks)
								children.Add(GetRibbonElementInfo(link, group));
						}
						break;
				}
				return children.ToArray();
			}
			return null;
		}
		public void PerformRibbonLinkClick(IntPtr windowHandle, RibbonElementSearchInfo elementInfo, bool isDblClick) {
			RibbonControl ribbon = Control.FromHandle(windowHandle) as RibbonControl;
			if(ribbon != null) {
				BarItemLink link = GetRibbonItemLink(ribbon, elementInfo);
				if(link != null && link.Item != null) {
					ribbon.BeginInvoke(new MethodInvoker(delegate() {
						if(isDblClick) {
							ribbon.Manager.SelectionInfo.DoubleClickLink(link);
							link.Item.OnDoubleClick(link);
						} else {
							ribbon.Manager.SelectionInfo.PressLink(link);
							link.Item.PerformClick(link);
						}
					}));
				}
			}
		}
		BarItemLink GetRibbonItemLink(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			if(elementInfo.PageGroup == null) {
				if(elementInfo.InToolbar)
					return GetQuickToolbarItemLink(ribbon, elementInfo);
				else
					return GetRibbonItemLinkFromCollection(ribbon.PageHeaderItemLinks, elementInfo);
			} else {
				RibbonPage page = GetRibbonPage(ribbon, elementInfo);
				if(page != null) {
					RibbonPageGroup group = GetRibbonGroup(page, elementInfo);
					if(group != null)
						return GetRibbonItemLink(group, elementInfo);
				}
			}
			return null;
		}
		BarItemLink GetQuickToolbarItemLink(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			BarItemLink link = GetRibbonItemLinkFromCollection(ribbon.QuickToolbarItemLinks, elementInfo);
			if(link != null)
				return link;
			foreach(RibbonItemViewInfo itemInfo in ribbon.ViewInfo.Toolbar.Items)
				if((itemInfo.Item as BarItemLink) != null) {
					BarItemLink itemLink = itemInfo.Item as BarItemLink;
					if(GetLinkName(itemLink.Links, itemLink) == elementInfo.Item)
						if(itemInfo.Bounds != Rectangle.Empty)
							return itemLink;
				}
			foreach(RibbonItemViewInfo itemInfo in ribbon.ViewInfo.Toolbar.Items)
				if((itemInfo.Item as BarItemLink) != null) {
					BarItemLink itemLink = itemInfo.Item as BarItemLink;
					if(GetLinkNameOld(itemLink.Links, itemLink) == elementInfo.Item)
						if(itemInfo.Bounds != Rectangle.Empty)
							return itemLink;
				}
			return null;
		}
		BarItemLink GetRibbonToolbarOrPageHeaderItemLink(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			BaseRibbonItemLinkCollection collection;
			if(elementInfo.InToolbar)
				collection = ribbon.QuickToolbarItemLinks;
			else
				collection = ribbon.PageHeaderItemLinks;
			if(elementInfo.OwnerItem != null) {
				BarCustomContainerItemLink ownerLink = GetLinkFromName(collection, elementInfo.OwnerItem) as BarCustomContainerItemLink;
				if(ownerLink != null)
					return GetLinkFromName(ownerLink.VisibleLinks, elementInfo.Item);
				else
					return null;
			} else
				return GetLinkFromName(collection, elementInfo.Item);
		}
		RibbonPageCategory GetRibbonCategory(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			if(elementInfo.PageCategory != null) {
				foreach(RibbonPageCategory category in ribbon.PageCategories)
					if(GetRibbonCategoryName(category) == elementInfo.PageCategory)
						return category;
				foreach(RibbonPageCategory category in ribbon.MergedCategories)
					if(GetRibbonCategoryName(category) == elementInfo.PageCategory)
						return category;
			}
			return null;
		}
		RibbonPage GetRibbonPage(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			RibbonPageCategory category = GetRibbonCategory(ribbon, elementInfo);
			if(category != null) {
				foreach(RibbonPage page in category.Pages)
					if(GetRibbonPageName(page) == elementInfo.Page)
						return page;
				foreach(RibbonPage page in category.MergedPages)
					if(GetRibbonPageName(page) == elementInfo.Page)
						return page;
			}
			foreach(RibbonPage page in ribbon.Pages)
				if(GetRibbonPageName(page) == elementInfo.Page)
					return page;
			foreach(RibbonPage page in ribbon.MergedPages)
				if(GetRibbonPageName(page) == elementInfo.Page)
					return page;
			return null;
		}
		RibbonPageGroup GetRibbonGroup(RibbonControl ribbon, RibbonElementSearchInfo elementInfo) {
			RibbonPage page = GetRibbonPage(ribbon, elementInfo);
			if(page != null)
				return GetRibbonGroup(page, elementInfo);
			else
				return null;
		}
		RibbonPageGroup GetRibbonGroup(RibbonPage page, RibbonElementSearchInfo elementInfo) {
			foreach(RibbonPageGroup group in page.Groups)
				if(GetRibbonGroupName(group) == elementInfo.PageGroup)
					return group;
			foreach(RibbonPageGroup group in page.MergedGroups)
				if(GetRibbonGroupName(group) == elementInfo.PageGroup)
					return group;
			return null;
		}
		BarItemLink GetRibbonItemLink(RibbonPageGroup group, RibbonElementSearchInfo elementInfo) {
			return GetRibbonItemLinkFromCollection(group.ItemLinks, elementInfo);
		}
		BarItemLink GetRibbonItemLinkFromCollection(BaseRibbonItemLinkCollection collection, RibbonElementSearchInfo elementInfo) {
			if(elementInfo.OwnerItem != null) {
				BarCustomContainerItemLink ownerLink = GetLinkFromName(collection, elementInfo.OwnerItem) as BarCustomContainerItemLink;
				if(ownerLink != null)
					return GetLinkFromName(ownerLink.VisibleLinks, elementInfo.Item);
				else
					return null;
			} else
				return GetLinkFromName(collection, elementInfo.Item);
		}
		string GetRibbonCategoryName(RibbonPageCategory category) {
			if(!string.IsNullOrEmpty(category.Name))
				return category.Name;
			else {
				string categoryName = category.Ribbon.MergedCategories.Contains(category) ? "Merged" : String.Empty + category.GetType().Name + category.Collection.IndexOf(category).ToString();
				foreach(RibbonPageCategory pageCategory in category.Collection)
					if(pageCategory.Name == categoryName)
						categoryName = "Custom" + categoryName;
				return categoryName;
			}
		}
		string GetRibbonPageName(RibbonPage page) {
			if(!string.IsNullOrEmpty(page.Name))
				return page.Name;
			else {
				string pageName = string.Format("{0}{1}{2}", page.Ribbon.MergedPages.Contains(page) ? "Merged" : String.Empty, page.GetType().Name, page.Collection.IndexOf(page));
				foreach(RibbonPage ribbonPage in page.Collection)
					if(ribbonPage.Name == pageName)
						pageName = "Custom" + pageName;
				return pageName;
			}
		}
		string GetRibbonGroupName(RibbonPageGroup group) {
			if(!string.IsNullOrEmpty(group.Name))
				return group.Name;
			else {
				bool isMerged = group.Page.MergedGroups.Contains(group);
				string groupName;
				RibbonPageGroupCollection collection;
				if(isMerged) {
					collection = group.Page.MergedGroups;
					groupName = "Merged" + group.GetType().Name + collection.IndexOf(group).ToString();
				} else {
					collection = group.Page.Groups;
					groupName = group.GetType().Name + collection.IndexOf(group).ToString();
				}
				foreach(RibbonPageGroup ribbonGroup in collection)
					if(ribbonGroup.Name == groupName)
						groupName = "Custom" + groupName;
				return groupName;
			}
		}
		object GetDockingManagerFromHandle(IntPtr windowHandle) {
			MdiClientSubclasser mdiClient = MdiClientSubclasser.FromHandle(windowHandle) as MdiClientSubclasser;
			if(mdiClient != null)
				return mdiClient.Owner;
			else {
				IDocumentsHost documentsHost = Control.FromHandle(windowHandle) as IDocumentsHost;
				if(documentsHost != null)
					return documentsHost.Owner;
			}
			return null;
		}
		public MdiClientElementInfo GetMdiClientElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			Point clientPoint = new Point(pointX, pointY);
			object manager = GetDockingManagerFromHandle(windowHandle);
			if(manager is DocumentManager) {
				DocumentManager documentManager = manager as DocumentManager;
				BaseViewHitInfo hitInfo = documentManager.CalcHitInfo(clientPoint);
				if(documentManager.UIView.LayoutRoot != null)
					if(documentManager.View is TabbedView) {
						MdiClientElementInfo result = GetTabbedViewElementFromPoint(documentManager, hitInfo);
						result.Manager = DockingManager.DocumentManager;
						return result;
					}
			} else if(manager is XtraTabbedMdiManager) {
				XtraTabbedMdiManager tabbedMdiManager = manager as XtraTabbedMdiManager;
				BaseTabHitInfo hitInfo = tabbedMdiManager.CalcHitInfo(clientPoint);
				MdiClientElementInfo result = GetXtraTabbedMdiManagerElementFromPoint(tabbedMdiManager, hitInfo);
				result.Manager = DockingManager.XtraTabbedMdiManager;
				return result;
			}
			return new MdiClientElementInfo();
		}
		MdiClientElementInfo GetXtraTabbedMdiManagerElementFromPoint(XtraTabbedMdiManager manager, BaseTabHitInfo hitInfo) {
			switch(hitInfo.HitTest) {
				case XtraTabHitTest.PageHeader:
					MdiClientElementInfo result = new MdiClientElementInfo() { ElementType = MdiClientElementType.Document };
					if(hitInfo.InPageControlBox) {
						BaseTabPageViewInfo pageInfo = manager.ViewInfo.HeaderInfo.FindPage(hitInfo.HitPoint);
						if(pageInfo != null && GetCloseButtonRect(pageInfo.ButtonsPanel).Contains(hitInfo.HitPoint))
							result.ElementType = MdiClientElementType.DocumentCloseButton;
					}
					result.DocumentName = GetXtraTabbedMdiManagerPageName(manager.Pages, hitInfo.Page);
					return result;
				case XtraTabHitTest.PageHeaderButtons:
					return GetXtraTabbedMdiTabButtonElementInfo(manager.ViewInfo.HeaderInfo.Buttons, hitInfo.HitPoint);
			}
			return new MdiClientElementInfo();
		}
		string GetXtraTabbedMdiManagerPageName(XtraMdiTabPageCollection pages, DevExpress.XtraTab.IXtraTabPage page) {
			string name = null;
			if(page != null) {
				name = page.Text;
				int num = 0;
				foreach(XtraMdiTabPage tabPage in pages)
					if(tabPage.Text == page.Text) {
						if(tabPage == page)
							break;
						else
							num++;
					}
				if(num != 0)
					name += "[" + num + "]";
			}
			return name;
		}
		MdiClientElementInfo GetTabbedViewElementFromPoint(DocumentManager manager, BaseViewHitInfo hitInfo) {
			switch(hitInfo.Info.HitTest) {
				case LayoutElementHitTest.Header:
					if(hitInfo.Document is Document)
						return GetDocumentInfo(hitInfo.Document as Document);
					break;
				case LayoutElementHitTest.ControlBox:
					if(hitInfo.Info.Element is DocumentInfoElement) {
						DocumentInfoElement documentInfo = hitInfo.Info.Element as DocumentInfoElement;
						BaseTabHeaderViewInfo headerInfo = documentInfo.TabPage.TabControl.ViewInfo.HeaderInfo;
						if(GetCloseButtonRect(headerInfo.VisiblePages[documentInfo.TabPage].ButtonsPanel).Contains(hitInfo.HitPoint)) {
							MdiClientElementInfo result = GetDocumentInfo(hitInfo.Document as Document);
							result.ElementType = MdiClientElementType.DocumentCloseButton;
							return result;
						} else {
							MdiClientElementInfo result = GetTabbedViewTabButtonElementInfo(documentInfo.Parent as DocumentGroupInfoElement, hitInfo.HitPoint);
							return result;
						}
					} else if(hitInfo.Info.Element is DocumentGroupInfoElement) {
						DocumentGroupInfoElement groupInfo = hitInfo.Info.Element as DocumentGroupInfoElement;
						MdiClientElementInfo result = GetTabbedViewTabButtonElementInfo(groupInfo, hitInfo.HitPoint);
						return result;
					}
					break;
			}
			return new MdiClientElementInfo();
		}
		public string GetMdiClientElementRectangle(IntPtr windowHandle, MdiClientElementInfo elementInfo) {
			Rectangle result = Rectangle.Empty;
			object manager = GetDockingManagerFromHandle(windowHandle);
			if(manager is DocumentManager) {
				DocumentManager documentManager = manager as DocumentManager;
				if(documentManager.UIView.LayoutRoot == null)
					documentManager.UIView.EnsureLayoutRoot();
				if(documentManager.UIView.LayoutRoot != null)
					if(documentManager.View is TabbedView)
						result = GetTabbedViewElementRectangle(documentManager, elementInfo);
			} else if(manager is XtraTabbedMdiManager)
				result = GetXtraTabbedMdiManagerElementRectangle(manager as XtraTabbedMdiManager, elementInfo);
			if(result != Rectangle.Empty)
				return CodedUIUtils.ConvertToString(result);
			else
				return null;
		}
		Rectangle GetXtraTabbedMdiManagerElementRectangle(XtraTabbedMdiManager manager, MdiClientElementInfo elementInfo) {
			switch(elementInfo.ElementType) {
				case MdiClientElementType.Document:
				case MdiClientElementType.DocumentCloseButton:
					PageViewInfoCollection pagesInfo = new PageViewInfoCollection();
					for(int i = 0; i < manager.ViewInfo.HeaderInfo.AllPages.Count; i++)
						pagesInfo.Add(manager.ViewInfo.HeaderInfo.AllPages[i]);
					foreach(BaseTabPageViewInfo pageInfo in pagesInfo)
						if(pageInfo.Page.Text == elementInfo.DocumentName) {
							if(elementInfo.ElementType == MdiClientElementType.DocumentCloseButton)
								return GetCloseButtonRect(pageInfo.ButtonsPanel);
							else
								return pageInfo.Bounds;
						}
					break;
				case MdiClientElementType.TabPanelButton:
					TabButtonInfo buttonInfo = GetTabButtonInfo(manager.ViewInfo.HeaderInfo.Buttons, elementInfo);
					if(buttonInfo != null)
						return buttonInfo.Bounds;
					break;
			}
			return Rectangle.Empty;
		}
		Rectangle GetTabbedViewElementRectangle(DocumentManager manager, MdiClientElementInfo elementInfo) {
			DocumentInfoElement documentInfo = null;
			DocumentGroupInfoElement groupInfo = null;
			switch(elementInfo.ElementType) {
				case MdiClientElementType.Document:
					documentInfo = GetTabbedViewDocumentInfo(manager, elementInfo);
					if(documentInfo != null) {
						documentInfo.EnsureBounds();
						Rectangle documentBounds = documentInfo.Bounds;
						if(documentInfo.TabPage.TabControl.ViewInfo.HeaderInfo.VisiblePages[documentInfo.TabPage] != null && documentInfo.TabPage.TabControl.ViewInfo.Bounds.IntersectsWith(documentBounds)) {
							documentBounds.Intersect(documentInfo.TabPage.TabControl.ViewInfo.Bounds);
							int buttonsX = documentInfo.TabPage.TabControl.ViewInfo.HeaderInfo.ButtonsBounds.X;
							if(buttonsX > 0 && documentBounds.Right > buttonsX) {
								if(documentBounds.X < buttonsX)
									return new Rectangle(documentBounds.X, documentBounds.Y, buttonsX - documentBounds.X, documentBounds.Height);
							} else
								return documentBounds;
						}
					}
					break;
				case MdiClientElementType.DocumentCloseButton:
					documentInfo = GetTabbedViewDocumentInfo(manager, elementInfo);
					if(documentInfo != null)
						return GetCloseButtonRect(documentInfo.TabPage.TabControl.ViewInfo.HeaderInfo.VisiblePages[documentInfo.TabPage].ButtonsPanel);
					break;
				case MdiClientElementType.TabPanelButton:
					TabbedViewElement viewElement = GetTabbedViewElement(manager);
					if(elementInfo.DocumentGroupIndex < viewElement.Items.Count)
						groupInfo = viewElement.Items[elementInfo.DocumentGroupIndex] as DocumentGroupInfoElement;
					else
						groupInfo = viewElement.Items[viewElement.Items.Count - 1] as DocumentGroupInfoElement;
					if(groupInfo != null) {
						TabButtonInfo buttonInfo = GetTabButtonInfo(groupInfo.Tab.ViewInfo.HeaderInfo.Buttons, elementInfo);
						if(buttonInfo != null)
							return buttonInfo.Bounds;
					}
					break;
			}
			return Rectangle.Empty;
		}
		Rectangle GetCloseButtonRect(IButtonsPanel buttonsPanel) {
			if(buttonsPanel != null && buttonsPanel.ViewInfo != null) {
				foreach(BaseButtonInfo item in buttonsPanel.ViewInfo.Buttons) {
					if(item.Button is BaseCloseButton)
						return item.Bounds;
				}
			}
			return Rectangle.Empty;
		}
		MdiClientElementInfo GetTabbedViewTabButtonElementInfo(DocumentGroupInfoElement groupInfo, Point point) {
			TabButtonInfoCollection buttons = GetTabbedViewTabButtonInfoCollection(groupInfo.Tab.ViewInfo.HeaderInfo.Buttons);
			if(buttons != null)
				foreach(TabButtonInfo buttonInfo in buttons)
					if(buttonInfo.Bounds.Contains(point)) {
						MdiClientElementInfo elementInfo = GetTabButtonElementInfo(buttonInfo);
						elementInfo.DocumentGroupIndex = ((TabbedView)groupInfo.Info.Owner).DocumentGroups.IndexOf(groupInfo.Info.Group);
						return elementInfo;
					}
			return new MdiClientElementInfo();
		}
		MdiClientElementInfo GetXtraTabbedMdiTabButtonElementInfo(TabButtonsPanel buttonsPanel, Point point) {
			TabButtonInfoCollection buttons = GetTabbedViewTabButtonInfoCollection(buttonsPanel);
			if(buttons != null)
				foreach(TabButtonInfo buttonInfo in buttons)
					if(buttonInfo.Bounds.Contains(point)) {
						return GetTabButtonElementInfo(buttonInfo);
					}
			return new MdiClientElementInfo();
		}
		MdiClientElementInfo GetTabButtonElementInfo(TabButtonInfo buttonInfo) {
			switch(buttonInfo.ButtonType) {
				case TabButtonType.Close:
					return new MdiClientElementInfo() { ButtonType = TabPanelButtonType.Close, ElementType = MdiClientElementType.TabPanelButton };
				case TabButtonType.Next:
					return new MdiClientElementInfo() { ButtonType = TabPanelButtonType.Next, ElementType = MdiClientElementType.TabPanelButton };
				case TabButtonType.Prev:
					return new MdiClientElementInfo() { ButtonType = TabPanelButtonType.Prev, ElementType = MdiClientElementType.TabPanelButton };
				case TabButtonType.User:
					if(buttonInfo.Button.Kind == ButtonPredefines.DropDown && buttonInfo.Button.Tag == null)
						return new MdiClientElementInfo() { ButtonType = TabPanelButtonType.DropDown, ElementType = MdiClientElementType.TabPanelButton };
					else
						return new MdiClientElementInfo() {
							ButtonType = TabPanelButtonType.Custom,
							ButtonName = !string.IsNullOrEmpty(buttonInfo.Button.Tag as string) ? buttonInfo.Button.Tag.ToString() : buttonInfo.Button.Index.ToString(),
							ElementType = MdiClientElementType.TabPanelButton
						};
			}
			return new MdiClientElementInfo();
		}
		TabButtonInfo GetTabButtonInfo(TabButtonsPanel buttonsPanel, MdiClientElementInfo elementInfo) {
			TabButtonInfoCollection buttons = GetTabbedViewTabButtonInfoCollection(buttonsPanel);
			foreach(TabButtonInfo buttonInfo in buttons) {
				MdiClientElementInfo buttonElementInfo = GetTabButtonElementInfo(buttonInfo);
				if(buttonElementInfo.ButtonName == elementInfo.ButtonName && buttonElementInfo.ButtonType == elementInfo.ButtonType)
					return buttonInfo;
			}
			return null;
		}
		DocumentInfoElement GetTabbedViewDocumentInfo(DocumentManager manager, MdiClientElementInfo elementInfo) {
			IEnumerator<ILayoutElement> e = manager.UIView.LayoutRoot.GetEnumerator();
			while(e.MoveNext()) {
				BaseLayoutElement element = (BaseLayoutElement)e.Current;
				if(element is DocumentInfoElement) {
					MdiClientElementInfo documentInfo = GetDocumentInfo(((element as DocumentInfoElement).TabPage as DocumentInfo).Document);
					if(elementInfo.DocumentName == documentInfo.DocumentName)
						return element as DocumentInfoElement;
				}
			}
			return null;
		}
		TabbedViewElement GetTabbedViewElement(DocumentManager manager) {
			IEnumerator<ILayoutElement> e = manager.UIView.LayoutRoot.GetEnumerator();
			while(e.MoveNext())
				if(e.Current is TabbedViewElement)
					return e.Current as TabbedViewElement;
			return null;
		}
		TabButtonInfoCollection GetTabbedViewTabButtonInfoCollection(TabButtonsPanel panel) {
			PropertyInfo pi = panel.GetType().GetProperty("Buttons", BindingFlags.NonPublic | BindingFlags.Instance);
			if(pi != null)
				return pi.GetValue(panel, new object[] { }) as TabButtonInfoCollection;
			return null;
		}
		public MdiClientDocumentDockInfo GetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo) {
			MdiClientDocumentDockInfo result = new MdiClientDocumentDockInfo();
			object manager = GetDockingManagerFromHandle(windowHandle);
			if(manager is DocumentManager) {
				DocumentManager documentManager = manager as DocumentManager;
				if(documentManager.View is TabbedView) {
					TabbedView tabbedView = documentManager.View as TabbedView;
					Document document = GetTabbedViewDocument(tabbedView, elementInfo);
					if(document != null)
						return GetTabbedViewDocumentDockInfo(tabbedView, document);
				}
			} else if(manager is XtraTabbedMdiManager) {
				return GetXtraTabbedMdiPageDockInfo(manager as XtraTabbedMdiManager, elementInfo.DocumentName);
			}
			return result;
		}
		public void SetMdiClientDocumentDockInfo(IntPtr windowHandle, MdiClientElementInfo elementInfo, MdiClientDocumentDockInfo dockInfo) {
			object manager = GetDockingManagerFromHandle(windowHandle);
			if(manager is DocumentManager) {
				DocumentManager documentManager = manager as DocumentManager;
				if(documentManager.View is TabbedView)
					(documentManager.MdiParent == null ? documentManager.ContainerControl : documentManager.MdiParent).BeginInvoke(new MethodInvoker(delegate() {
						Document document = GetTabbedViewDocument(documentManager.View as TabbedView, elementInfo);
						SetTabbedViewDocumentDockInfo(documentManager.View as TabbedView, document, dockInfo);
					}));
			} else if(manager is XtraTabbedMdiManager) {
				(manager as XtraTabbedMdiManager).MdiParent.BeginInvoke(new MethodInvoker(delegate() {
					SetXtraTabbedMdiPageDockInfo(manager as XtraTabbedMdiManager, elementInfo.DocumentName, dockInfo);
				}));
			} else {
				DockPanel panel = Control.FromHandle(windowHandle) as DockPanel;
				if(panel != null) {
					if(elementInfo.DockPanelId != Guid.Empty)
						panel = GetDockPanelFromId(panel.DockManager, elementInfo.DockPanelId);
					if(panel != null && panel.DockManager != null && panel.DockManager.DocumentManager != null && panel.DockManager.DocumentManager.View is TabbedView)
						panel.BeginInvoke(new MethodInvoker(delegate() {
							TabbedViewController controller = (panel.DockManager.DocumentManager.View as TabbedView).Controller as TabbedViewController;
							Point location = panel.TopLevelControl.PointToClient(panel.PointToScreen(panel.Location));
							panel.Dock = DockingStyle.Float;
							panel.FloatLocation = location;
							Document document = controller.RegisterDockPanel(panel.FloatForm) as Document;
							SetTabbedViewDocumentDockInfo(panel.DockManager.DocumentManager.View as TabbedView, document, dockInfo);
						}));
				}
			}
		}
		void SetTabbedViewDocumentDockInfo(TabbedView tabbedView, Document document, MdiClientDocumentDockInfo dockInfo) {
			TabbedViewController controller = tabbedView.Controller as TabbedViewController;
			if(document != null) {
				if(!(document.Control is FloatForm && (document.Control as FloatForm).FloatLayout.Float))
					controller.Float(document);
				if(dockInfo.IsFloating)
					return;
				if(dockInfo.DocumentIndex == -1) {
					controller.CreateNewDocumentGroup(document, dockInfo.Orientation == OrientationKind.Vertical ? Orientation.Horizontal : Orientation.Vertical, dockInfo.DocumentGroupIndex);
				} else
					controller.Dock(document, tabbedView.DocumentGroups[dockInfo.DocumentGroupIndex], dockInfo.DocumentIndex);
			}
		}
		void SetXtraTabbedMdiPageDockInfo(XtraTabbedMdiManager manager, string pageName, MdiClientDocumentDockInfo dockInfo) {
			foreach(Form form in manager.FloatForms)
				if(form.Text == pageName) {
					if(!dockInfo.IsFloating) {
						MethodInfo dockCoreMethod = manager.GetType().GetMethod("DockCore", BindingFlags.NonPublic | BindingFlags.Instance);
						if(dockCoreMethod != null) {
							dockCoreMethod.Invoke(manager, new object[] { form, manager });
							foreach(XtraMdiTabPage page in manager.Pages)
								if(page.MdiChild == form) {
									if(manager.Pages.IndexOf(page) != dockInfo.DocumentIndex) {
										manager.Pages.Remove(page);
										manager.Pages.Insert(dockInfo.DocumentIndex, page);
										manager.SelectedPage = page;
										manager.LayoutChanged();
									}
									return;
								}
						}
					}
					return;
				}
			foreach(XtraMdiTabPage page in manager.Pages)
				if(page.Text == pageName) {
					if(dockInfo.IsFloating)
						manager.Float(page, page.MdiChild.PointToScreen(page.MdiChild.Location));
					else if(manager.Pages.IndexOf(page) != dockInfo.DocumentIndex) {
						manager.Pages.Remove(page);
						manager.Pages.Insert(dockInfo.DocumentIndex, page);
						manager.SelectedPage = page;
						manager.LayoutChanged();
					}
					return;
				}
		}
		MdiClientDocumentDockInfo GetTabbedViewDocumentDockInfo(TabbedView tabbedView, Document document) {
			MdiClientDocumentDockInfo info = new MdiClientDocumentDockInfo();
			info.Manager = DockingManager.DocumentManager;
			if(tabbedView.Orientation == Orientation.Horizontal)
				info.Orientation = OrientationKind.Horizontal;
			else
				info.Orientation = OrientationKind.Vertical;
			if(document.IsFloating) {
				info.IsFloating = true;
				return info;
			}
			if(document.Info.GroupInfo == null) {
				info.DocumentGroupIndex = 0;
				info.DocumentIndex = -1;
				return info;
			}
			if(document.Info.GroupInfo.Group.Items.Count == 1)
				info.DocumentIndex = -1;
			else
				info.DocumentIndex = document.Info.GroupInfo.Group.Items.IndexOf(document);
			info.DocumentGroupIndex = tabbedView.DocumentGroups.IndexOf(document.Info.GroupInfo.Group);
			return info;
		}
		MdiClientDocumentDockInfo GetXtraTabbedMdiPageDockInfo(XtraTabbedMdiManager manager, string documentName) {
			MdiClientDocumentDockInfo info = new MdiClientDocumentDockInfo();
			foreach(XtraMdiTabPage page in manager.Pages)
				if(page.Text == documentName) {
					info.DocumentIndex = manager.Pages.IndexOf(page);
					info.Manager = DockingManager.XtraTabbedMdiManager;
					return info;
				}
			foreach(Form form in manager.FloatForms)
				if(form.Text == documentName) {
					info.IsFloating = true;
					info.Manager = DockingManager.XtraTabbedMdiManager;
				}
			return info;
		}
		public MdiClientElementInfo IfFormIsMdiChildGetDocumentInfo(IntPtr windowHandle) {
			MdiClientElementInfo result = new MdiClientElementInfo();
			Control control = Control.FromHandle(windowHandle);
			if(control != null) {
				DocumentManager manager = DocumentManager.FromControl(control);
				if(manager != null && manager.View is TabbedView) {
					BaseDocument document = manager.GetDocument(control);
					if(document == null)
						if(control is FloatDocumentForm)
							document = (control as FloatDocumentForm).Document;
					if(document != null) {
						result = GetDocumentInfo(document);
						if(manager.MdiParent != null) {
							MdiClient mdiClient = MdiClientSubclasserService.GetMdiClient(manager.MdiParent);
							if(mdiClient != null)
								result.MdiClientHandle = mdiClient.Handle.ToInt32();
						} else if(manager.GetOwnerControl() != null)
							result.MdiClientHandle = manager.GetOwnerControlHandle().ToInt32();
					}
				} else {
					if(control is Form) {
						Form form = control as Form;
						XtraTabbedMdiManager tabbedMdiManager = XtraTabbedMdiManager.GetXtraTabbedMdiManager(form.Owner);
						if(tabbedMdiManager != null) {
							result.ElementType = MdiClientElementType.Document;
							result.DocumentName = control.Text;
							MdiClient mdiClient = MdiClientSubclasserService.GetMdiClient(tabbedMdiManager.MdiParent);
							if(mdiClient != null)
								result.MdiClientHandle = mdiClient.Handle.ToInt32();
						}
					}
				}
			}
			return result;
		}
		public MdiClientElementInfo GetDocumentContainerDocumentInfo(IntPtr windowHandle) {
			DocumentContainer control = Control.FromHandle(windowHandle) as DocumentContainer;
			if(control != null) {
				if(control.Document != null) {
					MdiClientElementInfo documentInfo = GetDocumentInfo(control.Document);
					if(control.Document.Manager != null && control.Document.Manager.View is TabbedView) {
						if(control.Document.Manager.MdiParent != null) {
							MdiClient mdiClient = MdiClientSubclasserService.GetMdiClient(control.Document.Manager.MdiParent);
							if(mdiClient != null)
								documentInfo.MdiClientHandle = mdiClient.Handle.ToInt32();
						} else if(control.Document.Manager.GetOwnerControl() != null)
							documentInfo.MdiClientHandle = control.Document.Manager.GetOwnerControlHandle().ToInt32();
					}
					return documentInfo;
				}
			}
			return new MdiClientElementInfo();
		}
		public DocumentContainerButtonType GetDocumentContainerButtonTypeFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			DocumentContainer control = Control.FromHandle(windowHandle) as DocumentContainer;
			if(control != null) {
				Point clientPoint = new Point(pointX, pointY);
				if(control.ButtonsPanel != null && control.ButtonsPanel.ViewInfo != null && control.ButtonsPanel.ViewInfo.Buttons != null)
					foreach(BaseButtonInfo buttonInfo in control.ButtonsPanel.ViewInfo.Buttons)
						if(buttonInfo.Bounds.Contains(clientPoint)) {
							if(buttonInfo.Button is DevExpress.XtraBars.Docking.CloseButton)
								return DocumentContainerButtonType.Close;
							else if(buttonInfo.Button is DevExpress.XtraBars.Docking.MaximizeButton)
								return DocumentContainerButtonType.Maximize;
							else if(buttonInfo.Button is DevExpress.XtraBars.Docking.PinButton)
								return DocumentContainerButtonType.Pin;
						}
			}
			return DocumentContainerButtonType.Undefined;
		}
		public string GetDocumentContainerButtonRectangle(IntPtr windowHandle, DocumentContainerButtonType buttonType) {
			DocumentContainer control = Control.FromHandle(windowHandle) as DocumentContainer;
			if(control != null) {
				BaseButtonInfo buttonInfo = GetDocumentContainerButtonInfo(control, buttonType);
				if(buttonInfo != null)
					return CodedUIUtils.ConvertToString(buttonInfo.Bounds);
			}
			return null;
		}
		BaseButtonInfo GetDocumentContainerButtonInfo(DocumentContainer control, DocumentContainerButtonType buttonType) {
			if(control != null) {
				if(control.ButtonsPanel != null && control.ButtonsPanel.ViewInfo != null && control.ButtonsPanel.ViewInfo.Buttons != null)
					foreach(BaseButtonInfo buttonInfo in control.ButtonsPanel.ViewInfo.Buttons)
						if((buttonType == DocumentContainerButtonType.Close && buttonInfo.Button is DevExpress.XtraBars.Docking.CloseButton) ||
							(buttonType == DocumentContainerButtonType.Maximize && buttonInfo.Button is DevExpress.XtraBars.Docking.MaximizeButton) ||
							(buttonType == DocumentContainerButtonType.Pin && buttonInfo.Button is DevExpress.XtraBars.Docking.PinButton))
							return buttonInfo;
			}
			return null;
		}
		MdiClientElementInfo GetDocumentInfo(BaseDocument document) {
			if(document != null)
				return new MdiClientElementInfo() {
					ElementType = MdiClientElementType.Document,
					DocumentName = GetDocumentName(document),
					DockPanelId = GetDocumentDockPanelID(document),
					Manager = DockingManager.DocumentManager
				};
			else
				return new MdiClientElementInfo();
		}
		Guid GetDocumentDockPanelID(BaseDocument document) {
			if(document.Control is FloatForm && (document.Control as FloatForm).FloatLayout.Panel != null)
				return (document.Control as FloatForm).FloatLayout.Panel.ID;
			else
				return Guid.Empty;
		}
		string GetDocumentName(BaseDocument document) {
			return document.Caption;
		}
		Document GetTabbedViewDocument(TabbedView view, MdiClientElementInfo elementInfo) {
			Document document = GetTabbedViewDocumentFromCollection(view.Documents, elementInfo);
			if(document == null)
				document = GetTabbedViewDocumentFromCollection(view.FloatDocuments, elementInfo);
			return document;
		}
		Document GetTabbedViewDocumentFromCollection(BaseDocumentCollection collection, MdiClientElementInfo elementInfo) {
			foreach(Document document in collection) {
				MdiClientElementInfo documentInfo = GetDocumentInfo(document);
				if(elementInfo.DocumentName == null && elementInfo.DockPanelId != Guid.Empty) {
					if(elementInfo.DockPanelId == documentInfo.DockPanelId)
						return document;
				} else if(elementInfo.DocumentName == documentInfo.DocumentName)
					return document;
			}
			return null;
		}
		public FormElementInfo GetFloatDocumentFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			FloatDocumentForm form = Control.FromHandle(windowHandle) as FloatDocumentForm;
			FormElementInfo result = new FormElementInfo();
			if(form != null) {
				Point point = new Point(pointX, pointY);
				if(form.ButtonsPanel != null && form.ButtonsPanel.ViewInfo.Buttons != null) {
					foreach(BaseButtonInfo button in form.ButtonsPanel.ViewInfo.Buttons)
						if(button.Bounds.Contains(point)) {
							result.ElementType = FormElements.Button;
							result.Name = button.Button.Properties.ToolTip;
							break;
						}
				}
				if(result.ElementType == FormElements.Unknown)
					if(form.Info.caption.Contains(point))
						result.ElementType = FormElements.Caption;
			}
			return result;
		}
		public string GetFloatDocumentFormElementRectangle(IntPtr windowHandle, FormElementInfo elementInfo) {
			FloatDocumentForm form = Control.FromHandle(windowHandle) as FloatDocumentForm;
			if(form != null) {
				switch(elementInfo.ElementType) {
					case FormElements.Button:
						if(form.ButtonsPanel != null && form.ButtonsPanel.ViewInfo.Buttons != null)
							foreach(BaseButtonInfo button in form.ButtonsPanel.ViewInfo.Buttons)
								if(elementInfo.Name == button.Button.Properties.ToolTip)
									return CodedUIUtils.ConvertToString(button.Bounds);
						break;
					case FormElements.Caption:
						return CodedUIUtils.ConvertToString(form.Info.caption);
				}
			}
			return null;
		}
		public Guid GetDockPanelIdFromTabHeader(IntPtr windowHandle, string tabName) {
			DocumentManager documentManager = GetDockingManagerFromHandle(windowHandle) as DocumentManager;
			if(documentManager != null) {
				foreach(Document document in documentManager.View.Documents) {
					MdiClientElementInfo documentInfo = GetDocumentInfo(document);
					if(documentInfo.DocumentName == tabName && documentInfo.DockPanelId != Guid.Empty)
						return documentInfo.DockPanelId;
				}
			} else {
				DockPanel panel = Control.FromHandle(windowHandle) as DockPanel;
				if(panel != null) {
					int tabButtonIndex = GetDockPanelTabButtonIndexFromName(panel.DockLayout, tabName);
					if(tabButtonIndex != -1 && panel.DockLayout.Count > tabButtonIndex)
						return panel.DockLayout[tabButtonIndex].Panel.ID;
				}
			}
			return Guid.Empty;
		}
		DockPanel GetDockPanelFromName(DockManager manager, string name) {
			if(manager != null)
				foreach(DockPanel panel in manager.Panels)
					if(panel.Name == name)
						return panel;
			return null;
		}
		DockPanel GetDockPanelFromId(DockManager manager, Guid id) {
			if(manager != null)
				foreach(DockPanel panel in manager.Panels)
					if(panel.ID == id)
						return panel;
			return null;
		}
		public string HandleDocumentManagerViaReflection(IntPtr windowHandle, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			DocumentManager documentManager = GetDockingManagerFromHandle(windowHandle) as DocumentManager;
			if(documentManager != null) {
				ClientSideHelper csh = new ClientSideHelper(this.remoteObject);
				return csh.HandleViaReflection(documentManager, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
			}
			return null;
		}
		public string GetDocumentSelectorItemNameFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			DocumentSelectorAdornerElementInfoArgs selector = GetDocumentSelector(GetDockingManagerFromHandle(windowHandle) as DocumentManager);
			if(selector != null) {
				Point point = selector.Owner.ClientToScreen(selector.RenderOrigin);
				Point point2 = new Point(pointX - point.X, pointY - point.Y);
				if(selector.Documents != null)
					foreach(DocumentSelectorItemInfo item in selector.Documents.PaintItemInfo)
						if(item.Bounds.Contains(point2))
							return item.Text;
				if(selector.Panels != null)
					foreach(DocumentSelectorItemInfo item in selector.Panels.PaintItemInfo)
						if(item.Bounds.Contains(point2))
							return item.Text;
			}
			return null;
		}
		DocumentSelectorAdornerElementInfoArgs GetDocumentSelector(DocumentManager documentManager) {
			if(documentManager != null) {
				FieldInfo fi = documentManager.GetType().GetField("documentSelectorBootStrapperCore", BindingFlags.NonPublic | BindingFlags.Instance);
				if(fi != null) {
					DocumentSelectorBootStrapper documentSelectorBootStrapper = fi.GetValue(documentManager) as DocumentSelectorBootStrapper;
					if(documentSelectorBootStrapper != null) {
						fi = documentSelectorBootStrapper.GetType().GetField("Selector", BindingFlags.NonPublic | BindingFlags.Instance);
						if(fi != null)
							return fi.GetValue(documentSelectorBootStrapper) as DocumentSelectorAdornerElementInfoArgs;
					}
				}
			}
			return null;
		}
		public string GetDocumentSelectorItemRectangle(IntPtr windowHandle, string itemName) {
			DocumentSelectorAdornerElementInfoArgs selector = GetDocumentSelector(GetDockingManagerFromHandle(windowHandle) as DocumentManager);
			if(selector != null) {
				DocumentSelectorItemInfo selectorItem = null;
				if(selector.Documents != null)
					foreach(DocumentSelectorItemInfo item in selector.Documents.PaintItemInfo)
						if(item.Text == itemName)
							selectorItem = item;
				if(selector.Panels != null)
					foreach(DocumentSelectorItemInfo item in selector.Panels.PaintItemInfo)
						if(item.Text == itemName)
							selectorItem = item;
				if(selectorItem != null) {
					Point point = selector.Owner.ClientToScreen(selector.RenderOrigin);
					Rectangle result = new Rectangle(new Point(selector.RenderOrigin.X + selectorItem.Bounds.Location.X, selector.RenderOrigin.Y + selectorItem.Bounds.Location.Y), selectorItem.Bounds.Size);
					return CodedUIUtils.ConvertToString(result);
				}
			}
			return null;
		}
		public void MakeMdiClientDocumentActive(IntPtr documentManagerControlHandle, IntPtr formHandle) {
			DocumentManager manager = GetDockingManagerFromHandle(documentManagerControlHandle) as DocumentManager;
			Control form = Control.FromHandle(formHandle);
			if(manager != null && form != null) {
				if(manager.View is TabbedView) {
					TabbedView view = manager.View as TabbedView;
					BaseDocument document = null;
					if(form is FloatDocumentForm)
						document = (form as FloatDocumentForm).Document;
					if(document == null)
						document = manager.GetDocument(form);
					if(document != null && !document.IsFloating)
						(manager.MdiParent == null ? manager.ContainerControl : manager.MdiParent).BeginInvoke(new MethodInvoker(delegate() {
							view.ActivateDocument(document.Control);
						}));
				}
			}
		}
		public AccordionControlElementInfo GetAccordionControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			AccordionControl accordion = Control.FromHandle(windowHandle) as AccordionControl;
			AccordionControlElementInfo elementInfo = new AccordionControlElementInfo();
			if(accordion == null)
				return elementInfo;
			AccordionControlHitInfo hitInfo = accordion.CalcHitInfo(new Point(pointX, pointY));
			switch(hitInfo.HitTest) {
				case AccordionControlHitTest.Item:
					if(hitInfo.ItemInfo is AccordionItemViewInfo && (hitInfo.ItemInfo as AccordionItemViewInfo).ParentElementInfo != null) {
						elementInfo.ElementType = AccordionControlElements.Item;
						elementInfo.ItemName = hitInfo.ItemInfo.Text;
						elementInfo.GroupName = (hitInfo.ItemInfo as AccordionItemViewInfo).ParentElementInfo.Text;
					}
					break;
				case AccordionControlHitTest.Group:
					elementInfo.ElementType = AccordionControlElements.Group;
					elementInfo.GroupName = hitInfo.ItemInfo.Text;
					break;
			}
			return elementInfo;
		}
		public string GetAccordionControlElementRectangle(IntPtr windowHandle, AccordionControlElementInfo elementInfo) {
			AccordionControl accordion = Control.FromHandle(windowHandle) as AccordionControl;
			if(accordion == null)
				return null;
			switch(elementInfo.ElementType) {
				case AccordionControlElements.Group:
				case AccordionControlElements.Item:
					Rectangle accordionElementRectangle = GetAccordionControlElementRectangle(accordion, elementInfo);
					if(accordionElementRectangle != Rectangle.Empty)
						return CodedUIUtils.ConvertToString(accordionElementRectangle);
					else
						return null;
				default:
					return null;
			}
		}
		AccordionControlElement GetAccordionControlElement(AccordionControl accordion, AccordionControlElementInfo elementInfo) {
			foreach(AccordionControlElement group in accordion.Elements)
				if(group.Text == elementInfo.GroupName) {
					if(elementInfo.ElementType == AccordionControlElements.Group)
						return group;
					else if(elementInfo.ElementType == AccordionControlElements.Item)
						foreach(AccordionControlElement item in group.Elements)
							if(item.Text == elementInfo.ItemName)
								return item;
				}
			return null;
		}
		Rectangle GetAccordionControlElementRectangle(AccordionControl accordion, AccordionControlElementInfo elementInfo) {
			AccordionControlElement element = GetAccordionControlElement(accordion, elementInfo);
			if(element != null && IsAccordionControlElementVisible(accordion, element))
				return element.Bounds;
			else
				return Rectangle.Empty;
		}
		bool IsAccordionControlElementVisible(AccordionControl accordion, AccordionControlElement element) {
			return element.Bounds.Bottom <= accordion.ControlInfo.ContentRect.Bottom && element.Bounds.Y >= accordion.ControlInfo.ContentRect.Y;
		}
		public void MakeAccordionControlElementVisible(IntPtr windowHandle, AccordionControlElementInfo elementInfo) {
			AccordionControl accordion = Control.FromHandle(windowHandle) as AccordionControl;
			if(accordion == null)
				return;
			AccordionControlElement element = GetAccordionControlElement(accordion, elementInfo);
			if(element != null)
				accordion.BeginInvoke(new MethodInvoker(delegate() {
					accordion.MakeElementVisible(element);
				}));
		}
	}
}
