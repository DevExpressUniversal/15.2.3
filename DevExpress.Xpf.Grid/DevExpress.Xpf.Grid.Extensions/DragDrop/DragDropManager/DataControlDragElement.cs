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

using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid {
	public class DataControlDragElement : CustomDragElement {
		protected internal FloatingWindowContainer FloatingContainer { get { return container as FloatingWindowContainer; } }
		internal bool ShouldOpenContainer { get { return this.shouldOpenContainer; } set { this.shouldOpenContainer = value; } }
		bool shouldOpenContainer = true;
		protected readonly Point initialOffset;
		protected readonly double xScale;
		protected readonly double yScale;
		public DataControlDragElement(DragDropManagerBase dragDropManager, Point offset, FrameworkElement owner) {
			initialOffset = offset;
			container.Owner = owner;
			container.Content = new ContentPresenter() {
				Content = dragDropManager.ViewInfo,
				ContentTemplate = dragDropManager.DragElementTemplate
				?? (dragDropManager.TemplatesContainer != null ? dragDropManager.TemplatesContainer.DefaultDragElementTemplate : null),
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
			};
			container.ShowContentOnly = true;
#if SL
			container.FloatSize = new Size(350, 300);
			xScale = 1.0;
			yScale = 1.0;
#else
			container.SizeToContent = SizeToContent.WidthAndHeight;
			PresentationSource source = PresentationSource.FromVisual(container.Owner);
			xScale = source.CompositionTarget.TransformToDevice.M11;
			yScale = source.CompositionTarget.TransformToDevice.M22;
#endif
			if(FloatingContainer != null && FloatingContainer.Window != null)
				FloatingContainer.Window.IsHitTestVisible = false;
		}
		protected override Point CorrectLocation(Point newLocation) {
			PointHelper.Offset(ref newLocation, (initialOffset.X + 10) * xScale, (initialOffset.Y + 16) * yScale);
			return newLocation;
		}
		protected override void UpdateContainer() {
			if(ShouldOpenContainer)
				this.container.IsOpen = true;
#if!SL
			container.UpdateAutoSize();
#endif
		}
		public override void Destroy() {
			base.Destroy();
			if(container.Owner is ILogicalOwner)
				((ILogicalOwner)container.Owner).RemoveChild(container);
		}
	}
}
