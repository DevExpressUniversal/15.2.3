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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram.ViewInfo {
	public class DiagramDefaultAppearances : IDisposable {
		Font rulerTickMarkFont;
		UserLookAndFeel lookAndFeel;
		public DiagramDefaultAppearances(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			this.rulerTickMarkFont = CreateRulerTickMarkFont();
		}
		protected virtual Font CreateRulerTickMarkFont() {
			return new Font("Arial", 7f, GraphicsUnit.Point);
		}
		public AppearanceDefault CreateDefaultShapeAppearance() { return CreateDefaultShapeAppearance(AppearanceObject.DefaultFont); }
		public AppearanceDefault CreateDefaultShapeAppearance(Font font) {
			return new AppearanceDefault(GetShapeTextColor(), GetShapeBackColor(), GetShapeBorderColor(), font);
		}
		public AppearanceDefault CreateDefaultConnectorAppearance(Font font) {
			return new AppearanceDefault(GetConnectorForeColor(), GetConnectorBackColor(), GetConnectorBorderColor(), font);
		}
		public AppearanceDefault CreateToolboxShapeDefaultAppearance() {
			return CreateDefaultShapeAppearance(AppearanceObject.DefaultFont);
		}
		protected virtual Color GetShapeTextColor() { return Color.White; }
		protected virtual Color GetShapeBorderColor() { return Color.FromArgb(200, 200, 200); }
		protected virtual Color GetShapeBackColor() { return Color.FromArgb(91, 155, 213); }
		protected virtual Color GetConnectorForeColor() { return Color.FromArgb(79, 136, 187); }
		protected virtual Color GetConnectorBackColor() { return GetShapeBackColor(); }
		protected virtual Color GetConnectorBorderColor() { return GetShapeBorderColor(); }
		public virtual Color GetMajorGridAxisLineColor() {
			return Color.FromArgb(218, 218, 218);
		}
		public virtual Color GetMinorGridAxisLineColor() {
			return Color.FromArgb(228, 228, 228);
		}
		public virtual Color GetEditTextForeColor(DiagramItem item) {
			if(item.IsRoutable) return GetConnectorForeColor();
			return Color.FromArgb(110, 110, 110);
		}
		public virtual Color GetEditorBackColor() { return Color.Empty; }
		public virtual Color GetConnectorPointDragBackColor() {
			return Color.Black;
		}
		public virtual Color GetSelectionPartBackColor() {
			return Color.FromArgb(36, 64, 143);
		}
		public virtual Color GetItemDragPreviewBorderColor() {
			return Color.FromArgb(105, 204, 255);
		}
		public virtual Color GetRulerShadowColor() {
			return Color.FromArgb(57, 85, 163);
		}
		public virtual Color GetSnapLineColor() {
			return Color.FromArgb(90, 200, 90);
		}
		public virtual Color GetSelectionPreviewBorderColor() {
			return Color.FromArgb(171, 171, 171);
		}
		public virtual Color GetSelectionPreviewBackColor() {
			return Color.FromArgb(110, 232, 232, 232);
		}
		public virtual Color GetShapeParameterBorderColor() {
			return Color.FromArgb(119, 119, 119);
		}
		public virtual Color GetShapeParameterBackColor() {
			return Color.FromArgb(255, 232, 115);
		}
		public virtual Color GetInplaceEditSurfaceBorderColor() {
			return Color.FromArgb(185, 185, 185);
		}
		public virtual Color GetInplaceEditSurfaceBackColor() {
			return Color.White;
		}
		public virtual Color GetInplaceEditSurfaceAlternativeBorderColor() {
			return Color.FromArgb(241, 241, 241);
		}
		public virtual Color GetSelectionBorderColor() {
			return Color.FromArgb(147, 147, 147);
		}
		public virtual Color GetSelectionSizeGripColor() {
			return Color.FromArgb(168, 168, 168);
		}
		public virtual Color GetSelectionSizeGripCornerColor() {
			return Color.FromArgb(186, 188, 190);
		}
		public virtual Color GetConnectionPointColor() {
			return Color.FromArgb(102, 102, 102);
		}
		public virtual Color GetItemGlueBorderColor() {
			return Color.FromArgb(34, 208, 6);
		}
		public virtual Color GetPointGlueBorderColor() {
			return Color.FromArgb(38, 196, 61);
		}
		public virtual ConnectorSelectionColors GetConnectorSelectionColors() {
			Color border = GetSelectionBorderColor();
			Color sizeGrip = GetSelectionSizeGripColor();
			Color cornerColor = GetSelectionSizeGripCornerColor();
			return new ConnectorSelectionColors(border, sizeGrip, cornerColor);
		}
		public virtual ConnectorSelectionFreeBeginPointColors GetConnectorSelectionFreeBeginPointColors() {
			Color border = GetConnectorSelectionFreeBeginPointBorderColor();
			Color corner = GetConnectorSelectionFreeBeginPointCornerColor();
			return new ConnectorSelectionFreeBeginPointColors(border, corner);
		}
		protected virtual Color GetConnectorSelectionFreeBeginPointBorderColor() {
			return Color.FromArgb(147, 147, 147);
		}
		protected virtual Color GetConnectorSelectionFreeBeginPointCornerColor() {
			return Color.FromArgb(191, 191, 191);
		}
		public virtual ConnectorSelectionConnectedPointColors GetConnectorSelectionConnectedBeginPointColors() {
			Color back = GetConnectorSelectionConnectedBeginPointBackColor();
			Color border = GetConnectorSelectionConnectedBeginPointBorderColor();
			return new ConnectorSelectionConnectedPointColors(back, border);
		}
		protected virtual Color GetConnectorSelectionConnectedBeginPointBackColor() {
			return Color.FromArgb(34, 208, 6);
		}
		protected virtual Color GetConnectorSelectionConnectedBeginPointBorderColor() {
			return Color.FromArgb(180, 2, 100, 9);
		}
		public virtual ConnectorSelectionConnectedPointColors GetConnectorSelectionConnectedEndPointColors() {
			Color back = GetConnectorSelectionConnectedEndPointBackColor();
			Color corner = GetConnectorSelectionConnectedEndPointBorderColor();
			return new ConnectorSelectionConnectedPointColors(back, corner);
		}
		protected virtual Color GetConnectorSelectionConnectedEndPointBackColor() {
			return Color.FromArgb(14, 166, 25);
		}
		protected virtual Color GetConnectorSelectionConnectedEndPointBorderColor() {
			return Color.FromArgb(180, 2, 100, 9);
		}
		public virtual ConnectorSelectionFreeEndPointColors GetConnectorSelectionFreeEndPointColors() {
			Color border = GetConnectorSelectionFreeEndPointBorderColor();
			Color back = GetConnectorSelectionFreeEndPointBackColor();
			Color corner = GetConnectorSelectionFreeEndPointCornerColor();
			return new ConnectorSelectionFreeEndPointColors(border, back, corner);
		}
		protected virtual Color GetConnectorSelectionFreeEndPointBorderColor() {
			return Color.FromArgb(168, 168, 168);
		}
		protected virtual Color GetConnectorSelectionFreeEndPointBackColor() {
			return Color.FromArgb(209, 209, 209);
		}
		protected virtual Color GetConnectorSelectionFreeEndPointCornerColor() {
			return Color.FromArgb(204, 204, 204);
		}
		public virtual ConnectorSelectionIntermediatePointColors GetConnectorSelectionIntermediatePointColors() {
			Color back = GetConnectorSelectionIntermediatePointBackColor();
			return new ConnectorSelectionIntermediatePointColors(back);
		}
		protected virtual Color GetConnectorSelectionIntermediatePointBackColor() {
			return Color.FromArgb(81, 153, 242);
		}
		protected Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(this.lookAndFeel, color);
		}
		public virtual AppearanceDefault CreateDefaultHRulerAppearance() {
			return CreateRulerDefaultAppearance();
		}
		public virtual AppearanceDefault CreateDefaultVRulerAppearance() {
			return CreateRulerDefaultAppearance();
		}
		protected virtual AppearanceDefault CreateRulerDefaultAppearance() {
			AppearanceDefault appearance = new AppearanceDefault();
			appearance.Font = this.rulerTickMarkFont;
			appearance.ForeColor = ReportsSkins.GetSkin(LookAndFeel).Properties.GetColor("RulerForeColor");
			return appearance;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool dispose) {
			if(dispose) {
				if(this.rulerTickMarkFont != null) this.rulerTickMarkFont.Dispose();
			}
			this.rulerTickMarkFont = null;
		}
		#endregion
		protected UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
	}
}
