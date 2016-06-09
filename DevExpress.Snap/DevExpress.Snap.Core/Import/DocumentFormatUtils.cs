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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.Snap.Core.Import {
	#region ImportSnapPieceTableInfo
	public class ImportSnapPieceTableInfo : ImportPieceTableInfo {
		#region Fields
		readonly Dictionary<string, ImportSnapBookmarkInfo> snapBookmarks;
		readonly Dictionary<string, ImportSnapTemplateIntervalInfo> snapTemplateIntervals;
		#endregion
		public ImportSnapPieceTableInfo(SnapPieceTable pieceTable)
			: base(pieceTable) {
				this.snapBookmarks = new Dictionary<string, ImportSnapBookmarkInfo>();
				this.snapTemplateIntervals = new Dictionary<string, ImportSnapTemplateIntervalInfo>();
		}
		#region Properties
		public Dictionary<string, ImportSnapBookmarkInfo> SnapBookmarks { get { return snapBookmarks; } }
		public Dictionary<string, ImportSnapTemplateIntervalInfo> SnapTemplateIntervals { get { return snapTemplateIntervals; } }
		#endregion
	}
	#endregion
	#region ImportSnapBookmarkInfo
	public class ImportSnapBookmarkInfo : ImportBookmarkInfoCore {
		public string Id { get; set; }
		public string TemplateIntervalId { get; set; }
		public string ParentId { get; set; }
		public int FieldContextId { get; set; }
		public string HeaderBookmarkId { get; set; }
		public string FooterBookmarkId { get; set; }
		public ImportSnapTemplateInfo TemplateInfo { get; set; }
		public ImportSnapBookmarkInfo(string id) {
			Guard.ArgumentIsNotNullOrEmpty(id, "id");
			Id = id;
		}
		public override bool Validate(PieceTable pieceTable) {
			if (String.IsNullOrEmpty(TemplateIntervalId) || FieldContextId <= 0)
				return false;
			return base.Validate(pieceTable);
		}
	}
	#endregion
	#region ImportSnapTemplateIntervalInfo
	public class ImportSnapTemplateIntervalInfo : ImportBookmarkInfoCore {
	}
	#endregion
	#region ImportSnapTemplateIntervalInfo
	public class ImportSnapTemplateInfo {
		public int FieldInGroupCount { get; set; }
		public int FirstGroupIndex { get; set; }
		public int LastGroupIndex { get; set; }
		public SnapTemplateIntervalType TemplateType { get; set; }
		public string FirstGroupBookmarkId { get; set; }
		public string LastGroupBookmarkId { get; set; }
		public string FirstListBookmarkId { get; set; }
		public string LastListBookmarkId { get; set; }
	}
	#endregion
}
