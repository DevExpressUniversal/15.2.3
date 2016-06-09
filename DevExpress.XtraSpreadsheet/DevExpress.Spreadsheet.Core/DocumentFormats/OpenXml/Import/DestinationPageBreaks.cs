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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PageBreaksDestination
	public class PageBreaksDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("brk", OnPageBreak);
			return result;
		}
		static PageBreaksDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PageBreaksDestination)importer.PeekDestination();
		}
		static Destination OnPageBreak(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PageBreakDestination(importer, GetThis(importer).breakCollection);
		}
		readonly PageBreakCollection breakCollection;
		public PageBreaksDestination(SpreadsheetMLBaseImporter importer, PageBreakCollection breakCollection)
			: base(importer) {
			Guard.ArgumentNotNull(breakCollection, "breakCollection");
			this.breakCollection = breakCollection;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PageBreakDestination
	public class PageBreakDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PageBreakCollection breakCollection;
		public PageBreakDestination(SpreadsheetMLBaseImporter importer, PageBreakCollection breakCollection)
			: base(importer) {
			Guard.ArgumentNotNull(breakCollection, "breakCollection");
			this.breakCollection = breakCollection;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool isManual = Importer.GetWpSTOnOffValue(reader, "man", false);
			if (!isManual)
				return;
			int index = Importer.GetWpSTIntegerValue(reader, "id", Int32.MinValue);
			if (index < 1)
				return;
			breakCollection.TryInsert(index);
		}
	}
	#endregion
}
