using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace analyzerCodeFix;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FunctionNameCodeFix)), Shared]
public class FunctionNameCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("SLH001");

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        context.RegisterCodeFix(CodeAction.Create(title: "test", c => CreateNewDocument(), equivalenceKey: "test2"),
            diagnostic);
    }

    private static async Task<Document> CreateNewDocument()
    {
        return null;
    }
}