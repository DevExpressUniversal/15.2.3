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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Goto: FlowBreak
	{
		const int INT_MaintainanceComplexity = 10;
		SourceRange _LabelRange;
		bool _IsGotoCaseLabel;
		bool _IsGotoCaseDefault;
		string _Label;
		#region Goto
		public Goto()
		{
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Goto))
				return;
			Goto lSource = (Goto)source;
	  _IsGotoCaseLabel = lSource._IsGotoCaseLabel;
			_IsGotoCaseDefault = lSource._IsGotoCaseDefault;
			_Label = lSource._Label;
	  _LabelRange = LabelRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _LabelRange = LabelRange;
	}
		#region ToString
		public override string ToString()
		{
			return "Goto " + _Label;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Goto;
		}
		#endregion
		#region FindCaseTarget
		private LanguageElement FindCaseTarget()
		{
			Goto lGoto = this;
			string lTargetLabelName = lGoto.Label;
			if (lTargetLabelName != null)
				lTargetLabelName = lTargetLabelName.Trim();
			if (lGoto.IsGotoCaseLabel || lGoto.IsGotoCaseDefault)
			{
				Switch lSwitch = lGoto.GetParent(LanguageElementType.Switch) as Switch;
				if (lSwitch != null)
				{
					for (int i =0; i < lSwitch.NodeCount; i++)
					{
						Case lCase = lSwitch.Nodes[i] as Case;
						if (lCase != null)
						{
							if (lGoto.IsGotoCaseDefault)
							{
								if (lCase.IsDefault)
									return lCase;
							}
							else if (lGoto.IsGotoCaseLabel)
							{
								if (lCase.Expression.ToString().Trim() == lTargetLabelName)
									return lCase;
							}
						}
					}
				}
			}
			return null;
		}
		#endregion
		#region FindTarget
		public override LanguageElement FindTarget()
		{
			LanguageElement lTargetElement = null;
			Goto lGoto = this;
			string lTargetLabelName = lGoto.Label;
			if (lTargetLabelName != null)
				lTargetLabelName = lTargetLabelName.Trim();
			lTargetElement = FindCaseTarget();
			if (lTargetElement != null)
				return lTargetElement;
			LanguageElement lOuterBounds = GetParentMethodOrPropertyAccessor();
			if (lOuterBounds != null)
			{
				LanguageElement lNextElement = lOuterBounds.NextNode;
				while (lNextElement != null)
				{
					if (!lNextElement.IsParentedBy(lOuterBounds))
						break;
					if (lNextElement is Label)
					{
						Label lLabel = (Label)lNextElement;
						if (lLabel.Name == lTargetLabelName)
						{
							lTargetElement = lLabel.NextCodeSibling;
							if (lTargetElement == null)
								lTargetElement = lLabel;
							break;
						}
					}
					lNextElement = lNextElement.NextNode;
				}
			}
			return lTargetElement;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Goto lClone = new Goto();
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
				return LanguageElementType.Goto;
			}
		}
		#region Label
		public string Label
		{
			get
			{
				return _Label;
			}
			set
			{
				_Label = value;
			}
		}
		#endregion
		#region IsGotoCaseLabel
		public bool IsGotoCaseLabel
		{
			get
			{
				return _IsGotoCaseLabel;
			}
			set
			{
				_IsGotoCaseLabel = value;
			}
		}
		#endregion
		#region IsGotoCaseDefault
		public bool IsGotoCaseDefault
		{
			get
			{
				return _IsGotoCaseDefault;
			}
			set
			{
				_IsGotoCaseDefault = value;
			}
		}
		#endregion
		public SourceRange LabelRange
		{
			get
			{
				return GetTransformedRange(_LabelRange);
			}
			set
			{
		ClearHistory();
				_LabelRange = value;
			}
		}
	}
}
