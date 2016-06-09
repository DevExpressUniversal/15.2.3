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
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.API.Native {
	#region DocumentPosition (abstract class)
	[ComVisible(true)]
	public abstract class DocumentPosition : IComparable<DocumentPosition> {
		#region Operators
		public static bool operator ==(DocumentPosition pos1, DocumentPosition pos2) {
			if (Object.ReferenceEquals(pos1, null) && Object.ReferenceEquals(pos2, null))
				return true;
			if (Object.ReferenceEquals(pos1, null) || Object.ReferenceEquals(pos2, null))
				return false;
			return pos1.EqualsCore(pos2);
		}
		public static bool operator !=(DocumentPosition pos1, DocumentPosition pos2) {
			return !(pos1 == pos2);
		}
		public static bool operator <(DocumentPosition pos1, DocumentPosition pos2) {
			return pos1.LessThan(pos2);
		}
		public static bool operator <=(DocumentPosition pos1, DocumentPosition pos2) {
			return (pos1 < pos2) || pos1.EqualsCore(pos2);
		}
		public static bool operator >(DocumentPosition pos1, DocumentPosition pos2) {
			return !(pos1 <= pos2);
		}
		public static bool operator >=(DocumentPosition pos1, DocumentPosition pos2) {
			return !(pos1 < pos2);
		}
		#endregion
		public override bool Equals(object obj) {
			return EqualsCore((DocumentPosition)obj);
		}
		public override int GetHashCode() {
			return GetHashCodeCore();
		}
		public int ToInt() {
			return ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)LogPosition).ToInt();
		}
		#region IComparable<DocumentPosition> Members
		int IComparable<DocumentPosition>.CompareTo(DocumentPosition other) {
			return CompareToCore(other);
		}
		#endregion
		protected internal abstract DevExpress.XtraRichEdit.Model.DocumentLogPosition LogPosition { get; }
		protected internal abstract bool EqualsCore(DocumentPosition pos);
		protected internal abstract bool LessThan(DocumentPosition pos);
		protected internal abstract int GetHashCodeCore();
		protected internal abstract int CompareToCore(DocumentPosition pos);
		public override string ToString() {
			return LogPosition.ToString();
		}
		public abstract SubDocument BeginUpdateDocument();
		public abstract void EndUpdateDocument(SubDocument document);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelPositionAnchor = DevExpress.XtraRichEdit.Model.DocumentModelPositionAnchor;
	#region NativeDocumentPosition
	public class NativeDocumentPosition : DocumentPosition {
		readonly NativeSubDocument document;
		readonly ModelPositionAnchor anchor;
		internal NativeDocumentPosition(NativeSubDocument document, ModelPosition pos) {
			this.anchor = new ModelPositionAnchor(pos.Clone());
			this.document = document;
			document.InternalAPI.RegisterAnchor(this.anchor);
		}
		public ModelPosition Position { get { return anchor.Position; } }
		protected internal override DevExpress.XtraRichEdit.Model.DocumentLogPosition LogPosition { get { return Position.LogPosition; } }
		protected internal NativeSubDocument Document { get { return document; } }
		protected internal override bool EqualsCore(DocumentPosition pos) {
			return LogPosition == pos.LogPosition;
		}
		protected internal override int GetHashCodeCore() {
			return ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)LogPosition).ToInt();
		}
		protected internal override int CompareToCore(DocumentPosition pos) {
			return ((IComparable<ModelLogPosition>)LogPosition).CompareTo(pos.LogPosition);
		}
		protected internal override bool LessThan(DocumentPosition pos) {
			return LogPosition < pos.LogPosition;
		}
		public override SubDocument BeginUpdateDocument() {
			document.ReferenceCount++;
			return document;
		}
		public override void EndUpdateDocument(SubDocument document) {
			NativeSubDocument nativeDocument = (NativeSubDocument)document;
			nativeDocument.ReferenceCount--;
		}
	}
	#endregion
}
