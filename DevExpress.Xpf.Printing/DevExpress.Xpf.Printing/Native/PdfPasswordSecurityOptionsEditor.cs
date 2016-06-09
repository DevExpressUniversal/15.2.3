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
using DevExpress.Xpf.Utils.Themes;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System;
#if !SILVERLIGHT
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class PdfPasswordSecurityOptionsEditor : IDXTypeEditor {
#if !SILVERLIGHT
		public void Edit(object value, Window ownerWindow) {
#else
		public void Edit(object value) {
#endif
			PdfPasswordSecurityOptions options = (PdfPasswordSecurityOptions)value;
			PdfPasswordSecurityOptionsView view = new PdfPasswordSecurityOptionsView();
#if !SILVERLIGHT
			if(ownerWindow != null) {
				view.Owner = ownerWindow;
				view.FlowDirection = ownerWindow.FlowDirection;
			}
#endif
			PdfPasswordSecurityOptionsPresenter presenter = new PdfPasswordSecurityOptionsPresenter(options, view);
			presenter.Initialize();
			view.ShowDialog();
#if SILVERLIGHT
			view.Closed += (o, e) => OnEditComplete();
#endif
		}
#if SILVERLIGHT
		public event EventHandler EditComplete;
		void OnEditComplete() {
			if(EditComplete != null)
				EditComplete(this, EventArgs.Empty);
		}
#endif
	}
}
