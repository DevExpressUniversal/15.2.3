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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraEditors.Drawing;
using Imaging = System.Drawing.Imaging;
using DevExpress.Skins;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet {
	[DXToolboxItem(false)]
	public partial class ReferenceEditControl : ButtonEdit, IReferenceEditControl, IReferenceEditControllerOwner {
		const string ImageCollapsed = "ReferenceEditControl_Collapsed_16x16.png";
		private const string ImageExpand = "ReferenceEditControl_Expand_16x16.png";
		ReferenceEditController controller;
		bool activated;
		List<CellRange> lastSelection;
		public ReferenceEditControl() {
			InitializeComponent();
			this.lastSelection = new List<CellRange>();
			this.ButtonClick += OnButtonClick;
		}	  
		#region Properties
		protected internal ReferenceEditController Controller { get { return controller; } }
		public ISpreadsheetControl SpreadsheetControl {
			get { return Controller != null ? Controller.Spreadsheet : null; }
			set {
				if (this.SpreadsheetControl == value)
					return;
				UnsubscribeControllerEvents();
				this.controller = value != null ? CreateController(value) : null;
				ApplyControllerProperties();
				SubscribeControllerEvents();
				UpdateSkinImage();
			}
		}
		public bool Activated {
			get { return activated; }
			set {
				if (activated == value)
					return;
				activated = value;
				RaiseActivatedChanged();
			}
		}
		public List<CellRange> Selection { get { return lastSelection; } }
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				this.Refresh();
			}
		}
		public PositionType PositionType {
			get { return InnerPositionType; }
			set {
				InnerPositionType = value;
				ApplyControllerProperties();
			}
		}
		public string EditValuePrefix {
			get { return InnerEditValuePrefix; }
			set {
				InnerEditValuePrefix = value;
				ApplyControllerProperties();
			}
		}
		public bool IncludeSheetName {
			get { return InnerIncludeSheetName; }
			set {
				InnerIncludeSheetName = value;
				ApplyControllerProperties();
			}
		}
		public bool SuppressActiveSheetChanging {
			get { return InnerSuppressActiveSheetChanging; }
			set {
				InnerSuppressActiveSheetChanging = value;
				ApplyControllerProperties();
			}
		}
		internal PositionType InnerPositionType { get; set; }
		internal string InnerEditValuePrefix { get; set; }
		internal bool InnerIncludeSheetName { get; set; }
		internal bool InnerSuppressActiveSheetChanging { get; set; }
		#endregion
		void ApplyControllerProperties() {
			if (Controller == null)
				return;
			Controller.PositionType = this.InnerPositionType;
			Controller.EditValuePrefix = this.InnerEditValuePrefix;
			Controller.IncludeSheetName = this.InnerIncludeSheetName;
			Controller.SuppressActiveSheetChanging = this.InnerSuppressActiveSheetChanging;
		}
		void SubscribeControllerEvents() {
			if (Controller == null)
				return;
			Controller.TextChanged += OnControllerTextChanged;
			Controller.SelectionChanged += OnControllerSelectionChanged;
			Controller.CollapsedChanged += OnControllerCollapsedChanged;
		}
		void UnsubscribeControllerEvents() {
			if (Controller == null)
				return;
			Controller.TextChanged -= OnControllerTextChanged;
			Controller.SelectionChanged -= OnControllerSelectionChanged;
			Controller.CollapsedChanged -= OnControllerCollapsedChanged;
		}
		void OnButtonClick(object sender, ButtonPressedEventArgs e) {
			RaiseCollapseButtonClick();
		}
		void OnControllerCollapsedChanged(object sender, EventArgs e) {
			UpdateSkinImage();
			RaiseCollapsedChanged();
		}
		void OnControllerSelectionChanged(object sender, ReferenceEditSelectionChangedEventArgs e) {
			Selection.Clear();
			Selection.AddRange(e.ReferenceEditSelection);
		}
		void OnControllerTextChanged(object sender, TextChangedEventArgs e) {
			this.Text = e.Text;
		}
		protected internal virtual ReferenceEditController CreateController(ISpreadsheetControl spreadsheetControl) {
			return new ReferenceEditController(this, spreadsheetControl);
		}
		public void UpdateSkinImage() {
			if (Properties.Buttons.Count < 1)
				return;
			string imagePath = Controller.IsCollapsed ? ImageExpand : ImageCollapsed;
			Bitmap buttonImage = (Bitmap)ResourceImageHelper.CreateImageFromResources("DevExpress.XtraSpreadsheet.Images." + imagePath, GetType().Assembly);
			Bitmap coloredButtonImage = SkinImageDecorator.CreateImageColoredToButtonForeColor(buttonImage, this.LookAndFeel);
			Properties.Buttons[0].Image = coloredButtonImage;
			Properties.Buttons[0].ImageLocation = ImageLocation.MiddleCenter;
			Properties.Buttons[0].Kind = ButtonPredefines.Glyph;
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			UpdateSkinImage();
		}
		#region Dispose
		protected override void Dispose(bool disposing) {
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				base.Dispose(disposing);
			}
		}
		void DisposeCore() {
			if (Controller == null)
				return;
			UnsubscribeControllerEvents();
			Controller.Dispose();
			controller = null;
		}
		#endregion
	}
}
