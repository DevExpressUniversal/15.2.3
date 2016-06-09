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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils.Controls;
using System.Reflection;
using System.Drawing;
namespace DevExpress.XtraLayout.Customization {
	public enum ConstraintsGlyphTypes {
		Locked,
		MinSize,
		MaxSize,
		NoConstraints
	}
	public class ConstraintsVisualizer {
		protected static ConstraintsGlyphTypes CalcGlyphType(int size, int min, int max, bool shouldUseMoreThen = false) {
			if(shouldUseMoreThen) {
				if(max == min && min > 0) return ConstraintsGlyphTypes.Locked;
				if(size <= min && min > 0) return ConstraintsGlyphTypes.MinSize;
				if(size >= max && max > 0) return ConstraintsGlyphTypes.MaxSize;
				return ConstraintsGlyphTypes.NoConstraints;
			}
			if (size == min && size == max) return ConstraintsGlyphTypes.Locked;
			if (size == min) return ConstraintsGlyphTypes.MinSize;
			if (size == max) return ConstraintsGlyphTypes.MaxSize;
			return ConstraintsGlyphTypes.NoConstraints;
		}
		Size hArrowSize = LayoutControlImageStorage.Default.LeftArrow.Size;
		Size vArrowSize = LayoutControlImageStorage.Default.UpArrow.Size;
		Size lockedSignSize = LayoutControlImageStorage.Default.LockedSign.Size;
		Size signSize = LayoutControlImageStorage.Default.BigSign.Size;
		protected int CalcCenter(int x, int w1, int w2) {
			return x + (w1 - w2) / 2;
		}
		protected void PaintImage(Graphics g, Image target, Rectangle rect) {
			g.DrawImageUnscaled(target, rect.Location);
		}
		public Image GetItemStateImage(BaseLayoutItem lci) {
			return GetItemStateImageCore(lci);
		}
		internal Image GetItemStateImageCore(BaseLayoutItem lci) {
			Size minSize = lci.MinSize;
			Size maxSize = lci.MaxSize;
			Size itemSize = lci.Size;
			ConstraintsGlyphTypes hType = CalcGlyphType(itemSize.Width, minSize.Width, maxSize.Width);
			ConstraintsGlyphTypes vType = CalcGlyphType(itemSize.Height, minSize.Height, maxSize.Height);
			Rectangle itemRectangle = lci.ViewInfo.BoundsRelativeToControl;
			Size stateImageSize = new Size((hArrowSize.Width + 1) * 2 + signSize.Width, (vArrowSize.Height + 1) * 2 + signSize.Height);
			Rectangle stateImageRect = new Rectangle(
				Point.Empty,
				stateImageSize
				);
			Rectangle topElement, bottomElement, leftElement, rightElement, centerElement;
			topElement = new Rectangle(CalcCenter(stateImageRect.X, stateImageRect.Width, vArrowSize.Width), stateImageRect.Y, vArrowSize.Width, vArrowSize.Height);
			bottomElement = new Rectangle(CalcCenter(stateImageRect.X, stateImageRect.Width, vArrowSize.Width), stateImageRect.Bottom - vArrowSize.Height, vArrowSize.Width, vArrowSize.Height);
			leftElement = new Rectangle(stateImageRect.X, CalcCenter(stateImageRect.Y, stateImageRect.Height, hArrowSize.Height), hArrowSize.Width, hArrowSize.Height);
			rightElement = new Rectangle(stateImageRect.Right - hArrowSize.Width, CalcCenter(stateImageRect.Y, stateImageRect.Height, hArrowSize.Height), hArrowSize.Width, hArrowSize.Height);
			centerElement = new Rectangle(CalcCenter(stateImageRect.X, stateImageRect.Width, signSize.Width), CalcCenter(stateImageRect.Y, stateImageRect.Height, signSize.Height), signSize.Width, signSize.Height);
			Bitmap resultStateImage = new Bitmap(stateImageSize.Width, stateImageSize.Height);
			if(hType == ConstraintsGlyphTypes.NoConstraints && vType == ConstraintsGlyphTypes.NoConstraints) return null;
			using(Graphics g = Graphics.FromImage(resultStateImage)) {
				PaintImage(g, LayoutControlImageStorage.Default.BigSign, centerElement);
				switch(hType) {
					case ConstraintsGlyphTypes.NoConstraints:
						break;
					case ConstraintsGlyphTypes.MinSize:
						PaintImage(g, LayoutControlImageStorage.Default.RightArrow, leftElement);
						PaintImage(g, LayoutControlImageStorage.Default.LeftArrow, rightElement);
						break;
					case ConstraintsGlyphTypes.MaxSize:
						PaintImage(g, LayoutControlImageStorage.Default.LeftArrow, leftElement);
						PaintImage(g, LayoutControlImageStorage.Default.RightArrow, rightElement);
						break;
					case ConstraintsGlyphTypes.Locked:
						PaintImage(g, LayoutControlImageStorage.Default.LockedSign, leftElement);
						PaintImage(g, LayoutControlImageStorage.Default.LockedSign, rightElement);
						break;
				}
				switch(vType) {
					case ConstraintsGlyphTypes.NoConstraints:
						break;
					case ConstraintsGlyphTypes.MinSize:
						PaintImage(g, LayoutControlImageStorage.Default.DownArrow, topElement);
						PaintImage(g, LayoutControlImageStorage.Default.UpArrow, bottomElement);
						break;
					case ConstraintsGlyphTypes.MaxSize:
						PaintImage(g, LayoutControlImageStorage.Default.UpArrow, topElement);
						PaintImage(g, LayoutControlImageStorage.Default.DownArrow, bottomElement);
						break;
					case ConstraintsGlyphTypes.Locked:
						PaintImage(g, LayoutControlImageStorage.Default.LockedSign, topElement);
						PaintImage(g, LayoutControlImageStorage.Default.LockedSign, bottomElement);
						break;
				}
			}
			return resultStateImage;
		}
		internal static ConstraintsGlyphTypes CalcGlyphType(BaseLayoutItem lci, LayoutType layoutType) {
			switch(layoutType) {
				case LayoutType.Horizontal:
					return CalcGlyphType(lci.Size.Width, lci.MinSize.Width, lci.MaxSize.Width,true);
				case LayoutType.Vertical:
					return CalcGlyphType(lci.Size.Height, lci.MinSize.Height, lci.MaxSize.Height,true);
			}
			return ConstraintsGlyphTypes.NoConstraints;
		}
	}
}
