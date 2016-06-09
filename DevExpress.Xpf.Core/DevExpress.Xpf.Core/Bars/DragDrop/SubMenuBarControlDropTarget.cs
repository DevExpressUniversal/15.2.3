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
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Bars {
	public class SubMenuBarControlDropTarget : BarControlDropTarget {
		public SubMenuBarControlDropTarget(LinksControl linksControl) : base(linksControl) { }
		protected override bool IsAfter(Point p, BarItemLinkControl linkControl, Rect linkControlBounds) {
			return p.Y > linkControl.ActualHeight / 2;
		}
		protected override void UpdateDropIndicatorParams(ContentControl dropIndicator) {
			dropIndicator.Height = LinksControl.ItemsPresenter.ActualWidth;
			BarDragProvider.SetDropIndicatorOrientation(dropIndicator, Orientation.Vertical);
		}
		protected override Point GetAdornerLocation(BarItemLinkControl linkControl, bool insertArter) {
			GeneralTransform transform = linkControl.TransformToVisual(LinksControl.ItemsPresenter);
			Point selLinkLeftTopCorner = linkControl.TranslatePointWithoutTransform(new Point(0, 0));
			Point selLinkLeftBottomCorner = linkControl.TranslatePointWithoutTransform(new Point(0, linkControl.ActualHeight));
			return insertArter ? transform.Transform(selLinkLeftBottomCorner) : transform.Transform(selLinkLeftTopCorner);
		}
	}
}
