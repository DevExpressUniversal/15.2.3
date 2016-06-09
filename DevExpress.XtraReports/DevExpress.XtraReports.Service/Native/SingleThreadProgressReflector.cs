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

using System.Threading;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Service.Native {
	class SingleThreadProgressReflector : ProgressReflector {
		readonly Thread currentThread;
		bool IsCurrentThread {
			get { return currentThread == Thread.CurrentThread; }
		}
		public SingleThreadProgressReflector() {
			currentThread = Thread.CurrentThread;
		}
		public override void InitializeRange(int maximum) {
			if(IsCurrentThread) {
				base.InitializeRange(maximum);
			}
		}
		public override void MaximizeRange() {
			if(IsCurrentThread) {
				base.MaximizeRange();
			}
		}
		public override float RangeValue {
			get {
				return base.RangeValue;
			}
			set {
				if(IsCurrentThread) {
					base.RangeValue = value;
				}
			}
		}
		public override void SetProgressRanges(float[] ranges) {
			if(IsCurrentThread) {
				base.SetProgressRanges(ranges);
			}
		}
	}
}
