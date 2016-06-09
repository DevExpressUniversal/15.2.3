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

using System.Reflection;
using DevExpress.Xpf.Core.Internal;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public abstract class RenderControlBase : FrameworkRenderElement {
		protected abstract FrameworkElement CreateFrameworkElement(FrameworkRenderElementContext context);
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			var controlContext = (RenderControlBaseContext)context;
			controlContext.Control = CreateFrameworkElement(context);
			controlContext.UpdateControlForeground();
			base.InitializeContext(context);
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			RenderControlBaseContext controlContext = (RenderControlBaseContext)context;
			var control = controlContext.Control;
			if (control == null)
				return new Size();
			control.Measure(availableSize);
			return control.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			RenderControlBaseContext controlContext = (RenderControlBaseContext)context;
			var control = controlContext.Control;
			if (control == null)
				return finalSize;
			if (!Equals(control.RenderSize, finalSize))
				controlContext.UpdateLayout(FREInvalidateOptions.AffectsVisual);
			control.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			return finalSize;
		}
		protected override void RenderOverride(DrawingContext dc, IFrameworkRenderElementContext context) {
			base.RenderOverride(dc, context);
			var freContext = (RenderControlBaseContext)context;
			var transform = freContext.GeneralTransform;
			var control = freContext.Control;
			if (control == null)
				return;
			var vto = control as IVisualTransformOwner;
			if (vto != null) 
				vto.VisualTransform = transform;
			else 
				RenderTriggerHelper.SetValue(control, "VisualTransform", transform, BindingFlags.Instance | BindingFlags.NonPublic);
		}
	}
}
