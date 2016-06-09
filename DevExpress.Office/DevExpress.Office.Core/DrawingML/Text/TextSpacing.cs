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

using DevExpress.Office;
using DevExpress.Utils;
using System.Runtime.InteropServices;
namespace DevExpress.Office.Drawing {
	#region DrawingTextSpacingValueType
	public enum DrawingTextSpacingValueType { 
		Automatic,
		Percent,
		Points
	}
	#endregion
	#region DrawingTextSpacingInfo
	public class DrawingTextSpacingInfo : ICloneable<DrawingTextSpacingInfo>, ISupportsCopyFrom<DrawingTextSpacingInfo>, ISupportsSizeOf {
		#region Fields
		DrawingTextSpacingValueType type;
		int value;
		#endregion
		#region Properties
		public DrawingTextSpacingValueType Type { 
			get { return type; } 
			set {
				if (value == DrawingTextSpacingValueType.Automatic)
					this.value = 0;
				type = value; 
			} 
		}
		public int Value { get { return this.value; } set { this.value = value; } }
		#endregion
		#region ICloneable<DrawingTextSpacingInfo> Members
		public DrawingTextSpacingInfo Clone() {
			DrawingTextSpacingInfo result = new DrawingTextSpacingInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextSpacingInfo> Members
		public void CopyFrom(DrawingTextSpacingInfo value) {
			this.type = value.type;
			this.value = value.value;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextSpacingInfo other = obj as DrawingTextSpacingInfo;
			if (other == null)
				return false;
			return this.type == other.type && this.value == other.value;
		}
		public override int GetHashCode() {
			return
				(int)this.type ^ value;
		}
		#endregion
	}
	#endregion
	#region DrawingTextSpacingInfoCache
	public class DrawingTextSpacingInfoCache : UniqueItemsCache<DrawingTextSpacingInfo> {
		public const int DefaultItemIndex = 0;
		public DrawingTextSpacingInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingTextSpacingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingTextSpacingInfo();
		}
	}
	#endregion
	#region IDrawingTextSpacing
	public interface IDrawingTextSpacing {
		DrawingTextSpacingValueType Type { get; set; }
		int Value { get; set; }
	}
	#endregion
}
