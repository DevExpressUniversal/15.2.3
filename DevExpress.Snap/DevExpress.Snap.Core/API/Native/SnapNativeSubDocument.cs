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

using DevExpress.Utils;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
namespace DevExpress.Snap.API.Native {
	public class SnapNativeSubDocument : NativeSubDocument, SnapSubDocument {
		SnapNativeFieldOwner fieldOwner;
		internal SnapNativeSubDocument(SnapPieceTable pieceTable, InnerRichEditDocumentServer server, SnapNativeDocument mainDocument)
			: base(pieceTable, server) {
				this.fieldOwner = new SnapNativeFieldOwner(this, mainDocument);
		}
		public new SnapDocumentRange Range { get { return (SnapDocumentRange)base.Range; } }
		#region SnapSubDocument members
		internal void SetActiveEntity(SnapEntity value) {
			this.fieldOwner.SetActiveEntity(value);
		}
		public SnapEntity ActiveEntity { get { return fieldOwner.ActiveEntity; } }
		public SnapList FindListByName(string name) {
			return fieldOwner.FindListByName(name);
		}
		public SnapList CreateSnList(DocumentPosition position, string name) {
			return fieldOwner.CreateSnList(position, name);
		}
		public void RemoveSnList(string name) {
			fieldOwner.RemoveSnList(name);
		}
		public SnapEntity ParseField(Field field) {
			return fieldOwner.ParseField(field);
		}
		public SnapEntity ParseField(int index) {
			return fieldOwner.ParseField(index);
		}
		public void RemoveField(int index) {
			fieldOwner.RemoveField(index);
		}
		public SnapText CreateSnText(DocumentPosition position, string dataFieldName) {
			return fieldOwner.CreateSnText(position, dataFieldName);
		}
		public SnapImage CreateSnImage(DocumentPosition position, string dataFieldName) {
			return fieldOwner.CreateSnImage(position, dataFieldName);
		}
		public SnapCheckBox CreateSnCheckBox(DocumentPosition position, string dataFieldName) {
			return fieldOwner.CreateSnCheckBox(position, dataFieldName);
		}
		public SnapBarCode CreateSnBarCode(DocumentPosition position) {
			return fieldOwner.CreateSnBarCode(position);
		}
		public SnapSparkline CreateSnSparkline(DocumentPosition position, string dataFieldName) {
			return fieldOwner.CreateSnSparkline(position, dataFieldName);
		}
		public SnapHyperlink CreateSnHyperlink(DocumentPosition position, string dataFieldName) {
			return fieldOwner.CreateSnHyperlink(position, dataFieldName);
		}
		#endregion
		public new SnapDocumentRange CreateRange(int start, int length) {
			return (SnapDocumentRange)base.CreateRange(start, length);
		}
		public new SnapDocumentRange CreateRange(DocumentPosition start, int length) {
			return (SnapDocumentRange)base.CreateRange(start, length);
		}
		public new SnapDocumentPosition CreatePosition(int start) {
			return (SnapDocumentPosition)base.CreatePosition(start);
		}
		protected internal new SnapDocumentRange CreateRange(ModelLogPosition start, int length) {
			return CreateRange(((IConvertToInt<ModelLogPosition>)start).ToInt(), length);
		}
		protected internal new NativeDocumentRange CreateZeroLengthRange(ModelLogPosition logPosition) {
			return new SnapDocumentRange((SnapDocumentPosition)CreatePositionCore(logPosition), (SnapDocumentPosition)CreatePositionCore(logPosition));
		}
		protected override NativeDocumentPosition CreateNativePosition(XtraRichEdit.Model.DocumentModelPosition pos) {
			return new SnapDocumentPosition(this, pos);
		}
		protected override NativeDocumentRange CreateNativeRange(ModelPosition start, ModelPosition end) {
			return new SnapDocumentRange(this, start, end);
		}
	}
}
