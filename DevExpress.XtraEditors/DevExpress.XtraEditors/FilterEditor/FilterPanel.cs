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
using System.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
namespace DevExpress.Utils.Drawing {
	public class FilterPanelInfoArgsBase : StyleObjectInfoArgs {
		CheckObjectInfoArgs activeButtonInfo;
		ObjectState textState;
		EditorButtonObjectInfoArgs closeButtonInfo, mruButtonInfo, customizeButtonInfo;
		Rectangle textBounds;
		string displayText;
		bool showCloseButton, showActiveButton, filterActive = true, allowMRU, showCustomizeButton;
		public FilterPanelInfoArgsBase() {
			this.customizeButtonInfo = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Glyph), null, true);
			this.closeButtonInfo = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Close), null, true);
			this.mruButtonInfo = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Combo), null, true);
			this.activeButtonInfo = new CheckObjectInfoArgs(null);
			this.activeButtonInfo.GlyphAlignment = HorzAlignment.Center;
			this.displayText = "";
			this.showCustomizeButton = this.allowMRU = this.showActiveButton = this.showCloseButton = true;
			this.textBounds = Rectangle.Empty;
			this.textState = ObjectState.Normal;
			this.MaxFilterPanelTextHeight = 100;
			CustomizeText = "Test"; 
		}		
		public string CustomizeText { get { return CustomizeButtonInfo.Button.Caption; } set { CustomizeButtonInfo.Button.Caption = value; } }
		public ObjectState TextState { get { return textState; } set { textState = value; } }
		public EditorButtonObjectInfoArgs CustomizeButtonInfo { get { return customizeButtonInfo; } }
		public EditorButtonObjectInfoArgs CloseButtonInfo { get { return closeButtonInfo; } }
		public EditorButtonObjectInfoArgs MRUButtonInfo { get { return mruButtonInfo; } }
		public CheckObjectInfoArgs ActiveButtonInfo { get { return activeButtonInfo; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool ShowCloseButton { get { return showCloseButton; } set { showCloseButton = value; } }
		public bool ShowCustomizeButton { get { return showCustomizeButton; } set { showCustomizeButton = value; } }
		public bool AllowMRU { get { return allowMRU; } set { allowMRU = value; } }
		public bool ShowActiveButton { get { return showActiveButton; } set { showActiveButton = value; } }
		public bool FilterActive { get { return filterActive; } set { filterActive = value; } }
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; } }
		public int MaxFilterPanelTextHeight { get; set; }
	}
	public class FilterPanelPainterBase : StyleObjectPainter {		
		const int MinButtonSize = 12;
		EditorButtonPainter buttonPainter;
		CheckObjectPainter checkPainter;
		ObjectPainter panelPainter;
		public FilterPanelPainterBase(EditorButtonPainter buttonPainter, CheckObjectPainter checkPainter) {
			this.buttonPainter = buttonPainter;
			this.checkPainter = checkPainter;
			this.panelPainter = CreatePanelPainter();
		}		
		protected virtual ObjectPainter CreatePanelPainter() {
			return new SimpleBorderPainter();
		}
		public EditorButtonPainter ButtonPainter { get { return buttonPainter; } }
		public ObjectPainter PanelPainter { get { return panelPainter; } }
		public CheckObjectPainter CheckPainter { get { return checkPainter; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return PanelPainter.GetObjectClientRectangle(UpdateInfo(e));
		}
		protected virtual Size GetButtonMinSize(FilterPanelInfoArgsBase e) {
			int h = Math.Max(MinButtonSize, ObjectPainter.CalcObjectMinBounds(e.Graphics, ButtonPainter, e.CloseButtonInfo).Height);
			h = Math.Max(h, ObjectPainter.CalcObjectMinBounds(e.Graphics, ButtonPainter, e.MRUButtonInfo).Height);
			h = Math.Max(h, ObjectPainter.CalcObjectMinBounds(e.Graphics, CheckPainter, e.ActiveButtonInfo).Height);
			h = Math.Max(h, ObjectPainter.CalcObjectMinBounds(e.Graphics, ButtonPainter, e.CustomizeButtonInfo).Height);
			return new Size(MinButtonSize, h);
		}
		protected virtual Size CalcButtonSize(Graphics g, ObjectPainter painter, ObjectInfoArgs info, Size minSize) {
			Rectangle bounds = ObjectPainter.CalcObjectMinBounds(g, painter, info);			
			if(bounds.Height < minSize.Height) bounds.Height = minSize.Height;
			if(bounds.Width < minSize.Width) bounds.Width = minSize.Width;
			if(bounds.Height % 2 == 0) bounds.Height += 1;
			return bounds.Size;
		}
		protected virtual Point CalcButtonLocation(Rectangle client, Size size, bool isRight) {
			int y = client.Y + (client.Height - size.Height) / 2;
			int x = isRight ? client.Right - size.Width : client.X;
			return new Point(x, y);
		}
		void CalcButton(FilterPanelInfoContext context, ObjectPainter painter, ObjectInfoArgs info, bool isRight = false, bool showAlways = true) {
			if(context.RightToLeft) isRight = !isRight;
			Size size = CalcButtonSize(context.Graphics, painter, info, context.MinButtonSize);
			Point location = CalcButtonLocation(context.ClientBounds, size, isRight);
			if(!showAlways && context.ClientBounds.Width < size.Width) return;
			info.Bounds = new Rectangle(location, size);
			context.ReduceClientBounds(info.Bounds, isRight);
		}
		protected virtual Rectangle CalcTextBounds(FilterPanelInfoArgsBase e, Rectangle client) {
			AppearanceObject app = GetStyle(e);
			int width = (int)app.CalcTextSize(e.Graphics, app.GetStringFormat(TextOptions.DefaultOptionsNoWrapEx), e.DisplayText, 0).Width; if(client.Width < width) width = client.Width;
			int x = e.RightToLeft ? client.Right - width : client.Left;
			Rectangle bounds = new Rectangle(x, client.Y - 1, width, client.Height);
			if(bounds.Height > e.MaxFilterPanelTextHeight) bounds.Height = e.MaxFilterPanelTextHeight;
			if(bounds.Width > width) bounds.Width = width;
			bounds.Inflate(0, -1);
			return bounds;
		}
		void CalcText(FilterPanelInfoContext context, FilterPanelInfoArgsBase e) {
			if(context.ClientBounds.Width <= 0) return;
			e.TextBounds = CalcTextBounds(e, context.ClientBounds);
			context.ReduceClientBounds(e.TextBounds, e.RightToLeft);
			if(e.AllowMRU)
				CalcButton(context, ButtonPainter, e.MRUButtonInfo, showAlways: false);				
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			FilterPanelInfoArgsBase ee = e as FilterPanelInfoArgsBase;
			ee.CustomizeButtonInfo.Bounds = ee.MRUButtonInfo.Bounds = ee.TextBounds = ee.ActiveButtonInfo.Bounds = ee.CloseButtonInfo.Bounds = Rectangle.Empty;
			if(e.Bounds.IsEmpty) return e.Bounds;
			using(FilterPanelInfoContext context = new FilterPanelInfoContext(e.Graphics, GetObjectClientRectangle(e))) {
				context.MinButtonSize = GetButtonMinSize(ee);
				context.RightToLeft = ee.RightToLeft;
				if(ee.ShowCloseButton)
					CalcButton(context, ButtonPainter, ee.CloseButtonInfo);
				if(ee.ShowActiveButton) {
					CalcButton(context, CheckPainter, ee.ActiveButtonInfo);
					CheckPainter.CalcObjectBounds(ee.ActiveButtonInfo);
				}
				if(ee.ShowCustomizeButton)
					CalcButton(context, ButtonPainter, ee.CustomizeButtonInfo, true, false);
				CalcText(context, ee);
			}
			return PanelPainter.CalcObjectBounds(UpdateInfo(e));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return PanelPainter.CalcBoundsByClientRectangle(UpdateInfo(e), client);
		}	  
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			FilterPanelInfoArgsBase ee = e as FilterPanelInfoArgsBase;
			int h = GetButtonMinSize(ee).Height;
			h = Math.Max(Math.Min(ee.MaxFilterPanelTextHeight, Convert.ToInt32(GetStyle(e).CalcTextSize(e.Graphics, ee.DisplayText.Trim() == "" ? "Wg" : ee.DisplayText, 0).Height + 4)), h) + 3;
			if(h % 2 == 0) h++;
			return CalcBoundsByClientRectangle(e, new Rectangle(0, 0, 100, h));
		}
		protected virtual ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) { return e; }
		public virtual void DrawBackground(ObjectInfoArgs e) {
			PanelPainter.DrawObject(UpdateInfo(e));
			GetStyle(e).FillRectangle(e.Cache, GetObjectClientRectangle(e));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawBackground(e);
			DrawForeground(e);
		}
		public virtual void DrawForeground(ObjectInfoArgs e) {
			Rectangle r = GetObjectClientRectangle(e);
			AppearanceObject style = GetStyle(e);
			FilterPanelInfoArgsBase ee = e as FilterPanelInfoArgsBase;
			if(!ee.CloseButtonInfo.Bounds.IsEmpty) {
				ObjectPainter.DrawObject(e.Cache, ButtonPainter, ee.CloseButtonInfo);
			}
			if(!ee.ActiveButtonInfo.Bounds.IsEmpty) {
				ee.ActiveButtonInfo.CheckState = ee.FilterActive ? CheckState.Checked : CheckState.Unchecked;				
				ObjectPainter.DrawObject(e.Cache, CheckPainter, ee.ActiveButtonInfo);
			}
			if(!ee.CustomizeButtonInfo.Bounds.IsEmpty) {
				ObjectPainter.DrawObject(e.Cache, ButtonPainter, ee.CustomizeButtonInfo);
			}
			if(!ee.TextBounds.IsEmpty) {
				Font font = ee.Appearance.Font;
				if(ee.AllowMRU && ee.TextState != ObjectState.Normal) font = new Font(font, FontStyle.Underline);
				ee.Appearance.DrawString(ee.Cache, ee.DisplayText, ee.TextBounds, font, ee.Appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrapEx));
				if(ee.AllowMRU && ee.TextState != ObjectState.Normal) font.Dispose();
			}
			if(!ee.MRUButtonInfo.Bounds.IsEmpty) {
				ObjectPainter.DrawObject(e.Cache, ButtonPainter, ee.MRUButtonInfo);
			}
		}
		class FilterPanelInfoContext : IDisposable {
			const int Indent = 3;
			Rectangle clientBoundsCore;
			public FilterPanelInfoContext(Graphics g, Rectangle bounds) { 
				Graphics = g;
				clientBoundsCore = bounds;
				clientBoundsCore.X += Indent;
				clientBoundsCore.Width -= 2 * Indent;
			}
			public Size MinButtonSize { get; set; }
			public Rectangle ClientBounds { get { return clientBoundsCore; } }
			public Graphics Graphics { get; private set; }
			public bool RightToLeft { get; set; }
			public void ReduceClientBounds(Rectangle bounds, bool isRight) {
				int edge = 0;
				if(!isRight) {
					edge = (bounds.Right + Indent) - clientBoundsCore.Left;
					clientBoundsCore.X = bounds.Right + Indent;					
				}
				else
					edge = clientBoundsCore.Right - (bounds.Left - Indent);
				clientBoundsCore.Width -= edge;
			}			
			public void Dispose() {
				Graphics = null;
			}
		}
	}
}
