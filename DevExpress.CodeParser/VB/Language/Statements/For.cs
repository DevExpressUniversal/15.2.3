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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class VBFor : For
	{
		NodeList _NextExpressionList;
		public VBFor()
		{
		}
	public override void AddNextExpression(LanguageElement element)
	{
	  base.AddNextExpression(element);
	  _NextExpressionList.Add(element);
	}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is VBFor))
				return;
			VBFor lSource = (VBFor)source;
			if (lSource._NextExpressionList != null)
			{
				_NextExpressionList = new NodeList();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _NextExpressionList, lSource._NextExpressionList);
				if (_NextExpressionList.Count == 0 && lSource._NextExpressionList.Count > 0)
					_NextExpressionList = lSource._NextExpressionList.DeepClone(options) as NodeList;
			}		
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_NextExpressionList != null && _NextExpressionList.Contains(oldElement))
				_NextExpressionList.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
	public override void CleanUpOwnedReferences()
	{
	  base.CleanUpOwnedReferences();
	  if (_NextExpressionList != null)
	  {
		_NextExpressionList.Clear();
		_NextExpressionList = null;
	  }
	}
		#region ToString
		public override string ToString()
		{
			return "For";
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			VBFor lClone = new VBFor();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region NextExpressionList
		public new NodeList NextExpressionList
		{
			get
			{
				return _NextExpressionList;
			}
			set
			{
				_NextExpressionList = value;
			}
		}
		#endregion
	}
}
