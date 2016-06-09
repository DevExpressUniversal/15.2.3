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
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Printing.Native.PrintEditor {
	public static class PrintQueueStatusConverter {
		static Dictionary<PrinterStatus, PreviewStringId> stringIds = new Dictionary<PrinterStatus, PreviewStringId>();
		static PrintQueueStatusConverter() {
			stringIds.Add(PrinterStatus.None, PreviewStringId.PrinterStatus_Ready);
			stringIds.Add(PrinterStatus.Paused, PreviewStringId.PrinterStatus_Paused);
			stringIds.Add(PrinterStatus.Error, PreviewStringId.PrinterStatus_Error);
			stringIds.Add(PrinterStatus.PendingDeletion, PreviewStringId.PrinterStatus_PendingDeletion);
			stringIds.Add(PrinterStatus.PaperJam, PreviewStringId.PrinterStatus_PaperJam);
			stringIds.Add(PrinterStatus.PaperOut, PreviewStringId.PrinterStatus_PaperOut);
			stringIds.Add(PrinterStatus.ManualFeed, PreviewStringId.PrinterStatus_ManualFeed);
			stringIds.Add(PrinterStatus.PaperProblem, PreviewStringId.PrinterStatus_PaperProblem);
			stringIds.Add(PrinterStatus.Offline, PreviewStringId.PrinterStatus_Offline);
			stringIds.Add(PrinterStatus.IOActive, PreviewStringId.PrinterStatus_IOActive);
			stringIds.Add(PrinterStatus.Busy, PreviewStringId.PrinterStatus_Busy);
			stringIds.Add(PrinterStatus.Printing, PreviewStringId.PrinterStatus_Printing);
			stringIds.Add(PrinterStatus.OutputBinFull, PreviewStringId.PrinterStatus_OutputBinFull);
			stringIds.Add(PrinterStatus.NotAvailable, PreviewStringId.PrinterStatus_NotAvailable);
			stringIds.Add(PrinterStatus.Waiting, PreviewStringId.PrinterStatus_Waiting);
			stringIds.Add(PrinterStatus.Processing, PreviewStringId.PrinterStatus_Processing);
			stringIds.Add(PrinterStatus.Initializing, PreviewStringId.PrinterStatus_Initializing);
			stringIds.Add(PrinterStatus.WarmingUp, PreviewStringId.PrinterStatus_WarmingUp);
			stringIds.Add(PrinterStatus.TonerLow, PreviewStringId.PrinterStatus_TonerLow);
			stringIds.Add(PrinterStatus.NoToner, PreviewStringId.PrinterStatus_NoToner);
			stringIds.Add(PrinterStatus.PagePunt, PreviewStringId.PrinterStatus_PagePunt);
			stringIds.Add(PrinterStatus.UserIntervention, PreviewStringId.PrinterStatus_UserIntervention);
			stringIds.Add(PrinterStatus.OutOfMemory, PreviewStringId.PrinterStatus_OutOfMemory);
			stringIds.Add(PrinterStatus.DoorOpen, PreviewStringId.PrinterStatus_DoorOpen);
			stringIds.Add(PrinterStatus.ServerUnknown, PreviewStringId.PrinterStatus_ServerUnknown);
			stringIds.Add(PrinterStatus.PowerSave, PreviewStringId.PrinterStatus_PowerSave);
			stringIds.Add(PrinterStatus.ServerOffline, PreviewStringId.PrinterStatus_ServerOffline);
			stringIds.Add(PrinterStatus.DriverUpdateNeeded, PreviewStringId.PrinterStatus_DriverUpdateNeeded);
		}
		public static String GetString(this PrinterStatus printQueueStatus) {
			if(printQueueStatus == PrinterStatus.None)
				return PreviewLocalizer.Active.GetLocalizedString(stringIds[PrinterStatus.None]);
			List<string> result = new List<string>();
			foreach(PrinterStatus value in Enum.GetValues(typeof(PrinterStatus)))
				if(value != PrinterStatus.None && printQueueStatus.HasFlag(value))
					result.Add(PreviewLocalizer.Active.GetLocalizedString(stringIds[value]));
			return result.Count == 1 ? result[0] : string.Join(", ", result);
		}
	}
}
