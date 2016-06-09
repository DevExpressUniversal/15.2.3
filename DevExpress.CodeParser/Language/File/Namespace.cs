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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class Namespace : DelimiterCapableBlock, INamespaceElement
  {
		TextRange _NameRange;
		#region private fields...
		private string _FullName;
		private IDocument _ParentDocument;
	private bool _HasEndingSemicolon;
		#endregion
		#region Namespace()
		protected Namespace()
		{
			_ParentDocument = null;			
		}
		#endregion
		#region Namespace(string name)
		public Namespace(string name)
		{
			_ParentDocument = null;			
			InternalName = name;
		}
		#endregion
	#region Namespace(IDocument document)
	public Namespace(IDocument document)
	{
			_ParentDocument = document;
	}
	#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Namespace))
				return;
			Namespace namespaceElement = (Namespace)source;
			_NameRange = namespaceElement.NameRange;
			_FullName = namespaceElement._FullName;
			_ParentDocument = namespaceElement._ParentDocument;
	  _HasEndingSemicolon = namespaceElement._HasEndingSemicolon;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Namespace;
		}
		#endregion
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Internal;
		}
		#endregion
	#region GetValidVisibilities
	public override MemberVisibility[] GetValidVisibilities()
	{
	  return new MemberVisibility[] { 
									  MemberVisibility.Internal, 
									  MemberVisibility.Public 
									};
	}
	#endregion
	#region PathSegment
		public override string PathSegment
		{
			get
			{
		return StructuralParserServicesHolder.GetSignaturePart(this); 
			}
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Namespace lClone = new Namespace();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region SetNamespaceCollection
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetNamespaceCollection(NamespaceCollection namespaceCollection)
		{
		}
		#endregion
		#region CleanUpOwnedReferences()
		public override void CleanUpOwnedReferences()
		{
	  base.CleanUpOwnedReferences();
		}
		#endregion
		#region FullName
		public virtual string FullName
		{
			get
			{
				if (_FullName != null && _FullName != String.Empty)
					return _FullName;
				_FullName = Name;
				Namespace lParentNamespace = GetParentNamespace();
				while (lParentNamespace != null)
				{
					_FullName = lParentNamespace.Name + "." + _FullName;
					lParentNamespace = lParentNamespace.GetParentNamespace();
				}
				IProjectElement project = Project;
				if (project != null && project.SupportsRootNamespace)
				{
					string rootNamespace = project.RootNamespace;
					if (rootNamespace != null && rootNamespace.Length > 0)
						_FullName = String.Format("{0}.{1}", rootNamespace, _FullName);
				}
				return _FullName;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Namespace;
			}
		}
		public override bool DeclaresIdentifier
		{
			get
			{
				return true;
			}
		}
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = (TextRange)value;
			}
		}
		public IEnumerable AllNamespaces
		{
			get
			{
				return new ElementEnumerable(this, typeof(Namespace), true);
			}
		}
		public IEnumerable AllTypes
		{
			get
			{
				return new ElementEnumerable(this, typeof(TypeDeclaration), true);
			}
		}
	public bool HasEndingSemicolon
	{
	  get
	  {
		return _HasEndingSemicolon;
	  }
	  set
	  {
		_HasEndingSemicolon = value;
	  }
	}
		#region INamespaceElement Members
		INamespaceElementCollection INamespaceElement.Namespaces
		{
			get
			{
				LiteNamespaceElementCollection lNamespaces = new LiteNamespaceElementCollection();
				for (int i = 0; i < Nodes.Count; i++)				
					if (Nodes[i] is Namespace)
						lNamespaces.Add(Nodes[i]);
				return lNamespaces;
			}
		}
		ITypeElementCollection INamespaceElement.Types
		{
			get
			{
				LiteTypeElementCollection lTypes = new LiteTypeElementCollection();
				for (int i = 0; i < Nodes.Count; i++)
					if (Nodes[i] is TypeDeclaration || Nodes[i] is DelegateDefinition)
						lTypes.Add(Nodes[i]);
				return lTypes;
			}
		}		
		#endregion
	}
}
