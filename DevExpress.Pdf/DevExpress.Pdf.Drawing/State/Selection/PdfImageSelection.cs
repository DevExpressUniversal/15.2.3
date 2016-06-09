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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfImageSelection : PdfSelection {
		public static bool AreEqual(PdfImageSelection s1, PdfImageSelection s2) {
			if (Object.ReferenceEquals(s1, null))
				return object.ReferenceEquals(s2, null);
			if (Object.ReferenceEquals(s2, null))
				return false;
			return s1.pageIndex == s2.pageIndex && PdfRectangle.AreEqual(s1.clipRectangle, s2.clipRectangle, 0.0001) && s1.pageImageData.Equals(s2.pageImageData);
		}
		readonly int pageIndex;
		readonly PdfPageImageData pageImageData;
		readonly PdfRectangle clipRectangle;
		PdfOrientedRectangle selectionRecangle;
		public PdfPageImageData PageImageData { get { return pageImageData; } }
		public PdfRectangle ClipRectangle {
			get {
				PdfRectangle imageRectangle = pageImageData.BoundingRectangle;
				if (clipRectangle == null)
					return new PdfRectangle(0, 0, imageRectangle.Width, imageRectangle.Height);
				return new PdfRectangle(clipRectangle.Left - imageRectangle.Left, clipRectangle.Bottom - imageRectangle.Bottom, clipRectangle.Right - imageRectangle.Left, clipRectangle.Top - imageRectangle.Bottom);
			}
		}
		public override PdfDocumentContentType ContentType { get { return PdfDocumentContentType.Image; } }
		public override IList<PdfContentHighlight> Highlights {
			get {
				if (selectionRecangle == null) {
					PdfRectangle imageRectangle = pageImageData.BoundingRectangle;
					if (clipRectangle == null) {
						selectionRecangle = new PdfOrientedRectangle(imageRectangle.TopLeft, imageRectangle.Width, imageRectangle.Height, 0);
					}
					else {
						PdfRectangle clip = ClipRectangle;
						selectionRecangle = new PdfOrientedRectangle(new PdfPoint(imageRectangle.Left + clip.Left, imageRectangle.Bottom + clip.Top), clip.Width, clip.Height, 0);
					}
				}
				return new PdfContentHighlight[] { new PdfImageHighlight(pageIndex, selectionRecangle) };
			}
		}
		public PdfImageSelection(int pageIndex, PdfPageImageData pageImageData, PdfRectangle clipRectangle) {
			this.pageIndex = pageIndex;
			this.pageImageData = pageImageData;
			this.clipRectangle = clipRectangle;
		}
		public PdfImageSelection(PdfImageSelection imageSelection) : this(imageSelection.pageIndex, imageSelection.pageImageData, imageSelection.clipRectangle) {
		}
		public override PdfRectangle GetBoundingBox(int pageIndex) {
			if (pageIndex != this.pageIndex)
				return null;
			PdfRectangle boundingRectangle = pageImageData.BoundingRectangle;
			double left = boundingRectangle.Left + clipRectangle.Left;
			double bottom = boundingRectangle.Bottom + clipRectangle.Bottom;
			return new PdfRectangle(left, bottom, left + clipRectangle.Width, bottom + clipRectangle.Height);
		}
	}
}
