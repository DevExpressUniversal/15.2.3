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

using System.Windows;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Utils;
namespace DevExpress.Xpf.PropertyGrid {
	public class RenderResizingPanel : RenderPanel {
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderResizingPanelContext(this);
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
			((RenderResizingPanelContext)context).Attach();
		}
		FrameworkRenderElementContext GetHeader(IFrameworkRenderElementContext context) {
			return context.GetRenderChild(0) as FrameworkRenderElementContext;
		}
		FrameworkRenderElementContext GetSeparator(IFrameworkRenderElementContext context) {
			return context.GetRenderChild(1) as FrameworkRenderElementContext;
		}
		FrameworkRenderElementContext GetContent(IFrameworkRenderElementContext context) {
			return context.GetRenderChild(2) as FrameworkRenderElementContext;
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			RenderResizingPanelContext pContext = (RenderResizingPanelContext)context;
			var rowControl = pContext.RowControl;
			if (rowControl == null)
				return base.MeasureOverride(availableSize, context);
			HeaderShowMode hsm = rowControl.HeaderShowMode;
			var sizeAdjusted = rowControl.ResizingStrategy.AdjustAvailableSize(ref availableSize);
			var header = GetHeader(context);
			var separator = GetSeparator(context);
			var content = GetContent(context);
			Size result;
			do {
				if (hsm == HeaderShowMode.OnlyHeader) {
					header.Measure(availableSize);
					separator.Measure(new Size());
					content.Measure(new Size());
					result = header.DesiredSize;
					break;
				}
				if(hsm == HeaderShowMode.Left) {
					double headerPercent = rowControl.ResizingStrategy.HeaderPercent;
					double contentPercent = 1 - headerPercent;
					double separatorWidth = Math.Min(availableSize.Width, separator.DesiredSize.Width);
					separator.Measure(new Size(double.PositiveInfinity, availableSize.Height));
					var restWidth = availableSize.Width - separatorWidth;
					header.Measure(new Size(restWidth * headerPercent, availableSize.Height));
					content.Measure(new Size(restWidth * contentPercent, availableSize.Height));
					result = new Size(header.DesiredSize.Width + content.DesiredSize.Width + separatorWidth, Math.Max(header.DesiredSize.Height, content.DesiredSize.Height));
					break;
				}				
				if(hsm == HeaderShowMode.Hidden) {
					content.Measure(availableSize);
					separator.Measure(new Size());
					header.Measure(new Size());
					result = content.DesiredSize;
					break;
				}
				if(hsm == HeaderShowMode.Top) {
					var mSize = new Size(availableSize.Width, double.PositiveInfinity);
					header.Measure(mSize);
					separator.Measure(mSize);
					content.Measure(mSize);
					result = new Size(Math.Max(header.DesiredSize.Width, content.DesiredSize.Width), header.DesiredSize.Height + content.DesiredSize.Height + separator.DesiredSize.Height);
					break;
				}
				result = new Size();
			}
			while (false);
			if (sizeAdjusted)
				result.Width = availableSize.Width;
			return result;
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			RenderResizingPanelContext pContext = (RenderResizingPanelContext)context;
			var rowControl = pContext.RowControl;
			if (rowControl == null)
				return base.ArrangeOverride(finalSize, context);
			HeaderShowMode hsm = rowControl.HeaderShowMode;
			var header = GetHeader(context);
			var separator = GetSeparator(context);
			var content = GetContent(context);
			if (hsm == HeaderShowMode.OnlyHeader) {
				header.Arrange(new Rect(new Point(), finalSize));
				separator.Arrange(new Rect());
				content.Arrange(new Rect());
				return finalSize;
			}
			if (hsm == HeaderShowMode.Left) {
				double headerPercent = rowControl.ResizingStrategy.HeaderPercent;
				double contentPercent = 1 - headerPercent;
				double separatorWidth = Math.Min(finalSize.Width, separator.DesiredSize.Width);
				var restWidth = finalSize.Width - separatorWidth;
				header.Arrange(new Rect(new Point(0, 0), new Size(restWidth * headerPercent, finalSize.Height)));
				separator.Arrange(new Rect(new Point(restWidth * headerPercent, 0), new Size(separatorWidth, finalSize.Height)));
				content.Arrange(new Rect(new Point(restWidth * headerPercent + separatorWidth, 0), new Size(restWidth * contentPercent, finalSize.Height)));
				return finalSize;
			}
			if(hsm == HeaderShowMode.Hidden) {
				content.Arrange(new Rect(new Point(), finalSize));
				separator.Arrange(new Rect());
				header.Arrange(new Rect());
				return finalSize;
			}
			if (hsm == HeaderShowMode.Top) {
				double top = 0d;
				header.Arrange(new Rect(new Point(), new Size(finalSize.Width, header.DesiredSize.Height)));
				top = header.DesiredSize.Height;
				separator.Arrange(new Rect(new Point(0, top), new Size(finalSize.Width, separator.DesiredSize.Height)));
				top += separator.DesiredSize.Height;
				content.Arrange(new Rect(new Point(0, top), new Size(finalSize.Width, Math.Max(0, finalSize.Height - top))));
				return finalSize;
			}
			return new Size();
		}
	}
	public class RenderResizingPanelContext : RenderPanelContext {		
		public RowControl RowControl { get; private set; }
		public RenderResizingPanelContext(RenderResizingPanel factory) : base(factory) { }
		public virtual void Attach() {
			RowControl = ElementHost.Parent as RowControl ?? ElementHost.Parent.With(PropertyGridHelper.GetRowControl) as RowControl;
			RowControl.HeaderPercentChanged += OnHeaderPercentChanged;
		}		
		public override void Release() {
			if (RowControl != null) {
				RowControl.HeaderPercentChanged -= OnHeaderPercentChanged;
				RowControl = null;
			}
			base.Release();
		}
		void OnHeaderPercentChanged(object sender, EventArgs e) {
			UpdateLayout();
		}
	}
}
