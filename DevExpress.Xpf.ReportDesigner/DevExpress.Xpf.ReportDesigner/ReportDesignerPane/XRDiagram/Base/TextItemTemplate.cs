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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class XRDiagramTextItem : XRDiagramItem {
		protected class XRDiagramTextItemController : XRDiagramItemController {
			readonly IDiagramItem item;
			public XRDiagramTextItemController(IDiagramItem item) : base(item) {
				this.item = item;
			}
			public override IFontTraits GetFontTraits() {
				return TextProperty == null ? null : new FontTraits(item);
			}
			public override PropertyDescriptor GetTextProperty() {
				return DependencyPropertyDescriptor.FromProperty(TextProperty, Item.GetType());
			}
		}
		public static readonly DependencyProperty TextProperty;
		static XRDiagramTextItem() {
			DependencyPropertyRegistrator<XRDiagramTextItem>.New()
				.Register(d => d.Text, out TextProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		protected sealed override XRDiagramItemControllerBase CreateXRItemController() {
			return CreateXRTextItemController();
		}
		protected virtual XRDiagramTextItemController CreateXRTextItemController() {
			return new XRDiagramTextItemController(this);
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
	public class XRDiagramTextContainer : XRDiagramContainer {
		protected class XRDiagramTextContainerController : XRDiagramContainerController {
			readonly IDiagramItem item;
			public XRDiagramTextContainerController(IDiagramContainer item) : base(item) {
				this.item = item;
			}
			public override IFontTraits GetFontTraits() {
				return TextProperty == null ? null : new FontTraits(item);
			}
			public override PropertyDescriptor GetTextProperty() {
				return DependencyPropertyDescriptor.FromProperty(TextProperty, Item.GetType());
			}
		}
		public static readonly DependencyProperty TextProperty;
		static XRDiagramTextContainer() {
			DependencyPropertyRegistrator<XRDiagramTextContainer>.New()
				.Register(d => d.Text, out TextProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		protected sealed override XRDiagramContainerControllerBase CreateXRContainerController() {
			return CreateXRTextContainerController();
		}
		protected virtual XRDiagramTextContainerController CreateXRTextContainerController() {
			return new XRDiagramTextContainerController(this);
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
