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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.Native.Lines {
	[ToolboxItem(false),]
	public abstract class BaseLine : System.Windows.Forms.Control, ILine {
		#region static
		static protected Size glyphSize = new Size(13, 19);
		static protected Size MeasureString(string text, Font font) {
			return Size.Round(XtraPrinting.Native.Measurement.MeasureString(text + "w", font, GraphicsUnit.Pixel));
		}
		#endregion
		bool disableUpdatingValue;
		UserLookAndFeel lookAndFeel;
		LinesContainer fContainer;
		protected internal LinesContainer LinesContainer { get { return fContainer; } set { fContainer = value; } }
		public virtual bool HasError { get { return false; } }
		public bool DisableUpdatingValue {
			get { return disableUpdatingValue; }
			set { disableUpdatingValue = value; }
		}
		protected UserLookAndFeel UserLookAndFeel { get { return lookAndFeel; } }
		public BaseLine() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, false);
			BackColor = Color.Transparent;
			ForeColor = Color.Transparent;
		}
		public virtual void Init(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			Initialize();
			Size = GetLineSize();
			Controls.AddRange(GetControls());
		}
		public virtual System.Windows.Forms.Control GetPopupWindow() {
			return null;
		}
		public virtual Size GetLineSize() {
			return Size;
		}
		protected virtual void Initialize() {
			RefreshProperty();
		}
		protected virtual System.Windows.Forms.Control[] GetControls() {
			return new System.Windows.Forms.Control[0];
		}
		public virtual void RefreshProperty() {
		}
		public virtual void CommitChanges() {
		}
		protected override void OnLeave(EventArgs e) {
			CommitChanges();
		}
		protected void SetParentLookAndFeel(ISupportLookAndFeel lookAndFeelObject) {
			SuspendLayout();
			lookAndFeelObject.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			ResumeLayout(false);
		}
		public abstract void SetText(string text);
	}
}
