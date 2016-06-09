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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.Drawing;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.Registrator {
	class TileViewPaintStyle : ViewPaintStyle {
		public override bool CanUsePaintStyle {
			get {
				return true;
			}
		}
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new TileViewInfo((TileView)view);
		}
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new TileViewPainter(view);
		}
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) {
			return null;
		}
		public override bool IsSkin {
			get {
				return false;
			}
		}
		public override string Name {
			get {
				return "";
			}
		}
		public override Utils.AppearanceDefaultInfo[] GetAppearanceDefaultInfo(Views.Base.BaseView view) {
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ItemNormal", new AppearanceDefault()), 
				new AppearanceDefaultInfo("ItemHovered", new AppearanceDefault()), 
				new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault()),
				new AppearanceDefaultInfo("ItemDescriptionNormal", new AppearanceDefault()), 
				new AppearanceDefaultInfo("ItemDescriptionHovered", new AppearanceDefault()), 
				new AppearanceDefaultInfo("ItemDescriptionPressed", new AppearanceDefault()),
				new AppearanceDefaultInfo("GroupNormal", new AppearanceDefault()), 
				new AppearanceDefaultInfo("GroupHovered", new AppearanceDefault()), 
				new AppearanceDefaultInfo("GroupPressed", new AppearanceDefault())};
		}
	}
}
