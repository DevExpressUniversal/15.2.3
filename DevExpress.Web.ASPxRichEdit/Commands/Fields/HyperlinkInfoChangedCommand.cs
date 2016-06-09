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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class HyperlinkInfoChangedCommand : WebRichEditUpdateModelCommandBase {
		public HyperlinkInfoChangedCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.HyperlinkInfoChanged; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			int fieldEndPosition = (int)Parameters["end"];
			FieldCollection fields = PieceTable.Fields;
			RunIndex resultEndRunIndex = PositionConverter.ToDocumentModelPosition(PieceTable, new DocumentLogPosition(fieldEndPosition - 1)).RunIndex;
			int fieldIndex = Algorithms.BinarySearch(fields, new FieldLastRunIndexComparable(resultEndRunIndex));
			if (fieldIndex < 0)
				throw new Exception("Field not found");
			Field field = fields[fieldIndex];
			if (Parameters.Contains("noInfo")) {
				PieceTable.HyperlinkInfos.Remove(field.Index);
				return;
			}
			if(!PieceTable.HyperlinkInfos.IsHyperlink(field.Index)) {
				HyperlinkInfo newHypInfo = new HyperlinkInfo();
				newHypInfo.Target = String.Empty;
				PieceTable.HyperlinkInfos[field.Index] = newHypInfo;
			}
			string uri = RestoreTextFromRequest((string)Parameters["uri"]);
			string anchor = RestoreTextFromRequest((string)Parameters["anchor"]);
			string tip = RestoreTextFromRequest((string)Parameters["tip"]);
			bool visited = (bool)Parameters["visited"];
			HyperlinkInfo info = PieceTable.HyperlinkInfos[field.Index];
			info.Anchor = anchor;
			info.ToolTip = tip;
			info.Visited = visited;
			info.NavigateUri = uri;
		}
	}
}
