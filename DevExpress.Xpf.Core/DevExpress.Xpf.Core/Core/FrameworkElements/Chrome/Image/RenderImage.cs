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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public class RenderImage : FrameworkRenderElement {
		ImageSource source;
		Stretch stretch;
		StretchDirection stretchDirection;
		public ImageSource Source {
			get { return source; }
			set { SetProperty(ref source, value); }
		}
		public Stretch Stretch {
			get { return stretch; }
			set { SetProperty(ref stretch, value); }
		}
		public StretchDirection StretchDirection {
			get { return stretchDirection; }
			set { SetProperty(ref stretchDirection, value); }
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderImageContext(this);
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			return MeasureInternal(availableSize, (RenderImageContext)context);
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			return finalSize;
		}
		protected override void RenderOverride(DrawingContext dc, IFrameworkRenderElementContext context) {
			base.RenderOverride(dc, context);
			var imageContext = (RenderImageContext)context;
			var imageSource = imageContext.Source ?? Source;
			if (imageSource == null)
				return;
			dc.DrawImage(imageSource, new Rect(new Point(), context.RenderRect.Size));
		}
		Size MeasureInternal(Size inputSize, RenderImageContext context) {
			ImageSource source = context.Source ?? Source;
			Stretch actStretch = context.Stretch.HasValue ? context.Stretch.Value : Stretch;
			StretchDirection actStretchDirection = context.StretchDirection.HasValue ? context.StretchDirection.Value : StretchDirection;
			Size contentSize = new Size();
			if (source == null)
				return contentSize;
			contentSize = (Size)RenderTriggerHelper.GetValue(source, "Size", BindingFlags.NonPublic | BindingFlags.Instance);
			var computeScaleFactorHandler = RenderTriggerHelper.Helper.GetStaticMethodHandler<Func<Size, Size, Stretch, StretchDirection, Size>>(typeof(Viewbox), "ComputeScaleFactor", BindingFlags.NonPublic | BindingFlags.Static);
			Size scaleFactor = computeScaleFactorHandler(inputSize, contentSize, actStretch, actStretchDirection);
			return new Size(contentSize.Width * scaleFactor.Width, contentSize.Height * scaleFactor.Height);
		}
	}
}
