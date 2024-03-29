﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using MockTracer.UI.Shared.Entity;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.CSharp.Formatting;

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

  internal static LineFragment Line(this string code, string line, Exception? ex = null)
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

  public static List<LineFragment> AddNameSpace(this List<LineFragment> lines, params string[] nameSpaces)
  {
    if (nameSpaces != null)
    {
      foreach (var space in nameSpaces)
      {
        lines.Add(new LineFragment(BuildingConstans.Using, $"using {space};"));
      }
    }

    return lines;
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

  /// <summary>
  /// Resolve generic type
  /// </summary>
  /// <param name="typeName">type in stack trace notation</param>
  /// <returns>generic type</returns>
  public static Type? FindType(this string typeName)
  {
    try
    {
      if (typeName.EndsWith('>'))
      {
        var sb = new StringBuilder();
        var split = new Regex("(?<=<).*(?<!>)").Match(typeName).Value.Split(',', ' ').Where(w => !string.IsNullOrEmpty(w)).ToArray();
        var genericTypeName = typeName.Substring(0, typeName.IndexOf('<')) + "`" + split.Length;


        var genericType = FindType(genericTypeName);

        return genericType?.MakeGenericType(split.Select(s => FindType(s) ?? throw new NullReferenceException($"{s} is not found")).ToArray());
      }

      Type? type = Type.GetType(typeName);

      if (type != null)
      {
        return type;
      }
      else
      {
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies().Where(w => w is not null))
        {
          try
          {
            type = asm.GetType(typeName);
            if (type != null)
            {
              return type;
            }
          }
          catch { }
        }

        return null;
      }
    }
    catch { return null; }
  }

  /// <summary>
  /// Parse stack trace
  /// </summary>
  /// <param name="isSourcable">skip external sources</param>
  /// <returns><see cref="StackCallItem"/></returns>
  public static StackCallItem[] ReadTrace(bool isSourcable = true)
  {
    var query = new StackTrace(true).GetFrames().Select(w => new { fileName = w.GetFileName(), line = w.GetFileLineNumber(), method = w.GetMethod() as MethodInfo });
    if (isSourcable)
    {
      query = query.Where(w => w.fileName != null);
    }

    return query.Where(w => w.method != null && w.method.DeclaringType != null
              && (w.method.DeclaringType.Namespace == null
              || !w.method.DeclaringType.Namespace.StartsWith("MockTracer.UI")))
      .Select(s => new StackCallItem()
      {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        DeclaringTypeNamespace = s.method.DeclaringType.Namespace,
        DeclaringTypeName = s.method.DeclaringType.GetRealTypeName(),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        MethodName = s.method.Name,
        OutputTypeNamespace = s.method.ReturnType.Namespace,
        OutputTypeName = s.method.ReturnType.GetRealTypeName(),

        FileName = s.fileName,
        Line = s.line
      }).ToArray();

  }

  /// <summary>
  /// Resolve programmer's type name
  /// </summary>
  /// <param name="type">source type</param>
  /// <param name="withNamespace">skip namespace</param>
  /// <returns>type name</returns>
  public static string GetRealTypeName(this Type type, bool withNamespace = false)
  {
    if (!type.IsGenericType)
    {
      if (withNamespace)
      {
        return type.Namespace + "." + (type.ReflectedType == null ? type.Name : type.ReflectedType.Name + "." + type.Name);
      }

      return type.ReflectedType == null ? type.Name : type.ReflectedType.Name + "." + type.Name;
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
      return type.FullName + "**FAILED_PARSE** " + ex.Message;
    }
  }

  /// <summary>
  /// Resolve programmer's type name
  /// </summary>
  /// <param name="method">source MethodInfo</param>
  /// <returns>type name</returns>
  public static string GetRealMethodName(this MethodInfo method)
  {
    try
    {
      var generics = method.GetGenericArguments();
      var name = method.Name;

      if (generics == null || !generics.Any())
      {
        return name;
      }

      StringBuilder sb = new StringBuilder();

      sb.Append('<');
      bool appendComma = false;
      foreach (Type arg in generics)
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
      return method.Name + "**FAILED_PARSE** " + ex.Message;
    }
  }

  /// <summary>
  /// Resolve programmer's type name
  /// </summary>
  /// <param name="type">source type</param>
  /// <returns>namespace and class</returns>
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
      return (type.Namespace, type.Name + "**FAILEd_PARSE**" + ex.Message);
    }
  }

  public static string ArrangeUsingRoslyn(this string csCode)
  {
    var cw = new AdhocWorkspace();
    cw.Options.WithChangedOption(CSharpFormattingOptions.NewLineForClausesInQuery, true)
      .WithChangedOption(CSharpFormattingOptions.IndentBlock, true)
      .WithChangedOption(CSharpFormattingOptions.IndentBraces, false)
      .WithChangedOption(CSharpFormattingOptions.IndentSwitchCaseSection, true)
      .WithChangedOption(CSharpFormattingOptions.IndentSwitchCaseSectionWhenBlock, true)
      .WithChangedOption(CSharpFormattingOptions.IndentSwitchSection, true)
      .WithChangedOption(CSharpFormattingOptions.LabelPositioning, LabelPositionOptions.OneLess)

      .WithChangedOption(CSharpFormattingOptions.NewLineForCatch, true)
      .WithChangedOption(CSharpFormattingOptions.NewLineForElse, true)
      .WithChangedOption(CSharpFormattingOptions.NewLineForFinally, true)
      .WithChangedOption(CSharpFormattingOptions.NewLineForMembersInAnonymousTypes, true)
      .WithChangedOption(CSharpFormattingOptions.NewLineForMembersInObjectInit, true)
      .WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInControlBlocks, true)
      .WithChangedOption(CSharpFormattingOptions.NewLineForClausesInQuery, true)

      .WithChangedOption(CSharpFormattingOptions.SpaceAfterCast, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceAfterColonInBaseTypeDeclaration, true)
      .WithChangedOption(CSharpFormattingOptions.SpaceAfterComma, true)
      .WithChangedOption(CSharpFormattingOptions.SpaceAfterDot, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceAfterControlFlowStatementKeyword, true)
      .WithChangedOption(CSharpFormattingOptions.SpaceAfterSemicolonsInForStatement, true)
      .WithChangedOption(CSharpFormattingOptions.SpacingAroundBinaryOperator, BinaryOperatorSpacingOptions.Ignore)
      .WithChangedOption(CSharpFormattingOptions.SpaceAfterColonInBaseTypeDeclaration, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceBeforeColonInBaseTypeDeclaration, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceBetweenEmptySquareBrackets, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceBetweenEmptyMethodCallParentheses, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceBetweenEmptyMethodDeclarationParentheses, false)
      .WithChangedOption(CSharpFormattingOptions.SpaceBetweenEmptySquareBrackets, false)

      .WithChangedOption(CSharpFormattingOptions.WrappingPreserveSingleLine, true)
      .WithChangedOption(CSharpFormattingOptions.WrappingKeepStatementsOnSingleLine, true)

      .WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n")
      .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, false)
      .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4)
      .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4);

    var root = SyntaxFactory.ParseCompilationUnit(csCode);
    var formattedRoot = Formatter.Format(root, cw);
    return formattedRoot.ToFullString();
  }
}
