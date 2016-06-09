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
	public class Enumeration : TypeDeclaration, IEnumerationElement
	{
		#region private fields 
		bool _IsEnumClass;
		SourceRange _AsRange;
		string _UnderlyingType;
		SourceRange _UnderlyingTypeRange;		
		#endregion
		#region Enumeration
		public Enumeration()
		{
			_AsRange = SourceRange.Empty;
			_UnderlyingType = String.Empty;
			_UnderlyingTypeRange = SourceRange.Empty;
		}
		#endregion
		#region Enumeration(string name)
		public Enumeration(string name) : this()
		{
			InternalName = name;
		}
		#endregion
		protected virtual void SetDefaultUnderlyingType()
		{			
			_UnderlyingType = PrimitiveTypeUtils.GetFullTypeName(PrimitiveType.Int32);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Enumeration))
				return;			
			Enumeration lSource = (Enumeration)source;
			_AsRange = lSource.AsRange;
			_UnderlyingType = lSource._UnderlyingType;
			_UnderlyingTypeRange = lSource.UnderlyingTypeRange;
			_IsEnumClass = lSource.IsEnumClass;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _AsRange = AsRange;
	  _UnderlyingTypeRange = UnderlyingTypeRange;
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if (IsStatic)
		return ImageIndex.StaticEnumeration;
			else
	  	return ImageIndex.Enumeration;
		}
		#endregion
	#region GetDefaultVisibility
	public override MemberVisibility GetDefaultVisibility()
	{
	  return MemberVisibility.Public;
	}
	#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Enumeration lClone = new Enumeration();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Enum;
			}
		}
		public SourceRange AsRange
		{
			get
			{
				return GetTransformedRange(_AsRange);
			}
			set
			{
		ClearHistory();
				_AsRange = value;
			}
		}
		public string UnderlyingType
		{
			get
			{
				return _UnderlyingType;
			}
			set
			{
				_UnderlyingType = value;
			}
		}
		public SourceRange UnderlyingTypeRange
		{
			get
			{
				return GetTransformedRange(_UnderlyingTypeRange);
			}
			set
			{
		ClearHistory();
				_UnderlyingTypeRange = value;
			}
		}
		public bool IsEnumClass
		{
			get
			{
				return _IsEnumClass;
			}
			set
			{
				_IsEnumClass = value;
			}
		}
	}
}
