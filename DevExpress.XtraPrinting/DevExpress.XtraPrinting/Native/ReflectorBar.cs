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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview.Native;
namespace DevExpress.XtraPrinting.Native {
	public abstract class ControlReflectorBar : ReflectorBarBase {
		protected abstract System.Windows.Forms.Control PlaceHolder { get; }
		protected override void Invalidate(bool withChildren) {
			if(!Updating) InvalidateCore(withChildren);
		}
		void InvalidateCore(bool withChildren) {
			if(this.PlaceHolder != null) {
				this.PlaceHolder.Invalidate(withChildren);
				this.PlaceHolder.Update();
			}
		}
		protected override void EndUpdate() {
			base.EndUpdate();
			if(!Updating) InvalidateCore(true);
		}
	}
	public class ReflectorBar : ControlReflectorBar {
		DevExpress.XtraEditors.ProgressBarControl progressBar;
		public override bool Visible { get { return progressBar.Visible; } set { progressBar.Visible = value; } }
		protected override System.Windows.Forms.Control PlaceHolder { get { return progressBar.Parent; } }
		protected internal override int PositionCore {
			set { progressBar.Position = value; }
			get { return progressBar.Position; }
		}
		protected internal override int MaximumCore {
			set { progressBar.Properties.Maximum = value; }
			get { return progressBar.Properties.Maximum; }
		}
		public ReflectorBar(DevExpress.XtraEditors.ProgressBarControl progressBar) {
			this.progressBar = progressBar;
			Initialize();
		}
	}
	public class ReflectorBarItem : ControlReflectorBar {
		System.Windows.Forms.Control placeHolder;
		ProgressBarEditItem progressBar;
		BarManager barManager;
		int position;
		int maximum = 100;
		bool visible = false;
		ProgressBarEditItem ProgressBar {
			get {
				if(progressBar == null)
					progressBar = (ProgressBarEditItem)PreviewItemsLogicBase.GetBarItemByStatusPanelID(barManager, StatusPanelID.Progress);
				return progressBar;
			}
		}
		public override bool Visible { 
			get { return visible; } 
			set {
				if(visible != value) {
					visible = value;
					UpdateProgressBarVisibility();
				}
			}
		}
		protected override System.Windows.Forms.Control PlaceHolder { 
			get {
				if(placeHolder == null)
					placeHolder = PreviewItemsLogicBase.GetStatusControl(barManager);
				return placeHolder; 
			}
		}
		protected internal override int PositionCore {
			get { return position; }
			set {
				if(position != value) {
					position = value;
					UpdateProgressBarPosition();
				}
			}
		}
		protected internal override int MaximumCore {
			get { return maximum; }
			set {
				if(maximum != value) {
					maximum = value;
					UpdateProgressBarMaximum();
				}
			}
		}
		public ReflectorBarItem(BarManager barManager) {
			this.barManager = barManager;
			Initialize();
		}
		void UpdateProgressBarVisibility() {
			BarItemVisibility visibility = visible ? BarItemVisibility.Always : BarItemVisibility.Never;
			if(ProgressBar != null && ProgressBar.Visibility != visibility)
				ProgressBar.Visibility = visibility;
		}
		void UpdateProgressBarMaximum() {
			if(ProgressBar != null && ProgressBar.RepositoryItem.Maximum != maximum)
				ProgressBar.RepositoryItem.Maximum = maximum;
		}
		void UpdateProgressBarPosition() {
			if(ProgressBar != null && !Object.Equals(ProgressBar.EditValue, position))
				ProgressBar.EditValue = position;
		}
	}
}
