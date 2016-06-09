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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Customization {
	static class DockHintExtension {
		public static bool IsCenterSide(DockHint hint) {
			return
				(hint == DockHint.CenterSideLeft) || (hint == DockHint.CenterSideTop) ||
				(hint == DockHint.CenterSideRight) || (hint == DockHint.CenterSideBottom);
		}
		public static Size GetDockZoneSize(Size size, DockHint hint) {
			switch(hint) {
				case DockHint.CenterSideRight:
				case DockHint.CenterSideLeft:
				case DockHint.SideRight:
				case DockHint.SideLeft:
					return new Size(size.Width / 4, size.Height);
				case DockHint.CenterSideTop:
				case DockHint.CenterSideBottom:
				case DockHint.SideTop:
				case DockHint.SideBottom:
					return new Size(size.Width, size.Height / 4);
				case DockHint.SnapLeft:
				case DockHint.SnapRight:
				case DockHint.CenterLeft:
				case DockHint.CenterRight:
					return new Size(size.Width / 2, size.Height);
				case DockHint.SnapBottom:
				case DockHint.CenterTop:
				case DockHint.CenterBottom:
					return new Size(size.Width, size.Height / 2);
				case DockHint.SnapScreen:
				case DockHint.Center:
					return size;
			}
			throw new NotSupportedException();
		}
		public static Size GetSnapGuideSize(Size size, Size container, DockGuide guide) {
			switch(guide) {
				case DockGuide.SnapLeft:
				case DockGuide.SnapRight:
					return new Size(size.Width, container.Height);
				case DockGuide.SnapTop:
				case DockGuide.SnapBottom:
					return new Size(container.Width, size.Height);
			}
			throw new NotSupportedException();
		}
		public static ContentAlignment ToAlignment(DockHint hint) {
			switch(hint) {
				case DockHint.SnapLeft:
				case DockHint.SideLeft:
				case DockHint.CenterSideLeft:
				case DockHint.CenterLeft:
					return ContentAlignment.MiddleLeft;
				case DockHint.SnapRight:
				case DockHint.SideRight:
				case DockHint.CenterSideRight:
				case DockHint.CenterRight:
					return ContentAlignment.MiddleRight;
				case DockHint.SnapScreen:
				case DockHint.SideTop:
				case DockHint.CenterSideTop:
				case DockHint.CenterTop:
					return ContentAlignment.TopCenter;
				case DockHint.SnapBottom:
				case DockHint.SideBottom:
				case DockHint.CenterSideBottom:
				case DockHint.CenterBottom:
					return ContentAlignment.BottomCenter;
				case DockHint.Center:
					return ContentAlignment.MiddleCenter;
			}
			throw new NotSupportedException();
		}
		public static Docking.DockingStyle GetDockingStyle(DockHint hint) {
			switch(hint) {
				case DockHint.CenterSideLeft:
				case DockHint.SideLeft:
					return Docking.DockingStyle.Left;
				case DockHint.CenterSideTop:
				case DockHint.SideTop:
					return Docking.DockingStyle.Top;
				case DockHint.CenterSideRight:
				case DockHint.SideRight:
					return Docking.DockingStyle.Right;
				case DockHint.CenterSideBottom:
				case DockHint.SideBottom:
					return Docking.DockingStyle.Bottom;
			}
			throw new NotSupportedException(hint.ToString());
		}
	}
	static class DockGuideExtension {
		public static ContentAlignment ToAlignment(DockGuide guide) {
			switch(guide) {
				case DockGuide.SnapLeft: 
				case DockGuide.Left: 
					return ContentAlignment.MiddleLeft;
				case DockGuide.SnapRight: 
				case DockGuide.Right: 
					return ContentAlignment.MiddleRight;
				case DockGuide.SnapTop: 
				case DockGuide.Top: 
					return ContentAlignment.TopCenter;
				case DockGuide.SnapBottom: 
				case DockGuide.Bottom: 
					return ContentAlignment.BottomCenter;
			}
			return ContentAlignment.MiddleCenter;
		}
		public static DockHint ToSideDockHint(DockGuide guide) {
			DockHint result = DockHint.None;
			switch(guide) {
				case DockGuide.Left: result = DockHint.SideLeft; break;
				case DockGuide.Right: result = DockHint.SideRight; break;
				case DockGuide.Top: result = DockHint.SideTop; break;
				case DockGuide.Bottom: result = DockHint.SideBottom; break;
			}
			return result;
		}
		public static DockHint ToSnapDockHint(DockGuide guide) {
			DockHint result = DockHint.None;
			switch(guide) {
				case DockGuide.SnapLeft: result = DockHint.SnapLeft; break;
				case DockGuide.SnapRight: result = DockHint.SnapRight; break;
				case DockGuide.SnapTop: result = DockHint.SnapScreen; break;
				case DockGuide.SnapBottom: result = DockHint.SnapBottom; break;
			}
			return result;
		}
		public static DockGuide GetSnap(Rectangle r, Point p) {
			bool f1 = (p.Y - p.X) < (r.Top - r.Left);
			bool f2 = (p.Y + p.X) < (r.Top + r.Right);
			bool f3 = (p.Y - p.X) > (r.Bottom - r.Right);
			bool f4 = (p.Y + p.X) > (r.Bottom + r.Left);
			if(f1 && f2 && (p.Y < r.Top + r.Height / 2))
				return DockGuide.SnapTop;
			if(f3 && f4 && (p.Y > r.Top + r.Height / 2))
				return DockGuide.SnapBottom;
			if(!f1 && !f4 && (p.X < r.Left + r.Width / 2))
				return DockGuide.SnapLeft;
			if(!f2 && !f3 && (p.X > r.Left + r.Width / 2))
				return DockGuide.SnapRight;
			if(p.X == r.Left + r.Width / 2)
				return DockGuide.SnapLeft;
			return DockGuide.SnapTop;
		}
	}
}
