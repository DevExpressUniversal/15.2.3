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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class QueuedDelete: QueuedEdit
	{
		const string STR_Delete = "Delete";
		private LanguageElementCollection _SeveredNodes;
		private LanguageElement _StartDelete;
		private LanguageElement _EndDelete;		
		#region QueuedDelete()
		protected QueuedDelete()
		{			
		}
		#endregion
		#region QueuedDelete(SourceRange sourceRange)
		public QueuedDelete(SourceRange sourceRange)
		{
			SetRange(sourceRange);
			InternalName = STR_Delete;
		}
		#endregion
		#region QueuedDelete(LanguageElement element)
		public QueuedDelete(LanguageElement element)
		{
			ValidateElement(element);
			_StartDelete = element;
			_EndDelete = element;
			SetRange(element.Range);
			InternalName = STR_Delete;
			BorrowNodeFromTree(element);
		}
		#endregion
		#region QueuedDelete(LanguageElement firstSibling, LanguageElement lastSibling)
		public QueuedDelete(LanguageElement firstSibling, LanguageElement lastSibling)
		{
			ValidateSiblings(firstSibling, lastSibling);
			SetRange(new SourceRange(firstSibling.Range.Start, lastSibling.Range.End));
			InternalName = STR_Delete;
			BorrowNodesFromTree(firstSibling, lastSibling);
		}
		#endregion
		#region ValidateElement
		private void ValidateElement(LanguageElement element)
		{
			if (element == null)
				throw new ArgumentException("element parameter to Delete constructor must be assigned!");		
		}
		#endregion
		#region ValidateSiblings
		private void ValidateSiblings(LanguageElement firstSibling, LanguageElement lastSibling)
		{
			if (firstSibling == null)
				throw new ArgumentException("firstSibling parameter to Delete constructor must be assigned!");		
			if (lastSibling == null)
				throw new ArgumentException("lastSibling parameter to Delete constructor must be assigned!");		
			if (firstSibling == lastSibling)		
				return;		
			if (!firstSibling.IsSibling(lastSibling))
				throw new ArgumentException("Parameters to Delete constructor must be siblings!");		
			if (firstSibling.Index >= lastSibling.Index)
				throw new ArgumentException("firstSibling must come *before* lastSibling in Delete constructor!");		
		}
		#endregion
		#region BorrowNodesFromTree
		private void BorrowNodesFromTree(LanguageElement firstSibling, LanguageElement lastSibling)
		{
			LanguageElement lThisElement = lastSibling;		
			while (lThisElement != null)
			{
				BorrowNodeFromTree(lThisElement);
				if (lThisElement == firstSibling)
					break;
				lThisElement = lThisElement.PreviousSibling;
			}
		}
		#endregion
		#region BorrowNodeFromTree
		private void BorrowNodeFromTree(LanguageElement element)
		{
			SeveredNodes.Add(element);
		}
		#endregion
		#region ApplyEdit
		protected override void ApplyEdit(IDocument iDocument, bool format)
		{
			iDocument.DeleteText(BoundRange);
			SetRange(BoundRange.Start, BoundRange.Start);
			SeveredNodes.Clear();
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is QueuedDelete))
				return;
			QueuedDelete lSource = (QueuedDelete)source;
			if (lSource._StartDelete != null)
			{				
				_StartDelete = ParserUtils.GetCloneFromNodes(this, lSource, lSource._StartDelete) as LanguageElement;
				if (_StartDelete == null)
					_StartDelete = lSource._StartDelete.Clone(options) as LanguageElement;
			}
			if (lSource._EndDelete != null)
			{				
				_EndDelete = ParserUtils.GetCloneFromNodes(this, lSource, lSource._EndDelete) as LanguageElement;
				if (_EndDelete == null)
					_EndDelete = lSource._EndDelete.Clone(options) as LanguageElement;
			}
			if (lSource._SeveredNodes != null)
			{
				_SeveredNodes = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _SeveredNodes, lSource._SeveredNodes);
				if (_SeveredNodes.Count == 0 && lSource._SeveredNodes.Count > 0)
					ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _SeveredNodes, lSource._SeveredNodes);
				if (_SeveredNodes.Count == 0 && lSource._SeveredNodes.Count > 0)
					_SeveredNodes = lSource._SeveredNodes.DeepClone(options) as LanguageElementCollection;
			}		
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QueuedDelete lClone = new QueuedDelete();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.QueuedDelete;
			}
		}
		#endregion
		#region SeveredNodes
		[EditorBrowsable(EditorBrowsableState.Never)]		
		public LanguageElementCollection SeveredNodes
		{
			get
			{
				if (_SeveredNodes == null)
					_SeveredNodes = new LanguageElementCollection();
				return _SeveredNodes;
			}
		}
		#endregion
	}
}
