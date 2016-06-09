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
namespace DevExpress.Office.Drawing {
	#region DrawingTextFont
	public class DrawingTextFont : ICloneable<DrawingTextFont>, ISupportsCopyFrom<DrawingTextFont>, IDrawingBullet {
		#region Fields
		readonly IDocumentModel documentModel;
		readonly InvalidateProxy innerParent;
		public const byte DefaultCharset = 1;
		public const byte DefaultPitchFamily = 0;
		const int TypefaceIndex = 0;
		const int PanoseIndex = 1;
		const int CharsetIndex = 0;
		const int PitchFamilyIndex = 1;
		string[] stringArray;
		byte[] byteArray;
		#endregion
		public DrawingTextFont(IDocumentModel documentModel) {
			this.innerParent = new InvalidateProxy();
			this.documentModel = documentModel;
			stringArray = new string[2] { String.Empty, String.Empty };
			byteArray = new byte[2] { DefaultCharset, DefaultPitchFamily };
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return documentModel; } }
		protected internal ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public string Typeface { get { return stringArray[TypefaceIndex]; } set { SetStringArray(TypefaceIndex, value); } }
		public string Panose { get { return stringArray[PanoseIndex]; } set { SetStringArray(PanoseIndex, value); } }
		public byte Charset { get { return byteArray[CharsetIndex]; } set { SetByteArray(CharsetIndex, value); } }
		public byte PitchFamily { get { return byteArray[PitchFamilyIndex]; } set { SetByteArray(PitchFamilyIndex, value); } }
		public bool IsDefault {
			get {
				return String.IsNullOrEmpty(stringArray[TypefaceIndex]) &&
					   String.IsNullOrEmpty(stringArray[PanoseIndex]) &&
					   byteArray[CharsetIndex] == DefaultCharset &&
					   byteArray[PitchFamilyIndex] == DefaultPitchFamily;
			}
		}
		#endregion
		#region SetMethods
		void SetStringArray(int index, string value) {
			if (stringArray[index] != value)
				ApplyHistoryItem(new DrawingTextFontStringChangedHistoryItem(this, index, stringArray[index], value));
		}
		protected internal void SetStringCore(int index, string value) {
			stringArray[index] = value;
			this.innerParent.Invalidate();
		}
		void SetByteArray(int index, byte value) {
			if (byteArray[index] != value)
				ApplyHistoryItem(new DrawingTextFontByteChangedHistoryItem(this, index, byteArray[index], value));
		}
		protected internal void SetByteCore(int index, byte value) {
			byteArray[index] = value;
			this.innerParent.Invalidate();
		}
		void ApplyHistoryItem(HistoryItem item) {
			documentModel.History.Add(item);
			item.Execute();
		}
		#endregion
		#region CopyFrom
		public void CopyFrom(DrawingTextFont value) {
			stringArray[TypefaceIndex] = value.stringArray[TypefaceIndex];
			stringArray[PanoseIndex] = value.stringArray[PanoseIndex];
			byteArray[CharsetIndex] = value.byteArray[CharsetIndex];
			byteArray[PitchFamilyIndex] = value.byteArray[PitchFamilyIndex];
		}
		#endregion
		#region Clone
		public DrawingTextFont Clone() {
			DrawingTextFont result = new DrawingTextFont(documentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextFont other = obj as DrawingTextFont;
			if (other == null)
				return false;
			return 
				this.stringArray[TypefaceIndex] == other.stringArray[TypefaceIndex] &&
				this.stringArray[PanoseIndex] == other.stringArray[PanoseIndex] &&
				this.byteArray[CharsetIndex] == other.byteArray[CharsetIndex] &&
				this.byteArray[PitchFamilyIndex] == other.byteArray[PitchFamilyIndex];
		}
		public override int GetHashCode() {
			return 
				stringArray[TypefaceIndex].GetHashCode() ^ stringArray[PanoseIndex].GetHashCode() ^ 
				byteArray[CharsetIndex] ^ byteArray[PitchFamilyIndex];
		}
		#endregion
		#region Clear
		public void Clear() {
			documentModel.BeginUpdate();
			try {
				Typeface = String.Empty;
				Panose = String.Empty;
				Charset = DefaultCharset;
				PitchFamily = DefaultPitchFamily;
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		#endregion
		#region IDrawingBullet Members
		DrawingBulletType IDrawingBullet.Type { get { return DrawingBulletType.Typeface; } }
		IDrawingBullet IDrawingBullet.CloneTo(IDocumentModel documentModel) {
			DrawingTextFont result = new DrawingTextFont(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
	}
	#endregion
}
