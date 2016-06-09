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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class RegionDirectiveCollection : NodeList
  {
		protected override NodeList CreateInstance()
		{
			return new RegionDirectiveCollection();
		}
  	#region Add
  	public void Add(RegionDirective aRegionDirective)
  	{
  		base.Add(aRegionDirective);
  	}
  	#endregion
  	#region Remove
  	public void Remove(RegionDirective aRegionDirective)
  	{
  		base.Remove(aRegionDirective);
  	}
  	#endregion
  	#region Find
  	public RegionDirective Find(string aRegionDirective)
  	{
			LanguageElement lChildRegion;
  		foreach(RegionDirective lRegionDirective in this)
  		{
  			if (lRegionDirective.Name == aRegionDirective)	
  				return lRegionDirective;
				lChildRegion = lRegionDirective.FindChildByName(aRegionDirective);
				if (lChildRegion is RegionDirective)
					return (RegionDirective)lChildRegion;
  		}
  		return null;	
  	}
  	#endregion
		#region Find
		public RegionDirective Find(RegionDirective region)
		{
			foreach(RegionDirective lRegionDirective in this)
			{
				if (lRegionDirective == region)	
					return lRegionDirective;
			}
			return null;	
		}
		#endregion
  	#region Contains
		public bool Contains(string aRegionDirective)
  	{
  		return (Find(aRegionDirective) != null);
  	}
  	#endregion
		#region Contains
		public bool Contains(RegionDirective region)
		{
			return (Find(region) != null);
		}
		#endregion		
  	#region this[int aIndex]
  	public new RegionDirective this[int aIndex] 
  	{
  		get
  		{
  			return (RegionDirective) base[aIndex];
  		}
  	}
  	#endregion
  }
}
