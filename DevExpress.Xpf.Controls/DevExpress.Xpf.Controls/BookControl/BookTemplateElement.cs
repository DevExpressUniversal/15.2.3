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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Controls.Internal {
	public class BookTemplateElement {
		#region Property
		protected internal FrameworkElement Element { get; set; }
		protected internal ContentControl Content { get { return (ContentControl)Element; } }
		#endregion
		public BookTemplateElement() { }
		public BookTemplateElement(FrameworkElement element) { Element = element; }
		public virtual double GetWidth() { return Element.ActualWidth; }
		public virtual double GetHeight() { return Element.ActualHeight; }
		public virtual double GetLeft() { return Element.GetLeft(); }
		public virtual double GetTop() { return Element.GetTop(); }
		public virtual void SetHeight(double value) { Element.Height = value; }
		public virtual void SetTop(double value) { Element.SetTop(value); }
		public virtual void SetVisible(bool value) { Element.SetVisible(value); }
		public virtual void SetZIndex(int value) { Element.SetZIndex(value); }
		public virtual void SetColumn(int value) { System.Windows.Controls.Grid.SetColumn(Element, value); }
		public virtual void SetClip(Geometry value) { Element.Clip = value; }
		public virtual void SetTransform(Transform value) { Element.RenderTransform = value; }
		public virtual void SetContent(object value) { (value as DependencyObject).Do(x => Bars.BarNameScope.SetIsScopeOwner(x, true)); Content.Content = value; }
		public virtual void SetTemplate(DataTemplate value) { Content.ContentTemplate = value; }
		public virtual void SetHorizontalContentAlignment(HorizontalAlignment value) { Content.HorizontalContentAlignment = value; }
		public virtual void SetVerticalContentAlignment(VerticalAlignment value) { Content.VerticalContentAlignment = value; }
	}
	public class BookShadowTemplateElement : BookTemplateElement {
		public BookShadowTemplateElement() {
		}
		public BookShadowTemplateElement(FrameworkElement element)
			: base(element) {
		}
		bool isVisible;
		double height;
		public override void SetHeight(double value) {
			height = value;
			if(isVisible) base.SetHeight(height);
		}
		public override void SetVisible(bool value) {
			isVisible = value;
			base.SetHeight(isVisible ? height : 0);
		}
	}
	public class NullBookTemplateElement : BookTemplateElement {
		public override double GetWidth() { return 0.0; }
		public override double GetHeight() { return 0.0; }
		public override double GetLeft() { return 0.0; }
		public override double GetTop() { return 0.0; }
		public override void SetHeight(double value) { }
		public override void SetTop(double value) { }
		public override void SetVisible(bool value) { }
		public override void SetZIndex(int value) { }
		public override void SetColumn(int value) { }
		public override void SetClip(Geometry value) { }
		public override void SetTransform(Transform value) { }
		public override void SetContent(object value) { }
		public override void SetTemplate(DataTemplate value) { }
		public override void SetHorizontalContentAlignment(HorizontalAlignment value) { }
		public override void SetVerticalContentAlignment(VerticalAlignment value) { }
	}
}
