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
using System.Windows.Forms;
using DevExpress.Utils.CodedUISupport;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Helpers;
namespace DevExpress.XtraBars.CodedUISupport {
	class CodedUIMessagesHandler {
		protected static bool controlsAddedToDictionary;
		protected static DXControlsSupported GetCodedUIEnum(object control) {
			if(!controlsAddedToDictionary) {
				ControlsToCodedUIEnum.Add(typeof(SubMenuBarControl), DXControlsSupported.SubMenuBarControl);
				ControlsToCodedUIEnum.Add(typeof(PopupMenuBarControl), DXControlsSupported.PopupMenuBarControl);
				ControlsToCodedUIEnum.Add(typeof(GalleryDropDownBarControl), DXControlsSupported.GalleryDropDownBarControl);
				ControlsToCodedUIEnum.Add(typeof(CustomLinksControl), DXControlsSupported.CustomLinksControl);
				ControlsToCodedUIEnum.Add(typeof(CustomPopupBarControl), DXControlsSupported.CustomPopupBarControl);
				ControlsToCodedUIEnum.Add(typeof(CustomBarControl), DXControlsSupported.CustomBarControl);
				ControlsToCodedUIEnum.Add(typeof(PopupContainerForm), DXControlsSupported.XtraBarsPopupContainerForm);
				ControlsToCodedUIEnum.Add(typeof(SubMenuControlForm), DXControlsSupported.SubMenuControlForm);
				ControlsToCodedUIEnum.Add(typeof(AlertForm), DXControlsSupported.AlertForm);
				ControlsToCodedUIEnum.Add(typeof(FloatingBarControlForm), DXControlsSupported.FloatingBarControlForm);
				ControlsToCodedUIEnum.Add(typeof(DockPanel), DXControlsSupported.DockPanel);
				ControlsToCodedUIEnum.Add(typeof(AutoHideContainer), DXControlsSupported.AutoHideContainer);
				ControlsToCodedUIEnum.Add(typeof(AutoHideControl), DXControlsSupported.AutoHideControl);
				ControlsToCodedUIEnum.Add(typeof(RibbonControl), DXControlsSupported.RibbonControl);
				ControlsToCodedUIEnum.Add(typeof(FloatForm), DXControlsSupported.FloatForm);
				ControlsToCodedUIEnum.Add(typeof(GalleryItemImagePopupForm), DXControlsSupported.GalleryItemImagePopupForm);
				ControlsToCodedUIEnum.Add(typeof(BackstageViewControl), DXControlsSupported.BackstageViewControl);
				ControlsToCodedUIEnum.Add(typeof(GalleryControl), DXControlsSupported.GalleryControl);
				ControlsToCodedUIEnum.Add(typeof(GalleryControlClient), DXControlsSupported.GalleryControlClient);
				ControlsToCodedUIEnum.Add(typeof(AppMenuFileLabel), DXControlsSupported.AppMenuFileLabel);
				ControlsToCodedUIEnum.Add(typeof(MdiClientSubclasser), DXControlsSupported.MdiClient);
				ControlsToCodedUIEnum.Add(typeof(DocumentsHost), DXControlsSupported.DocumentsHost);
				ControlsToCodedUIEnum.Add(typeof(RibbonStatusBar), DXControlsSupported.RibbonStatusBar);
				ControlsToCodedUIEnum.Add(typeof(FloatDocumentForm), DXControlsSupported.FloatDocumentForm);
				ControlsToCodedUIEnum.Add(typeof(AdornerLayeredWindow), DXControlsSupported.AdornerLayeredWindow);
				ControlsToCodedUIEnum.Add(typeof(DocumentContainer), DXControlsSupported.DocumentContainer);
				ControlsToCodedUIEnum.Add(typeof(AccordionControl), DXControlsSupported.AccordionControl);
				controlsAddedToDictionary = true;
			}
			return ControlsToCodedUIEnum.GetCodedUIEnum(control);
		}
		public static void ProcessCodedUIMessage(ref Message message, object sender) {
			if(CodedUIUtils.IsIdentifyControlMessage(ref message, sender)) {
				message.Result = new IntPtr((int)GetCodedUIEnum(sender));
			}
		}
	}
}
