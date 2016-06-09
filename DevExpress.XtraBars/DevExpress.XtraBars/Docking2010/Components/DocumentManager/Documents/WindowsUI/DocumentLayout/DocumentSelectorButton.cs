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

using DevExpress.XtraEditors.ButtonPanel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IDocumentSelectorButton : IBaseButton {
		BaseButtonInfo Info { get; }
		BaseButtonPainter Painter { get; }
		BaseButtonHandler Handler { get; }
		void PerformClick(IBaseButton button);
		void UpdateStyle();
		void ResetStyle();
		bool HitTest(Point point);
	}
	class DocumentSelectorButton : BaseButton, IDocumentSelectorButton {
		BaseButtonInfo infoCore;
		BaseButtonHandler handlerCore;
		BaseButtonPainter painterCore;
		bool isDisposing;
		public DocumentSelectorButton(IButtonsPanel owner)
			: base() {
			handlerCore = CreateHandler();
			infoCore = CreateInfo();
			SetOwner(owner);
			UpdateStyle();
		}
		protected virtual BaseButtonInfo CreateInfo() { return new BaseButtonInfo(this); }
		protected virtual BaseButtonHandler CreateHandler() { return new DocumentSelectorButtonHandler(this); }
		public override string Caption {
			get { return string.Empty; }
			set { }
		}
		public override ButtonStyle Style {
			get { return ButtonStyle.CheckButton; }
			set { }
		}
		public override Image Image {
			get { return LoadImageFromResources("DocumentSelectorButton"); }
			set { }
		}
		Image LoadImageFromResources(string name) {
			return DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Docking2010.Resources.{0}.png", name), typeof(DocumentSelectorButton).Assembly);
		}
		public bool IsDisposing { get { return isDisposing; } }
		protected override void OnDispose() {
			if(!isDisposing) {
				isDisposing = true;
				infoCore = null;
				ResetStyle();
				handlerCore = null;
				base.OnDispose();
			}
		}
		#region IDocumentSelectorButton Members
		public void UpdateStyle() { painterCore = GetButtonPainter(); }
		public void ResetStyle() { painterCore = null; }
		bool IDocumentSelectorButton.HitTest(Point p) {
			if(Info.Bounds.Contains(p)) return true;
			return false;
		}
		BaseButtonHandler IDocumentSelectorButton.Handler { get { return handlerCore; } }
		public BaseButtonInfo Info { get { return infoCore; } }
		public BaseButtonPainter Painter { get { return painterCore; } }
		void IDocumentSelectorButton.PerformClick(IBaseButton button) {
			if(button.Properties.Style == ButtonStyle.CheckButton) {
				button.Properties.BeginUpdate();
				button.Properties.Checked = !button.Properties.Checked;
				button.Properties.CancelUpdate();
			}
		}
		protected virtual BaseButtonPainter GetButtonPainter() {
			if(Info == null) return null;
			if(Info.ButtonPanelOwner == null) return null;
			BaseButtonsPanelPainter painter = Info.ButtonPanelOwner.GetPainter() as BaseButtonsPanelPainter;
			if(painter == null) return null;
			return painter.GetButtonPainter();
		}
		#endregion
		class DocumentSelectorButtonHandler : BaseButtonHandler {
			IDocumentSelectorButton buttonCore;
			public DocumentSelectorButtonHandler(IDocumentSelectorButton button) {
				buttonCore = button;
			}
			protected override BaseButtonInfo CalcHitInfo(Point point) {
				if(buttonCore == null || !buttonCore.HitTest(point)) return null;
				return buttonCore.Info;
			}
			protected override void Invalidate() {
				if(buttonCore == null) return;
				IButtonsPanelOwner owner = buttonCore.Info.ButtonPanelOwner;
				if(owner == null) return;
				IContentContainerInfo container = owner as IContentContainerInfo;
				if(container != null && container.Owner != null)
					container.Owner.Invalidate(buttonCore.Info.Bounds);
				else
					owner.Invalidate();
			}
			protected override void PerformClick(IBaseButton button) {
				if(buttonCore == null) return;
				buttonCore.PerformClick(button);
			}
		}
	}
}
