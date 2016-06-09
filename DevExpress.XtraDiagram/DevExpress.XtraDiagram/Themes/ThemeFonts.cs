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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Themes {
	public class DiagramThemeFonts : IDisposable {
		Dictionary<DiagramThemeFontKey, Font> fonts;
		public DiagramThemeFonts() {
			this.fonts = new Dictionary<DiagramThemeFontKey, Font>();
		}
		public Font GetFont(string family, double size) {
			return GetFont(family, (float)size);
		}
		public Font GetFont(string family, float size) {
			DiagramThemeFontKey key = new DiagramThemeFontKey(family, size);
			return this.fonts.GetOrAdd(key, () => CreateFont(family, size));
		}
		protected Font CreateFont(string family, float size) {
			return new Font(family, size);
		}
		#region IDispose
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.fonts != null) {
					this.fonts.ForEachValue(font => font.Dispose());
					this.fonts.Clear();
				}
				this.fonts = null;
			}
		}
		#endregion
	}
	public struct DiagramThemeFontKey {
		readonly string family;
		readonly float size;
		public DiagramThemeFontKey(string family, float size) {
			this.family = family;
			this.size = size;
		}
		public float Size { get { return size; } }
		public string Family { get { return family; } }
		public override bool Equals(object obj) {
			DiagramThemeFontKey other = (DiagramThemeFontKey)obj;
			return Size == other.Size && Family == other.Family;
		}
		public override int GetHashCode() {
			return Size.GetHashCode() ^ Family.GetHashCode();
		}
		public override string ToString() { return string.Format("{0}: {1}", Family, Size.ToString()); }
	}
}
