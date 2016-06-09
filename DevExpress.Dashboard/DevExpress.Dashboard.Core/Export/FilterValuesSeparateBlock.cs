#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardExport {
	public class FilterValuesSeparateBlock {
		readonly IList<DimensionFilterValues> filterValues;
		public FilterValuesSeparateBlock(IList<DimensionFilterValues> filterValues) {
			this.filterValues = filterValues;
		}
		static string GenerateFilterValuesText(IList<FormattableValue> masterFilterValues) {
			const string parametersSeparator = ", ";
			StringBuilder sb = new StringBuilder();
			string separator = string.Empty;
			foreach(FormattableValue masterFilterValue in masterFilterValues) {
				FormatterBase formatterBase = FormatterBase.CreateFormatter(masterFilterValue.Format);
				if(masterFilterValue.Value != null) {
					sb.AppendFormat("{0}{1}", separator, formatterBase.Format(masterFilterValue.Value));
					separator = parametersSeparator;
				}
				if(masterFilterValue.RangeLeft != null && masterFilterValue.RangeRight != null) {
					sb.AppendFormat("{0}{1} - {2}", separator, formatterBase.Format(masterFilterValue.RangeLeft), formatterBase.Format(masterFilterValue.RangeRight));
					separator = parametersSeparator;
				}
			}
			return sb.ToString();
		}
		public XRControl CreateControl(DashboardFontInfo fontInfo) {
			using (DocumentModel model = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				model.BeginSetContent();
				InputPosition pos = new InputPosition(model.MainPieceTable);
				pos.CharacterFormatting.FontName = FontHelper.GetFont(ExportHelper.DefaultCaptionFont, fontInfo).Name;
				pos.CharacterFormatting.DoubleFontSize = 20;
				for(int i = 0; i < filterValues.Count; i++) {
					DimensionFilterValues filter = filterValues[i];
					model.MainPieceTable.InsertParagraphCore(pos);
					if(i > 0)
						model.MainPieceTable.Paragraphs[pos.ParagraphIndex - 1].SpacingAfter = model.UnitConverter.PointsToModelUnits(8);
					pos.CharacterFormatting.FontBold = true;
					model.MainPieceTable.InsertTextCore(pos, String.Format("{0}:", filter.Name));
					model.MainPieceTable.InsertParagraphCore(pos);
					pos.CharacterFormatting.FontBold = false;
					model.MainPieceTable.InsertTextCore(pos, GenerateFilterValuesText(filter.Values));
				}
				model.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, new FieldUpdateOnLoadOptions(false, false));
				XRRichText text = new XRRichText();
				using(MemoryStream stream = new MemoryStream()) {
					model.SaveDocument(stream, DocumentFormat.Rtf, string.Empty);
					stream.Position = 0;
					text.LoadFile(stream, XRRichTextStreamType.RtfText);
				}
				return text;
			}
		}
	}
}
