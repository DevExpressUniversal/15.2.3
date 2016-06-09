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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Updating;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public enum VersionsCompatibilitySeverity { None, ShowWarning };
	public class VersionsCompatibilityController : ViewController {
		public string IgnoreNestedActiveKey = "IgnoreNested";
		private Int32 checkPeriod = 300;
		private VersionsCompatibilitySeverity severity = VersionsCompatibilitySeverity.ShowWarning;
		protected static DateTime lastCheckTime;
		protected static CompatibilityError lastError;
		protected virtual DateTime Now() {
			return DateTime.Now;
		}
		protected void Check() {
			if((lastError == null) && (checkPeriod > 0) && (Now().Ticks - lastCheckTime.Ticks) >= checkPeriod * TimeSpan.TicksPerSecond) {
				lastCheckTime = Now();
				lastError = CheckVersionsCompatibility();
			}
			if((lastError is CompatibilityApplicationIsOldError) && (severity == VersionsCompatibilitySeverity.ShowWarning)) {
				ProcessCompatibilityError(lastError);
			}
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			if(ObjectSpace.IsModified) {
				Check();
			}
		}
		protected virtual CompatibilityError CheckVersionsCompatibility() {
			CompatibilityError result = null;
			if(Application.DatabaseUpdateMode != DatabaseUpdateMode.Never) {
				foreach(IObjectSpaceProvider objectSpaceProvider in Application.ObjectSpaceProviders) {
					DatabaseUpdaterBase dbUpdater = Application.CreateDatabaseUpdater(objectSpaceProvider);
					if(dbUpdater != null) {
						result = dbUpdater.CheckCompatibility();
						if(result != null) {
							break;
						}
					}
				}
			}
			return result;
		}
		protected virtual void ProcessCompatibilityError(CompatibilityError error) {
			WinApplication.Messaging.Show(CaptionHelper.GetLocalizedText("Texts", "Warning"), error.Message);
		}
		protected override void OnViewChanging(View view) {
			Active[IgnoreNestedActiveKey]= !(view.ObjectSpace is INestedObjectSpace);
			base.OnViewChanging(view);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if((severity != VersionsCompatibilitySeverity.None)) {
				ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
				if(ObjectSpace.IsModified) {
					Check();
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(ObjectSpace != null) {
				ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			}
		}
		public VersionsCompatibilityController() : base() {
			TypeOfView = typeof(ObjectView);
			TargetViewNesting = Nesting.Root;
		}
		public VersionsCompatibilitySeverity Severity {
			get { return severity; }
			set { severity = value; }
		}
		public Int32 CheckPeriod {
			get { return checkPeriod; }
			set { checkPeriod = value; }
		}
	}
}
