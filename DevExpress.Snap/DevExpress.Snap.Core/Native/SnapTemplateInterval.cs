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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Export;
namespace DevExpress.Snap.Core.Native {
	public enum SnapTemplateIntervalType {
		DataRow = 1,
		GroupHeader = 2,
		GroupFooter = 3,
		ListHeader = 4,
		ListFooter = 5,
		Separator = 6,
		GroupSeparator = 7
	}
	public class SnapTemplateInfo {
		public SnapTemplateIntervalType TemplateType { get; set; }
		public SnapBookmark FirstListBookmark { get; set; }
		public SnapBookmark LastListBookmark { get; set; }
		public SnapBookmark FirstGroupBookmark { get; set; }
		public SnapBookmark LastGroupBookmark { get; set; }
		public int FirstGroupIndex { get; set; }
		public int LastGroupIndex { get { return FirstGroupIndex + FieldInGroupCount - 1; } }
		public int FieldInGroupCount { get; set; }
		public SnapTemplateInfo(SnapTemplateIntervalType templateType) {
			TemplateType = templateType;
		}
	}
	public class SnapTemplateInterval : VisitableDocumentInterval {
		readonly SnapTemplateInfo templateInfo;
		public SnapTemplateInterval(SnapPieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end, SnapTemplateInfo templateInfo)
			: base(pieceTable) {
			SetStartCore(start);
			SetEndCore(end);
			this.templateInfo = templateInfo;
		}
		public SnapTemplateInfo TemplateInfo { get { return templateInfo; } }
		public new SnapPieceTable PieceTable { get { return (SnapPieceTable)base.PieceTable; } }
		protected internal override void OnChangedCore() {
		}
		protected override RunInfo CreateRunInfo(PieceTable pieceTable) {
			return new ReferencedIntervalRunInfo(pieceTable);
		}
		public override void Visit(IDocumentIntervalVisitor visitor) {
			((ISnapBookmarkVisitor)visitor).Visit(this);
		}
		public override bool Equals(object obj) {
			SnapTemplateInterval interval = obj as SnapTemplateInterval;
			if (interval == null)
				return false;
			return Interval.Equals(interval.Interval) && Object.ReferenceEquals(PieceTable, interval.PieceTable);
		}
		public override int GetHashCode() {
			return Interval.GetHashCode();
		}
		public void RegisterInterval(DocumentModelPositionManager documentModelPositionManager) {
			((ReferencedIntervalRunInfo)Interval).RegisterInterval(PieceTable.DocumentPositionManager);
		}
		public void AttachInterval(DocumentModelPositionManager documentModelPositionManager) {
			((ReferencedIntervalRunInfo)Interval).AttachInterval(PieceTable.DocumentPositionManager);
		}
		public void DetachInterval(DocumentModelPositionManager documentModelPositionManager) {
			((ReferencedIntervalRunInfo)Interval).DetachInterval(PieceTable.DocumentPositionManager);
		}
	}
}
