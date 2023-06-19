using NorskaLib.Extensions;
using NorskaLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NorskaLib.Tools
{
    [System.Flags]
    public enum ModifierKeys
    {
        NONE        = 0,   

        Override    = 1 << 0,
        Virtual     = 1 << 1,
        Static      = 1 << 2,
        Readonly    = 1 << 3,
    }

    public enum AccessKey
    {
        NONE,
        Private,
        Protected,
        Public,
        Internal
    }

    public enum TypeKey
    {
        Class,
        Struct,
        Enum
    }

    public class CodeFileContent : IEnumerable<string>
    {
        public int tabulation = 0;
        private List<string> lines;

        public CodeFileContent(int capacity = 100)
        {
            lines = new List<string>(capacity);
        }

        private void BeginType(
            string name, 
            TypeKey typeKey, 
            AccessKey accessKey = AccessKey.Public,
            ModifierKeys modifierKeys = ModifierKeys.NONE,
            bool partial = false)
        {
            var type = ToString(typeKey);
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys, partial);
            builder.Append($" {type} {name}");
            BeginBody(builder.ToString());
        }
        private void BeginType(
            string name, 
            string baseName, 
            TypeKey typeKey, 
            AccessKey accessKey = AccessKey.Public,
            ModifierKeys modifierKeys = ModifierKeys.NONE,
            bool partial = false)
        {
            var type = ToString(typeKey);
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys, partial);
            builder.Append($" {type} {name} : {baseName}");
            BeginBody(builder.ToString());
        }

        public void BeginBody(string expression)
        {
            AddLine(expression);
            AddLine("{");
            tabulation++;
        }
        public void EndBody()
        {
            tabulation--;
            AddLine("}");
        }
        private void EndCollection()
        {
            tabulation--;
            AddLine("};");
        }

        private void AppendKeys(StringBuilder builder, AccessKey accessKey, ModifierKeys modifierKeys, bool partial = false)
        {
            builder.Append(ToString(accessKey));

            if (partial)
            {
                builder.Append(" partial");
            }

            if (EnumUtils.HasFlag((int)modifierKeys, (int)ModifierKeys.Static))
            {
                builder.Append(' ');
                builder.Append(ToString(ModifierKeys.Static));
            }
            if (EnumUtils.HasFlag((int)modifierKeys, (int)ModifierKeys.Readonly))
            {
                builder.Append(' ');
                builder.Append(ToString(ModifierKeys.Readonly));
            }
            if (EnumUtils.HasFlag((int)modifierKeys, (int)ModifierKeys.Override))
            {
                builder.Append(' ');
                builder.Append(ToString(ModifierKeys.Override));
            }
            if (EnumUtils.HasFlag((int)modifierKeys, (int)ModifierKeys.Virtual))
            {
                builder.Append(' ');
                builder.Append(ToString(ModifierKeys.Virtual));
            }
        }

        private string ToString(TypeKey key)
        {
            switch (key)
            {
                case TypeKey.Class:
                    return "class";
                case TypeKey.Struct:
                    return "struct";
                case TypeKey.Enum:
                    return "enum";
                default:
                    return string.Empty;
            }
        }
        private string ToString(AccessKey key)
        {
            switch (key)
            {
                case AccessKey.Private:
                    return "private";
                case AccessKey.Protected:
                    return "protected";
                case AccessKey.Public:
                    return "public";
                case AccessKey.Internal:
                    return "internal";
                default:
                    return string.Empty;
            }
        }
        private string ToString(ModifierKeys key)
        {
            switch (key)
            {
                case ModifierKeys.Override:
                    return "override";
                case ModifierKeys.Virtual:
                    return "virtual";
                case ModifierKeys.Static:
                    return "static";
                case ModifierKeys.Readonly:
                    return "readonly";
                default:
                    return string.Empty;
            }
        }

        public void AddLine(string line, bool ignoreTabulation = false)
        {
            if (ignoreTabulation)
                lines.Add(line);
            else
            {
                var stringBuilder = new StringBuilder(tabulation + 1);
                for (int i = 0; i < tabulation; i++)
                    stringBuilder.Append('\t');
                stringBuilder.Append(line);
                lines.Add(stringBuilder.ToString());
            }
        }

        public void AddUsing(string name)
        {
            AddLine("using " + name + ';', true);
        }

        public void AddSpace()
        {
            AddLine(string.Empty);
        }

        public void BeginNamespace(string name)
            => BeginBody("namespace " + name);
        public void EndNamespace()
            => EndBody();

        public void BeginRegion(string name)
        {
            AddLine("#region " + name);
        }
        public void EndRegion()
        {
            AddLine("#endregion");
        }

        public void BeginEnum(string name, AccessKey accessKey = AccessKey.Public)
            => BeginType(name, TypeKey.Enum, accessKey);
        public void PopulateEnum(string elementName)
        {
            AddLine(elementName + ",");
        }
        public void PopulateEnum(string elementName, int elementValue)
        {
            AddLine(elementName + " = " + elementValue + ",");
        }
        public void PopulateEnumFlag(string elementName, int elementIndex)
        {
            AddLine(elementName + " = " + "1 << " + elementIndex + ",");
        }
        public void EndEnum()
            => EndBody();

        public void BeginStruct(string name, AccessKey accessKey = AccessKey.Public, bool partial = false)
            => BeginType(name, TypeKey.Struct, accessKey, partial: partial);
        public void BeginStruct(string name, string baseName, AccessKey accessKey = AccessKey.Public, bool partial = false)
            => BeginType(name, baseName, TypeKey.Struct, accessKey, partial: partial);
        public void EndStruct()
            => EndBody();

        public void BeginClass(string name, AccessKey accessKey = AccessKey.Public, bool partial = false)
            => BeginType(name, TypeKey.Class, accessKey, partial: partial);
        public void BeginClass(string name, string baseName, AccessKey accessKey = AccessKey.Public, bool partial = false)
            => BeginType(name, baseName, TypeKey.Class, accessKey, partial: partial);
        public void EndClass()
            => EndBody();

        public void BeginArray(string arrayName, string typeName, AccessKey accessKey = AccessKey.Private, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {typeName}[] {arrayName} = new {typeName}[]");
            BeginBody(builder.ToString());
        }
        public void EndArray()
            => EndCollection();

        public void BeginDictionary(string keyTypeName, string valueTypeName, AccessKey accessKey = AccessKey.Private, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" Dictionary<{keyTypeName},{valueTypeName}> = new Dictionary<{keyTypeName},{valueTypeName}>");
            BeginBody(builder.ToString());
        }
        public void PopulateDictionary(string key, string value)
        {
            AddLine($"{{{key}, {value}}}");
        }
        public void EndDictionary()
            => EndCollection();

        public void AddReturn()
        {
            AddLine($"return;");
        }
        public void AddReturn(string expression)
        {
            AddLine($"return {expression};");
        }

        public void AddException<E>() where E : Exception
        {
            AddLine($"throw new {typeof(E).Name}();");
        }
        public void AddException<E>(string message) where E : Exception
        {
            AddLine($"throw new {typeof(E).Name}({message});");
        }

        public void AddField(string name, string type, AccessKey accessKey = AccessKey.Public, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {type} {name};");
            AddLine(builder.ToString());
        }

        public void AddGetter(string name, string type, string expression, AccessKey accessKey = AccessKey.Public, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {type} {name} => {expression}");
        }

        public void BeginProperty(string name, string type, AccessKey accessKey = AccessKey.Public, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {type} {name}");
            BeginBody(builder.ToString());
        }
        public void BeginGet()
            => BeginBody("get");
        public void EndGet()
            => EndBody();
        public void BeginSet()
            => BeginBody("set");

        public void EndSet()
            => EndBody();
        public void EndProperty()
            => EndBody();

        public void BeginMethod(string name, string returnType, AccessKey accessKey = AccessKey.Public, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {returnType} {name}()");
            BeginBody(builder.ToString());
        }
        public void BeginMethod(string name, string returnType, string paramType, string paramName, AccessKey accessKey = AccessKey.Public, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {returnType} {name}({paramType} {paramName})");
            BeginBody(builder.ToString());
        }
        public void BeginMethod(string name, string returnType, IEnumerable<KeyValuePair<string, string>> parameters, AccessKey accessKey = AccessKey.Public, ModifierKeys modifierKeys = ModifierKeys.NONE)
        {
            var builder = new StringBuilder();
            AppendKeys(builder, accessKey, modifierKeys);
            builder.Append($" {returnType} {name}(");
            var firstParam = true;
            foreach (var pair in parameters)
            {
                if (!firstParam)
                    builder.Append(", ");

                builder.Append(pair.Key);
                builder.Append(' ');
                builder.Append(pair.Value);
                firstParam = false;
            }
            builder.Append($")");
            BeginBody(builder.ToString());
        }
        public void EndMethod()
            => EndBody();

        public void BeginSwitch(string variableName)
        => BeginBody($"switch ({variableName})");
        public void BeginCase(string value)
        {
            AddLine($"case {value}:");
            tabulation++;
        }
        public void BeginCase(string enumTypeName, string enumValueName)
        {
            AddLine($"case {enumTypeName}.{enumValueName}:");
            tabulation++;
        }
        public void BeginCaseDefault()
        {
            AddLine($"default:");
            tabulation++;
        }
        public void EndCaseReturn(string expression)
        {
            AddReturn(expression);
            tabulation--;
        }
        public void EndCaseException<E>() where E : Exception
        {
            AddException<E>();
            tabulation--;
        }
        public void EndCaseException<E>(string message) where E : Exception
        {
            AddException<E>(message);
            tabulation--;
        }
        public void EndCase()
        {
            AddLine("break;");
            tabulation--;
        }
        public void EndSwitch()
            => EndBody();

        public void BeginIf(string expression)
            => BeginBody($"if ({expression})");
        public void BeginIfSingleLine(string expression)
        {
            AddLine($"if ({expression})");
            tabulation++;
        }
        public void EndIf()
            =>EndBody();
        public void EndIfSingleLine()
        {
            tabulation--;
        }

        public void AddHint(string summary)
        {
            AddLine("/// <summary>");
            AddLine($"/// {summary}");
            AddLine("/// </summary>");
        }
        public void AddComment(string comment)
        {
            AddLine($"// {comment}");
        }

        public void AddAttribute<A>() where A : Attribute
        {
            AddLine($"[{typeof(A).Name.Replace("Attribute", "")}]");
        }

        #region IEnumerable<string>

        public string this[int idx] => lines[idx];
        public int Count => lines.Count;

        public IEnumerator<string> GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        #endregion
    } 
}