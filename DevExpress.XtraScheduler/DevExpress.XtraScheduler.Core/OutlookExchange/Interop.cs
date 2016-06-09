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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace DevExpress.XtraScheduler.Outlook.Interop {
	#region Application
	[ComImport, TypeLibType((short)0x1040), Guid("00063001-0000-0000-C000-000000000046"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), CoClass(typeof(ApplicationClass))]
	public interface _Application {
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x110)]
		_NameSpace GetNamespace([In, MarshalAs(UnmanagedType.BStr)] string Type);
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10a)]
		object CreateItem([In] OlItemType ItemType);
		[DispId(0x116)]
		string Version { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x116)] get; }
	}
	[ComImport, ClassInterface((short)0), TypeLibType((short)11), ComSourceInterfaces("Microsoft.Office.Interop.Outlook.ApplicationEvents_10\0Microsoft.Office.Interop.Outlook.ApplicationEvents\0"), Guid("0006F03A-0000-0000-C000-000000000046"), CLSCompliant(false)]
	public class ApplicationClass : _Application {
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10a)]
		public virtual extern object CreateItem([In] OlItemType ItemType);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x110)]
		public virtual extern _NameSpace GetNamespace([In, MarshalAs(UnmanagedType.BStr)] string Type);
		[DispId(0x116)]
		public extern string Version { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(0x116)] get; }
	}
	#endregion
	#region Items
	[ComImport, Guid("00063041-0000-0000-C000-000000000046"), TypeLibType((short)0x1040), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface _Items : IEnumerable {
		[DispId(0xf000)]
		_Application Application { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf000)] get; }
		[DispId(0xf00a)]
		OlObjectClass Class { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00a)] get; }
		[DispId(0xf00b)]
		_NameSpace Session { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00b)] get; }
		[DispId(0xf001)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf001)] get; }
		[DispId(80)]
		int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(80)] get; }
		[DispId(0)]
		object this[object Index] { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
		[DispId(90)]
		object RawTable { [return: MarshalAs(UnmanagedType.IUnknown)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(90), TypeLibFunc((short)0x40)] get; }
		[DispId(0xce)]
		bool IncludeRecurrences { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xce)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xce)] set; }
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x5f)]
		object Add([In, Optional, MarshalAs(UnmanagedType.Struct)] object Type);
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x62)]
		object Find([In, MarshalAs(UnmanagedType.BStr)] string Filter);
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x63)]
		object FindNext();
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x56)]
		object GetFirst();
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x58)]
		object GetLast();
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x57)]
		object GetNext();
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x59)]
		object GetPrevious();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x54)]
		void Remove([In] int Index);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x5d)]
		void ResetColumns();
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)]
		_Items Restrict([In, MarshalAs(UnmanagedType.BStr)] string Filter);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x5c)]
		void SetColumns([In, MarshalAs(UnmanagedType.BStr)] string Columns);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x61)]
		void Sort([In, MarshalAs(UnmanagedType.BStr)] string Property, [In, Optional, MarshalAs(UnmanagedType.Struct)] object Descending);
	}
	#endregion
	#region AppointmentItem
	[ComImport, Guid("00063033-0000-0000-C000-000000000046"), TypeLibType((short)0x1040), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface _AppointmentItem {
		[DispId(0x820d)]
		DateTime Start { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x820d)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x820d)] set; }
		[DispId(0xfc27)]
		_TimeZone StartTimeZone { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc27)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc27)] set; }
		[DispId(0x820e)]
		DateTime End { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x820e)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x820e)] set; }
		[DispId(0xfc28)]
		_TimeZone EndTimeZone { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc28)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc28)] set; }
		[DispId(0x37)]
		string Subject { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x37)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x37)] set; }
		[DispId(0x9100)]
		string Body { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x9100)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x9100)] set; }
		[DispId(0x8215)]
		bool AllDayEvent { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8215)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8215)] set; }
		[DispId(0x8205)]
		OlBusyStatus BusyStatus { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8205)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8205)] set; }
		[DispId(0xf01e)]
		string EntryID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf01e)] get; }
		[DispId(0x8223)]
		bool IsRecurring { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8223)] get; }
		[DispId(0x8208)]
		string Location { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8208)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8208)] set; }
		[DispId(0x8503)]
		bool ReminderSet { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8503)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8503)] set; }
		[DispId(0x8501)]
		int ReminderMinutesBeforeStart { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8501)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x8501)] set; }
		[DispId(0x3008)]
		DateTime LastModificationTime { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3008)] get; }
		[DispId(0x3007)]
		DateTime CreationTime { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3007)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf0a5)]
		void ClearRecurrencePattern();
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf0a4)]
		RecurrencePattern GetRecurrencePattern();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf048)]
		void Save();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf04a)]
		void Delete();
	}
	#endregion
	#region NameSpace
	[ComImport, TypeLibType((short)0x1040), Guid("00063002-0000-0000-C000-000000000046"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface _NameSpace {
		[DispId(0x2103)]
		_Folders Folders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2103)] get; }
   		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x210b)]
		MAPIFolder GetDefaultFolder([In] OlDefaultFolders FolderType);
	}
	#endregion
	#region RecurrencePattern
	[ComImport, Guid("00063044-0000-0000-C000-000000000046"), TypeLibType((short)0x1040), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface RecurrencePattern {
		[DispId(0xf000)]
		_Application Application { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf000)] get; }
		[DispId(0xf00a)]
		OlObjectClass Class { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00a)] get; }
		[DispId(0xf00b)]
		_NameSpace Session { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00b)] get; }
		[DispId(0xf001)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf001)] get; }
		[DispId(0x1000)]
		int DayOfMonth { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1000)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1000)] set; }
		[DispId(0x1001)]
		OlDaysOfWeek DayOfWeekMask { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1001)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1001)] set; }
		[DispId(0x100d)]
		int Duration { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100d)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100d)] set; }
		[DispId(0x100c)]
		DateTime EndTime { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100c)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100c)] set; }
		[DispId(0x100e)]
		Exceptions Exceptions { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100e)] get; }
		[DispId(0x1003)]
		int Instance { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1003)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1003)] set; }
		[DispId(0x1004)]
		int Interval { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1004)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1004)] set; }
		[DispId(0x1006)]
		int MonthOfYear { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1006)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1006)] set; }
		[DispId(0x100b)]
		bool NoEndDate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100b)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100b)] set; }
		[DispId(0x1005)]
		int Occurrences { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1005)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1005)] set; }
		[DispId(0x1002)]
		DateTime PatternEndDate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1002)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1002)] set; }
		[DispId(0x1008)]
		DateTime PatternStartDate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1008)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1008)] set; }
		[DispId(0x1007)]
		OlRecurrenceType RecurrenceType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1007)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1007)] set; }
		[DispId(0x100a)]
		bool Regenerate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100a)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100a)] set; }
		[DispId(0x1009)]
		DateTime StartTime { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1009)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1009)] set; }
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x100f)]
		_AppointmentItem GetOccurrence([In] DateTime StartDate);
	}
	#endregion
	#region Exception
	[ComImport, TypeLibType((short)0x1040), Guid("0006304D-0000-0000-C000-000000000046"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface Exception {
		[DispId(0xf000)]
		_Application Application { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf000)] get; }
		[DispId(0xf00a)]
		OlObjectClass Class { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00a)] get; }
		[DispId(0xf00b)]
		_NameSpace Session { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00b)] get; }
		[DispId(0xf001)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf001)] get; }
		[DispId(0x2001)]
		_AppointmentItem AppointmentItem { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2001)] get; }
		[DispId(0x2002)]
		bool Deleted { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2002)] get; }
		[DispId(0x2000)]
		DateTime OriginalDate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2000)] get; }
	}
	#endregion
	#region Exceptions
	[ComImport, TypeLibType((short)0x1040), Guid("0006304C-0000-0000-C000-000000000046"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface Exceptions : IEnumerable {
		[DispId(0xf000)]
		_Application Application { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf000)] get; }
		[DispId(0xf00a)]
		OlObjectClass Class { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00a)] get; }
		[DispId(0xf00b)]
		_NameSpace Session { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00b)] get; }
		[DispId(0xf001)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf001)] get; }
		[DispId(80)]
		int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(80)] get; }
		[DispId(0)]
		Exception this[object Index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
	}
	#endregion
	#region Folders
	[ComImport, TypeLibType((short)0x1040), Guid("00063040-0000-0000-C000-000000000046"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface _Folders : IEnumerable {
		[DispId(80)]
		int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(80)] get; }
		[DispId(0)]
		MAPIFolder this[object Index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
	}
	#endregion
	#region MAPIFolder
	[ComImport, TypeLibType((short)0x1040), Guid("00063006-0000-0000-C000-000000000046"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface MAPIFolder {
		[DispId(0x3100)]
		_Items Items { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3100)] get; }
		[DispId(0x2103)]
		_Folders Folders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2103)] get; }
		[DispId(0x3001)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3001)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3001)] set; }
		[DispId(0xfa78)]
		string FolderPath { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfa78)] get; }
		[DispId(0xfa91)]
		string FullFolderPath { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0xfa91)] get; }
		[DispId(0x3106)]
		OlItemType DefaultItemType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3106)] get; }
	}
	#endregion
	#region OlBusyStatus
	public enum OlBusyStatus {
		olFree,
		olTentative,
		olBusy,
		olOutOfOffice,
		olWorkingElsewhere
	}
	#endregion
	#region OlDaysOfWeek
	public enum OlDaysOfWeek {
		olFriday = 0x20,
		olMonday = 2,
		olSaturday = 0x40,
		olSunday = 1,
		olThursday = 0x10,
		olTuesday = 4,
		olWednesday = 8
	}
	#endregion
	#region OlRecurrenceType
	public enum OlRecurrenceType {
		olRecursDaily = 0,
		olRecursMonthly = 2,
		olRecursMonthNth = 3,
		olRecursWeekly = 1,
		olRecursYearly = 5,
		olRecursYearNth = 6
	}
	#endregion
	#region OlItemType
	public enum OlItemType {
		olMailItem,
		olAppointmentItem,
		olContactItem,
		olTaskItem,
		olJournalItem,
		olNoteItem,
		olPostItem,
		olDistributionListItem
	}
	#endregion
	#region OlDefaultFolders
	public enum OlDefaultFolders {
		olFolderCalendar = 9,
		olFolderContacts = 10,
		olFolderDeletedItems = 3,
		olFolderDrafts = 0x10,
		olFolderInbox = 6,
		olFolderJournal = 11,
		olFolderNotes = 12,
		olFolderOutbox = 4,
		olFolderSentMail = 5,
		olFolderTasks = 13,
		olPublicFoldersAllPublicFolders = 0x12
	}
	#endregion
	#region OlObjectClass
	public enum OlObjectClass {
		olAction = 0x20,
		olActions = 0x21,
		olAddressEntries = 0x15,
		olAddressEntry = 8,
		olAddressList = 7,
		olAddressLists = 20,
		olApplication = 0,
		olAppointment = 0x1a,
		olAttachment = 5,
		olAttachments = 0x12,
		olContact = 40,
		olDistributionList = 0x45,
		olDocument = 0x29,
		olException = 30,
		olExceptions = 0x1d,
		olExplorer = 0x22,
		olExplorers = 60,
		olFolder = 2,
		olFolders = 15,
		olFormDescription = 0x25,
		olInspector = 0x23,
		olInspectors = 0x3d,
		olItemProperties = 0x62,
		olItemProperty = 0x63,
		olItems = 0x10,
		olJournal = 0x2a,
		olLink = 0x4b,
		olLinks = 0x4c,
		olMail = 0x2b,
		olMeetingCancellation = 0x36,
		olMeetingRequest = 0x35,
		olMeetingResponseNegative = 0x37,
		olMeetingResponsePositive = 0x38,
		olMeetingResponseTentative = 0x39,
		olNamespace = 1,
		olNote = 0x2c,
		olOutlookBarGroup = 0x42,
		olOutlookBarGroups = 0x41,
		olOutlookBarPane = 0x3f,
		olOutlookBarShortcut = 0x44,
		olOutlookBarShortcuts = 0x43,
		olOutlookBarStorage = 0x40,
		olPages = 0x24,
		olPanes = 0x3e,
		olPost = 0x2d,
		olPropertyPages = 0x47,
		olPropertyPageSite = 70,
		olRecipient = 4,
		olRecipients = 0x11,
		olRecurrencePattern = 0x1c,
		olReminder = 0x65,
		olReminders = 100,
		olRemote = 0x2f,
		olReport = 0x2e,
		olResults = 0x4e,
		olSearch = 0x4d,
		olSelection = 0x4a,
		olSyncObject = 0x48,
		olSyncObjects = 0x49,
		olTask = 0x30,
		olTaskRequest = 0x31,
		olTaskRequestAccept = 0x33,
		olTaskRequestDecline = 0x34,
		olTaskRequestUpdate = 50,
		olUserProperties = 0x26,
		olUserProperty = 0x27,
		olView = 80,
		olViews = 0x4f
	}
	#endregion
	#region _TimeZone
	[ComImport, Guid("000630FD-0000-0000-C000-000000000046"), TypeLibType((Int16)0x1040), CLSCompliant(false)]
	public interface _TimeZone {
		[DispId(0xf000)]
		_Application Application { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf000)] get; }
		[DispId(0xf00a)]
		OlObjectClass Class { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00a)] get; }
		[DispId(0xf00b)]
		_NameSpace Session { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf00b)] get; }
		[DispId(0xf001)]
		object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xf001)] get; }
		[DispId(0x2102)]
		string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2102)] get; }
		[DispId(0xfc2b)]
		string DaylightDesignation { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc2b)] get; }
		[DispId(0xfc2c)]
		string StandardDesignation { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc2c)] get; }
		[DispId(0xfc21)]
		int Bias { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc21)] get; }
		[DispId(0xfc22)]
		int StandardBias { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc22)] get; }
		[DispId(0xfc23)]
		int DaylightBias { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc23)] get; }
		[DispId(0xfc24)]
		DateTime StandardDate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc24)] get; }
		[DispId(0xfc25)]
		DateTime DaylightDate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc25)] get; }
		[DispId(0xfc32)]
		string ID { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xfc32)] get; }
	}
	#endregion
}
