// GtkSharp.Generation.StructBase.cs - The Structure/Boxed Base Class.
//
// Authors:
//   Mike Kestner <mkestner@speakeasy.net>
//   Stephan Sundermann <stephansundermann@gmail.com>
//
// Copyright (c) 2001-2003 Mike Kestner
// Copyright (c) 2013 Stephan Sundermann
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the GNU General Public
// License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using GapiCodegen.Interfaces;
using GapiCodegen.Utils;

namespace GapiCodegen.Generatables {
	public abstract class StructBase : ClassBase, IManualMarshaler {
	
		IList<StructField> fields = new List<StructField> ();
		bool need_read_native = false;

		protected StructBase (XmlElement namespaceElement, XmlElement element) : base (namespaceElement, element)
		{
			foreach (XmlNode node in element.ChildNodes) {

				if (!(node is XmlElement)) continue;
				XmlElement member = (XmlElement) node;

				switch (node.Name) {
				case "field":
					fields.Add (new StructField (member, this));
					break;

				case "callback":
					Statistics.IgnoreCount++;
					break;

				default:
					if (!IsNodeNameHandled (node.Name))
						Console.WriteLine ("Unexpected node " + node.Name + " in " + CName);
					break;
				}
			}
		}

		public override string DefaultValue {
			get {
				return QualifiedName + ".Zero";
			}
		}

		public override string MarshalType {
			get {
				return "IntPtr";
			}
		}

		public override string AssignToName {
			get { throw new NotImplementedException (); }
		}

		public override string CallByName ()
		{
			return "this_as_native";
		}

		public override string CallByName (string var)
		{
			return var + "_as_native";
		}

		public override string FromNative (string varName)
		{
			if (DisableNew)
				return varName + " == IntPtr.Zero ? " + QualifiedName + ".Zero : (" + QualifiedName + ") System.Runtime.InteropServices.Marshal.PtrToStructure (" + varName + ", typeof (" + QualifiedName + "))";
			else
				return QualifiedName + ".New (" + varName + ")";
		}
		
		public string AllocNative (string var)
		{
			return "GLib.Marshaller.StructureToPtrAlloc (" + var + ")";
		}

		public string ReleaseNative (string var)
		{
			return "Marshal.FreeHGlobal (" +var + ")";
		}

		private bool DisableNew {
			get {
				return Element.GetAttributeAsBoolean ("disable_new");
			}
		}

		public virtual bool Union {
			get {
				return false;
			}
		}

		protected void GenEqualsAndHash (StreamWriter sw)
		{
			int bitfields = 0;
			bool need_field = true;
			StringBuilder hashcode = new StringBuilder ();
			StringBuilder equals = new StringBuilder ();

			hashcode.Append ("this.GetType ().FullName.GetHashCode ()");
			equals.Append ("true");

			foreach (StructField field in fields) {
				if (field.IsPadding || field.Hidden)
					continue;
				if (field.IsBitfield) {
					if (need_field) {
						equals.Append (" && _bitfield");
						equals.Append (bitfields);
						equals.Append (".Equals (other._bitfield");
						equals.Append (bitfields);
						equals.Append (")");
						hashcode.Append (" ^ ");
						hashcode.Append ("_bitfield");
						hashcode.Append (bitfields++);
						hashcode.Append (".GetHashCode ()");
						need_field = false;
					}
				} else {
					need_field = true;
					equals.Append (" && ");
					equals.Append (field.EqualityName);
					equals.Append (".Equals (other.");
					equals.Append (field.EqualityName);
					equals.Append (")");
					hashcode.Append (" ^ ");
					hashcode.Append (field.EqualityName);
					hashcode.Append (".GetHashCode ()");
				}
			}

			if (!Element.GetAttributeAsBoolean ("noequals")) {
				sw.WriteLine ("\t\tpublic bool Equals ({0} other)", Name);
				sw.WriteLine ("\t\t{");
				sw.WriteLine ("\t\t\treturn {0};", equals.ToString ());
				sw.WriteLine ("\t\t}");
				sw.WriteLine ();
			}
			sw.WriteLine ("\t\tpublic override bool Equals (object other)");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\treturn other is {0} && Equals (({0}) other);", Name);
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
			if (Element.GetAttributeAsBoolean ("nohash"))
				return;
			sw.WriteLine ("\t\tpublic override int GetHashCode ()");
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\treturn {0};", hashcode.ToString ());
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();

		}

		protected new void GenFields (GenerationInfo gen_info)
		{
			int bitfields = 0;
			bool need_field = true;
			StreamWriter sw = gen_info.Writer;

			foreach (StructField field in fields) {
				if (Union)
					sw.WriteLine ("\t\t[FieldOffset(0)]");
				if (field.IsBitfield) {
					if (need_field) {
						sw.WriteLine ("\t\tprivate uint _bitfield{0};\n", bitfields++);
						need_field = false;
					}
				} else
					need_field = true;
				field.Generate (gen_info, "\t\t");
			}
		}

		public override bool Validate ()
		{
			LogWriter log = new LogWriter (QualifiedName);
			foreach (StructField field in fields) {
				if (!field.Validate (log)) {
					if (!field.IsPointer)
						return false;
				}
			}

			return base.Validate ();
		}

		public override bool CanGenerateAbiStruct(LogWriter logWriter) {
			logWriter.Info("Not generating any ABI structs for managed structures");

			return false;
		}

		public override void Generate (GenerationInfo generationInfo)
		{
			bool need_close = false;

			if (generationInfo.Writer == null) {
				generationInfo.Writer = generationInfo.OpenStream (Name, Namespace);
				need_close = true;
			}

			StreamWriter sw = generationInfo.Writer;
			
			sw.WriteLine ("namespace " + Namespace + " {");
			sw.WriteLine ();
			sw.WriteLine ("\tusing System;");
			sw.WriteLine ("\tusing System.Collections;");
			sw.WriteLine ("\tusing System.Collections.Generic;");
			sw.WriteLine ("\tusing System.Runtime.InteropServices;");
			sw.WriteLine ();
			
			sw.WriteLine ("#region Autogenerated code");
			if (IsDeprecated)
				sw.WriteLine ("\t[Obsolete]");
			if (Union)
				sw.WriteLine ("\t[StructLayout(LayoutKind.Explicit)]");
			else
				sw.WriteLine ("\t[StructLayout(LayoutKind.Sequential)]");
			string access = IsInternal ? "internal" : "public";
			sw.WriteLine ("\t" + access + " partial struct {0} : IEquatable<{0}> {{", Name);
			sw.WriteLine ();

			need_read_native = false;
			GenFields (generationInfo);
			sw.WriteLine ();
			GenerateConstructors (generationInfo);
			GenerateMethods (generationInfo, null, this);
			if (need_read_native)
				GenReadNative (sw);
			GenEqualsAndHash (sw);

			if (!need_close)
				return;

			sw.WriteLine ("#endregion");

			sw.WriteLine ("\t}");
			sw.WriteLine ("}");
			sw.Close ();
			generationInfo.Writer = null;
		}
		
		protected override void GenerateConstructors (GenerationInfo generationInfo)
		{
			StreamWriter sw = generationInfo.Writer;

			sw.WriteLine ("\t\tpublic static {0} Zero = new {0} ();", QualifiedName);
			sw.WriteLine();
			if (!DisableNew) {
				sw.WriteLine ("\t\tpublic static " + QualifiedName + " New(IntPtr raw) {");
				sw.WriteLine ("\t\t\tif (raw == IntPtr.Zero)");
				sw.WriteLine ("\t\t\t\treturn {0}.Zero;", QualifiedName);
				sw.WriteLine ("\t\t\treturn ({0}) Marshal.PtrToStructure (raw, typeof ({0}));", QualifiedName);
				sw.WriteLine ("\t\t}");
				sw.WriteLine ();
			}

			foreach (Ctor ctor in Constructors)
				ctor.IsStatic = true;

			base.GenerateConstructors (generationInfo);
		}

		void GenReadNative (StreamWriter sw)
		{
			sw.WriteLine ("\t\tstatic void ReadNative (IntPtr native, ref {0} target)", QualifiedName);
			sw.WriteLine ("\t\t{");
			sw.WriteLine ("\t\t\ttarget = New (native);");
			sw.WriteLine ("\t\t}");
			sw.WriteLine ();
		}

		public override void Prepare (StreamWriter sw, string indent)
		{
			sw.WriteLine (indent + "IntPtr this_as_native = System.Runtime.InteropServices.Marshal.AllocHGlobal (System.Runtime.InteropServices.Marshal.SizeOf (this));");
			sw.WriteLine (indent + "System.Runtime.InteropServices.Marshal.StructureToPtr (this, this_as_native, false);");
		}

		public override void Finish (StreamWriter sw, string indent)
		{
			need_read_native = true;
			sw.WriteLine (indent + "ReadNative (this_as_native, ref this);");
			sw.WriteLine (indent + "System.Runtime.InteropServices.Marshal.FreeHGlobal (this_as_native);");
		}
	}
}

