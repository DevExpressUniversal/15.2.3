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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Utils;
namespace DevExpress.Office.Drawing {
	#region FontInfoMeasurer
	public abstract class FontInfoMeasurer : IDisposable {
		#region Fields
		bool isDisposed;
		readonly DocumentLayoutUnitConverter unitConverter;
		#endregion
		protected FontInfoMeasurer(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			Initialize();
		}
		protected internal abstract void Initialize();
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		internal DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		#endregion
		public virtual int MeasureCharacterWidth(char character, FontInfo fontInfo) {
			return (int)Math.Ceiling(MeasureCharacterWidthF(character, fontInfo));
		}
		public abstract float MeasureCharacterWidthF(char character, FontInfo fontInfo);
		public abstract Size MeasureString(string text, FontInfo fontInfo);
		public abstract float MeasureMaxDigitWidthF(FontInfo fontInfo);
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
}
