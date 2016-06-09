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
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
namespace DevExpress.Office.Services.Implementation {
	public class FontCharsetService : IFontCharacterSetService, IDisposable {
		FontCache cache;
		FontCharacterSet characterSet;
		#region IFontCharacterSetService implementation
		public void BeginProcessing(string fontName) {
#if SL || DXPORTABLE
			this.cache = new PrecalculatedMetricsFontCache(new DocumentLayoutUnitDocumentConverter());
#else
			this.cache = new GdiPlusFontCache(new DocumentLayoutUnitDocumentConverter());
#endif
			this.characterSet = cache.GetFontCharacterSet(fontName);
		}
		public bool ContainsChar(char ch) {
			if (characterSet == null)
				return false;
			return characterSet.ContainsChar(ch);
		}
		public void EndProcessing() {
			ClearCache();
		}
		#endregion
		void ClearCache() {
			if (cache != null) {
				cache.Dispose();
				cache = null;
			}
			characterSet = null;
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				ClearCache();
		}
		#endregion
	}
}
