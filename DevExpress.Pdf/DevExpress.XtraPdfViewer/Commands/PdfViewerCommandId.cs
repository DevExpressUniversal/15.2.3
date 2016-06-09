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
using DevExpress.Utils;
namespace DevExpress.XtraPdfViewer.Commands {
	public struct PdfViewerCommandId : IConvertToInt<PdfViewerCommandId>, IEquatable<PdfViewerCommandId> {
		public static readonly PdfViewerCommandId None = CreateCommand();
		public static readonly PdfViewerCommandId OpenFile = CreateCommand();
		public static readonly PdfViewerCommandId SaveAsFile = CreateCommand();
		public static readonly PdfViewerCommandId ExportFormData = CreateCommand();
		public static readonly PdfViewerCommandId ImportFormData = CreateCommand();
		public static readonly PdfViewerCommandId PrintFile = CreateCommand();
		public static readonly PdfViewerCommandId PreviousPage = CreateCommand();
		public static readonly PdfViewerCommandId NextPage = CreateCommand();
		public static readonly PdfViewerCommandId FindText = CreateCommand();
		public static readonly PdfViewerCommandId ShowExactZoomList = CreateCommand();
		public static readonly PdfViewerCommandId ZoomIn = CreateCommand();
		public static readonly PdfViewerCommandId ZoomOut = CreateCommand();
		public static readonly PdfViewerCommandId SetZoom = CreateCommand();
		public static readonly PdfViewerCommandId Zoom500 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom400 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom200 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom150 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom125 = CreateCommand();		
		public static readonly PdfViewerCommandId Zoom100 = CreateCommand();	   
		public static readonly PdfViewerCommandId Zoom75 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom50 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom25 = CreateCommand();
		public static readonly PdfViewerCommandId Zoom10 = CreateCommand();
		public static readonly PdfViewerCommandId SetActualSizeZoomMode = CreateCommand();
		public static readonly PdfViewerCommandId SetPageLevelZoomMode = CreateCommand();
		public static readonly PdfViewerCommandId SetFitWidthZoomMode = CreateCommand();
		public static readonly PdfViewerCommandId SetFitVisibleZoomMode = CreateCommand();
		public static readonly PdfViewerCommandId RotatePageClockwise = CreateCommand();
		public static readonly PdfViewerCommandId RotatePageCounterclockwise = CreateCommand();
		public static readonly PdfViewerCommandId PreviousView = CreateCommand();
		public static readonly PdfViewerCommandId NextView = CreateCommand();
		public static readonly PdfViewerCommandId ShowDocumentProperties = CreateCommand();
		public static readonly PdfViewerCommandId HandTool = CreateCommand();
		public static readonly PdfViewerCommandId SelectTool = CreateCommand();
		public static readonly PdfViewerCommandId SelectAll = CreateCommand();
		public static readonly PdfViewerCommandId Copy = CreateCommand();
		public static readonly PdfViewerCommandId OutlinesWrapLongLines = CreateCommand();
		public static readonly PdfViewerCommandId OutlinesTextSizeToLarge = CreateCommand();
		public static readonly PdfViewerCommandId OutlinesTextSizeToMedium = CreateCommand();
		public static readonly PdfViewerCommandId OutlinesTextSizeToSmall = CreateCommand();
		public static readonly PdfViewerCommandId GotoOutline = CreateCommand();
		public static readonly PdfViewerCommandId OutlineViewerHideAfterUse = CreateCommand();
		public static readonly PdfViewerCommandId ExpandCurrentOutline = CreateCommand();
		public static readonly PdfViewerCommandId OutlinePrintPages = CreateCommand();
		public static readonly PdfViewerCommandId OutlinePrintSections = CreateCommand();
		public static readonly PdfViewerCommandId OutlinesExpandCollapseTopLevel = CreateCommand();
		static int lastCommandId = -1;
		static PdfViewerCommandId CreateCommand() {
			return new PdfViewerCommandId(++lastCommandId);
		}
		readonly int value;
		public PdfViewerCommandId(int value) {
			this.value = value;
		}
		public bool Equals(PdfViewerCommandId other) {
			return value == other.value;
		}
		public override bool Equals(object obj) {
			return ((obj is PdfViewerCommandId) && value == ((PdfViewerCommandId)obj).value);
		}
		public override int GetHashCode() {
			return value.GetHashCode();
		}
		int IConvertToInt<PdfViewerCommandId>.ToInt() {
			return value;
		}
		PdfViewerCommandId IConvertToInt<PdfViewerCommandId>.FromInt(int value) {
			return new PdfViewerCommandId(value);
		}
	}
}
