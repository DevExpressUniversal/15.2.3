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
namespace DevExpress.XtraReports.UI.BarCode {
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.CodabarStartStopPair class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.CodabarStartStopPair class instead.")]
	public enum CodabarStartStopPair {
		None = 0,
		AT = 1,
		BN = 2,
		CStar = 3,
		DE = 4
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.Code128Charset class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code128Charset class instead.")]
	public enum Code128Charset {
		CharsetA = 103,
		CharsetB = 104,
		CharsetC = 105,
		CharsetAuto = 0
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.MSICheckSum class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.MSICheckSum class instead.")]
	public enum MSICheckSum {
		None = 0,
		Modulo10 = 1,
		DoubleModulo10 = 2,
	}
}
