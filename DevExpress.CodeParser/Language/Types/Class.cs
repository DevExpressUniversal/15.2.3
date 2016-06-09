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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Class : TypeDeclaration, IClassElement
	{
		#region protected fields...
		string _AncestorName = String.Empty;
		StringCollection _Implements;
		#endregion
		#region Class()
		public Class()
		{
	}
		#endregion
		#region Class(string name)
		public Class(string name) : this()
		{
			InternalName = name;
		}
		#endregion
		#region AncestorDeclaresMember
		public bool AncestorDeclaresMember(LanguageElement member)
		{
			if (member == null)
				return false;
			Class lAncestor = GetAncestor();
			while (lAncestor != null)
			{
				if (lAncestor.DeclaresMember(member))
					return true;
				lAncestor = lAncestor.GetAncestor();
			}
			return false;
		}
		#endregion
		#region DeclaresMember
		public bool DeclaresMember(LanguageElement member)
		{
			return member != null && member.Parent == this;
		}
		#endregion
		#region GetMostComplexMember
		public virtual LanguageElement GetMostComplexMember()
		{
			int lHighestCyclomaticComplexity = 0;
			LanguageElement lMostComplexMember = null;
			if (Nodes == null)
				return null;
			for (int i = 0; i < Nodes.Count; i++)
			{
				LanguageElement lElement = (LanguageElement)Nodes[i];
				if (lElement == null)
					continue;
				int lThisCyclomaticComplexity = lElement.GetCyclomaticComplexity();
				if (lThisCyclomaticComplexity <= lHighestCyclomaticComplexity)
					continue;
				lHighestCyclomaticComplexity = lThisCyclomaticComplexity;
				lMostComplexMember = lElement;
			}
			return lMostComplexMember;
		}
		#endregion
		#region GetAverageCyclomaticComplexity
		public virtual double GetAverageCyclomaticComplexity()
		{
			int lTotalCyclomaticComplexity = 0;
			int lTotalMembersWithPaths = 0;
			if (Nodes == null)
				return 0;
			for (int i = 0; i < Nodes.Count; i++)
			{
				int lThisCyclomaticComplexity = 0;
				LanguageElement lElement = (LanguageElement)Nodes[i];
				if (lElement == null)
					continue;
				lThisCyclomaticComplexity = lElement.GetCyclomaticComplexity();
				if (lThisCyclomaticComplexity == 0)
					continue;
		lTotalCyclomaticComplexity += lThisCyclomaticComplexity;
				lTotalMembersWithPaths += 1;
			}
			if (lTotalMembersWithPaths == 0)
				return 0;
			return lTotalCyclomaticComplexity / lTotalMembersWithPaths;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if (IsStatic)
		return ImageIndex.StaticClass;
			else
	  	return ImageIndex.Class;
		}
		#endregion
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
	  return MemberVisibility.Private;
	}
		#endregion
	#region GetValidVisibilities
	public override MemberVisibility[] GetValidVisibilities()
	{
			if (!IsStatic)
				return new MemberVisibility[] { 
									  MemberVisibility.Private, 
									  MemberVisibility.Protected, 
									  MemberVisibility.ProtectedInternal, 
									  MemberVisibility.Internal, 
									  MemberVisibility.Public 
									};
			else
				return new MemberVisibility[] { 
									  MemberVisibility.Private, 
									  MemberVisibility.Internal, 
									  MemberVisibility.Public 
									};
	}
	#endregion
		#region RangeIsClean
		public override bool RangeIsClean(SourceRange sourceRange)
		{
			return (sourceRange.Top > BlockStart.Bottom) && (sourceRange.Bottom < BlockEnd.Top);
		}
		#endregion
		#region GetTypeName
		public override string GetTypeName()
		{
			return Name;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Class))
				return;			
			Class lSource = (Class)source;
			_AncestorName = lSource._AncestorName;
			_Implements = StringHelper.CloneStringCollection(lSource._Implements);
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Class lClone = new Class();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Class;
			}
		}
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
	#region GetAncestor
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Class GetAncestor()
	{
	  LanguageElement lResult = this.FindDeclaration(_AncestorName, this);
	  if (lResult is Class)
		return (Class)lResult;
	  else
		return null;
	}
	#endregion
	#region GetImplements
	[EditorBrowsable(EditorBrowsableState.Never)]
	public LanguageElementList GetImplements()
	{
	  LanguageElementList lResult = new LanguageElementList();
	  for (int i = 0; i < Implements.Count; i++)
	  {
		LanguageElement lInterface = FindDeclaration(Implements[i], this);
		if (lInterface != null)
		  if (lInterface is Interface)
			lResult.Add(lInterface);
	  }
	  return lResult;
	}
	#endregion
		#region AncestorName
	[EditorBrowsable(EditorBrowsableState.Never)]
		[Description("Gets the name of the ancestor class.")]
		[Category("Family")]
		public string AncestorName
		{
			get
			{
				return _AncestorName;
			}
			set
			{
				_AncestorName = value;
			}
		}
		#endregion
		#region Implements
	[EditorBrowsable(EditorBrowsableState.Never)]
		[Description("Gets names of the implemented interfaces.")]
		[Category("Family")]
		public StringCollection Implements
		{
			get
			{
				if (_Implements == null)
					_Implements = new StringCollection();
				return _Implements;
			}
		}
		#endregion
	}
}
