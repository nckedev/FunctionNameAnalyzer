using analyzer;
using analyzerCodeFix;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace AnalyzerTest;


public class UnitTest1 : CSharpAnalyzerTest<FunctionNameAnalyzer, XUnitVerifier>
{
    [Fact]
    public async Task FunctionNameShouldGiveWarning()
    {
        TestCode = @"public class Test { public void FunctionGetList() { } }";
        
        ExpectedDiagnostics.Add(new DiagnosticResult("SLH001", DiagnosticSeverity.Warning).WithSpan(1, 33, 1, 48).WithArguments("FunctionGetList"));
        await RunAsync();
    }
    [Fact]
    public async Task FunctionNameShouldNotGiveWarning()
    {
        TestCode = @"public class Test { public void GetFunctionList() { } }";
        
        ExpectedDiagnostics.Clear();
        await RunAsync();
    }
}

public class UnitTest2 : CSharpCodeFixTest<FunctionNameAnalyzer, FunctionNameCodeFix, XUnitVerifier>
{
    [Fact]
    public async Task WhenInconsistentName_AddsExceptionPostfix()
    {
        TestCode = @"public class Test { public void FunctionGetList() { } }";

        ExpectedDiagnostics.Add(
            new DiagnosticResult("SLH001", DiagnosticSeverity.Warning)
                //.WithMessage("CustomError class name should end with Exception")
                .WithSpan(1, 33,1,48 ));

        FixedCode = "public class Test { public void GetFunctionList() { } }";

        await RunAsync();
    }
}