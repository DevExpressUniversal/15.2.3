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
using System.Windows;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Xpf.Printing.Native {
	public static class WatermarkLocalizers {
		public static string LocalizeImageViewMode(ImageViewMode mode) {
			string result;
			switch(mode) {
				case ImageViewMode.Clip:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_ImageClip);
					break;
				case ImageViewMode.Stretch:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_ImageStretch);
					break;
				case ImageViewMode.Zoom:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_ImageZoom);
					break;
				default:
					throw new InvalidOperationException();
			}
			return result;
		}
		public static string LocalizeDirectionMode(DirectionMode mode) {
			string result;
			switch(mode) {
				case DirectionMode.BackwardDiagonal:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_BackwardDiagonal);
					break;
				case DirectionMode.ForwardDiagonal:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_ForwardDiagonal);
					break;
				case DirectionMode.Horizontal:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_Horizontal);
					break;
				case DirectionMode.Vertical:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_Vertical);
					break;
				default:
					throw new InvalidOperationException();
			}
			return result;
		}
		public static string LocalizeVerticalAlignment(VerticalAlignment alignment) {
			string result;
			switch(alignment) {
				case VerticalAlignment.Top:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_VertAlign_Top);
					break;
				case VerticalAlignment.Center:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_VertAlign_Middle);
					break;
				case VerticalAlignment.Bottom:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_VertAlign_Bottom);
					break;
				default:
					throw new InvalidOperationException();
			}
			return result;
		}
		public static string LocalizeHorizontalAlignment(HorizontalAlignment alignment) {
			string result;
			switch(alignment) {
				case HorizontalAlignment.Left:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_HorzAlign_Left);
					break;
				case HorizontalAlignment.Center:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_HorzAlign_Center);
					break;
				case HorizontalAlignment.Right:
					result = PreviewLocalizer.GetString(PreviewStringId.WMForm_HorzAlign_Right);
					break;
				default:
					throw new InvalidOperationException();
			}
			return result;
		}
	}
}
