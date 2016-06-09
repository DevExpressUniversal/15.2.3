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

using System.Drawing;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraPrinting.XamlExport {
	public class XamlLineStyle : XamlResourceBase {
		Color stroke;
		float strokeThickness;
		float[] strokeDashArray;
		public Color Stroke { get { return stroke; } }
		public float StrokeThickness { get { return strokeThickness; } }
		public float[] StrokeDashArray { get { return strokeDashArray; } }
		public XamlLineStyle(Color stroke, float strokeThickness, float[] strokeDashArray) {
			this.stroke = stroke;
			this.strokeThickness = strokeThickness;
			this.strokeDashArray = strokeDashArray;
		}
		public override bool Equals(object obj) {
			XamlLineStyle style = obj as XamlLineStyle;
			if(style == null)
				return false;
			return (style.Stroke == stroke && style.StrokeThickness == strokeThickness && FloatArrayEquals(style.StrokeDashArray ,strokeDashArray));
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode(Stroke.GetHashCode(), StrokeThickness.GetHashCode(),
				HashCodeHelper.CalcHashCode(ArrayHelper.ConvertAll<float, int>(StrokeDashArray, delegate(float value) { return (int)value; })));
		}
		static bool FloatArrayEquals(float[] array1, float[] array2) {
			if(array1.Length != array2.Length) {
				return false;
			}
			for(int i = 0; i < array1.Length; i++) {
				if(array1[i] != array2[i]) {
					return false;
				}
			}
			return true;
		}
	}
}
