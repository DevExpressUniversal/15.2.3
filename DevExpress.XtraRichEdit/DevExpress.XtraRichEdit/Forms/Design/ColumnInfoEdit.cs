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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Design {
	[DXToolboxItem(false)]
	public partial class ColumnInfoEdit : XtraUserControl {
		#region Fields
		ColumnInfoUI columnInfo;
		bool allowWidth = true;
		bool allowSpacing = true;
		#endregion
		public ColumnInfoEdit() {
			InitializeComponent();
		}
		public ColumnInfoEdit(ColumnInfoUI columnInfo, DocumentUnit defaultUnitType, DocumentModelUnitConverter valueUnitConverter) {
			Guard.ArgumentNotNull(columnInfo, "columnInfo");
			Guard.ArgumentNotNull(valueUnitConverter, "valueUnitConverter");
			this.columnInfo = columnInfo;
			InitializeComponent();
			this.edtWidth.Properties.DefaultUnitType = defaultUnitType;
			this.edtSpacing.Properties.DefaultUnitType = defaultUnitType;
			UpdateForm();
		}
		#region Properties
		protected override System.Windows.Forms.CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		[DefaultValue(true)]
		public bool AllowWidth {
			get { return allowWidth; }
			set {
				allowWidth = value;
				edtWidth.Properties.ReadOnly = !value;
			}
		}
		public bool AllowSpacing {
			get { return allowSpacing; }
			set {
				allowSpacing = value;
				edtSpacing.Properties.ReadOnly = !value;
			}
		}
		public ColumnInfoUI ColumnInfo {
			get { return columnInfo; }
			set {
				columnInfo = value;
				UpdateForm();
			}
		}
		#endregion
		#region Events
		#region WidthChanged
		EventHandler onWidthChanged;
		public event EventHandler WidthChanged { add { onWidthChanged += value; } remove { onWidthChanged -= value; } }
		protected internal virtual void RaiseWidthChanged() {
			if (onWidthChanged != null)
				onWidthChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SpacingChanged
		EventHandler onSpacingChanged;
		public event EventHandler SpacingChanged { add { onSpacingChanged += value; } remove { onSpacingChanged -= value; } }
		protected internal virtual void RaiseSpacingChanged() {
			if (onSpacingChanged != null)
				onSpacingChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			edtWidth.Value = ColumnInfo.Width;
			edtSpacing.Value = ColumnInfo.Spacing;
			edtIndex.EditValue = String.Format("{0}:", ColumnInfo.Number);
		}
		protected internal virtual void SubscribeControlsEvents() {
			edtWidth.ValueChanged += OnWidthChanged;
			edtSpacing.ValueChanged += OnSpacingChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			edtWidth.ValueChanged -= OnWidthChanged;
			edtSpacing.ValueChanged -= OnSpacingChanged;
		}
		void OnWidthChanged(object sender, EventArgs e) {
			ColumnInfo.Width = edtWidth.Value;
			RaiseWidthChanged();
		}
		void OnSpacingChanged(object sender, EventArgs e) {
			ColumnInfo.Spacing = edtSpacing.Value;
			RaiseSpacingChanged();
		}
	}
}
