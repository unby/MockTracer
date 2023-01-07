using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using MockTracer.UI.Shared.Entity;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MockTracer.UI.Server.Application.Generation;

internal static class FragmentExtention
{
  private static readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;

  internal static string ToTitle(this string source)
  {
    return _textInfo.ToTitleCase(source.ToLower());
  }

  public static string? FirstCharToLowerCase(this string? source)
  {
    if (!string.IsNullOrEmpty(source) && char.IsUpper(source[0]))
      return source.Length == 1 ? char.ToLower(source[0]).ToString() : char.ToLower(source[0]) + source[1..];

    return source;
  }

  internal static LineFragment Line(this string code, string line, Exception ex = null)
  {
    if (ex == null)
    {
      return new LineFragment(code, line);
    }
    else
    {
      return new LineFragment(code, $"{line} : {ex.Message} + {GetLineNumber(ex)}");
    }
  }

  public static string ResolveClassName(this List<LineFragment> lines, TracedObject obj)
  {
    lines.Add(new LineFragment(BuildingConstans.Using, $"using {obj.Namespace};"));

    return obj.ClassName;
  }

  private static string GetLineNumber(Exception ex)
  {
    const string lineSearch = ":line ";
    var index = ex.StackTrace?.LastIndexOf(lineSearch);
    if (index != -1 && index.HasValue && !string.IsNullOrEmpty(ex.StackTrace))
    {
      var lineNumberText = ex.StackTrace.Substring(index.Value + lineSearch.Length);
      return lineNumberText;
    }

    return string.Empty;
  }

  public static Type FindType(this string typeName)
  {
    if (typeName.EndsWith('>'))
    {
      var sb = new StringBuilder();
      var split = new Regex("(?<=<).*(?<!>)").Match(typeName).Value.Split(',', ' ').Where(w => !string.IsNullOrEmpty(w)).ToArray();
      var genericTypeName = typeName.Substring(0, typeName.IndexOf('<')) + "`" + split.Length;

      
      var genericType = FindType(genericTypeName);

      return genericType.MakeGenericType(split.Select(s => FindType(s)).ToArray());
    }

    Type t = Type.GetType(typeName);

    if (t != null)
    {
      return t;
    }
    else
    {
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies().Where(w => w is not null).Where(w => w.FullName.Contains("MockTracer.Test.Api")))
      {
        t = asm.GetType(typeName);
        if (t != null)
        {
          return t;
        }
      }
      return null;
    }
  }

  public static StackCallItem[] ReadTrace(bool isSourcable = true)
  {

    var query = new StackTrace(true).GetFrames().Select(w => new { fileName = w.GetFileName(), line = w.GetFileLineNumber(), method = w.GetMethod() as MethodInfo });
    if (isSourcable)
    {
      query = query.Where(w => w.fileName != null);
    }
    return query.Where(w => w.method != null && w.method.DeclaringType != null).Where(w => w.method.DeclaringType.Namespace == null || !w.method.DeclaringType.Namespace.StartsWith("MockTracer.UI")).Select(s => new StackCallItem()
    {
      DeclaringTypeNamespace = s.method.DeclaringType.Namespace,
      DeclaringTypeName = s.method.DeclaringType.GetRealTypeName(),
      MethodName = s.method.Name,
      OutputTypeNamespace = s.method.ReturnType.Namespace,
      OutputTypeName = s.method.ReturnType.GetRealTypeName(),

      FileName = s.fileName,
      Line = s.line
    }).ToArray();
  }

  public static string GetRealTypeName(this Type type, bool withNamespace = false)
  {
    if (!type.IsGenericType)
    {
      if (withNamespace)
      {
        return type.Namespace + "." + type.Name;
      }

      return type.Name;
    }


    try
    {
      StringBuilder sb = new StringBuilder();
      if (withNamespace && !string.IsNullOrEmpty(type.Namespace))
      {
        sb.Append(type.Namespace);
        sb.Append('.');
      }

      sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
      sb.Append('<');
      bool appendComma = false;
      foreach (Type arg in type.GetGenericArguments())
      {
        if (appendComma) sb.Append(',');
        sb.Append(GetRealTypeName(arg, true));
        appendComma = true;
      }
      sb.Append('>');
      return sb.ToString();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      var x = ex.ToString();
      return type.FullName + "**FAILEd_PARSE**";
    }
  }

  public static (string? nameSpace, string name) GetRealFullTypeName(this Type type)
  {
    if (!type.IsGenericType)
      return (type.Namespace, type.Name);

    try
    {
      StringBuilder sb = new StringBuilder();

      sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
      sb.Append('<');
      bool appendComma = false;
      foreach (Type arg in type.GetGenericArguments())
      {
        if (appendComma) sb.Append(',');
        sb.Append(GetRealTypeName(arg));
        appendComma = true;
      }
      sb.Append('>');
      return (type.Namespace, sb.ToString());
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      var x = ex.ToString();
      return (type.Namespace, type.Name + "**FAILEd_PARSE**");
    }
  }

  public static string ArrangeUsingRoslyn(this string csCode)
  {
    try
    {
      SyntaxTree tree = CSharpSyntaxTree.ParseText(csCode);
      CompilationUnitSyntax root = tree.GetCompilationUnitRoot();


      var container = tree.GetText().Container;
      var workspace = new AdhocWorkspace();
      Console.WriteLine(workspace);

      var temp = Formatter.Format(root, workspace);
      return temp.ToFullString();
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex);
      throw;
    }
  }
}
