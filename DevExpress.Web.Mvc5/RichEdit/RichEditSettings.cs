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

using DevExpress.Web.ASPxRichEdit;
using DevExpress.Web.Office;
using DevExpress.XtraRichEdit;
namespace DevExpress.Web.Mvc {
	public class RichEditSettings : SettingsBase {
		public RichEditSettings() 
			: base() {
			ImagesRuler = new RichEditRulerImages(null);
			RibbonTabs = new RichEditRibbonTabCollection(null);
			SettingsDocumentSelector = new RichEditDocumentSelectorSettings(null);
			Settings = new ASPxRichEditSettings(null);
			StylesButton = new RichEditButtonStyles(null);
			StylesEditors = new RichEditEditorsStyles(null);
			StylesFileManager = new RichEditFileManagerStyles(null);
			StylesPopupMenu = new RichEditMenuStyles(null);
			StylesRibbon = new RichEditRibbonStyles(null);
			StylesRuler = new RichEditRulerStyles(null);
			StylesStatusBar = new RichEditStatusBarStyles(null);
			ActiveTabIndex = 1;
			ConfirmOnLosingChanges = string.Empty;
			RibbonMode = RichEditRibbonMode.Ribbon;
			ShowConfirmOnLosingChanges = true;
			ShowStatusBar = true;
			WorkDirectory = string.Empty;
		}
		public int ActiveTabIndex { get; set; }
		public string AssociatedRibbonName { get; set; }
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public RichEditClientSideEvents ClientSideEvents { get { return (RichEditClientSideEvents)ClientSideEventsInternal; } }
		public string ConfirmOnLosingChanges { get; set; }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public bool ReadOnly { get; set; }
		public RichEditImages Images { get { return (RichEditImages)ImagesInternal; } }
		public RichEditRulerImages ImagesRuler { get; private set; }
		public RichEditRibbonMode RibbonMode { get; set; }
		public RichEditRibbonTabCollection RibbonTabs { get; private set; }
		public ASPxRichEditSettings Settings { get; private set; }
		public RichEditDocumentSelectorSettings SettingsDocumentSelector { get; private set; }
		public bool ShowConfirmOnLosingChanges { get; set; }
		public bool ShowStatusBar { get; set; }
		public RichEditStyles Styles { get { return (RichEditStyles)StylesInternal; } }
		public RichEditButtonStyles StylesButton { get; private set; }
		public RichEditEditorsStyles StylesEditors { get; private set; }
		public RichEditFileManagerStyles StylesFileManager { get; private set; }
		public RichEditMenuStyles StylesPopupMenu { get; private set; }
		public RichEditRibbonStyles StylesRibbon { get; private set; }
		public RichEditRulerStyles StylesRuler { get; private set; }
		public RichEditStatusBarStyles StylesStatusBar { get; private set; }
		public bool ViewMergedData { get; set; }
		public string WorkDirectory { get; set; }
		public DocumentSavingEventHandler Saving { get; set; }
		public CalculateDocumentVariableEventHandler CalculateDocumentVariable { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RichEditClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new RichEditImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new RichEditStyles(null);
		}
	}
}
