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
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Native {
	internal class XRProgressReflectorLogic : ProgressReflectorLogic {
		List<object> accessors;
		int sharedRangesCount;
		int currentMaximum;
		public XRProgressReflectorLogic(ProgressReflector reflector, IEnumerable<object> accessors)
			: base(reflector) {
			if(accessors == null)
				throw new NullReferenceException();
			this.accessors = new List<object>(accessors);
		}
		public void InitializeRange(object accessor, int maximum) {
			if(!accessors.Contains(accessor))
				return;
			if(SharedRangesCount > 0) {
				if(Ranges.Count > 1) {
					Ranges[1] = (float)Ranges[0] + (float)Ranges[1];
					Ranges.RemoveAt(0);
					currentMaximum += Convert.ToInt32(maximum - RangeValue);
					base.InitializeRange(currentMaximum);
				}
			} else {
				currentMaximum = maximum;
				base.InitializeRange(maximum);
			}
			SharedRangesCount++;
		}
		public void MaximizeRange(object accessor) {
			if(!accessors.Contains(accessor))
				return;
			SharedRangesCount--;
			if(SharedRangesCount == 0)
				base.MaximizeRange();
		}
		int SharedRangesCount {
			get { return sharedRangesCount; }
			set {
				if(value < 0)
					value = 0;
				sharedRangesCount = value;
			}
		}
		public void SetRangeValue(object accessor, float value) {
			if(!accessors.Contains(accessor))
				return;
			base.RangeValue = value;
		}
	}
}
