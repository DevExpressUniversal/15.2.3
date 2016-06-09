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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region HeaderDestination
	public class HeaderDestination : HeaderFooterDestination {
		public HeaderDestination(OpenDocumentTextImporter importer, MasterPageStyleInfo info, PieceTable header)
			: base(importer, info, header) {
		}
	}
	#endregion
	#region FooterDestination
	public class FooterDestination : HeaderFooterDestination {
		public FooterDestination(OpenDocumentTextImporter importer, MasterPageStyleInfo info, PieceTable footer)
			: base(importer, info, footer) {
		}
	}
	#endregion
	#region HeaderFooterDestination (abstract class)
	public abstract class HeaderFooterDestination : TextDestination {
		readonly MasterPageStyleInfo masterPageInfo;
		protected HeaderFooterDestination(OpenDocumentTextImporter importer, MasterPageStyleInfo info, PieceTable headerFooter)
			: base(importer) {
			Guard.ArgumentNotNull(headerFooter, "headerFooter");
			Guard.ArgumentNotNull(info, "info");
			this.masterPageInfo = info;
			importer.PushCurrentPieceTable(headerFooter);
		}
		public MasterPageStyleInfo MasterPageInfo { get { return masterPageInfo; } }
		public override void ProcessElementClose(XmlReader reader) {
			Debug.Assert(DocumentModel.DocumentCapabilities.HeadersFootersAllowed);
			Importer.PieceTable.CheckIntegrity();
			Importer.PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			Importer.PopCurrentPieceTable();
		}
	}
	#endregion 
}
