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
	public class TextString : LanguageElement
	{
	string _EscapedValue;
		bool _IsVerbatim;
		public TextString()
			: this(String.Empty, false)
	{
	}
		public TextString(bool isVerbatim)
			: this(String.Empty, isVerbatim)
		{
		}
		public TextString(string text, bool isVerbatim)
		{
			InternalName = text;
			_IsVerbatim = isVerbatim;
		}
		string GetEscapedString(string name, bool isVerbatim)
		{
			if (name == null || name.Length == 0)
				return name;
			if (Project == null)
				return name;
	  return StructuralParserServicesHolder.GetEscapedString(Project.Language, name, isVerbatim);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is TextString))
				return;			
			TextString lSource = (TextString)source;
			_EscapedValue = lSource._EscapedValue;
			_IsVerbatim = lSource._IsVerbatim;
		}
		#endregion
		protected PrimitiveExpression GetPrimitiveExpression()
		{
			SourceFile fileNode = FileNode;
			if (fileNode == null)
				return null;
			return fileNode.GetNodeAt(Range.Start) as PrimitiveExpression;
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ResourceString;	
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TextString lClone = new TextString();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region IsLiteralString
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use TextString.IsVerbatim")]
		public bool IsLiteralString
		{
			get
			{
				return _IsVerbatim;
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentHistorySlice History
		{
			get
			{
				SourceFile fileNode = FileNode;
				if (fileNode == null)
					return base.History;
				return fileNode.History;
			}
		}
	public string EscapedValue
	{
	  get
	  {
		if (_EscapedValue == null)
		  _EscapedValue = GetEscapedString(InternalName, _IsVerbatim);
		return _EscapedValue;
	  }
	}
	public bool IsVerbatim
	{
	  get
	  {
		return _IsVerbatim;
	  }
	}
		public PrimitiveExpression PrimitiveExpression
		{
			get
			{
				return GetPrimitiveExpression();
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.TextString;
			}
		}
	}
}
