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
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public class WindowsUIViewInfo : BaseViewInfo {
		public WindowsUIViewInfo(WindowsUIView view)
			: base(view) {
		}
		public IContentContainerInfo ContentInfo { get; private set; }
		public IContentContainerInfo FlyoutInfo { get; private set; }
		protected WindowsUIView WindowsUIView {
			get { return View as WindowsUIView; }
		}
		protected override IDockingAdornerInfo CreateEmptyViewAdornerInfo() {
			return new EmptyViewDockingAdornerInfo(WindowsUIView);
		}
		protected internal override Point GetFloatLocation(BaseDocument document) {
			return document.Control.PointToScreen(Point.Empty);
		}
		protected override Rectangle[] CalculateCore(Graphics g, Rectangle bounds) {
			if(ContentInfo != null)
				ContentInfo.Calc(g, bounds);
			if(FlyoutInfo != null)
				FlyoutInfo.Calc(g, bounds);
			return new Rectangle[] { };
		}
		protected override void UpdateAppearancesCore() {
			ColoredElementsCache.Reset();
			base.UpdateAppearancesCore();
		}
		public void RegisterContentContainer(IContentContainer container) {
			if(container.Info is IFlyoutInfo)
				FlyoutInfo = container.Info;
			else
				ContentInfo = container.Info;
			LayoutHelper.Register(View, container.Info);
		}
		public void UnRegisterContentContainer(IContentContainer container) {
			if(container == null || container.Info == null) return;
			if(container.Info is IFlyoutInfo) {
				LayoutHelper.Unregister(View, FlyoutInfo);
				FlyoutInfo = null;
			}
			else {
				LayoutHelper.Unregister(View, ContentInfo);
				ContentInfo = null;
			}
		}
		protected internal override Rectangle GetBounds(BaseDocument document) {
			if(ContentInfo != null) 
				return ContentInfo.GetBounds((Document)document);
			return Bounds;
		}
	}
} 
