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
namespace DevExpress.Xpf.Core.Native {
	public class RenderItemsControl : FrameworkRenderElement {
		public RenderTemplate ItemTemplate { get; set; }
		public RenderPanel ItemPanelTemplate { get; set; }
		protected override void PreApplyTemplate(FrameworkRenderElementContext context) {
			base.PreApplyTemplate(context);
			var icc = (RenderItemsControlContext)context;
			if (icc.ItemsHost == null)
				InitializeItemsHost(icc);
		}
		void InitializeItemsHost(RenderItemsControlContext context) {
			var panelTemplate = context.ItemPanelTemplate ?? ItemPanelTemplate ?? new RenderPanel();
			var panelContext = (RenderPanelContext)panelTemplate.CreateContext(context.Namescope, context.ElementHost);
			context.AddChild(panelContext);
			if (context.ItemsSource == null)
				return;
			foreach (var item in context.ItemsSource) {
				var itemTemplate = context.ItemTemplate ?? ItemTemplate ?? RenderTemplate.Default;
				RenderControl cc = CreateContainerForItemOverride();
				cc.RenderTemplate = itemTemplate;
				var ccContext = (RenderControlContext)cc.CreateContext(context.Namescope, context.ElementHost);
				PrepareContainerForItemOverride(ccContext, item);
				panelContext.AddChild(ccContext);
			}
		}
		protected virtual RenderControl CreateContainerForItemOverride() {
			return new RenderControl();
		}
		protected virtual void PrepareContainerForItemOverride(RenderControlContext container, object item) {
			container.DataContext = item;
		}
		protected virtual void ClearContainerForItemOverride(RenderControlContext container) {
			container.DataContext = null;
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderItemsControlContext(this);
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			return LayoutProvider.GridInstance.MeasureOverride(availableSize, context);
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			return LayoutProvider.GridInstance.ArrangeOverride(finalSize, context);
		}
	}
}
