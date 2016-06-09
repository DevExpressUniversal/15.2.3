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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
namespace DevExpress.XtraLayout.Customization.UserCustomization {
	public class BeakPos {
		public Point bPoint;
		public Point cPoint;
		Rectangle selectionRect, bFormRect, cFormRect, screenRect;
		public BeakPos(Rectangle SelectionRect, Rectangle BFormRect, Rectangle CFormRect, Rectangle ScreenRect) {
			selectionRect = SelectionRect;
			bFormRect = BFormRect;
			cFormRect = CFormRect;
			screenRect = ScreenRect;
		}
		private bool RposRout() {
			if(selectionRect.Right < screenRect.Right - (cFormRect.Width + bFormRect.Width)) return false;
			return true;
		}
		private bool RposBout() {
			if(selectionRect.Y + (selectionRect.Height / 2) + Math.Max(bFormRect.Height, cFormRect.Height) < screenRect.Bottom) return false;
			return true;
		}
		private bool BposRout() {
			if(selectionRect.Left + (selectionRect.Width / 2) + (cFormRect.Width + bFormRect.Width) < screenRect.Right) return false;
			return true;
		}
		private bool BposBout() {
			if(selectionRect.Bottom + Math.Max(bFormRect.Height, cFormRect.Height) < screenRect.Bottom) return false;
			return true;
		}
		public void Calculate() {
			if(!RposRout()) {
				if(!RposBout()) {
					bPoint = new Point(selectionRect.Right, selectionRect.Y + (selectionRect.Height / 2));
					cPoint = new Point(bPoint.X, bPoint.Y);
				} else {
					bPoint = new Point(selectionRect.Right, selectionRect.Y - bFormRect.Height);
					cPoint = new Point(bPoint.X, bPoint.Y);
				}
			} else {
				if(!BposRout()) {
					if(!BposBout()) {
						bPoint = new Point(selectionRect.Right - (selectionRect.Width / 2), selectionRect.Bottom);
						cPoint = new Point(bPoint.X, bPoint.Y);
					} else {
						if(selectionRect.Height > Math.Max(bFormRect.Height, cFormRect.Height)) {
							bPoint = new Point(selectionRect.Right - (selectionRect.Width / 2), selectionRect.Y);
							cPoint = new Point(bPoint.X, bPoint.Y);
						} else {
							bPoint = new Point(selectionRect.Right - (selectionRect.Width / 2), selectionRect.Y - bFormRect.Height);
							cPoint = new Point(bPoint.X, bPoint.Y);
						}
					}
				} else {
					if((selectionRect.Height / 2) + (selectionRect.Y) > Math.Max(bFormRect.Height, cFormRect.Height)) {
						bPoint = new Point(selectionRect.Left - bFormRect.Width, selectionRect.Y + (selectionRect.Height / 2) - bFormRect.Height);
						cPoint = new Point(bPoint.X, bPoint.Y);
					} else {
						bPoint = new Point(selectionRect.Left - bFormRect.Width, selectionRect.Y + (selectionRect.Height / 2));
						cPoint = new Point(bPoint.X, bPoint.Y);
					}
				}
			}
		}
	}
}
