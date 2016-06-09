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

using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentView;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(GroupingPageInfoTextBrickExporter))]
	public class GroupingPageInfoTextBrick : PageInfoTextBrickBase {
		object groupingObject;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public object GroupingObject {
			get { return groupingObject; }
			set { groupingObject = value; }
		}
		public override string BrickType { 
			get { return BrickTypes.GroupingPageInfoText; } 
		}
		public GroupingPageInfoTextBrick()
			: base() {
		}
		public GroupingPageInfoTextBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
		}
		public GroupingPageInfoTextBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: base(sides, borderWidth, borderColor, backColor, foreColor) {
		}
		internal override string GetTextInfo(PrintingSystemBase ps, IPageItem drawingPage) {
			if(ps.Document.IsModified)
				return Text;
			GroupingManager manager = ps.GetService<GroupingManager>();
			int pageIndex = drawingPage != null ? drawingPage.Index : -1;
			GroupingInfo info = manager != null && pageIndex >= 0 && GroupingObject != null ?
			   manager.GetGroupingInfo(GroupingObject, pageIndex) :
			   null;
			int pageCount = info != null ? info.EndPageIndex - info.BeginPageIndex : 0;
			int pageNumber = info != null ? pageIndex - info.BeginPageIndex : 0;
			string text = GetTextInfo(pageCount + StartPageNumber, pageNumber + StartPageNumber);
			return string.IsNullOrEmpty(text) ? Text : text;
		}
	}
}
