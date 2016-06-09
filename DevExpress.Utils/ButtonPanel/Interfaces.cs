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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010;
namespace DevExpress.XtraEditors.ButtonPanel {
	public enum ImageLocation {
		Default, BeforeText, AfterText, AboveText, BelowText
	}
	public interface IBaseButton : IDisposable {
		IButtonProperties Properties { get; }
		event EventHandler Changed;
		event EventHandler CheckedChanged;
		event EventHandler Disposed;
		bool? IsChecked { get; }
		void SetOwner(IButtonsPanel owner);
		void SetMerged(IButtonsPanel mergedOwner);
	}
	public interface ISupportGroupUpdate {
		void LockCheckEvent();
		void UnlockCheckEvent();
	}
	public interface IButtonProperties : IBaseButton, ISupportGroupUpdate {
		string Caption { get; set; }
		Image Image { get; set; }
		string ImageUri { get; set; }
		ImageLocation ImageLocation { get; set; }
		int ImageIndex { get; set; }
		bool UseImage { get; set; }
		bool UseCaption { get; set; }
		AppearanceObject Appearance { get; }
		object Images { get; }
		object Glyphs { get; set; }
		ButtonStyle Style { get; set; }
		bool Visible { get; set; }
		bool Enabled { get; set; }
		bool Checked { get; set; }
		int VisibleIndex { get; set; }
		string ToolTip { get; set; }
		SuperToolTip SuperTip { get; set; }
		object Tag { get; set; }
		int GroupIndex { get; set; }
		bool IsUpdateLocked { get; }
		void BeginUpdate();
		void EndUpdate();
		void CancelUpdate();
	}
	public interface IButtonsPanel : IToolTipControlClient, IDisposable {
		IButtonsPanelOwner Owner { get; }
		BaseButtonCollection Buttons { get; }
		Rectangle Bounds { get; set; }
		Orientation Orientation { get; set; }
		RotationAngle ButtonRotationAngle { get; set; }
		ContentAlignment ContentAlignment { get; set; }
		int ButtonInterval { get; set; }
		object Images { get; }
		bool WrapButtons { get; set; }
		bool RightToLeft { get; }
		bool IsHorizontal { get; }
		bool IsSelected { get; }
		event BaseButtonEventHandler ButtonClick;
		event BaseButtonEventHandler ButtonChecked;
		event BaseButtonEventHandler ButtonUnchecked;
		IButtonsPanelHandler Handler { get; }
		IButtonsPanelViewInfo ViewInfo { get; }
		void PerformClick(IBaseButton button);
		void LayoutChanged();
		void CheckedChanged(object sender, EventArgs e);
		void BeginUpdate();
		void EndUpdate();
		void CancelUpdate();
	}
	public interface IButtonsPanelOwnerEx : IButtonsPanelOwner {
		Padding ButtonBackgroundImageMargin { get; set; }
		bool CanPerformClick(IBaseButton button);
	}
	public interface IButtonsPanelGlyphSkinningOwner {
		Color GetGlyphSkinningColor(BaseButtonInfo info);
	}
	public interface IButtonsPanelOwner {
		object ButtonBackgroundImages { get; }
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance AppearanceButton { get; }
		bool Enabled { get; }
		bool AllowHtmlDraw { get; }
		bool AllowGlyphSkinning { get; }
		object Images { get; }
		ObjectPainter GetPainter();
		bool IsSelected { get; }
		void Invalidate();
	}
	public interface IGroupBoxButtonsPanelOwner : IButtonsPanelOwner {
		BaseButtonCollection CustomHeaderButtons { get; }
		bool IsRightToLeft { get; }
		void RaiseButtonsPanelButtonChecked(BaseButtonEventArgs ea);
		void RaiseButtonsPanelButtonUnchecked(BaseButtonEventArgs ea);
		void RaiseButtonsPanelButtonClick(BaseButtonEventArgs ea);
		void LayoutChanged();
	}
	public interface IButtonsPanelViewInfo {
		IButtonsPanel Panel { get; }
		Rectangle Bounds { get; }
		bool IsReady { get; }
		void SetDirty();
		IList<BaseButtonInfo> Buttons { get; }
		void Calc(Graphics g, Rectangle bounds);
		Size CalcMinSize(Graphics g);
		BaseButtonInfo CalcHitInfo(Point point);
	}
	public interface IButtonsPanelHandler {
		IButtonsPanel Panel { get; }
		IBaseButton PressedButton { get; }
		IBaseButton HotButton { get; }
		void OnMouseDown(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseLeave();
		void Reset();
	}
	public interface IButtonsPanelHandlerWithState {
		void SaveState(System.Collections.Hashtable storage);
		void ApplyState(System.Collections.Hashtable storage);
	}
	public enum TabButtonsPanelState {
		Normal, Active, Selected
	}
}
