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

using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ITileInfo : IBaseElementInfo, IUIElement {
		BaseTile BaseTile { get; }
		TileItemViewInfo ItemInfo { get; }
		ITileContainerInfo ContainerInfo { get; set; }
	}
	class TileInfo : BaseElementInfo, ITileInfo, IBaseDocumentInfo {
		public TileInfo(WindowsUIView owner, BaseTile tile, TileItemViewInfo itemInfo)
			: base(owner) {
			ItemInfo = itemInfo;
			BaseTile = tile;
		}
		public override System.Type GetUIElementKey() {
			return typeof(ITileInfo);
		}
		public BaseTile BaseTile { get; private set; }
		public TileItemViewInfo ItemInfo { get; private set; }
		public ITileContainerInfo ContainerInfo { get; set; }
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
		public BaseDocument BaseDocument {
			get { return BaseTile is Tile ? ((Tile)BaseTile).Document : null; }
		}
	}
}
