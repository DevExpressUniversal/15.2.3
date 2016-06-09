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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface IDragFrameInfo {
		Size Calc(Size content);
		void Draw(GraphicsCache cache);
	}
	public abstract class DragFrameBase : DXLayeredWindowEx {
		ISkinProvider skinProviderCore;
		public DragFrameBase() {
			Alpha = 160;
		}
		protected override void OnDisposing() {
			skinProviderCore = null;
			infoCore = null;
			base.OnDisposing();
		}
		public ISkinProvider SkinProvider {
			get { return skinProviderCore; }
			set {
				if(SkinProvider == value) return;
				skinProviderCore = value;
				OnSkinProviderChanged();
			}
		}
		IDragFrameInfo infoCore;
		protected IDragFrameInfo Info {
			get { return infoCore; }
		}
		protected void OnSkinProviderChanged() {
			EnsureInfo();
		}
		protected void EnsureInfo() {
			infoCore = CreateInfo();
		}
		protected override void DrawCore(GraphicsCache cache) {
			Info.Draw(cache);
		}
		protected abstract IDragFrameInfo CreateInfo();
	}
	public class DragFrame : DragFrameBase {
		Form targetCore;
		protected override void OnDisposing() {
			targetCore = null;
			base.OnDisposing();
		}
		public Form Target {
			get { return targetCore; }
			set {
				if(targetCore == value) return;
				targetCore = value;
				OnTargetChanged();
			}
		}
		protected void OnTargetChanged() {
			if(Target != null) {
				Size = Info.Calc(Target.ClientSize);
				Invalidate();
			}
		}
		protected override IDragFrameInfo CreateInfo() {
			return new DragFrameInfo(SkinProvider);
		}
	}
	public class DragFrameInfo : IDragFrameInfo {
		Rectangle boundsCore;
		Rectangle contentCore;
		protected readonly ISkinProvider SkinProvider;
		public DragFrameInfo(ISkinProvider provider) {
			SkinProvider = provider;
		}
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		public Rectangle Content {
			get { return contentCore; }
		}
		public Size Calc(Size content) {
			Padding p = GetContentMargin();
			boundsCore = new Rectangle(0, 0,
				(content.Width / 2) + p.Horizontal,
				(content.Height / 2) + p.Vertical);
			contentCore = new Rectangle(
				p.Left, p.Top,
				content.Width / 2,
				content.Height / 2);
			return Bounds.Size;
		}
		public void Draw(GraphicsCache cache) {
			DrawCore(cache);
		}
		protected virtual Padding GetContentMargin() {
			return new Padding(2, 17, 2, 2);
		}
		protected virtual void DrawCore(GraphicsCache cache) {
			Brush border = cache.GetSolidBrush(LookAndFeelHelper.GetSystemColorEx(SkinProvider, SystemColors.ControlDark));
			Brush content = cache.GetSolidBrush(LookAndFeelHelper.GetSystemColorEx(SkinProvider, SystemColors.Control));
			cache.FillRectangle(border, new Rectangle(0, 15, Bounds.Width, Bounds.Height - 15));
			cache.FillRectangle(content, new Rectangle(1, 16, Bounds.Width - 2, Bounds.Height - 17));
			cache.FillRectangle(border, new Rectangle(0, 0, 80, 16));
			cache.FillRectangle(content, new Rectangle(1, 1, 78, 15));
		}
	}
	class DragMoveHelper : System.IDisposable {
		DocumentManager manager;
		Control ownerControl;
		System.Predicate<Views.BaseViewHitInfo> hitTest;
		public DragMoveHelper(DocumentManager manager, System.Predicate<Views.BaseViewHitInfo> hitTest) {
			Base.AssertionException.IsNotNull(manager);
			Base.AssertionException.IsNotNull(hitTest);
			this.manager = manager;
			this.hitTest = hitTest;
			if(manager != null)
				this.ownerControl = manager.GetOwnerControl();
			SubscribeEvents();
		}
		void System.IDisposable.Dispose() {
			UnubscribeEvents();
			this.ownerControl = null;
			this.manager = null;
			this.hitTest = null;
			System.GC.SuppressFinalize(this);
		}
		void SubscribeEvents() {
			if(ownerControl != null)
				ownerControl.MouseDown += ownerControl_MouseDown;
		}
		void UnubscribeEvents() {
			if(ownerControl != null)
				ownerControl.MouseDown -= ownerControl_MouseDown;
		}
		void ownerControl_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == System.Windows.Forms.MouseButtons.Left) {
				var hitInfo = manager.CalcHitInfo(e.Location);
				if(!hitInfo.IsEmpty && hitTest(hitInfo))
					EmulateDragMove(((Control)sender).FindForm());
			}
		}
		static void EmulateDragMove(Control parentForm) {
			if(parentForm != null && parentForm.IsHandleCreated) {
				DevExpress.Utils.Drawing.Helpers.NativeMethods.ReleaseCapture();
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(parentForm.Handle,
					0xA1 , (System.IntPtr)0x2 , System.IntPtr.Zero);
			}
		}
	}
}
