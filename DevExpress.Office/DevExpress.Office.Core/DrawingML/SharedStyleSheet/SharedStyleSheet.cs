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
using DevExpress.Office;
using DevExpress.Office.DrawingML;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
namespace DevExpress.Office.Drawing {
	#region IDrawingCache
	public interface IDrawingCache {
		DrawingColorModelInfoCache DrawingColorModelInfoCache { get; }
		DrawingBlipFillInfoCache DrawingBlipFillInfoCache { get; }
		DrawingBlipTileInfoCache DrawingBlipTileInfoCache { get; }
		DrawingGradientFillInfoCache DrawingGradientFillInfoCache { get; }
		OutlineInfoCache OutlineInfoCache { get; }
		Scene3DPropertiesInfoCache Scene3DPropertiesInfoCache { get; }
		Scene3DRotationInfoCache Scene3DRotationInfoCache { get; }
		DrawingTextCharacterInfoCache DrawingTextCharacterInfoCache { get; }
		DrawingTextParagraphInfoCache DrawingTextParagraphInfoCache { get; }
		DrawingTextSpacingInfoCache DrawingTextSpacingInfoCache { get; }
		DrawingTextBodyInfoCache DrawingTextBodyInfoCache { get; }
	}
	#endregion
	#region DrawingUndoableIndexBasedObject<T> (abstract class)
	public abstract class DrawingUndoableIndexBasedObject<T> : UndoableIndexBasedObject<T, DocumentModelChangeActions>
		where T : ICloneable<T>, ISupportsCopyFrom<T>, ISupportsSizeOf {
		InvalidateProxy innerParent;
		protected DrawingUndoableIndexBasedObject(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
			this.innerParent = new InvalidateProxy();
		}
		protected internal IDrawingCache DrawingCache { get { return DocumentModel.DrawingCache; } }
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		protected ISupportsInvalidate InnerParent { get { return innerParent; } }
		protected internal override void ApplyChanges(DocumentModelChangeActions changeActions) {
			InvalidateParent();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None; 
		}
		public virtual void CopyFrom(DrawingUndoableIndexBasedObject<T> sourceItem) {
			if (Object.ReferenceEquals(sourceItem.DocumentModel, this.DocumentModel))
				base.CopyFrom(sourceItem);
			else {
				this.BeginUpdate();
				sourceItem.BeginUpdate();
				try {
					this.Info.CopyFrom(sourceItem.Info);
				} finally {
					this.EndUpdate();
					sourceItem.EndUpdate();
				}
			}
		}
		protected void InvalidateParent() {
			this.innerParent.Invalidate();
		}
		public void AssignInfo(T info) {
			SetIndexInitial(GetCache(DocumentModel).AddItem(info));
		}
	}
	#endregion
	#region DrawingMultiIndexObject<TOwner, TActions> (abstract class)
	public abstract class DrawingMultiIndexObject<TOwner, TActions> : MultiIndexObject<TOwner, TActions>
		where TOwner : DrawingMultiIndexObject<TOwner, TActions>
		where TActions : struct {
		InvalidateProxy innerParent;
		readonly IDocumentModel documentModel;
		protected DrawingMultiIndexObject(IDocumentModel documentModel) { 
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.innerParent = new InvalidateProxy();
			this.documentModel = documentModel;
		}
		public IDrawingCache DrawingCache { get { return documentModel.DrawingCache; } }
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		protected ISupportsInvalidate InnerParent { get { return innerParent; } }
		protected override IDocumentModel GetDocumentModel() {
			return documentModel;
		}
		protected internal override void ApplyChanges(TActions actions) {
			InvalidateParent();
		}
		protected void InvalidateParent() {
			this.innerParent.Invalidate();
		}
	}
	#endregion
	#region EventArgs
	#region SetFill
	public delegate void SetFillEventHandler(object sender, SetFillEventArgs e);
	public class SetFillEventArgs : EventArgs {
		readonly IDrawingFill fill;
		public SetFillEventArgs(IDrawingFill fill) {
			this.fill = fill;
		}
		public IDrawingFill Fill { get { return fill; } }
	}
	#endregion
	#endregion
}
namespace DevExpress.Office.Model {
}
