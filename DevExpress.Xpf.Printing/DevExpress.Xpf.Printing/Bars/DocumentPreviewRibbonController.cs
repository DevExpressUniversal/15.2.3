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

using DevExpress.Xpf.Ribbon;
using System.Windows;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Printing {
	public class DocumentPreviewRibbonController : TemplatedRibbonController {
		public static readonly DependencyProperty DocumentViewerProperty = DependencyPropertyManager.Register(
						"DocumentViewer",
						typeof(DocumentViewer),
						typeof(DocumentPreviewRibbonController),
						new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentViewerPropertyChanged)));
		public DocumentViewer DocumentViewer {
			get { return (DocumentViewer)GetValue(DocumentViewerProperty); }
			set { SetValue(DocumentViewerProperty, value); }
		}
		static DocumentPreviewRibbonController() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentPreviewRibbonController), new FrameworkPropertyMetadata(typeof(DocumentPreviewRibbonController)));
		}
		public DocumentPreviewRibbonController() {
			ApplyTemplate();
		}
		static void OnDocumentViewerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			((DocumentPreviewRibbonController)sender).OnDocumentViewerChanged(e.NewValue);
		}
		void OnDocumentViewerChanged(object previewControl) {
			var ribbon = RibbonControl.GetRibbon(this);
			if (ribbon != null)
				ribbon.SetValue(DocumentViewer.DocumentViewerProperty, previewControl);
		}
	}
}
