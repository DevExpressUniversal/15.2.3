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
using DevExpress.LookAndFeel;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class SimpleControl : Control, ISupportLookAndFeel {
		UserLookAndFeel lookAndFeel;
		BorderStyles borderStyle;
		public SimpleControl() {
			this.borderStyle = DefaultBorderStyle;
			this.lookAndFeel = null;
			CreateLookAndFeel();
		}
		protected virtual BorderStyles DefaultBorderStyle { get { return BorderStyles.Default; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(LookAndFeel != null) {
					LookAndFeel.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void CreateLookAndFeel() {
			lookAndFeel = new ControlUserLookAndFeel(this);
			lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		protected internal virtual void LayoutChanged() {
			Invalidate();
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				OnPropertiesChanged();
			}
		}
		protected internal bool shouldRaiseSizeableChanged = true;
		protected internal virtual void OnPropertiesChanged() {
			LayoutChanged();
			shouldRaiseSizeableChanged = false;
		}
	}
}
