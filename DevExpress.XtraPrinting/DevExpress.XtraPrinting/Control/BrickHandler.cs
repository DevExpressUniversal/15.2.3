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
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentView.Controls;
namespace DevExpress.XtraPrinting.Control
{
	internal class BrickHandler : IDisposable
	{
		protected PrintControl pc;
		private Brick brick;
		private System.Timers.Timer showTimer;
		private System.Timers.Timer hideTimer;
		private IPopupForm PopupForm { get { return pc.PopupForm; } }
		private Cursor Cursor { get { return Cursor.Current; } }
		internal BrickHandler(PrintControl pc) : base() {
			this.pc = pc;			
			showTimer = new System.Timers.Timer(1000);
			showTimer.SynchronizingObject = pc;
			hideTimer = new System.Timers.Timer(10000);
			hideTimer.SynchronizingObject = pc;
			showTimer.Elapsed += new System.Timers.ElapsedEventHandler(showTimer_Elapsed);
			hideTimer.Elapsed += new System.Timers.ElapsedEventHandler(hideTimer_Elapsed);
			pc.BrickMouseMove += new BrickEventHandler(pc_OnBrickMove);
			pc.BrickClick += new BrickEventHandler(pc_BrickClick);
		}
		public void Dispose() {
			if(showTimer != null) {
				showTimer.Elapsed -= new System.Timers.ElapsedEventHandler(showTimer_Elapsed);
				showTimer.Dispose();
				showTimer = null;
			}
			if(hideTimer != null) {
				hideTimer.Elapsed -= new System.Timers.ElapsedEventHandler(hideTimer_Elapsed);
				hideTimer.Dispose();
				hideTimer = null;
			}
			if(pc != null) {
				pc.BrickMouseMove -= new BrickEventHandler(pc_OnBrickMove);
				pc.BrickClick -= new BrickEventHandler(pc_BrickClick);
				pc = null;
			}
		}
		private void showTimer_Elapsed(object source, System.Timers.ElapsedEventArgs e) {
			if(pc == null) return;
			Brick br = pc.CreateBrickEventArgs(null).Brick;
			if(brick != null && brick.Equals(br)) {
				PopupForm.ShowText(brick.Hint, Cursor.Position, MarginSide.Bottom, brick);
				hideTimer.Start();
			} else
				brick = null;
			showTimer.Stop();
		}
		private void hideTimer_Elapsed(object source, System.Timers.ElapsedEventArgs e) {
			PopupForm.HidePopup();
			hideTimer.Stop();
		}
		void pc_BrickClick(object sender, BrickEventArgs e) {
			HideInfo();
			Brick br = e.Brick;
			if(br != null) {
				if(!Cursor.Equals(Cursors.Hand))
					return;
				VisualBrick visualBrick = br as VisualBrick;
				BrickPagePair bpPair = visualBrick == null ? BrickPagePair.Empty : visualBrick.NavigationPair;
				if(bpPair != BrickPagePair.Empty) {
					pc.ShowBrick(bpPair);
				} else if(!string.IsNullOrEmpty(br.Url) && String.Compare(br.Url, SR.BrickEmptyUrl, true) != 0)
					ProcessLaunchHelper.StartProcess(br.Url);
			}
		}
		private void pc_OnBrickMove(object sender, BrickEventArgs e) {
			Brick eBrick = e.Brick;
			if(eBrick == null) {
				HideInfo();
			} else {
				if(brick == null) {
					if(!string.IsNullOrEmpty(eBrick.Hint)) {
						brick = eBrick;
						showTimer.Start();
					}
				} else if(brick.Equals(eBrick) == false)
					HideInfo();
			}
		}
		private void HideInfo() {
			PopupForm.HidePopup();
			brick = null;
			showTimer.Stop();
			hideTimer.Stop();
		}
	}
}
