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
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.Data.XtraReports.Wizard {
	public class CustomLabelInformation : ICloneable {
		public float Width { get; set; }
		public float Height { get; set; }
		public float VerticalPitch { get; set; }
		public float HorizontalPitch { get; set; }
		public float TopMargin { get; set; }
		public float LeftMargin { get; set; }
		public int PaperKindDataId { get; set; }
		public GraphicsUnit Unit { get; set; }
		public override bool Equals(object obj) {
			var other = obj as CustomLabelInformation;
			if(other == null)
				return false;
			return
				Width == other.Width &&
				Height == other.Height &&
				VerticalPitch == other.VerticalPitch &&
				HorizontalPitch == other.HorizontalPitch &&
				TopMargin == other.TopMargin &&
				LeftMargin == other.LeftMargin &&
				PaperKindDataId == other.PaperKindDataId &&
				Unit == other.Unit;
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode(Width, Height, VerticalPitch, HorizontalPitch, TopMargin, LeftMargin, PaperKindDataId, Unit);
		}
		public object Clone() {
			var clone = new CustomLabelInformation() {
				Width = this.Width,
				Height = this.Height,
				VerticalPitch = this.VerticalPitch,
				HorizontalPitch = this.HorizontalPitch,
				TopMargin = this.TopMargin,
				LeftMargin = this.LeftMargin,
				PaperKindDataId = this.PaperKindDataId,
				Unit = this.Unit,
			};
			return clone;
		}
	}
}
