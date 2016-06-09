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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraDiagram.Utils {
	public class DiagramAppearanceUtils : AppearanceHelper {
		public static void CombineAppearances(DiagramAppearanceObject target, DiagramAppearanceObject main, DiagramAppearanceObject defaultAppearance) {
			Combine(target, main, defaultAppearance);
			target.BeginUpdate();
			try {
				target.borderSize = main.GetAppearanceByOption(DiagramAppearanceObject.OptUseBorderSize, defaultAppearance).BorderSize;
				target.Options.UseBorderSize = main.GetOptionState(DiagramAppearanceObject.OptUseBorderSize, 0, defaultAppearance);
			}
			finally {
				target.EndUpdate();
			}
		}
		public static void CombineAppearances(DiagramAppearanceObject target, DiagramAppearanceObject first, DiagramAppearanceObject second, DiagramAppearanceObject defaultAppearance) {
			DiagramAppearanceObject temp = new DiagramAppearanceObject(second, defaultAppearance);
			try {
				CombineAppearances(target, first, temp);
			}
			finally {
				temp.Dispose();
			}
		}
		public static void CombineAppearances(DiagramAppearanceObject target, DiagramAppearanceObject[] sources, AppearanceDefault appearanceDefault) {
			target.BeginUpdate();
			try {
				Combine(target, sources);
				target.BeginUpdate();
				try {
					target.borderSize = ((DiagramAppearanceObject)GetAppearanceByOption(target, DiagramAppearanceObject.OptUseBorderSize, sources)).BorderSize;
					target.Options.UseBorderSize = GetOptionState(DiagramAppearanceObject.OptUseBorderSize, sources);
				}
				finally {
					target.EndUpdate();
				}
				Combine(target, appearanceDefault, true);
			}
			finally {
				target.EndUpdate();
			}
		}
	}
}
