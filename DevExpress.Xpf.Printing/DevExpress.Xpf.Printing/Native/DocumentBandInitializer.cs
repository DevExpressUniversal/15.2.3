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
using System.Windows;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	public class DocumentBandInitializer {
		readonly RowViewBuilder rowViewBuilder;
		readonly BrickCollector brickCollector;
		readonly Size usablePageSize;
		public DocumentBandInitializer(BrickCollector brickCollector, Size usablePageSize) {
			Guard.ArgumentNotNull(brickCollector, "brickCollector");
			this.brickCollector = brickCollector;
			this.usablePageSize = usablePageSize;
			this.rowViewBuilder = new RowViewBuilder();
		}
		public void Initialize(DocumentBand band, Func<bool, RowViewInfo> getRowViewInfo) {
			Initialize(band, getRowViewInfo, null);
		}
		public void Initialize(DocumentBand band, Func<bool, RowViewInfo> getRowViewInfo, int? nodeIndex) {
			Guard.ArgumentNotNull(band, "band");
			if(getRowViewInfo == null)
				return;
			RowViewInfo rowViewInfo = getRowViewInfo(true);
			if(rowViewInfo == null)
				return;
			RowContent rowContent = new RowContent() {
				Content = rowViewInfo.Content,
				UsablePageWidth = usablePageSize.Width,
				UsablePageHeight = usablePageSize.Height,
			};
			if(nodeIndex != null) {
				rowContent.IsEven = (nodeIndex % 2 == 0);
			}
			FrameworkElement container = rowViewBuilder.Create(rowViewInfo.Template, rowContent);
			float containerBrickHeight;
			BrickCollectionBase containerBricks = brickCollector.ToBricks(container, out containerBrickHeight);
			foreach(Brick brick in containerBricks) {
				brick.SetAttachedValue<int>(BrickAttachedProperties.ParentID, band.ID);
				band.Bricks.Add(brick);
			}
			if(band.SelfHeight < containerBrickHeight)
				band.BottomSpan = containerBrickHeight - band.SelfHeight;
		}
		public void Clear() {
			rowViewBuilder.Clear();
		}
	}
}
