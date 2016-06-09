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

using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Base;
	public class Module : TypeDeclaration, IModuleElement
	{
		protected int ModuleToken = TokenType.Module;
		public Module()
		{
		}
		public Module(string name)
			: this (name, SourceRange.Empty, null, SourceRange.Empty)
		{
		}
		public Module(string name, LanguageElementCollection block)
			: this (name, SourceRange.Empty, block, SourceRange.Empty)
		{
		}
		public Module(string name, SourceRange nameRange, LanguageElementCollection block, SourceRange range)
		{
			InternalName = name;
			NameRange = nameRange;
			if (block != null)
				AddNodes(block);
			SetRange(range);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Module))
				return;
			Module lSource = (Module)source;
			ModuleToken = lSource.ModuleToken;
		}
		#endregion
		public override int GetImageIndex()
		{
	  return ImageIndex.Module;
		}
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Private;
		}
		public override MemberVisibility[] GetValidVisibilities()
		{
			return new MemberVisibility[] { 
																			MemberVisibility.Private,
																			MemberVisibility.Public,
																			MemberVisibility.Internal
																		};
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Module lClone = new Module();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Module;
			}
		}
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
		public override bool IsStatic
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
	}
}
