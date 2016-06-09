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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
	public static partial class BarInfos {
		#region Pages
		public static BarInfo Pages { get { return pages; } }
		static readonly BarInfo pages = new BarInfo(
			String.Empty,
			"Insert",
			"Pages",
			new BarInfoItems(
				new string[] { "InsertPageBreak" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupPages"
		);
		#endregion
		#region Tables
		public static BarInfo Tables { get { return tables; } }
		static readonly BarInfo tables = new BarInfo(
			String.Empty,
			"Insert",
			"Tables",
			new BarInfoItems(
				new string[] { "InsertTable" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupTables"
		);
		#endregion
		#region Illustrations
		public static BarInfo Illustrations { get { return illustrations; } }
		static readonly BarInfo illustrations = new BarInfo(
			String.Empty,
			"Insert",
			"Illustrations",
			new BarInfoItems(
				new string[] { "InsertPicture", "InsertFloatingPicture" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupIllustrations"
		);
		#endregion
		#region Links
		public static BarInfo Links { get { return links; } }
		static readonly BarInfo links = new BarInfo(
			String.Empty,
			"Insert",
			"Links",
			new BarInfoItems(
				new string[] { "InsertBookmark", "InsertHyperlink" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupLinks"
		);
		#endregion
		#region HeaderFooter
		public static BarInfo HeaderFooter { get { return headerFooter; } }
		static readonly BarInfo headerFooter = new BarInfo(
			String.Empty,
			"Insert",
			"HeaderFooter",
			new BarInfoItems(
				new string[] { "InsertHeader", "InsertFooter", "InsertPageNumber", "InsertPageCount" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupHeaderFooter"
		);
		#endregion
		#region Text
		public static BarInfo Text { get { return text; } }
		static readonly BarInfo text = new BarInfo(
			String.Empty,
			"Insert",
			"Text",
			new BarInfoItems(
				new string[] { "InsertTextBox" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupText"
		);
		#endregion
		#region Symbols
		public static BarInfo Symbols { get { return symbols; } }
		static readonly BarInfo symbols = new BarInfo(
			String.Empty,
			"Insert",
			"Symbols",
			new BarInfoItems(
				new string[] { "InsertSymbol" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupSymbols"
		);
		#endregion
	}
}
