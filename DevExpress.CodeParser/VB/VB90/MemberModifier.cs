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
	public class MemberModifier
	{
		Token _MustInherit;
		Token _Default;
		Token _Friend;
		Token _Shadows;
		Token _Overrides;
		Token _Private;
		Token _Protected;
		Token _Public;
		Token _NotInheritable;
		Token _NotOverridable;
		Token _Shared;
		Token _Overridable;
		Token _Overloads;
		Token _Readonly;
		Token _Writeonly;
		public MemberModifier()
		{
		}
		public Token MustInherit
		{
			get
			{
				return _MustInherit;
			}
			set
			{
				_MustInherit = value;
			}
		}
		public Token Default
		{
			get
			{
				return _Default;
			}
			set
			{
				_Default = value;
			}
		}
		public Token Friend
		{
			get
			{
				return _Friend;
			}
			set
			{
				_Friend = value;
			}
		}
		public Token Shadows
		{
			get
			{
				return _Shadows;
			}
			set
			{
				_Shadows = value;
			}
		}
		public Token Overrides
		{
			get
			{
				return _Overrides;
			}
			set
			{
				_Overrides = value;
			}
		}
		public Token Private
		{
			get
			{
				return _Private;
			}
			set
			{
				_Private = value;
			}
		}
		public Token Protected
		{
			get
			{
				return _Protected;
			}
			set
			{
				_Protected = value;
			}
		}
		public Token Public
		{
			get
			{
				return _Public;
			}
			set
			{
				_Public = value;
			}
		}
		public Token NotInheritable
		{
			get
			{
				return _NotInheritable;
			}
			set
			{
				_NotInheritable = value;
			}
		}
		public Token NotOverridable
		{
			get
			{
				return _NotOverridable;
			}
			set
			{
				_NotOverridable = value;
			}
		}
		public Token Shared
		{
			get
			{
				return _Shared;
			}
			set
			{
				_Shared = value;
			}
		}
		public Token Overridable
		{
			get
			{
				return _Overridable;
			}
			set
			{
				_Overridable = value;
			}
		}
		public Token Overloads
		{
			get
			{
				return _Overloads;
			}
			set
			{
				_Overloads = value;
			}
		}
		public Token Readonly
		{
			get
			{
				return _Readonly;
			}
			set
			{
				_Readonly = value;
			}
		}
		public Token Writeonly
		{
			get
			{
				return _Writeonly;
			}
			set
			{
				_Writeonly = value;
			}
		}
	}
}
