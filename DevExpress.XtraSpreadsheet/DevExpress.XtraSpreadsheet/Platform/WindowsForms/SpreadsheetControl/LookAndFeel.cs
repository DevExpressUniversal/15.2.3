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
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl : ISupportLookAndFeel {
		UserLookAndFeel lookAndFeel;
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlLookAndFeel"),
#endif
 Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
		#region SubscribeLookAndFeelEvents
		protected internal virtual void SubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged += OnLookAndFeelChanged;
		}
		#endregion
		#region UnsubscribeLookAndFeelEvents
		protected internal virtual void UnsubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged -= OnLookAndFeelChanged;
		}
		#endregion
		protected internal virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			if (!IsHandleCreated)
				return;
			ForceHandleLookAndFeelChanged();
			Redraw();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			if (!IsHandleCreated)
				return;
			ForceHandleLookAndFeelChanged();
			Redraw();
		}
		protected internal virtual void ForceHandleLookAndFeelChanged() {
			RecreateBackgroundPainter(ActiveView);
			RecreateViewPainter(ActiveView);
			OnResizeCore();
		}
		void DisposeLookAndFeel() {
			if (lookAndFeel != null) {
				UnsubscribeLookAndFeelEvents();
				lookAndFeel.Dispose();
				lookAndFeel = null;
			}
		}
	}
}
