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

using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Linq;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Printing {
	[DXToolboxBrowsable(false)]
	public class DocumentPreview : DocumentPreviewBase {
		#region Fields and Properties
#if SILVERLIGHT
		public static readonly DependencyProperty AutoCreateDocumentProperty = DependencyPropertyManager.Register(
			"AutoCreateDocument",
			typeof(bool),
			typeof(DocumentPreview),
			new PropertyMetadata(false));
#endif
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register(
			"Model",
			typeof(IDocumentPreviewModel),
			typeof(DocumentPreview),
			new PropertyMetadata(null));
		private static readonly DependencyPropertyKey DocumentViewerPropertyKey = DependencyPropertyManager.RegisterReadOnly(
			"DocumentViewer",
			typeof(DocumentViewer),
			typeof(DocumentPreview),
			new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty DocumentViewerProperty = DocumentViewerPropertyKey.DependencyProperty;
#if SILVERLIGHT
		public bool AutoCreateDocument {
			get { return (bool)GetValue(AutoCreateDocumentProperty); }
			set { SetValue(AutoCreateDocumentProperty, value); }
		}
#endif
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("DocumentPreviewModel")]
#endif
		public override IDocumentPreviewModel Model {
			get { return (IDocumentPreviewModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("DocumentPreviewDocumentViewer")]
#endif
		public override DocumentViewer DocumentViewer {
			get { return (DocumentViewer)GetValue(DocumentViewerProperty); }
			protected set { this.SetValue(DocumentViewerPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("RibbonDocumentPreviewBarManager")]
#endif
		public BarManager BarManager { get { return (BarManager)this.GetTemplateChild("barManager"); } }
		#endregion
		#region Constructors
		static DocumentPreview() {
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentPreview), new FrameworkPropertyMetadata(typeof(DocumentPreview)));
#endif
			BarManager.CheckBarItemNames = false;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DocumentViewer = GetTemplateChild("DocumentViewer") as DocumentViewer;
			DocumentPreviewBarManagerController controller;
			if(BarManager != null && (controller = BarManager.Controllers.OfType<DocumentPreviewBarManagerController>().FirstOrDefault()) != null) {
				BindingOperations.SetBinding(
				controller,
				DocumentPreviewBarManagerController.DocumentViewerProperty,
				new Binding(DocumentViewerProperty.GetName()) { Source = this, Mode = BindingMode.OneWay });
				if(BarManagerCustomization.GetTemplate(this) != null)
					BarManagerCustomization.ApplyTemplate(this);
			}
		}
		public DocumentPreview() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(DocumentPreview);
#endif
		}
		#endregion
		#region Methods
#if SILVERLIGHT
#endif
		#endregion
	}
}
