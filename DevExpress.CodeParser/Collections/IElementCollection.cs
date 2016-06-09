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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum CollectionType
  {
	Unknown,
	IElementCollection,
	AttributeElementCollection,
	CaseClauseCollection,
	CaseStatementCollection,
	ExpressionCollection,
	HtmlAttributeCollection,
	JoinExpressionCollection,
	MemberElementCollection,
	NamespaceElementCollection,
	ParameterElementCollection,
	ProjectElementCollection,
	QueryIdentCollection,
	SourceFileCollection,
	TypeElementCollection,
	TypeParameterCollection,
	TypeParameterConstraintCollection,
	TypeReferenceExpressionCollection,
	VariableDeclarationStatementCollection,
	XmlAttributeDeclarationCollection,
	XmlContentParticleCollection,
	TextRangeCollection,
	LiteElementCollection,
	CssSelectorCollection,
	CssExpressionCollection,
	CssPropertyDeclarationCollection,
	CssElementCollection,
	CssTermCollection
  }
  public interface ISerializableCollection : IList
  {
	CollectionType CollectionType { get; }
  }
	[Serializable]	
	public class IElementCollection : NodeList, IEnumerable<IElement>, ISerializableCollection
	{
		public new static readonly IElementCollection Empty = new EmptyIElementCollection();
		public IElementCollection() : this(1)
		{ 
		}
		public IElementCollection(int capacity)
			: base(capacity)
		{
		}
	class IElementCollectionEnumerator : IEnumerator<IElement>
	{
	  IEnumerator _Enumerator;
	  public IElementCollectionEnumerator(IEnumerator enumerator)
	  {
		_Enumerator = enumerator;
	  }
	  object IEnumerator.Current
	  {
		get { return Current; }
	  }
	  public void Dispose()
	  {
		_Enumerator = null;
	  }
	  public bool MoveNext()
	  {
		if (_Enumerator == null)
		  return false;
		return _Enumerator.MoveNext();
	  }
	  public void Reset()
	  {
		if (_Enumerator == null)
		  return;
		_Enumerator.Reset();
	  }
	  public IElement Current
	  {
		get
		{
		  if (_Enumerator == null)
			return null;
		  return _Enumerator.Current as IElement;
		}
	  }
	}
	IEnumerator<IElement> IEnumerable<IElement>.GetEnumerator()
	{
	  IEnumerator enumerator = GetEnumerator();
	  return new IElementCollectionEnumerator(enumerator);
	}
		protected override NodeList CreateInstance()
		{
			return new IElementCollection();
		}
		#region Add
		public virtual int Add(IElement element)
		{
			return base.Add(element);
		}
		#endregion
	public void AddRange(IEnumerable<IElement> elements)
	{
	  if (elements == null)
		return;
	  foreach (IElement element in elements)
		Add(element);
	}
		#region IndexOf
		public int IndexOf(IElement element)
		{
			return base.IndexOf(element);
		}
		#endregion
		#region Insert
		public virtual void Insert(int index, IElement element)
		{
			base.Insert(index, element);
		}
		#endregion
		#region Remove
		public virtual void Remove(IElement element)
		{
			base.Remove(element);
		}
		#endregion		
		#region Find
		public IElement Find(IElement element)
		{
			int lIdx = IndexOf(element);
			if (lIdx < 0)
				return null;
			return this[lIdx];
		}
		#endregion
		#region Contains
		public bool Contains(IElement element)
		{
			return Find(element) != null;
		}
		#endregion
		public IElement FindElementByFullName(string fullName, bool caseSensitive)
		{
	  return StructuralParserServicesHolder.FindElementByFullName(this, fullName, caseSensitive);
		}
		public new IElementCollection DeepClone()
		{
			return base.DeepClone(ElementCloneOptions.Default) as IElementCollection;
		}
		public new IElementCollection DeepClone(ElementCloneOptions options)
		{						
			return base.DeepClone(options) as IElementCollection;
		}
		public LanguageElementCollection ToLanguageElementCollection()
		{
			return LanguageElementRestorer.ConvertToLanguageElements(this);
		}
		public static int IndexOf(IElement[] elements, IElement element)
		{
			int count = elements.Length;
			for (int i = 0; i < count; i++)
			{
				IElement test = elements[i];
				if (test == element)
					return i;
			}
			return -1;
		}
		public IEnumerable<IElement> GetEnumerable()
		{
			foreach (IElement element in this)
				yield return element;
		}
	public SourceRange GetRange()
	{
	  return GetRange(this);
	}
	public static SourceRange GetRange(IElement element)
	{
	  if (element == null)
		return SourceRange.Empty;
	  SourceRange range = element.FirstRange;
	  if (range.IsEmpty)
		range = element.FirstNameRange;
	  return range;
	}
	public static SourceRange GetRange(IEnumerable<IElement> elements)
	{
	  if (elements == null)
		return SourceRange.Empty;
	  SourceRange range = SourceRange.Empty;
	  foreach (IElement element in elements)
		range = SourceRange.Union(range, GetRange(element));
	  return range;
	}
		#region this[int aIndex]
		public new IElement this[int index] 
		{
			get
			{
				return base[index] as IElement;
			}
			set
			{
				base[index] = value;
			}
		}
		#endregion
	public CollectionType CollectionType
	{
	  get
	  {
		return CollectionType.IElementCollection;
	  }
	}
  }
	[Serializable]
	internal class EmptyIElementCollection : IElementCollection
	{
		public override int Add(IElement element)
		{
			throw new Exception("Can not modify EmptyIElementCollection object.");
		}
		public override void Insert(int index, IElement element)
		{
			throw new Exception("Can not modify EmptyIElementCollection object.");
		}
		public override void Remove(IElement element)
		{
			throw new Exception("Can not modify EmptyIElementCollection object.");
		}
		public new IElement this[int index] 
		{
			get
			{
				return base[index];
			}
			set
			{
				throw new Exception("Can not modify EmptyIElementCollection object.");
			}
		}
	}
}
