#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Localization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class DashboardTitleControl : DashboardUserControl, IDashboardTitleView {
		const int DashboardTitleLabelFontSize = 12;
		readonly DashboardTitleFilterToolTip filterToolTip;
		readonly ToolTipController toolTipController = new ToolTipController();
		DashboardItemCaptionButtonInfo exportButtonInfo;
		ImageButtonControl exportImageButton;
		DashboardItemCaptionButtonInfo parametersButtonInfo;
		ImageButtonControl parametersImageButton;
		SmallImageControl filterImage;
		Color parametersForeColor;
		string titleText = string.Empty;
		string parametersText = string.Empty;
		internal ImageButtonControl ExportImageButton { get { return exportImageButton; } }
		internal ImageButtonControl ParametersImageButton { get { return parametersImageButton; } }
		internal DashboardItemCaptionButtonInfo ExportButtonInfo { get { return exportButtonInfo; } }
		internal DashboardItemCaptionButtonInfo ParametersButtonInfo { get { return parametersButtonInfo; } }
		internal string TitleText { get { return lblDashboardTitle.Text; } }
		public DashboardTitleControl()
			: this(null) {
		}
		public DashboardTitleControl(DashboardViewer viewer) {
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			InitializeComponent();
			foreach(Control control in Controls) {
				control.MouseClick += OnChildMouseDown;
				control.MouseEnter += OnChildMouseEnter;
				control.MouseLeave += OnChildMouseLeave;
			}
			Visible = false;
			lblDashboardTitle.Font = new Font(Appearance.Font.Name, DashboardTitleLabelFontSize);
			pbDashboardTitleImage.MaximumSize = new Size(0, DashboardTitleViewModel.MaxCaptionHeight);
			exportButtonInfo = new DashboardItemCaptionButtonInfo(DashboardButtonType.Export, ImageHelper.ExportImage, ObjectState.Normal, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportTo));
			exportButtonInfo.AddBarItems(new ExportBarItemsPopupMenuCreator().GetBarItems());
			exportImageButton = CreateButton(viewer, "Export", exportButtonInfo);
			pnlButtons.Controls.Add(exportImageButton);
			parametersButtonInfo = new DashboardItemCaptionButtonInfo(DashboardButtonType.Parameters, null, ObjectState.Normal, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDashboardParametersCaption));
			parametersButtonInfo.AddBarItems(new ParametersBarItemsPopupMenuCreator().GetBarItems());
			parametersImageButton = CreateButton(viewer, "Parameters", parametersButtonInfo);
			pnlButtons.Controls.Add(parametersImageButton);
			filterImage = new SmallImageControl("Filter");
			filterToolTip = new DashboardTitleFilterToolTip();
			filterToolTip.RequestScreenHeight += OnRequestScreenHeight;
			filterImage.ToolTipController = filterToolTip.ToolTipController;
			filterImage.SuperTip = filterToolTip.SuperTip;
			pnlFilterImage.Visible = false;
			filterImage.Width = DashboardTitlePresenter.ImageSize;
			filterImage.Height = DashboardTitlePresenter.ImageSize;
			pnlFilterImage.Controls.Add(filterImage);
			filterImage.Dock = DockStyle.Right;
			UpdateLookAndFeel();
			LookAndFeel.StyleChanged += LookAndFeelChanged;
		}
		private void OnRequestScreenHeight(object sender, RequestScreenHeightEventArgs e) {
			e.ScreenHeight = Screen.FromControl(this).Bounds.Height;
		}
		ImageButtonControl CreateButton(DashboardViewer viewer, string image, DashboardItemCaptionButtonInfo buttonInfo) {
			ImageButtonControl result = new ImageButtonControl(
				image,
				() => buttonInfo.Execute(viewer, null, DashboardArea.DashboardTitle),
				buttonInfo.Tooltip
			);
			result.Visible = false;
			result.ToolTipController = toolTipController;
			return result;
		}
		void LookAndFeelChanged(object sender, EventArgs e) {
			UpdateLookAndFeel();
		}
		void UpdateLookAndFeel() {
			Skin skin = DashboardSkins.GetSkin(LookAndFeel);
			SkinElement sel = null;
			if(skin != null)
				sel = skin[DashboardSkins.SkinDashboardItemPanel];
			if(sel == null) {
				skin = CommonSkins.GetSkin(LookAndFeel);
				sel = skin[CommonSkins.SkinGroupPanel];
			}
			Color titleForeColor;
			if(sel != null) {
				Color foreColor = sel.Color.GetForeColor();
				titleForeColor = foreColor != Color.Empty ? foreColor : skin.CommonSkin.CommonSkin.Colors.GetColor("ControlText", Color.Black);
				parametersForeColor = sel.Properties.GetColor("ForeColor2", Color.Gray);
			}
			else {
				titleForeColor = skin == null ? Color.Black : skin.CommonSkin.CommonSkin.Colors.GetColor("WindowText", Color.Black);
				parametersForeColor = Color.Gray;
			}
			exportImageButton.ImageColor = titleForeColor;
			parametersImageButton.ImageColor = titleForeColor;
			lblDashboardTitle.ForeColor = titleForeColor;
			UpdateLabel();
		}
		void UpdateLabel() {
			if(titleText != null && parametersText != null) {
				StringBuilder sb = new StringBuilder();
				sb.Append(titleText);
				if(!string.IsNullOrEmpty(parametersText))
					DashboardWinHelper.AppendColoredText(sb, false, parametersForeColor, parametersText);
				string text = sb.ToString();
				lblDashboardTitle.Text = text;
			}
		}
		void OnChildMouseLeave(object sender, EventArgs e) {
			if(!RectangleToScreen(Bounds).Contains(Cursor.Position))
				OnMouseLeave(e);
		}
		void OnChildMouseEnter(object sender, EventArgs e) {
			OnMouseEnter(e);
		}
		void OnChildMouseDown(object sender, MouseEventArgs e) {
			OnMouseDown(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
		}
		#region IDashboardTitleView members
		bool IDashboardTitleView.Visible { get { return Visible; } set { Visible = value; } }
		int IDashboardTitleView.ButtonsWidth { get { return pnlButtons.Width; } set { pnlButtons.Width = value; } }
		int IDashboardTitleView.Height { get { return Height; } 
			set { 
				Height = value; 
			} }
		int IDashboardTitleView.Width { get { return Width; } }
		void IDashboardTitleView.UpdateLabelBounds(Rectangle bounds) {
			lblDashboardTitle.Bounds = bounds;
		}
		void IDashboardTitleView.UpdateImageBounds(Rectangle bounds) {
			pbDashboardTitleImage.Bounds = bounds;
		}
		void IDashboardTitleView.UpdateFilterImageBounds(Rectangle bounds) {
			pnlFilterImage.Bounds = bounds;
		}
		void IDashboardTitleView.UpdateExportButtonBounds(Rectangle? bounds) {
			if(bounds.HasValue) {
				exportImageButton.Visible = true;
				exportImageButton.Bounds = bounds.Value;
			} else
				exportImageButton.Visible = false;
		}
		void IDashboardTitleView.UpdateParameterButtonBounds(Rectangle? bounds) {
			if(bounds.HasValue) {
				parametersImageButton.Visible = true;
				parametersImageButton.Bounds = bounds.Value;
			} else
				parametersImageButton.Visible = false;
		}
		void IDashboardTitleView.UpdateFilterImageVisible(bool visible) {
			pnlFilterImage.Visible = visible;
		}
		void IDashboardTitleView.UpdateMasterFilterValues(IList<DashboardCommon.Printing.DimensionFilterValues> values) {
			filterToolTip.MasterFilterValues = values;
		}
		void IDashboardTitleView.UpdateImage(Image image) {
			if(image != null) {
				pbDashboardTitleImage.Image = image;
				pbDashboardTitleImage.Visible = true;
			} else
				pbDashboardTitleImage.Visible = false;
		}
		Size IDashboardTitleView.UpdateText(string titleText, string parametersText) {
			this.titleText = titleText;
			this.parametersText = parametersText;
			UpdateLabel();
			return lblDashboardTitle.CalcBestSize();
		}
		event EventHandler IDashboardTitleView.SizeChanged {
			add { SizeChanged += value; }
			remove { SizeChanged -= value; }
		}
		void IDashboardTitleView.BeginUpdate() {
			SuspendLayout();
		}
		void IDashboardTitleView.EndUpdate() {
			ResumeLayout();
			Invalidate();
			pnlButtons.Invalidate();
			pnlFilterImage.Invalidate();
			lblDashboardTitle.Invalidate();
			exportImageButton.Invalidate();
			parametersImageButton.Invalidate();
		}
		#endregion
	}
}
