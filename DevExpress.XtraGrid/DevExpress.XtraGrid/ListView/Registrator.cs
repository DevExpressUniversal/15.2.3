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

using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.WinExplorer.Handler;
namespace DevExpress.XtraGrid.Registrator {
	public class WinExplorerViewInfoRegistrator : BaseInfoRegistrator {
		public override BaseViewHandler CreateHandler(BaseView view) {
			return new WinExplorerViewHandler(view);
		}
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new FlatWinExplorerViewPaintStyle());
			PaintStyles.Add(new SkinWinExplorerViewPaintStyle());
			PaintStyles.Add(new WindowsClassicPaintStyle());
			PaintStyles.Add(new Windows8PaintStyle());
		}
		public override bool IsInternalView { get { return false; } }
		public override string ViewName { get { return "WinExplorerView"; } }
		public override string StyleOwnerName { get { return "Grid"; } }
		public override BaseView CreateView(GridControl grid) {
			BaseView view = new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView();
			view.SetGridControl(grid);
			return view;
		}
		protected override BaseGridDesigner CreateDesigner() {
			return new WinExplorerViewDesigner();
		}
		public override ViewPaintStyle PaintStyleByLookAndFeel(LookAndFeel.UserLookAndFeel lookAndFeel, string name) {
			if(name == ViewPaintStyle.DefaultPaintStyleName) {
				switch(lookAndFeel.ActiveStyle) {
					case ActiveLookAndFeelStyle.WindowsXP : name = "WindowsXP"; break;
					case ActiveLookAndFeelStyle.Skin: name = "Skin"; break;
					case ActiveLookAndFeelStyle.Flat: name = "Flat"; break;
					case ActiveLookAndFeelStyle.Style3D : name = "Flat"; break;
					case ActiveLookAndFeelStyle.Office2003 : name = "Classic"; break;
					case ActiveLookAndFeelStyle.UltraFlat : name = "Flat"; break;
				}
			}
			ViewPaintStyle res = PaintStyles[name];
			if(res != null && !res.CanUsePaintStyle) res = null;
			if(res == null) return PaintStyles[0];
			return res;
		}
	}
}
