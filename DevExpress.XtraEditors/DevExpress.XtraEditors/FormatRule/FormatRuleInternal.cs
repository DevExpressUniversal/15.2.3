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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using System.Linq;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraEditors.Helpers {
	public class FormatConditionIconSetPredefinedQuick : FormatConditionIconSetPredefined {
		int imageCount;
		string imageMask;
		string[] rangeDescriptions = new string[] { "", "", "≥50%, ≥0%", "≥67%, ≥33%, ≥0%", "≥75%, ≥50%, ≥25%, ≥0%", "≥80%, ≥60%, ≥40%, ≥20%, ≥0%" };
		protected internal FormatConditionIconSetPredefinedQuick(int imageCount, string imageMask, string categoryName, string name, string title)
			: base(false, name) {
			this.imageCount = imageCount;
			this.imageMask = imageMask;
			this.CategoryName = categoryName;
			this.Title = title;
			if(imageCount < rangeDescriptions.Length)
				this.RangeDescription = rangeDescriptions[imageCount];
			Populate();
		}
		protected override void PopulateCore() {
			ValueType = FormatConditionValueType.Percent;
			PopulateIcons();
			base.PopulateCore();
		}
		protected virtual void PopulateIcons() {
			for(int n = 0; n < EImageCount; n++) {
				decimal d = decimal.Round(100m * ((EImageCount - 1 - n) / (decimal)EImageCount));
				Icons.Add(new FormatConditionIconSetIcon() { Value = d, PredefinedName = GetImageName(n), ValueComparison = FormatConditionComparisonType.GreaterOrEqual });
			}
		}
		protected virtual string GetImageName(int index) {
			return string.Format(EImageMask, index + 1);
		}
		protected virtual int EImageCount { get { return imageCount; } }
		protected virtual string EImageMask { get { return imageMask; } }
	}
}
