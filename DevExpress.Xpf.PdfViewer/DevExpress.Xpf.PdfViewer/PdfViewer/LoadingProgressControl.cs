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

using DevExpress.Xpf.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.PdfViewer {
	public class LoadingProgressControl : Control {
		public static readonly DependencyProperty MessageProperty;
		public static readonly DependencyProperty ProgressProperty;
		public static readonly DependencyProperty TotalProgressProperty;
		static LoadingProgressControl() {
			Type ownerType = typeof(LoadingProgressControl);
			MessageProperty = DependencyPropertyManager.Register("Message", typeof(string), ownerType, 
				new FrameworkPropertyMetadata(PdfViewerLocalizer.GetString(PdfViewerStringId.LoadingDocumentCaption)));
			ProgressProperty = DependencyPropertyManager.Register("Progress", typeof(double), ownerType, 
				new FrameworkPropertyMetadata(0d));
			TotalProgressProperty = DependencyPropertyManager.Register("TotalProgress", typeof(double), ownerType, 
				new FrameworkPropertyMetadata(1d));
		}
		public LoadingProgressControl() {
			DefaultStyleKey = typeof(LoadingProgressControl);
		}
		public string Message {
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public double Progress {
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}
		public double TotalProgress {
			get { return (double)GetValue(TotalProgressProperty); }
			set { SetValue(TotalProgressProperty, value); }
		}
	}
}
