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

namespace DevExpress.XtraSpreadsheet {
	public enum MailMergeMode {
		OneSheetMode,
		OneDocumentMode,
		DocumentsMode
	}
	public static class MailMergeConstants {
		#region MailMergeMode
		public const string DocumentsMode = "\"Documents\"";
		public const string OneDocumentMode = "\"Worksheets\"";
		public const string OneSheetMode = "\"OneWorksheet\"";
		#endregion
	}
	public static class MailMergeDefinedNames {
		public const string MailMergeMode = "MAILMERGEMODE";
		public const string DetailLevel = "DETAILLEVEL";
		public const string DetailRange = "DETAILRANGE";
		public const string HeaderRange = "HEADERRANGE";
		public const string FooterRange = "FOOTERRANGE";
		public const string FilterField = "FILTERFIELD";
		public const string GroupHeader = "GROUPHEADER";
		public const string GroupFooter = "GROUPFOOTER";
		public const string GroupName = "SORTFIELD";
		public const string DetailDataMember = "DETAILDATAMEMBER";
		public const string HorizontalMode = "HORIZONTALMODE";
	}
}
