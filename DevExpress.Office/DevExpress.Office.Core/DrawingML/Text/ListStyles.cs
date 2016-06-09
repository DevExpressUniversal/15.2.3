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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
using DevExpress.Utils;
namespace DevExpress.Office.Drawing {
	public class DrawingTextListStyles : ISupportsCopyFrom<DrawingTextListStyles> {
		public const int Count = 9;
		readonly InvalidateProxy innerParent;
		readonly DrawingTextParagraphProperties defaultParagraphStyle;
		readonly DrawingTextParagraphProperties[] listLevelStyles;
		public DrawingTextListStyles(IDocumentModel documentModel) {
			this.innerParent = new InvalidateProxy();
			this.defaultParagraphStyle = new DrawingTextParagraphProperties(documentModel) { Parent = this.innerParent };
			this.listLevelStyles = new DrawingTextParagraphProperties[Count];
			for (int i = 0; i < Count; i++)
				this.listLevelStyles[i] = new DrawingTextParagraphProperties(documentModel) { Parent = this.innerParent };
		}
		#region Properties
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public DrawingTextParagraphProperties DefaultParagraphStyle { get { return defaultParagraphStyle; } }
		public DrawingTextParagraphProperties this[int index] { get { return listLevelStyles[index]; } }
		public bool IsDefault {
			get {
				bool result = DefaultParagraphStyle.IsDefault;
				for (int i = 0; i< Count; i++ )
					result &= this[i].IsDefault;
				return result;
			}
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextListStyles> Members
		public void CopyFrom(DrawingTextListStyles value) {
			Guard.ArgumentNotNull(value, "value");
			this.defaultParagraphStyle.CopyFrom(value.defaultParagraphStyle);
			for (int i = 0; i < Count; i++)
				this.listLevelStyles[i].CopyFrom(value.listLevelStyles[i]);
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingTextListStyles other = obj as DrawingTextListStyles;
			if (other == null)
				return false;
			if (!this.defaultParagraphStyle.Equals(other.defaultParagraphStyle))
				return false;
			for (int i = 0; i < Count; i++) {
				if (!this.listLevelStyles[i].Equals(other.listLevelStyles[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = this.defaultParagraphStyle.GetHashCode();
			for (int i = 0; i < Count; i++)
				result ^= this.listLevelStyles[i].GetHashCode();
			return result;
		}
		public void ResetToStyle() {
			this.defaultParagraphStyle.ResetToStyle();
			for (int i = 0; i < Count; i++)
				this.listLevelStyles[i].ResetToStyle();
		}
	}
}
