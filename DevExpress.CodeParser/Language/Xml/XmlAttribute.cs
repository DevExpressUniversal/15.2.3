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
  public class XmlAttribute : XmlNode, IXmlAttribute, IMarkupElement
	{
		string _Value;
		TextRange _NameRange;
		TextRange _ValueRange;
		public XmlAttribute()
		{
		}
		public XmlAttribute(string name) : this()
		{
			InternalName = name;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlAttribute))
				return;			
			XmlAttribute lSource = (XmlAttribute)source;
			_Value = lSource.Value;
	  _NameRange = lSource.NameRange;
			_ValueRange = lSource.ValueRange;			
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
			_ValueRange = ValueRange;			
	}
		public override string ToString()
		{
			return InternalName + " = " + _Value;
		}
		public override int GetImageIndex()
		{
			return ImageIndex.DocCommentAttribute;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetValue(string newValue)
		{
			_Value = newValue;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetNameRange(SourceRange range)
		{
	  ClearHistory();
			_NameRange = range;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetValueRange(SourceRange newRange)
		{
	  ClearHistory();
			_ValueRange = newRange;
		}
		public virtual string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
		public virtual SourceRange ValueRange
		{
			get
			{
				return GetTransformedRange(_ValueRange);
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.XmlDocAttribute;
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
		}
		string IXmlAttribute.Value
		{
			get 
			{ 
				return Value;
			}
		}
	TextRange IXmlAttribute.ValueRange
	{
	  get
	  {
		return _ValueRange;
	  }
	}
	}
}
