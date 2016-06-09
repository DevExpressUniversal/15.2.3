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
namespace DevExpress.Printing.Native.PrintEditor {
	[Flags]
	public enum PrinterStatus {
		None = 0x00000000,
		Paused = 0x00000001,
		Error = 0x00000002,
		PendingDeletion = 0x00000004,
		PaperJam = 0x00000008,
		PaperOut = 0x00000010,
		ManualFeed = 0x00000020,
		PaperProblem = 0x00000040,
		Offline = 0x00000080,
		IOActive = 0x00000100,
		Busy = 0x00000200,
		Printing = 0x00000400,
		OutputBinFull = 0x00000800,
		NotAvailable = 0x00001000,
		Waiting = 0x00002000,
		Processing = 0x00004000,
		Initializing = 0x00008000,
		WarmingUp = 0x00010000,
		TonerLow = 0x00020000,
		NoToner = 0x00040000,
		PagePunt = 0x00080000,
		UserIntervention = 0x00100000,
		OutOfMemory = 0x00200000,
		DoorOpen = 0x00400000,
		ServerUnknown = 0x00800000,
		PowerSave = 0x01000000,
		ServerOffline = 0x02000000,
		DriverUpdateNeeded = 0x04000000,
	}
}
