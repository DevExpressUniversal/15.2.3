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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.PlainText;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Internal {
	public class RichEditToolTipHelper {
		internal string GetCommentToolTipText(Comment comment) {
			IPlainTextExporter exporter = comment.Content.DocumentModel.DocumentFormatsDependencies.ExportersFactory.CreatePlainTextExporter(comment.Content.DocumentModel, new PlainTextDocumentExporterOptions());
			return exporter.ExportPieceTableContent(comment.Content.PieceTable);
		}
		internal string GetCommentToolTipHeader(Comment comment) {
			string endHeader = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.CommentToolTipHeader), comment.Author);
			if (comment.Date > Comment.MinCommentDate)
				endHeader = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.CommentToolTipHeaderWithDate), endHeader, comment.Date);
			string commentNumber = Convert.ToString(comment.Index + 1);
			return String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Comment)+" {2}:", comment.Name, commentNumber, endHeader);
		}
		internal string GetCommentToolTipHeader(CommentViewInfo commentViewInfo) {
			Comment comment = commentViewInfo.Comment;
			string properties = String.Format("{0}", comment.Author);
			if (comment.Date > Comment.MinCommentDate)
				properties = String.Format("{0} {1}", properties, comment.Date);
			if (String.IsNullOrEmpty(properties))
				return String.Empty;
			return String.Format("{0} ", properties);
		}
	}
}
