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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Forms.Design {
	[DXToolboxItem(false)]
	public partial class BorderShadingUserControl : UserControl, IBorderShadingUserControl {
		#region Fields
		static Image borderLineLeftImage;
		static Image borderLineRightImage;
		static Image borderLineVerticalInImage;
		static Image borderLineUpImage;
		static Image borderLineDownImage;
		static Image borderLineHorizontalInImage;
		private const string imageResourcePrefix = "DevExpress.XtraRichEdit.Images";
		private static readonly System.Reflection.Assembly imageResourceAssembly = typeof(IRichEditControl).Assembly;
		BorderInfo borderLineUp;
		BorderInfo borderLineDown;
		BorderInfo borderLineHorizontalIn;
		BorderInfo borderLineLeft;
		BorderInfo borderLineRight;
		BorderInfo borderLineVerticalIn;
		BorderInfo currentBorderInfo = new BorderInfo();
		BorderInfo inactiveBorderInfo = new BorderInfo();
		int updateCount;
		SetMode setMode;
		bool btnsVisible;
		bool borderLineHorizontalInVisible;
		bool borderLineVerticalInVisible;
		#endregion
		static BorderShadingUserControl() {
			borderLineLeftImage = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, "BorderLeft", imageResourceAssembly);
			borderLineRightImage = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, "BorderRight", imageResourceAssembly);
			borderLineVerticalInImage = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, "BorderInsideVertical", imageResourceAssembly);
			borderLineUpImage = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, "BorderTop", imageResourceAssembly);
			borderLineDownImage = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, "BorderBottom", imageResourceAssembly);
			borderLineHorizontalInImage = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, "BorderInsideHorizontal", imageResourceAssembly);
		} 
		public BorderShadingUserControl() {
			InitializeComponent();
			btnVerticalBorderLeft.Image = borderLineLeftImage;
			btnVerticalBorderRight.Image = borderLineRightImage;
			btnVerticalBorderIn.Image = borderLineVerticalInImage;
			btnHorizontBorderDown.Image = borderLineDownImage;
			btnHorizontBorderUp.Image = borderLineUpImage;
			btnHorizontBorderIn.Image = borderLineHorizontalInImage;
			inactiveBorderInfo.Style = BorderLineStyle.None;
			setMode = SetMode.Custom;
			previewBSUserControl.BorderInfoSource = this;
		}
		public void BeginSetProperties() {
			updateCount++;
		}
		public void EndSetProperties() {
			updateCount--;
		}
		#region Properties
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		public DocumentModel DocumentModel {
			get { return previewBSUserControl.DocumentModel; } 
			set { previewBSUserControl.DocumentModel = value; 
			} 
		}
		internal bool DrawColumns {
			get { return previewBSUserControl.DrawColumns; }
			set { previewBSUserControl.DrawColumns = value;
			}
		}
		internal bool DrawParagraph {
			get { return previewBSUserControl.DrawParagraph; }
			set { previewBSUserControl.DrawParagraph = value;
			}
		}
		bool InsideSetProperties { get { return updateCount > 0; } }
		public Color FillColor {
			get { return (previewBSUserControl.FillColor); }
			set {
				previewBSUserControl.FillColor = value;
			}
		}
	   public bool BtnsVisible {
			get { return btnsVisible; }
			set {
				btnsVisible = value;
				BtnsVisibleChanged();
			}
		}	   
		public bool BorderLineHorizontalInVisible {
			get { return borderLineHorizontalInVisible; }
			set { 
				borderLineHorizontalInVisible= value;
				if (borderLineHorizontalInVisible) 
					btnHorizontBorderIn.Visible = true;
				else {
					btnHorizontBorderIn.Visible = false;
					previewBSUserControl.HorizontalLineIn = BorderLineState.No;
				}
			}
		}	  
		public bool BorderLineVerticalInVisible {
			get { return borderLineVerticalInVisible; }
			set {
				borderLineVerticalInVisible = value;
				if (borderLineVerticalInVisible)
					btnVerticalBorderIn.Visible = true;
				else {
					btnVerticalBorderIn.Visible = false;
					previewBSUserControl.VerticalLineIn = BorderLineState.No;
				}
			}
		}
		public BorderInfo CurrentBorderInfo {
			get { return (currentBorderInfo); }
			set {
				currentBorderInfo = value;
				if (setMode != SetMode.Custom)
					SetNoneModeCore();
				if (setMode == SetMode.Box)
					SetBoxModeCore();
				if (setMode == SetMode.All)
					SetAllModeCore();
				if (setMode == SetMode.Grid)
					SetGridModeCore();
			}
		}
		public BorderInfo BorderLineUp { 
			get { return (borderLineUp); } 
			set {
				borderLineUp = value;
				if (value != null){
					if (value.Style != BorderLineStyle.None) {
						btnHorizontBorderUp.Checked = true;
						previewBSUserControl.HorizontalLineUp = BorderLineState.Known;
					}
					else {
						btnHorizontBorderUp.Checked = false;
						previewBSUserControl.HorizontalLineUp = BorderLineState.No;
					}
				}
				else {
					btnHorizontBorderUp.Checked = false;
					previewBSUserControl.HorizontalLineUp = BorderLineState.Unknown;
				}
			} 
		}
		public BorderInfo BorderLineDown {
			get { return (borderLineDown); }
			set {
				borderLineDown = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnHorizontBorderDown.Checked = true;
						previewBSUserControl.HorizontalLineDown = BorderLineState.Known;
					}
					else {
						btnHorizontBorderDown.Checked = false;
						previewBSUserControl.HorizontalLineDown = BorderLineState.No;
					}
				}
				else {
					btnHorizontBorderDown.Checked = false;
					previewBSUserControl.HorizontalLineDown = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineHorizontalIn {
			get { return (borderLineHorizontalIn); }
			set {
				borderLineHorizontalIn = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnHorizontBorderIn.Checked = true;
						previewBSUserControl.HorizontalLineIn = BorderLineState.Known;
					}
					else {
						btnHorizontBorderIn.Checked = false;
						previewBSUserControl.HorizontalLineIn = BorderLineState.No;
					}
				}
				else {
					btnHorizontBorderIn.Checked = false;
					previewBSUserControl.HorizontalLineIn = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineLeft {
			get { return (borderLineLeft); }
			set {
				borderLineLeft = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnVerticalBorderLeft.Checked = true;
						previewBSUserControl.VerticalLineLeft = BorderLineState.Known;
					}
					else {
						btnVerticalBorderLeft.Checked = false;
						previewBSUserControl.VerticalLineLeft = BorderLineState.No;
					}
				}
				else {
					btnVerticalBorderLeft.Checked = false;
					previewBSUserControl.VerticalLineLeft = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineRight {
			get { return (borderLineRight); }
			set {
				borderLineRight = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnVerticalBorderRight.Checked = true;
						previewBSUserControl.VerticalLineRight = BorderLineState.Known;
					}
					else {
						btnVerticalBorderRight.Checked = false;
						previewBSUserControl.VerticalLineRight = BorderLineState.No;
					}
				}
				else {
					btnVerticalBorderRight.Checked = false;
					previewBSUserControl.VerticalLineRight = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineVerticalIn {
			get { return (borderLineVerticalIn); }
			set {
				borderLineVerticalIn = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnVerticalBorderIn.Checked = true;
						previewBSUserControl.VerticalLineIn = BorderLineState.Known;
					}
					else {
						btnVerticalBorderIn.Checked = false;
						previewBSUserControl.VerticalLineIn = BorderLineState.No;
					}
				}
				else {
					btnVerticalBorderIn.Checked = false;
					previewBSUserControl.VerticalLineIn = BorderLineState.Unknown;
				}
			}
		}
		#endregion
		#region Events
		public event EventHandler BorderLineUpChanged;
		protected void RaiseBorderLineUpChanged() {
			if (BorderLineUpChanged != null)
				BorderLineUpChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineDownChanged;
		protected void RaiseBorderLineDownChanged() { 
		if (BorderLineDownChanged != null)
			BorderLineDownChanged(this, EventArgs.Empty); 
		}
		public event EventHandler BorderLineHorizontalInChanged;
		protected void RaiseBorderLineHorizontalInChanged() {
			if (BorderLineHorizontalInChanged != null)
				BorderLineHorizontalInChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineVerticalInChanged;
		protected void RaiseBorderLineVerticalInChanged() {
			if (BorderLineVerticalInChanged != null)
				BorderLineVerticalInChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineLeftChanged;
		protected void RaiseBorderLineLeftChanged() {
			if (BorderLineLeftChanged != null)
				BorderLineLeftChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineRightChanged;
		protected void RaiseBorderLineRightChanged() {
			if (BorderLineRightChanged != null)
				BorderLineRightChanged(this, EventArgs.Empty);
		}
		#endregion
		public void btnHorizontBorderUp_CheckedChanged(object sender, EventArgs e) {
			if (InsideSetProperties)
				return;
			if (btnHorizontBorderUp.Checked) {
				borderLineUp = currentBorderInfo;
				previewBSUserControl.HorizontalLineUp = BorderLineState.Known;
			}
			else {
				borderLineUp = inactiveBorderInfo;
				previewBSUserControl.HorizontalLineUp = BorderLineState.No;
			}
			RaiseBorderLineUpChanged();
		}
		private void previewBSUserControl1_Load(object sender, EventArgs e) {
		}
		private void btnHorizontBorderIn_CheckedChanged(object sender, EventArgs e) {
			if (InsideSetProperties)
				return;
			if (btnHorizontBorderIn.Checked && borderLineHorizontalInVisible) {
				if (setMode == SetMode.Grid) {
					borderLineHorizontalIn = currentBorderInfo.Clone();
					borderLineHorizontalIn.Style = BorderLineStyle.Single;
					borderLineHorizontalIn.Width = DocumentModel.UnitConverter.TwipsToModelUnits(15);
				}
				else
					borderLineHorizontalIn = currentBorderInfo;
				previewBSUserControl.HorizontalLineIn = BorderLineState.Known;
			}
			else {
				borderLineHorizontalIn = inactiveBorderInfo;
				previewBSUserControl.HorizontalLineIn = BorderLineState.No;
			}
			RaiseBorderLineHorizontalInChanged();
		}
		private void btnHorizontBorderDown_CheckedChanged(object sender, EventArgs e) {
			if (InsideSetProperties)
				return;
			if (btnHorizontBorderDown.Checked) {
				borderLineDown = currentBorderInfo;
				previewBSUserControl.HorizontalLineDown = BorderLineState.Known;
			}
			else {
				borderLineDown = inactiveBorderInfo;
				previewBSUserControl.HorizontalLineDown = BorderLineState.No;
			}
			RaiseBorderLineDownChanged();
		}
		private void btnVerticalBorderLeft_CheckedChanged(object sender, EventArgs e) {
			if (InsideSetProperties)
				return;
			if (btnVerticalBorderLeft.Checked) {
				borderLineLeft = currentBorderInfo;
				previewBSUserControl.VerticalLineLeft = BorderLineState.Known;
			}
			else {
				borderLineLeft = inactiveBorderInfo;
				previewBSUserControl.VerticalLineLeft = BorderLineState.No;
			}
			RaiseBorderLineLeftChanged();
		}
		private void btnVerticalBorderIn_CheckedChanged(object sender, EventArgs e) {
			if (InsideSetProperties)
				return;
			if (btnVerticalBorderIn.Checked && borderLineVerticalInVisible) {
				if (setMode == SetMode.Grid) {
					borderLineVerticalIn = currentBorderInfo.Clone();
					borderLineVerticalIn.Style = BorderLineStyle.Single;
					borderLineVerticalIn.Width = DocumentModel.UnitConverter.TwipsToModelUnits(15);
				}
				else 
					borderLineVerticalIn = currentBorderInfo;
				previewBSUserControl.VerticalLineIn = BorderLineState.Known;
			}
			else {
				borderLineVerticalIn = inactiveBorderInfo;
				previewBSUserControl.VerticalLineIn = BorderLineState.No;
			}
			RaiseBorderLineVerticalInChanged();
		}
		private void btnVerticalBorderRight_CheckedChanged(object sender, EventArgs e) {
			if (InsideSetProperties)
				return;
			if (btnVerticalBorderRight.Checked) {
				borderLineRight = currentBorderInfo;
				previewBSUserControl.VerticalLineRight = BorderLineState.Known;
			}
			else {
				borderLineRight = inactiveBorderInfo;
				previewBSUserControl.VerticalLineRight = BorderLineState.No;
			}
			RaiseBorderLineRightChanged();
		}
		void SetNoneModeCore() {
			btnHorizontBorderDown.Checked = false;
			btnHorizontBorderUp.Checked = false;
			btnVerticalBorderRight.Checked = false;
			btnVerticalBorderLeft.Checked = false;
			btnHorizontBorderIn.Checked = false;
			btnVerticalBorderIn.Checked = false;
		}
		public void SetNoneMode() {
			setMode = SetMode.None;
			SetNoneModeCore();
		}
		void SetBoxModeCore() {
			btnHorizontBorderDown.Checked = true;
			btnHorizontBorderUp.Checked = true;
			btnVerticalBorderRight.Checked = true;
			btnVerticalBorderLeft.Checked = true;
			btnHorizontBorderIn.Checked = false;
			btnVerticalBorderIn.Checked = false;
		}
		public void SetBoxMode() {
			setMode = SetMode.Box;
			SetBoxModeCore();
		}
		void SetAllModeCore() {
			btnHorizontBorderDown.Checked = true;
			btnHorizontBorderUp.Checked = true;
			btnVerticalBorderRight.Checked = true;
			btnVerticalBorderLeft.Checked = true;
			btnHorizontBorderIn.Checked = true;
			btnVerticalBorderIn.Checked = true;
		}
		public void SetAllMode() {
			setMode = SetMode.All;
			SetAllModeCore();
		}
		void SetGridModeCore() {
			btnHorizontBorderDown.Checked = true;
			btnHorizontBorderUp.Checked = true;
			btnVerticalBorderRight.Checked = true;
			btnVerticalBorderLeft.Checked = true;
			btnHorizontBorderIn.Checked = true;
			btnVerticalBorderIn.Checked = true;
		}
		public void SetGridMode() {
			setMode = SetMode.Grid;
			SetGridModeCore();
		}
		void SetCustomModeCore() {
			btnHorizontBorderDown.Checked = true;
			btnHorizontBorderUp.Checked = true;
			btnVerticalBorderRight.Checked = true;
			btnVerticalBorderLeft.Checked = true;
			btnHorizontBorderIn.Checked = true;
			btnVerticalBorderIn.Checked = true;
		}
		public void SetCustomMode() {
			setMode = SetMode.Custom;
			SetCustomModeCore();  
		}
		private void previewBSUserControl_Click(object sender, EventArgs e) {
		}
		public enum SetMode { None, Box, All, Grid, Custom }
		private void BtnsVisibleChanged() {
			btnHorizontBorderUp.Visible = btnsVisible;
			if (borderLineHorizontalInVisible)
			  btnHorizontBorderIn.Visible = btnsVisible;
			btnHorizontBorderDown.Visible = btnsVisible;
			if (borderLineVerticalInVisible)
				btnVerticalBorderIn.Visible = btnsVisible;
			btnVerticalBorderLeft.Visible = btnsVisible;
			btnVerticalBorderRight.Visible = btnsVisible;
			labelControl2.Visible = btnsVisible;
			labelControl4.Visible = btnsVisible;
		}
	}
}
