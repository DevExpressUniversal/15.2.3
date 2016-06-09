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
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.XtraGauges.Base;
namespace DevExpress.XtraGauges.Presets {
	static class ControlLoader {
		public static IGaugeContainer CreateGaugeContainer() {
			return activatorRoutine();
		}
		public static Type GetWinGaugeControlType() {
			return WinAssembly.GetType("DevExpress.XtraGauges.Win.GaugeControl");
		}
		static Func<IGaugeContainer> activatorRoutine;
		static Assembly WinAssembly;
		static ControlLoader() {
			AssemblyName winAssemblyName = new AssemblyName(AssemblyInfo.SRAssemblyGaugesWin);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for(int i = 0; i < assemblies.Length; i++) {
				if(WinAssembly != null) break;
				AssemblyName name = assemblies[i].GetName();
				if(name.Equals(winAssemblyName)) WinAssembly = assemblies[i];
			}
			if(WinAssembly == null)
				WinAssembly = Assembly.Load(winAssemblyName);
			var constructor = GetWinGaugeControlType().GetConstructor(new Type[] { });
			var constructorExpression = Expression.New(constructor);
			var typeAsExpression = Expression.TypeAs(constructorExpression, typeof(IGaugeContainer));
			activatorRoutine = Expression.Lambda(typeof(Func<IGaugeContainer>),
				typeAsExpression)
				.Compile() as Func<IGaugeContainer>;
		}
	}
}
