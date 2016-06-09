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
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public abstract class ReportDesignerPreview {
		public static readonly DependencyProperty DocumentProperty;
		static ReportDesignerPreview() {
			DependencyPropertyRegistrator<ReportDesignerPreview>.New()
				.RegisterAttached((DependencyObject d) => GetDocument(d), out DocumentProperty, null, OnDocumentChanged)
			;
		}
		public static ReportDesignerDocument GetDocument(DependencyObject preview) { return (ReportDesignerDocument)preview.GetValue(DocumentProperty); }
		public static void SetDocument(DependencyObject preview, ReportDesignerDocument document) { preview.SetValue(DocumentProperty, document); }
		static void OnDocumentChanged(DependencyObject preview, DependencyPropertyChangedEventArgs e) {
			var oldDocument = (ReportDesignerDocument)e.OldValue;
			var newDocument = (ReportDesignerDocument)e.NewValue;
			IReportDesignerDocument oldNativeDocument = oldDocument;
			IReportDesignerDocument newNativeDocument = newDocument;
			if(oldDocument != null)
				oldNativeDocument.Preview = null;
			if(newDocument != null)
				newNativeDocument.Preview = preview;
			ReportDesignerDocumentView.SetDocument(preview, newDocument);
		}
	}
}
