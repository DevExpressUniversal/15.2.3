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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public class CellsAutomationPeer : ScrollableAreaAutomationPeer, IScrollProvider {
		public CellsAutomationPeer(PivotGridControl pivotGrid)
			: base(pivotGrid, LayoutHelper.FindElement(pivotGrid, (d) => d is CellsAreaPresenter)) { }
		ScrollableCellsAreaPresenter Presenter { get { return (ScrollableCellsAreaPresenter)Owner; } }
		IScrollInfo ScrollInfo { get { return Presenter; } }
		#region IScrollProvider Members
		public double HorizontalScrollPercent {
			get { return ScrollInfo.HorizontalOffset / ScrollInfo.ExtentWidth; }
		}
		public double HorizontalViewSize {
			get { return ScrollInfo.ViewportWidth; }
		}
		public bool HorizontallyScrollable {
			get { return ScrollInfo.ScrollOwner.ComputedHorizontalScrollBarVisibility == Visibility.Visible; }
		}
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			Presenter.Left += (int)horizontalAmount;
			Presenter.Top += (int)verticalAmount;
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {
			if(horizontalPercent > 1 || verticalPercent > 1 || horizontalPercent < 0 || verticalPercent < 0)
				throw new ArgumentException();
			Presenter.Left = Convert.ToInt32(ScrollInfo.ExtentWidth * horizontalPercent);
			Presenter.Top = Convert.ToInt32(ScrollInfo.ExtentWidth * horizontalPercent);
		}
		public double VerticalScrollPercent {
			get { return ScrollInfo.HorizontalOffset / ScrollInfo.ExtentWidth; }
		}
		public double VerticalViewSize {
			get { return ScrollInfo.ViewportHeight; }
		}
		public bool VerticallyScrollable {
			get { return ScrollInfo.ScrollOwner.ComputedVerticalScrollBarVisibility == Visibility.Visible; }
		}
		#endregion
		protected override AutomationPeer CreatePeerCore(DependencyObject obj) {
			ScrollableAreaCell cell = obj as ScrollableAreaCell;
			if(cell == null)
				return base.CreatePeerCore(obj);
			else
				return new Celltem(PivotGrid, cell);
		}
		protected override string GetNameCore() {
			return "Cells";
		}
	}
}
