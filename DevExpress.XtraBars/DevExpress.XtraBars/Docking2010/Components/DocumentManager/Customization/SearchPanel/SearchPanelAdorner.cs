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

using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Views;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Customization {
	interface ISearchPanelAdorner : IBaseAdorner {
		System.Windows.Forms.Keys Shortcut { get; }
		void Update();
		bool Enabled { get; }
		void ProcessMouseWheel(System.Windows.Forms.MouseEventArgs e);
		bool RaiseShowing(System.Windows.Forms.Control target);
	}
	class SearchPanelAdornerElementInfoArgs : AdornerElementInfoArgs, ISearchPanelAdorner {
		IBaseFlyoutPanelInfo flyoutPanelInfoCore;
		WindowsUIView ownerCore;
		public SearchPanelAdornerElementInfoArgs(WindowsUIView owner)
			: base() {
			ownerCore = owner;
			flyoutPanelInfoCore = CreateFlyoutInfo();
		}
		System.Windows.Forms.Keys ISearchPanelAdorner.Shortcut { 
			get { return GetShortcut(); } 
		}
		public AdornerHitTest HitTest(Point point) {
			if(!Owner.Manager.IsOwnerControlHandleCreated) return AdornerHitTest.None;
			Point realLocation = Owner.Manager.ScreenToClient(point);
			if(flyoutPanelInfoCore.HitTest(realLocation)) 
				return AdornerHitTest.Control;
			return AdornerHitTest.None;
		}
		protected virtual System.Windows.Forms.Keys GetShortcut() {
			SearchPanelInfo info = FlyoutPanelInfo as SearchPanelInfo;
			if(info == null) return System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F;
			return info.Shortcut.Key;
		}
		void IBaseAdorner.RaiseShown() { RaiseShownCore(); }
		void IBaseAdorner.RaiseHidden() { RaiseHiddenCore(); }
		bool IBaseAdorner.Show() {
			Calc();
			return flyoutPanelInfoCore.ProcessShow(Point.Empty, true);
		}
		void IBaseAdorner.Hide() { flyoutPanelInfoCore.ProcessHide(); }
		void IBaseAdorner.Cancel() { flyoutPanelInfoCore.ProcessCancel(); }
		protected virtual void RaiseShownCore() {
			if(Owner != null) {
				Owner.HideNavigationAdorner();
				Owner.RaiseSearchPanelShown();
			}
		}
		protected virtual void RaiseHiddenCore() {
			if(Owner != null)
				Owner.RaiseSearchPanelHidden();
		}
		protected IBaseFlyoutPanelInfo FlyoutPanelInfo { get { return flyoutPanelInfoCore; } }
		protected WindowsUIView Owner { get { return ownerCore; } }
		protected virtual Views.WindowsUI.IBaseFlyoutPanelInfo CreateFlyoutInfo() { return new SearchPanelInfo(Owner); }
		protected override int CalcCore() {
			using(IMeasureContext context = Owner.BeginMeasure()) {
				Size size = flyoutPanelInfoCore.CalcMinSize(context.Graphics);
				flyoutPanelInfoCore.Calc(context.Graphics, new Rectangle(Point.Empty, size));
			}
			return -1;
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) { return null; }
		void ISearchPanelAdorner.ProcessMouseWheel(System.Windows.Forms.MouseEventArgs e) {
			SearchPanelInfo info = FlyoutPanelInfo as SearchPanelInfo;
			if(info == null) return;
			info.ProcessMouseWheel(e);
		}
		void ISearchPanelAdorner.Update() {
			if(FlyoutPanelInfo == null) return;
			FlyoutPanelInfo.UpdateStyle();
		}
		bool ISearchPanelAdorner.Enabled { get { return Owner.SearchPanelProperties.Enabled; } }
		#region ISearchPanelAdorner Members
		bool ISearchPanelAdorner.RaiseShowing(System.Windows.Forms.Control target) {
			bool cancel = !CanShowAdorner(target);
			return Owner.RaiseSearchPanelShowing(target, cancel);			
		}
		List<string> ignoreShowSearchPanel = new List<string>() { "DevExpress.XtraGrid.GridControl", "DevExpress.XtraTreeList.TreeList", "DevExpress.XtraVerticalGrid.VGridControlBase", "DevExpress.XtraSpreadsheet.SpreadsheetControl", "DevExpress.XtraSpreadsheet.Forms.FindReplaceForm" };
		bool CanShowAdorner(System.Windows.Forms.Control target) {
			System.Type targetType = target.GetType();
			while(targetType != typeof(System.Windows.Forms.Control)) {
				if(ignoreShowSearchPanel.Contains(targetType.FullName))
					return false;
				targetType = targetType.BaseType;
			}
			if(target.Parent == null) return true;
			return CanShowAdorner(target.Parent);
		}
		#endregion
	}
}
