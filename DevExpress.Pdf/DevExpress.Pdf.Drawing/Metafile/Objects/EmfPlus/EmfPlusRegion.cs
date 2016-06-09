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
using System.Drawing.Drawing2D;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusRegion : PdfDisposableObject {
		static Graphics graphics;
		static EmfPlusRegion() {
			using (Bitmap bmp = new Bitmap(1, 1))
				graphics = Graphics.FromImage(bmp);
		}
		static Region Create(EmfPlusReader reader) {
			EmfPlusRegionNodeDataType type = EmfEnumToValueConverter.ParseEmfEnum<EmfPlusRegionNodeDataType>(reader.ReadInt32());
			Region region = null;
			switch (type) {
				case EmfPlusRegionNodeDataType.RegionNodeDataTypeRect:
					return new Region(reader.ReadRectF(false));
				case EmfPlusRegionNodeDataType.RegionNodeDataTypePath:
					reader.ReadInt32();
					EmfPlusPath path = new EmfPlusPath(reader);
					return new Region(new GraphicsPath(path.Points, path.Types));
				case EmfPlusRegionNodeDataType.RegionNodeDataTypeInfinite:
					region = new Region();
					region.MakeInfinite();
					return region;
				case EmfPlusRegionNodeDataType.RegionNodeDataTypeEmpty:
					region = new Region();
					region.MakeEmpty();
					return region;
				default:
					break;
			}
			Region left = Create(reader); ;
			using (Region right = Create(reader)) {
				EmfPlusCombineMode combineMode;
				switch (type) {
					case EmfPlusRegionNodeDataType.RegionNodeDataTypeAnd:
						combineMode = EmfPlusCombineMode.CombineModeIntersect;
						break;
					case EmfPlusRegionNodeDataType.RegionNodeDataTypeOr:
						combineMode = EmfPlusCombineMode.CombineModeUnion;
						break;
					case EmfPlusRegionNodeDataType.RegionNodeDataTypeXor:
						combineMode = EmfPlusCombineMode.CombineModeXOR;
						break;
					case EmfPlusRegionNodeDataType.RegionNodeDataTypeExclude:
						combineMode = EmfPlusCombineMode.CombineModeExclude;
						break;
					case EmfPlusRegionNodeDataType.RegionNodeDataTypeComplement:
						combineMode = EmfPlusCombineMode.CombineModeComplement;
						break;
					default:
						left.Dispose();
						return null;
				}
				Combine(combineMode, left, right);
			}
			return left;
		}
		static void Combine(EmfPlusCombineMode mode, Region left, Region rigth) {
			switch (mode) {
				case EmfPlusCombineMode.CombineModeUnion:
					left.Union(rigth);
					break;
				case EmfPlusCombineMode.CombineModeIntersect:
					left.Intersect(rigth);
					break;
				case EmfPlusCombineMode.CombineModeXOR:
					left.Xor(rigth);
					break;
				case EmfPlusCombineMode.CombineModeExclude:
					left.Exclude(rigth);
					break;
				case EmfPlusCombineMode.CombineModeComplement:
					left.Complement(rigth);
					break;
				default:
					break;
			}
		}
		Region region;
		public EmfPlusRegion(EmfPlusReader reader) {
			reader.ReadInt32();
			int count = reader.ReadInt32();
			region = Create(reader);
		}
		public EmfPlusRegion(Region region) {
			this.region = region;
		}
		public RectangleF GetClip() {
			return region.GetBounds(graphics);
		}
		public void Transform(Matrix transformMatrix) {
			region.Transform(transformMatrix);
		}
		public void Combine(EmfPlusCombineMode mode, EmfPlusRegion newRegion) {
			if (mode == EmfPlusCombineMode.CombineModeReplace)
				region = newRegion.region.Clone();
			else
				Combine(mode, region, newRegion.region);
		}
		public EmfPlusRegion Clone() {
			return new EmfPlusRegion(region.Clone());
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				region.Dispose();
		}
	}
}
