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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class ResourceHeader : SchedulerHeader {
		readonly SchedulerResourceHeaderOptions options;
		protected ResourceHeader(BaseHeaderAppearance appearance, SchedulerResourceHeaderOptions options)
			: base(appearance) {
			if (options == null)
				Exceptions.ThrowArgumentException("options", options);
			this.options = options;
		}
		#region Properties
		protected internal SchedulerResourceHeaderOptions Options { get { return options; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ResourceHeaderHitTestType")]
#endif
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.ResourceHeader; } }
		#endregion
		protected internal override string CalcActualSkinElementName(bool hideSelection) {
			return SchedulerSkins.SkinHeaderResource;
		}
		internal override InterpolationMode GetImageInterpolationMode() {
			return Options.ImageInterpolationMode;
		}
		protected internal override void CalculateIntermediateParameters(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			base.CalculateIntermediateParameters(preliminaryResult);
			preliminaryResult.FixedHeaderHeight = Options.Height;
			preliminaryResult.FixedImageSize = Options.ImageSize;
			preliminaryResult.ImageAlign = Options.ImageAlign;
			preliminaryResult.ImageSizeMode = Options.ImageSizeMode;
			preliminaryResult.RotateCaption = Options.RotateCaption;
			if (preliminaryResult.FixedImageSize == Size.Empty)
				preliminaryResult.FixedImageSize = CalculateFixedImageSize(preliminaryResult);
		}
		protected virtual Size CalculateResourceHeaderAutoSize(SchedulerHeaderPreliminaryLayoutResult preliminaryResult, int xPadding, int yPadding) {
			return CalculateResourceHeaderImageSize(Bounds.Width - xPadding, preliminaryResult.ViewSize.Height / 6 - yPadding);
		}
		protected Size CalculateResourceHeaderImageSize(int width, int height) {
			if (this is HorizontalResourceHeader)
				return new Size(width, height);
			return new Size(height, width);
		}
		protected internal override bool RaiseCustomDrawEvent(GraphicsCache cache, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate) {
			this.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(this, this.Bounds, defaultDrawDelegate);
				customDrawProvider.RaiseCustomDrawResourceHeader(args);
				return args.Handled;
			} finally {
				this.Cache = null;
			}
		}
		protected override void InitializeHeaderImage() {
			Image = Resource.GetImage();
		}
		Size CalculateFixedImageSize(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			int xPadding = preliminaryResult.ContentLeftPadding + preliminaryResult.ContentRightPadding;
			int yPadding = preliminaryResult.ContentTopPadding + preliminaryResult.ContentBottomPadding;
			if (preliminaryResult.FixedHeaderHeight > 0)
				return CalculateResourceHeaderImageSize(Bounds.Width - xPadding, preliminaryResult.FixedHeaderHeight - yPadding);
			return CalculateResourceHeaderAutoSize(preliminaryResult, xPadding, yPadding);
		}
	}
}
