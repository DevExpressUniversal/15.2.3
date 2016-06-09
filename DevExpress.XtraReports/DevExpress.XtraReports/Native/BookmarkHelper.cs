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

using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native {
	class BookmarkHelper {
		readonly List<VisualBrick> childBricks = new List<VisualBrick>();
		VisualBrick brick;
		Band previousBand;
		Dictionary<XRControl, bool> childBeforeParent = new Dictionary<XRControl, bool>();
		public void SetCurrentBrick(VisualBrick brick, Band band) {
			Guard.ArgumentNotNull(brick, "brick");
			previousBand = band;
			if(childBricks.Count != 0) {
				childBricks.ForEach(x => x.BookmarkInfo.ParentBrick = brick);
				childBricks.Clear();
			}
			this.brick = brick;
		}
		public void SetChildBookmarkBrick(VisualBrick childBrick, Band band) {
			Guard.ArgumentNotNull(childBrick, "childBrick");
			XRControl ownerControl = childBrick.BrickOwner as XRControl;
			if(ownerControl != null && !childBeforeParent.ContainsKey(ownerControl))
				childBeforeParent.Add(ownerControl, brick == null);
			bool isChildBefore;
			if((previousBand != null && NestedWeightingFactorComparer.Default.Compare(previousBand, band) > 0) ||
				(childBeforeParent.TryGetValue(ownerControl, out isChildBefore) && isChildBefore)) {
				brick = null;
			}
			previousBand = band;
			if(brick != null) {
				childBrick.BookmarkInfo.ParentBrick = brick;
			} else {
				childBricks.Add(childBrick);
			}
		}				
	}
}
