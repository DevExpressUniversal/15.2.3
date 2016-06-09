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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Utils;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
#if !SL
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public class BorderRenderer {
		BaseEdit edit;
		public BorderRenderer(BaseEdit edit) {
			this.edit = edit;
		}
		protected BaseEdit Edit { get { return edit; } }
		protected Thickness BorderThickness { get { return Edit.BorderThickness; } }
		protected Brush BorderBrush { get { return Edit.BorderBrush; } }
		protected Size RenderSize { get { return Edit.RenderSize; } }
		protected FrameworkElement EditCore { get { return Edit.EditCore; } }
		public virtual bool CanRenderBorder { get { return Edit.IsPrintingMode && EditCore != null ; } }
		protected bool IsThicknessNotEmpty(double value) { return value > 0.0; }
#if !SL
		public void Render(DrawingContext drawingContext) {
			if(!CanRenderBorder) return;
			if(IsThicknessNotEmpty(BorderThickness.Left))
				RenderBorderLine(drawingContext, RenderLeftBorder, BorderThickness.Left);
			if(IsThicknessNotEmpty(BorderThickness.Right))
				RenderBorderLine(drawingContext, RenderRightBorder, BorderThickness.Right);
			if(IsThicknessNotEmpty(BorderThickness.Top))
				RenderBorderLine(drawingContext, RenderTopBorder, BorderThickness.Top);
			if(IsThicknessNotEmpty(BorderThickness.Bottom))
				RenderBorderLine(drawingContext, RenderBottomBorder, BorderThickness.Bottom);
			if(Edit.Background != null)
				drawingContext.DrawRectangle(Edit.Background, null, DeflateRect(new Rect(RenderSize), BorderThickness));
		}
		void RenderLeftBorder(DrawingContext context, Pen pen, double lineThickness) {
			context.DrawLine(pen, new Point(lineThickness, 0.0), new Point(lineThickness, RenderSize.Height));
		}
		void RenderRightBorder(DrawingContext context, Pen pen, double lineThickness) {
			context.DrawLine(pen, new Point(RenderSize.Width - lineThickness, 0.0), new Point(RenderSize.Width - lineThickness, RenderSize.Height));
		}
		void RenderTopBorder(DrawingContext context, Pen pen, double lineThickness) {
			context.DrawLine(pen, new Point(0.0, lineThickness), new Point(RenderSize.Width, lineThickness));
		}
		void RenderBottomBorder(DrawingContext context, Pen pen, double lineThickness) {
			context.DrawLine(pen, new Point(0.0, RenderSize.Height - lineThickness), new Point(RenderSize.Width, RenderSize.Height - lineThickness));
		}
		protected void RenderBorderLine(DrawingContext context, RenderLineDelegate renderLinkDelegate, double penThickness) {
			Pen borderPen = new Pen() { Brush = BorderBrush };
			borderPen.Thickness = penThickness;
			renderLinkDelegate(context, borderPen, borderPen.Thickness * 0.5);
		}
		protected delegate void RenderLineDelegate(DrawingContext context, Pen pen, double lineThickness);
#endif
		public Size MeasureOverride(Size constraint) {
			if(!CanRenderBorder) return constraint;
			UIElement child = EditCore;
			Size result = new Size();
			Size thicknessSize = CollapseThickness(BorderThickness);
			Size paddingSize = new Size(0, 0); 
			if(child != null) {
				Size size = new Size(thicknessSize.Width + paddingSize.Width, thicknessSize.Height + paddingSize.Height);
				Size childConstraint = new Size(Math.Max(0.0, (constraint.Width - size.Width)), Math.Max(0.0, (constraint.Height - size.Height)));
				child.Measure(childConstraint);
				Size desiredSize = child.DesiredSize;
				result.Width = desiredSize.Width + size.Width;
				result.Height = desiredSize.Height + size.Height;
				return result;
			}
			return constraint;
		}
		public Size ArrangeOverride(Size arrangeSize) {
			if(!CanRenderBorder) return arrangeSize;
			UIElement child = EditCore;
			Rect rt = new Rect(new Point(0, 0), arrangeSize);
			Rect rect = DeflateRect(rt, BorderThickness);
			if(child != null) {
				Rect finalRect = rect; 
				child.Arrange(finalRect);
			}
			return arrangeSize;
		}
		protected Rect DeflateRect(Rect rt, Thickness thick) {
			return new Rect(rt.Left + thick.Left, rt.Top + thick.Top, Math.Max(0.0, ((rt.Width - thick.Left) - thick.Right)), Math.Max(0.0, ((rt.Height - thick.Top) - thick.Bottom)));
		}
		protected Size CollapseThickness(Thickness th) {
			return new Size(th.Left + th.Right, th.Top + th.Bottom);
		}
	}
}
