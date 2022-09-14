using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FunctionNameAnalyzer : DiagnosticAnalyzer
{
    public const string Title = "Wierd func name";
    public const string MessageFormat = "Funktionsnamn: Get i {0} borde vara först";
    public const string Description = "FunctionNameAnalyzer.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor("SLH001", Title, MessageFormat,
        "stateless",
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeFunctionName, SyntaxKind.MethodDeclaration);
    }

    public void TestGetList()
    {
    }

    public void TestSetStuff()
    {
    }

    public void TestSet()
    {
    }

    public void GetTestList()
    {
    }

    public void UpdateSettings()
    {
        //denn ska inte visa fel 
    }

    private static void AnalyzeFunctionName(SyntaxNodeAnalysisContext context)
    {
        MethodDeclarationSyntax mds = (MethodDeclarationSyntax) context.Node;

        var identifier = mds.Identifier.ValueText;
        if (HasBadOrdering(identifier))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, mds.Identifier.GetLocation(), mds.Identifier.ValueText));
        }
    }

    private static bool HasBadOrdering(string identifier)
    {
        var bannedWords = new List<string>() {"get", "set"};
        foreach (var word in bannedWords)
        {
            if (identifier.Contains(word, StringComparison.CurrentCultureIgnoreCase) &&
                !identifier.StartsWith(word, StringComparison.CurrentCultureIgnoreCase))
            {
                var nextCharIndex =
                    identifier.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) + word.Length;
                if (nextCharIndex >= identifier.Length - 1)
                    return true;
                if (nextCharIndex < identifier.Length)
                {
                    var nextChar = identifier[nextCharIndex];
                    if (char.IsUpper(nextChar) || nextCharIndex == identifier.Length - 1)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}