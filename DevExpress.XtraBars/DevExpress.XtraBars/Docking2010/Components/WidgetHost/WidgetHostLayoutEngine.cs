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
using System.Windows.Forms;
using System.Windows.Forms.Layout;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class WidgetsLayoutEngine : LayoutEngine {
		public override bool Layout(object container, LayoutEventArgs layoutEventArgs) {
			if(!(container is WidgetsHost)) return base.Layout(container, layoutEventArgs);
			WidgetsHost host = container as WidgetsHost;
			if(host.LayoutSuspend) return true;
			int animationCount = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Animations.GetAnimationsCountByObject(host);
			if(animationCount != 0) return true;
			if(host.MaximizedContainer != null) {
				host.MaximizedContainer.Bounds = new Rectangle(Point.Empty, (container as Control).ClientRectangle.Size);
			}
			foreach(DocumentContainer item in host.Containers) {
				Document document = item.Document as Document;
				if(document == null ||  document.Info == null) continue;
				if(!document.IsVisible) {
					item.Bounds = new Rectangle(new Point(-10000, -10000), document.Info.Bounds.Size);
					continue;
				}
				if(item.Bounds == document.Info.Bounds || item.IsDisposing || document.ContainerControl == host.MaximizedContainer) continue;
				item.Bounds = document.Info.Bounds;
			}
			return true;
		}
	}
}
