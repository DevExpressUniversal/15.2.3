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
namespace DevExpress.XtraPdfViewer.Native {
	public enum PdfNavigationMode { None, Page, Position, Scroll, Zoom, Rotation, Selection, ReferencedDocumentOpening };
	public class PdfDocumentViewState {
		readonly PdfNavigationMode navigationMode;
		readonly PointF location;
		readonly PdfZoomMode zoomMode;
		readonly float zoom;
		readonly int rotationAngle;
		readonly string filePath;
		readonly DateTime timeStamp;
		public PdfNavigationMode NavigationMode { get { return navigationMode; } }
		public PointF Location { get { return location; } }
		public PdfZoomMode ZoomMode { get { return zoomMode; } }
		public float Zoom { get { return zoom; } }
		public int RotationAngle { get { return rotationAngle; } }
		public string FilePath { get { return filePath; } }
		public DateTime TimeStamp { get { return timeStamp; } }
		public PdfDocumentViewState(PdfNavigationMode navigationMode, PointF location, PdfZoomMode zoomMode, float zoom, int rotationAngle, string filePath, DateTime timeStamp) {
			this.navigationMode = navigationMode;
			this.location = location;
			this.zoomMode = zoomMode;
			this.zoom = zoom;
			this.rotationAngle = rotationAngle;
			this.filePath = filePath;
			this.timeStamp = timeStamp;
		}
		public bool CompareNavigationModesAndTimeStamps(PdfDocumentViewState viewState) {
			return ((navigationMode == PdfNavigationMode.Zoom && viewState.NavigationMode == PdfNavigationMode.Zoom) ||
			   (navigationMode == PdfNavigationMode.Scroll && viewState.NavigationMode == PdfNavigationMode.Scroll) ||
			   (navigationMode == PdfNavigationMode.Selection && viewState.NavigationMode == PdfNavigationMode.Selection)) &&
			   viewState.TimeStamp - timeStamp < new TimeSpan(0, 0, 0, 2, 0) ||
			   (navigationMode == PdfNavigationMode.Zoom && viewState.NavigationMode == PdfNavigationMode.Zoom && zoomMode == viewState.ZoomMode && zoomMode != PdfZoomMode.Custom && zoomMode != PdfZoomMode.ActualSize);
		}
	};
}
