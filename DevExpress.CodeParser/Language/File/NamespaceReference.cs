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
  public class NamespaceReference : CodeElement, INamespaceReference
  {
		const int INT_MaintainanceComplexity = 3;
		TextRange _NameRange;
		TextRange _AliasNameRange;
		bool _IsAlias = false;
		string _AliasName = null;
		public NamespaceReference()
			: this (String.Empty)
		{
		}
	public NamespaceReference(string nameSpace)
	{
			InternalName = nameSpace;
	}
		public NamespaceReference(string aliasName, string nameSpace)
		{
			InternalName = nameSpace;
			_AliasName = aliasName;
			_IsAlias = true;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is NamespaceReference))
				return;
			NamespaceReference lSource = (NamespaceReference)source;
			_IsAlias = lSource._IsAlias;
			_AliasName = lSource._AliasName;
			_NameRange = lSource.NameRange;
			_AliasNameRange = lSource.AliasNameRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	  _AliasNameRange = AliasNameRange;
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if (_IsAlias)
				return ImageIndex.ClassAliasImport;
			else 
				return ImageIndex.UsingOrImports;
		}
		#endregion
	#region ToString
	public override string ToString()
	{
	  if ( _IsAlias )
		return( "Class Alias " + _AliasName + "=" + InternalName );
	  else
		return( InternalName );		
	}
	#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NamespaceReference lClone = new NamespaceReference();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.NamespaceReference;
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
				_NameRange = value;
			}
		}
		[Description("The name of the namespace (or potentially the class in the VB Imports statement) that this language element alias points to.")]
		[Category("Details")]
		[DefaultValue("")]
		public string AliasName
	{
		get
		{
			return _AliasName;
		}
			set
			{
				_AliasName = value;
			}
	}
		[Description("Gets or sets source range if the alias name.")]
		[Category("Details")]
		public SourceRange AliasNameRange
		{
			get
			{
				return GetTransformedRange(_AliasNameRange);
			}
			set
			{
		ClearHistory();
				_AliasNameRange = value;
			}
		}
		[Description("True if this Using or Imports statement is an alias.")]
		[Category("Details")]
		[DefaultValue(false)]
		public bool IsAlias
		{
			get
			{
				return _IsAlias;
			}
			set
			{
				_IsAlias = value;
			}
		}
	public Expression AliasExpression
	{
	  get
	  {
		if (DetailNodeCount == 0)
		  return null;
		foreach (LanguageElement element in DetailNodes)
		  if (!(element is SupportElement))
			return element as Expression;
		return null;
	  }
	}
	IExpression INamespaceReference.NameExpression
	{
	  get
	  {
		if (IsAlias)
		  return DetailNodeCount == 0 ? null : DetailNodes[0] as Expression;
		return null;
	  }
	}
	IExpression INamespaceReference.Expression
	{
	  get
	  {
		if (IsAlias)
		  return DetailNodeCount < 2 ? null : DetailNodes[1] as Expression;
		return DetailNodeCount == 0 ? null : DetailNodes[0] as Expression;
	  }
	}
	}
	public class XmlNamespaceReference : NamespaceReference, IXmlNamespaceReference
	{
		string _NamespaceName = String.Empty;
		public XmlNamespaceReference()
			:base()
		{
		}
		public XmlNamespaceReference(string prefixName)
			: base(prefixName)
		{
		}
		public XmlNamespaceReference(string prefixName, string namespaceName)
			: this(prefixName)
		{
			_NamespaceName = namespaceName;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlNamespaceReference lClone = new XmlNamespaceReference();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlNamespaceReference))
				return;
			XmlNamespaceReference lSource = (XmlNamespaceReference)source;
			_NamespaceName = lSource._NamespaceName;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
				return ImageIndex.UsingOrImports;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string result = "<xmlns";
			if (InternalName != null)
			{
				result += ":" + InternalName;
			}
			if (_NamespaceName != null)
			{
				result += _NamespaceName;
			}
			result += ">";
			return result;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.XmlNamespaceReference;
			}
		}
		public string NamespaceName
		{
			get
			{
				return _NamespaceName;
			}
			set
			{
				_NamespaceName = value;
			}
		}
	}
  public class AliasDeclaration : CodeElement, IAliasDeclaration
  {
	string _AliasName;
	IExpression _Expression;
	protected AliasDeclaration()
	  : this(String.Empty, null)
	{
	}
	public AliasDeclaration(string name, IExpression expression)
	{
	  _AliasName = name;
	  _Expression = expression;
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
	  if (!(source is AliasDeclaration))
				return;
	  AliasDeclaration alias = (AliasDeclaration)source;
	  _AliasName = alias._AliasName;
	  _Expression = alias._Expression;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ClassAliasImport;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
	  AliasDeclaration clone = new AliasDeclaration();
	  clone.CloneDataFrom(this, options);
	  return clone;
		}
		#endregion
	public override LanguageElementType ElementType
	{
	  get { return LanguageElementType.NamespaceReference; }
	}
	public string AliasName
	{
	  get { return _AliasName; }
	}
	public override string Name
	{
	  get { return _AliasName; }
	  set { _AliasName = value; }
	}
	public IExpression Expression
	{
	  get { return _Expression; }
	}
  }
  public class ExternAliasDeclaration : CodeElement, IExternAlias
  {
	string _AliasName;
	IAssemblyReference _Reference;
	IElementCollection _References;
	protected ExternAliasDeclaration()
	  : this(String.Empty, new IElementCollection())
	{
	}
	[Obsolete("Use constructor with collection references in the second parameter")]
	public ExternAliasDeclaration(string name, IAssemblyReference reference)
	{
	  _AliasName = name;
	  _Reference = reference;
	}
	public ExternAliasDeclaration(string name, IElementCollection references)
	{
	  _AliasName = name;
	  _References = references;
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
	  if (!(source is ExternAliasDeclaration))
				return;
	  ExternAliasDeclaration alias = (ExternAliasDeclaration)source;
	  _AliasName = alias._AliasName;
	  _Reference = alias._Reference;
	  _References = alias.References;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ClassAliasImport;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
	  ExternAliasDeclaration clone = new ExternAliasDeclaration();
	  clone.CloneDataFrom(this, options);
	  return clone;
		}
		#endregion
	public override LanguageElementType ElementType
	{
	  get { return LanguageElementType.ExternAlias; }
	}
	public override string Name
	{
	  get { return _AliasName; }
	  set { _AliasName = value; }
	}
	[Obsolete("Use property References")]
	public IAssemblyReference Reference
	{
	  get { return _Reference; }
	}
	public IElementCollection References
	{
	  get { return _References; }
	}
  }
}
