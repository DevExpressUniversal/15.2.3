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
using DevExpress.Data.Mask;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	public class PSCommandHelper {
		public static PrintingSystemCommand[] ExportCommands  {
			get {
				return new PrintingSystemCommand[] {
					PrintingSystemCommand.ExportPdf,
					PrintingSystemCommand.ExportHtm,
					PrintingSystemCommand.ExportMht,
					PrintingSystemCommand.ExportRtf,
					PrintingSystemCommand.ExportXls,
					PrintingSystemCommand.ExportXlsx,
					PrintingSystemCommand.ExportCsv,			
					PrintingSystemCommand.ExportTxt,
					PrintingSystemCommand.ExportGraphic,
					PrintingSystemCommand.ExportXps
				};
			}
		}
		public static PrintingSystemCommand[] SendCommands {
			get {
				return new PrintingSystemCommand[]{
					PrintingSystemCommand.SendPdf,
					PrintingSystemCommand.SendMht,
					PrintingSystemCommand.SendRtf,
					PrintingSystemCommand.SendXls,
					PrintingSystemCommand.SendXlsx,
					PrintingSystemCommand.SendCsv,
					PrintingSystemCommand.SendTxt,
					PrintingSystemCommand.SendGraphic,
					PrintingSystemCommand.SendXps
				};
			}
		}
		public static PrintingSystemCommand[] PageExportCommands {
			get {
				return new PrintingSystemCommand[] {
														 PrintingSystemCommand.ExportGraphic,
														 PrintingSystemCommand.ExportPdf,
														 PrintingSystemCommand.ExportMht,
														 PrintingSystemCommand.ExportHtm,
														 PrintingSystemCommand.ExportXps
				};
			}
		}
		public static PrintingSystemCommand[] ContinuousExportCommands {
			get {
				return new PrintingSystemCommand[] {
														 PrintingSystemCommand.ExportTxt,
														 PrintingSystemCommand.ExportCsv,
														 PrintingSystemCommand.ExportXls,
														 PrintingSystemCommand.ExportXlsx,
														 PrintingSystemCommand.ExportRtf};
			}
		}
		public static PrintingSystemCommand[] AllowOnlyContinuousExportCommands {
			get {
				return new PrintingSystemCommand[] { 
					PrintingSystemCommand.ExportTxt,
					PrintingSystemCommand.ExportCsv					
				};
			}
		}
		public static PrintingSystemCommand[] AllowOnlyContinuousSendCommands {
			get {
				return new PrintingSystemCommand[] {
					PrintingSystemCommand.SendTxt,
					PrintingSystemCommand.SendCsv
				};
			}
		}
		public static PrintingSystemCommand[] PageSendCommands {
			get {
				return new PrintingSystemCommand[] {
														 PrintingSystemCommand.SendGraphic,
														 PrintingSystemCommand.SendPdf,
														 PrintingSystemCommand.SendMht,
														 PrintingSystemCommand.SendXps
				};
			}
		}
		public static PrintingSystemCommand[] ContinuousSendCommands {
			get {
				return new PrintingSystemCommand[] {
														 PrintingSystemCommand.SendRtf,
														 PrintingSystemCommand.SendTxt,
														 PrintingSystemCommand.SendCsv,
														 PrintingSystemCommand.SendXls,
														 PrintingSystemCommand.SendXlsx};
			}
		}
		public static PrintingSystemCommand[] GetCommands() {
			return (PrintingSystemCommand[])EnumExtensions.GetValues(typeof(PrintingSystemCommand));
		}
		public static bool IsExportCommand(PrintingSystemCommand command) {
			return CommandInArray(ExportCommands, command) || command == PrintingSystemCommand.None;
		}
		public static bool IsSendCommand(PrintingSystemCommand command) {
			return CommandInArray(SendCommands, command) || command == PrintingSystemCommand.None;
		}
		static bool CommandInArray(PrintingSystemCommand[] commands, PrintingSystemCommand command) {			
			foreach(PrintingSystemCommand psCommand in commands) {
				if(command == psCommand)
					return true;
			}
			return false;
		}
	}
}
