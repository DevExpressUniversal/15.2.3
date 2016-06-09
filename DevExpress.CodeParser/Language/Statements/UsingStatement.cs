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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class UsingStatement: ParentToSingleStatement, IUsingStatement
	{
	#region private fields...
	LanguageElementCollection _Initializers;
	#endregion
		#region UsingStatement
		public UsingStatement(): base()
		{  
			_Initializers = new LanguageElementCollection();
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is UsingStatement))
				return;
			UsingStatement lSource = (UsingStatement)source;			
			if (lSource._Initializers != null)
			{
				_Initializers = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Initializers, lSource._Initializers);
				if (_Initializers.Count == 0 && lSource._Initializers.Count > 0)
					_Initializers = lSource._Initializers.DeepClone(options) as LanguageElementCollection;
			}			
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Initializers != null && _Initializers.Contains(oldElement))
				_Initializers.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public void AddInitializer(LanguageElement initializer)
		{
			if (initializer == null)
				return;
			AddDetailNode(initializer);
			if (_Initializers != null)
				_Initializers.Add(initializer);
		}
		public void AddInitializers(LanguageElementCollection initializers)
		{
		  if (initializers == null)
			return;
		  foreach (LanguageElement initializer in initializers) 
			AddInitializer(initializer);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.UsingStatement;	
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			UsingStatement lClone = new UsingStatement();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.UsingStatement;
			}
		}
		#region InitializedDeclaration
		[Obsolete("Use Initializers instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement InitializedDeclaration
		{
		  get
		  {
				if (_Initializers.Count > 0)
					return _Initializers[0];
				return null;
		  }
		}
	#endregion
		#region Initializers
		public LanguageElementCollection Initializers
		{
			get
			{
				return _Initializers;
			}
		}
		#endregion
		#region IUsingStatement Members
		IElementCollection IUsingStatement.Initializers
		{
			get 
			{
				IElementCollection result = new IElementCollection();
				result.AddRange(Initializers);
				return result;
			}
		}
		#endregion		
	}
}
