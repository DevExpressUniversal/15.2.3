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

using System.ComponentModel.Design;
using System;
namespace DevExpress.XtraReports.Design.Commands {
	public static class FieldListCommands {
		const int cmdidAddCalculatedField = 1;
		const int cmdidDeleteCalculatedField = 2;
		const int cmdidEditCalculatedFields = 3;
		const int cmdidEditExpressionCalculatedField = 4;
		const int cmdidAddParameter = 5;
		const int cmdidEditParameters = 6;
		const int cmdidDeleteParameter = 7;
		const int cmdidClearCalculatedFields = 8;
		const int cmdidClearParameters = 9;
		static readonly Guid fieldListCommandSet = new Guid("{7D5470D9-8FDB-4c6e-AF96-868E698A459C}");
		public static readonly CommandID AddCalculatedField = new CommandID(fieldListCommandSet, cmdidAddCalculatedField);
		public static readonly CommandID DeleteCalculatedField = new CommandID(fieldListCommandSet, cmdidDeleteCalculatedField);
		public static readonly CommandID EditCalculatedFields = new CommandID(fieldListCommandSet, cmdidEditCalculatedFields);
		public static readonly CommandID EditExpressionCalculatedField = new CommandID(fieldListCommandSet, cmdidEditExpressionCalculatedField);
		public static readonly CommandID AddParameter = new CommandID(fieldListCommandSet, cmdidAddParameter);
		public static readonly CommandID EditParameters = new CommandID(fieldListCommandSet, cmdidEditParameters);
		public static readonly CommandID DeleteParameter = new CommandID(fieldListCommandSet, cmdidDeleteParameter);
		public static readonly CommandID ClearCalculatedFields = new CommandID(fieldListCommandSet, cmdidClearCalculatedFields);
		public static readonly CommandID ClearParameters = new CommandID(fieldListCommandSet, cmdidClearParameters);
	}
}
