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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.XtraReports.UI;
using System.Collections;
namespace DevExpress.XtraPrinting.Native {
	static class BrickLayoutInfoExtensions {
		public static VisualBrick VisualBrick(this BrickLayoutInfo info) {
			return info.Brick as VisualBrick;
		}
		public static XRControl Control(this BrickLayoutInfo info) {
			return info.VisualBrick().BrickOwner as XRControl;
		}
		public static XRControl RealControl(this BrickLayoutInfo info) {
			return info.VisualBrick().BrickOwner.RealControl as XRControl;
		}
		public static bool IsStart(this BrickLayoutInfo info) {
			return object.ReferenceEquals(info.Control().Band, ((XRCrossBandControl)info.RealControl()).StartBand);
		}
		public static bool IsEnd(this BrickLayoutInfo info) {
			return object.ReferenceEquals(info.Control().Band, ((XRCrossBandControl)info.RealControl()).EndBand);
		}
		public static bool IsWrappingDetailArea(this BrickLayoutInfo info) {
			Band startBand = ((XRCrossBandControl)info.RealControl()).StartBand;
			Band endBand = ((XRCrossBandControl)info.RealControl()).EndBand;
			return (startBand is TopMarginBand || startBand is ReportHeaderBand || startBand is PageHeaderBand || startBand is GroupHeaderBand || startBand is DetailBand) &&
				   (endBand is BottomMarginBand || endBand is ReportFooterBand || endBand is PageFooterBand || endBand is GroupFooterBand || endBand is DetailBand);
		}
	}
}
namespace DevExpress.XtraReports.Native {
	class XRMergeBrickHelper : MergeBrickHelper {
		public Dictionary<PSPage, float> PageContentBottomDictionary;
		float pageContentBottom;
		public XRMergeBrickHelper() {
			PageContentBottomDictionary = new Dictionary<PSPage, float>();
		}
		static int GetEndValue(BrickLayoutInfo b) {
			return b.IsEnd() ? 1 : 0;
		}
		protected override void OnStartProcess(PSPage page) {
			if(page == null || !PageContentBottomDictionary.TryGetValue(page, out pageContentBottom))
				pageContentBottom = -1;
		}
		protected override void OnEndProcess(PSPage page) {
			if(page != null)
				PageContentBottomDictionary.Remove(page);
		}
		protected override void MergeBricks(List<BrickLayoutInfo> bricksToMerge, Action<Brick, RectangleF> addBrick, PSPage page) {
			if(!(bricksToMerge.First().RealControl() is XRCrossBandControl)) {
				base.MergeBricks(bricksToMerge, addBrick, page);
				return;
			}
			bricksToMerge.Sort((x, y) => {
				int c = Comparer<double>.Default.Compare(x.Rect.Top, y.Rect.Top);
				return c != 0 ? c : GetEndValue(x).CompareTo(GetEndValue(y));
			});
			List<List<BrickLayoutInfo>> brickListContainer = new List<List<BrickLayoutInfo>>();
			brickListContainer.Add(new List<BrickLayoutInfo>());
			foreach(var item in bricksToMerge) {
				brickListContainer.Last().Add(item);
				if(item.IsEnd())
					brickListContainer.Add(new List<BrickLayoutInfo>());
			}
			VisualBrick prototypeBrick = null;
			foreach(var item in brickListContainer) {
				if(item.Count == 0)
					continue;
				item.ForEach(x => x.Brick.IsVisible = false);
				if(item[0].IsWrappingDetailArea()) {
					if(!item.Last().IsEnd() && pageContentBottom > 0) {
						RectangleDF rect = item.Last().Rect;
						BrickLayoutInfo info = new BrickLayoutInfo(null, new RectangleDF(rect.X, pageContentBottom, rect.Width, 0));
						item.Add(info);
					} else if(item.Count == 1 && !item[0].IsStart() && item[0].IsEnd()) {
						RectangleDF rect = item[0].Rect;
						BrickLayoutInfo info = new BrickLayoutInfo(null, new RectangleDF(rect.X, 0, rect.Width, 0));
						item.Add(info);
					}
				}
				var unionRect = GetUnionRect(item);
				if(prototypeBrick == null)
					prototypeBrick = item.First().VisualBrick();
				addBrick((VisualBrick)prototypeBrick.Clone(), unionRect);
			}
		}
#if DEBUGTEST
		public void Test_MergeBricks(List<BrickLayoutInfo> bricksToMerge, Action<Brick, RectangleF> addBrick, PSPage page) {
			MergeBricks(bricksToMerge, addBrick, page);
		}
#endif
	}
}
