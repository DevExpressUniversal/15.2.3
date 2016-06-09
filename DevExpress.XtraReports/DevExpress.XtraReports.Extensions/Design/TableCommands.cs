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
using System.Text;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design.Commands {
	public static class TableCommands {
		private const int cmdidInsRowAbove = 1;
		private const int cmdidInsRowBelow = 2;
		private const int cmdidInsColumnToLeft = 3;
		private const int cmdidInsColumnToRight = 4;
		private const int cmdidInsCell = 5;
		private const int cmdidDelRow = 6;
		private const int cmdidDelColumn = 7;
		private const int cmdidDelCell = 8;
		private const int cmdidConvert = 9;
		private static readonly Guid tableCommandSet = new Guid("{6EA16E02-A13D-4154-9EE4-1192669E4A9B}");
		public static readonly CommandID InsertRowAbove = new CommandID(tableCommandSet, cmdidInsRowAbove);
		public static readonly CommandID InsertRowBelow = new CommandID(tableCommandSet, cmdidInsRowBelow);
		public static readonly CommandID InsertColumnToLeft = new CommandID(tableCommandSet, cmdidInsColumnToLeft);
		public static readonly CommandID InsertColumnToRight = new CommandID(tableCommandSet, cmdidInsColumnToRight);
		public static readonly CommandID InsertCell = new CommandID(tableCommandSet, cmdidInsCell);
		public static readonly CommandID DeleteRow = new CommandID(tableCommandSet, cmdidDelRow);
		public static readonly CommandID DeleteColumn = new CommandID(tableCommandSet, cmdidDelColumn);
		public static readonly CommandID DeleteCell = new CommandID(tableCommandSet, cmdidDelCell);
		public static readonly CommandID ConvertToLabels = new CommandID(tableCommandSet, cmdidConvert);
	}
}
