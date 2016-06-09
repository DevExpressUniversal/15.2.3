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
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet;
#if SL || WPF
using DevExpress.Xpf.Spreadsheet.Views;
#else
using DevExpress.LookAndFeel;
#endif
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region SpreadsheetControl
	public partial class SpreadsheetControl {
		#region ActiveView
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SpreadsheetView ActiveView { get { return InnerControl != null ? InnerControl.ActiveView : null; } }
		#endregion
		SpreadsheetViewRepository IInnerSpreadsheetControlOwner.CreateViewRepository() {
			return this.CreateViewRepository();
		}
		protected internal virtual SpreadsheetViewRepository CreateViewRepository() {
#if SL || WPF
			return new XpfSpreadsheetViewRepository(this);
#else
			return new WinFormsSpreadsheetViewRepository(this);
#endif
		}
		void IInnerSpreadsheetControlOwner.ActivateViewPlatformSpecific(SpreadsheetView view) {
			this.ActivateViewPlatformSpecific(view);
		}
		protected internal virtual void ActivateViewPlatformSpecific(SpreadsheetView view) {
			CreateBackgroundPainter(view);
			CreateViewPainter(view);
		}
		void IInnerSpreadsheetControlOwner.DeactivateViewPlatformSpecific(SpreadsheetView view) {
			this.DeactivateViewPlatformSpecific(view);
		}
		protected internal virtual void DeactivateViewPlatformSpecific(SpreadsheetView view) {
			DisposeBackgroundPainter();
			DisposeViewPainter();
		}
	}
	#endregion
}
