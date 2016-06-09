#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public abstract class ImageAlignmentCommand : DashboardItemInteractionCommand<ImageDashboardItem> {
		protected abstract ImageHorizontalAlignment HorizontalAlignment { get; }
		protected abstract ImageVerticalAlignment VerticalAlignment { get; }
		protected ImageAlignmentCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(ImageDashboardItem item) {
			return item.HorizontalAlignment == HorizontalAlignment && item.VerticalAlignment == VerticalAlignment;
		}
		protected override IHistoryItem CreateHistoryItem(ImageDashboardItem dashboardItem, bool enabled) {
			return new ImageAlignmentHistoryItem(dashboardItem, HorizontalAlignment, VerticalAlignment);
		}
		protected override Image LoadLargeImage() {
			return null;
		}
	}
	public class ImageAlignmentTopLeftCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentTopLeft; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentTopLeftCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentTopLeftDescription; } }
		public override string ImageName { get { return "ImageAlignmentTopLeft"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Left; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Top; } }
		public ImageAlignmentTopLeftCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentTopCenterCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentTopCenter; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentTopCenterCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentTopCenterDescription; } }
		public override string ImageName { get { return "ImageAlignmentTopCenter"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Center; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Top; } }
		public ImageAlignmentTopCenterCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentTopRightCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentTopRight; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentTopRightCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentTopRightDescription; } }
		public override string ImageName { get { return "ImageAlignmentTopRight"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Right; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Top; } }
		public ImageAlignmentTopRightCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentCenterLeftCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentCenterLeft; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentCenterLeftCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentCenterLeftDescription; } }
		public override string ImageName { get { return "ImageAlignmentCenterLeft"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Left; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Center; } }
		public ImageAlignmentCenterLeftCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentCenterCenterCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentCenterCenter; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentCenterCenterCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentCenterCenterDescription; } }
		public override string ImageName { get { return "ImageAlignmentCenterCenter"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Center; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Center; } }
		public ImageAlignmentCenterCenterCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentCenterRightCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentCenterRight; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentCenterRightCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentCenterRightDescription; } }
		public override string ImageName { get { return "ImageAlignmentCenterRight"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Right; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Center; } }
		public ImageAlignmentCenterRightCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentBottomLeftCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentBottomLeft; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentBottomLeftCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentBottomLeftDescription; } }
		public override string ImageName { get { return "ImageAlignmentBottomLeft"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Left; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Bottom; } }
		public ImageAlignmentBottomLeftCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentBottomCenterCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentBottomCenter; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentBottomCenterCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentBottomCenterDescription; } }
		public override string ImageName { get { return "ImageAlignmentBottomCenter"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Center; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Bottom; } }
		public ImageAlignmentBottomCenterCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
	public class ImageAlignmentBottomRightCommand : ImageAlignmentCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageAlignmentBottomRight; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageAlignmentBottomRightCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageAlignmentBottomRightDescription; } }
		public override string ImageName { get { return "ImageAlignmentBottomRight"; } }
		protected override ImageHorizontalAlignment HorizontalAlignment { get { return ImageHorizontalAlignment.Right; } }
		protected override ImageVerticalAlignment VerticalAlignment { get { return ImageVerticalAlignment.Bottom; } }
		public ImageAlignmentBottomRightCommand(DashboardDesigner control)
			: base(control) {
		}
	} 
}
