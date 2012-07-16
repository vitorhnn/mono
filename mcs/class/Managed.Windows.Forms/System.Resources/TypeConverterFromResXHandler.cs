// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2012 Gary Barnett
//
// Authors:
//	Gary Barnett

using System;
using System.Reflection;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Resources {
	internal class TypeConverterFromResXHandler : ResXDataNodeHandler, IWritableHandler {

		string dataString;
		string mime_type;
		string typeString;

		public TypeConverterFromResXHandler (string data, string _mime_type, string _typeString)
		{
			dataString = data;
			mime_type = _mime_type;
			typeString = _typeString;
		}

		#region implemented abstract members of System.Resources.ResXDataNodeHandler
		public override object GetValue (ITypeResolutionService typeResolver)
		{
			if (typeString == String.Empty || typeString == null)
				return null;

			Type type = ResolveType (typeString, typeResolver);
			if (type == null)
				throw new TypeLoadException();

			TypeConverter c = TypeDescriptor.GetConverter (type);
			if (c == null)
				throw new TypeLoadException();

			return ConvertData (c);
		}

		public override object GetValue (AssemblyName[] assemblyNames)
		{
			if (typeString == String.Empty || typeString == null)
				return null;

			Type type = ResolveType (typeString, assemblyNames);
			if (type == null)
				throw new TypeLoadException();

			TypeConverter c = TypeDescriptor.GetConverter (type);
			if (c == null)
				throw new TypeLoadException();

			return ConvertData (c);
		}

		public override string GetValueTypeName (ITypeResolutionService typeResolver)
		{
			//FIXME: need test to see what is returned if typeString empty
			if (typeString == String.Empty || typeString == null)
				return String.Empty;

			Type type = ResolveType (typeString, typeResolver);

			if (type == null)
				return typeString;
			else
				return type.AssemblyQualifiedName;
		}

		public override string GetValueTypeName (AssemblyName[] assemblyNames)
		{
			//FIXME: need test to see what is returned if typeString empty
			if (typeString == String.Empty || typeString == null)
				return String.Empty;

			Type type = ResolveType (typeString, assemblyNames);

			if (type == null)
				return typeString;
			else
				return type.AssemblyQualifiedName;
		}
		#endregion

		#region IWritableHandler implementation
		public string DataString {
			get {
				return dataString;
			}
		}
		#endregion

		object ConvertData (TypeConverter c)
		{
			//FIXME: throw errors when converter not found?
			if (mime_type == ResXResourceWriter.ByteArraySerializedObjectMimeType) {
				if (c.CanConvertFrom (typeof (byte [])))
					return c.ConvertFrom (Convert.FromBase64String (dataString));
			} else if (String.IsNullOrEmpty (mime_type)) {
				if (c.CanConvertFrom (typeof (string)))
					return c.ConvertFromInvariantString (dataString);
			}
			// shouldnt get here
			throw new Exception ("shouldnt see me -> mimetype invalid, this handler shouldnt have been used");
		}

	}
}

