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
using System.Resources;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Design.Commands;
using System.Collections.Generic;
namespace DevExpress.XtraReports.UserDesigner.Native
{
	public static class EventArgsHelper {
		public static bool AreValidArgs(ComponentChangedEventArgs e) {
			return !(e.Member is EventDescriptor); 
		}
	} 
	public class LocalizableToolboxItem : ToolboxItem {
		Type type;
		string baseDisplayName;
		public LocalizableToolboxItem(Type type)
			: base(type) {
			this.type = type;
			base.DisplayName = GetDisplayName();
			baseDisplayName = base.DisplayName;
			System.Diagnostics.Debug.Assert(string.Equals(baseDisplayName, base.DisplayName));
		}
		public new string DisplayName {
			get { return string.Equals(baseDisplayName, base.DisplayName) ? GetDisplayName() : base.DisplayName; }
		}
		string GetDisplayName() {
			return DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(type);
		}
	}
	public static class ToolboxHelper {
		static Type[] xrControlTypes;
		static ToolboxItem[] xrToolboxItems;
		static ToolboxItem[] xrLocalizableToolboxItems;
		public static Type[] XRToolboxTypes {
			get {
				if(xrControlTypes == null) {
					xrControlTypes = new Type[] { 
						typeof(XRLabel), typeof(XRCheckBox), typeof(XRRichText), typeof(XRPictureBox), typeof(XRPanel), typeof(XRTable),
						typeof(XRLine), typeof(XRShape), typeof(XRBarCode), typeof(XRZipCode), typeof(PrintableComponentContainer),
						typeof(XRChart), typeof(XRGauge), typeof(XRSparkline), typeof(XRPivotGrid), typeof(XRSubreport),
						typeof(XRTableOfContents), typeof(XRPageInfo), typeof(XRPageBreak), typeof(XRCrossBandLine), typeof(XRCrossBandBox) };
				}
				return xrControlTypes;
			}
		}
		public static ToolboxItem[] XRToolboxItems { 
			get {
				if(xrToolboxItems == null)
					xrToolboxItems = CreateToolboxItems(type => {
						return new ToolboxItem(type);
					});
				return xrToolboxItems;
			}
		}
		public static ToolboxItem[] XRLocalizableToolboxItems {
			get {
				if(xrLocalizableToolboxItems == null)
					xrLocalizableToolboxItems = CreateToolboxItems(type => {
						return type != typeof(PrintableComponentContainer) ? new LocalizableToolboxItem(type) : null;
					});
				return xrLocalizableToolboxItems;
			}
		}
		static ToolboxItem[] CreateToolboxItems(Function<ToolboxItem, Type> itemCreator) {
			List<ToolboxItem> items = new List<ToolboxItem>(XRToolboxTypes.Length);
			foreach(Type type in XRToolboxTypes) {
				ToolboxItem item = itemCreator(type);
				if(item != null) items.Add(item);
			}
			return items.ToArray();
		}
	} 
#if DEBUGTEST
	public static class Helper {
		public static void AddCommandButton(ToolBar toolBar, CommandID cmdID, int imageIndex, string toolTip) {
			ToolBarButton btn = new ToolBarButton();
			if(cmdID != null) {
				btn.ImageIndex = imageIndex;
				btn.Tag = cmdID;
				btn.Enabled = false;
				btn.ToolTipText = toolTip;
			} else {
				btn.Style = ToolBarButtonStyle.Separator;
			}
			toolBar.Buttons.Add(btn);
		}	
		public static string[] ToolTips = { 
												ReportLocalizer.GetString(ReportStringId.UD_TTip_FileOpen), ReportLocalizer.GetString(ReportStringId.UD_TTip_FileSave), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_EditCut), ReportLocalizer.GetString(ReportStringId.UD_TTip_EditCopy), ReportLocalizer.GetString(ReportStringId.UD_TTip_EditPaste), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_Undo), ReportLocalizer.GetString(ReportStringId.UD_TTip_Redo), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignToGrid), null, 
												ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignLeft), ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignVerticalCenters), ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignRight), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignTop), ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignHorizontalCenters), ReportLocalizer.GetString(ReportStringId.UD_TTip_AlignBottom), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_SizeToControlWidth), ReportLocalizer.GetString(ReportStringId.UD_TTip_SizeToGrid), ReportLocalizer.GetString(ReportStringId.UD_TTip_SizeToControlHeight), ReportLocalizer.GetString(ReportStringId.UD_TTip_SizeToControl), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_HorizSpaceMakeEqual), ReportLocalizer.GetString(ReportStringId.UD_TTip_HorizSpaceIncrease), ReportLocalizer.GetString(ReportStringId.UD_TTip_HorizSpaceDecrease), ReportLocalizer.GetString(ReportStringId.UD_TTip_HorizSpaceConcatenate), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_VertSpaceMakeEqual), ReportLocalizer.GetString(ReportStringId.UD_TTip_VertSpaceIncrease), ReportLocalizer.GetString(ReportStringId.UD_TTip_VertSpaceDecrease), ReportLocalizer.GetString(ReportStringId.UD_TTip_VertSpaceConcatenate), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_CenterHorizontally), ReportLocalizer.GetString(ReportStringId.UD_TTip_CenterVertically), null,
												ReportLocalizer.GetString(ReportStringId.UD_TTip_BringToFront), ReportLocalizer.GetString(ReportStringId.UD_TTip_SendToBack)
										  };
		public static CommandID[] Commands = {		
												UICommands.OpenFile, UICommands.SaveFile, null, StandardCommands.Cut, StandardCommands.Copy, StandardCommands.Paste, null,
												StandardCommands.Undo, StandardCommands.Redo, 
												null, StandardCommands.AlignToGrid, null, 
												StandardCommands.AlignLeft, StandardCommands.AlignVerticalCenters, StandardCommands.AlignRight, null,
												StandardCommands.AlignTop, StandardCommands.AlignHorizontalCenters, StandardCommands.AlignBottom, null,
												StandardCommands.SizeToControlWidth, StandardCommands.SizeToGrid, StandardCommands.SizeToControlHeight, StandardCommands.SizeToControl, null,
												StandardCommands.HorizSpaceMakeEqual, StandardCommands.HorizSpaceIncrease, StandardCommands.HorizSpaceDecrease, StandardCommands.HorizSpaceConcatenate, null,
												StandardCommands.VertSpaceMakeEqual, StandardCommands.VertSpaceIncrease, StandardCommands.VertSpaceDecrease, StandardCommands.VertSpaceConcatenate, null,
												StandardCommands.CenterHorizontally, StandardCommands.CenterVertically, null,
												StandardCommands.BringToFront, StandardCommands.SendToBack
											 };	
		public static CommandID[] FormattingCommands = {		
														   DevExpress.XtraReports.Design.Commands.FormattingCommands.Bold, DevExpress.XtraReports.Design.Commands.FormattingCommands.Italic, DevExpress.XtraReports.Design.Commands.FormattingCommands.Underline, null,
														   DevExpress.XtraReports.Design.Commands.FormattingCommands.ForeColor, DevExpress.XtraReports.Design.Commands.FormattingCommands.BackColor, null,
														   DevExpress.XtraReports.Design.Commands.FormattingCommands.JustifyLeft, DevExpress.XtraReports.Design.Commands.FormattingCommands.JustifyCenter, DevExpress.XtraReports.Design.Commands.FormattingCommands.JustifyRight, DevExpress.XtraReports.Design.Commands.FormattingCommands.JustifyJustify
													   };	
		public static CommandID[] AdditionalCommands = {
													 StandardCommands.Delete, StandardCommands.SelectAll, UICommands.Exit, UICommands.NewReport, UICommands.NewReportWizard, UICommands.SaveFileAs, ReportTabControlCommands.ShowDesignerTab, ReportTabControlCommands.ShowScriptsTab, ReportTabControlCommands.ShowHTMLViewTab, ReportTabControlCommands.ShowPreviewTab
												 };
		public static void SubscribeMenuCommandEvents(IServiceProvider serviceProvider, EventHandler handler, CommandID[] commands) {
			IMenuCommandService commandServ = (IMenuCommandService)serviceProvider.GetService(typeof(IMenuCommandService));
			MenuCommand menuCommand = null;
			foreach(CommandID commandID in commands) {
				menuCommand = commandServ.FindCommand(commandID);
				if(menuCommand != null)
					menuCommand.CommandChanged += new EventHandler(handler);
			}
		}
		public static void UnSubscribeMenuCommandEvents(IServiceProvider serviceProvider, EventHandler handler, CommandID[] commands) {
			IMenuCommandService commandServ = (IMenuCommandService)serviceProvider.GetService(typeof(IMenuCommandService));
			MenuCommand menuCommand = null;
			foreach(CommandID commandID in commands) {
				menuCommand = commandServ.FindCommand(commandID);
				if(menuCommand != null)
					menuCommand.CommandChanged -= new EventHandler(handler);
			}
		}
	}
#endif
}
