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
using DevExpress.XtraScheduler.Native;
using System.Drawing;
namespace DevExpress.XtraScheduler.Animation.Internal {
	public class LastPaintInfo : IDisposable {
		int lastStateIdentity = Int32.MaxValue;
		SchedulerAnimationInfo prevAnimationInfo;
		SchedulerAnimationInfo currentAnimationInfo;
		public SchedulerAnimationInfo PrevAnimationInfo { get { return prevAnimationInfo; } }
		public SchedulerAnimationInfo CurrentAnimationInfo { get { return currentAnimationInfo; } }
		public bool ShouldObtainAnimationInfo(SchedulerControl control) {
			if (prevAnimationInfo == null || currentAnimationInfo == null)
				return true;
			return control.StateIdentity != lastStateIdentity;
		}
		public void ObtainCurrentAnimationInfo(SchedulerControl control) {
			this.lastStateIdentity = control.StateIdentity;
			Bitmap oldBitmap = GetBitmap(this.prevAnimationInfo);
			Bitmap existingBitmap = CreateOrReuseBitmap(control, this.prevAnimationInfo);
			if (oldBitmap != null && oldBitmap != existingBitmap)
				oldBitmap.Dispose();
			this.prevAnimationInfo = currentAnimationInfo;
			this.currentAnimationInfo = CalculateAnimationInfo(control);
			this.currentAnimationInfo.Bitmap = existingBitmap;			
		}		
		public void ResetAnimationInfo(SchedulerControl control) {
			this.prevAnimationInfo = this.currentAnimationInfo;
			this.lastStateIdentity = control.StateIdentity;
		}
		Bitmap GetBitmap(SchedulerAnimationInfo animationInfo) {
			if (animationInfo == null)
				return null;
			return animationInfo.Bitmap;
		}
		SchedulerAnimationInfo CalculateAnimationInfo(SchedulerControl control) {
			return SchedulerAnimationInfo.Create(control);
		}
		Bitmap CreateOrReuseBitmap(SchedulerControl control, SchedulerAnimationInfo animationInfo) {
			if (animationInfo == null || animationInfo.Bitmap == null)
				return control.CreateControlBitmap(null);
			if (control.Size != animationInfo.Bitmap.Size)
				return control.CreateControlBitmap(null);
			else
				return control.CreateControlBitmap(animationInfo.Bitmap);
		}
		public void Reset(SchedulerControl control) {
			Rectangle bounds = control.Bounds;
			if (bounds.Width <= 0 || bounds.Height <= 0)
				return;
			ObtainCurrentAnimationInfo(control);
			ObtainCurrentAnimationInfo(control);
		}
		#region IDisposable implementation
		public void Dispose() {
			Bitmap bitmap = GetBitmap(this.prevAnimationInfo);
			if (bitmap != null)
				bitmap.Dispose();
			this.prevAnimationInfo = null;
			bitmap = GetBitmap(this.currentAnimationInfo);
			if (bitmap != null)
				bitmap.Dispose();
			this.currentAnimationInfo = null;
		}
		#endregion
	}
}
