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
using System.Globalization;
using System.Threading;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public interface ICurrentAspectProvider {
		string CurrentAspect { get; set; }
		event EventHandler CurrentAspectChanged;
	}
	public class CurrentAspectProvider : ICurrentAspectProvider {
		private string currentAspect;
		public CurrentAspectProvider() : this(CaptionHelper.DefaultLanguage) { }
		public CurrentAspectProvider(string currentAspect) {
			this.CurrentAspect = currentAspect;
		}
		public string CurrentAspect {
			get { return currentAspect; }
			set {
				if(currentAspect != value) {
					currentAspect = value;
					if(CurrentAspectChanged != null) {
						CurrentAspectChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		public event EventHandler CurrentAspectChanged;
	}
	public class CurrentAspectProviderFromCulture : ICurrentAspectProvider {
		private string currentAspect = null; 
		private CultureInfo CreateSpecificCultureSafe(string cultureName, CultureInfo defaultCultureInfo) {
			try {
				if(defaultCultureInfo != null && defaultCultureInfo.Name == cultureName) {
					return defaultCultureInfo;
				}
				else {
					return CultureInfo.CreateSpecificCulture(cultureName);
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(new ArgumentException("Error occurs while creating a CultureInfo object by name: '" + cultureName + "'", e));
			}
			return defaultCultureInfo;
		}
		public string CurrentAspect {
			get {
				string aspect = "";
				if(CultureInfo.InvariantCulture.Name != Thread.CurrentThread.CurrentUICulture.Name) {
					aspect = Thread.CurrentThread.CurrentUICulture.Name;
				}
				return aspect;
			}
			set {
				if(value == CultureInfo.InvariantCulture.TwoLetterISOLanguageName) {
					Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
				}
				else {
					Thread.CurrentThread.CurrentUICulture = CreateSpecificCultureSafe(value, Thread.CurrentThread.CurrentUICulture);
				}
				if(currentAspect != value) {
					if(CurrentAspectChanged != null) {
						CurrentAspectChanged(this, EventArgs.Empty);
					}
					currentAspect = value; 
				}
			}
		}
		public event EventHandler CurrentAspectChanged; 
	}
}
