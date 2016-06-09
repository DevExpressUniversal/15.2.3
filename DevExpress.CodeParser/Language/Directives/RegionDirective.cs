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
	public class RegionDirective : PreprocessorDirective
	{
		int _EndTokenLength;
		int _StartTokenLength;
		SourceRange _NameRange;
		#region RegionDirective
		public RegionDirective()
		{
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			if (!(source is RegionDirective))
				return;
			base.CloneDataFrom(source, options);
			RegionDirective lSource = (RegionDirective)source;
			_StartTokenLength = lSource._StartTokenLength;
			_EndTokenLength = lSource._EndTokenLength;
			_NameRange = lSource.NameRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		#region Contains
		public override bool Contains(int line, int offset)
		{
			return Range.Contains(line, offset);
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Region;
		}
		#endregion
		#region SetStartTokenLength
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStartTokenLength(Token token)
		{
			if (token == null)
				return;
			string lValue = token.EscapedValue;
			if (lValue == null)
				return;
			SetStartTokenLength(lValue.Length);
		}
		#endregion
		#region SetStartTokenLength
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStartTokenLength(int value)
		{
			if (_StartTokenLength == value)
				return;
			_StartTokenLength = value;
		}
		#endregion
		#region SetEndTokenLength
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEndTokenLength(Token token)
		{
			if (token == null)
				return;
			string lValue = token.EscapedValue;
			if (lValue == null)
				return;
			SetEndTokenLength(lValue.Length);
		}
		#endregion
		#region SetEndTokenLength
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEndTokenLength(int value)
		{
			if (_EndTokenLength == value)
				return;
			_EndTokenLength = value;
		}
		#endregion
		#region ToggleCollapsedState
		public void ToggleCollapsedState()
		{
			ICollapsibleRegion lRegion = GetCollapsibleRegion();
			if (lRegion == null)
				return;
			lRegion.Collapsed = !lRegion.Collapsed;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			RegionDirective lClone = new RegionDirective();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public static bool Equals(RegionDirective region1, RegionDirective region2)
	{
	  if (region1 == null || region2 == null)
	  {
		return region1 == null && region2 == null;
	  }
	  return String.Compare(region1.Name, region2.Name, StringComparison.CurrentCulture) == 0 && region1.Range.Equals(region2.Range) && region1.NameRange.Equals(region2.NameRange);
	}
		public RegionDirectiveCollection GetSubRegions()
		{
			RegionDirectiveCollection result = new RegionDirectiveCollection();
			foreach (LanguageElement child in Nodes)
			{
				RegionDirective subRegion = child as RegionDirective;
				if (subRegion != null)
					result.Add(subRegion);
			}
			return result;
		}
		public override bool CompletesPrevious
		{
			get 
			{
				return false;
			}
		}
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Region;
			}
		}
		#endregion
		#region EndTokenLength
	public int EndTokenLength
		{
		get
	  {
	  	return _EndTokenLength;
	  }
	}
		#endregion
		#region StartTokenLength
		public int StartTokenLength
		{
		get
	  {
	  	return _StartTokenLength;
	  }
	}
		#endregion
		#region FullRange
		[Obsolete("Use Range instead.")]
		public SourceRange FullRange
		{
			get
			{
				return Range;
			}
		}
		#endregion 
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
	}
}
