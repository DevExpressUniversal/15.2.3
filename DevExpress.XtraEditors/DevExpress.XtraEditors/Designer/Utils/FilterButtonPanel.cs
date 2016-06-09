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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.Utils;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
namespace DevExpress.XtraEditors.Designer.Utils {
	public interface IFilterButtonsPanel : IButtonsPanel {
		bool IsSearchControlVisible { get; }
		SearchControlEx SearchControl { get; }
	}
	[ToolboxItem(false)]
	public class FilterButtonPanel : ButtonsPanelControl.ButtonPanelControl, IButtonsPanelOwner {
		public FilterButtonPanel() {
			AllowGlyphSkinning = true;
			this.Controls.Add(ButtonsPanel.SearchControl);
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(IsSkinPaintStyle)
				return new FilterButtonPanelSkinPainter((Parent as DevExpress.LookAndFeel.ISupportLookAndFeel).LookAndFeel);
			return new FilterButtonPanelPainter();
		}
		internal bool AllowDrawBorder { get { return (Parent is DXPropertyGridEx) && (Parent as DXPropertyGridEx).AllowDrawBorder; } }
		internal Color BorderColor { get { return (Parent is DXPropertyGridEx) ? (Parent as DXPropertyGridEx).GridLineColor : Color.Empty; } }
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			base.OnPaint(e);
			if(AllowDrawBorder) {
				Rectangle borderBounds = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
				e.Graphics.DrawRectangle(new Pen(BorderColor), borderBounds);
			}
		}
		public void FilterAgain() { ButtonsPanel.SearchControl.SearchAgain(); }
		public ISearchControlClient Client {
			get { return ButtonsPanel.Client; }
			set { ButtonsPanel.Client = value; }
		}
		public bool SearchControlVisible {
			get { return ButtonsPanel.SearchControlVisible; }
			set { ButtonsPanel.SearchControlVisible = value; }
		}
		protected new FilterButtonsPanel ButtonsPanel {
			get { return base.ButtonsPanel as FilterButtonsPanel; }
		}
		protected override Size DefaultSize { get { return new Size(200, 20); } }
		protected override ButtonsPanelControl.ButtonsPanelControl CreateButtonsPanel() {
			return new FilterButtonsPanel(this);
		}
	}
	public class FilterButtonsPanel : ButtonsPanelControl.ButtonsPanelControl, IFilterButtonsPanel {
		SearchControlEx searchControl;
		public FilterButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
			searchControlVisible = true;
			CreateSearchControl();
		}
		protected override ButtonPanel.IButtonsPanelViewInfo CreateViewInfo() {
			return new FilterButtonsPanelViewInfo(this);
		}
		#region ISearchPropertyGridButtonsPanel Members
		public SearchControlEx SearchControl {
			get { return searchControl; }
		}
		bool searchControlVisible;
		public bool SearchControlVisible {
			get { return searchControlVisible; }
			set { SetValue(ref searchControlVisible, value, "SearchControlVisible"); }
		}
		ISearchControlClient clientCore;
		public ISearchControlClient Client {
			get { return clientCore; }
			set { SetValue(ref clientCore, value, "Client"); }
		}
		protected override void OnUpdateObjectCore(EventArgs e) {
			PropertyChangedEventArgs args = e as PropertyChangedEventArgs;
			if(args != null)
				OnPropertyChanged(args);
			base.OnUpdateObjectCore(e);
		}
		void OnPropertyChanged(PropertyChangedEventArgs args) {
			switch(args.PropertyName) {
				case "SearchControlVisible":
					OnSearchControlVisibleChanged();
					break;
				case "Client":
					OnClientChanged();
					break;
			}
		}
		protected virtual void OnClientChanged() {
			if(SearchControlVisible)
				SearchControl.Client = Client;
		}
		protected virtual void OnSearchControlVisibleChanged() {
			SearchControl.Visible = SearchControlVisible;
		}
		void CreateSearchControl() {
			if(SearchControl == null) {
				searchControl = new SearchControlEx();
				searchControl.Name = "searchControl";
				searchControl.Client = Client;
				searchControl.Properties.AutoHeight = false;
			}
		}
		void DisposeSearchControl() {
			if(SearchControl != null) {
				SearchControl.Dispose();
				searchControl = null;
			}
		}
		bool IFilterButtonsPanel.IsSearchControlVisible { get { return SearchControlVisible; } }
		protected override void OnDispose() {
			base.OnDispose();
			DisposeSearchControl();
		}
		#endregion
	}
	public class FilterButtonsPanelViewInfo : ButtonsPanelControlViewInfo {
		Rectangle searchControlBounds;
		const int searchControlInterval = 4;
		public FilterButtonsPanelViewInfo(IFilterButtonsPanel panel)
			: base(panel) {
		}
		public Rectangle SearchControlBounds { get { return searchControlBounds; } }
		public new IFilterButtonsPanel Panel { get { return base.Panel as IFilterButtonsPanel; } }
		public override Size CalcMinSize(Graphics g) {
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			if(IsReady && !Content.Size.IsEmpty)
				return MinSize;
			Size result = new Size(0, 0);
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			int visibleButtons = 0;
			foreach(BaseButtonInfo buttonInfo in Buttons) {
				Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
				visibleButtons += buttonSize.Width != 0 ? 1 : 0;
				result.Width = result.Width + buttonSize.Width;
				result.Height = Math.Max(buttonSize.Height, result.Height);
			}
			result.Width += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			Size searchControlMinSize = CalcSearchControlMinSize(g);
			result.Width += searchControlMinSize.Width;
			if(result.Height < searchControlMinSize.Height)
				result.Height = searchControlMinSize.Height;
			MinSize = painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, result)).Size;
			return MinSize;
		}
		protected virtual IList<BaseButtonInfo> GetInfoButtons() {
			IList<BaseButtonInfo> buttonInfos = new List<BaseButtonInfo>();
			IBaseButton[] buttons = SortButtonList(Panel.Buttons);
			foreach(IBaseButton button in buttons)
				buttonInfos.Add(CreateButtonInfo(button));
			return buttonInfos;
		}
		public override void Calc(Graphics g, Rectangle bounds) {
			if(IsReady) return;
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			Buttons = GetInfoButtons();
			Bounds = painter.CalcBoundsByClientRectangle(new ObjectInfoArgs(Cache, bounds, State), bounds);
			Size minSize = CalcMinSize(g);
			Rectangle content = painter.GetObjectClientRectangle(new ObjectInfoArgs(Cache, bounds, State));
			if(minSize.Height > content.Height)
				content.Height = minSize.Height;
			Content = content;
			int interval = Panel.ButtonInterval;
			Point offset = Content.Location;
			foreach(BaseButtonInfo bInfo in Buttons) {
				if(!Content.Contains(offset)) break;
				if(!bInfo.Button.Properties.Visible) continue;
				bInfo.Calc(g, buttonPainter, offset, Content, true, true);
				offset.X = bInfo.Bounds.Right + interval;
			}
			if(Content.Contains(offset))
				searchControlBounds = CalcSearchControlBounds(Content, minSize, offset);
			Panel.BeginUpdate();
			Panel.SearchControl.Bounds = searchControlBounds;
			Panel.Bounds = Bounds;
			Panel.CancelUpdate();
		}
		protected virtual Rectangle CalcSearchControlBounds(Rectangle Content, Size minSize, Point location) {
			int x = location.X + searchControlInterval;
			int width = Content.Right - x;
			int height = Content.Height > minSize.Height ? Content.Height : minSize.Height;
			return new Rectangle(x, location.Y, width, height);
		}
		protected virtual Size CalcSearchControlMinSize(Graphics g) {
			return Panel.IsSearchControlVisible ? Panel.SearchControl.Size : Size.Empty;
		}
	}
	public class FilterButtonPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public FilterButtonPanelSkinPainter(DevExpress.Skins.ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new XtraButtonControlSkinPainter(Provider);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -1, -1);
		}
	}
	public class FilterButtonPanelPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new XtraButtonControlPainter();
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -1, -1);
		}
	}
	public class XtraButtonControlPainter : BaseButtonPainter {
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 3, 3);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -3, -3);
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			base.DrawBackground(cache, info);
		}
	}
	public class XtraButtonControlSkinPainter : ButtonControlSkinPainter {
		public XtraButtonControlSkinPainter(DevExpress.Skins.ISkinProvider provider)
			: base(provider) {
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage && info.Button.Properties.UseImage) {
				Image image = GetActualImage(info);
				DrawImageCore(cache, info, image);
			}
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(Info.State == ObjectState.Normal) return;
			SkinElement skin = GetSkin()[BarSkins.SkinLinkSelected];
			SkinElementInfo eInfo = new SkinElementInfo(skin, Info.Bounds);
			if((Info.State & ObjectState.Hot) != 0)
				eInfo.ImageIndex = 1;
			if((Info.State & ObjectState.Pressed) != 0)
				eInfo.ImageIndex = 2;
			if((Info.State & ObjectState.Selected) != 0)
				eInfo.ImageIndex = 4;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, eInfo);
		}
		protected void DrawImageCore(GraphicsCache cache, BaseButtonInfo info, Image image) {
			Rectangle srcRect = new Rectangle(Point.Empty, image.Size);
			Rectangle destRect = PlacementHelper.Arrange(image.Size, info.ImageBounds, ContentAlignment.MiddleCenter);
			int stateIndex = GetStateIndex(info);
			Color bgColor = GetForegroundColor(stateIndex);
			using(Image coloredImage = DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, bgColor)) {
				if(info.Disabled)
					cache.Paint.DrawImage(info.Graphics, coloredImage, destRect, srcRect, DevExpress.Utils.Helpers.ColoredImageHelper.DisabledImageAttr);
				else
					cache.Graphics.DrawImage(coloredImage, destRect, srcRect, GraphicsUnit.Pixel);
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 3, 3);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -3, -3); ;
		}
		protected override Color GetInvertedColor() {
			return DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColorEx(SkinProvider, SystemColors.WindowText);
		}
		protected override Skin GetSkin() {
			return BarSkins.GetSkin(SkinProvider);
		}
		protected override Color GetHotColor(Color defaultColor) {
			return DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColorEx(SkinProvider, SystemColors.ControlText);
		}
		protected override Color GetColor() {
			return DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColorEx(SkinProvider, SystemColors.ControlText);
		}
	}
	[ToolboxItem(false)]
	public class SearchControlEx : SearchControl {
		public void SearchAgain() { ActionSearch(); }
		protected override bool ProcessDialogKey(Keys keyData) {
			switch((keyData & Keys.KeyCode)) {
				case Keys.Return:
				case Keys.Escape:
					return false;
			}
			return base.ProcessDialogKey(keyData);
		}
	}
}
