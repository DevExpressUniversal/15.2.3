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
namespace DevExpress.XtraScheduler.Drawing {
	public class SchedulerHeaderPreliminaryLayoutResult {
		#region Fields
		SchedulerHeaderPainter painter;
		int contentLeftPadding;
		int contentRightPadding;
		int contentTopPadding;
		int contentBottomPadding;
		int imageTextGapSize;
		HeaderImageAlign imageAlign;
		HeaderImageSizeMode imageSizeMode;
		Size textSize;		 
		Size imageSize;		
		Size fixedImageSize;   
		Size size;			 
		Size viewSize;		 
		bool rotateCaption;
		int fixedHeaderHeight; 
		#endregion
		#region Properties
		public int ContentLeftPadding { get { return contentLeftPadding; } set { contentLeftPadding = value; } }
		public int ContentRightPadding { get { return contentRightPadding; } set { contentRightPadding = value; } }
		public int ContentTopPadding { get { return contentTopPadding; } set { contentTopPadding = value; } }
		public int ContentBottomPadding { get { return contentBottomPadding; } set { contentBottomPadding = value; } }
		public int ImageTextGapSize { get { return imageTextGapSize; } set { imageTextGapSize = value; } }
		public HeaderImageAlign ImageAlign { get { return imageAlign; } set { imageAlign = value; } }
		public HeaderImageSizeMode ImageSizeMode { get { return imageSizeMode; } set { imageSizeMode = value; } }
		public SchedulerHeaderPainter Painter { get { return painter; } set { painter = value; } }
		internal Size TextSize { get { return textSize; } set { textSize = value; } }
		internal Size ImageSize { get { return imageSize; } set { imageSize = value; } }
		internal Size FixedImageSize { get { return fixedImageSize; } set { fixedImageSize = value; } }
		internal Size Size { get { return size; } set { size = value; } }
		internal bool RotateCaption { get { return rotateCaption; } set { rotateCaption = value; } }
		internal int FixedHeaderHeight { get { return fixedHeaderHeight; } set { fixedHeaderHeight = value; } }
		public Size ViewSize { get { return this.viewSize; } set { this.viewSize = value; } }
		#endregion
	}
}
