#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.ExpressApp.Editors {
	public enum Alignment { Left, Right, Center, Justify };
	public class AlignmentProvider {
		private static AlignmentProvider instance = new AlignmentProvider();
		private List<Type> alignedToRightTypes = new List<Type>();
		protected AlignmentProvider() {
			alignedToRightTypes.AddRange(new Type[]  {
				typeof(Byte),
				typeof(Decimal),
				typeof(Double),
				typeof(Int16),
				typeof(Int32),
				typeof(Int64),
				typeof(SByte),
				typeof(Single),
				typeof(UInt16),
				typeof(UInt32),
				typeof(UInt64)
			});
		}
		protected virtual Alignment GetAlignmentCore(Type type) {
			Type typeToCheck = type;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null) {
				typeToCheck = underlyingType;
			}
			if(alignedToRightTypes.Contains(typeToCheck)) {
				return Alignment.Right;
			}
			return Alignment.Left;
		}
		public static void SetInstance(AlignmentProvider newInstance) {
			instance = newInstance;
		}
		public static Alignment GetAlignment(Type type) {
			return instance.GetAlignmentCore(type);
		}
	}
}
