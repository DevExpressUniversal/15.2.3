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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public abstract class ImageSizeModeCommand : DashboardItemInteractionCommand<ImageDashboardItem> {
		protected abstract ImageSizeMode SizeMode { get; }
		protected ImageSizeModeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(ImageDashboardItem item) {
			return item.SizeMode == SizeMode;
		}
		protected override IHistoryItem CreateHistoryItem(ImageDashboardItem dashboardItem, bool enabled) {
			return new ImageSizeModeHistoryItem(dashboardItem, SizeMode);
		}
	}
	public class ImageSizeModeClipCommand : ImageSizeModeCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageSizeModeClip; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageSizeModeClipCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageSizeModeClipDescription; } }
		public override string ImageName { get { return "ImageSizeModeClip"; } }
		protected override ImageSizeMode SizeMode { get { return ImageSizeMode.Clip; } }
		public ImageSizeModeClipCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ImageSizeModeStretchCommand : ImageSizeModeCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageSizeModeStretch; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageSizeModeStretchCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageSizeModeStretchDescription; } }
		public override string ImageName { get { return "ImageSizeModeStretchCommand"; } }
		protected override ImageSizeMode SizeMode { get { return ImageSizeMode.Stretch; } }
		public ImageSizeModeStretchCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ImageSizeModeSqueezeCommand : ImageSizeModeCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageSizeModeSqueeze; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageSizeModeSqueezeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageSizeModeSqueezeDescription; } }
		public override string ImageName { get { return "ImageSizeModeSqueezeCommand"; } }
		protected override ImageSizeMode SizeMode { get { return ImageSizeMode.Squeeze; } }
		public ImageSizeModeSqueezeCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ImageSizeModeZoomCommand : ImageSizeModeCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ImageSizeModeZoom; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandImageSizeModeZoomCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandImageSizeModeZoomDescription; } }
		public override string ImageName { get { return "ImageSizeModeZoomCommand"; } }
		protected override ImageSizeMode SizeMode { get { return ImageSizeMode.Zoom; } }
		public ImageSizeModeZoomCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
