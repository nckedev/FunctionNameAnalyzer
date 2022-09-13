using System.Collections.Immutable;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FunctionNameAnalyzer : DiagnosticAnalyzer
{
    public const string Title = "Wierd func name";
    public const string MessageFormat = "Get i {0} borde vara först";
    public const string Description = "FunctionNameAnalyzer.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor("SLH001", Title, MessageFormat, "stateless",
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeFunctionName, SyntaxKind.MethodDeclaration);
    }

    public void TestGetList()
    {
    }

    public void GetTest()
    {
    }

    private static void AnalyzeFunctionName(SyntaxNodeAnalysisContext context)
    {
        MethodDeclarationSyntax mds = (MethodDeclarationSyntax) context.Node;
        var identifier = mds.Identifier.ValueText;
        if (identifier.Contains("get", StringComparison.CurrentCultureIgnoreCase) &&
            !identifier.StartsWith("get", StringComparison.CurrentCultureIgnoreCase))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, mds.Identifier.GetLocation(), mds.Identifier.ValueText));
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
}