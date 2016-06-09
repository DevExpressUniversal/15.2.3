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
using System.ComponentModel;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[DefaultProperty("FullName")]
	[ImageName("BO_Person")]
	[CalculatedPersistentAliasAttribute("FullName", "FullNamePersistentAlias")]
	public class Person : Party, IPerson {
		private const string defaultFullNameFormat = "{FirstName} {MiddleName} {LastName}";
		private const string defaultFullNamePersistentAlias = "concat(FirstName, MiddleName, LastName)";
#if MediumTrust
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public PersonImpl person = new PersonImpl();
#else
		private PersonImpl person = new PersonImpl();
#endif
		static Person() {
			PersonImpl.FullNameFormat = defaultFullNameFormat;
		}
		private static string fullNamePersistentAlias = defaultFullNamePersistentAlias;
		public static string FullNamePersistentAlias {
			get { return fullNamePersistentAlias; }
		}
		public static void SetFullNameFormat(string format, string persistentAlias) {
			PersonImpl.FullNameFormat = format;
			fullNamePersistentAlias = persistentAlias;
		}
		public Person(Session session) : base(session) { }
		public void SetFullName(string fullName) {
			person.SetFullName(fullName);
		}
		public string FirstName {
			get { return person.FirstName; }
			set {
				string oldValue = person.FirstName;
				person.FirstName = value;
				OnChanged("FirstName", oldValue, person.FirstName);
			}
		}
		public string LastName {
			get { return person.LastName; }
			set {
				string oldValue = person.LastName;
				person.LastName = value;
				OnChanged("LastName", oldValue, person.LastName);
			}
		}
		public string MiddleName {
			get { return person.MiddleName; }
			set {
				string oldValue = person.MiddleName;
				person.MiddleName = value;
				OnChanged("MiddleName", oldValue, person.MiddleName);
			}
		}
		public DateTime Birthday {
			get { return person.Birthday; }
			set {
				DateTime oldValue = person.Birthday;
				person.Birthday = value;
				OnChanged("Birthday", oldValue, person.Birthday);
			}
		}
		[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsNonPersistentNorAliased)), SearchMemberOptions(SearchMemberMode.Include)]
		public virtual string FullName {
			get { return ObjectFormatter.Format(PersonImpl.FullNameFormat, this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string DisplayName { 
			get { return FullName; }
		}
		[Size(255)]
		public string Email {
			get { return person.Email; }
			set {
				string oldValue = person.Email;
				person.Email = value;
				OnChanged("Email", oldValue, person.Email);
			}
		}
	}
}
