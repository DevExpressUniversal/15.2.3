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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ModelCustomMark = DevExpress.XtraRichEdit.Model.CustomMark;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.API.Native {
	[ComVisible(true)]
	public interface CustomMark {
		DocumentPosition Position { get; }
		object UserData { get; }
	}
	#region ReadOnlyCustomMarkCollection
	[ComVisible(true)]
	public interface ReadOnlyCustomMarkCollection : ISimpleCollection<CustomMark> {
		ReadOnlyCustomMarkCollection Get(DocumentRange range);
		CustomMark GetByVisualInfo(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfo customMarkVisualInfo);
	}
	#endregion
	[ComVisible(true)]
	public interface CustomMarkCollection : ReadOnlyCustomMarkCollection {
		CustomMark Create(DocumentPosition position, object userData);
		void Remove(CustomMark customMark);
	}
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using DocumentModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	public class NativeCustomMark : CustomMark {
		readonly ModelCustomMark modelCustomMark;
		readonly NativeSubDocument document;
		bool isValid = true;
		public NativeCustomMark(NativeSubDocument document, ModelCustomMark modelCustomMark) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(modelCustomMark, "modelCustomMark");
			this.document = document;
			this.modelCustomMark = modelCustomMark;
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		protected NativeSubDocument Document { get { return document; } }
		protected internal ModelCustomMark ModelCustomMark { get { return modelCustomMark; } }
		protected DocumentPosition GetPosition() {
			return new NativeDocumentPosition(document, modelCustomMark.Position.Clone());
		}
		protected virtual void CheckValid() {
			if (!IsValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedCustomMarkError);
		}
		#region CustomMark Members
		public DocumentPosition Position {
			get {
				CheckValid();
				return GetPosition();
			}
		}
		public object UserData { get { return ModelCustomMark.UserData; } }
		#endregion
	}
	public class NativeCustomMarkCollection : List<NativeCustomMark>, CustomMarkCollection {
		readonly NativeSubDocument document;
		public NativeCustomMarkCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		#region ISimpleCollection<CustomMark> Members
		CustomMark ISimpleCollection<CustomMark>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<CustomMark> Members
		IEnumerator<CustomMark> IEnumerable<CustomMark>.GetEnumerator() {
			return new EnumeratorAdapter<CustomMark, NativeCustomMark>(this.GetEnumerator());
		}
		#endregion
		public CustomMark Create(DocumentPosition position, object userData) {
			document.CheckValid();
			Guard.ArgumentNotNull(position, "position");
			document.CheckDocumentPosition(position);
			int prevCount = this.Count;
			PieceTable.CreateCustomMark(document.NormalizeLogPosition(position.LogPosition), userData);
			if (this.Count > prevCount)
				return this[prevCount];
			else
				return null;
		}
		public void Remove(CustomMark customMark) {
			document.CheckValid();
			Guard.ArgumentNotNull(customMark, "customMark");
			int index = (this).IndexOf((NativeCustomMark)customMark);
			if (index < 0)
				Exceptions.ThrowArgumentException("customMark", customMark);
			PieceTable.DeleteCustomMark(index);
		}
		public CustomMark GetByVisualInfo(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfo customMarkVisualInfo) {
			document.CheckValid();
			Guard.ArgumentNotNull(customMarkVisualInfo, "customMarkVisualInfo");
			int count = this.Count;
			for (int i = 0; i < count; i++)
				if (customMarkVisualInfo.CustomMark == ((NativeCustomMark)this[i]).ModelCustomMark)
					return this[i];
			return null;
		}
		public ReadOnlyCustomMarkCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			DocumentModelPosition startRange = nativeRange.NormalizedStart.Position;
			DocumentModelPosition endRange = nativeRange.NormalizedEnd.Position;
			NativeCustomMarkCollection result = new NativeCustomMarkCollection(document);
			int count = PieceTable.CustomMarks.Count;
			for (int i = 0; i < count; i++) {
				DocumentModelPosition positionCustomMark = PieceTable.CustomMarks[i].Position;
				if ((positionCustomMark >= startRange) && (positionCustomMark <= endRange)) {
					NativeCustomMark customMark = new NativeCustomMark(document, PieceTable.CustomMarks[i]);
					result.Add(customMark);
				}
			}
			return result;
		}
	}
}
