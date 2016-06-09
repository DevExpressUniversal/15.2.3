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
using System.Text;
using System.Collections;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class AttributeSection : SupportElement, IAttributeSectionElement, IHasAttributesModifier
	{
		NodeList _AttributeCollection;
		#region AttributeSection
		public AttributeSection()
		{
			_AttributeCollection = new NodeList();
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AttributeSection))
				return;			
			AttributeSection lSource = (AttributeSection)source;
			if (lSource.AttributeCollection == null)
				return;
			if (_AttributeCollection == null)
				_AttributeCollection = new NodeList();
			for (int i = 0; i < lSource.AttributeCollection.Count; i++)
			{
				Attribute lAttribute = lSource.AttributeCollection[i] as Attribute;
				if (lAttribute == null)
					continue;
				int lIndex = lSource.DetailNodes.IndexOf(lAttribute);
				if (lIndex >= 0 && lIndex < lSource.DetailNodes.Count)
					_AttributeCollection.Add(DetailNodes[lIndex]);
			}
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Attribute;
		}
		#endregion
		#region ToString()
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			string lComma = String.Empty;
			for (int i=0; i<AttributeCollection.Count; i++)
			{
				lResult.AppendFormat("{0}{1}", lComma, AttributeCollection[i].ToString());
				lComma = ", ";
			}
			return String.Format("[{0}]", lResult.ToString());
		}
		#endregion
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			if (_AttributeCollection != null)
			{
				_AttributeCollection.Clear();
				_AttributeCollection = null;
			}
			base.OwnedReferencesTransfered();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_AttributeCollection != null && _AttributeCollection.Contains(oldElement))
				_AttributeCollection.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override void SetTarget(LanguageElement targetNode)
		{
			base.SetTarget(targetNode);
			if (_AttributeCollection == null)
				return;
			for (int i = 0; i < _AttributeCollection.Count; i++)
			{
				Attribute lAttribute = _AttributeCollection[i] as Attribute;
				if (lAttribute != null)
					lAttribute.SetTarget(targetNode);
			}
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AttributeSection lClone = new AttributeSection();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
	public void AddAttribute(Attribute attribute)
	{
	  if (attribute == null)
		return;
	  AttributeCollection.Add(attribute);
	  AddDetailNode(attribute);
	}
	public void RemoveAttribute(Attribute attribute)
	{
	  if (attribute == null)
		return;
	  AttributeCollection.Remove(attribute);
	  RemoveDetailNode(attribute);
	}
	IAttributeElementCollection IHasAttributes.Attributes
	{
	  get
	  {
		if (AttributeCollection == null)
		  return EmptyLiteElements.EmptyIAttributeElementCollection;
		LiteAttributeElementCollection lAttribures = new LiteAttributeElementCollection();
		lAttribures.AddRange(AttributeCollection);
		return lAttribures;
	  }
	}
		#region AttributeCollection
		public NodeList AttributeCollection
		{
			get
			{
				return _AttributeCollection;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.AttributeSection;
			}
		}
	#region IAttributeSectionElement Members
	bool IAttributeSectionElement.HasTargetNode
	{
	  get
	  {
		return TargetNode != null;
	  }
	}
	#endregion
	#region IHasAttributesModifier Members
	void IHasAttributesModifier.AddAttribute(IAttributeElement attribute)
	{
	  AddAttribute(attribute as Attribute);
	}
	void IHasAttributesModifier.RemoveAttribute(IAttributeElement attribute)
	{
	  RemoveAttribute(attribute as Attribute);
	}
	#endregion
  }
}
