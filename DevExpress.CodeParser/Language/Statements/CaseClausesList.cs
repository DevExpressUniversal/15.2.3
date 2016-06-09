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
	public class CaseClausesList : LanguageElement, ICaseClausesList
	{
		private const int INT_MaintainanceComplexity = 2;
		NodeList _Clauses;
		#region CaseClausesList
		public CaseClausesList()
		{
			_Clauses = new NodeList();
		}
		#endregion
	public CaseClausesList(NodeList clauses)
	{
	  _Clauses = clauses;
	}
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Clauses == null)
				return;
			int lIndex = _Clauses.IndexOf(oldElement);
			if (lIndex >= 0)
			{
				_Clauses.RemoveAt(lIndex);
				if (newElement != null)
					_Clauses.Insert(lIndex, newElement);
			}
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is CaseClausesList))
				return;
			CaseClausesList lSource = (CaseClausesList)source;			
			if (lSource._Clauses != null)
			{
				_Clauses = new NodeList();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Clauses, lSource._Clauses);
				if (_Clauses.Count == 0 && lSource._Clauses.Count > 0)
					_Clauses = lSource._Clauses.DeepClone(options) as NodeList;
			}			
		}
		#endregion
		#region GetImageIndex()
		public override int GetImageIndex()
		{
			return ImageIndex.SwitchStatement;
		}
		#endregion
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_Clauses.Clear();
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Clauses.Clear();
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			string lComma = String.Empty;
			foreach (CaseClause lClause in _Clauses)
			{
				lResult += lComma + lClause.ToString();
				lComma = ", ";
			}
			return lResult;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			CaseClausesList lClone = new CaseClausesList();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region Clauses
		public NodeList Clauses
		{
			get
			{
				return _Clauses;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.CaseClausesList;
			}
		}
		#region ICaseClausesList Members
		ICaseClauseCollection ICaseClausesList.Clauses
		{
			get 
			{
				if (_Clauses == null)
					return EmptyLiteElements.EmptyICaseClauseCollection;
				LiteCaseClauseCollection clauses = new LiteCaseClauseCollection();
				clauses.AddRange(_Clauses);
				return clauses;
			}
		}
		#endregion
	}
}
