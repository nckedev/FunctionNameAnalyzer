using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FunctionNameCodeFix)), Shared]
public class FunctionNameCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("SLH001");

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var node = root?.FindNode(context.Span); // the span reported by analzyer

        if (node is not MethodDeclarationSyntax methodDeclaration) return;

        SyntaxToken identifier = methodDeclaration.Identifier;

        Document document = context.Document;
        Solution solution = document.Project.Solution;
        var documentSemanticModel = await document.GetSemanticModelAsync(context.CancellationToken);
        var classModel = documentSemanticModel?.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);
        
        string suggestedName = NewFucntionName(identifier.ValueText);

        var diagnostic = context.Diagnostics.First();
        context.RegisterCodeFix(CodeAction.Create(title: "Change order",
                async ct => await Renamer.RenameSymbolAsync(solution, classModel, new SymbolRenameOptions(),
                    suggestedName, ct), equivalenceKey: null),
            diagnostic);
    }
    
    private string NewFucntionName(string oldName)
    {
        if (oldName.Contains("get", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Get" + oldName.Replace("get", "", StringComparison.CurrentCultureIgnoreCase);
        }

        return "123";
    }
}