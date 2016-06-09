﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class PictureProcessorForRangeCopyOperation {
		ITargetRangeCalculatorOwner owner;
		IRangeCopyPicturePart rangeCopy;
		public PictureProcessorForRangeCopyOperation(ITargetRangeCalculatorOwner owner, IRangeCopyPicturePart rangeCopy) {
			this.owner = owner;
			this.rangeCopy = rangeCopy;
		}
		protected ITargetRangeCalculatorOwner Owner { get { return owner; } }
		public IRangeCopyPicturePart RangeCopy { get { return rangeCopy; } }
		public virtual void Execute(RangeCopyInfo rangeInfo, Picture sourcePicture) {
			foreach (Model.CellRange currentTargetRange in rangeInfo.TargetRanges) {
				CellPositionOffset offset = Owner.GetTargetRangeOffset(currentTargetRange);
				RangeCopy.CreateTargetPicture(sourcePicture, offset);
			}
		}
	}
}
