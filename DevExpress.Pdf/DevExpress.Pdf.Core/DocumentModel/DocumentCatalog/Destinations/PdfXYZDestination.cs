#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfXYZDestination : PdfDestination {
		internal static void ValidateZoomValue(double? zoom) {
			if (zoom.HasValue && zoom.Value < 0)
				throw new ArgumentOutOfRangeException("zoom", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectZoom));
		}
		internal const string Name = "XYZ";
		readonly double? left;
		readonly double? top;
		readonly double? zoom;
		public double? Left { get { return left; } }
		public double? Top { get { return top; } }
		public double? Zoom { get { return zoom; } }
		public PdfXYZDestination(PdfPage page, double? left, double? top, double? zoom) : base(page) {
			this.left = left;
			this.top = top;
			this.zoom = zoom;
			ValidateZoomValue(zoom);
		}
		internal PdfXYZDestination(PdfDocumentCatalog documentCatalog, object pageObject, double? left, double? top, double? zoom) : base(documentCatalog, pageObject) {
			this.left = left;
			this.top = top;
			this.zoom = zoom;
		}
		PdfXYZDestination(PdfXYZDestination destination)
			: base(destination) {
			this.left = destination.left;
			this.top = destination.top;
			this.zoom = destination.zoom;
		}
		protected internal override PdfDestination CreateDuplicate() {
			return new PdfXYZDestination(this);
		}
		protected internal override PdfTarget CreateTarget(IList<PdfPage> pages) {
			double? actualZoom;
			if (zoom.HasValue) {
				double zoomValue = zoom.Value;
				actualZoom = zoomValue == 0 ? null : (double?)zoomValue;
			}
			else
				actualZoom = null;
			return new PdfTarget(CalculatePageIndex(pages), left, ValidateVerticalCoordinate(top), actualZoom);
		}
		protected override void AddWriteableParameters(IList<object> parameters) {
			parameters.Add(new PdfName(Name));
			AddParameter(parameters, left);
			AddParameter(parameters, top);
			AddParameter(parameters, zoom);
		}
	}
}
